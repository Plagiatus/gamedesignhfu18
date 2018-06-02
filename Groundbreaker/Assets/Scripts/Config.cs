using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config {

	public static bool UseDebugGpsPosition;
	public static Helper.LocationData DebugGpsPosition;
	public static float TimeToGoodGpsFix;
	
	public static Vector3 MaxCameraOffset;

        public static string MapLoaderBaseUrl = "http://a.tile.openstreetmap.org/";


	static Config()
	{
		DebugGpsPosition = new Helper.LocationData();
                DebugGpsPosition.Latitude = 48.05080f;
                DebugGpsPosition.Longitude = 8.20934f;
                DebugGpsPosition.Altitude = 800;
                DebugGpsPosition.HorizontalAccuracy = 10;
                DebugGpsPosition.VerticalAccuracy = 10;
                DebugGpsPosition.Timestamp = 5;
                DebugGpsPosition.Status = "Debugging";

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
