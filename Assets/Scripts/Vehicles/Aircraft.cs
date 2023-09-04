using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Aircraft : MonoBehaviour
{

    // Start is called before the first frame update
    Rigidbody rigidbody;
    float thrust;
    Vector3 position;

    float pitchAxis;
    public float maxPitch;
    public float minPitch;

    float rollAxis;
    public float maxRoll;
    public float minRoll;

    float yawAxis;
    public float maxYaw;
    public float minYaw;


    public TextMeshProUGUI speedIndicator;

    void Start()
    {

        rigidbody = gameObject.GetComponent<Rigidbody>();
        position = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {



        //Pitch
        pitchAxis += 100 * Mathf.Clamp(Input.GetAxis("Pitch"), minPitch, maxPitch) * Time.deltaTime;
        //pitchAxis *= Time.deltaTime;

        //Roll
        rollAxis += 100 * Mathf.Clamp(Input.GetAxis("Roll"), minRoll, maxRoll) * Time.deltaTime;
        //rollAxis *=Time.deltaTime;

        //Yaw
        yawAxis += 100 * Mathf.Clamp(Input.GetAxis("Yaw"), minYaw, maxYaw) * Time.deltaTime;
        //yawAxis = Time.deltaTime;

        //Rotate
        Quaternion rotation = Quaternion.Euler(rollAxis, yawAxis, pitchAxis);
        gameObject.transform.localRotation = rotation;


        thrust = position.y * Time.deltaTime;

        //rigidbody.AddForce(transform.forward * thrust);
    }

}
