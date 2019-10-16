using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MechanicsOfMaterials.Draw;
using MechanicsOfMaterials.Calculate;

namespace MechanicsOfMaterials
{
    public partial class Form1 : Form
    {
        const double pi = Math.PI;
        const double permissibleStress = 80000000;
        const double E = 200000000000;
        const double a_1 = 300000000;
        const double t_1 = 155000000;
        const double a_b = 650000000;
        const double a_s = 353000000;

        const int safeN = 2;
        private int status;//表示所处的状态
        private int status5 = 0, status6 = 0;

        public static double[] mz = new double[501];
        public static double[] my = new double[501];
        public static double[] mx = new double[501];
        public static double[] myAverage = new double[501];
        public static double[] mzAverage = new double[501];
        public static double[] myMyAverage = new double[501];
        public static double[] mzMzAverage = new double[501];

        private double p, p1, n, d, d1, d2, g2, g1, a, degree, diameter1;//直径1，计算挠度时使用
        private double diameter2, Ka, Kt, Ea, Et, B, sensitivity;//直径2，校核疲劳强度时使用
        private double f2, f1, f, m2, m1, m, fby, fay, faz, fbz;

        public static Brush brush1 = new SolidBrush(Color.Black);//正文笔刷
        public static Font font1 = new Font("宋体", 14, FontStyle.Regular);//正文字体
        public static Font font2 = new Font("宋体", 14, FontStyle.Bold);
        public static Pen pen1 = new Pen(Color.Black, 3);
        public static Pen pen2 = new Pen(Color.Black, 2);
        
