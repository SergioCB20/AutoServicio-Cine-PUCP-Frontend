<%@ Page Title="Gestión de Películas" Language="C#" MasterPageFile="~/Admin.master" AutoEventWireup="true" CodeBehind="GestionPeliculas.aspx.cs" Inherits="AutoServicioCineWeb.GestionPeliculas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Películas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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

        // Function to preview selected image from URL
        function previewImage(input) {
            const imageUrl = input.value;
            const imgPreviewElement = document.getElementById('<%= imgPreview.ClientID %>');

            if (imageUrl) {
                imgPreviewElement.src = imageUrl;
                imgPreviewElement.style.display = 'block'; // Show the image preview
            } else {
                imgPreviewElement.src = '';
                imgPreviewElement.style.display = 'none'; // Hide if no URL
            }
        }
    </script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h1>Gestión de Películas</h1>
    </div>
    <div class="actions">
        <asp:Button ID="btnAgregarPelicula" runat="server" Text="Agregar Nueva Película" CssClass="button primary" OnClick="btnAgregarPelicula_Click" />
    </div>
    <div class="table-container">
        <asp:GridView ID="gvPeliculas" runat="server" AutoGenerateColumns="False" CssClass="data-table" HeaderStyle-CssClass="data-table-header" RowStyle-CssClass="data-table-row" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvPeliculas_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="peliculaId" HeaderText="ID" SortExpression="peliculaId" />
                <asp:BoundField DataField="tituloEs" HeaderText="Título (ES)" SortExpression="tituloEs" />
                <asp:BoundField DataField="tituloEn" HeaderText="Título (EN)" SortExpression="tituloEn" />
                <asp:BoundField DataField="duracionMin" HeaderText="Duración (min)" SortExpression="duracionMin" />
                <asp:BoundField DataField="clasificacion" HeaderText="Clasificación" SortExpression="clasificacion" />
                <asp:TemplateField HeaderText="Sinopsis (ES)">
                    <ItemTemplate>
                        <%# Eval("sinopsisEs").ToString().Length > 50 ? Eval("sinopsisEs").ToString().Substring(0, 50) + "..." : Eval("sinopsisEs") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Imagen">
                    <ItemTemplate>
                        <img src="<%# Eval("imagenUrl") %>" alt="Imagen de película" style="width: 50px; height: auto;" onerror="this.onerror=null;this.src='/path/to/default_image.png';" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="estaActiva" HeaderText="Activa" SortExpression="estaActiva" />
                <asp:BoundField DataField="fechaModificacion" HeaderText="Última Mod." DataFormatString="{0:yyyy-MM-dd HH:mm}" SortExpression="fechaModificacion" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="button edit" CommandName="EditarPelicula" CommandArgument='<%# Eval("peliculaId") %>' OnClick="btnEditar_Click" />
                        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="button delete" CommandName="EliminarPelicula" CommandArgument='<%# Eval("peliculaId") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta película?');" OnClick="btnEliminar_Click" />
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
    <div id="peliculaModal" class="modal" style="display: none;">
        <div class="modal-content">
            <span class="close-button" onclick="document.getElementById('peliculaModal').style.display='none';">&times;</span>
            <h2><asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal> Película</h2>
            <div class="form-grid">
                <div class="form-group">
                    <label for="<%= txtTituloEs.ClientID %>">Título (Español):</label>
                    <asp:TextBox ID="txtTituloEs" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTituloEs" runat="server" ControlToValidate="txtTituloEs" ErrorMessage="El título en español es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtTituloEn.ClientID %>">Título (Inglés):</label>
                    <asp:TextBox ID="txtTituloEn" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTituloEn" runat="server" ControlToValidate="txtTituloEn" ErrorMessage="El título en inglés es requerido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtDuracionMin.ClientID %>">Duración (minutos):</label>
                    <asp:TextBox ID="txtDuracionMin" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDuracionMin" runat="server" ControlToValidate="txtDuracionMin" ErrorMessage="La duración es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rvDuracionMin" runat="server" ControlToValidate="txtDuracionMin" MinimumValue="1" MaximumValue="500" Type="Integer" ErrorMessage="La duración debe ser un número positivo." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RangeValidator>
                </div>
                <div class="form-group">
                    <label for="<%= txtClasificacion.ClientID %>">Clasificación:</label>
                    <asp:TextBox ID="txtClasificacion" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvClasificacion" runat="server" ControlToValidate="txtClasificacion" ErrorMessage="La clasificación es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtSinopsisEs.ClientID %>">Sinopsis (Español):</label>
                    <asp:TextBox ID="txtSinopsisEs" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSinopsisEs" runat="server" ControlToValidate="txtSinopsisEs" ErrorMessage="La sinopsis en español es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group full-width">
                    <label for="<%= txtSinopsisEn.ClientID %>">Sinopsis (Inglés):</label>
                    <asp:TextBox ID="txtSinopsisEn" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSinopsisEn" runat="server" ControlToValidate="txtSinopsisEn" ErrorMessage="La sinopsis en inglés es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                </div>
                
                <%-- NEW: Image URL input instead of FileUpload --%>
                <div class="form-group">
                    <label for="<%= txtImagenUrl.ClientID %>">URL de Imagen:</label>
                    <asp:TextBox ID="txtImagenUrl" runat="server" CssClass="form-control" onkeyup="previewImage(this);" onchange="previewImage(this);"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvImagenUrl" runat="server" ControlToValidate="txtImagenUrl" ErrorMessage="La URL de la imagen es requerida." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RequiredFieldValidator>
                    <%-- Basic URL regex for image formats. Can be more robust if needed. --%>
                    <asp:RegularExpressionValidator ID="revImagenUrl" runat="server" ControlToValidate="txtImagenUrl"
    ValidationExpression="^(http(s?):\/\/)[\w\-\._~:\/\?#\[\]@!\$&'\(\)\*\+,;=\.\/]+(?:jpg|gif|png|jpeg|bmp|svg|webp)$"
    ErrorMessage="Formato de URL de imagen inválido." Display="Dynamic" ForeColor="Red" ValidationGroup="PeliculaValidation"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group">
                    <label>Previsualización:</label>
                    <asp:Image ID="imgPreview" runat="server" style="max-width: 150px; max-height: 150px; display: none; border: 1px solid #ddd; padding: 5px;" />
                    <asp:HiddenField ID="hdnExistingImageUrl" runat="server" /> 
                </div>

                <div class="form-group form-check">
                    <asp:CheckBox ID="chkEstaActiva" runat="server" Text="¿Está Activa?" />
                </div>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnPeliculaId" runat="server" Value="0" />
                <asp:Button ID="btnGuardarPelicula" runat="server" Text="Guardar" CssClass="button primary" OnClick="btnGuardarPelicula_Click" ValidationGroup="PeliculaValidation" />
                <button type="button" class="button secondary" onclick="document.getElementById('peliculaModal').style.display='none';">Cancelar</button>
            </div>
            <asp:Literal ID="litMensajeModal" runat="server" ></asp:Literal>
        </div>
    </div>

    <script type="text/javascript">
        function showPeliculaModal() {
            document.getElementById('peliculaModal').style.display = 'block';
            if (document.getElementById('<%= hdnPeliculaId.ClientID %>').value === '0') {
                // If it's a new movie, clear and hide preview
                document.getElementById('<%= imgPreview.ClientID %>').src = '';
                document.getElementById('<%= imgPreview.ClientID %>').style.display = 'none';
                
                // Clear the image URL textbox
                const txtImageUrlElement = document.getElementById('<%= txtImagenUrl.ClientID %>');
                if (txtImageUrlElement) {
                    txtImageUrlElement.value = '';
                }

                // Clear any client-side validation messages immediately when opening for a new record
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
        }
    </script>
</asp:Content>