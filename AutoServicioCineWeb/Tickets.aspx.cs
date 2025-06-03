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
            //Esta parte es para enviar los valores que fueron modificados a la siguiente vista (Butacas.aspx)
            HiddenField hfAdulto = (HiddenField)Master.FindControl("hfEntradasAdulto");
            HiddenField hfInfantil = (HiddenField)Master.FindControl("hfEntradasInfantil");
            HiddenField hfMayor = (HiddenField)Master.FindControl("hfEntradasMayor");
            HiddenField hfTotal = (HiddenField)Master.FindControl("hfTotal");

            var resumen = new ResumenCompra
            {
                AdultoTicket = hfAdulto.Value,
                InfantilTicket = hfInfantil.Value,
                MayorTicket = hfMayor.Value,
                TotalTicket = hfTotal.Value
            };

            Session["ResumenCompra"] = resumen;
            Response.Redirect("Butacas.aspx");
        }

    }
}