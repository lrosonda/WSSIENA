using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    resp = (RespuestaNotificaciones)runProcess(keyValuesProcess, null, resp.GetType());
                }
                else
                {
                    resp =(RespuestaNotificaciones) errorFillREsponse(resp.GetType(), "Error, webservice de la pagina web de ENSA inactivo", -301);
                }
            }
            catch (SQLUtilException e)
            {
                resp = (RespuestaNotificaciones)errorFillREsponse(resp.GetType(), "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                resp = (RespuestaNotificaciones)errorFillREsponse(resp.GetType(), "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
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
                     respuestaCorteNAC =(RespuestaCorteNAC)   runProcess( keyValuesProcess, solicitudCorteNAC, respuestaCorteNAC.GetType());
                    }
                    else
                    {
                        respuestaCorteNAC = (RespuestaCorteNAC)errorFillREsponse(respuestaCorteNAC.GetType(), "Error, webservice de la pagina web de ENSA inactivo", -301);
                    }
                }
                else
                {
                    respuestaCorteNAC = (RespuestaCorteNAC)errorFillREsponse(respuestaCorteNAC.GetType(), "Error, el parametro de entrada del área no puede ser nulo", -303);
                }
            }
            catch (SQLUtilException e)
            {
                respuestaCorteNAC = (RespuestaCorteNAC)errorFillREsponse(respuestaCorteNAC.GetType(), "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                respuestaCorteNAC = (RespuestaCorteNAC)errorFillREsponse(respuestaCorteNAC.GetType(), "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
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
                        resp = (RespuestaNotificaciones)runProcess(keyValuesProcess, solicitudCorteAREA, resp.GetType());
                    }
                    else
                    {
                        resp = (RespuestaNotificaciones)errorFillREsponse(resp.GetType(), "Error, webservice de la pagina web de ENSA inactivo", -301);
                    }
                }
                else
                {
                    resp = (RespuestaNotificaciones)errorFillREsponse( resp.GetType(), "Error, el parametro de entrada del área no puede ser nulo", -303);
                }
            }
            catch (SQLUtilException e)
            {
                resp = (RespuestaNotificaciones)errorFillREsponse(resp.GetType(), "Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                resp = (RespuestaNotificaciones)errorFillREsponse(resp.GetType(), "Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
                log.Error(e.Message);
            }
            log.Debug("resp: " + resp.ToString());
            log.Debug("Bey!!");
            return resp;
        }

        private Object errorFillREsponse(Type t, string message, Int32 codMessage)
        {
            //RespuestaCorteNAC
            //RespuestaNotificaciones
            Encabezado encabezado = new Encabezado();
            encabezado.codigoMensaje = codMessage;
            encabezado.mensajeRespuesta = message;
            encabezado.totalEnviada = 0;
            
            if (t.Equals(typeof(RespuestaNotificaciones))  )
            {
                RespuestaNotificaciones _resp = new RespuestaNotificaciones();
                Detalle[] _detalle = new Detalle[0];
                _resp.detalles = _detalle;
                _resp.encabezado = encabezado;
                return _resp;
            }
            else if (t.Equals(typeof(RespuestaCorteNAC)))
            {
                RespuestaCorteNAC _resp = new RespuestaCorteNAC();
                DetalleNAC[] detalleNAC = new DetalleNAC[0];
                _resp.encabezado = encabezado;
                _resp.detalles = detalleNAC;
                // A a = obj as A;
                return _resp;
            }
            return null;
        }
        private Object runProcess( Dictionary<String, Object> keyValuesProcess, object resquest, Type t)
        {
            Int32 idProcess = (Int32)keyValuesProcess["id_proceso"];
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni = (DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe = (String)keyValuePairs["dayExec"];
            string iniHour = (String)keyValuePairs["hora_inicial"];
            string endHour = (String)keyValuePairs["hora_fin"];
            string sql = (String)keyValuesProcess["sql"];
            DateTime currentDateTime = DateTime.Now;
            int comparetoDate = currentDateTime.CompareTo(dateIni);
            if (comparetoDate >= 0 && !keyValuePairs.ContainsKey("fecha_vencimiento"))
            {
                if (validateExecutionDays(strDayExe))
                {
                    if (iniHour != null && endHour != null)
                    {
                        if (validateExecutionHours(iniHour, endHour, currentDateTime))
                        {
                           return buildsMessageResponse(idProcess, sql, resquest,t);
                        }
                        else
                        {
                            string messageErr = "Error, webservice de ENSA fuera rango de ejecucución: [" + iniHour + "-" + endHour + "]";
                            return errorFillREsponse(t, messageErr, -303);
                        }
                    }
                    else
                    {
                        return buildsMessageResponse(idProcess, sql,resquest, t);
                    }

                }
                else
                {
                    string messageErr = "Error, hoy no se ejecuta el servicio web de la pag web de ENSA.Los dias que se ejecuta son: " + strDayExe;
                 return errorFillREsponse(t, messageErr, -302);
                }
            }
            else
            {
                return errorFillREsponse(t, "Error, webservice de la pagina web de ENSA inactivo", -301);
            }

        }


        private RespuestaNotificaciones buillNotificaciones(string str)
        {
            RespuestaNotificaciones resp = new RespuestaNotificaciones();
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
                    d.duracion = Convert.ToString(m["duracion"]);
                    _detalle[i] = d;
                    i++;
                }
                resp.detalles = _detalle;
                resp.encabezado = encabezado;
            }
            else
            {
                resp = (RespuestaNotificaciones) errorFillREsponse(resp.GetType(), "No hay registros", 200);
            }
            return resp;
        }

        private RespuestaCorteNAC buillNotificacionesNac( string str)
        {
            RespuestaCorteNAC resp = new RespuestaCorteNAC();
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
                    d.duracion = Convert.ToString(m["duracion"]);
                    _detalle[i] = d;
                    i++;
                }
                resp.detalles = _detalle;
                resp.encabezado = encabezado;
            }
            else
            {
                resp =(RespuestaCorteNAC) errorFillREsponse(resp.GetType(), "No hay registros", 200);
            }
            return resp;
        }
        private object buildsMessageResponse(Int32 idProcess, string sql, object resquest, Type t)
        {
            List<Dictionary<String, Object>> filters = getFiltersByProcessId(idProcess);
            StringBuilder sbFilters = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
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
                if (resquest is SolicitudCorteAREA)
                {
                    SolicitudCorteAREA _req = (SolicitudCorteAREA)resquest;

                    sbWhere.Append(" WHERE UPPER(purpose)=UPPER('").Append(_req.areaAfectada).Append("') AND ");
                }
                else if (resquest is SolicitudCorteNAC)
                {
                    SolicitudCorteNAC _req = (SolicitudCorteNAC)resquest;
                    sbWhere.Append(" WHERE UPPER(switchingPlan_mRID)=UPPER('").Append(_req.NAC).Append("') AND ");
                }
            }
            if (sbFilters.Length > 0)
            {
                sb.Replace("'CORTE'", sbFilters.ToString());
            }
            if(sbWhere.Length > 0)
            {
                sb.Replace("WHERE", sbWhere.ToString());
            }

            log.Debug("SQL maneuversReceived:" + sb.ToString());
            if (t.Equals(typeof(RespuestaNotificaciones)) ) {
                return buillNotificaciones(sb.ToString());
            }
            else if (t.Equals(typeof(RespuestaCorteNAC)) )
            {
                return buillNotificacionesNac(sb.ToString());
            }
            return null;
        }
      
    }
}