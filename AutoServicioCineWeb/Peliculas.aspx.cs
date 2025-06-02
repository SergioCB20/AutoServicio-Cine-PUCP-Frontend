using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .ToList()
using System.Web.UI;
using System.Web.UI.WebControls; // Necesario para el Repeater y Button

// Asegúrate de que este namespace sea correcto para tu referencia de servicio
using AutoServicioCineWeb.PeliculaWebService;

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
            if (!IsPostBack) // Solo cargar en la primera carga de la página
            {
                CargarPeliculasDisponibles(); // Llama al nuevo método para cargar las películas
            }
        }

        private void CargarPeliculasDisponibles()
        {
            try
            {
                // Llama al servicio web para obtener la lista de películas activas
                // Asumo que listarPeliculas(false) trae solo las activas
                List<pelicula> peliculas = peliculaServiceClient.listarPeliculas().ToList();

                // Asigna la lista de películas al Repeater
                rptPeliculas.DataSource = peliculas;
                rptPeliculas.DataBind();

                // Mostrar el mensaje de "No hay películas" si la lista está vacía
                // Esto es manejado por el FooterTemplate del Repeater, pero a veces necesitas un control extra
                if (peliculas == null || peliculas.Count == 0)
                {
                    litMensaje.Text = "No hay películas disponibles en este momento.";
                    litMensaje.Visible = true; // Asegúrate de que el Literal esté visible
                }
                else
                {
                    litMensaje.Visible = false; // Oculta el mensaje si hay películas
                }

            }
            catch (Exception ex)
            {
                litMensaje.Text = "Error al cargar las películas: " + ex.Message;
                litMensaje.Visible = true;
                // Loguea el error para depuración
                System.Diagnostics.Debug.WriteLine("Error al cargar películas en la vista principal: " + ex.ToString());
            }
        }

        // Evento para el botón "Comprar" dentro del Repeater (opcional, para futuras funcionalidades)
        protected void btnComprar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int peliculaId = Convert.ToInt32(btn.CommandArgument);

            // Aquí puedes redirigir al usuario a una página de selección de funciones/horarios
            // o añadir la película al carrito de compras, etc.
            Response.Redirect($"SeleccionarFunciones.aspx?peliculaId={peliculaId}");
        }

        // NOTA: Si tenías otros métodos como PageIndexChanging o eventos de GridView en esta página,
        // ya no serán necesarios si solo usas el Repeater para la visualización.
        // Asegúrate de que la página Peliculas.aspx no esté mezclando GridView y Repeater para la misma data.
        // Si tienes una página de "Gestión de Películas" con GridView y esta es solo la "Vista de Cliente",
        // entonces el GridView de Gestión de Películas es diferente.
    }
}