using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Vehicles.Components;

namespace Assets.Scripts.Vehicles
{
    public class VehicleConfigurationBase
    {
        public IList<ComponentBase> VehicleComponents { get; set; } = new List<ComponentBase>();
        public int Mass { get; set; }
    }

    [Serializable]
    public class AircraftConfiguration : VehicleConfigurationBase
    {
        #region Floats
        public float FrontalArea { get; set; }
        public float airPreassureCoefficient { get; set; }
        public float liftPower { get; set; }
        public float rudderLiftPower { get; set; }
        public float pitchGLimit { get; set; }
        public float gLimit { get; set; }
        public float WingArea { get; set; }
        public float WingSpan { get; set; }
        public float inducedDragCoefficient { get; set; }
        #endregion

        #region Vectors
        public Vector3 turnSpeed { get; set; }
        public Vector3 turnAcceleration { get; set; }
        #endregion

        #region AnimationCurves
        public AnimationCurve AltitudeEffectivenessCurve { get; set; }

        public AnimationCurve liftCurve { get; set; }
        public AnimationCurve inducedDragCurve { get; set; }
        public AnimationCurve rudderLiftCurve { get; set; }
        public AnimationCurve rudderInducedDragCurve { get; set; }
        public AnimationCurve steeringCurve { get; set; }
        #endregion
    }
}