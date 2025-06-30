using AutoServicioCineWeb.AutoservicioCineWS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
                var resumen = Session["ResumenCompra"] as ResumenCompra; //Carga los valores que vinieron de la vista Butacas
                if (resumen != null)
                {
                    var master = this.Master as Form; //Para usar el texto que está definido en el Form.Master

                    if (master != null)
                    {
                        master.EntradasAdultoTexto.InnerText = resumen.AdultoTicket;
                        master.EntradasInfantilTexto.InnerText = resumen.InfantilTicket;
                        master.EntradasMayorTexto.InnerText = resumen.MayorTicket;
                        master.TotalResumen.InnerText = resumen.TotalTicket;
                        master.ImgPoster.Src = resumen.ImagenUrl;
                        master.TituloSpan.InnerText = resumen.TituloDePelicula;
                        master.TotalResumen.Attributes["data-base"] = resumen.TotalTicket;
                    }
                }
                funcion funcionSeleccionada = Session["FuncionSeleccionada"] as funcion;
                if (funcionSeleccionada != null)
                {
                    var master = this.Master as Form;

                    if (DateTime.TryParseExact(funcionSeleccionada.fechaHora, "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaHora))
                    {
                        master.FechaSpan.InnerText = fechaHora.ToString("dddd, dd MMMM yyyy", new CultureInfo("es-PE"));
                        master.HoraSpan.InnerText = fechaHora.ToString("hh:mm tt", new CultureInfo("es-PE"));
                    }
                    else
                    {
                        master.HoraSpan.InnerText = funcionSeleccionada.fechaHora;
                        master.FechaSpan.InnerText = "";
                    }
                }
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
            //Esta parte es para enviar los valores que fueron modificados a la siguiente vista (Pago.aspx)

            var master = this.Master as Form;

            var resumen = new ResumenCompra
            {
                AdultoTicket = master.EntradasAdultoTexto.InnerText,
                InfantilTicket = master.EntradasInfantilTexto.InnerText,
                MayorTicket = master.EntradasMayorTexto.InnerText,
                TotalTicket = master.TotalResumen.InnerText,
                TituloDePelicula = master.TituloSpan.InnerText,
                ImagenUrl = master.ImgPoster.Src
            };
            string totalTexto = Request.Form["hfTotal"];
            if (double.TryParse(totalTexto, out double montoFinal))
            {
                resumen.TotalTicket = "S/ " + montoFinal.ToString("0.00");
            }
            string abcd = master.HfTotal.Value;
            resumen.TotalTicket = "S/ " + abcd;
            Session["ResumenCompra"] = resumen;

            //En esta parte se carga la lista de BoletoDetalle
            var detalles = new List<boletoDetalle>();

            // Aquí faltaría definir los precios con lo obtenido en ResumenCompra
            double precioAdulto = 8.5;
            double precioInfantil = 7.0;
            double precioMayor = 7.0;
            // Contador para el número de asiento (cuando se tenga una sala de la base de datos en butaca esto se cambiará)
            int numeroAsiento = 22; //el máximo número es 31 porque hay 22-31 asientos en la base de datos (id)
            // Procesar entrada adulto
            if (!string.IsNullOrWhiteSpace(master.EntradasAdultoTexto.InnerText))
            {
                string[] partes = master.EntradasAdultoTexto.InnerText.Trim().Split(' ');
                if (int.TryParse(partes[0], out int cantidadAdultos)) //El primer caracter que tendrán estas cadenas será la cantidad
                {
                    for (int i = 0; i < cantidadAdultos; i++)
                    {
                        var detalle = new boletoDetalle
                        {
                            tipo = tipoBoleto.ADULTO,
                            precio = precioAdulto,
                            detalleId = 0,
                            tipoSpecified = true,
                            detalleIdSpecified = true,
                            precioSpecified = true,
                            asiento = new asiento
                            {
                                id = numeroAsiento, //se le asigna el número de asiento
                                idSpecified = true
                            }
                        };
                        detalles.Add(detalle);
                        numeroAsiento++;
                    }
                }
            }

            // Procesar entrada infantil
            if (!string.IsNullOrWhiteSpace(master.EntradasInfantilTexto.InnerText))
            {
                string[] partes = master.EntradasInfantilTexto.InnerText.Trim().Split(' ');
                if (int.TryParse(partes[0], out int cantidadInfantiles))
                {
                    for (int i = 0; i < cantidadInfantiles; i++)
                    {
                        var detalle = new boletoDetalle
                        {
                            tipo = tipoBoleto.NIÑO,
                            precio = precioInfantil,
                            detalleId = 0,
                            detalleIdSpecified = true,
                            precioSpecified = true,
                            tipoSpecified = true,
                            asiento = new asiento
                            {
                                id = numeroAsiento,
                                idSpecified = true
                            }
                        };
                        detalles.Add(detalle);
                        numeroAsiento++;
                    }
                }
            }

            // Procesar entrada adulto mayor
            if (!string.IsNullOrWhiteSpace(master.EntradasMayorTexto.InnerText))
            {
                string[] partes = master.EntradasMayorTexto.InnerText.Trim().Split(' ');
                if (int.TryParse(partes[0], out int cantidadMayores))
                {
                    for (int i = 0; i < cantidadMayores; i++)
                    {
                        var detalle = new boletoDetalle
                        {
                            detalleId = 0,
                            tipo = tipoBoleto.ADULTO_MAYOR,
                            precio = precioMayor,
                            detalleIdSpecified = true,
                            precioSpecified = true,
                            tipoSpecified = true,
                            asiento = new asiento
                            {
                                id = numeroAsiento,
                                idSpecified = true,
                            }
                        };
                        detalles.Add(detalle);
                        numeroAsiento++;
                    }
                }
            }

            Session["ListaBoletoDetalle"] = detalles; //Todas las entradas procesadas se guardan para la siguiente vista

            string resumenComidaTexto = hfResumenComida.Value;
            List<ventaProducto> listaVentaProducto = new List<ventaProducto>();
            
            //Procesando las líneas de comida
            if (!string.IsNullOrEmpty(resumenComidaTexto))
            {
                var items = resumenComidaTexto.Split('|');
                foreach (var item in items)
                {
                    var partes = item.Split(';');
                    if (partes.Length == 4)
                    {
                        int productoId;
                        string nombreComida = partes[1];
                        int cantidad;
                        double precioUnitario;

                        if (int.TryParse(partes[0], out productoId) &&
                            int.TryParse(partes[2], out cantidad) &&
                            double.TryParse(partes[3], NumberStyles.Any, CultureInfo.InvariantCulture, out precioUnitario))
                        {
                            var ventaProd = new ventaProducto
                            {
                                cantidad = cantidad,
                                cantidadSpecified = true,
                                precioUnitario = precioUnitario,
                                precioUnitarioSpecified = true,
                                nombreProducto = nombreComida, //esta propiedad o atributo solo se usa en el frontend
                                producto = new producto { id = productoId, idSpecified = true } // vinculación con el producto real
                                // las demás asignaciones se realizarán en la siguiente vista
                            };

                            listaVentaProducto.Add(ventaProd);
                        }
                    }
                }
            }

            // Se guarda la lista en Session para usarla en la vista de pago
            Session["ListaVentaProducto"] = listaVentaProducto;

            Response.Redirect("Pago.aspx");
        }

    }
}