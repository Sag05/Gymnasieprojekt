using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Vehicles.Components
{
    internal class HelmetMountedDisplay : ComponentBase
    {
        public HelmetMountedDisplay(VehicleBase vehicle) : base(vehicle) { }

        public Slider throttleSlider { get; set; }

        public TextMeshProUGUI speedIndicator { get; set; }

        public bool tick()
        {
            this.speedIndicator.text = this.ParentVehicle.VelocityMagnitude.ToString("0");
            this.throttleSlider.value = this.ParentVehicle

            return true;
        }
    }
}
