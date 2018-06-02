﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Numerics;

namespace server
{
    public class UdpServer
    {
        public static void Main()
        {
            int recv;
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
    
            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);
    
            newsock.Bind(ipep);
            Console.WriteLine("Waiting for a client...");
    
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
    
            recv = newsock.ReceiveFrom(data, ref Remote);
    
            Console.WriteLine("Message received from {0}:", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
    
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                data = new byte[1024];
                recv = newsock.ReceiveFrom(data, ref Remote);
    
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                newsock.SendTo(data, recv, SocketFlags.None, Remote);
            }
        }
    }

    [Serializable]
    public class CircleOfAction{
        public Vector2 center { get; set; }
        public float radius { get; set; }
        public long id { get; set; }
        public PointOfAction[] pointsOfAction {get; set;}

    }

    [Serializable]
    public class PointOfAction {
        public Vector2 position;
        public string name;
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
}