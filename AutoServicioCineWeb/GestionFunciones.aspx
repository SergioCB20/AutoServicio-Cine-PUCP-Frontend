<%@ Page Title="Gestión de Funciones" Language="C#"
    MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="GestionFunciones.aspx.cs"
    Inherits="AutoServicioCineWeb.GestionFunciones" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Funciones
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/gestionPeliculas.css" />
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    🎞️ Funciones de:
    <asp:Literal ID="litTituloPelicula" runat="server" />
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra las funciones programadas
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <asp:Button ID="btnOpenAddFuncionModal" runat="server" Text="➕ Nueva Función" CssClass="btn btn-primary"
        OnClick="btnOpenAddFuncionModal_Click" />
    <asp:Button ID="btnOpenFuncionCsvModal" runat="server" Text="📥 Cargar Funciones CSV"
        CssClass="btn btn-secondary" BackColor="ForestGreen"
        OnClick="btnOpenFuncionCsvModal_Click" />

</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number"><%= GetTotalFunciones() %></div>
            <div class="stat-label">Total Funciones</div>
        </div>
        <div class="stat-card">
            <div class="stat-number"><%= GetFuncionesActivas() %></div>
            <div class="stat-label">Activas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number"><%= GetFuncionesInactivas() %></div>
            <div class="stat-label">Inactivas</div>
        </div>
    </div>

    <div class="table-container">
        <div class="table-header">
            <div class="search-bar">
                <asp:DropDownList ID="ddlFormatoFilter" runat="server" CssClass="filter-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFormatoFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">Todos los formatos</asp:ListItem>
                    <asp:ListItem Value="2D">2D</asp:ListItem>
                    <asp:ListItem Value="3D">3D</asp:ListItem>
                    <asp:ListItem Value="IMAX">IMAX</asp:ListItem>
                    <asp:ListItem Value="4DX">4DX</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <asp:GridView ID="gvFunciones" runat="server" AutoGenerateColumns="False"
            CssClass="data-table" AllowPaging="true" PageSize="10"
            DataKeyNames="FuncionId" OnRowCommand="gvFunciones_RowCommand"
            OnPageIndexChanging="gvFunciones_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="FuncionId" HeaderText="ID" />
                <asp:BoundField DataField="FechaHora" HeaderText="Fecha y Hora" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                    HtmlEncode="false" />
                <asp:BoundField DataField="FormatoProyeccion" HeaderText="Formato" />
                <asp:BoundField DataField="Idioma" HeaderText="Idioma" />
                <asp:TemplateField HeaderText="Subtítulos">
                    <ItemTemplate>
                        <%# (bool)Eval("Subtitulos") ? "Sí" : "No" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class='status-badge status-<%# ((bool)Eval("EstaActiva")) ? "activa" : "inactiva" %>'>
                            <%# ((bool)Eval("EstaActiva")) ? "Activa" : "Inactiva" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEditarFuncion" runat="server" CssClass="btn-edit" CommandName="EditFuncion"
                                CommandArgument='<%# Eval("FuncionId") %>'>
                        <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnEliminarFuncion" runat="server" CssClass="btn-delete" CommandName="DeleteFuncion"
                                CommandArgument='<%# Eval("FuncionId") %>' OnClientClick="return confirm('¿Estás seguro que deseas eliminar esta función?');">
                        <i class="fas fa-trash-alt"></i> Eliminar
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>No se encontraron funciones.</EmptyDataTemplate>
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>

    </div>

    <%-- Modal para crear o editar función --%>
    <div class="modal" id="funcionModal" runat="server" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litFuncionModalTitle" runat="server" />
                </h2>
                <asp:LinkButton ID="btnCloseFuncionModal" runat="server" CssClass="close-button"
                    OnClick="btnCloseFuncionModal_Click">&times;</asp:LinkButton>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Película:</label>
                    <asp:Label ID="lblPeliculaNombre" runat="server" CssClass="form-control disabled" />
                    <asp:HiddenField ID="hdnPeliculaId" runat="server" />
                </div>

                <div class="form-group">
                    <label>Sala:</label>
                    <asp:DropDownList ID="ddlSalas" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Fecha y Hora:</label>
                    <asp:TextBox ID="txtFechaHora" runat="server" CssClass="form-control" placeholder="yyyy-MM-dd HH:mm" />
                </div>
                <div class="form-group">
                    <label>Formato:</label>
                    <asp:DropDownList ID="ddlFormato" runat="server" CssClass="form-control">
                        <asp:ListItem Text="2D" Value="2D" />
                        <asp:ListItem Text="3D" Value="3D" />
                        <asp:ListItem Text="IMAX" Value="IMAX" />
                        <asp:ListItem Text="4DX" Value="4DX" />
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label>Idioma:</label>
                    <asp:TextBox ID="txtIdioma" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group form-check">
                    <asp:CheckBox ID="chkSubtitulos" runat="server" Text="¿Con subtítulos?" />
                </div>
                <div class="form-group form-check">
                    <asp:CheckBox ID="chkFuncionActiva" runat="server" Text="¿Activa?" />
                </div>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnFuncionId" runat="server" />
                <asp:Button ID="btnCancelarFuncion" runat="server" Text="Cancelar" CssClass="btn btn-secondary"
                    OnClick="btnCancelarFuncion_Click" />
                <asp:Button ID="btnGuardarFuncion" runat="server" Text="Guardar Función" CssClass="btn btn-primary"
                    OnClick="btnGuardarFuncion_Click" />
            </div>
            <asp:Literal ID="litMensajeCrear" runat="server" />
        </div>
    </div>
    <%-- Modal para Carga de CSV de Funciones --%>
    <div class="modal" id="funcionCsvModal" runat="server" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">Cargar Funciones desde CSV</h2>
                <asp:LinkButton ID="btnCloseFuncionCsvModal" runat="server" CssClass="close-button"
                    OnClick="btnCloseCsvModal_Click">&times;</asp:LinkButton>
            </div>

            <div class="form-group" style="padding: 20px;">
                <div class="csv-upload-header">
                    <label for="<%= FileUploadFuncionCsv.ClientID %>" class="form-label">
                        Selecciona un archivo CSV:
                    <button type="button" class="help-button" onclick="toggleCsvHelp();">?</button>
                    </label>
                </div>

                <asp:FileUpload ID="FileUploadFuncionCsv" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvFuncionCsv" runat="server" ControlToValidate="FileUploadFuncionCsv"
                    ErrorMessage="Por favor selecciona un archivo CSV." ValidationGroup="FuncionCsvUpload"
                    Display="Dynamic" ForeColor="Red" />
                <asp:CustomValidator ID="cvFuncionCsvFileExtension" runat="server" ControlToValidate="FileUploadFuncionCsv"
                    OnServerValidate="cvFuncionCsvFileExtension_ServerValidate"
                    ErrorMessage="El archivo debe tener extensión .csv" ValidationGroup="FuncionCsvUpload"
                    Display="Dynamic" ForeColor="Red" />
            </div>

            <%-- Ayuda CSV para funciones --%>
            <div id="csvHelpBox" class="csv-help-box">
                <p class="mb-2">El archivo CSV debe tener las siguientes columnas (el orden no importa):
                </p>
                <ul class="csv-column-list">
                    <li><code>FechaHora</code> (formato <code>yyyy-MM-dd HH:mm</code>)</li>
                    <li><code>SalaId</code></li>
                    <li><code>FormatoProyeccion</code> (2D, 3D, IMAX, 4DX)</li>
                    <li><code>Idioma</code></li>
                    <li><code>Subtitulos</code> (TRUE/FALSE o 1/0)</li>
                    <li><code>EstaActiva</code> (TRUE/FALSE o 1/0)</li>
                </ul>
                <p class="small text-muted">Asegúrate de que los valores booleanos sean válidos y que
                    la <code>FechaHora</code> tenga el formato correcto.</p>
            </div>

            <div class="form-actions">
                <asp:Button ID="btnCancelFuncionCsvModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary"
                    OnClick="btnCancelFuncionCsvModal_Click" CausesValidation="false" />
                <asp:Button ID="btnUploadFuncionCsv" runat="server" Text="Subir CSV" CssClass="btn btn-primary"
                    OnClick="btnUploadFuncionCsv_Click" ValidationGroup="FuncionCsvUpload" />
            </div>

            <asp:Literal ID="litMensajeFuncionCsv" runat="server" />
        </div>
    </div>
    <script type="text/javascript">
        function toggleCsvHelp() {
            const csvHelpBox = document.getElementById('csvHelpBox');
            if (csvHelpBox) {
                csvHelpBox.classList.toggle('visible');
            }
        }
    </script>
</asp:Content>

