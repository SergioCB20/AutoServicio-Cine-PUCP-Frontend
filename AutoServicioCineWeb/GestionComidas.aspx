<%@ Page Title="Gestión de Comidas" Language="C#" 
MasterPageFile="~/Admin.Master" AutoEventWireup="true" 
CodeBehind="GestionComidas.aspx.cs" 
Inherits="AutoServicioCineWeb.GestionComidas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
      <%-- Modal para Agregar/Editar Película --%>
     <link rel="stylesheet" href="./styles/GestionComida.css">
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    🍿 Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra el menú del autoservicio de tu cine
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <button type="button" class="btn btn-primary" onclick="openModal(); return false;">
        ➕ Agregar Nuevo Producto
    </button>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalProducts">0</div>
            <div class="stat-label">Total Productos</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="availableProducts">0</div>
            <div class="stat-label">Disponibles</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="soldOutProducts">0</div>
            <div class="stat-label">Agotados</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="totalCategories">0</div>
            <div class="stat-label">Categorías</div>
        </div>
    </div>

    <div class="table-container">
    <div class="table-header">
        <div class="search-bar">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="🔍 Buscar comidas..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
            
            <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged">
                <asp:ListItem Value="">Todas las categorías</asp:ListItem>
                <%-- Los valores aquí deben coincidir con los del enum TipoProducto --%>
                <asp:ListItem Value="BEBIDA">Bebidas</asp:ListItem>
                <asp:ListItem Value="SNACK">Snack</asp:ListItem>
                <asp:ListItem Value="COMBO">Combo</asp:ListItem>
            </asp:DropDownList>
            
            <%-- <asp:Button ID="btnApplyFilters" runat="server" Text="Aplicar Filtros" OnClick="btnApplyFilters_Click" /> --%>
        </div>
    </div>
    
    <asp:GridView ID="gvComidas" runat="server" AutoGenerateColumns="False" 
                  CssClass="food-table" 
                  HeaderStyle-CssClass="table-header" 
                  RowStyle-CssClass="table-row" 
                  AlternatingRowStyle-CssClass="table-row-alt" 
                  AllowPaging="True" PageSize="10" OnPageIndexChanging="gvComidas_PageIndexChanging"
                  DataKeyNames="ID"> <%-- Asume que tu objeto de datos tiene una propiedad 'ID' --%>
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
            
            <asp:TemplateField HeaderText="Imagen">
                <ItemTemplate>
                    <%-- Asume que tu objeto de datos tiene una propiedad 'URLImagen' --%>
                    <img src='<%# Eval("URLImagen") %>' alt='<%# Eval("Nombre") %>' style="width: 50px; height: 50px; object-fit: cover;" />
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:BoundField DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" />
            <asp:BoundField DataField="Categoria" HeaderText="Categoría" SortExpression="Categoria" />
            <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="{0:C}" SortExpression="Precio" /> <%-- Formato de moneda --%>
            
            <asp:TemplateField HeaderText="Estado">
                <ItemTemplate>
                    <%-- Asume que tu objeto de datos tiene una propiedad 'Activo' (bool) --%>
                    <%# Eval("Activo").ToString().ToLower() == "true" ? "Activo" : "Inactivo" %>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="EditComida" CommandArgument='<%# Eval("ID") %>'>Editar</asp:LinkButton>
                    &nbsp;|&nbsp;
                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="DeleteComida" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('¿Estás seguro de que quieres eliminar esta comida?');">Eliminar</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
        <div class="table-messages">
    <asp:Literal ID="litMensajeTabla" runat="server"></asp:Literal>
