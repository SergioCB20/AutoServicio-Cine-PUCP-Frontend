using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.ComidaWS; // Make sure this namespace is correct for your service reference

namespace AutoServicioCineWeb
{
    public partial class GestionComidas : System.Web.UI.Page
    {
        // Declare the SOAP client proxy
        private readonly ComidaWSClient comidaServiceClient;

        public GestionComidas()
        {
            // Initialize the SOAP client
            comidaServiceClient = new ComidaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the query string contains an "id" parameter
                string comidaIdStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(comidaIdStr))
                {
                    // Try to parse the ID from the query string
                    if (int.TryParse(comidaIdStr, out int comidaId))
                    {
                        hdnComidaId.Value = comidaId.ToString();

                        CargarDatosComidaParaEdicion(comidaId);
                        litModalTitle.Text = "Editar Comida";
                    }
                    else
                    {
                        // Handle invalid ID format
                        hdnComidaId.Value = string.Empty;
                        litModalTitle.Text = "Agregar Comida";
                        LimpiarCamposModal();

                    }
                }
                else
                {
                    // No ID provided, set to empty
                    hdnComidaId.Value = string.Empty;
                    litModalTitle.Text = "Agregar Comida";
                    LimpiarCamposModal();
                }

                CargarComidas();
            }
        }

        private void CargarDatosComidaParaEdicion(int comidaId)
        {
            try
            {
                // Call the SOAP service to get the comida details
                producto producto = comidaServiceClient.buscarComidaPorId(comidaId);
                if (producto != null)
                {
                    txtNombre.Text = producto.nombre;
                    txtPrecio.Text = producto.precio.ToString();
                    txtDescripcion.Text = producto.descripcion;
                    txtTipo.Text = producto.tipo.ToString();
                    chkActivo.Checked = producto.estaActivo;

                    if (!string.IsNullOrEmpty(producto.descripcion)) //descripcion es la imagen
                    {
                        imgPreview.ImageUrl = producto.descripcion;
                        imgPreview.Style["display"] = "block"; // Show the image preview
                    }
                    else
                    {
                        imgPreview.ImageUrl = ""; // Default image if none is provided
                        imgPreview.Style["display"] = "none"; // Hide the image preview
                    }

                }
                else
                {
                    litMensajeModal.Text = "Comida no encontrada.";
                }
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar los datos de la comida: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void CargarComidas()
        {
            try
            {
                // Call the SOAP service to get the list of comidas
                List<producto> productos = comidaServiceClient.listarComidas().ToList();
                gvComidas.DataSource = productos;
                gvComidas.DataBind();
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar las comidas: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void LimpiarCamposModal()
        {
            txtNombre.Text = string.Empty;
            txtPrecio.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtTipo.Text = string.Empty;
            chkActivo.Checked = false;
            imgPreview.ImageUrl = string.Empty;
            imgPreview.Style["display"] = "none"; // Hide the image preview

            if(Page.Validators != null)
            {
                foreach (BaseValidator validator in Page.Validators)
                {
                    validator.IsValid = true; // Reset validation
                    validator.ErrorMessage = string.Empty; // Clear error messages
                }
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {

        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
        }
        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        protected void gvComidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }
        protected void gvComidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
    }  
}