using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Make sure this namespace is correct for your service reference
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class GestionPeliculas : System.Web.UI.Page
    {
        // Declare the SOAP client proxy
        // Use a lazy initialization or create in Page_Load if you prefer
        private readonly PeliculaWSClient peliculaServiceClient;

        public GestionPeliculas()
        {
            // Initialize the SOAP client
            peliculaServiceClient = new PeliculaWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Only execute on the initial page load, not on postbacks
            {
                CargarPeliculas();
            }
        }

        // New helper method to load movie data into the modal for editing
        private void CargarDatosPeliculaParaEdicion(int peliculaId)
        {
            try
            {
                // Use the SOAP client to find movie by ID
                pelicula peli = peliculaServiceClient.buscarPeliculaPorId(peliculaId);

                if (peli != null)
                {
                    txtTituloEs.Text = peli.tituloEs;
                    txtTituloEn.Text = peli.tituloEn;
                    txtDuracionMin.Text = peli.duracionMin.ToString();
                    ddlClasificacion.SelectedValue = peli.clasificacion; // Set dropdown selected value
                    txtSinopsisEs.Text = peli.sinopsisEs;
                    txtSinopsisEn.Text = peli.sinopsisEn;
                    chkEstaActiva.Checked = peli.estaActiva;
                    txtImagenUrl.Text = peli.imagenUrl;
                    hdnExistingImageUrl.Value = peli.imagenUrl; // Store for client-side preview re-application

                    // Register script to show the modal and populate image preview
                    string script = $"showEditPeliculaModal({peli.peliculaId}, '{peli.tituloEs.Replace("'", "\\'")}', '{peli.tituloEn.Replace("'", "\\'")}', '{peli.sinopsisEs.Replace("'", "\\'")}', '{peli.sinopsisEn.Replace("'", "\\'")}', {peli.duracionMin}, '{peli.clasificacion}', '{peli.imagenUrl.Replace("'", "\\'")}', {peli.estaActiva.ToString().ToLower()});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowEditModal", script, true);
                }
                else
                {
                    litMensajeModal.Text = "Película no encontrada para edición.";
                    // Optionally, redirect if movie not found
                    // Response.Redirect("GestionPeliculas.aspx");
                }
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar datos de película para edición: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar datos de película para edición: " + ex.ToString());
            }
        }

        private void CargarPeliculas()
        {
            try
            {
                List<pelicula> peliculas = peliculaServiceClient.listarPeliculas().ToList();

                // Apply filters
                peliculas = FiltrarPeliculas(peliculas);

                gvPeliculas.DataSource = peliculas;
                gvPeliculas.DataBind();
            }
            catch (Exception ex)
            {
                // Ensure litMensajeModal is visible or use a different mechanism for error display
                litMensajeModal.Text = "Error al cargar películas: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar películas: " + ex.ToString());
            }
        }

        private List<pelicula> FiltrarPeliculas(List<pelicula> peliculas)
        {
            // Filter by search text
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

            // Filter by classification
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
                hdnPeliculaId.Value = peliculaId.ToString(); // Set the hidden field ID for the modal form
                litModalTitle.Text = "Editar Película";
                CargarDatosPeliculaParaEdicion(peliculaId); // Reuse the loading logic
            }
            else if (e.CommandName == "DeletePelicula")
            {
                try
                {
                    // Call the delete method from your SOAP client
                    peliculaServiceClient.eliminarPelicula(peliculaId); // Assuming 'eliminarPelicula' exists

                    litMensajeModal.Text = "Película eliminada exitosamente.";
                    CargarPeliculas(); // Reload the GridView
                    // Clear message after a short delay or after next postback if it's for user feedback
                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearMessage", "setTimeout(function(){ document.getElementById('" + litMensajeModal.ClientID + "').innerHTML = ''; }, 3000);", true);
                }
                catch (Exception ex)
                {
                    litMensajeModal.Text = $"Error al eliminar la película: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine("Error al eliminar película: " + ex.ToString());
                }
            }
        }


        protected void btnGuardarPelicula_Click(object sender, EventArgs e)
        {
            Page.Validate("PeliculaValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                // Re-show the modal if validation fails
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "openPeliculaModal();", true);
                return;
            }

            pelicula peli = new pelicula
            {
                peliculaId = Convert.ToInt32(hdnPeliculaId.Value),
                peliculaIdSpecified = true, // Must be true when sending an explicit ID (even 0 for new)

                tituloEs = txtTituloEs.Text,
                tituloEn = txtTituloEn.Text,
                duracionMin = Convert.ToInt32(txtDuracionMin.Text),
                clasificacion = ddlClasificacion.SelectedValue, // Get value from DropDownList
                sinopsisEs = txtSinopsisEs.Text,
                sinopsisEn = txtSinopsisEn.Text,
                estaActiva = chkEstaActiva.Checked,
                imagenUrl = txtImagenUrl.Text.Trim(),

                usuarioModificacion = 4 // Your fixed user ID
            };
            // Ensure this is set to true for userModificacion to be sent
            peli.usuarioModificacionSpecified = true;


            // Debug output in Visual Studio's Output window
            System.Diagnostics.Debug.WriteLine("--- C# Pelicula object before sending ---");
            System.Diagnostics.Debug.WriteLine($"peliculaId: {peli.peliculaId}");
            System.Diagnostics.Debug.WriteLine($"peliculaIdSpecified: {peli.peliculaIdSpecified}");
            System.Diagnostics.Debug.WriteLine($"tituloEs: {peli.tituloEs}");
            System.Diagnostics.Debug.WriteLine($"duracionMin: {peli.duracionMin}");
            System.Diagnostics.Debug.WriteLine($"estaActiva: {peli.estaActiva}");
            System.Diagnostics.Debug.WriteLine($"usuarioModificacion: {peli.usuarioModificacion}");
            System.Diagnostics.Debug.WriteLine($"usuarioModificacionSpecified: {peli.usuarioModificacionSpecified}");
            System.Diagnostics.Debug.WriteLine("-----------------------------------------");

            try
            {
                if (peli.peliculaId == 0)
                {
                    // Call registrarPelicula which will handle setting fecha_modificacion in DB
                    peliculaServiceClient.registrarPelicula(peli);
                    litMensajeModal.Text = "Película agregada exitosamente.";
                }
                else
                {
                    // Call actualizarPelicula
                    peliculaServiceClient.actualizarPelicula(peli);
                    litMensajeModal.Text = "Película actualizada exitosamente.";
                }

                CargarPeliculas(); // Reload the GridView to show updated data
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideModal", "closePeliculaModal();", true);
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar la película: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "openPeliculaModal();", true);
                System.Diagnostics.Debug.WriteLine("Error al guardar película: " + ex.ToString());
            }
        }



        private void LimpiarCamposModal()
        {
            txtTituloEs.Text = "";
            txtTituloEn.Text = "";
            txtSinopsisEs.Text = "";
            txtSinopsisEn.Text = "";
            txtDuracionMin.Text = "";
            ddlClasificacion.SelectedValue = ""; // Clear dropdown selection
            txtImagenUrl.Text = "";
            chkEstaActiva.Checked = true; // Default to active
            litMensajeModal.Text = ""; // Clear any previous messages

            // Ensure image preview is hidden
            imgPreview.ImageUrl = "";
            imgPreview.Style["display"] = "none";
            hdnExistingImageUrl.Value = ""; // Clear hidden field for image URL
        }

        // --- Métodos para estadísticas (llamados desde el script JS) ---
        public int GetTotalPeliculas()
        {
            try
            {
                return peliculaServiceClient.listarPeliculas().Length;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener total de películas: " + ex.ToString());
                return 0;
            }
        }

        public int GetPeliculasActivas()
        {
            try
            {
                // Assuming listarPeliculas returns all movies, then filter them
                return peliculaServiceClient.listarPeliculas().Count(p => p.estaActiva);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener películas activas: " + ex.ToString());
                return 0;
            }
        }

        public int GetPeliculasInactivas()
        {
            try
            {
                return peliculaServiceClient.listarPeliculas().Count(p => !p.estaActiva);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener películas inactivas: " + ex.ToString());
                return 0;
            }
        }

        public int GetClasificacionesUnicas()
        {
            try
            {
                return peliculaServiceClient.listarPeliculas().Select(p => p.clasificacion).Distinct().Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al obtener clasificaciones únicas: " + ex.ToString());
                return 0;
            }
        }

        protected void txtSearchPeliculas_TextChanged(object sender, EventArgs e)
        {
            gvPeliculas.PageIndex = 0; // Reset pagination when searching
            CargarPeliculas();
        }

        protected void ddlClasificacionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPeliculas.PageIndex = 0; // Reset pagination when filtering
            CargarPeliculas();
        }
    }
}