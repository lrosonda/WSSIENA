using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinessInterruptionsPerMonth : BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RespuestaNotificaciones notificacionesCorteTODAS()
        {
            log.Debug("Welcome!!");
            RespuestaNotificaciones resp = new RespuestaNotificaciones();

            try
            {
                Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_PAG_WEB_ENSA");
                string status = (string)keyValuesProcess["status"];
                log.Debug("status:" + status);
                if (status.Equals("NT001"))
                {
                    runProcess(resp, keyValuesProcess, null);
                }
                else
                {
                    errorFillREsponse(resp, "Error, webservice de la pagina web de ENSA inactivo", -301);
                }
            }
            catch (SQLUtilException e)
            {
                errorFillREsponse(resp, "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                errorFillREsponse(resp, "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
                log.Error(e.Message);
            }
            log.Debug("resp: " + resp.ToString());
            log.Debug("Bey!!");
            return resp;
        }

        public RespuestaCorteNAC notificacionesCorteNAC(SolicitudCorteNAC solicitudCorteNAC)
        {
            RespuestaCorteNAC respuestaCorteNAC = new RespuestaCorteNAC();
            log.Debug("Welcome!!");
            log.Debug("solicitudCorteNAC:" + solicitudCorteNAC.ToString());
            try
            {
                if (solicitudCorteNAC != null && solicitudCorteNAC.NAC != null && solicitudCorteNAC.NAC.Length > 0)
                {
                    Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_PAG_WEB_ENSA");
                    string status = (string)keyValuesProcess["status"];
                    log.Debug("status:" + status);
                    if (status.Equals("NT001"))
                    {
                        runProcess(respuestaCorteNAC, keyValuesProcess, solicitudCorteNAC);
                    }
                    else
                    {
                        errorFillREsponse(respuestaCorteNAC, "Error, webservice de la pagina web de ENSA inactivo", -301);
                    }
                }
                else
                {
                    errorFillREsponse(respuestaCorteNAC, "Error, el parametro de entrada del área no puede ser nulo", -303);
                }
            }
            catch (SQLUtilException e)
            {
                errorFillREsponse(respuestaCorteNAC, "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                errorFillREsponse(respuestaCorteNAC, "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
                log.Error(e.Message);
            }

            log.Debug("respuestaCorteNAC: " + respuestaCorteNAC.ToString());
            log.Debug("Bey!!");
            return respuestaCorteNAC;
        }

        public RespuestaNotificaciones notificacionesCorteAREA(SolicitudCorteAREA solicitudCorteAREA)
        {
            log.Debug("Welcome!!");
            log.Debug("solicitudCorteAREA:" + solicitudCorteAREA.ToString());
            RespuestaNotificaciones resp = new RespuestaNotificaciones();
            try
            {
                if (solicitudCorteAREA != null && solicitudCorteAREA.areaAfectada != null && solicitudCorteAREA.areaAfectada.Length > 0)
                {
                    Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_PAG_WEB_ENSA");
                    string status = (string)keyValuesProcess["status"];
                    log.Debug("status:" + status);
                    if (status.Equals("NT001"))
                    {
                        runProcess(resp, keyValuesProcess, solicitudCorteAREA);
                    }
                    else
                    {
                        errorFillREsponse(resp, "Error, webservice de la pagina web de ENSA inactivo", -301);
                    }
                }
                else
                {
                    errorFillREsponse(resp, "Error, el parametro de entrada del área no puede ser nulo", -303);
                }
            }
            catch (SQLUtilException e)
            {
                errorFillREsponse(resp, "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                errorFillREsponse(resp, "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
                log.Error(e.Message);
            }
            log.Debug("resp: " + resp.ToString());
            log.Debug("Bey!!");
            return resp;
        }
        private void errorFillREsponse(object response, string message, Int32 codMessage)
        {
            //RespuestaCorteNAC
            //RespuestaNotificaciones
            Encabezado encabezado = new Encabezado();
            encabezado.codigoMensaje = codMessage;
            encabezado.mensajeRespuesta = message;
            encabezado.totalEnviada = 0;
            if (response.GetType().Equals("RespuestaNotificaciones"))
            {
                RespuestaNotificaciones _resp = new RespuestaNotificaciones();
                Detalle[] _detalle = new Detalle[0];
                _resp.detalle = _detalle;
                _resp.encabezado = encabezado;
                response = (RespuestaNotificaciones)_resp;
            }
            else if (response.GetType().Equals("RespuestaCorteNAC"))
            {
                RespuestaCorteNAC _resp = new RespuestaCorteNAC();
                DetalleNAC[] detalleNAC = new DetalleNAC[0];
                _resp.encabezado = encabezado;
                _resp.detalle = detalleNAC;
                response = (RespuestaCorteNAC)_resp;
            }

        }
        private void runProcess(object resp, Dictionary<String, Object> keyValuesProcess, object resquest)
        {
            Int32 idProcess = (Int32)keyValuesProcess["id_proceso"];
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni = (DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe = (String)keyValuePairs["dias"];
            string iniHour = (String)keyValuePairs["hora_inicial"];
            string endHour = (String)keyValuePairs["hora_fin"];
            string sql = (String)keyValuesProcess["sql"];
            DateTime currentDateTime = DateTime.Now;

            int comparetoDate = currentDateTime.CompareTo(dateIni);
            if (comparetoDate >= 0 && !keyValuePairs.ContainsKey("fecha_vencimiento"))
            {
                if (validateExecutionDays(strDayExe, currentDateTime))
                {
                    if (iniHour != null && endHour != null)
                    {
                        if (!validateExecutionHours(iniHour, endHour, currentDateTime))
                        {
                            buildsMessageResponse(idProcess, sql, resp, resquest);
                        }
                        else
                        {
                            string messageErr = "Error, webservice de ENSA fuera rango de ejecucución: [" + iniHour + "-" + endHour + "]";
                            errorFillREsponse(resp, messageErr, -303);
                        }
                    }
                    else
                    {
                        buildsMessageResponse(idProcess, sql, resp, resquest);
                    }

                }
                else
                {
                    string messageErr = "Error, hoy no se ejecuta el servicio web de la pag web de ENSA.Los dias que se ejecuta son: " + strDayExe;
                    errorFillREsponse(resp, messageErr, -302);
                }
            }
            else
            {
                errorFillREsponse(resp, "Error, webservice de la pagina web de ENSA inactivo", -301);
            }
        }


        private void settingNotificaciones(RespuestaNotificaciones resp,string str)
        {
            string[] columms = { "areaAfectada", "fechaAfetacion", "horaInicioAfectacion", "horaFinAfectacion", "cantidadClienteAfectado", "horasAfectacion", "duracion" };
            List<Dictionary<String, Object>> maneuversReceived = SQLUtil.getQueryResultList(str, columms);
            if (maneuversReceived.Count() > 0)
            {
                Encabezado encabezado = new Encabezado();
                encabezado.codigoMensaje = 0;
                encabezado.mensajeRespuesta = "Operación realizada de forma exitosa";
                encabezado.totalEnviada = maneuversReceived.Count();

                Detalle[] _detalle = new Detalle[maneuversReceived.Count()];
                int i = 0;
                foreach (Dictionary<String, Object> m in maneuversReceived)
                {
                    Detalle d = new Detalle();
                    d.areaAfectada = (string)m["areaAfectada"];
                    d.fechaAfetacion = (string)m["fechaAfetacion"];
                    d.horaInicioAfectacion = (int)m["horaInicioAfectacion"];
                    d.horaFinAfectacion = (int)m["horaFinAfectacion"];
                    d.cantidadClienteAfectado = (int)m["cantidadClienteAfectado"];
                    d.fechaAfetacion = (string)m["horasAfectacion"];
                    d.duracion = (string)m["duracion"];
                    _detalle[i] = d;
                    i++;
                }
                resp.detalle = _detalle;
                resp.encabezado = encabezado;
            }
            else
            {
                errorFillREsponse(resp, "No hay registros", 200);
            }

        }

        private void settingNotificacionesNac(RespuestaCorteNAC resp, string str)
        {
            string[] columms = { "areaAfectada", "fechaAfetacion", "horaInicioAfectacion", "horaFinAfectacion", "horasAfectacion", "duracion" };
            List<Dictionary<String, Object>> maneuversReceived = SQLUtil.getQueryResultList(str, columms);
            if (maneuversReceived.Count() > 0)
            {
                Encabezado encabezado = new Encabezado();
                encabezado.codigoMensaje = 0;
                encabezado.mensajeRespuesta = "Operación realizada de forma exitosa";
                encabezado.totalEnviada = maneuversReceived.Count();

                DetalleNAC[] _detalle = new DetalleNAC[maneuversReceived.Count()];
                int i = 0;
                foreach (Dictionary<String, Object> m in maneuversReceived)
                {
                    DetalleNAC d = new DetalleNAC();
                    d.areaAfectada = (string)m["areaAfectada"];
                    d.fechaAfetacion = (string)m["fechaAfetacion"];
                    d.horaInicioAfectacion = (int)m["horaInicioAfectacion"];
                    d.horaFinAfectacion = (int)m["horaFinAfectacion"];
                    d.fechaAfetacion = (string)m["horasAfectacion"];
                    d.duracion = (string)m["duracion"];
                    _detalle[i] = d;
                    i++;
                }
                resp.detalle = _detalle;
                resp.encabezado = encabezado;
            }
            else
            {
                errorFillREsponse(resp, "No hay registros", 200);
            }

        }
        private void buildsMessageResponse(Int32 idProcess, string sql, object resp, object resquest)
        {
            List<Dictionary<String, Object>> filters = getFiltersByProcessId(idProcess);
            StringBuilder sbFilters = new StringBuilder();
            StringBuilder sb = new StringBuilder(sql);

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

            if (resquest != null)
            {
                //sbFilters.Append("'").Append(resquest.tipo).Append("'");
                if (resquest.GetType().Equals("SolicitudCorteAREA"))
                {
                    SolicitudCorteAREA _req = (SolicitudCorteAREA)resquest;
                    sbFilters.Append(" AND UPPER(purpose)=UPPER('").Append(_req.areaAfectada).Append("') ");
                }
                else if (resquest.GetType().Equals("SolicitudCorteNAC"))
                {
                    SolicitudCorteNAC _req = (SolicitudCorteNAC)resquest;
                    sbFilters.Append(" AND UPPER(switchingPlan_mRID)=UPPER('").Append(_req.NAC).Append("') ");
                }
            }

            if (sbFilters.Length > 0)
            {
                sb.Replace("'CORTE'", sbFilters.ToString());
            } 
            log.Debug("SQL maneuversReceived:" + sb.ToString());
            if (resp.GetType().Equals("RespuestaNotificaciones")) {
                settingNotificaciones((RespuestaNotificaciones)resp, sb.ToString());
            }
            else if (resp.GetType().Equals("RespuestaCorteNAC"))
            {
                settingNotificacionesNac((RespuestaCorteNAC )resp, sb.ToString());
            }

        }

    }
}