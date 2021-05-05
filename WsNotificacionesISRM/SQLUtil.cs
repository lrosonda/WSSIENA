using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WsNotificacionesISRM
{
    public class SQLUtilException : Exception
    {
        public SQLUtilException(string message)
           : base(message)
        {
        }
    }
    public class SQLUtil
    {
        public const int OK = 100;
        public const int COMMUNICATION_ERROR = -100;
        public const int EXECUTION_TO_BD_ERROR = -1500;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Dictionary<String, Object> getQueryResult(string query, string[] columms)
        {
            Dictionary<String, Object> d = new Dictionary<String, Object>();
            try
            {
                using (SqlConnection cnxtar = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SIENA_NOTIF"].ToString()))
                {
                    String SqlActiontar = query;
                    using (SqlCommand cmdtar = new SqlCommand(SqlActiontar, cnxtar))
                    {
                        cnxtar.Open();
                        SqlDataAdapter datar = new SqlDataAdapter(cmdtar);
                        DataTable dttar = new DataTable();
                        datar.Fill(dttar);
                        DataRow rowtar = dttar.Rows[0];
                        foreach (string key in columms)
                        {
                            d.Add(key, rowtar[key]);
                        }
                        cnxtar.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Query error :[" + query + "] in " + ex.ToString());
                throw new SQLUtilException("Exception in getQueryResult..");
            }
            return d;
        }
        public static int executeQueryParams(string query, Dictionary<string, Object> pars)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SIENA_NOTIF"].ToString()))
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        foreach (KeyValuePair<string, object> item in pars)
                        {
                            command.Parameters.AddWithValue(item.Key, item.Value);
                        }
                        connection.Open();
                        int valor = command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Query error :[" + query + "] in " + ex.ToString());
                throw new SQLUtilException("Exception in executeQueryParams..");
            }
            return OK;
        }
        public static int executeQuery(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(
                ConfigurationManager.ConnectionStrings["SIENA_NOTIF"].ToString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                log.Error("Query error :[" + query + "] in " + ex.ToString());
                throw new SQLUtilException("Exception in executeQuery..");
            }
            return OK;
        }
        public static List<Dictionary<String, Object>> getQueryResultList(string query, string[] columms) {
            List<Dictionary<String, Object>> keyValuePairs = new List<Dictionary<string, object>>();
            try
            {
                using (SqlConnection cnxtar = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SIENA_NOTIF"].ToString()))
                {
                    String SqlActiontar = query;
                    using (SqlCommand cmdtar = new SqlCommand(SqlActiontar, cnxtar))
                    {
                        cnxtar.Open();
                        SqlDataAdapter datar = new SqlDataAdapter(cmdtar);
                        DataTable dttar = new DataTable();
                        datar.Fill(dttar);
                        int count = dttar.Rows.Count;
                        if (count <= 0)
                            return keyValuePairs;
                        foreach (DataRow row in dttar.Rows)
                        {
                            Dictionary<String, Object> d = new Dictionary<String, Object>();
                            foreach (DataColumn column in dttar.Columns)
                            {
                                d.Add(column.ColumnName, row[column]);
                            }
                            keyValuePairs.Add(d);
                        }

                        cnxtar.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Query error :[" + query + "] in " + ex.ToString());
                throw new SQLUtilException("Exception in getQueryResultList..");
            }
            return keyValuePairs;
        }
    }
}