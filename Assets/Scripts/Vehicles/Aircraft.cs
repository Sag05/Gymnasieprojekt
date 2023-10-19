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
    float throttleFactor;

    float rollInput;
    float yawInput;
    float pitchInput;
    float throttleInput;
    
    public Slider throttleSlider;
    public TextMeshProUGUI speedIndicator;

    public AnimationCurve testCurve;

    new void Start()
    {
        this.VehicleComponents = new ComponentBase[]
        {
            new AircraftEngine(this)
            {
                HitPoints = 100f,
                TurbineMaxRPM = 20000,
                TurbineAcceleration = 600
            },
            new HelmetMountedDisplay(this)
            {
                throttleSlider = this.throttleSlider,
                speedIndicator = this.speedIndicator
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


        foreach (ComponentBase component in this.VehicleComponents)
        {
            if (component is HelmetMountedDisplay)
            {
                HelmetMountedDisplay hmd = (HelmetMountedDisplay)component;
                hmd.tick();
            }
        }
    }

    void FixedUpdate()
    {
        //Set throttle
        this.throttleFactor = Mathf.Clamp(this.throttleFactor + this.throttleInput, 0f, 100f);
        this.throttleSlider.value = this.throttleFactor;

        //Set speed indicator
        this.speed = this.VehicleBody.velocity.magnitude * 10;

        foreach (ComponentBase component in this.VehicleComponents)
        {
            if(component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.TargetRPMFactor = throttleFactor / 100f;
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
 