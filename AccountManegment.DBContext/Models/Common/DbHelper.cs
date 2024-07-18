using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.Common
{
    public class DbHelper
    {
        public static DataSet GetDataSet(string cmdText, CommandType cmdType, SqlParameter[] parameters, string ConnectionString)
        {
            try
            {
                string conString = ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = cmdType;
                        if (parameters != null)
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                if (null != parameter) cmd.Parameters.Add(parameter);
                            }
                        }


                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            con.Close();
                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
