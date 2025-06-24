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

        // Esta función la debes llamar desde el code-behind para cada producto
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
            console.log("alo:", baseAttr);
            console.log("alo:", base);
            const resumenDiv = document.getElementById("resumenCompraComida");
            if (!resumenDiv) return;

            // Limpiar solo los items agregados
            const subtitulo = document.getElementById("subtituloComida");

            resumenDiv.innerHTML = '';
            if (subtitulo) resumenDiv.appendChild(subtitulo);

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
                }
            }

            const nuevoTotal = base + total;
            totalResumen.textContent = "S/ " + nuevoTotal.toFixed(2);
            document.getElementById("hfTotal").value = nuevoTotal.toFixed(2);
        }
        
        
        //function calcularTotal() {
            //total = 0; // reiniciar el total
            //for (let id in cantidades) {
            //    total += cantidades[id] * precios[id];
            //}

            //// Mostrar total visible
            //const spanTotal = document.getElementById("totalResumen");
            //if (spanTotal) {
            //    spanTotal.textContent = "S/ " + total.toFixed(2);
            //}

            //// Guardar en hidden field
            //const hfTotal = document.getElementById("hfTotal");
            //if (hfTotal) {
            //    hfTotal.value = total.toFixed(2);
            //}
            //// Mostrar fecha y hora por separado
            //const ahora = new Date();

            //// Formatear fecha
            //const fecha = ahora.toLocaleDateString("es-PE", {
            //    day: "2-digit",
            //    month: "2-digit",
            //    year: "numeric"
            //});

            //// Formatear hora
            //const hora = ahora.toLocaleTimeString("es-PE", {
            //    hour: "2-digit",
            //    minute: "2-digit",
            //    hour12: false
            //});

            //document.getElementById("litFecha").textContent = fecha;
            //document.getElementById("litHora").textContent = hora;
        //}

        
    </script>
</asp:Content>
