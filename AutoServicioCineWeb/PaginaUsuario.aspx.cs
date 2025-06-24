using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class PaginaUsuario : System.Web.UI.Page
    {
        private readonly UsuarioWSClient usuarioServiceClient;

        public PaginaUsuario(){
             usuarioServiceClient = new UsuarioWSClient() ;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
            // Verificar si el usuario está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosUsuario();
            }
        }

        private void CargarDatosUsuario()
        {
            try
            {
                // Obtener el ticket de autenticación de la cookie
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                {
                    // Desencriptar el ticket
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (ticket != null && !ticket.Expired)
                    {
                        // Extraer los datos del userData (recuerda que guardaste: "id|email|tipoUsuario")
                        string[] userData = ticket.UserData.Split('|');

                        if (userData.Length >= 3)
                        {
                            string userId = userData[0];
                            string userEmail = userData[1];
                            string userTipoUsuario = userData[2];
                            int userIdInt = int.Parse(userId);

                            // Obtener el usuario desde el servicio web
                            usuario usuarioData = usuarioServiceClient.buscarUsuarioPorId(userIdInt);

                            // Mostrar los datos en tu página
                            lblNombre.Text = usuarioData.nombre;
                            lblEmail.Text = userEmail;
                            lblTelefono.Text = usuarioData.telefono;

                            // También puedes usar el nombre del ticket
                            // lblNombreUsuario.Text = ticket.Name; // Esto es el email que pusiste
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Manejar errores
                // lbl.Text = "Error al cargar datos del usuario: " + ex.Message;
                // lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void CargarHistorialCompras(int usuarioId)
        {
            // Obtener historial de compras del usuario
            var historial = ObtenerHistorialCompras(usuarioId);

            gvHistorial.DataSource = historial;
            gvHistorial.DataBind();
        }

        private List<CompraHistorial> ObtenerHistorialCompras(int usuarioId)
        {
            // Aquí implementarías la lógica para obtener el historial de compras
            // desde tu servicio web o base de datos
            // Esto es un ejemplo con datos dummy

            return new List<CompraHistorial>
            {
                new CompraHistorial {
                    Id = 1,
                    Pelicula = "Avengers: Endgame",
                    Fecha = DateTime.Now.AddDays(-10),
                    Hora = "15:30",
                    Cantidad = 2,
                    Total = 35.00m
                },
                new CompraHistorial {
                    Id = 2,
                    Pelicula = "Spider-Man: No Way Home",
                    Fecha = DateTime.Now.AddDays(-30),
                    Hora = "18:45",
                    Cantidad = 3,
                    Total = 52.50m
                }
            };
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // Mostrar el modal
            pnlEdicion.Style["display"] = "flex";
            // 2. Mostrar el modal mediante JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal",
                "mostrarModal();", true);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener el ID del usuario desde la cookie
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                {
                    MostrarMensaje("No se pudo autenticar al usuario", true);
                    return;
                }

                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || ticket.Expired)
                {
                    MostrarMensaje("La sesión ha expirado", true);
                    return;
                }

                string[] userData = ticket.UserData.Split('|');
                if (userData.Length < 3)
                {
                    MostrarMensaje("Datos de usuario inválidos", true);
                    return;
                }

                int userId = int.Parse(userData[0]);

                // Validar campos obligatorios
                if (string.IsNullOrEmpty(txtNombreEdit.Text))
                {
                    MostrarMensaje("El nombre es requerido", true);
                    return;
                }

                if (string.IsNullOrEmpty(txtEmailEdit.Text))
                {
                    MostrarMensaje("El email es requerido", true);
                    return;
                }

                // Crear objeto con los datos actualizados
                usuario usuarioActualizado = new usuario
                {
                    id = userId,
                    nombre = txtNombreEdit.Text,
                    telefono = txtTelefonoEdit.Text,
                    // email no se actualiza aquí porque está en el ticket
                };

                // Actualizar contraseña si se proporcionó
                string nuevaPassword = txtPasswordEdit.Text;
                bool actualizarPassword = !string.IsNullOrEmpty(nuevaPassword);

                // Llamar al servicio web para actualizar
                usuarioServiceClient.actualizarUsuario(usuarioActualizado);
                pnlEdicion.Style["display"] = "none";
                MostrarMensaje("Datos actualizados correctamente", false);
                
            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al actualizar: " + ex.Message, true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Simplemente cerrar el modal sin guardar cambios
            pnlEdicion.Style["display"] = "none";
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            // Puedes usar un control Label o un script para mostrar el mensaje
            string script = $"alert('{mensaje}');";
            if (esError)
            {
                script = $"alert('ERROR: {mensaje}');";
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", script, true);
        }
    }

    // Clase auxiliar para el historial de compras
    public class CompraHistorial
    {
        public int Id { get; set; }
        public string Pelicula { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
