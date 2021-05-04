using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            log.Debug("solicitudEnvioIVR: " + solicitudEnvioIVR.ToString());
            RespuestaEnvioIVR resp = new RespuestaEnvioIVR();
            Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses();
            string status = (string)keyValuesProcess["status"];
            log.Debug("status:"+ status);
            if (status.Equals("NT001"))
            {
                Int32 id = (Int32)keyValuesProcess["id_proceso"];
                if (validateIVRProcess(id, resp))
                {
                    resp.mensajeRespuesta = "Operación realizada de forma exitosa";
                    resp.codigoMensaje = 100;
                }
                
            }
            else {
                log.Error("IVR ha sido desactivado consultar los parametros notificaciones");
                resp.mensajeRespuesta = "Error, IVR Inactivo";
                resp.codigoMensaje = -101;
            }
            
            log.Debug("Bey!!");
            return resp;
        }

        private Dictionary<String, Object> retrieveNotificationProcesses()
        {
            StringBuilder sb = new StringBuilder("SELECT id_proceso, status, fecha_actualizacion, repetir_cada, repetir_x, sql");
            sb.Append(" FROM NOTIFICA.procesos_notificaciones  WHERE nombre_proceso='WS_IVR'");
            string[] columms = { "id_proceso", "status", "fecha_actualizacion", "repetir_cada", "repetir_x", "sql" };
            log.Debug("SQL01:"+ sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        private Dictionary<String, Object> getProcessParameters(Int32 iDprocess) {
            StringBuilder sb = new StringBuilder("SELECT fecha_inicial, fecha_vencimiento, dias, hora_inicial, hora_fin FROM NOTIFICA.parametros_procesos WHERE id_proceso =");
            sb.Append(iDprocess);
            string[] columms = { "fecha_inicial", "dias", "hora_inicial", "hora_fin" };
            log.Debug("SQL02:" + sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        private bool validateIVRProcess(Int32 idProcess, RespuestaEnvioIVR resp) {
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni =(DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe  =(String)keyValuePairs["dias"];
            DateTime currentDateTime = DateTime.Now;
            int comparetoDate = currentDateTime.CompareTo(dateIni);
            if (comparetoDate >= 0)
            {
                if (validateExecutionDays(strDayExe, currentDateTime))
                {
                }
                else {
                    log.Error("Error, hoy no se ejecuta IVR. Los dias que se ejecuta son: "+ strDayExe);
                    resp.mensajeRespuesta = "Error, hoy no se ejecuta IVR";
                    resp.codigoMensaje = -102;
                    return false;
                }
            }
            else {
                log.Error("IVR No se encuentra activo");
                resp.mensajeRespuesta = "Error, IVR Inactivo";
                resp.codigoMensaje = -101;
                return false;
            }
            return true;
        }
        private bool validateExecutionDays(string strDayExe, DateTime cDateTime)
        {
            if (strDayExe.Equals("TODOS"))
            {
                return true;
            }
            else {
                string[] split = strDayExe.Split(new Char[] { ';' });
                foreach (string sDay in split)  
                {
                   String _sDay = sDay.ToUpper();
                   if (_sDay.Equals("LUNES") && cDateTime.DayOfWeek == DayOfWeek.Monday) {
                        return true;
                    }
                   else if (_sDay.Equals("MARTES") && cDateTime.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        return true;
                    }
                   else if (_sDay.Equals("MIERCOLES") && cDateTime.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        return true;
                    }
                   else if (_sDay.Equals("JUEVES") && cDateTime.DayOfWeek == DayOfWeek.Thursday)
                    {
                        return true;
                    }
                    else if (_sDay.Equals("VIERNES") && cDateTime.DayOfWeek == DayOfWeek.Friday)
                    {
                        return true;
                    }
                    else if (_sDay.Equals("SABADO") && cDateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        return true;
                    }
                    else if (_sDay.Equals("DOMINGO") && cDateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        return true;
                    }
                }
                 
            }
            return false;
        }
    }
   
}