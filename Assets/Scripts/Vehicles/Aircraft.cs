using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Aircraft : VehicleBase
{
    float throttle;

    public AircraftEngine[] JetEngines;

    public float thrustMultiplier;

    float rollAxis;
    float rollInput;
    public float rollMultiplier;
    public float maxRoll;
    public float minRoll;

    float yawAxis;
    float yawInput;
    public float yawMultiplier;
    public float maxYaw;
    public float minYaw;

    float pitchAxis;
    float pitchInput;
    public float pitchMultiplier;
    public float maxPitch;
    public float minPitch;

    float gravity = 9.82f;
    public float mass;
    
    float lift;
    public float liftMultiplier;

    float throttleInput;
    public Slider throttleSlider;
    public TextMeshProUGUI speedIndicator;

    float speed;

    void Start()
    {
        this.JetEngines = new AircraftEngine[]
        {
            new AircraftEngine()
            {
                HitPoints = 100f,
                TurbineMaxRPM = 20000,
                TurbineAcceleration = 600
            }
        };
        this.VehicleConfiguration = new AircraftConfiguration()
        {
            optimalLiftSpeedAtZeroAoA = 20
        };

        this.mass = this.VehicleBody.mass;
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
        throttle = Mathf.Clamp(throttle + throttleInput, 0f, 100f);
        throttleSlider.value = throttle;

        foreach (AircraftEngine engine in this.JetEngines)
        {
            engine.TargetRPMFactor = throttle / 100f;
            engine.Tick();
        }

        // speed = VehicleBody.velocity.magnitude * 10;
        // 
        // speedIndicator.text = Mathf.Round(speed).ToString();
        // 
        // 
        // //Roll
        // VehicleBody.AddTorque(transform.forward * rollAxis);
        // rollAxis = Mathf.Clamp(rollMultiplier * rollInput, minRoll, maxRoll);
        // 
        // //Yaw
        // yawAxis = Mathf.Clamp(yawMultiplier * yawInput, minYaw, maxYaw);
        // VehicleBody.AddTorque(transform.up *  yawAxis);
        // 
        // //Pitch
        // pitchAxis = Mathf.Clamp(pitchMultiplier * pitchInput, minPitch, maxPitch) ;
        // VehicleBody.AddTorque(transform.right * pitchAxis);
        // 
        //Thrust

        
        // VehicleBody.AddForce(transform.forward * throttle * thrustMultiplier);
        // 
        // //Gravity
        // VehicleBody.AddForce(Vector3.down * gravity * mass);
        // 
        // //Lift
        // //cl = (2*m*g)/(p*v^2) 
        // float liftCoefficient = (2 * mass * gravity) / (1 * 0);
        // 
        // 
        // VehicleBody.AddForce(transform.up * lift);
        // 
        // //Drag
    }
}
 