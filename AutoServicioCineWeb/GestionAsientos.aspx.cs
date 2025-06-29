using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class GestionAsientos : System.Web.UI.Page
    {
        private readonly AsientoWSClient asientoServiceClient;
        private List<asiento> _cachedAsientos;
        private int salaId;
        private int idUsuario = 34;//base
        public GestionAsientos()
        {            
            asientoServiceClient = new AsientoWSClient();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["SalaId"] != null)
                {
                    salaId = Convert.ToInt32(Request.QueryString["SalaId"]);
                    // Usar salaId para cargar asientos o lo que necesites
                    hfSalaId.Value = salaId.ToString();
                    cargarAsientos(salaId);
                    if (Context.User.Identity.IsAuthenticated)
                    {
                        FormsIdentity id = (FormsIdentity)Context.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;
                        string userData = ticket.UserData;
                        string[] userInfo = userData.Split('|');
                        idUsuario = int.Parse(userInfo[0]); // Asumiendo que el ID de usuario es el primer elemento
                    }
                }
                else
                {
                    cargarDefecto();
                }

            }
            else
            {
                // Asegúrate de que el valor no esté vacío (por ejemplo, si se perdió)
                if (string.IsNullOrEmpty(hfSalaId.Value) && Request.QueryString["SalaId"] != null)
                {
                    hfSalaId.Value = Request.QueryString["SalaId"];
                }
            }

        }
        private void cargarDefecto()
        {
            try
            {
                _cachedAsientos = asientoServiceClient.listarAsientos().ToList();
                List<asiento> asientosFiltrados = FiltrarAsientos(_cachedAsientos);

                gvAsientos.DataSource = asientosFiltrados;
                gvAsientos.DataBind();
            }
            catch (System.Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar asientos: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar asientos: " + ex.ToString());
            }
        }
        private void cargarAsientos(int salaID)
        {
            try
            {
                _cachedAsientos = asientoServiceClient.listarAsientosSala(salaID).ToList();
                List<asiento> asientosFiltrados = FiltrarAsientos(_cachedAsientos);

                gvAsientos.DataSource = asientosFiltrados;
                gvAsientos.DataBind();
            }
            catch (System.Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar asientos: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar asientos: " + ex.ToString());
            }
        }
        private List<asiento> FiltrarAsientos(List<asiento> asientos)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchAsientos.Text))
            {
                string searchTerm = txtSearchAsientos.Text.Trim().ToLower();
                asientos = asientos.Where(p =>
                    p.numero.ToString().Contains(searchTerm)||
                    p.fila.ToString().ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(ddlTipoFilter.SelectedValue))
            {
                string classificationFilter = ddlTipoFilter.SelectedValue;
                asientos = asientos.Where(p => p.tipo.ToString() == classificationFilter).ToList();
            }
            return asientos;
        }
        protected void gvAsientos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAsientos.PageIndex = e.NewPageIndex;    
            cargarAsientos(salaId);
        }
        protected void gvAsientos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAsiento")
            {
                hdnAsientoId.Value = Convert.ToString(e.CommandArgument);
                litModalTitle.Text = "Editar Asiento";
                CargarDatosAsientoParaEdicion(Convert.ToInt32(e.CommandArgument));
                MostrarModalAsiento(); // Abre el modal de asiento
            }
            else if (e.CommandName == "DeleteAsiento")
            {
                try
                {
                    asientoServiceClient.eliminarAsiento(Convert.ToInt32(e.CommandArgument), idUsuario);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionSuccess", "alert('Asiento eliminado exitosamente.');", true);


                    _cachedAsientos = null; // Invalida la caché para que se vuelva a cargar
                    cargarAsientos(salaId); // Cargar asientos después de la eliminación
                }
                catch (System.Exception ex)
                {
                    // Similar al comentario anterior, este mensaje solo se verá si el modal de edición está abierto.
                    litMensajeModal.Text = $"Error al eliminar el asiento: {ex.Message}";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionError", "alert('Error al eliminar el asiento: " + ex.Message.Replace("'", "\\'") + "');", true);
                    System.Diagnostics.Debug.WriteLine("Error al eliminar asiento: " + ex.ToString());
                }
            }
        }
        private void CargarDatosAsientoParaEdicion(int asientoId)
        {
            try
            {
                asiento asiento = asientoServiceClient.buscarAsientoPorId(asientoId);

                if (asiento != null)
                {
                    Fila.Text = asiento.fila.ToString();
                    Numero.Text = asiento.numero.ToString();
                    ddlTipoAsiento.SelectedValue = asiento.tipo.ToString();
                    chkEstaActiva.Checked = asiento.activo;
                }
                else
                {
                    litMensajeModal.Text = "Asiento no encontrada para edición.";
                }
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = "Error al cargar datos de asiento para edición: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar datos de asiento para edición: " + ex.ToString());
            }
        }
        protected void btnGuardarAsiento_Click(object sender, EventArgs e)
        {
            Page.Validate("AsientoValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                MostrarModalAsiento();
                return;
            }
            salaId = int.Parse(hfSalaId.Value);
            asiento asiento = new asiento
            {
                id = int.Parse(hdnAsientoId.Value),
                idSpecified = true,
                sala = new sala
                {
                    id = salaId,
                    idSpecified = true
                },
                fila = (ushort)Fila.Text.ToCharArray()[0],
                numero = int.Parse(Numero.Text),
                numeroSpecified = true,
                tipo = (tipoAsiento)Enum.Parse(typeof(tipoAsiento), ddlTipoAsiento.SelectedValue, ignoreCase: true),
                tipoSpecified = true,                
                activo = chkEstaActiva.Checked,
                usuarioModificacion = idUsuario, // Asignar un usuario de modificación por defecto, esto debería ser dinámico en una aplicación real
                usuarioModificacionSpecified = true

            };

            try
            {
                if (asiento.id == 0)
                {
                    asientoServiceClient.registrarAsiento(asiento);
                    litMensajeModal.Text = "asiento agregado exitosamente.";
                }
                else
                {
                    asientoServiceClient.actualizarAsiento(asiento);
                    litMensajeModal.Text = "asiento actualizado exitosamente.";
                }

                _cachedAsientos = null;
                
                cargarAsientos(salaId);
                OcultarModalAsiento();
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar el asiento: {ex.Message}";
                MostrarModalAsiento();
                System.Diagnostics.Debug.WriteLine("Error al guardar asiento: " + ex.ToString());
            }
        }
        private void MostrarModalAsiento()
        {
            asientoModal.Style["display"] = "flex";
        }
        private void OcultarModalAsiento()
        {
            asientoModal.Style["display"] = "none";
            litMensajeModal.Text = "";
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hdnAsientoId.Value = "0";
            litModalTitle.Text = "Agregar Nueva ";
            LimpiarCamposModalAsiento();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearValidators", "clearModalValidators();", true);
            MostrarModalAsiento();
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModalAsiento();
        }
        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModalAsiento();
        }

        private void LimpiarCamposModalAsiento()
        {
            Fila.Text = string.Empty;
            Numero.Text = string.Empty;
            ddlTipoAsiento.SelectedIndex = 0;
            chkEstaActiva.Checked = false;
        }

        // --- NUEVOS Métodos para Carga de CSV ---
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

            int asientosAgregados = 0;
            int asientosActualizados = 0;
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
                    string[] requiredColumns = { "Fila", "Numero", "TipoAsiento", "Activo" };
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

                        asiento asiento =new asiento();

                        try
                        {
                            string filaStr = GetCsvValue(data, headerMap, "Fila");

                            if (!string.IsNullOrEmpty(filaStr) && filaStr.Length == 1)
                            {
                                asiento.fila = (ushort)filaStr[0]; // Convertir char a ushort
                            }
                            else
                            {
                                throw new FormatException("El valor de Fila debe ser un solo carácter.");
                            }
                            int numero;
                            if (int.TryParse(GetCsvValue(data, headerMap, "Numero"), out numero))
                            {
                                asiento.numero = numero;
                            }
                            else
                            {
                                throw new FormatException("Número no es un número válido.");
                            }
                            asiento.tipo = (tipoAsiento)Enum.Parse(typeof(tipoAsiento), GetCsvValue(data, headerMap, "TipoAsiento"), ignoreCase: true);

                            bool activo;
                            if (bool.TryParse(GetCsvValue(data, headerMap, "Activo"), out activo))
                            {
                                asiento.activo = activo;
                            }
                            else
                            {
                                // Intentar parsear "TRUE"/"FALSE" o "1"/"0" si bool.TryParse falla directamente
                                string activeString = GetCsvValue(data, headerMap, "Activo").ToLower().Trim();
                                if (activeString == "true" || activeString == "1") asiento.activo = true;
                                else if (activeString == "false" || activeString == "0") asiento.activo = false;
                                else throw new FormatException("Activo no es un valor booleano válido (TRUE/FALSE, 1/0).");
                            }
                            asiento.usuarioModificacion = 4; //
                            asiento.usuarioModificacionSpecified = true;

                            if (asiento.id == 0)
                            {
                                asientoServiceClient.registrarAsiento(asiento);
                                asientosAgregados++;
                            }
                            else
                            {
                                asientoServiceClient.actualizarAsiento(asiento);
                                asientosActualizados++;
                            }
                        }
                        catch (System.Exception exLinea)
                        {
                            errores++;
                            System.Diagnostics.Debug.WriteLine($"Error procesando línea CSV: {line}. Error: {exLinea.Message}");
                        }
                    }
                }

                _cachedAsientos = null; // Invalida la caché
                salaId = int.Parse(hfSalaId.Value);
                cargarAsientos(salaId); // Recargar la tabla y estadísticas

                litMensajeCsvModal.Text = $"Carga CSV completada: {asientosAgregados} agregadas, {asientosActualizados} actualizadas, {errores} con errores.";
                OcultarModalCsv(); // Cerrar el modal después de la carga exitosa (o con resumen de errores)
            }
            catch (System.Exception ex)
            {
                litMensajeCsvModal.Text = $"Error general al procesar el archivo CSV: {ex.Message}";
                System.Diagnostics.Debug.WriteLine("Error al procesar CSV: " + ex.ToString());
                MostrarModalCsv(); // Mantener el modal abierto si hay un error grave
            }
        }
        // Helper para obtener valor de CSV por nombre de columna
        private string GetCsvValue(string[] data, Dictionary<string, int> headerMap, string columnName)
        {
            if (headerMap.ContainsKey(columnName) && headerMap[columnName] < data.Length)
            {
                return data[headerMap[columnName]].Trim();
            }
            return string.Empty;
        }

        // --- Métodos para estadísticas (optimizados para usar la caché) ---
        private List<asiento> GetCachedAsientos()
        {
            if (_cachedAsientos == null)
            {
                try
                {
                    _cachedAsientos = asientoServiceClient.listarAsientosSala(salaId).ToList();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener asientos para estadísticas: " + ex.ToString());
                    _cachedAsientos = new List<asiento>();
                }
            }
            return _cachedAsientos;
        }
        public int GetTotalAsientos()
        {
            return GetCachedAsientos().Count;
        }
        public int GetAsientosActivos()
        {
            return GetCachedAsientos().Count(p => p.activo);
        }

        public int GetAsientosInactivos()
        {
            return GetCachedAsientos().Count(p => !p.activo);
        }
        protected void txtSearchAsientos_TextChanged(object sender, EventArgs e)
        {
            gvAsientos.PageIndex = 0;
            salaId = int.Parse(hfSalaId.Value);
            cargarAsientos(salaId);
        }

        protected void ddlTipoFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvAsientos.PageIndex = 0;
            salaId = int.Parse(hfSalaId.Value);
            cargarAsientos(salaId);
        }

    }
}