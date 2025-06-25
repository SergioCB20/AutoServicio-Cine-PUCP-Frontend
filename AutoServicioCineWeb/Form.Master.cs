using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class Form : System.Web.UI.MasterPage
    {
        public HtmlGenericControl TituloSpan => tituloSpan;
        public HtmlImage ImgPoster => imgPoster;
        public HtmlGenericControl EntradasAdultoTexto => entradasAdultoTexto;
        public HtmlGenericControl EntradasInfantilTexto => entradasInfantilTexto;
        public HtmlGenericControl EntradasMayorTexto => entradasMayorTexto;
        public HtmlGenericControl TotalResumen => totalResumen;
        public HiddenField HfTotal => hfTotal;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)Context.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string userData = ticket.UserData;
                string[] userInfo = userData.Split('|');

                if (userInfo.Length >= 3)
                {
                    string userName = userInfo[1];
                    if (lbUserName != null)
                    {
                        lbUserName.Text = userName;
                    }
                }
            }
            else
            {
                if (lbUserName != null)
                {
                    lbUserName.Text = "Invitado";
                }
            }
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("Inicio.aspx");
        }

    }
}