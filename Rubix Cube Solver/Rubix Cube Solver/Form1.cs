using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rubix_Cube_Solver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Color draw_color = Color.Black;

        private void pick_color(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Button[] colors = {White, Yellow, Red, Orange, Green, Blue};
            foreach (Button c in colors)
                c.FlatAppearance.BorderSize = 1;

            btn.FlatAppearance.BorderSize = 3;
            draw_color = btn.BackColor;
        }

        private void Bchange_color(object sender, EventArgs e)
        {
            PictureBox btn = (PictureBox)sender;
            if(draw_color != Color.Black)
                btn.BackColor = draw_color;
        }

        private void Breset_Click(object sender, EventArgs e)
        {
            //White
            PictureBox[] whites = { w1, w2, w3, w4, w5, w6, w7, w8, w9 };
            foreach (PictureBox w in whites)
                w.BackColor = Color.White;

            //Yellows
            PictureBox[] yellows = { y1, y2, y3, y4, y5, y6, y7, y8, y9 };
            foreach (PictureBox y in yellows)
                y.BackColor = Color.Yellow;

            //Red
            PictureBox[] reds = { r1, r2, r3, r4, r5, r6, r7, r8, r9 };
            foreach (PictureBox r in reds)
                r.BackColor = Color.Red;

            //Orange
            PictureBox[] oranges = { o1, o2, o3, o4, o5, o6, o7, o8, o9 };
            foreach (PictureBox o in oranges)
                o.BackColor = Color.Orange;

            //Green
            PictureBox[] green = { g1, g2, g3, g4, g5, g6, g7, g8, g9 };
            foreach (PictureBox g in green)
                g.BackColor = Color.Green;

            //Blue
            PictureBox[] blues = { b1, b2, b3, b4, b5, b6, b7, b8, b9 };
            foreach (PictureBox b in blues)
                b.BackColor = Color.Blue;
        }

        private void Solve_Click(object sender, EventArgs e)
        {
            //setting the side cubits

        }
    }
}
/*
 OLD RESET BUTTON CODE  
            
            //White
            PictureBox[] whites = { w1, w2, w3, w4, w5, w6, w7, w8, w9 };
            foreach (PictureBox w in whites)
                w.BackColor = Color.White;

            //Yellow
            PictureBox[] yellows = { y1, y2, y3, y4, y5, y6, y7, y8, y9 };
            foreach (PictureBox y in yellows)
                y.BackColor = Color.Yellow;

            //Red
            PictureBox[] reds = { r1, r2, r3, r4, r5, r6, r7, r8, r9 };
            foreach (PictureBox r in reds)
                r.BackColor = Color.Red;

            //Orange
            PictureBox[] oranges = { o1, o2, o3, o4, o5, o6, o7, o8, o9 };
            foreach (PictureBox o in oranges)
                o.BackColor = Color.Orange;

            //Green
            PictureBox[] green = { g1, g2, g3, g4, g5, g6, g7, g8, g9 };
            foreach (PictureBox g in green)
                g.BackColor = Color.Green;

            //Blue
            PictureBox[] blues = { b1, b2, b3, b4, b5, b6, b7, b8, b9 };
            foreach (PictureBox b in blues)
                b.BackColor = Color.Blue;
     */
