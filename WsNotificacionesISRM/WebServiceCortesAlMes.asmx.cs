using System.Web.Services;
using WsNotificacionesISRM.Business;
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
            return new BusinessInterruptionsPerMonth().notificacionesCorteTODAS();
        }
        [WebMethod]
        public RespuestaNotificaciones notificacionesCorteAREA(SolicitudCorteAREA solicitudCorteAREA)
        {

            return new BusinessInterruptionsPerMonth().notificacionesCorteAREA(solicitudCorteAREA);
        }
        [WebMethod]
        public RespuestaCorteNAC notificacionesCorteNAC(SolicitudCorteNAC solicitudCorteNAC)
        {
            return new BusinessInterruptionsPerMonth().notificacionesCorteNAC(solicitudCorteNAC);
        }
    }
}
