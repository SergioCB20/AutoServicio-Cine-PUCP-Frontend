using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class PerfilUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosUsuario();
            }
        }

        private void CargarDatosUsuario()
        {
            try
            {
                // Obtener el ID del usuario de la sesión
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

                // Crear cliente del servicio web
                using (UsuarioWSClient cliente = new UsuarioWSClient())
                {
                    // Obtener datos del usuario
                    usuario usuario = cliente.buscarUsuarioPorId(usuarioId);

                    if (usuario != null)
                    {
                        // Mostrar información básica
                        nombreUsuario.InnerText = $"{usuario.nombre}";
                        emailUsuario.InnerText = usuario.email;

                        // Convertir fechaRegistro a cadena sin usar formato personalizado
                        fechaRegistro.InnerText = usuario.fechaRegistro.ToString();

                        // Llenar formulario de información personal
                        txtNombres.Text = usuario.nombre;
                        txtEmail.Text = usuario.email;
                        txtTelefono.Text = usuario.telefono;

                        // Cargar configuración de notificaciones
                        //chkEmailPromociones.Checked = usuario.recibirPromociones ?? false;
                        //chkEmailEstrenos.Checked = usuario.notificarEstrenos ?? false;
                        //chkSMSRecordatorios.Checked = usuario.recordatoriosSMS ?? false;
                        //chkPerfilPublico.Checked = usuario.perfilPublico ?? false;
                        //chkCompartirHistorial.Checked = usuario.compartirHistorial ?? false;

                        //// Cargar avatar si existe
                        //if (!string.IsNullOrEmpty(usuario.avatarUrl))
                        //{
                        //    imgAvatar.Src = usuario.avatarUrl;
                        //}
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Manejar error (puedes mostrar un mensaje al usuario)
                MostrarError("Error al cargar los datos del usuario: " + ex.Message);
            }
        }

        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

                using (UsuarioWSClient cliente = new UsuarioWSClient())
                {
                    // Obtener usuario actual para preservar datos no editables
                    usuario usuarioActual = cliente.buscarUsuarioPorId(usuarioId);

                    // Actualizar datos editables
                    usuarioActual.nombre = txtNombres.Text;
                    usuarioActual.telefono = txtTelefono.Text;

                    // Procesar imagen de avatar si se subió una nueva
                    //if (fileUploadAvatar.HasFile)
                    //{
                    //    string extension = Path.GetExtension(fileUploadAvatar.FileName).ToLower();
                    //    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    //    {
                    //        string nombreArchivo = $"avatar_{usuarioId}{extension}";
                    //        string rutaGuardar = Server.MapPath($"~/images/avatars/{nombreArchivo}");

                    //        fileUploadAvatar.SaveAs(rutaGuardar);
                    //        usuarioActual.avatarUrl = $"./images/avatars/{nombreArchivo}";
                    //    }
                    //}

                    // Actualizar usuario en el servicio web
                    cliente.actualizarUsuario(usuarioActual); // Cambiado: El método actualizarUsuario es de tipo void, no devuelve un bool.

                    MostrarExito("Perfil actualizado correctamente.");
                    // Actualizar la imagen del avatar si cambió
                    //if (!string.IsNullOrEmpty(usuarioActual.avatarUrl))
                    //{
                    //    imgAvatar.Src = usuarioActual.avatarUrl;
                    //}
                }
            }
            catch (System.Exception ex)
            {
                MostrarError("Error al actualizar el perfil: " + ex.Message);
            }
        }

        protected void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

                using (UsuarioWSClient cliente = new UsuarioWSClient())
                {
                    usuario usuarioActual = cliente.buscarUsuarioPorId(usuarioId);

                    // Actualizar preferencias
                    //usuarioActual.recibirPromociones = chkEmailPromociones.Checked;
                    //usuarioActual.notificarEstrenos = chkEmailEstrenos.Checked;
                    //usuarioActual.recordatoriosSMS = chkSMSRecordatorios.Checked;
                    //usuarioActual.perfilPublico = chkPerfilPublico.Checked;
                    //usuarioActual.compartirHistorial = chkCompartirHistorial.Checked;

                    cliente.actualizarUsuario(usuarioActual);

                    //if (resultado)
                    //{
                    //    MostrarExito("Configuración guardada correctamente.");
                    //}
                    //else
                    //{
                    //    MostrarError("No se pudo guardar la configuración. Intente nuevamente.");
                    //}
                }
            }
            catch (System.Exception ex)
            {
                MostrarError("Error al guardar la configuración: " + ex.Message);
            }
        }

        private void MostrarExito(string mensaje)
        {
            // Implementar lógica para mostrar mensaje de éxito (puede ser un modal, alerta, etc.)
            ScriptManager.RegisterStartupScript(this, GetType(), "exito", $"alert('{mensaje}');", true);
        }

        private void MostrarError(string mensaje)
        {
            // Implementar lógica para mostrar mensaje de error
            ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('{mensaje}');", true);
        }

        // Métodos para las acciones de la zona de peligro (implementar según necesidad)
        private void DesactivarCuenta()
        {
            // Implementar lógica para desactivar cuenta
        }

        private void EliminarCuenta()
        {
            try
            {
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);

                using (UsuarioWSClient cliente = new UsuarioWSClient())
                {
                    // El método eliminarUsuario es de tipo void, por lo que no devuelve un bool.
                    cliente.eliminarUsuario(usuarioId);

                    // Si no hay excepciones, asumimos que la operación fue exitosa.
                    Session.Abandon();
                    Response.Redirect("~/Default.aspx");
                }
            }
            catch (System.Exception ex)
            {
                MostrarError("Error al eliminar la cuenta: " + ex.Message);
            }
        }
    }
}