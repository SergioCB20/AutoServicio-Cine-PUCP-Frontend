using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class GestionPeliculas : System.Web.UI.Page
    {
        private readonly PeliculaWSClient peliculaServiceClient;
        // Cachear la lista de películas para las estadísticas y filtrado
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
            // Si hay una URL en el campo de texto, llama a previewImage.
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReapplyImagePreview",
                "const txtImageUrlElement = document.getElementById('" + txtImagenUrl.ClientID + "'); if (txtImageUrlElement && txtImageUrlElement.value) { previewImage(txtImageUrlElement); }", true);
        }

        private void CargarPeliculas()
        {
            try
            {
                // Solo cargar de la BD si no está cacheado (o si es el primer postback relevante)
                _cachedPeliculas = peliculaServiceClient.listarPeliculas().ToList();

                // Aplicar filtros a la lista cacheada
                List<pelicula> peliculasFiltradas = FiltrarPeliculas(_cachedPeliculas);

                gvPeliculas.DataSource = peliculasFiltradas;
                gvPeliculas.DataBind();
            }
            catch (Exception ex)
            {
                // Asegúrate de que litMensajeModal sea visible al usuario si no está en el modal
                // O usa un control de mensaje en la página principal
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
                MostrarModal(); // Abre el modal después de cargar los datos
            }
            else if (e.CommandName == "DeletePelicula")
            {
                try
                {
                    peliculaServiceClient.eliminarPelicula(peliculaId);
                    litMensajeModal.Text = "Película eliminada exitosamente.";
                    // Recargar la lista completa después de eliminar para actualizar estadísticas y GridView
                    _cachedPeliculas = null; // Invalida la caché para que se vuelva a cargar
                    CargarPeliculas();
                }
                catch (Exception ex)
                {
                    litMensajeModal.Text = $"Error al eliminar la película: {ex.Message}";
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
                    hdnExistingImageUrl.Value = peli.imagenUrl; // Guarda la URL original

                    // Llama a la función JS para previsualizar la imagen si existe
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
                MostrarModal(); // Vuelve a mostrar el modal con los errores de validación
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
                usuarioModificacion = 4, // Your fixed user ID
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

                _cachedPeliculas = null; // Invalida la caché para que se vuelva a cargar
                CargarPeliculas(); // Recarga el GridView y las estadísticas
                OcultarModal(); // Cierra el modal
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar la película: {ex.Message}";
                MostrarModal(); // Si hay error, mantener el modal abierto para que el usuario vea el mensaje
                System.Diagnostics.Debug.WriteLine("Error al guardar película: " + ex.ToString());
            }
        }

        // --- Nuevos métodos para manejar el modal desde el CodeBehind ---
        private void MostrarModal()
        {
            peliculaModal.Style["display"] = "flex"; // Establece display: flex para mostrar el modal
        }

        private void OcultarModal()
        {
            peliculaModal.Style["display"] = "none"; // Oculta el modal
            litMensajeModal.Text = ""; // Limpia el mensaje del modal al cerrarlo
        }

        protected void btnOpenAddModal_Click(object sender, EventArgs e)
        {
            hdnPeliculaId.Value = "0"; // Reinicia el ID para indicar nueva película
            litModalTitle.Text = "Agregar Nueva ";
            LimpiarCamposModal(); // Limpia los campos del formulario
            // Llama a la función JS para limpiar validadores client-side
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearValidators", "clearModalValidators();", true);
            MostrarModal(); // Muestra el modal
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            OcultarModal(); // Cierra el modal
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            OcultarModal(); // Cierra el modal
        }

        private void LimpiarCamposModal()
        {
            txtTituloEs.Text = "";
            txtTituloEn.Text = "";
            txtSinopsisEs.Text = "";
            txtSinopsisEn.Text = "";
            txtDuracionMin.Text = "";
            ddlClasificacion.SelectedValue = ""; // Limpia la selección del dropdown
            txtImagenUrl.Text = "";
            chkEstaActiva.Checked = true; // Valor predeterminado a activa

            // Oculta la previsualización de imagen
            imgPreview.ImageUrl = "";
            imgPreview.Style["display"] = "none";
            hdnExistingImageUrl.Value = "";
        }

        // --- Métodos para estadísticas (optimizados para usar la caché) ---
        // Asegurarse de que _cachedPeliculas no sea null antes de usarlo
        private List<pelicula> GetCachedPeliculas()
        {
            if (_cachedPeliculas == null)
            {
                // Si la caché es nula, la recargamos. Esto asegura que los métodos de estadísticas
                // siempre tengan datos, incluso si no se ha llamado a CargarPeliculas() directamente.
                try
                {
                    _cachedPeliculas = peliculaServiceClient.listarPeliculas().ToList();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener películas para estadísticas: " + ex.ToString());
                    _cachedPeliculas = new List<pelicula>(); // Retorna una lista vacía para evitar NullReferenceException
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
            gvPeliculas.PageIndex = 0; // Reset pagination when searching
            CargarPeliculas(); // Recarga aplicando el filtro
        }

        protected void ddlClasificacionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPeliculas.PageIndex = 0; // Reset pagination when filtering
            CargarPeliculas(); // Recarga aplicando el filtro
        }
    }
}