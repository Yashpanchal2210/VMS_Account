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
    public partial class DemandApproval : System.Web.UI.Page
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
                monthapp.Value = MonthYear;
                BindGridView("0");
                BindApprovedGridView(MonthYear);
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
                    foreach (GridViewRow itm in GridViewRationScale.Rows)
                    {                        
                        if (((CheckBox)itm.FindControl("chk")).Checked)
                        {
                            Label lbldem = (Label)itm.FindControl("lblDemandNo");
                            string query = "UPDATE Demand SET Status=1,DemandDate=@DemandDate WHERE DemandNo=@DemandNo";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@DemandNo", lbldem.Text);
                            cmd.Parameters.AddWithValue("@DemandDate", System.DateTime.Now);
                            cmd.ExecuteNonQuery();
                            lblStatus.Text = "Data approved successfully.";
                        }
                    }                    
                }               
                BindGridView(monthYearPicker.Value);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        [WebMethod]
        public static List<object> GetItemNames()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT iLueItem, Id FROM BasicLieuItems";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            var items = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new
                {
                    Text = row["iLueItem"].ToString(),
                    Value = row["Id"].ToString(),
                });
            }

            return items;
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
                    SqlCommand cmd = new SqlCommand();
                    if (monthYear == "0" || monthYear == "")
                    {
                        cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,ReqDate,SupplyDate FROM Demand WHERE Status=0 ORDER By Id desc", conn);
                    }
                    else
                    {
                        cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,ReqDate,SupplyDate FROM Demand WHERE Status=0 AND  CONVERT(VARCHAR(7), ReqDate, 120) = @Month ORDER By Id desc", conn);
                    }
                    cmd.Parameters.AddWithValue("@Month", monthYear);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();

                    if (dt.Rows.Count > 0)
                    {
                        ViewState["DemandId"] = dt.Rows[0]["ID"].ToString();
                        SubmitButton.Visible = true;
                    }
                    else
                    {
                        SubmitButton.Visible = false;
                    }
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

        [WebMethod]
        public static string GetItemDenom(string ItemVal)
        {
            string Denomination = "";

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Denomination, VegScale, NonVegScale  FROM InLieuItems WHERE Id = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", ItemVal);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Denomination = reader["Denomination"].ToString();
                }
            }

            return Denomination;
        }

        protected void GridViewRationScale_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewRationScale.EditIndex = e.NewEditIndex;
            BindGridView(monthYearPicker.Value);

            try
            {
                DropDownList ddlItemName = (DropDownList)GridViewRationScale.Rows[e.NewEditIndex].FindControl("ddlItemName");
                if (ddlItemName != null)
                {
                    List<object> items = GetItemNames();
                    ddlItemName.DataSource = items;
                    ddlItemName.DataTextField = "Text";
                    ddlItemName.DataValueField = "Value";
                    ddlItemName.DataBind();

                    GridViewRow row = GridViewRationScale.Rows[e.NewEditIndex];
                    if (row != null && row.DataItem != null)
                    {
                        string currentItemName = ((DataRowView)row.DataItem)["ItemName"].ToString();
                        if (ddlItemName.Items.FindByText(currentItemName) != null)
                        {
                            ddlItemName.SelectedValue = currentItemName;
                        }
                    }
                }
                else
                {
                    lblStatus.Text = "DropDownList ddlItemName not found.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while editing: " + ex.Message;
            }
        }

        protected void GridViewRationScale_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            int id = (int)e.Keys["ID"];
            //DropDownList ddlItemName = (DropDownList)GridViewRationScale.Rows[e.RowIndex].FindControl("ddlItemName");
            TextBox txtQty = (TextBox)GridViewRationScale.Rows[e.RowIndex].FindControl("txtQty");
            //string itemname = ddlItemName.SelectedItem.Text;
            //string rate = (string)e.NewValues["Rate"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE Demand SET Qty=@Qty WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(txtQty.Text));
                //cmd.Parameters.AddWithValue("@Rate", rate);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            GridViewRationScale.EditIndex = -1;
            BindGridView(monthYearPicker.Value);
        }

        protected void GridViewRationScale_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewRationScale.EditIndex = -1;
            BindGridView(monthYearPicker.Value);
        }

        protected void GridViewRationScale_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            int id = (int)e.Keys["ID"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Demand WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGridView(monthYearPicker.Value);
        }

        protected void GridViewRation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Row Type: " + e.Row.RowType.ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["Role"] != null && Session["Role"].ToString() == "Store Keeper")
                {
                    // Find the delete button in the row and hide it
                    LinkButton deleteButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                    LinkButton editButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Edit");
                    if (deleteButton != null && editButton != null)
                    {
                        deleteButton.Visible = false;
                        editButton.Visible = false;
                    }
                }
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