using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Ordinance;
using UnityEngine;

namespace Assets.Scripts.Ordinance
{
    public class OrdinanceBase : Entity
    {
        public OrdinanceBase()
        {
            configuration = ConfigurationReader.LoadOrdinance(@".\configs\aircrafts\" + gameObject.name + ".cfg", this);
        }
        public OrdinanceConfig configuration;
        public float Mass { get => mass; set {if (value <= 0) throw new ArgumentOutOfRangeException("Mass", Mass, "Mass can not be less than or equal to 0"); mass = value; } }
        private float mass;
        
        /// <summary>
        /// The type of guidance used by the ordinance
        /// </summary>
        public GuidanceType GuidanceType { get; set; }

        public void Fire()
        {
            
        }
    }
}