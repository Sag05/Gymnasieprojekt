using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VehicleLoader : MonoBehaviour
{
    private readonly string aircraftDirectory = @".\configs\aircrafts";
    private GameObject aircraftPrefab;
    private GameObject selectionMenuPrefab;
    private GameObject selectionButtonPrefab;
    private GameObject selectionMenu;

    public void LoadVehicleSelectionMenu(PlayerController player)
    {
        aircraftPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/aircraft.prefab");
        Debug.Log("Loaded aircraft prefab: " + aircraftPrefab.name);



        // Load Vehicles From Configs 
        string[] aircraftConfigs = Directory.GetFiles(aircraftDirectory, "*.cfg");
        switch (aircraftConfigs.Length)
        {
            case 0:
                Debug.Log("No aircraft configs found.");
                break;
            case 1:
                Debug.Log("Found 1 aircraft config.");
                break;
            default:
                Debug.Log("Found " + aircraftConfigs.Length + " aircraft configs.");
                break;
        }
    }

    private void LoadVehicleSelectionMenu(string[] aircraftConfigs, PlayerController player)
    {
        selectionMenuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SelectionMenu.prefab");
        Debug.Log("Loaded selection menu prefab: " + selectionMenuPrefab.name);
        selectionButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SelectionButton.prefab");
        Debug.Log("Loaded selection button prefab: " + selectionButtonPrefab.name);

        Vector3 buttonPosition = new Vector3(-280, 200, 0);
        selectionMenu = Instantiate(selectionMenuPrefab);
        foreach (string configName in aircraftConfigs)
        {
            //Get name of aircraft from config file name
            string aircraftName = configName.Split('\\')[3].Remove(configName.Split('\\')[3].Length - ".cfg".Length);
            Debug.Log("Found aircraft: " + aircraftName + " at " + configName + ". Adding to selection menu.");
            //Add button to selection menu
            GameObject buttonObject = Instantiate(selectionButtonPrefab, selectionMenu.transform);
            //Set button position
            buttonObject.transform.localPosition = buttonPosition;
            //Move position of next button
            buttonPosition.y -= 35;
            //Set button name
            buttonObject.name = aircraftName;
            //Get button component
            Button button = buttonObject.GetComponent<Button>();
            //Add Listener to run AircraftSelected() with the name of the button, to load the selected aircraft
            button.onClick.AddListener(() => AircraftSlected(buttonObject.name, player));
            //Set button text
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = aircraftName;
        }
    }


    public void AircraftSlected(string aircraftName, PlayerController player)
    {
        Vector3 position = new Vector3(0, 100, 0);
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        Destroy(selectionMenu);
        Debug.Log("Attempting to load " + aircraftName);
        GameObject newAircraft = Instantiate(aircraftPrefab, position, rotation);
        Debug.Log("Instantiated " + aircraftName + " at " + position + " with " + rotation.eulerAngles + " rotation.");
        newAircraft.name = aircraftName;
        Debug.Log("Loaded " + aircraftName);
        player.SelectVehicle(newAircraft);
    }

}
