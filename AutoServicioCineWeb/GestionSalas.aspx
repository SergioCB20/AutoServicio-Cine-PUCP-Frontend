<%@ Page Title="Gestión de Salas" Language="C#" 
    MasterPageFile="~/Admin.Master" AutoEventWireup="true" 
    CodeBehind="GestionSalas.aspx.cs" 
    Inherits="AutoServicioCineWeb.GestionSalas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Salas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .form-field { margin-bottom: 10px; }
        .form-label { display: inline-block; width: 100px; }
        .form-input { width: 200px; }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageTitleContent" runat="server">
    Gestión de Salas
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra las salas disponibles en el sistema.
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <asp:Literal ID="litMensajeModal" runat="server" EnableViewState="false" />

            <h2><asp:Literal ID="litModalTitle" runat="server" Text="Agregar Sala" /></h2>

            <asp:HiddenField ID="hdnSalaId" runat="server" />

            <div class="form-field">
                <span class="form-label">Nombre:</span>
                <asp:TextBox ID="txtNombreSala" runat="server" CssClass="form-input" />
            </div>

            <div class="form-field">
                <span class="form-label">Capacidad:</span>
                <asp:TextBox ID="txtCapacidad" runat="server" CssClass="form-input" />
            </div>

            <div class="form-field">
                <span class="form-label">Tipo de Sala:</span>
                <asp:DropDownList ID="ddlTipoSala" runat="server" CssClass="form-input">
                    <asp:ListItem Text="Seleccionar" Value="" />
                    <asp:ListItem Text="Estandar" Value="Estandar" />
                    <asp:ListItem Text="3D" Value="3D" />
                    <asp:ListItem Text="Premium" Value="Premium" />
                    <asp:ListItem Text="4DX" Value="4DX" />
                </asp:DropDownList>
            </div>

            <div class="form-field">
                <span class="form-label">Activa:</span>
                <asp:CheckBox ID="chkActiva" runat="server" />
            </div>

            <asp:Button ID="btnGuardarSala" runat="server" Text="Guardar Sala" OnClick="btnGuardarSala_Click" CssClass="btn btn-primary" />

            <hr />

            <asp:GridView ID="gvSalas" runat="server" AutoGenerateColumns="False" 
                          OnRowCommand="gvSalas_RowCommand" 
                          OnPageIndexChanging="gvSalas_PageIndexChanging"
                          AllowPaging="true" PageSize="10" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="ID" />
                    <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="capacidad" HeaderText="Capacidad" />
                    <asp:BoundField DataField="tipoSala" HeaderText="Tipo de Sala" />
                    <asp:CheckBoxField DataField="activa" HeaderText="Activa" />
                    <asp:ButtonField CommandName="EditarSala" Text="Editar" ButtonType="Button" HeaderText="Acción" />
                </Columns>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ScriptContent" runat="server">
    <!-- Scripts adicionales si es necesario -->
</asp:Content>
