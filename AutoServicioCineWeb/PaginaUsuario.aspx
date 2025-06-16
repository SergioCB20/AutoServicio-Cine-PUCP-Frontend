<%@ Page Title="" Language="C#" MasterPageFile="~/Form.Master" AutoEventWireup="true" CodeBehind="PaginaUsuario.aspx.cs" Inherits="AutoServicioCineWeb.PaginaUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Mi perfil
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/perfil.css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-main-content">
        <!-- Sección de Información Personal -->
        <div class="profile-section">
            <div class="profile-header">
                <div class="profile-avatar">
                    <img id="imgAvatar" runat="server" src="./images/default-avatar.png" alt="Avatar del usuario" />
                    <div class="avatar-upload">
                        <asp:FileUpload ID="fileUploadAvatar" runat="server" accept="image/*" />
                        <label for="<%= fileUploadAvatar.ClientID %>">
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" d="M6.827 6.175A2.31 2.31 0 0 1 5.186 7.23c-.38.054-.757.112-1.134.175C2.999 7.58 2.25 8.507 2.25 9.574V18a2.25 2.25 0 0 0 2.25 2.25h15A2.25 2.25 0 0 0 21.75 18V9.574c0-1.067-.75-1.994-1.802-2.169a47.865 47.865 0 0 0-1.134-.175 2.31 2.31 0 0 1-1.64-1.055l-.822-1.316a2.192 2.192 0 0 0-1.736-1.039 48.774 48.774 0 0 0-5.232 0 2.192 2.192 0 0 0-1.736 1.039l-.821 1.316Z" />
                                <path stroke-linecap="round" stroke-linejoin="round" d="M16.5 12.75a4.5 4.5 0 1 1-9 0 4.5 4.5 0 0 1 9 0ZM18.75 10.5h.008v.008h-.008V10.5Z" />
                            </svg>
                        </label>
                    </div>
                </div>
                <div class="profile-info">
                    <h2 id="nombreUsuario" runat="server">Sergio Mendoza</h2>
                    <p id="emailUsuario" runat="server">sergio.mendoza@pucp.edu.pe</p>
                    <span class="member-since">Miembro desde: <span id="fechaRegistro" runat="server">Enero 2024</span></span>
                </div>
            </div>

            <!-- Pestañas de navegación -->
            <div class="profile-tabs">
                <button class="tab-button active" onclick="showTab('personal')">Información Personal</button>
                <button class="tab-button" onclick="showTab('historial')">Historial de Compras</button>
                <button class="tab-button" onclick="showTab('configuracion')">Configuración</button>
            </div>

            <!-- Contenido de las pestañas -->
            <div class="tab-content">
                <!-- Información Personal -->
                <div id="personal" class="tab-pane active">
                    <div class="form-group-container">
                        <div class="form-group">
                            <label for="txtNombres">Nombres:</label>
                            <asp:TextBox ID="txtNombres" runat="server" CssClass="form-input" placeholder="Ingrese sus nombres"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtApellidos">Apellidos:</label>
                            <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-input" placeholder="Ingrese sus apellidos"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtEmail">Correo Electrónico:</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" TextMode="Email" placeholder="correo@ejemplo.com"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtTelefono">Teléfono:</label>
                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-input" placeholder="+51 999 999 999"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtFechaNacimiento">Fecha de Nacimiento:</label>
                            <asp:TextBox ID="txtFechaNacimiento" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="ddlGenero">Género:</label>
                            <asp:DropDownList ID="ddlGenero" runat="server" CssClass="form-input">
                                <asp:ListItem Text="Seleccionar..." Value=""></asp:ListItem>
                                <asp:ListItem Text="Masculino" Value="M"></asp:ListItem>
                                <asp:ListItem Text="Femenino" Value="F"></asp:ListItem>
                                <asp:ListItem Text="Otro" Value="O"></asp:ListItem>
                                <asp:ListItem Text="Prefiero no decir" Value="N"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnGuardarPerfil" runat="server" Text="Guardar Cambios" CssClass="button primary" OnClick="btnGuardarPerfil_Click" />
                        <button type="button" class="button secondary" onclick="cancelarCambios()">Cancelar</button>
                    </div>
                </div>

                <!-- Historial de Compras -->
                <div id="historial" class="tab-pane">
                    <div class="purchase-history">
                        <div class="history-filters">
                            <select class="form-input filter-select">
                                <option value="">Todos los meses</option>
                                <option value="2024-12">Diciembre 2024</option>
                                <option value="2024-11">Noviembre 2024</option>
                                <option value="2024-10">Octubre 2024</option>
                            </select>
                            <select class="form-input filter-select">
                                <option value="">Todas las películas</option>
                                <option value="accion">Acción</option>
                                <option value="comedia">Comedia</option>
                                <option value="drama">Drama</option>
                            </select>
                        </div>

                        <div class="purchase-list">
                            <div class="purchase-item">
                                <div class="purchase-movie">
                                    <img src="./images/movie1.jpg" alt="Película" class="movie-thumbnail" />
                                    <div class="movie-details">
                                        <h4>Avengers: Endgame</h4>
                                        <p>Sala 3 - 18/05/2024 - 7:30 PM</p>
                                        <span class="seats">Butacas: F5, F6</span>
                                    </div>
                                </div>
                                <div class="purchase-info">
                                    <span class="purchase-date">15/05/2024</span>
                                    <span class="purchase-amount">S/ 24.00</span>
                                    <span class="purchase-status completed">Completado</span>
                                </div>
                                <div class="purchase-actions">
                                    <button class="button-link">Ver Detalles</button>
                                    <button class="button-link">Descargar Ticket</button>
                                </div>
                            </div>

                            <div class="purchase-item">
                                <div class="purchase-movie">
                                    <img src="./images/movie2.jpg" alt="Película" class="movie-thumbnail" />
                                    <div class="movie-details">
                                        <h4>Spider-Man: No Way Home</h4>
                                        <p>Sala 1 - 10/05/2024 - 4:15 PM</p>
                                        <span class="seats">Butacas: A1, A2, A3</span>
                                    </div>
                                </div>
                                <div class="purchase-info">
                                    <span class="purchase-date">08/05/2024</span>
                                    <span class="purchase-amount">S/ 36.00</span>
                                    <span class="purchase-status completed">Completado</span>
                                </div>
                                <div class="purchase-actions">
                                    <button class="button-link">Ver Detalles</button>
                                    <button class="button-link">Descargar Ticket</button>
                                </div>
                            </div>

                            <div class="purchase-item">
                                <div class="purchase-movie">
                                    <img src="./images/movie3.jpg" alt="Película" class="movie-thumbnail" />
                                    <div class="movie-details">
                                        <h4>Dune: Part Two</h4>
                                        <p>Sala 2 - 25/04/2024 - 9:45 PM</p>
                                        <span class="seats">Butacas: H8, H9</span>
                                    </div>
                                </div>
                                <div class="purchase-info">
                                    <span class="purchase-date">23/04/2024</span>
                                    <span class="purchase-amount">S/ 28.00</span>
                                    <span class="purchase-status cancelled">Cancelado</span>
                                </div>
                                <div class="purchase-actions">
                                    <button class="button-link">Ver Detalles</button>
                                    <button class="button-link disabled">Reembolso Procesado</button>
                                </div>
                            </div>
                        </div>

                        <div class="pagination">
                            <button class="button secondary">Anterior</button>
                            <span class="page-info">Página 1 de 3</span>
                            <button class="button secondary">Siguiente</button>
                        </div>
                    </div>
                </div>

                <!-- Configuración -->
                <div id="configuracion" class="tab-pane">
                    <div class="settings-section">
                        <h3>Notificaciones</h3>
                        <div class="setting-item">
                            <label class="switch">
                                <asp:CheckBox ID="chkEmailPromociones" runat="server" />
                                <span class="slider"></span>
                            </label>
                            <div class="setting-info">
                                <span class="setting-title">Promociones por email</span>
                                <span class="setting-description">Recibir ofertas especiales y descuentos</span>
                            </div>
                        </div>
                        <div class="setting-item">
                            <label class="switch">
                                <asp:CheckBox ID="chkEmailEstrenos" runat="server" />
                                <span class="slider"></span>
                            </label>
                            <div class="setting-info">
                                <span class="setting-title">Notificaciones de estrenos</span>
                                <span class="setting-description">Alertas sobre nuevas películas</span>
                            </div>
                        </div>
                        <div class="setting-item">
                            <label class="switch">
                                <asp:CheckBox ID="chkSMSRecordatorios" runat="server" />
                                <span class="slider"></span>
                            </label>
                            <div class="setting-info">
                                <span class="setting-title">Recordatorios por SMS</span>
                                <span class="setting-description">Recordatorios de funciones reservadas</span>
                            </div>
                        </div>
                    </div>

                    <div class="settings-section">
                        <h3>Privacidad</h3>
                        <div class="setting-item">
                            <label class="switch">
                                <asp:CheckBox ID="chkPerfilPublico" runat="server" />
                                <span class="slider"></span>
                            </label>
                            <div class="setting-info">
                                <span class="setting-title">Perfil público</span>
                                <span class="setting-description">Hacer visible mi perfil a otros usuarios</span>
                            </div>
                        </div>
                        <div class="setting-item">
                            <label class="switch">
                                <asp:CheckBox ID="chkCompartirHistorial" runat="server" />
                                <span class="slider"></span>
                            </label>
                            <div class="setting-info">
                                <span class="setting-title">Compartir historial de películas</span>
                                <span class="setting-description">Mostrar las películas que he visto</span>
                            </div>
                        </div>
                    </div>

                    <div class="settings-section">
                        <h3>Seguridad</h3>
                        <div class="security-actions">
                            <button class="button secondary" onclick="cambiarContrasena()">Cambiar Contraseña</button>
                            <button class="button secondary" onclick="activar2FA()">Activar Autenticación en Dos Pasos</button>
                            <button class="button danger" onclick="cerrarSesionesActivas()">Cerrar Todas las Sesiones</button>
                        </div>
                    </div>

                    <div class="settings-section danger-zone">
                        <h3>Zona de Peligro</h3>
                        <p>Las siguientes acciones son irreversibles. Procede con precaución.</p>
                        <div class="danger-actions">
                            <button class="button danger" onclick="desactivarCuenta()">Desactivar Cuenta</button>
                            <button class="button danger" onclick="eliminarCuenta()">Eliminar Cuenta Permanentemente</button>
                        </div>
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnGuardarConfiguracion" runat="server" Text="Guardar Configuración" CssClass="button primary" OnClick="btnGuardarConfiguracion_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<%--<asp:Content ID="Content4" ContentPlaceHolderID="TicketsSummary" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TotalAmount" runat="server">
</asp:Content>--%>
<asp:Content ID="Content6" ContentPlaceHolderID="ActionButtons" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
