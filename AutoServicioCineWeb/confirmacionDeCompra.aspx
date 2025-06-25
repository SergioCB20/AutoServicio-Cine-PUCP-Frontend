

<%@ Page Title="confirmación de Compra" Language="C#" AutoEventWireup="true" CodeBehind="confirmacionDeCompra.aspx.cs" Inherits="AutoServicioCineWeb.confirmacionDeCompra" %>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Confirmación de Pago - CinePop</title>
    <link rel="stylesheet" href="./styles/confirmacionDePago.css">
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h1>¡Pago Confirmado!</h1>
                <p>Tu compra ha sido procesada exitosamente</p>
            </div>

            <div class="content">
                <div class="movie-section">
                    <div class="movie-poster">
                        <%-- Aquí se usará Image en lugar de img para que C# pueda controlar el src --%>
                        <asp:Image ID="moviePoster" runat="server" ImageUrl="https://images.unsplash.com/photo-1489599763306-b1b2c5b1b1b1?w=300&h=450&fit=crop" AlternateText="Película" CssClass="movie-poster-img" />
                    </div>
                    <div class="movie-info">
                        <%-- Usamos un Label para el título de la película --%>
                        <asp:Label ID="movieTitle" runat="server" CssClass="movie-title">Una película de objetos flotantes</asp:Label>
                    </div>
                </div>

                <div class="details-grid">
                    <div class="detail-card">
                        <div class="detail-label">Fecha y Hora de Compra</div>
                        <%-- Usamos un Label para la fecha y hora de compra --%>
                        <asp:Label ID="purchaseDateTime" runat="server" CssClass="detail-value">25 de Junio, 2025 - 14:32</asp:Label>
                    </div>

                    <div class="detail-card">
                        <div class="detail-label">Cantidad y Tipo de Entradas</div>
                        <%-- Usamos un Label para la información de tickets --%>
                        <asp:Label ID="ticketInfo" runat="server" CssClass="detail-value">
                            2 Adultos<br />
                            Función: 18:00 hrs
                        </asp:Label>
                    </div>

                    <div class="detail-card">
                        <div class="detail-label">Añadidos de Comida</div>
                        <%-- Usamos un Label para los ítems de comida --%>
                        <asp:Label ID="foodItems" runat="server" CssClass="detail-value">
                            2x Combo Grande<br />
                            1x Nachos con Queso
                        </asp:Label>
                    </div>

                    <div class="detail-card">
                        <div class="detail-label">Estado del Pedido</div>
                        <div class="detail-value">
                            <%-- Usamos un Label para el estado del pedido, la clase CSS se puede asignar desde C# si es dinámico --%>
                            <asp:Label ID="orderStatus" runat="server" CssClass="status-badge status-paid">Pagado</asp:Label>
                        </div>
                    </div>
                </div>

                <div class="total-section">
                    <%-- Usamos un Label para el total pagado --%>
                    <asp:Label ID="totalAmount" runat="server" CssClass="total-amount">S/ 85.50</asp:Label>
                    <div class="total-label">Total Pagado</div>
                </div>

                <div class="qr-section">
                    <div class="qr-title">Código QR de Entrada</div>
                    <div class="qr-instruction">Escanea este código en el cine para validar tu entrada</div>
                    
                    <%-- El QR se mantiene como div para la animación JavaScript --%>
                    <div class="qr-code" onclick="scanQR()" id="qrCode">
                        <div class="qr-pattern"></div>
                        <div class="scan-animation" id="scanAnimation" style="display: none;"></div>
                    </div>
                    
                    <div style="font-size: 0.9em; color: #888; margin-top: 10px;">
                        Haz clic en el código QR para simular el escaneo
                    </div>
                </div>

                <div class="footer-note">
                    <strong>¡Disfruta tu película!</strong><br>
                    Presenta este código QR en el mostrador del cine 15 minutos antes de la función.
                    <br><br>
                    <%-- Usamos un Label para el número de orden --%>
                    <small>Número de orden: <asp:Label ID="orderNumber" runat="server">#CP-2025-001847</asp:Label></small>
                </div>
            </div>
        </div>

        <script>
            function scanQR() {
                const qrCode = document.getElementById('qrCode');
                const scanAnimation = document.getElementById('scanAnimation');
                const orderStatus = document.getElementById('<%= orderStatus.ClientID %>'); // Accede al ID del control de servidor

                // Mostrar animación de escaneo
                scanAnimation.style.display = 'block';
                qrCode.style.pointerEvents = 'none';

                // Simular proceso de escaneo
                setTimeout(() => {
                    // Cambiar estado a "Utilizado"
                    orderStatus.textContent = 'Utilizado';
                    orderStatus.className = 'status-badge status-used'; // Actualiza la clase CSS

                    // Ocultar animación
                    scanAnimation.style.display = 'none';

                    // Cambiar apariencia del QR para indicar que fue usado
                    qrCode.style.opacity = '0.6';
                    qrCode.style.pointerEvents = 'auto';

                    // Mostrar mensaje de confirmación
                    alert('¡Código QR escaneado exitosamente!\nEstado cambiado a: UTILIZADO');

                }, 2000);
            }

            // La función updateDateTime ya no es necesaria aquí si la manejas en el code-behind
            // pero la dejo si quieres que el cliente la actualice de forma inicial
            function updateDateTime() {
                const now = new Date();
                const options = {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                };
                const formatted = now.toLocaleDateString('es-ES', options);
                const purchaseDateTimeElement = document.getElementById('<%= purchaseDateTime.ClientID %>');
                if (purchaseDateTimeElement) {
                    purchaseDateTimeElement.textContent = formatted;
                }
            }

            // Ejecutar al cargar la página
            window.onload = function () {
                updateDateTime();
            };
        </script>
    </form>
</body>
</html>