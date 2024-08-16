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
    public partial class ByYardIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                string MonthYear = System.DateTime.Now.ToString("yyyy-MM");
                SubmitButton.Visible = false;
                monthYearPicker.Value = MonthYear;
                BindGridView(MonthYear);
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    foreach (GridViewRow item  in GridViewRationScale.Rows)
                    {
                        TextBox txtQty = (TextBox)item.FindControl("txtIssueQty");
                        if(((CheckBox)item.FindControl("chkitem")).Checked)                        
                        {
                            string query = "INSERT INTO BVYardIssue (DemandNo,ItemCode ,QtyIssued ,DateIssued,IssueRefNo,Status)VALUES(@DemandNo,@ItemCode ,@QtyIssued ,@DateIssued,@IssueRefNo,0)";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@DemandNo", item.Cells[2].Text);
                            cmd.Parameters.AddWithValue("@ItemCode", item.Cells[3].Text);
                            cmd.Parameters.AddWithValue("@QtyIssued", Convert.ToInt32(txtQty.Text));
                            cmd.Parameters.AddWithValue("@DateIssued", System.DateTime.Now);
                            cmd.Parameters.AddWithValue("@IssueRefNo",txtIssueRefNo.Text);
                            cmd.ExecuteNonQuery();
                        }
                    }                    
                }
                lblStatus.Text = "Demand Issued successfully.";
                BindGridView(monthYearPicker.Value);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
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
                    SqlCommand cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,DemandDate,SupplyDate FROM Demand WHERE Status=1 AND DemandNO NOT IN(SELECT DemandNO FROM BVYardIssue) AND  CONVERT(VARCHAR(7), DemandDate, 120) = @Month ORDER By Id desc", conn);
                    cmd.Parameters.AddWithValue("@Month", monthYear);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();
                    if (dt.Rows.Count > 0)
                    {
                        SubmitButton.Visible = true;
                        lblRef.Visible = true;
                        txtIssueRefNo.Visible = true;
                    }
                    else
                    {
                        SubmitButton.Visible = false;
                        lblRef.Visible = false;
                        txtIssueRefNo.Visible = false;
                    }
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
       
    }
}