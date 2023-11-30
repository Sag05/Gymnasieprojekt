using Assets.Scripts;
using Assets.Scripts.Components;
using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class SolidfFuelEngine : ComponentBase, ITickableComponent
    {
        public SolidfFuelEngine(Entity entity) : base(entity) { }

        /// <summary>
        /// The maximum thrust of the engine, measured in Newtons
        /// </summary>
        public AnimationCurve ThrustCurve { get; set; }
        
        /// <summary>
        /// The thrust of the engine, measured in Newtons
        /// </summary>
        public float Thrust { get; private set; }
        
        /// <summary>
        /// Fuel of the engine, measured in Kg
        /// </summary>
        public float Fuel { get; set; }
        
        /// <summary>
        /// Fuel consumption of the engine, measured in Kg/s
        /// </summary>
        public float FuelConsumption { get; set; }

        /// <summary>
        /// Total time the engine has been burning, measured in seconds
        /// </summary>
        public float BurnTime { get; set; }
        public bool EngineEnabled { get; set; }

        public bool PreTickComponent() 
        {
            this.Thrust = 0;
            if(EngineEnabled && this.Fuel > 0)
            {
                this.Thrust = this.ThrustCurve.Evaluate(this.BurnTime);
                this.Fuel -= this.FuelConsumption * Time.fixedDeltaTime;
                this.BurnTime += Time.fixedDeltaTime;    
            }
            else if(this.Fuel <= 0)
            {
                Debug.Log("Solid fuel engine out of fuel!");
                this.EngineEnabled = false;
            }
            
            return true;
        } 

        public bool PostTickComponent() => true;
    }
}