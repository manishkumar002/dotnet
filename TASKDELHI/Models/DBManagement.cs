using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace TASKDELHI.Models
{
    public class DBManagement
    {

        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-E0KS7SJ\\SQLEXPRESS;Initial Catalog=TASKDELHI;Integrated Security=True");

        public int ExceuteCUD(string procedure, SqlParameter[] parameters)
        {

            SqlCommand cmd = new SqlCommand(procedure, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            connection.Open();
            int result = cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }
        public DataTable Exceuteselect(string procedure, SqlParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(procedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            SqlDataAdapter sda = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;

        }
    }
}