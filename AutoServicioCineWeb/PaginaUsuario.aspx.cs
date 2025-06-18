using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class PaginaUsuario : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usuario"] != null)
                {
                    usuario usuario = (usuario)Session["Usuario"];
                    CargarDatosUsuario(usuario);
                    CargarHistorialCompras(usuario.id);
                }
                else
                {
                    // Redirigir a login si no hay sesión
                    //Response.Redirect("~/Sigup.aspx");
                }
            }
        }

        private void CargarDatosUsuario(usuario usuario)
        {
            lblNombre.Text = usuario.nombre;
            lblEmail.Text = usuario.email;
            lblTelefono.Text = usuario.telefono;

            // Cargar también en los campos de edición
            txtNombreEdit.Text = usuario.nombre;
            txtEmailEdit.Text = usuario.email;
            txtTelefonoEdit.Text = usuario.telefono;
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
            pnlEdicion.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Session["Usuario"] != null)
            {
                usuario usuario = (usuario)Session["Usuario"];

                // Actualizar datos del usuario
                usuario.nombre = txtNombreEdit.Text;
                usuario.email = txtEmailEdit.Text;
                usuario.telefono = txtTelefonoEdit.Text;

                // Si se proporcionó nueva contraseña, actualizarla
                //if (!string.IsNullOrEmpty(txtPasswordEdit.Text))
                //{
                //    usuario.Password = PasswordHasher.HashPassword(txtPasswordEdit.Text);
                //}

                try
                {
                    // Llamar al servicio web para actualizar
                    //var usuarioWS = new UsuarioWS();
                    //usuarioWS.actualizarUsuario(usuario);

                    // Actualizar sesión
                    Session["Usuario"] = usuario;
                    CargarDatosUsuario(usuario);

                    // Ocultar panel de edición
                    pnlEdicion.Visible = false;

                    // Mostrar mensaje de éxito
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccess",
                        "alert('Perfil actualizado correctamente');", true);
                }
                catch (System.Exception ex)
                {
                    // Mostrar mensaje de error
                    ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                        $"alert('Error al actualizar perfil: {ex.Message}');", true);
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlEdicion.Visible = false;
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
