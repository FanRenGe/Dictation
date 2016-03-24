using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 汉字听写大会专用系统
{
    class DrawWord
    {
        Bitmap originImg;
        Image finishImg;
        Graphics g;
        Point StartPoint, EndPoint, FontPoint;
        Pen p = new Pen(Color.Black, 1);
        bool IsDraw;
        Font font;
        Rectangle FontRect;
        private PictureBox picDraw;
        public string ClientIp;

        public DrawWord(PictureBox pictureBox, string clientIp)
        {
            this.ClientIp = clientIp;
            this.picDraw = pictureBox;
            picDraw.Text = clientIp;

            //将线帽样式设为圆线帽，否则笔宽变宽时会出现明显的缺口  
            p.StartCap = LineCap.Round;
            p.EndCap = LineCap.Round;

            originImg = new Bitmap(picDraw.Width, picDraw.Height);
            g = Graphics.FromImage(originImg);
            //画布背景初始化为白底  
            g.Clear(Color.White);

            picDraw.Image = originImg;
            finishImg = (Image)originImg.Clone();
        }

        public void Drawing(string[] arrCoordinate, string clientIp)
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
            picDraw.Image = originImg;

        }

        /// <summary>  
        /// 重绘绘图区（二次缓冲技术）  
        /// </summary>  
        public void reDraw()
        {
            Graphics graphics = picDraw.CreateGraphics();
            graphics.DrawImage(finishImg, new Point(0, 0));
            graphics.Dispose();
        }
        
    }
}
