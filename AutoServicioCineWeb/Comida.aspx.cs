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
        private void cargarProductos()
        {
            try
            {
                _cachedProductos = productoServiceClient.listarProductos().ToList();
                List<producto> listaproductos = _cachedProductos;// FiltrarProductos(_cachedProductos);

                rptComidas.DataSource = listaproductos;
                rptComidas.DataBind();
                //string script = "";
                //foreach (var prod in listaproductos)
                //{
                //    string id = prod.id.ToString();
                //    string precio = prod.precio.ToString(CultureInfo.InvariantCulture); // Usa "." como separador decimal
                //    script += $"registrarPrecio('{id}', {precio});";
                //}
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "inicializarPrecios", $"<script>{script}</script>");
                string script = "window.onload = function() {";
                foreach (var prod in listaproductos)
                {
                    string id = prod.id.ToString();
                    string precio = prod.precio.ToString(CultureInfo.InvariantCulture);
                    script += $"registrarPrecio('{id}', {precio});";
                }
                script += "};";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "initPrecios", $"<script>{script}</script>");


            }
            catch (System.Exception ex)
            {
                // Este mensaje se mostrará en litMensajeModal, que está en el modal de edición/registro.
                // Podrías considerar un Literal en la página principal para errores globales.
                litMensajeModal.Text = "Error al cargar productos: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Error al cargar productos: " + ex.ToString());
            }

        }
        protected void btnContinuar_Click(object sender, EventArgs e)
        {


            Response.Redirect("Pago.aspx");
        }
    }
}