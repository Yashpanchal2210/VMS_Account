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
            if (Session["NudId"] != null)
            {
                if (!IsPostBack)
                {
                    if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.RedirectToLoginPage();
                    }
                    string MonthYear = System.DateTime.Now.ToString("yyyy-MM");
                    //SubmitButton.Visible = false;
                    monthYearPicker.Value = MonthYear;
                    if (Session["NudId"].ToString() == "2044567F")
                    {
                        BindGridView(MonthYear, "F");
                    }
                    else if (Session["NudId"].ToString() == "2344567D")
                    {
                        BindGridView(MonthYear, "D");
                    }
                    else
                    { BindGridView(MonthYear, "ALL"); }
                }
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
                    foreach (GridViewRow item in GridViewRationScale.Rows)
                    {
                        TextBox txtQty = (TextBox)item.FindControl("txtIssueQty");
                        Label lblUnit = (Label)item.FindControl("lblUnitCode");
                        TextBox txtBatch = (TextBox)item.FindControl("txtBatch");
                        CheckBox chkClose = (CheckBox)item.FindControl("chkclose");
                        int close = 0;
                        if (chkClose.Checked)
                        {
                            close = 1;
                        }

                        if (((CheckBox)item.FindControl("chkitem")).Checked)
                        {
                            string query = "INSERT INTO BVYardIssue (DemandNo,ItemCode ,QtyIssued ,DateIssued,IssueRefNo,Status,UnitCode,BatchNo)VALUES(@DemandNo,@ItemCode ,@QtyIssued ,@DateIssued,@IssueRefNo,0,@UnitCode,@BatchNo)";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@DemandNo", item.Cells[2].Text);
                            cmd.Parameters.AddWithValue("@ItemCode", item.Cells[3].Text);
                            cmd.Parameters.AddWithValue("@QtyIssued", Convert.ToInt32(txtQty.Text));
                            cmd.Parameters.AddWithValue("@DateIssued", System.DateTime.Now);
                            cmd.Parameters.AddWithValue("@IssueRefNo", txtIssueRefNo.Text);
                            cmd.Parameters.AddWithValue("@UnitCode", lblUnit.Text);
                            cmd.Parameters.AddWithValue("@BatchNo", txtBatch.Text);                            
                            cmd.ExecuteNonQuery();
                            if (close > 0)
                            {
                                string queryupdate = "update Demand SET CloseDemand=1 WHERE DemandNo=@DemandNo";
                                SqlCommand cmdupdate = new SqlCommand(queryupdate, conn);
                                cmdupdate.Parameters.AddWithValue("@DemandNo", item.Cells[2].Text);
                                cmdupdate.ExecuteNonQuery();
                            }
                        }
                    }
                }
                lblStatus.Text = "Demand Issued successfully.";
                //BindGridView(monthYearPicker.Value);
                if (Session["NudId"].ToString() == "2044567F")
                {
                    BindGridView(monthYearPicker.Value, "F");
                }
                else if (Session["NudId"].ToString() == "2344567D")
                {
                    BindGridView(monthYearPicker.Value, "D");
                }
                else
                { BindGridView(monthYearPicker.Value, "ALL"); }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        private void BindGridView(string monthYear, string Category)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand("EXEC usp_GetDemandListForIssue '" + monthYear + "','" + Category + "'", conn);
                    //cmd.Parameters.AddWithValue("@Month", monthYear);
                    //cmd.Parameters.AddWithValue("@Category", Category);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();
                    if (dt.Rows.Count > 0)
                    {
                        rmk.Visible = true;
                        //lblRef.Visible = true;
                        //txtIssueRefNo.Visible = true;
                    }
                    else
                    {
                        rmk.Visible = false;
                        //lblRef.Visible = false;
                        //txtIssueRefNo.Visible = false;
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
            if (Session["NudId"].ToString() == "2044567F")
            {
                BindGridView(monthYearPicker.Value, "F");
            }
            else if (Session["NudId"].ToString() == "2344567D")
            {
                BindGridView(monthYearPicker.Value, "D");
            }
            else
            { BindGridView(monthYearPicker.Value, "ALL"); }
        }
    }
}