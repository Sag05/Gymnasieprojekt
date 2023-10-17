using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FreeTrackClientDll64 : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FreeTrackData
    {
        public int dataid;
        public int camwidth, camheight;
        public Single Yaw, Pitch, Roll, X, Y, Z;
        public Single RawYaw, RawPitch, RawRoll;
        public Single RawX, RawY, RawZ;
        public Single x1, y1, x2, y2, x3, y3, x4, y4;
    }

    [DllImport("FreeTrackClient64")]
    public static extern bool FTGetData(ref FreeTrackData data);

    [DllImport("FreeTrackClient64")]
    public static extern string FTGetDllVersion();

    [DllImport("FreeTrackClient64")]
    public static extern void FTReportID(Int32 name);

    [DllImport("FreeTrackClient64")]
    public static extern string FTProvider();

    FreeTrackClientDll64.FreeTrackData FreeTrackDataRef;

    public Vector3 rotation;
    public Vector3 position;
    public bool tracking;
    int storedId;

    void Start()
    {
        FreeTrackDataRef = new FreeTrackClientDll64.FreeTrackData();
        if (!FreeTrackClientDll64.FTGetData(ref FreeTrackDataRef))
        {
            Debug.Log("FTGetData returned false. FreeTrack likely not working.");
            return;
        }

        storedId = FreeTrackDataRef.dataid;
    }

    void Update()
    {
        //Debug.Log(FreeTrackDataRef.dataid);
        if (FreeTrackDataRef.dataid == storedId)
        { 
            //Debug.Log("Tracking disabled");
            tracking = false;
        }
        else
        {
            tracking = true;
        }

        FreeTrackClientDll64.FTGetData(ref FreeTrackDataRef);

        //Pitch = x, Yaw = y, Roll = z
        rotation = new Vector3(FreeTrackDataRef.Pitch, FreeTrackDataRef.Yaw, FreeTrackDataRef.Roll);
        rotation *= Mathf.Rad2Deg;
        position = new Vector3(FreeTrackDataRef.X, FreeTrackDataRef.Y, FreeTrackDataRef.Z);

        storedId = FreeTrackDataRef.dataid;
    }
}
