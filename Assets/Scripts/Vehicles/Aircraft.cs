using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        speed = rigidbody.velocity.magnitude * 10;
        
        speedIndicator.text = Mathf.Round(speed).ToString();


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
        //cl = (2*m*g)/(p*v^2) 
        float liftCoefficient = (2 * mass * gravity) / (1 * 0);

        
        rigidbody.AddForce(transform.up * lift);

        //Drag
    }
}
 