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
        static CircleOfAction[] circles = LoadCircles();
        public static void Main()
        {
            IDictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            // CircleOfAction[]  circles = LoadCircles();
            // Console.WriteLine(JsonConvert.SerializeObject(circles));


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
                            Vector2 newPos = JsonConvert.DeserializeObject<Vector2>(splitStrings[1]);
                            positions[Remote.ToString().Split(":")[0]] = newPos;
                            Console.WriteLine("SendPosition from " + Remote.ToString().Split(":")[0] + ": " + newPos);
                            newsock.SendTo(Encoding.ASCII.GetBytes("ACK"), SocketFlags.None, Remote);
                        break;    

                        case (ServerRequestType.RecieveCircle):
                            Vector2 playerPos = JsonConvert.DeserializeObject<Vector2>(splitStrings[1]);
                            Console.WriteLine("RecieveCircle");
                            newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(getClosestCircle(playerPos))), SocketFlags.None, Remote);
                        break;

                        default:
                        break;
                    }
                } else if (splitStrings.Length == 1)
                {   
                    ServerRequest sr = JsonConvert.DeserializeObject<ServerRequest>(splitStrings[0]);
                    switch (sr.request)
                    {
                        case (ServerRequestType.RecievePositions):
                            Console.WriteLine("RecievePositions from " + Remote.ToString().Split(":")[0]);
                            newsock.SendTo(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(positions)), SocketFlags.None, Remote);
                        break;
                        default:
                        break;
                    }
                }
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

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
            //TODO: Maybe load this from a JSON File or something.
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
        }
    }


    [Serializable]
    public class CircleOfAction{
        public string name;
        public long id;
        public Vector2 center;
        public float radius;
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
    public struct ServerRequest{
        public ServerRequestType request;
    }
}
