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
		private GameController GC;
		private IDictionary playerPositions;

		public static UDPClient Instance;

		public delegate void ServerResponseLogin(Helper.Player p);
		public static event ServerResponseLogin OnLoginResult;
		public static event ServerResponseLogin OnRegisterResult;

		void Start()
		{

			Instance = this;
			try 
			{
				GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			} catch (Exception)
			{
				throw;
			}
			GC.CurrentGpsPosition = Config.DebugGpsPosition;

			// tryToLogin("Lukas", "1234567");

			//TODO: MAKE SURE THAT THE GAME DOESN'T CRASH IF SERVER CONNECTION CAN'T BE ESTABLISHED!
			

			// Helper.ServerRequest login = new Helper.ServerRequest() {request = Helper.ServerRequestType.LogIn};
			// StartCoroutine(SendRequest<Helper.LoginCredentials, Helper.Player>(login, new Helper.LoginCredentials(){name = "Lukas", password = "1234567"},true));
			
			// Helper.ServerRequest newplayer = new Helper.ServerRequest() {request = Helper.ServerRequestType.NewPlayer};
			// StartCoroutine(SendRequest<Helper.LoginCredentials, Helper.Player>(newplayer, new Helper.LoginCredentials(){name = "Lars", password = "1234567"},true));

			// Helper.ServerRequest updateplayer = new Helper.ServerRequest() {request = Helper.ServerRequestType.UpdatePlayer};
			// StartCoroutine(SendRequest<Helper.Player, string>(updateplayer, new Helper.Player("Lars","1234567",2) {xp = 100},true));

			// Helper.ServerRequest sendPos = new Helper.ServerRequest() {request = Helper.ServerRequestType.SendPosition};
			// Debug.Log(JsonUtility.ToJson(new Helper.PlayerLocation(){name = "Lars", id = 2, timestamp = DateTime.Now.ToString(), position = new Vector2(48.05f, 8.2f)}));
			// StartCoroutine(SendRequest<Helper.PlayerLocation, string>(sendPos, new Helper.PlayerLocation(){name = "Lars", id = 2, timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), position = new Vector2(48.05f, 8.2f)},true));
			// StartCoroutine(SendRequest<Helper.PlayerLocation, string>(sendPos, new Helper.PlayerLocation(){name = "Lukas", id = 0, timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), position = new Vector2(48.0501f, 8.202f)},true));

			// Helper.ServerRequest getPos = new Helper.ServerRequest() {request = Helper.ServerRequestType.RecievePositions};
			// StartCoroutine(SendRequest<Vector2,List<Helper.PlayerLocation>>(getPos, new Vector2(48.05f, 8.2f), true));


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

    public void tryToRegister(string name, string password)
    {
        Helper.ServerRequest tryRegister = new Helper.ServerRequest() {request = Helper.ServerRequestType.NewPlayer};
		Helper.LoginCredentials login = new Helper.LoginCredentials() {name = name, password = password};
		StartCoroutine(SendRequest<Helper.LoginCredentials, Helper.Player>(tryRegister, login, true, (returnValue) => {
				if(OnLoginResult != null){
					OnRegisterResult(returnValue);
				}
			}));
    }

    public void tryToLogin(string name, string password){
			//subscribe to the OnLoginResult event to get the result of this function

			Helper.ServerRequest tryLogin = new Helper.ServerRequest() { request = Helper.ServerRequestType.LogIn};
			Helper.LoginCredentials login = new Helper.LoginCredentials() {name = name, password = password};
			// System.Action<Helper.Player> ac = new System.Action<Helper.Player>(loginResult);
			StartCoroutine(SendRequest<Helper.LoginCredentials, Helper.Player>(tryLogin, login, true, (returnValue) => {
				if(OnLoginResult != null){
					OnLoginResult(returnValue);
				}
			}));
		}

		// public void loginResult(Helper.Player p){
		// 	if(p == null){
		// 		Debug.Log("login failed");
		// 	} else {
		// 		Debug.Log("login success " + p.name);
		// 	}
		// }

		public void startPeriodicRequests(){
			Debug.Log("startPeriodicRequests");
			StartCoroutine(PeriodicallySendPosition());
			StartCoroutine(PeriodicallyGetPlayerPositions());
			StartCoroutine(PeriodicallyCheckCircle());
		}

		IEnumerator PeriodicallySendPosition(){
			while(true)
			{
				yield return new WaitForSeconds(10);
				Helper.ServerRequest sendPos = new Helper.ServerRequest() { request = Helper.ServerRequestType.SendPosition };
				StartCoroutine(SendRequest<Helper.PlayerLocation, string>(sendPos, new Helper.PlayerLocation(){position = new Vector2(GameController.Instance.CurrentGpsPosition.Latitude, GameController.Instance.CurrentGpsPosition.Longitude), id = GameController.Instance.player.id, name = GameController.Instance.player.name, timestamp = DateTime.UtcNow.ToString()}));
			}
		}

		IEnumerator PeriodicallyCheckCircle(){
			while(true)
			{
				Helper.ServerRequest getCirc = new Helper.ServerRequest() { request = Helper.ServerRequestType.RecieveCircle };
				StartCoroutine(SendRequest<Vector2, Helper.CircleOfAction>(getCirc, new Vector3(GameController.Instance.CurrentGpsPosition.Latitude, GameController.Instance.CurrentGpsPosition.Longitude), true, (returnValue) =>{
					GameController.Instance.circle = returnValue;
				}));
				yield return new WaitForSeconds(60);
			}
		}

		IEnumerator PeriodicallyGetPlayerPositions(){
			while(true){
				yield return new WaitForSeconds(10);
				Helper.ServerRequest getPos = new Helper.ServerRequest() { request = Helper.ServerRequestType.RecievePositions };
				StartCoroutine(SendRequest<Vector2, Helper.PlayerLocationWrapper>(getPos, new Vector2(GameController.Instance.CurrentGpsPosition.Latitude, GameController.Instance.CurrentGpsPosition.Longitude), true, (returnValue) => {
					GC.playerPositions = returnValue.list;
					GameObject.FindGameObjectWithTag("Map").GetComponent<MapObjects>().positionPlayers(returnValue.list);
				}));
			}
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
				// Debug.Log("Answer: " + Encoding.ASCII.GetString(toRecieve, 0, recv));
				if(answer != null){
					if(Encoding.ASCII.GetString(toRecieve, 0, recv).Equals("null")){
						answer(default(TAnswer));
					} else {
						answer(JsonUtility.FromJson<TAnswer>(Encoding.ASCII.GetString(toRecieve, 0, recv).Replace("\"X\"", "\"x\"").Replace("\"Y\"", "\"y\"")));
					}
				}
				
			}
			server.Close();
			yield return null;
		}
	}