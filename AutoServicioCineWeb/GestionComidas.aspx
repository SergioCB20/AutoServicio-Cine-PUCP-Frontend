<%@ Page Title="Gestión de Comidas" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="GestionComidas.aspx.cs" Inherits="AutoServicioCineWeb.GestionComidas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="./styles/GestionComida.css">
    <script type="text/javascript">
        // Función para abrir el modal (llamada desde C# o btnOpenAddModal_Click)
        // Función para previsualizar la imagen (AHORA USA txtImagenUrl)
        function previewImage(input) {
            const imageUrl = input.value;
            const imgPreviewElement = document.getElementById('<%= imgPreview.ClientID %>');

            if (imageUrl && isValidUrl(imageUrl)) {
                imgPreviewElement.src = imageUrl;
                imgPreviewElement.style.display = 'block';
            } else {
                imgPreviewElement.src = '';
                imgPreviewElement.style.display = 'none';
            }
        }

        // Función auxiliar para validar URL (puedes mejorar la regex si necesitas más robustez)
        function isValidUrl(string) {
            try {
                new URL(string);
                return true;
            } catch (_) {
                return false;
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

        // Función para limpiar los mensajes de los validadores de ASP.NET
        function clearModalValidators() {
            console.log('Limpiando validadores...');
            if (typeof (Page_Validators) !== 'undefined') {
                for (var i = 0; i < Page_Validators.length; i++) {
                    // Solo afecta los validadores dentro del grupo 'ComidaValidation'

                    //ValidatorHookupControlID(Page_Validators[i].controltovalidate, Page_Validators[i]);
                    //ValidatorUpdateDisplay(Page_Validators[i]);

                    if (Page_Validators[i].validationGroup === "ComidaValidation") {
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
    🍿 Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra los productos de tu cafetería
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <asp:Button ID="btnOpenAddModal" runat="server" Text="➕ Agregar Nueva Comida" CssClass="btn btn-primary" OnClick="btnOpenAddModal_Click" />
    <asp:Button ID="btnOpenCsvImportModal" runat="server" Text="📤 Ingresar datos con CSV" CssClass="btn btn-secondary" BackColor="ForestGreen" OnClick="btnOpenCsvImportModal_Click" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalProducts"><%= GetTotalComidas() %></div>
            <div class="stat-label">Total Comidas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="availableProducts"><%= GetComidasActivas() %></div>
            <div class="stat-label">Comidas Activas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="soldOutProducts"><%= GetComidasInactivas() %></div>
            <div class="stat-label">Comidas Inactivas</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="totalCategories"><%= GetTiposProductoUnicos() %></div>
            <div class="stat-label">Tipos Únicos</div>
        </div>
    </div>

    <div class="table-container">
        <div class="table-header">
            <div class="search-bar">
                <asp:TextBox ID="txtSearchProductos" runat="server" CssClass="search-input" placeholder="🔍 Buscar comidas..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">Todas las categorías</asp:ListItem>
                    <asp:ListItem Value="BEBIDA">Bebida</asp:ListItem>
                    <asp:ListItem Value="COMIDA">Comida</asp:ListItem>
                    <asp:ListItem Value="COMBO">Combo</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <asp:Literal ID="litMensajeTabla" runat="server"></asp:Literal>
        <asp:GridView ID="gvComidas" runat="server" AutoGenerateColumns="False"
                      CssClass="data-table"
                      HeaderStyle-CssClass="data-table-header"
                      RowStyle-CssClass="data-table-row"
                      AlternatingRowStyle-CssClass="data-table-row-alt"
                      AllowPaging="True" PageSize="10" OnPageIndexChanging="gvComidas_PageIndexChanging"
                      DataKeyNames="id" OnRowCommand="gvComidas_RowCommand">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                <asp:BoundField DataField="nombre_es" HeaderText="Nombre (ES)" SortExpression="nombre_es" />
                 <asp:BoundField DataField="nombre_en" HeaderText="Nombre (EN)" SortExpression="nombre_en" />
                <asp:TemplateField HeaderText="Tipo">
                    <ItemTemplate>
                        <span class="category-badge category-<%# Eval("tipo").ToString().ToLower() %>">
                            <%# Eval("tipo") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" SortExpression="precio" />
                <asp:TemplateField HeaderText="Imagen">
                    <ItemTemplate>
                        <img src='<%# Eval("imagenUrl") %>' alt='<%# Eval("nombre_es") %>' class="food-image" onerror="this.onerror=null;this.src='/path/to/default_food_image.png';" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <span class="status-badge status-<%# Eval("estaActivo").ToString().ToLower() == "true" ? "activa" : "inactiva" %>">
                            <%# Eval("estaActivo").ToString().ToLower() == "true" ? "Activo" : "Inactivo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="fechaModificacion" HeaderText="Última Mod." DataFormatString="{0:yyyy-MM-dd HH:mm}" SortExpression="fechaModificacion" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-edit" CommandName="EditComida" CommandArgument='<%# Eval("id") %>'>
                                <i class="fas fa-edit"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn-delete" CommandName="DeleteComida" CommandArgument='<%# Eval("id") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar este producto?');">
                                <i class="fas fa-trash-alt"></i> Eliminar
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No se encontraron productos de comida.
            </EmptyDataTemplate>
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>
    </div>

    <%-- Modal para Agregar/Editar Comida --%>
    <div class="modal" id="productoModal" runat="server" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Comida
                </h2>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="close-button" OnClick="btnCloseModal_Click" CausesValidation="false">&times;</asp:LinkButton>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label for="<%= txtNombre.ClientID %>" class="form-label">Nombre (Español):</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre" ErrorMessage="El nombre en español es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtNombreEn.ClientID %>" class="form-label">Nombre (Inglés):</label>
                    <asp:TextBox ID="txtNombreEn" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombreEn" runat="server" ControlToValidate="txtNombreEn" ErrorMessage="El nombre en inglés es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtDescripcionEs.ClientID %>" class="form-label">Descripción (Español):</label>
                    <asp:TextBox ID="txtDescripcionEs" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescripcionEs" runat="server" ControlToValidate="txtDescripcionEs" ErrorMessage="La descripción en español es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtDescripcionEn.ClientID %>" class="form-label">Descripción (Inglés):</label>
                    <asp:TextBox ID="txtDescripcionEn" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescripcionEn" runat="server" ControlToValidate="txtDescripcionEn" ErrorMessage="La descripción en inglés es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= ddlTipo.ClientID %>" class="form-label">Tipo:</label>
                    <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">Seleccionar</asp:ListItem>
                        <asp:ListItem Value="BEBIDA">Bebida</asp:ListItem>
                        <asp:ListItem Value="COMIDA">Comida</asp:ListItem>
                        <asp:ListItem Value="COMBO">Combo</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvTipo" runat="server" ControlToValidate="ddlTipo" InitialValue="" ErrorMessage="El tipo es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtPrecio.ClientID %>" class="form-label">Precio:</label>
                    <asp:TextBox ID="txtPrecio" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" ControlToValidate="txtPrecio" ErrorMessage="El precio es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rvPrecio" runat="server" ControlToValidate="txtPrecio" MinimumValue="0.01" MaximumValue="1000" Type="Double" ErrorMessage="El precio debe ser un número positivo." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RangeValidator>
                </div>
                <div class="form-group full-width"> <%-- Se ajusta el layout a full-width si es necesario --%>
                    <label for="<%= txtImagenUrl.ClientID %>" class="form-label">URL de Imagen:</label>
                    <asp:TextBox ID="txtImagenUrl" runat="server" CssClass="form-control" onkeyup="previewImage(this);" onchange="previewImage(this);"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvImagenUrl" runat="server" ControlToValidate="txtImagenUrl" ErrorMessage="La URL de imagen es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revImagenUrl" runat="server" ControlToValidate="txtImagenUrl"
                        ValidationExpression="^(http(s?):\/\/)[\w\-\._~:\/\?#\[\]@!\$&'\(\)\*\+,;=\.\/]+(?:jpg|gif|png|jpeg|bmp|svg|webp)$"
                        ErrorMessage="Formato de URL de imagen inválido." Display="Dynamic" ForeColor="Red" ValidationGroup="ComidaValidation"></asp:RegularExpressionValidator>
                </div>
                <div class="form-group full-width"> <%-- Se ajusta el layout a full-width si es necesario --%>
                    <label class="form-label">Previsualización:</label>
                    <asp:Image ID="imgPreview" runat="server" CssClass="image-preview" />
                    <asp:HiddenField ID="hdnExistingImageUrl" runat="server" />
                </div>
                <div class="form-group form-check">
                    <asp:CheckBox ID="chkActivo" runat="server" Text="¿Está Activo?" />
                </div>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnComidaId" runat="server" Value="0" /> <%-- 0 para nuevo, >0 para editar --%>
                <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCloseModal_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Comida" CssClass="btn btn-primary" OnClick="btnGuardar_Click" ValidationGroup="ComidaValidation" />
            </div>
            <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
        </div>
    </div>

    <%-- NUEVO Modal para Carga de CSV --%>
<div class="modal" id="csvUploadModal" runat="server" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title">Cargar Productos desde CSV</h2>
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
                <li>`nombre_es`</li>
                <li>`nombre_en`</li>
                <li>`descripcion_es`</li>
                <li>`descripcion_en`</li>
                <li>`precio`</li>
                <li>`tipo`</li>
                <li>`esta_Activo` (TRUE/FALSE o 1/0)</li>
                <li>`ImagenUrl`</li>
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
    <%-- No se necesita JS adicional aquí, ya está en ContentHead --%>
</asp:Content>