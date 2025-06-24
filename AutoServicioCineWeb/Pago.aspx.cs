using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
        private boletoDetalle[] listaBoletoDetalle;
        public Pago()
        {
            boletoServiceClient = new BoletoWSClient();
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
            listaBoletoDetalle = listaDetalle.ToArray();
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
                    id = 4
                },
                fechaHora = null, //la fecha se asignará en el backend
                total = asignar,
                subtotal = asignar * 0.72,
                impuestos = asignar *0.28,
                estado = estadoVenta.COMPLETADA,
                estadoSpecified = true,
                metodoPago = metodoPago.QR,
                metodoPagoSpecified = true,
                impuestosSpecified = true,
                subtotalSpecified = true,
                totalSpecified = true
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
            Response.Redirect("Noexiste.aspx"); //Esta vista no existe, pero por si acaso evita errores inesperados como doble llamada
        }

    }
}