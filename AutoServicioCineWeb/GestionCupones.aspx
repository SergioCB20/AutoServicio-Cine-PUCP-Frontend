<%@ Page Title="Gestión de Cupones" Language="C#" MasterPageFile="~/Admin.master" AutoEventWireup="true" CodeBehind="GestionCupones.aspx.cs" Inherits="AutoServicioCineWeb.GestionCupones" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="PageTitle" runat="server">
    Gestión de Cupones
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
                     link.parentElement.classList.remove('active'); // Asegura que solo uno esté activo
                }
            });
        });
    </script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-title">
        <h1>Gestión de Cupones</h1>
    </div>
    <div class="actions">
        <button class="button primary">Agregar Nuevo Cupón</button>
        <button class="button secondary">Importar desde CSV</button>
    </div>
    <div class="table-container">
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Código</th>
                    <th>Descuento (%)</th>
                    <th>Fecha de Inicio</th>
                    <th>Fecha de Fin</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>1</td>
                    <td>CINEPUCP20</td>
                    <td>20</td>
                    <td>2025-05-20</td>
                    <td>2025-06-30</td>
                    <td class="actions-cell">
                        <button class="button edit">Editar</button>
                        <button class="button delete">Eliminar</button>
                    </td>
                </tr>
                <tr>
                    <td>2</td>
                    <td>VERANO15</td>
                    <td>15</td>
                    <td>2025-06-01</td>
                    <td>2025-07-31</td>
                    <td class="actions-cell">
                        <button class="button edit">Editar</button>
                        <button class="button delete">Eliminar</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>