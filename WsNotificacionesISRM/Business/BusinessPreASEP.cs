using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinessPreASEP : BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RespuestaEnvioPreASEP envioLotePreASEP()
        {
            log.Debug("Welcome!!");
            RespuestaEnvioPreASEP resp = new RespuestaEnvioPreASEP();
            try
            {
                Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_SISTEMA_REGULATORIO");
                string status = (string)keyValuesProcess["status"];
                log.Debug("status:" + status);
                if (status.Equals("NT001"))
                {
                    resp = runProcess(keyValuesProcess);
                }

            }
            catch (SQLUtilException e)
            {
                resp = errorFillREsponse("Error, operación fallida de la base de datos", -1001);
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                resp = errorFillREsponse("Error, parametrización errada en el webservice de la pagina web ENSA", -1002);
                log.Error(e.Message);
            }
            log.Debug("resp: " + resp.ToString());
            return resp;
        }

        private RespuestaEnvioPreASEP runProcess(Dictionary<String, Object> keyValuesProcess)
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
                            return buildsMessageResponse(idProcess, sql);
                        }
                        else
                        {
                            string messageErr = "Error, webservice de ENSA fuera rango de ejecucución: [" + iniHour + "-" + endHour + "]";
                            return errorFillREsponse(messageErr, -203);
                        }
                    }
                    else
                    {
                        return buildsMessageResponse(idProcess, sql);
                    }

                }
                else
                {
                    string messageErr = "Error, hoy no se ejecuta el servicio web de la pag web de ENSA.Los dias que se ejecuta son: " + strDayExe;
                    return errorFillREsponse(messageErr, -202);
                }
            }
            else
            {
                return errorFillREsponse("Error, webservice de la pagina web de ENSA inactivo", -201);
            }

        }

        private RespuestaEnvioPreASEP buildsMessageResponse(Int32 idProcess, string sql)
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

            if (sbFilters.Length > 0)
            {
                sb.Replace("'CORTE'", sbFilters.ToString());
            }

            log.Debug("SQL maneuversReceived:" + sb.ToString());
            return buillnotificationsProcessed(sb.ToString());

        }
        private RespuestaEnvioPreASEP buillnotificationsProcessed(string str)
        {
            RespuestaEnvioPreASEP resp = new RespuestaEnvioPreASEP();
            string[] columms = { "fechaSENDMSG", "MRID_SAP", "MRID_SDPNAC", "MRDIDWORK", "TIPO_MSG", "idNotificacionEnviada" };
            List<Dictionary<String, Object>> maneuversReceived = SQLUtil.getQueryResultList(str, columms);
            if (maneuversReceived.Count() > 0)
            {
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string strDate = dt.ToString("yyMMddhhmm");
                EncabezadoEnvioPreASEP encabezado = new EncabezadoEnvioPreASEP();
                encabezado.codigoMensaje = 0;
                encabezado.mensajeRespuesta = "Operación realizada de forma exitosa";
                encabezado.totalRegistrosEnviados = maneuversReceived.Count();
                encabezado.numeroLote = Convert.ToInt32(strDate);

                DetalleEnvioPreASEP[] _detalles = new DetalleEnvioPreASEP[maneuversReceived.Count()];
                int i = 0;
                foreach (Dictionary<String, Object> m in maneuversReceived)
                {
                    DetalleEnvioPreASEP d = new DetalleEnvioPreASEP();
                    d.fechaSENDMSG = (string)m["fechaSENDMSG"];
                    d.MRID_SAP = (string)m["MRID_SAP"];
                    d.MRID_SDPNAC = (string)m["MRID_SDPNAC"];
                    d.MRDIDWOR = (string)m["MRDIDWORK"];
                    d.TIPO_MSG = (string)m["TIPO_MSG"];
                    String otherUpdate = "num_lote='"+strDate + "'";
                    d.idNotificacionEnviada=(Int32)m["IdentificadorMensaje"];
                    updatePREASEP("ES001", d.idNotificacionEnviada, null, otherUpdate);
                    _detalles[i] = d;
                    i++;
                }
                resp.detalles = _detalles;
                resp.encabezado = encabezado;
                DateTime currentDateTime = DateTime.Now;

                int waittime = A_DAY_IN_MILLISECONDS - ((currentDateTime.Hour * 60 * 60 * 1000) + (currentDateTime.Minute * 60 * 1000) + (currentDateTime.Second * 1000));
                Thread t = new Thread(() => doWorkProcessUpdate(strDate, waittime));
                t.Start();
            }
            else
            {
                resp = errorFillREsponse("No hay registros", 200);
            }
            return resp;
        }
        private RespuestaEnvioPreASEP errorFillREsponse(string message, Int32 codMessage)
        {
            //RespuestaCorteNAC
            //RespuestaNotificaciones
            EncabezadoEnvioPreASEP encabezado = new EncabezadoEnvioPreASEP();
            encabezado.codigoMensaje = codMessage;
            encabezado.mensajeRespuesta = message;
            encabezado.numeroLote = 0;
            encabezado.totalRegistrosEnviados = 0;

            RespuestaEnvioPreASEP _resp = new RespuestaEnvioPreASEP();
            DetalleEnvioPreASEP[] _detalle = new DetalleEnvioPreASEP[0];
            _resp.detalles = _detalle;
            _resp.encabezado = encabezado;
            return _resp;
        }
        public RespuestaConfirmacionPreASEP confirmacionLotePreASEP(SolicitudConfirmacionPreASEP request)
        {
            log.Debug("Welcome!!");
            log.Debug("request: "+ request.ToString());
            RespuestaConfirmacionPreASEP response = processResponse(request);
            log.Debug("response: " + response.ToString());
            return response;
        }
        private RespuestaConfirmacionPreASEP processResponse(SolicitudConfirmacionPreASEP request) {
            RespuestaConfirmacionPreASEP response = new RespuestaConfirmacionPreASEP();
            try {
                DetalleNotificacionFallida[] _detalle = request.detalle;
                foreach (DetalleNotificacionFallida d in _detalle) {
                    string otherUpdate = "mensaje_actualizacion='" + d.mensajeRespuesta + "'";
                    switch (d.codigoMensaje) {
                        case 0:
                           updatePREASEP("ES002", d.idNotificacionEnviada, null, otherUpdate);
                            break;//ok
                        case -211:
                        case -212:
                            updatePREASEP("ES003", d.idNotificacionEnviada, null, otherUpdate);
                            break;//Error
                        default:
                            throw new BusinessException("El codigo devuelto no es reconocido:"+ d.codigoMensaje);
                        
                    }
                }
                response.mensajeRespuesta   =  "Operación realizada de forma exitosa";
                response.codigoMensaje = 0;
            }
            catch (SQLUtilException e)
            {
                response.mensajeRespuesta = "Error, operación fallida de la base de datos";
                response.codigoMensaje = -1001;
                log.Error(e.Message);
            }
            catch (BusinessException e)
            {
                response.mensajeRespuesta = "Error, codigo desconocido en el webservice de PRE-ASEP";
                response.codigoMensaje = -1002;
                log.Error(e.Message);
            }
            return response;
        }
        private static void updatePREASEP(string status, Int32 idmanReceived, string lotNum, string otherUpdate) {
            StringBuilder sb = new StringBuilder("UPDATE NOTIFICA.notificaciones_procesadas SET status_ASEP ='");
            if (otherUpdate.Length > 1)
            {
                sb.Append(status).Append("' ,").Append(otherUpdate).Append(" , fecha_act_ASEP = GETDATE()");
            }
            else
            {
                sb.Append(status).Append("', fecha_act_ASEP = GETDATE()");
            }
            if (idmanReceived > 0)
            {
              sb.Append(" WHERE id_maniobra_recibida=").Append(idmanReceived);
            }
            else {
              sb.Append(" WHERE num_lote='").Append(lotNum).Append("'");
            }
            log.Debug("updatePREASEP:" + sb.ToString());
            SQLUtil.executeQuery(sb.ToString());
        }
        private static void doWorkProcessUpdate(string lotNumber, int waittime)
        {
            Thread.Sleep(waittime);
            updatePREASEP("ES004", 0, lotNumber, "");
        }
    }
}