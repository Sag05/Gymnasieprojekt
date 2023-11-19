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
        selectionMenuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SelectionMenu.prefab");
        Debug.Log("Loaded selection menu prefab: " + selectionMenuPrefab.name);
        selectionButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SelectionButton.prefab");
        Debug.Log("Loaded selection button prefab: " + selectionButtonPrefab.name);

        selectionMenu = Instantiate(selectionMenuPrefab);


        Vector3 buttonPosition = new Vector3(-280, 200, 0);
        // Load Vehicles From Configs 
        foreach (string configName in Directory.GetFiles(aircraftDirectory, "*.cfg"))
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
        Destroy(selectionMenu);
        Debug.Log("Attempting to load " + aircraftName);
        GameObject newAircraft = Instantiate(aircraftPrefab);
        newAircraft.name = aircraftName;
        Debug.Log("Loaded " + aircraftName);
        player.SelectVehicle(newAircraft);
    }

}
