using System;
using System.Web.UI;
using System.Web.Security; // Necesario para FormsAuthentication
using System.Web; // Necesario para HttpContext

namespace AutoServicioCineWeb
{
    public partial class Inicio : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si el usuario está autenticado usando Forms Authentication
                if (Context.User.Identity.IsAuthenticated)
                {
                    // Si está autenticado:
                    // 1. Ocultar el div de botones de inicio/registro
                    divBotonesAcceso.Visible = false;
                    // 2. Mostrar el div del botón de cerrar sesión
                    divBotonCerrarSesion.Visible = true;

                    // Opcional: Mostrar mensaje de bienvenida usando los datos de la cookie
                    try
                    {
                        FormsIdentity id = (FormsIdentity)Context.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;
                        string userData = ticket.UserData;
                        string[] userInfo = userData.Split('|');

                        if (userInfo.Length >= 3) 
                        {
                            string userName = userInfo[1];
                            lblBienvenida.Text = $"¡Hola, {userName}!";
                        }
                        else
                        {
                            // Si el formato del userData no es el esperado, usa el nombre por defecto del ticket
                            lblBienvenida.Text = $"¡Hola, {User.Identity.Name}!";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejar cualquier error al leer la cookie (ej. cookie corrupta)
                        lblBienvenida.Text = $"¡Hola, {User.Identity.Name}!"; // Fallback al nombre de identidad
                        // Puedes loggear el error para depuración: System.Diagnostics.Debug.WriteLine($"Error al leer userData: {ex.Message}");
                    }
                }
                else
                {
                    // Si NO está autenticado:
                    // 1. Mostrar el div de botones de inicio/registro
                    divBotonesAcceso.Visible = true;
                    // 2. Ocultar el div del botón de cerrar sesión
                    divBotonCerrarSesion.Visible = false;
                }
            }
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            // Redirige a la misma página para que Page_Load se ejecute de nuevo y actualice los botones
            Response.Redirect(Request.RawUrl);
        }
    }
}