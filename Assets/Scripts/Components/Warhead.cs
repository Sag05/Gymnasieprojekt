using Assets.Scripts.Ordinance;
using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public abstract class Warhead : ComponentBase
    {
        public Warhead (Entity entity) : base(entity) { }

        public int TNTEqivelent { get => tntEquivelent; set { if (value <= 0) throw new ArgumentOutOfRangeException("TNTequivelent", TNTEqivelent, "THTEquivelent can not be less than or equal to 0!"); tntEquivelent = value; } }
        private int tntEquivelent;
        public string ExplosionFXName { private get; set; }
        private GameObject ExplosionFX;
        public void Detonate() {}
    }

    public class HEWarhead : Warhead
    {
        public HEWarhead(Entity entity) : base(entity) { }
        public float BlastRadius { get => blastRadius; set { if (value <= 0) throw new ArgumentOutOfRangeException("BlastRadius", value, "BlastRadius can not be less than or equal to 0!"); blastRadius = value; } }
        private float blastRadius;


    }
    
    public class HEATWarhead : Warhead
    {
        public HEATWarhead(Entity entity) : base(entity) { }
        public float Penetration { get => penetration; set { if (value <= 0) throw new ArgumentOutOfRangeException("Penetration", value, "Penetration can not be less than or equal to 0!"); penetration = value; } }
        private float penetration;
    }
}
