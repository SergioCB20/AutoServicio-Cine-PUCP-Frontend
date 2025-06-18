using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class Butacas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var resumen = Session["ResumenCompra"] as ResumenCompra; //Carga los valores que vinieron de la vista Tickets
                if (resumen != null)
                {
                    var master = this.Master as Form; //Para usar el texto que está definido en el Form.Master

                    if (master != null)
                    {
                        master.EntradasAdultoTexto.InnerText = resumen.AdultoTicket;
                        master.EntradasInfantilTexto.InnerText = resumen.InfantilTicket;
                        master.EntradasMayorTexto.InnerText = resumen.MayorTicket;
                        master.TotalResumen.InnerText = resumen.TotalTicket;
                        master.ImgPoster.Src = resumen.ImagenUrl;
                        master.TituloSpan.InnerText = resumen.TituloDePelicula;
                    }
                }
            }
        }
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            //Esta parte es para enviar los valores que fueron modificados a la siguiente vista (Comida.aspx)
            
            var master = this.Master as Form;
            var resumen = new ResumenCompra
            {
                AdultoTicket = master.EntradasAdultoTexto.InnerText,
                InfantilTicket = master.EntradasInfantilTexto.InnerText,
                MayorTicket = master.EntradasMayorTexto.InnerText,
                TotalTicket = master.TotalResumen.InnerText,
                TituloDePelicula = master.TituloSpan.InnerText,
                ImagenUrl = master.ImgPoster.Src
            };

            Session["ResumenCompra"] = resumen;
            Response.Redirect("Comida.aspx");
        }
    }
}