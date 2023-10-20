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
                TurbineAcceleration = 600,
                MaxThrust = 10000
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
        this.Throttle = Mathf.Clamp(this.Throttle + this.throttleInput, 0f, 100f);

        foreach (ComponentBase component in this.VehicleComponents)
        {
            if(component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.TargetRPMFactor = Throttle / 100f;
                engine.Tick();
                //Apply thrust
                this.VehicleBody.AddForce(transform.forward * engine.Thrust);
            } 
        }
    }
}
 