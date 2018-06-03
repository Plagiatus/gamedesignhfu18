using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Collections;
 
	public class UDPClient : MonoBehaviour
	{
	
		public Transform Cube;
		private GameController GC;
		private IDictionary playerPositions;

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

			Helper.ServerRequest sendPos = new Helper.ServerRequest() { request = Helper.ServerRequestType.SendPosition };
			StartCoroutine(SendRequest<Vector2, string>(sendPos, new Vector2(UnityEngine.Random.value * 100, UnityEngine.Random.value * 100)));

			Helper.ServerRequest getPos = new Helper.ServerRequest() { request = Helper.ServerRequestType.RecievePositions };
			StartCoroutine(SendRequest<string, Dictionary<string, Vector2>>(getPos, null, true, (returnValue) => {
				foreach(KeyValuePair<string, Vector2> player in returnValue){
					Debug.Log("Player position: " + player.Value);
				}
			}));

			Helper.ServerRequest getCirc = new Helper.ServerRequest() { request = Helper.ServerRequestType.RecieveCircle };
			StartCoroutine(SendRequest<Vector2, Helper.CircleOfAction>(getCirc, new Vector2(48.044f, 8.305f), true, (returnValue) =>{
					Debug.Log(returnValue.name);
			}));

			/*
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
			*/
		}

		public IEnumerator SendRequest<TRequest, TAnswer>(Helper.ServerRequest request, TRequest data, bool expectAnswer = false, System.Action<TAnswer> answer = null){
			byte[] toSend = new byte[1024];
			IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(Config.ServerIP),Config.ServerPort);
			Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			toSend = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request));
			if(data == null || String.IsNullOrEmpty(data.ToString()))
			{
				toSend = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request));
			} else 
			{
				toSend = Encoding.ASCII.GetBytes(JsonUtility.ToJson(request) + ";" + JsonUtility.ToJson(data));
			}

			server.SendTo(toSend, toSend.Length, SocketFlags.None, ipep);

			if(expectAnswer)
			{
				byte[] toRecieve = new byte[1024];
				IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
				EndPoint tmpRemote = (EndPoint)sender;
	
				int recv = server.ReceiveFrom(toRecieve, ref tmpRemote);
				Debug.Log("Answer: " + Encoding.ASCII.GetString(toRecieve, 0, recv));
				if(answer != null){
					answer(JsonUtility.FromJson<TAnswer>(Encoding.ASCII.GetString(toRecieve, 0, recv)));
				}
			}
			server.Close();
			yield return null;
		}
	}