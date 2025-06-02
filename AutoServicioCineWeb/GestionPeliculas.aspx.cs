using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Make sure this namespace is correct for your service reference
using AutoServicioCineWeb.PeliculaWebService;

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
                // --- Logic to handle loading for EDITING or NEW record ---
                string peliculaIdStr = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(peliculaIdStr))
                {
                    // --- EDIT MODE ---
                    int peliculaId;
                    if (int.TryParse(peliculaIdStr, out peliculaId))
                    {
                        hdnPeliculaId.Value = peliculaId.ToString(); // Store the ID in the hidden field

                        // Populate the form fields with existing movie data
                        CargarDatosPeliculaParaEdicion(peliculaId);
                        litModalTitle.Text = "Editar Película"; // Set modal title for editing
                    }
                    else
                    {
                        // Invalid ID in query string, treat as new or redirect
                        hdnPeliculaId.Value = "0";
                        litModalTitle.Text = "Agregar Película"; // Set modal title for new
                        LimpiarCamposModal();
                    }
                }
                else
                {
                    // --- NEW RECORD MODE ---
                    hdnPeliculaId.Value = "0"; // Ensure it's 0 for new movies
                    litModalTitle.Text = "Agregar Película"; // Set modal title for new
                    LimpiarCamposModal(); // Clear fields just in case
                }

                CargarPeliculas(); // Reload the GridView after setting up the form
            }
        }

        // New helper method to load movie data into the modal for editing
        private void CargarDatosPeliculaParaEdicion(int peliculaId)
        {
            try
            {
                // Use the SOAP client to find movie by ID
                pelicula peli = peliculaServiceClient.buscarPeliculaPorId(peliculaId); // Assuming 'buscarPelicula' exists in your WS

                if (peli != null)
                {
                    txtTituloEs.Text = peli.tituloEs;
                    txtTituloEn.Text = peli.tituloEn;
                    txtDuracionMin.Text = peli.duracionMin.ToString();
                    txtClasificacion.Text = peli.clasificacion;
                    txtSinopsisEs.Text = peli.sinopsisEs;
                    txtSinopsisEn.Text = peli.sinopsisEn;
                    chkEstaActiva.Checked = peli.estaActiva;
                    txtImagenUrl.Text = peli.imagenUrl;

                    // Display the image preview
                    if (!string.IsNullOrEmpty(peli.imagenUrl))
                    {
                        imgPreview.ImageUrl = peli.imagenUrl;
                        imgPreview.Style["display"] = "block";
                    }
                    else
                    {
                        imgPreview.ImageUrl = "";
                        imgPreview.Style["display"] = "none";
                    }
                    // Optionally show the modal after loading data, if you want it to appear immediately on edit page load
                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowEditModal", "showPeliculaModal();", true);
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
                // Use the SOAP client for listing movies
                // Assuming listarPeliculas() takes a boolean for 'incluir_inactivas'
                // If not, adjust accordingly. I'll assume it takes a boolean parameter.
                List<pelicula> peliculas = peliculaServiceClient.listarPeliculas().ToList(); // Example: only active movies
                gvPeliculas.DataSource = peliculas;
                gvPeliculas.DataBind();
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al cargar películas: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar películas: " + ex.ToString());
            }
        }

        protected void gvPeliculas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPeliculas.PageIndex = e.NewPageIndex;
            CargarPeliculas();
        }

        protected void btnAgregarPelicula_Click(object sender, EventArgs e)
        {
            hdnPeliculaId.Value = "0"; // Indicate new record
            litModalTitle.Text = "Agregar Película";
            LimpiarCamposModal();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAddModal", "showPeliculaModal();", true);
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int peliculaId = Convert.ToInt32(btn.CommandArgument);
            hdnPeliculaId.Value = peliculaId.ToString(); // Set the hidden field ID for the modal form
            litModalTitle.Text = "Editar Película";

            CargarDatosPeliculaParaEdicion(peliculaId); // Reuse the loading logic

            // Show the modal after loading data
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowEditModal", "showPeliculaModal();", true);
        }

        protected void btnGuardarPelicula_Click(object sender, EventArgs e)
        {
            Page.Validate("PeliculaValidation");

            if (!Page.IsValid)
            {
                litMensajeModal.Text = "Por favor, corrige los errores en el formulario.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "showPeliculaModal();", true);
                return;
            }

            pelicula peli = new pelicula
            {
                peliculaId = Convert.ToInt32(hdnPeliculaId.Value),
                peliculaIdSpecified = true, // Must be true when sending an explicit ID (even 0 for new)

                tituloEs = txtTituloEs.Text,
                tituloEn = txtTituloEn.Text,
                duracionMin = Convert.ToInt32(txtDuracionMin.Text),
                clasificacion = txtClasificacion.Text,
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
            // System.Diagnostics.Debug.WriteLine($"fechaModificacion: {peli.fechaModificacion}"); // Remove if no longer sent
            // System.Diagnostics.Debug.WriteLine($"fechaModificacionSpecified: {peli.fechaModificacionSpecified}"); // Remove if no longer sent
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
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideModal", "document.getElementById('peliculaModal').style.display='none';", true);
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = $"Error al guardar la película: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "showPeliculaModal();", true);
                System.Diagnostics.Debug.WriteLine("Error al guardar película: " + ex.ToString());
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int peliculaId = Convert.ToInt32(btn.CommandArgument);

            try
            {
                // Assuming eliminarPelicula takes peliculaId and usuarioModificacion
                // You might need to pass the usuarioModificacion here as well
                peliculaServiceClient.eliminarPelicula(peliculaId); // Example: user ID 4

                litMensajeModal.Text = "Película eliminada exitosamente.";
                CargarPeliculas();
            }
            catch (Exception ex)
            {
                litMensajeModal.Text = "Error al eliminar película: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al eliminar película: " + ex.ToString());
            }
        }

        private void LimpiarCamposModal()
        {
            txtTituloEs.Text = string.Empty;
            txtTituloEn.Text = string.Empty;
            txtDuracionMin.Text = string.Empty;
            txtClasificacion.Text = string.Empty;
            txtSinopsisEs.Text = string.Empty;
            txtSinopsisEn.Text = string.Empty;
            txtImagenUrl.Text = string.Empty;

            imgPreview.ImageUrl = "";
            imgPreview.Style["display"] = "none";
            // Assuming hdnExistingImageUrl is for managing image uploads, it should also be cleared.
            // If you don't have this, remove the line.
            // hdnExistingImageUrl.Value = ""; 

            chkEstaActiva.Checked = false;
            litMensajeModal.Text = string.Empty;

            // Clear validation messages
            if (Page.Validators != null)
            {
                foreach (BaseValidator validator in Page.Validators)
                {
                    if (validator != null && validator.ValidationGroup == "PeliculaValidation")
                    {
                        validator.IsValid = true;
                        validator.ErrorMessage = string.Empty; // Clear error message text
                    }
                }
            }
        }
    }
}