using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Ordinance
{
    internal class OrdinanceBase
    {
        /// <summary>
        /// The mass of the ordinance in Kg
        /// </summary>
        public float Mass { get; set; }
        /// <summary>
        /// The type of ordinance
        /// </summary>
        public OrdinanceType OrdinanceType { get; set; }
        /// <summary>
        /// The type of guidance used by the ordinance
        /// </summary>
        public GuidanceType GuidanceType { get; set; }
    }
}