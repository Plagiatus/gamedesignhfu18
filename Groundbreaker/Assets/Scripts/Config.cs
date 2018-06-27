using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config {

	public static bool UseDebugGpsPosition;
	public static Helper.LocationData DebugGpsPosition;
	public static float TimeToGoodGpsFix;
	
	public static Vector3 MaxCameraOffset;

        public static string MapLoaderBaseUrl = "http://a.tile.openstreetmap.org/";
        public static float TileSizeInGame = 10;      //size of a single Quad in the Map Object
        public static float TileSizeInRL = 150f;
        public static string MapCacheFolderName = "MapCache";
        public static string ServerIP = "141.28.105.206";
        // public static string ServerIP = "127.0.0.1";
        public static int ServerPort = 9050;


	static Config()
	{
		DebugGpsPosition = new Helper.LocationData();
                DebugGpsPosition.Latitude = 48.05080f;
                DebugGpsPosition.Longitude = 8.20834f;
                DebugGpsPosition.Altitude = 800;
                DebugGpsPosition.HorizontalAccuracy = 10;
                DebugGpsPosition.VerticalAccuracy = 10;
                DebugGpsPosition.Timestamp = 5;
                DebugGpsPosition.Status = "Debugging";
                DebugGpsPosition.HeadingDirection = 0;

                MaxCameraOffset = new Vector3(1f, 1f, 0.5f);

                #if UNITY_EDITOR
                UseDebugGpsPosition = true;
                TimeToGoodGpsFix = 1;
                Debug.Log("Using debug position.");
                #elif UNITY_ANDROID
                UseDebugGpsPosition = false;
                TimeToGoodGpsFix = 10;
                #endif
	}
}
