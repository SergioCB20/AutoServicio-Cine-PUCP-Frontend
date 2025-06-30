using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutoServicioCineWeb.AutoservicioCineWS;
using Newtonsoft.Json;

namespace AutoServicioCineWeb
{
    public partial class Butacas : System.Web.UI.Page
    {
        private readonly AsientoFuncionWSClient asientoFuncionServiceClient;
        private readonly AsientoWSClient asientoServiceClient;
        private List<asientoFuncion> listaAsientosFunciones;
        private List<asiento> listaAsientos;
        int funcionGuardada = 0;
        int salaGuardada = 0;
        public Butacas()
        {
            asientoFuncionServiceClient = new AsientoFuncionWSClient();
            asientoServiceClient = new AsientoWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var resumen = Session["ResumenCompra"] as ResumenCompra; //Carga los valores que vinieron de la vista Tickets
                if (resumen != null)
                {
                    var master = this.Master as Form; //Para usar el texto que está definido en el Form.Master

                    if (master != null)
                    {
                        master.EntradasAdultoTexto.InnerText = resumen.AdultoTicket;
                        master.EntradasInfantilTexto.InnerText = resumen.InfantilTicket;
                        master.EntradasMayorTexto.InnerText = resumen.MayorTicket;
                        master.TotalResumen.InnerText = resumen.TotalTicket;
                        master.ImgPoster.Src = resumen.ImagenUrl;
                        master.TituloSpan.InnerText = resumen.TituloDePelicula;
                    }
                }
                funcion funcionSeleccionada = Session["FuncionSeleccionada"] as funcion;
                if (funcionSeleccionada != null)
                {
                    var master = this.Master as Form;
                    funcionGuardada = funcionSeleccionada.funcionId;
                    salaGuardada = funcionSeleccionada.salaId;
                    if (DateTime.TryParseExact(funcionSeleccionada.fechaHora, "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaHora))
                    {
                        master.FechaSpan.InnerText = fechaHora.ToString("dddd, dd MMMM yyyy", new CultureInfo("es-PE"));
                        master.HoraSpan.InnerText = fechaHora.ToString("hh:mm tt", new CultureInfo("es-PE"));
                    }
                    else
                    {
                        master.HoraSpan.InnerText = funcionSeleccionada.fechaHora;
                        master.FechaSpan.InnerText = "";
                    }
                }

                //listaAsientosFunciones = asientoFuncionServiceClient.listarAsientosPorFunciones(funcionGuardada).ToList();
                //listaAsientos = asientoServiceClient.listarAsientosSala(salaGuardada).ToList();

                //string asientosJson = JsonConvert.SerializeObject(listaAsientos);
                //string asientosFuncionJson = JsonConvert.SerializeObject(listaAsientosFunciones);
                //string asientosJsonEncoded = HttpUtility.JavaScriptStringEncode(asientosJson);
                //string asientosFuncionJsonEncoded = HttpUtility.JavaScriptStringEncode(asientosFuncionJson);
                //ClientScript.RegisterStartupScript(
                //this.GetType(),
                //"asientosData",
                //$@"var asientos = JSON.parse(""{asientosJsonEncoded}"");
                //var asientosFuncion = JSON.parse(""{asientosFuncionJsonEncoded}"");
                //    document.addEventListener('DOMContentLoaded', function () {{
                //    generarSala(asientos, asientosFuncion);
                //    ajustarPantallaDinamico();
                //    }});",
                //true
                //);
//                string asientosJson = JsonConvert.SerializeObject(listaAsientos);
//                string asientosFuncionJson = JsonConvert.SerializeObject(listaAsientosFunciones);
//                ClientScript.RegisterStartupScript(this.GetType(), "asientosData", $@"
//    var asientos = {asientosJson};
//    var asientosFuncion = {asientosFuncionJson};
//    document.addEventListener('DOMContentLoaded', function () {{
//        generarSala(asientos, asientosFuncion);
//        ajustarPantallaDinamico();
//    }});
//", true);

            }
        }
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            //Esta parte es para enviar los valores que fueron modificados a la siguiente vista (Comida.aspx)
            
            var master = this.Master as Form;
            var resumen = new ResumenCompra
            {
                AdultoTicket = master.EntradasAdultoTexto.InnerText,
                InfantilTicket = master.EntradasInfantilTexto.InnerText,
                MayorTicket = master.EntradasMayorTexto.InnerText,
                TotalTicket = master.TotalResumen.InnerText,
                TituloDePelicula = master.TituloSpan.InnerText,
                ImagenUrl = master.ImgPoster.Src
            };

            Session["ResumenCompra"] = resumen;
            Response.Redirect("Comida.aspx");
        }
    }
}