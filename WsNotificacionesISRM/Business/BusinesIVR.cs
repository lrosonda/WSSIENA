using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{

    public class BusinesIVR : BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int A_DAY_IN_MILLISECONDS= 1000*60*60*24;
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
                    if (runIVRProcess(keyValuesProcess, solicitudEnvioIVR, resp))
                    {
                        resp.mensajeRespuesta = "Operación realizada de forma exitosa";
                        resp.codigoMensaje = 0;
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
            log.Debug("resp:" + resp.ToString());
            log.Debug("Bey!!");
            return resp;
        }


        private bool runIVRProcess(Dictionary<String, Object> dProcess, SolicitudEnvioIVR solicitudEnvioIVR, RespuestaEnvioIVR resp)
        {
            Int32 idProcess = (Int32)dProcess["id_proceso"];
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni = (DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe = (String)keyValuePairs["dias"];
            string iniHour = (String)keyValuePairs["hora_inicial"];
            string endHour = (String)keyValuePairs["hora_fin"];
            string sql = (String)dProcess["sql"];
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
                    else
                    {
                        if (buildsMessageResponse(idProcess, sql, solicitudEnvioIVR, resp) < 0)
                        {
                            resp.mensajeRespuesta = "No hay registros para enviar a IVR";
                            resp.codigoMensaje = 100;
                            return false;
                        }
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
        private int buildsMessageResponse(Int32 idProcess, string sql, SolicitudEnvioIVR resquest, RespuestaEnvioIVR response)
        {
            int _ret = 0;
            StringBuilder sb = new StringBuilder(sql);
            List<Dictionary<String, Object>> filters = getFiltersByProcessId(idProcess);
            StringBuilder sbFilters = new StringBuilder();
            if (resquest != null && resquest.tipo != null && resquest.tipo.Length > 0)
            {
                sbFilters.Append("'").Append(resquest.tipo).Append("'");
            }
            if (filters != null && filters.Count() > 0)
            {
                foreach (Dictionary<String, Object> filter in filters)
                {
                    if (sbFilters.Length > 0)
                    {
                        sbFilters.Append(",'").Append(filter["filtro"]).Append("'");
                    }
                    else
                    {
                        sbFilters.Append("'").Append(filter["filtro"]).Append("'");
                    }
                }
            }
            if (sbFilters.Length > 0)
            {
                sb.Replace("'CORTE'", sbFilters.ToString());
            }
            log.Debug("SQL maneuversReceived:" + sb.ToString());
            string[] columms = { "IdentificadorMensaje", "areaAfectada", "tipo", "FechaDeCorte", "FechaDeRestauracion", "estadoTrabajo", "nombre", "segundoNombre", "telefonoResidencial", "telefonoMovil", "switchingPlan_mRID" };
            List<Dictionary<String, Object>> maneuversReceived = SQLUtil.getQueryResultList(sb.ToString(), columms);
            if (maneuversReceived.Count() > 0)
            {
                foreach (Dictionary<String, Object> d in maneuversReceived)
                {
                    response.identificadorMensaje = (Int32)d["IdentificadorMensaje"];
                    response.areaAfectada = (string)d["areaAfectada"];
                    response.tipo = (string)d["tipo"];
                    response.fechaDeCorte = (string)d["FechaDeCorte"];
                    response.fechaDeRestauracion = (string)d["FechaDeRestauracion"];
                    response.estadoTrabajo = (string)d["estadoTrabajo"];
                    response.nombre = (string)d["nombre"];
                    response.segundoNombre = (string)d["segundoNombre"];
                    response.telefonoResidencial = (string)d["telefonoResidencial"];
                    response.telefonoMovil = (string)d["telefonoMovil"];
                    updateIVR("EV001", response.identificadorMensaje);
                    DateTime currentDateTime = DateTime.Now;

                    int waittime = A_DAY_IN_MILLISECONDS - ( (currentDateTime.Hour*60*60*1000)+(currentDateTime.Minute*60*1000)+(currentDateTime.Second*1000));
                    Thread t = new Thread(() => doWorkProcessUpdate(response.identificadorMensaje, waittime));
                    t.Start();
                }

                _ret = 1;
            }
            else
            {
                _ret = -1;
            }
            return _ret;
        }
        private void updateIVR(string status, Int32 idmanReceived)
        {
            updateIVR(status, idmanReceived, "");
        }
        private static void updateIVR(string status, Int32 idmanReceived, string otherOondition)
        {
            StringBuilder sb = new StringBuilder("UPDATE NOTIFICA.notificaciones_procesadas SET status_IVR ='");
            sb.Append(status).Append("', fecha_act_IVR = GETDATE()").Append(" WHERE id_maniobra_recibida=").Append(idmanReceived);
            if (otherOondition.Length > 1)
            {
                sb.Append(otherOondition);
            }
            log.Debug("updateIVR:" + sb.ToString());
            SQLUtil.executeQuery(sb.ToString());
        }
        private static void doWorkProcessUpdate( Int32 idmanReceived,int waittime)
        {
            Thread.Sleep(waittime);
            updateIVR("EV004", idmanReceived, " AND status_IVR ='EV001'");
        }

    }

}