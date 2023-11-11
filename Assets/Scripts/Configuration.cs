using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEngine;
using System.Reflection;

namespace Assets.Scripts
{
    internal class Configuration
    {
        private enum CONFIGCONTEXT { NONE, AIRCRAFTCONFIG, COMPONENTCONFIG, ANIMATIONCURVE }

        public static AircraftConfiguration LoadAircraft(string configName, VehicleBase caller)
        {
            bool ApplyFieldToObject(ref object obj, string fieldName, string statementValue)
            {
                FieldInfo objectFieldInfo = null;
                PropertyInfo objectPropInfo = null;
                Type objectType = obj.GetType();
                while (objectFieldInfo is null && objectType is not null && objectPropInfo is null)
                {
                    objectFieldInfo = objectType.GetRuntimeField(fieldName);//, BindingFlags.Instance);
                    objectPropInfo = objectType.GetRuntimeProperty(fieldName);
                    objectType = objectType.BaseType;
                }

                //Check if value exists
                if (objectType is null && objectFieldInfo is null && objectPropInfo is null)
                {
                    Debug.LogError("Field [" + statementValue + "] does not exist");
                    return false;
                }


                if (objectFieldInfo is not null)
                {
                    //if (objectFieldInfo.FieldType == typeof(int)) objectFieldInfo.SetValue(obj, int.Parse(statementValue));

                    if (objectFieldInfo.FieldType == typeof(string))
                        objectFieldInfo.SetValue(obj, statementValue);
                    else if (objectFieldInfo.FieldType == typeof(int))
                        objectFieldInfo.SetValue(obj, int.Parse(statementValue));
                    else if (objectFieldInfo.FieldType == typeof(float))
                        objectFieldInfo.SetValue(obj, float.Parse(statementValue));
                    else if (objectFieldInfo.FieldType == typeof(double))
                        objectFieldInfo.SetValue(obj, double.Parse(statementValue));
                    else if (objectFieldInfo.FieldType == typeof(Vector3))
                        objectFieldInfo.SetValue(obj, new Vector3(float.Parse(statementValue.Split(',')[0]), float.Parse(statementValue.Split(',')[1]), float.Parse(statementValue.Split(',')[2])));
                }
                else
                {
                    if (objectPropInfo.PropertyType == typeof(string))
                        objectPropInfo.SetValue(obj, statementValue);
                    else if (objectPropInfo.PropertyType == typeof(int))
                        objectPropInfo.SetValue(obj, int.Parse(statementValue));
                    else if (objectPropInfo.PropertyType == typeof(float))
                        objectPropInfo.SetValue(obj, float.Parse(statementValue));
                    else if (objectPropInfo.PropertyType == typeof(double))
                        objectPropInfo.SetValue(obj, double.Parse(statementValue));
                    else if (objectPropInfo.PropertyType == typeof(Vector3))
                        objectPropInfo.SetValue(obj, new Vector3(float.Parse(statementValue.Split(',')[0]), float.Parse(statementValue.Split(',')[1]), float.Parse(statementValue.Split(',')[2])));
                }

                Debug.Log("APPLIED [" + fieldName + "] = [" + statementValue + "]");
                return true;
            }

            bool ApplyAnimationCurve(ref object obj, string curveName, List<Keyframe> keyframes)
            {
                FieldInfo objectFieldInfo = null;
                PropertyInfo objectPropInfo = null;
                Type objectType = obj.GetType();
                while (objectFieldInfo is null && objectType is not null && objectPropInfo is null)
                {
                    objectFieldInfo = objectType.GetRuntimeField(curveName);//, BindingFlags.Instance);
                    objectPropInfo = objectType.GetRuntimeProperty(curveName);
                    objectType = objectType.BaseType;
                }

                //Check if value exists
                if (objectType is null && objectFieldInfo is null && objectPropInfo is null)
                {
                    Debug.LogError("Field [" + curveName + "] does not exist");
                    return false;
                }
                objectPropInfo.SetValue(obj, new AnimationCurve(keyframes.ToArray()));

                Debug.Log("APPLIED [" + curveName + "]");
                return true;
            }

            // Debug.Log(typeof(HelmetMountedDisplay).FullName);
            CONFIGCONTEXT CurrentConfigurationContext = CONFIGCONTEXT.NONE;
            CONFIGCONTEXT StoredConfigurationContext = CONFIGCONTEXT.NONE;

            object ComponentObject = null;
            string animationCurveName = null;
            List<Keyframe> keyframes = new List<Keyframe>();

            //Type ComponentType = null;

            object aircraftConfiguration = new AircraftConfiguration();
            using (StreamReader r = new StreamReader(configName))
            {
                List<string[]> statements = r.ReadToEnd().Replace("\n", "").Replace("\r", "").Split(';').Where(e => !e.StartsWith('#')).Select(e => e.Split(' ')).ToList();

                for (int i = 0; i < statements.Count; i++)
                {
                    string[] statement = statements[i];
                    switch (statement[0])
                    {
                        case "":
                            break;
                        // Context handling
                        case "CONTEXT":
                            switch (statement[1])
                            {
                                // Switch to aircraft configuration context
                                case "AIRCRAFTCONFIG":
                                    CurrentConfigurationContext = CONFIGCONTEXT.AIRCRAFTCONFIG;
                                    break;
                                // Switch to component configuration context
                                case "COMPONENTCONFIG":
                                    CurrentConfigurationContext = CONFIGCONTEXT.COMPONENTCONFIG;
                                    break;
                            }
                            break;

                        // Component handling
                        case "NEWCOMPONENT":
                            if (CurrentConfigurationContext != CONFIGCONTEXT.COMPONENTCONFIG) break;
                            Type componentType = Type.GetType(statement[1]);
                            ComponentObject = Activator.CreateInstance(componentType, new object[] { caller });
                            // ComponentObject = componentType.GetConstructor(new Type[] { typeof(VehicleBase) }).Invoke();
                            break;
                        case "FINISHCOMPONENT":
                            ((AircraftConfiguration)aircraftConfiguration).VehicleComponents.Add((ComponentBase)ComponentObject);
                            break;

                        // Animation curve handling
                        case "ANIMATIONCURVE":
                            if (CurrentConfigurationContext == CONFIGCONTEXT.ANIMATIONCURVE) break;
                            StoredConfigurationContext = CurrentConfigurationContext;
                            CurrentConfigurationContext = CONFIGCONTEXT.ANIMATIONCURVE;
                            animationCurveName = statement[1];
                            break;
                        case "FINISHANIMATIONCURVE":
                            CurrentConfigurationContext = StoredConfigurationContext;
                            //Add animationcurve to configuration

                            ApplyAnimationCurve(ref aircraftConfiguration, animationCurveName, keyframes);
                            //Reset animationcurve variable
                            keyframes.Clear();
                            break;

                        // Value handling
                        default:

                            switch (CurrentConfigurationContext)
                            {
                                case CONFIGCONTEXT.AIRCRAFTCONFIG:
                                    ApplyFieldToObject(ref aircraftConfiguration, statement[0], statement[1]);
                                    break;
                                case CONFIGCONTEXT.COMPONENTCONFIG:
                                    ApplyFieldToObject(ref ComponentObject, statement[0], statement[1]);
                                    break;
                                case CONFIGCONTEXT.ANIMATIONCURVE:
                                    Keyframe keyframe = new Keyframe(float.Parse(statement[0]), float.Parse(statement[1]));
                                    keyframes.Add(keyframe);
                                    break;
                            }
                            break;
                    }
                }
            }

            #region oldCode
            /*
            string[] input = File.ReadAllLines(configName);
            foreach (string line in input)
            {
                string modifiedLine = line.ToLower();
                modifiedLine = modifiedLine.Replace(" ", "");
                string[] split = modifiedLine.Split(',');
                switch (split[0])
                {
                    case "#":
                        break;
                    case "":
                        break;
                    case "component":
                        switch (split[1])
                        {
                            case "aircraftengine":
                                Debug.Log("Hitpoints = " + split[2] + ", TurbineMaxRPM = " + split[3] + ", TurbineAcceleration = " + split[4] + ", Max Thrust = " + split[5]);
                                aircraftConfiguration.VehicleComponents.Add(new AircraftEngine(caller)
                                {
                                    HitPoints = float.Parse(split[2]),
                                    TurbineMaxRPM = float.Parse(split[3]),
                                    TurbineAcceleration = float.Parse(split[4]),
                                    MaxThrust = float.Parse(split[5])
                                });
                                break;
                            case "helmetmounteddisplay":
                                aircraftConfiguration.VehicleComponents.Add(new HelmetMountedDisplay(caller)
                                {
                                    HitPoints = float.Parse(split[2])
                                });
                                break;
                            default:
                                throw new Exception("Unknown component type");
                        }
                        break;

                    case "liftcurve":
                        aircraftConfiguration.liftCurve = new AnimationCurve();
                        for (int i = 1; i < split.Length; i++)
                        {
                            string[] curveSplit = split[i].Split(':');
                            aircraftConfiguration.liftCurve.AddKey(float.Parse(curveSplit[0]), float.Parse(curveSplit[1]));
                        }
                        break;

                    case "frontarea":
                        aircraftConfiguration.FrontalArea = float.Parse(split[1]);
                        break;

                    case "mass":
                        aircraftConfiguration.Mass = float.Parse(split[1]);
                        break;
                    default:
                        throw new Exception("Unknown configuration parameter");
                }
            }
            */
            #endregion
            return (AircraftConfiguration)aircraftConfiguration;
        }
    }
}
