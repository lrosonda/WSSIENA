using System.Web.Services;
using WsNotificacionesISRM.Business;
using WsNotificacionesISRM.DTO;
using static WsNotificacionesISRM.DTO.RespuestaEnvioIVR;

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
            return new BusinesIVR().envioIVR(solicitudEnvioIVR);
        }
        [WebMethod]
        public RespuestaConfirmacionIVR confirmacionIVR(SolitudConfirmacionIVR solitudConfirmacionIVR)
        {
            return new BusinesIVR().confirmacionIVR(solitudConfirmacionIVR);
        }
    }
}
