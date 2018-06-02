using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {

	public class LocationData
	{
		public double Timestamp { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float HorizontalAccuracy { get; set; }
        public float VerticalAccuracy { get; set; }
		public string Status { get; set; }	

		public Vector2 OsmTilePosition { get; set; }
		public Vector2 OsmOnTilePosition { get; set; }
	}

	public static Vector2 WorldToTilePos(float lat, float lon, int zoom)
    {
        Vector2 p = new Vector2
        {
            x = (lon + 180.0f) / 360.0f * (1 << zoom),
            y = (1.0f - Mathf.Log(Mathf.Tan(lat * Mathf.PI / 180.0f) + 1.0f / Mathf.Cos(lat * Mathf.PI / 180.0f)) / Mathf.PI) / 2.0f * (1 << zoom)
        };

        return p;
    }

    public static Vector3 TileToGamePosition(Vector2 pos)
    {
        float TileSizeInGame = 10;      //size of a single Quad in the Map Object
        Vector3 ret = new Vector3(TileSizeInGame / 2 - pos.x * TileSizeInGame, 0, (pos.y * TileSizeInGame) - (TileSizeInGame / 2));
        return ret;
    }

}
