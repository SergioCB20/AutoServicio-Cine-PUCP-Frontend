using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class GestionCupones : System.Web.UI.Page
    {
        private CuponWSClient cuponWS;

        protected void Page_Load(object sender, EventArgs e)
        {
            cuponWS = new CuponWSClient();

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
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidarFechas())
            {
                try
                {
                    var cupon = CrearCuponDesdeFormulario();

                    if (string.IsNullOrEmpty(hiddenCuponId.Value))
                    {
                        // Nuevo cupón
                        cuponWS.registrarCupon(cupon);
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
                codigo = txtCodigo.Text.Trim().ToUpper(),
                descripcionEn = txtDescripcion.Text.Trim(),
                descuentoValor = Convert.ToInt32(txtPorcentajeDescuento.Text),
                descuentoTipo = (AutoServicioCineWeb.AutoservicioCineWS.tipoDescuento)Enum.Parse(
                                    typeof(AutoServicioCineWeb.AutoservicioCineWS.tipoDescuento),
                                    ddlDescuentoTipo.SelectedValue,
                                    true // Ignorar mayúsculas/minúsculas
                                ),
                //fechaInicio = Convert.ToDateTime(txtFechaInicio.Text),
                //fechaFin = Convert.ToDateTime(txtFechaFin.Text),
                maxUsos = Convert.ToInt32(txtCantidadMaxima.Text),
                usosActuales = 0 // Siempre inicia en 0 para cupones nuevos
                // = chkActivo.Checked

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
    }
}