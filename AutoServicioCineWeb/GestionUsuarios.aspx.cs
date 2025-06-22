using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS; // Asegúrate de que tu servicio web tenga métodos para Usuario
using System.IO; // Para CSV, aunque no se implementará CSV en usuarios por ahora, se mantiene la referencia
using System.Text.RegularExpressions; // Para validación de email/password si es necesario en el backend
using System.Security.Cryptography; // Para hashing de contraseñas
using System.Text; // Para Encoding

namespace AutoServicioCineWeb
{
    public partial class GestionUsuarios : System.Web.UI.Page
    {
        // Asume que tienes un Cliente para el servicio web de Usuarios
        private readonly UsuarioWSClient usuarioServiceClient;
        private List<usuario> _cachedUsuarios; // Caché para mejorar el rendimiento de filtros y estadísticas

        public GestionUsuarios()
        {
            usuarioServiceClient = new UsuarioWSClient(); // Instancia de tu cliente de servicio web de Usuarios
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarUsuarios();
                // Opcional: Si tienes DropDownLists en el modal (como IdiomaPreferido), cargarlos aquí si no son estáticos
                // CargarIdiomas();
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                _cachedUsuarios = usuarioServiceClient.listarUsuarios().ToList();
                List<usuario> usuariosFiltrados = FiltrarUsuarios(_cachedUsuarios);

                gvUsuarios.DataSource = usuariosFiltrados;
                gvUsuarios.DataBind();
            }
            catch (System.Exception ex)
            {
                // Muestra un mensaje de error si falla la carga de usuarios
                // Podrías usar un Literal en la página principal para esto si no quieres usar el modal
                litMensajeModal.Text = "Error al cargar usuarios: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar usuarios: " + ex.ToString());
            }
        }

        private List<usuario> FiltrarUsuarios(List<usuario> usuarios)
        {
            // Aplicar filtro de búsqueda por nombre, email o teléfono
            if (!string.IsNullOrWhiteSpace(txtSearchUsuarios.Text))
            {
                string searchTerm = txtSearchUsuarios.Text.Trim().ToLower();
                usuarios = usuarios.Where(u =>
                    u.nombre.ToLower().Contains(searchTerm) ||
                    u.email.ToLower().Contains(searchTerm) ||
                    (u.telefono != null && u.telefono.ToLower().Contains(searchTerm))
                ).ToList();
            }

            // Aplicar filtro por estado (Activo/Inactivo)
            if (!string.IsNullOrEmpty(ddlEstadoFilter.SelectedValue))
            {
                bool estadoFilter = ddlEstadoFilter.SelectedValue == "Activo";
                usuarios = usuarios.Where(u => u.estaActivo == estadoFilter).ToList();
            }

            // Aplicar filtro por rol (Administrador/Usuario Regular)
            if (!string.IsNullOrEmpty(ddlAdminFilter.SelectedValue))
            {
                bool adminFilter = ddlAdminFilter.SelectedValue == "Administrador";
                usuarios = usuarios.Where(u => u.admin == adminFilter).ToList();
            }

            return usuarios;
        }

        protected void gvUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsuarios.PageIndex = e.NewPageIndex;
            CargarUsuarios();
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Asegúrate de que el CommandArgument es el ID del usuario
            int usuarioId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditUsuario")
            {
                hdnUsuarioId.Value = usuarioId.ToString();
                litModalTitle.Text = "Editar Usuario";
                CargarDatosUsuarioParaEdicion(usuarioId);
                MostrarModalUsuario();
            }
            else if (e.CommandName == "DeleteUsuario")
            {
                try
                {
                    usuarioServiceClient.eliminarUsuario(usuarioId);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionSuccess", "alert('Usuario eliminado exitosamente.');", true);

                    _cachedUsuarios = null; // Invalida la caché para que se vuelva a cargar
                    CargarUsuarios();
                }
                catch (System.Exception ex)
                {
                    litMensajeModal.Text = $"Error al eliminar el usuario: {ex.Message}";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionError", "alert('Error al eliminar el usuario: " + ex.Message.Replace("'", "\\'") + "');", true);
                    System.Diagnostics.Debug.WriteLine("Error al eliminar usuario: " + ex.ToString());
                }
            }
            else if (e.CommandName == "ToggleAdmin")
            {
                try
                {
                    usuario userToToggle = usuarioServiceClient.buscarUsuarioPorId(usuarioId);
                    if (userToToggle != null)
                    {
                        userToToggle.admin = !userToToggle.admin; // Cambia el rol

                        // También podrías actualizar campos de auditoría si los tuvieras en el modelo de usuario
                        // userToToggle.usuarioModificacion = ObtenerIdUsuarioActual();
                        // userToToggle.fechaModificacion = DateTime.Now;
                        // userToToggle.fechaModificacionSpecified = true;

                        usuarioServiceClient.actualizarUsuario(userToToggle);

                        string action = userToToggle.admin ? "ascendido a administrador" : "degradado a usuario regular";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ToggleAdminSuccess", $"alert('Usuario {userToToggle.nombre} ha sido {action} exitosamente.');", true);

                        _cachedUsuarios = null; // Invalida la caché
                        CargarUsuarios(); // Recargar la tabla
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ToggleAdminNotFound", "alert('Usuario no encontrado para cambiar rol.');", true);
                    }
                }
                catch (System.Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToggleAdminError", "alert('Error al cambiar el rol del usuario: " + ex.Message.Replace("'", "\\'") + "');", true);
                    System.Diagnostics.Debug.WriteLine("Error al cambiar rol de usuario: " + ex.ToString());
                }
            }
        }

        private void CargarDatosUsuarioParaEdicion(int usuarioId)
        {
            try
            {
                usuario user = usuarioServiceClient.buscarUsuarioPorId(usuarioId);

                if (user != null)
                {
                    txtNombre.Text = user.nombre;
                    txtEmail.Text = user.email;
                    txtTelefono.Text = user.telefono;
                    // NO CARGAR LA CONTRASEÑA EN EL CAMPO DE TEXTO POR SEGURIDAD
                    txtPassword.Text = ""; // Siempre limpiar o dejar vacío al editar
                    rfvPassword.Enabled = false; // Deshabilitar el RequiredFieldValidator para la contraseña en edición
                    revPassword.Enabled = false; // Deshabilitar el RegularExpressionValidator para la contraseña en edición

                    if (ddlIdiomaPreferido.Items.FindByValue(user.idiomaPreferido) != null)
                    {
                        ddlIdiomaPreferido.SelectedValue = user.idiomaPreferido;
                    }
                    else
                    {
                        ddlIdiomaPreferido.SelectedValue = "es"; // Default si no se encuentra
                    }

                    chkEstaActivo.Checked = user.estaActivo;
                    chkIsAdmin.Checked = user.admin;
                }
                else
                {
                    litMensajeModal.Text = "Usuario no encontrado para edición.";
                }
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = "Error al cargar datos de usuario para edición: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar datos de usuario para edición: " + ex.ToString());
            }
        }

        protected void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            Page.Validate("UsuarioValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                MostrarModalUsuario();
                return;
            }

            usuario user = new usuario
            {
                id = Convert.ToInt32(hdnUsuarioId.Value),
                nombre = txtNombre.Text.Trim(),
                email = txtEmail.Text.Trim(),
                telefono = txtTelefono.Text.Trim(),
                estaActivo = chkEstaActivo.Checked,
                idiomaPreferido = ddlIdiomaPreferido.SelectedValue,
                admin = chkIsAdmin.Checked,
            };

            // Solo actualizar la contraseña si el campo no está vacío (en caso de edición)
            // O si es un nuevo registro
            if (user.id == 0 || !string.IsNullOrEmpty(txtPassword.Text))
            {
                user.password = HashPassword(txtPassword.Text);
            }
            else
            {
                // Si estamos editando y no se cambió la contraseña, mantener la existente
                // Necesitarías cargar el usuario actual para obtener la contraseña hash existente
                usuario existingUser = usuarioServiceClient.buscarUsuarioPorId(user.id);
                if (existingUser != null)
                {
                    user.password = existingUser.password;
                }
            }


            try
            {
                if (user.id == 0) // Nuevo usuario
                {
                    // Asigna fecha de registro al crear nuevo usuario

                    // Update the line causing the error
                    //user.fechaRegistro = ConvertToLocalDate(DateTime.Now);

                    usuarioServiceClient.registrarUsuario(user);
                    litMensajeModal.Text = "Usuario agregado exitosamente.";
                }
                else // Edición de usuario
                {
                    usuarioServiceClient.actualizarUsuario(user);
                    litMensajeModal.Text = "Usuario actualizado exitosamente.";
                }

                _cachedUsuarios = null; // Invalida la caché
                CargarUsuarios(); // Recargar la tabla y estadísticas
                OcultarModalUsuario();
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar el usuario: {ex.Message}";
                MostrarModalUsuario();
                System.Diagnostics.Debug.WriteLine("Error al guardar usuario: " + ex.ToString());
            }
        }

        // --- Métodos para manejar el modal de Usuario ---
        private void MostrarModalUsuario()
        {
            usuarioModal.Style["display"] = "flex";
        }

        private void OcultarModalUsuario()
        {
            usuarioModal.Style["display"] = "none";
            litMensajeModal.Text = "";
            rfvPassword.Enabled = true; // Restablecer el validador de contraseña para nuevos usuarios
            revPassword.Enabled = true; // Restablecer el validador de contraseña para nuevos usuarios
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hdnUsuarioId.Value = "0";
            litModalTitle.Text = "Agregar Nuevo ";
            LimpiarCamposModalUsuario();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearValidators", "clearModalValidators();", true);
            MostrarModalUsuario();
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModalUsuario();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModalUsuario();
        }

        private void LimpiarCamposModalUsuario()
        {
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtPassword.Text = "";
            ddlIdiomaPreferido.SelectedValue = "es"; // Establece un valor por defecto
            chkEstaActivo.Checked = true;
            chkIsAdmin.Checked = false; // Por defecto un nuevo usuario no es admin

            rfvPassword.Enabled = true; // Asegurarse de que sea requerido al agregar
            revPassword.Enabled = true; // Asegurarse de que el regex sea aplicado al agregar
        }

        // --- Métodos para estadísticas (optimizados para usar la caché) ---
        private List<usuario> GetCachedUsuarios()
        {
            if (_cachedUsuarios == null)
            {
                try
                {
                    _cachedUsuarios = usuarioServiceClient.listarUsuarios().ToList();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener usuarios para estadísticas: " + ex.ToString());
                    _cachedUsuarios = new List<usuario>();
                }
            }
            return _cachedUsuarios;
        }

        public int GetTotalUsuarios()
        {
            return GetCachedUsuarios().Count;
        }

        public int GetUsuariosActivos()
        {
            return GetCachedUsuarios().Count(u => u.estaActivo);
        }

        public int GetUsuariosAdmin()
        {
            return GetCachedUsuarios().Count(u => u.admin);
        }

        public int GetUsuariosRegistradosHoy()
        {
            // Asumiendo que fechaRegistro es DateTime y buscas por la fecha actual (sin hora)
            return GetCachedUsuarios().Count(u => u.fechaRegistro == ConvertToLocalDate(DateTime.Today));
        }

        // --- Eventos de filtro ---
        protected void txtSearchUsuarios_TextChanged(object sender, EventArgs e)
        {
            gvUsuarios.PageIndex = 0; // Reiniciar paginación al buscar
            CargarUsuarios();
        }

        protected void ddlEstadoFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvUsuarios.PageIndex = 0; // Reiniciar paginación al filtrar
            CargarUsuarios();
        }

        protected void ddlAdminFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvUsuarios.PageIndex = 0; // Reiniciar paginación al filtrar
            CargarUsuarios();
        }

        // --- Utilidades ---
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Hash la entrada y convierte el array de bytes a una cadena
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convierte el array de bytes a una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private localDate ConvertToLocalDate(DateTime dateTime)
        {
            // Assuming localDate has a constructor or method to set its value from DateTime
            localDate localDateInstance = new localDate();
            // You may need to set specific properties of localDate based on its implementation
            // Example: localDateInstance.Value = dateTime; (if such a property exists)
            return localDateInstance;
        }
    }
}