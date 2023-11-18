using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Vehicles.Components
{
    /// <summary>
    /// A component manager for a vehicle
    /// </summary>
    public class ComponentManager
    {
        public ComponentManager(VehicleBase root)
        {
            this.RootVehicle = root;
            this.Components = new List<ComponentBase>();
        }
        /// <summary>
        /// Root vehicle
        /// </summary>
        public VehicleBase RootVehicle { get; private set; }
        /// <summary>
        /// Contains all components for an aircraft
        /// </summary>
        public ICollection<ComponentBase> Components { get; private set; }
        /// <summary>
        /// Adds a component to the component manager
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(ComponentBase component)
        {
            component.ParentVehicle = this.RootVehicle;
            this.Components.Add(component);
        }
        public void AddComponents(ComponentBase[] components)
        {
            foreach(ComponentBase c in components)
                c.ParentVehicle = this.RootVehicle;
            this.Components.AddRange(components);
        }
        /// <summary>
        /// Returns the first component in the list with the given type
        /// </summary>
        /// <typeparam name="T">Component type to search for</typeparam>
        /// <returns></returns>
        public ComponentBase GetComponentOfType<T>()
        {
            return this.Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }
        /// <summary>
        /// Returns all components in the list with the given type
        /// </summary>
        /// <typeparam name="T">Component type to search for</typeparam>
        /// <returns></returns>

        public ComponentBase[] GetComponentsOfType<T>()
        {
            return (from c in this.Components where c.GetType() == typeof(T) select c).ToArray();
        }
    }
}
