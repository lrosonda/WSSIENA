using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM
{
    /// <summary>
    /// Descripción breve de WebServiceCortesAlMes
    /// </summary>
    [WebService(Namespace = "http://ensa.com.pa/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceCortesAlMes : System.Web.Services.WebService
    {

        [WebMethod]
        public RespuestaNotificaciones notificacionesCorteTODAS()
        {
            RespuestaNotificaciones respuestaTodasNotificaciones = new RespuestaNotificaciones();
            return respuestaTodasNotificaciones;
        }
        [WebMethod]
        public RespuestaNotificaciones notificacionesCorteAREA(SolicitudCorteAREA solicitudCorteAREA) {
            RespuestaNotificaciones respuestaNotificaciones = new RespuestaNotificaciones();
            return respuestaNotificaciones;
        }
        [WebMethod]
        public RespuestaCorteNAC notificacionesCorteNAC(SolicitudCorteNAC solicitudCorteNAC) {
            RespuestaCorteNAC respuestaCorteNAC = new RespuestaCorteNAC();
            return respuestaCorteNAC;
        }
    }
}
