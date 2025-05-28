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
                    <span>S/ 8.50</span>
                    <div class="cantidad-selector">
                        <button class="cantidad-button minus">-</button>
                        <span class="cantidad">0</span>
                        <button class="cantidad-button plus">+</button>
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
                    <span>S/ 7.00</span>
                    <div class="cantidad-selector">
                        <button class="cantidad-button minus">-</button>
                        <span class="cantidad">0</span>
                        <button class="cantidad-button plus">+</button>
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
                    <span>S/ 7.00</span>
                    <div class="cantidad-selector">
                        <button class="cantidad-button minus">-</button>
                        <span class="cantidad">0</span>
                        <button class="cantidad-button plus">+</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TicketsSummary" runat="server">
    <%-- Este contenido se llenaría dinámicamente con la cantidad de entradas seleccionadas --%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TotalAmount" runat="server">
    S/ 0.00
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
    <a href="Butacas.aspx" class="button primary continuar">Continuar</a>
    <button class="button secondary cancelar">Cancelar compra</button>
</asp:Content>
