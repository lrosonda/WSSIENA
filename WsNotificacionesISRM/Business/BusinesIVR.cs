using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
  
    public class BusinesIVR: BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RespuestaEnvioIVR envioIVR(SolicitudEnvioIVR solicitudEnvioIVR)
        {
            log.Debug("Welcome!!");
            log.Debug("solicitudEnvioIVR: " + solicitudEnvioIVR.ToString());
            RespuestaEnvioIVR resp = new RespuestaEnvioIVR();
            Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_IVR");
            string status = (string)keyValuesProcess["status"];
            log.Debug("status:" + status);
            try
            {
                if (status.Equals("NT001"))
                {
                    Int32 id = (Int32)keyValuesProcess["id_proceso"];
                    if (validateIVRProcess(id, resp))
                    {
                        resp.mensajeRespuesta = "Operación realizada de forma exitosa";
                        resp.codigoMensaje = 100;
                    }

                }
                else
                {
                    log.Error("IVR ha sido desactivado consultar los parametros notificaciones");
                    resp.mensajeRespuesta = "Error, IVR Inactivo";
                    resp.codigoMensaje = -101;
                }
            }
            catch (SQLUtilException e)
            {
                resp.mensajeRespuesta = "Error, operación fallida de la base de datos";
                resp.codigoMensaje = -1001;
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                resp.mensajeRespuesta = "Error, parametrización errada en el webservice de IVR";
                resp.codigoMensaje = -1002;
                log.Error(e.Message);
            }
            log.Debug("Bey!!");
            return resp;
        }

      
        private bool validateIVRProcess(Int32 idProcess, RespuestaEnvioIVR resp)
        {
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni = (DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe = (String)keyValuePairs["dias"];
            string iniHour = (String)keyValuePairs["hora_inicial"];
            string endHour = (String)keyValuePairs["hora_fin"];
            string sql  = (String)keyValuePairs["sql"];
            DateTime currentDateTime = DateTime.Now;

            int comparetoDate = currentDateTime.CompareTo(dateIni);
            if (comparetoDate >= 0 && !keyValuePairs.ContainsKey("fecha_vencimiento"))
            {
                if (validateExecutionDays(strDayExe, currentDateTime))
                {
                    if (!validateExecutionHours(iniHour, endHour, currentDateTime))
                    {
                        log.Error("Error, IVR fuera rango de ejecucución: [" + iniHour + "-" + endHour + "]");
                        resp.mensajeRespuesta = "Error, IVR fuera rango de ejecución";
                        resp.codigoMensaje = -103;
                        return false;
                    }
                }
                else
                {
                    log.Error("Error, hoy no se ejecuta IVR. Los dias que se ejecuta son: " + strDayExe);
                    resp.mensajeRespuesta = "Error, hoy no se ejecuta IVR";
                    resp.codigoMensaje = -102;
                    return false;
                }
            }
            else
            {
                log.Error("IVR No se encuentra activo");
                resp.mensajeRespuesta = "Error, IVR Inactivo";
                resp.codigoMensaje = -101;
                return false;
            }
            return true;
        }
        private void setIVRValues(string sal, RespuestaEnvioIVR resp) { 
        }
        

        
    }

}