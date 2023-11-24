using System;
using UnityEngine;
using Assets.Scripts.Ordinance;
using Assets.Scripts.Vehicles;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// The base class for hardpoints. Use <b><see cref="Hardpoint"/></b> or <b><see cref="StaticHardpoint"/></b> to utilize this
    /// </summary>
    public abstract class BaseHardpoint : ComponentBase, ITickableComponent
    {
        public string HardpointName { get => this.hardpointName; set { this.HardpointObject = GameObject.Find(value); this.hardpointName = value; } }
        private string hardpointName;
        public GameObject HardpointObject { get; set; }

        /// <summary>
        /// Weapons attached to this hardpoint
        /// 
        /// <b>PLACEHOLDER TYPE</b>
        /// </summary>
        /// <value>Weapons contained on this pylon</value>
        // TODO: Implement core weapon logic
        public dynamic Attatchment { get; set; }
        
        public BaseHardpoint(VehicleBase vehicle) : base(vehicle)
        {

        }

        bool ITickableComponent.PostTickComponent() => true;
        bool ITickableComponent.PreTickComponent() => true;
    }

    /// <summary>
    /// Class for weapon hardpoints.
    /// <para>A vehicle <b>may</b> have multiple hardpoints in components.</para>
    /// <para>Use <see cref="ComponentManager.GetComponentsOfType{WeaponHardpoint}"/> if you want to gather all weapon pylons for a vehicle.</para>
    /// 
    /// <para>This type is for dynamic hardpoints that <b>can</b> be removed, for static weapons that <b>can not</b> be removed use <see cref="StaticHardpoint"/>.</para>
    /// </summary>
    public class Hardpoint : BaseHardpoint
    {
        public Hardpoint(VehicleBase vehicle) : base(vehicle) { }
        public float MaxAcceptedMass { get => this.maxAcceptedMass; set { if (value <= 0) throw new ArgumentOutOfRangeException("MaxAcceptedMass", value, "MaxAcceptedMass can not be less than or equal to 0"); this.maxAcceptedMass = value; } }
        private float maxAcceptedMass;
        public OrdinanceType AcceptedOrdinanceTypes { get; set; }
        public GuidanceType AcceptedGuidanceTypes { get; set; }

        void UpdateOrdinance()
        {
            if (this.Attatchment != null)
            {
                this.Attatchment.transform.SetParent(this.HardpointObject.transform);
                this.Attatchment.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Class for static weapon hardpoints.
    /// <para>A vehicle <b>may</b> have multiple hardpoints in components.</para>
    /// <para>Use <see cref="ComponentManager.GetComponentsOfType{StaticWeaponHardpoint}"/> if you want to gather all static pylons for a vehicle.</para>
    /// 
    /// <para>This type is for static hardpoints that <b>can not</b> be removed, for dynamic weapons that <b>can</b> be removed use <see cref="Hardpoint"/>.</para>
    /// </summary>
    public class StaticHardpoint : BaseHardpoint
    {
        public StaticHardpoint(VehicleBase vehicle) : base(vehicle) 
        {
            
        }
    }
}
