using System;

namespace Assets.Scripts.Ordinance.Components
{
    public class Warhead : OrdinanceComponentBase
    {
        public Warhead (OrdinanceBase ordinance) : base(ordinance) { }

        public int TNTEqivelent { get => tntEquivelent; set { if (value <= 0) throw new ArgumentOutOfRangeException("TNTequivelent", TNTEqivelent, "THTEquivelent can not be less than or equal to 0!"); tntEquivelent = value; } }
        private int tntEquivelent;
    }
}
