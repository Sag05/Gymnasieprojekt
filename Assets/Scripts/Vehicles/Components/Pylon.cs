using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles.Components
{
    internal class Pylon : ComponentBase
    {
        public Pylon(VehicleBase vehicle) : base(vehicle)
        {
            
        }
        public string PylonName { get; set; }
        public List<OrdinanceType> AcceptedOrdinanceTypes { get; set; }
        public List<GuidanceType> AcceptedGuidanceTypes { get; set; }
    }
}
