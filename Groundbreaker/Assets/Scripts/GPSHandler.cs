﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSHandler : MonoBehaviour {

	private GameController GC;

	public Text gpsText;
	public static GPSHandler Instance {get; set;}

	void Start()
	{
		if(Instance == null) {
			Instance = this;
			StartCoroutine(StartLocationService());
		}
	}

	// void OnLevelWasLoaded(){
		// gpsText = GameObject.Find("GPSText").GetComponent<Text>();
	// }

	IEnumerator StartLocationService()
	{
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			// Debug.Log(GC);
		} catch (Exception)
		{
			throw;
		}

		if (Config.UseDebugGpsPosition)
		{
			GC.CurrentGpsPosition = Config.DebugGpsPosition;
		}
		else
		{
			Debug.Log("Starting GPS search");
			if(!Input.location.isEnabledByUser)
			{
				Debug.Log("GPS not enabled");
				yield break;
			}
			
			Input.location.Start();
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			{
				yield return new WaitForSeconds(1);
				maxWait--;
				Debug.Log("Waited " + maxWait);
			}

			if (maxWait <= 0)
			{
				Debug.Log("Timed Out");
				yield break;
			}

			if (Input.location.status == LocationServiceStatus.Failed)
			{
				Debug.Log("Cannot determine Location");
				yield break;
			}
			else
			{
				Debug.Log("Location: Lat:" + Input.location.lastData.latitude + ", Long:" + Input.location.lastData.longitude + ", Time:" + Input.location.lastData.timestamp + ", Accu:" + Input.location.lastData.horizontalAccuracy);
			}
		}


	}

	void Update()
	{
		if (!Config.UseDebugGpsPosition)
		{
			GC.CurrentGpsPosition.Latitude = Input.location.lastData.latitude;
            GC.CurrentGpsPosition.Longitude = Input.location.lastData.longitude;
            GC.CurrentGpsPosition.Altitude = Input.location.lastData.altitude;
            GC.CurrentGpsPosition.HorizontalAccuracy = Input.location.lastData.horizontalAccuracy;
            GC.CurrentGpsPosition.VerticalAccuracy = Input.location.lastData.verticalAccuracy;
            GC.CurrentGpsPosition.Timestamp = Input.location.lastData.timestamp;
			GC.CurrentGpsPosition.Status = Input.location.status.ToString();
			GC.CurrentGpsPosition.HeadingDirection = Input.compass.trueHeading;
		}
		else
		{
			GC.CurrentGpsPosition = Config.DebugGpsPosition;
		}

		Vector2 tilePos = Helper.WorldToTilePos(GC.CurrentGpsPosition.Latitude, GC.CurrentGpsPosition.Longitude, GC.CurrentZoom);
		GC.CurrentGpsPosition.OsmTilePosition = new Vector2(Mathf.FloorToInt(tilePos.x), Mathf.FloorToInt(tilePos.y));

		GC.CurrentGpsPosition.OsmOnTilePosition = new Vector2(tilePos.x - GC.CurrentGpsPosition.OsmTilePosition.x, tilePos.y - GC.CurrentGpsPosition.OsmTilePosition.y);

		// Debug.Log("Tile: " + GC.CurrentGpsPosition.OsmTilePosition.ToString() + "; PositionOnTile: " + GC.CurrentGpsPosition.OsmOnTilePosition.ToString());
		// gpsText.text = GC.CurrentGpsPosition.Latitude.ToString() + ", " + GC.CurrentGpsPosition.Longitude.ToString();
		if(gpsText != null){
			gpsText.text = GameController.Instance.CurrentGpsPosition.HeadingDirection.ToString();
		}
	}

}
