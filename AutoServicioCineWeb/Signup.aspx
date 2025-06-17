<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="AutoServicioCineWeb.Signup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrarse - Cine PUCP</title>

    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap" rel="stylesheet" />
    
    <link rel="stylesheet" type="text/css" href="~/styles/login.css" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container" style="min-width:800px">
            <img src="~/images/logo.png" alt="Cine PUCP Logo" class="logo" runat="server" />
            
            <h2>Crear Cuenta</h2>

            <div style="display: flex; gap: 10px; width:100%; justify-content:space-between">
                <div class="form-group" style="width:45%">
                    <label for="txtNombre">Nombre Completo:</label>
                    <asp:TextBox ID="txtNombre" runat="server" TextMode="SingleLine" style="width:100%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es requerido." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group" style="width:45%">
                    <label for="txtEmail">Correo Electrónico:</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" style="width:100%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                        ErrorMessage="El correo electrónico es requerido." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Formato de correo electrónico inválido." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
                </div>
            </div>

            <div style="display: flex; gap: 10px; width:100%; justify-content:space-between">
                <div class="form-group" style="width:45%">
                    <label for="txtTelefono">Número de Teléfono:</label>
                    <asp:TextBox ID="txtTelefono" runat="server" TextMode="SingleLine" style="width:100%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTelefono" runat="server" ControlToValidate="txtTelefono"
                        ErrorMessage="El número de teléfono es requerido." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revTelefono" runat="server" ControlToValidate="txtTelefono"
                        ValidationExpression="^\+?[0-9]{7,15}$"
                        ErrorMessage="Formato de teléfono inválido (ej: +51987654321 o 987654321)." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
                </div>

                <div class="form-group" style="width:45%">
                    <label for="txtPassword">Contraseña:</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width:100%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                        ErrorMessage="La contraseña es requerida." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword"
                        ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$"
                        ErrorMessage="La contraseña debe tener al menos 8 caracteres, incluyendo letras y números." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
                </div>
            </div>
            

            <div style="display: flex; gap: 10px; width:100%; justify-content:space-between">
                <div class="form-group" style="width:45%">
                    <label for="txtConfirmPassword">Confirmar Contraseña:</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" style="width: 100%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        ErrorMessage="Confirmar contraseña es requerido." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cmpPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        ControlToCompare="txtPassword" ErrorMessage="Las contraseñas no coinciden." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="asp-message" EnableViewState="false" Font-Size="X-Small" Width="45%"></asp:Label>
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Registrarse" CssClass="btn-submit" OnClick="btnRegister_Click" />

            <div class="form-links">
                <p>¿Ya tienes cuenta? <a href="Login.aspx">Inicia sesión aquí</a></p>
                <p><a href="Default.aspx">Volver al inicio</a></p>
            </div>
        </div>
    </form>
</body>
</html>