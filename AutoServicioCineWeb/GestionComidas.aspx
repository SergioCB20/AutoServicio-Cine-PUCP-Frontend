<%@ Page Title="Gestión de Comidas" Language="C#" 
MasterPageFile="~/Admin.Master" AutoEventWireup="true" 
CodeBehind="GestionComidas.aspx.cs" 
Inherits="AutoServicioCineWeb.GestionComidas" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
      <%-- Modal para Agregar/Editar Película --%>
     <link rel="stylesheet" href="./styles/GestionCupones.css">
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    🍿 Gestión de Comidas
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra el menú del autoservicio de tu cine
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server">
    <button class="btn btn-primary" onclick="openModal()">
        ➕ Agregar Nueva Comida
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
                <asp:ListItem Value="Snacks">Snacks</asp:ListItem>
                <asp:ListItem Value="Bebidas">Bebidas</asp:ListItem>
                <asp:ListItem Value="Dulces">Dulces</asp:ListItem>
                <asp:ListItem Value="Combos">Combos</asp:ListItem>
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
</div>
    <div class="modal" id="foodModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title" id="modalTitle">
                    <asp:Literal ID="litModalTitle" runat="server" Text=""></asp:Literal>Agregar nueva Comida
                    <label for="<%=txtNombre.ClientID %>">Nombre:</label>
                    <asp:TextBox ID="txtNombre" runat="server"></asp:TextBox><br />

                    <label for="<%=txtTipo.ClientID %>">Tipo:</label>
                    <asp:TextBox ID="txtTipo" runat="server"></asp:TextBox><br />

                    <label for="<%=txtPrecio.ClientID %>">Precio:</label>
                    <asp:TextBox ID="txtPrecio" runat="server"></asp:TextBox><br />

                    <label for="<%=txtDescripcion.ClientID %>">Descripción:</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox><br />

                    <asp:CheckBox ID="chkActivo" runat="server" Text="Activo"></asp:CheckBox><br />

                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
                      
                </h2>
            </div>
            
            <div id="foodForm">
                <div class="form-group">
                    <label class="form-label">Nombre del producto</label>
                    <input type="text" class="form-input" id="productName" placeholder="Ej: Palomitas Grandes" required>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Categoría</label>
                    <select class="form-input" id="productCategory" required>
                        <option value="">Seleccionar categoría</option>
                        <%-- Los valores aquí deben coincidir con los del enum TipoProducto --%>
                        <option value="Snacks">Snacks</option>
                        <option value="Bebidas">Bebidas</option>
                        <option value="Dulces">Dulces</option>
                        <option value="Combos">Combos</option>
                    </select>
                </div>
                <div class="form-group">
                    <label>Previsualización:</label>
                    <asp:Image ID="imgPreview" runat="server" style="max-width: 150px; max-height: 150px; display: none; border: 1px solid #ddd; padding: 5px;" />
                    <asp:HiddenField ID="hdnExistingImageUrl" runat="server" /> 
                </div>
                <div class="form-group">
                    <label class="form-label">Precio (S/)</label>
                    <input type="number" class="form-input" id="productPrice" step="0.10" min="0" placeholder="0.00" required>
                </div>
                <asp:Literal ID="litMensajeModal" runat="server" ></asp:Literal>
                <%-- Eliminado el campo Stock, ya que no está en la clase Producto --%>
                
                <div class="form-group">
                    <label class="form-label">URL de imagen</label>
                    <input type="url" class="form-input" id="productImage" placeholder="https://ejemplo.com/imagen.jpg">
                </div>

                <div class="form-group">
                    <label class="form-label">Estado</label>
                    <input type="checkbox" id="productActive"> Activo
                </div>
                
                <div class="form-buttons">
                    <button type="button" class="btn-secondary" onclick="closeModal()">Cancelar</button>
                    <button type="button" class="btn-primary" onclick="submitForm()">Guardar Producto</button>
                </div>
            </div>
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
    </script>
    <asp:HiddenField ID="hdnComidaId" runat="server" />
</asp:Content>