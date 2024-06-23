using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Divers_ExtraIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridView();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] name = Request.Form.GetValues("name");
                string[] rank = Request.Form.GetValues("rank");
                string[] pno = Request.Form.GetValues("pno");
                string[] days = Request.Form.GetValues("days");
                string[] itemnameId = Request.Form.GetValues("itemname");
                string[] qty = Request.Form.GetValues("qty");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < itemnameId.Length; i++)
                    {
                        (string itemName, string denomination) = GetItemNameById(conn, itemnameId[i]);

                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssue (Date, Name, Rank, PNo, Days, ItemId, ItemName, Qty, Type) VALUES (@Date, @Name, @Rank, @PNo, @Days, @ItemId, @ItemName, @Qty, @Type)", conn);

                        cmd.Parameters.AddWithValue("@Date", i < date.Length ? date[i] : date[0]);
                        cmd.Parameters.AddWithValue("@Name", i < name.Length ? name[i] : name[0]);
                        cmd.Parameters.AddWithValue("@Rank", i < rank.Length ? rank[i] : rank[0]);
                        cmd.Parameters.AddWithValue("@PNo", i < pno.Length ? pno[i] : pno[0]);
                        cmd.Parameters.AddWithValue("@Days", i < days.Length ? days[i] : days[0]);
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@ItemId", itemnameId[i]);
                        cmd.Parameters.AddWithValue("@Qty", decimal.Parse(qty[i]));
                        cmd.Parameters.AddWithValue("@Type", "DiversIssue");

                        cmd.ExecuteNonQuery();


                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denomination);
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", itemName);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                lblStatus.Text = "Data entered successfully.";

                LoadGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private (string itemName, string denomination) GetItemNameById(SqlConnection conn, string itemId)
        {
            string itemName = string.Empty;
            string denomination = string.Empty;
            string query = "SELECT ILueItem, iLueDenom FROM BasicLieuItems WHERE Id = @ItemId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemName = reader["ILueItem"].ToString();
                        denomination = reader["iLueDenom"].ToString();
                    }
                }
            }

            return (itemName, denomination);
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

        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssue Order By Id desc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueDivers.DataSource = dt;
                    GridViewExtraIssueDivers.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewExtraIssueDivers.EditIndex = e.NewEditIndex;
            LoadGridView();

            // Bind the dropdown list in the edited row
            GridViewRow row = GridViewExtraIssueDivers.Rows[e.NewEditIndex];
            DropDownList ddlitemname = (DropDownList)row.FindControl("ddlitemname");
            if (ddlitemname != null)
            {
                ddlitemname.DataSource = GetItemNames(); // Call your GetItemNames method here
                ddlitemname.DataTextField = "Text";
                ddlitemname.DataValueField = "Value";
                ddlitemname.DataBind();
            }
        }


        protected void GridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            GridViewRow row = GridViewExtraIssueDivers.Rows[e.RowIndex];
            //int id = Convert.ToInt32(GridView.DataKeys[e.RowIndex].Values[0]);
            //string date = ((TextBox)row.FindControl("lblDate")).Text;
            string name = ((TextBox)row.FindControl("lblname")).Text;
            string rank = ((TextBox)row.FindControl("txtrank")).Text;
            string pno = ((TextBox)row.FindControl("txtpno")).Text;
            string days = ((TextBox)row.FindControl("txtdays")).Text;
            DropDownList itemname = (DropDownList)row.FindControl("ddlitemname");
            string qty = ((TextBox)row.FindControl("txtqty")).Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE ExtraIssue SET Name = @Name, Rank = @Rank, PNo = @PNo, Days = @Days, ItemId = @ItemID,  ItemName=@ItemName, Qty = @Qty WHERE Id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", e.RowIndex);
                    //cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Rank", rank);
                    cmd.Parameters.AddWithValue("@PNo", pno);
                    cmd.Parameters.AddWithValue("@Days", days);
                    cmd.Parameters.AddWithValue("@ItemID", itemname);
                    cmd.Parameters.AddWithValue("@ItemName", itemname);
                    cmd.Parameters.AddWithValue("@Qty", qty);

                    cmd.ExecuteNonQuery();
                }
                GridViewExtraIssueDivers.EditIndex = -1;
                LoadGridView();
                lblStatus.Text = "Record updated successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while updating the record: " + ex.Message;
            }
        }

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && GridViewExtraIssueDivers.EditIndex == e.Row.RowIndex)
            {
                DropDownList ddlitemname = (DropDownList)e.Row.FindControl("ddlitemname");
                if (ddlitemname != null)
                {
                    // Bind the DropDownList to a data source
                    ddlitemname.DataSource = GetItemNames(); // Set YourDataSource to your data source
                    ddlitemname.DataTextField = "ItemName";
                    ddlitemname.DataValueField = "ItemId";
                    ddlitemname.DataBind();

                    // Set the selected value based on the current row's data
                    DataRowView drv = (DataRowView)e.Row.DataItem;
                    ddlitemname.SelectedValue = drv["ItemId"].ToString();
                }
            }
        }


        protected void GridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewExtraIssueDivers.EditIndex = -1;
            LoadGridView();
        }

        protected void GridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                int id = Convert.ToInt32(GridViewExtraIssueDivers.DataKeys[e.RowIndex].Value);

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM ExtraIssue WHERE Id=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);

                    cmd.ExecuteNonQuery();
                }
                LoadGridView();
                lblStatus.Text = "Record deleted successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while deleting the record: " + ex.Message;
            }
        }

    }
}