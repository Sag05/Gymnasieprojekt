using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour
{

    /// <summary>
    /// Calculates current state of the vehicle, updating Altitude, RadarAltitude, Velocity, LocalVelocity, LocalAngularVelocity, AngleOfAttack, AngleOfAttackYaw, LocalGForce
    /// </summary>
    public void UpdateState()
    {
        //Calculate inverse rotation used in some calculations
        Quaternion inverseRotation = Quaternion.Inverse(this.VehicleBody.rotation);

        //Calculate altitude
        Altitude = gameObject.transform.position.y;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Physics.Raycast(transform.position, Vector3.down, out hit, 2000);
        RadarAltitude = hit.distance;

        //Calculate Velocity            
        this.Velocity = this.VehicleBody.velocity;
        this.LocalVelocity = inverseRotation * this.Velocity;
        this.LocalAngularVelocity = inverseRotation * this.VehicleBody.angularVelocity;

        //Calculate AoA
        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z);
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z);

        //Calculate GForce
        Vector3 acceleration = (Velocity - lastVelocity) / Time.fixedDeltaTime;
        LocalGForce = inverseRotation * acceleration;
        lastVelocity = Velocity;
    }


    /// <summary>
    /// Calculates the drag force
    /// </summary>
    /// <returns></returns>
    public Vector3 CalculateDragForce(float dragCoefficient, float currentAirDensity, float frontalArea, Vector3 LocalVelocity)
    {
        //float dragCoefficient = 0.1f;
        return 0.5f * dragCoefficient * currentAirDensity * frontalArea * LocalVelocity.sqrMagnitude * -LocalVelocity.normalized;
    }


    /// <summary>
    /// Solves the drag coefficient to match the maximum thrust @ <paramref name="topSpeed"/> speed, taking into accont the <paramref name="airDensity"/> and <paramref name="frontalArea"/>
    /// </summary>
    /// <param name="thrust"></param>
    /// <param name="topSpeed"></param>
    /// <param name="airDensity"></param>
    /// <param name="frontalArea"></param>
    public float SolveDragCoefficient(float thrust, float topSpeed, float airDensity, float frontalArea)
    {
        return 2 * (thrust / (airDensity * frontalArea * Mathf.Pow(topSpeed, 2)));
    }

    /// <summary>
    /// Calculates the lift force
    /// </summary>
    /// <param name="liftCoefficient"></param>
    /// <param name="airPreasure"></param>
    /// <param name="wingArea"></param>
    /// <param name="velocity"></param>
    /// <returns></returns>
    public float CalculateLift(float liftCoefficient, float airPreasure, float wingArea, float velocity)
    {
        return 0.5f * liftCoefficient * airPreasure * wingArea * Mathf.Pow(velocity, 2);
    }
}
