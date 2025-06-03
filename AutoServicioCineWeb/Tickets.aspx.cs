using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class ResumenCompra
{
    public string AdultoTicket { get; set; }
    public string MayorTicket { get; set; }
    public string InfantilTicket { get; set; }
    public string TotalTicket {  get; set; }
}

namespace AutoServicioCineWeb
{
    public partial class Tickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            var resumen = new ResumenCompra
            {
                AdultoTicket = hfEntradasAdulto.Value,
                InfantilTicket = hfEntradasInfantil.Value,
                MayorTicket = hfEntradasMayor.Value,
                TotalTicket = hfTotal.Value,
            };

            Session["ResumenCompra"] = resumen;
            Response.Redirect("Butacas.aspx");
        }

    }
}