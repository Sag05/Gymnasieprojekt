using Assets.Scripts;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEngine;
using System.Linq;

public class Aircraft : VehicleBase
{
    #region Variables
    private AircraftConfiguration AircraftConfiguration;
    private DoubleVector3 steering = new DoubleVector3();
    public float Throttle { get; set; }
    Animation gearAnimation;
    #endregion
    
    public void SwitchSOI()
    {
        base.VehicleComponents.LoopSOI();
    }

    private void SetController()
    {
        Debug.Log("Setting camera position to " + this.AircraftConfiguration.CameraPosition + " for " + base.ControllerCameraPosition);
        base.ControllerCameraPosition.transform.localPosition = this.AircraftConfiguration.CameraPosition;
        Controller.transform.SetParent(base.ControllerCameraPosition.transform);
        Controller.transform.localPosition = Vector3.zero;
    }

    private void LoadModel(){
        //model = Instantiate(      (@".\configs\aircrafts\" + AircraftConfiguration.ModelName), transform);
        //model = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Aircraft/" + AircraftConfiguration.ModelName), transform);
        base.Model = GameObject.Find(AircraftConfiguration.ModelName);
        base.Model.transform.localScale = Utilities.FloatToVector3(GameManager.scaleFactor);
        
        gearAnimation = base.Model.AddComponent<Animation>();
    }

