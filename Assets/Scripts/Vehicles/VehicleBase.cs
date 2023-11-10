using Assets.Scripts.Vehicles.Components;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml.Serialization;
using Unity.VisualScripting;

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

        public Rigidbody VehicleBody { get; set; }

        //Velocity
        public Vector3 Velocity { get => this.VehicleBody.velocity; }
        public Vector3 LocalVelocity { get; private set; }
        public Vector3 LocalAngularVelocity { get => this.InverseRotation * this.VehicleBody.angularVelocity; }
        public Vector3 LastVelocity { get; private set; }
        public Vector3 Acceleration { get => this.Velocity - this.LastVelocity / Time.fixedDeltaTime; }

        //Altitude
        public float Altitude { get => this.gameObject.transform.position.z; }
        public float RadarAltitude { get; private set; }

        //Angle of attack
        public float AngleOfAttack { get => Mathf.Atan2(-this.LocalVelocity.y, this.LocalVelocity.z); }
        public float AngleOfAttackYaw { get => Mathf.Atan2(this.LocalVelocity.x, this.LocalVelocity.z); }

        public Vector3 LocalGForce { get => this.InverseRotation * this.Acceleration; }
        

        public Quaternion InverseRotation {get => Quaternion.Inverse(this.VehicleBody.rotation);}

        private float currentAirPreassure { get => 101325f * MathF.Exp(-GameManager.gravity * 0.0289644f * this.Altitude / (8.31447f * 288.15f)); }
        private float currentAirDensity { get => this.currentAirPreassure / (287.058f * (273.15f + 15f)); }
        #endregion

        public void UpdateLastVelocity()
        {
            this.LastVelocity = this.Velocity;
        }   

        public void UpdateRadarAltitude(GameObject vehicle){
            RaycastHit hit;
            Physics.Raycast(vehicle.transform.position, Vector3.down, out hit, 1000, LayerMask.GetMask("Terrain"));
            this.RadarAltitude = hit.distance;
        }

        public void Start()
        {
            //Set last velocity to zero since we just started and otherwise it will be null and give errors
            this.LastVelocity = Vector3.zero;
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
