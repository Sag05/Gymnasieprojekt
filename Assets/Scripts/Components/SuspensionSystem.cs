using Assets.Scripts.Vehicles;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Components
{
    internal class SuspensionManager : ComponentBase, ITickableComponent
    {
        public SuspensionManager(VehicleBase vehicle) : base(vehicle)
        {
            parentVehicle = vehicle;
        }

        public List<Suspension> Suspensions { get; set; }
        private VehicleBase parentVehicle;

        public void Start(){
            foreach (Suspension suspension in this.Suspensions)
            {
                suspension.Start();
            }
        }

        public bool PreTickComponent() => true;
        public bool PostTickComponent()
        {
            foreach (Suspension suspension in this.Suspensions)
            {
                //if no wheel or suspension object is found, return true to prevent errors
                if(suspension.ConnectedWheelObject == null || suspension.SuspensionObject == null) return true;
                
                
                if(Physics.Raycast(suspension.WheelBottomPosition, Vector3.down, out RaycastHit hit, 0.5f, LayerMask.GetMask("Terrain")))
                {
                    
                }

                return true;
            }
            return true;
        }

    }

    internal class Suspension : ComponentBase
    {
        public Suspension(VehicleBase vehicle) : base(vehicle)
        {
            parentVehicle = vehicle;
            if(parentVehicle.VehicleComponents.GetComponentOfType<SuspensionManager>()is null) parentVehicle.VehicleComponents.AddComponent(new SuspensionManager(vehicle));
        }
        private VehicleBase parentVehicle;

        /// <summary>
        /// Maximumm movement of the suspension, from default position
        /// </summary>
        public float MaxSuspensionMovement { get => maxSuspensionMovement; set { if (value == 0) throw new ArgumentOutOfRangeException("MaxSuspensionMovement", value, "MaxSuspensionMovement can not be 0"); this.maxSuspensionMovement = Mathf.Abs(value * GameManager.scaleFactor); } }
        private float maxSuspensionMovement;
        /// <summary>
        /// Force where the suspension is fully compressed, measured in Newton
        /// </summary>
        public float MaxSuspensionForce { get => maxSuspensionForce; set { if (value == 0) throw new ArgumentOutOfRangeException("MaxSuspensionForce", value, "MaxSuspensionForce can not be 0"); this.maxSuspensionForce = Mathf.Abs(value * GameManager.scaleFactor); } }
        private float maxSuspensionForce;

        public string SuspensionModelName { get; set; }
        public GameObject SuspensionObject { get; private set;}
        
        //Connected wheel
        public string ConnectedWheelModelName { get; set; }
        public GameObject ConnectedWheelObject { get; private set;}
        public SphereCollider ConnectedWheelCollider { get; private set;}
        public Vector3 WheelBottomPosition { get; private set; }

        public void Start()
        {
            this.ConnectedWheelObject = Utilities.GetAnyChildOf(parentVehicle.gameObject, this.ConnectedWheelModelName);
            this.ConnectedWheelCollider = this.ConnectedWheelObject.AddComponent<SphereCollider>();
            this.WheelBottomPosition = new Vector3(
                this.ConnectedWheelObject.transform.position.x, 
                this.ConnectedWheelObject.transform.position.y - this.ConnectedWheelCollider.radius, 
                this.ConnectedWheelObject.transform.position.z);
            
            this.SuspensionObject = Utilities.GetAnyChildOf(parentVehicle.gameObject, this.SuspensionModelName);
        }
        /* To be removed
        public void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Vehicle"))
            {
                float totalVerticalVehicleForce = ParentVehicle.VehicleBody.GetAccumulatedForce().y;

                float movementCoefficient = totalVerticalVehicleForce / collision.contactCount / MaxSuspensionForce;
                Debug.Log("SuspeensionMovementCoefficient: " + movementCoefficient);
                Vector3 newSuspensionPosition = suspensionObject.transform.localPosition;
                newSuspensionPosition.y += maxSuspensionMovement * movementCoefficient;

                this.suspensionObject.transform.position = newSuspensionPosition;
                Debug.Log("New suspension position: " + newSuspensionPosition);
            }
        }
        */

    }
}