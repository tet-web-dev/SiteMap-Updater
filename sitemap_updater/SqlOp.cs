using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace sitemap_updater
{
    class SqlOp
    {
        private IConfigurationRoot Config() { 
            var b = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = b.Build();
            return config;
        }
        private string connstr() {
            return Config().GetSection("dbconnstring").Value;
        }

        private string spname() {
            return Config().GetSection("spname").Value;
        }

        public List<string> ExecSpSiteMap(List<string> doc)
        {
            using (SqlConnection conn = new SqlConnection(connstr()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = spname();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {

                        while (rdr.Read())
                        {
                            doc.Add(rdr[0].ToString());
                        }
                    }
                    conn.Close();
                }
            }
            return doc;
        }
    }
}
