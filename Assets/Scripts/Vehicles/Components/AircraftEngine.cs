﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles.Components
{
    public class AircraftEngine : ComponentBase
    {
        public AircraftEngine(VehicleBase vehicle) : base(vehicle) { }
        public float TurbineMaxRPM { get => turbineMaxRPM; set { if (value <= 0) throw new ArgumentOutOfRangeException("TurbineMaxRPM", value, "TargetMaxRPM can not be less than or equal to 0"); this.turbineMaxRPM = value; } }
        private float turbineMaxRPM;
        public float TurbineMinRPM { get; set; }
        public float TurbineAcceleration { get; set; }
        public bool EngineEnabled { get; set; }

        /// <summary>
        /// The current turbine RPM, measured in RPM
        /// </summary>
        public float TurbineRPM { get; set; }

        /// <summary>
        /// The current thrust factor, measured in 0-1
        /// </summary>
        public float TurbineThrustFactor { get => this.TurbineRPM / this.TurbineMaxRPM;}
        
        /// <summary>
        /// Target RPM factor, measured in 0-1
        /// </summary>
        public float TargetRPMFactor
        { 
            get => this.currentTargetRPMFactor; 
            set
            {
                if (value > 1) throw new ArgumentOutOfRangeException("TargetRPM", value, "TargetRPM expected to be in 0-1");
                if (value < 0) throw new ArgumentOutOfRangeException("TargetRPM", value, "TargetRPM expected to be in 0-1");
                this.currentTargetRPMFactor = value;
            } 
        }
        private float currentTargetRPMFactor;

        public float turbineDragCoefficient
        {
            get
            {
                return 2 * this.TurbineAcceleration / (this.TurbineMaxRPM * this.TurbineMaxRPM);
            }
        }


        public bool Tick()
        {
            this.TurbineRPM -=
                // Turbine "drag" (deacceleration)
                (this.turbineDragCoefficient * 1 * this.TurbineRPM * this.TurbineRPM * 0.5f);
                // Turbine power (acceleration)
            this.TurbineRPM +=
                (this.TurbineAcceleration * this.currentTargetRPMFactor * (this.EngineEnabled ? 1 : 0));

            //UnityEngine.Debug.Log("T-RPM:" + this.TurbineRPM + "\n" + "T-TAR:" + this.TargetRPMFactor);
            


            return true;
        }
    }
}
