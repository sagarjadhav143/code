using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AspWeb
{
    public class Connection
    {
        public SqlDataAdapter da;
        public SqlDataReader dr;
        public SqlCommand comd;
        public DataTable dt;
        public DataSet ds;
        public ConnectionState state;
        public SqlConnection con;

        SqlConnection getConnection(bool flgforconnection = true)
        {
            string sqlConnection = string.Empty;
            
                sqlConnection = System.Configuration.ConfigurationManager.
        ConnectionStrings["MConnectionString"].ConnectionString;          
            
            return new SqlConnection(sqlConnection);
        }

        public DataTable operationOnDataBase(string query, int queryNo, int timeout = 30)
        {
            da = new SqlDataAdapter();
            dt = new DataTable();
            ds = new DataSet();
            try
            {
                con = getConnection();
                
                    state = con.State;
                    if (state == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                
                //else
                //{
                //    state = con.State;
                //    if (state == ConnectionState.Closed)
                //    {
                //        con.Open();
                //    }
                //}

                switch (queryNo)
                {
                    case 1:
                        /* Insert, Update ,Delete*/
                        try
                        {
                            using (comd = new SqlCommand(query, con))
                            {
                                comd.ExecuteNonQuery();
                            }
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {

                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            con.Close();
                        }
                        return dt;
                        // select
                    case 2:
                        try
                        {
                            using (comd = new SqlCommand(query, con))
                            {
                                using (da = new SqlDataAdapter(comd))
                                {
                                    da.Fill(dt);
                                }
                            }
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            con.Close();
                        }
                        return dt;
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return dt;
        }
    }
}