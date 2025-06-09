using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;
using System.IO; // Necesario para FileStream, StreamReader
using System.Text.RegularExpressions; // Opcional, para validación avanzada

namespace AutoServicioCineWeb
{
    public partial class GestionPeliculas : System.Web.UI.Page
    {
        private readonly PeliculaWSClient peliculaServiceClient;
        private List<pelicula> _cachedPeliculas;

        public GestionPeliculas()
        {
            peliculaServiceClient = new PeliculaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPeliculas();
            }
            // Después de cualquier postback, asegurarnos de que la previsualización de imagen funcione
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReapplyImagePreview",
                "const txtImageUrlElement = document.getElementById('" + txtImagenUrl.ClientID + "'); if (txtImageUrlElement && txtImageUrlElement.value) { previewImage(txtImageUrlElement); }", true);
        }

        private void CargarPeliculas()
        {
            try
            {
                _cachedPeliculas = peliculaServiceClient.listarPeliculas().ToList();
                List<pelicula> peliculasFiltradas = FiltrarPeliculas(_cachedPeliculas);

                gvPeliculas.DataSource = peliculasFiltradas;
                gvPeliculas.DataBind();
            }
            catch (Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar películas: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar películas: " + ex.ToString());
            }
        }

        private List<pelicula> FiltrarPeliculas(List<pelicula> peliculas)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchPeliculas.Text))
            {
                string searchTerm = txtSearchPeliculas.Text.Trim().ToLower();
                peliculas = peliculas.Where(p =>
                    p.tituloEs.ToLower().Contains(searchTerm) ||
                    p.tituloEn.ToLower().Contains(searchTerm) ||
                    p.sinopsisEs.ToLower().Contains(searchTerm) ||
                    p.sinopsisEn.ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(ddlClasificacionFilter.SelectedValue))
            {
                string classificationFilter = ddlClasificacionFilter.SelectedValue;
                peliculas = peliculas.Where(p => p.clasificacion == classificationFilter).ToList();
            }
            return peliculas;
        }

        protected void gvPeliculas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPeliculas.PageIndex = e.NewPageIndex;
            CargarPeliculas();
        }

        protected void gvPeliculas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int peliculaId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditPelicula")
            {
                hdnPeliculaId.Value = peliculaId.ToString();
                litModalTitle.Text = "Editar Película";
                CargarDatosPeliculaParaEdicion(peliculaId);
                MostrarModalPelicula(); // Abre el modal de película
            }
            else if (e.CommandName == "DeletePelicula")
            {
                try
                {
                    peliculaServiceClient.eliminarPelicula(peliculaId);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionSuccess", "alert('Película eliminada exitosamente.');", true);


                    _cachedPeliculas = null; // Invalida la caché para que se vuelva a cargar
                    CargarPeliculas();
                }
                catch (Exception ex)
                {
                    // Similar al comentario anterior, este mensaje solo se verá si el modal de edición está abierto.
                    litMensajeModal.Text = $"Error al eliminar la película: {ex.Message}";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "DeletionError", "alert('Error al eliminar la película: " + ex.Message.Replace("'", "\\'") + "');", true);
                    System.Diagnostics.Debug.WriteLine("Error al eliminar película: " + ex.ToString());
                }
            }
        }

        private void CargarDatosPeliculaParaEdicion(int peliculaId)
        {
            try
            {
                pelicula peli = peliculaServiceClient.buscarPeliculaPorId(peliculaId);

                if (peli != null)
                {
                    txtTituloEs.Text = peli.tituloEs;
                    txtTituloEn.Text = peli.tituloEn;
                    txtDuracionMin.Text = peli.duracionMin.ToString();
                    ddlClasificacion.SelectedValue = peli.clasificacion;
                    txtSinopsisEs.Text = peli.sinopsisEs;
                    txtSinopsisEn.Text = peli.sinopsisEn;
                    chkEstaActiva.Checked = peli.estaActiva;
                    txtImagenUrl.Text = peli.imagenUrl;
                    hdnExistingImageUrl.Value = peli.imagenUrl;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PreviewImageOnLoad",
                        "previewImage(document.getElementById('" + txtImagenUrl.ClientID + "'));", true);
                }
                else
                {
                    litMensajeModal.Text = "Película no encontrada para edición.";
                }
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar datos de película para edición: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar datos de película para edición: " + ex.ToString());
            }
        }

        protected void btnGuardarPelicula_Click(object sender, EventArgs e)
        {
            Page.Validate("PeliculaValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                MostrarModalPelicula();
                return;
            }

            pelicula peli = new pelicula
            {
                peliculaId = Convert.ToInt32(hdnPeliculaId.Value),
                peliculaIdSpecified = true,
                tituloEs = txtTituloEs.Text,
                tituloEn = txtTituloEn.Text,
                duracionMin = Convert.ToInt32(txtDuracionMin.Text),
                clasificacion = ddlClasificacion.SelectedValue,
                sinopsisEs = txtSinopsisEs.Text,
                sinopsisEn = txtSinopsisEn.Text,
                estaActiva = chkEstaActiva.Checked,
                imagenUrl = txtImagenUrl.Text.Trim(),
                usuarioModificacion = 4,
                usuarioModificacionSpecified = true
            };

            try
            {
                if (peli.peliculaId == 0)
                {
                    peliculaServiceClient.registrarPelicula(peli);
                    litMensajeModal.Text = "Película agregada exitosamente.";
                }
                else
                {
                    peliculaServiceClient.actualizarPelicula(peli);
                    litMensajeModal.Text = "Película actualizada exitosamente.";
                }

                _cachedPeliculas = null;
                CargarPeliculas();
                OcultarModalPelicula();
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar la película: {ex.Message}";
                MostrarModalPelicula();
                System.Diagnostics.Debug.WriteLine("Error al guardar película: " + ex.ToString());
            }
        }

        // --- Métodos para manejar el modal de Película ---
        private void MostrarModalPelicula()
        {
            peliculaModal.Style["display"] = "flex";
        }

        private void OcultarModalPelicula()
        {
            peliculaModal.Style["display"] = "none";
            litMensajeModal.Text = "";
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hdnPeliculaId.Value = "0";
            litModalTitle.Text = "Agregar Nueva ";
            LimpiarCamposModalPelicula();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearValidators", "clearModalValidators();", true);
            MostrarModalPelicula();
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModalPelicula();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModalPelicula();
        }

        private void LimpiarCamposModalPelicula()
        {
            txtTituloEs.Text = "";
            txtTituloEn.Text = "";
            txtSinopsisEs.Text = "";
            txtSinopsisEn.Text = "";
            txtDuracionMin.Text = "";
            ddlClasificacion.SelectedValue = "";
            txtImagenUrl.Text = "";
            chkEstaActiva.Checked = true;

            imgPreview.ImageUrl = "";
            imgPreview.Style["display"] = "none";
            hdnExistingImageUrl.Value = "";
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

            int peliculasAgregadas = 0;
            int peliculasActualizadas = 0;
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
                    string[] requiredColumns = { "TituloEs", "TituloEn", "DuracionMin", "Clasificacion", "SinopsisEs", "SinopsisEn", "EstaActiva", "ImagenUrl" };
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

                        pelicula peli = new pelicula();

                        try
                        {

                            peli.tituloEs = GetCsvValue(data, headerMap, "TituloEs");
                            peli.tituloEn = GetCsvValue(data, headerMap, "TituloEn");

                            int duracion;
                            if (int.TryParse(GetCsvValue(data, headerMap, "DuracionMin"), out duracion))
                            {
                                peli.duracionMin = duracion;
                            }
                            else
                            {
                                throw new FormatException("Duración (DuracionMin) no es un número válido.");
                            }

                            peli.clasificacion = GetCsvValue(data, headerMap, "Clasificacion");
                            peli.sinopsisEs = GetCsvValue(data, headerMap, "SinopsisEs");
                            peli.sinopsisEn = GetCsvValue(data, headerMap, "SinopsisEn");

                            bool estaActiva;
                            if (bool.TryParse(GetCsvValue(data, headerMap, "EstaActiva"), out estaActiva))
                            {
                                peli.estaActiva = estaActiva;
                            }
                            else
                            {
                                // Intentar parsear "TRUE"/"FALSE" o "1"/"0" si bool.TryParse falla directamente
                                string activeString = GetCsvValue(data, headerMap, "EstaActiva").ToLower().Trim();
                                if (activeString == "true" || activeString == "1") peli.estaActiva = true;
                                else if (activeString == "false" || activeString == "0") peli.estaActiva = false;
                                else throw new FormatException("EstaActiva no es un valor booleano válido (TRUE/FALSE, 1/0).");
                            }

                            peli.imagenUrl = GetCsvValue(data, headerMap, "ImagenUrl");
                            peli.usuarioModificacion = 4; // Usuario fijo para la carga
                            peli.usuarioModificacionSpecified = true;

                            if (peli.peliculaId == 0)
                            {
                                peliculaServiceClient.registrarPelicula(peli);
                                peliculasAgregadas++;
                            }
                            else
                            {
                                peliculaServiceClient.actualizarPelicula(peli);
                                peliculasActualizadas++;
                            }
                        }
                        catch (Exception exLinea)
                        {
                            errores++;
                            System.Diagnostics.Debug.WriteLine($"Error procesando línea CSV: {line}. Error: {exLinea.Message}");
                        }
                    }
                }

                _cachedPeliculas = null; // Invalida la caché
                CargarPeliculas(); // Recargar la tabla y estadísticas

                litMensajeCsvModal.Text = $"Carga CSV completada: {peliculasAgregadas} agregadas, {peliculasActualizadas} actualizadas, {errores} con errores.";
                OcultarModalCsv(); // Cerrar el modal después de la carga exitosa (o con resumen de errores)
            }
            catch (Exception ex)
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
        private List<pelicula> GetCachedPeliculas()
        {
            if (_cachedPeliculas == null)
            {
                try
                {
                    _cachedPeliculas = peliculaServiceClient.listarPeliculas().ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener películas para estadísticas: " + ex.ToString());
                    _cachedPeliculas = new List<pelicula>();
                }
            }
            return _cachedPeliculas;
        }

        public int GetTotalPeliculas()
        {
            return GetCachedPeliculas().Count;
        }

        public int GetPeliculasActivas()
        {
            return GetCachedPeliculas().Count(p => p.estaActiva);
        }

        public int GetPeliculasInactivas()
        {
            return GetCachedPeliculas().Count(p => !p.estaActiva);
        }

        public int GetClasificacionesUnicas()
        {
            return GetCachedPeliculas().Select(p => p.clasificacion).Distinct().Count();
        }

        protected void txtSearchPeliculas_TextChanged(object sender, EventArgs e)
        {
            gvPeliculas.PageIndex = 0;
            CargarPeliculas();
        }

        protected void ddlClasificacionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPeliculas.PageIndex = 0;
            CargarPeliculas();
        }
    }
}