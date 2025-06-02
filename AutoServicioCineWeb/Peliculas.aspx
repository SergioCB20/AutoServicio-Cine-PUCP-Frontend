<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Peliculas.aspx.cs" Inherits="AutoServicioCineWeb.Peliculas" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Seleccionar Película - Autoservicio Cine</title>
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/base.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/navbar.css") %>" />
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/peliculas.css") %>" />
</head>
<body>
    <form id="form1" runat="server">
        <header class="header">
            <nav class="navbar">
                <div class="navbar-left">
                    <div class="logo-navbar">
                        <img src="<%= ResolveUrl("~/images/logo.png") %>" alt="Logo del cine" />
                    </div>
                    <div class="secciones-navbar">
                        <span class="seccion-pagina active">Películas</span>
                        <span class="seccion-pagina">Comida</span>
                    </div>
                </div>
                <div class="navbar-right">
                    <div class="cart-icon icon">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke-width="1.5"
                            stroke="currentColor"
                            class="hero-icon">
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                d="M15.75 10.5V6a3.75 3.75 0 1 0-7.5 0v4.5m11.356-1.993 1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 0 1-1.12-1.243l1.264-12A1.125 1.125 0 0 1 5.513 7.5h12.974c.576 0 1.059.435 1.119 1.007ZM8.625 10.5a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm7.5 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z" />
                        </svg>
                        <span class="cart-count">0</span>
                    </div>
                    <div class="user-icon icon">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke-width="1.5"
                            stroke="currentColor"
                            class="hero-icon">
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                        </svg>
                        <span>Sergio</span>
                    </div>
                    <div class="logout-icon icon">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke-width="1.5"
                            stroke="currentColor"
                            class="hero-icon">
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                d="M15.75 9V5.25A2.25 2.25 0 0 0 13.5 3h-6a2.25 2.25 0 0 0-2.25 2.25v13.5A2.25 2.25 0 0 0 7.5 21h6a2.25 2.25 0 0 0 2.25-2.25V15M12 9l-3 3m0 0 3 3m-3-3h12.75" />
                        </svg>
                    </div>
                </div>
            </nav>
        </header>
        <div class="container flex-column">
            <main class="main-content">
                <div class="search-bar-container">
                    <div class="search-bar">
                        <input type="text" placeholder="Buscar película..." />
                        <button aria-label="Buscar">
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke-width="1.5"
                                stroke="currentColor"
                                class="hero-icon search-icon">
                                <path
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
                            </svg>
                        </button>
                    </div>
                </div>
                <section class="peliculas-grid">
                    <%-- REPEATER PARA MOSTRAR LAS PELÍCULAS DINÁMICAMENTE --%>
                    <asp:Repeater ID="rptPeliculas" runat="server">
                        <ItemTemplate>
                            <div class="pelicula">
                                <div class="pelicula-info">
                                    <%-- Muestra la imagen de la película --%>
                                    <img src='<%# Eval("imagenUrl") %>' alt='<%# Eval("tituloEs") %>' />
                                    <%-- Botón Comprar, podrías pasar el ID de la película como CommandArgument --%>
                                    <asp:Button ID="btnComprar" runat="server" CssClass="boton-comprar" Text="Comprar"
                                        CommandName="Comprar" CommandArgument='<%# Eval("peliculaId") %>' OnClick="btnComprar_Click" />
                                </div>
                                <%-- Opcional: Mostrar el título de la película u otra info --%>
                                <div class="pelicula-titulo">
                                    <h3><%# Eval("tituloEs") %></h3>
                                    <p><%# Eval("duracionMin") %> min</p>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <%-- No se necesita separador visual para una cuadrícula, pero puedes poner un <br /> si quieres --%>
                        </SeparatorTemplate>
                        <FooterTemplate>
                            <%-- Opcional: Mostrar un mensaje si no hay películas --%>
                           <asp:Literal ID="litNoPeliculas" runat="server" Text="No hay películas disponibles." Visible="false"></asp:Literal>
                        </FooterTemplate>
                    </asp:Repeater>
                    <%-- FIN DEL REPEATER --%>

                    <%-- Aquí podrías añadir un Literal para mensajes de error si los hubiera --%>
                    <asp:Literal ID="litMensaje" runat="server"></asp:Literal>

                </section>
            </main>
        </div>
    </form>
</body>
</html>