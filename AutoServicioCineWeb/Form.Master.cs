using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AutoServicioCineWeb
{
    public partial class Form : System.Web.UI.MasterPage
    {
        public HtmlGenericControl TituloSpan => tituloSpan;
        public HtmlImage ImgPoster => imgPoster;
        public HtmlGenericControl EntradasAdultoTexto => entradasAdultoTexto;
        public HtmlGenericControl EntradasInfantilTexto => entradasInfantilTexto;
        public HtmlGenericControl EntradasMayorTexto => entradasMayorTexto;
        public HtmlGenericControl TotalResumen => totalResumen;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
    }
}