using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

// Asegúrate de que este namespace sea correcto para tu referencia de servicio
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class Peliculas : System.Web.UI.Page
    {
        // Declara el cliente SOAP
        private readonly PeliculaWSClient peliculaServiceClient;

        public Peliculas()
        {
            // Inicializa el cliente SOAP
            peliculaServiceClient = new PeliculaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPeliculasDisponibles(string.Empty);
            }
        }


        // Modifica este método para aceptar un parámetro de búsqueda
        private void CargarPeliculasDisponibles(string searchTerm) //
        {
            try
            {
                List<pelicula> peliculas = peliculaServiceClient.listarPeliculas().ToList(); //

                // Aplica el filtro de búsqueda si searchTerm no está vacío
                if (!string.IsNullOrWhiteSpace(searchTerm)) //
                {
                    // Filtra por tituloEs o tituloEn, según lo que quieras buscar
                    peliculas = peliculas.Where(p =>
                        p.tituloEs != null && p.tituloEs.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 || //
                        p.tituloEn != null && p.tituloEn.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList(); //
                }

                rptPeliculas.DataSource = peliculas; //
                rptPeliculas.DataBind(); //

                // Mostrar el mensaje de "No hay películas" si la lista está vacía
                if (peliculas == null || peliculas.Count == 0) //
                {
                    litMensaje.Text = "No se encontraron películas que coincidan con su búsqueda."; //
                    litMensaje.Visible = true; //
                }
                else
                {
                    litMensaje.Visible = false; //
                }

            }
            catch (System.Exception ex)
            {
                litMensaje.Text = "Error al cargar las películas: " + ex.Message; //
                litMensaje.Visible = true; //
                System.Diagnostics.Debug.WriteLine("Error al cargar películas en la vista principal: " + ex.ToString()); //
            }
        }

        // Nuevo evento Click para el botón de búsqueda
        protected void btnBuscar_Click(object sender, EventArgs e) //
        {
            string searchTerm = txtBuscarPelicula.Text.Trim();
            CargarPeliculasDisponibles(searchTerm); 
        }

        protected void btnComprar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int peliculaId = Convert.ToInt32(btn.CommandArgument);

            Response.Redirect($"Tickets.aspx?peliculaId={peliculaId}");
        }

    }
}