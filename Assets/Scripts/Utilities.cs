using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Vehicles.Components;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents 2 vectors in one
    /// </summary>
    public struct DoubleVector3{
        public Vector3 vector1 { get; set; }
        public Vector3 vector2 { get; set; }
    }
    /// <summary>
    /// A set of flags for a pylon for what ordinance it may carry for a <see cref="BaseWeaponHardpoint"/>
    /// </summary>
    [Flags]
    public enum OrdinanceType
    {
        AirToAirMissile,
        AirToGroundMissile,
        Bomb,
        Rocket,
        Gun,
        Pod,
        FuelTank
    }
    [Flags]
    public enum GuidanceType
    {
        None,
        ActiveRadar,
        Infrared,
        Laser,
        TV,
        GPS,
        INS,
        AntiRadiation
    }


    internal class Utilities : MonoBehaviour
    {
        //https://github.com/vazgriz/FlightSim/blob/main/Assets/Scripts/Utilities.cs#L13
        public static Vector3 Secale6(
            Vector3 value,
            float xPositive, float xNegative,
            float yPositive, float yNegative,
            float zPositive, float zNegative
            )
        {
            Vector3 result = value;

            if(result.x > 0){
                result.x *= xPositive;
            } else {
                result.x *= xNegative;
            }

            if (result.y > 0){
                result.y *= yPositive;
            } else {
                result.y *= yNegative;
            }

            if (result.z > 0) {
                result.z *= zPositive;
            } else {
                result.z *= zNegative;
            }

            return result;
        }

        public static Slider GetSlider(string name)
        {
            return GameObject.Find(name).GetComponent<Slider>();
        }

        public static TextMeshProUGUI GetText(string name)
        {
            return GameObject.Find(name).GetComponent<TextMeshProUGUI>();
        }

    }
}
