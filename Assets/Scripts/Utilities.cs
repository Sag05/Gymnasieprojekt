﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents 2 vectors in one
    /// </summary>
    public struct DoubleVector3
    {
        public Vector3 vector1 { get; set; }
        public Vector3 vector2 { get; set; }
    }

    public enum VehicleType
    {
        Aircraft,
        Helicopter,
        GroundVehicle
    }

    public enum PodType
    {
        ECM,
        Radar,
        TargetingPod
    }

    /// <summary>
    /// A set of flags for a pylon for what ordinance it may carry for a <see cref="BaseHardpoint"/>
    /// </summary>
    [Flags]
    public enum OrdinanceType
    {
        AirToAirMissile = 1,
        AirToGroundMissile = 2,
        Bomb = 4,
        Rocket = 8,
        Gun = 16,
        Pod = 32,
        FuelTank = 64,

        All = AirToAirMissile | AirToGroundMissile | Bomb | Rocket | Gun | Pod | FuelTank
    }
    [Flags]
    public enum GuidanceType
    {
        None = 1,
        ActiveRadar = 2,
        Infrared = 4,
        Laser = 8,
        TV = 16,
        GPS = 32,
        INS = 64,
        AntiRadiation = 128,

        All = None | ActiveRadar | Infrared | Laser | TV | GPS | INS | AntiRadiation
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

            if (result.x > 0)
            {
                result.x *= xPositive;
            }
            else
            {
                result.x *= xNegative;
            }

            if (result.y > 0)
            {
                result.y *= yPositive;
            }
            else
            {
                result.y *= yNegative;
            }

            if (result.z > 0)
            {
                result.z *= zPositive;
            }
            else
            {
                result.z *= zNegative;
            }

            return result;
        }


        /// <summary>
        /// Gets a child of <paramref name="parent"/> by <paramref name="name"/> 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetChildOf(GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }
            return null;
        }


        /// <summary>
        /// Gets a child of <paramref name="parent"/> by <paramref name="name"/>, including grandchildren
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetAnyChildOf(GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
                else
                {
                    GameObject result = GetAnyChildOf(child.gameObject, name);
                    if (result != null) return result;
                }
            }
            return null;
        }


        //UNFINISHED
        /// <summary>
        /// Returns a <see cref="Vector3"/> with all values set to the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Vector3 FloatToVector3(float input)
        {
            return new Vector3(input, input, input);
        }

        /// <summary>
        /// Returns a <see cref="Slider"/> with the given name, for use in classes that do not inherit from <see cref="MonoBehaviour"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Slider GetSlider(string name)
        {
            return GameObject.Find(name).GetComponent<Slider>();
        }
        /// <summary>
        /// Returns a <see cref="TextMeshProUGUI"/> with the given name, for use in classes that do not inherit from <see cref="MonoBehaviour"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TextMeshProUGUI GetText(string name)
        {
            return GameObject.Find(name).GetComponent<TextMeshProUGUI>();
        }

    }

    public class Vector6
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float Front { get; set; }
        public float Back { get; set; }


        public Vector6(float top, float bottom, float left, float right, float front, float back)
        {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Front = front;
            this.Back = back;
        }

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(
                    Mathf.Pow(this.Top, 2) +
                    Mathf.Pow(this.Bottom, 2) +
                    Mathf.Pow(this.Left, 2) +
                    Mathf.Pow(this.Right, 2) +
                    Mathf.Pow(this.Front, 2) +
                    Mathf.Pow(this.Back, 2)
                    );
            }
        }
    }
}
