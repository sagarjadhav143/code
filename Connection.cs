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

---------------- SignUp------------
    
    protected void btnSignUp_Click(object sender, EventArgs e)
        {
            if(txtUserName.Text==""||txtPassword.Text=="")
            {
                string script = "alert(\"Please Try Again!\");";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
                return;
            }
            else
            {
                string strsql = "select * from LogIn where UserName='"+txtUserName.Text+"' and Password='"+txtPassword.Text+"' and Flag='1'";
                DataTable dt= cn.operationOnDataBase(strsql, 2);
                if (dt.Rows.Count > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('User Add Sucessfully!');", true);
                    HttpCookie un = new HttpCookie("username");
                    un["UName"] = txtUserName.Text;
                    Response.Cookies.Add(un);

                    string url = "Main.aspx";
                    Response.Redirect(url, false);
                }
                else
                {
                    string script = "alert(\"Invalid UserNamw or password!\");";
                    ScriptManager.RegisterStartupScript(this, GetType(),
                                          "ServerControlScript", script, true);
                }
               Clear();
            }
        }

------------ ---------
 private void getNewCustomers()
    {      
        DataTable dt = new DataTable();
        string strsql = "select top 10 Name,MobNo,Email from CRM_Customers where CRM_ClientID=" + UserId + " ORDER BY CRM_CustId DESC ";
        dt = cn.operationOnDataBase(strsql, 3);
        gdvCustomer.DataSource = dt;
        gdvCustomer.DataBind();
    }

    private void CountSms()
    {
        DataTable dt = new DataTable();
        string strsql = "SELECT COUNT(*) as TotalCust from CRM_Customers where CRM_ClientID="+UserId+"";
        dt = cn.operationOnDataBase(strsql, 3);
        string custCnt = dt.Rows[0]["TotalCust"].ToString().Trim();
        if (custCnt == null || custCnt == "") { lblSMSCounter.Text = "0"; }
        else { lblSMSCounter.Text = custCnt; }
    }

-----------------------load

protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            HttpCookie uid = Request.Cookies["userid"];
            if (uid == null)
            {
                string url = "SignUp.aspx";
                Response.Redirect(url, false);
            }
            else
            {
                UserId = uid["UID"];
                CountSms();
                CountCustomer();
                getNewCustomers();
                NewSendSms();
            }
        }
    }
