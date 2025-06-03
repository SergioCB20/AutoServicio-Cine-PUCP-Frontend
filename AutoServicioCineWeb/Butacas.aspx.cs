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
                var resumen = Session["ResumenCompra"] as ResumenCompra;
                if (resumen != null)
                {
                    entradasAdulto.InnerText = resumen.AdultoTicket;
                    entradasInfantil.InnerText = resumen.InfantilTicket;
                    entradasMayor.InnerText = resumen.MayorTicket;
                    totalText.InnerText = resumen.TotalTicket;
                }
            }
        }
    }
}