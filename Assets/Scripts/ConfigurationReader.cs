using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using Assets.Scripts.Ordinance;

namespace Assets.Scripts
{
    internal class ConfigurationReader
    {
        private enum AIRCRAFTCONFIGCONTEXT { NONE, AIRCRAFTCONFIG, COMPONENTCONFIG, ANIMATIONCURVE }
        private enum ORDINANCECONFIGCONTEXT { NONE, ORDINANCECONFIG, ANIMATIONCURVE }

        private static bool SetCulture()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return true;
        }

        private static bool ApplyFieldToObject(ref object obj, string fieldName, string statementValue)
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
                Debug.Log("Trying to apply [" + fieldName + "] = [" + statementValue + "]\nField type: " + objectFieldInfo.FieldType);
                if (objectFieldInfo.FieldType == typeof(string))
                    objectFieldInfo.SetValue(obj, statementValue);
                else if (objectFieldInfo.FieldType == typeof(int))
                    objectFieldInfo.SetValue(obj, int.Parse(statementValue));
                else if (objectFieldInfo.FieldType == typeof(float))
                    objectFieldInfo.SetValue(obj, float.Parse(statementValue));
                else if (objectFieldInfo.FieldType == typeof(double))
                    objectFieldInfo.SetValue(obj, double.Parse(statementValue));
                else if (objectFieldInfo.FieldType == typeof(bool))
                    objectFieldInfo.SetValue(obj, bool.Parse(statementValue));
                else if (objectFieldInfo.FieldType == typeof(Vector3))
                    objectFieldInfo.SetValue(obj, new Vector3(float.Parse(statementValue.Split(',')[0]), float.Parse(statementValue.Split(',')[1]), float.Parse(statementValue.Split(',')[2])));
                else if (objectFieldInfo.FieldType == typeof(List<>))
                    objectFieldInfo.SetValue(obj, statementValue.Split(',').ToList());
                else if (objectPropInfo.PropertyType.IsEnum)
                {
                    int enumFlags = 0;
                    // Enum is flag type; Holds multiple values
                    if (objectPropInfo.PropertyType.GetCustomAttribute<FlagsAttribute>() is not null)
                    {

                        foreach (string enumFlag in statementValue.Split("|"))
                        {
                            enumFlags |= (int)Enum.Parse(objectPropInfo.PropertyType, enumFlag);
                        }
                    }
                    else
                    {
                        enumFlags = (int)Enum.Parse(objectPropInfo.PropertyType, statementValue);
                    }
                    objectPropInfo.SetValue(obj, enumFlags);
                }
            }
            else
            {
                Debug.Log("Trying to apply [" + fieldName + "] = [" + statementValue + "]\nProperty type: " + objectPropInfo.PropertyType);
                if (objectPropInfo.PropertyType == typeof(string))
                    objectPropInfo.SetValue(obj, statementValue);
                else if (objectPropInfo.PropertyType == typeof(int))
                    objectPropInfo.SetValue(obj, int.Parse(statementValue));
                else if (objectPropInfo.PropertyType == typeof(float))
                    objectPropInfo.SetValue(obj, float.Parse(statementValue));
                else if (objectPropInfo.PropertyType == typeof(double))
                    objectPropInfo.SetValue(obj, double.Parse(statementValue));
                else if (objectPropInfo.PropertyType == typeof(bool))
                    objectPropInfo.SetValue(obj, bool.Parse(statementValue));
                else if (objectPropInfo.PropertyType == typeof(Vector3))
                    objectPropInfo.SetValue(obj, new Vector3(float.Parse(statementValue.Split(',')[0]), float.Parse(statementValue.Split(',')[1]), float.Parse(statementValue.Split(',')[2])));
                else if (objectPropInfo.PropertyType == typeof(List<>))
                    objectPropInfo.SetValue(obj, statementValue.Split(',').ToList());
                else if (objectPropInfo.PropertyType.IsEnum)
                {
                    int enumFlags = 0;
                    // Enum is flag type; Holds multiple values
                    if (objectPropInfo.PropertyType.GetCustomAttribute<FlagsAttribute>() is not null)
                    {

                        foreach (string enumFlag in statementValue.Split("|"))
                        {
                            enumFlags |= (int)Enum.Parse(objectPropInfo.PropertyType, enumFlag);
                        }
                    }
                    else
                    {
                        enumFlags = (int)Enum.Parse(objectPropInfo.PropertyType, statementValue);
                    }
                    objectPropInfo.SetValue(obj, enumFlags);
                }

                /*
                else if (objectPropInfo.PropertyType == typeof(OrdinanceType))
                {
                    OrdinanceType ordinanceTypes = 0;
                    foreach (string OrdinanceType in statementValue.Split(','))
                    {
                        ordinanceTypes |= (OrdinanceType)Enum.Parse(typeof(OrdinanceType), OrdinanceType, false);
                    }
                    objectPropInfo.SetValue(obj, ordinanceTypes);
                }
                else if (objectPropInfo.PropertyType == typeof(GuidanceType))
                {
                    GuidanceType guidanceTypes = 0;
                    foreach (string guidanceType in statementValue.Split(','))
                    {
                        guidanceTypes |= (GuidanceType)Enum.Parse(typeof(GuidanceType), guidanceType, false);
                    }
                    objectPropInfo.SetValue(obj, guidanceTypes);
                }
                */
            }

            Debug.Log("APPLIED [" + fieldName + "] = [" + statementValue + "]");
            return true;
        }

        private static bool ApplyAnimationCurve(ref object obj, string curveName, List<Keyframe> keyframes)
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

        public static TargetingPodConfig LoadTargetingPod(string configPath){
            SetCulture();

            ORDINANCECONFIGCONTEXT CurrentConfigurationContext = ORDINANCECONFIGCONTEXT.NONE;
            ORDINANCECONFIGCONTEXT storedConfigurationContext = ORDINANCECONFIGCONTEXT.NONE;
            string animationCurveName = null;
            object targetingPodConfig = new TargetingPodConfig();
            List<Keyframe> keyframes = new List<Keyframe>();

            using (StreamReader r = new StreamReader(configPath)){
                List<string[]> statements = r.ReadToEnd().Replace("\n", "").Replace("\r", "").Split(';').Where(e => !e.StartsWith('#')).Select(e => e.Split(' ')).ToList();

                for (int i = 0; i < statements.Count; i++){
                    string[] statement = statements[i];
                    switch (statement[0]){
                        case "":
                            break;
                        case "CONTEXT":
                            switch (statement[0])
                            {
                                case "ORDINANCECONFIG":
                                    CurrentConfigurationContext = ORDINANCECONFIGCONTEXT.ORDINANCECONFIG;
                                    break;
                            }
                            break;
                        case "ANIMATIONCURVE":
                            if (CurrentConfigurationContext == ORDINANCECONFIGCONTEXT.ANIMATIONCURVE) break;
                            storedConfigurationContext = CurrentConfigurationContext;
                            CurrentConfigurationContext = ORDINANCECONFIGCONTEXT.ANIMATIONCURVE;
                            animationCurveName = statement[1];
                            break;
                        case "FINISHANIMATIONCURVE":
                            CurrentConfigurationContext = storedConfigurationContext;
                            switch (CurrentConfigurationContext){
                                case ORDINANCECONFIGCONTEXT.ORDINANCECONFIG:
                                    ApplyAnimationCurve(ref targetingPodConfig, animationCurveName, keyframes);
                                    break;
                            }
                            break;
                        default:
                            switch (CurrentConfigurationContext){
                                case ORDINANCECONFIGCONTEXT.ORDINANCECONFIG:
                                    ApplyFieldToObject(ref targetingPodConfig, statement[0], statement[1]);
                                    break;
                                case ORDINANCECONFIGCONTEXT.ANIMATIONCURVE:
                                    Keyframe keyframe = new Keyframe(float.Parse(statement[0]), float.Parse(statement[1]));
                                    keyframes.Add(keyframe);
                                    break;
                            }
                            break;
                    }
                }
            }

            return (TargetingPodConfig)targetingPodConfig;
        }

        public static AircraftConfiguration LoadAircraft(string configPath, VehicleBase caller)
        {
            SetCulture();

            // Debug.Log(typeof(HelmetMountedDisplay).FullName);
            AIRCRAFTCONFIGCONTEXT CurrentConfigurationContext = AIRCRAFTCONFIGCONTEXT.NONE;
            AIRCRAFTCONFIGCONTEXT StoredConfigurationContext = AIRCRAFTCONFIGCONTEXT.NONE;

            object ComponentObject = null;
            string animationCurveName = null;
            List<Keyframe> keyframes = new List<Keyframe>();

            //Type ComponentType = null;

            object aircraftConfiguration = new AircraftConfiguration();
            using (StreamReader r = new StreamReader(configPath))
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
                                    CurrentConfigurationContext = AIRCRAFTCONFIGCONTEXT.AIRCRAFTCONFIG;
                                    break;
                                // Switch to component configuration context
                                case "COMPONENTCONFIG":
                                    CurrentConfigurationContext = AIRCRAFTCONFIGCONTEXT.COMPONENTCONFIG;
                                    break;
                            }
                            break;

                        // Component handling
                        case "NEWCOMPONENT":
                            if (CurrentConfigurationContext != AIRCRAFTCONFIGCONTEXT.COMPONENTCONFIG) break;
                            Type componentType = Type.GetType(statement[1]);
                            ComponentObject = Activator.CreateInstance(componentType, new object[] { caller });
                            // ComponentObject = componentType.GetConstructor(new Type[] { typeof(VehicleBase) }).Invoke();
                            break;
                        case "FINISHCOMPONENT":
                            ((AircraftConfiguration)aircraftConfiguration).VehicleComponents.Add((ComponentBase)ComponentObject);
                            break;

                        // Animation curve handling
                        case "ANIMATIONCURVE":
                            if (CurrentConfigurationContext == AIRCRAFTCONFIGCONTEXT.ANIMATIONCURVE) break;
                            StoredConfigurationContext = CurrentConfigurationContext;
                            CurrentConfigurationContext = AIRCRAFTCONFIGCONTEXT.ANIMATIONCURVE;
                            animationCurveName = statement[1];
                            break;
                        case "FINISHANIMATIONCURVE":
                            CurrentConfigurationContext = StoredConfigurationContext;
                            //Add animationcurve to configuration
                            switch (CurrentConfigurationContext)
                            {
                                case AIRCRAFTCONFIGCONTEXT.AIRCRAFTCONFIG:
                                    ApplyAnimationCurve(ref aircraftConfiguration, animationCurveName, keyframes);
                                    break;
                                case AIRCRAFTCONFIGCONTEXT.COMPONENTCONFIG:
                                    ApplyAnimationCurve(ref ComponentObject, animationCurveName, keyframes);
                                    break;
                            }
                            //Reset animationcurve variable
                            keyframes.Clear();
                            break;

                        // Value handling
                        default:
                            switch (CurrentConfigurationContext)
                            {
                                case AIRCRAFTCONFIGCONTEXT.AIRCRAFTCONFIG:
                                    ApplyFieldToObject(ref aircraftConfiguration, statement[0], statement[1]);
                                    break;
                                case AIRCRAFTCONFIGCONTEXT.COMPONENTCONFIG:
                                    ApplyFieldToObject(ref ComponentObject, statement[0], statement[1]);
                                    break;
                                case AIRCRAFTCONFIGCONTEXT.ANIMATIONCURVE:
                                    Keyframe keyframe = new Keyframe(float.Parse(statement[0]), float.Parse(statement[1]));
                                    keyframes.Add(keyframe);
                                    break;
                            }
                            break;
                    }
                }
            }
            return (AircraftConfiguration)aircraftConfiguration;
        }
    }
}
