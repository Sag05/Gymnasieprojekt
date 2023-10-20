using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Configuration
    {
        public static AircraftConfiguration LoadAircraft(string configName, VehicleBase caller)
        {
            AircraftConfiguration aircraftConfiguration = new AircraftConfiguration();
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
            Debug.Log("Loaded aircraft configuration: " + configName);


            return aircraftConfiguration;
        }
    }
}
