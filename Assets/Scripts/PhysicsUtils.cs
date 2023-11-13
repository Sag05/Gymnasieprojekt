using Assets.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{
    /* Variables that might be used at a later point
        //float dynamicAirViscosity = 1.7894e-5f * Mathf.Pow(1 + 120f / (airDensity + 120f) * Mathf.Pow(LocalVelocity.magnitude, 1.5f), -1);
        //float reynoldsNumber = currentAirDensity * LocalVelocity.magnitude * frontalArea / dynamicAirViscosity;
    */

    #region AircaftSteering
    /// <summary>
    /// Calculates the steering, using <paramref name="angularVelocity"/>, <paramref name="targetVelocity"/> and <paramref name="acceleration"/>
    /// </summary>
    /// <param name="angularVelocity"></param>
    /// <param name="targetVelocity"></param>
    /// <param name="acceleration"></param>
    /// <returns></returns>
    private static float CalculateSteering(float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float deltaAcceleration = acceleration * Time.fixedDeltaTime;
        return Mathf.Clamp(error, -deltaAcceleration, deltaAcceleration);
    }

    /// <summary>
    /// Calculates the steering, using <paramref name="localVelocity"/>, <paramref name="localAngularVelocity"/>, 
    /// <paramref name="steeringCurve"/>, <paramref name="controlInput"/>, <paramref name="turnSpeed"/>, 
    /// <paramref name="turnAcceleration"/>
    /// and limits it using <paramref name="pitchGLimit"/> and <paramref name="gLimit"/>
    /// </summary>
    /// <param name="localVelocity"></param>
    /// <param name="localAngularVelocity"></param>
    /// <param name="steeringCurve"></param>
    /// <param name="controlInput"></param>
    /// <param name="turnSpeed"></param>
    /// <param name="turnAcceleration"></param>
    /// <param name="pitchGLimit"></param>
    /// <param name="gLimit"></param>
    /// <returns></returns>
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

        /* Debug 
        Debug.Log("Correction: " + correction.ToString("0.0") + 
            "\nGForceScale: " + gForceScale.ToString("0.0") + 
            "\nSteeringPower: " + steeringPower.ToString("0.0" + 
            "\nSpeed: " + speed.ToString("0.0") + "\nInput: " + controlInput.ToString("0.0") +
            "\nOutput: " + result.vector1.ToString("0.0")));
        */

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

    /// <summary>
    /// Calculates the G limiter, using the <paramref name="controlinput"/>, <paramref name="maxAngularVelocity"/>, <paramref name="localVelocity"/>, <paramref name="pitchGLimit"/> and <paramref name="gLimit"/>
    /// </summary>
    /// <param name="controlinput"></param>
    /// <param name="maxAngularVelocity"></param>
    /// <param name="localVelocity"></param>
    /// <param name="pitchGLimit"></param>
    /// <param name="gLimit"></param>
    /// <returns></returns>
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
    /// calculates the current G force, using local velocity and angular velocity
    /// </summary>
    /// <param name="localAngularVelocity"></param>
    /// <param name="localVelocity"></param>
    /// <returns></returns>
    public static Vector3 CalculateLocalGForce(Vector3 localAngularVelocity, Vector3 localVelocity)
    {
        //get => Vector3.Cross(AngularVelocity, Velocity); 
        return Vector3.Cross(localAngularVelocity, localVelocity);
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
    public static float CalculateAngleOfAttackYaw(Vector3 localVelocity) 
    { 
        //get => Mathf.Atan2(this.LocalVelocity.x, this.LocalVelocity.z); 
        return MathF.Atan2(localVelocity.x, localVelocity.z);
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
        float currentAirDensity = CalculateAirDensity(altitude);
        float dragCoefficient = LocalVelocity.magnitude * 0.5f * currentAirDensity;

        return 0.5f * dragCoefficient * currentAirDensity * frontalArea * LocalVelocity.sqrMagnitude * -LocalVelocity.normalized;
    }

    /* Separate function for solving drag coefficient, not used
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

    #region newLift
    #region Referance
    //https://www.phind.com/search?cache=itn0w9vl2cr6hia4ix2rc2gm
    //private static Vector3 CalculateLift(TextMeshProUGUI debugText, AnimationCurve liftCurve, AnimationCurve inducedDragCurve, Vector3 rightAxis,
    //float airPreasureCoefficient, Vector3 localVelocity, float angleOfAttack, float liftPower, Vector3 inducedDragCoefficient, float wingSpan, float wingArea)
    //{
    //    // Move localVelocity to local-space and solve for AoA with trigonometry
    //    var localVelocity = transform.InverseTransformDirection(rb.velocity);
    //    var angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

    //    // Simplify your equations with Lifting Line Theory
    //    var aspectRatio = (wingSpan * wingSpan) / wingArea;
    //    var inducedLift = angleOfAttack * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI;
    //    var inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);

    //    // Compute the lift direction by crossing the normalized velocity vector with the aircraft's lateral direction and apply drag opposite to velocity
    //    var dragDirection = -rb.velocity.normalized;
    //    var liftDirection = Vector3.Cross(dragDirection, transform.right);

    //    // Lift + Drag = Total Force
    //    rb.AddForce(liftDirection * lift - dragDirection * drag);
    //}
    #endregion

    public static Vector3 CalculateLift(TextMeshProUGUI debugText, Vector3 localVelocity, Vector3 velocity, Transform transform, float altitude, float wingSpan, float wingArea, float liftPower, AnimationCurve altitudeEffectiveness)
    {
        wingSpan *= GameManager.scaleFactor;
        wingArea *= GameManager.scaleFactor;

        float aspectRatio = (wingSpan * wingSpan) / wingArea;
        float inducedLift = CalculateAngleOfAttack(localVelocity) * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI; //* liftPower;
        float inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);

        //Vector3 dragDirection = -transform.forward;
        //Vector3 liftDirection = transform.up;
        Vector3 dragDirection = -velocity.normalized;
        Vector3 liftDirection = velocity.normalized;
        
        Vector3 lift = liftDirection * inducedLift - dragDirection * inducedDrag;
        debugText.text = "lift: " + lift + "\n  inducedLift: " + inducedLift + "\n  inducedDrag: " + inducedDrag;
        return lift;
    }

    #endregion

    // OLD LIFT
    #region Lift
    /// <summary>
    /// Calculate Lift 
    /// </summary>
    /// <param name="debugText"></param>
    /// <param name="liftCurve"></param>
    /// <param name="inducedDragCurve"></param>
    /// <param name="rightAxis"></param>
    /// <param name="airPreasureCoefficient"></param>
    /// <param name="localVelocity"></param>
    /// <param name="angleOfAttack"></param>
    /// <param name="liftPower"></param>
    /// <returns></returns>

    /* private static Vector3 CalculateLiftT(TextMeshProUGUI debugText, AnimationCurve liftCurve, AnimationCurve inducedDragCurve, Vector3 rightAxis,
     float airPreasureCoefficient, Vector3 localVelocity, float angleOfAttack, float liftPower)
     {
         //Project velocity onto plane
         Vector2 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);

         //Calculate flight path angle
         float flightPathAngle = Vector3.Angle(localVelocity, Vector3.forward);

         //Coefficiient varies with aoa
         float liftCoefficient = liftCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
         float liftForce = liftVelocity.sqrMagnitude * liftCoefficient * liftPower * Mathf.Cos(flightPathAngle * Mathf.Deg2Rad);

         //Lift is perpendicular to velocity
         Vector3 lift = Vector3.Cross(liftVelocity, rightAxis) * liftForce;

         //Continue as before...
     }*/


    private static Vector3 CalculateLift(TextMeshProUGUI debugText, AnimationCurve liftCurve, AnimationCurve inducedDragCurve, Vector3 rightAxis,
       float airPreasureCoefficient, Vector3 localVelocity, float angleOfAttack, float liftPower, Vector3 inducedDragCoefficient)
    {
       //Project velocity onto plane
       Vector2 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);

       //Coefficiient vaies with aoa
       float liftCoefficient = liftCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);

       //Calculate flight path angle
       //float flightPathAngle = Vector3.Angle(localVelocity, Vector3.forward);

       //float liftForce = liftVelocity.sqrMagnitude * liftCoefficient * liftPower * Mathf.Cos(flightPathAngle * Mathf.Deg2Rad);
       // Without flight path angle
       float liftForce = liftVelocity.sqrMagnitude * liftCoefficient * liftPower;
       

       //Lift is perpendicular to velocity
       Vector3 lift = Vector3.Cross(liftVelocity, rightAxis) * liftForce;

       //incued drag
       float dragForce = liftCoefficient * liftCoefficient;
       Vector3 inducedDrag = dragForce * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z)) * liftVelocity.sqrMagnitude * inducedDragCoefficient * -liftVelocity.normalized;

       //Air preassure
       Vector3 airPreassure = airPreasureCoefficient * localVelocity.sqrMagnitude * -liftVelocity.normalized;

        #region Debug

       debugText.text = "Lift Velocity: " + liftVelocity + 
           "\nLift Coefficient: " + liftCoefficient + 
           "\n  Angle of Attack: " + angleOfAttack * Mathf.Rad2Deg +
           //"\nFlight Path Angle: " + flightPathAngle +
           "\nLift Force: " + liftForce +
           "\n  liftVeloityM: " + liftVelocity.sqrMagnitude +
           "\n  liftCoefficient: " + liftCoefficient +
           "\n  liftPower: " + liftPower +
           //"\n  Angle: " + Mathf.Cos(flightPathAngle * Mathf.Deg2Rad) +
           "\nLift: " + lift +
           "\n  Cross: " + Vector3.Cross(liftVelocity, rightAxis) +
           "\n    LiftVelocity: " + liftVelocity + "\n    RightAxis: " + rightAxis +
           "\n  liftForce: " + liftForce +
           "\nDragForce(LCf^2): " + dragForce +
           "\n  liftCoefficient: " + liftCoefficient +
           "\nInduced Drag: " + inducedDrag +
           "\n  dragForce: " + dragForce +
           "\n  liftVelocityM: " + liftVelocity.sqrMagnitude +
           "\n  inducedDragCoefficient: " + inducedDragCoefficient +
           "\n  inducedDragCurve: " + inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z)) +
           "\n    localVelocity.z: " + localVelocity.z +   
           "\n  liftVelocity.normalized: " + -liftVelocity.normalized +
           "\nAirPreassure: " + airPreassure + 
           "\n  APCf: " + airPreasureCoefficient + 
           "\n  LV2: " + localVelocity.sqrMagnitude + 
           "\n  velocity: " + -liftVelocity.normalized;


       //debugText.text = (lift * airPreassure).ToString() + (inducedDrag * airPreassure).ToString();
        #endregion
       return lift + airPreassure + inducedDrag;
    }

    #region Referance

    ////static Vector3 CalculateLift(Vector3 inducedDragCoefficient)
    ////static Vector3 CalculateLift(Vector3 localVelocity, float angleOfAttack, Vector3 inducedDragCoefficient, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve)
    ////{
    ////    Vector2 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);    //project velocity onto YZ plane
    ////    var v2 = liftVelocity.sqrMagnitude;                                     //square of velocity
    ////                                                                            //lift = velocity^2 * coefficient * liftPower
    ////                                                                            //coefficient varies with AOA
    ////    float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
    ////    float liftForce = v2 * liftCoefficient * liftPower;


    ////    //lift is perpendicular to velocity
    ////    Vector3 lift = Vector3.Cross(liftVelocity.normalized, rightAxis) * liftForce;

    ////    //induced drag varies with square of lift coefficient
    ////    var dragForce = liftCoefficient * liftCoefficient;
    ////    Vector3 inducedDrag = dragForce * v2 * -liftVelocity.normalized * inducedDragCoefficient * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));
    ////    return lift + inducedDrag;
    ////}
    #endregion

    /// <summary>
    /// Calculate total lift
    /// </summary>
    /// <param name="debugText"></param>
    /// <param name="localVelocity"></param>
    /// <param name="aOACurve"></param>
    /// <param name="inducedDragCurve"></param>
    /// <param name="airPreasureCoefficient"></param>
    /// <param name="liftPower"></param>
    /// <param name="rudderAOACurve"></param>
    /// <param name="rudderLiftPower"></param>
    /// <returns></returns>
    public static Vector3 CalculatelTotalLift(TextMeshProUGUI debugText, Vector3 localVelocity, AnimationCurve aOACurve, AnimationCurve inducedDragCurve, 
       float airPreasureCoefficient, float liftPower, AnimationCurve rudderAOACurve, float rudderLiftPower, Vector3 inducedDragCoefficient)
    {
       Vector3 verticalLift = CalculateLift(debugText, aOACurve, inducedDragCurve, Vector3.right, airPreasureCoefficient, localVelocity, CalculateAngleOfAttack(localVelocity), liftPower, inducedDragCoefficient);
       Vector3 rudderLift = CalculateLift(debugText, rudderAOACurve, inducedDragCurve, Vector3.up, airPreasureCoefficient, localVelocity, CalculateAngleOfAttackYaw(localVelocity), rudderLiftPower, inducedDragCoefficient);

       #region Test
       //Vector3 verticalLift = CalculateLift(localVelocity, CalculateAngleOfAttack(localVelocity), inducedDragCoefficient, Vector3.right, liftPower, aOACurve, inducedDragCurve);
       //Vector3 rudderLift = CalculateLift(localVelocity, CalculateAngleOfAttackYaw(localVelocity), inducedDragCoefficient, Vector3.up, rudderLiftPower, rudderAOACurve, inducedDragCurve);

       //Vector3 force = Quaternion.AngleAxis(-90, Vector3.forward) * verticalLift + rudderLift;

       //Vector3 force = new Vector3(verticalLift.x + rudderLift.x, verticalLift.y + rudderLift.y, verticalLift.z + rudderLift.z);
       #endregion
       return verticalLift + rudderLift;
    }
    #endregion

}
