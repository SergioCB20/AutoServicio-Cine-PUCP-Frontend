using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class GestionFunciones : System.Web.UI.Page
    {
        private readonly FuncionWSClient funcionService = new FuncionWSClient();
        private readonly SalaWSClient salaService = new SalaWSClient();
        private List<funcion> _cachedFunciones;
        private int idUsuario = 34; //Por defecto

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarParametrosPelicula();
                CargarSalas();
                CargarFunciones();
                if (Context.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)Context.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    string userData = ticket.UserData;
                    string[] userInfo = userData.Split('|');
                    idUsuario = int.Parse(userInfo[0]); // Asumiendo que el ID de usuario es el primer elemento
                }
            }
        }

        private void CargarParametrosPelicula()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["PeliculaId"]) &&
                !string.IsNullOrEmpty(Request.QueryString["NombrePelicula"]))
            {
                hdnPeliculaId.Value = Request.QueryString["PeliculaId"];
                lblPeliculaNombre.Text = Server.UrlDecode(Request.QueryString["NombrePelicula"]);
                litTituloPelicula.Text = Server.UrlDecode(Request.QueryString["NombrePelicula"]);
            }
            else
            {
                Response.Redirect("~/GestionPeliculas.aspx");
            }
        }

        private void CargarSalas()
        {
            var salas = salaService.listarSalas()?.ToList() ?? new List<sala>();
            ddlSalas.DataSource = salas;
            ddlSalas.DataTextField = "nombre";
            ddlSalas.DataValueField = "id";
            ddlSalas.DataBind();
        }

        private void CargarFunciones()
        {
            int peliculaId = Convert.ToInt32(hdnPeliculaId.Value);
            string formatoSeleccionado = ddlFormatoFilter.SelectedValue;

            var funciones = funcionService.listarFunciones()?.Where(f => f.peliculaId == peliculaId).ToList() ?? new List<funcion>();

            if (!string.IsNullOrEmpty(formatoSeleccionado))
            {
                funciones = funciones.Where(f => f.formatoProyeccion.Equals(formatoSeleccionado, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            _cachedFunciones = funciones;
            gvFunciones.DataSource = _cachedFunciones;
            gvFunciones.DataBind();
        }

        protected void gvFunciones_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFunciones.PageIndex = e.NewPageIndex;
            CargarFunciones();
        }

        protected void gvFunciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int funcionId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditFuncion")
            {
                CargarFuncionEnModal(funcionId);
                MostrarModal();
            }
            else if (e.CommandName == "DeleteFuncion")
            {
                funcionService.eliminarFuncion(funcionId);
                CargarFunciones();
            }
        }

        private void CargarFuncionEnModal(int funcionId)
        {
            var funcion = funcionService.buscarFuncionPorId(funcionId);
            if (funcion != null)
            {
                hdnFuncionId.Value = funcion.funcionId.ToString();
                hdnPeliculaId.Value = funcion.peliculaId.ToString();
                lblPeliculaNombre.Text = Server.UrlDecode(Request.QueryString["XNombrePelicula"]);
                ddlSalas.SelectedValue = funcion.salaId.ToString();
                txtFechaHora.Text = funcion.fechaHora;
                ddlFormato.SelectedValue = funcion.formatoProyeccion;
                txtIdioma.Text = funcion.idioma;
                chkSubtitulos.Checked = funcion.subtitulos;
                chkFuncionActiva.Checked = funcion.estaActiva;
                litFuncionModalTitle.Text = "Editar Función";
            }
        }

        protected void btnGuardarFuncion_Click(object sender, EventArgs e)
        {
            var funcion = new funcion
            {
                funcionId = string.IsNullOrEmpty(hdnFuncionId.Value) ? 0 : Convert.ToInt32(hdnFuncionId.Value),
                funcionIdSpecified = true,
                peliculaId = Convert.ToInt32(hdnPeliculaId.Value),
                peliculaIdSpecified = true,
                salaId = Convert.ToInt32(ddlSalas.SelectedValue),
                salaIdSpecified = true,
                fechaHora = txtFechaHora.Text,
                formatoProyeccion = ddlFormato.SelectedValue,
                idioma = txtIdioma.Text,
                subtitulos = chkSubtitulos.Checked,
                subtitulosSpecified = true,
                estaActiva = chkFuncionActiva.Checked,
                usuarioModificacion = idUsuario,
                usuarioModificacionSpecified = true,
            };

            try
            {
                if (funcion.funcionId == 0)
                    funcionService.registrarFuncion(funcion);
                else
                    funcionService.actualizarFuncion(funcion);
            }
            catch (System.ServiceModel.FaultException ex)
            {
                // Manejo de errores del servicio
                litMensajeCrear.Text = "Error al guardar la función: " + ex.Message;
                MostrarModal();
                return;
            }


            CargarFunciones();
            OcultarModal();
        }

        protected void btnOpenAddFuncionModal_Click(object sender, EventArgs e)
        {
            LimpiarModal();
            MostrarModal();
        }

        protected void btnCancelarFuncion_Click(object sender, EventArgs e) => OcultarModal();
        protected void btnCloseFuncionModal_Click(object sender, EventArgs e) => OcultarModal();

        private void MostrarModal() => funcionModal.Style["display"] = "flex";
        private void OcultarModal() => funcionModal.Style["display"] = "none";

        private void LimpiarModal()
        {
            hdnFuncionId.Value = "";
            ddlSalas.SelectedIndex = 0;
            txtFechaHora.Text = "";
            ddlFormato.SelectedIndex = 0;
            txtIdioma.Text = "";
            chkSubtitulos.Checked = false;
            chkFuncionActiva.Checked = true;
            litFuncionModalTitle.Text = "Nueva Función";
        }

        protected void txtBuscarFuncion_TextChanged(object sender, EventArgs e) => CargarFunciones();
        protected void ddlFormatoFilter_SelectedIndexChanged(object sender, EventArgs e) => CargarFunciones();

        private void MostrarFuncionCsvModal()
        {
            funcionCsvModal.Style["display"] = "flex";
            litMensajeFuncionCsv.Text = "";
        }

        private void OcultarFuncionCsvModal()
        {
            funcionCsvModal.Style["display"] = "none";
            litMensajeFuncionCsv.Text = "";
            FileUploadFuncionCsv.Attributes.Remove("value");
        }

        protected void btnOpenFuncionCsvModal_Click(object sender, EventArgs e) => MostrarFuncionCsvModal();
        protected void btnCloseCsvModal_Click(object sender, EventArgs e) => OcultarFuncionCsvModal();
        protected void btnCancelFuncionCsvModal_Click(object sender, EventArgs e) => OcultarFuncionCsvModal();

        protected void cvFuncionCsvFileExtension_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (FileUploadFuncionCsv.HasFile)
            {
                string ext = Path.GetExtension(FileUploadFuncionCsv.FileName).ToLower();
                args.IsValid = ext == ".csv";
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnUploadFuncionCsv_Click(object sender, EventArgs e)
        {
            Page.Validate("FuncionCsvUpload");

            if (!Page.IsValid)
            {
                MostrarFuncionCsvModal();
                return;
            }

            if (!FileUploadFuncionCsv.HasFile)
            {
                litMensajeFuncionCsv.Text = "Selecciona un archivo CSV.";
                MostrarFuncionCsvModal();
                return;
            }

            int funcionesAgregadas = 0;
            int errores = 0;
            int peliculaId = Convert.ToInt32(hdnPeliculaId.Value);

            try
            {
                using (StreamReader reader = new StreamReader(FileUploadFuncionCsv.PostedFile.InputStream))
                {
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        litMensajeFuncionCsv.Text = "El archivo CSV está vacío o sin cabecera.";
                        MostrarFuncionCsvModal();
                        return;
                    }

                    var headers = headerLine.Split(',').Select(h => h.Trim()).ToList();
                    var headerMap = headers.Select((h, i) => new { h, i }).ToDictionary(x => x.h, x => x.i);

                    string[] requiredColumns = { "FechaHora", "SalaId", "FormatoProyeccion", "Idioma", "Subtitulos", "EstaActiva" };
                    foreach (var col in requiredColumns)
                    {
                        if (!headerMap.ContainsKey(col))
                        {
                            litMensajeFuncionCsv.Text = $"Falta la columna '{col}'.";
                            MostrarFuncionCsvModal();
                            return;
                        }
                    }

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] data = line.Split(',');
                        try
                        {
                            funcion f = new funcion
                            {
                                peliculaId = peliculaId,
                                peliculaIdSpecified = true,
                                fechaHora = GetCsvValue(data, headerMap, "FechaHora"),
                                salaId = int.Parse(GetCsvValue(data, headerMap, "SalaId")),
                                salaIdSpecified = true,
                                formatoProyeccion = GetCsvValue(data, headerMap, "FormatoProyeccion"),
                                idioma = GetCsvValue(data, headerMap, "Idioma"),
                                subtitulos = ParseBool(GetCsvValue(data, headerMap, "Subtitulos")),
                                subtitulosSpecified = true,
                                estaActiva = ParseBool(GetCsvValue(data, headerMap, "EstaActiva")),
                                usuarioModificacion = idUsuario,
                                usuarioModificacionSpecified = true
                            };

                            funcionService.registrarFuncion(f);
                            funcionesAgregadas++;
                        }
                        catch
                        {
                            errores++;
                        }
                    }
                }

                _cachedFunciones = null;
                CargarFunciones();

                litMensajeFuncionCsv.Text = $"Carga completada: {funcionesAgregadas} agregadas, {errores} errores.";
                OcultarFuncionCsvModal();
            }
            catch (System.Exception ex)
            {
                litMensajeFuncionCsv.Text = "Error al procesar CSV: " + ex.Message;
                MostrarFuncionCsvModal();
            }
        }

        private bool ParseBool(string val)
        {
            val = val.Trim().ToLower();
            return val == "true" || val == "1";
        }

        private string GetCsvValue(string[] data, Dictionary<string, int> headerMap, string columnName)
        {
            if (headerMap.ContainsKey(columnName) && headerMap[columnName] < data.Length)
                return data[headerMap[columnName]].Trim();
            return "";
        }

        public int GetTotalFunciones() => _cachedFunciones?.Count ?? 0;
        public int GetFuncionesActivas() => _cachedFunciones?.Count(f => f.estaActiva) ?? 0;
        public int GetFuncionesInactivas() => _cachedFunciones?.Count(f => !f.estaActiva) ?? 0;
    }
}
