using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Vehicles.Components;

namespace Assets.Scripts.Vehicles
{
    /// <summary>
    /// The base class for representing all types vehicle configurations
    /// </summary>
    public class VehicleConfigurationBase
    {
        /// <summary>
        /// The name of the model
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// Represents base components, you'll have to forward them into the <see cref="ComponentManager"/> of your <see cref="VehicleBase"/> or derived class
        /// </summary>
        public IList<ComponentBase> VehicleComponents { get; set; } = new List<ComponentBase>();
        /// <summary>
        /// The vehicle's mass, in kg
        /// </summary>
        public int Mass { get; set; } //[Venom] Considering making this a float?
    }

    [Serializable]
    public class AircraftConfiguration : VehicleConfigurationBase 
    {
        public AnimationCurve AltitudeEffectivenessCurve { get; set; }
        
        #region Lift
        public int LiftPower { get; set; }
        public int RudderLiftPower { get; set; }
        public float InducedDragCoefficient { get; set; }
        public AnimationCurve LiftCurve { get; set; }
        public AnimationCurve InducedDragCurve { get; set; }
        public AnimationCurve RudderLiftCurve { get; set; }
        #endregion

        #region Drag
        public AnimationCurve SideDragCurve { get; set; }
        public AnimationCurve TopDragCurve { get; set; }
        public AnimationCurve FrontDragCurve { get; set; }
        #endregion
        
        #region Steering
        public float PitchGLimit { get => pitchGLimit; set { if (value < 2) throw new ArgumentOutOfRangeException("PitchGLimit", value, "PitchGLimit can not be less than 2"); this.pitchGLimit = value - 1; } }
        private float pitchGLimit;
        public float GLimit { get => gLimit; set { if (value < 2) throw new ArgumentOutOfRangeException("GLimit", value, "GLimit can not be less than 2"); this.gLimit = value - 1; } }
        private float gLimit;
        public Vector3 TurnSpeed { get; set; }
        public Vector3 TurnAcceleration { get; set; }
        public AnimationCurve SteeringCurve { get; set; }
        #endregion

    }
}