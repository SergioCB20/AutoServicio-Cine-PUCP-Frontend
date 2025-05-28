<%@ Page Language="C#"  MasterPageFile="~/Form.master" AutoEventWireup="true" CodeBehind="Butacas.aspx.cs" Inherits="AutoServicioCineWeb.Butacas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Selección de Butacas
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/form-butacas.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="middle-section">
        <h2>Selecciona tus butacas</h2>
        <div class="sala">
            <div class="pantalla">PANTALLA</div>
            <div class="filas">
                <%-- Contenido de las butacas generado por el script o backend --%>
            </div>
        </div>
        <div class="leyenda">
            <span class="disponible"></span> Disponible
            <span class="ocupado"></span> Ocupado
            <span class="seleccionado"></span> Seleccionado
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="TicketsSummary" runat="server">
    <span>2 Adultos</span>
    <span>1 Infantil</span>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TotalAmount" runat="server">
    S/ 0.00
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
    <a href="../chocolateria/index.html" class="button primary continuar">Continuar</a>
    <button class="button secondary cancelar">Cancelar compra</button>
    <script>
        // El script JavaScript original para la generación de butacas puede ir aquí
        // o ser referenciado desde un archivo .js externo.
        // Para ASP.NET, este script aún sería de lado del cliente.
        const salaDiv = document.querySelector('.sala .filas');
        const cantidadEntradas = 8; // Ejemplo: Obtener del paso anterior
        let butacasSeleccionadas = [];
        const butacasSeleccionadasSpan = document.querySelector('.butacas-seleccionadas span');
        const totalSpan = document.querySelector('.total span');

        function generarSala(filas = 10, columnas = 12) {
            for (let i = 0; i < filas; i++) {
                const filaDiv = document.createElement('div');
                filaDiv.classList.add('fila');
                for (let j = 0; j < columnas; j++) {
                    const butacaDiv = document.createElement('div');
                    butacaDiv.classList.add('butaca');
                    butacaDiv.dataset.fila = String.fromCharCode(65 + i);
                    butacaDiv.dataset.columna = j + 1;
                    // Simulación de butacas ocupadas (aleatorio)
                    if (Math.random() < 0.2) {
                        butacaDiv.classList.add('ocupado');
                    } else {
                        butacaDiv.addEventListener('click', seleccionarButaca);
                    }
                    filaDiv.appendChild(butacaDiv);
                }
                salaDiv.appendChild(filaDiv);
            }
        }

        function seleccionarButaca(event) {
            const butaca = event.target;
            if (!butaca.classList.contains('ocupado')) {
                const fila = butaca.dataset.fila;
                const columna = butaca.dataset.columna;
                const idButaca = `${fila}${columna}`;

                if (butaca.classList.contains('seleccionado')) {
                    butaca.classList.remove('seleccionado');
                    butacasSeleccionadas = butacasSeleccionadas.filter(id => id !== idButaca);
                } else if (butacasSeleccionadas.length < cantidadEntradas) {
                    butaca.classList.add('seleccionado');
                    butacasSeleccionadas.push(idButaca);
                } else {
                    alert(`Solo puedes seleccionar ${cantidadEntradas} butacas.`);
                }
                actualizarResumen();
            }
        }

        function actualizarResumen() {
            if (butacasSeleccionadasSpan) { // Verificar si el elemento existe
                butacasSeleccionadasSpan.textContent = butacasSeleccionadas.join(', ');
            }
            if (totalSpan) { // Verificar si el elemento existe
                totalSpan.textContent = `S/ ${cantidadEntradas * 8.50}`; // Ejemplo de precio único
            }
        }

        generarSala();
    </script>
</asp:Content>