</div>
</div>
    <div class="modal" id="foodModal">
    <div class="modal-content">
        <div class="modal-header">
            <h2 class="modal-title">
                <asp:Literal ID="litModalTitle" runat="server" Text="Agregar Nueva Comida"></asp:Literal>
            </h2>
        </div>
        
        <div id="foodForm">
            <div class="form-group">
                <label class="form-label">Nombre del producto</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-input" placeholder="Ej: Palomitas Grandes"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNombre" runat="server" 
                    ControlToValidate="txtNombre" 
                    ValidationGroup="ComidaValidation"
                    ErrorMessage="El nombre es requerido" 
                    CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label class="form-label">Categoría</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-input">
                    <asp:ListItem Value="">Seleccionar categoría</asp:ListItem>
                    <asp:ListItem Value="SNACK">SNACK</asp:ListItem>
                    <asp:ListItem Value="BEBIDA">BEBIDA</asp:ListItem>
                    <asp:ListItem Value="COMBO">COMBO</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvTipo" runat="server" 
                    ControlToValidate="ddlTipo" 
                    ValidationGroup="ComidaValidation"
                    ErrorMessage="La categoría es requerida" 
                    CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label class="form-label">Precio (S/)</label>
                <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-input" placeholder="0.00"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" 
                    ControlToValidate="txtPrecio" 
                    ValidationGroup="ComidaValidation"
                    ErrorMessage="El precio es requerido" 
                    CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rvPrecio" runat="server" 
                    ControlToValidate="txtPrecio" 
                    ValidationGroup="ComidaValidation"
                    Type="Double" MinimumValue="0.01" MaximumValue="999.99"
                    ErrorMessage="El precio debe ser mayor a 0" 
                    CssClass="error-message" Display="Dynamic"></asp:RangeValidator>
            </div>

            <div class="form-group">
                <label class="form-label">Descripción (URL de imagen)</label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-input" placeholder="https://ejemplo.com/imagen.jpg"></asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Previsualización:</label>
                <asp:Image ID="imgPreview" runat="server" 
                    style="max-width: 150px; max-height: 150px; display: none; border: 1px solid #ddd; padding: 5px; border-radius: 4px;" />
            </div>

            <div class="form-group">
                <asp:CheckBox ID="chkActivo" runat="server" Text=" Activo" Checked="true" CssClass="form-checkbox" />
            </div>

            <div class="form-group">
                <asp:Literal ID="litMensajeModal" runat="server"></asp:Literal>
            </div>
            
            <div class="form-buttons">
                <button type="button" class="btn-secondary" onclick="closeModal()">Cancelar</button>
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn-primary" 
                    OnClick="btnGuardar_Click" ValidationGroup="ComidaValidation" />
            </div>
        </div>
    </div>
</div>

