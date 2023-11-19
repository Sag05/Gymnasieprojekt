using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
        DebugText.text = ControlInput.ToString("0.0");
    }

    public void OnThrottle(InputValue value)
    {
        ThrottleInput = value.Get<float>();
    }

    public void OnDebug()
    {
        DebugText.enabled = !DebugText.enabled;
    }
    #endregion

    public void SelectVehicle(GameObject aircraft){
        Debug.Log("Trying to select vehicle " + aircraft.name);
        controlledVehicle = aircraft; //GameObject.Find(vehicleName);
        Debug.Log("Found vehicle " + controlledVehicle.name);
        if (controlledVehicle.GetComponent<Aircraft>() != null){
            Debug.Log("Found aircraft component on " + controlledVehicle.name);
            controlledVehicleType = VehicleType.Aircraft;
            controlledVehicle.GetComponent<Aircraft>().Controller = this;
            Debug.Log(this.name + " is now controlling " + controlledVehicle.name);
        }

        UpdateControls();
    }

    void Start()
    {
        DebugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();
        freeTrackClient = gameObject.AddComponent<FreeTrackClientDll64>();

        VehicleLoader vehicleLoader = gameObject.AddComponent<VehicleLoader>();
        vehicleLoader.LoadVehicleSelectionMenu(this);
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
                break;
            default:
                break;
        }
    }   
}