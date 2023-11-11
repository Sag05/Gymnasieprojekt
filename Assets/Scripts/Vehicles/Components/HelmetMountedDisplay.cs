using TMPro;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Components
{
    internal class HelmetMountedDisplay : ComponentBase, ITickableComponent
    {
        public HelmetMountedDisplay(VehicleBase vehicle) : base(vehicle) 
        {
            speedText = GameObject.Find("speedText").GetComponent<TextMeshProUGUI>();
            altitudeText = GameObject.Find("altitudeText").GetComponent<TextMeshProUGUI>();
            radarAltitudeIndicator = GameObject.Find("radarAltitudeIndicator").GetComponent<TextMeshProUGUI>();
            gForceText = GameObject.Find("gForceText").GetComponent<TextMeshProUGUI>();
            machText = GameObject.Find("machText").GetComponent<TextMeshProUGUI>();
            
            //set color 
            speedText.color = Color.green;
            altitudeText.color = Color.green;
            radarAltitudeIndicator.color = Color.green;
            gForceText.color = Color.green;
            machText.color = Color.green;
        }
        //HMD elements
        private TextMeshProUGUI speedText;
        private TextMeshProUGUI altitudeText;
        private TextMeshProUGUI radarAltitudeIndicator;
        private TextMeshProUGUI gForceText;
        private TextMeshProUGUI machText;

        //Variables
        public Vector3 Velocity { private get; set; }
        public float Altitude { private get; set; }
        public float RadarAltitude { private get; set; }
        public float GForce { private get; set; }

        public bool PostTick()
        {
            //Set speed text, display in km/h
            this.speedText.text = (this.Velocity.magnitude * 3.6f / GameManager.scaleFactor).ToString("0");
            this.gForceText.text = GForce.ToString("0.0") + "G";
            this.machText.text = (this.Velocity.magnitude / 340.29f / GameManager.scaleFactor).ToString("0.00") + "M";

            //Set altitude text. if radar altitude is less than 200(2000m), use that instead
            if(this.RadarAltitude < 200 && this.RadarAltitude > 0)
            {
                this.altitudeText.text = (this.RadarAltitude / GameManager.scaleFactor).ToString("0");
                this.radarAltitudeIndicator.enabled = true;
            } else
            {
                this.altitudeText.text = (this.Altitude / GameManager.scaleFactor).ToString("0");
                this.radarAltitudeIndicator.enabled = false;
            }
             return true;
        }
        public bool PreTickComponent() => true;
        public bool PostTickComponent() => this.PostTick();
    }
}