<asp:HiddenField ID="HiddenField1" runat="server" />
     <div id="loadingIndicator" style="display: none;">
        <div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 9999; display: flex; align-items: center; justify-content: center;">
            <div style="background: white; padding: 20px; border-radius: 5px;">
                Cargando...
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ContentScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        // foodData ahora será poblada por initialFoodData del CodeBehind
        let foodData = initialFoodData || []; // Asegúrate de que exista o sea un array vacío
        let editingId = null;

        // Función para renderizar la tabla con los datos actuales de foodData
        function renderFoodTable(dataToRender = foodData) {
            const tableBody = document.getElementById('foodTable').getElementsByTagName('tbody')[0];
            tableBody.innerHTML = ''; // Limpiar la tabla antes de renderizar

            dataToRender.forEach(product => {
                const row = tableBody.insertRow();
                row.setAttribute('data-id', product.Id); // Guarda el ID en el atributo data-id

                row.insertCell().textContent = product.Id;
                const imgCell = row.insertCell();
                const img = document.createElement('img');
                img.src = product.Imagen || 'https://via.placeholder.com/50'; // Usar una imagen por defecto si no hay
                img.alt = product.Nombre;
                img.style.width = '50px';
                img.style.height = '50px';
                img.style.objectFit = 'cover';
                imgCell.appendChild(img);
                row.insertCell().textContent = product.Nombre;
                row.insertCell().textContent = product.Tipo; // Corresponde al campo 'tipo' del Producto
                row.insertCell().textContent = `S/ ${product.Precio.toFixed(2)}`;
                row.insertCell().textContent = product.EstaActivo ? 'Activo' : 'Inactivo';
                // row.insertCell().textContent = 'N/A'; // No hay campo de última modificación en Producto, lo dejamos como N/A o lo eliminamos.

                const actionsCell = row.insertCell();
                actionsCell.innerHTML = `
                    <button class="btn-edit" onclick="editItem(${product.Id})">✏️ Editar</button>
                    <button class="btn-delete" onclick="deleteItem(${product.Id})">🗑️ Eliminar</button>
                `;
            });
            updateStats();
        }

        // Llamar a renderFoodTable al cargar la página para mostrar los datos iniciales
        document.addEventListener('DOMContentLoaded', renderFoodTable);

        function updateStats() {
            document.getElementById('totalProducts').textContent = foodData.length;
            const available = foodData.filter(p => p.EstaActivo).length;
            document.getElementById('availableProducts').textContent = available;
            document.getElementById('soldOutProducts').textContent = foodData.length - available;

            const categories = new Set(foodData.map(p => p.Tipo));
            document.getElementById('totalCategories').textContent = categories.size;
        }


        function openModal() {
            document.getElementById('foodModal').style.display = 'flex';
            document.getElementById('modalTitle').textContent = 'Agregar Nueva Comida';
            clearForm();
            editingId = null;
        }

        function closeModal() {
            document.getElementById('foodModal').style.display = 'none';
        }

        function clearForm() {
            document.getElementById('productName').value = '';
            document.getElementById('productCategory').value = '';
            document.getElementById('productPrice').value = '';
            document.getElementById('productImage').value = '';
            document.getElementById('productActive').checked = true; // Por defecto activo
        }

        function editItem(id) {
            openModal();
            document.getElementById('modalTitle').textContent = 'Editar Comida';
            editingId = id;

            // Encontrar el producto por ID en foodData
            const productToEdit = foodData.find(p => p.Id === id);

            if (productToEdit) {
                document.getElementById('productName').value = productToEdit.Nombre;
                // El campo 'tipo' en C# se mapea a 'Tipo' en JSON debido a la serialización por defecto
                document.getElementById('productCategory').value = productToEdit.Tipo;
                document.getElementById('productPrice').value = productToEdit.Precio;
                document.getElementById('productImage').value = productToEdit.Imagen || ''; // Si no hay imagen, cadena vacía
                document.getElementById('productActive').checked = productToEdit.EstaActivo;
            }
        }

        function deleteItem(id) {
            if (confirm('¿Estás seguro de que deseas eliminar este producto?')) {
                // Eliminar el producto de foodData
                foodData = foodData.filter(product => product.Id !== id);
                renderFoodTable(); // Volver a renderizar la tabla
                showNotification('Producto eliminado correctamente', 'success');
                // Aquí iría la lógica para eliminar del servidor
            }
        }

        function filterTable() {
            const input = document.querySelector('.search-input');
            const filter = input.value.toLowerCase();

            const filteredData = foodData.filter(product => {
                // Buscamos en nombre y categoría para la búsqueda general
                return product.Nombre.toLowerCase().includes(filter) ||
                    product.Tipo.toLowerCase().includes(filter);
            });
            renderFoodTable(filteredData);
        }

        function filterByCategory() {
            const select = document.querySelector('.filter-select');
            const filter = select.value.toLowerCase(); // 'snacks', 'bebidas', etc.

            if (filter === '') {
                renderFoodTable(foodData); // Mostrar todos si el filtro está vacío
            } else {
                const filteredData = foodData.filter(product =>
                    product.Tipo.toLowerCase() === filter
                );
                renderFoodTable(filteredData);
            }
        }

        // Función de utilidad para validación simple (puedes expandirla)
        function validateForm(formId) {
            const form = document.getElementById(formId);
            const requiredInputs = form.querySelectorAll('[required]');
            let isValid = true;
            requiredInputs.forEach(input => {
                if (!input.value) {
                    input.classList.add('is-invalid'); // Puedes agregar una clase CSS para indicar error
                    isValid = false;
                } else {
                    input.classList.remove('is-invalid');
                }
            });
            return isValid;
        }

        function submitForm() {
            // No es necesario 'foodForm' porque ya validamos los campos individuales
            if (!validateForm('foodForm')) {
                showNotification('Por favor, completa todos los campos requeridos.', 'error');
                return;
            }

            const productName = document.getElementById('productName').value;
            const productCategory = document.getElementById('productCategory').value;
            const productPrice = parseFloat(document.getElementById('productPrice').value);
            const productImage = document.getElementById('productImage').value;
            const productActive = document.getElementById('productActive').checked;


            if (editingId) {
                // Actualizar producto existente
                const productIndex = foodData.findIndex(p => p.Id === editingId);
                if (productIndex !== -1) {
                    foodData[productIndex].Nombre = productName;
                    foodData[productIndex].Tipo = productCategory;
                    foodData[productIndex].Precio = productPrice;
                    foodData[productIndex].Imagen = productImage; // Asumiendo que hay una propiedad Imagen
                    foodData[productIndex].EstaActivo = productActive;
                }
                showNotification('Producto actualizado correctamente', 'success');
            } else {
                // Agregar nuevo producto
                const newId = foodData.length > 0 ? Math.max(...foodData.map(p => p.Id)) + 1 : 1;
                const newProduct = {
                    Id: newId,
                    Nombre: productName,
                    Descripcion: '', // La clase Producto tiene descripción, pero no la capturamos en el formulario actual
                    Precio: productPrice,
                    Tipo: productCategory,
                    EstaActivo: productActive,
                    Imagen: productImage // Asumiendo que hay una propiedad Imagen
                };
                foodData.push(newProduct);
                showNotification('Nuevo producto agregado correctamente', 'success');
            }

            renderFoodTable(); // Volver a renderizar la tabla con los datos actualizados
            closeModal();
            // Aquí iría la lógica para enviar los datos al servidor (mediante AJAX o un PostBack completo si lo necesitas)
            // Por ejemplo, podrías usar PageMethods de ASP.NET AJAX si los configuras.
            // O una llamada a un Web API.
        }

        // Función de notificación (asumida que ya tienes una implementación básica)
        function showNotification(message, type) {
            // Implementa tu lógica de notificación aquí.
            // Por ejemplo: alert(message);
            console.log(`Notificación (${type}): ${message}`);
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            const modal = document.getElementById('foodModal');
            if (event.target === modal) {
                closeModal();
            }
        }
        function previewImage() {
            const urlInput = document.getElementById('<%= txtDescripcion.ClientID %>');
            const preview = document.getElementById('<%= imgPreview.ClientID %>');

            if (urlInput.value && isValidUrl(urlInput.value)) {
                preview.src = urlInput.value;
                preview.style.display = 'block';
            } else {
                preview.style.display = 'none';
            }
        }

        function isValidUrl(string) {
            try {
                new URL(string);
                return true;
            } catch (_) {
                return false;
            }
        }

        // Agregar evento al campo de descripción para previsualización automática
        document.addEventListener('DOMContentLoaded', function () {
            const descInput = document.getElementById('<%= txtDescripcion.ClientID %>');
    if (descInput) {
        descInput.addEventListener('blur', previewImage);
        descInput.addEventListener('input', function () {
            // Debounce para evitar muchas llamadas
            clearTimeout(this.timeout);
            this.timeout = setTimeout(previewImage, 500);
        });
    }
});
    </script>
    <asp:HiddenField ID="hdnComidaId" runat="server" />
</asp:Content>