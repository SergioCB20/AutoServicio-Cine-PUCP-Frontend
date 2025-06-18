using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class Pago : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var resumen = Session["ResumenCompra"] as ResumenCompra; //Carga los valores que vinieron de la vista Butacas
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
}