        private void tabPage7_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString(
                "//2019.09.21" +
                "\n软件制作仓促，还在修改中，bug和逻辑上的错误在所难免，只能作为材力课设" +
                "\n7.6C轴的检验，不能用作其他用途。" +
                "\n\n修改记录：\n//2019.09.24" +
                "\n1.设计直径模块中增加 按比例确定最后轴径 功能。" +
                "\n2.优化疲劳强度判断代码（两端键槽部分单独判断）。"+
                "\n\n修改记录：\n//2019.10.11" +
                "\n3.将My,Mz画图的单位改成N·m。" +
                "\n4.增加设计直径部分说明，按照比例关系设计直径部分不同人有不同理解。", font2, brush1, 20, 20);
        }
        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button3.PerformClick();
            }
        }

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button2.PerformClick();
            }
        }

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button1.PerformClick();
            }
        }
    
        private void button3_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox14.Text, out diameter2) && double.TryParse(textBox15.Text, out Ka) &&
                double.TryParse(textBox16.Text, out Kt) && double.TryParse(textBox17.Text, out Ea) &&
                double.TryParse(textBox18.Text, out Et) && double.TryParse(textBox19.Text, out B) &&
                double.TryParse(textBox20.Text, out sensitivity))
            {
                diameter2 /= 1000;
                status6 = 1;
                tabPage6.Refresh();
            }
            else
            {
                MessageBox.Show("数据有误，请仔细检查！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void tabPage6_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int x=0;
            switch (comboBox1.SelectedIndex)
            {
                case 0: x = 0; break;
                case 1: x = 50; break;
                case 2: x = 150; break;
                case 3: x = 300; break;
                case 4: x = 350; break;
                case 5: x = 450; break;
                case 6: x = 500; break;
            }
            g.DrawString("Mx= " + Math.Round(mx[x], 2) +
    " N•m     My= " + Math.Round(my[x], 2) + " N•m     Mz= " + Math.Round(mz[x], 2) + " N•m",
    font1, brush1, 100, 100);
            if (status6 == 1)
            {
                double W = (pi * diameter2 * diameter2 * diameter2) / 32;
                double aMax = (Math.Sqrt(my[x] * my[x] + mz[x] * mz[x])) / W;
                double Wp = (pi * diameter2 * diameter2 * diameter2) / 16;
                double tMax = mx[x] / Wp;
                double tm = tMax / 2;
                double ta = tMax / 2;
                double na = (a_1 * Ea * B) / (Ka * aMax);
                double nt = t_1 / (Kt * ta / (Et * B) + sensitivity * tm);
                double nat = (na * nt) / Math.Sqrt(na * na + nt * nt);
                double a_r3 = Math.Sqrt(MyCalculate.SumOfSquaresMxyz(x)) / W;
                double nat1 = a_s / a_r3;
                string isOk = "";
                if (double.IsNaN(nat) || double.IsNaN(nat1))
                {
                    if (nt > safeN)
                    {
                        isOk = "是";
                    }
                    else
                    {
                        isOk = "否";
                    }
                }
                else
                {
                    if (nat > safeN && nat1 > safeN)
                    {
                        isOk = "是";
                    }
                    else
                    {
                        isOk = "否";
                    }
                }
               
                g.DrawString("σmax= " + Math.Round(aMax / 1000000, 2) + " Mpa" + "     τmax= " + Math.Round(tMax / 1000000, 2) + " Mpa\n\n" +
                    "Na= " + Math.Round(na, 2) + "    Nt= " + Math.Round(nt, 2) + "    Nat= " + Math.Round(nat, 2) + "    Nat′= " + Math.Round(nat1, 2) +
                    "\n\n该截面是否满足强度要求： " + isOk,
                    font2, brush1, 100, 300);
                status6 = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox14.Text = "";
            textBox15.Text = "";
            textBox16.Text = "";
            textBox17.Text = "";
            textBox18.Text = "";
            textBox19.Text = "";
            textBox20.Text = "";
            tabPage6.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox13.Text, out diameter1))
            {
                status5 = 1;
                diameter1 /= 1000;
                tabPage5.Refresh();
            }
            else
            {
                status5 = 0;
                MessageBox.Show("输入数据有误，请仔细检查！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void tabPage5_Paint(object sender, PaintEventArgs e)
        {
            if (status5 == 1)
            {
                Graphics g = e.Graphics;
                //z轴挠度
                for (int i = 0; i <= 100; i++)
                {
                    double x = a * i / 100.0;
                    myAverage[i] = -x;
                }
                for (int i = 101; i <= 400; i++)
                {
                    double x = a * i / 100.0;
                    myAverage[i] = 1 / 3.0 * (x - 4 * a);
                }
                for (int i = 401; i <= 500; i++)
                {
                    myAverage[i] = 0;
                }
                for (int i = 0; i <= 500; i++)
                {
                    myMyAverage[i] = my[i] * myAverage[i];
                }
                double I = (pi * diameter1 * diameter1 * diameter1 * diameter1) / 64;
                double bendingZ = (MyCalculate.DefiniteIntegral(myMyAverage, 0, 500) * a / 100) / (E * I);
                g.DrawString("Δz= " + Math.Round(bendingZ * 1000, 3) + " mm", font1, brush1, 100, 100);

                //y轴挠度
                for (int i = 0; i <= 500; i++)
                {
                    mzAverage[i] = myAverage[i];
                    mzMzAverage[i] = mz[i] * mzAverage[i];
                }
                double bendingY = (MyCalculate.DefiniteIntegral(mzMzAverage, 0, 500) * a / 100) / (E * I);
                g.DrawString("Δy= " + Math.Round(bendingY * 1000, 3) + " mm", font1, brush1, 100, 140);

                //总挠度
                double bendingXY = Math.Sqrt(bendingZ * 1000 * bendingZ * 1000 + bendingY * 1000 * bendingY * 1000);
                g.DrawString("Δ= " + Math.Round(bendingXY, 3) + " mm", font1, brush1, 100, 180);
            }

        }

        private void tabPage4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int maxX = 0;
            int[] tempDiameter = new int[5];
            double[] diameter = new double[5];
            double maxDiameter;
            float x1 = 100, y1 = 10;
            for (int i = 0; i <= 500; i += 100)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            int maxDiameterIndex;
            if ((maxX >= 0 && maxX <= 50) || (maxX >= 450 && maxX <= 500))
            {
                maxDiameterIndex = 4;
            }
            else if (maxX > 50 && maxX <= 150)
            {
                maxDiameterIndex = 3;
            }
            else if (maxX > 150 && maxX <= 350)
            {
                maxDiameterIndex = 2;
            }
            else
            {
                maxDiameterIndex = 1;
            }
            g.DrawString("危险截面的横坐标：  x=" + Math.Round(a * maxX / 100.0, 2) + "m     截面直径为Φ" + maxDiameterIndex + "\n\n设计直径：",
                font1, brush1, x1, y1);

            //设计直径1
            maxX = 350;
            for (int i = 351; i <= 450; i++)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            maxDiameter = Math.Pow((32 * Math.Sqrt(MyCalculate.SumOfSquaresMxyz(maxX))) / (pi * permissibleStress), 1 / 3.0);
            diameter[1]=maxDiameter*1000;
            g.DrawString("Φ1≥" + Math.Round(maxDiameter * 1000, 2) + "mm    在 x=" + maxX * a / 100.0 + "m 处应力最大",
                font1, brush1, x1, y1 + 80);

            //设计直径2
            maxX = 150;
            for (int i = 151; i <= 350; i++)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            maxDiameter = Math.Pow((32 * Math.Sqrt(MyCalculate.SumOfSquaresMxyz(maxX))) / (pi * permissibleStress), 1 / 3.0);
            diameter[2] = maxDiameter * 1000;
            g.DrawString("Φ2≥" + Math.Round(maxDiameter * 1000, 2) + "mm    在 x=" + maxX * a / 100.0 + "m 处应力最大",
                font1, brush1, x1, y1 + 120);

            //设计直径3
            maxX = 50;
            for (int i = 51; i <= 150; i++)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            maxDiameter = Math.Pow((32 * Math.Sqrt(MyCalculate.SumOfSquaresMxyz(maxX))) / (pi * permissibleStress), 1 / 3.0);
            diameter[3] = maxDiameter * 1000;
            g.DrawString("Φ3≥" + Math.Round(maxDiameter * 1000, 2) + "mm    在 x=" + maxX * a / 100.0 + "m 处应力最大",
                font1, brush1, x1, y1 + 160);

            //设计直径4
            maxX = 0;
            for (int i = 1; i <= 50; i++)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            for (int i = 450; i <= 500; i++)
            {
                if (MyCalculate.IsMxMyMzMore(i, maxX))
                {
                    maxX = i;
                }
            }
            maxDiameter = Math.Pow((32 * Math.Sqrt(MyCalculate.SumOfSquaresMxyz(maxX))) / (pi * permissibleStress), 1 / 3.0);
            diameter[4] = maxDiameter * 1000;
            g.DrawString("Φ4≥" + Math.Round(maxDiameter * 1000, 2) + "mm    在 x=" + maxX * a / 100.0 + "m 处应力最大",
                font1, brush1, x1, y1 + 200);

            g.DrawString("之后根据比例关系得(这部分不同人有不同理解）：", font2, brush1, x1, y1 + 240);
            int diameterStatus = 1;
            tempDiameter[1] = MyCalculate.NearestMoreEvenNumber(diameter[1])-2;
            while (diameterStatus != 4)
            {
                tempDiameter[1] += 2;
                if (tempDiameter[1] / 1.1 >= diameter[2])
                {
                    tempDiameter[2] = MyCalculate.NearestMoreEvenNumber(tempDiameter[1] / 1.1);
                    diameterStatus = 2;
                }
                if (diameterStatus == 2 && tempDiameter[2] / 1.1 >= diameter[3])
                {
                    tempDiameter[3] = MyCalculate.NearestMoreEvenNumber(tempDiameter[2] / 1.1);
                    diameterStatus = 3;
                }
                if (diameterStatus==3 && tempDiameter[3] / 1.1 >= diameter[4])
                {
                    tempDiameter[4] = MyCalculate.NearestMoreEvenNumber(tempDiameter[3] / 1.1);
                    diameterStatus = 4;
                }
            }
            g.DrawString("Φ1≥ " + tempDiameter[1] + " mm" +
                "\n\nΦ2≥ "+tempDiameter[2]+ " mm" +
                "\n\nΦ3≥ "+tempDiameter[3]+" mm" +
                "\n\nΦ4≥ "+tempDiameter[4]+" mm",
    font2, brush1, x1, y1 + 270);
            textBox13.Text = "" + tempDiameter[1];
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(double.TryParse(textBox3.Text, out p) && double.TryParse(textBox4.Text, out p1) &&
                double.TryParse(textBox5.Text, out n) && double.TryParse(textBox6.Text, out d) &&
                double.TryParse(textBox7.Text, out d1) && double.TryParse(textBox8.Text, out d2) &&
                double.TryParse(textBox9.Text, out g2) && double.TryParse(textBox10.Text, out g1) &&
                double.TryParse(textBox11.Text, out a) && double.TryParse(textBox12.Text, out degree)))
            {
                status = 0;
                MessageBox.Show("数据不完整或者有错误，请仔细检查！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                d /= 1000;
                d1 /= 1000;
                d2 /= 1000;
                a /= 1000;
                degree = degree * pi / 180;
                status = 1;
                for (int i=0; i<tabControl1.TabCount; i++)
                {
                    tabControl1.SelectedIndex = i;
                }
                tabControl1.SelectedIndex = 1;
                MessageBox.Show("生成完毕！", "提示");
            }
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //for (int i = 0; i < 779; i += 100)
            //{
            //    g.DrawLine(pen1, i, 0, i, 10);
            //}
            //for (int i = 0; i < 490; i += 100)
            //{
            //    g.DrawLine(pen1, 0, i, 10, i);
            //}
            g.DrawString("  首先对传动轴进行受力分析，轴总共受到7个力作用，分别为皮带轮D对传动轴的力F₂\n" +
                         "和2F₂,皮带轮D₁对传动轴的力F₁和2F₁，齿轮D₂对传动轴的力F，还有皮带轮D的重力和G₂\n" +
                         "皮带轮D₁的重力G₁.受力简图如下所示：\n", font1, brush1, 10, 10);
            g.DrawLine(pen1, 200, 200, 700, 200);//主轴线
            MyDrawing.DrawX(g, "right", 100, 200, 150, 200);
            MyDrawing.DrawY(g, "up", 100, 200, 100, 150);
            MyDrawing.DrawDownLeftArrow(g, 100, 200, 65, 235);
            g.DrawString("X", font2, brush1, 150 + 5, 200 + 5);//x轴
            g.DrawString("Y", font2, brush1, 100 - 20, 150 - 20);//y轴
            g.DrawString("Z", font2, brush1, 65, 235);//z轴
            MyDrawing.DrawY(g, "down", 200, 200, 200, 250);//fcos
            g.DrawString("Fcosα", font2, brush1, 170, 270);
            MyDrawing.DrawUpRightArrow(g, 200, 200, 200 + 35, 200 - 35);//fsin
            g.DrawString("Fsinα", font2, brush1, 210, 200 - 70);
            MyDrawing.DrawY(g, "up", 300, 200, 300, 150);//fay
            g.DrawString("Fay", font2, brush1, 300 - 20, 150 - 40);
            MyDrawing.DrawDownLeftArrow(g, 300, 200, 300 - 35, 200 + 35);//faz
            g.DrawString("Faz", font2, brush1, 300 - 35, 200 + 40);
            MyDrawing.DrawY(g, "down", 500, 200, 500, 250);//3f1+G1
            g.DrawString("3F₁+G₁", font2, brush1, 500 - 30, 250 + 25);
            MyDrawing.DrawY(g, "up", 600, 200, 600, 150);//fby
            g.DrawString("Fby", font2, brush1, 600 - 20, 150 - 40);
            MyDrawing.DrawDownLeftArrow(g, 600, 200, 600 - 35, 200 + 35);//fbz
            g.DrawString("Fbz", font2, brush1, 600 - 35, 200 + 40);
            MyDrawing.DrawUpRightArrow(g, 700, 200, 700 + 35, 200 - 35);//3f2
            g.DrawString("3F₂", font2, brush1, 700 + 30, 200 - 35 - 30);
            MyDrawing.DrawY(g, "down", 700, 200, 700, 250);//g2
            g.DrawString("G₂", font2, brush1, 700 - 10, 250 + 20);
            g.DrawArc(pen1, 200 - 20, 200 - 30, 40, 60, 75, 195);
            MyDrawing.DrawX(g, "right", 200, 200 - 30, 200, 200 - 30);//M
            g.DrawString("M", font2, brush1, 200 - 10, 200 - 30 - 25);
            g.DrawArc(pen1, 500 - 20, 200 - 30, 40, 60, 75, 195);
            MyDrawing.DrawX(g, "right", 500, 200 - 30, 500, 200 - 30);//M1
            g.DrawString("M₁", font2, brush1, 500 - 15, 200 - 30 - 25);
            g.DrawArc(pen1, 700 - 20, 200 - 30, 40, 60, 90, 195);
            MyDrawing.DrawX(g, "right", 700, 200 + 30, 700, 200 + 30);//M2
            g.DrawString("M₂", font2, brush1, 700 + 25, 200 + 20);
            for (int i = 200; i <= 700; i += 100)
            {
                g.DrawLine(pen1, i, 200, i, 193);
            }
            g.DrawString("  此传动轴受弯扭组合变形，把各力分解，使每组力只产生一种变形，如上图。", font1, brush1, 10, 300);
        }

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            double max;
            float[] stringUpOrDown = new float[10];
            string[] plusOrMinus = new string[10];
            Graphics g = e.Graphics;
            g.TranslateTransform(this.tabPage3.AutoScrollPosition.X, tabPage3.AutoScrollPosition.Y);//GDI绘图时滚动条必要操作
            m2 = 9549 * p / n;
            f2 = m2 * 2 / d;
            m1 = 9549 * p1 / n;
            f1 = m1 * 2 / d1;
            m = m2 - m1;
            f = m * 2 / d2;
            fby = (-f * Math.Cos(degree) + 4 * g2 + 2 * (3 * f1 + g1)) / 3;
            fay = -fby + f * Math.Cos(degree) + (3 * f1 + g1) + g2;
            g.DrawString("F1=" + Math.Round(f1, 2) + "N\nF2=" + Math.Round(f2, 2) + "N\nF=" + Math.Round(f, 2) + "N",
                font1, brush1, 10, 10);
            g.DrawString("Fay=" + Math.Round(fay, 2) + "N\nFby=" + Math.Round(fby, 2) + "N", font1, brush1, 200, 10);
            //扭矩图
            g.DrawString("扭矩图", font2, brush1, 10, 200);
            MyDrawing.DrawXY(g, 100, 200);
            g.DrawString("Mx/(N•m)", font2, brush1, 100, 200 - 100 - 30);
            double M1PlusM = 100;
            double M = m * 100 / (m1 + m);
            for (int i = 0; i < 300; i++)
            {
                mx[i] = m;
            }
            for (int i = 300; i <= 500; i++)
            {
                mx[i] = m + m1;
            }
            PointF[] pt = new PointF[5] { new PointF(100,(float)(200-M)),new PointF(100+300,(float)(200-M)),new PointF(100+300,(float)(200-M1PlusM)),
                new PointF(100+500,(float)(200-M1PlusM)),new PointF(100+500,200)};
            g.DrawLines(pen2, pt);
            for (int i = 100; i <= 100 + 300; i += 10)
            {
                g.DrawLine(pen2, (float)i, 200, (float)i, (float)(200 - M));
            }
            for (int i = 100 + 300; i <= 100 + 500; i += 10)
            {
                g.DrawLine(pen2, (float)i, 200, (float)i, (float)(200 - M1PlusM));
            }
            g.DrawString("" + Math.Round(m, 2), font1, brush1, 100 + 100, (float)(200 - M - 20));
            g.DrawString("" + Math.Round(m + m1, 2), font1, brush1, 100 + 400, (float)(200 - M1PlusM - 20));

            //剪力图z
            double[] fqzSection = new double[4];
            double[] fqzSectionRatio = new double[4];
            fqzSection[0] = -f * Math.Cos(degree);
            fqzSection[1] = fqzSection[0] + fay;
            fqzSection[2] = fqzSection[1] - (3 * f1 + g1);
            fqzSection[3] = fqzSection[2] + fby;
            float x2 = 100, y2 = 200 + 250;
            MyDrawing.DrawXY(g, x2, y2);
            g.DrawString("剪力图z", font2, brush1, 10, y2);
            g.DrawString("Fq/N", font2, brush1, x2+5, y2 - 120);
            max = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Math.Abs(fqzSection[i]) > max)
                {
                    max = Math.Abs(fqzSection[i]);
                }
                if (fqzSection[i] > 0)
                {
                    stringUpOrDown[i] = -20;
                }
                else
                {
                    stringUpOrDown[i] = 5;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                fqzSectionRatio[i] = fqzSection[i] * 100 / max;
                //g.DrawString(""+Math.Round(fqzSectionRatio))
            }
            PointF[] ptFqz = new PointF[9] {new PointF(x2,(float)(y2-fqzSectionRatio[0])),new PointF(x2+100,(float)(y2-fqzSectionRatio[0])),
                new PointF(x2+100,(float)(y2-fqzSectionRatio[1])), new PointF(x2+300,(float)(y2-fqzSectionRatio[1])),
                new PointF(x2+300, (float)(y2-fqzSectionRatio[2])),new PointF(x2+400,(float)(y2-fqzSectionRatio[2])),
                new PointF(x2+400,(float)(y2-fqzSectionRatio[3])),new PointF(x2+500,(float)(y2-fqzSectionRatio[3])),
                new PointF(x2+500,y2)};
            g.DrawLines(pen2, ptFqz);
            MyDrawing.DrawShodow(g, new PointF(x2, y2), ptFqz[1]);
            MyDrawing.DrawShodow(g, ptFqz[2], new PointF(x2 + 300, y2));
            MyDrawing.DrawShodow(g, new PointF(x2 + 300, y2), ptFqz[5]);
            MyDrawing.DrawShodow(g, ptFqz[6], ptFqz[8]);
            g.DrawString("" + Math.Round(Math.Abs(fqzSection[0]), 2), font1, brush1,
                x2 + 10, (float)(y2 - fqzSectionRatio[0] + stringUpOrDown[0]));
            g.DrawString("" + Math.Round(Math.Abs(fqzSection[1]), 2), font1, brush1,
                x2 + 200, (float)(y2 - fqzSectionRatio[1] + stringUpOrDown[1]));
            g.DrawString("" + Math.Round(Math.Abs(fqzSection[2]), 2), font1, brush1,
                x2 + 300, (float)(y2 - fqzSectionRatio[2] + stringUpOrDown[2]));
            g.DrawString("" + Math.Round(Math.Abs(fqzSection[3]), 2), font1, brush1,
                x2 + 400, (float)(y2 - fqzSectionRatio[3] + stringUpOrDown[3]));

            //弯矩图z
            for (int i = 0; i < 501; i++)
            {
                double x = a * i / 100.0;
                if (i >= 0 && i <= 100)
                {
                    mz[i] = fqzSection[0] * x;
                }
                else if (i > 100 && i <= 300)
                {
                    mz[i] = mz[100] + fqzSection[1] * (x - a);
                }
                else if (i > 300 && i <= 400)
                {
                    mz[i] = mz[300] + fqzSection[2] * (x - 3 * a);
                }
                else
                {
                    mz[i] = mz[400] + fqzSection[3] * (x - 4 * a);
                }
            }
            double[] mzRatio = new double[6];
            max = 0;
            for (int i = 0; i < 6; i++)
            {
                mzRatio[i] = mz[i * 100];
                if (Math.Abs(mzRatio[i]) > max)
                {
                    max = Math.Abs(mzRatio[i]);
                }
                if (mzRatio[i] > 0)
                {
                    stringUpOrDown[i] = -20;
                }
                else
                {
                    stringUpOrDown[i] = 5;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                mzRatio[i] = mzRatio[i] * 100 / max;
            }
            float x3 = x2, y3 = y2 + 250;
            MyDrawing.DrawXY(g, x3, y3);
            g.DrawString("弯矩图z", font2, brush1, 10, y3);
            g.DrawString("Mz/(N·m)", font2, brush1, x3 + 5, y3 - 120);
            PointF[] ptMz = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                ptMz[i] = new PointF(x3 + i * 100, y3 - (float)(mzRatio[i]));
            }
            g.DrawLines(pen2, ptMz);
            MyDrawing.DrawShadowM(g, x3, y3, mz, max);
            MyDrawing.DrawStringM(g, x3, y3, mz, stringUpOrDown, max);
            for (int i = 0; i < 4; i++)
            {
                if (fqzSection[i] >= 0)
                {
                    plusOrMinus[i] = "+";
                }
                else
                {
                    plusOrMinus[i] = "";
                }
            }
            g.DrawString("Mz在各区间大小分别为：\n\n" +
                "0≤x≤a:    Mz=" + Math.Round(fqzSection[0], 2) + "x\n" +
                "a<x≤3a:    Mz=" + Math.Round(mz[100], 2) + plusOrMinus[1] + Math.Round(fqzSection[1], 2) + "(x-a)\n" +
                "3a<x≤4a:   Mz=" + Math.Round(mz[300], 2) + plusOrMinus[2] + Math.Round(fqzSection[2], 2) + "(x-3a)\n" +
                "4a<x≤5a:   Mz=" + Math.Round(mz[400], 2) + plusOrMinus[3] + Math.Round(fqzSection[3], 2) + "(x-4a)",
                font1, brush1, x3, y3 + 120);

            //剪力图y
            fbz = (-f * Math.Sin(degree) + 12 * f2) / 3;
            faz = -fbz + f * Math.Sin(degree) + 3 * f2;
            g.DrawString("Faz=" + Math.Round(faz,2)+"N", font1, brush1, 400, 10);
            g.DrawString("Fbz=" + Math.Round(fbz,2)+"N", font1, brush1, 400, 30);
            float x4 = x3, y4 = y3 + 400;
            MyDrawing.DrawXY(g, x4, y4);
            g.DrawString("剪力图y", font2, brush1, 10, y4);
            g.DrawString("Fq/N", font2, brush1, x4 + 5, y4 - 120);
            double[] fqySection = new double[3];
            double[] fqySectionRatio = new double[3];
            fqySection[0] = -f * Math.Sin(degree);
            fqySection[1] = fqySection[0] + faz;
            fqySection[2] = fqySection[1] + fbz;
            max = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(fqySection[i]) > max)
                {
                    max = Math.Abs(fqySection[i]);
                }
                if (fqySection[i] > 0)
                {
                    stringUpOrDown[i] = -20;
                }
                else
                {
                    stringUpOrDown[i] = 5;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                fqySectionRatio[i] = fqySection[i] * 100 / max;
            }
            PointF[] ptFqy = new PointF[7] {new PointF(x4,(float)(y4-fqySectionRatio[0])),new PointF(x4+100,(float)(y4-fqySectionRatio[0])),
                new PointF(x4+100,(float)(y4-fqySectionRatio[1])), new PointF(x4+400,(float)(y4-fqySectionRatio[1])),
                new PointF(x4+400, (float)(y4-fqySectionRatio[2])),new PointF(x4+500,(float)(y4-fqySectionRatio[2])),
                new PointF(x4+500,y4)};
            g.DrawLines(pen2, ptFqy);
            MyDrawing.DrawShodow(g, new PointF(x4, y4), ptFqy[1]);
            MyDrawing.DrawShodow(g, ptFqy[2], new PointF(x4 + 400, y4));
            MyDrawing.DrawShodow(g, new PointF(x2 + 400, y4), ptFqy[5]);
            g.DrawString("" + Math.Round(Math.Abs(fqySection[0]), 2), font1, brush1,
                x4 + 10, (float)(y4 - fqySectionRatio[0] + stringUpOrDown[0]));
            g.DrawString("" + Math.Round(Math.Abs(fqySection[1]), 2), font1, brush1,
                x4 + 200, (float)(y4 - fqySectionRatio[1] + stringUpOrDown[1]));
            g.DrawString("" + Math.Round(Math.Abs(fqySection[2]), 2), font1, brush1,
                x4 + 400, (float)(y4 - fqySectionRatio[2] + stringUpOrDown[2]));

            //弯矩图y
            for (int i = 0; i < 501; i++)
            {
                double x = a * i / 100.0;
                if (i >= 0 && i <= 100)
                {
                    my[i] = fqySection[0] * x;
                }
                else if (i > 100 && i <= 400)
                {
                    my[i] = my[100] + fqySection[1] * (x - a);
                }
                else
                {
                    my[i] = my[400] + fqySection[2] * (x - 4 * a);
                }
            }
            double[] myRatio = new double[6];
            max = 0;
            for (int i = 0; i < 6; i++)
            {
                myRatio[i] = my[i * 100];
                if (Math.Abs(myRatio[i]) > max)
                {
                    max = Math.Abs(myRatio[i]);
                }
                if (myRatio[i] > 0)
                {
                    stringUpOrDown[i] = -20;
                }
                else
                {
                    stringUpOrDown[i] = 5;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                myRatio[i] = myRatio[i] * 100 / max;
            }
            float x5 = x4, y5 = y4 + 250;
            MyDrawing.DrawXY(g, x5, y5);
            g.DrawString("弯矩图y", font2, brush1, 10, y5);
            g.DrawString("My/(N·m)", font2, brush1, x5 + 5, y5 - 120);
            PointF[] ptMy = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                ptMy[i] = new PointF(x5 + i * 100, y5 - (float)(myRatio[i]));
            }
            g.DrawLines(pen2, ptMy);
            MyDrawing.DrawShadowM(g, x5, y5, my, max);
            MyDrawing.DrawStringM(g, x5, y5, my, stringUpOrDown, max);
            for (int i = 0; i < 3; i++)
            {
                if (fqySection[i] >= 0)
                {
                    plusOrMinus[i] = "+";
                }
                else
                {
                    plusOrMinus[i] = "";
                }
            }
            g.DrawString("My在各区间大小分别为：\n\n" +
                "0≤x≤a:    My=" + Math.Round(fqySection[0], 2) + "x\n" +
                "a<x≤4a:    My=" + Math.Round(my[100], 2) + plusOrMinus[1] + Math.Round(fqySection[1], 2) + "(x-a)\n" +
                "4a<x≤5a:   My=" + Math.Round(my[400], 2) + plusOrMinus[3] + Math.Round(fqySection[2], 2) + "(x-4a)",
                font1, brush1, x5, y5 + 120);
        }

        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)//导入（&I）
        {
            MessageBox.Show("有点累，不写了");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // treeView1.node
            status = 0;
            treeView1.Nodes[0].ForeColor = Color.Red;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (status == 0)
            {
                if (e.Node.Index != 0)
                {
                    MessageBox.Show("请先输入数据！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    treeView1.SelectedNode = treeView1.Nodes[0];
                    treeView1.Nodes[0].ForeColor = Color.Red;
                    tabControl1.SelectedIndex = 0;
                }
            }
            else
            {
                tabControl1.SelectTab(e.Node.Index);
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (status == 0)
            {
                if (e.TabPageIndex != 0)
                {
                    MessageBox.Show("请先输入数据！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl1.SelectedIndex = 0;
                }
            }
            else
            {
                treeView1.Nodes[e.TabPageIndex].ForeColor = Color.Red;
                for (int i = 0; i < treeView1.GetNodeCount(false); i++)
                {
                    if (i != e.TabPageIndex)
                    {
                        treeView1.Nodes[i].ForeColor = Color.Black;
                    }
                }
            }
        }
    }
}
