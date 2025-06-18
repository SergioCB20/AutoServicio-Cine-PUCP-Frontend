using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class Comida : System.Web.UI.Page
    {
        private readonly ProductoWSClient productoServiceClient;
        private List<producto> _cachedProductos;
        public Comida()
        {
            productoServiceClient = new ProductoWSClient();
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cargarProductos();
            }

        }
        private void cargarProductos() {
            try
            {
                _cachedProductos = productoServiceClient.listarProductos().ToList();
                List<producto> peliculasFiltradas = _cachedProductos;// FiltrarProductos(_cachedProductos);

                rptComidas.DataSource = peliculasFiltradas;
                rptComidas.DataBind();
                

            }
            catch (System.Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar productos: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar productos: " + ex.ToString());
            }

        }
        /* Ignorar 
        private List<producto> FiltrarProductos(List<producto> productos)
        {
            if (!string.IsNullOrWhiteSpace(txtSearchProductos.Text))
            {
                string searchTerm = txtSearchProductos.Text.Trim().ToLower();
                productos = productos.Where(p =>
                    p.nombre_en.ToLower().Contains(searchTerm) ||
                    p.nombre_es.ToLower().Contains(searchTerm) ||
                    p.descripcion_en.ToLower().Contains(searchTerm) ||
                    p.descripcion_es.ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(ddlClasificacionFilter.SelectedValue))
            {
                string classificationFilter = ddlClasificacionFilter.SelectedValue;
                productos = productos.Where(p => p.tipo.ToString() == classificationFilter).ToList();
            }
            return productos;
        }*/

    }
}