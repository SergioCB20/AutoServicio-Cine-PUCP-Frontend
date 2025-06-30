using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    /// <summary>
    /// Página para gestionar las salas del cine.
    /// Permite agregar, editar y listar salas.
    /// </summary>
    public partial class GestionSalas : System.Web.UI.Page
    {
        private readonly SalaWSClient salaServiceClient;
        private List<sala> _cachedSalas;
        private int idUsuario = 34; //Por defecto
        public GestionSalas()
        {
            // Inicializa el cliente del servicio SOAP para interactuar con las salas
            salaServiceClient = new SalaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarSalas();
                if (Context.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)Context.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    string userData = ticket.UserData;
                    string[] userInfo = userData.Split('|');
                    idUsuario = int.Parse(userInfo[0]); // Asumiendo que el ID de usuario es el primer elemento
                }
            }
            /*ScriptManager.RegisterStartupScript(this, this.GetType(), "ReapplyImagePreview",
                "const txtImageUrlElement = document.getElementById('" + txtImagenUrl.ClientID + "'); if (txtImageUrlElement && txtImageUrlElement.value) { previewImage(txtImageUrlElement); }", true);*/
        }
        private void CargarSalas()
        {
            try
            {
                _cachedSalas = salaServiceClient.listarSalas().ToList();
                List<sala> salasFiltradas = FiltrarSalas(_cachedSalas);

                gvSalas.DataSource = salasFiltradas;
                gvSalas.DataBind();
            }
            catch (System.Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar salas: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar salas: " + ex.ToString());
            }
        }
        private List<sala> FiltrarSalas(List<sala> salas)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchSalas.Text))
            {
                string searchTerm = txtSearchSalas.Text.Trim().ToLower();
                salas = salas.Where(p =>
                    p.nombre.ToLower().Contains(searchTerm)                    
                ).ToList();
            }
            
             
            if (!string.IsNullOrEmpty(ddlTipoFilter.SelectedValue))
            { 
                string classificationFilter = ddlTipoFilter.SelectedValue;
                if (Enum.TryParse(classificationFilter, out tipoSala classificationFilterEnum))
                {
                    salas = salas.Where(p => p.tipoSala == classificationFilterEnum).ToList();
                }
            }
            return salas;
        }
        protected void gvSalas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSalas.PageIndex = e.NewPageIndex;
            CargarSalas();
        }

        protected void gvSalas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int SalaId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditSala":
                    hdnSalaId.Value = SalaId.ToString();
                    litModalTitle.Text = "Editar Sala";
                    CargarDatosSalaParaEdicion(SalaId);
                    MostrarModalSala(); // Abre el modal de película
                    break;
                case "DeleteSala":
                    {
                        try
                        {
                            salaServiceClient.eliminarSala(SalaId,idUsuario);
                            ScriptManager.RegisterStartupScript(this, GetType(), "DeletionSuccess", "alert('Sala eliminada exitosamente.');", true);


                            _cachedSalas = null; // Invalida la caché para que se vuelva a cargar
                            CargarSalas();
                        }
                        catch (System.Exception ex)
                        {
                            // Similar al comentario anterior, este mensaje solo se verá si el modal de edición está abierto.
                            litMensajeModal.Text = $"Error al eliminar la sala: {ex.Message}";
                            ScriptManager.RegisterStartupScript(this, GetType(), "DeletionError", "alert('Error al eliminar la sala: " + ex.Message.Replace("'", "\\'") + "');", true);
                            System.Diagnostics.Debug.WriteLine("Error al eliminar sala: " + ex.ToString());
                        }

                        break;
                    }

                case "EditAsiento":
                    Response.Redirect($"GestionAsientos.aspx?SalaId={SalaId}");
                    break;
            }
        }
        private void CargarDatosSalaParaEdicion(int salaId)
        {
            try
            {
                sala sala = salaServiceClient.buscarSalaPorId(salaId);

                if (sala != null)
                {
                     Nombre.Text = sala.nombre;
                     Capacidad.Text = sala.capacidad.ToString();
                     ddlTipoSala.SelectedValue = sala.tipoSala.ToString();
                     chkEstaActiva.Checked = sala.activa;


                     /*ScriptManager.RegisterStartupScript(this, this.GetType(), "PreviewImageOnLoad",
                         "previewImage(document.getElementById('" + txtImagenUrl.ClientID + "'));", true);*/
                }
                else
                {
                    litMensajeModal.Text = "Sala no encontrada para edición.";
                }
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = "Error al cargar datos de sala para edición: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar datos de sala para edición: " + ex.ToString());
            }
        }
        protected void btnGuardarSala_Click(object sender, EventArgs e)
        {
            Page.Validate("SalaValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                MostrarModalSala();
                return;
            }
            
            sala sala = new sala
            {
                 id = int.Parse(hdnSalaId.Value),
                 idSpecified = true,
                 nombre = Nombre.Text,
                 capacidad = int.Parse(Capacidad.Text),
                 capacidadSpecified = true,
                 tipoSala = (tipoSala)Enum.Parse(typeof(tipoSala), ddlTipoSala.SelectedValue, ignoreCase: true),
                 tipoSalaSpecified=true,
                 activa = chkEstaActiva.Checked,
                 usuarioModificacion = idUsuario,
                 usuarioModificacionSpecified = true,
                 fechaModificacionSpecified = true

            };

            try
            {
                if (sala.id == 0)
                {
                    salaServiceClient.registrarSala(sala);
                    litMensajeModal.Text = "sala agregada exitosamente.";
                }
                else
                {
                    salaServiceClient.actualizarSala(sala);
                    litMensajeModal.Text = "sala actualizada exitosamente.";
                }

                _cachedSalas = null;
                CargarSalas();
                OcultarModalSala();
            }
            catch (System.Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar la sala: {ex.Message}";
                MostrarModalSala();
                System.Diagnostics.Debug.WriteLine("Error al guardar sala: " + ex.ToString());
            }
        }
        private void MostrarModalSala()
        {
            salaModal.Style["display"] = "flex";
        }

        private void OcultarModalSala()
        {
            salaModal.Style["display"] = "none";
            litMensajeModal.Text = "";
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hdnSalaId.Value = "0";
            litModalTitle.Text = "Agregar Nueva ";
            LimpiarCamposModalSala();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearValidators", "clearModalValidators();", true);
            MostrarModalSala();
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModalSala();
        }
        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModalSala();
        }

        private void LimpiarCamposModalSala()
        {
            Nombre.Text = string.Empty;
            Capacidad.Text = string.Empty;
            ddlTipoSala.SelectedIndex = 0;
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

            int salasAgregadas = 0;
            int salasActualizadas = 0;
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
                    string[] requiredColumns = { "Nombre", "Capacidad", "TipoSala", "EstaActiva" };
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

                        sala sala = new sala();

                        try
                        {
                            sala.nombre = GetCsvValue(data, headerMap, "Nombre");
                            int capacidad;
                            if (int.TryParse(GetCsvValue(data, headerMap, "Capacidad"), out capacidad))
                            {
                                sala.capacidad = capacidad;
                                ;
                            }
                            else
                            {
                                throw new FormatException("Capacidad no es un número válido.");
                            }
                            /*
                            tipoProducto tipoprod;
                            if (Enum.TryParse(GetCsvValue(data, headerMap, "precio"), out tipoprod))
                            {
                                prod.tipo = tipoprod;
                            }
                            else
                            {
                                throw new FormatException("Tipo no es válido.");
                            }
                             */
                            tipoSala tiposala;
                            if (Enum.TryParse(GetCsvValue(data, headerMap, "TipoSala"), out tiposala))
                            {                                
                                sala.tipoSala = tiposala;
                            }
                            else
                            {
                                 throw new FormatException("TipoSala no es válido.");
                            }                              
                            bool estaActiva;
                            if (bool.TryParse(GetCsvValue(data, headerMap, "EstaActiva"), out estaActiva))
                            {
                                sala.activa = estaActiva;
                            }
                            else
                            {
                                // Intentar parsear "TRUE"/"FALSE" o "1"/"0" si bool.TryParse falla directamente
                                string activeString = GetCsvValue(data, headerMap, "EstaActiva").ToLower().Trim();
                                if (activeString == "true" || activeString == "1") sala.activa = true;
                                else if (activeString == "false" || activeString == "0") sala.activa = false;
                                else throw new FormatException("EstaActiva no es un valor booleano válido (TRUE/FALSE, 1/0).");
                            }
                            sala.usuarioModificacion = idUsuario; //
                            sala.usuarioModificacionSpecified = true; 

                            if (sala.id == 0)
                            {
                                salaServiceClient.registrarSala(sala);
                                salasAgregadas++;
                            }
                            else
                            {
                                salaServiceClient.actualizarSala(sala);
                                salasActualizadas++;
                            }
                        }
                        catch (System.Exception exLinea)
                        {
                            errores++;
                            System.Diagnostics.Debug.WriteLine($"Error procesando línea CSV: {line}. Error: {exLinea.Message}");
                        }
                    }
                }

                _cachedSalas = null; // Invalida la caché
                CargarSalas(); // Recargar la tabla y estadísticas

                litMensajeCsvModal.Text = $"Carga CSV completada: {salasAgregadas} agregadas, {salasActualizadas} actualizadas, {errores} con errores.";
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
        private List<sala> GetCachedSalas()
        {
            if (_cachedSalas == null)
            {
                try
                {
                    _cachedSalas = salaServiceClient.listarSalas().ToList();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener salas para estadísticas: " + ex.ToString());
                    _cachedSalas = new List<sala>();
                }
            }
            return _cachedSalas;
        }

        public int GetTotalSalas()
        {
            return GetCachedSalas().Count;
        }
        public int GetSalasActivas()
        {
            return GetCachedSalas().Count(p => p.activa);
        }

        public int GetSalasInactivas()
        {
            return GetCachedSalas().Count(p => !p.activa);
        }

        public int GetClasificacionesUnicas()
        {
            return GetCachedSalas().Select(p => p).Distinct().Count();
        }

        protected void txtSearchSalas_TextChanged(object sender, EventArgs e)
        {
            gvSalas.PageIndex = 0;
            CargarSalas();
        }

        protected void ddlTipoFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvSalas.PageIndex = 0;
            CargarSalas();
        }

    }
}