using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles.Components
{
    public class ComponentBase
    {
        public ComponentBase(VehicleBase vehicle) {}
        
        /// <summary>
        /// Parent vehiclebase of the component
        /// </summary>
        public VehicleBase ParentVehicle { get; set; }

        /// <summary>
        /// Hit points of the component
        /// </summary>
        public float HitPoints { get; set; }
    }
}
