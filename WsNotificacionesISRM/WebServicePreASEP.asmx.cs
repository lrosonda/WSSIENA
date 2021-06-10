using System.Web.Services;
using WsNotificacionesISRM.Business;
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
            return new BusinessPreASEP().envioLotePreASEP();
        }
        [WebMethod]
        public RespuestaConfirmacionPreASEP confirmacionLotePreASEP(SolicitudConfirmacionPreASEP request)
        {
            return new BusinessPreASEP().confirmacionLotePreASEP(request);
        }
    }
}
