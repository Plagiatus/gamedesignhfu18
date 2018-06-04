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
    public class UdpServer
    {
        static CircleOfAction[] circles;
        static string playersJsonPath = "players.json";
        static string circlesJsonPath = "circles.json";
        static int nextFreeID = -1;
        public static void Main()
        {
            //load different data
            List<PlayerLocation> playerLocations = new List<PlayerLocation>();
            circles = LoadCircles();
            List<Player> players = LoadPlayers();
            //get latest ID
            foreach(Player p in players){
                if(nextFreeID < p.id){
                    nextFreeID = p.id;
                }
            }
            nextFreeID++;

            //start the coroutine that saves the current player list to the file structure.

            // Player p1 = new Player {id = 0, name = "Lukas", password = "123456", level = 15, energy = 100, xp = 20};
            // Player p2 = new Player {id = 1, name = "Marlene", password = "987654", level = 9, energy = 90, xp = 8};
            // players.Add(p1);
            // players.Add(p2);


            /*
            PointOfAction poa = new PointOfAction() {name = "Furtwangen", attack = Attacks.None, position = new Vector2(45.1f, 8.2f), power = 0};
            string json = JsonConvert.SerializeObject(poa);
            // Console.WriteLine(json);
            var poa2 = JsonConvert.DeserializeObject<Object>(json);
            // Console.WriteLine($"Deserialized: Name={poa2.name}, Attack={poa2.attack}, Position={poa2.position}");
            // Console.WriteLine();
    	    */

            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
    
            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);
    
            newsock.Bind(ipep);
            Console.WriteLine("Waiting for a client on 9050...");
    
            
    
            // recv = newsock.ReceiveFrom(data, ref Remote);
    
            // Console.WriteLine("Message received from {0}:", Remote.ToString());
            // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
    
            // string welcome = "Welcome to my test server";
            // data = Encoding.ASCII.GetBytes(welcome);
            // newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint Remote = (EndPoint)(sender);
                data = new byte[1024];
                recv = newsock.ReceiveFrom(data, ref Remote);
                string sentString = Encoding.ASCII.GetString(data, 0, recv);
                string[] splitStrings = sentString.Split(";");
                if(splitStrings.Length == 2){
                    ServerRequest sr = JsonConvert.DeserializeObject<ServerRequest>(splitStrings[0]);
                    switch (sr.request)
                    {
                        case (ServerRequestType.SendPosition):
                            PlayerLocation newLocation = JsonConvert.DeserializeObject<PlayerLocation>(splitStrings[1]);
                            // Console.WriteLine(splitStrings[1]);
                            bool found = false;
                            for (int i = 0; i < playerLocations.Count; i++){
                                if(playerLocations[i].id == newLocation.id){
                                    playerLocations[i] = newLocation;
                                    found = true;
                                    break;
                                }
                            }
                            if(!found){
                                playerLocations.Add(newLocation);
                            }
                            Console.WriteLine("SendPosition from " + Remote.ToString().Split(":")[0] + ": " + newLocation.position);
                            newsock.SendTo(Encoding.ASCII.GetBytes("ACK"), SocketFlags.None, Remote);
                        break;    

                        case (ServerRequestType.RecieveCircle):
                            Vector2 playerPos = JsonConvert.DeserializeObject<Vector2>(splitStrings[1]);
                            Console.WriteLine("RecieveCircle from " + Remote.ToString().Split(":")[0]);
                            newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(getClosestCircle(playerPos))), SocketFlags.None, Remote);
                        break;

                        case (ServerRequestType.RecievePositions):
                            Console.WriteLine("RecievePositions from " + Remote.ToString().Split(":")[0]);
                            Vector2 pos = JsonConvert.DeserializeObject<Vector2>(splitStrings[1]);
                            //get rid of the old locations
                            // Console.WriteLine(DateTime.Now);
                            for(int i = 0; i < playerLocations.Count; i++){
                                // Console.WriteLine("Parsed: " + DateTime.Parse(playerLocations[i].timestamp));
                                if(DateTime.Parse(playerLocations[i].timestamp).AddMinutes(5).CompareTo(DateTime.UtcNow) < 0)
                                {
                                    playerLocations.RemoveAt(i);
                                    i--;
                                }
                            }
                            //get rid of the locations too far away
                            List<PlayerLocation> locationsToSend = playerLocations;
                            for(int i = 0; i < locationsToSend.Count; i++){
                                if(calculateDistanceinKm(locationsToSend[i].position, pos)> 5){
                                    locationsToSend.RemoveAt(i);
                                    i--;
                                }
                            }
                            newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(locationsToSend)), SocketFlags.None, Remote);
                        break;

                        case (ServerRequestType.LogIn):
                            LoginCredentials login = JsonConvert.DeserializeObject<LoginCredentials>(splitStrings[1]);
                            Console.Write("Log attempt for " + login.name + " with pw: " + login.password);
                            bool foundLogin = false;
                            foreach(Player p in players){
                                if (p.name.ToLower() == login.name.ToLower() && p.password == login.password){
                                    newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(p)), SocketFlags.None, Remote);
                                    foundLogin = true;
                                    Console.WriteLine(" - success");
                                    break;
                                }
                            }
                            if(!foundLogin){
                                newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(null)), SocketFlags.None, Remote);
                                Console.WriteLine(" - failed");
                            }
                        break;

                        case (ServerRequestType.NewPlayer):
                            LoginCredentials newLogin = JsonConvert.DeserializeObject<LoginCredentials>(splitStrings[1]);
                            bool nameExists = false;
                            foreach(Player p in players){
                                if(p.name.ToLower() == newLogin.name.ToLower()){
                                    nameExists = true;
                                    newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject("Name already exists. Please choose a different one.")), SocketFlags.None, Remote);
                                    break;
                                }
                            }
                            if(!nameExists){
                                Player newPlayer = new Player(newLogin.name, newLogin.password, nextFreeID);
                                newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(newPlayer)), SocketFlags.None, Remote);
                                players.Add(newPlayer);
                                SavePlayers(players);
                                Console.WriteLine("New Player: " + newPlayer.name);
                            }
                        break;

                        case(ServerRequestType.UpdatePlayer):
                            Player updatedPlayer = JsonConvert.DeserializeObject<Player>(splitStrings[1]);
                            foreach(Player p in players){
                                if(p.id == updatedPlayer.id && p.name == updatedPlayer.name && p.password == updatedPlayer.password){
                                    players.Remove(p);
                                    players.Add(updatedPlayer);
                                    SavePlayers(players);
                                    break;
                                }
                            }
                            newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject("ACK")), SocketFlags.None, Remote);
                        break;
                        default:
                        break;
                    }
                } else if (splitStrings.Length == 1)
                {   
                    ServerRequest sr = JsonConvert.DeserializeObject<ServerRequest>(splitStrings[0]);
                    switch (sr.request)
                    {
                        
                        default:
                        break;
                    }
                }
                // Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

                // newsock.SendTo(data, recv, SocketFlags.None, Remote);
            }
        }

        // get the circle closest to the player and return it
        static CircleOfAction getClosestCircle(Vector2 position){
            float smallestDistance = 1000f;
            CircleOfAction closestCircle = null;
            float distance;

            foreach(CircleOfAction circle in circles) {
                distance = calculateDistanceinKm(circle.center, position);
                if(distance < smallestDistance){
                    smallestDistance = distance;
                    closestCircle = circle;
                }
            }

            return closestCircle;
        }

        //support function for distance calculation
        static float degreesToRadians(float degrees){
            return degrees * (float) Math.PI / 180;
        }
        
        //calculates the distance between two points on the earth
        static float calculateDistanceinKm(Vector2 pos1, Vector2 pos2){
            float earthRadius = 6371f;
            Vector2 delta = new Vector2(degreesToRadians(pos2.X - pos1.X), degreesToRadians(pos2.Y - pos1.Y));
            float lat1 = degreesToRadians(pos1.X);
            float lat2 = degreesToRadians(pos2.X);
            double a = Math.Sin(delta.X / 2) * Math.Sin(delta.X /2) +
                        Math.Sin(delta.Y / 2) * Math.Sin(delta.Y / 2) * Math.Cos(lat1) * Math.Cos(lat2);
    	    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));

            return earthRadius * (float) c;
        }
        
        public static CircleOfAction[] LoadCircles(){

            //load circles from json file
            string readResult = String.Empty;
            string writeResult = String.Empty;
            using (StreamReader sr = new StreamReader(circlesJsonPath)){
                var json = sr.ReadToEnd();
                CircleOfAction[] jsonObj = JsonConvert.DeserializeObject<CircleOfAction[]>(json);
                return jsonObj;
            }

            /*
            CircleOfAction fuwa = new CircleOfAction();
            fuwa.center = new Vector2(48.05078984227376f,8.205344080924988f);
            fuwa.id = 0;
            fuwa.radius = 1500f;
            fuwa.name = "Furtwangen";

            PointOfAction poa1 = new PointOfAction{attack = Attacks.None, name = "A Bau", power = 0, position = new Vector2(48.051491968664195f,8.207626640796661f)};
            PointOfAction poa2 = new PointOfAction{attack = Attacks.Wind, name = "I Bau", power = 20, position = new Vector2(48.04993637648092f,8.21077823638916f)};
            PointOfAction poa3 = new PointOfAction{attack = Attacks.Rain, name = "Engel", power = 50, position = new Vector2(48.049567015050954f,8.206738829612732f)};

            fuwa.pointsOfAction = new PointOfAction[] {poa1, poa2, poa3};

            CircleOfAction voeba = new CircleOfAction();
            voeba.center = new Vector2(48.04483680295032f,8.305728435516357f);
            voeba.id = 1;
            voeba.radius = 1000f;
            voeba.name = "Vöhrenbach";

            PointOfAction poa4 = new PointOfAction{attack = Attacks.None, name = "Ochsen", power = 0, position = new Vector2(48.04599155977438f,8.303295676369771f)};
            PointOfAction poa5 = new PointOfAction{attack = Attacks.Wind, name = "Schwimmbad", power = 20, position = new Vector2(48.04513805761341f,8.298733234405518f)};
            PointOfAction poa6 = new PointOfAction{attack = Attacks.Rain, name = "Sparkasse", power = 50, position = new Vector2(48.044363399205636f,8.306286334991455f)};

            voeba.pointsOfAction = new PointOfAction[] {poa4, poa5, poa6};

            CircleOfAction[] circles = new CircleOfAction[] {fuwa, voeba};
            return circles;
            */
        }

        static List<Player> LoadPlayers(){
            //load players from json file
            string readResult = String.Empty;
            string writeResult = String.Empty;
            using (StreamReader sr = new StreamReader(playersJsonPath)){
                var json = sr.ReadToEnd();
                List<Player> jsonObj = JsonConvert.DeserializeObject<List<Player>>(json);
                return jsonObj;
            }
        }

        static void SavePlayers(List<Player> players){
            string input = JsonConvert.SerializeObject(players);
            File.WriteAllText(playersJsonPath,input);
        }
    }


    [Serializable]
    public class CircleOfAction{
        public string name;
        public long id;
        public Vector2 center;
        public float radius;
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
    public struct ServerRequest{
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
