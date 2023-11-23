using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        object a = new ConfigurationReaderV2.ConfigReader("./configs/example.cfg2").Read(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
