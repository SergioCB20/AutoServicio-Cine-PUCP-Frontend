<%@ Page Title="Gestión de Usuarios" Language="C#"
MasterPageFile="~/Admin.Master" AutoEventWireup="true"
CodeBehind="GestionUsuarios.aspx.cs"
Inherits="AutoServicioCineWeb.GestionUsuarios" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Usuarios
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/styles/GestionUsuarios.css") %>">
    <script type="text/javascript">
        function clearModalValidators() {
            if (typeof (Page_Validators) !== 'undefined') {
                for (var i = 0; i < Page_Validators.length; i++) {
                    if (Page_Validators[i].validationGroup === "UsuarioValidation") {
                        ValidatorHookupControlID(Page_Validators[i].controltovalidate, Page_Validators[i]);
                        ValidatorUpdateDisplay(Page_Validators[i]);
                        Page_Validators[i].isvalid = true;
                        if (Page_Validators[i].style.display !== "none") {
                            Page_Validators[i].style.display = "none";
                        }
                    }
                }
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            const currentPath = window.location.pathname;
            const navLinks = document.querySelectorAll('.sidebar ul li a');
            navLinks.forEach(link => {
                const linkPath = new URL(link.href).pathname;
                if (currentPath.includes(linkPath) && linkPath !== '/') {
                    link.classList.add('active');
                } else {
                    link.classList.remove('active');
                }
            });

            const usuarioModal = document.getElementById('<%= usuarioModal.ClientID %>');
            if (usuarioModal && usuarioModal.style.display === 'none' && '<%= hdnModalVisible.Value %>' === 'true') {
                   usuarioModal.style.display = 'flex';
                   document.getElementById('<%= txtNombre.ClientID %>').focus();
            }
        });
    </script>
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    👥 Gestión de Usuarios
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra los usuarios del sistema
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalUsuarios"><%= GetTotalUsuarios() %></div>
            <div class="stat-label">Total Usuarios</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="usuariosActivos"><%= GetUsuariosActivos() %></div>
            <div class="stat-label">Usuarios Activos</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="usuariosAdmin"><%= GetUsuariosAdmin() %></div>
            <div class="stat-label">Administradores</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="usuariosRegistradosHoy"><%= GetUsuariosRegistradosHoy() %></div>
            <div class="stat-label">Registrados Hoy</div>
        </div>
    </div>

    <div class="table-container">
        <div class="table-header">
            <div class="search-bar">
                <asp:TextBox ID="txtSearchUsuarios" runat="server" CssClass="search-input"
                    placeholder="🔍 Buscar usuarios..." AutoPostBack="true"
                    OnTextChanged="txtSearchUsuarios_TextChanged" autocomplete="off"></asp:TextBox>
                <asp:DropDownList ID="ddlEstadoFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEstadoFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">Todos los estados</asp:ListItem>
                    <asp:ListItem Value="Activo">Activo</asp:ListItem>
                    <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlAdminFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAdminFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">Todos los roles</asp:ListItem>
                    <asp:ListItem Value="Administrador">Administrador</asp:ListItem>
                    <asp:ListItem Value="Usuario Regular">Usuario Regular</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False"
                      CssClass="data-table"
                      HeaderStyle-CssClass="data-table-header"
                      RowStyle-CssClass="data-table-row"
                      AlternatingRowStyle-CssClass="data-table-row-alt"
                      AllowPaging="True" PageSize="10" OnPageIndexChanging="gvUsuarios_PageIndexChanging"
                      DataKeyNames="Id" OnRowCommand="gvUsuarios_RowCommand">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" />
                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" SortExpression="Telefono" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class="status-badge status-<%# Eval("EstaActivo").ToString().ToLower() == "true" ? "activo" : "inactivo" %>">
                            <%# Eval("EstaActivo").ToString().ToLower() == "true" ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                   <asp:TemplateField HeaderText="Rol">
                    <ItemTemplate>
                        <span class="role-badge role-<%# Eval("admin").ToString().ToLower() == "true" ? "admin" : "user" %>">
                            <%# Eval("admin").ToString().ToLower() == "true" ? "Administrador" : "Usuario Regular" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-edit" CommandName="EditUsuario" CommandArgument='<%# Eval("Id") %>'>
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn-delete" CommandName="DeleteUsuario" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar a este usuario?');">
                                <i class="fas fa-trash-alt"></i> Eliminar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnToggleAdmin" runat="server"
                                CssClass='<%# Eval("admin").ToString().ToLower() == "true" ? "btn-degradar-admin" : "btn-ascender-admin" %>'
                                CommandName="ToggleAdmin" CommandArgument='<%# Eval("Id") %>'
                                OnClientClick='<%# Eval("admin").ToString().ToLower() == "true" ? "return confirm(\"¿Estás seguro de que quieres degradar a este usuario a usuario regular?\");" : "return confirm(\"¿Estás seguro de que quieres ascender a este usuario a administrador?\");" %>'>
                                <i class="fas fa-user-shield"></i>
                                <%# Eval("admin").ToString().ToLower() == "true" ? "Degradar" : "Ascender" %>
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No se encontraron usuarios.
            </EmptyDataTemplate>
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>
    </div>

    <div class="modal" id="usuarioModal" runat="server" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Usuario
                </h2>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="close-button" OnClick="btnCloseModal_Click">&times;</asp:LinkButton>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label for="<%= txtNombre.ClientID %>" class="form-label">Nombre:</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="El nombre es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtEmail.ClientID %>" class="form-label">Email:</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" autocomplete="off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="El email es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Formato de email inválido." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtTelefono.ClientID %>" class="form-label">Teléfono:</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="15" autocomplete="off"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTelefono" runat="server" ControlToValidate="txtTelefono" ErrorMessage="El teléfono es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtPassword.ClientID %>" class="form-label">Contraseña:</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" autocomplete="new-password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="La contraseña es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
                        ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$"
                        ErrorMessage="La contraseña debe tener al menos 8 caracteres, una letra y un número." Display="Dynamic" ForeColor="Red" ValidationGroup="UsuarioValidation"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group">
                    <label for="<%= ddlIdiomaPreferido.ClientID %>" class="form-label">Idioma Preferido:</label>
                    <asp:DropDownList ID="ddlIdiomaPreferido" runat="server" CssClass="form-control">
                        <asp:ListItem Value="es">Español</asp:ListItem>
                        <asp:ListItem Value="en">Inglés</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group form-check">
                    <asp:CheckBox ID="chkEstaActivo" runat="server" Text="¿Está Activo?" />
                </div>
                <div class="form-group form-check">
                    <asp:CheckBox ID="chkIsAdmin" runat="server" Text="¿Es Administrador?" />
                </div>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnUsuarioId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnModalVisible" runat="server" Value="false" />
                <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelModal_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardarUsuario" runat="server" Text="Guardar Usuario" CssClass="btn btn-primary" OnClick="btnGuardarUsuario_Click" ValidationGroup="UsuarioValidation" />
            </div>
            <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentScript" ContentPlaceHolderID="ScriptContent" runat="server">
</asp:Content>