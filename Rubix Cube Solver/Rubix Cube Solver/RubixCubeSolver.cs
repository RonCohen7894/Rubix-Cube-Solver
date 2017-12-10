using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rubix_Cube_Solver
{
    public partial class RubixCubeSolver : Form
    {
        public RubixCubeSolver()
        {
            InitializeComponent();
            if (!Directory.Exists(@"C:\RCS\")) Directory.CreateDirectory(@"C:\RCS\");
            if (File.Exists(@"C:\RCS\log.txt")) File.Delete(@"C:\RCS\log.txt");          
        }
        //The cube is in a fixed position, white on top, red in front

        #region variables

        Color draw_color = Color.Black;
        private Color[,] White_Array;
        private Color[,] Green_Array;
        private Color[,] Red_Array;
        private Color[,] Blue_Array;
        private Color[,] Orange_Array;
        private Color[,] Yellow_Array;

        private Color[,] temp_White_Array;
        private Color[,] temp_Green_Array;
        private Color[,] temp_Red_Array;
        private Color[,] temp_Blue_Array;
        private Color[,] temp_Orange_Array;
        private Color[,] temp_Yellow_Array;

        private bool move_sides_DONE = false;
        private bool move_edges_DONE = false;
        private bool move_second_sides_DONE = false;
        private bool bottom_DONE = false;
        private bool last_layer_DONE = false;

        private bool corners = false;
        private bool centers = false;

        private List<String> log_data = new List<string>();

        DialogResult dialogresult;
        bool P_popup = false;
        #endregion

        #region Pick Color / Reset
        private void pick_color(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Button[] colors = { White, Yellow, Red, Orange, Green, Blue };
            foreach (Button c in colors)
                c.FlatAppearance.BorderSize = 1;

            btn.FlatAppearance.BorderSize = 3;
            draw_color = btn.BackColor;
        }

        private void Bchange_color(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            if (draw_color != Color.Black)
                pb.BackColor = draw_color;

            move_edges_DONE = false;
            move_sides_DONE = false;
            move_second_sides_DONE = false;
            bottom_DONE = false;
            last_layer_DONE = false;
            corners = false;
            centers = false;
        }

        private void Breset_Click(object sender, EventArgs e)
        {
            //White
            PictureBox[] whites = { a1, a2, a3, b1, b2, b3, c1, c2, c3 };
            foreach (PictureBox w in whites)
                w.BackColor = Color.White;

            //Yellows
            PictureBox[] yellows = { g1, g2, g3, h1, h2, h3, i1, i2, i3 };
            foreach (PictureBox y in yellows)
                y.BackColor = Color.Yellow;

            //Red
            PictureBox[] reds = { d4, d5, d6, e4, e5, e6, f4, f5, f6 };
            foreach (PictureBox r in reds)
                r.BackColor = Color.Red;

            //Orange
            PictureBox[] oranges = { d10, d11, d12, e10, e11, e12, f10, f11, f12 };
            foreach (PictureBox o in oranges)
                o.BackColor = Color.Orange;

            //Green
            PictureBox[] green = { d1, d2, d3, e1, e2, e3, f1, f2, f3 };
            foreach (PictureBox g in green)
                g.BackColor = Color.Green;

            //Blue
            PictureBox[] blues = { d7, d8, d9, e7, e8, e9, f7, f8, f9 };
            foreach (PictureBox b in blues)
                b.BackColor = Color.Blue;
        }

        #endregion      

        private Color c(PictureBox PB)
        {
            return PB.BackColor;
        }

        private void log(string content, bool l = true)
        {                       
            if (l)
            {
                log_data.Add(content);
                int count = log_data.Count;

                //four in a row
                if (log_data[count - 1] == "down " && log_data[count - 2] == "down " &&
                    log_data[count - 3] == "down " && log_data[count - 4] == "down ")                
                    for (int i = 1; i <= 4; i++)
                        log_data.RemoveAt(count - i);

                    if ((count + 1) % 10 == 0)
                        log_data.Add("-----------------------------");

                if (last_layer_DONE)
                {        
                    for (int i = 0; i < log_data.Count; i++)
                    {
                        //three in a row
                        if (log_data[i] == "down " && log_data[i + 1] == "down " &&
                            log_data[i + 2] == "down " && log_data[i + 3] != "down ")
                        {
                            log_data.RemoveAt(i);
                            log_data.RemoveAt(i);
                            log_data[i] = "I-down ";
                        }

                        //two in a row
                        if (log_data[i] == "down " && log_data[i + 1] == "down " &&
                            log_data[i + 2] != "down ")
                        {
                            log_data.RemoveAt(i);
                            log_data[i] = "downX2 ";
                        }

                    }
                    string path = @"C:\RCS\log.txt";

                    using (StreamWriter sw = File.AppendText(path))
                        foreach (string item in log_data)
                            sw.WriteLine(item + " ");
                }
            }           
        }

        private void Solve_Click(object sender, EventArgs e)
        {
            Update();

            if (!check_cube_solve())
            {
                log("============SOLVE================ " + DateTime.Now);
                if (!move_sides_DONE)
                    move_sides(); P_popup = false; 
                if (!move_edges_DONE)
                    move_edges(); P_popup = false;
                if (!move_second_sides_DONE)
                    move_second_sides(); P_popup = false;
                if (!bottom_DONE)
                    bottom(); P_popup = false;
                if (!last_layer_DONE)
                    last_layer(); P_popup = false;
            }
        }

        private void right(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {
                #region Array Color Change

                #region white
                for (int i = 0; i <= 2; i++)
                    White_Array[i, 2] = temp_Red_Array[i, 2];
                #endregion

                #region blue
                //Blue - top        
                Blue_Array[0, 0] = temp_Blue_Array[2, 0];
                Blue_Array[0, 1] = temp_Blue_Array[1, 0];
                Blue_Array[0, 2] = temp_Blue_Array[0, 0];
                //Blue - right
                for (int i = 0; i <= 2; i++)
                    Blue_Array[i, 2] = temp_Blue_Array[0, i];
                //Blue - bottom
                Blue_Array[2, 2] = temp_Blue_Array[0, 2];
                Blue_Array[2, 1] = temp_Blue_Array[1, 2];
                Blue_Array[2, 0] = temp_Blue_Array[2, 2];
                //Blue - left
                for (int i = 0; i <= 2; i++)
                    Blue_Array[i, 0] = temp_Blue_Array[2, i];

                #endregion

                #region yellow
                Yellow_Array[0, 2] = temp_Orange_Array[2, 0];
                Yellow_Array[1, 2] = temp_Orange_Array[1, 0];
                Yellow_Array[2, 2] = temp_Orange_Array[0, 0];
                #endregion

                #region orange
                Orange_Array[0, 0] = temp_White_Array[2, 2];
                Orange_Array[1, 0] = temp_White_Array[1, 2];
                Orange_Array[2, 0] = temp_White_Array[0, 2];
                #endregion

                #region red
                for (int i = 0; i <= 2; i++)
                    Red_Array[i, 2] = temp_Yellow_Array[i, 2];
                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] whites = { a3, b3, c3 };
                for (int i = 0; i <= 2; i++)
                    whites[i].BackColor = temp_Red_Array[i, 2];
                #endregion

                #region yellow
                PictureBox[] yellows = { g3, h3, i3 };
                yellows[0].BackColor = temp_Orange_Array[2, 0];
                yellows[1].BackColor = temp_Orange_Array[1, 0];
                yellows[2].BackColor = temp_Orange_Array[0, 0];
                #endregion

                #region orange
                PictureBox[] oranges = { d10, e10, f10 };
                oranges[0].BackColor = temp_White_Array[2, 2];
                oranges[1].BackColor = temp_White_Array[1, 2];
                oranges[2].BackColor = temp_White_Array[0, 2];
                #endregion

                #region blue
                PictureBox[] Blues = { d7, d8, d9, e7, e8, e9, f7, f8, f9 };
                //top
                Blues[0].BackColor = temp_Blue_Array[2, 0];
                Blues[1].BackColor = temp_Blue_Array[1, 0];
                Blues[2].BackColor = temp_Blue_Array[0, 0];
                //bottom
                Blues[6].BackColor = temp_Blue_Array[2, 2];
                Blues[7].BackColor = temp_Blue_Array[1, 2];
                Blues[8].BackColor = temp_Blue_Array[0, 2];
                //right
                PictureBox[] Blues1 = { d9, e9, f9 };
                for (int i = 0; i <= 2; i++)
                    Blues1[i].BackColor = temp_Blue_Array[0, i];
                //left
                PictureBox[] Blues2 = { d7, e7, f7 };
                for (int i = 0; i <= 2; i++)
                    Blues2[i].BackColor = temp_Blue_Array[2, i];
                #endregion

                #region red
                PictureBox[] reds = { d6, e6, f6 };
                for (int i = 0; i <= 2; i++)
                    reds[i].BackColor = temp_Yellow_Array[i, 2];
                #endregion

                #endregion

                Update();
            }
            if (r == 3)
                log("I-right ", l);
            else if (r == 2) { log("rightX2 ", l); }
            else
                log("right ", l);
        }

        private void front(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {
                #region Array Color Change

                #region white
                White_Array[2, 0] = temp_Green_Array[2, 2];
                White_Array[2, 1] = temp_Green_Array[1, 2];
                White_Array[2, 2] = temp_Green_Array[0, 2];
                #endregion

                #region blue
                for (int i = 0; i <= 2; i++)
                    Blue_Array[i, 0] = temp_White_Array[2, i];
                #endregion

                #region yellow
                for (int i = 0; i <= 2; i++)
                    Yellow_Array[0, i] = temp_Blue_Array[2 - i, 0];
                #endregion

                #region green
                for (int i = 0; i <= 2; i++)
                    Green_Array[i, 2] = temp_Yellow_Array[0, i];
                #endregion

                #region red
                //red - top        
                Red_Array[0, 0] = temp_Red_Array[2, 0];
                Red_Array[0, 1] = temp_Red_Array[1, 0];
                Red_Array[0, 2] = temp_Red_Array[0, 0];
                //red - right
                for (int i = 0; i <= 2; i++)
                    Red_Array[i, 2] = temp_Red_Array[0, i];
                //red - bottom
                Red_Array[2, 2] = temp_Red_Array[0, 2];
                Red_Array[2, 1] = temp_Red_Array[1, 2];
                Red_Array[2, 0] = temp_Red_Array[2, 2];
                //red - left
                for (int i = 0; i <= 2; i++)
                    Red_Array[i, 0] = temp_Red_Array[2, i];

                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] whites = { c1, c2, c3 };
                whites[0].BackColor = temp_Green_Array[2, 2];
                whites[1].BackColor = temp_Green_Array[1, 2];
                whites[2].BackColor = temp_Green_Array[0, 2];
                #endregion

                #region yellow
                PictureBox[] yellows = { g1, g2, g3 };
                yellows[0].BackColor = temp_Blue_Array[2, 0];
                yellows[1].BackColor = temp_Blue_Array[1, 0];
                yellows[2].BackColor = temp_Blue_Array[0, 0];
                #endregion

                #region green
                PictureBox[] greens = { d3, e3, f3 };
                for (int i = 0; i <= 2; i++)
                    greens[i].BackColor = temp_Yellow_Array[0, i];
                #endregion

                #region blue
                PictureBox[] blues = { d7, e7, f7 };
                for (int i = 0; i <= 2; i++)
                    blues[i].BackColor = temp_White_Array[2, i];
                #endregion

                #region red
                PictureBox[] reds = { d4, d5, d6, e4, e5, e6, f4, f5, f6 };
                //top
                reds[0].BackColor = temp_Red_Array[2, 0];
                reds[1].BackColor = temp_Red_Array[1, 0];
                reds[2].BackColor = temp_Red_Array[0, 0];
                //bottom
                reds[6].BackColor = temp_Red_Array[2, 2];
                reds[7].BackColor = temp_Red_Array[1, 2];
                reds[8].BackColor = temp_Red_Array[0, 2];
                //right
                PictureBox[] reds1 = { d6, e6, f6 };
                for (int i = 0; i <= 2; i++)
                    reds1[i].BackColor = temp_Red_Array[0, i];
                //left
                PictureBox[] reds2 = { d4, e4, f4 };
                for (int i = 0; i <= 2; i++)
                    reds2[i].BackColor = temp_Red_Array[2, i];

                #endregion

                #endregion

                Update();

            }
            if (r == 3)
                log("I-front ", l);
            else if (r == 2) { log("frontX2 ", l); }
            else
                log("front ", l);
        }

        private void left(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {
                #region Array Color Change

                #region white
                White_Array[2, 0] = temp_Orange_Array[0, 2];
                White_Array[1, 0] = temp_Orange_Array[1, 2];
                White_Array[0, 0] = temp_Orange_Array[2, 2];
                #endregion

                #region green           
                //Green - top        
                Green_Array[0, 0] = temp_Green_Array[2, 0];
                Green_Array[0, 1] = temp_Green_Array[1, 0];
                Green_Array[0, 2] = temp_Green_Array[0, 0];
                //Green - right
                for (int i = 0; i <= 2; i++)
                    Green_Array[i, 2] = temp_Green_Array[0, i];
                //Green - bottom
                Green_Array[2, 2] = temp_Green_Array[0, 2];
                Green_Array[2, 1] = temp_Green_Array[1, 2];
                Green_Array[2, 0] = temp_Green_Array[2, 2];
                //Green - left
                for (int i = 0; i <= 2; i++)
                    Green_Array[i, 0] = temp_Blue_Array[2, i];
                #endregion

                #region yellow
                for (int i = 0; i <= 2; i++)
                    Yellow_Array[i, 0] = temp_Red_Array[i, 0];
                #endregion

                #region orange
                Orange_Array[2, 2] = temp_Yellow_Array[0, 0];
                Orange_Array[1, 2] = temp_Yellow_Array[1, 0];
                Orange_Array[0, 2] = temp_Yellow_Array[2, 0];
                #endregion

                #region red
                for (int i = 0; i <= 2; i++)
                    Red_Array[i, 0] = temp_White_Array[i, 0];
                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] whites = { a1, b1, c1 };
                whites[0].BackColor = temp_Orange_Array[2, 2];
                whites[1].BackColor = temp_Orange_Array[1, 2];
                whites[2].BackColor = temp_Orange_Array[0, 2];
                #endregion

                #region yellow
                PictureBox[] yellows = { g1, h1, i1 };
                for (int i = 0; i <= 2; i++)
                    yellows[i].BackColor = temp_Red_Array[i, 0];
                #endregion

                #region orange
                PictureBox[] oranges = { d12, e12, f12 };
                oranges[0].BackColor = temp_Yellow_Array[2, 0];
                oranges[1].BackColor = temp_Yellow_Array[1, 0];
                oranges[2].BackColor = temp_Yellow_Array[0, 0];
                #endregion

                #region green
                PictureBox[] Greens = { d1, d2, d3, e1, e2, e3, f1, f2, f3 };
                //top
                Greens[0].BackColor = temp_Green_Array[2, 0];
                Greens[1].BackColor = temp_Green_Array[1, 0];
                Greens[2].BackColor = temp_Green_Array[0, 0];
                //bottom
                Greens[6].BackColor = temp_Green_Array[2, 2];
                Greens[7].BackColor = temp_Green_Array[1, 2];
                Greens[8].BackColor = temp_Green_Array[0, 2];
                //right
                PictureBox[] Greens1 = { d3, e3, f3 };
                for (int i = 0; i <= 2; i++)
                    Greens1[i].BackColor = temp_Green_Array[0, i];
                //left
                PictureBox[] Greens2 = { d1, e1, f1 };
                for (int i = 0; i <= 2; i++)
                    Greens2[i].BackColor = temp_Green_Array[2, i];
                #endregion

                #region red
                PictureBox[] reds = { d4, e4, f4 };
                for (int i = 0; i <= 2; i++)
                    reds[i].BackColor = temp_White_Array[i, 0];
                #endregion

                #endregion

                Update();

            }
            if (r == 3)
                log("I-left ", l);
            else if (r == 2) { log("leftX2 ", l); }
            else
                log("left ", l);
        }

        private void up(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {
                #region Array Color Change

                #region white
                //White - top        
                White_Array[0, 0] = temp_White_Array[2, 0];
                White_Array[0, 1] = temp_White_Array[1, 0];
                White_Array[0, 2] = temp_White_Array[0, 0];
                //White - right
                for (int i = 0; i <= 2; i++)
                    White_Array[i, 2] = temp_White_Array[0, i];
                //White - bottom
                White_Array[2, 2] = temp_White_Array[0, 2];
                White_Array[2, 1] = temp_White_Array[1, 2];
                White_Array[2, 0] = temp_White_Array[2, 2];
                //White - left
                for (int i = 0; i <= 2; i++)
                    White_Array[i, 0] = temp_Blue_Array[2, i];
                #endregion

                #region blue
                for (int i = 0; i <= 2; i++)
                    Blue_Array[0, i] = temp_Orange_Array[0, i];
                #endregion

                #region Orange
                for (int i = 0; i <= 2; i++)
                    Orange_Array[0, i] = temp_Green_Array[0, i];
                #endregion

                #region green
                for (int i = 0; i <= 2; i++)
                    Green_Array[0, i] = temp_Red_Array[0, i];
                #endregion

                #region red
                for (int i = 0; i <= 2; i++)
                    Red_Array[0, i] = temp_Blue_Array[0, i];
                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] Whites = { a1, a2, a3, b1, b2, b3, c1, c2, c3 };
                //top
                Whites[0].BackColor = temp_White_Array[2, 0];
                Whites[1].BackColor = temp_White_Array[1, 0];
                Whites[2].BackColor = temp_White_Array[0, 0];
                //bottom
                Whites[6].BackColor = temp_White_Array[2, 2];
                Whites[7].BackColor = temp_White_Array[1, 2];
                Whites[8].BackColor = temp_White_Array[0, 2];
                //right
                PictureBox[] Whites1 = { a3, b3, c3 };
                for (int i = 0; i <= 2; i++)
                    Whites1[i].BackColor = temp_White_Array[0, i];
                //left
                PictureBox[] Whites2 = { a1, b1, c1 };
                for (int i = 0; i <= 2; i++)
                    Whites2[i].BackColor = temp_White_Array[2, i];
                #endregion

                #region orange
                PictureBox[] oranges = { d10, d11, d12 };
                for (int i = 0; i <= 2; i++)
                    oranges[i].BackColor = temp_Green_Array[0, i];
                #endregion

                #region green
                PictureBox[] greens = { d1, d2, d3 };
                for (int i = 0; i <= 2; i++)
                    greens[i].BackColor = temp_Red_Array[0, i];
                #endregion

                #region blue
                PictureBox[] blues = { d7, d8, d9 };
                for (int i = 0; i <= 2; i++)
                    blues[i].BackColor = temp_Orange_Array[0, i];
                #endregion

                #region red
                PictureBox[] reds = { d4, d5, d6 };
                for (int i = 0; i <= 2; i++)
                {
                    reds[i].BackColor = temp_Blue_Array[0, i];
                }
                #endregion

                #endregion

                Update();

            }
            if (r == 3)
                log("I-up ", l);
            else if (r == 2) { log("upX2 ", l); }
            else
                log("up ", l);
        }

        private void down(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {

                #region Array Color Change

                #region yellow
                //Yellow - top        
                Yellow_Array[0, 0] = temp_Yellow_Array[2, 0];
                Yellow_Array[0, 1] = temp_Yellow_Array[1, 0];
                Yellow_Array[0, 2] = temp_Yellow_Array[0, 0];
                //Yellow - right
                for (int i = 0; i <= 2; i++)
                    Yellow_Array[i, 2] = temp_Yellow_Array[0, i];
                //Yellow - bottom
                Yellow_Array[2, 2] = temp_Yellow_Array[0, 2];
                Yellow_Array[2, 1] = temp_Yellow_Array[1, 2];
                Yellow_Array[2, 0] = temp_Yellow_Array[2, 2];
                //Yellow - left
                for (int i = 0; i <= 2; i++)
                    Yellow_Array[i, 0] = temp_Blue_Array[2, i];
                #endregion

                #region blue
                for (int i = 0; i <= 2; i++)
                    Blue_Array[2, i] = temp_Red_Array[2, i];
                #endregion

                #region Orange
                for (int i = 0; i <= 2; i++)
                    Orange_Array[2, i] = temp_Blue_Array[2, i];
                #endregion

                #region green
                for (int i = 0; i <= 2; i++)
                    Green_Array[2, i] = temp_Orange_Array[2, i];
                #endregion

                #region red
                for (int i = 0; i <= 2; i++)
                    Red_Array[2, i] = temp_Green_Array[2, i];
                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] Yellows = { g1, g2, g3, h1, h2, h3, i1, i2, i3 };
                //top
                Yellows[0].BackColor = temp_Yellow_Array[2, 0];
                Yellows[1].BackColor = temp_Yellow_Array[1, 0];
                Yellows[2].BackColor = temp_Yellow_Array[0, 0];
                //bottom
                Yellows[6].BackColor = temp_Yellow_Array[2, 2];
                Yellows[7].BackColor = temp_Yellow_Array[1, 2];
                Yellows[8].BackColor = temp_Yellow_Array[0, 2];
                //right
                PictureBox[] Yellows1 = { g3, h3, i3 };
                for (int i = 0; i <= 2; i++)
                    Yellows1[i].BackColor = temp_Yellow_Array[0, i];
                //left
                PictureBox[] Yellows2 = { g1, h1, i1 };
                for (int i = 0; i <= 2; i++)
                    Yellows2[i].BackColor = temp_Yellow_Array[2, i];
                #endregion

                #region orange
                PictureBox[] oranges = { f10, f11, f12 };
                for (int i = 0; i <= 2; i++)
                    oranges[i].BackColor = temp_Blue_Array[2, i];
                #endregion

                #region green
                PictureBox[] greens = { f1, f2, f3 };
                for (int i = 0; i <= 2; i++)
                    greens[i].BackColor = temp_Orange_Array[2, i];
                #endregion

                #region blue
                PictureBox[] blues = { f7, f8, f9 };
                for (int i = 0; i <= 2; i++)
                    blues[i].BackColor = temp_Red_Array[2, i];
                #endregion

                #region red
                PictureBox[] reds = { f4, f5, f6 };
                for (int i = 0; i <= 2; i++)
                    reds[i].BackColor = temp_Green_Array[2, i];
                #endregion

                #endregion

                Update();
            }
            if (r == 3)
                log("I-down ", l);
            else if (r == 2) { log("downX2 ", l); }
            else
                log("down ", l);
        }

        private void back(int r = 1, bool l = true)
        {
            for (int j = 1; j <= r; j++)
            {
                #region Array Color Change

                #region white
                for (int i = 0; i <= 2; i++)
                    White_Array[0, i] = temp_Blue_Array[i, 2];
                #endregion

                #region blue
                Blue_Array[0, 2] = temp_Yellow_Array[2, 2];
                Blue_Array[1, 2] = temp_Yellow_Array[2, 1];
                Blue_Array[2, 2] = temp_Yellow_Array[2, 0];
                #endregion

                #region yellow
                for (int i = 0; i <= 2; i++)
                    Yellow_Array[2, i] = temp_Green_Array[i, 0];

                #endregion

                #region green
                for (int i = 0; i <= 2; i++)
                    Green_Array[i, 0] = temp_White_Array[0, i];
                #endregion

                #region orange
                //Orange - top        
                Orange_Array[0, 0] = temp_Orange_Array[2, 0];
                Orange_Array[0, 1] = temp_Orange_Array[1, 0];
                Orange_Array[0, 2] = temp_Orange_Array[0, 0];
                //Orange - right
                for (int i = 0; i <= 2; i++)
                    Orange_Array[i, 2] = temp_Orange_Array[0, i];
                //Orange - bottom
                Orange_Array[2, 2] = temp_Orange_Array[0, 2];
                Orange_Array[2, 1] = temp_Orange_Array[1, 2];
                Orange_Array[2, 0] = temp_Orange_Array[2, 2];
                //Orange - left
                for (int i = 0; i <= 2; i++)
                    Orange_Array[i, 0] = temp_Orange_Array[2, i];

                #endregion

                #endregion

                #region Cube Color Change

                #region white
                PictureBox[] whites = { a1, a2, a3 };
                for (int i = 0; i <= 2; i++)
                    whites[i].BackColor = temp_Blue_Array[i, 2];

                #endregion

                #region yellow
                PictureBox[] yellows = { i1, i2, i3 };
                for (int i = 0; i <= 2; i++)
                    yellows[i].BackColor = temp_Green_Array[i, 0];

                #endregion

                #region green
                PictureBox[] greens = { f1, e1, d1 };
                for (int i = 0; i <= 2; i++)
                    greens[i].BackColor = temp_White_Array[0, i];
                #endregion

                #region blue
                PictureBox[] blues = { d9, e9, f9 };
                blues[0].BackColor = temp_Yellow_Array[2, 2];
                blues[1].BackColor = temp_Yellow_Array[2, 1];
                blues[2].BackColor = temp_Yellow_Array[2, 0];
                #endregion

                #region orange
                PictureBox[] Oranges = { d10, d11, d12, e10, e11, e12, f10, f11, f12 };
                //top
                Oranges[0].BackColor = temp_Orange_Array[2, 0];
                Oranges[1].BackColor = temp_Orange_Array[1, 0];
                Oranges[2].BackColor = temp_Orange_Array[0, 0];
                //bottom
                Oranges[6].BackColor = temp_Orange_Array[2, 2];
                Oranges[7].BackColor = temp_Orange_Array[1, 2];
                Oranges[8].BackColor = temp_Orange_Array[0, 2];
                //right
                PictureBox[] Oranges1 = { d12, e12, f12 };
                for (int i = 0; i <= 2; i++)
                    Oranges1[i].BackColor = temp_Orange_Array[0, i];
                //left
                PictureBox[] Oranges2 = { d10, e10, f10 };
                for (int i = 0; i <= 2; i++)
                    Oranges2[i].BackColor = temp_Orange_Array[2, i];

                #endregion

                #endregion

                Update();

            }
            if (r == 3)
                log("I-back ", l);
            else if (r == 2) { log("backX2 ", l); }
            else
                log("back ", l);
        }

        private new void Update()
        {
            #region Cube
            White_Array = new Color[,]{ { c(a1), c(a2), c(a3) },
                                    { c(b1), c(b2), c(b3) },
                                    { c(c1), c(c2), c(c3) } };

            Green_Array = new Color[,]{ { c(d1), c(d2), c(d3) },
                                    { c(e1), c(e2), c(e3) },
                                    { c(f1), c(f2), c(f3) } };

            Red_Array = new Color[,]{ { c(d4), c(d5), c(d6) },
                                    { c(e4), c(e5), c(e6) },
                                    { c(f4), c(f5), c(f6) } };

            Blue_Array = new Color[,]{ { c(d7), c(d8), c(d9) },
                                    { c(e7), c(e8), c(e9) },
                                    { c(f7), c(f8), c(f9) } };

            Orange_Array = new Color[,]{ { c(d10), c(d11), c(d12) },
                                    { c(e10), c(e11), c(e12) },
                                    { c(f10), c(f11), c(f12) } };

            Yellow_Array = new Color[,]{ { c(g1), c(g2), c(g3) },
                                    { c(h1), c(h2), c(h3) },
                                    { c(i1), c(i2), c(i3) } };
            #endregion

            #region temp
            temp_White_Array = new Color[,]{ { c(a1), c(a2), c(a3) },
                                    { c(b1), c(b2), c(b3) },
                                    { c(c1), c(c2), c(c3) } };

            temp_Green_Array = new Color[,]{ { c(d1), c(d2), c(d3) },
                                    { c(e1), c(e2), c(e3) },
                                    { c(f1), c(f2), c(f3) } };

            temp_Red_Array = new Color[,]{ { c(d4), c(d5), c(d6) },
                                    { c(e4), c(e5), c(e6) },
                                    { c(f4), c(f5), c(f6) } };

            temp_Blue_Array = new Color[,]{ { c(d7), c(d8), c(d9) },
                                    { c(e7), c(e8), c(e9) },
                                    { c(f7), c(f8), c(f9) } };

            temp_Orange_Array = new Color[,]{ { c(d10), c(d11), c(d12) },
                                    { c(e10), c(e11), c(e12) },
                                    { c(f10), c(f11), c(f12) } };

            temp_Yellow_Array = new Color[,]{ { c(g1), c(g2), c(g3) },
                                    { c(h1), c(h2), c(h3) },
                                    { c(i1), c(i2), c(i3) } };
            #endregion
        }

        private void move_sides()
        {
            popup pop = new popup("move sides");
            if (!P_popup)
            { dialogresult = pop.ShowDialog(); P_popup = true; }

            #region RED TO YELLOW
            if (c(d5) == Color.White || c(f5) == Color.White || c(e4) == Color.White || c(e6) == Color.White)
            {
                //moves white g2 from the way
                do if (c(g2) == Color.White) down();
                while (c(g2) == Color.White);

                //move any white part on the red {} to e6
                if (c(e6) != Color.White)
                    if (c(d5) == Color.White) front();
                    else if (c(f5) == Color.White) front(3);
                    else if (c(e4) == Color.White) front(2);

                //move the white part (e6) to an empty spot in the yellow {}            
                do if (c(h3) == Color.White) down();
                while (c(h3) == Color.White && c(e6) == Color.White);
                right(3);
            }
            #endregion

            #region WHITE TO YELLOW

            if (c(a2) == Color.White || c(c2) == Color.White || c(b1) == Color.White || c(b3) == Color.White)
                if (!(c(a2) == Color.White && c(c2) == Color.White && c(b1) == Color.White && c(b3) == Color.White))
                {
                    //move any white part on the white {} to b3
                    if (c(b3) != Color.White)
                        if (c(a2) == Color.White) up();
                        else if (c(c2) == Color.White) up(3);
                        else if (c(b1) == Color.White) up(2);

                    //move the white part (b3) to an empty spot in the yellow {}            
                    do if (c(h3) == Color.White)
                            down();
                    while (c(h3) == Color.White && c(b3) == Color.White);
                    right(2);
                }

            #endregion

            #region GREEN TO YELLOW

            if (c(d2) == Color.White || c(f2) == Color.White || c(e1) == Color.White || c(e3) == Color.White)
            {
                //moves white g2 from the way
                do if (c(h1) == Color.White) down();
                while (c(h1) == Color.White);

                //move any white part on the green {} to e3
                if (c(e3) != Color.White)
                    if (c(d2) == Color.White) left();
                    else if (c(f2) == Color.White) left(3);
                    else if (c(e1) == Color.White) left(2);

                //move the white part (e3) to an empty spot in the yellow {}            
                do if (c(g2) == Color.White) down();
                while (c(g2) == Color.White && c(e3) == Color.White);
                front(3);
            }

            #endregion

            #region BLUE TO YELLOW

            if (c(d8) == Color.White || c(f8) == Color.White || c(e7) == Color.White || c(e9) == Color.White)
            {
                //moves white g2 from the way
                do if (c(h3) == Color.White) down();
                while (c(h3) == Color.White);

                //move any white part on the blue {} to e7
                if (c(e7) != Color.White)
                    if (c(d8) == Color.White) right(3);
                    else if (c(f8) == Color.White) right();
                    else if (c(e9) == Color.White) right(2);

                //move the white part (e7) to an empty spot in the yellow {}            
                do if (c(g2) == Color.White) down();
                while (c(g2) == Color.White && c(e7) == Color.White);
                front();
            }

            #endregion

            #region Orange TO YELLOW
            if (c(d11) == Color.White || c(f11) == Color.White || c(e10) == Color.White || c(e12) == Color.White)
            {
                //moves white i2 from the way
                do if (c(i2) == Color.White) down();
                while (c(i2) == Color.White);

                //move any white part on the red {} to e12
                if (c(e12) != Color.White)
                    if (c(d11) == Color.White) back();
                    else if (c(f11) == Color.White) back(3);
                    else if (c(e10) == Color.White) back(2);

                //move the white part (e12) to an empty spot in the yellow {}            
                do if (c(h1) == Color.White) down();
                while (c(h1) == Color.White && c(e12) == Color.White);
                left(3);
            }
            #endregion

            if ((c(a2) == Color.White || c(c2) == Color.White ||
                c(b1) == Color.White || c(b3) == Color.White) &&
                (c(d8) == Color.Blue || c(d2) == Color.Green || c(d5) == Color.Red))
            {
                front(2, false); back(2, false); right(2, false); left(1, false);
                move_sides();
            }
            //repeat the above if statements until all the white side parts are in there right position
            else if (c(g2) != Color.White || c(i2) != Color.White ||
                c(h1) != Color.White || c(h3) != Color.White)
                move_sides();

            #region YELLOW TO WHITE
            //moves the corrent side part to the right position on the yellow {} and then moves it to the white {}
            else
            {
                do if (c(h3) != Color.White || c(f8) != Color.Blue) down();
                while (c(h3) != Color.White || c(f8) != Color.Blue);
                right(2);

                do if (c(g2) != Color.White || c(f5) != Color.Red) down();
                while (c(g2) != Color.White || c(f5) != Color.Red);
                front(2);

                do if (c(h1) != Color.White || c(f2) != Color.Green) down();
                while (c(h1) != Color.White || c(f2) != Color.Green);
                left(2);

                do if (c(i2) != Color.White || c(f11) != Color.Orange) down();
                while (c(i2) != Color.White || c(f11) != Color.Orange);
                back(2);
            }
            #endregion

            move_sides_DONE = true;
            //log("-----move_sides_DONE------ ");
        }

        private void move_edges()
        {
            popup pop = new popup("move edges");
            if (!P_popup)
            { dialogresult = pop.ShowDialog(); P_popup = true; }

            #region RED
            //circles through the bottem red line looking for white in f4 or in f6
            for (int i = 1; i <= 4; i++)
            {
                if (c(f4) == Color.White || c(f6) == Color.White)
                {
                    //if f4 is white, checks if the other two parts's colors are corrent 
                    if (c(f4) == Color.White && c(f3) == Color.Green && c(g1) == Color.Red)
                    { down(); left(); down(3); left(3); } //move the corner piece the the white {}

                    //if f6 is white, checks if the other two parts's colors are corrent 
                    else if ((c(f6) == Color.White && c(f7) == Color.Blue && c(g3) == Color.Red))
                    { down(3); right(3); down(); right(); }//move the corner piece the the white {}

                    //if not, continue to move the bottom red line
                    else down();
                }

                else if (c(d4) == Color.White)
                { left(); down(3); left(3); move_edges(); }
                else if (c(d6) == Color.White)
                { right(3); down(); right(); move_edges(); }

                //if not, continue to move the bottom red line
                else down();
            }
            #endregion

            #region GREEN

            //circles through the bottem red line looking for white in f1 or in f3
            for (int i = 1; i <= 4; i++)
            {
                if (c(f1) == Color.White || c(f3) == Color.White)
                {
                    //if f1 is white, checks if the other two parts's colors are corrent 
                    if (c(f1) == Color.White && c(f12) == Color.Orange && c(i1) == Color.Green)
                    { down(); back(); down(3); back(3); } //move the corner piece the the white {}

                    //if f3 is white, checks if the other two parts's colors are corrent 
                    else if ((c(f3) == Color.White && c(f4) == Color.Red && c(g1) == Color.Green))
                    { down(3); front(3); down(); front(); }//move the corner piece the the white {}

                    //if not, continue to move the bottom red line
                    else down();
                }

                else if (c(d1) == Color.White)
                { back(); down(3); back(3); move_edges(); }
                else if (c(d3) == Color.White)
                { front(3); down(); front(); move_edges(); }

                //if not, continue to move the bottom red line
                else down();
            }

            #endregion

            #region BLUE

            //circles through the bottem red line looking for white in f7 or in f9
            for (int i = 1; i <= 4; i++)
            {
                if (c(f7) == Color.White || c(f9) == Color.White)
                {
                    //if f7 is white, checks if the other two parts's colors are corrent 
                    if (c(f7) == Color.White && c(f6) == Color.Red && c(g3) == Color.Blue)
                    { down(); front(); down(3); front(3); } //move the corner piece the the white {}

                    //if f9 is white, checks if the other two parts's colors are corrent 
                    else if ((c(f9) == Color.White && c(f10) == Color.Orange && c(i3) == Color.Blue))
                    { down(3); back(3); down(); back(); }//move the corner piece the the white {}

                    //if not, continue to move the bottom red line
                    else down();
                }

                else if (c(d7) == Color.White)
                { front(); down(3); front(3); move_edges(); }
                else if (c(d9) == Color.White)
                { back(3); down(); back(); move_edges(); }

                //if not, continue to move the bottom red line
                else down();
            }

            #endregion

            #region Orange

            //circles through the bottem red line looking for white in f10 or in f12
            for (int i = 1; i <= 4; i++)
            {
                if (c(f10) == Color.White || c(f12) == Color.White)
                {
                    //if f10 is white, checks if the other two parts's colors are corrent 
                    if (c(f10) == Color.White && c(f9) == Color.Blue && c(i3) == Color.Orange)
                    { down(); right(); down(3); right(3); } //move the corner piece the the white {}

                    //if f12 is white, checks if the other two parts's colors are corrent 
                    else if ((c(f12) == Color.White && c(f1) == Color.Green && c(i1) == Color.Orange))
                    { down(3); left(3); down(); left(); }//move the corner piece the the white {}

                    //if not, continue to move the bottom red line
                    else down();
                }

                else if (c(d10) == Color.White)
                { right(); down(3); right(3); move_edges(); }
                else if (c(d12) == Color.White)
                { left(3); down(); left(); move_edges(); }

                //if not, continue to move the bottom red line
                else down();
            }

            #endregion

            #region YELLOW

            if (c(g1) == Color.White || c(g3) == Color.White ||
                c(i1) == Color.White || c(i3) == Color.White)
            {
                do if (c(i1) != Color.White) down();
                while (c(i1) != Color.White);

                left(3); down(); left(); down(3);
            }
            #endregion            

            if (c(a1) != Color.White || c(a3) != Color.White ||
                c(c1) != Color.White || c(c3) != Color.White)
                move_edges();

            move_edges_DONE = true;
            //log("-----move_edges_DONE------ ");
            
        }

        private void move_second_sides()
        {
            popup pop = new popup("move second sides");
            if (!P_popup)
            { dialogresult = pop.ShowDialog(); P_popup = true; }

            #region RED
            for (int i = 1; i <= 4; i++)
            {
                if (c(f5) == Color.Red && c(g2) == Color.Green || c(e3) != Color.Green || c(e4) != Color.Red)
                { down(); left(); down(3); left(3); down(3); front(3); down(); front(); }

                else if (c(f5) == Color.Red && c(g2) == Color.Blue)
                { down(3); right(3); down(); right(); down(); front(); down(3); front(3); }

                else down();
            }
            #endregion

            #region GREEN
            for (int i = 1; i <= 4; i++)
            {
                if (c(f2) == Color.Green && c(h1) == Color.Red)
                { down(3); front(3); down(); front(); down(); left(); down(3); left(3); ; }

                else if (c(f2) == Color.Green && c(h1) == Color.Orange || c(e1) != Color.Green || c(e12) != Color.Orange)
                { down(); back(); down(3); back(3); down(3); left(3); down(); left(); ; }

                else down();
            }
            #endregion

            #region BLUE
            for (int i = 1; i <= 4; i++)
            {
                if (c(f8) == Color.Blue && c(h3) == Color.Red || c(e6) != Color.Red || c(e7) != Color.Blue)
                { down(); front(); down(3); front(3); down(3); right(3); down(); right(); ; }

                else if (c(f8) == Color.Blue && c(h3) == Color.Orange)
                { down(3); back(3); down(); back(); down(); right(); down(3); right(3); ; }

                else down();
            }
            #endregion

            #region Orange
            for (int i = 1; i <= 4; i++)
            {
                if (c(f11) == Color.Orange && c(i2) == Color.Blue || c(e9) != Color.Blue || c(e10) != Color.Orange)
                { down(); right(); down(3); right(3); down(3); back(3); down(); back(); }

                else if (c(f11) == Color.Orange && c(i2) == Color.Green)
                { down(3); left(3); down(); left(); down(); back(); down(3); back(3); }

                else down();
            }
            #endregion

            if (c(e3) != Color.Green || c(e4) != Color.Red || c(e6) != Color.Red ||
                 c(e7) != Color.Blue || c(e9) != Color.Blue || c(e10) != Color.Orange ||
                 c(e12) != Color.Orange || c(e1) != Color.Green) move_second_sides();
            else
                move_second_sides_DONE = true;
            //log("-----move_second_sides_DONE------ ");
        }

        private void bottom()
        {
            popup pop = new popup("bottom");
            if (!P_popup)
            { dialogresult = pop.ShowDialog(); P_popup = true; }
            bool done = false;
           
            #region has a plus (with or without extras)
            if (c(g2) == Color.Yellow && c(h3) == Color.Yellow && c(i2) == Color.Yellow && c(h1) == Color.Yellow)
            {
                #region only has a plus
                if (c(g1) != Color.Yellow && c(g3) != Color.Yellow && c(i3) != Color.Yellow && c(i1) != Color.Yellow)
                {
                    //moves the bottom layer until there is no yellow part in the bottom orange {}
                    for (int i = 1; i <= 4; i++)
                    {
                        if (c(f10)  == Color.Yellow && c(f12) == Color.Yellow)
                            down();
                        else
                        { done = true; break; }
                    }
                }
                #endregion

                #region fish
                if (done == false && (c(g1) == Color.Yellow || c(g3) == Color.Yellow || c(i3) == Color.Yellow || c(i1) == Color.Yellow))
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (c(i1) == Color.Yellow && c(i3) != Color.Yellow && c(g1) != Color.Yellow && c(g3) != Color.Yellow)
                        { done = true; break;  }
                        else
                            down();
                    }
                }
                #endregion

                #region two yellow edges

                if (done == false &&( c(g1) == Color.Yellow && c(g3) == Color.Yellow ||
                        (c(g3) == Color.Yellow && c(i3) == Color.Yellow) ||
                        (c(i3) == Color.Yellow && c(i1) == Color.Yellow) ||
                        (c(i1) == Color.Yellow && c(g1) == Color.Yellow) ||
                        (c(g1) != Color.Yellow && c(i3) != Color.Yellow) ||
                        (c(g3) != Color.Yellow && c(i1) != Color.Yellow)))
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (c(f12) == Color.Yellow)
                        { done = true; break; }
                        else
                            down();
                    }
                }

                #endregion                               

                done = false;
                right(); down(); right(3); down(); right(); down(2); right(3);
            }
            #endregion

            #region else
            else
            {
                //no part of the plus is yellow
                if (c(g2) != Color.Yellow && c(h3) != Color.Yellow && c(i2) != Color.Yellow && c(h1) != Color.Yellow)
                { back(); down(); right(); down(3); right(3); back(3); }

                #region L shap
                else if (c(g2) == Color.Yellow && c(h3) == Color.Yellow || (c(h3) == Color.Yellow && c(i2) == Color.Yellow) ||
                        (c(i2) == Color.Yellow && c(h1) == Color.Yellow) || (c(h1) == Color.Yellow && c(g2) == Color.Yellow))
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (c(h1) == Color.Yellow && c(g2) == Color.Yellow)
                            break;
                        else
                            down();
                    }
                    back(); right(); down(); right(3); down(3); back(3);
                }
                #endregion

                #region center line

                else if (c(h1) == Color.Yellow && c(h3) == Color.Yellow)
                { back(); right(); down(); right(3); down(3); back(3); }

                else if (c(g2) == Color.Yellow && c(i2) == Color.Yellow)
                { down(); back(); right(); down(); right(3); down(3); back(3); }

                #endregion

                else
                { back(); right(); down(); right(3); down(3); back(3); }
            }
            #endregion

            if (c(g1) != Color.Yellow || c(g2) != Color.Yellow || c(g3) != Color.Yellow ||
                         c(h1) != Color.Yellow || c(h3) != Color.Yellow || c(i1) != Color.Yellow ||
                         c(i2) != Color.Yellow || c(i3) != Color.Yellow) bottom();

            bottom_DONE = true;
            //log("-----bottom_DONE------ ");
        }

        private void last_layer()
        {
            popup pop = new popup("last layer");
            if (!P_popup)
            { dialogresult = pop.ShowDialog(); P_popup = true; }
            #region corners
            for (int j = 1; j <= 4; j++)
            {
                if (c(f4) != c(f6) || c(f1) != c(f3))
                {
                    for (int i = 1; i <= 4; i++)
                        if (c(f4) == c(f6))
                        { right(3); back(); right(3); front(2); right(); back(3); right(3); front(2); right(2); down(3); break; }
                        else
                            down();

                    if (c(f4) != c(f6))
                    { right(3); back(); right(3); front(2); right(); back(3); right(3); front(2); right(2); down(3); }


                    for (int i = 1; i <= 4; i++)
                        if (c(f4) != Color.Red && c(f5) != Color.Red)
                            down();
                        else
                            break;

                    if (c(f4) == c(f6) && c(f1) == c(f3))
                        break;                   
                }
            }
            #endregion

            #region centers
            for (int j = 1; j <= 4; j++)
            {
                if (c(f2) != Color.Green || c(f5) != Color.Red || c(f8) != Color.Blue)
                {
                    for (int i = 1; i <= 4; i++)
                        if ((c(f4) == c(f5) && c(f5) == c(f6)))
                        { back(2); down(3); left(); right(3); back(2); left(3); right(); down(3); back(2); break; }
                        else
                            down();

                    if (c(f4) != c(f5) || c(f5) != c(f6))
                    { back(2); down(3); left(); right(3); back(2); left(3); right(); down(3); back(2); }

                    for (int i = 1; i <= 4; i++)
                        if (c(f5) != Color.Red)
                            down();
                        else
                            break;

                    if (c(f2) == Color.Green && c(f5) == Color.Red && c(f8) == Color.Blue)
                        break;

                }
            }
            #endregion

            if (c(f2) != Color.Green || c(f5) != Color.Red || c(f8) != Color.Blue ||
                c(f1) != Color.Green || c(f4) != Color.Red || c(f7) != Color.Blue)
                last_layer();

            last_layer_DONE = true;
            log("-----last_layer_DONE------ ");
        }

        private void Random_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\RCS\log.txt")) File.Delete(@"C:\RCS\log.txt");
            log_data.Clear();
            Update();

            #region pattern
           /*
            front(1, false);
            left(1, false);
            front(1, false);
            up(3, false);
            right(1, false);
            up(1, false);
            front(2, false);
            left(2, false);
            up(3, false);
            left(3, false);
            back(1, false);
            down(3, false);
            back(3, false);
            left(2, false);
            up(1, false);*/
            #endregion
                         
            #region Randomize
            Random ran = new Random();
            List<Int32> moves = new List<Int32>();

            for (int i = 0; i < 1000; i++)
                moves.Add(ran.Next(1, 7));

            for (int i = 0; i < moves.Count; i++)
            {
                switch (moves[i])
                {
                    case 1:
                        front(1, false);
                        break;
                    case 2:
                        back(1, false);
                        break;
                    case 3:
                        right(1, false);
                        break;
                    case 4:
                        left(1, false);
                        break;
                    case 5:
                        up(1, false);
                        break;
                    case 6:
                        down(1, false);
                        break;
                }
            }


                #endregion
               
            move_edges_DONE = false;
            move_sides_DONE = false;
            move_second_sides_DONE = false;
            bottom_DONE = false;
            last_layer_DONE = false;
            corners = false;
            centers = false;
        }

        bool check_cube_solve()
        {
            bool white = false, yellow = false, green = false, blue = false, red = false, orange = false;

            PictureBox[] whites = { a1, a2, a3, b1, b2, b3, c1, c2, c3 };
            foreach (PictureBox w in whites)
                if (w.BackColor == Color.White) white = true; else { white = false; break; }

            //Yellows
            PictureBox[] yellows = { g1, g2, g3, h1, h2, h3, i1, i2, i3 };
            foreach (PictureBox y in yellows)
                if (y.BackColor == Color.Yellow) yellow = true; else { yellow = false; break; }

            //Red
            PictureBox[] reds = { d4, d5, d6, e4, e5, e6, f4, f5, f6 };
            foreach (PictureBox r in reds)
                if (r.BackColor == Color.Red) red = true; else { red = false; break; }

            //Orange
            PictureBox[] oranges = { d10, d11, d12, e10, e11, e12, f10, f11, f12 };
            foreach (PictureBox o in oranges)
                if (o.BackColor == Color.Orange) orange = true; else { orange = false; break; }

            //Green
            PictureBox[] greens = { d1, d2, d3, e1, e2, e3, f1, f2, f3 };
            foreach (PictureBox g in greens)
                if (g.BackColor == Color.Green) green = true; else { green = false; break; }

            //Blue
            PictureBox[] blues = { d7, d8, d9, e7, e8, e9, f7, f8, f9 };
            foreach (PictureBox b in blues)
                if (b.BackColor == Color.Blue) blue = true; else { blue = false; break; }

            if (white && yellow && green && orange && blue && red) return true;
            else return false;
        }

        public List<string> log_data_get()
        {
            return log_data;
        }
    }
}
// for (int i = 0; i <= 2; i++) 
//Yellow_Array[0, i] = temp_Blue_Array[2 - i, 0];     
    
//stop the stackoverflow problem---change all the do-while lops to for lops (in move_sides)