using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{

    /// <summary>
    /// Calculates current state of the vehicle, updating Altitude, RadarAltitude, Velocity, LocalVelocity, LocalAngularVelocity, AngleOfAttack, AngleOfAttackYaw, LocalGForce
    /// </summary>
    public void UpdateState(Rigidbody vehicleBody, Vector3 lastVelocity)
    {
        //Velcoities
        //Vector3 velocity;
        Vector3 localVelocity;
        Vector3 localAngularVelocity;
        //G force
        Vector3 localGForce;


        //Calculate inverse rotation used in some calculations
        Quaternion inverseRotation = Quaternion.Inverse(vehicleBody.rotation);

        //Calculate altitude

        //Calculate Velocity            
        //velocity = vehicleBody.velocity;
        localVelocity = inverseRotation * velocity;
        localAngularVelocity = inverseRotation * vehicleBody.angularVelocity;

        //Calculate AoA
        //angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);
        //angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z);

        //Calculate GForce
        Vector3 acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
        localGForce = inverseRotation * acceleration;
        lastVelocity = velocity;

    }



#region AirDensity
    public static float CalculateAirDensity(float altitude)
    {
        return 1.225f * Mathf.Pow(1 - (0.0000225577f * altitude), 4.256f);
    }
#endregion

#region AngleOfAttack
    /// <summary>
    /// Calculates the angle of attack of the vehicle
    /// </summary>
    /// <param name="localVelocity"></param>
    /// <returns>Vector3</returns>
    public static float CalculateAngleOfAttack(Vector3 localVelocity)
    {
        return Mathf.Atan2(-localVelocity.y, localVelocity.z);
    }

    /// <summary>
    /// Calculates the angle of attack of the vehicle on the yaw axis
    /// </summary>
    /// <param name="localVelocity"></param>
    /// <returns>Vector3</returns>
    public static float CalculateAngleOfAttackYaw(Vector3 localVelocity)
    {
        return Mathf.Atan2(localVelocity.x, localVelocity.z);
    }
#endregion

#region Altitude
    /// <summary>
    /// Calculates the radar altitude of the vehicle, up to 10000m
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns>float</returns>
    public static float CalculateRadarAltitude(GameObject vehicle){
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Terrain");
        Physics.Raycast(vehicle.transform.position, Vector3.down, out hit, 1000);
        return hit.distance;
    }
#endregion

#region Drag 
    /// <summary>
    /// Calculates the drag force using <paramref name="dragCoefficient"/>, <paramref name="currentAirDensity"/>, <paramref name="frontalArea"/> and <paramref name="LocalVelocity"/>
    /// </summary>
    /// <param name="dragCoefficient"></param>
    /// <param name="currentAirDensity"></param>
    /// <param name="frontalArea"></param>
    /// <param name="LocalVelocity"></param>
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
#endregion

#region Lift
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
    #endregion
}
