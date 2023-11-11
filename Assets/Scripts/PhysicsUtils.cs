using Assets.Scripts;
using System;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{
    /* Variables that might be used at a later point
        //float dynamicAirViscosity = 1.7894e-5f * Mathf.Pow(1 + 120f / (airDensity + 120f) * Mathf.Pow(LocalVelocity.magnitude, 1.5f), -1);
        //float reynoldsNumber = currentAirDensity * LocalVelocity.magnitude * frontalArea / dynamicAirViscosity;
    */

    #region AircaftSteering
    private static float CalculateSteering(float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float deltaAcceleration = acceleration * Time.fixedDeltaTime;
        return Mathf.Clamp(error, -deltaAcceleration, deltaAcceleration);
    }


    public static DoubleVector3 Steering(Vector3 localVelocity, Vector3 localAngularVelocity, AnimationCurve steeringCurve, 
        Vector3 controlInput, Vector3 turnSpeed, Vector3 turnAcceleration, float pitchGLimit, float gLimit)
    {
        DoubleVector3 result = new DoubleVector3();
        float speed = Mathf.Max(0, localVelocity.z);
        float steeringPower = steeringCurve.Evaluate(speed);

        float gForceScale = CalculateGLimiter(controlInput, turnSpeed * Mathf.Deg2Rad * steeringPower, localVelocity, pitchGLimit, gLimit);

        Vector3 targetAngularVelocity = Vector3.Scale(controlInput, turnSpeed * steeringPower * gForceScale);
        Vector3 angularVelocity = localAngularVelocity * Mathf.Rad2Deg;

        Vector3 correction = new Vector3(
            CalculateSteering(angularVelocity.x, targetAngularVelocity.x, turnAcceleration.x * steeringPower),
            CalculateSteering(angularVelocity.y, targetAngularVelocity.y, turnAcceleration.y * steeringPower),
            CalculateSteering(angularVelocity.z, targetAngularVelocity.z, turnAcceleration.z * steeringPower));
        result.vector1 = correction * Mathf.Deg2Rad;


        Debug.Log("Correction: " + correction.ToString("0.0") + 
            "\nGForceScale: " + gForceScale.ToString("0.0") + 
            "\nSteeringPower: " + steeringPower.ToString("0.0" + 
            "\nSpeed: " + speed.ToString("0.0") + "\nInput: " + controlInput.ToString("0.0") +
            "\nOutput: " + result.vector1.ToString("0.0")));
        
        //Effective input, used for animations
        #region EffectiveInput

        Vector3 correctionInput = new Vector3(
            Mathf.Clamp((targetAngularVelocity.x - angularVelocity.x)/turnAcceleration.x, -1, 1),
            Mathf.Clamp((targetAngularVelocity.y - angularVelocity.y)/turnAcceleration.y, -1, 1),
            Mathf.Clamp((targetAngularVelocity.z - angularVelocity.z)/turnAcceleration.z, -1, 1));

        Vector3 effectiveInput = (correctionInput + controlInput) * gForceScale;

        Vector3 EffectiveInput = new Vector3(
            Mathf.Clamp(effectiveInput.x, -1, 1),
            Mathf.Clamp(effectiveInput.y, -1, 1),
            Mathf.Clamp(effectiveInput.z, -1, 1));

        result.vector2 = EffectiveInput;
        #endregion

        return result;
    }

    private static float CalculateGLimiter(Vector3 controlinput, Vector3 maxAngularVelocity, Vector3 localVelocity, float pitchGLimit, float gLimit)
    {
        Vector3 limit = Utilities.Secale6(controlinput.normalized,
            gLimit, pitchGLimit,
            gLimit, gLimit,
            gLimit, gLimit);
        Vector3 maxGForce = CalculateLocalGForce(Vector3.Scale(controlinput.normalized, maxAngularVelocity), localVelocity);

        if(maxGForce.magnitude > limit.magnitude)
        {
            return limit.magnitude / maxGForce.magnitude;
        }
        return 1f;
    }
    #endregion

    #region GForce
    /// <summary>
    /// calculates the current G force 
    /// </summary>
    /// <param name="angularVelocity"></param>
    /// <param name="velocity"></param>
    /// <returns></returns>
    public static Vector3 CalculateLocalGForce(Vector3 angularVelocity, Vector3 velocity)
    {
        //get => Vector3.Cross(AngularVelocity, Velocity); 
        return Vector3.Cross(angularVelocity, velocity);

    }
    #endregion

    #region AngleOfAttack
    /// <summary>
    /// Calculates the angle of attack in the vertical axis
    /// </summary>
    /// <param name="localVelocity"></param>
    /// <returns></returns>
    public static float CalculateAngleOfAttack(Vector3 localVelocity) 
    { 
        //get => Mathf.Atan2(-this.LocalVelocity.y, this.LocalVelocity.z); 
        return MathF.Atan2(-localVelocity.y, localVelocity.z);
    }
    

    /// <summary>
    /// Calculates the angle of attack in the yaw axis
    /// </summary>
    /// <param name="localVelocit"></param>
    /// <returns></returns>
    public static float CalculateAngleOfAttackYaw(Vector3 localVelocit) 
    { 
        //get => Mathf.Atan2(this.LocalVelocity.x, this.LocalVelocity.z); 
        return MathF.Atan2(localVelocit.x, localVelocit.z);
    }
    #endregion

    #region AirDensity
    /// <summary>
    /// //Calculates the air density at <paramref name="altitude"/>
    /// </summary>
    /// <param name="altitude"></param>
    /// <returns></returns>
    public static float CalculateAirDensity(float altitude)
    {
        return 1.225f * Mathf.Pow(1 - (0.0000225577f * altitude), 4.256f);
    }
    #endregion

    #region AirPreassure
    /// <summary>
    /// Calculates the air preassure at <paramref name="altitude"/>
    /// </summary>
    /// <param name="altitude"></param>
    /// <returns></returns>
    public static float CalculateAirPreassure(float altitude)
    {
        return 101325f * MathF.Exp(-GameManager.gravity * 0.0289644f * altitude / (8.31447f * 288.15f));
    }
    #endregion

    #region Drag 
    /// <summary>
    /// Calculates the drag force using <paramref name="altitude"/>, <paramref name="frontalArea"/> and <paramref name="LocalVelocity"/>
    /// </summary>
    /// <param name="altitude"></param>
    /// <param name="frontalArea"></param>
    /// <param name="LocalVelocity"></param>
    /// <returns></returns>
    public static Vector3 CalculateDragForce(float altitude, float frontalArea, Vector3 LocalVelocity)
    {
        //dragCoefficient = 2 * (thrust / (airDensity * frontalArea * Mathf.Pow(topSpeed, 2)));
        //float dragCoefficient = 0.1f;
        float currentAirDensity = CalculateAirDensity(altitude);
        float dragCoefficient = LocalVelocity.magnitude * 0.5f * currentAirDensity;

        return 0.5f * dragCoefficient * currentAirDensity * frontalArea * LocalVelocity.sqrMagnitude * -LocalVelocity.normalized;
    }

    /* Separate function for solving drag coefficient, not needed
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
    */
    #endregion

    #region Lift
    /// <summary>
    /// Calculates the lift force, including induced drag and air preassure
    /// </summary>
    /// <param name="liftCurve"></param>
    /// <param name="inducedDragCurve"></param>
    /// <param name="airPreasureCoefficient"></param>
    /// <param name="localVelocity"></param>
    /// <param name="angleOfAttack"></param>
    /// <param name="liftPower"></param>
    /// <returns>Vector3 lift</returns>
    private static Vector3 CalculateLift(AnimationCurve liftCurve, AnimationCurve inducedDragCurve, Vector3 rightAxis, float airPreasureCoefficient, Vector3 localVelocity, float angleOfAttack, float liftPower)
    {
        //Project velocity onto plane
        Vector2 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);

        //Coefficiient vaies with aoa
        float liftCoefficient = liftCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        float liftForce = liftVelocity.sqrMagnitude * liftCoefficient * liftPower;

        //Lift is perpendicular to velocity
        Vector3 lift = Vector3.Cross(liftVelocity, rightAxis) * liftForce;

        //incued drag
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 inducedDrag = -liftVelocity.normalized * liftVelocity.sqrMagnitude * dragForce * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));

        //Air preassure
        Vector3 airPreassure = -liftVelocity.normalized * airPreasureCoefficient * localVelocity.sqrMagnitude;

        return lift + inducedDrag + airPreassure;
    }

    /// <summary>
    /// Calculates the total lift force, in the vertical and yaw axis
    /// </summary>
    /// <param name="aOACurve"></param>
    /// <param name="inducedDragCurve"></param>
    /// <param name="airPreasureCoefficient"></param>
    /// <param name="localVelocity"></param>
    /// <param name="angleOfAttack"></param>
    /// <param name="liftPower"></param>
    /// <param name="rudderAOACurve"></param>
    /// <param name="inducedRudderDragCurve"></param>
    /// <param name="angleOfAttackYaw"></param>
    /// <param name="rudderLiftPower"></param>
    /// <returns>Vector3 lift</returns>
    public static Vector3 CalculatelTotalLift(Vector3 localVelocity, AnimationCurve aOACurve, AnimationCurve inducedDragCurve, float airPreasureCoefficient, float liftPower, AnimationCurve rudderAOACurve, AnimationCurve inducedRudderDragCurve, float rudderLiftPower)
    {
        Vector3 verticalLift = CalculateLift(aOACurve, inducedDragCurve, Vector3.right, airPreasureCoefficient, localVelocity, CalculateAngleOfAttack(localVelocity), liftPower);
        Vector3 rudderLift = CalculateLift(rudderAOACurve, inducedDragCurve, Vector3.up, airPreasureCoefficient, localVelocity, CalculateAngleOfAttackYaw(localVelocity), rudderLiftPower);
        return verticalLift + rudderLift;
    }
    #endregion
}
