using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WsNotificacionesISRM.Business;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM
{
    /// <summary>
    /// Descripción breve de WebServiceIVR
    /// </summary>
    [WebService(Namespace = "http://ensa.com.pa/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceIVR : System.Web.Services.WebService
    {

        [WebMethod]
        public RespuestaEnvioIVR envioIVR(SolicitudEnvioIVR solicitudEnvioIVR)
        {
            BusinesIVR ivr = new BusinesIVR();
            return ivr.envioIVR(solicitudEnvioIVR);
        }
        [WebMethod]
        public RespuestaConfirmacionIVR confirmacionIVR(SolitudConfirmacionIVR solitudConfirmacionIVR) {
            RespuestaConfirmacionIVR respuestaConfirmacionIVR = new RespuestaConfirmacionIVR();
            return respuestaConfirmacionIVR;
        }
    }
}
