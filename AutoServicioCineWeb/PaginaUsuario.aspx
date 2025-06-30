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

        .cupon-container {
            margin-bottom: 15px;
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            background-color: #f9f9f9;
            display: block;
        }

        .historial-container {
            margin-bottom: 15px;
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            background-color: #f9f9f9;
            display: block;
        }

        .empty-state {
            text-align: center;
            padding: 20px;
            color: #666;
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
        <div class="perfil-content-top">
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

            <!-- Sección de historial de cupones (siempre visible) -->
            <div class="perfil-section-right">
                <h2>Cupones disponibles</h2>

                <asp:Repeater ID="rptCupones" runat="server" OnItemDataBound="rptCupones_ItemDataBound">
                    <ItemTemplate>
                        <div class="cupon-container">
                            <div class="cupon-linea-superior">
                                <span class="cupon-codigo"><%# Eval("Codigo") %></span>
                                <span class="cupon-valor">
                                    <asp:Literal ID="litDescuento" runat="server"></asp:Literal>
                                </span>
                            </div>
                            <div class="cupon-fecha">
                                <span class="icono-calendario">📅</span>
                                Válido hasta <%# Eval("fechaFin", "{0:dd/MM/yyyy}") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlEmptyState" runat="server" CssClass="empty-state" Visible="false">
                    <div class="empty-icon">🎫</div>
                    <div class="empty-title">No hay cupones disponibles</div>
                    <div class="empty-message">
                        ¡Mantente atento! Pronto tendremos nuevas ofertas para ti
                    </div>
                </asp:Panel>
            </div>
        </div>
        <%--HISTORIAL DE COMPRAS--%>
        <div class="perfil-content-bottom">
            <div class="perfil-section-full">
                <h2>Historial de Compras</h2>
                <asp:Repeater ID="rptHistorial" runat="server" OnItemDataBound="rptHistorial_ItemDataBound">
                    <ItemTemplate>
                        <div class="historial-container">
                            <div class="historial-linea-superior">
                                <span class="historial-fecha"><%# Eval("fechaHora") %></span>
                                <span class="historial-total"><%# Eval("total", "{0:C}") %></span>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoCompras" runat="server" Visible="false" CssClass="empty-state">
                    <p class="no-compras">No hay compras registradas.</p>
                </asp:Panel>
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
