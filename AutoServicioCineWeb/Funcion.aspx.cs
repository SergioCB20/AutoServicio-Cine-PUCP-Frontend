using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class Funcion : System.Web.UI.Page
    {
        private readonly FuncionWSClient funcionServiceClient;
        private readonly PeliculaWSClient peliculaServiceClient;
        private List<funcion> _cachedFunciones;

        public Funcion()
        {
            funcionServiceClient = new FuncionWSClient();
            peliculaServiceClient = new PeliculaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarFunciones();
                string idStr = Request.QueryString["peliculaId"];
                if (int.TryParse(idStr, out int peliculaId))
                {
                    CargarDatosPelicula(peliculaId);
                }
            }
        }
        private void cargarFunciones()
        {
            try
            {
                string idStr = Request.QueryString["peliculaId"];
                if (int.TryParse(idStr, out int peliculaId))
                {
                    _cachedFunciones = funcionServiceClient.listarFuncionesPorPelicula(peliculaId).ToList();
                }
                List<funcion> listafunciones= _cachedFunciones;

                rptFunciones.DataSource = listafunciones;
                rptFunciones.DataBind();
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = "Error al cargar funciones: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar funciones: " + ex.ToString());
            }

        }

        private void CargarDatosPelicula(int id)
        {
            pelicula peliculaElegida = peliculaServiceClient.buscarPeliculaPorId(id);

            var master = this.Master as Form;
            if (peliculaElegida != null)
            {
                // Título
                master.TituloSpan.InnerText = peliculaElegida.tituloEs;

                // Imagen
                master.ImgPoster.Src = peliculaElegida.imagenUrl;
            }

        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            string idStr = Request.QueryString["peliculaId"];
            if (int.TryParse(idStr, out int peliculaId))
            {
                Response.Redirect($"Tickets.aspx?peliculaId={peliculaId}");
            }
        }

        protected void rptFunciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SeleccionarFuncion")
            {
                string[] datos = e.CommandArgument.ToString().Split('|');

                if (datos.Length >= 3)
                {
                    int idFuncion = int.Parse(datos[0]);
                    int salaId = int.Parse(datos[1]);
                    string fechaHora = datos[2];

                    var funcionDetalle = new funcion
                    {
                        funcionId = idFuncion,
                        funcionIdSpecified = true,
                        salaId = salaId,
                        salaIdSpecified = true,
                        fechaHora = fechaHora,
                    };

                    // Se guarda en Session para usar en la siguiente vista
                    Session["FuncionSeleccionada"] = funcionDetalle;

                    string idStr = Request.QueryString["peliculaId"];
                    if (int.TryParse(idStr, out int peliculaId))
                    {
                        Response.Redirect($"Tickets.aspx?peliculaId={peliculaId}");
                    }
                }
            }
        }
    }
}