    new void Start()
    {
        this.AircraftConfiguration = ConfigurationReader.LoadAircraft(@".\configs\aircrafts\" + gameObject.name + ".cfg", this);
        base.VehicleComponents.AddComponents(this.AircraftConfiguration.VehicleComponents.ToArray());

        base.Start();

        SetController();

        LoadModel();

        foreach (ComponentBase component in base.VehicleComponents.Components)
        {
            if (component is AircraftEngine engine)
            {   //Enable engines
                engine.EngineEnabled = true;
            }
            else if (component is StoresManagementSystem SMS)
            {   //Reload Stores Management System
                SMS.ReloadSMS();
            }
            else if (component is SuspensionManager suspensionManager)
            {   //Start Suspension
                suspensionManager.Start();
            }
        }

        this.VehicleBody.mass = this.AircraftConfiguration.Mass;
    }

    void Update()
    {
        #region Debug code
        // == DEBUGGING CONTEXT == //
        // All debugging content should be between this area
        //Controller.DebugText.text = "";
        // == END OF DEUBBING CONTEXT == //
        #endregion
    }

    void FixedUpdate()
    {
        //Run pre-update on base
        base.PreUpdate();

        //Set throttle
        this.Throttle = Mathf.Clamp(this.Throttle + base.Controller.ThrottleInput, 0f, 100f);
        
        //Reset thrust and additional drag
        float totalThrust = 0;
        float additionalDrag = 0;

        //Go through all components
        foreach (ComponentBase component in this.VehicleComponents.Components)
        {
            // Ticks any tickable components in the pre-phase
            if (component is ITickableComponent tickableComponent)
            {
                tickableComponent.PreTickComponent();
            }
            // Componenents should have their values get/set in this area
            if (component is AircraftEngine engine)
            {
                engine.Altitude = base.Altitude;
                engine.TargetRPMFactor = Throttle / 100f;
                //Apply thrust
                totalThrust += engine.Thrust;
            }
            else if (component is HelmetMountedDisplay hmd)
            {
                hmd.GForce = PhysicsUtils.CalculateLocalGForce(base.LocalAngularVelocity, base.LocalVelocity).y;
                hmd.Velocity = base.Velocity;
                hmd.Altitude = base.Altitude;
                hmd.RadarAltitude = base.RadarAltitude;
            }
            // Ticks any tickable components in the post-phase
            if (component is ITickableComponent tickableComponent1)
            {
                tickableComponent1.PostTickComponent();
            }
        }

        #region ForceCalculation
        //Drag
        Vector3 drag = PhysicsUtils.CalculateDrag(base.LocalVelocity, base.Altitude, AircraftConfiguration.AltitudeEffectivenessCurve,
            this.AircraftConfiguration.SideDragCurve, this.AircraftConfiguration.TopDragCurve, this.AircraftConfiguration.FrontDragCurve, additionalDrag);

        //Lift
        Vector3 lift = PhysicsUtils.CalculatelTotalAircraftLift(this.Controller.DebugText, base.LocalVelocity, base.Altitude, this.AircraftConfiguration);

        
        //Calculate Steering
        steering = PhysicsUtils.AircraftSteering(
            base.LocalVelocity, base.LocalAngularVelocity, base.Controller.ControlInput,
            this.AircraftConfiguration, this.Controller.DebugText);
        
        
        //Apply forces
        this.VehicleBody.AddRelativeForce(lift + drag);
        this.VehicleBody.AddRelativeForce(Vector3.forward * totalThrust);

        //Apply steering
        base.VehicleBody.AddRelativeTorque(steering.vector1, ForceMode.VelocityChange);

        #region Debug
        #region DrawVectors
        //Lift
        Debug.DrawRay(base.transform.position, transform.rotation * lift / 1000, Color.green);
        //Drag
        Debug.DrawRay(base.transform.position, transform.rotation * drag / 1000, Color.red);
        //Thrust
        Debug.DrawRay(base.transform.position, transform.forward * totalThrust / 1000, Color.blue);
        //Gravity
        Debug.DrawRay(base.transform.position, base.VehicleBody.mass * 9.81f / 1000 * Vector3.down, Color.black);
        //Velocity
        Debug.DrawRay(base.transform.position, base.VehicleBody.velocity / 1000, Color.white);
        #endregion
        #region Logs
        //Debug.Log("Applied steering: " + steering.vector1);
        //Controller.DebugText.text = "Thrust: " + totalThrust;

        //Debug.Log("Lift: " + lift);

        /*
        Controller.DebugText.text += 
            "Drag: " + (transform.rotation * drag).ToString("0.0") + "N + T: " + drag.magnitude.ToString("0.0") +
            "N\nLift: " + (transform.rotation * lift).ToString("0.0") + "N + T: " + lift.magnitude.ToString("0.0") +
            "N\nGravity: " + (base.VehicleBody.mass * 9.81f).ToString("0.0") + 
            "N\nThrust: " + totalThrust.ToString("0.0") + 
            "N\nThrottle: " + this.Throttle.ToString("0.0") + "%" +
            "\nInput: " + controlInput.ToString("0.0") +
            "\nSteering: " + steering.vector1.ToString("0.0") + 
            "\nAOA: " + PhysicsUtils.CalculateAngleOfAttack(base.LocalVelocity).ToString("0.0") + 
            "\nRudderAOA: " + PhysicsUtils.CalculateAngleOfAttackYaw(base.LocalVelocity).ToString("0.0");
        */

        /*Log forces
        Debug.Log("Drag: " + drag + 
            "N\nLift: " + lift + 
            "N\nThrust: " + totalThrust + 
            "N\nGravity: " + base.VehicleBody.mass * 9.81f);
            
        */

        /*
        if(drag.x >= lastDrag.x + 100 || drag.y >= lastDrag.y + 100 || drag.z >= lastDrag.z + 100 || drag.x <= lastDrag.x - 100 || drag.y <= lastDrag.y - 100 || drag.z <= lastDrag.z - 100 && drag != Vector3.zero)
        {
            Debug.LogWarning("Drag: " + transform.rotation * drag + "N");
        }
        if (lift.x >= lastLift.x + 100 || lift.y >= lastLift.y + 100 || lift.z >= lastLift.z + 100 || lift.x <= lastLift.x - 100 || lift.y <= lastLift.y - 100 || lift.z <= lastLift.z - 100 && lift != Vector3.zero)
        {
            Debug.LogWarning("Lift: " + transform.rotation *  lift + "N");
        }
        lastDrag = drag;
        lastLift = lift;
        */
        #endregion
        #endregion

        #endregion

        //Run post-update on base
        base.PostUpdate();
    }

    public void AnimationHandler()
    {

    }

}
