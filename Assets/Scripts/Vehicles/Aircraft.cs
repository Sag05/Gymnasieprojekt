using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aircraft : MonoBehaviour
{
    Rigidbody rigidbody;
    float thrust;
    public float thrustMultiplyer;

    float rollAxis;
    public float rollMultiplyer;
    public float maxRoll;
    public float minRoll;

    float yawAxis;
    public float yawMultiplyer;
    public float maxYaw;
    public float minYaw;

    float pitchAxis;
    public float pitchMultiplyer;
    public float maxPitch;
    public float minPitch;

    float lift = 0;
    float gravity = 9.8f;

    public Slider throttle;
    

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

    }

    void Update()
    { 



        //Roll
        rollAxis = Mathf.Clamp(rollMultiplyer * Input.GetAxis("Roll"), minRoll, maxRoll) * Time.deltaTime;
        rigidbody.AddTorque(transform.forward * rollAxis);
        
        //Yaw
        yawAxis = Mathf.Clamp(yawMultiplyer * Input.GetAxis("Yaw"), minYaw, maxYaw) * Time.deltaTime;
        rigidbody.AddTorque(transform.up *  yawAxis);

        //Pitch
        pitchAxis = Mathf.Clamp(pitchMultiplyer * Input.GetAxis("Pitch"), minPitch, maxPitch) * Time.deltaTime;
        rigidbody.AddTorque(transform.right * pitchAxis);


        //Thrust
        thrust = Mathf.Clamp(thrust + Input.GetAxis("Thrust"), 0f, 100f);
        throttle.value = thrust;
        rigidbody.AddForce(transform.forward * thrust * thrustMultiplyer * Time.deltaTime);

        
        //Gravity
        rigidbody.AddForce(Vector3.down * gravity);

        //Lift
        lift = gravity;
        rigidbody.AddForce(transform.up * lift);


    }

}
