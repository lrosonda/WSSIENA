using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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
        protected Dictionary<String, Object> retrieveNotificationProcesses(string nameProcess)
        {
            StringBuilder sb = new StringBuilder("SELECT id_proceso, status, fecha_actualizacion, repetir_cada, repetir_x, sql");
            sb.Append(" FROM NOTIFICA.procesos_notificaciones  WHERE nombre_proceso='").Append(nameProcess).Append("'");
            string[] columms = { "id_proceso", "status", "fecha_actualizacion", "repetir_cada", "repetir_x", "sql" };
            log.Debug("SQL01:" + sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        protected Dictionary<String, Object> getProcessParameters(Int32 iDprocess)
        {
            StringBuilder sb = new StringBuilder("SELECT fecha_inicial, fecha_vencimiento, dias, hora_inicial, hora_fin FROM NOTIFICA.parametros_procesos WHERE id_proceso =");
            sb.Append(iDprocess);
            string[] columms = { "fecha_inicial", "dias", "hora_inicial", "hora_fin" };
            log.Debug("SQL02:" + sb.ToString());
            return SQLUtil.getQueryResult(sb.ToString(), columms);
        }

        protected List<Dictionary<String, Object>> getFiltersByProcessId(Int32 idProcess) {
            StringBuilder sb = new StringBuilder("SELECT filtro,id_proc_dep FROM NOTIFICA.filtros WHERE id_proceso =");
            sb.Append(idProcess);
            string[] columms = { "filtro", "id_proc_dep" };
            return SQLUtil.getQueryResultList(sb.ToString(), columms);
        }

        protected bool validateExecutionDays(string strDayExe, DateTime cDateTime)
        {
            if (strDayExe.Equals("TODOS"))
            {
                return true;
            }
            else
            {
                string[] split = strDayExe.Split(new Char[] { ';' });
                foreach (string sDay in split)
                {
                    String _sDay = sDay.ToUpper();
                    if (_sDay.Equals("LUNES") && cDateTime.DayOfWeek == DayOfWeek.Monday)
                    {
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

        protected bool validateExecutionHours(string iniHour, string endHour, DateTime cDateTime)
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

                    if ((h >= hIniParam && m >= mIniParam && s >= sParam)
                        && ((h < hEndParam) || (h == hEndParam && m < mEndParam) ||
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
    }
}