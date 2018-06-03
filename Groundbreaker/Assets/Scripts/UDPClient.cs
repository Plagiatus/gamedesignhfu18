using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
 
	public class UDPClient : NetworkBehaviour
	{
	
		public Transform Cube;
		private GameController GC;

		void Start()
		{
			try 
			{
				GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			} catch (Exception)
			{
				throw;
			}
			GC.CurrentGpsPosition = Config.DebugGpsPosition;
			Helper.PointOfAction poa = new Helper.PointOfAction() {name = "Furtwangen", attack = Helper.Attacks.None, position = new Vector2(GC.CurrentGpsPosition.Latitude, GC.CurrentGpsPosition.Longitude), power = 0};
			
			// print (Cube.position);
			byte[] data = new byte[1024];
			IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
	
			Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	
			// string welcome = "Hello, what's up?";
			Debug.Log(poa);
			Helper.ServerRequest sr = new Helper.ServerRequest() {request = Helper.ServerRequestType.SendPosition};
			data = Encoding.ASCII.GetBytes(JsonUtility.ToJson(sr) +";"+JsonUtility.ToJson(poa));
			Debug.Log(data);
			server.SendTo(data, data.Length, SocketFlags.None, ipep);
	
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint tmpRemote = (EndPoint)sender;
	
			data = new byte[1024];
			int recv = server.ReceiveFrom(data, ref tmpRemote);
	
			Debug.Log (String.Format("Message received from {0}:", tmpRemote.ToString()));
			Debug.Log (Encoding.ASCII.GetString(data, 0, recv));
			Debug.Log (JsonUtility.FromJson<Helper.ServerRequest>(Encoding.ASCII.GetString(data, 0, recv)));
	
			// server.SendTo(Encoding.ASCII.GetBytes(Cube.position.ToString()), tmpRemote);
	
	
			Console.WriteLine("Stopping client");
			server.Close();
	
		}
	}