using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoServicioCineWeb.AutoservicioCineWS
{
    public partial class ventaProducto
    {
        [System.Xml.Serialization.XmlIgnore]
        public string nombreProducto { get; set; } // uso solo en frontend
    }
}