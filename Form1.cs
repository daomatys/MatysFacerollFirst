using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MatysApp
{
    public partial class Form1 : Form
    {
        const int Wall = 4; //буфер вокруг обозримого поля 
        const int Cell = 18; //размер клетки
        const int ISize = 50 + Wall * 2;
        const int JSize = 21 + Wall * 2;
        static int[,] a = new int[ISize, JSize];
        static int MatchTurns, SummaryTurns;
        static int NullCount, CrossCount;
        static bool Result = false; //победа
        static bool IsX = false; //чей ход 
        static bool Intellect = false; //ai

        public Form1() 
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //TIRED
        {
            label2.ResetText();
            label3.ResetText();
            panel2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            MatchTurns = 0;
            SummaryTurns = 0;
            NullCount = 0;
            CrossCount = 0;
            IsX = false;
            Result = false;
        }

        private void button2_Click(object sender, EventArgs e) //MIND
        {
            if (MatchTurns == 0 && SummaryTurns == 0 )
            {
                if (!Intellect)
                {
                    Intellect = true;
                    button2.Text = "AI ON";
                }
                else
                if (Intellect)
                {
                    Intellect = false;
                    button2.Text = "AI OFF";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //START
        {
            InitArray();
            button3.Visible = false;
            button4.Visible = false;
            panel2.Visible = false;
            label1.Visible = true;
            label1.Text = "GO!";
        }

        private void button4_Click(object sender, EventArgs e) //LATER
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gPanel = panel1.CreateGraphics();
            ActualSurface(gPanel);
        }

        static void ActualSurface(Graphics gPanel)
        {
            Pen z = new Pen(Color.Gray, 1);
            Pen r = new Pen(Color.DimGray, 3);
            Pen x = new Pen(Color.IndianRed, 3);
            Pen o = new Pen(Color.DarkTurquoise, 3);
            ActualMarks(gPanel, r, x, o);
            ActualContainers(gPanel, z);
        }

        static void ActualMarks(Graphics gPanel, Pen r, Pen x, Pen o)
        {
            for (int i = 4; i < ISize - Wall; i++)
                for (int j = 4; j < JSize - Wall; j++)
                    switch (a[i, j])
                    {
                        case -1:
                            if (Result)
                            {
                                a[i, j] = 3;
                                MarkX(gPanel, r, i, j);
                            }
                            else MarkX(gPanel, x, i, j);
                            break;
                        case 0:
                            if (Result)
                            {
                                a[i, j] = 4;
                                MarkO(gPanel, r, i, j);
                            }
                            else MarkO(gPanel, o, i, j);
                            break;
                        case 3:
                            MarkX(gPanel, r, i, j);
                            break;
                        case 4:
                            MarkO(gPanel, r, i, j);
                            break;
                    }
        }

        static void ActualContainers(Graphics gPanel, Pen z)
        {
            int c = 3;
            for (int i = 0; i < ISize - Wall * 2 + 1; i++)
                for (int j = 0; j < JSize - Wall * 2 + 1; j++)
                {
                    gPanel.DrawLine(z, i * Cell - 1, j * Cell - c - 1, i * Cell - 1, j * Cell + c - 1);
                    gPanel.DrawLine(z, i * Cell - c - 1, j * Cell - 1, i * Cell + c - 1, j * Cell - 1);
                }
        }


        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics gPanel = panel1.CreateGraphics();
            int i = (e.Location.X - 1) / Cell + Wall;
            int j = (e.Location.Y - 1) / Cell + Wall;
            while (a[i, j + 1] == 1)
                j++;
            if (a[i, j] == 1)
            {
                TurnMY(gPanel, label1, label2, label3, i, j);
                Thread.Sleep(10);
                TurnAI(gPanel, label1, label2, label3, i, j);
            }
        }

        static void TurnMY(Graphics gPanel, Label label1, Label label2, Label label3, int i, int j)
        {
            if (!Result && a[i, j + 1] != 1 && i < ISize - Wall && j < JSize - Wall)
                AfterTurn(gPanel, label1, label2, label3, i, j);
        }

        static void TurnAI(Graphics gPanel, Label label1, Label label2, Label label3, int i, int j)
        {
            if (!Result && Intellect && a[i, j - 1] == 1)
            {
                Random rnd = new Random();
                i += rnd.Next(-2, 2);
                while (a[i, j] != 1)
                    j--;
                while (a[i, j + 1] == 1)
                    j++;
                AfterTurn(gPanel, label1, label2, label3, i, j);
            }
        }

        static void Mark(Graphics gPanel, int i, int j)
        {
            switch (a[i, j] = IsX ? -1 : 0)
            {
                case -1:
                    Pen x = new Pen(Color.IndianRed, 3);
                    MarkX(gPanel, x, i, j);
                    break;
                case 0:
                    Pen o = new Pen(Color.DarkTurquoise, 3);
                    MarkO(gPanel, o, i, j);
                    break;
            }
        }

        static void MarkX(Graphics gPanel, Pen r, int i, int j)
        {
            gPanel.DrawLine(r, (i - Wall) * Cell + 1, 1 + (j - Wall) * Cell, (i - Wall) * Cell + 15, 15 + (j - Wall) * Cell);
            gPanel.DrawLine(r, (i - Wall) * Cell + 15, 1 + (j - Wall) * Cell, (i - Wall) * Cell + 1, 15 + (j - Wall) * Cell);
        }

        static void MarkO(Graphics gPanel, Pen r, int i, int j)
        {
            gPanel.DrawEllipse(r, (i - Wall) * Cell + 1, 1 + (j - Wall) * Cell, 14, 14);
        }

        static void AfterTurn(Graphics gPanel, Label label1, Label label2, Label label3, int i, int j)
        {
            Mark(gPanel, i, j);
            CheckWin(i, j);
            MatchTurns++;
            label1.Text = $"  {MatchTurns}";
            if (Result)
            {
                Salute(gPanel, label2, label3);
                label1.Text = "NXT";
            }
            IsX = !IsX;
        }

        static void Salute(Graphics gPanel, Label label2, Label label3)
        {
            Random rnd = new Random();
            switch (IsX)
            {
                case true:
                    SaluteCross(gPanel, rnd);
                    label2.Text = $"X : {++CrossCount}";
                    break;
                case false:
                    SaluteNull(gPanel, rnd);
                    label3.Text = $"O : {++NullCount}";
                    break;
            }
        }

        static void SaluteCross(Graphics gPanel, Random rnd)
        {
            int red, blue, lineToX, lineToY;
            for (int k = 0; k < 300; k++)
            {
                red = rnd.Next(60, 255);
                blue = rnd.Next(30, 85);
                Pen w = new Pen(Color.FromArgb(red, k / 120, blue), rnd.Next(6, 17));
                lineToX = rnd.Next(-600, 1700);
                lineToY = rnd.Next(0, 1000);
                gPanel.DrawLine(w, 448, -17, lineToX, lineToY);
                Thread.Sleep(5);
            }
        }

        static void SaluteNull(Graphics gPanel, Random rnd)
        {
            int red, blue, circleX, circleY;
            for (int k = 0; k < 300; k++)
            {
                red = rnd.Next(70, 190);
                blue = rnd.Next(100, 235);
                Pen v = new Pen(Color.FromArgb(red, 1, blue), rnd.Next(3, 55));
                circleX = rnd.Next(-100, 999);
                circleY = rnd.Next(-100, 478);
                gPanel.DrawEllipse(v, circleX, circleY, k + 10, k + 10);
                Thread.Sleep(5);
            }
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics gPanel = panel1.CreateGraphics();
            if (Result)
            {
                AfterSalute(gPanel);
                panel2.Visible = true;
                panel2.Visible = false;
                Result = false;
                SummaryTurns += MatchTurns;
                MatchTurns = 0;
                label1.Text = "GO!";
            }
        }

        static void AfterSalute(Graphics gPanel)
        {
            Random rnd = new Random();
            for (int i = 1; i < 40; i++)
            {
                Pen u = new Pen(Color.FromArgb(33, 43, 52), i * 5);
                gPanel.DrawEllipse(u, rnd.Next(-100, 999), rnd.Next(-100, 478), i * 5, i * 5);
                Thread.Sleep(10);
                gPanel.DrawEllipse(u, rnd.Next(-100, 999), rnd.Next(-100, 478), i * 5, i * 5);
                Thread.Sleep(10);
            }
        }


        static void InitArray()
        {
            for (int i = 0; i < ISize; i++)
                for (int j = 0; j < JSize; j++)
                    if (j >= JSize - Wall)
                        a[i, j] = 2;
                    else a[i, j] = 1;
        }

        static void CheckWin(int i, int j)
        {
            CheckHorizon(a, i, j);
            CheckVertical(i, j);
            CheckDiagonalOne(a, i, j);
            CheckDiagonalTwo(i, j);
        }

        static void CheckHorizon(int[,] a, int i, int j)
        {
            int m = a[i, j];
            if (a[i, j - 1] == m && m == a[i, j + 1]) // ooo?X?ooo
            {
                if (a[i, j - 2] == m && m == a[i, j + 2]) // oo?xXx?oo
                    Result = true;
                else
                {
                    if (a[i, j - 2] == m && a[i, j - 3] == m) // o??xXx
                        Result = true;
                    else
                        if (m == a[i, j + 2] && m == a[i, j + 3]) // xXx??o
                        Result = true;
                }
            }
            else
            {
                if (a[i, j - 1] == m && a[i, j - 2] == m && a[i, j - 3] == m && a[i, j - 4] == m) // ????X
                    Result = true;
                else
                    if (m == a[i, j + 1] && m == a[i, j + 2] && m == a[i, j + 3] && m == a[i, j + 4]) // X????
                    Result = true;
            }
        }

        static void CheckVertical(int i, int j)
        {
            int[,] b = new int [9, 9];
            for (int p = 0; p < 9; p++)
                for (int q = 0; q < 9; q++)
                    b[q, p] = a[i - 4 + p, j - 4 + q];
            CheckHorizon(b, 4, 4);
        }

        static void CheckDiagonalOne(int[,] a, int i, int j)
        {
            int m = a[i, j];
            if (a[i - 1, j - 1] == m && m == a[i + 1, j + 1])
            {
                if (a[i - 2, j - 2] == m && m == a[i + 2, j + 2])
                    Result = true;
                else
                {
                    if (a[i - 2, j - 2] == m && a[i - 3, j - 3] == m)
                        Result = true;
                    else
                        if (m == a[i + 2, j + 2] && m == a[i + 3, j + 3])
                        Result = true;
                }
            }
            else
            {
                if (a[i - 1, j - 1] == m && a[i - 2, j - 2] == m && a[i - 3, j - 3] == m && a[i - 4, j - 4] == m)
                    Result = true;
                else
                    if (m == a[i + 1, j + 1] && m == a[i + 2, j + 2] && m == a[i + 3, j + 3] && m == a[i + 4, j + 4])
                    Result = true;
            }
        }

        static void CheckDiagonalTwo(int i, int j)
        {
            int[,] b = new int[9, 9];
            for (int p = 0; p < 9; p++)
                for (int q = 0; q < 9; q++)
                    b[8 - p, q] = a[i - 4 + p, j - 4 + q];
            CheckDiagonalOne(b, 4, 4);
        }
    }
}