using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles.Components
{
    /// <summary>
    /// The base class for weapon hardpoints. Use <b><see cref="WeaponHardpoint" langword=""/></b> or <b><see cref="StaticWeaponHardpoint"/></b> to utilize this
    /// </summary>
    public abstract class BaseWeaponHardpoint : ComponentBase, ITickableComponent
    {
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
    /// <para>For static weapons that can <b>not</b> be removed. Use <see cref="StaticWeaponHardpoint"/>.</para>
    /// </summary>
    public sealed class WeaponHardpoint : BaseWeaponHardpoint
    {
        public WeaponHardpoint(VehicleBase vehicle) : base(vehicle)
        {
        }
    }
    /// <summary>
    /// Class for weapon hardpoints.
    /// <para>A vehicle <b>may</b> have multiple hardpoints in components.</para>
    /// <para>Use <see cref="ComponentManager.GetComponentsOfType{StaticWeaponHardpoint}"/> if you want to gather all weapon pylons for a vehicle.</para>
    /// 
    /// <para>For dynamic weapons that <b>can</b> be removed. Use <see cref="WeaponHardpoint"/>.</para>
    /// </summary>
    public sealed class StaticWeaponHardpoint : BaseWeaponHardpoint
    {
        public StaticWeaponHardpoint(VehicleBase vehicle) : base(vehicle)
        {
        }
    }
}
