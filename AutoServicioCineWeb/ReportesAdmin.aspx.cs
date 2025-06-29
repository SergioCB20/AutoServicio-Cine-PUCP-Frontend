using AutoServicioCineWeb.AutoservicioCineWS;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class ReportesAdmin : System.Web.UI.Page
    {
        private readonly VentaWSClient ventaServiceCliente;
        private readonly LogWSClient logServiceCliente;
     
        public ReportesAdmin()
        {
            // Inicializar los clientes de servicio web
            ventaServiceCliente = new VentaWSClient();
            logServiceCliente = new LogWSClient();
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Inicializar fechas por defecto si es necesario
                // Por ejemplo, último mes
                DateTime fechaFin = DateTime.Today;
                DateTime fechaInicio = fechaFin.AddDays(-30);

                txtVentasFechaInicio.Value = fechaInicio.ToString("yyyy-MM-dd");
                txtVentasFechaFin.Value = fechaFin.ToString("yyyy-MM-dd");

                txtLogsFechaInicio.Value = fechaInicio.ToString("yyyy-MM-dd");
                txtLogsFechaFin.Value = fechaFin.ToString("yyyy-MM-dd");
            }
        }

        protected void btnGenerarVentasPdf_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar fechas del lado servidor
                if (string.IsNullOrEmpty(txtVentasFechaInicio.Value) || string.IsNullOrEmpty(txtVentasFechaFin.Value))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('Por favor, selecciona ambas fechas.', 'error');", true);
                    return;
                }

                DateTime fechaInicio, fechaFin;
                if (!DateTime.TryParse(txtVentasFechaInicio.Value, out fechaInicio) ||
                    !DateTime.TryParse(txtVentasFechaFin.Value, out fechaFin))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('Formato de fecha inválido.', 'error');", true);
                    return;
                }

                if (fechaInicio > fechaFin)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('La fecha de inicio no puede ser posterior a la fecha de fin.', 'error');", true);
                    return;
                }

                // Convertir fechas al formato requerido por las funciones (AAAA-MM-DD)
                string fechaInicioStr = fechaInicio.ToString("yyyy-MM-dd");
                string fechaFinStr = fechaFin.ToString("yyyy-MM-dd");

                // Obtener datos del reporte de ventas
                var datosVentas = ventaServiceCliente.listarVentasReporte(fechaInicioStr, fechaFinStr).ToList();

                if (datosVentas == null || datosVentas.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                        "showNotification('No se encontraron datos de ventas para el período seleccionado.', 'warning');", true);
                    return;
                }

                // Generar PDF
                GenerarPDFVentas(datosVentas, fechaInicio, fechaFin);

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "showNotification('Reporte de ventas generado exitosamente.', 'success'); closeModal('reporteVentasModal');", true);
            }
            catch (System.Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"showNotification('Error al generar el reporte: {ex.Message}', 'error');", true);
            }
        }

        protected void btnGenerarLogsPdf_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar fechas del lado servidor
                if (string.IsNullOrEmpty(txtLogsFechaInicio.Value) || string.IsNullOrEmpty(txtLogsFechaFin.Value))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('Por favor, selecciona ambas fechas.', 'error');", true);
                    return;
                }

                DateTime fechaInicio, fechaFin;
                if (!DateTime.TryParse(txtLogsFechaInicio.Value, out fechaInicio) ||
                    !DateTime.TryParse(txtLogsFechaFin.Value, out fechaFin))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('Formato de fecha inválido.', 'error');", true);
                    return;
                }

                if (fechaInicio > fechaFin)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "showNotification('La fecha de inicio no puede ser posterior a la fecha de fin.', 'error');", true);
                    return;
                }

                // Convertir fechas al formato requerido por las funciones (AAAA-MM-DD)
                string fechaInicioStr = fechaInicio.ToString("yyyy-MM-dd");
                string fechaFinStr = fechaFin.ToString("yyyy-MM-dd");

                // Obtener datos del reporte de logs
                var datosLogs = logServiceCliente.listarLogs(fechaInicioStr, fechaFinStr).ToList();

                if (datosLogs == null || datosLogs.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                        "showNotification('No se encontraron logs para el período seleccionado.', 'warning');", true);
                    return;
                }

                // Generar PDF
                GenerarPdfLogs(datosLogs, fechaInicio, fechaFin);

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "showNotification('Reporte de logs generado exitosamente.', 'success'); closeModal('reporteLogsModal');", true);
            }
            catch (System.Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"showNotification('Error al generar el reporte: {ex.Message}', 'error');", true);
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
        private void GenerarPDFVentas(List<venta> datos, DateTime fechaInicio, DateTime fechaFin)
        {
            // Crear documento PDF
            Document documento = new Document(PageSize.A4, 50, 50, 25, 25);

            // Crear el stream de memoria para el PDF
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(documento, stream);
                documento.Open();

                // Título del documento
                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

                // Agregar título
                Paragraph titulo = new Paragraph("REPORTE DE VENTAS", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingAfter = 10f;
                documento.Add(titulo);

                // Agregar período
                Paragraph periodo = new Paragraph($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", subtituloFont);
                periodo.Alignment = Element.ALIGN_CENTER;
                periodo.SpacingAfter = 20f;
                documento.Add(periodo);

                // Crear tabla
                PdfPTable tabla = new PdfPTable(5); // Ajustar según las columnas de tu ReporteVenta
                tabla.WidthPercentage = 100f;
                tabla.SetWidths(new float[] { 15f, 20f, 15f, 25f, 25f }); // Ajustar según necesidades

                // Headers de la tabla
                string[] headers = { "Fecha", "Usuario", "Subtotal", "Impuesto", "Total" };
                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = BaseColor.DARK_GRAY;
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 8f;
                    tabla.AddCell(headerCell);
                }

                // Agregar datos
                double totalGeneral = 0;
                foreach (var venta in datos)
                {                    
                    tabla.AddCell(new PdfPCell(new Phrase(venta.fechaHora, cellFont)) { Padding = 5f });
                    tabla.AddCell(new PdfPCell(new Phrase(venta.usuario.id.ToString(), cellFont)) { Padding = 5f });
                    tabla.AddCell(new PdfPCell(new Phrase(venta.subtotal.ToString("C"), cellFont)) { Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    tabla.AddCell(new PdfPCell(new Phrase(venta.impuestos.ToString("C"), cellFont)) { Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });
                    tabla.AddCell(new PdfPCell(new Phrase(venta.total.ToString("C"), cellFont)) { Padding = 5f, HorizontalAlignment = Element.ALIGN_RIGHT });

                    totalGeneral += venta.total;
                }

                // Fila de total
                PdfPCell totalLabelCell = new PdfPCell(new Phrase("TOTAL GENERAL", headerFont));
                totalLabelCell.Colspan = 4;
                totalLabelCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalLabelCell.Padding = 8f;
                tabla.AddCell(totalLabelCell);

                PdfPCell totalValueCell = new PdfPCell(new Phrase(totalGeneral.ToString("C"), headerFont));
                totalValueCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalValueCell.Padding = 8f;
                tabla.AddCell(totalValueCell);

                documento.Add(tabla);

                // Agregar información adicional
                Paragraph info = new Paragraph($"\nReporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}", subtituloFont);
                info.Alignment = Element.ALIGN_RIGHT;
                info.SpacingBefore = 20f;
                documento.Add(info);

                documento.Close();
                // Enviar el PDF al navegador
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename=ReporteVentas_{fechaInicio.ToString("yyyyMMdd")}_{fechaFin.ToString("yyyyMMdd")}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(stream.ToArray());
                Response.End();

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
                    
                    cell = new PdfPCell(new Phrase(log.fecha, cellFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(log.id_usuario.ToString(), cellFont));
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