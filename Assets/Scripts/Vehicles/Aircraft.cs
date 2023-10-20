using Assets.Scripts;
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
    
    AircraftConfiguration AircraftConfiguration;

    string vehicleName = "F5";   

    new void Start()
    {
        this.AircraftConfiguration = Configuration.LoadAircraft(@".\configs\" + vehicleName + ".cfg", this);

        foreach (ComponentBase component in this.VehicleComponents)
        {
            if (component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.EngineEnabled = true;
            }
        }

        base.Start();
        this.VehicleBody.mass = this.AircraftConfiguration.Mass;
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
        this.Throttle = Mathf.Clamp(this.Throttle + this.throttleInput, 0f, 100f);
        //base.UpdateState();

        float totalThrust = 0;
        foreach (ComponentBase component in this.VehicleComponents)
        {
            if(component is AircraftEngine)
            {
                AircraftEngine engine = (AircraftEngine)component;
                engine.TargetRPMFactor = Throttle / 100f;
                engine.Tick();
                Debug.Log("Thrust: " + engine.Thrust);
                //Apply thrust
                totalThrust += engine.Thrust;
            }
            else if (component is HelmetMountedDisplay)
            {
                HelmetMountedDisplay hmd = (HelmetMountedDisplay)component;
                hmd.GForce = base.LocalGForce.y;
                hmd.Velocity = base.Velocity;
                hmd.Altitude = base.Altitude;
                hmd.RadarAltitude = base.RadarAltitude;
                hmd.Throttle = this.Throttle / 100f;
                hmd.Tick();
            }
        }

        //Apply forces
        //Thrust
        this.VehicleBody.AddForce(this.transform.forward * totalThrust);
        //Drag
        //this.VehicleBody.AddRelativeForce();
        //Lift
    }
}
 