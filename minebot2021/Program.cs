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
            var m = new minebot();
            //m.Connect("127.0.0.1").Wait();

            m.setblock(64, 47, 64);
            m.chat("hello, I am dickbot");
            m.move(0, 0, 0, 0, 0);
            m.move(64, 47, 64, 0, 0);

            Thread movingthread = new Thread(m.movearound);
            movingthread.Start();

            Task<bool> ec = m.enablechat();
            
            // Task<bool> moving = m.movearound();
            // moving.Wait();

            //m.move(64, 50, 64, 0, 0);
        }

    }
    class minebot
    {
        static NetworkStream stream;
        public minebot()
        {
            try
            {
                String serverip = "127.0.0.1";
                Int32 port = 25565;
                TcpClient client = new TcpClient(serverip, port);
                stream = client.GetStream();
                //NetworkStream stream = client.GetStream();

                handshake(ref stream);
            }
            catch
            {

            }
                // while(true)
                // {
                //    var msg = Console.ReadLine();
                //    chat(ref stream, msg);
                //    //Console.WriteLine(msg);
                // }

                //Thread.Sleep(2000);
                //Task.Delay(1000);
                //stream.Close();
                //client.Close();

        }
        public async Task<bool> enablechat()
        {
            while(true)
                {
                   var msg = Console.ReadLine();
                   chat(msg);
                   //Console.WriteLine(msg);
                }
        }
        public async Task<string> readstream()
        {
            using var reader = new StreamReader(stream);
            stream.ReadTimeout = 2*1000;
            Console.WriteLine("beginning read");
            //this code doesn't work yet
            while(true)
            {
                string response = await reader.ReadToEndAsync();
                //string response = await reader.ReadBlock();
                Console.WriteLine(response);
            }
            return "yes";
        }
        public void movearound()
        {
            //move in an infinity symbol
            double t = 0;
            int i = 0;
            while(true)
            {
            // Thread.Sleep(500);
            // move(64,48,64,0,0);
            // Thread.Sleep(500);
            // move(64,47,64,0,0);
            if(t>=2*Math.PI){t=0;i=0;}
            double x1 = Math.Sin(t)*20+64;
            double y1 = Math.Sin(t)*Math.Cos(t)*20+64;
            move((short)x1,48,(short)y1,0,0);
            if(i%8==0)
            {
            //setblock((short)x1,48,(short)y1);
            }
            Thread.Sleep(1);
            t+=Math.PI/(64);
            }
        }
        public void handshake(ref NetworkStream stream)
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
        public void setblock(short x, short y, short z)
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
                bytelist.Add((byte)20); //glass
                var data = bytelist.ToArray();
                stream.Write(data, 0, data.Length);
        }
        public void chat(string chat)
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
        public void move(short x, short y, short z, UInt16 pitch, UInt16 yaw)
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
