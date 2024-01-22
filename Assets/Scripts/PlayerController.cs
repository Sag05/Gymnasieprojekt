using Assets.Scripts;
using Assets.Scripts.Components;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private MonoBehaviour monoBehaviour => this;
    private VehicleType controlledVehicleType;
    private FreeTrackClientDll64 freeTrackClient;
    private GameObject controlledVehicle;
    public TextMeshProUGUI DebugText;

    #region Input
    public Vector3 ControlInput {get; private set;}
    public float ThrottleInput {get; private set;}

    public void OnMovement(InputValue value)
    {
        ControlInput = value.Get<Vector3>();
    }

    public void OnThrottle(InputValue value)
    {
        ThrottleInput = value.Get<float>();
    }

    public void OnGear()
    {
        Debug.Log("Input: Gear");
        controlledVehicle.GetComponent<Aircraft>().AnimationHandler("Gear.Gear");
    }

    public void OnFlaps()
    {
        //NOT YET IMPLEMENTED 
        //controlledVehicle.GetComponent<Aircraft>().AnimationHandler("Flaps");
    }

    public void OnAirbrake()
    {
        Debug.Log("input: Airbrake");
        controlledVehicle.GetComponent<Aircraft>().AnimationHandler("Airbrake.Airbrake");
    }

    public void OnCanopy()
    {
        Debug.Log("input: Canopy"); 
        controlledVehicle.GetComponent<Aircraft>().AnimationHandler("Canopy.Canopy");
    }

    public void OnDebug()
    {
        DebugText.enabled = !DebugText.enabled;
    }

    #region FireWeapon
    //public void OnFirePrimaryWeapon()
    //{
    //action.started += ctx => StartFirePrimaryWeapon();
    //action.canceled += ctx => StopFirePrimaryWeapon();
    //}


    //public void OnFirePrimaryWeapon(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        StartFirePrimaryWeapon();
    //    }
    //    else if (context.canceled)
    //    {
    //        StopFirePrimaryWeapon();
    //    }
    //}


    bool fireWasPressed = false;

    public void OnFirePrimaryWeapon(InputValue value)
    {
        if (controlledVehicle is not null){
            if(fireWasPressed != value.isPressed)
            {
                if (value.isPressed)
                {
                    StartFirePrimaryWeapon();
                }
                else
                {
                    StopFirePrimaryWeapon();
                }
            }
            fireWasPressed = value.isPressed;
        }
    }
    #endregion

    private void StartFirePrimaryWeapon() => controlledVehicle.GetComponent<Aircraft>().EntityComponents.GetComponentOfType<StoresManagementSystem>().StartFirePrimaryWeapon();
    private void StopFirePrimaryWeapon() => controlledVehicle.GetComponent<Aircraft>().EntityComponents.GetComponentOfType<StoresManagementSystem>().StopFirePrimaryWeapon();
    
    public void OnFireSecondaryWeapon()
    {
        controlledVehicle.GetComponent<Aircraft>().EntityComponents.GetComponentOfType<StoresManagementSystem>().FireSecondaryWeapon();
    }

    public void OnSwitchSOI()
    {
        switch (controlledVehicleType)
        {
            case VehicleType.Aircraft:
                controlledVehicle.GetComponent<Aircraft>().SwitchSOI();
                break;
            case VehicleType.Helicopter:
                
                break;
            case VehicleType.GroundVehicle:
                
                break;
        }
    }
    #endregion

    public void SelectVehicle(GameObject aircraft)
    {
        Debug.Log("Trying to select vehicle " + aircraft.name);
        controlledVehicle = aircraft; //GameObject.Find(vehicleName);
        Debug.Log("Found vehicle " + controlledVehicle.name);
        if (controlledVehicle.GetComponent<Aircraft>() != null)
        {
            Debug.Log("Found aircraft component on " + controlledVehicle.name);
            controlledVehicleType = VehicleType.Aircraft;
            controlledVehicle.GetComponent<Aircraft>().Controller = this;
            Debug.Log(this.name + " is now controlling " + controlledVehicle.name);
        }
        Debug.Log("Updating controls");
        UpdateControls();
    }

    void Start()
    {
        DebugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();
        freeTrackClient = gameObject.AddComponent<FreeTrackClientDll64>();

        #region TEMP_AIRCRAFT_SPAWN
        SelectVehicle(Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/aircraft.prefab")));
        #endregion
        //VehicleLoader vehicleLoader = gameObject.AddComponent<VehicleLoader>();
        //vehicleLoader.LoadVehicleSelectionMenu(this);
    }

    void Update()
    {
        UpdateTracking();
    }

    private void UpdateTracking()
    {
        if (freeTrackClient.IsTracking)
        {
            DoubleVector3 trackingData = freeTrackClient.GetTrackingData();
            transform.rotation = Quaternion.Euler(trackingData.vector1);
            transform.localPosition = trackingData.vector2;
        }
    }
    private void UpdateControls()
    {
        switch (controlledVehicleType)
        {
            case VehicleType.Aircraft:
                PlayerInput.all[0].SwitchCurrentActionMap("Aircraft");
                Debug.Log("Set Controls to 'Aircraft'");
                break;
            default:
                Debug.LogWarning("VehicleType " + controlledVehicleType + " has no controls assigned.");
                break;
        }
    }   
}