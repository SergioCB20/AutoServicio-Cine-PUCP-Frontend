<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="AutoServicioCineWeb.Inicio" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Inicio - Autoservicio Cine</title>
    <%-- Rutas de CSS: Usamos ResolveUrl("~/") para asegurar que funcionen desde la raíz de la aplicación --%>
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/base.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/inicio.css") %>" />
</head>
<body>
    <%-- La etiqueta <form runat="server"> es NECESARIA en ASP.NET Web Forms si quieres usar controles de servidor o manejar eventos.
         Envuelve todo el contenido que pueda necesitar procesamiento del lado del servidor. --%>
    <form id="form1" runat="server">
        <main class="container">
            <section class="hero">
                <div class="logo">
                    <%-- Rutas de imágenes: También usamos ResolveUrl("~/") --%>
                    <img src="<%= ResolveUrl("~/images/logo.png") %>" alt="Logo del cine" />
                </div>
                <h1 class="titulo-pregunta">¿Qué desea comprar?</h1>
                <div class="opciones">
                    <%-- Ajusta los href a las URLs reales de tus páginas .aspx (o HTML estáticas) --%>
                    <a href="<%= ResolveUrl("~/Peliculas.aspx") %>" class="opcion-1">
                        <img src="<%= ResolveUrl("~/images/ticket-inicio.png") %>" alt="Comprar entradas" />
                    </a>
                    <a href="<%= ResolveUrl("~/Comida.aspx") %>" class="opcion-2"> <%-- Cambia a la URL de tu página de comida --%>
                        <img src="<%= ResolveUrl("~/images/palomitas-inicio.png") %>" alt="Comprar comida" />
                    </a>
                </div>
                <div class="botones-acceso">
                    <%-- Si estos botones solo navegan, HTML simple está bien.
                         Si necesitas lógica de servidor (ej. iniciar sesión con autenticación en C#), considera usar <asp:Button runat="server"> --%>
                    <button class="boton-sesion">Iniciar sesión</button>
                    <button class="boton-invitado">Ingresar como invitado</button>
                </div>
            </section>
            <aside class="imagen-principal">
                <img src="<%= ResolveUrl("~/images/imagen-inicio.png") %>" alt="Imagen principal del cine" />
            </aside>
        </main>
    </form>
</body>
</html>