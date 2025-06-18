using System;
using System.Web.Security; // Necesario para FormsAuthentication
using System.Web.UI;
using System.Web;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class Login : Page
    {
        // Instancia del cliente del servicio SOAP de autenticación
        // Asegúrate de que tu servicio SOAP esté configurado y disponible
        private readonly AuthWSClient authServiceClient;

        public Login()
        {
            // Inicializa el cliente del servicio SOAP
            authServiceClient = new AuthWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Limpiar cualquier mensaje de error anterior al cargar la página por primera vez
                lblMessage.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Asegúrate de que todos los validadores del formulario sean válidos
            if (Page.IsValid)
            {
                try
                {
                    string email = txtEmail.Text;
                    string password = txtPassword.Text;

                    // Llama al servicio SOAP para autenticar al usuario
                    // El método 'loginUsuario' debe existir en tu AuthWS y en el REST API
                    authLoginResponseData loginResponse = authServiceClient.loginUsuario(email, password);

                    if (loginResponse != null)
                    {
                        // Autenticación exitosa
                        // Puedes almacenar información del usuario en la sesión o en un ticket de FormsAuthentication
                        // para usar en otras páginas protegidas.

                        // Construye el userData para el ticket de FormsAuthentication
                        // Esto te permitirá acceder al ID, email y tipo de usuario en otras partes de tu aplicación
                        string userData = $"{loginResponse.id}|{loginResponse.email}|{loginResponse.tipoUsuario}";

                        // Crea un ticket de FormsAuthentication
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                            1, // Versión del ticket
                            loginResponse.email, // Nombre de usuario para el ticket (puede ser el email)
                            DateTime.Now, // Hora de emisión
                            DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), // Hora de expiración (usa el timeout de Web.config)
                            true, // Persistente (la cookie se guarda en el navegador si es true)
                            userData, // Datos de usuario personalizados
                            FormsAuthentication.FormsCookiePath // Ruta de la cookie
                        );

                        // Encripta el ticket y añade la cookie de autenticación a la respuesta HTTP
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                        // Si el ticket es persistente, establece la fecha de expiración de la cookie
                        if (ticket.IsPersistent)
                        {
                            authCookie.Expires = ticket.Expiration;
                        }
                        Response.Cookies.Add(authCookie);

                        Response.Redirect("Inicio.aspx");
                    }
                    else
                    {
                        // Las credenciales no son válidas
                        // Este caso solo ocurrirá si el servicio SOAP retorna null en lugar de lanzar una excepción
                        lblMessage.Text = "Credenciales inválidas. Por favor, intente de nuevo.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (System.ServiceModel.FaultException faultEx)
                {
                    // Captura errores específicos que tu servicio SOAP pueda enviar (ej. desde el backend REST)
                    // El mensaje de faultEx.Message debería ser el que viene de tu AuthResource
                    lblMessage.Text = faultEx.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    // Para depuración: System.Diagnostics.Debug.WriteLine($"Error de FaultException en Login: {faultEx.Detail?.InnerXml ?? faultEx.Message}");
                }
                catch (System.Exception ex)
                {
                    // Captura cualquier otro error inesperado durante el proceso de login
                    lblMessage.Text = $"Ha ocurrido un error inesperado: {ex.Message}";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    // Para depuración: System.Diagnostics.Debug.WriteLine($"Error general en Login: {ex.ToString()}");
                }
            }
        }
    }
}