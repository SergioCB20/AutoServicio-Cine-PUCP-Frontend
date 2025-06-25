<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportesAdmin.aspx.cs" Inherits="AutoServicioCineWeb.ReportesAdmin" MasterPageFile="~/Admin.master" %>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Reportes
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/modal.css") %>" /> <%-- Asegúrate de tener este CSS para los modales --%>
    <style>
        /* Estilos específicos para esta página de reportes */
        .report-options {
            display: flex;
            gap: 20px;
            margin-top: 30px;
            justify-content: center; /* Centrar los botones */
        }

        .report-card {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            padding: 30px;
            text-align: center;
            width: 300px;
            transition: transform 0.2s ease-in-out;
            cursor: pointer;
        }

        .report-card:hover {
            transform: translateY(-5px);
        }

        .report-card h3 {
            color: #333;
            margin-bottom: 15px;
            font-size: 1.5em;
        }

        .report-card p {
            color: #666;
            font-size: 0.9em;
            margin-bottom: 20px;
        }

        .report-card .button {
            padding: 10px 20px;
            border-radius: 5px;
            font-size: 1em;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

        /* Estilos para el modal */
        .modal-body {
            padding: 20px;
        }

        .modal-body label {
            display: block;
            margin-bottom: 8px;
            font-weight: bold;
            color: #555;
        }

        .modal-body input[type="date"] {
            width: calc(100% - 20px); /* Ajuste para padding */
            padding: 10px;
            margin-bottom: 15px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 1em;
        }

        .modal-footer {
            display: flex;
            justify-content: flex-end;
            padding: 15px 20px;
            border-top: 1px solid #eee;
            gap: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentPageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    Generación de Reportes
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Genera informes y analiza datos del sistema.
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="report-options">
        <div class="report-card">
            <h3>Reporte de Ventas</h3>
            <p>Genera un informe detallado de las ventas de entradas y productos de comida.</p>
            <button type="button" class="button primary" onclick="openModal('reporteVentasModal')">Generar Reporte</button>
        </div>
        <div class="report-card">
            <h3>Reporte de Logs del Sistema</h3>
            <p>Obtén un registro de las acciones y eventos importantes dentro del sistema.</p>
            <button type="button" class="button primary" onclick="openModal('reporteLogsModal')">Generar Reporte</button>
        </div>
    </div>

    <%-- Modal para Reporte de Ventas (por ahora solo el esqueleto) --%>
    <div id="reporteVentasModal" class="modal">
        <div class="modal-content">
            <span class="close-button" onclick="closeModal('reporteVentasModal')">&times;</span>
            <h2>Generar Reporte de Ventas</h2>
            <div class="modal-body">
                <label for="txtVentasFechaInicio">Fecha de Inicio:</label>
                <input type="date" id="txtVentasFechaInicio" runat="server" class="form-control" />

                <label for="txtVentasFechaFin">Fecha de Fin:</label>
                <input type="date" id="txtVentasFechaFin" runat="server" class="form-control" />
            </div>
            <div class="modal-footer">
                <button type="button" class="button secondary" onclick="closeModal('reporteVentasModal')">Cancelar</button>
                <asp:Button ID="btnGenerarVentasPdf" runat="server" Text="Generar PDF" CssClass="button primary" OnClick="btnGenerarVentasPdf_Click" />
            </div>
        </div>
    </div>

    <%-- Modal para Reporte de Logs del Sistema --%>
    <div id="reporteLogsModal" class="modal">
        <div class="modal-content">
            <span class="close-button" onclick="closeModal('reporteLogsModal')">&times;</span>
            <h2>Generar Reporte de Logs del Sistema</h2>
            <div class="modal-body">
                <label for="txtLogsFechaInicio">Fecha de Inicio:</label>
                <input type="date" id="txtLogsFechaInicio" runat="server" class="form-control" />

                <label for="txtLogsFechaFin">Fecha de Fin:</label>
                <input type="date" id="txtLogsFechaFin" runat="server" class="form-control" />
            </div>
            <div class="modal-footer">
                <button type="button" class="button secondary" onclick="closeModal('reporteLogsModal')">Cancelar</button>
                <asp:Button ID="btnGenerarLogsPdf" runat="server" Text="Generar PDF" CssClass="button primary" OnClick="btnGenerarLogsPdf_Click" />
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="ContentScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        // Funciones para abrir y cerrar modales
        function openModal(modalId) {
            document.getElementById(modalId).style.display = 'block';
        }

        function closeModal(modalId) {
            document.getElementById(modalId).style.display = 'none';
        }

        // Cerrar modal al hacer clic fuera de él
        window.onclick = function (event) {
            const ventasModal = document.getElementById('reporteVentasModal');
            const logsModal = document.getElementById('reporteLogsModal');

            if (event.target == ventasModal) {
                closeModal('reporteVentasModal');
            }
            if (event.target == logsModal) {
                closeModal('reporteLogsModal');
            }
        }
    </script>
</asp:Content>
