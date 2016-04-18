using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 汉字听写大会专用系统
{
    public partial class Form1 : Form
    {
        //应用程序变量
        private IPAddress HostIP;
        private bool flag = true;
        private Socket serverSocket;
        private Socket clientSocket = null;
        private Thread _createServer;
        private Hashtable clientList = new Hashtable();
        private int userID = 0;
        private string ip = System.Configuration.ConfigurationManager.AppSettings["serverip"];
        private int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
        private int beginTimer = 0;

        //委托函数，添加用户
        private delegate void addDelegate(string clientIp);
        private void addUser(string clientIp)
        {

            if (pictureBox1.BackColor == SystemColors.ControlDark)
            {
                if (this.pictureBox1.InvokeRequired)
                {
                    addDelegate md = new addDelegate(this.addUser);
                    this.Invoke(md, new object[] { clientIp });
                }
                else
                {
                    this.pictureBox1.BackColor = SystemColors.Control;
                    pictureBox1.Text = clientIp;
                }
            }
            else if (pictureBox2.BackColor == SystemColors.ControlDark)
            {
                if (this.pictureBox2.InvokeRequired)
                {
                    addDelegate md = new addDelegate(this.addUser);
                    this.Invoke(md, new object[] { clientIp });
                }
                else
                {
                    this.pictureBox2.BackColor = SystemColors.Control;
                    pictureBox2.Text = clientIp;
                }
            }
            else if (pictureBox3.BackColor == SystemColors.ControlDark)
            {
                if (this.pictureBox3.InvokeRequired)
                {
                    addDelegate md = new addDelegate(this.addUser);
                    this.Invoke(md, new object[] { clientIp });
                }
                else
                {
                    this.pictureBox3.BackColor = SystemColors.Control;
                    pictureBox3.Text = clientIp;
                }
            }
            else if (pictureBox4.BackColor == SystemColors.ControlDark)
            {
                if (this.pictureBox4.InvokeRequired)
                {
                    addDelegate md = new addDelegate(this.addUser);
                    this.Invoke(md, new object[] { clientIp });
                }
                else
                {
                    this.pictureBox4.BackColor = SystemColors.Control;
                    pictureBox4.Text = clientIp;
                }
            }
        }

        //委托函数，删除用户
        private delegate void removeDelegate(client use);
        private void removeUser(client user)
        {
            if (user.PicDraw.InvokeRequired)
            {
                removeDelegate md = new removeDelegate(this.removeUser);
                this.Invoke(md, new object[] { user });
            }
            else
            {
                user.ClearPictureBox();
            }

            //if (pictureBox1.Text == clientIp)
            //{
            //    if (this.pictureBox1.InvokeRequired)
            //    {
            //        removeDelegate md = new removeDelegate(this.removeUser);
            //        this.Invoke(md, new object[] { clientIp });
            //    }
            //    else
            //    {
            //        this.pictureBox1.BackColor = SystemColors.ControlDark;
            //        this.pictureBox1.Text = string.Empty;
            //    }
            //}
            //else if (pictureBox2.Text == clientIp)
            //{
            //    if (this.pictureBox2.InvokeRequired)
            //    {
            //        removeDelegate md = new removeDelegate(this.removeUser);
            //        this.Invoke(md, new object[] { clientIp });
            //    }
            //    else
            //    {
            //        this.pictureBox2.BackColor = SystemColors.ControlDark;
            //        this.pictureBox2.Text = string.Empty;
            //    }
            //}
            //else if (pictureBox3.Text == clientIp)
            //{
            //    if (this.pictureBox3.InvokeRequired)
            //    {
            //        removeDelegate md = new removeDelegate(this.removeUser);
            //        this.Invoke(md, new object[] { clientIp });
            //    }
            //    else
            //    {
            //        this.pictureBox3.BackColor = SystemColors.ControlDark;
            //        this.pictureBox3.Text = string.Empty;
            //    }
            //}
            //else if (pictureBox4.Text == clientIp)
            //{
            //    if (this.pictureBox4.InvokeRequired)
            //    {
            //        removeDelegate md = new removeDelegate(this.removeUser);
            //        this.Invoke(md, new object[] { clientIp });
            //    }
            //    else
            //    {
            //        this.pictureBox4.BackColor = SystemColors.ControlDark;
            //        this.pictureBox4.Text = string.Empty;
            //    }
            //}
        }

        private delegate void ChangeDelegate(int newTime);
        private void ChageTime(int newTime)
        {
            if (this.lblTimer.InvokeRequired)
            {
                ChangeDelegate md = new ChangeDelegate(this.ChageTime);
                this.Invoke(md, new object[] { newTime });
            }
            else
            {
                this.lblTimer.Text = newTime.ToString("00");
                if (newTime == 0)
                {
                    this.lblTimer.ForeColor = Color.White;
                }
                else if (newTime < 10)
                {
                    this.lblTimer.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    this.lblTimer.ForeColor = System.Drawing.SystemColors.HotTrack;
                }
            }
        }
        public Form1()
        {
            InitializeComponent();

            #region 启动线程，开始侦听客户端连接消息

            flag = true;
            _createServer = new Thread(new ThreadStart(StartListening));
            _createServer.IsBackground = true;
            _createServer.Start();
            #endregion
        }

        private void btnShowTopic_Click(object sender, EventArgs e)
        {
            btnShowTopic.Enabled = false;
            #region 出题
            lblTopic.Text = words.ShowWord();
            #endregion

            #region 倒计时开始
            string strBeginTimer = System.Configuration.ConfigurationManager.AppSettings["timer"];
            beginTimer = string.IsNullOrEmpty(strBeginTimer) ? 60 : Convert.ToInt32(strBeginTimer);
            ChageTime(beginTimer);

            timer1.Start();
            #endregion
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            beginTimer -= 1;
            ChageTime(beginTimer);

            if (beginTimer == 0)
            {
                timer1.Stop();
                btnShowTopic.Enabled = true;
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            client user;
            foreach (DictionaryEntry clientObj in clientList)
            {
                user = (client)clientObj.Value;
                user.ClearPictureBox();
            }
        }

        //启动服务器
        private void StartListening()
        {
            //获取本机IP地址
            HostIP = IPAddress.Parse(ip);
            try
            {
                //打开配置端口，开始侦听
                IPEndPoint iep = new IPEndPoint(HostIP, port);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(iep);
                serverSocket.Listen(4);

                //如果有客户端连接进来，就加入队列               
                while (flag)
                {
                    clientSocket = serverSocket.Accept();
                    if (clientSocket != null)
                    {
                        //用户计数 
                        userID++;
                        if (userID > 4) break;
                        //用户PictureBox
                        PictureBox pb = new PictureBox();
                        switch (userID)
                        {
                            case 1:
                                pb = pictureBox1;
                                break;
                            case 2:
                                pb = pictureBox2;
                                break;
                            case 3:
                                pb = pictureBox3;
                                break;
                            case 4:
                                pb = pictureBox4;
                                break;
                        }

                        string str = clientSocket.RemoteEndPoint.ToString();
                        string[] Ipstr = str.Split(':');

                        string clientIp = Ipstr[0];

                        if (!clientList.Contains(clientIp))
                        {

                            client user = new client(clientSocket, pb);

                            //注册断开事件
                            user.OnClientdisConnect += new client.clientdisConnect(this.removeclient);

                            //注册消息事件
                            user.OnclientMessage += new client.clientMessage(this.getClientMessage);



                            //将此人添加到用户列表
                            clientList.Add(clientIp, user);

                            //更新显示
                            //this.addUser(clientIp);
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
        //当客户端断开连接，删除这个客户端
        private void removeclient(object sender, EventArgs e)
        {
            client user = (client)sender;
            string clientIp = user.IP;

            try
            {
                user.ClearPictureBox();
                //从客户端列表移除
                clientList.Remove(clientIp);
                //从用户列表移除
                this.removeUser(user);
            }
            catch (Exception exp)
            {
                MessageBox.Show("删除用户出错：" + exp.Message);
            }
        }

        //当客户端发送消息，解析这个消息

        /// <summary>
        /// 接收客户端的消息，并根据接收到的坐标在画布上同步。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message">接收到的客户端坐标，格式为 x1,y1,x2,y2#x1,y1,x2,y2#....</param>
        private void getClientMessage(object sender, EventArgs e, string message)
        {
            #region 坐标解析
            string[] arrCoordinate = message.Split('#');
            //客户端IP地址
            client user = (client)sender;
            if (beginTimer > 0)
            {
                user.drawWordToPictureBox(arrCoordinate);
            }

            #endregion

        }

        private void 导入试题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog1.FileName);
                string[] str = new string[] { ".xls", ".xlsx" };
                if (!((IList)str).Contains(extension))
                {
                    MessageBox.Show("仅能上次Execl");
                }
                else
                {
                    MessageBox.Show(words.Import(openFileDialog1.FileName) ? "导入成功" : "导入失败，请重试。");
                }
            }
        }

    }

}
