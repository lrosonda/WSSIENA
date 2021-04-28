using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinesIVR
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RespuestaEnvioIVR envioIVR(SolicitudEnvioIVR solicitudEnvioIVR)
        {
            log.Debug("Welcome!!");
            log.Debug("solicitudEnvioIVR: "+ solicitudEnvioIVR.ToString());
            RespuestaEnvioIVR resp = new RespuestaEnvioIVR();
            log.Debug("Bey!!");
            return resp;
        }
    }

}