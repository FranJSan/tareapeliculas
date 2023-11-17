using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

/// <summary>
/// Tarea 04 Desarrollo de Interfaces.
/// Ha excepción del formulario principal, el resto de los controles han sido creados y configurados mediante código. Estos controles estan
/// concentrados en el método IniciarComponentes() (está colocado al final del código porque es un buen trozo que solo contiene configuraciones),
/// salvo los RadioButtons que se crean en otro método adiccional CrearRadioButtons() y se carga en el evento Load del Form principal. 
/// 
/// El programa lee el archivo Peliculas.txt y crear tantos RadioButtons como distintas categorias existan. Además de la funcionalidad
/// de filtrado de los RadioButtons, todos los items de las ListBox pueden filtra por su selección, pudiendo filtrar no solo por categoria, 
/// sino también por título, actor y director. 
/// 
/// También he añadido una ListBox de peliculas favoritas. Para añadir una película a la lista hacer click con en botón derecho sobre un título.
/// Para visualizar la lista de favoritas, pulsar el botón. Para borrar una película de la lista de favoritas, hacer click derecho sobre el título
/// desde la lista de favoritas.
/// 
/// Al final me ha quedado un buen trozo de código, he intentado organizarlo un poco poniendo por arriba los métodos importantes para resolver
/// la tarea, según vayas bajando irás encontrando las funcionalidades y eventos adiccionales, por si quieres ignorarlo.
/// 
/// 
/// todo: reorganizar código -> nuevas clases¿?, repasar comentarios¿?
/// </summary>
namespace Peliculas
{    
    public partial class FrmMain : Form
    {

        private List<Pelicula> lista = new List<Pelicula>();
        private List<string> listaCategorias = new List<string>();
        private ListBox LbTitulos, LbActores, LbDirectores, LbFavoritas;
        private Button BtnLimpiar, BtnFavoritas;
        private FrmFav FrmFav;
        private ContextMenu CMenu;
        private MenuItem AddFavoritas, borrarFavoritas;
        private Panel PanelMain;
        private GroupBox GroupRB;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigurarComponentes();
            // El directorio raíz en runtime es "debug", tener en cuenta para las rutas
            LeerFichero("..//..//Peliculas.txt");
            CrearCategorias();
            CrearRadioButtons(listaCategorias);
        }

        /// <summary>
        /// Método para leer el fichero.
        /// </summary>
        /// <param name="path">ruta del fichero a leer</param>
        /// <remarks>
        /// Para que no me saliesen caracteres extraños he tenido que usar en el constructor del StreamReader un 
        /// Encoding.Default.
        /// </remarks>
        private void LeerFichero(string path)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(path, Encoding.Default);

                string line = sr.ReadLine();
                while (line != null)
                {
                    Pelicula p = new Pelicula(line);
                    lista.Add(p);
                    line = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al leer el archivo con los datos. Compruebe la ruta del archivo.");
            } finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// Método para crear la lista de categorías.
        /// </summary>     
        private void CrearCategorias()
        {
            foreach (Pelicula p in lista)
            {
                if (listaCategorias.Contains(p.Categoria)) continue;
                listaCategorias.Add(p.Categoria);
            }
        }

