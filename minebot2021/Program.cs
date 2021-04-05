using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;


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
            minebot.Connect("127.0.0.1");
            //return;
        }

    }
    class minebot
    {
        static NetworkStream stream;
        public static async void Connect(String serverip)
        {
            try
            {
                Int32 port = 25565;
                TcpClient client = new TcpClient(serverip, port);
                NetworkStream stream = client.GetStream();
                

                handshake(ref stream);
                var reader = readstream();
                setblock(ref stream, 64, 47, 64);
                chat(ref stream, "hello, I am dickbot");
                move(ref stream, 0, 0, 0, 0, 0);
                
                while(true)
                {
                   var msg = Console.ReadLine();
                   chat(ref stream, msg);
                   //Console.WriteLine(msg);
                }


                //Thread.Sleep(2000);
                //Task.Delay(1000);
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

            //Console.WriteLine("\n Press Enter to continue...");
        }
        static async Task<string> readstream()
        {
            using var reader = new StreamReader(stream);
            stream.ReadTimeout = 2*1000;
            Console.WriteLine("beginning read");
            //this code doesn't work yet
            while(true)
            {
                string response = await reader.ReadToEndAsync();
                Console.WriteLine(response);
            }
            return "yes";
        }
        static void handshake(ref NetworkStream stream)
        {
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
                var data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);

                //insert the following into handshake() also:
                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
        }
        static void setblock(ref NetworkStream stream, short x, short y, short z)
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
        static void chat(ref NetworkStream stream, string chat)
        {
                var bytelist = new List<byte>();
                bytelist = new List<byte>();
                bytelist.Add(0x0d);
                bytelist.Add(0xFF);
                chat = chat.PadRight(64);
                //change this
                foreach (char p in chat)
                {
                    bytelist.Add((byte)p);
                }
                var data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);
        }
        static void move(ref NetworkStream stream, short x, short y, short z, UInt16 pitch, UInt16 yaw)
        {
                //position/orientation update
                var bytelist = new List<byte>();
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
                var data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);
        }
    }
}


//https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-5.0
//shift alt F ->to format document

//c# console async thread
//https://stackoverflow.com/questions/17630506/async-at-console-app-in-c


//https://zetcode.com/csharp/tcpclient/
