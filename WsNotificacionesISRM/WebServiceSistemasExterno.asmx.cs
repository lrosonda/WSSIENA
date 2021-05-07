using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM
{
    /// <summary>
    /// Descripción breve de WebServiceSistemasExterno
    /// </summary>
    [WebService(Namespace = "http://ensa.com.pa/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceSistemasExterno : System.Web.Services.WebService
    {

        [WebMethod]
        public RespuestaSistemaExternos envioCorreoExterno(SolicitudSistemaExternos solicitud)
        {
            RespuestaSistemaExternos resp = new RespuestaSistemaExternos();
            return resp;
        }
    }
}
