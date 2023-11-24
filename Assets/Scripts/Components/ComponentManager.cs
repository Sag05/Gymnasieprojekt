using Assets.Scripts.Ordinance;
using Assets.Scripts.Vehicles;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// A component manager for a vehicle
    /// </summary>
    public class ComponentManager
    {
        public ComponentManager(VehicleBase root)
        {
            this.RootObject = root;
            this.Components = new List<ComponentBase>();
        }
        public ComponentManager(OrdinanceBase root)
        {
            this.RootObject = root;
            this.Components = new List<ComponentBase>();
        }

        /// <summary>
        /// Root vehicle
        /// </summary>
        public dynamic RootObject { get; private set; }
        /// <summary>
        /// Contains all components for an aircraft
        /// </summary>
        public ICollection<ComponentBase> Components { get; private set; }

        /// <summary>
        /// Adds a component to the component manager
        /// <para>Use <see cref="ComponentManager.AddComponents(ComponentBase[])"/> if you want to add multiple components</para>
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(ComponentBase component)
        {
            component.ParentObject = this.RootObject;
            this.Components.Add(component);
        }
        /// <summary>
        /// Adds an array of components to the component manager
        /// <para>Use <see cref="ComponentManager.AddComponent(ComponentBase)"/> if you want to add one component</para>
        /// </summary>
        /// <param name="component"></param>
        public void AddComponents(ComponentBase[] components)
        {
            foreach(ComponentBase c in components)
                c.ParentObject = this.RootObject;
            this.Components.AddRange(components);
        }


        /// <summary>
        /// Returns the first component in the list with the given type
        /// </summary>
        /// <typeparam name="T">Component type to search for</typeparam>
        /// <returns></returns>
        public T GetComponentOfType<T>() where T : ComponentBase
        {
            return (T) this.Components.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// Returns all components in the list with the given type
        /// </summary>
        /// <typeparam name="T">Component type to search for</typeparam>
        /// <returns></returns>
        public T[] GetComponentsOfType<T>() where T : ComponentBase
        {
            return (T[]) (from c in this.Components where c.GetType() == typeof(T) select c).ToArray();
        }

        #region  INCOMPLETE, WILL BE REWORKED
        MultiFunctionDisplay currentDisplay;
        /// <summary>
        /// Loop through current selected sensor of interest
        /// </summary>
        public void LoopSOI()
        { 
            int currentDisplayNumber = 0;
            int i = 0;
            foreach (MultiFunctionDisplay display in GetComponentsOfType<MultiFunctionDisplay>())
            {
                if (display.IsSensorOfInterest)
                {
                    currentDisplayNumber = i;
                    display.IsSensorOfInterest = false;
                }
                
                if (currentDisplayNumber == i++)
                {
                    display.IsSensorOfInterest = true;
                }
                i++;
            }
        }

        /// <summary>
        /// Set current sensor of interest to the given MultifunctionDisplay <paramref name="MFD"/>
        /// </summary>
        /// <param name="MFD"></param>
        public void SetCurrentSensorOfInterest(MultiFunctionDisplay MFD){
            //Set all MFDs as not soi
            foreach (MultiFunctionDisplay display in GetComponentsOfType<MultiFunctionDisplay>())
            {
                if (display.IsSensorOfInterest)
                {
                    display.IsSensorOfInterest = false;
                }
            }
            //Set given sensor as soi
            MFD.IsSensorOfInterest = true;            
        }
        #endregion
    }
}
