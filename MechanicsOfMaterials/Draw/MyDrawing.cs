using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MechanicsOfMaterials.Draw
{
    class MyDrawing
    {
        public static void DrawX(Graphics g, string str, float x0, float y0, float x, float y)
        {
            g.DrawLine(Form1.pen2, x0, y0, x, y);
            int i = 0;
            if (str == "left")
            {
                i = -20;
            }
            else if (str == "right")
            {
                i = 20;
            }
            PointF[] xPt = new PointF[3] { new PointF(x, y - 6), new PointF(x, y + 6), new PointF(x + i, y) };
            g.FillPolygon(new SolidBrush(Color.Black), xPt);
        }

        public static void DrawY(Graphics g, string str, float x0, float y0, float x, float y)
        {
            g.DrawLine(Form1.pen2, x0, y0, x, y);
            int i = 0;
            if (str == "up")
            {
                i = -20;
            }
            else if (str == "down")
            {
                i = 20;
            }
            PointF[] yPt = new PointF[3] { new PointF(x - 6, y), new PointF(x + 6, y), new PointF(x, y + i) };
            g.FillPolygon(new SolidBrush(Color.Black), yPt);
        }

        public static void DrawUpRightArrow(Graphics g, float x0, float y0, float x, float y)
        {
            g.DrawLine(Form1.pen2, x0, y0, x, y);
            PointF[] Pt = new PointF[3] { new PointF(x - 4.24f, y - 4.24f), new PointF(x + 4.24f, y + 4.24f), new PointF(x + 14.14f, y - 14.14f) };
            g.FillPolygon(new SolidBrush(Color.Black), Pt);
        }

        public static void DrawDownLeftArrow(Graphics g, float x0, float y0, float x, float y)
        {
            g.DrawLine(Form1.pen2, x0, y0, x, y);
            PointF[] Pt = new PointF[3] { new PointF(x - 4.24f, y - 4.24f), new PointF(x + 4.24f, y + 4.24f), new PointF(x - 14.14f, y + 14.14f) };
            g.FillPolygon(new SolidBrush(Color.Black), Pt);
        }

        public static void DrawXY(Graphics g, float x, float y)
        {
            MyDrawing.DrawX(g, "right", x, y, x + 600, y);
            g.DrawString("X", Form1.font2, Form1.brush1, x + 600, y + 10);
            MyDrawing.DrawY(g, "up", x, y + 100, x, y - 100);
            for (int i = (int)(x + 100); i <= x + 500; i += 100)
            {
                g.DrawLine(Form1.pen1, i, y, i, y - 7);
                g.DrawString((int)(i / 100) - 1 + "a", Form1.font2, Form1.brush1, i - 7, y + 5);
            }
        }

        public static void DrawShadow(Graphics g, float x1, float y1, float x2, float y2)
        {
            float len = x2 - x1;
            for (float i = x1; i <= x2; i += 10)
            {
                g.DrawLine(Form1.pen2, i, y1, i, y2);
            }
        }

        public static void DrawShodow(Graphics g, PointF p1, PointF p2)
        {
            DrawShadow(g, p1.X, p1.Y, p2.X, p2.Y);
        }

        public static void DrawShadowM(Graphics g, float x, float y, double[] M, double max)
        {
            for (int i = 0; i <= M.Length; i += 10)
            {
                g.DrawLine(Form1.pen2, x + i, y, x + i, (float)(y - M[i] * 100 / max));
            }
        }

        public static void DrawStringM(Graphics g, float x, float y, double[] M,float[] StringUporDown, double max)
        {
            for (int i=1; i<M.Length/100; i++)
            {
                g.DrawString("" + Math.Round(Math.Abs(M[i * 100]), 2), Form1.font1, Form1.brush1,
                    x + 100 * i - 30, (float)(y - M[i * 100] * 100 / max + StringUporDown[i]));
            }
        }
    }
}
