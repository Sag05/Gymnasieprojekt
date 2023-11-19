using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts;
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

    private Vector3 rotation;
    private Vector3 position;
    public bool IsTracking { get; private set; }
    private int storedId;

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
    
    /// <summary>
    /// Returns the tracking data from FreeTrack, in the form of a <see cref="DoubleVector3"/>, where vector1 is rotation and vector2 is position.
    /// </summary>
    /// <returns></returns>
    public DoubleVector3 GetTrackingData()
    {
        //Pitch = x, Yaw = y, Roll = z
        rotation = new Vector3(FreeTrackDataRef.Pitch, FreeTrackDataRef.Yaw, FreeTrackDataRef.Roll);
        rotation *= Mathf.Rad2Deg;
        position = new Vector3(FreeTrackDataRef.X, FreeTrackDataRef.Y, FreeTrackDataRef.Z);
        DoubleVector3 output = new()
        {
            vector1 = rotation,
            vector2 = position
        };
        return output;
    }

    void Update()
    {
        //Debug.Log(FreeTrackDataRef.dataid);
        if (FreeTrackDataRef.dataid == storedId)
        { 
            //Debug.Log("Tracking disabled");
            IsTracking = false;
        }
        else
        {
            IsTracking = true;
        }

        FreeTrackClientDll64.FTGetData(ref FreeTrackDataRef);
        storedId = FreeTrackDataRef.dataid;
    }
}
