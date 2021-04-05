using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;


namespace minebot2021
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Console.WriteLine("Hello World!");
            //minebot newbot = new minebot();
            MainAsync().Wait();
        }
        static async Task MainAsync()
        {
            minebot.Connect("127.0.0.1", "hello");
        }

    }
    class minebot
    {
        public static async void Connect(String server, String message)
        {
            try
            {
                Int32 port = 25565;
                TcpClient client = new TcpClient(server, port);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message); //this will be replaced later

                var bytelist = new List<byte>();
                bytelist.Add(0x00);
                bytelist.Add(0x07);
                string username = "dickbot".PadRight(64);
                foreach (char p in username)
                {
                    bytelist.Add((byte)p);
                }
                string mppass = "asdf".PadRight(64);
                foreach (char p in mppass)
                {
                    bytelist.Add((byte)p);
                }
                bytelist.Add(0x00);
                data = bytelist.ToArray();

                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);
                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);


                //Thread.Sleep(2000);


                setblock(ref stream, 64, 47, 64);

                //position/orientation update
                short x = 0;
                short y = 0;
                short z = 0;
                UInt16 pitch = 0;
                UInt16 yaw = 0;
                
                bytelist = new List<byte>();
                bytelist.Add(0x08); //packet id
                bytelist.Add(0xFF); //self
                bytelist.Add((byte)(x*32>>8));
                bytelist.Add((byte)(x*32));
                bytelist.Add((byte)(y*32>>8));
                bytelist.Add((byte)(y*32));
                bytelist.Add((byte)(z*32>>8));
                bytelist.Add((byte)(z*32));
                bytelist.Add((byte)yaw);
                bytelist.Add((byte)pitch);
                data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);


                bytelist = new List<byte>();
                bytelist.Add(0x0d);
                bytelist.Add(0xFF);
                string chat = "hello, I am dickbot".PadRight(64);
                foreach (char p in chat)
                {
                    bytelist.Add((byte)p);
                }
                data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);


                //stream.Close();
                //client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
        public static void handshake(ref NetworkStream stream){
        }
        public static void setblock(ref NetworkStream stream, short x, short y, short z)
        {
                var bytelist = new List<byte>();
                bytelist = new List<byte>();
                bytelist.Add(0x05);
                bytelist.Add((byte)(x>>8));
                bytelist.Add((byte)x);
                bytelist.Add((byte)(y>>8));
                bytelist.Add((byte)y);
                bytelist.Add((byte)(z>>8));
                bytelist.Add((byte)z);
                bytelist.Add(0x01); //build block
                bytelist.Add(0x20); //stone
                var data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);
        }
    }
}


//https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-5.0
//shift alt F ->to format document

//c# console async thread
//https://stackoverflow.com/questions/17630506/async-at-console-app-in-c