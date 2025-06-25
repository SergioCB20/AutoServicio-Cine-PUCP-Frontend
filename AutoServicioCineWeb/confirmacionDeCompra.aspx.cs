using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;

namespace AutoServicioCineWeb
{
    public partial class confirmacionDeCompra : System.Web.UI.Page
    {
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
                        totalAmount.Text = $"S/ {resumen.TotalTicket}";
                    }

                    // Actualizar datos de comida
                    ActualizarDatosComida(resumen);
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

            // Asumiendo que Horario es una propiedad de ResumenCompra
            /*
             if (!string.IsNullOrEmpty(resumen.Horario)) // Si Horario es una propiedad directamente en ResumenCompra
            {
                ticketInfoString += $"<br>Función: {resumen.Horario}";
            }
            else
            {
                ticketInfoString += "<br>Función: 18:00 hrs"; // Valor por defecto si no hay horario
            }
            */
            return ticketInfoString;
        }

        private void ActualizarDatosComida(ResumenCompra resumen)
        {
            /*
            // Asumiendo que ComidaItems es una propiedad de ResumenCompra
            if (!string.IsNullOrEmpty(resumen.))
            {
                foodItems.Text = resumen.ListaVentaProducto;
            }
            */
        }

        private void ActualizarFechaHora()
        {
            // Puedes asignar la fecha y hora directamente al Label en el code-behind
            string fechaHora = DateTime.Now.ToString("dd 'de' MMMM, yyyy - HH:mm",
                new System.Globalization.CultureInfo("es-ES"));
            purchaseDateTime.Text = fechaHora;

            // Opcional: Si quieres que el JavaScript del cliente también actualice,
            // podrías mantener la llamada a updateDateTime() en el cliente
            // pero el valor inicial lo asigna C# al cargar la página.
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

        // Ya no es necesario un override de Render para los scripts de actualización de contenido
        // porque estamos manipulando directamente los controles de servidor.
        // Si necesitas registrar scripts para otras cosas, puedes mantenerlo.
        // protected override void Render(HtmlTextWriter writer)
        // {
        //     ClientScript.RegisterStartupScript(this.GetType(), "initPage",
        //         "console.log('Página de confirmación cargada exitosamente');", true);
        //     base.Render(writer);
        // }
    }
}