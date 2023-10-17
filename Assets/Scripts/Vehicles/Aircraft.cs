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

    float rollInput;
    float yawInput;
    float pitchInput;

    float throttleInput;
    public Slider throttleSlider;
    public TextMeshProUGUI speedIndicator;

    float speed;

    new void Start()
    {

        this.VehicleComponents = new ComponentBase[]
        {
            new AircraftEngine(this)
            {
                HitPoints = 100f,
                TurbineMaxRPM = 20000,
                TurbineAcceleration = 600
            }
        };
        this.VehicleConfiguration = new AircraftConfiguration()
        {
            OptimalLiftSpeedAtZeroAoA = 20
        };

        base.Start();

        this.Mass = this.VehicleBody.mass;
        //Debug.Log("Mass rigidbody mass: " + this.VehicleBody.mass);
    }

    void Update()
    {
        this.rollInput = Input.GetAxis("Roll");
        this.yawInput = Input.GetAxis("Yaw");
        this.pitchInput =  Input.GetAxis("Pitch");
        this.throttleInput = Input.GetAxis("Throttle");
    }

    void FixedUpdate()
    {
        //Set throttle
        this.throttle = Mathf.Clamp(this.throttle + this.throttleInput, 0f, 100f);
        this.throttleSlider.value = this.throttle;

        //Set speed indicator
        this.speed = this.VehicleBody.velocity.magnitude * 10;
        this.speedIndicator.text = Mathf.Round(this.speed).ToString();

        foreach (ComponentBase component in this.VehicleComponents)
        {
            if(component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.TargetRPMFactor = throttle / 100f;
                engine.Tick();
            }
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
 