using System;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Components
{
    public class AircraftEngine : ComponentBase, ITickableComponent
    {
        public AircraftEngine(VehicleBase vehicle) : base(vehicle) { }
        #region Variables
        /// <summary>
        /// The maximum RPM of the turbine, measured in RPM
        /// </summary>
        public float TurbineMaxRPM { get => turbineMaxRPM; set { if (value <= 0) throw new ArgumentOutOfRangeException("TurbineMaxRPM", value, "TargetMaxRPM can not be less than or equal to 0"); this.turbineMaxRPM = value; } }
        private float turbineMaxRPM;
        /// <summary>
        /// The minimum RPM of the turbine, measured in RPM
        /// </summary>
        public float TurbineMinRPM { get; set; }
        /// <summary>
        /// The acceleration of the turbine, measured in RPM/s
        /// </summary>
        public float TurbineAcceleration { get; set; }
        /// <summary>
        /// Toggle engine on/off(True/False)
        /// </summary>
        public bool EngineEnabled { get; set; }
        /// <summary>
        /// The maximum thrust of the engine, measured in Newtons
        /// </summary>
        public float MaxThrust { get => maxThrust; set { if (value <= 0) throw new ArgumentOutOfRangeException("MaxThrust", value, "MaxThrust can not be less than or equal to 0"); this.maxThrust = value * GameManager.scaleFactor; } }
        private float maxThrust;
        /// <summary>
        /// The current turbine RPM, measured in RPM
        /// </summary>
        public float TurbineRPM { get; set; }
        /// <summary>
        /// The current thrust factor, measured in 0-1
        /// </summary>
        public float TurbineThrustFactor { get => this.TurbineRPM / this.TurbineMaxRPM; }
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

        public float Thrust { get; private set; }

        public AnimationCurve ThrustCoefficientCurve { get; set; }
        public float Altitude { get; set; }
        #endregion
        public bool Tick()
        {
            this.TurbineRPM -=
                // Turbine "drag" (deacceleration)
                (this.turbineDragCoefficient * 1 * this.TurbineRPM * this.TurbineRPM * 0.5f);
                // Turbine power (acceleration)
            this.TurbineRPM +=
                (this.TurbineAcceleration * this.currentTargetRPMFactor * (this.EngineEnabled ? 1 : 0));
            //Debug.Log("T-RPM:" + this.TurbineRPM + "\n" + "T-TAR:" + this.TargetRPMFactor + "\n" + "T-AX" + this.currentTargetRPMFactor);

            //thrust is the thrustfactor times the maximum thrust
            //Debug.Log(MaxThrust);
            this.Thrust = this.TurbineThrustFactor * this.ThrustCoefficientCurve.Evaluate(Altitude) * this.maxThrust;
            return true;
        }

        public bool PreTickComponent() => true;
        public bool PostTickComponent() => this.Tick();
    }
}
