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
        public VehicleBase ParentVehicle { get; set; }
        public float HitPoints { get; set; }
    }
}
