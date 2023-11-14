using Assets.Scripts.Vehicles.Components;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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
        public Vector3 LocalVelocity { get => transform.InverseTransformDirection(VehicleBody.velocity); }
        public Vector3 AngularVelocity { get => this.VehicleBody.angularVelocity;}
        public Vector3 LocalAngularVelocity { get => this.InverseRotation * this.VehicleBody.angularVelocity; }
        public Vector3 LastVelocity { get; private set; }
        public Vector3 Acceleration { get => this.Velocity - this.LastVelocity / Time.fixedDeltaTime; }


        //Altitude
        public float Altitude { get => this.gameObject.transform.position.y; }
        public float RadarAltitude { get; private set; }
        
        //Rotation
        public Quaternion InverseRotation {get => Quaternion.Inverse(this.VehicleBody.rotation);}
        #endregion

        public void PreUpdate()
        {
            UpdateRadarAltitude();
        }

        public void PostUpdate()
        {
            this.LastVelocity = this.Velocity;
        }   

        public void UpdateRadarAltitude(){
            RaycastHit hit;
            Physics.Raycast(this.transform.position, Vector3.down, out hit, 1000, LayerMask.GetMask("Terrain"));
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
