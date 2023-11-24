using Assets.Scripts.Ordinance;
using System;

namespace Assets.Scripts.Components
{
    public class Warhead : ComponentBase
    {
        public Warhead (OrdinanceBase ordinance) : base(ordinance) { }

        public int TNTEqivelent { get => tntEquivelent; set { if (value <= 0) throw new ArgumentOutOfRangeException("TNTequivelent", TNTEqivelent, "THTEquivelent can not be less than or equal to 0!"); tntEquivelent = value; } }
        private int tntEquivelent;
    }
}
