using Assets.Scripts;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Aircraft : VehicleBase
{
    /*
    private float rollInput;
    private float yawInput;
    private float pitchInput;
    */
    private float throttleInput;
    
    private DoubleVector3 steering = new DoubleVector3();

    private AircraftConfiguration AircraftConfiguration;
    public Vector3 controlInput { get => new Vector3( Input.GetAxis("Pitch"), Input.GetAxis("Yaw"), Input.GetAxis("Roll")); }

    string vehicleName = "F5";
    public TextMeshProUGUI DebugText;

    //Temporary debug variables
    #region TEMP
    Vector3 lastDrag = Vector3.zero;
    Vector3 lastLift = Vector3.zero;
    #endregion


    new void Start()
    {
        this.AircraftConfiguration = Configuration.LoadAircraft(@".\configs\" + vehicleName + ".cfg", this);

        base.VehicleComponents = this.AircraftConfiguration.VehicleComponents;

        foreach (ComponentBase component in base.VehicleComponents)
        {
            if (component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.EngineEnabled = true;
            }
        }

        base.Start();
        this.VehicleBody.mass = this.AircraftConfiguration.Mass * GameManager.scaleFactor;
    }

    void Update()
    {
        /*
        this.rollInput = Input.GetAxis("Roll");
        this.yawInput = Input.GetAxis("Yaw");
        this.pitchInput =  Input.GetAxis("Pitch");
        controlInput = this.rollInput, this.pitchInput, this.yawInput;
        */

        //Update Throttle
        this.throttleInput = Input.GetAxis("Throttle");

        steering = PhysicsUtils.Steering(
            base.LocalVelocity, base.LocalAngularVelocity,
            this.AircraftConfiguration.steeringCurve,
            this.controlInput, this.AircraftConfiguration.turnSpeed,
            this.AircraftConfiguration.turnAcceleration, this.AircraftConfiguration.pitchGLimit,
            this.AircraftConfiguration.gLimit);

        //Apply steering
        base.VehicleBody.AddRelativeTorque(steering.vector1, ForceMode.VelocityChange);

        if (Input.GetButtonDown("Debug"))
        {
            DebugText.enabled = !DebugText.enabled;
        }
    }

    void FixedUpdate()
    {
        //Set throttle
        this.Throttle = Mathf.Clamp(this.Throttle + this.throttleInput, 0f, 100f);
        //Reset thrust
        float totalThrust = 0;

        //Go through all components
        foreach (ComponentBase component in this.VehicleComponents)
        {
            // Ticks any tickable components in the pre-phase
            if (component is ITickableComponent)
            {
                ((ITickableComponent)component).PreTickComponent();
            }
            // Componenents should have their values get/set in this area
            if (component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.TargetRPMFactor = Throttle / 100f;
                //Debug.Log("Thrust Per Engine: " + engine.Thrust.ToString("0") + "N \n Real: " + (engine.Thrust / 100).ToString("0.0") + "kN");
                //Apply thrust
                totalThrust += engine.Thrust;
            }
            else if (component is HelmetMountedDisplay)
            {
                HelmetMountedDisplay hmd = (HelmetMountedDisplay)component;
                hmd.GForce = PhysicsUtils.CalculateLocalGForce(base.LocalAngularVelocity, base.LocalVelocity).y;
                hmd.Velocity = base.Velocity;
                hmd.Altitude = base.Altitude;
                hmd.RadarAltitude = base.RadarAltitude;
            }
            // Ticks any tickable components in the post-phase
            if (component is ITickableComponent)
            {
                ((ITickableComponent)component).PostTickComponent();
            }
        }

        #region ForceCalculation
        //Drag
        Vector3 drag = PhysicsUtils.CalculateDragForce(
            base.Altitude, this.AircraftConfiguration.FrontalArea, 
            base.LocalVelocity);

        //Lift
        Vector3 lift = PhysicsUtils.CalculatelTotalLift(
            this.DebugText,
            base.LocalVelocity,
            this.AircraftConfiguration.liftCurve,
            this.AircraftConfiguration.inducedDragCurve,
            this.AircraftConfiguration.airPreassureCoefficient,
            this.AircraftConfiguration.liftPower,
            this.AircraftConfiguration.rudderLiftCurve,
            this.AircraftConfiguration.rudderInducedDragCurve,
            this.AircraftConfiguration.rudderLiftPower);
        
        //Apply forces
        this.VehicleBody.AddRelativeForce(lift + drag);
        this.VehicleBody.AddRelativeForce(Vector3.forward * totalThrust);

        #region Debug
        
        if(drag.x >= lastDrag.x + 100 || drag.y >= lastDrag.y + 100 || drag.z >= lastDrag.z + 100 || drag.x <= lastDrag.x - 100 || drag.y <= lastDrag.y - 100 || drag.z <= lastDrag.z - 100 && drag != Vector3.zero)
        {
            Debug.LogWarning("Drag: " + transform.rotation * drag + "N");
        }
        if (lift.x >= lastLift.x + 100 || lift.y >= lastLift.y + 100 || lift.z >= lastLift.z + 100 || lift.x <= lastLift.x - 100 || lift.y <= lastLift.y - 100 || lift.z <= lastLift.z - 100 && lift != Vector3.zero)
        {
            Debug.LogWarning("Lift: " + transform.rotation *  lift + "N");
        }
        

        #region DrawVectors
        //Lift
        Debug.DrawRay(base.transform.position, transform.rotation * lift, Color.green);
        //Drag
        Debug.DrawRay(base.transform.position, transform.rotation * drag, Color.red);
        //Thrust
        Debug.DrawRay(base.transform.position, transform.rotation * Vector3.forward * totalThrust, Color.blue);
        //Gravity
        Debug.DrawRay(base.transform.position, base.VehicleBody.mass * 9.81f * Vector3.down, Color.black);

        Debug.DrawRay(base.transform.position, base.VehicleBody.velocity, Color.white);
        #endregion
        
        DebugText.text +=
            "Drag: " + (transform.rotation * drag).ToString("0.0") + "N + T: " + drag.magnitude.ToString("0.0") +
            "N\nLift: " + (transform.rotation * lift).ToString("0.0") + "N + T: " + lift.magnitude.ToString("0.0") +
            "N\nGravity: " + (base.VehicleBody.mass * 9.81f).ToString("0.0") + 
            "N\nThrust: " + totalThrust.ToString("0.0") + 
            "N\nThrottle: " + this.Throttle.ToString("0.0") + "%" +
            "\nInput: " + controlInput.ToString("0.0") +
            "\nSteering: " + steering.vector1.ToString("0.0") + 
            "\nAOA: " + PhysicsUtils.CalculateAngleOfAttack(base.LocalVelocity).ToString("0.0") + 
            "\nRudderAOA: " + PhysicsUtils.CalculateAngleOfAttackYaw(base.LocalVelocity).ToString("0.0");

        /*Log forces
        Debug.Log("Drag: " + drag + 
            "N\nLift: " + lift + 
            "N\nThrust: " + totalThrust + 
            "N\nGravity: " + base.VehicleBody.mass * 9.81f);*/
        
        lastDrag = drag;
        lastLift = lift;
       
        #endregion
        
        #endregion

        //Run post update on base
        base.PostUpdate();
    }
}
 