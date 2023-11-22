using System;
using Assets.Scripts;
using Assets.Scripts.Ordinance;
using UnityEngine;

public class OrdinanceBase : MonoBehaviour
{
    public OrdinanceConfig configuration;
    public string Name { get; set; }
    public float Mass { get => mass; set {if (value <= 0) throw new ArgumentOutOfRangeException("Mass", Mass, "Mass can not be less than or equal to 0"); mass = value; } }
    private float mass;
    public void FireWeapon()
    {

    }
}

public class Rocket : OrdinanceBase
{

}

public class Bomb : OrdinanceBase
{

}


public class Pod : OrdinanceBase
{

}

public class Gun : OrdinanceBase
{

}

public class Dispenser : OrdinanceBase
{

}