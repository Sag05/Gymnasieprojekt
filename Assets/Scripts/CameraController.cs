using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    FreeTrackClientDll64 freeTrackClientDll64;

    void Start()
    {
        freeTrackClientDll64 = gameObject.AddComponent<FreeTrackClientDll64>();
    }

    void Update()
    {
        if (freeTrackClientDll64.tracking)
        {
            transform.rotation = Quaternion.Euler(freeTrackClientDll64.rotation);
            transform.localPosition = freeTrackClientDll64.position;
        }
    }
}