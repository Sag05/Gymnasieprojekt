using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Vehicles.Components;

namespace Assets.Scripts.Vehicles
{
    public class VehicleConfigurationBase
    {
        /// <summary>
        /// List of components attached to the vehicle
        /// </summary>
        public IList<ComponentBase> VehicleComponents { get; set; } = new List<ComponentBase>();

        /// <summary>
        /// Mass of the vehicle
        /// </summary>
        public float Mass { get; set; }
    }

    [Serializable]
    public class AircraftConfiguration : VehicleConfigurationBase
    {
        /// <summary>
        /// The frontal area of the aircraft
        /// </summary>
        public float FrontalArea;
        public AnimationCurve liftCurve;
    }
}