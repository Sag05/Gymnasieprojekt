using System;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Components
{
    internal class Pylon : ComponentBase
    {
        public Pylon(VehicleBase vehicle) : base(vehicle)
        {
        }
        public string GetPylonInfo()
        {
            #region OrdinanceType
            string ordinanceTypes = "";
            //If all ordinance types are accepted, just return "All"
            if (this.AcceptedOrdinanceTypes.HasFlag(OrdinanceType.All))
            {
                ordinanceTypes = "All";
            }
            else //Otherwise, loop through all ordinance types and add them to the string
            {
                foreach (OrdinanceType type in Enum.GetValues(typeof(OrdinanceType)))
                {
                    if (this.AcceptedOrdinanceTypes.HasFlag(type))
                    {
                        ordinanceTypes += type + ", ";
                    }
                }
                //Remove last comma and space
                ordinanceTypes = ordinanceTypes.Remove(ordinanceTypes.Length - 2);
            }
            string guidanceTypes = "";
            #endregion
            #region GuidanceType
            //If all guidance types are accepted, just return "All"
            if (this.AcceptedGuidanceTypes.HasFlag(GuidanceType.All))
            {
                guidanceTypes = "All";
            }
            else //Otherwise, loop through all guidance types and add them to the string
            {
                foreach (GuidanceType type in Enum.GetValues(typeof(GuidanceType)))
                {
                    if (this.AcceptedGuidanceTypes.HasFlag(type))
                    {
                        guidanceTypes += type + ", ";
                    }
                }
                //Remove last comma and space
                guidanceTypes = guidanceTypes.Remove(guidanceTypes.Length - 2);
            }
            #endregion

            return $"Pylon: {this.PylonName}\n" +
                $"Accepted ordinance types: {ordinanceTypes}\n" +
                $"Accepted guidance types: {guidanceTypes}";
        }
        public string PylonName { get => this.pylonName; set { this.PylonObject = GameObject.Find(value); this.pylonName = value; } }
        private string pylonName;
        public GameObject PylonObject { get; set; }
        public float MaxAcceptedMass { get => this.maxAcceptedMass; set { if (value <= 0) throw new ArgumentOutOfRangeException("MaxAcceptedMass", value, "MaxAcceptedMass can not be less than or equal to 0"); this.maxAcceptedMass = value; } }
        private float maxAcceptedMass;
        public OrdinanceType AcceptedOrdinanceTypes { get; set; }
        public GuidanceType AcceptedGuidanceTypes { get; set; } 
        public WeaponHardpoint Hardpoint { get; set; }
    }
}