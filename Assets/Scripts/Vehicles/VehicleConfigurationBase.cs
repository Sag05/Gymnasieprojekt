using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles
{
    public class VehicleConfigurationBase
    {
    }

    public class AircraftConfiguration : VehicleConfigurationBase
    {
        
        public float optimalLiftSpeedAtZeroAoA { get; set; }

    }
}