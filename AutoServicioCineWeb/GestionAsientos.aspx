<%@ Page Language="C#"  MasterPageFile="~/Admin.Master" AutoEventWireup="true" 
    CodeBehind="GestionAsientos.aspx.cs" Inherits="AutoServicioCineWeb.GestionAsientos" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Asientos
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/GestionAsientos.css">
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
                    if (Page_Validators[i].validationGroup === "AsientoValidation") {
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
    Gestión de Asientos
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra los asientos disponibles en el sistema.
</asp:Content>
<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <%-- Botón para agregar nuevo asiento --%>
    <asp:Button ID="btnOpenAddModal" runat="server" Text="➕ Agregar Nuevo Asiento" CssClass="btn btn-primary" OnClick="btnOpenAddModal_Click" />
    <%-- NUEVO Botón para importar CSV --%>
    <asp:Button ID="btnOpenCsvImportModal" runat="server" Text="📤 Ingresar datos con CSV" CssClass="btn btn-secondary" BackColor="ForestGreen" OnClick="btnOpenCsvImportModal_Click" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <asp:HiddenField ID="hfSalaId" runat="server" />

        <div class="stat-card">
            <div class="stat-number" id="totalAsientos"><%= GetTotalAsientos() %></div>
            <div class="stat-label">Total Asientos</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="asientosActivas"><%= GetAsientosActivos() %></div>
            <div class="stat-label">Activas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="asientosInactivas"><%= GetAsientosInactivos() %></div>
            <div class="stat-label">Inactivas</div>
        </div>
    </div>
    
    <div class="table-container">
    <div class="table-header">
        <div class="search-bar">
            <asp:TextBox ID="txtSearchAsientos" runat="server" CssClass="search-input" placeholder="🔍 Buscar asientos..." AutoPostBack="true" OnTextChanged="txtSearchAsientos_TextChanged"></asp:TextBox>

            <asp:DropDownList ID="ddlTipoFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoFilter_SelectedIndexChanged">
                <asp:ListItem Value="">Todas las clasificaciones</asp:ListItem>
                <asp:ListItem Value="NORMAL">normal</asp:ListItem>
                <asp:ListItem Value="VIP">vip</asp:ListItem>
                <asp:ListItem Value="DISCAPACITADO">discapacitado</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>
    <asp:GridView ID="gvAsientos" runat="server" AutoGenerateColumns="False"
                  CssClass="data-table"
                  HeaderStyle-CssClass="data-table-header"
                  RowStyle-CssClass="data-table-row"
                  AlternatingRowStyle-CssClass="data-table-row-alt"
                  AllowPaging="True" PageSize="10" OnPageIndexChanging="gvAsientos_PageIndexChanging"
                  DataKeyNames="Id" OnRowCommand="gvAsientos_RowCommand">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" />
            <asp:BoundField DataField="Fila" HeaderText="Fila " SortExpression="Fila" />
            <asp:BoundField DataField="Numero" HeaderText="Numero " SortExpression="Numero" />

            <asp:TemplateField HeaderText="TipoAsiento">
                <ItemTemplate>
                    <span class="classification-badge classification-<%# Eval("Tipo").ToString() %>">
                        <%# Eval("Tipo") %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Estado">
                <ItemTemplate>
                    <span class="status-badge status-<%# Eval("Activo").ToString().ToLower() == "true" ? "activa" : "inactiva" %>">
                        <%# Eval("Activo").ToString().ToLower() == "true" ? "Activa" : "Inactiva" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaModificacion" HeaderText="Última Mod." DataFormatString="{0:yyyy-MM-dd HH:mm}" SortExpression="FechaModificacion" />
           
            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <div class="action-buttons">
                        <asp:LinkButton ID="btnEditarAsiento" runat="server" CssClass="btn-edit" CommandName="EditAsiento" CommandArgument='<%# Eval("Id") %>'>
                            <i class="fas fa-edit"></i> Editar
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnEliminarAsiento" runat="server" CssClass="btn-delete" CommandName="DeleteAsiento" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar este asiento?');">
                            <i class="fas fa-trash-alt"></i> Eliminar
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No se encontraron asientos.
        </EmptyDataTemplate>
        <PagerStyle CssClass="gridview-pager" />
    </asp:GridView>
</div>
<%-- Modal para Agregar/Editar Asiento --%>
<div class="modal" id="asientoModal" runat="server" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title">
                <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Asiento
            </h2>
            <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="close-button" OnClick="btnCloseModal_Click">&times;</asp:LinkButton>
        </div>

        <div class="form-grid">

            <div class="form-group">
                <label for="<%= Fila.ClientID %>" class="form-label">Fila:</label>
                <asp:TextBox ID="Fila" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFila" runat="server" ControlToValidate="Fila" ErrorMessage="La Fila es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="AsientoValidation"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <label for="<%= Numero.ClientID %>" class="form-label">Numero:</label>
                <asp:TextBox ID="Numero" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNumero" runat="server" ControlToValidate="Numero" ErrorMessage="El Numero es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="AsientoValidation"></asp:RequiredFieldValidator>
                 <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="Numero" MinimumValue="1" MaximumValue="200" Type="Integer" ErrorMessage="El numerp debe ser un número positivo (20-400)." Display="Dynamic" ForeColor="Red" ValidationGroup="AsientoValidation"></asp:RangeValidator>   
            </div>

            <div class="form-group">
                <label for="<%= ddlTipoAsiento.ClientID %>" class="form-label">Tipo de Asiento:</label>
                <asp:DropDownList ID="ddlTipoAsiento" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Seleccionar</asp:ListItem>
                    <asp:ListItem Value="NORMAL">normal</asp:ListItem>
                    <asp:ListItem Value="VIP">vip</asp:ListItem>
                    <asp:ListItem Value="DISCAPACITADO">discapacitado</asp:ListItem>       
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvTipoAsiento" runat="server" ControlToValidate="ddlTipoAsiento" InitialValue="" ErrorMessage="Debe seleccionar un tipo de asiento." Display="Dynamic" ForeColor="Red" ValidationGroup="AsientoValidation"></asp:RequiredFieldValidator>
           </div>                      

            <div class="form-group form-check">
                <asp:CheckBox ID="chkEstaActiva" runat="server" Text="¿Está Activa?" />
            </div>
        </div>

        <div class="form-actions">
            <asp:HiddenField ID="hdnAsientoId" runat="server" Value="0" />
            <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelModal_Click" CausesValidation="false" />
            <asp:Button ID="btnGuardarAsiento" runat="server" Text="Guardar Asiento" CssClass="btn btn-primary" OnClick="btnGuardarAsiento_Click" ValidationGroup="AsientoValidation" />
        </div>
        <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
    </div>
</div>
<%-- NUEVO Modal para Carga de CSV --%>
<div class="modal" id="csvUploadModal" runat="server" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title">Cargar Asientos desde CSV</h2>
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
                <li>`Fila`</li>
                <li>`Numero`</li>
                <li>`TipoAsiento`</li>
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