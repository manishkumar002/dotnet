
    using System;
    using System.Data;
    using System.Configuration;
    using System.Collections;
    using System.Data.SqlClient;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Linq;
    using System.Text.RegularExpressions;

    //103.30.72.30
    //WIN-BP4T7KE8CA1
    public class Database
    {
        public static string _WF_APPLICATION = "";
        public static string _WF_APPLICATION2 = "";
        //public static string _WF_APPLICATION = "Data Source=103.30.72.30; Initial Catalog=SMARTACCESS_MASTER; persist security info=false; User ID=sa; Password=Admin@2455; Integrated Security=False;MultipleActiveResultSets=true;;Connection Timeout=0";
        public static string _WF_MASTER = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        public static string apiUrl = ConfigurationManager.ConnectionStrings["apiUrl"].ConnectionString;
        public static string OFFICEPUNCH;
        //public static string _WF_MASTERDB = "Data Source=192.168.1.2; Initial Catalog=CQUICK_MASTER_V1; persist security info=false; User ID=sa; Password=Admin@2455; Integrated Security=False;MultipleActiveResultSets=true;;Connection Timeout=0";
        //public static string _WF_APPLICATION = DapperUtitlity._WF_APPLICATION;
        private static Hashtable parameterCacheTable = Hashtable.Synchronized(new Hashtable());
        public Database()
        {
            string servername = "", DBname = "";
            //string s = Application.StartupPath + "\\SERVER.cfg";
            string s = HttpContext.Current.Server.MapPath("~/SERVER.cfg");
            string[] lines;
            var list = new List<string>();
            if (File.Exists(s))
            {
                var fileStream = new FileStream(s, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Substring(0, 6).ToUpper() == "SERVER")
                        { servername = line.Split('=')[1]; }
                        if (line.Substring(0, 7).ToUpper() == "DB NAME")
                        { DBname = line.Split('=')[1]; }
                        list.Add(line);
                    }
                }
                lines = list.ToArray();
                //_WF_APPLICATION = "Data Source=" + servername + "; Initial Catalog=" + DBname + "; persist security info=false; User ID=sa; Password=Akash@bonito#1; Integrated Security=False;";
                //_WF_APPLICATION = "Data Source=" + servername + "; Initial Catalog=" + DBname + "; persist security info=false; User ID=sa; Password=Akash@bonito#1; Integrated Security=False;";
                _WF_APPLICATION = "Data Source=" + servername + "; Initial Catalog=" + DBname + "; persist security info=false; User ID=sa; Password=Admin@2455; Integrated Security=False;";


                _WF_APPLICATION2 = "Data Source=" + servername + "; Initial Catalog=" + DBname + "; persist security info=false; User ID=sa; Password=Admin@2455; Integrated Security=False;";


            }
        }

        public static List<Dictionary<string, object>> ConvertDatatableToJson(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }
        public static int ExecuteNonQuery(string conectionString, string commandText, SqlParameter[] commandParms)
        {
            SqlCommand command = new SqlCommand();
            using (SqlConnection con = new SqlConnection(conectionString))
            {
                PrepareCommand(command, con, null, commandText, commandParms);
                command.CommandTimeout = 300; // required for large mail like 2Mb
                int val = command.ExecuteNonQuery();
                command.Parameters.Clear();
                return val;
            }
        }
        public static int ExecuteText1(string connectionString, string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);  // Add parameters to the command
                }

                conn.Open();
                return cmd.ExecuteNonQuery();  // Executes the query and returns the number of rows affected
            }
        }
        public static int ExecuteText(string conectionString, string commandText)
        {
            SqlCommand command = new SqlCommand();
            using (SqlConnection con = new SqlConnection(conectionString))
            {
                PrepareCommandText(command, con, null, commandText);
                command.CommandTimeout = 100;
                int val = command.ExecuteNonQuery();
                command.Parameters.Clear();
                return val;
            }
        }
        public static int ExecuteNonQuery(SqlTransaction trans, string commandText, SqlParameter[] commandParms)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, trans.Connection, trans, commandText, commandParms);
            int val = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return val;
        }
        public static int ExecuteNonQuery(SqlConnection SqlCon, string commandText, SqlParameter[] commandParms)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, SqlCon, null, commandText, commandParms);
            int val = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return val;
        }
        public static SqlDataReader ExecuteReader(string conectionString, string commandText, SqlParameter[] commandParms)
        {
            SqlCommand command = new SqlCommand();
            SqlConnection con = new SqlConnection(conectionString);
            try
            {
                PrepareCommand(command, con, null, commandText, commandParms);
                SqlDataReader rdr = command.ExecuteReader(CommandBehavior.CloseConnection);
                command.Parameters.Clear();
                return rdr;
            }
            catch (Exception e)
            {
                con.Close();
                throw e;
            }
        }
        public static SqlDataReader ExecuteReader(string conectionString, string commandText)
        {
            SqlCommand command = new SqlCommand();
            SqlConnection con = new SqlConnection(conectionString);
            try
            {
                // PrepareCommand(command, con, null, commandText, commandParms);+
                con.Open();
                command = new SqlCommand(commandText, con);
                SqlDataReader rdr = command.ExecuteReader(CommandBehavior.CloseConnection);
                // command.Parameters.Clear();
                return rdr;
            }
            catch (Exception e)
            {
                con.Close();
                throw e;
            }
        }
        public static DataTable RemoveSpaceWithUnderscore(DataTable dt)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Replace(' ', '_').Replace('(', '_').Replace(')', '_').Replace('/', '_');
            }
            return dt;
        }
        public static SqlDataReader ExecuteBLOBReader(string conectionString, string commandText, SqlParameter[] commandParms)
        {
            SqlCommand command = new SqlCommand();
            SqlConnection con = new SqlConnection(conectionString);
            try
            {
                PrepareCommand(command, con, null, commandText, commandParms);
                SqlDataReader rdr = command.ExecuteReader(CommandBehavior.SequentialAccess);
                command.Parameters.Clear();
                return rdr;
            }
            catch (Exception e)
            {
                con.Close();
                throw e;
            }
        }
        public static object ExecuteScalar(string conectionString, string commandText, SqlParameter[] commandParameter)
        {
            SqlCommand command = new SqlCommand();
            using (SqlConnection con = new SqlConnection(conectionString))
            {
                PrepareCommand(command, con, null, commandText, commandParameter);
                object val = command.ExecuteScalar();
                command.Parameters.Clear();
                return val;
            }
        }
        public static object ExecuteTextScalar(string conectionString, string commandText)
        {
            SqlCommand command = new SqlCommand();
            using (SqlConnection con = new SqlConnection(conectionString))
            {
                PrepareCommandText(command, con, null, commandText);
                object val = command.ExecuteScalar();
                command.Parameters.Clear();
                return val;
            }
        }
        public static DataTable ExecuteDataTable(string connectionString, string commandtext, SqlParameter[] commandParmeter)
        {
            SqlCommand command = new SqlCommand();
            DataTable dataTable = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                PrepareCommand(command, con, null, commandtext, commandParmeter);
                command.CommandTimeout = 300;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                command.Parameters.Clear();
                return dataTable;
            }
        }
        public static DataTable ExecuteTextDataTable(string connectionString, string commandtext)
        {
            SqlCommand command = new SqlCommand();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    PrepareCommandText(command, con, null, commandtext);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    command.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
            }
            return dataTable;
        }


        public static DataTable ExecuteTextDataTable1(string connectionString, string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddRange(parameters);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }


        public static DataSet ExecuteTextDataset(string connectionString, string commandtext)
        {
            SqlCommand command = new SqlCommand();
            DataSet dataTable = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                PrepareCommandText(command, con, null, commandtext);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);
                command.Parameters.Clear();
                return dataTable;
            }
        }
        public static DataSet ExecuteDataSet(string connectionString, string commandText, SqlParameter[] commandparameter)
        {
            SqlCommand command = new SqlCommand();
            DataSet dataSet = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                PrepareCommand(command, con, null, commandText, commandparameter);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
                command.Parameters.Clear();
                return dataSet;
            }
        }
        public static void CacheParameters(string spName, SqlParameter[] parameter)
        {
            parameterCacheTable[spName] = parameter;
        }
        public static SqlParameter[] GetCacheParameter(string spName)
        {
            SqlParameter[] cacheParameter = (SqlParameter[])parameterCacheTable[spName];
            if (cacheParameter == null)
                return null;
            SqlParameter[] clonedParameter = new SqlParameter[cacheParameter.Length];
            for (int i = 0; i < cacheParameter.Length; i++)
                clonedParameter[i] = (SqlParameter)((ICloneable)cacheParameter[i]).Clone();
            return clonedParameter;
        }
        public static SqlParameter MakeInParameter(string parametername, SqlDbType dbType, int size)
        {
            return MakeParam(parametername, dbType, size, ParameterDirection.Input);
        }
        public static SqlParameter MakeOutParameter(string parametername, SqlDbType dbType, int size)
        {
            return MakeParam(parametername, dbType, size, ParameterDirection.Output);
        }
        private static SqlParameter MakeParam(string parametername, SqlDbType dbType, int size, ParameterDirection direction)
        {
            SqlParameter parameter;
            if (size > 0)
                parameter = new SqlParameter(parametername, dbType, size);
            else
                parameter = new SqlParameter(parametername, dbType);
            parameter.Direction = direction;
            return parameter;
        }
        private static void PrepareCommand(SqlCommand command, SqlConnection con, SqlTransaction trans, string commandText, SqlParameter[] commandParameter)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                command.Connection = con;
                command.CommandText = commandText;
                if (trans != null)
                    command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;
                if (commandParameter != null)
                {
                    foreach (SqlParameter parm in commandParameter)
                        command.Parameters.Add(parm);
                }
            }
            catch (Exception ee)
            {
            }
        }
        public static string RemoveSql(string strValue)
        {
            string strremove = "";
            strremove = strValue.Replace("'", "");
            string[] NotAllowed = { ";", "--", "xp_", "*", "<", ">", "[", "]", "(", ")", "select", "union", "drop", "insert", "delete", "update" };
            if (strremove != "")
            {
                for (int i = 0; i < NotAllowed.Length; i++)
                {
                    strremove = strremove.Replace(NotAllowed[i], "").Trim();
                }
            }
            else
            {
                strremove = "";
            }
            return strremove;
        }


        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static System.Boolean IsNumeric(System.Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;
            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;
            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            }
            catch { } // just dismiss errors but return false
            return false;
        }
        private static void PrepareCommandText(SqlCommand command, SqlConnection con, SqlTransaction trans, string commandText)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                command.Connection = con;
                command.CommandText = commandText;
                if (trans != null)
                    command.Transaction = trans;
                command.CommandType = CommandType.Text;
            }
            catch (Exception ex)
            { }
        }
        public static DataTable Report(string fromDate, string toDate, string employeeId, string procedureName, string Database_WF_APPLICATION)
        {
            if (procedureName.StartsWith("DAILY"))
            {
                DataTable dt = Database.ExecuteTextDataTable(Database_WF_APPLICATION, "EXEC " + procedureName + "'" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd") + "','" + employeeId + "'");
                return dt;
            }
            else
            {
                DataTable dt = Database.ExecuteTextDataTable(Database_WF_APPLICATION, "EXEC " + procedureName + " '" + Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd") + "','" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd") + "','" + employeeId + "'");
                return dt;
            }

        }
        public static DataTable ReportBalance(string fromDate, string toDate, string employeeId, string procedureName, string Database_WF_APPLICATION)
        {

            DataTable dt = Database.ExecuteTextDataTable(Database_WF_APPLICATION, "EXEC " + procedureName + "'" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd") + "','" + employeeId + "'");
            return dt;

        }

        public static DataTable Reports(string toDate, string employeeId, string procedureName, string Database_WF_APPLICATION)
        {
            {
                DataTable dt = Database.ExecuteTextDataTable(Database_WF_APPLICATION, "EXEC " + procedureName + "'" + Convert.ToDateTime(toDate).ToString("yyyy-MM-dd") + "','" + employeeId + "'");
                return dt;
            }
        }
        public static void SendMail(string toemail, string subject, string body, Attachment attachmentFilePath)
        {
            using (IDbConnection _db = new SqlConnection(Database._WF_APPLICATION))
            {
                try
                {

                    MailMessage mm = new MailMessage("bbioroles@gmail.com", toemail);

                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    mm.Subject = subject;
                    mm.Body = body;
                    mm.BodyEncoding = System.Text.Encoding.UTF8;
                    body = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'>" + body;
                    mm.SubjectEncoding = System.Text.Encoding.Default;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    mm.Attachments.Add(attachmentFilePath);                               // smtp.EnableSsl = true;
                                                                                          //smtp.EnableSsl = false;
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = "bbioroles@gmail.com";
                    NetworkCred.Password = "tjcf suaw qpbq ypdo";
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {


                }
            }
        }
    }
