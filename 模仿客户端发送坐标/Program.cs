using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 模仿客户端发送坐标
{
    class Program
    {
        static Socket clientSocket;
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("192.168.31.246");// IPAddress.Parse("172.16.3.65");
            IPEndPoint iep = new IPEndPoint(ip, 9009);
            bool isContinue = true;
            string strContinue = "Y";
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(iep);
                Console.WriteLine("建立连接：{0}", iep.Address);
                while (true)
                {


                    Console.WriteLine("请输入坐标（x1,y1,x2,y2#....）:");
                    string message = Console.ReadLine();
                    if (string.IsNullOrEmpty(message))
                    {
                        message = GeneratingCoordinates();
                    }

                    //if (!clientSocket.Connected)
                    //{
                    //    clientSocket.Connect(iep); Console.WriteLine("再次建立连接：{0}", iep.Address);
                    //}
                    sentClientInfo(message);

                    //等一下，服务器处理信息可能会太快
                    Thread.Sleep(100);

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine("连接" + ip + "失败！" + ex.Message);
            }
            Console.ReadLine();
        }

        private static void WaitForSendData()
        {
            bool flag = true;
            while (flag)
            {
                if (clientSocket.Connected)
                {
                    try
                    {
                        //byte[] bytes = new byte[1024];
                        //int bytesRec = clientSocket.Receive(bytes);
                        string message = GeneratingCoordinates();// System.Text.Encoding.UTF8.GetString(bytes);
                        sentClientInfo(message);
                        flag = false;
                    }
                    catch (Exception exp)
                    {
                        flag = false;
                        Console.WriteLine("通信错误：" + exp.Message);
                    }
                }
                else
                {
                    flag = false;
                    Console.WriteLine("与服务器断开！");
                }
            }
        }

        private static void sentClientInfo(string message)
        {
            try
            {
                byte[] sendbytes = System.Text.Encoding.UTF8.GetBytes(message);
                int successSendBtyes = clientSocket.Send(sendbytes, sendbytes.Length, SocketFlags.None);
                Console.WriteLine("发送消息到{0} ", clientSocket.RemoteEndPoint);
            }
            catch (Exception exp)
            {
                Console.WriteLine("sentClientInfo Error: " + exp.Message);
            }
        }

        private static string GeneratingCoordinates()
        {
            string str = string.Empty;


            str += string.Format("{0},{1},{2},{3}#", "322.002f", " 469.002f", "323.002f", "467.002f");
            str += string.Format("{0},{1},{2},{3}#", "323.002", "467", "324", "465");
            str += string.Format("{0},{1},{2},{3}#", "337.002f", "457", "346", "453");
            str += string.Format("{0},{1},{2},{3}#", "354.002f", "449", "363", "446");
            str += string.Format("{0},{1},{2},{3}#", "372.002f", "443", "381", "439");
            str += string.Format("{0},{1},{2},{3}#", "391.002f", "435", "399", "432");
            str += string.Format("{0},{1},{2},{3}#", "407", "430", "440", "417");
            str += string.Format("{0},{1},{2},{3}#", "449", "413", "460", "409");

            str += string.Format("{0},{1},{2},{3}#", "5", "5", "15", "5");
            str += string.Format("{0},{1},{2},{3}#", "", "", "", "");
            str += string.Format("{0},{1},{2},{3}#", "", "", "", "");
            return str.Trim('#');
        }
    }
}
