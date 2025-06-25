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
            }
        }

        private void CargarCupones()
        {
            try
            {
                var cupones = cuponWS.listarCupones();
                rptCupones.DataSource = cupones;
                rptCupones.DataBind();
            }
            catch (System.Exception ex)
            {
                MostrarMensaje("Error al cargar cupones: " + ex.Message, "error");
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
                    usuario usu = new usuario {id = 11};
                    cupon.modificadoPor = usu;
                    cupon.cuponIdSpecified = true;
                    cupon.descuentoTipoSpecified = true;
                    cupon.descuentoValorSpecified = true;
                    cupon.maxUsosSpecified = true;
                    cupon.usosActualesSpecified = true;
                    cupon.creadoPor = usu;

                    if (string.IsNullOrEmpty(hiddenCuponId.Value))
                    {
                        // Nuevo cupón
                        cuponWS.registrarCupon(cupon,stringIni,stringFin);
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
                    CargarCupones();
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
                CargarCupones();
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
                codigo = txtCodigo.Text.Trim(),
                descripcionEs = txtDescripcion.Text.Trim(),
                descripcionEn = txtDescripcion.Text.Trim(),
                descuentoTipo = (tipoDescuento)Enum.Parse(typeof(tipoDescuento), ddlDescuentoTipo.SelectedValue, true),
                descuentoTipoSpecified = true,
                descuentoValor = double.Parse(txtPorcentajeDescuento.Text),
                descuentoValorSpecified = true,
                // Para localDate, necesitas crear objetos localDate
                fechaInicio = null,
                fechaFin = null,
                maxUsos = int.Parse(txtCantidadMaxima.Text),
                maxUsosSpecified = true,
                activo = chkActivo.Checked,
                usosActuales=0
             
            };
        }



        private void CargarDatosEnFormulario(cupon cupon)
        {
            txtCodigo.Text = cupon.codigo;
            txtDescripcion.Text = cupon.descripcionEs;
            txtPorcentajeDescuento.Text = cupon.descuentoValor.ToString();
            txtCantidadMaxima.Text = cupon.maxUsos.ToString();
            //txtFechaInicio.Text = cupon.fechaInicio.ToString("yyyy-MM-dd");
            //txtFechaFin.Text = cupon.fechaFin.ToString("yyyy-MM-dd");
            //chkActivo.Checked = cupon.activo;
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
        }

        private bool ValidarFechas()
        {
            DateTime fechaInicio = Convert.ToDateTime(txtFechaInicio.Text);
            DateTime fechaFin = Convert.ToDateTime(txtFechaFin.Text);

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

        // Métodos para el Repeater - Helpers para mostrar datos
        protected string GetUsagePercentage(object cantidadUsada, object cantidadMaxima)
        {
            if (cantidadUsada == null || cantidadMaxima == null) return "0";

            int usada = Convert.ToInt32(cantidadUsada);
            int maxima = Convert.ToInt32(cantidadMaxima);

            if (maxima == 0) return "0";

            return Math.Round((double)usada / maxima * 100, 1).ToString();
        }

        protected string GetStatusClass(object activo, object fechaFin)
        {
            bool esActivo = Convert.ToBoolean(activo);
            DateTime fecha = Convert.ToDateTime(fechaFin);

            if (!esActivo)
                return "status-inactive";
            else if (fecha < DateTime.Now)
                return "status-expired";
            else
                return "status-active";
        }

        protected string GetStatusText(object activo, object fechaFin)
        {
            bool esActivo = Convert.ToBoolean(activo);
            DateTime fecha = Convert.ToDateTime(fechaFin);

            if (!esActivo)
                return "Inactivo";
            else if (fecha < DateTime.Now)
                return "Expirado";
            else
                return "Activo";
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string script = $"showNotification('{mensaje}', '{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "notification", script, true);
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

            int cuponesAgregados = 0;
            int cuponesModificados = 0;
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

                    // Verificar que las columnas mínimas existan para cupones
                    string[] requiredCouponColumns = {
                        "Codigo",
                        "Descripcion",
                        "TipoDescuento",
                        "ValorDescuento",
                        "CantidadMaxima",
                        "FechaInicio",
                        "FechaFin",
                        "EstaActivo"
                    };

                    foreach (var col in requiredCouponColumns)
                    {
                        if (!headerMap.ContainsKey(col))
                        {
                            litMensajeCsvModal.Text = $"Error: La columna '{col}' es requerida en el CSV para cupones.";
                            MostrarModalCsv();
                            return;
                        }
                    }

                    // Procesar cada línea del CSV
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split(',');

                        try
                        {
                            // Validación adicional para el tipo de descuento
                            var tipoDescuento = values[headerMap["TipoDescuento"]].Trim().ToLower();
                            if (tipoDescuento != "porcentaje" && tipoDescuento != "monto_fijo")
                            {
                                errores++;
                                continue; // Saltar esta fila pero continuar con las demás
                            }

                            // Validar que el valor de descuento sea adecuado según el tipo
                            if (!decimal.TryParse(values[headerMap["ValorDescuento"]].Trim(), out decimal valor))
                            {
                                errores++;
                                continue;
                            }

                            if (tipoDescuento == "porcentaje" && (valor <= 0 || valor > 100))
                            {
                                errores++;
                                continue;
                            }
                            else if (tipoDescuento == "monto_fijo" && valor <= 0)
                            {
                                errores++;
                                continue;
                            }

                            // Validación de fechas
                            if (!DateTime.TryParse(values[headerMap["FechaInicio"]].Trim(), out DateTime fechaInicio) ||
                                !DateTime.TryParse(values[headerMap["FechaFin"]].Trim(), out DateTime fechaFin))
                            {
                                errores++;
                                continue;
                            }

                            if (fechaFin < fechaInicio)
                            {
                                errores++;
                                continue;
                            }

                            // Validación de cantidad máxima
                            if (!int.TryParse(values[headerMap["CantidadMaxima"]].Trim(), out int cantidad) || cantidad <= 0)
                            {
                                errores++;
                                continue;
                            }

                            // Aquí iría la lógica para guardar/actualizar el cupón en la base de datos
                            // Ejemplo:
                            /*
                            var cupon = new Cupon {
                                Codigo = values[headerMap["Codigo"]].Trim(),
                                Descripcion = values[headerMap["Descripcion"]].Trim(),
                                TipoDescuento = tipoDescuento,
                                ValorDescuento = valor,
                                FechaInicio = fechaInicio,
                                FechaFin = fechaFin,
                                MaxUsos = cantidad,
                                EstaActivo = bool.Parse(values[headerMap["EstaActivo"]].Trim())
                            };

                            bool resultado = CuponService.GuardarCupon(cupon);
                            if (resultado) cuponesAgregados++;
                            */

                            // Simulación de éxito para el ejemplo
                            cuponesAgregados++;
                        }
                        catch (System.Exception ex)
                        {
                            errores++;
                            System.Diagnostics.Debug.WriteLine($"Error procesando fila: {ex.Message}");
                        }
                    }

                    _cachedCupones = null; // Invalida la caché
                    CargarCupones(); // Recargar la tabla y estadísticas

                    litMensajeCsvModal.Text = $"Carga CSV completada: {cuponesAgregados} agregados, {cuponesModificados} actualizados, {errores} con errores.";
                    OcultarModalCsv(); // Cerrar el modal después de la carga exitosa
                }
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
        private List<cupon> GetCachedCupones()
        {
            if (_cachedCupones == null)
            {
                try
                {
                    _cachedCupones = cuponWS.listarCupones().ToList();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener películas para estadísticas: " + ex.ToString());
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