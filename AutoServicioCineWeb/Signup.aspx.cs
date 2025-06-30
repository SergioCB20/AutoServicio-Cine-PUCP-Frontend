using System;
using System.Web.UI;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class Signup : Page
    {
        private readonly AuthWSClient authServiceClient;

        public Signup()
        {
            authServiceClient = new AuthWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Text = ""; // Limpiar cualquier mensaje de error
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    usuario nuevoUsuario = new usuario();
                    nuevoUsuario.nombre = txtNombre.Text;
                    nuevoUsuario.email = txtEmail.Text;
                    nuevoUsuario.telefono = txtTelefono.Text;
                    nuevoUsuario.password = txtPassword.Text;

                    string resultMessage = authServiceClient.registrarUsuario(nuevoUsuario);

                    lblMessage.Text = resultMessage;
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    lblMessage.CssClass = "asp-message success";

                    txtNombre.Text = "";
                    txtEmail.Text = "";
                    txtTelefono.Text = "";
                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";

                    Response.Redirect("Login.aspx");
                }
                catch (System.ServiceModel.FaultException faultEx)
                {
                    lblMessage.Text = faultEx.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.CssClass = "asp-message";
                }
                catch (System.Exception ex)
                {
                    lblMessage.Text = $"Ha ocurrido un error inesperado durante el registro: {ex.Message}";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.CssClass = "asp-message";
                }
            }
        }
    }
}