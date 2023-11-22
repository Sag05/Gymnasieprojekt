using Assets.Scripts;
using Assets.Scripts.Vehicles;
using System;
using TMPro;
using UnityEngine;

public class PhysicsUtils : MonoBehaviour
{
    /* Variables that might be used at a later point
        //float dynamicAirViscosity = 1.7894e-5f * Mathf.Pow(1 + 120f / (airDensity + 120f) * Mathf.Pow(LocalVelocity.magnitude, 1.5f), -1);
        //float reynoldsNumber = currentAirDensity * LocalVelocity.magnitude * frontalArea / dynamicAirViscosity;
    */

    #region AircaftSteering
    private static float CalculateAircraftSteering(float angularVelocity, float targetVelocity, float acceleration, TextMeshProUGUI debugText)
    {
        float error = targetVelocity - angularVelocity;
        float deltaAcceleration = acceleration * Time.fixedDeltaTime;

        //debugText.text += "Error: " + error.ToString() + "\nTargetVelocity: " + targetVelocity.ToString() + "\nAngularVelocity: " + angularVelocity.ToString() + "\n";
        //debugText.text += "Error: " + error.ToString() + "\nDeltaAcceleration: " + deltaAcceleration.ToString() + "\n";

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
    public static DoubleVector3 AircraftSteering(Vector3 localVelocity, Vector3 localAngularVelocity, Vector3 controlInput,
        AircraftConfiguration config, TextMeshProUGUI debugText)
    {
        debugText.text = "";
        DoubleVector3 result = new DoubleVector3();
        float speed = Mathf.Max(0, localVelocity.z);
        float steeringPower = config.SteeringCurve.Evaluate(speed);

        float gForceScale = CalculateGLimiter(controlInput, Mathf.Deg2Rad * steeringPower * config.TurnSpeed, localVelocity, config.PitchGLimit, config.GLimit, debugText);

        Vector3 targetAngularVelocity = Vector3.Scale(controlInput, gForceScale * steeringPower * config.TurnSpeed);
        Vector3 angularVelocity = localAngularVelocity * Mathf.Rad2Deg;

        Vector3 correction = new(
            CalculateAircraftSteering(angularVelocity.x, targetAngularVelocity.x, config.TurnAcceleration.x * steeringPower, debugText),
            CalculateAircraftSteering(angularVelocity.y, targetAngularVelocity.y, config.TurnAcceleration.y * steeringPower, debugText),
            CalculateAircraftSteering(angularVelocity.z, targetAngularVelocity.z, config.TurnAcceleration.z * steeringPower, debugText));
        result.vector1 = correction * Mathf.Deg2Rad;

        #region Debug
        
        debugText.text += "\nTarget Angular Velocity = Control Input * G-Force scale * Steering Power * Turn Speed:\n" + 
        targetAngularVelocity.ToString("0.0") + " = " + controlInput.ToString("0.0") + ", " + gForceScale.ToString("0.0") + " * " + steeringPower.ToString("0.0") + " * " + config.TurnSpeed.ToString("0.0");
        
        //debugText.text += "Turn Speed: " + config.TurnSpeed.ToString("0.0") + 
        //    "\nSteeringPower: " + steeringPower.ToString("0.0") + "\nGForceScale: " + gForceScale.ToString("0.0");
        /*
        debugText.text = "Correction: " + correction.ToString() + 
            "\nGForceScale: " + gForceScale.ToString() + 
            "\nSteeringPower: " + steeringPower.ToString() + 
            "\nSpeed: " + speed.ToString() + "\nInput: " + controlInput.ToString() +
            "\nOutput: " + result.vector1.ToString();
        */
        /*
        Debug.Log("Correction: " + correction.ToString() + 
            "\nGForceScale: " + gForceScale.ToString() + 
            "\nSteeringPower: " + steeringPower.ToString() + 
            "\nSpeed: " + speed.ToString() + "\nInput: " + controlInput.ToString() +
            "\nOutput: " + result.vector1.ToString());
        */
        #endregion
        //Effective input, used for animations
        #region EffectiveInput

        Vector3 correctionInput = new Vector3(
            Mathf.Clamp((targetAngularVelocity.x - angularVelocity.x)/ config.TurnAcceleration.x, -1, 1),
            Mathf.Clamp((targetAngularVelocity.y - angularVelocity.y)/ config.TurnAcceleration.y, -1, 1),
            Mathf.Clamp((targetAngularVelocity.z - angularVelocity.z)/ config.TurnAcceleration.z, -1, 1));

        Vector3 effectiveInput = (correctionInput + controlInput) * gForceScale;

        Vector3 EffectiveInput = new Vector3(
            Mathf.Clamp(effectiveInput.x, -1, 1),
            Mathf.Clamp(effectiveInput.y, -1, 1),
            Mathf.Clamp(effectiveInput.z, -1, 1));

        result.vector2 = EffectiveInput;
        #endregion

        return result;
    }

    private static float CalculateGLimiter(Vector3 controlinput, Vector3 maxAngularVelocity, Vector3 localVelocity, float pitchGLimit, float gLimit, TextMeshProUGUI debugText)
    {
        Vector3 limit = Utilities.Secale6(controlinput.normalized,
            gLimit, pitchGLimit,
            gLimit, gLimit,
            gLimit, gLimit);
        Vector3 maxGForce = CalculateLocalGForce(Vector3.Scale(controlinput.normalized, maxAngularVelocity), localVelocity);

        #region Debug
        //debugText.text += "MaxGForce: " + maxGForce.magnitude.ToString("0.0") + "\nLimit: " + limit.magnitude.ToString("0.0") + 
        //"\nForce larger than limit: " + (maxGForce.magnitude > limit.magnitude).ToString() + "\nReturn: " + (limit.magnitude / maxGForce.magnitude).ToString("0.0") + "\n";
        #endregion 
        
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

    #region Drag 
    public static Vector3 CalculateDrag(Vector3 localVelocity, float altitude, AnimationCurve altitudeEffectivenessCurve, AnimationCurve sideDrag, AnimationCurve topDrag, AnimationCurve frontDrag, float additionalDragCoefficient)
    {
        float sideDragCoefficient = sideDrag.Evaluate(Mathf.Abs(localVelocity.x));
        float topDragCoefficient = topDrag.Evaluate(Mathf.Abs(localVelocity.y));
        float forwardDragCoefficient = frontDrag.Evaluate(Mathf.Abs(localVelocity.z)) + additionalDragCoefficient;
        
        Vector3 totalDragCoefficient = Utilities.Secale6(localVelocity.normalized,
            sideDragCoefficient, sideDragCoefficient,
            topDragCoefficient, topDragCoefficient,
            forwardDragCoefficient, forwardDragCoefficient);

        Vector3 dragForce = totalDragCoefficient.magnitude * localVelocity.sqrMagnitude * -localVelocity.normalized;
        return dragForce * altitudeEffectivenessCurve.Evaluate(altitude);
    }
    #endregion

    #region Lift
    /// <summary>
    /// Calculate total lift using <paramref name="localVelocity"/> and <paramref name="aircraftConfig"/>
    /// </summary>
    /// <param name="debugText"></param>
    /// <param name="localVelocity"></param>
    /// <param name="aircraftConfig"></param>
    /// <returns>Vector3 Lift</returns>
    public static Vector3 CalculatelTotalAircraftLift(TextMeshProUGUI debugText, Vector3 localVelocity, float altitude,AircraftConfiguration aircraftConfig)
    {
       Vector3 verticalLift = CalculateLift(debugText, aircraftConfig.LiftCurve, aircraftConfig.InducedDragCurve, Vector3.right, localVelocity, CalculateAngleOfAttack(localVelocity), aircraftConfig.LiftPower, aircraftConfig.InducedDragCoefficient);
       Vector3 rudderLift = CalculateLift(debugText, aircraftConfig.RudderLiftCurve, aircraftConfig.InducedDragCurve, Vector3.up, localVelocity, CalculateAngleOfAttackYaw(localVelocity), aircraftConfig.RudderLiftPower, aircraftConfig.InducedDragCoefficient);
       return (verticalLift + rudderLift) * aircraftConfig.AltitudeEffectivenessCurve.Evaluate(altitude);
    }

    private static Vector3 CalculateLift(TextMeshProUGUI debugText, AnimationCurve liftCurve, AnimationCurve inducedDragCurve, Vector3 rightAxis,
       Vector3 localVelocity, float angleOfAttack, int liftPower, float inducedDragCoefficient)
    {
        //Project velocity onto plane
        Vector3 liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightAxis);        

        //Coefficiient vaies with aoa
        float liftCoefficient = liftCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);

        //liftforce
        float liftForce = liftVelocity.sqrMagnitude * liftCoefficient * liftPower;
       
        //Lift is perpendicular to velocity
        Vector3 lift = Vector3.Cross(liftVelocity, rightAxis) * liftForce;

        //incued drag
        float dragForce = liftCoefficient * liftCoefficient;
        Vector3 inducedDrag = dragForce * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z)) * liftVelocity.sqrMagnitude * inducedDragCoefficient * -liftVelocity.normalized;

        #region Debug

        /*EVERYTHING
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
        */
        
        //debugText.text = "Lift: " + lift + "\nInduced Drag: " + inducedDrag  + "\n";

        #endregion

        return lift + inducedDrag; 
    }

    #endregion
}
