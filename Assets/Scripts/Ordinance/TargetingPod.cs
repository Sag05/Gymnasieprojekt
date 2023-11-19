using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Ordinance;
using UnityEngine;

public class TargetingPod : OrdinanceBase
{
    private TargetingPodConfig targetingPodConfiguration;

    void Start()
    {
        targetingPodConfiguration = ConfigurationReader.LoadTargetingPod(@".\configs\ordinance\" + gameObject.name + ".cfg");
        base.Mass = this.targetingPodConfiguration.Mass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
