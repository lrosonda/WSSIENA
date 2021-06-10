using System;
using System.Collections.Generic;
using System.Text;
using WsNotificacionesISRM.DTO;

namespace WsNotificacionesISRM.Business
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
    public class BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static int A_DAY_IN_MILLISECONDS = 1000 * 60 * 60 * 24;
        protected Dictionary<String, Object> retrieveNotificationProcesses(string nameProcess)
        {
            StringBuilder sb = new StringBuilder("SELECT id_proceso, status, fecha_actualizacion, repetir_cada, repetir_x, sql");
            sb.Append(" FROM NOTIFICA.procesos_notificaciones  WHERE nombre_proceso='").Append(nameProcess).Append("'");
            string[] columms = { "id_proceso", "status", "fecha_actualizacion", "repetir_cada", "repetir_x", "sql" };
            log.Debug("SQL01:" + sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        protected static Dictionary<String, Object> getProcessParameters(Int32 iDprocess)
        {
            StringBuilder sb = new StringBuilder("SELECT fecha_inicial, fecha_vencimiento, hora_inicial, hora_fin ,(CASE dias WHEN   'TODOS' THEN  'OK' ELSE CASE  DATEPART(weekday,GETDATE()) WHEN 1 THEN CASE dia_domingo WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 2 THEN CASE dia_lunes WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 3 THEN CASE dia_martes WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 4 THEN CASE dia_miercoles WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 5 THEN CASE dia_jueves WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 6 THEN CASE dia_viernes WHEN 'TRUE' THEN 'OK' ELSE 'NO' END WHEN 7 THEN CASE dia_sabado WHEN 'TRUE' THEN 'OK' ELSE 'NO' END END END) AS dayExec FROM NOTIFICA.parametros_procesos WHERE id_proceso =");
            sb.Append(iDprocess);
            string[] columms = { "fecha_inicial", "hora_inicial", "hora_fin", "dayExec" };
            log.Debug("SQL02:" + sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        protected List<Dictionary<String, Object>> getFiltersByProcessId(Int32 idProcess)
        {
            StringBuilder sb = new StringBuilder("SELECT filtro,id_proc_dep FROM NOTIFICA.filtros WHERE id_proceso =");
            sb.Append(idProcess);
            string[] columms = { "filtro", "id_proc_dep" };
            return SQLUtil.getQueryResultList(sb.ToString(), columms);
        }

        protected static bool validateExecutionDays(string strDayExe)
        {
            if (strDayExe.Equals("OK"))
            {
                return true;
            }
            return false;
        }

        protected static bool validateExecutionHours(string iniHour, string endHour, DateTime cDateTime)
        {
            int h = cDateTime.Hour;
            int m = cDateTime.Minute;
            int s = cDateTime.Second;
            string[] splitIniHour = iniHour.Split(new Char[] { ':' });
            string[] splitEndHour = endHour.Split(new Char[] { ':' });
            if (!(splitEndHour.Length == 3 && splitIniHour.Length == 3))
            {
                throw new BusinessException("Los parametros de la ejecución del proceso se encuentra errados..");
            }
            else
            {
                try
                {
                    String ssInit = splitIniHour[2];
                    string ssEnd = splitEndHour[2];
                    string[] splitSsInit = ssInit.Split(new Char[] { ' ' });
                    string[] splitSsEnd = ssEnd.Split(new Char[] { ' ' });
                    int hIniParam = Int32.Parse(splitIniHour[0]);
                    int mIniParam = Int32.Parse(splitIniHour[1]);
                    int sParam = 0;
                    int hEndParam = Int32.Parse(splitEndHour[0]);
                    int mEndParam = Int32.Parse(splitEndHour[1]);
                    int sEndParam = 0;
                    if (splitSsInit.Length == 2)
                    {
                        if (splitSsInit[1].Equals("PM"))
                        {
                            hIniParam += 12;
                        }
                        sParam = Int32.Parse(splitSsInit[0]);
                    }
                    else
                    {
                        sParam = Int32.Parse(ssInit);
                    }
                    if (splitSsEnd.Length == 2)
                    {
                        if (splitSsEnd[1].Equals("PM"))
                        {
                            hEndParam += 12;
                        }
                        sEndParam = Int32.Parse(splitSsEnd[0]);
                    }
                    else
                    {
                        sEndParam = Int32.Parse(ssEnd);
                    }

                    if (((h >= hIniParam && m >= mIniParam && s >= sParam)
                        && ((h < hEndParam) || (h == hEndParam && m < mEndParam)) ||
                        (h == hEndParam && m == mEndParam && s <= sEndParam)))
                    {
                        return true;
                    }

                }
                catch (FormatException e)
                {
                    log.Error(e.Message);
                    throw new BusinessException("Los parametros de la ejecución del proceso se encuentra errados..");
                }
            }
            return false;
        }
        protected static void SMTPErrorHandling(SMTP.MailRequest mailRequest, int codError, string codExtern)
        {
            Dictionary<String, Object> fields = new Dictionary<string, object>();
            //"asunto","codRespSOAP","descripcionErrorSOAP","codRespSMTP","descripcionErrorSMTP",codSistemaExterno
            fields.Add("asunto", mailRequest.Subject);
            fields.Add("descripcionErrorSOAP", "");
            fields.Add("codRespSOAP", 0);
            fields.Add("codSistemaExterno", codExtern);
            StringBuilder sb = new StringBuilder();
            foreach (var mail in mailRequest.ToEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("ToEmails", sb.ToString());
            sb.Clear();
            foreach (var mail in mailRequest.CcEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("CcEmails", sb.ToString());
            sb.Clear();
            foreach (var mail in mailRequest.BccEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("BccEmails", sb.ToString());
            switch (codError)
            {
                case -1003:
                    fields.Add("descripcionErrorSMTP", "Error, en la autentificación de usuario del correo");
                    fields.Add("codRespSMTP", -1003);
                    break;
                case -1004:
                    //    Console.WriteLine("\tRecipient not accepted: {0}", ex.Mailbox);
                    fields.Add("descripcionErrorSMTP", "Error, en los datos del destinatario del correo");
                    fields.Add("codRespSMTP", -1004);
                    break;
                case -1005:
                    //   Console.WriteLine("\tSender not accepted: {0}", ex.Mailbox);
                    fields.Add("descripcionErrorSMTP", "Error, en los datos del remitente del correo");
                    fields.Add("codRespSMTP", -1005);
                    break;
                case -1006:
                    //   Console.WriteLine("\tMessage not accepted.");
                    fields.Add("descripcionErrorSMTP", "Error, en los datos del mensaje del correo");
                    fields.Add("codRespSMTP", -1006);
                    break;
                case -1007:
                    fields.Add("descripcionErrorSMTP", "Error, en el protocolo de envio (SMTP) del correo");
                    fields.Add("codRespSMTP", -1007);
                    break;
                case -1010:
                    fields.Add("descripcionErrorSMTP", "Error, en el formato del/los correo(s) a enviar");
                    fields.Add("codRespSMTP", -1010);
                    break;
            }
            insertLogEnvioCorreoExterno(fields);
        }
        protected void SOAPError(SolicitudSistemaExternos req, int codErrorHTTP, string error)
        {
            Dictionary<String, Object> fields = new Dictionary<string, object>();
            //"asunto","codRespSOAP","descripcionErrorSOAP","codRespSMTP","descripcionErrorSMTP",codSistemaExterno
            fields.Add("asunto", req.asunto);
            fields.Add("descripcionErrorSOAP", error);
            fields.Add("codRespSOAP", codErrorHTTP);
            fields.Add("descripcionErrorSMTP", "");
            fields.Add("codRespSMTP", 0);
            fields.Add("codSistemaExterno", req.codigoSiExt);
            fields.Add("ToEmails", req.correosDestinatarios);
            fields.Add("CcEmails", req.correosCC);
            fields.Add("BccEmails", req.correosCO);
            insertLogEnvioCorreoExterno(fields);
        }
        protected static void SaveCorrectMailDelivery(SMTP.MailRequest mailRequest, string cod)
        {
            Dictionary<String, Object> fields = new Dictionary<string, object>();
            //"asunto","codRespSOAP","descripcionErrorSOAP","codRespSMTP","descripcionErrorSMTP",codSistemaExterno
            fields.Add("asunto", mailRequest.Subject);
            fields.Add("descripcionErrorSOAP", "");
            fields.Add("codRespSOAP", 0);
            fields.Add("descripcionErrorSMTP", "Operación realizada de forma exitosa");
            fields.Add("codRespSMTP", 0);
            fields.Add("codSistemaExterno", cod);
            StringBuilder sb = new StringBuilder();
            foreach (var mail in mailRequest.ToEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("ToEmails", sb.ToString());
            sb.Clear();
            foreach (var mail in mailRequest.CcEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("CcEmails", sb.ToString());
            sb.Clear();
            foreach (var mail in mailRequest.BccEmails)
            {
                sb.Append(mail).Append(";");
            }
            fields.Add("BccEmails", sb.ToString());
            insertLogEnvioCorreoExterno(fields);
        }
        private static void insertLogEnvioCorreoExterno(Dictionary<String, Object> fields)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO NOTIFICA.log_envio_correo_externo (asunto,cod_resp_SOAP,descripcion_error_SOAP,cod_resp_SMTP,descripcion_error_SMTP,fecha_registro ,cod_sistema_externo,correos_destino,correos_cc,correos_co) VALUES('");
            sb.Append(fields["asunto"]).Append("',").Append(fields["codRespSOAP"]).Append(",'").Append(fields["descripcionErrorSOAP"]).Append("',").Append(fields["codRespSMTP"]).Append(",'").Append(fields["descripcionErrorSMTP"]).Append("',GETDATE(),'").Append(fields["codSistemaExterno"]).Append("','").Append(fields["ToEmails"]).Append("','").Append(fields["CcEmails"]).Append("','").Append(fields["BccEmails"]).Append("')");
            log.Debug("insertLogEnvioCorreoExterno:" + sb.ToString());
            SQLUtil.executeQuery(sb.ToString());
        }
    }
}