        /// <summary>
        /// Método para crear los RadioButtons necesarios.
        /// </summary>
        /// <param name="categorias">Lista de vategorías a crear</param>
        /// <remarks>
        /// El método crea tantos RadioButtons como categorías tenga la lista que pasemos por parámetro y les agrega al GroupBox creado 
        /// previamente. Además, crea un RadiButton adiccional "Mostrar todos", el cual recibe un controlador de evento distinto a los demás.
        /// </remarks>
        private void CrearRadioButtons(List<string> categorias)
        {
            RadioButton rbTodos = new RadioButton();
            rbTodos.Text = "Mostrar todo";
            rbTodos.AutoSize = false;
            rbTodos.Width = 200;
            rbTodos.Height = 20;
            rbTodos.Top = 30;
            rbTodos.Left = 20;
            rbTodos.Visible = true;
            rbTodos.CheckedChanged += new EventHandler(RadioButtonTodosChanged);
            GroupRB.Controls.Add(rbTodos);

            listaCategorias.Sort();
            for (int i = 0; i < categorias.Count(); i++)
            {
                RadioButton rb = new RadioButton();
                rb.Text = categorias.ElementAt(i);
                rb.AutoSize = false;
                rb.Width = 200;
                rb.Height = 20;
                rb.Top = 30 + ((i + 1) * rb.Height) * 2;
                rb.Left = 20;
                rb.Visible = true;
                rb.CheckedChanged += new EventHandler(RadioButonChekedChanged);
                GroupRB.Controls.Add(rb);
            }
        }

        /// <summary>
        /// Controlador del evento ChekedChanged de los RadioButtons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Limpia y agrega los datos correspondientes a las ListBox segúna la categoría recibida.
        /// </remarks>
        private void RadioButonChekedChanged(object sender, EventArgs e)
        {
            // Solo se ejecuta el método si es un cheked
            if (!((RadioButton)sender).Checked) return;

            string categoria = ((RadioButton)sender).Text;

            LbTitulos.Items.Clear();
            LbActores.Items.Clear();
            LbDirectores.Items.Clear();

            foreach (Pelicula p in lista)
            {
                if (p.Categoria.Equals(categoria))
                {
                    // Títulos
                    if (p.Titulo.Length == 0) continue;
                    if (!LbTitulos.Items.Contains(p.Titulo)) LbTitulos.Items.Add(p.Titulo);

                    // Actores
                    foreach (string a in p.Actores)
                    {
                        if (a.Trim().Length == 0) continue;
                        if (!LbActores.Items.Contains(a)) LbActores.Items.Add(a.Trim());
                    }

                    // Directores
                    if (p.Director.Length == 0) continue;
                    if (!LbDirectores.Items.Contains(p.Director)) LbDirectores.Items.Add(p.Director);
                }
            }
        }

        /// <summary>
        /// Controlador del RadioButton "Mostrar todos".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Limpia y actualiza las listBox para mostrar todas las categorías.
        /// </remarks>
        private void RadioButtonTodosChanged(object sender, EventArgs e)
        {
            // Al limpiar los RB se activava el evento y se ejecutaba el método, por eso compruebo antes de continuar si es un 'check' 
            if (!((RadioButton)sender).Checked) return;

            LbTitulos.Items.Clear();
            LbActores.Items.Clear();
            LbDirectores.Items.Clear();

            foreach (Pelicula p in lista)
            {
                // Títulos
                if (p.Titulo.Length == 0) continue;
                if (!LbTitulos.Items.Contains(p.Titulo)) LbTitulos.Items.Add(p.Titulo);

                // Actores                
                foreach (string a in p.Actores)
                {
                    if (a.Trim().Length == 0) continue;
                    if (!LbActores.Items.Contains(a)) LbActores.Items.Add(a.Trim());
                }

                // Directores                
                if (p.Director.Length == 0) continue;
                if (!LbDirectores.Items.Contains(p.Director)) LbDirectores.Items.Add(p.Director);
            }
        }

       

        /// <summary>
        /// Controlador de la acción 'doble click' sobre cualquier item de las ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Según la fuente del evento se llama al método apropiado para que actualice las ListBox, filtrando por 
        /// el Item seleccionado.
        /// </remarks>
        private void DoubleClick_ListBox(Object sender, EventArgs e)
        {
            ListBox listBoxTemp = (ListBox)sender;
            switch (listBoxTemp.Tag.ToString())
            {
                case "titulos":
                    FiltrarPorTitulo((string)listBoxTemp.SelectedItem);
                    break;
                case "actores":
                    FiltrarPorActor((string)listBoxTemp.SelectedItem);
                    break;
                case "directores":
                    FiltrarPorDirector((string)listBoxTemp.SelectedItem);
                    break;
            }
        }

