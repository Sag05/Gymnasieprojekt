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

        public Rigidbody VehicleBody;

        private float dragCoefficient;

        /// <summary>
        /// Solves the drag coefficient to match the maximum thrust @ <paramref name="topSpeed"/> speed
        /// </summary>
        /// <param name="thrust">The thrust provided by engine</param>
        /// <param name="topSpeed">The absolute top speed of the vehicle</param>
        public void SolveDragCoefficient(float thrust, float topSpeed)
        {
            this.dragCoefficient = 2 * (thrust / (topSpeed * topSpeed));
        }

        public void Start()
        {
            this.VehicleBody = gameObject.GetComponent<Rigidbody>();
        }
    }
}
