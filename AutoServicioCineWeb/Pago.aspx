<%@ Page Language="C#" MasterPageFile="~/Form.master" AutoEventWireup="true" CodeBehind="Pago.aspx.cs" Inherits="AutoServicioCineWeb.Pago" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Pago con QR
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/form-pago.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="middle-section">
    <h2>Realiza tu pago</h2>

    <!-- Sección de Código Promocional -->
    <div class="codigo-promocional mb-4 text-center">
        <asp:TextBox ID="txtCodigoPromocional" runat="server" CssClass="codigo-input" placeholder="Ingresa tu código aquí"></asp:TextBox>
        <asp:Button ID="btnAplicarCodigo" runat="server" Text="Aplicar" CssClass="button aplicar" OnClientClick="actualizarTotalHidden();" OnClick="btnAplicarCodigo_Click" />
        <asp:Label ID="lblMensajeCodigo" runat="server" CssClass="text-sm text-red-500" EnableViewState="false"></asp:Label>
    </div>

    <!-- Botón para generar QR -->
    <div class="text-center mt-4">
        <asp:Button ID="btnGenerarQR" runat="server" Text="Generar QR de Compra" CssClass="button generarqr" OnClick="btnGenerarQR_Click" />
        <p></p>
    </div>

    <!-- Contenedor del QR, oculto inicialmente -->
    <asp:Panel ID="pnlQR" runat="server" CssClass="bg-white p-8 rounded-xl shadow-md max-w-lg mx-auto mt-4" Visible="false">
        <div class="flex flex-col items-center">
            <div class="qr-container mb-4">
                <img src="https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=CineAutoservicio-Pago-123456"
                    alt="QR de Pago"
                    class="w-48 h-48 block items-center" />
            </div>
        </div>
        <p class="text-center text-gray-600 text-sm">
            Escanea este código QR con tu app de billetera móvil para completar el pago.
        </p>
    </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
    <asp:Button id="btnContinuar" CssClass="button primary continuar" OnClick="btnContinuar_Click" runat="server" Text="Continuar"/>
    <button class="button secondary cancelar">Cancelar compra</button>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="Script" runat="server">
    <script>
        let cantidadEntradas = 0;
        function actualizarTotalHidden() {
            const totalTexto = document.getElementById("totalResumen").textContent;
            const soloNumero = totalTexto.replace("S/", "").trim();
            document.getElementById("hfTotal").value = soloNumero;
        }
    </script>
</asp:Content>

