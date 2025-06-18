<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signup.aspx.cs" Inherits="AutoServicioCineWeb.Signup" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrarse - Cine PUCP</title>

    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap" rel="stylesheet" />
    
    <link rel="stylesheet" type="text/css" href="~/styles/auth.css" runat="server" />
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
                    <div style="position: relative; display: inline-block; width: 100%;">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" style="width:100%"></asp:TextBox>
                        <button type="button" id="togglePassword" style="position: absolute; right: 5px; top: 50%; transform: translateY(-50%); background: none; border: none; cursor: pointer; width: 20px; height: 20px; padding: 0;">
                            <svg id="iconOpenPass" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" style="display: block; fill: currentColor;"><path d="M288 32c-80.8 0-145.5 36.8-192.6 80.6C48.6 156 17.3 208 2.5 243.7c-3.3 7.9-3.3 16.7 0 24.6C17.3 304 48.6 356 95.4 399.4C142.5 443.2 207.2 480 288 480s145.5-36.8 192.6-80.6c46.8-43.5 78.1-95.4 93-131.1c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C433.5 68.8 368.8 32 288 32zM144 256a144 144 0 1 1 288 0 144 144 0 1 1 -288 0zm144-64c0 35.3-28.7 64-64 64c-7.1 0-13.9-1.2-20.3-3.3c-5.5-1.8-11.9 1.6-11.7 7.4c.3 6.9 1.3 13.8 3.2 20.7c13.7 51.2 66.4 81.6 117.6 67.9s81.6-66.4 67.9-117.6c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3z"/></svg>

                            <svg id="iconClosedPass" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512" style="display: none; fill: currentColor;"><path d="M38.8 5.1C28.4-3.1 13.3-1.2 5.1 9.2S-1.2 34.7 9.2 42.9l592 464c10.4 8.2 25.5 6.3 33.7-4.1s6.3-25.5-4.1-33.7L525.6 386.7c39.6-40.6 66.4-86.1 79.9-118.4c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C465.5 68.8 400.8 32 320 32c-68.2 0-125 26.3-169.3 60.8L38.8 5.1zM223.1 149.5C248.6 126.2 282.7 112 320 112c79.5 0 144 64.5 144 144c0 24.9-6.3 48.3-17.4 68.7L408 294.5c8.4-19.3 10.6-41.4 4.8-63.3c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3c0 10.2-2.4 19.8-6.6 28.3l-90.3-70.8zM373 389.9c-16.4 6.5-34.3 10.1-53 10.1c-79.5 0-144-64.5-144-144c0-6.9 .5-13.6 1.4-20.2L83.1 161.5C60.3 191.2 44 220.8 34.5 243.7c-3.3 7.9-3.3 16.7 0 24.6c14.9 35.7 46.2 87.7 93 131.1C174.5 443.2 239.2 480 320 480c47.8 0 89.9-12.9 126.2-32.5L373 389.9z"/></svg>
                        </button>
                    </div>
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
                    <div style="position: relative; display: inline-block; width: 100%;">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" style="width: 100%"></asp:TextBox>
                        <button type="button" id="toggleConfirmPassword" style="position: absolute; right: 5px; top: 50%; transform: translateY(-50%); background: none; border: none; cursor: pointer; width: 20px; height: 20px; padding: 0;">
                            <svg id="iconOpenConfirm" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" style="display: block; fill: currentColor;"><path d="M288 32c-80.8 0-145.5 36.8-192.6 80.6C48.6 156 17.3 208 2.5 243.7c-3.3 7.9-3.3 16.7 0 24.6C17.3 304 48.6 356 95.4 399.4C142.5 443.2 207.2 480 288 480s145.5-36.8 192.6-80.6c46.8-43.5 78.1-95.4 93-131.1c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C433.5 68.8 368.8 32 288 32zM144 256a144 144 0 1 1 288 0 144 144 0 1 1 -288 0zm144-64c0 35.3-28.7 64-64 64c-7.1 0-13.9-1.2-20.3-3.3c-5.5-1.8-11.9 1.6-11.7 7.4c.3 6.9 1.3 13.8 3.2 20.7c13.7 51.2 66.4 81.6 117.6 67.9s81.6-66.4 67.9-117.6c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3z"/></svg>

                            <svg id="iconClosedConfirm" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512" style="display: none; fill: currentColor;"><path d="M38.8 5.1C28.4-3.1 13.3-1.2 5.1 9.2S-1.2 34.7 9.2 42.9l592 464c10.4 8.2 25.5 6.3 33.7-4.1s6.3-25.5-4.1-33.7L525.6 386.7c39.6-40.6 66.4-86.1 79.9-118.4c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C465.5 68.8 400.8 32 320 32c-68.2 0-125 26.3-169.3 60.8L38.8 5.1zM223.1 149.5C248.6 126.2 282.7 112 320 112c79.5 0 144 64.5 144 144c0 24.9-6.3 48.3-17.4 68.7L408 294.5c8.4-19.3 10.6-41.4 4.8-63.3c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3c0 10.2-2.4 19.8-6.6 28.3l-90.3-70.8zM373 389.9c-16.4 6.5-34.3 10.1-53 10.1c-79.5 0-144-64.5-144-144c0-6.9 .5-13.6 1.4-20.2L83.1 161.5C60.3 191.2 44 220.8 34.5 243.7c-3.3 7.9-3.3 16.7 0 24.6c14.9 35.7 46.2 87.7 93 131.1C174.5 443.2 239.2 480 320 480c47.8 0 89.9-12.9 126.2-32.5L373 389.9z"/></svg>
                        </button>
                    </div>
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
                <p><a href="Inicio.aspx">Volver al inicio</a></p>
            </div>
        </div>
    </form>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Password Field Toggle
            const passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            const toggleButtonPass = document.getElementById('togglePassword');
            const iconOpenPass = document.getElementById('iconOpenPass');
            const iconClosedPass = document.getElementById('iconClosedPass');

            toggleButtonPass.addEventListener('click', function () {
                if (passwordField.type === 'password') {
                    passwordField.type = 'text';
                    iconOpenPass.style.display = 'none';
                    iconClosedPass.style.display = 'block';
                } else {
                    passwordField.type = 'password';
                    iconOpenPass.style.display = 'block';
                    iconClosedPass.style.display = 'none';
                }
            });

            // Confirm Password Field Toggle
            const confirmPasswordField = document.getElementById('<%= txtConfirmPassword.ClientID %>');
            const toggleButtonConfirm = document.getElementById('toggleConfirmPassword');
            const iconOpenConfirm = document.getElementById('iconOpenConfirm');
            const iconClosedConfirm = document.getElementById('iconClosedConfirm');

            toggleButtonConfirm.addEventListener('click', function () {
                if (confirmPasswordField.type === 'password') {
                    confirmPasswordField.type = 'text';
                    iconOpenConfirm.style.display = 'none';
                    iconClosedConfirm.style.display = 'block';
                } else {
                    confirmPasswordField.type = 'password';
                    iconOpenConfirm.style.display = 'block';
                    iconClosedConfirm.style.display = 'none';
                }
            });
        });
    </script>
</body>
</html>