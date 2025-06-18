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
            
            <div class="form-group">
                <label for="txtPassword">Contraseña:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
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
</html>