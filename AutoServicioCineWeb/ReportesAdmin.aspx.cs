using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO; // Para MemoryStream
using iTextSharp.text; // iTextSharp
using iTextSharp.text.pdf;
using AutoServicioCineWeb.AutoservicioCineWS; // iTextSharp

// Asumiendo que tienes una referencia al servicio web o una clase que mapea el modelo Java
// Si estás usando un servicio web, tendrías una referencia como:
// using AutoServicioCineWeb.AutoservicioCineWS; // Sustituye por el nombre real de tu referencia de servicio
// Si es un API REST, necesitarás tus clases de modelos C# equivalentes o usar dynamic/JObject

namespace AutoServicioCineWeb
{

    public partial class ReportesAdmin : System.Web.UI.Page
    {
        private readonly LogWSClient logClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Configuración inicial si es necesario
            }
        }

        protected void btnGenerarVentasPdf_Click(object sender, EventArgs e)
        {
            // Lógica para generar reporte de ventas
            // Por ahora, solo un mensaje de demostración
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSalesNotification",
                "showNotification('Reporte de Ventas en desarrollo.', 'info'); closeModal('reporteVentasModal');", true);
        }

        protected void btnGenerarLogsPdf_Click(object sender, EventArgs e)
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            // Validar y obtener fechas del modal  
            if (!DateTime.TryParse(txtLogsFechaInicio.Value, out fechaInicio))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationFailed",
                    "showNotification('Por favor, ingresa una fecha de inicio válida.', 'error');", true);
                return;
            }

            if (!DateTime.TryParse(txtLogsFechaFin.Value, out fechaFin))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationFailed",
                    "showNotification('Por favor, ingresa una fecha de fin válida.', 'error');", true);
                return;
            }

            if (fechaInicio > fechaFin)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationFailed",
                    "showNotification('La fecha de inicio no puede ser posterior a la fecha de fin.', 'error');", true);
                return;
            }


            try
            {
                List<logSistema> logs = logClient.listarLogs().ToList();

                // Filtrar logs por rango de fechas
                logs = logs.Where(l => ConvertToDateTime(l.fecha) >= fechaInicio && ConvertToDateTime(l.fecha) <= fechaFin).ToList();

                if (logs.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "NoData",
                        "showNotification('No se encontraron logs para el rango de fechas seleccionado.', 'info');", true);
                    return;
                }

                // Generar PDF con iTextSharp
                GenerarPdfLogs(logs, fechaInicio, fechaFin);

                ScriptManager.RegisterStartupScript(this, GetType(), "ReportSuccess",
                    "showNotification('Reporte de Logs generado exitosamente.', 'success'); closeModal('reporteLogsModal');", true);
            }
            catch (System.Exception ex)
            {
                // Manejo de errores si la llamada al servicio falla
                ScriptManager.RegisterStartupScript(this, GetType(), "ReportError",
                    $"showNotification('Error al generar el reporte: {ex.Message.Replace("'", "")}', 'error');", true);
            }
        }

        private DateTime ConvertToDateTime(localDate localDateValue)
        {
            if (localDateValue is null)
            {
                throw new ArgumentNullException(nameof(localDateValue), "El valor de localDate no puede ser nulo.");
            }

            // Asumiendo que localDate tiene una propiedad para obtener la fecha completa como un string o DateTime.  
            // Si no tiene propiedades como Year, Month, Day, se debe usar una propiedad existente que represente la fecha completa.  
            // Ejemplo: localDateValue.Date o localDateValue.ToString()  

            // Si localDate tiene una propiedad que representa la fecha completa:  
            DateTime parsedDate;
            if (DateTime.TryParse(localDateValue.ToString(), out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                throw new FormatException("El valor de localDate no se pudo convertir a DateTime.");
            }
        }

        private void GenerarPdfLogs(List<logSistema> logs, DateTime fechaInicio, DateTime fechaFin)
        {
            // Crear un nuevo documento PDF
            Document document = new Document(PageSize.A4, 25, 25, 25, 25);
            MemoryStream ms = new MemoryStream();

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fuente para el título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

                // Título del documento
                Paragraph title = new Paragraph("Reporte de Logs del Sistema Cine PUCP", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                document.Add(title);

                // Rango de fechas
                Paragraph dateRange = new Paragraph($"Periodo: {fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}", cellFont);
                dateRange.Alignment = Element.ALIGN_CENTER;
                dateRange.SpacingAfter = 15f;
                document.Add(dateRange);

                // Crear tabla para los logs
                PdfPTable table = new PdfPTable(4); // 4 columnas: ID, Acción, Fecha, Usuario ID
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 0.8f, 3f, 1.5f, 1.2f }); // Proporciones de ancho de columna
                table.SpacingBefore = 10f;

                // Añadir cabeceras de la tabla
                PdfPCell cell;
                BaseColor headerBgColor = new BaseColor(52, 152, 219); // Azul para cabecera

                cell = new PdfPCell(new Phrase("ID", headerFont));
                cell.BackgroundColor = headerBgColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Acción", headerFont));
                cell.BackgroundColor = headerBgColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Fecha", headerFont));
                cell.BackgroundColor = headerBgColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("ID Usuario", headerFont));
                cell.BackgroundColor = headerBgColor;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);

                // Añadir filas de datos
                foreach (var log in logs)
                {
                    cell = new PdfPCell(new Phrase(log.id.ToString(), cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(log.accion, cellFont));
                    cell.Padding = 5;
                    table.AddCell(cell);

                    // Convertir localDate a DateTime antes de usar ToShortDateString
                    DateTime logFecha = ConvertToDateTime(log.fecha);
                    cell = new PdfPCell(new Phrase(logFecha.ToShortDateString(), cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(log.usuario.ToString(), cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    table.AddCell(cell);
                }

                document.Add(table);
            }
            catch (System.Exception ex)
            {
                // Manejar cualquier error durante la creación del PDF
                throw new System.Exception("Error al generar el PDF de logs.", ex);
            }
            finally
            {
                document.Close();
            }

            // Enviar el PDF al navegador
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename=ReporteLogs_{fechaInicio.ToString("yyyyMMdd")}_{fechaFin.ToString("yyyyMMdd")}.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }
    }
}