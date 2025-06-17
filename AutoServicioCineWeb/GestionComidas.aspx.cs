using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS; // Make sure this namespace is correct for your service reference

namespace AutoServicioCineWeb
{
    public partial class GestionComidas : System.Web.UI.Page
    {
        // Declare the SOAP client proxy
        private readonly ProductoWSClient comidaServiceClient;

        public GestionComidas()
        {
            // Initialize the SOAP client
            comidaServiceClient = new ProductoWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string comidaIdStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(comidaIdStr))
                {
                    if (int.TryParse(comidaIdStr, out int comidaId))
                    {
                        hdnComidaId.Value = comidaId.ToString();
                        CargarDatosComidaParaEdicion(comidaId);
                        litModalTitle.Text = "Editar Comida";
                    }
                    else
                    {
                        hdnComidaId.Value = string.Empty;
                        litModalTitle.Text = "Agregar Comida";
                        LimpiarCamposModal();
                    }
                }
                else
                {
                    hdnComidaId.Value = string.Empty;
                    litModalTitle.Text = "Agregar Comida";
                    LimpiarCamposModal();
                }

                // Intentar cargar comidas, pero no fallar si hay error
                CargarComidasSeguro();
            }
        }

        private void CargarComidasSeguro()
        {
            try
            {
                CargarComidas();
            }
            catch (System.Exception ex)
            {
                // Log del error pero no mostrar en la interfaz todavía
                System.Diagnostics.Debug.WriteLine("Error al cargar comidas en Page_Load: " + ex.Message);

                // Mostrar GridView vacío para que la página funcione
                gvComidas.DataSource = new List<producto>();
                gvComidas.DataBind();

                // Mostrar mensaje informativo EN LA TABLA, no en el modal
                litMensajeTabla.Text = "<div class='alert alert-info text-center' style='margin-top: 20px;'>🔄 Cargando datos... Si persiste el problema, verifique la conexión al servicio.</div>";
                litMensajeModal.Text = ""; // No mostrar en modal
            }
        }

        private void CargarDatosComidaParaEdicion(int comidaId)
        {
            //try
            //{
            //    producto producto = comidaServiceClient.buscarProductoPorId(comidaId);
            //    if (producto != null)
            //    {
            //        //txtNombre.Text = producto.nombre;
            //        //txtPrecio.Text = producto.precio.ToString("F2");
            //        //txtDescripcion.Text = producto.descripcion;

            //        // Asegurar que el tipo se asigne correctamente
            //        ddlTipo.SelectedValue = producto.tipo.ToString();

            //        chkActivo.Checked = producto.estaActivo;

            //        // Mostrar previsualización de imagen si existe
            //        if (!string.IsNullOrEmpty(producto.descripcion) && IsValidUrl(producto.descripcion))
            //        {
            //            imgPreview.ImageUrl = producto.descripcion;
            //            imgPreview.Style["display"] = "block";
            //        }
            //        else
            //        {
            //            imgPreview.Style["display"] = "none";
            //        }

            //        // Mostrar el modal automáticamente si venimos con ID en query string
            //        ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditModal",
            //            "document.getElementById('foodModal').style.display = 'flex';", true);
            //    }
            //    else
            //    {
            //        litMensajeModal.Text = "<div class='alert alert-warning'>Comida no encontrada.</div>";
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    litMensajeModal.Text = "<div class='alert alert-danger'>Error al cargar los datos de la comida: " + ex.Message + "</div>";
            //    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            //}
        }

        private void CargarComidas()
        {
            try
            {
                var productosArray = comidaServiceClient.listarProductos();

                List<producto> productos = productosArray?.ToList() ?? new List<producto>();
                gvComidas.DataSource = productos;
                gvComidas.DataBind();

                // Mostrar mensaje informativo si no hay productos - AHORA EN LA TABLA
                if (productos.Count == 0)
                {
                    litMensajeTabla.Text = "<div class='alert alert-info text-center' style='margin-top: 20px;'>📦 No hay productos registrados.<br/>Agregue el primer producto usando el botón '➕ Agregar Nuevo Producto'.</div>";
                    litMensajeModal.Text = ""; // Limpiar mensaje del modal
                }
                else
                {
                    litMensajeTabla.Text = ""; // Limpiar mensaje de tabla
                    litMensajeModal.Text = ""; // Limpiar mensaje del modal
                }
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                // Manejar específicamente el caso de tabla vacía
                if (faultEx.Message.Contains("No se pudo listar el registro"))
                {
                    // Tratar como lista vacía
                    gvComidas.DataSource = new List<producto>();
                    gvComidas.DataBind();
                    litMensajeTabla.Text = "<div class='alert alert-info text-center' style='margin-top: 20px;'>📦 No hay productos registrados. La tabla está vacía.</div>";
                    litMensajeModal.Text = ""; // Limpiar mensaje del modal
                }
                else
                {
                    litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error del servicio: {faultEx.Message}</div>";
                    litMensajeModal.Text = ""; // No mostrar en modal
                }
            }
            catch (System.Exception ex)
            {
                litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error al cargar productos: {ex.Message}</div>";
                litMensajeModal.Text = ""; // No mostrar en modal
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

                // Mostrar GridView vacío para que la página funcione
                gvComidas.DataSource = new List<producto>();
                gvComidas.DataBind();
            }
        }

        private void LimpiarCamposModal()
        {
            txtNombre.Text = string.Empty;
            txtPrecio.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            ddlTipo.SelectedIndex = 0; // Seleccionar el primer elemento (vacío)
            chkActivo.Checked = true; // Cambié esto a true por defecto
            imgPreview.ImageUrl = string.Empty;
            imgPreview.Style["display"] = "none"; // Hide the image preview

            if (Page.Validators != null)
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
            Page.Validate("ComidaValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "<div class='alert alert-danger'>Por favor, corrija los errores antes de guardar.</div>";
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('foodModal').style.display = 'flex';", true);
                return;
            }

            // Validar que se haya seleccionado una categoría
            if (string.IsNullOrEmpty(ddlTipo.SelectedValue) || ddlTipo.SelectedValue == "")
            {
                litMensajeModal.Text = "<div class='alert alert-danger'>Por favor, seleccione una categoría.</div>";
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('foodModal').style.display = 'flex';", true);
                return;
            }

            // Validar precio
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) || precio <= 0)
            {
                litMensajeModal.Text = "<div class='alert alert-danger'>El precio debe ser un número válido mayor a 0.</div>";
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('foodModal').style.display = 'flex';", true);
                return;
            }

            try
            {
                // CAMBIO PRINCIPAL: Crear el producto con una inicialización más explícita
                producto prod = new producto();
                //prod.nombre = txtNombre.Text.Trim();
                //prod.precio = (double)precio;
                //prod.descripcion = txtDescripcion.Text.Trim();
                //prod.estaActivo = chkActivo.Checked;

                // Asignar el enum directamente por string
                string tipoSeleccionado = ddlTipo.SelectedValue.Trim().ToUpper();

                // Verificar que el valor seleccionado sea válido antes de convertir
                if (!Enum.IsDefined(typeof(tipoProducto), tipoSeleccionado))
                {
                    litMensajeModal.Text = "<div class='alert alert-danger'>El tipo de producto seleccionado no es válido: '" + tipoSeleccionado + "'</div>";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                        "document.getElementById('foodModal').style.display = 'flex';", true);
                    return;
                }

                // Asignar usando Enum.Parse
                prod.tipo = (tipoProducto)Enum.Parse(typeof(tipoProducto), tipoSeleccionado);

                // --- LÍNEA CLAVE PARA QUE EL ENUM SE ENVÍE CORRECTAMENTE AL SERVICIO JAVA ---
                prod.tipoSpecified = true; //
                // --- FIN DE LÍNEA CLAVE ---

                // Debug mejorado
                System.Diagnostics.Debug.WriteLine($"Valor del dropdown: '{ddlTipo.SelectedValue}'");
                System.Diagnostics.Debug.WriteLine($"Tipo procesado: '{tipoSeleccionado}'");
                System.Diagnostics.Debug.WriteLine($"Enum asignado: {prod.tipo}");
                // System.Diagnostics.Debug.WriteLine($"Enum es null: {prod.tipo == null}"); // Esta línea generaba la advertencia CS0472

                // Verificar si es edición o nuevo registro
                int comidaId = 0;
                bool esEdicion = !string.IsNullOrEmpty(hdnComidaId.Value) &&
                                 int.TryParse(hdnComidaId.Value, out comidaId) &&
                                 comidaId > 0;

                if (esEdicion)
                {
                    prod.id = comidaId;
                    System.Diagnostics.Debug.WriteLine($"Editando producto ID: {comidaId} con tipo: {prod.tipo}");

                    // No es necesario verificar prod.tipo == null aquí en C# por el warning CS0472
                    // El problema de null ocurre si tipoSpecified no se establece correctamente.

                    comidaServiceClient.actualizarProducto(prod);
                    litMensajeModal.Text = "<div class='alert alert-success'>Comida actualizada exitosamente.</div>";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Creando nuevo producto con tipo: {prod.tipo}");

                    // No es necesario verificar prod.tipo == null aquí en C# por el warning CS0472
                    // El problema de null ocurre si tipoSpecified no se establece correctamente.

                    comidaServiceClient.registrarProducto(prod);
                    litMensajeModal.Text = "<div class='alert alert-success'>Comida agregada exitosamente.</div>";
                }

                // Limpiar campos y cerrar modal
                LimpiarCamposModal();
                hdnComidaId.Value = "";
                CargarComidas();

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal",
                    "closeModal(); showNotification('" + (esEdicion ? "Comida actualizada" : "Comida agregada") + " exitosamente', 'success');", true);
            }
            catch (System.Exception ex)
            {
                // Mostrar información detallada del error
                string errorDetallado = $"Error al guardar la comida: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorDetallado += $"<br/>Error interno: {ex.InnerException.Message}";
                }
                errorDetallado += $"<br/>Tipo seleccionado: {ddlTipo.SelectedValue}";

                litMensajeModal.Text = $"<div class='alert alert-danger'>{errorDetallado}</div>";
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal",
                    "document.getElementById('foodModal').style.display = 'flex';", true);
                System.Diagnostics.Debug.WriteLine("Error completo: " + ex.ToString());
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Implementar lógica de búsqueda si es necesario
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implementar lógica de filtrado si es necesario
        }

        protected void gvComidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvComidas.PageIndex = e.NewPageIndex;
            CargarComidas();
        }

        protected void gvComidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (int.TryParse(e.CommandArgument.ToString(), out id))
            {
                if (e.CommandName == "EditComida")
                {
                    // Redirigir para editar
                    Response.Redirect($"GestionComidas.aspx?id={id}");
                }
                else if (e.CommandName == "DeleteComida")
                {
                    try
                    {
                        comidaServiceClient.eliminarProducto(id);
                        CargarComidas();
                        litMensajeTabla.Text = "<div class='alert alert-success text-center' style='margin-top: 20px;'>✅ Producto eliminado exitosamente.</div>";
                    }
                    catch (System.Exception ex)
                    {
                        litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error al eliminar: {ex.Message}</div>";
                    }
                }
            }
        }
    }
}