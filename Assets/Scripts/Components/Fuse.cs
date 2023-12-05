
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public abstract class FuseBase : ComponentBase
    {
        public FuseBase(Entity entity) : base(entity) { }

        public bool Armed { get; set; }
        public Warhead Warhead { get; set; }
        public void Trigger() => this.Warhead.Detonate();
    }

    /// <summary>
    /// Fuse that detonates when it gets within range of something
    /// <b> NOT YET IMPLEMENTED </b>
    /// </summary>
    public class ProximityFuse : FuseBase
    {
        public ProximityFuse(Entity entity) : base(entity) { }
        public float ProximityRange { get; set; }
    }

    /// <summary>
    /// Fuse that detonates after a set amount of time
    /// <b> NOT YET IMPLEMENTED </b>
    /// </summary>
    public class TimedFuse : FuseBase
    {
        public TimedFuse(Entity entity) : base(entity) { }
        public float FuseTime { get; set; }
    }

    /// <summary>
    /// Fuse that detonates when it hits something
    /// </summary>
    public class ContactFuse : FuseBase
    {
        public ContactFuse(Entity entity) : base(entity) { entity1 = entity;}
        private Entity entity1;
        public float FuseDelay { get; set; }


        Vector3 lastPosition = Vector3.zero;
        public void Update()
        {
            Vector3 rotation = this.entity1.transform.position - lastPosition;
            Physics.Raycast(this.entity1.transform.position, rotation, out RaycastHit hit);
            if(hit.collider is not null)
            {
                this.Warhead.Detonate();
            }
        }
    }
}