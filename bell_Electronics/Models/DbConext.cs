using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace bell_Electronics.Models
{
    public class DbConext
    {
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LTTE"].ConnectionString);

        public List<UsersModel> GetUser()
        {
            List<UsersModel> lst = new List<UsersModel>();
            SqlCommand cmd = new SqlCommand("sp_User_CURD", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECT");
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new UsersModel
                {
                    id = Convert.ToInt32(dr[0]),
                    Name = Convert.ToString(dr[1]),
                    MobileNo = Convert.ToString(dr[2]),
                    Email = Convert.ToString(dr[3]),
                    UserName = Convert.ToString(dr[4]),
                    Password = Convert.ToString(dr[5])
                });
            }
            return lst;
        }
    }
}