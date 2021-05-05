using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinessInterruptionsPerMonth: BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RespuestaNotificaciones notificacionesCorteTODAS() {
            RespuestaNotificaciones resp = new RespuestaNotificaciones();
            return resp;
        }
    }
}