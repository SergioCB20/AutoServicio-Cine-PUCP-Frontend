<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Peliculas.aspx.cs" Inherits="AutoServicioCineWeb.Peliculas" MasterPageFile="~/SiteWithHeader.master" %>

<%-- Content para el título de la página --%>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Seleccionar Película
</asp:Content>

<%-- Content para estilos/scripts específicos de la cabeza de la página --%>
<asp:Content ID="ContentHeadExtra" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/peliculas.css") %>" />
</asp:Content>

<%--
    TODO EL CONTENIDO PRINCIPAL DE Peliculas.aspx ahora va aquí,
    dentro del ContentPlaceHolder "MainContent" de SiteWithHeader.master.
    No hay "left-section" o "right-section" forzados por esta Master.
--%>
<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <main class="main-content"> <%-- Puedes agregar la clase main-content aquí si la necesitas --%>
        <div class="search-bar-container">
            <div class="search-bar">
                <asp:TextBox ID="txtBuscarPelicula" runat="server" CssClass="search-input" placeholder="Buscar película..."></asp:TextBox>
                <asp:LinkButton ID="btnBuscar" runat="server" CssClass="search-button" OnClick="btnBuscar_Click">
                    <span class="search-icon-wrapper">
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
                    </span>
                </asp:LinkButton>
            </div>
        </div>
        <section class="peliculas-grid">
            <asp:Repeater ID="rptPeliculas" runat="server">
                <ItemTemplate>
                    <div class="pelicula">
                        <div class="pelicula-info">
                            <img src='<%# Eval("imagenUrl") %>' alt='<%# Eval("tituloEs") %>' />
                            <asp:Button ID="btnComprar" runat="server" CssClass="boton-comprar" Text="Comprar"
                                CommandName="Comprar" CommandArgument='<%# Eval("peliculaId") %>' OnClick="btnComprar_Click" />
                        </div>
                        <div class="pelicula-titulo">
                            <h3><%# Eval("tituloEs") %></h3>
                            <p><%# Eval("duracionMin") %> min</p>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                   <asp:Literal ID="litNoPeliculas" runat="server" Text="No hay películas disponibles." Visible="false"></asp:Literal>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal ID="litMensaje" runat="server"></asp:Literal>
        </section>
    </main>
</asp:Content>

<%-- Si necesitas scripts al final del body para esta página, usa el ContentPlaceHolder Script --%>
<asp:Content ID="ContentScript" ContentPlaceHolderID="Script" runat="server">
    <%-- tus scripts específicos aquí --%>
</asp:Content>

<%-- Si necesitas un modal específico para esta página, usa el ContentPlaceHolder modal --%>
<asp:Content ID="ContentModal" ContentPlaceHolderID="modal" runat="server">
    <%-- tu HTML del modal aquí --%>
</asp:Content>