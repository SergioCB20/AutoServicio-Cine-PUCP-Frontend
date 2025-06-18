<%@ Page Language="C#" MasterPageFile="~/Form.Master" AutoEventWireup="true" CodeBehind="Comida.aspx.cs" Inherits="AutoServicioCineWeb.Comida" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Selección de Comida
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/form-comida.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="middle-section">
        <h2>Productos Disponibles</h2>

        <asp:Repeater ID="rptComidas" runat="server">
            <ItemTemplate>
                <div class="producto">
                    <img class="producto-img" src='<%# Eval("ImagenURL") %>' alt="Imagen del producto" />

                    <div class="producto-detalle">
                        <h3><%# Eval("Nombre_es") %> / <%# Eval("Nombre_en") %></h3>
                        <p><strong>ES:</strong> <%# Eval("Descripcion_eS") %></p>
                        <p><strong>EN:</strong> <%# Eval("Descripcion_en") %></p>
                        <p class="precio">S/ <%# Eval("Precio", "{0:F2}") %></p>

                        <div class="cantidad-selector">
                            <button type="button" class="cantidad-button minus" onclick="actualizarCantidad('<%# Eval("Id") %>', -1)">-</button>
                            <span id='cantidad_<%# Eval("Id") %>' class="cantidad">0</span>
                            <button type="button" class="cantidad-button plus" onclick="actualizarCantidad('<%# Eval("Id") %>', 1)">+</button>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>

        <hr />

        <div class="resumen-compra">
            <h3>Total de Compra:</h3>
            <span id="totalResumen">S/ 0.00</span>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        let cantidades = {};
        let precios = {};

        // Este script es inicializado desde el backend usando RegisterStartupScript si es necesario
        function registrarPrecio(id, precio) {
            precios[id] = precio;
            cantidades[id] = 0;
        }

        function actualizarCantidad(id, cambio) {
            if (!(id in cantidades)) {
                cantidades[id] = 0;
            }
            cantidades[id] += cambio;
            if (cantidades[id] < 0) cantidades[id] = 0;

            document.getElementById("cantidad_" + id).textContent = cantidades[id];
            calcularTotal();
        }

        function calcularTotal() {
            let total = 0;
            for (let id in cantidades) {
                total += cantidades[id] * precios[id];
            }
            document.getElementById("totalResumen").textContent = "S/ " + total.toFixed(2);
        }
    </script>
</asp:Content>
