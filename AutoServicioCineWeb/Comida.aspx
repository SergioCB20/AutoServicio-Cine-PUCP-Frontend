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
        <div class="productos-container">
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
                            <button type="button" class="cantidad-button minus" 
                                            onclick="actualizarCantidadYServidor('<%# Eval("Id") %>', '<%# Eval("Nombre_es") %>', -1)">-</button>
                            <span id='cantidad_<%# Eval("Id") %>' class="cantidad" data-nombre='<%# Eval("Nombre_es") %>'>0</span>
                            <button type="button" class="cantidad-button plus" 
                                            onclick="actualizarCantidadYServidor('<%# Eval("Id") %>', '<%# Eval("Nombre_es") %>', 1)">+</button>
                        </div>
                    </div>
                </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
        
<%--        <div class="resumen-compra">
            <h3>Total de Compra:</h3>
            <span id="totalResumen">S/ 0.00</span>
        </div>--%>
    </div>
    <asp:HiddenField ID="hfResumenComida" runat="server" ClientIDMode="Static" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ActionButtons" runat="server">
    <asp:Button id="btnContinuar" CssClass="button primary continuar" OnClick="btnContinuar_Click" runat="server" Text="Continuar"/>
    <button class="button secondary cancelar">Cancelar compra</button>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        let cantidades = {};
        let precios = {};
        let total = 0.00;

        
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

        function actualizarCantidadYServidor(id, nombre, cambio) {
            actualizarCantidad(id, cambio);
        }

        function calcularTotal() {
            
            total = 0.00;

            const baseAttr = document.getElementById("totalResumen").getAttribute("data-base");
            const baseClean = baseAttr.replace(/[^\d.]/g, '');
            const base = parseFloat(baseClean) || 0;
            //console.log("valor:", baseAttr); //para imprimir el valor de los datos en la consola
            //console.log("base:", base);
            const resumenDiv = document.getElementById("resumenCompraComida");
            if (!resumenDiv) return;

            // Limpiar solo los items agregados
            const subtitulo = document.getElementById("subtituloComida");

            resumenDiv.innerHTML = '';
            if (subtitulo) resumenDiv.appendChild(subtitulo);

            let resumenStrings = []; //para guardar cada opción de comida

            for (let id in cantidades) {
                const cantidad = cantidades[id];
                if (cantidad > 0) {
                    const span = document.getElementById("cantidad_" + id);
                    const nombre = span.dataset.nombre;
                    const precioUnitario = precios[id];
                    const subtotal = cantidad * precioUnitario;
                    total += subtotal;

                    const p = document.createElement("p");
                    p.textContent = `${nombre} x ${cantidad} = S/ ${subtotal.toFixed(2)}`;
                    resumenDiv.appendChild(p);

                    resumenStrings.push(`${id};${nombre};${cantidad};${precioUnitario.toFixed(2)}`);//carga una línea detalle
                }
            }

            const nuevoTotal = base + total;
            totalResumen.textContent = "S/ " + nuevoTotal.toFixed(2);
            document.getElementById("hfTotal").value = nuevoTotal.toFixed(2);

            document.getElementById("hfResumenComida").value = resumenStrings.join("|"); //actualiza el valor del hiddenfield
        }
        
    </script>
</asp:Content>
