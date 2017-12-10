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
    public partial class popup : Form
    {
        string Content;

        public popup(string c)
        {
            InitializeComponent();
            set_info(c);
            Content = c;
        }

        private void set_info(string content)
        {
            switch (content)
            {
                case "move sides":
                    info.Font = new Font(info.Font.FontFamily, 19, FontStyle.Bold);
                    info.Text = @"The first thing we need to do is make a white cross.
there isn't a specific algorithm to do so,
you will need to use your intuition!

Try doing a whtie cross on the white layer!
If you managed to do it, click the 'I'm Done!' button.";
                    break;

                case "move edges":
                    info.Font = new Font(info.Font.FontFamily, 16, FontStyle.Bold);
                    info.Text = @"Now that the white cross is finished we need to complete the rest of the layer.
In order to do so we need to locate the remaining white corner pieces,
when you find one move it until the two different colors are matching the ones in the bottom layer.
now all you need to do is this algorithm:
down, left, I-down, I-left

If you managed to do it, click the 'I'm Done!' button.";
                    break;

                case "move second sides":
                    info.Font = new Font(info.Font.FontFamily, 16, FontStyle.Bold);
                    info.Text = @"All right you just finished the upper layer!!
Now we need to start working on the middle one...
All you need to do is is find a side piece move it until the cube above it is in the same color
now just do this algorithm:
down, left, I-down, I-left, I-down, I-front, down, front

If you managed to do it, click the 'I'm Done!' button.";
                    break;

                case "bottom":
                    info.Font = new Font(info.Font.FontFamily, 16, FontStyle.Bold);
                    info.Text = @"Now the bottom layer:
if you have a cross\fish pattern do this algorithm:
right, down, I-right, down, right, downX2, I-right

if you got a L shape or a line that runs through the center, do this algorithm: 
back, right, down, I-right, I-down, I-back

If you managed to do it, click the 'I'm Done!' button.";
                    break;

                case "last layer":
                    info.Font = new Font(info.Font.FontFamily, 16, FontStyle.Bold);
                    info.Text = @"Now the last layer:
for the corners do this algorithm:
right(3); back(); right(3); frontX, right, I-back, I-right, frontX2, rightX2, I-down

for the centers do this algorithm:
backX2, I-down, left, I-right, backX2, I-left, right, I-down, backX2

If you managed to do it, click the 'I'm Done!' button.";
                    break;

            }
        }

        private void B_finish_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}