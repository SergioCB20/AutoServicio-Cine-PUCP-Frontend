<%@ Page Language="C#" MasterPageFile="~/Form.Master" AutoEventWireup="true" CodeBehind="Funcion.aspx.cs" Inherits="AutoServicioCineWeb.Funcion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Selección de Función
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/form-funcion.css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="middle-section">
        <h2>Funciones Disponibles:</h2>
        <div class="funciones-container">
            <asp:Repeater ID="rptFunciones" runat="server" OnItemCommand="rptFunciones_ItemCommand">
                <ItemTemplate>
                <asp:LinkButton runat="server" CssClass="funcion" CommandName="SeleccionarFuncion" 
                    CommandArgument='<%# Eval("FuncionId") + "|" + Eval("SalaId") + "|" + Eval("FechaHora") %>'>
                    <div class="formato-proyeccion"><%# Eval("FormatoProyeccion") %></div>
                    <div class="funcion-detalle">
                        <p><strong>Idioma:</strong> <%# Eval("Idioma") %></p>
                        <p><strong>Subtítulos:</strong> <%# Convert.ToBoolean(Eval("Subtitulos")) ? "Sí" : "No" %></p>
                        <p><strong>Fecha y hora:</strong><br /><%# Eval("FechaHora") %></p>
                    </div>
                </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ActionButtons" runat="server">
    <asp:Button id="btnContinuar" CssClass="button primary continuar" OnClick="btnContinuar_Click" runat="server" Text="Continuar"/>
    <button class="button secondary cancelar">Cancelar compra</button>
</asp:Content>