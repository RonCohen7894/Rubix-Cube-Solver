using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubix_Cube_Solver
{
    class cubit
    {
        private Color _color1;
        private Color _color2;
        private Color _color3;

        public cubit(Color color1, Color color2)
        {
            _color1 = color1;
            _color2 = color2;
        }

        public cubit(Color color1, Color color2, Color color3)
        {
            _color1 = color1;
            _color2 = color2;
            _color3 = color3;
        }

        public Color[] getColor()
        {
            Color[] temp = { _color1, _color2, _color3 };
            return temp;
        }
    }
}
