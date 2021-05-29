using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinessExternalSystems : BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RespuestaSistemaExternos envioCorreoExterno(SolicitudSistemaExternos solicitud)
        {
            log.Debug("Welcome!!");
            log.Debug("request: " + solicitud.ToString());
            RespuestaSistemaExternos resp = new RespuestaSistemaExternos();
            log.Debug("resp:" + resp.ToString());
            return resp;
        }
    }
}