    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;
    

namespace AutoServicioCineWeb
{
    public partial class confirmacionDeCompra : System.Web.UI.Page
    {
        private ventaProducto[] listaProducto;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosResumen();
                ActualizarFechaHora();
                // Opcional: Generar y mostrar número de orden
                orderNumber.Text = GenerarNumeroOrden();
            }
        }

        private void CargarDatosResumen()
        {
            try
            {
                var resumen = Session["ResumenCompra"] as ResumenCompra;
                var listaDetalle = Session["ListaVentaProducto"] as List<ventaProducto>;
               
                if (resumen != null)
                {
                    // Actualizar título de película directamente al control Label
                    if (!string.IsNullOrEmpty(resumen.TituloDePelicula))
                    {
                        movieTitle.Text = resumen.TituloDePelicula;
                    }

                    // Actualizar imagen de película directamente al control Image
                    if (!string.IsNullOrEmpty(resumen.ImagenUrl))
                    {
                        moviePoster.ImageUrl = resumen.ImagenUrl;
                    }

                    // Construir información de tickets y asignarla al Label
                    string ticketInfoContent = ConstruirInfoTickets(resumen);
                    if (!string.IsNullOrEmpty(ticketInfoContent))
                    {
                        ticketInfo.Text = ticketInfoContent;
                    }

                    // Actualizar total directamente al Label
                    if (!string.IsNullOrEmpty(resumen.TotalTicket))
                    {
                        totalAmount.Text = $"{resumen.TotalTicket}";
                    }



      
                    if (listaDetalle != null && listaDetalle.Count > 0)
                    {
                        listaProducto = listaDetalle.ToArray();
                       
                        foodItems.Text = ConstruirInfoProductos(listaDetalle); 

                        var foodItemsContainer = this.FindControl("foodItems") as HtmlGenericControl;
                        if (foodItemsContainer != null)
                        {
                            foodItemsContainer.Controls.Clear(); // Limpiar contenido previo
                            var p = new HtmlGenericControl("p"); // Usamos un <p> para el texto formateado
                            p.InnerHtml = ConstruirInfoProductos(listaDetalle); // Usamos InnerHtml para interpretar <br>
                            foodItemsContainer.Controls.Add(p);
                        }
                    }
                    string numeroDeOrden = orderNumber.Text;
                    string datosParaQR = $"CineAutoservicio-Compra-{numeroDeOrden}";
                    string datosCodificados = Server.UrlEncode(datosParaQR);
                    string qrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={datosCodificados}";
                    qrCode.ImageUrl = qrCodeUrl;



                    if (Session["Boleto"] is boleto boletoInicial) // Usar 'is' para verificar y castear de forma segura
                    {
                        orderStatus.Text = boletoInicial.estado.ToString(); // Convierte el enum a string para mostrarlo
                                                                       // Opcional: Aplicar clase CSS según el estado inicial
                        if (boletoInicial.estado == estadoBoleto.VALIDO)
                        {
                            orderStatus.CssClass = "status-badge status-valid";
                        }
                        else if (boletoInicial.estado == estadoBoleto.USADO)
                        {
                            orderStatus.CssClass = "status-badge status-used";
                        }
                        // Agrega más else if para otros estados si los tienes
                    }

                }
                else
                {
                    // Si no hay datos en sesión, redirigir
                    Response.Redirect("~/Pago.aspx");
                }
                
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos del resumen: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "errorAlert",
                    "alert('Error al cargar los datos de la compra. Por favor, intente nuevamente.');", true);
            }
        }


        protected void qrCodeButton_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // --- AJUSTE AQUÍ: Usar 'is' para verificar y castear de forma segura ---
                if (Session["Boleto"] is boleto boletoRecibido)
                {
                    
                    boletoRecibido.estado = estadoBoleto.USADO;
                    boletoRecibido.estadoSpecified = true; // Mantener si el servicio web lo requiere explícitamente para enums

                    // 2. Actualizar el boleto en la sesión
                    Session["Boleto"] = boletoRecibido;

                    // 3. (Opcional, pero vital para persistencia) Llamar al servicio para actualizar en la DB
                    using (var boletoServiceClient = new BoletoWSClient())
                    {
                        boletoServiceClient.actualizarBoleto(boletoRecibido);
                    }

                    // 4. Actualizar la interfaz de usuario en el servidor usando el valor del enum convertido a string
                    orderStatus.Text = "USADO";//boletoRecibido.estado.ToString()""; // O directamente "UTILIZADO" si lo prefieres literal
                    orderStatus.CssClass = "status-badge status-used"; // Aplica la clase para el estado de utilizado

                    ClientScript.RegisterStartupScript(this.GetType(), "alertSuccess",
                        "alert('¡Código QR escaneado exitosamente!\\nEstado cambiado a: UTILIZADO');", true);

                }
                else // Este bloque se ejecutará si Session["BoletoConfirmacion"] es null o no es un 'boleto'
                {
                    System.Diagnostics.Debug.WriteLine("Error: No se encontró o el objeto de boleto en la sesión no es válido al intentar actualizar.");
                    ClientScript.RegisterStartupScript(this.GetType(), "errorUpdate", "alert('Error: La información del boleto no está disponible o no es válida.');", true);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción al actualizar boleto: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "exceptionAlert", "alert('Ocurrió un error inesperado al actualizar el boleto. Intente de nuevo.');", true);
            }
        }

        private string ConstruirInfoProductos(List<ventaProducto> listaDetalle)
        {
            var productParts = new List<string>();
            foreach (var item in listaDetalle)
            {
                productParts.Add($"{item.nombreProducto} x {item.cantidad} = S/ {(item.precioUnitario * item.cantidad).ToString("0.00")}");
            }
            return string.Join("<br>", productParts);
        }
      
        private string ConstruirInfoTickets(ResumenCompra resumen)
        {
            var ticketParts = new List<string>();

            if (!string.IsNullOrEmpty(resumen.AdultoTicket) && resumen.AdultoTicket != "0")
            {
                ticketParts.Add($"{resumen.AdultoTicket} Adultos");
            }
            if (!string.IsNullOrEmpty(resumen.InfantilTicket) && resumen.InfantilTicket != "0")
            {
                ticketParts.Add($"{resumen.InfantilTicket} Niños");
            }
            if (!string.IsNullOrEmpty(resumen.MayorTicket) && resumen.MayorTicket != "0")
            {
                ticketParts.Add($"{resumen.MayorTicket} Adultos Mayores");
            }

            string ticketInfoString = string.Join("<br>", ticketParts);

            
            return ticketInfoString;
        }


        private void ActualizarFechaHora()
        {
            // Puedes asignar la fecha y hora directamente al Label en el code-behind
            string fechaHora = DateTime.Now.ToString("dd 'de' MMMM, yyyy - HH:mm",
                new System.Globalization.CultureInfo("es-ES"));
            purchaseDateTime.Text = fechaHora;

    
        }

        // Método para generar número de orden único (opcional)
        private string GenerarNumeroOrden()
        {
            return $"#CP-{DateTime.Now.Year}-{DateTime.Now.Ticks.ToString().Substring(10, 6)}"; // Substring para un número más corto
        }

        // Método para limpiar la sesión después de mostrar la confirmación (opcional)
        protected void LimpiarSesion()
        {
            if (Session["ResumenCompra"] != null)
            {
                Session.Remove("ResumenCompra");
            }
        }

        
    }
}