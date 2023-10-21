using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEngine;
using Unity.VisualScripting;
using System.Reflection;

namespace Assets.Scripts
{
    internal class Configuration
    {
        private enum CONFIGCONTEXT { NONE, AIRCRAFTCONFIG, COMPONENTCONFIG }
        public static AircraftConfiguration LoadAircraft(string configName, VehicleBase caller)
        {
            bool ApplyFieldToObject(ref object obj, string fieldName, string statementValue)
            {
                FieldInfo objectFieldInfo = null;
                PropertyInfo objectPropInfo = null;
                Type t = obj.GetType();
                while (objectFieldInfo is null && t is not null && objectPropInfo is null)
                {
                    objectFieldInfo = t.GetRuntimeField(fieldName);//, BindingFlags.Instance);
                    objectPropInfo = t.GetRuntimeProperty(fieldName);
                    t = t.BaseType;
                }

                if (t is null && objectFieldInfo is null && objectPropInfo is null)
                {
                    Debug.LogError("Field [" + statementValue + "] does not exist");
                    return false;
                }

                if (objectFieldInfo is not null)
                {
                    if (objectFieldInfo.FieldType == typeof(string))
                        objectFieldInfo.SetValue(obj, statementValue);
                    else if (objectFieldInfo.FieldType == typeof(int))
                        objectFieldInfo.SetValue(obj, int.Parse(statementValue));
                    else if (objectFieldInfo.FieldType == typeof(float))
                        objectFieldInfo.SetValue(obj, float.Parse(statementValue));
                    else if (objectFieldInfo.FieldType == typeof(double))
                        objectFieldInfo.SetValue(obj, double.Parse(statementValue));
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
                }

                Debug.Log("APPLIED [" + fieldName + "] = [" + statementValue + "]");
                return true;
            }

            // Debug.Log(typeof(HelmetMountedDisplay).FullName);
            CONFIGCONTEXT CurrentConfigurationContext = CONFIGCONTEXT.NONE;

            object ComponentObject = null;
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
                        // Handle context
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
                        case "NEWCOMPONENT":
                            if (CurrentConfigurationContext != CONFIGCONTEXT.COMPONENTCONFIG) break;
                            Type componentType = Type.GetType(statement[1]);
                            ComponentObject = Activator.CreateInstance(componentType, new object[] { caller });
                            //ComponentObject = componentType.GetConstructor(new Type[] { typeof(VehicleBase) }).Invoke();
                            break;
                        case "FINISHCOMPONENT":
                            ((AircraftConfiguration)aircraftConfiguration).VehicleComponents.Add((ComponentBase)ComponentObject);
                            break;
                        default:
                            switch (CurrentConfigurationContext)
                            {
                                case CONFIGCONTEXT.AIRCRAFTCONFIG:
                                    ApplyFieldToObject(ref aircraftConfiguration, statement[0], statement[1]);
                                    break;
                                case CONFIGCONTEXT.COMPONENTCONFIG:
                                    ApplyFieldToObject(ref ComponentObject, statement[0], statement[1]);
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
