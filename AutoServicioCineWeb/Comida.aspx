<%@ Page Language="C#" MasterPageFile="~/Form.master" AutoEventWireup="true" CodeBehind="Comida.aspx.cs" Inherits="AutoServicioCineWeb.Comida" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Comida
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="comida-container">
        <h2>Elige tu comida</h2>
        <div class="lista-comidas">
            <div class="comida-item">
                <img src="images/canchamediana.png" alt="Combo Clásico" class="comida-img" />
                <div class="comida-detalle">
                    <h3>Combo Clásico</h3>
                    <p>Canchita mediana + Gaseosa 16oz</p>
                    <span class="precio">S/ 18.00</span>
                </div>
            </div>
            <div class="comida-item">
                <img src="images/canchagrande.png" alt="Combo Doble" class="comida-img" />
                <div class="comida-detalle">
                    <h3>Combo Doble</h3>
                    <p>Canchita grande + 2 Gaseosas 22oz</p>
                    <span class="precio">S/ 30.00</span>
                </div>
            </div>
            <div class="comida-item">
                <img src="images/hotdog.png" alt="Hot Dog" class="comida-img" />
                <div class="comida-detalle">
                    <h3>Hot Dog</h3>
                    <p>Pan con salchicha y salsas a elección</p>
                    <span class="precio">S/ 10.00</span>
                </div>
            </div>
            
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .comida-container {
            padding: 1.5rem;
        }
        .lista-comidas {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
            gap: 1.5rem;
        }
        .comida-item {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            overflow: hidden;
            transition: transform 0.2s;
        }
        .comida-item:hover {
            transform: translateY(-5px);
        }
        .comida-img {
            width: 100%;
            height: 180px;
            object-fit: cover;
        }
        .comida-detalle {
            padding: 1rem;
        }
        .comida-detalle h3 {
            margin: 0 0 0.5rem;
        }
        .precio {
            font-weight: bold;
            color: #c00;
        }
    </style>
</asp:Content>

