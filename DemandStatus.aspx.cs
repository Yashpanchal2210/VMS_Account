using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class DemandStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                string MonthYear = System.DateTime.Now.ToString("MMM");
                monthYearPicker.Value = MonthYear;
                BindGridView(MonthYear);
            }

        }
        private void BindGridView(string monthYear)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,DemandDate,SupplyDate FROM Demand WHERE Status=0 AND  CONVERT(VARCHAR(7), DemandDate, 120) = @Month ORDER By Id desc", conn);
                    cmd.Parameters.AddWithValue("@Month", monthYear);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();

                   
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
        private void BindApprovedGridView(string monthYear)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,DemandDate,SupplyDate FROM Demand WHERE Status=1 AND  CONVERT(VARCHAR(7), DemandDate, 120) = @Month ORDER By Id desc", conn);
                    cmd.Parameters.AddWithValue("@Month", monthYear);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    gvapproved.DataSource = dt;
                    gvapproved.DataBind();

                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGridView(monthYearPicker.Value);
        }
        protected void btnApprovedSearch_Click(object sender, EventArgs e)
        {
            BindApprovedGridView(monthYearPicker.Value);
        }
    }
}