using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace LVoronoi
{
    public partial class Form1 : Form
    {
        Graphics g;
        Delaynay D_TIN = new Delaynay(); //核心功能类


        bool bShowID = true;
        int t = -50;
        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            D_TIN.DS.BBOX.XLeft = 10;
            D_TIN.DS.BBOX.YTop = 30;
            D_TIN.DS.BBOX.XRight = this.Width - 20;
            D_TIN.DS.BBOX.YBottom = this.Height - D_TIN.DS.BBOX.YTop - 10;
            DrawBoundaryBox();
            ShowPoints(g);
            //ShowTriangle();
            ShowConvex(0);
        }
        private void DrawBoundaryBox()
        {
            g = this.CreateGraphics();
            g.Clear(Color.White);

            g.DrawLine(Pens.Blue, Convert.ToSingle(D_TIN.DS.BBOX.XLeft), Convert.ToSingle(D_TIN.DS.BBOX.YTop),
                Convert.ToSingle(D_TIN.DS.BBOX.XRight), Convert.ToSingle(D_TIN.DS.BBOX.YTop));
            g.DrawLine(Pens.Blue, Convert.ToSingle(D_TIN.DS.BBOX.XRight), Convert.ToSingle(D_TIN.DS.BBOX.YTop),
                Convert.ToSingle(D_TIN.DS.BBOX.XRight), Convert.ToSingle(D_TIN.DS.BBOX.YBottom));
            g.DrawLine(Pens.Blue, Convert.ToSingle(D_TIN.DS.BBOX.XRight), Convert.ToSingle(D_TIN.DS.BBOX.YBottom),
                Convert.ToSingle(D_TIN.DS.BBOX.XLeft), Convert.ToSingle(D_TIN.DS.BBOX.YBottom));
            g.DrawLine(Pens.Blue, Convert.ToSingle(D_TIN.DS.BBOX.XLeft), Convert.ToSingle(D_TIN.DS.BBOX.YBottom),
                Convert.ToSingle(D_TIN.DS.BBOX.XLeft), Convert.ToSingle(D_TIN.DS.BBOX.YTop));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //检查是否有重复点
            for (int i = 0; i < D_TIN.DS.VerticesNum; i++)
            {
                if ((long)e.X == D_TIN.DS.Vertex[i].x && (long)e.Y == D_TIN.DS.Vertex[i].y)
                    return;  //若该点已有则不再加入
            }
            g = this.CreateGraphics();
            Pen mypen = new Pen(Color.Red);


            if (D_TIN.DS.VerticesNum % 2 == 0)
                g.DrawLine(mypen, e.X - t, e.Y + t, e.X + t, e.Y - t);

            else
                g.DrawLine(mypen, e.X + t, e.Y + t, e.X - t, e.Y - t);
            t = t + 1;

            //g.DrawLine(mypen, e.X - 50, e.Y + 50, e.X + 50, e.Y - 50);
            //g.DrawLine(mypen, e.X + 50, e.Y + 50, e.X - 50, e.Y - 50);

            //加点            
            D_TIN.DS.Vertex[D_TIN.DS.VerticesNum].x = e.X;
            D_TIN.DS.Vertex[D_TIN.DS.VerticesNum].y = e.Y;
            D_TIN.DS.Vertex[D_TIN.DS.VerticesNum].ID = D_TIN.DS.VerticesNum;

            if (D_TIN.DS.VerticesNum % 2 == 0)
                g.DrawLine(mypen, e.X - t, e.Y + t, e.X + t, e.Y - t);

            else
                g.DrawLine(mypen, e.X + t, e.Y + t, e.X - t, e.Y - t);
            t = t + 10;
            D_TIN.DS.VerticesNum++;
            //DrawBoundaryBox();
            ShowPoints(g);
            ShowTriangle();
            //ShowConvex(50);
            
            

        }
        private void ShowTriangle()
        {
            if (D_TIN.DS.VerticesNum > 2)  //构建三角网
                D_TIN.CreateTIN();

            //输出三角形
            for (int i = 0; i < D_TIN.DS.TriangleNum; i++)
            {
                Point point1 = new Point(Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V1Index].x), Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V1Index].y));
                Point point2 = new Point(Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V2Index].x), Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V2Index].y));
                Point point3 = new Point(Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V3Index].x), Convert.ToInt32(D_TIN.DS.Vertex[D_TIN.DS.Triangle[i].V3Index].y));

                Pen p = new Pen(Color.Green, 1);
                g.DrawLine(p, point1, point2);
                g.DrawLine(p, point2, point3);
                g.DrawLine(p, point1, point3);

                if (bShowID)      //显示数字标识
                    g.DrawString((i + 1).ToString(), new Font(FontFamily.GenericSerif, 10), Brushes.Black,
                        (float)(point1.X + point2.X + point3.X) / 3, (float)(point1.Y + point2.Y + point3.Y) / 3);
            }
        }

        private void ShowPoints(Graphics DC)
        {
            Pen p = new Pen(Color.Red, 1);

            for (int i = 0; i < D_TIN.DS.VerticesNum; i++)
            {
                DC.DrawEllipse(p, D_TIN.DS.Vertex[i].x, D_TIN.DS.Vertex[i].y, 3, 3);
                //if(bShowID)      //显示数字标识
                //    g.DrawString((D_TIN.DS.Vertex[i].ID + 1).ToString(), new Font(FontFamily.GenericSerif, 8), Brushes.Blue,
                //        (float)(D_TIN.DS.Vertex[i].x), (float)(D_TIN.DS.Vertex[i].y));
            }
        }
        private void centre()
        {

            D_TIN.CalculateBC();    //求出每个三角形的圆心
            Pen p2 = new Pen(Color.SkyBlue, 1);
            for (int i = 0; i < D_TIN.DS.TriangleNum; i++) //显示
            {
                g.DrawEllipse(p2, Convert.ToSingle(D_TIN.DS.Barycenters[i].X), Convert.ToSingle(D_TIN.DS.Barycenters[i].Y), 3, 3);

                //if (bShowID)      //显示数字标识
                //    g.DrawString((i + 1).ToString(), new Font(FontFamily.GenericSerif, 9, FontStyle.Underline), Brushes.Black,
                //        (float)(D_TIN.DS.Barycenters[i].X), (float)(D_TIN.DS.Barycenters[i].Y));
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ShowPoints(g); //显示点
            //button2_Click(sender, e);
            centre();
            D_TIN.CreateVoronoi(g);

        }
        private void ShowConvex(int interval)
        {
            D_TIN.CreateConvex();

            for (int i = 0; i < D_TIN.HullPoint.Count; i++)
            {
                g.DrawLine(Pens.Black, D_TIN.DS.Vertex[D_TIN.HullPoint[i]].x, D_TIN.DS.Vertex[D_TIN.HullPoint[i]].y,
                 D_TIN.DS.Vertex[D_TIN.HullPoint[(i + 1) % D_TIN.HullPoint.Count]].x,
                 D_TIN.DS.Vertex[D_TIN.HullPoint[(i + 1) % D_TIN.HullPoint.Count]].y);
                Thread.Sleep(interval);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            D_TIN = new Delaynay();
            t = -50;
            this.Invalidate();
        }

        

        
    }
}
