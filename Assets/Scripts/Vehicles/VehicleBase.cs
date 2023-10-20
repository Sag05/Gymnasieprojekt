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
        /// <summary>
        /// Calculates current state of the vehicle, updating Altitude, RadarAltitude, Velocity, LocalVelocity, LocalAngularVelocity, AngleOfAttack, AngleOfAttackYaw, LocalGForce
        /// </summary>
        public void UpdateState()
        {
            //Calculate inverse rotation used in some calculations
            Quaternion inverseRotation = Quaternion.Inverse(this.VehicleBody.rotation);

            //Calculate altitude
            Altitude = gameObject.transform.position.y;
            RaycastHit hit;
            LayerMask layerMask = LayerMask.GetMask("Terrain");
            Physics.Raycast(transform.position, Vector3.down, out hit, 2000);
            RadarAltitude = hit.distance;

            //Calculate Velocity            
            this.Velocity = this.VehicleBody.velocity;
            this.LocalVelocity = inverseRotation * this.Velocity;
            this.LocalAngularVelocity = inverseRotation * this.VehicleBody.angularVelocity;

            //Calculate AoA
            AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z);
            AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z);
        
            //Calculate GForce
            Vector3 acceleration = (Velocity - lastVelocity) / Time.fixedDeltaTime;
            LocalGForce = inverseRotation * acceleration;
            lastVelocity = Velocity;
        }


        /// <summary>
        /// Calculates the drag force
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateDragForce()
        {
            dragCoefficient = 0.1f;
            return 0.5f * dragCoefficient * currentAirDensity * AircraftConfiguration.FrontalArea * LocalVelocity.sqrMagnitude * -LocalVelocity.normalized;
        }


        /// <summary>
        /// Solves the drag coefficient to match the maximum thrust @ <paramref name="topSpeed"/> speed, taking into accont the <paramref name="airDensity"/> and <paramref name="frontalArea"/>
        /// </summary>
        /// <param name="thrust"></param>
        /// <param name="topSpeed"></param>
        /// <param name="airDensity"></param>
        /// <param name="frontalArea"></param>
        //public void SolveDragCoefficient(float thrust, float topSpeed, float airDensity, float frontalArea)
        //{
        //    this.dragCoefficient = 2 * (thrust / (airDensity * frontalArea * Mathf.Pow(topSpeed, 2)));
        //}

        /// <summary>
        /// Calculates the lift force
        /// </summary>
        /// <param name="liftCoefficient"></param>
        /// <param name="airPreasure"></param>
        /// <param name="wingArea"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public float CalculateLift(float liftCoefficient, float airPreasure, float wingArea, float velocity)
        {
            return 0.5f * liftCoefficient * airPreasure * wingArea * Mathf.Pow(velocity, 2);
        }

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
