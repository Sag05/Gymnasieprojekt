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
        public OrdinanceType AcceptedOrdinanceTypes { get; set; } //OrdinanceType is now a flag, the list part can be removed
        public GuidanceType AcceptedGuidanceTypes { get; set; } //GuidanceType is now a flag, the list part can be removed
    }
}
