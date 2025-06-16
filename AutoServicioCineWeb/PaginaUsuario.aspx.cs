using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class PaginaUsuario : System.Web.UI.Page
    {
        // Propiedades para almacenar información del usuario
        private int UsuarioId
        {
            get
            {
                if (Session["UsuarioId"] != null)
                    return Convert.ToInt32(Session["UsuarioId"]);
                return 0;
            }
        }

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["CineConnectionString"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si el usuario está autenticado
                if (Session["UsuarioId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                CargarDatosUsuario();
                CargarConfiguracionUsuario();
            }
        }

        private void CargarDatosUsuario()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            u.Nombres,
                            u.Apellidos,
                            u.Email,
                            u.Telefono,
                            u.FechaNacimiento,
                            u.Genero,
                            u.FechaRegistro,
                            u.RutaAvatar
                        FROM Usuarios u 
                        WHERE u.UsuarioId = @UsuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Llenar los campos del formulario
                                txtNombres.Text = reader["Nombres"].ToString();
                                txtApellidos.Text = reader["Apellidos"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtTelefono.Text = reader["Telefono"].ToString();

                                if (reader["FechaNacimiento"] != DBNull.Value)
                                {
                                    DateTime fechaNac = Convert.ToDateTime(reader["FechaNacimiento"]);
                                    txtFechaNacimiento.Text = fechaNac.ToString("yyyy-MM-dd");
                                }

                                if (reader["Genero"] != DBNull.Value)
                                {
                                    ddlGenero.SelectedValue = reader["Genero"].ToString();
                                }

                                // Información del header
                                nombreUsuario.InnerText = $"{reader["Nombres"]} {reader["Apellidos"]}";
                                emailUsuario.InnerText = reader["Email"].ToString();

                                if (reader["FechaRegistro"] != DBNull.Value)
                                {
                                    DateTime fechaReg = Convert.ToDateTime(reader["FechaRegistro"]);
                                    fechaRegistro.InnerText = fechaReg.ToString("MMMM yyyy",
                                        new System.Globalization.CultureInfo("es-ES"));
                                }

                                // Avatar
                                string rutaAvatar = reader["RutaAvatar"].ToString();
                                if (!string.IsNullOrEmpty(rutaAvatar) && File.Exists(Server.MapPath(rutaAvatar)))
                                {
                                    imgAvatar.Src = rutaAvatar;
                                }
                                else
                                {
                                    imgAvatar.Src = "./images/default-avatar.png";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos del usuario: {ex.Message}");

                // Mostrar mensaje de error al usuario
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error al cargar los datos del perfil. Por favor, intente nuevamente.');", true);
            }
        }

        private void CargarConfiguracionUsuario()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            EmailPromociones,
                            EmailEstrenos,
                            SMSRecordatorios,
                            PerfilPublico,
                            CompartirHistorial
                        FROM ConfiguracionUsuario 
                        WHERE UsuarioId = @UsuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                chkEmailPromociones.Checked = Convert.ToBoolean(reader["EmailPromociones"]);
                                chkEmailEstrenos.Checked = Convert.ToBoolean(reader["EmailEstrenos"]);
                                chkSMSRecordatorios.Checked = Convert.ToBoolean(reader["SMSRecordatorios"]);
                                chkPerfilPublico.Checked = Convert.ToBoolean(reader["PerfilPublico"]);
                                chkCompartirHistorial.Checked = Convert.ToBoolean(reader["CompartirHistorial"]);
                            }
                            else
                            {
                                // Crear configuración por defecto si no existe
                                CrearConfiguracionDefecto();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar configuración: {ex.Message}");
            }
        }

        private void CrearConfiguracionDefecto()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ConfiguracionUsuario 
                        (UsuarioId, EmailPromociones, EmailEstrenos, SMSRecordatorios, PerfilPublico, CompartirHistorial)
                        VALUES (@UsuarioId, 1, 1, 0, 0, 0)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Establecer valores por defecto en los controles
                chkEmailPromociones.Checked = true;
                chkEmailEstrenos.Checked = true;
                chkSMSRecordatorios.Checked = false;
                chkPerfilPublico.Checked = false;
                chkCompartirHistorial.Checked = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear configuración por defecto: {ex.Message}");
            }
        }

        protected void btnGuardarPerfil_Click(object sender, EventArgs e)
        {
            if (!ValidarDatosPersonales())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Usuarios 
                        SET 
                            Nombres = @Nombres,
                            Apellidos = @Apellidos,
                            Email = @Email,
                            Telefono = @Telefono,
                            FechaNacimiento = @FechaNacimiento,
                            Genero = @Genero,
                            FechaModificacion = GETDATE()
                        WHERE UsuarioId = @UsuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        cmd.Parameters.AddWithValue("@Nombres", txtNombres.Text.Trim());
                        cmd.Parameters.AddWithValue("@Apellidos", txtApellidos.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Telefono",
                            string.IsNullOrEmpty(txtTelefono.Text.Trim()) ? (object)DBNull.Value : txtTelefono.Text.Trim());

                        if (string.IsNullOrEmpty(txtFechaNacimiento.Text))
                        {
                            cmd.Parameters.AddWithValue("@FechaNacimiento", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@FechaNacimiento", DateTime.Parse(txtFechaNacimiento.Text));
                        }

                        cmd.Parameters.AddWithValue("@Genero",
                            string.IsNullOrEmpty(ddlGenero.SelectedValue) ? (object)DBNull.Value : ddlGenero.SelectedValue);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // Procesar la subida de avatar si hay un archivo
                            ProcesarAvatar();

                            // Actualizar la información mostrada
                            nombreUsuario.InnerText = $"{txtNombres.Text.Trim()} {txtApellidos.Text.Trim()}";
                            emailUsuario.InnerText = txtEmail.Text.Trim();

                            ScriptManager.RegisterStartupScript(this, GetType(), "success",
                                "alert('Perfil actualizado correctamente.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "error",
                                "alert('No se pudo actualizar el perfil. Intente nuevamente.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar perfil: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error al guardar el perfil. Por favor, intente nuevamente.');", true);
            }
        }

        protected void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE ConfiguracionUsuario 
                        SET 
                            EmailPromociones = @EmailPromociones,
                            EmailEstrenos = @EmailEstrenos,
                            SMSRecordatorios = @SMSRecordatorios,
                            PerfilPublico = @PerfilPublico,
                            CompartirHistorial = @CompartirHistorial,
                            FechaModificacion = GETDATE()
                        WHERE UsuarioId = @UsuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                        cmd.Parameters.AddWithValue("@EmailPromociones", chkEmailPromociones.Checked);
                        cmd.Parameters.AddWithValue("@EmailEstrenos", chkEmailEstrenos.Checked);
                        cmd.Parameters.AddWithValue("@SMSRecordatorios", chkSMSRecordatorios.Checked);
                        cmd.Parameters.AddWithValue("@PerfilPublico", chkPerfilPublico.Checked);
                        cmd.Parameters.AddWithValue("@CompartirHistorial", chkCompartirHistorial.Checked);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "success",
                                "alert('Configuración guardada correctamente.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "error",
                                "alert('No se pudo guardar la configuración. Intente nuevamente.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar configuración: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('Error al guardar la configuración. Por favor, intente nuevamente.');", true);
            }
        }

        private bool ValidarDatosPersonales()
        {
            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('El campo Nombres es obligatorio.');", true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('El campo Apellidos es obligatorio.');", true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('El campo Email es obligatorio.');", true);
                return false;
            }

            // Validar formato de email
            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                if (addr.Address != txtEmail.Text)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                        "alert('El formato del email no es válido.');", true);
                    return false;
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('El formato del email no es válido.');", true);
                return false;
            }

            // Verificar que el email no esté en uso por otro usuario
            if (!ValidarEmailUnico())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('Este email ya está registrado por otro usuario.');", true);
                return false;
            }

            return true;
        }

        private bool ValidarEmailUnico()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email AND UsuarioId != @UsuarioId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al validar email único: {ex.Message}");
                return false;
            }
        }

        private void ProcesarAvatar()
        {
            if (fileUploadAvatar.HasFile)
            {
                try
                {
                    // Validar el archivo
                    if (!ValidarArchivoAvatar())
                        return;

                    // Crear directorio si no existe
                    string directorioCargas = Server.MapPath("~/uploads/avatars/");
                    if (!Directory.Exists(directorioCargas))
                    {
                        Directory.CreateDirectory(directorioCargas);
                    }

                    // Generar nombre único para el archivo
                    string extension = Path.GetExtension(fileUploadAvatar.FileName);
                    string nombreArchivo = $"avatar_{UsuarioId}_{DateTime.Now.Ticks}{extension}";
                    string rutaCompleta = Path.Combine(directorioCargas, nombreArchivo);
                    string rutaRelativa = $"~/uploads/avatars/{nombreArchivo}";

                    // Guardar el archivo
                    fileUploadAvatar.SaveAs(rutaCompleta);

                    // Actualizar la base de datos
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Usuarios SET RutaAvatar = @RutaAvatar WHERE UsuarioId = @UsuarioId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@RutaAvatar", rutaRelativa);
                            cmd.Parameters.AddWithValue("@UsuarioId", UsuarioId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Actualizar la imagen mostrada
                    imgAvatar.Src = rutaRelativa;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al procesar avatar: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Error al subir la imagen del avatar.');", true);
                }
            }
        }

        private bool ValidarArchivoAvatar()
        {
            // Validar tamaño (máximo 5MB)
            if (fileUploadAvatar.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('El archivo es demasiado grande. Máximo 5MB.');", true);
                return false;
            }

            // Validar tipo de archivo
            string[] tiposPermitidos = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string extension = Path.GetExtension(fileUploadAvatar.FileName).ToLower();

            if (Array.IndexOf(tiposPermitidos, extension) == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "validation",
                    "alert('Solo se permiten archivos de imagen (JPG, PNG, GIF, BMP).');", true);
                return false;
            }

            return true;
        }
    }
}