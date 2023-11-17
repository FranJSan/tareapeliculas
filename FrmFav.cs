using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Peliculas
{
    public class FrmFav : Form
    {
        public FrmFav()
        {
            Text = "Favoritas";
            Size = new Size(250, 300);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Comic Sans MS", 8, FontStyle.Regular);
            BackColor = Color.AntiqueWhite;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
        }
    }
}
