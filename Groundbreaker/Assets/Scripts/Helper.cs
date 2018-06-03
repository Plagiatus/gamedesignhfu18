using System.Collections;
using System.Collections.Generic;
using System;
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

        public float HeadingDirection {get; set;}
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
        
        Vector3 ret = new Vector3(Config.TileSizeInGame / 2 - pos.x * Config.TileSizeInGame, 0, (pos.y * Config.TileSizeInGame) - (Config.TileSizeInGame / 2));
        return ret;
    }

    [Serializable]
    public class CircleOfAction{
        public string name;
        public Vector2 center;
        public float radius;
        public long id;
        public PointOfAction[] pointsOfAction;

    }

    [Serializable]
    public class PointOfAction {
        public string name;
        public Vector2 position;
        public Attacks attack;
        public int power;

        override public string ToString()
        {
            string ret = "";
            ret += "(" + name + ", " + position.ToString() + ", " + attack + ", " + power + ")";
            return ret;

            // return JsonUtility.ToJson(this);
        }
    }

    public enum Attacks{
        None,
        Wind,
        Rain,
        Sandstorm,
        Frost,
        Fire,
        Earthquake,
        Tsunami,
        Vulcano
    }

    public enum ServerRequestType{
        SendPosition,
        RecievePositions,
        RecieveCircle
    }

    public class ServerRequest{
        public ServerRequestType request;
    }

}
