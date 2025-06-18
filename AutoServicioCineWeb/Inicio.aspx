<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="AutoServicioCineWeb.Inicio" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Inicio - Autoservicio Cine</title>
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/base.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/inicio.css") %>" />
</head>
<body>
    <form id="form1" runat="server">
        <main class="container">
            <section class="hero">
                <div class="logo">
                    <img src="<%= ResolveUrl("~/images/logo.png") %>" alt="Logo del cine" />
                </div>
                <h1 class="titulo-pregunta">¿Qué desea comprar?</h1>
                <div class="opciones">
                    <a href="<%= ResolveUrl("~/Peliculas.aspx") %>" class="opcion-1">
                        <img src="<%= ResolveUrl("~/images/ticket-inicio.png") %>" alt="Comprar entradas" />
                    </a>
                    <a href="<%= ResolveUrl("~/Comida.aspx") %>" class="opcion-2">
                        <img src="<%= ResolveUrl("~/images/palomitas-inicio.png") %>" alt="Comprar comida" />
                    </a>
                </div>

                <%-- Contenedor para los botones de acceso --%>
                <%-- Eliminamos visible="false" del markup, la visibilidad se controla en C# --%>
                <div id="divBotonesAcceso" runat="server" class="botones-acceso">
                    <a href="<%= ResolveUrl("~/Login.aspx") %>" class="boton-sesion" style="text-decoration:none">Iniciar sesión</a>
                    <a href="<%= ResolveUrl("~/Signup.aspx") %>" class="boton-invitado" style="text-decoration:none">Registrarse</a>
                </div>

                <%-- Contenedor para el botón de cerrar sesión --%>
                <%-- Eliminamos visible="false" del markup. La visibilidad se controla en C# --%>
                <div id="divBotonCerrarSesion" runat="server" class="botones-acceso">
                    <asp:LinkButton ID="lnkCerrarSesion" runat="server" OnClick="lnkCerrarSesion_Click" CssClass="boton-sesion" style="text-decoration:none">Cerrar sesión</asp:LinkButton>
                    <asp:Label ID="lblBienvenida" runat="server" CssClass="welcome-message"></asp:Label>
                </div>
            </section>
            <aside class="imagen-principal">
                <img src="<%= ResolveUrl("~/images/imagen-inicio.png") %>" alt="Imagen principal del cine" />
            </aside>
        </main>
    </form>
</body>
</html>