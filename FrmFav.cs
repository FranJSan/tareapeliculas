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
            Size = new Size(250, 300);
            AutoSizeMode = AutoSizeMode.GrowAndShrink; // No se aplica correctamente. He tenido que establecer valores mínimos del size
            MinimumSize = Size;          // Solo pasa en este formulario.
            MaximumSize = Size;          // y aún así sigue saliendo el icono del ratón para cambiar el tamaño del Form.
            Text = "Favoritas";
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Comic Sans MS", 8, FontStyle.Regular);
            BackColor = Color.AntiqueWhite;
            MaximizeBox = false;
            Icon = new Icon("../../fav.ico");
        }
    }
}
