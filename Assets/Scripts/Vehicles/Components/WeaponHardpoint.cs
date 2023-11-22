using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Components
{
    /// <summary>
    /// The base class for weapon hardpoints. Use <b><see cref="WeaponHardpoint"/></b> or <b><see cref="StaticWeaponHardpoint"/></b> to utilize this
    /// </summary>
    public abstract class BaseWeaponHardpoint : ComponentBase, ITickableComponent
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
        public dynamic[] Weapons { get; set; }
        public BaseWeaponHardpoint(VehicleBase vehicle) : base(vehicle)
        {
            this.Weapons = new object[0] { };
        }

        bool ITickableComponent.PostTickComponent() => true;
        bool ITickableComponent.PreTickComponent() => true;
    }
    /// <summary>
    /// Class for weapon hardpoints.
    /// <para>A vehicle <b>may</b> have multiple hardpoints in components.</para>
    /// <para>Use <see cref="ComponentManager.GetComponentsOfType{WeaponHardpoint}"/> if you want to gather all weapon pylons for a vehicle.</para>
    /// 
    /// <para>This type is for dynamic hardpoints that <b>can</b> be removed, for static weapons that <b>can not</b> be removed use <see cref="StaticWeaponHardpoint"/>.</para>
    /// </summary>
    public sealed class WeaponHardpoint : BaseWeaponHardpoint
    {
        public WeaponHardpoint(VehicleBase vehicle) : base(vehicle)
        {
        }
        public float MaxAcceptedMass { get => this.maxAcceptedMass; set { if (value <= 0) throw new ArgumentOutOfRangeException("MaxAcceptedMass", value, "MaxAcceptedMass can not be less than or equal to 0"); this.maxAcceptedMass = value; } }
        private float maxAcceptedMass;
        public OrdinanceType AcceptedOrdinanceTypes { get; set; }
        public GuidanceType AcceptedGuidanceTypes { get; set; }
        public OrdinanceBase CurrentOrdinance { get; set; }

        void UpdateOrdinance()
        {
            if (this.CurrentOrdinance is not null)
            {
                this.CurrentOrdinance.transform.SetParent(this.HardpointObject.transform);
                this.CurrentOrdinance.transform.localPosition = Vector3.zero;
                this.CurrentOrdinance.transform.localRotation = Quaternion.identity;
            }
        }
    }
    /// <summary>
    /// Class for static weapon hardpoints.
    /// <para>A vehicle <b>may</b> have multiple hardpoints in components.</para>
    /// <para>Use <see cref="ComponentManager.GetComponentsOfType{StaticWeaponHardpoint}"/> if you want to gather all weapon pylons for a vehicle.</para>
    /// 
    /// <para>This type is for static hardpoints that <b>can not</b> be removed, for dynamic weapons that <b>can</b> be removed use <see cref="WeaponHardpoint"/>.</para>
    /// </summary>
    public sealed class StaticWeaponHardpoint : BaseWeaponHardpoint
    {
        public StaticWeaponHardpoint(VehicleBase vehicle) : base(vehicle)
        {
        }
    }
}
