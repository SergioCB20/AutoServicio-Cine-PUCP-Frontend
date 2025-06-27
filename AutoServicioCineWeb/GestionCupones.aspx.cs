using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class GestionCupones : System.Web.UI.Page
    {
        private readonly CuponWSClient cuponWS;
        private List<cupon> _cachedCupones;

        public GestionCupones()
        {
            cuponWS = new CuponWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCupones();
                ActualizarEstadisticas();
            }
        }

        private void CargarCupones()
        {
            try
            {
                var cupones = cuponWS.listarCupones();
                _cachedCupones = cupones?.ToList() ?? new List<cupon>();
                rptCupones.DataSource = _cachedCupones;
                rptCupones.DataBind();
            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al cargar cupones: " + ex.Message, "error");
            }
        }

        private void ActualizarEstadisticas()
        {
            try
            {
                var cupones = GetCachedCupones();
                int total = cupones.Count;
                int activos = cupones.Count(c => c.activo && Convert.ToDateTime(c.fechaFin) >= DateTime.Now);
                int inactivos = total - activos;

                // Actualizar estadísticas via JavaScript
                string script = $@"
                    document.getElementById('totalProducts').textContent = '{total}';
                    document.getElementById('availableProducts').textContent = '{activos}';
                    document.getElementById('soldOutProducts').textContent = '{inactivos}';
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "updateStats", script, true);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al actualizar estadísticas: " + ex.ToString());
            }
        }

        protected void btnNuevoCupon_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblFormTitle.Text = "Registrar Nuevo Cupón";
            hiddenCuponId.Value = "";
            formContainer.Visible = true;

            // Establecer fecha mínima como hoy
            txtFechaInicio.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaFin.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
            ddlDescuentoTipo.SelectedIndex = 0;
            MostrarModalCupon();
        }

        private void MostrarModalCupon()
        {
            formContainer.Style["display"] = "flex";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidarFechas())
            {
                try
                {
                    var cupon = CrearCuponDesdeFormulario();

                    string stringIni = txtFechaInicio.Text;
                    string stringFin = txtFechaFin.Text;
                    usuario usu = new usuario { id = 11 };
                    cupon.cuponIdSpecified = true;
                    cupon.descuentoTipoSpecified = true;
                    cupon.descuentoValorSpecified = true;
                    cupon.maxUsosSpecified = true;
                    cupon.usosActualesSpecified = true;
                    cupon.creadoPor = usu;

                    if (string.IsNullOrEmpty(hiddenCuponId.Value))
                    {
                        // Nuevo cupón
                        cuponWS.registrarCupon(cupon, stringIni, stringFin);
                        MostrarMensaje("Cupón registrado exitosamente", "success");
                    }
                    else
                    {
                        // Actualizar cupón existente
                        cupon.cuponId = Convert.ToInt32(hiddenCuponId.Value);
                        cuponWS.actualizarCupon(cupon);
                        MostrarMensaje("Cupón actualizado exitosamente", "success");
                    }

                    formContainer.Visible = false;
                    _cachedCupones = null; // Invalidar caché
                    CargarCupones();
                    ActualizarEstadisticas();
                }
                catch (System.Exception ex)
                {
                    MostrarMensaje("Error al guardar cupón: " + ex.Message, "error");
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            formContainer.Visible = false;
            LimpiarFormulario();
        }

        protected void rptCupones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int cuponId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "Editar":
                    EditarCupon(cuponId);
                    break;
                case "Eliminar":
                    EliminarCupon(cuponId);
                    break;
            }
        }

        private void EditarCupon(int cuponId)
        {
            try
            {
                var cupon = cuponWS.buscarCuponPorId(cuponId);
                if (cupon != null)
                {
                    CargarDatosEnFormulario(cupon);
                    lblFormTitle.Text = "Editar Cupón";
                    hiddenCuponId.Value = cupon.cuponId.ToString();
                    formContainer.Visible = true;
                    MostrarModalCupon();
                }
            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al cargar cupón: " + ex.Message, "error");
            }
        }

        private void EliminarCupon(int cuponId)
        {
            try
            {
                cuponWS.eliminarCupon(cuponId);
                MostrarMensaje("Cupón eliminado exitosamente", "success");
                _cachedCupones = null; // Invalidar caché
                CargarCupones();
                ActualizarEstadisticas();
            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al eliminar cupón: " + ex.Message, "error");
            }
        }

        private cupon CrearCuponDesdeFormulario()
        {
            return new cupon
            {
                codigo = txtCodigo.Text.Trim().ToUpper(),
                descripcionEs = txtDescripcion.Text.Trim(),
                descripcionEn = txtDescripcion.Text.Trim(),
                descuentoTipo = (tipoDescuento)Enum.Parse(typeof(tipoDescuento), ddlDescuentoTipo.SelectedValue, true),
                descuentoTipoSpecified = true,
                descuentoValor = double.Parse(txtPorcentajeDescuento.Text),
                descuentoValorSpecified = true,
                fechaInicio = null,
                fechaFin = null,
                maxUsos = int.Parse(txtCantidadMaxima.Text),
                maxUsosSpecified = true,
                activo = chkActivo.Checked,
                usosActuales = 0,
                usosActualesSpecified = true
            };
        }

        private void CargarDatosEnFormulario(cupon cupon)
        {
            txtCodigo.Text = cupon.codigo;
            txtDescripcion.Text = cupon.descripcionEs;
            ddlDescuentoTipo.SelectedValue = cupon.descuentoTipo.ToString().ToLower();
            txtPorcentajeDescuento.Text = cupon.descuentoValor.ToString();
            txtCantidadMaxima.Text = cupon.maxUsos.ToString();

            // Manejar fechas de manera segura
            if (cupon.fechaInicio != null)
            {
                txtFechaInicio.Text = FormatDateForInput(cupon.fechaInicio);
            }
            if (cupon.fechaFin != null)
            {
                txtFechaFin.Text = FormatDateForInput(cupon.fechaFin);
            }

            chkActivo.Checked = cupon.activo;
        }

        private string FormatDateForInput(object date)
        {
            if (date == null) return "";

            if (date is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }

            if (DateTime.TryParse(date.ToString(), out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }

            return "";
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            txtPorcentajeDescuento.Text = "";
            txtCantidadMaxima.Text = "";
            txtFechaInicio.Text = "";
            txtFechaFin.Text = "";
            chkActivo.Checked = true;
            ddlDescuentoTipo.SelectedIndex = 0;
            hiddenCuponId.Value = "";
        }

        private bool ValidarFechas()
        {
            if (!DateTime.TryParse(txtFechaInicio.Text, out DateTime fechaInicio) ||
                !DateTime.TryParse(txtFechaFin.Text, out DateTime fechaFin))
            {
                MostrarMensaje("Formato de fecha inválido", "warning");
                return false;
            }

            if (fechaInicio >= fechaFin)
            {
                MostrarMensaje("La fecha de fin debe ser posterior a la fecha de inicio", "warning");
                return false;
            }

            if (fechaInicio < DateTime.Now.Date)
            {
                MostrarMensaje("La fecha de inicio no puede ser anterior a hoy", "warning");
                return false;
            }

            return true;
        }

        // Métodos helper para el Repeater
        protected string FormatDiscount(object tipo, object valor)
        {
            if (tipo == null || valor == null) return "N/A";

            string tipoStr = tipo.ToString().ToLower();
            double valorNum = Convert.ToDouble(valor);

            if (tipoStr == "porcentaje")
            {
                return $"{valorNum}%";
            }
            else
            {
                return $"S/ {valorNum:F2}";
            }
        }

        protected string FormatDateString(object date)
        {
            if (date == null) return "N/A";

            if (DateTime.TryParse(date.ToString(), out DateTime parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy");
            }

            return "N/A";
        }

        protected string GetUsageText(object usosActuales, object maxUsos)
        {
            if (usosActuales == null || maxUsos == null) return "0/0";

            int actual = Convert.ToInt32(usosActuales);
            int maximo = Convert.ToInt32(maxUsos);

            return $"{actual}/{maximo}";
        }

        protected string GetUsageProgressStyle(object usosActuales, object maxUsos)
        {
            if (usosActuales == null || maxUsos == null) return "width: 0%";

            int actual = Convert.ToInt32(usosActuales);
            int maximo = Convert.ToInt32(maxUsos);

            if (maximo == 0) return "width: 0%";

            double porcentaje = (double)actual / maximo * 100;
            return $"width: {Math.Min(porcentaje, 100)}%";
        }

        protected string GetStatusClass(object activo, object fechaFin)
        {
            bool esActivo = Convert.ToBoolean(activo);

            if (!esActivo)
                return "status-badge status-inactive";

            if (fechaFin != null && DateTime.TryParse(fechaFin.ToString(), out DateTime fecha))
            {
                if (fecha < DateTime.Now)
                    return "status-badge status-expired";
            }

            return "status-badge status-active";
        }

        protected string GetStatusText(object activo, object fechaFin)
        {
            bool esActivo = Convert.ToBoolean(activo);

            if (!esActivo)
                return "Inactivo";

            if (fechaFin != null && DateTime.TryParse(fechaFin.ToString(), out DateTime fecha))
            {
                if (fecha < DateTime.Now)
                    return "Expirado";
            }

            return "Activo";
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string script = $"showNotification('{mensaje.Replace("'", "\\'")}', '{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "notification", script, true);
        }

        // --- Métodos para Carga de CSV ---
        private void MostrarModalCsv()
        {
            csvUploadModal.Style["display"] = "flex";
            litMensajeCsvModal.Text = "";
        }

        private void OcultarModalCsv()
        {
            csvUploadModal.Style["display"] = "none";
            litMensajeCsvModal.Text = "";
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

        protected void cvCsvFileExtension_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (FileUploadCsv.HasFile)
            {
                string fileExtension = Path.GetExtension(FileUploadCsv.FileName).ToLower();
                args.IsValid = (fileExtension == ".csv");
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnUploadCsv_Click(object sender, EventArgs e)
        {
            Page.Validate("CsvUploadValidation");

            if (!Page.IsValid)
            {
                litMensajeCsvModal.Text = "Por favor, corrige los errores en el formulario de carga CSV.";
                MostrarModalCsv();
                return;
            }

            if (!FileUploadCsv.HasFile)
            {
                litMensajeCsvModal.Text = "Por favor, selecciona un archivo CSV para subir.";
                MostrarModalCsv();
                return;
            }

            string fileExtension = Path.GetExtension(FileUploadCsv.FileName).ToLower();
            if (fileExtension != ".csv")
            {
                litMensajeCsvModal.Text = "Formato de archivo no válido. Por favor, sube un archivo CSV.";
                MostrarModalCsv();
                return;
            }

            int cuponesAgregados = 0;
            int errores = 0;
            List<string> mensajesError = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(FileUploadCsv.PostedFile.InputStream))
                {
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        litMensajeCsvModal.Text = "El archivo CSV está vacío o no tiene cabecera.";
                        MostrarModalCsv();
                        return;
                    }

                    var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();
                    var headerMap = new Dictionary<string, int>();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        headerMap[headers[i]] = i;
                    }

                    string[] requiredColumns = {
                        "Codigo", "Descripcion", "TipoDescuento", "ValorDescuento",
                        "CantidadMaxima", "FechaInicio", "FechaFin", "EstaActivo"
                    };

                    foreach (var col in requiredColumns)
                    {
                        if (!headerMap.ContainsKey(col))
                        {
                            litMensajeCsvModal.Text = $"Error: La columna '{col}' es requerida en el CSV.";
                            MostrarModalCsv();
                            return;
                        }
                    }

                    int lineNumber = 1; // Para tracking de errores
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split(',');

                        try
                        {
                            // Validar y crear cupón desde CSV
                            var cupon = CrearCuponDesdeCsv(values, headerMap, lineNumber, mensajesError);
                            if (cupon != null)
                            {
                                // Configurar campos requeridos
                                usuario usu = new usuario { id = 11 };
                                cupon.cuponIdSpecified = true;
                                cupon.descuentoTipoSpecified = true;
                                cupon.descuentoValorSpecified = true;
                                cupon.maxUsosSpecified = true;
                                cupon.usosActualesSpecified = true;
                                cupon.creadoPor = usu;
                                cupon.usosActuales = 0;

                                // Obtener fechas como strings
                                string fechaInicioStr = GetCsvValue(values, headerMap, "FechaInicio");
                                string fechaFinStr = GetCsvValue(values, headerMap, "FechaFin");

                                // Registrar cupón
                                cuponWS.registrarCupon(cupon, fechaInicioStr, fechaFinStr);
                                cuponesAgregados++;
                            }
                            else
                            {
                                errores++;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            errores++;
                            mensajesError.Add($"Línea {lineNumber}: {ex.Message}");
                        }
                    }

                    _cachedCupones = null; // Invalidar caché
                    CargarCupones();
                    ActualizarEstadisticas();

                    string mensaje = $"Carga CSV completada: {cuponesAgregados} cupones agregados";
                    if (errores > 0)
                    {
                        mensaje += $", {errores} con errores";
                    }

                    MostrarMensaje(mensaje, cuponesAgregados > 0 ? "success" : "warning");
                    OcultarModalCsv();
                }
            }
            catch (System.Exception ex)
            {
                litMensajeCsvModal.Text = $"Error al procesar el archivo CSV: {ex.Message}";
                MostrarModalCsv();
            }
        }

        private cupon CrearCuponDesdeCsv(string[] values, Dictionary<string, int> headerMap, int lineNumber, List<string> errores)
        {
            try
            {
                // Validar tipo de descuento
                var tipoDescuento = GetCsvValue(values, headerMap, "TipoDescuento").ToLower();
                if (tipoDescuento != "porcentaje" && tipoDescuento != "monto_fijo")
                {
                    errores.Add($"Línea {lineNumber}: Tipo de descuento inválido. Use 'porcentaje' o 'monto_fijo'");
                    return null;
                }

                // Validar valor de descuento
                if (!double.TryParse(GetCsvValue(values, headerMap, "ValorDescuento"), out double valor))
                {
                    errores.Add($"Línea {lineNumber}: Valor de descuento inválido");
                    return null;
                }

                if (tipoDescuento == "porcentaje" && (valor <= 0 || valor > 100))
                {
                    errores.Add($"Línea {lineNumber}: El porcentaje debe estar entre 0 y 100");
                    return null;
                }
                else if (tipoDescuento == "monto_fijo" && valor <= 0)
                {
                    errores.Add($"Línea {lineNumber}: El monto fijo debe ser mayor a 0");
                    return null;
                }

                // Validar fechas
                if (!DateTime.TryParse(GetCsvValue(values, headerMap, "FechaInicio"), out DateTime fechaInicio) ||
                    !DateTime.TryParse(GetCsvValue(values, headerMap, "FechaFin"), out DateTime fechaFin))
                {
                    errores.Add($"Línea {lineNumber}: Formato de fecha inválido");
                    return null;
                }

                if (fechaFin <= fechaInicio)
                {
                    errores.Add($"Línea {lineNumber}: La fecha fin debe ser posterior a la fecha inicio");
                    return null;
                }

                // Validar cantidad máxima
                if (!int.TryParse(GetCsvValue(values, headerMap, "CantidadMaxima"), out int cantidad) || cantidad <= 0)
                {
                    errores.Add($"Línea {lineNumber}: Cantidad máxima inválida");
                    return null;
                }

                // Validar estado activo
                if (!bool.TryParse(GetCsvValue(values, headerMap, "EstaActivo"), out bool estaActivo))
                {
                    errores.Add($"Línea {lineNumber}: Estado activo inválido. Use true o false");
                    return null;
                }

                // Crear y retornar cupón
                return new cupon
                {
                    codigo = GetCsvValue(values, headerMap, "Codigo").ToUpper(),
                    descripcionEs = GetCsvValue(values, headerMap, "Descripcion"),
                    descripcionEn = GetCsvValue(values, headerMap, "Descripcion"),
                    descuentoTipo = (tipoDescuento)Enum.Parse(typeof(tipoDescuento), tipoDescuento.Replace("_", ""), true),
                    descuentoValor = valor,
                    maxUsos = cantidad,
                    activo = estaActivo,
                    fechaInicio = null, // Se pasa como string separado
                    fechaFin = null     // Se pasa como string separado
                };
            }
            catch (System.Exception ex)
            {
                errores.Add($"Línea {lineNumber}: Error inesperado - {ex.Message}");
                return null;
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

        private List<cupon> GetCachedCupones()
        {
            if (_cachedCupones == null)
            {
                try
                {
                    _cachedCupones = cuponWS.listarCupones()?.ToList() ?? new List<cupon>();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener cupones para estadísticas: " + ex.ToString());
                    _cachedCupones = new List<cupon>();
                }
            }
            return _cachedCupones;
        }

        public int GetTotalCupones()
        {
            return GetCachedCupones().Count;
        }
    }
}