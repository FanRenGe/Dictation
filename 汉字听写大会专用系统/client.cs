/*------------------------------------------
 * 
 * ����
 * IP:�ͻ���Ψһ��־
 * �¼�
 * OnClientdisConnect:���ͻ��˶Ͽ�ʱ����
 * OnclientMessage:���ͻ�������Ϣ��������,������Ϣ�ַ���
 * ����:
 * setMessage:��ͻ��˷�����Ϣ,��������Ҫ���ݵ��ַ���
 * killSelf:�ͻ�����ɱ
 ------------------------------------------ */
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ������д���ר��ϵͳ
{
    class client
    {
        //����IP���ԣ�Ψһ�綨�ͻ���
        public string IP
        {
            get { return this.UserIP; }
        }

        public PictureBox PicDraw
        {
            get { return this.UsePicDraw; }
        }
        //ע��Ͽ��ͻ����¼�        
        public delegate void clientdisConnect(object sender, EventArgs e);
        public event clientdisConnect OnClientdisConnect;

        //ע���յ��ͻ�����Ϣ�¼�
        public delegate void clientMessage(object sender, EventArgs e, string message);
        public event clientMessage OnclientMessage;

        //����
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

        //���캯��
        public client(Socket Socket, PictureBox pb)
        {
            string str = Socket.RemoteEndPoint.ToString();
            string[] Ipstr = str.Split(':');
            string clinetIP = Ipstr[0];
            UserIP = clinetIP;
            UserName = "������" + clinetIP;


            //׼����ͼ
            this.UsePicDraw = pb;
            UsePicDraw.BackColor = SystemColors.Control;
            //UsePicDraw.Text = clinetIP;

            //����ñ��ʽ��ΪԲ��ñ������ʿ���ʱ��������Ե�ȱ��  
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;

            originImg = new Bitmap(UsePicDraw.Width, UsePicDraw.Height);
            g = Graphics.FromImage(originImg);
            //����������ʼ��Ϊ�׵�  
            g.Clear(Color.White);

            UsePicDraw.Image = originImg;
            finishImg = (Image)originImg.Clone();

            //׼�����ջ�ͼ����
            clientSocket = Socket;
            thread = new Thread(new ThreadStart(WaitForSendData));
            thread.IsBackground = true;
            thread.Name = clientSocket.RemoteEndPoint.ToString();
            thread.Start();
        }

        //�ȴ�����ͨ�ź���
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

        //�ͻ����Իٷ���
        public void killSelf()
        {
            flag = false;

            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Close();
                clientSocket = null;
            }
            ClearPictureBox();

            //һ��Ҫд���߳̽���ǰ�����򲻴���
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

        //��ͻ��˷�����Ϣ����
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


        #region ��ͼд��

        public void drawWordToPictureBox(string[] arrCoordinate)
        {

            g = Graphics.FromImage(finishImg);
            g.SmoothingMode = SmoothingMode.AntiAlias; //�����  
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
        /// �ػ��ͼ�������λ��弼����  
        /// </summary>  
        private void reDraw()
        {
            Graphics graphics = UsePicDraw.CreateGraphics();
            graphics.DrawImage(finishImg, new Point(0, 0));
            graphics.Dispose();
        }

        #endregion


        //�������
    }
}
