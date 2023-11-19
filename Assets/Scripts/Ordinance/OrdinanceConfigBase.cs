using System;

namespace Assets.Scripts.Ordinance
{
    internal class OrdinanceConfigBase
    {
        /// <summary>
        /// The mass of the ordinance in Kg
        /// </summary>
        public float Mass { get => mass; set { if (value <= 0) throw new ArgumentOutOfRangeException("Mass", value, "Mass can not be less than or equal to 0"); this.mass = value * GameManager.scaleFactor;}} 
        private float mass;
        /// <summary>
        /// The type of ordinance
        /// </summary>
        public OrdinanceType OrdinanceType { get; set; }
        /// <summary>
        /// The type of guidance used by the ordinance
        /// </summary>
        public GuidanceType GuidanceType { get; set; }
        /// <summary>
        /// The name of the model
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// The additional drag of the ordinance
        /// </summary>
        public float AdditionalDragCoefficient { get; set; }
    }
    
    internal class TargetingPodConfig : OrdinanceConfigBase
    {
        /// <summary>
        /// Maximum zoom of the pod
        /// </summary>
        public float MaxZoom { get => maxZoom; set { if(value < 1) throw new ArgumentOutOfRangeException("MaxZoom", value, "MaxZoom can not be less than 1"); this.maxZoom = value; } }
        private float maxZoom;
        /// <summary>
        /// Maximum horizontal angle of the pod
        /// </summary>
        public float MaximumHorizontalAngle { get => maximumHorizontalAngle; set { if(value < 0) throw new ArgumentOutOfRangeException("MaximumHorizontalAngle", value, "MaximumHorizontalAngle can not be less than 0"); this.maximumHorizontalAngle = value; } }
        private float maximumHorizontalAngle;
        /// <summary>
        /// Maximum vertical angle of the pod
        /// </summary>
        public float MaximumVerticalAngle { get => maximumVerticalAngle; set { if(value < 0) throw new ArgumentOutOfRangeException("MaximumVerticalAngle", value, "MaximumVerticalAngle can not be less than 0"); this.maximumVerticalAngle = value; } }
        private float maximumVerticalAngle;
    }
}