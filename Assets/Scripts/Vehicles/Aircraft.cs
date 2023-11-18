using Assets.Scripts;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class Aircraft : VehicleBase
{
    #region Variables
    private float throttleInput;

    private DoubleVector3 steering = new DoubleVector3();

    private AircraftConfiguration AircraftConfiguration;
    Vector3 controlInput = new Vector3();

    bool emulateInput = false;
    Slider pitchSlider;
    Slider rollSlider;
    Slider yawSlider;
    Vector3 inputToApply;


    TextMeshProUGUI DebugText;

    public GameObject model;
    Animation gearAnimation;
    #endregion



    #region Input
    public void OnMovement(InputValue value)
    {
        controlInput = value.Get<Vector3>();
        DebugText.text = controlInput.ToString("0.0");
    }

    public void OnThrottle(InputValue value)
    {
        throttleInput = value.Get<float>();
    }

    public void OnDebug()
    {
        DebugText.enabled = !DebugText.enabled;
    }

    public void OnDebug1()
    {
        emulateInput = !emulateInput;
        pitchSlider.gameObject.SetActive(!pitchSlider.gameObject.activeSelf);
        rollSlider.gameObject.SetActive(!rollSlider.gameObject.activeSelf);
        yawSlider.gameObject.SetActive(!yawSlider.gameObject.activeSelf);
    }
    #endregion  

    private void LoadModel(){
        //model = Instantiate(      (@".\configs\aircrafts\" + AircraftConfiguration.ModelName), transform);
        //model = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Aircraft/" + AircraftConfiguration.ModelName), transform);
        model = GameObject.Find(AircraftConfiguration.ModelName);
        model.transform.localScale = Utilities.FloatToVector3(GameManager.scaleFactor);

        gearAnimation = model.AddComponent<Animation>();
    }

    new void Start()
    {
        this.AircraftConfiguration = Configuration.LoadAircraft(@".\configs\aircrafts\" + gameObject.name + ".cfg", this);
        base.VehicleComponents.AddComponents(this.AircraftConfiguration.VehicleComponents.ToArray());
        LoadModel();
        
        DebugText = Utilities.GetText("DebugText");
        pitchSlider = Utilities.GetSlider("PitchSlider");
        rollSlider = Utilities.GetSlider("RollSlider");
        yawSlider = Utilities.GetSlider("YawSlider");

        pitchSlider.gameObject.SetActive(false);
        rollSlider.gameObject.SetActive(false);
        yawSlider.gameObject.SetActive(false);

        foreach (ComponentBase component in base.VehicleComponents.Components)
        {
            if(component is Pylon pylon)
            {
                Debug.Log(pylon.GetPylonInfo());
            }
            if (component is AircraftEngine engine)
            {
                engine.EngineEnabled = true;
            }
        }

        base.Start();
        this.VehicleBody.mass = this.AircraftConfiguration.Mass * GameManager.scaleFactor;
    }

    void Update()
    {
        #region Debug code
        // == DEBUGGING CONTEXT == //
        // All debugging content should be between this area
        //DebugText.text = "";
        // == END OF DEUBBING CONTEXT == //
        #endregion

        if (emulateInput)
        {
            this.controlInput = new Vector3(pitchSlider.value, yawSlider.value, rollSlider.value);
        }
        inputToApply = controlInput;
    }

    void FixedUpdate()
    {
        //Run pre-update on base
        base.PreUpdate();

        //Set throttle
        this.Throttle = Mathf.Clamp(this.Throttle + this.throttleInput, 0f, 100f);
        
        //Reset thrust and additional drag
        float totalThrust = 0;
        float additionalDrag = 0;

        //Go through all components
        foreach (ComponentBase component in this.VehicleComponents.Components)
        {
            // Ticks any tickable components in the pre-phase
            if (component is ITickableComponent)
            {
                ((ITickableComponent)component).PreTickComponent();
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
            if (component is ITickableComponent)
            {
                ((ITickableComponent)component).PostTickComponent();
            }
        }

        #region ForceCalculation
        //Drag
        Vector3 drag = PhysicsUtils.CalculateDrag(base.LocalVelocity, base.Altitude, AircraftConfiguration.AltitudeEffectivenessCurve,
            this.AircraftConfiguration.SideDragCurve, this.AircraftConfiguration.TopDragCurve, this.AircraftConfiguration.FrontDragCurve, additionalDrag);

        //Lift
        Vector3 lift = PhysicsUtils.CalculatelTotalAircraftLift(this.DebugText, base.LocalVelocity, base.Altitude, this.AircraftConfiguration);

        
        //Calculate Steering
        steering = PhysicsUtils.AircraftSteering(
            base.LocalVelocity, base.LocalAngularVelocity, this.inputToApply,
            this.AircraftConfiguration, this.DebugText);
        
        
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
        //DebugText.text = "Thrust: " + totalThrust;

        //Debug.Log("Lift: " + lift);

        /*
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

        //Reset input
        inputToApply = Vector3.zero;        
        
        //Run post-update on base
        base.PostUpdate();
    }

    public void AnimationHandler()
    {

    }

}
