using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.SalaWS;

namespace AutoServicioCineWeb
{
    /// <summary>
    /// Página para gestionar las salas del cine.
    /// Permite agregar, editar y listar salas.
    /// </summary>
    public partial class GestionSalas : System.Web.UI.Page
    {
        private readonly SalaWSClient salaServiceClient;

        public GestionSalas()
        {
            // Inicializa el cliente del servicio SOAP para interactuar con las salas
            salaServiceClient = new SalaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string salaIdStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(salaIdStr) && int.TryParse(salaIdStr, out int salaId))
                {
                    hdnSalaId.Value = salaId.ToString();
                    CargarDatosSalaParaEdicion(salaId);
                    litModalTitle.Text = "Editar Sala";
                }
                else
                {
                    hdnSalaId.Value = string.Empty;
                    litModalTitle.Text = "Agregar Sala";
                    LimpiarCamposModal();
                }
                CargarSalas();
            }
        }

        private void CargarDatosSalaParaEdicion(int salaId)
        {
            try
            {
                sala sala = salaServiceClient.buscarSalaPorId(salaId);
                if (sala != null)
                {
                    txtNombreSala.Text = sala.nombre;
                    txtCapacidad.Text = sala.capacidad.ToString();
                    ddlTipoSala.SelectedValue = sala.tipoSala.ToString();
                    chkActiva.Checked = sala.activa;
                }
                else
                {
                    litMensajeModal.Text = "Sala no encontrada.";
                }
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar los datos de la sala: " + ex.Message;
            }
        }

        private void CargarSalas()
        {
            try
            {
                List<sala> salas = salaServiceClient.listarSalas().ToList();
                gvSalas.DataSource = salas;
                gvSalas.DataBind();
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar las salas: " + ex.Message;
            }
        }

        private void LimpiarCamposModal()
        {
            txtNombreSala.Text = string.Empty;
            txtCapacidad.Text = string.Empty;
            ddlTipoSala.SelectedIndex = 0;
            chkActiva.Checked = false;
        }

        protected void btnGuardarSala_Click(object sender, EventArgs e)
        {
            try
            {
                sala sala = new sala
                {
                    nombre = txtNombreSala.Text,
                    capacidad = int.Parse(txtCapacidad.Text),
                    tipoSala = (tipoSala)Enum.Parse(typeof(tipoSala), ddlTipoSala.SelectedValue),
                    activa = chkActiva.Checked
                };

                if (int.TryParse(hdnSalaId.Value, out int id))
                {
                    sala.id = id;
                    salaServiceClient.actualizarSala(sala);
                }
                else
                {
                    salaServiceClient.registrarSala(sala);
                }

                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al guardar la sala: " + ex.Message;
            }
        }

        protected void gvSalas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditarSala")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("GestionSalas.aspx?id=" + id);
            }
        }

        protected void gvSalas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSalas.PageIndex = e.NewPageIndex;
            CargarSalas();
        }
    }
}

