using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Vehicles
{
    public abstract class VehicleBase : Entity
    {
        public VehicleBase() : base()
        {

        }
        
        #region Variables
        /// <summary>
        /// Current controller of the vehicle
        /// </summary>
        public PlayerController Controller { get; set; }

        /// <summary>
        /// The position of the camera of the controller
        /// </summary>
        public GameObject ControllerCameraPosition { get; private set; }
        public GameObject Model { get; set; }

        //Velocity
        public Vector3 Velocity { get => this.EntityBody.velocity; }
        public Vector3 LocalVelocity { get => transform.InverseTransformDirection(EntityBody.velocity); }
        public Vector3 AngularVelocity { get => this.EntityBody.angularVelocity;}
        public Vector3 LocalAngularVelocity { get => this.InverseRotation * this.EntityBody.angularVelocity; }
        public Vector3 LastVelocity { get; private set; }
        public Vector3 Acceleration { get => this.Velocity - this.LastVelocity / Time.fixedDeltaTime; }


        //Altitude
        public float Altitude { get => this.gameObject.transform.position.y; }
        public float RadarAltitude { get; private set; }
        
        //Rotation
        public Quaternion InverseRotation { get => Quaternion.Inverse(this.EntityBody.rotation); }
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
            Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 1000, LayerMask.GetMask("Terrain"));
            this.RadarAltitude = hit.distance;
        }

        public void Start()
        {
            //Set last velocity to zero since we just started and otherwise it will be null and give errors
            this.LastVelocity = Vector3.zero;

            this.ControllerCameraPosition = Utilities.GetChildOf(this.gameObject, "CameraPosition");
        }
    }
}
