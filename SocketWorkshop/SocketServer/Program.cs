using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SocketServer
{
    class Program
    {
        //定义玩家初始血量和敌人初始血量
        public static int playerHP = 50;
        public static int enemyHP = 80;

        static void Main(string[] args)
        {
            //创建一个新的Socket
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //将该socket绑定到主机上面的某个端口
            socket.Bind(new IPEndPoint(IPAddress.Any, 4530));

            //启动监听，并且设置一个最大的队列长度
            socket.Listen(4);

            //开始接受客户端连接请求
            socket.BeginAccept(new AsyncCallback((ar) =>
            {
                
                var client = socket.EndAccept(ar);

                //给客户端发送一个欢迎消息
                client.Send(Encoding.Unicode.GetBytes("Hi there, I accept you request. " ));
                
                var timer = new System.Timers.Timer();
                timer.Interval = 5000D;
                timer.Enabled = true;
                timer.Elapsed += (o, a) =>
                {
                    //检测客户端Socket的状态
                    if (client.Connected)
                    {
                        try
                        {
                            //随机减去血量
                            Random rd1 = new Random();
                            int i1 = rd1.Next(1, 10);
                            playerHP = playerHP - i1;

                            Random rd2 = new Random();
                            int i2 = rd2.Next(1, 20);
                            enemyHP = enemyHP - i2;
                            if (playerHP >= 0 && enemyHP >= 0)
                            {
                                client.Send(Encoding.Unicode.GetBytes("playerHP= " + playerHP + " enemyHP=" + enemyHP));
                            }
                            else
                            {
                                client.Shutdown(SocketShutdown.Both);
                                client.Close();
                            }
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        timer.Stop();
                        timer.Enabled = false;
                        Console.WriteLine("Client is disconnected, the timer is stop.");
                    }
                };
                timer.Start();

                //接收客户端的消息
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);

            }), null);

            Console.WriteLine("Server is ready!");
            Console.Read();
        }

        static byte[] buffer = new byte[1024];

        public static void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                var length = socket.EndReceive(ar);
                //读取出来消息内容
                var message = Encoding.Unicode.GetString(buffer, 0, length);
                //显示消息
                Console.WriteLine("接受指令"+message+"  敌人血量减少10");
                enemyHP = enemyHP - 10;
                socket.Send(Encoding.Unicode.GetBytes("playerHP= " + playerHP + " enemyHP=" + enemyHP));
                //接收下一个消息
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
