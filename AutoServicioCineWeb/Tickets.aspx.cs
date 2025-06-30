using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;

public class ResumenCompra
{
    public string AdultoTicket { get; set; }
    public string MayorTicket { get; set; }
    public string InfantilTicket { get; set; }
    public string TotalTicket {  get; set; }
    public string ImagenUrl { get; set; }
    public string TituloDePelicula {  get; set; }
}

namespace AutoServicioCineWeb
{
    public partial class Tickets : System.Web.UI.Page
    {
        // Declara el cliente SOAP
        private readonly PeliculaWSClient peliculaServiceClient;

        public Tickets()
        {
            // Inicializa el cliente SOAP
            peliculaServiceClient = new PeliculaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["peliculaId"];
                if (int.TryParse(idStr, out int peliculaId))
                {
                    CargarDatosPelicula(peliculaId);
                }
                else //para que se aprecie la vista sin pasar por Película.aspx, mostrará la película con id=10 (Spiderman)
                {
                    CargarDatosPelicula(42);
                }

                CargarFechaHoraFuncion();
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
                tituloPelicula.InnerText = peliculaElegida.tituloEs;
            }

        }

        private void CargarFechaHoraFuncion()
        {
            funcion funcionSeleccionada = Session["FuncionSeleccionada"] as funcion;
            if (funcionSeleccionada != null)
            {
                var master = this.Master as Form;

                if (DateTime.TryParseExact(funcionSeleccionada.fechaHora, "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaHora))
                {
                    master.FechaSpan.InnerText = fechaHora.ToString("dddd, dd MMMM yyyy", new CultureInfo("es-PE"));
                    master.HoraSpan.InnerText = fechaHora.ToString("hh:mm tt", new CultureInfo("es-PE"));
                }
                else
                {
                    master.HoraSpan.InnerText = funcionSeleccionada.fechaHora;
                    master.FechaSpan.InnerText = "";
                }
            }
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            //Esta parte es para enviar los valores que fueron modificados a la siguiente vista (Butacas.aspx)
            HiddenField hfAdulto = (HiddenField)Master.FindControl("hfEntradasAdulto");
            HiddenField hfInfantil = (HiddenField)Master.FindControl("hfEntradasInfantil");
            HiddenField hfMayor = (HiddenField)Master.FindControl("hfEntradasMayor");
            HiddenField hfTotal = (HiddenField)Master.FindControl("hfTotal");
            var master = this.Master as Form;
            var resumen = new ResumenCompra
            {
                AdultoTicket = hfAdulto.Value,
                InfantilTicket = hfInfantil.Value,
                MayorTicket = hfMayor.Value,
                TotalTicket = hfTotal.Value,
                TituloDePelicula = master.TituloSpan.InnerText,
                ImagenUrl = master.ImgPoster.Src
            };

            Session["ResumenCompra"] = resumen;
            Response.Redirect("Butacas.aspx");
        }

    }
}