using IniParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WsNotificacionesISRM.DTO;
using WsNotificacionesISRM.SMTP;

namespace WsNotificacionesISRM.Business
{
    public class BusinessExternalSystems : BusinessParent
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IMailService mailService;
        private static readonly string DATA_MAIL = "datamail.ini";
        private static readonly string DISPLAY_NAME = "wsSistemasExternos";

        public BusinessExternalSystems()
        {
            mailService = new MailService(GetMailSettings());
        }
        private MailSettings GetMailSettings()
        {
            MailSettings settings = new MailSettings();
            string pathConfEmailNot = Environment.GetEnvironmentVariable("PATH_CONFIG_EMAIL_NOTIFICACIONES");
            // If necessary, create it.
            if (pathConfEmailNot == null)
            {
                log.Error("La variable PATH_CONFIG_EMAIL_NOTIFICACIONES is null");
                throw new BusinessException("La variable PATH_CONFIG_EMAIL_NOTIFICACIONES is null");
            }
            var parser = new FileIniDataParser();
            StringBuilder sb = new StringBuilder(pathConfEmailNot).Append("/").Append(DATA_MAIL);
            if (!File.Exists(sb.ToString()))
            {
                log.Error("No existe el archivo: " + sb.ToString());
                throw new BusinessException("No existe el archivo: " + sb.ToString());
            }
            var data = parser.ReadFile(sb.ToString());
            settings.Host = data["CONFIGMAIL"]["SMTP"];
            settings.Port = Int32.Parse(data["CONFIGMAIL"]["PUERTOSMTP"]);
            settings.Password = data["CONFIGMAIL"]["PASSWDMAIL"];
            settings.Mail = data["CONFIGMAIL"]["MAILSMTP"];
            settings.SecureSmtp = data["CONFIGMAIL"]["SECURE"];
            settings.DisplayName = DISPLAY_NAME;

            return settings;
        }
        public RespuestaSistemaExternos envioCorreoExterno(SolicitudSistemaExternos solicitud)
        {
            log.Debug("Welcome!!");
            log.Debug("request: " + solicitud.ToString());
            RespuestaSistemaExternos resp = new RespuestaSistemaExternos();
            try
            {
                Dictionary<String, Object> keyValuesProcess = retrieveNotificationProcesses("WS_SISTEMSAS_EXTERNOS");
                string status = (string)keyValuesProcess["status"];
                log.Debug("status:" + status);
                if (status.Equals("NT001"))
                {
                    if (runProcess(keyValuesProcess, solicitud, resp))
                    {
                        resp.mensajeRespuesta = "Operación realizada de forma exitosa";
                        resp.codigoMensaje = 0;
                    }
                }
                else
                {
                    log.Error("Sistema externos ha sido desactivado consultar los parametros notificaciones");
                    resp.mensajeRespuesta = "Error, webservice de sistemas externo se encuentra inactivo";
                    resp.codigoMensaje = -401;
                    SOAPError(solicitud, resp.codigoMensaje, resp.mensajeRespuesta);
                }
            }
            catch (SQLUtilException e)
            {
                log.Error(e.Message);
                resp.mensajeRespuesta = "Error, operación fallida de la base de datos";
                resp.codigoMensaje = -1001;
                SOAPError(solicitud, resp.codigoMensaje, resp.mensajeRespuesta);
            }
            catch (BusinessException e)
            {
                log.Error(e.Message);
                resp.mensajeRespuesta = "Error, parametrización errada en el webservice de sistema externo";
                resp.codigoMensaje = -1002;
                SOAPError(solicitud, resp.codigoMensaje, resp.mensajeRespuesta);
            }
            log.Debug("resp:" + resp.ToString());
            return resp;
        }
        private bool runProcess(Dictionary<String, Object> dProcess, SolicitudSistemaExternos solicitudEnvioMail, RespuestaSistemaExternos resp)
        {
            Int32 idProcess = (Int32)dProcess["id_proceso"];
            Dictionary<String, Object> keyValuePairs = getProcessParameters(idProcess);
            DateTime dateIni = (DateTime)keyValuePairs["fecha_inicial"];
            string strDayExe = (String)keyValuePairs["dayExec"];
            string iniHour = (String)keyValuePairs["hora_inicial"];
            string endHour = (String)keyValuePairs["hora_fin"];

            DateTime currentDateTime = DateTime.Now;

            int comparetoDate = currentDateTime.CompareTo(dateIni);
            if (comparetoDate >= 0 && !keyValuePairs.ContainsKey("fecha_vencimiento"))
            {
                if (validateExecutionDays(strDayExe))
                {
                    if (!validateExecutionHours(iniHour, endHour, currentDateTime))
                    {
                        log.Error("Error, sistemas externos fuera rango de ejecucución: [" + iniHour + "-" + endHour + "]");
                        resp.mensajeRespuesta = "Error, en el webservice de sistemas externo fuera de rango";
                        resp.codigoMensaje = -403;
                        SOAPError(solicitudEnvioMail, resp.codigoMensaje, resp.mensajeRespuesta);
                        return false;
                    }
                    else
                    {
                        if (!validateRequiredFields(solicitudEnvioMail))
                        {
                            resp.mensajeRespuesta = "Error, en los datos del entra del envio del correo del  sistema externo";
                            resp.codigoMensaje = -404;
                            SOAPError(solicitudEnvioMail, resp.codigoMensaje, resp.mensajeRespuesta);
                            return false;
                        }
                        else
                        {
                            Dictionary<String, Object> dcod = new Dictionary<string, object>();
                            dcod.Add("codSistemaExterno", solicitudEnvioMail.codigoSiExt);
                            dcod.Add("description", solicitudEnvioMail.descripcionSiExt);
                            string cod = getExternalCode(dcod);
                            Thread t = new Thread(() => SendMail(solicitudEnvioMail, idProcess, cod));
                            t.Start();
                        }

                    }
                }
                else
                {
                    log.Error("Error, hoy no se ejecuta el ws sistemas externos. Los dias que se ejecuta son: " + strDayExe);
                    resp.mensajeRespuesta = "Error, hoy no se ejecuta el webservice de sistemas externos";
                    resp.codigoMensaje = -402;
                    SOAPError(solicitudEnvioMail, resp.codigoMensaje, resp.mensajeRespuesta);
                    return false;
                }
            }
            else
            {
                log.Error("Sistemas externo No se encuentra activo");
                resp.mensajeRespuesta = "Error, webservice de sistemas externo se encuentra inactivo";
                resp.codigoMensaje = -401;
                SOAPError(solicitudEnvioMail, resp.codigoMensaje, resp.mensajeRespuesta);
                return false;
            }
            return true;
        }
        private bool validateRequiredFields(SolicitudSistemaExternos resquestSistemExtern)
        {
            string nameFile = resquestSistemExtern.path;
            if (nameFile != null && nameFile.Length > 0 && !File.Exists(nameFile))
            {
                log.Warn("El archivo no existe: " + nameFile);
                return false;
            }
            return ((resquestSistemExtern.correosDestinatarios != null && resquestSistemExtern.correosDestinatarios.Length > 0)
            && (resquestSistemExtern.mensaje != null && resquestSistemExtern.mensaje.Length > 0) &&
            (resquestSistemExtern.codigoSiExt != null && resquestSistemExtern.codigoSiExt.Length == 5)
            && (resquestSistemExtern.descripcionSiExt != null && (resquestSistemExtern.descripcionSiExt.Length > 0 && resquestSistemExtern.descripcionSiExt.Length <= 255)));
        }
        private static async void SendMail(SolicitudSistemaExternos resquestSistemExtern, int idProcess, string cod)
        {
            int repeat = resquestSistemExtern.cantidadReenvio;
            int i = 0;
            string[] subsCO = resquestSistemExtern.correosCO.Split(';');
            string[] subsCC = resquestSistemExtern.correosCC.Split(';');
            string[] subsMails = resquestSistemExtern.correosDestinatarios.Split(';');
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = resquestSistemExtern.mensaje;
            mailRequest.Subject = resquestSistemExtern.asunto;
            mailRequest.BccEmails = subsCO.ToList();
            mailRequest.CcEmails = subsCC.ToList();
            mailRequest.ToEmails = subsMails.ToList();
            List<IFormFile> Attachments = null;
            if (File.Exists(resquestSistemExtern.path))
            {
                Attachments = new List<IFormFile>();
                IFormFile formFile;
                using (var fstream = new FileStream(resquestSistemExtern.path, FileMode.Open))
                {
                    var mstream = new MemoryStream();
                    fstream.CopyTo(mstream);
                    var strContentType = getStrContentType(fstream.Name);
                    formFile = new FormFile(mstream, 0, mstream.Length, null, Path.GetFileName(fstream.Name))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = strContentType
                    };
                }

                //  FileStream fs =File.OpenRead(resquestSistemExtern.path);
                Attachments.Add(formFile);

            }
            mailRequest.Attachments = Attachments;
            bool flagExecuteError = true;
            int result = 0;
            do
            {
                try
                {
                    result = await mailService.SendEmailAsync(mailRequest, cod);
                }
                catch (Exception e)
                {
                    log.Warn("Excepcion no controlada: " + e.Message);
                    result = -9999;
                }
                if (result == 0)
                {
                    flagExecuteError = false;
                    SaveCorrectMailDelivery(mailRequest, cod);
                    break;
                }
                else
                {
                    insertRetries(idProcess, result, i);
                }
                i++;
            } while (i < repeat);
            if (flagExecuteError)
            {
                SMTPErrorHandling(mailRequest, result, cod);
            }

        }

        private static string getStrContentType(string file)
        {
            String[] prefix = file.Split('.');
            if (prefix.Length > 1)
            {
                switch (prefix[prefix.Length - 1])
                {
                    case "bz": return "application/x-bzip";
                    case "bz2": return "application/x-bzip2";
                    case "doc": return "application/msword";
                    case "gif": return "image/gif";
                    case "jpg":
                    case "jpeg":
                        return "image/jpeg";
                    case "odt": return "application/vnd.oasis.opendocument.text";
                    case "pdf": return "application/pdf";
                    case "rar": return "application/x-rar-compressed";
                    case "rtf": return "application/rtf";
                    case "tar": return "application/x-tar";
                    case "txt": return "text/plain";
                    case "xls": return "application/vnd.ms-excel";
                    case "xml": return "application/xml";
                    case "zip": return "application/zip";
                    default: return "application/octet-stream";
                }
            }
            return "application/octet-stream";

        }

        private static string getExternalCode(Dictionary<String, Object> dcod)
        {
            string code = (string)dcod["codSistemaExterno"];
            if (ExistExternalCode(code))
            {
                InsertExternalCode(dcod);
            }
            return code;
        }
        private static void InsertExternalCode(Dictionary<String, Object> dcod)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO NOTIFICA.codigos (codigo,descripcion,gruposid) VALUES('");
            sb.Append(dcod["codSistemaExterno"]).Append("','").Append(dcod["description"]).Append("',7)");
            log.Debug("InsertExternalCode:" + sb.ToString());
            SQLUtil.executeQuery(sb.ToString());
        }
        private static bool ExistExternalCode(string code)
        {
            StringBuilder sb = new StringBuilder("SELECT COUNT(1) countCodExt FROM NOTIFICA.codigos WHERE gruposid= 7 AND codigo ='");
            sb.Append(code).Append("'");
            string[] columms = { "countCodExt" };
            Dictionary<String, Object> d = SQLUtil.getQueryResult(sb.ToString(), columms);
            int value = (int)d["countCodExt"];
            return value == 0;
        }
        private static void insertRetries(int idprocess, int err, int tried)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO NOTIFICA.reintentos(reintento,id_proceso,id_error) VALUES(").Append(tried).Append(",").Append(idprocess).Append(",").Append(getIdError(err)).Append(")");
            log.Debug("insertRetries:" + sb.ToString());
            SQLUtil.executeQuery(sb.ToString());
        }
        private static int getIdError(int codError)
        {
            StringBuilder sb = new StringBuilder("SELECT id_error FROM NOTIFICA.errores_sistemas WHERE codigo_error =").Append(codError);
            string[] columms = { "id_error" };
            Dictionary<String, Object> d = SQLUtil.getQueryResult(sb.ToString(), columms);
            int value = (int)d["id_error"];
            return value;
        }

    }
}