        /// <summary>
        /// Método que se ejecuta al hacer doble click sobre un item de la ListBox "titulos".
        /// </summary>
        /// <param name="titulo"></param>
        /// <remarks>
        /// Filtra el resto de ListBox según el título recibido por parámetro.  Primero limpia todos los items de las ListBox 
        /// para luego llenarlos con las coincidencias.
        /// </remarks>
        private void FiltrarPorTitulo(string titulo)
        {
            LbActores.Items.Clear();
            LbDirectores.Items.Clear();

            foreach (Pelicula p in lista)
            {
                if (p.Titulo.Equals(titulo))
                {
                    // Actores                                   
                    foreach (string act in p.Actores)
                    {
                        if (act.Length == 0) continue;
                        LbActores.Items.Add(act.Trim());
                    }

                    // Directores                   
                    if (p.Director.Length == 0) continue;
                    LbDirectores.Items.Add(p.Director);
                    break;
                }
            }
        }

        /// <summary>
        /// Método que se ejecuta al hacer doble click sobre un item de la ListBox "actores".
        /// </summary>
        /// <param name="actor"></param>
        /// <remarks>
        /// Este método filtra las ListBox según el actor que reciba como argumento.
        /// </remarks>
        private void FiltrarPorActor(string actor)
        {
            LbTitulos.Items.Clear();
            LbDirectores.Items.Clear();
            foreach (Pelicula p in lista)
            {
                // Actores
                string[] actores = p.Actores;

                foreach (string st in actores)
                {

                    if (st.Equals(actor))
                    {
                        // Titulos
                        if (p.Titulo.Length == 0) continue;
                        LbTitulos.Items.Add(p.Titulo);

                        // Directores
                        if (p.Director.Length == 0) continue;
                        LbDirectores.Items.Add(p.Director);
                    }
                }
            }
        }

        /// <summary>
        /// Método que se ejecuta al hacer doble click sobre un item de la ListBox "director".
        /// </summary>
        /// <param name="director"></param>
        /// <remarks>
        /// Al igual que los anteriores, este método filtra las demás ListBox según el director recibido como argumento.
        /// </remarks>
        private void FiltrarPorDirector(string director)
        {
            LbTitulos.Items.Clear();
            LbActores.Items.Clear();
            foreach (Pelicula p in lista)
            {
                if (p.Director.Equals(director))
                {
                    // Títulos
                    if (p.Titulo.Length == 0) continue;
                    LbTitulos.Items.Add(p.Titulo);

                    // Actores                    
                    foreach (string act in p.Actores)
                    {
                        if (act.Trim().Length == 0) continue;
                        LbActores.Items.Add(act.Trim());
                    }
                }
            }
        }

        /// <summary>
        /// Método del evento click del botón limpiar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Limpia LisBox y desmarca los RadioButtons.
        /// </remarks>
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LbTitulos.SelectedItem = null;
            LbTitulos.Items.Clear();
            LbActores.SelectedItem = null;
            LbActores.Items.Clear();
            LbDirectores.SelectedItem = null;
            LbDirectores.Items.Clear();

