using Assets.Scripts.Vehicles.Components;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml.Serialization;

namespace Assets.Scripts.Vehicles
{
    public abstract class VehicleBase : MonoBehaviour
    {
        public VehicleBase() : base()
        {
            this.VehicleComponents = new List<ComponentBase>();
        }
        #region Variables
        public float Throttle{ get; set; }

        //public VehicleConfigurationBase VehicleConfiguration { get; set; }
        public IList<ComponentBase> VehicleComponents { get; set; }

        //public float Mass { get => VehicleConfiguration.Mass; }
        public Rigidbody VehicleBody { get; set; }

        //Velocity
        public Vector3 Velocity { get; set; }
        public Vector3 LocalVelocity { get; set; }
        public Vector3 LocalAngularVelocity { get; private set; }
        private Vector3 lastVelocity;

        //Altitude
        public float Altitude { get; private set; }
        public float RadarAltitude { get; private set; }

        //Angle of attack
        public float AngleOfAttack { get; set; }
        public float AngleOfAttackYaw { get; set; }

        public Vector3 LocalGForce { get; set; }
        private float dragCoefficient;


        private float currentAirPreassure { get => 101325f * MathF.Exp(-GameManager.gravity * 0.0289644f * this.Altitude / (8.31447f * 288.15f)); }
        private float currentAirDensity { get => this.currentAirPreassure / (287.058f * (273.15f + 15f)); }
        #endregion

        /*


        public void SaveConfigFile(string path)
        {
            File.WriteAllText(path, AircraftConfiguration.SaveToJSON(this.AircraftConfiguration));
        }
        public void ReadConfigFile(string path)
        {
            this.AircraftConfiguration = AircraftConfiguration.CreateFromJSON(File.ReadAllText(path));
        }
        `*/

        public void Start()
        {
            if (this.gameObject.GetComponent<Rigidbody>() is not null)
            {
                this.VehicleBody = this.gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                this.VehicleBody = this.gameObject.AddComponent<Rigidbody>();
            }
        }
    }
}
