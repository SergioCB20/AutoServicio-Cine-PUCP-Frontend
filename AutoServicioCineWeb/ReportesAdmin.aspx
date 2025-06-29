<%@ Page Title="Genera Reportes" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="ReportesAdmin.aspx.cs" Inherits="AutoServicioCineWeb.ReportesAdmin" %>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Reportes
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/modal.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/reportes.css") %>" />
</asp:Content>

<asp:Content ID="ContentPageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    Generación de Reportes
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Genera informes y analiza datos del sistema de cine.
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="reports-container">
        <div class="reports-grid">
            <!-- Tarjeta de Reporte de Ventas -->
            <div class="report-card sales-card">
                <div class="report-icon">
                    <i class="icon-sales">💰</i>
                </div>
                <h3>Reporte de Ventas</h3>
                <p>Genera un informe detallado de las ventas de entradas y productos de comida en un período específico.</p>
           
                <button type="button" class="btn btn-primary" onclick="openModal('reporteVentasModal')">
                    <span class="btn-icon">📄</span>
                    Generar Reporte
                </button>
            </div>

            <!-- Tarjeta de Reporte de Logs -->
            <div class="report-card logs-card">
                <div class="report-icon">
                    <i class="icon-logs">📝</i>
                </div>
                <h3>Reporte de Logs del Sistema</h3>
                <p>Obtén un registro detallado de las acciones y eventos importantes dentro del sistema de administración.</p>
               
                <button type="button" class="btn btn-primary" onclick="openModal('reporteLogsModal')">
                    <span class="btn-icon">📋</span>
                    Generar Reporte
                </button>
            </div>
        </div>
    </div>

    <!-- Modal para Reporte de Ventas -->
    <div id="reporteVentasModal" class="modal">
        <div class="modal-content">
            <div class="modal-header">
                <h2>
                    <span class="modal-icon">💰</span>
                    Generar Reporte de Ventas
                </h2>
                <span class="close-button" onclick="closeModal('reporteVentasModal')">&times;</span>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtVentasFechaInicio">
                        <span class="label-icon">📅</span>
                        Fecha de Inicio:
                    </label>
                    <input type="date" id="txtVentasFechaInicio" runat="server" class="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtVentasFechaFin">
                        <span class="label-icon">📅</span>
                        Fecha de Fin:
                    </label>
                    <input type="date" id="txtVentasFechaFin" runat="server" class="form-control" />
                </div>

                <div class="info-box">
                    <span class="info-icon">ℹ️</span>
                    <p>El reporte incluirá todas las ventas realizadas en el período seleccionado, incluyendo entradas y productos de comida.</p>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" onclick="closeModal('reporteVentasModal')">
                    <span class="btn-icon">❌</span>
                    Cancelar
                </button>
                <asp:Button ID="btnGenerarVentasPdf" runat="server" Text="Generar PDF" CssClass="btn btn-success" 
                    OnClick="btnGenerarVentasPdf_Click" OnClientClick="return validateDateRange('txtVentasFechaInicio', 'txtVentasFechaFin')" />
            </div>
        </div>
    </div>

    <!-- Modal para Reporte de Logs del Sistema -->
    <div id="reporteLogsModal" class="modal">
        <div class="modal-content">
            <div class="modal-header">
                <h2>
                    <span class="modal-icon">📝</span>
                    Generar Reporte de Logs del Sistema
                </h2>
                <span class="close-button" onclick="closeModal('reporteLogsModal')">&times;</span>
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label for="txtLogsFechaInicio">
                        <span class="label-icon">📅</span>
                        Fecha de Inicio:
                    </label>
                    <input type="date" id="txtLogsFechaInicio" runat="server" class="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtLogsFechaFin">
                        <span class="label-icon">📅</span>
                        Fecha de Fin:
                    </label>
                    <input type="date" id="txtLogsFechaFin" runat="server" class="form-control" />
                </div>

                <div class="info-box">
                    <span class="info-icon">ℹ️</span>
                    <p>El reporte incluirá todos los logs de sistema registrados en el período seleccionado, mostrando acciones de usuarios y eventos del sistema.</p>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" onclick="closeModal('reporteLogsModal')">
                    <span class="btn-icon">❌</span>
                    Cancelar
                </button>
                <asp:Button ID="btnGenerarLogsPdf" runat="server" Text="Generar PDF" CssClass="btn btn-success" 
                    OnClick="btnGenerarLogsPdf_Click" OnClientClick="return validateDateRange('txtLogsFechaInicio', 'txtLogsFechaFin')" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        // Funciones para abrir y cerrar modales
        function openModal(modalId) {
            const modal = document.getElementById(modalId);
            modal.style.display = 'block';
            setTimeout(() => {
                modal.classList.add('show');
            }, 10);
        }

        function closeModal(modalId) {
            const modal = document.getElementById(modalId);
            modal.classList.remove('show');
            setTimeout(() => {
                modal.style.display = 'none';
            }, 300);
        }

        // Validación de rango de fechas
        function validateDateRange(startId, endId) {
            const startInput = document.getElementById(startId);
            const endInput = document.getElementById(endId);
            
            if (!startInput.value || !endInput.value) {
                showNotification('Por favor, selecciona ambas fechas.', 'warning');
                return false;
            }
            
            const startDate = new Date(startInput.value);
            const endDate = new Date(endInput.value);
            
            if (startDate > endDate) {
                showNotification('La fecha de inicio no puede ser posterior a la fecha de fin.', 'error');
                return false;
            }
            
            const today = new Date();
            if (startDate > today || endDate > today) {
                showNotification('Las fechas no pueden ser futuras.', 'warning');
                return false;
            }
            
            return true;
        }

        // Función para mostrar notificaciones (asumiendo que existe)
        function showNotification(message, type) {
            // Esta función debe estar definida en tu sistema de notificaciones
            console.log(`${type.toUpperCase()}: ${message}`);
            alert(`${type.toUpperCase()}: ${message}`); // Fallback básico
        }

        // Cerrar modal al hacer clic fuera de él
        window.onclick = function (event) {
            const modals = document.querySelectorAll('.modal');
            modals.forEach(modal => {
                if (event.target === modal) {
                    closeModal(modal.id);
                }
            });
        }

        // Cerrar modal con tecla Escape
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                const openModals = document.querySelectorAll('.modal[style*="block"]');
                openModals.forEach(modal => {
                    closeModal(modal.id);
                });
            }
        });

        // Establecer fecha máxima como hoy en los inputs de fecha
        document.addEventListener('DOMContentLoaded', function() {
            const today = new Date().toISOString().split('T')[0];
            const dateInputs = document.querySelectorAll('input[type="date"]');
            dateInputs.forEach(input => {
                input.max = today;
            });
        });
    </script>
</asp:Content>