            foreach (RadioButton rb in GroupRB.Controls)
            {
                rb.Checked = false;
            }

        }

        /// <summary>
        /// Método que se ejecuta al hacer Click con el botón derecho del ratón sobre la ListBox LbTitulos y LbFavoritas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Muestra un menú contextual diferente según la ListBox donde se haya producido el evento, mostrando la opción de añadir en la
        /// LbTitulos y de borrar en la LbFavotitas
        /// </remarks>
        private void ItemMouseUp_Click(object sender, MouseEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            
            if (e.Button == MouseButtons.Right && listBox == LbTitulos)
            {
                if (LbTitulos.SelectedIndex == -1) return;
                CMenu.MenuItems.Remove(borrarFavoritas);
                LbTitulos.ClearSelected();
                LbTitulos.SelectedIndex = LbTitulos.IndexFromPoint(e.Location);                
                CMenu.MenuItems.Add(AddFavoritas);
                CMenu.Show(LbTitulos, e.Location);                               
            } 
            else if (e.Button == MouseButtons.Right && listBox == LbFavoritas)
            {
                CMenu.MenuItems.Remove(AddFavoritas);
                LbFavoritas.ClearSelected();
                LbFavoritas.SelectedIndex = LbFavoritas.IndexFromPoint(e.Location);
                CMenu.MenuItems.Add(borrarFavoritas);
                CMenu.Show(LbFavoritas, e.Location);
            }
        }

        /// <summary>
        /// Método para añadir una película a la lista de favoritas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Añade un película a la lista de favoritos si no existe previamente. Muestra un mensaje al usuario informando del proceso.
        /// </remarks>
        private void MenuAddFavoritas_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LbFavoritas.Items.Contains(LbTitulos.SelectedItem))
                {
                    LbFavoritas.Items.Add(LbTitulos.SelectedItem);
                    MessageBox.Show("Se ha añadido correctamente");
                }
                else MessageBox.Show("Ya existe en la lista.");
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("No se ha seleccionado ningún elemento.");
            }
        }

        /// <summary>
        /// Método para borrar una película de la lista de favoritas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuBorrar_Click(object sender, EventArgs e)
        {
            LbFavoritas.Items.Remove(LbFavoritas.SelectedItem);
        }

        /// <summary>
        /// Mñetodo del evento Click del botón 'Favoritas'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Abre un nuevo formulario con la lista de películas favoritas.
        /// </remarks>
        private void BtnFavoritas_Click(object sender, EventArgs e)
        {
            FrmFav.ShowDialog();
        }
 
        /// <summary>
        /// Método adicional para inicializar y configurar los controles creados.
        /// </summary>
        private void ConfigurarComponentes()
        {
            this.Text = "Catalogo de películas";
            this.Font = new Font("Comic Sans MS", 8, FontStyle.Regular);

            // Todo está sobre un Panel
            PanelMain = new Panel();
            PanelMain.Width = this.Width;
            PanelMain.Height = this.Height;
            PanelMain.Visible = true;
            PanelMain.BackColor = Color.AntiqueWhite;
            this.Controls.Add(PanelMain);

            // A este Group se le añadiran los RadioButton. Le inicializo aquí para poder usarlo de referencia en la posición de las Listbox
            GroupRB = new GroupBox();
            GroupRB.Width = this.Width / 2;
            GroupRB.Height = this.Height - 120;
            GroupRB.Visible = true;
            PanelMain.Controls.Add(GroupRB);

            // =========== ListBox y sus Labels ===========

            // LabelTitulos
            Label LabelTitulos = new Label();
            LabelTitulos.Text = "Títulos";
            LabelTitulos.Top = 10;
            LabelTitulos.Left = 10 + GroupRB.Width;
            // LbTitulos
            LbTitulos = new ListBox();
            LbTitulos.Tag = "titulos";
            LbTitulos.Visible = true;
            LbTitulos.Sorted = true;
            LbTitulos.Top = 30;
            LbTitulos.Left = 10 + GroupRB.Width;
            LbTitulos.Width = 310;
            LbTitulos.DoubleClick += new EventHandler(DoubleClick_ListBox);
            LbTitulos.MouseUp += new MouseEventHandler(ItemMouseUp_Click);

            PanelMain.Controls.Add(LbTitulos);
            PanelMain.Controls.Add(LabelTitulos);

            // LabelActores
            Label LabelActores = new Label();
            LabelActores.Text = "Actores";
            LabelActores.Top = LbTitulos.Location.Y + LbTitulos.Height + 30;
            LabelActores.Left = 10 + GroupRB.Width;
            // LbActores
            LbActores = new ListBox();
            LbActores.Tag = "actores";
            LbActores.Sorted = true;
            LbActores.Top = LbTitulos.Location.Y + LbTitulos.Height + 50;
            LbActores.Left = 10 + GroupRB.Width;
            LbActores.Width = 150;
            LbActores.DoubleClick += new EventHandler(DoubleClick_ListBox);

            PanelMain.Controls.Add(LbActores);
            PanelMain.Controls.Add(LabelActores);

            // LabelDirectores
            Label LabelDirectores = new Label();
            LabelDirectores.Text = "Directores";
            LabelDirectores.Top = LbTitulos.Location.Y + LbTitulos.Height + 30;
            LabelDirectores.Left = LbActores.Location.X + LbActores.Width + 10;
            // LbDirectores
            LbDirectores = new ListBox();
            LbDirectores.Tag = "directores";
            LbDirectores.Sorted = true;
            LbDirectores.Top = LbTitulos.Location.Y + LbTitulos.Height + 50;
            LbDirectores.Left = LbActores.Location.X + LbActores.Width + 10;
            LbDirectores.Width = 150;
            LbDirectores.DoubleClick += new EventHandler(DoubleClick_ListBox);

            PanelMain.Controls.Add(LbDirectores);
            PanelMain.Controls.Add(LabelDirectores);

            // LabelFavorital
            Label LabelFavoritas = new Label();
            LabelFavoritas.Text = "Favoritas";
            LabelFavoritas.Top = 10;
            LabelFavoritas.Left = 17;
            // LbFavoritas
            LbFavoritas = new ListBox();
            LbFavoritas.Tag = "favoritas";
            LbFavoritas.Sorted = true;
            LbFavoritas.Top = 30;
            LbFavoritas.Left = 17;
            LbFavoritas.Width = 200;
            LbFavoritas.Height = 200;
            LbFavoritas.MouseUp += new MouseEventHandler(ItemMouseUp_Click);

            // ============= Botones ============= 
            BtnLimpiar = new Button();
            BtnLimpiar.Text = "Limpiar";
            BtnLimpiar.Size = new Size(70, 25);
            BtnLimpiar.Top = LbDirectores.Location.Y + LbDirectores.Height + 30;
            BtnLimpiar.Left = LbActores.Location.X + LbActores.Width - BtnLimpiar.Width;
            BtnLimpiar.Visible = true;
            BtnLimpiar.Click += new EventHandler(BtnLimpiar_Click);
            PanelMain.Controls.Add(BtnLimpiar);

            BtnFavoritas = new Button();
            BtnFavoritas.Text = "Favoritas";
            BtnFavoritas.Size = new Size(70, 25);
            BtnFavoritas.Top = BtnLimpiar.Location.Y;
            BtnFavoritas.Left = LbDirectores.Location.X;
            BtnFavoritas.Visible = true;
            BtnFavoritas.Click += new EventHandler(BtnFavoritas_Click);
            PanelMain.Controls.Add(BtnFavoritas);

            // FrmFav
            FrmFav = new FrmFav();
            FrmFav.Controls.Add(LbFavoritas);
            FrmFav.Controls.Add(LabelFavoritas);

            // ContextMenu y MenuItem
            CMenu = new ContextMenu();
            AddFavoritas = new MenuItem("Añadir a favoritas");
            AddFavoritas.Click += new EventHandler(MenuAddFavoritas_Click);
            borrarFavoritas = new MenuItem("Borrar");
            borrarFavoritas.Click += new EventHandler(MenuBorrar_Click);

            // LabelInfo
            Label lblInfo = new Label();
            lblInfo.Width = GroupRB.Width;
            lblInfo.Height = 100;
            lblInfo.Text = "Doble click para filtrar por título, actor o director.\n\nClick derecho sobre un título para añadir a favoritas.";
            lblInfo.Top = GroupRB.Height + 10;
            lblInfo.Left = 10;
            PanelMain.Controls.Add(lblInfo);
        }
    }
}
