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
            //do I need this for taskless methods?
            MainAsync().Wait();
        }
        static async Task MainAsync()
        {
            var m = new minebot();

            m.setblock(64, 47, 64);
            m.chat("hello, I am dickbot");
            m.chat("/goto small");
            m.move(0, 0, 0, 0, 0);
            m.move(64, 47, 64, 0, 0);

            Thread movingthread = new Thread(m.movearound);
            movingthread.Start();

            Console.WriteLine("test");

            Thread writethread = new Thread(m.writechat);
            writethread.Start();
            
            Thread readthread = new Thread(m.readchat);
            readthread.Start();

        }

    }
    class minebot
    {
        Dictionary<byte,string> dict = new Dictionary<byte, string>
        {
        [0x00] = "Server Identification",
        [0x01] = "Ping",
        [0x02] = "Level Initialize",
        [0x03] = "Level Data Chunk",
        [0x04] = "Level Finalize",
        [0x06] = "Set Block",
        [0x07] = "Spawn Player",
        [0x08] = "Position and Orientation (Player Teleport)",
        [0x09] = "Position and Orientation Update",
        [0x0a] = "Position Update",
        [0x0b] = "Orientation Update",
        [0x0c] = "Despawn Player",
        [0x0d] = "Message",
        [0x0e] = "Disconnect player",
        [0x0f] = "Update user type",
        };
        Dictionary<byte, int> packetlength = new Dictionary<byte, int>
        {
            [0x00] = 131,
            [0x01] = 1,
            [0x02] = 1,
            [0x03] = 1028,
            [0x04] = 7,
            [0x06] = 8,
            [0x07] = 74,
            [0x08] = 10,
            [0x09] = 7,
            [0x0a] = 5,
            [0x0b] = 4,
            [0x0c] = 2,
            [0x0d] = 66,
            [0x0e] = 65,
            [0x0f] = 2
        };
        static TcpClient client;
        static NetworkStream stream;
        public minebot()
        {
            try
            {
                String serverip = "127.0.0.1";
                Int32 port = 25565;
                client = new TcpClient(serverip, port);
                stream = client.GetStream();
                //NetworkStream stream = client.GetStream();

                handshake(ref stream);
            }
            catch
            {

            }
                //Thread.Sleep(2000);
                //Task.Delay(1000);
                //stream.Close();
                //client.Close();
        }
        public void writechat() //async Task<bool> enablechat()
        {
            while(true)
                {
                   var msg = Console.ReadLine();
                   chat(msg);
                //    Thread.Sleep(100);
                   //Console.WriteLine(msg);
                }
        }
        public void readchat()
        {
            //insert await for client stream?

            //headerbyte
            //packetid
            StreamReader sr = new StreamReader(client.GetStream());
            string data;
            while ((data = sr.ReadLine()) != null)
            {
                try
                {
                var packettype = BitConverter.GetBytes(data[0])[0];
                var identifier = dict[packettype];
                Console.WriteLine("packet received: " + identifier + ", " + data);
                }
                catch
                {

                }
            }

        }
/*         public async Task<string> readstream()
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
        } */
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
