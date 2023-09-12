using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI speedIndicator;

    float speed;

    float speedE;
    Vector3 previousPosition;
    

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(SpeedReckoner());
    }

    void Update()
    { 
        speedE = 

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

    float updateDelay = 0.2f;

    private IEnumerator SpeedReckoner()
    {
        YieldInstruction timedWait = new WaitForSeconds(updateDelay);
        Vector3 lastPosition = transform.position;
        float lastTimestamp = Time.time;

        while (enabled)
        {
            yield return timedWait;

            var deltaPosition = (transform.position - lastPosition).magnitude;
            var deltaTime = Time.time - lastTimestamp;

            if (Mathf.Approximately(deltaPosition, 0f)) // Clean up "near-zero" displacement
                deltaPosition = 0f;

            speed = (deltaPosition / deltaTime) * 100;
            speedIndicator.text = Mathf.RoundToInt(speed).ToString();

            lastPosition = transform.position;
            lastTimestamp = Time.time;
        }
    }
}
 