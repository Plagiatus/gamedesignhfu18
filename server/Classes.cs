using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Numerics;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace server
{
   
	[Serializable]
	public class CircleOfAction{
		public string name;
		public long id;
		public Vector2 center;
		public float radius;
		public PointOfAction[] pointsOfAction;
		public float status = 50f;

	}

	[Serializable]
	public class PointOfAction {
		public long id;
		public long circleID;
		public string name;
		public Vector2 position;
		public Attacks attack;
		public DateTime creationDate;
		public int power;
		public long ownerID;
		public List<long> attackers;

		override public string ToString()
		{
			string ret = "";
			ret += "(" + name + ", " + position.ToString() + ", " + attack + ", " + power + ")";
			return ret;

			// return JsonUtility.ToJson(this);
		}
	}

	public class PointAttack{
		public long circleID;
		public long pointID;
		public int energy;
	}

	public class NewPointAttack{
		public long circleID;
		public long pointID;
		public Attacks attack;
		public long playerID;
	}

	public class PointDefense : PointAttack{}

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
		UpdatePlayer,
		AttackPoint,
		CreatePoint,
		SupportPoint
	}
	public struct ServerRequest{
		public ServerRequestType request;
	}

	public class PlayerLocationWrapper{
		public List<PlayerLocation> list;
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
