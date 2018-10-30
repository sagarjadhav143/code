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

--------------
      <connectionStrings>
    <add name="MConnectionString" connectionString="Data Source=.;Initial Catalog=Machine;User ID=sa;Password=123" providerName="System.Data.SqlClient" />
  </connectionStrings>
  ----------------

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



----------------------------------------------------------------
    
    regController----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class RegisterController : Controller
    {
        MachineEntities db = new MachineEntities();
        //
        // GET: /Register/
        public ActionResult SetDataInDatabase()
        {
            return View();
        }
         [HttpPost]
        public ActionResult SetDataInDatabase(LoginPannel model)
        {
            LoginPannel tbl = new LoginPannel();
            tbl.UserName = model.UserName;
            tbl.Password = model.Password;
            db.LoginPannels.Add(tbl);
            db.SaveChanges();
            return View();
        }
         public ActionResult ShowDatabaseForUser()
         {
             var item = db.LoginPannels.ToList();
             return View(item);
         }

         public ActionResult Delete(int id)
         {
             var itm = db.LoginPannels.Where(x => x.Id == id).First();
             db.LoginPannels.Remove(itm);
             db.SaveChanges();
             var itemm= db.LoginPannels.ToList();
             return View("ShowDatabaseForUser",itemm);
         }

         public ActionResult Edit(int id)
         {
             var itm = db.LoginPannels.Where(x => x.Id == id).First();
             return View(itm);
         }
        [HttpPost]
         public ActionResult Edit(LoginPannel model)
         {
             var itm = db.LoginPannels.Where(x => x.Id == model.Id).First();
             itm.UserName = model.UserName;
             itm.Password = model.Password;
             db.SaveChanges();
             return View();
         }
	}
}


--------save
@model WebApplication.Models.LoginPannel
@using (Html.BeginForm())
{
    <div class="=container">
        <div class="form-group">
            <label>User Name</label>
            <input class="form-control" name="UserName" placeholder="Enter User Name" />
        </div>
        <div class="form-group">
            <label>Salary</label>
            <input class="form-control" name="Password" placeholder="Enter Password" />
        </div>
        <div class="button">
            <button>Submit</button>
        </div>
    </div>
}

----------------Show


@model List<WebApplication.Models.LoginPannel>
           <div class="container">
               <table class="table table-bordered">
                   <thead>
                       <tr>
                           <td>UserName</td>
                           <td>Password</td>
                           <td>Action</td>
                       </tr>
                   </thead>
                   <tbody>
                       @for(int i=0;i<Model.Count;i++)
                       {
                           <tr>
                               <td>@Model[i].UserName</td>
                               <td>@Model[i].Password</td>
                               <td>
                                   <a href="@Url.Action("Delete", new { id =Model[i].Id})"><i class="glyphicon glyphicon-trash"></i></a>
                                   <a href="#"><i class="glyphicon glyphicon-edit"></i></a>
                               </td>
                           </tr>
                       }
                   </tbody>
               </table>
           </div>


--------------------Edit

@model WebApplication.Models.LoginPannel
  @using (Html.BeginForm())
{
    <div class="=container">
        <div class="form-group">
            <label>User Name</label>
            @Html.TextBoxFor(x => x.UserName, new { @class="form-control"})
        </div>
        <div class="form-group">
            <label>User Name</label>
            @Html.TextBoxFor(x => x.Password, new { @class = "form-control" })
        </div>
        <div class="form-group">
            <label>Salary</label>
            <input class="form-control" name="Password" placeholder="Enter Password" />
        </div>
        <div class="button">
            <button>Submit</button>
        </div>
    </div>
}

------------------- Model e
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyBusyHotel.Model
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("SuperCategory")]
        public int supCatId { get; set; }
        public SuperCategory SuperCategory { get; set; }

        public string name { get; set; }
        public string type { get; set; }
        public bool isDelete { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }


    }
}


----------------------db context
using EasyBusyHotel.EntityClasses_Payroll;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBusyHotel.DatabaseContext
{
    class DbContexPayroll : DbContext
    {
        public DbContexPayroll()
            : base("EFPayroll")
        {

        }

        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<InsentiveDeduction> InsentiveDeduction { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<PublicHoliday> PublicHoliday { get; set; }
        public DbSet<Shift> Shift { get; set; }
    }
}

------------
remove home
change rout.config
