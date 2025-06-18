<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AutoServicioCineWeb.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iniciar Sesión - Cine PUCP</title>
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="~/styles/auth.css" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <img src="~/images/logo.png" alt="Cine PUCP Logo" class="logo" runat="server" />
            
            <h2>Iniciar Sesión</h2>

            <div class="form-group">
                <label for="txtEmail">Correo Electrónico:</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="El correo electrónico es requerido." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    ErrorMessage="Formato de correo electrónico inválido." ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>

            <div class="form-group" style="width:350px">
                <label for="txtPassword">Contraseña:</label>
                <div style="position: relative; display: inline-block; width:90%">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
                    <button type="button" id="togglePassword" style="position: absolute; right: 5px; top: 50%; transform: translateY(-50%); background: none; border: none; cursor: pointer; width: 20px; height: 20px; padding: 0;">
                        <svg id="iconOpen" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" style="display: block; fill: currentColor;">
                            <path d="M288 32c-80.8 0-145.5 36.8-192.6 80.6C48.6 156 17.3 208 2.5 243.7c-3.3 7.9-3.3 16.7 0 24.6C17.3 304 48.6 356 95.4 399.4C142.5 443.2 207.2 480 288 480s145.5-36.8 192.6-80.6c46.8-43.5 78.1-95.4 93-131.1c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C433.5 68.8 368.8 32 288 32zM144 256a144 144 0 1 1 288 0 144 144 0 1 1 -288 0zm144-64c0 35.3-28.7 64-64 64c-7.1 0-13.9-1.2-20.3-3.3c-5.5-1.8-11.9 1.6-11.7 7.4c.3 6.9 1.3 13.8 3.2 20.7c13.7 51.2 66.4 81.6 117.6 67.9s81.6-66.4 67.9-117.6c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3z" />
                        </svg>

                        <svg id="iconClosed" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512" style="display: none; fill: currentColor;">
                            <path d="M38.8 5.1C28.4-3.1 13.3-1.2 5.1 9.2S-1.2 34.7 9.2 42.9l592 464c10.4 8.2 25.5 6.3 33.7-4.1s6.3-25.5-4.1-33.7L525.6 386.7c39.6-40.6 66.4-86.1 79.9-118.4c3.3-7.9 3.3-16.7 0-24.6c-14.9-35.7-46.2-87.7-93-131.1C465.5 68.8 400.8 32 320 32c-68.2 0-125 26.3-169.3 60.8L38.8 5.1zM223.1 149.5C248.6 126.2 282.7 112 320 112c79.5 0 144 64.5 144 144c0 24.9-6.3 48.3-17.4 68.7L408 294.5c8.4-19.3 10.6-41.4 4.8-63.3c-11.1-41.5-47.8-69.4-88.6-71.1c-5.8-.2-9.2 6.1-7.4 11.7c2.1 6.4 3.3 13.2 3.3 20.3c0 10.2-2.4 19.8-6.6 28.3l-90.3-70.8zM373 389.9c-16.4 6.5-34.3 10.1-53 10.1c-79.5 0-144-64.5-144-144c0-6.9 .5-13.6 1.4-20.2L83.1 161.5C60.3 191.2 44 220.8 34.5 243.7c-3.3 7.9-3.3 16.7 0 24.6c14.9 35.7 46.2 87.7 93 131.1C174.5 443.2 239.2 480 320 480c47.8 0 89.9-12.9 126.2-32.5L373 389.9z" />
                        </svg>
                    </button>
                </div>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="La contraseña es requerida." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" EnableViewState="false"></asp:Label>

            <asp:Button ID="btnLogin" runat="server" Text="Iniciar Sesión" CssClass="btn-submit" OnClick="btnLogin_Click" />

            <div class="form-links">
                <p>¿No tienes cuenta? <a href="Registro.aspx">Regístrate aquí</a></p>
                <p><a href="Inicio.aspx">Volver al inicio</a></p>
            </div>
        </div>
    </form>
</body>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Get references to the password field and the icon elements
            const passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            const toggleButton = document.getElementById('togglePassword');
            const iconOpen = document.getElementById('iconOpen'); // Your "eye open" SVG
            const iconClosed = document.getElementById('iconClosed'); // Your "eye closed" SVG

            // Add a click event listener to the toggle button
            toggleButton.addEventListener('click', function () {
                // Check the current type of the password input
                if (passwordField.type === 'password') {
                    // If it's currently hidden (password type), show it
                    passwordField.type = 'text';
                    iconOpen.style.display = 'none';    // Hide the "eye open" icon
                    iconClosed.style.display = 'block'; // Show the "eye closed" icon
                } else {
                    // If it's currently visible (text type), hide it
                    passwordField.type = 'password';
                    iconOpen.style.display = 'block';   // Show the "eye open" icon
                    iconClosed.style.display = 'none';  // Hide the "eye closed" icon
                }
            });
        });
    </script>
</html>