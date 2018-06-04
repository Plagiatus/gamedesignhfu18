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

    public static Vector3 LocationToGamePosition(Vector2 location, GameController GC){
        Vector2 gamePos = WorldToTilePos(location.x, location.y, GC.CurrentZoom);
        // Debug.Log("center: " + location + "gamePos: " + gamePos + ", OsmTilePos: " + (GC.CurrentGpsPosition.OsmTilePosition + new Vector2(0.5f, 0.5f)));
        Vector2 difference = gamePos - (GC.CurrentGpsPosition.OsmTilePosition + new Vector2(0.5f, 0.5f) );
        
        Vector3 newPos = new Vector3(difference.x * Config.TileSizeInGame, 0.1f, difference.y * Config.TileSizeInGame * -1);
        return newPos;
    }

    [Serializable]
    public class CircleOfAction{
        public string name;
        public Vector2 center;
        public float radius;
        public long id;
        public PointOfAction[] pointsOfAction;
        public float status;

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
        RecieveCircle,
        NewPlayer,
        LogIn,
        UpdatePlayer
    }

    public class ServerRequest{
        public ServerRequestType request;
    }

    public class PlayerLocation{
        public Vector2 position;
        public long id;
        public string name;
        public string timestamp;
    }

    public class Player{
        public int id;
        public string name;
        public string password;
        public int level;
        public int energy;
        public int xp;
        public Player(string _name, string _pw, int _id){
            this.name = _name;
            this.password = _pw;
            this.id = _id;
            this.level = 1;
            this.energy = 10;
            this.xp = 0;
        }
    }

    public class LoginCredentials{
        public string name;
        public string password;
    }

}
