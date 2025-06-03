<%@ Page Language="C#" MasterPageFile="~/Form.master" AutoEventWireup="true" CodeBehind="Tickets.aspx.cs" Inherits="AutoServicioCineWeb.Tickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Compra de Entradas
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/form-tickets.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="middle-section">
        <h2 class="pelicula-titulo">Thunderbolts</h2>
        <div class="codigo-promocional">
            <label for="codigo">¿Tienes un código promocional?</label>
            <div class="input-group">
                <input type="text" id="codigo" placeholder="Código promocional" />
                <button class="button">Validar</button>
            </div>
        </div>
        <div class="tarifas-disponibles">
            <h3>Tarifas Disponibles:</h3>
            <div class="tarifa">
                <div class="tarifa-info">
                    <svg viewBox="0 0 24 24" fill="currentColor">
                        <path
                            d="M12 4a4 4 0 0 1 4 4 4 4 0 0 1-4 4 4 4 0 0 1-4-4 4 4 0 0 1 4-4m0 10c4.42 0 8 1.79 8 4v2H4v-2c0-2.21 3.58-4 8-4z"
                        ></path>
                    </svg>
                    <span>ADULTO</span>
                </div>
                <div class="tarifa-precio">
                    <span id="precioAdulto">S/ 8.50</span>
                    <div class="cantidad-selector">
                        <button class="cantidad-button minus" type="button" onclick="decrementar('adulto')">-</button>
                        <span id="CantidadAdulto" class="cantidad">0</span>
                        <button class="cantidad-button plus" type="button" onclick="incrementar('adulto')">+</button>
                    </div>
                </div>
            </div>
            <div class="tarifa">
                <div class="tarifa-info">
                    <svg viewBox="0 0 24 24" fill="currentColor">
                        <path
                            d="M15 16c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm-9 0c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0-8c-2.21 0-4 1.79-4 4v3h2v-3c0-1.1.9-2 2-2s2 .9 2 2v3h2v-3c0-2.21-1.79-4-4-4zm9 0c-2.21 0-4 1.79-4 4v3h2v-3c0-1.1.9-2 2-2s2 .9 2 2v3h2v-3c0-2.21-1.79-4-4-4zM12 4c-4.42 0-8 1.79-8 4v6h16V8c0-2.21-3.58-4-8-4z"
                        ></path>
                    </svg>
                    <span>INFANTIL DE 2-8</span>
                </div>
                <div class="tarifa-precio">
                    <span id="precioInfantil">S/ 7.00</span>
                    <div class="cantidad-selector">
                        <button class="cantidad-button minus" type="button" onclick="decrementar('infantil')">-</button>
                        <span id="CantidadInfantil" class="cantidad">0</span>
                        <button class="cantidad-button plus" type="button" onclick="incrementar('infantil')">+</button>
                    </div>
                </div>
            </div>
            <div class="tarifa">
                <div class="tarifa-info">
                    <svg viewBox="0 0 24 24" fill="currentColor">
                        <path
                            d="M12 4a4 4 0 0 1 4 4 4 4 0 0 1-4 4 4 4 0 0 1-4-4 4 4 0 0 1 4-4m0 10c4.42 0 8 1.79 8 4v2H4v-2c0-2.21 3.58-4 8-4z"
                        ></path>
                    </svg>
                    <span>AD. MAYOR +60</span>
                </div>
                <div class="tarifa-precio">
                    <span id="precioMayor">S/ 7.00</span>
                    <div class="cantidad-selector">
                        <button type="button" class="cantidad-button" onclick="decrementar('mayor')">-</button>
                        <span id="CantidadMayor" class="cantidad">0</span>
                        <button type="button" class="cantidad-button" onclick="incrementar('mayor')">+</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
    <asp:Button id="btnContinuar" CssClass="button primary continuar" OnClick="btnContinuar_Click" runat="server" Text="Continuar"/>
    <button class="button secondary cancelar">Cancelar compra</button>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        let total = 0.00;
        let adulto = 0.00;
        let infantil = 0.00;
        let mayor = 0.00;
        let cadenaMayor = " ";
        let cadenaInfantil = " ";
        let cadenaAdulto = " ";
        function incrementar(tipoTicket) {
            if (tipoTicket === "mayor") {
                mayor += 1;
                document.getElementById("CantidadMayor").textContent = mayor;
            }
            if (tipoTicket === "adulto") {
                adulto += 1;
                document.getElementById("CantidadAdulto").textContent = adulto;
            }
            if (tipoTicket === "infantil") {
                infantil += 1;
                document.getElementById("CantidadInfantil").textContent = infantil;
            }
            actualizarDatos();
        }

        function decrementar(tipoTicket) {
            if (tipoTicket === "mayor" && mayor > 0) {
                mayor -= 1;
                document.getElementById("CantidadMayor").textContent = mayor;
            }
            if (tipoTicket === "adulto" && adulto > 0) {
                adulto -= 1;
                document.getElementById("CantidadAdulto").textContent = adulto;
            }
            if (tipoTicket === "infantil" && infantil > 0) {
                infantil -= 1;
                document.getElementById("CantidadInfantil").textContent = infantil;
            }
            actualizarDatos();
        }

        function actualizarDatos() {
            if (mayor == 1) { cadenaMayor = "1 Adulto Mayor" }
            if (mayor == 0) { cadenaMayor = "" }
            if (mayor > 1) { cadenaMayor = mayor + " Adultos Mayores" }
            if (infantil == 1) { cadenaInfantil = "1 Infantil" }
            if (infantil == 0) { cadenaInfantil = "" }
            if (infantil > 1) { cadenaInfantil = infantil + " Infantiles" }
            if (adulto == 1) { cadenaAdulto = "1 Adulto" }
            if (adulto == 0) { cadenaAdulto = "" }
            if (adulto > 1) { cadenaAdulto = adulto + " Adultos" }
            total = mayor * 7.00 + infantil * 7.00 + adulto * 8.50;
            document.getElementById("entradasAdultoTexto").textContent = cadenaAdulto;
            document.getElementById("entradasInfantilTexto").textContent = cadenaInfantil;
            document.getElementById("entradasMayorTexto").textContent = cadenaMayor;
            document.getElementById("hfEntradasAdulto").value = cadenaAdulto;
            document.getElementById("hfEntradasInfantil").value = cadenaInfantil;
            document.getElementById("hfEntradasMayor").value = cadenaMayor;
            document.getElementById("totalResumen").textContent = "S/ " + total.toFixed(2);
            document.getElementById("hfTotal").value = "S/ " + total.toFixed(2);
        }
    </script>
    </asp:Content>
