<%@ Page Title="Gestión de Salas" Language="C#"  MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="GestionSalas.aspx.cs" Inherits="AutoServicioCineWeb.GestionSalas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Salas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/GestionSalas.css">
    <script type="text/javascript">      

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
                    if (Page_Validators[i].validationGroup === "SalaValidation") {
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

        // NUEVA FUNCIÓN para el botón de ayuda del CSV
        function toggleCsvHelp() {
            const csvHelpBox = document.getElementById('csvHelpBox');
            if (csvHelpBox) {
                csvHelpBox.classList.toggle('visible');
            }
        }
    </script>
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    Gestión de Salas
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra las salas disponibles en el sistema.
</asp:Content>
<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <%-- Botón para agregar nueva sala --%>
    <asp:Button ID="btnOpenAddModal" runat="server" Text="➕ Agregar Nueva Sala" CssClass="btn btn-primary" OnClick="btnOpenAddModal_Click" />
    <%-- NUEVO Botón para importar CSV --%>
    <asp:Button ID="btnOpenCsvImportModal" runat="server" Text="📤 Ingresar datos con CSV" CssClass="btn btn-secondary" BackColor="ForestGreen" OnClick="btnOpenCsvImportModal_Click" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalPeliculas"><%= GetTotalSalas() %></div>
            <div class="stat-label">Total Películas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="peliculasActivas"><%= GetSalasActivas() %></div>
            <div class="stat-label">Activas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="peliculasInactivas"><%= GetSalasInactivas() %></div>
            <div class="stat-label">Inactivas</div>
        </div>
    </div>

    <div class="table-container">
    <div class="table-header">
        <div class="search-bar">
            <asp:TextBox ID="txtSearchSalas" runat="server" CssClass="search-input" placeholder="🔍 Buscar salas..." AutoPostBack="true" OnTextChanged="txtSearchSalas_TextChanged"></asp:TextBox>

            <asp:DropDownList ID="ddlTipoFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoFilter_SelectedIndexChanged">
                <asp:ListItem Value="">Todas las clasificaciones</asp:ListItem>
                <asp:ListItem Value="estándar">estándar</asp:ListItem>
                <asp:ListItem Value="3D">3D</asp:ListItem>
                <asp:ListItem Value="premium">premium</asp:ListItem>
                <asp:ListItem Value="4D">4D</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <asp:GridView ID="gvSalas" runat="server" AutoGenerateColumns="False"
                  CssClass="data-table"
                  HeaderStyle-CssClass="data-table-header"
                  RowStyle-CssClass="data-table-row"
                  AlternatingRowStyle-CssClass="data-table-row-alt"
                  AllowPaging="True" PageSize="10" OnPageIndexChanging="gvSalas_PageIndexChanging"
                  DataKeyNames="SalaId" OnRowCommand="gvSalas_RowCommand">
        <Columns>
            <asp:BoundField DataField="SalaId" HeaderText="ID" SortExpression="SalaId" />
            <asp:BoundField DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" />
            <asp:BoundField DataField="Capacidad" HeaderText="Capacidad " SortExpression="Capacidad" />
            <asp:TemplateField HeaderText="TipoSala">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlTipoSala" runat="server" CssClass="form-input">
                        <asp:ListItem Text="Seleccionar" Value="" />
                        <asp:ListItem Text="Estandar" Value="Estandar" />
                        <asp:ListItem Text="3D" Value="3D" />
                        <asp:ListItem Text="Premium" Value="Premium" />
                        <asp:ListItem Text="4DX" Value="4DX" />
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Estado">
                <ItemTemplate>
                    <span class="status-badge status-<%# Eval("EstaActiva").ToString().ToLower() == "true" ? "activa" : "inactiva" %>">
                        <%# Eval("EstaActiva").ToString().ToLower() == "true" ? "Activa" : "Inactiva" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
           
            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <div class="action-buttons">
                        <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-edit" CommandName="EditSala" CommandArgument='<%# Eval("SalaId") %>'>
                            <i class="fas fa-edit"></i> Editar
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn-delete" CommandName="DeleteSala" CommandArgument='<%# Eval("SalaId") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta sala?');">
                            <i class="fas fa-trash-alt"></i> Eliminar
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No se encontraron salas.
        </EmptyDataTemplate>
        <PagerStyle CssClass="gridview-pager" />
    </asp:GridView>
</div>
 <%-- Modal para Agregar/Editar Sala --%>
 <div class="modal" id="salaModal" runat="server" style="display: none;">
     <div class="modal-content">
         <div class="modal-header">
             <h2 class="modal-title">
                 <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Sala
             </h2>
             <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="close-button" OnClick="btnCloseModal_Click">&times;</asp:LinkButton>
         </div>

         <div class="form-grid">
             <div class="form-group">
                 <label for="<%= Nombre.ClientID %>" class="form-label">Nombre :</label>
                 <asp:TextBox ID="Nombre" runat="server" CssClass="form-control"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="Nombre" ErrorMessage="El Nombre de sala es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="SalaValidation"></asp:RequiredFieldValidator>
             </div>
             <div class="form-group">
                 <label for="<%= Capacidad.ClientID %>" class="form-label">Capacidad:</label>
                 <asp:TextBox ID="Capacidad" runat="server" CssClass="form-control"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvTituloEn" runat="server" ControlToValidate="Capacidad" ErrorMessage="La capacidad es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="SalaValidation"></asp:RequiredFieldValidator>
                 <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="Capacidad" MinimumValue="20" MaximumValue="400" Type="Integer" ErrorMessage="La capacidad debe ser un número positivo (20-400)." Display="Dynamic" ForeColor="Red" ValidationGroup="SalaValidation"></asp:RangeValidator>
             </div>
             <div class="form-group">
                <label for="<%= ddlTipoSala.ClientID %>" class="form-label">Tipo de Sala:</label>
                <asp:DropDownList ID="ddlTipoSala" runat="server" CssClass="form-control">
                    <asp:ListItem Text="-- Seleccione --" Value="" />
                    <asp:ListItem Text="Estándar" Value="estandar" />
                    <asp:ListItem Text="3D" Value="3D" />
                    <asp:ListItem Text="Premium" Value="premium" />
                    <asp:ListItem Text="4D" Value="4D" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator 
                    ID="rfvTipoSala" 
                    runat="server" 
                    ControlToValidate="ddlTipoSala" 
                    InitialValue="" 
                    ErrorMessage="Debe seleccionar un tipo de sala." 
                    Display="Dynamic" 
                    ForeColor="Red" 
                    ValidationGroup="PeliculaValidation">
                </asp:RequiredFieldValidator>
            </div>                      

             <div class="form-group form-check">
                 <asp:CheckBox ID="chkEstaActiva" runat="server" Text="¿Está Activa?" />
             </div>
         </div>

         <div class="form-actions">
             <asp:HiddenField ID="hdnSalaId" runat="server" Value="0" />
             <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelModal_Click" CausesValidation="false" />
             <asp:Button ID="btnGuardarPelicula" runat="server" Text="Guardar Película" CssClass="btn btn-primary" OnClick="btnGuardarSala_Click" ValidationGroup="SalaValidation" />
         </div>
         <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
     </div>
 </div>
<%-- NUEVO Modal para Carga de CSV --%>
<div class="modal" id="csvUploadModal" runat="server" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title">Cargar Salas desde CSV</h2>
            <asp:LinkButton ID="btnCloseCsvModal" runat="server" CssClass="close-button" OnClick="btnCloseCsvModal_Click">&times;</asp:LinkButton>
        </div>
        <div class="form-group" style="padding:20px;">
            <div class="csv-upload-header"> <%-- Contenedor para el label y el botón de ayuda --%>
                <label for="<%= FileUploadCsv.ClientID %>" class="form-label">
                    Selecciona un archivo CSV:
                    <button type="button" class="help-button" onclick="toggleCsvHelp();">?</button>
                </label>
            </div>
            <asp:FileUpload ID="FileUploadCsv" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvFileUploadCsv" runat="server" ControlToValidate="FileUploadCsv" ErrorMessage="Por favor, selecciona un archivo CSV." Display="Dynamic" ForeColor="Red" ValidationGroup="CsvUploadValidation"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="cvCsvFileExtension" runat="server" ControlToValidate="FileUploadCsv"
                ErrorMessage="El archivo debe ser un CSV (.csv)." OnServerValidate="cvCsvFileExtension_ServerValidate"
                Display="Dynamic" ForeColor="Red" ValidationGroup="CsvUploadValidation"></asp:CustomValidator>
        </div>
        <div id="csvHelpBox" class="csv-help-box">
            <p class="mb-2">El archivo CSV debe tener las siguientes columnas (el orden no importa):</p>
            <ul class="csv-column-list">
                <li>`Nombre`</li>
                <li>`Capacidad`</li>
                <li>`TipoSala`</li>
                <li>`EstaActiva` (TRUE/FALSE o 1/0)</li>
            </ul>
            <p class="small text-muted">Asegúrate de que los valores booleanos para `EstaActiva` sean `TRUE` o `FALSE`, o `1` o `0`.</p>
            <p class="small text-muted">Las URLs de imagen deben ser válidas y apuntar a imágenes en línea.</p>
        </div>


        <div class="form-actions">
            <asp:Button ID="btnCancelCsvModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelCsvModal_Click" CausesValidation="false" />
            <asp:Button ID="btnUploadCsv" runat="server" Text="Subir CSV" CssClass="btn btn-primary" OnClick="btnUploadCsv_Click" ValidationGroup="CsvUploadValidation" />
        </div>
        <asp:Literal ID="litMensajeCsvModal" runat="server"></asp:Literal>
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
</asp:Content>