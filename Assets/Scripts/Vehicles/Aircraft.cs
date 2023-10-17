using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Aircraft : MonoBehaviour
{
    Rigidbody rigidbody;
    float throttle;
    public float thrustMultiplyer;

    float rollAxis;
    float rollInput;
    public float rollMultiplyer;
    public float maxRoll;
    public float minRoll;

    float yawAxis;
    float yawInput;
    public float yawMultiplyer;
    public float maxYaw;
    public float minYaw;

    float pitchAxis;
    float pitchInput;
    public float pitchMultiplyer;
    public float maxPitch;
    public float minPitch;

    float gravity = 9.8f;
    public float mass;
    
    float lift;
    public float liftMultiplyer;
    public float maxLift;
    float rotationMultiplyer;

    float throttleInput;
    public Slider throttleSlider;
    public TextMeshProUGUI speedIndicator;

    float speed;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        rollInput = Input.GetAxis("Roll");
        yawInput = Input.GetAxis("Yaw");
        pitchInput =  Input.GetAxis("Pitch");
        throttleInput = Input.GetAxis("Throttle");
    }

    void FixedUpdate()
    {
        speed = Mathf.RoundToInt(rigidbody.velocity.x + rigidbody.velocity.y + rigidbody.velocity.z * 10);
        speedIndicator.text = speed.ToString();
        
        rotationMultiplyer = GetRotation();


        //Roll
        rigidbody.AddTorque(transform.forward * rollAxis);
        rollAxis = Mathf.Clamp(rollMultiplyer * rollInput, minRoll, maxRoll);
        
        //Yaw
        yawAxis = Mathf.Clamp(yawMultiplyer * yawInput, minYaw, maxYaw);
        rigidbody.AddTorque(transform.up *  yawAxis);

        //Pitch
        pitchAxis = Mathf.Clamp(pitchMultiplyer * pitchInput, minPitch, maxPitch) ;
        rigidbody.AddTorque(transform.right * pitchAxis);

        //Thrust
        throttle = Mathf.Clamp(throttle + throttleInput, 0f, 100f);
        throttleSlider.value = throttle;
        rigidbody.AddForce(transform.forward * throttle * thrustMultiplyer);

        //Gravity
        rigidbody.AddForce(Vector3.down * gravity * mass);

        //Lift
        lift = Mathf.Clamp(liftMultiplyer * speed / rotationMultiplyer, 0, maxLift);
        rigidbody.AddForce(transform.up * lift);

        //Drag
    }

    private float GetRotation()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        //Round pitch rotation down to max 1, min -1
        float pitchRotation = currentRotation.x / 180;
        float rollRotation = currentRotation.z / 180;

        pitchRotation =  Mathf.Clamp(Mathf.Abs(pitchRotation), 0.1f, 1);
            

        float rotation = ;
        return rotation;
    }
}
 