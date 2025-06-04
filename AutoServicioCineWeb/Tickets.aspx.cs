using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.PeliculaWebService;

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
                    CargarDatosPelicula(10);
                }
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

                // Buscar la primera función (para cuando se tenga datos de funciones relacionadas a una película)
                /*if (peliculaElegida.funciones != null && peliculaElegida.funciones.Length > 0)
                {
                    var primeraFuncion = peliculaElegida.funciones[0];

                    fechaSpan.InnerText = primeraFuncion.fechaHora.ToString("dd/MM/yy");
                    horaSpan.InnerText = primeraFuncion.fechaHora.ToString("hh:mm tt");
                }*/
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