using Assets.Scripts.Ordinance;
using Assets.Scripts.Vehicles;

namespace Assets.Scripts.Components
{
    public class ComponentBase
    {
        public ComponentBase(dynamic parent) { this.ParentObject = parent; }
        
        /// <summary>
        /// Parent vehiclebase of the component
        /// </summary>
        public dynamic ParentObject { get; set; }

        /// <summary>
        /// Hit points of the component
        /// </summary>
        public float HitPoints { get; set; }
    }
}
