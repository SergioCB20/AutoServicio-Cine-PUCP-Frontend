<%@ Page Title="" Language="C#" MasterPageFile="~/Form.Master" AutoEventWireup="true"
    CodeBehind="PaginaUsuario.aspx.cs" Inherits="AutoServicioCineWeb.PaginaUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Mi perfil
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/perfil.css">
    <link rel="stylesheet" href="./styles/base.css">
    <style>
        .right-section {
            display: none !important;
        }
        /* Y ajustar el MainContent para que ocupe todo el ancho */
        .MainContent .perfil-container {
            justify-content: center;
        }
    </style>

</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="perfil-container">
        <div class="perfil-header">
            <h1>Mi Perfil</h1>
            <div class="perfil-actions">
                <asp:Button ID="btnEditar" runat="server" Text="Editar Perfil" CssClass="boton-sesion"
                    OnClick="btnEditar_Click" />
            </div>
        </div>
        <%-- Espacio de las 2 columnas --%>
        <div class="perfil-content">
            <div class="perfil-section-left">
                <h2>Información Personal</h2>
                <div class="perfil-info">
                    <div class="info-row">
                        <span class="info-label">Nombre:</span>
                        <asp:Label ID="lblNombre" runat="server" CssClass="info-value"></asp:Label>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Email:</span>
                        <asp:Label ID="lblEmail" runat="server" CssClass="info-value"></asp:Label>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Teléfono:</span>
                        <asp:Label ID="lblTelefono" runat="server" CssClass="info-value"></asp:Label>
                    </div>
                </div>
            </div>

            <!-- Sección de historial de compras (siempre visible) -->
            <div class="perfil-section-right">
                <h2>Historial de Compras</h2>
                <asp:GridView ID="gvHistorial" runat="server" CssClass="historial-grid" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="Pelicula" HeaderText="Película" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Hora" HeaderText="Hora" />
                        <asp:BoundField DataField="Cantidad" HeaderText="Entradas" />
                        <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                    </Columns>
                    <EmptyDataTemplate>
                        <p class="no-compras">No hay compras registradas.</p>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal de edición -->
        <asp:Panel ID="pnlEdicion" runat="server" CssClass="modal-overlay" Style="display: none;">
            <div class="modal-content">
                <div class="modal-header">
                    <h2>Editar Perfil</h2>
                    <span class="modal-close" onclick="cerrarModal()">×</span>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="txtNombreEdit">Nombre:</label>
                        <asp:TextBox ID="txtNombreEdit" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtEmailEdit">Email:</label>
                        <asp:TextBox ID="txtEmailEdit" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtTelefonoEdit">Teléfono:</label>
                        <asp:TextBox ID="txtTelefonoEdit" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label for="txtPasswordEdit">Nueva Contraseña (dejar en blanco para no cambiar):</label>
                        <asp:TextBox ID="txtPasswordEdit" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="boton-sesion"
                            OnClick="btnGuardar_Click" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="boton-invitado"
                            OnClick="btnCancelar_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        function mostrarModal() {
            document.getElementById('<%= pnlEdicion.ClientID %>').style.display = 'flex';
            modal.style.display = 'flex';
            return false; // Importante para evitar postback
        }

        function cerrarModal() {
            document.getElementById('<%= pnlEdicion.ClientID %>').style.display = 'none';
            modal.style.display = 'none';
        }

        // Cerrar modal al hacer clic fuera del contenido
        window.onclick = function (event) {
            var modal = document.getElementById('<%= pnlEdicion.ClientID %>');
            if (event.target == modal) {
                cerrarModal();
            }
        }
    </script>
</asp:Content>
