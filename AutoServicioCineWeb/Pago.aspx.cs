using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.Schema;
using AutoServicioCineWeb.AutoservicioCineWS;
using Microsoft.Ajax.Utilities;

namespace AutoServicioCineWeb
{
    public partial class Pago : System.Web.UI.Page
    {
        private readonly BoletoWSClient boletoServiceClient;
        private readonly CuponWSClient cuponServiceClient;
        private boletoDetalle[] listaBoletoDetalle;
        private ventaProducto[] listaProducto;
        private List<cupon> listaCupon;
        public Pago()
        {
            boletoServiceClient = new BoletoWSClient();
            cuponServiceClient = new CuponWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var resumen = Session["ResumenCompra"] as ResumenCompra; //Carga los valores que vinieron de la vista Comida
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
                    
                }
            }
            var listaDetalle = Session["ListaBoletoDetalle"] as List<boletoDetalle>;
            if (listaDetalle != null)
            {
                listaBoletoDetalle = listaDetalle.ToArray();
            }

            // Obtener los añadidos de comida
            var listaVentaProducto = Session["ListaVentaProducto"] as List<ventaProducto>;
            
            if (listaVentaProducto != null && listaVentaProducto.Count > 0)
            {
                listaProducto = listaVentaProducto.ToArray(); //se guarda en el arreglo para usarlo después
                // Buscar el div dentro del master
                var resumenDiv = this.Master.FindControl("resumenCompraComida") as HtmlGenericControl;

                if (resumenDiv != null)
                {
                    foreach (var item in listaVentaProducto)
                    {
                        string texto = $"{item.nombreProducto} x {item.cantidad} = S/ {(item.precioUnitario * item.cantidad).ToString("0.00")}";
                        var p = new HtmlGenericControl("p");
                        p.InnerText = texto;
                        resumenDiv.Controls.Add(p);
                    }
                }
            }

            listaCupon = cuponServiceClient.listarCupones().ToList();
        }

        protected void btnAplicarCodigo_Click(object sender, EventArgs e)
        {
            string codigoIngresado = txtCodigoPromocional.Text.Trim().ToUpper();

            // Buscar cupon válido (que coincida el código y esté en fecha)
            var cupon = listaCupon.FirstOrDefault(c =>
                c.codigo != null &&
                c.codigo.Trim().ToUpper() == codigoIngresado &&
                //(c.fechaInicio == null || c.fechaInicio <= DateTime.Today) &&
                //(c.fechaFin == null || c.fechaFin >= DateTime.Today) &&
                (c.usosActuales < c.maxUsos)
            );

            var master = this.Master as Form;

            if (master != null)
            {
                double total;
                if (!double.TryParse(master.HfTotal.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out total))
                {
                    total = 0;
                }
                if (cupon != null)
                {
                    string tipo = cupon.descuentoTipo.ToString();
                    double valor = cupon.descuentoValor;

                    if (tipo == "PORCENTAJE")
                    {
                        lblMensajeCodigo.Text = $"Código aplicado: {valor}% de descuento";
                        total -= total * (cupon.descuentoValor) / 100.0;

                    }
                    else if (tipo == "FIJO")
                    {
                        lblMensajeCodigo.Text = $"Código aplicado: S/ {valor} de descuento";
                        total -= (cupon.descuentoValor);
                    }
                    else
                    {
                        lblMensajeCodigo.Text = "Código válido pero tipo de descuento desconocido";
                    }
                    if (total < 0) total = 0;

                    lblMensajeCodigo.ForeColor = System.Drawing.Color.Green;
                    master.HfTotal.Value = total.ToString("F2", CultureInfo.InvariantCulture);
                    master.TotalResumen.InnerText = "S/ " + total.ToString("F2", CultureInfo.InvariantCulture);
                    // Se puede guardar el cupón en Session para usarlo luego
                    //Session["CuponAplicado"] = cupon;
                }
                else
                {
                    lblMensajeCodigo.Text = "Código no válido o expirado";
                    lblMensajeCodigo.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnGenerarQR_Click(object sender, EventArgs e)
        {
            pnlQR.Visible = true;

            // Desactivar el campo de código y el botón
            txtCodigoPromocional.Enabled = false;
            btnAplicarCodigo.Enabled = false;
            btnGenerarQR.Enabled = false;
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            double asignar = new double();
            var master = this.Master as Form;
            string totalTexto = master.TotalResumen.InnerText;
            double montoTotal;
            if (double.TryParse(totalTexto.Replace("S/", "").Trim(), out montoTotal))
            {
                asignar = montoTotal;
            }


            var nuevaVenta = new venta
            {
                ventaId = 0,
                ventaIdSpecified = true,
                usuario = new usuario
                {
                    id = 13 //usuario disponible de la base de datos
                },
                fechaHora = null, //la fecha se asignará en el backend
                total = asignar,
                subtotal = asignar * 0.72,
                impuestos = asignar * 0.28,
                estado = estadoVenta.COMPLETADA,
                estadoSpecified = true,
                metodoPago = metodoPago.QR,
                metodoPagoSpecified = true,
                impuestosSpecified = true,
                subtotalSpecified = true,
                totalSpecified = true,
                productosVendidos = listaProducto
            };

            var nuevoBoleto = new boleto
            {
                boletoId = 0,
                boletoIdSpecified = true,
                venta = nuevaVenta, //se le asigna la venta relacionada
                funcion = new funcion
                {
                    funcionId = 4, //es la única función disponible de momento /////// CAMBIAR CUANDO SE TENGA LA VISTA DE FUNCIONES
                    funcionIdSpecified = true
                },
                estado = estadoBoleto.VALIDO,
                estadoSpecified = true,
                detalles = listaBoletoDetalle
            };
            boletoServiceClient.registrarBoleto(nuevoBoleto);
            System.Diagnostics.Debug.WriteLine("Ejecutando btnContinuar_Click a las: " + DateTime.Now); //Línea para verificar los llamados
            Response.Redirect("confirmacionDeCompra.aspx");
        }

    }
}