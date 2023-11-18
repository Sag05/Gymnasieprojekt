using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Vehicles.Components
{
    public interface ITickableComponent
    {
        public virtual bool PreTickComponent() => true;
        public virtual bool PostTickComponent() => true;
    }
}
