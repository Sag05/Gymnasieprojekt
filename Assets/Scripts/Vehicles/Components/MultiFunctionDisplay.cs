using System.Collections.Generic;
using Assets.Scripts.Vehicles;
using Assets.Scripts.Vehicles.Components;
using UnityEditor;
using UnityEngine;

public class MultiFunctionDisplay : ComponentBase, ITickableComponent
{
    public enum MFDPage{
        TGP,
        RDR,
        HSD,
        SMS,
        WPN
    }
    MultiFunctionDisplay(VehicleBase vehicle, GameObject gameObject) : base(vehicle)
    {
        this.MFDCanvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/MFDCanvas.prefab");
        this.buttonTextsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/ButtonTexts.prefab");
        this.MFDCanvas = GameObject.Instantiate(MFDCanvasPrefab, gameObject.transform);
        this.buttonTexts = GameObject.Instantiate(buttonTextsPrefab, MFDCanvas.transform);


        this.TGPPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/TGP.prefab");
        this.RDRPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/RDR.prefab");
        this.HSDPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/HSD.prefab");
        this.SMSPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/SMS.prefab");
        this.WPNPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MFD/WPN.prefab");

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            this.sideButtons.Add(gameObject.transform.GetChild(i).gameObject);
            this.sideButtons[i].AddComponent<BoxCollider>();
        }

        SwitchToMFDPage(DefaultPage);
    }

    /// <summary>
    /// If sensor is the current sensor of interest. 
    /// <b>Default: false </b>
    /// </summary>
    public bool IsSensorOfInterest = false;
    private MFDPage currentPage;
    private GameObject MFDCanvasPrefab;
    private GameObject MFDCanvas;
    private GameObject buttonTextsPrefab;
    private GameObject buttonTexts;

    private GameObject TGPPrefab;
    private GameObject RDRPrefab;
    private GameObject HSDPrefab;
    private GameObject SMSPrefab;
    private GameObject WPNPrefab;
    private GameObject currentMFDPageObject;

    private List<GameObject> sideButtons;
    public MFDPage DefaultPage { get; set; }


    public bool PreTickComponent() => true;
    public bool PostTickComponent() 
    {
        switch (currentPage)
        {
            case MFDPage.TGP:
                
                break;
            case MFDPage.RDR:
                
                break;
            case MFDPage.HSD:
                
                break;
            case MFDPage.SMS:
                
                break;
            case MFDPage.WPN:
                
                break;
        }
        
        if (!IsSensorOfInterest)
        {
            return true;
        }
        //Todo: handle TDC slewing
        return true;
    }

    private void SwitchToMFDPage(MFDPage newPage)
    {
        switch(newPage)
        {
            case MFDPage.TGP:
                currentMFDPageObject = GameObject.Instantiate(TGPPrefab, MFDCanvas.transform);
                break;
            case MFDPage.RDR:
                currentMFDPageObject = GameObject.Instantiate(RDRPrefab, MFDCanvas.transform);
                break;
            case MFDPage.HSD:
                currentMFDPageObject = GameObject.Instantiate(HSDPrefab, MFDCanvas.transform);
                break;
            case MFDPage.SMS:
                currentMFDPageObject = GameObject.Instantiate(SMSPrefab, MFDCanvas.transform);
                break;
            case MFDPage.WPN:
                currentMFDPageObject = GameObject.Instantiate(WPNPrefab, MFDCanvas.transform);
                break;
        }
        currentPage = newPage;
    }
}