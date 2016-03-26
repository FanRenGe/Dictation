/*------------------------------------------
 * 
 * 属性
 * IP:客户端唯一标志
 * 事件
 * OnClientdisConnect:当客户端断开时触发
 * OnclientMessage:当客户端有消息过来触发,返回消息字符串
 * 方法:
 * setMessage:向客户端发送消息,参数是需要传递的字符串
 * killSelf:客户端自杀
 ------------------------------------------ */
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace 汉字听写大会专用系统
{
    class client
    {
        //定义IP属性，唯一界定客户端
        public string IP
        {
            get { return this.UserIP; }
        }

        public PictureBox PicDraw
        {
            get { return this.UsePicDraw; }
        }
        //注册断开客户端事件        
        public delegate void clientdisConnect(object sender, EventArgs e);
        public event clientdisConnect OnClientdisConnect;

        //注册收到客户端消息事件
        public delegate void clientMessage(object sender, EventArgs e, string message);
        public event clientMessage OnclientMessage;

        //变量
        private Socket clientSocket = null;
        private PictureBox UsePicDraw = null;
        private Thread thread;
        private bool flag = true;
        private Bitmap originImg;
        private Image finishImg;
        private Graphics g;
        private Point StartPoint, EndPoint, FontPoint;
        private Pen p = new Pen(Color.Black, 1);

        private string UserIP = null;
        private string UserName = null;

        //构造函数
        public client(Socket Socket, PictureBox pb)
        {
            string str = Socket.RemoteEndPoint.ToString();
            string[] Ipstr = str.Split(':');
            string clinetIP = Ipstr[0];
            UserIP = clinetIP;
            UserName = "答题者" + clinetIP;


            //准备画图
            this.UsePicDraw = pb;
            UsePicDraw.BackColor = SystemColors.Control;
            //UsePicDraw.Text = clinetIP;

            //将线帽样式设为圆线帽，否则笔宽变宽时会出现明显的缺口  
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;

            originImg = new Bitmap(UsePicDraw.Width, UsePicDraw.Height);
            g = Graphics.FromImage(originImg);
            //画布背景初始化为白底  
            g.Clear(Color.White);

            UsePicDraw.Image = originImg;
            finishImg = (Image)originImg.Clone();

            //准备接收画图坐标
            clientSocket = Socket;
            thread = new Thread(new ThreadStart(WaitForSendData));
            thread.IsBackground = true;
            thread.Name = clientSocket.RemoteEndPoint.ToString();
            thread.Start();
        }

        //等待数据通信函数
        private void WaitForSendData()
        {
            string message = null;
            while (flag)
            {
                if (clientSocket.Connected)
                {
                    try
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = clientSocket.Receive(bytes);
                        if (bytesRec > 0)
                        {
                            message = System.Text.Encoding.UTF8.GetString(bytes);

                            if (OnclientMessage != null)
                            {
                                OnclientMessage(this, new EventArgs(), message);
                            }
                        }
                    }
                    catch
                    {
                        killSelf();
                    }
                }
                else
                {
                    killSelf();
                }
            }
        }

        //客户端自毁方法
        public void killSelf()
        {
            flag = false;

            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Close();
                clientSocket = null;
            }
            ClearPictureBox();

            //一定要写在线程结束前，否则不触发
            if (OnClientdisConnect != null)
            {
                OnClientdisConnect(this, new EventArgs());
            }

            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread = null;
            }
        }

        //向客户端发送信息方法
        public void setMessage(string message)
        {
            try
            {
                byte[] sendbytes = System.Text.Encoding.UTF8.GetBytes(message);
                int successSendBtyes = clientSocket.Send(sendbytes, sendbytes.Length, SocketFlags.None);
            }
            catch
            {

            }
        }


        #region 画图写字

        public void drawWordToPictureBox(string[] arrCoordinate)
        {

            g = Graphics.FromImage(finishImg);
            g.SmoothingMode = SmoothingMode.AntiAlias; //抗锯齿  
            p.Width = 6;

            foreach (string coordinate in arrCoordinate)
            {
                string[] xy = coordinate.Split(',');
                float x1 = float.Parse(xy[0]);
                float y1 = float.Parse(xy[1]);
                float x2 = float.Parse(xy[2]);
                float y2 = float.Parse(xy[3]);
                g.DrawLine(p, x1, y1, x2, y2);

            }


            reDraw();
            originImg = (Bitmap)finishImg;
            UsePicDraw.Image = originImg;

        }

        public void ClearPictureBox()
        {
            UsePicDraw.BackColor = SystemColors.ControlDark;
            //UsePicDraw.Text = string.Empty;
            g.Clear(SystemColors.ControlDark);
            reDraw();
        }

        /// <summary>  
        /// 重绘绘图区（二次缓冲技术）  
        /// </summary>  
        private void reDraw()
        {
            Graphics graphics = UsePicDraw.CreateGraphics();
            graphics.DrawImage(finishImg, new Point(0, 0));
            graphics.Dispose();
        }

        #endregion


        //代码结束
    }
}
