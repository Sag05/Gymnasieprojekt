using Assets.Scripts;
using Assets.Scripts.Vehicles;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Components;
using UnityEditor;

public class Aircraft : VehicleBase
{
    #region Variables
    private AircraftConfiguration AircraftConfiguration;
    private DoubleVector3 steering = new DoubleVector3();
    public float Throttle { get; set; }
    Animation animation;
    PhysicMaterial wheelMaterial;
    #endregion
    
    public void SwitchSOI()
    {
        //base.EntityComponents.LoopSOI();
    }

    private void SetController()
    {
        Debug.Log("Setting camera position to " + this.AircraftConfiguration.CameraPosition + " for " + base.ControllerCameraPosition);
        base.ControllerCameraPosition.transform.localPosition = this.AircraftConfiguration.CameraPosition;
        Controller.transform.SetParent(base.ControllerCameraPosition.transform);
        Controller.transform.localPosition = Vector3.zero;
    }

    private void LoadModel(){
        base.Model = GameObject.Find(AircraftConfiguration.ModelName);
        base.Model.transform.localScale = Utilities.FloatToVector3(GameManager.scaleFactor);

        //Find Body, LeftWing and RightWing and add colliders
        this.AircraftConfiguration.Body = this.Model;
        this.AircraftConfiguration.Body.AddComponent<MeshCollider>().convex = true;
        
        #region Wings
        /*          FOR SOME REASON BROKEN
        Debug.Log("LeftWing: " + AircraftConfiguration.LeftWingName + " RightWing: " + AircraftConfiguration.RightWingName);
        this.AircraftConfiguration.LeftWing = base.Model.transform.Find(AircraftConfiguration.LeftWingName).gameObject;
        this.AircraftConfiguration.RightWing = base.Model.transform.Find(AircraftConfiguration.RightWingName).gameObject;
        this.AircraftConfiguration.LeftWing.AddComponent<MeshCollider>().convex = true;
        this.AircraftConfiguration.RightWing.AddComponent<MeshCollider>().convex = true;
        */
        #endregion

        #region Wheels
        this.AircraftConfiguration.FrontWheel = GameObject.Find(AircraftConfiguration.FrontWheelName);
        this.AircraftConfiguration.FrontWheel.AddComponent<CapsuleCollider>().material = wheelMaterial;

        this.AircraftConfiguration.LeftWheel = GameObject.Find(AircraftConfiguration.LeftWheelName);
        this.AircraftConfiguration.LeftWheel.AddComponent<CapsuleCollider>().material = wheelMaterial;
        
        this.AircraftConfiguration.RightWheel = GameObject.Find(AircraftConfiguration.RightWheelName);
        this.AircraftConfiguration.RightWheel.AddComponent<CapsuleCollider>().material = wheelMaterial;
        #endregion

        animation = base.Model.GetComponent<Animation>();
    }

    new void Start()
    {
        this.wheelMaterial = AssetDatabase.LoadAllAssetsAtPath("Assets/Materials//Wheel.physicMaterial").OfType<PhysicMaterial>().FirstOrDefault();
        this.AircraftConfiguration = ConfigurationReader.LoadAircraft(@".\configs\aircrafts\" + "F6"  /*gameObject.name*/ + ".cfg", this);
        base.EntityComponents.AddComponents(this.AircraftConfiguration.EntityComponents.ToArray());

        base.Start();
        SetController();
        LoadModel();

        foreach (ComponentBase component in base.EntityComponents.Components)
        {
            if (component is TurbineEngine engine)
            {   //Enable engines
                engine.EngineEnabled = true;
            }
            else if (component is StoresManagementSystem SMS)
            {   //Reload Stores Management System
                SMS.ReloadSMS();
            }
        }

        this.EntityBody.mass = this.AircraftConfiguration.Mass;
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
        foreach (ComponentBase component in this.EntityComponents.Components)
        {
            // Ticks any tickable components in the pre-phase
            if (component is ITickableComponent tickableComponent)
            {
                tickableComponent.PreTickComponent();
            }
            // Componenents should have their values get/set in this area
            if (component is TurbineEngine engine)
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
        this.EntityBody.AddRelativeForce(lift + drag);
        this.EntityBody.AddRelativeForce(Vector3.forward * totalThrust);

        //Apply steering
        base.EntityBody.AddRelativeTorque(steering.vector1, ForceMode.VelocityChange);

        #region Debug
        #region DrawVectors
        //Lift
        Debug.DrawRay(base.transform.position, transform.rotation * lift / 1000, Color.green);
        //Drag
        Debug.DrawRay(base.transform.position, transform.rotation * drag / 1000, Color.red);
        //Thrust
        Debug.DrawRay(base.transform.position, transform.forward * totalThrust / 1000, Color.blue);
        //Gravity
        Debug.DrawRay(base.transform.position, base.EntityBody.mass * 9.81f / 1000 * Vector3.down, Color.black);
        //Velocity
        Debug.DrawRay(base.transform.position, base.EntityBody.velocity / 1000, Color.white);
        #endregion
        #region Logs
        //Debug.Log("Applied steering: " + steering.vector1);
        //Controller.DebugText.text = "Thrust: " + totalThrust;

        //Debug.Log("Lift: " + lift);

        /*
        Controller.DebugText.text += 
            "Drag: " + (transform.rotation * drag).ToString("0.0") + "N + T: " + drag.magnitude.ToString("0.0") +
            "N\nLift: " + (transform.rotation * lift).ToString("0.0") + "N + T: " + lift.magnitude.ToString("0.0") +
            "N\nGravity: " + (base.EntityBody.mass * 9.81f).ToString("0.0") + 
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
            "N\nGravity: " + base.EntityBody.mass * 9.81f);
            
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

    public void AnimationHandler(string animationName)
    {
        Animator animator = base.Model.GetComponent<Animator>();
        //animator.Play(animationName);
        /*
        if(animation.isPlaying)
        {
            Debug.Log("Animation already playing");
        }
        else
        {
            animation.Play(animationName);
            Debug.Log("Playing animation " + animationName);
        }
        */
    }

}
