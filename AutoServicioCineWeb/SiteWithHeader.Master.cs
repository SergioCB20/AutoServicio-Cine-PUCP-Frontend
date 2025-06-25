using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class SiteWithHeader : System.Web.UI.MasterPage
    {
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
    }
}