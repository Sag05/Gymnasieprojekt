using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Vehicles
{
    public class VehicleConfigurationBase
    {
        public float Mass { get; set; } 
    }

    public class AircraftConfiguration : VehicleConfigurationBase
    {

        public float OptimalLiftSpeedAtZeroAoA { get; set; }
        public AnimationCurve liftCurve { get; set;}
    }
}