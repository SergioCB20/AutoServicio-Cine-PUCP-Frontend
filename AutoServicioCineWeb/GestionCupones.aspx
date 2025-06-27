<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true"
    CodeBehind="GestionCupones.aspx.cs"
    Inherits="AutoServicioCineWeb.GestionCupones" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Cupones
</asp:Content>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- Modal para Agregar/Editar Película --%>
    <link rel="stylesheet" href="./styles/GestionCupones.css">
</asp:Content>

<asp:Content ID="ContentPageTitle" ContentPlaceHolderID="PageTitleContent" runat="server">
    🎫 Gestión de Cupones
</asp:Content>

<asp:Content ID="ContentPageSubtitle" ContentPlaceHolderID="PageSubtitle" runat="server">
    Administra los cupones de descuento del sistema
</asp:Content>

<asp:Content ID="ContentHeaderActions" ContentPlaceHolderID="HeaderActions" runat="server"> 
    <asp:Button ID="btnOpenModal" runat="server" Text="➕ Agregar Nuevo Cupón"
        CssClass="btn btn-primary" BackColor="ForestGreen" OnClick="btnNuevoCupon_Click" />
    <%-- NUEVO Botón para importar CSV --%>
    <asp:Button ID="btnOpenCsvImportModal" runat="server" Text="📤 Ingresar datos con CSV"
        CssClass="btn btn-secondary" BackColor="ForestGreen" OnClick="btnOpenCsvImportModal_Click" />
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .coupon-container {
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }

        .form-section {
            background: #f8f9fa;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 20px;
            border-left: 4px solid #3498db;
        }

        .form-row {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
            flex-wrap: wrap;
        }

        .form-group {
            flex: 1;
            min-width: 250px;
        }

            .form-group label {
                display: block;
                margin-bottom: 5px;
                font-weight: 600;
                color: #2c3e50;
            }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #e9ecef;
            border-radius: 8px;
            font-size: 14px;
            transition: border-color 0.3s ease;
        }

            .form-control:focus {
                border-color: #3498db;
                outline: none;
                box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
            }

        .table-container {
            background: white;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .coupon-table {
            width: 100%;
            border-collapse: collapse;
        }

            .coupon-table th {
                background: #f8f9fa;
                color: darkblue;
                padding: 15px;
                text-align: left;
                font-weight: 600;
                font-size: 14px;
            }

            .coupon-table td {
                padding: 15px;
                border-bottom: 1px solid #f1f3f4;
                vertical-align: middle;
            }

            .coupon-table tbody tr:hover {
                background-color: rgba(52, 152, 219, 0.05);
            }

        .status-badge {
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }

        .status-active {
            background: rgba(46, 204, 113, 0.1);
            color: #27ae60;
        }

        .status-inactive {
            background: rgba(231, 76, 60, 0.1);
            color: #e74c3c;
        }

        .status-expired {
            background: rgba(243, 156, 18, 0.1);
            color: #f39c12;
        }

        .btn-small {
            padding: 6px 12px;
            font-size: 12px;
            margin: 0 2px;
        }

        .btn-edit {
            background: linear-gradient(135deg, #f39c12, #e67e22);
            color: white;
            border: none;
            border-radius: 6px;
        }

        .btn-delete {
            background: linear-gradient(135deg, #e74c3c, #c0392b);
            color: white;
            border: none;
            border-radius: 6px;
        }

        .discount-badge {
            background: linear-gradient(135deg, #27ae60, #2ecc71);
            color: white;
            padding: 4px 8px;
            border-radius: 12px;
            font-weight: bold;
            font-size: 12px;
        }

        .usage-progress {
            background: #ecf0f1;
            border-radius: 10px;
            height: 8px;
            overflow: hidden;
            margin-top: 5px;
        }

        .usage-bar {
            background: linear-gradient(135deg, #3498db, #2980b9);
            height: 100%;
            border-radius: 10px;
            transition: width 0.3s ease;
        }

        .date-range {
            font-size: 12px;
            color: #666;
        }

        .date-start {
            color: #27ae60;
            font-weight: 500;
        }

        .date-end {
            color: #e74c3c;
            font-weight: 500;
        }

        .date-separator {
            margin: 0 5px;
            color: #bdc3c7;
        }
    </style>
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-number" id="totalProducts">0</div>
            <div class="stat-label">Total Cupones</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="availableProducts">0</div>
            <div class="stat-label">Activos</div>
        </div>
        <div class="stat-card">
            <div class="stat-number" id="soldOutProducts">0</div>
            <div class="stat-label">Agotados</div>
        </div>
    </div>

    <%-- NUEVO Modal para Carga de CSV --%>
    <div class="modal" id="csvUploadModal" runat="server" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">Cargar Cupones desde CSV</h2>
                <asp:LinkButton ID="btnCloseCsvModal" runat="server" CssClass="close-button" OnClick="btnCloseCsvModal_Click">&times;</asp:LinkButton>
            </div>
            <div class="form-group" style="padding: 20px;">
                <div class="csv-upload-header">
                    <%-- Contenedor para el label y el botón de ayuda --%>
                    <label for="<%= FileUploadCsv.ClientID %>" class="form-label">
                        Selecciona un archivo CSV:
                    <button type="button" class="help-button" onclick="toggleCsvHelp();">?</button>
                    </label>
                </div>
                <asp:FileUpload ID="FileUploadCsv" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvFileUploadCsv" runat="server" ControlToValidate="FileUploadCsv"
                    ErrorMessage="Por favor, selecciona un archivo CSV." Display="Dynamic" ForeColor="Red"
                    ValidationGroup="CsvUploadValidation"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvCsvFileExtension" runat="server" ControlToValidate="FileUploadCsv"
                    ErrorMessage="El archivo debe ser un CSV (.csv)." OnServerValidate="cvCsvFileExtension_ServerValidate"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="CsvUploadValidation"></asp:CustomValidator>
            </div>

            <div class="form-actions">
                <asp:HiddenField ID="hdnPeliculaId" runat="server" Value="0" />
                <asp:Button ID="btnCancelModal" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelCsvModal_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardarCupones" runat="server" Text="Guardar Cupón" CssClass="btn btn-primary" OnClick="btnUploadCsv_Click" ValidationGroup="PeliculaValidation" />
            </div>
            <asp:Literal ID="litMensajeCsvModal" runat="server"></asp:Literal>
        </div>
    </div>

    <!-- Formulario de Cupón -->
    <div class="modal" id="formContainer" runat="server" visible="false">
        <div class="modal-content">
            <h3 style="margin-bottom: 20px; color: #2c3e50;">
                <asp:Label ID="lblFormTitle" runat="server" Text="Registrar Nuevo Cupón"></asp:Label>
            </h3>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtCodigo">Código del Cupón *</label>
                    <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control"
                        placeholder="Ej: DESC20, VERANO2024" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodigo" runat="server"
                        ControlToValidate="txtCodigo"
                        ErrorMessage="El código es requerido"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label for="txtDescripcion">Descripción *</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control"
                        placeholder="Descripción del cupón" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server"
                        ControlToValidate="txtDescripcion"
                        ErrorMessage="La descripción es requerida"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="ddlDescuentoTipo">Tipo de Descuento *</label>
                    <asp:DropDownList ID="ddlDescuentoTipo" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Porcentaje" Value="porcentaje"></asp:ListItem>
                        <asp:ListItem Text="Monto Fijo" Value="monto_fijo"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvDescuentoTipo" runat="server"
                        ControlToValidate="ddlDescuentoTipo"
                        InitialValue=""
                        ErrorMessage="El tipo de descuento es requerido"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label for="txtPorcentajeDescuento">Valor de Descuento *</label>
                    <asp:TextBox ID="txtPorcentajeDescuento" runat="server" CssClass="form-control"
                        placeholder="Ej: 15 (para %) o 50 (para monto fijo)" TextMode="Number" step="0.01"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPorcentaje" runat="server"
                        ControlToValidate="txtPorcentajeDescuento"
                        ErrorMessage="El valor del descuento es requerido"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rvPorcentaje" runat="server"
                        ControlToValidate="txtPorcentajeDescuento"
                        MinimumValue="0.01" MaximumValue="10000" Type="Double"
                        ErrorMessage="El valor debe ser mayor a 0"
                        ForeColor="Red" Display="Dynamic"></asp:RangeValidator>
                </div>

                <div class="form-group">
                    <label for="txtCantidadMaxima">Cantidad Máxima de Usos *</label>
                    <asp:TextBox ID="txtCantidadMaxima" runat="server" CssClass="form-control"
                        placeholder="Ej: 100" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCantidad" runat="server"
                        ControlToValidate="txtCantidadMaxima"
                        ErrorMessage="La cantidad máxima es requerida"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtFechaInicio">Fecha de Inicio *</label>
                    <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control"
                        TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFechaInicio" runat="server"
                        ControlToValidate="txtFechaInicio"
                        ErrorMessage="La fecha de inicio es requerida"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label for="txtFechaFin">Fecha de Fin *</label>
                    <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control"
                        TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFechaFin" runat="server"
                        ControlToValidate="txtFechaFin"
                        ErrorMessage="La fecha de fin es requerida"
                        ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <asp:CheckBox ID="chkActivo" runat="server" Text="Cupón Activo"
                        Checked="true" CssClass="form-check" />
                </div>
            </div>

            <div style="margin-top: 20px; text-align: right;">
                <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-secondary"
                    Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-success"
                    Text="Guardar Cupón" OnClick="btnGuardar_Click" />
            </div>
        </div>
    </div>

    <!-- Lista de Cupones -->
    <div class="table-container">
        <table class="coupon-table">
            <thead>
                <tr>
                    <th>Código</th>
                    <th>Descripción</th>
                    <th>Descuento</th>
                    <th>Vigencia</th>
                    <th>Uso</th>
                    <th>Estado</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptCupones" runat="server" OnItemCommand="rptCupones_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <strong><%# Eval("codigo") %></strong>
                            </td>
                            <td><%# Eval("descripcionEs") %></td>
                            <td>
                                <span class="discount-badge">
                                    <%# FormatDiscount(Eval("descuentoTipo"), Eval("descuentoValor")) %>
                                </span>
                            </td>
                            <td>
                                <div class="date-range">
                                    <span class="date-start"><%# FormatDateString(Eval("fechaInicio")) %></span>
                                    <span class="date-separator">→</span>
                                    <span class="date-end"><%# FormatDateString(Eval("fechaFin")) %></span>
                                </div>
                            </td>
                            <td>
                                <div>
                                    <small><%# GetUsageText(Eval("usosActuales"), Eval("maxUsos")) %></small>
                                    <div class="usage-progress">
                                        <div class="usage-bar" style='<%# GetUsageProgressStyle(Eval("usosActuales"), Eval("maxUsos")) %>'>
                                        </div>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:Label runat="server"
                                    CssClass='<%# GetStatusClass(Eval("activo"), Eval("fechaFin")) %>'
                                    Text='<%# GetStatusText(Eval("activo"), Eval("fechaFin")) %>' />
                            </td>
                            <td>
                                <asp:Button ID="btnEditar" runat="server" CssClass="btn btn-edit btn-small"
                                    Text="✏️ Editar" CommandName="Editar"
                                    CommandArgument='<%# Eval("cuponId") %>' />
                                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-delete btn-small"
                                    Text="🗑️ Eliminar" CommandName="Eliminar"
                                    CommandArgument='<%# Eval("cuponId") %>'
                                    OnClientClick="return confirmDelete('¿Estás seguro de eliminar este cupón?');" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <asp:HiddenField ID="hiddenCuponId" runat="server" />
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        // Validar fechas
        function validateDates() {
            const fechaInicio = document.getElementById('<%= txtFechaInicio.ClientID %>').value;
            const fechaFin = document.getElementById('<%= txtFechaFin.ClientID %>').value;

            if (fechaInicio && fechaFin) {
                if (new Date(fechaInicio) >= new Date(fechaFin)) {
                    showNotification('La fecha de fin debe ser posterior a la fecha de inicio', 'warning');
                    return false;
                }
            }
            return true;
        }

        // Formatear código en mayúsculas
        document.getElementById('<%= txtCodigo.ClientID %>').addEventListener('input', function () {
            this.value = this.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
        });

        // Validar formulario antes de enviar
        function Page_ClientValidate() {
            return validateDates();
        }

        // Función para confirmar eliminación
        function confirmDelete(message) {
            return confirm(message);
        }

        // Mostrar/ocultar ayuda CSV
        function toggleCsvHelp() {
            const helpBox = document.getElementById('csvHelpBox');
            if (helpBox) {
                helpBox.style.display = helpBox.style.display === 'none' ? 'block' : 'none';
            }
        }

        // Actualizar etiqueta de valor según tipo de descuento
        document.addEventListener('DOMContentLoaded', function() {
            const ddlTipo = document.getElementById('<%= ddlDescuentoTipo.ClientID %>');
            const lblValor = document.querySelector('label[for="<%= txtPorcentajeDescuento.ClientID %>"]');
            const txtValor = document.getElementById('<%= txtPorcentajeDescuento.ClientID %>');

            if (ddlTipo && lblValor && txtValor) {
                ddlTipo.addEventListener('change', function () {
                    if (this.value === 'porcentaje') {
                        lblValor.textContent = 'Porcentaje de Descuento (%) *';
                        txtValor.placeholder = 'Ej: 15';
                        txtValor.max = '100';
                    } else if (this.value === 'monto_fijo') {
                        lblValor.textContent = 'Monto Fijo de Descuento *';
                        txtValor.placeholder = 'Ej: 50.00';
                        txtValor.max = '10000';
                    }
                });
            }
        });
    </script>
</asp:Content>