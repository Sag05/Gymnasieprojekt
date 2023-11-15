using System.IO;
using UnityEngine;
using Assets.Scripts.Vehicles;
using Assets.Scripts;
using UnityEngine.UIElements.Experimental;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public const float gravity = 9.82f;
    public const float scaleFactor = 0.1f;
    public string AircraftDirectory = @".\configs\aircrafts";
    public GameObject aircraftPrefab;
    
    void Start()
    {
        //aircraftPrefab = Resources.Load<GameObject>("Prefabs/aircraft");
        aircraftPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/aircraft.prefab");
        foreach(string configName in Directory.GetFiles(AircraftDirectory, "*.cfg"))
        {
            string aircraftName = configName.Split('\\')[3].Remove(configName.Split('\\')[3].Length - ".cfg".Length);
            Debug.Log("Attempting to load " + aircraftName + " from " + configName);
            //AircraftConfiguration config = Configuration.LoadAircraft(@".\configs\" + gameObject.name + ".cfg", this);
            GameObject newAircraft = Instantiate(aircraftPrefab);
            newAircraft.name = aircraftName;
            //Instantiate(Resources.Load<GameObject>("Prefabs/Aircraft"));
            //AircraftConfiguration aircraftConfiguration = Configuration.LoadAircraft(s);
            Debug.Log("Loaded " + aircraftName);
        }
    }
    void Update()
    {
        
    }
}
