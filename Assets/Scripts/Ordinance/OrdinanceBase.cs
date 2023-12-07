using Assets.Scripts.Components;

namespace Assets.Scripts.Ordinance
{
    public class OrdinanceBase : Entity
    {
        public OrdinanceBase()
        {
            configuration = ConfigurationReader.LoadOrdinance(@".\configs\ordinance\" + gameObject.name + ".cfg", this);
        }
        public OrdinanceConfig configuration;
        
        public void Fire()
        {
            foreach (ComponentBase component in this.configuration.Components)
            {
                if(component is SolidfFuelEngine engine)
                {
                    engine.EngineEnabled = true;
                }
                else if(component is ProximityFuse proxy)
                {
                    proxy.Start();
                } 
                else if (component is TimedFuse timed)
                {
                    timed.Start();
                }
                ///TODO: Implement Guidance
            }
        }
    }
}