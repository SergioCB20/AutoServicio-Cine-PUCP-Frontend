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
            <div class="sala-container">
                <div class="identificadores-filas" id="identificadores-filas">
                    <!-- Identificadores de filas generados por JavaScript -->
                </div>
                <div class="filas-contenido">
                    <div class="identificadores-columnas" id="identificadores-columnas">
                        <!-- Identificadores de columnas generados por JavaScript -->
                    </div>
                    <div class="filas" id="filas">
                        <!-- Contenido de las butacas generado por JavaScript -->
                    </div>
                </div>
            </div>
        </div>
        <div class="leyenda">
            <span class="disponible"></span> Disponible
            <span class="ocupado"></span> Ocupado
            <span class="seleccionado"></span> Seleccionado
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
    <a href="../chocolateria/index.html" class="button primary continuar">Continuar</a>
    <button class="button secondary cancelar">Cancelar compra</button>
    <script>
        // El script JavaScript original para la generación de butacas puede ir aquí
        // o ser referenciado desde un archivo .js externo.
        // Para ASP.NET, este script aún sería de lado del cliente.
        const salaDiv = document.getElementById('filas');
        const identificadoresFilasDiv = document.getElementById('identificadores-filas');
        const identificadoresColumnasDiv = document.getElementById('identificadores-columnas');

        let cantidadEntradas = 0;
        let butacasSeleccionadas = [];

        function generarIdentificadores(filas, columnas) {
            // Generar identificadores de filas (A, B, C, ...)
            identificadoresFilasDiv.innerHTML = '';
            for (let i = 0; i < filas; i++) {
                const identificadorDiv = document.createElement('div');
                identificadorDiv.classList.add('identificador-fila');
                identificadorDiv.textContent = String.fromCharCode(65 + i);
                identificadoresFilasDiv.appendChild(identificadorDiv);
            }

            // Generar identificadores de columnas (1, 2, 3, ...)
            identificadoresColumnasDiv.innerHTML = '';
            for (let j = 0; j < columnas; j++) {
                const identificadorDiv = document.createElement('div');
                identificadorDiv.classList.add('identificador-columna');
                identificadorDiv.textContent = j + 1;
                identificadoresColumnasDiv.appendChild(identificadorDiv);
            }
        }

        function generarSala(filas, columnas) {
            // Primero generar los identificadores
            generarIdentificadores(filas, columnas);

            // Limpiar la sala existente
            salaDiv.innerHTML = '';
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
            }
        }

        //función que ajusta el tamaño de la pantalla en función a la sala
        function ajustarPantallaDinamico() {
            const pantalla = document.querySelector('.pantalla');
            const filasContenido = document.querySelector('.filas-contenido');

            if (pantalla && filasContenido) {
                const anchoFilas = filasContenido.offsetWidth;
                pantalla.style.width = `${anchoFilas}px`;
                pantalla.style.margin = '0 auto';
                pantalla.style.textAlign = 'center'; // centrar el texto también
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            calcularCantEntradas();
            generarSala(10, 12);  // valores de prueba
            ajustarPantallaDinamico(); // ajuste dinámico en el tamaño
        });

        function calcularCantEntradas() {
            const cadenaAdulto = document.getElementById("entradasAdultoTexto").textContent;
            const cadenaInfantil = document.getElementById("entradasInfantilTexto").textContent;
            const cadenaMayor = document.getElementById("entradasMayorTexto").textContent;

            const numAdulto = extraerNumero(cadenaAdulto);
            const numInfantil = extraerNumero(cadenaInfantil);
            const numMayor = extraerNumero(cadenaMayor);

            cantidadEntradas = numAdulto + numInfantil + numMayor;
        }

        function extraerNumero(cadena) {
            const match = cadena.trim().match(/^(\d+)/);
            return match ? parseInt(match[1]) : 0;
        }

        
    </script>
</asp:Content>
