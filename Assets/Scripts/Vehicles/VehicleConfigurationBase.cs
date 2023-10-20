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
<<<<<<< HEAD
        public IList<ComponentBase> VehicleComponents;
        public float Mass;
=======
        public float Mass { get; set; }
        public float MaxThrust { get; internal set; }
>>>>>>> main
    }

    [Serializable]
    public class AircraftConfiguration : VehicleConfigurationBase
    {
        public float FrontalArea;
        public AnimationCurve liftCurve;
    }
}