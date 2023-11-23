using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Ordinance
{
    public class OrdinanceComponentBase
    {
        public OrdinanceComponentBase(OrdinanceBase ordinance) { this.ParentOrdinance = ordinance; }

        /// <summary>
        /// Parent ordinancebase of the component
        /// </summary>
        public OrdinanceBase ParentOrdinance { get; set; }
    }
}
