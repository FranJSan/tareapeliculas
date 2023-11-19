using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peliculas
{
    /// <summary>
    /// La clase Pelicula almacena los datos de cada película en distintos atributos. Contiene un método para limpiar y normalizar los datos que
    /// se ejecuta en el constructor.
    /// </summary>
    class Pelicula
    {
        private string titulo;
        public string Titulo {
            get { return titulo; }
            set { titulo = value; } 
        }
        private string[] actores;
        public string[] Actores { 
            get { return actores; } 
            set { actores = value; } 
        }
        private string director;
        public string Director { 
            get { return director; } 
            set { director = value;  } 
        }
        private string categoria;
        public string Categoria { 
            get { return categoria; } 
            set { categoria = value; } 
        }

        private bool favorita;
        public bool Favorita
        {
            get { return favorita; }
            set { favorita = value; }
        }


        /// <summary>
        /// El constructor recibe directamente la línea del archivo, encargandose de separarlo por atributos.
        /// </summary>
        /// <param name="peli"></param>
        public Pelicula(string peli)
        {
            string[] pelicula = peli.Split('#');

            Titulo = pelicula[0];
            Actores = pelicula[1].Split(',');
            Director = pelicula[2];
            Categoria = pelicula[3];

            NormalizarAtributos();
        }

        /// <summary>
        /// Método para limpiar y normalizar atributos.
        /// </summary>
        private void NormalizarAtributos()
        {
            string[] charsRemplazar = { ".", "..", "...", "-"};
            foreach (string c in charsRemplazar)
            {
                Titulo = Titulo.Replace(c, " ").Trim();
                for (int i = 0; i < Actores.Length; i++)
                {
                    Actores[i] = Actores[i].Replace(c, "").Trim();
                }
                Director = Director.Replace(c, "").Trim();
                Categoria = Categoria.Replace(c, "").Trim();
            }            
        }
    }
}
