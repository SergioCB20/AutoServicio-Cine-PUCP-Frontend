using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS; // Asegúrate de que este namespace sea correcto para tu referencia de servicio
using System.Diagnostics; // Para Debug.WriteLine
using System.IO;
using System.Web.Security;

namespace AutoServicioCineWeb
{
    public partial class GestionComidas : System.Web.UI.Page
    {
        private readonly ProductoWSClient comidaServiceClient;
        // Caché para las estadísticas y evitar múltiples llamadas al listar en la misma petición
        private List<producto> _cachedComidas;
        private int idUsuario = 34; // ID del usuario que está haciendo las modificaciones, ajusta según tu lógica de autenticación
        public GestionComidas()
        {
            comidaServiceClient = new ProductoWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Solo cargar comidas la primera vez que se carga la página
                CargarComidas();
                if (Context.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)Context.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    string userData = ticket.UserData;
                    string[] userInfo = userData.Split('|');
                    idUsuario = int.Parse(userInfo[0]); // Asumiendo que el ID de usuario es el primer elemento
                }

            }
            // Registrar script para asegurar que la previsualización de imagen se muestre
            // incluso después de PostBacks (por ejemplo, si hay errores de validación)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReapplyImagePreview",
                "const txtImgUrlElement = document.getElementById('" + txtImagenUrl.ClientID + "'); if (txtImgUrlElement && txtImgUrlElement.value) { previewImage(txtImgUrlElement); }", true);
        }
        private void CargarComidas()
        {
            try
            {
                // Forzar la recarga de la caché para obtener los datos más recientes
                _cachedComidas = comidaServiceClient.listarProductos().ToList();
                List<producto> productosFiltrados = FiltrarComidas(_cachedComidas);

                gvComidas.DataSource = productosFiltrados;
                gvComidas.DataBind();

            }
            catch (System.Exception ex)
            {
                litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error al cargar las comidas: {ex.Message}</div>";
                Debug.WriteLine("Error en CargarComidas: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("Error al cargar productos: " + ex.ToString());
            }
        }

        private List<producto> FiltrarComidas(List<producto> productos)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchProductos.Text))
            {
                string searchTerm = txtSearchProductos.Text.Trim().ToLower();
                productos = productos.Where(p =>
                    p.nombre_es.ToLower().Contains(searchTerm) ||
                    p.nombre_en.ToLower().Contains(searchTerm) ||
                    p.descripcion_es.ToLower().Contains(searchTerm) ||
                    p.descripcion_en.ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(ddlCategoryFilter.SelectedValue))
            {
                string classificationFilter = ddlCategoryFilter.SelectedValue;
                if (Enum.TryParse(classificationFilter, out tipoProducto classificationFilterEnum)) // <-- ¡Aquí está la magia!
                {
                    // Si la conversión fue exitosa, 'classificationFilterEnum' ahora es del tipo 'tipoProducto'
                    // Y ahora sí podemos comparar p.tipo (enum) con classificationFilterEnum (enum)
                    productos = productos.Where(p => p.tipo == classificationFilterEnum).ToList();
                }
            }
            return productos;
        }

        protected void gvComidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvComidas.PageIndex = e.NewPageIndex;
            CargarComidas();
        }


        // Manejador para los comandos de fila (Editar, Eliminar) en el GridView
        protected void gvComidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int producto_id = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "EditComida")
            {
                hdnComidaId.Value = producto_id.ToString();
                litModalTitle.Text = "Editar Comida";
                CargarDatosComidaParaEdicion(producto_id);
                MostrarModalComida();
            }
            else if (e.CommandName == "DeleteComida")
            {
                try
                {
                    comidaServiceClient.eliminarProducto(producto_id);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionSuccess", "alert('Producto eliminada exitosamente.');", true);

                    _cachedComidas = null;
                    CargarComidas(); // Recargar la tabla después de eliminar
                }
                catch (System.Exception ex)
                {
                    litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error al eliminar: {ex.Message}</div>";
                    Debug.WriteLine("Error al eliminar en gvComidas_RowCommand: " + ex.ToString());
                }
            }

        }
        private void MostrarModalComida()
        {
            productoModal.Style["display"] = "flex";
        }

        private void CargarDatosComidaParaEdicion(int idComida)
        {
            try
            {
                producto producto = comidaServiceClient.buscarProductoPorId(idComida);
                if (producto != null)
                {
                    //hdnComidaId.Value = producto.id.ToString(); // Setear el ID para la edición
                    txtNombre.Text = producto.nombre_es;
                    txtNombreEn.Text = producto.nombre_en; // Cargar nombre en inglés
                    txtDescripcionEs.Text = producto.descripcion_es; // Cargar descripción en español
                    txtDescripcionEn.Text = producto.descripcion_en; // Cargar descripción en inglés
                    ddlTipo.SelectedValue = producto.tipo.ToString(); // Convertir Enum a string
                    txtPrecio.Text = producto.precio.ToString();
                    txtImagenUrl.Text = producto.imagenUrl; // Cargar la URL de la imagen
                    chkActivo.Checked = producto.estaActivo;
                    litModalTitle.Text = "Editar"; // Actualizar título del modal
                    litMensajeModal.Text = ""; // Limpiar mensajes anteriores

                    // Abrir el modal después de cargar los datos
                    ScriptManager.RegisterStartupScript(this, GetType(), "openEditModal",
                        @"setTimeout(function() { 
                            console.log('Abriendo modal para editar...'); 
                            openModal(); 
                            // Previsualizar imagen si existe
                            const txtImgUrlElement = document.getElementById('" + txtImagenUrl.ClientID + @"');
                            if (txtImgUrlElement && txtImgUrlElement.value) {
                                previewImage(txtImgUrlElement);
                            }
                        }, 150);", true);
                }
                else
                {
                    litMensajeTabla.Text = "<div class='alert alert-warning text-center' style='margin-top: 20px;'>⚠️ Producto no encontrado.</div>";
                }
            }
            catch (System.Exception ex)
            {
                litMensajeTabla.Text = $"<div class='alert alert-danger text-center' style='margin-top: 20px;'>❌ Error al cargar datos del producto: {ex.Message}</div>";
                Debug.WriteLine("Error en CargarDatosComidaParaEdicion: " + ex.ToString());
                //LimpiarCamposModal();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
    
            Page.Validate("ComidaValidation"); // Asegúrate de que este ValidationGroup esté configurado en tus validadores.

            if (!Page.IsValid)
            {
              
                litMensajeModal.Text = "<div class='alert alert-warning'>⚠️ Por favor, corrige los errores en el formulario.</div>";
                // Asegúrate de que la función JavaScript 'openModal()' exista y funcione correctamente.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openModalOnValidationFail",
                    @"setTimeout(function() {
                console.log('Errores de validación, manteniendo modal abierto...');
                openModal();
            }, 150);", true);
                return; // Detener la ejecución si la página no es válida.
            }

            // 2. Crear una instancia del objeto 'producto' y asignar los valores de los controles de la UI.
            producto prod = new producto
            {
                // El hdnComidaId debe contener el ID del producto si es una edición, o 0 si es nuevo.
                id = int.Parse(hdnComidaId.Value),
                idSpecified = true,

                nombre_es = txtNombre.Text.Trim(),
                nombre_en = txtNombreEn.Text.Trim(), // NUEVO: Asumiendo que tienes un control txtNombreEn

                descripcion_es = txtDescripcionEs.Text.Trim(), // NUEVO: Asumiendo que tienes un control txtDescripcionEs
                descripcion_en = txtDescripcionEn.Text.Trim(), // NUEVO: Asumiendo que tienes un control txtDescripcionEn

                precio = double.Parse(txtPrecio.Text.Trim()),
                tipo = (tipoProducto)Enum.Parse(typeof(tipoProducto), ddlTipo.SelectedValue, ignoreCase: true),
                tipoSpecified = true,

                imagenUrl = txtImagenUrl.Text.Trim(), // CORREGIDO: Asumiendo que tienes un control txtImagenUrl

                estaActivo = chkActivo.Checked,

                usuarioModificacion = idUsuario, // AJUSTADO: Se recomienda que este ID provenga de una variable global o sesión.
                usuarioModificacionSpecified = true,

                fechaModificacion = DateTime.Now,
                fechaModificacionSpecified = true
            };

            // 3. Intentar guardar o actualizar el producto.
            try
            {
                if (prod.id == 0) // Si el ID es 0, es un nuevo producto (registro).
                {
                    comidaServiceClient.registrarProducto(prod);
                    litMensajeModal.Text = "<div class='alert alert-success'>✅ Producto agregado exitosamente.</div>";
                }
                else // Si el ID es diferente de 0, es una edición (actualización).
                {
                    comidaServiceClient.actualizarProducto(prod);
                    litMensajeModal.Text = "<div class='alert alert-success'>✅ Producto actualizado exitosamente.</div>";
                }

                _cachedComidas = null;
                CargarComidas();
                OcultarModalProducto();
            }
            catch (System.Exception ex)
            {
                // 6. Manejo de cualquier otro tipo de error (ej. error del servicio web).
                litMensajeModal.Text = $"<div class='alert alert-danger'>❌ Error al guardar/actualizar el producto: {ex.Message}</div>";
                System.Diagnostics.Debug.WriteLine("Error en btnGuardar_Click: " + ex.ToString());
                // Mantener el modal abierto en caso de error.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openModalOnError",
                    @"setTimeout(function() {
                console.log('Error al guardar, manteniendo modal abierto...');
                openModal();
            }, 150);", true);
            }
        }

        private void OcultarModalProducto()
        {
            productoModal.Style["display"] = "none";
            litMensajeModal.Text = "";
        }
        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            // Limpiar campos primero
            hdnComidaId.Value = "0";
            litModalTitle.Text = "Agregar Nueva"; // Título para añadir
            LimpiarCamposModal();
            litMensajeModal.Text = ""; // Limpiar mensajes anteriores
            MostrarModalComida();

        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModalProducto();
        }
        // Manejador del botón "Cerrar" del modal
        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModalProducto();
        }

        private void LimpiarCamposModal()
        {
            hdnComidaId.Value = "0"; // Resetear a 0 para indicar "agregar nuevo"
            txtNombre.Text = string.Empty;
            txtNombreEn.Text = string.Empty; // Limpiar nombre en inglés
            txtDescripcionEs.Text = string.Empty; // Limpiar descripción en español
            txtDescripcionEn.Text = string.Empty; // Limpiar descripción en inglés
            ddlTipo.SelectedIndex = 0; // Resetear al primer elemento (vacío)
            txtPrecio.Text = string.Empty;
            txtImagenUrl.Text = string.Empty; // Limpiar la URL de la imagen
            chkActivo.Checked = true;
            litMensajeModal.Text = string.Empty;

            imgPreview.Style["display"] = "none";
            hdnExistingImageUrl.Value = string.Empty;
        }

        private void MostrarModalCsv()
        {
            csvUploadModal.Style["display"] = "flex";
            litMensajeCsvModal.Text = ""; // Limpiar mensajes previos al abrir
        }

        private void OcultarModalCsv()
        {
            csvUploadModal.Style["display"] = "none";
            litMensajeCsvModal.Text = ""; // Limpiar mensajes al cerrar
            FileUploadCsv.Attributes.Remove("value"); // Limpiar el nombre del archivo seleccionado en el control FileUpload
        }

        protected void btnOpenCsvImportModal_Click(object sender, EventArgs e)
        {
            MostrarModalCsv();
        }

        protected void btnCloseCsvModal_Click(object sender, EventArgs e)
        {
            OcultarModalCsv();
        }

        protected void btnCancelCsvModal_Click(object sender, EventArgs e)
        {
            OcultarModalCsv();
        }

        // Validación personalizada del tipo de archivo
        protected void cvCsvFileExtension_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (FileUploadCsv.HasFile)
            {
                string fileExtension = Path.GetExtension(FileUploadCsv.FileName).ToLower();
                args.IsValid = (fileExtension == ".csv");
            }
            else
            {
                args.IsValid = false; // El RequiredFieldValidator ya debería manejar esto, pero por seguridad
            }
        }

        protected void btnUploadCsv_Click(object sender, EventArgs e)
        {
            Page.Validate("CsvUploadValidation"); // Validar solo el grupo del CSV

            if (!Page.IsValid)
            {
                litMensajeCsvModal.Text = "Por favor, corrige los errores en el formulario de carga CSV.";
                MostrarModalCsv(); // Mantener modal abierto con errores
                return;
            }

            if (!FileUploadCsv.HasFile)
            {
                litMensajeCsvModal.Text = "Por favor, selecciona un archivo CSV para subir.";
                MostrarModalCsv();
                return;
            }

            // Asegurarse de que sea un archivo CSV (ya validado por cvCsvFileExtension_ServerValidate, pero doble chequeo)
            string fileExtension = Path.GetExtension(FileUploadCsv.FileName).ToLower();
            if (fileExtension != ".csv")
            {
                litMensajeCsvModal.Text = "Formato de archivo no válido. Por favor, sube un archivo CSV.";
                MostrarModalCsv();
                return;
            }

            int productosAgregados = 0;
            int productosActualizados = 0;
            int errores = 0;

            try
            {
                using (StreamReader reader = new StreamReader(FileUploadCsv.PostedFile.InputStream))
                {
                    string headerLine = reader.ReadLine(); // Leer la primera línea (cabecera)
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        litMensajeCsvModal.Text = "El archivo CSV está vacío o no tiene cabecera.";
                        MostrarModalCsv();
                        return;
                    }

                    // Convertir la cabecera a un diccionario para mapear nombres a índices
                    var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();
                    var headerMap = new Dictionary<string, int>();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        headerMap[headers[i]] = i;
                    }

                    // Verificar que las columnas mínimas existan
                    string[] requiredColumns = { "nombre_es", "nombre_en", "descripcion_es", "descripcion_en", "precio", "tipo", "esta_Activo", "ImagenUrl" };
                    foreach (var col in requiredColumns)
                    {
                        if (!headerMap.ContainsKey(col))
                        {
                            litMensajeCsvModal.Text = $"Error: La columna '{col}' es requerida en el CSV.";
                            MostrarModalCsv();
                            return;
                        }
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue; // Saltar líneas vacías

                        string[] data = line.Split(',');

                        producto prod = new producto();

                        try
                        {

                            prod.nombre_es = GetCsvValue(data, headerMap, "nombre_es");
                            prod.nombre_en = GetCsvValue(data, headerMap, "nombre_en");
                            prod.descripcion_es = GetCsvValue(data, headerMap, "descripcion_es");
                            prod.descripcion_en = GetCsvValue(data, headerMap, "descripcion_en");
                            double prec;
                            if (double.TryParse(GetCsvValue(data, headerMap, "precio"), out prec))
                            {
                                prod.precio = prec;
                            }
                            else
                            {
                                throw new FormatException("Precio no es un número válido.");
                            }
                            tipoProducto tipoprod;
                            if (Enum.TryParse(GetCsvValue(data, headerMap, "tipo"), out tipoprod))
                            {
                                prod.tipo = tipoprod;
                                prod.tipoSpecified = true; // Marcar como especificado para el servicio
                            }
                            else
                            {
                                throw new FormatException("Tipo no es válido.");
                            }

                            bool estaActiva;
                            if (bool.TryParse(GetCsvValue(data, headerMap, "esta_Activo"), out estaActiva))
                            {
                                prod.estaActivo = estaActiva;
                                
                            }
                            else
                            {
                                // Intentar parsear "TRUE"/"FALSE" o "1"/"0" si bool.TryParse falla directamente
                                string activeString = GetCsvValue(data, headerMap, "esta_Activo").ToLower().Trim();
                                if (activeString == "true" || activeString == "1") prod.estaActivo = true;
                                else if (activeString == "false" || activeString == "0") prod.estaActivo = false;
                                else throw new FormatException("EstaActiva no es un valor booleano válido (TRUE/FALSE, 1/0).");
                            }

                            prod.imagenUrl = GetCsvValue(data, headerMap, "ImagenUrl");
                            prod.usuarioModificacion = idUsuario; // Usuario fijo para la carga
                            prod.usuarioModificacionSpecified = true;

                            if (prod.id == 0)
                            {
                                comidaServiceClient.registrarProducto(prod);
                                productosAgregados++;
                            }
                            else
                            {
                                comidaServiceClient.actualizarProducto(prod);
                                productosActualizados++;
                            }
                        }
                        catch (System.Exception exLinea)
                        {
                            errores++;
                            System.Diagnostics.Debug.WriteLine($"Error procesando línea CSV: {line}. Error: {exLinea.Message}");
                        }
                    }
                }

                _cachedComidas = null; // Invalida la caché
                CargarComidas(); // Recargar la tabla y estadísticas

                litMensajeCsvModal.Text = $"Carga CSV completada: {productosAgregados} agregadas, {productosActualizados} actualizadas, {errores} con errores.";
                OcultarModalCsv(); // Cerrar el modal después de la carga exitosa (o con resumen de errores)
            }
            catch (System.Exception ex)
            {
                litMensajeCsvModal.Text = $"Error general al procesar el archivo CSV: {ex.Message}";
                System.Diagnostics.Debug.WriteLine("Error al procesar CSV: " + ex.ToString());
                MostrarModalCsv(); // Mantener el modal abierto si hay un error grave
            }
        }


        private string GetCsvValue(string[] data, Dictionary<string, int> headerMap, string columnName)
        {
            if (headerMap.ContainsKey(columnName) && headerMap[columnName] < data.Length)
            {
                return data[headerMap[columnName]].Trim();
            }
            return string.Empty;
        }


        // Métodos auxiliares para las estadísticas (igual que en Peliculas)
        private List<producto> GetCachedComidas()
        {
            // Solo lista del servicio si la caché está vacía
            if (_cachedComidas == null)
            {
                try
                {
                    // Llama al servicio para obtener la lista de productos

                    _cachedComidas = comidaServiceClient.listarProductos().ToList();
                }
                catch (System.Exception ex)
                {
                    // Loguear el error para depuración
                    Debug.WriteLine("Error al obtener productos para estadísticas: " + ex.ToString());
                    _cachedComidas = new List<producto>(); // Retornar lista vacía en caso de error
                }
            }
            return _cachedComidas;
        }

        public int GetTotalComidas()
        {
            return GetCachedComidas().Count;
        }

        public int GetComidasActivas()
        {
            return GetCachedComidas().Count(p => p.estaActivo);
        }

        public int GetComidasInactivas()
        {
            return GetCachedComidas().Count(p => !p.estaActivo);
        }

        public int GetTiposProductoUnicos()
        {
            return GetCachedComidas().Select(p => p.tipo).Distinct().Count();
        }

        // Manejador para el cambio de texto en el campo de búsqueda
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            gvComidas.PageIndex = 0; // Resetear la paginación al buscar
            CargarComidas();
        }

        // Manejador para el cambio de selección en el filtro de categoría
        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvComidas.PageIndex = 0; // Resetear la paginación al filtrar
            CargarComidas();
        }

        // Manejador para el cambio de página en el GridView


    }
}