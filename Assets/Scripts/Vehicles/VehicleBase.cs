using Assets.Scripts.Vehicles.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Vehicles
{
    public abstract class VehicleBase : MonoBehaviour
    {
        public VehicleConfigurationBase VehicleConfiguration { get; set; }

        /// <summary>
        /// List of all components of the vehicle
        /// </summary>
        public IList<ComponentBase> VehicleComponents { get; set; }

        /// <summary>
        /// Mass of the vehicle
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Rigidbody of the vehicle
        /// </summary>
        public Rigidbody VehicleBody { get; set; }

        private float dragCoefficient;

        /* Less accurate drag coefficient solver
        /// <summary>
        /// Solves the drag coefficient to match the maximum thrust @ <paramref name="topSpeed"/> speed
        /// </summary>
        /// <param name="thrust">The thrust provided by engine</param>
        /// <param name="topSpeed">The absolute top speed of the vehicle</param>
        public void SolveDragCoefficient(float thrust, float topSpeed)
        {
            this.dragCoefficient = 2 * (thrust / (topSpeed * topSpeed));
        }
        */

        /// <summary>
        /// Solves the drag coefficient to match the maximum thrust @ <paramref name="topSpeed"/> speed, taking into accont the air density and frontal area
        /// </summary>
        /// <param name="thrust"></param>
        /// <param name="topSpeed"></param>
        /// <param name="airDensity"></param>
        /// <param name="frontalArea"></param>
        public void SolveDragCoefficient(float thrust, float topSpeed, float airDensity, float frontalArea)
        {
            this.dragCoefficient = 2 * (thrust / (airDensity * frontalArea * Mathf.Pow(topSpeed, 2)));
        }

        /// <summary>
        /// Calculates the drag force 
        /// </summary>
        /// <param name="frontalArea"></param>
        /// <param name="airDensity"></param>
        /// <param name="speed"></param>
        public float CalculateDragForce(float airDensity, float frontalArea, float speed)
        {
            return 0.5f * dragCoefficient * airDensity * frontalArea * Mathf.Pow(speed, 2);
        }





        public void Start()
        {
            //Apply VehicleConfiguration
            this.Mass = this.VehicleConfiguration.Mass;


            if (this.gameObject.GetComponent<Rigidbody>() is not null)
            {
                this.VehicleBody = this.gameObject.GetComponent<Rigidbody>();
            }
            else
            {
                this.VehicleBody = this.gameObject.AddComponent<Rigidbody>();
            }

            //Apply mass to rigidbody
            this.VehicleBody.mass = this.Mass;
        }
    }
}
