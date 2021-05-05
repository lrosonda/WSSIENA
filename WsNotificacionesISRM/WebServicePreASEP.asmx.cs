using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM
{
    /// <summary>
    /// Descripción breve de WebServicePreASEP
    /// </summary>
    [WebService(Namespace = "http://ensa.com.pa/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServicePreASEP : System.Web.Services.WebService
    {

        [WebMethod]
        public RespuestaEnvioPreASEP envioLotePreASEP()
        {
            RespuestaEnvioPreASEP r = new RespuestaEnvioPreASEP();
            return r;
        }
        [WebMethod]
        public RespuestaConfirmacionPreASEP confirmacionLotePreASEP(SolicitudConfirmacionPreASEP request) {
            RespuestaConfirmacionPreASEP response = new RespuestaConfirmacionPreASEP();
            return response;
        }
    }
}
