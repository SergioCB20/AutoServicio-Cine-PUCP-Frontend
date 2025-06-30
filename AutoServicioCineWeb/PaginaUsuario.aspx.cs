using Antlr.Runtime.Tree;
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
        private List<cupon> _cachedCupones;
        private readonly CuponWSClient cuponServiceClient;
        private List<venta> _cachedCompras;
        private readonly VentaWSClient historialServiceClient;
        private int idUsuario;

        public PaginaUsuario(){
             usuarioServiceClient = new UsuarioWSClient() ;
            cuponServiceClient = new CuponWSClient();
            historialServiceClient = new VentaWSClient() ;
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

        // Método para obtener diferentes estilos de colores para las tarjetas
        protected string GetCuponStyle(int index)
        {
            return string.Empty; // O podrías devolver algún estilo mínimo si lo necesitas
        }

        protected void rptCupones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cupon = e.Item.DataItem as cupon;
                if (cupon == null) return;

                var litDescuento = (Literal)e.Item.FindControl("litDescuento");

                if (cupon.descuentoTipo   == tipoDescuento.PORCENTAJE)
                {
                    litDescuento.Text = $"• {cupon.descuentoValor}%";
                }
                else if (cupon.descuentoTipo == tipoDescuento.MONTO_FIJO)
                {
                    litDescuento.Text = $"• S/. {cupon.descuentoValor.ToString("N0")}";
                }
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

                        if (userData.Length >= 4)
                        {
                            string userId = userData[0];
                            string userName = userData[1];
                            string userEmail = userData[2];
                            string userTipoUsuario = userData[3];
                            idUsuario = int.Parse(userId);

                            // Obtener el usuario desde el servicio web
                            usuario usuarioData = usuarioServiceClient.buscarUsuarioPorId(idUsuario);

                            // Mostrar los datos en tu página
                            lblNombre.Text = usuarioData.nombre;
                            lblEmail.Text = usuarioData.email;
                            lblTelefono.Text = usuarioData.telefono;

                            // También puedes usar el nombre del ticket
                            // lblNombreUsuario.Text = ticket.Name; // Esto es el email que pusiste
                        }
                    }
                }

                // Llama al servicio web para obtener los cupones del usuario
                _cachedCupones = cuponServiceClient.listarCupones().ToList();
                List<cupon> cuponFiltrados = FiltrarCupones(_cachedCupones);

                if (cuponFiltrados.Any())
                {
                    // Enlazar directamente la lista de cupones, no crear objetos anónimos
                    rptCupones.DataSource = cuponFiltrados;
                    rptCupones.DataBind();
                    pnlEmptyState.Visible = false;
                }
                else
                {
                    pnlEmptyState.Visible = true;
                }

                var resultado = historialServiceClient.listarVentasPorUsuario(idUsuario);

                if (resultado != null)
                {
                    _cachedCompras = resultado.ToList();

                    if (_cachedCompras.Any())
                    {
                        rptHistorial.DataSource = _cachedCompras;
                        rptHistorial.DataBind();
                        pnlNoCompras.Visible = false;
                    }
                    else
                    {
                        pnlNoCompras.Visible = true;
                    }
                }
                else
                {
                    // El servicio devolvió null
                    pnlNoCompras.Visible = true;
                }

            }
            catch (System.Exception ex)
            {
                // Manejar errores
                MostrarMensaje("Ocurrió un error al cargar los datos. Por favor intenta nuevamente.",true);
            }
        }

        protected void rptHistorial_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Obtener el objeto de datos
                var compra = (venta)e.Item.DataItem; // Cambia 'venta' por tu tipo correcto

                // Encontrar los controles Literal
                Literal litFecha = (Literal)e.Item.FindControl("litFecha");
                Literal litTotal = (Literal)e.Item.FindControl("litTotal");

                // Asignar la fecha (string)
                if (litFecha != null && !string.IsNullOrEmpty(compra.fechaHora))
                {
                    // Si quieres formatear la fecha string
                    DateTime fecha;
                    if (DateTime.TryParse(compra.fechaHora, out fecha))
                    {
                        litFecha.Text = fecha.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        litFecha.Text = compra.fechaHora; // Mostrar como string si no se puede parsear
                    }
                }

                // Asignar el total
                if (litTotal != null)
                {
                    litTotal.Text = compra.total.ToString("C");
                }
            }
        }

        private List<cupon> FiltrarCupones(List<cupon> cupones)
        {
            // Filtrar cupones que no han sido utilizados y que no han expirado
            return cupones.Where(c => c.activo).ToList();
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
                    //MostrarMensaje("El nombre es requerido", true);
                    txtNombreEdit.Text = lblNombre.Text;
                }

                if (string.IsNullOrEmpty(txtEmailEdit.Text))
                {
                    //MostrarMensaje("El email es requerido", true);
                    txtEmailEdit.Text = lblEmail.Text;
                }

                if(string.IsNullOrEmpty(txtTelefonoEdit.Text))
                {
                    //MostrarMensaje("El teléfono es requerido", true);
                    txtTelefonoEdit.Text = lblTelefono.Text;
                }

                // Crear objeto con los datos actualizados
                
                usuario usuarioActualizado = usuarioServiceClient.buscarUsuarioPorId(userId);

                usuarioActualizado.nombre = txtNombreEdit.Text;
                usuarioActualizado.telefono = txtTelefonoEdit.Text;
                usuarioActualizado.email = txtEmailEdit.Text;
                

                // Llamar al servicio web para actualizar
                usuarioServiceClient.actualizarUsuario(usuarioActualizado);
                pnlEdicion.Style["display"] = "none";
                MostrarMensaje("Datos actualizados correctamente", false);
                Response.Redirect(Request.Url.AbsoluteUri); // Recarga la misma página

            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al actualizar: " + ex.Message, true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Limpiar el contenido del TextBox correctamente
            txtEmailEdit.Text = "";
            txtNombreEdit.Text = "";
            txtTelefonoEdit.Text = "";
            // Cerrar el modal
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

}
