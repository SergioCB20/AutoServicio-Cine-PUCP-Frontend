<%@ Page Title="Gestión de Películas" Language="C#"
MasterPageFile="~/Admin.Master" AutoEventWireup="true"
CodeBehind="GestionPeliculas.aspx.cs"
Inherits="AutoServicioCineWeb.GestionPeliculas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Películas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/GestionPeliculas.css">
    <script type="text/javascript">
        // Función para previsualizar la imagen seleccionada desde la URL (SE MANTIENE)
        function previewImage(input) {
            const imageUrl = input.value;
            const imgPreviewElement = document.getElementById('<%= imgPreview.ClientID %>');

            if (imageUrl) {
                imgPreviewElement.src = imageUrl;
                imgPreviewElement.style.display = 'block'; // Mostrar la previsualización
            } else {
                imgPreviewElement.src = '';
                imgPreviewElement.style.display = 'none'; // Ocultar si no hay URL
            }
        }

        // Script para activar el elemento del menú de navegación (se mantiene)
        document.addEventListener('DOMContentLoaded', function () {
            const currentPath = window.location.pathname;
            const navLinks = document.querySelectorAll('.sidebar ul li a');
            navLinks.forEach(link => {
                if (link.href && link.href.includes(currentPath)) {
                    link.parentElement.classList.add('active');
                } else {
                    link.parentElement.classList.remove('active');
                }
            });
        });

        // Función para limpiar validadores (se mantiene, llamada desde el CodeBehind)
        function clearModalValidators() {
            if (typeof (Page_Validators) !== 'undefined') {
                for (var i = 0; i < Page_Validators.length; i++) {
                    if (Page_Validators[i].validationGroup === "PeliculaValidation") {
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
    </script>
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    🎬 Gestión de Películas
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra la cartelera de tu cine
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <%-- El botón ahora provoca un PostBack para que el CodeBehind abra el modal --%>
    <asp:Button ID="btnOpenAddModal" runat="server" Text="➕ Agregar Nueva Película" CssClass="btn btn-primary" OnClick="btnOpenAddModal_Click" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalPeliculas"><%= GetTotalPeliculas() %></div>
            <div class="stat-label">Total Películas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="peliculasActivas"><%= GetPeliculasActivas() %></div>
            <div class="stat-label">Activas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="peliculasInactivas"><%= GetPeliculasInactivas() %></div>
            <div class="stat-label">Inactivas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="totalClasificaciones"><%= GetClasificacionesUnicas() %></div>
            <div class="stat-label">Clasificaciones Únicas</div>
        </div>
    </div>

    <div class="table-container">
        <div class="table-header">
            <div class="search-bar">
                <asp:TextBox ID="txtSearchPeliculas" runat="server" CssClass="search-input" placeholder="🔍 Buscar películas..." AutoPostBack="true" OnTextChanged="txtSearchPeliculas_TextChanged"></asp:TextBox>

                <asp:DropDownList ID="ddlClasificacionFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlClasificacionFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">Todas las clasificaciones</asp:ListItem>
                    <asp:ListItem Value="G">G</asp:ListItem>
                    <asp:ListItem Value="PG">PG</asp:ListItem>
                    <asp:ListItem Value="PG-13">PG-13</asp:ListItem>
                    <asp:ListItem Value="R">R</asp:ListItem>
                    <asp:ListItem Value="NC-17">NC-17</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <asp:GridView ID="gvPeliculas" runat="server" AutoGenerateColumns="False"
                      CssClass="data-table"
                      HeaderStyle-CssClass="data-table-header"
                      RowStyle-CssClass="data-table-row"
                      AlternatingRowStyle-CssClass="data-table-row-alt"
                      AllowPaging="True" PageSize="10" OnPageIndexChanging="gvPeliculas_PageIndexChanging"
                      DataKeyNames="PeliculaId" OnRowCommand="gvPeliculas_RowCommand">
            <Columns>
                <asp:BoundField DataField="PeliculaId" HeaderText="ID" SortExpression="PeliculaId" />
                <asp:BoundField DataField="TituloEs" HeaderText="Título (ES)" SortExpression="TituloEs" />
                <asp:BoundField DataField="TituloEn" HeaderText="Título (EN)" SortExpression="TituloEn" />
                <asp:TemplateField HeaderText="Imagen">
                    <ItemTemplate>
                        <img src='<%# Eval("ImagenUrl") %>' alt='<%# Eval("TituloEs") %>' class="movie-image" onerror="this.onerror=null;this.src='/path/to/default_movie_image.png';" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DuracionMin" HeaderText="Duración (min)" SortExpression="DuracionMin" />
                <asp:TemplateField HeaderText="Clasificación">
                    <ItemTemplate>
                        <span class="classification-badge classification-<%# Eval("Clasificacion").ToString().ToLower().Replace("-", "") %>">
                            <%# Eval("Clasificacion") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class="status-badge status-<%# Eval("EstaActiva").ToString().ToLower() == "true" ? "activa" : "inactiva" %>">
                            <%# Eval("EstaActiva").ToString().ToLower() == "true" ? "Activa" : "Inactiva" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaModificacion" HeaderText="Última Mod." DataFormatString="{0:yyyy-MM-dd HH:mm}" SortExpression="FechaModificacion" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-edit" CommandName="EditPelicula" CommandArgument='<%# Eval("PeliculaId") %>'>
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn-delete" CommandName="DeletePelicula" CommandArgument='<%# Eval("PeliculaId") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta película?');">
                                <i class="fas fa-trash-alt"></i> Eliminar
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No se encontraron películas.
            </EmptyDataTemplate>
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>
    </div>

    <%-- Modal para Agregar/Editar Película --%>
    <div class="modal" id="peliculaModal" runat="server"> <%-- Agregamos runat="server" --%>
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Película
                </h2>
                <%-- El botón de cerrar ahora provoca un PostBack con un CommandName específico --%>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="close-button" OnClick="btnCloseModal_Click">&times;</asp:LinkButton>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label for="<%= txtTituloEs.ClientID %>" class="form-label">Título (Español):</label>
                    <asp:TextBox ID="txtTituloEs" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTituloEs" runat="server" ControlToValidate="txtTituloEs" ErrorMessage="El título en español es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtTituloEn.ClientID %>" class="form-label">Título (Inglés):</label>
                    <asp:TextBox ID="txtTituloEn" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTituloEn" runat="server" ControlToValidate="txtTituloEn" ErrorMessage="El título en inglés es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtSinopsisEs.ClientID %>" class="form-label">Sinopsis (Español):</label>
                    <asp:TextBox ID="txtSinopsisEs" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSinopsisEs" runat="server" ControlToValidate="txtSinopsisEs" ErrorMessage="La sinopsis en español es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtSinopsisEn.ClientID %>" class="form-label">Sinopsis (Inglés):</label>
                    <asp:TextBox ID="txtSinopsisEn" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSinopsisEn" runat="server" ControlToValidate="txtSinopsisEn" ErrorMessage="La sinopsis en inglés es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtDuracionMin.ClientID %>" class="form-label">Duración (minutos):</label>
                    <asp:TextBox ID="txtDuracionMin" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDuracionMin" runat="server" ControlToValidate="txtDuracionMin" ErrorMessage="La duración es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rvDuracionMin" runat="server" ControlToValidate="txtDuracionMin" MinimumValue="1" MaximumValue="500" Type="Integer" ErrorMessage="La duración debe ser un número positivo (1-500)." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RangeValidator>
                </div>
                <div class="form-group">
                    <label for="<%= ddlClasificacion.ClientID %>" class="form-label">Clasificación:</label>
                    <asp:DropDownList ID="ddlClasificacion" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">Seleccionar</asp:ListItem>
                        <asp:ListItem Value="G">G</asp:ListItem>
                        <asp:ListItem Value="PG">PG</asp:ListItem>
                        <asp:ListItem Value="PG-13">PG-13</asp:ListItem>
                        <asp:ListItem Value="R">R</asp:ListItem>
                        <asp:ListItem Value="NC-17">NC-17</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvClasificacionModal" runat="server" ControlToValidate="ddlClasificacion" InitialValue="" ErrorMessage="La clasificación es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label for="<%= txtImagenUrl.ClientID %>" class="form-label">URL de Imagen:</label>
                    <asp:TextBox ID="txtImagenUrl" runat="server" CssClass="form-control" onkeyup="previewImage(this);" onchange="previewImage(this);"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvImagenUrl" runat="server" ControlToValidate="txtImagenUrl" ErrorMessage="La URL de la imagen es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revImagenUrl" runat="server" ControlToValidate="txtImagenUrl"
                        ValidationExpression="^(http(s?):\/\/)[\w\-\._~:\/\?#\[\]@!\$&'\(\)\*\+,;=\.\/]+(?:jpg|gif|png|jpeg|bmp|svg|webp)$"
                        ErrorMessage="Formato de URL de imagen inválido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group">
                    <label class="form-label">Previsualización:</label>
                    <asp:Image ID="imgPreview" runat="server" class="image-preview" />
                    <asp:HiddenField ID="hdnExistingImageUrl" runat="server" />
                </div>

                <div class="form-group form-check">
                    <asp:CheckBox ID="chkEstaActiva" runat="server" Text="¿Está Activa?" />
                </div>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnPeliculaId" runat="server" Value="0" />
                <%-- El botón Cancelar ahora provoca un PostBack para que el CodeBehind cierre el modal --%>
                <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelModal_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardarPelicula" runat="server" Text="Guardar Película" CssClass="btn btn-primary" OnClick="btnGuardarPelicula_Click" ValidationGroup="PeliculaValidation" />
            </div>
            <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
        </div>
    </div>

    <div id="loadingIndicator" style="display: none;">
        <div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 9999; display: flex; align-items: center; justify-content: center;">
            <div style="background: white; padding: 20px; border-radius: 5px;">
                Cargando...
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <%-- Script para previsualización de imagen se mantuvo en ContentHead --%>
    <%-- No hay scripts adicionales necesarios aquí ya que el CodeBehind maneja el modal --%>
</asp:Content>