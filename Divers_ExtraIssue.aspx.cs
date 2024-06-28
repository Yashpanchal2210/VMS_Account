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

                string date = Request.Form["date"];
                string name = Request.Form["name"];
                string rank = Request.Form["rank"];
                string category = Request.Form["category"];
                string pno = Request.Form["pno"];
                string days = Request.Form["days"];

                string[] itemNames = new string[]
                {
                    Request.Form["itemname1"],
                    Request.Form["itemname2"],
                    Request.Form["itemname3"],
                    "Sugar",
                    "Ground Nut",
                    "Chocolate",
                    "Horlicks"
                };

                decimal[] quantities = new decimal[]
                {
                    GetQuantity(itemNames[0], "Dropdown1") * Convert.ToDecimal(days),
                    GetQuantity(itemNames[1], "Dropdown2") * Convert.ToDecimal(days),
                    GetQuantity(itemNames[2], "Dropdown3") * Convert.ToDecimal(days),
                    0.05m * Convert.ToDecimal(days),
                    0.05m * Convert.ToDecimal(days),
                    0.05m * Convert.ToDecimal(days),
                    0.05m * Convert.ToDecimal(days)
                };

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        (string itemName, string denomination) = GetItemNameById(conn, itemNames[i]);
                        int itemId = 0;
                        using (SqlCommand cmd1 = new SqlCommand("select TOP 1 Id from InLieuItems where InLieuItem = @ItemName AND Category = @Category", conn))
                        {
                            cmd1.Parameters.AddWithValue("@ItemName", itemNames[i]);
                            cmd1.Parameters.AddWithValue("@Category", category);
                            using (SqlDataReader reader = cmd1.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    itemId = Convert.ToInt32(reader["Id"]);
                                }
                            }
                        }

                        string dateString = date;
                        string UserName = name;
                        string RankVal = rank;
                        string PNOVal = pno;
                        string Days = days;
                        DateTime dateParse = DateTime.Parse(dateString);

                        decimal qty = quantities[i];
                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssue (Date, Name, Rank, PNo, Days, ItemId, ItemName, Qty, Type) VALUES (@Date, @Name, @Rank, @PNo, @Days, @ItemId, @ItemName, @Qty, @Type)", conn);

                        cmd.Parameters.AddWithValue("@Date", dateParse);
                        cmd.Parameters.AddWithValue("@Name", UserName);
                        cmd.Parameters.AddWithValue("@Rank", RankVal);
                        cmd.Parameters.AddWithValue("@PNo", PNOVal);
                        cmd.Parameters.AddWithValue("@Days", Days);
                        cmd.Parameters.AddWithValue("@ItemName", itemNames[i]);
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@Qty", qty);
                        cmd.Parameters.AddWithValue("@Type", "DiversIssue");

                        decimal qtyIssued = qty;
                        SqlCommand checkReceiptCmd = new SqlCommand(
                            "SELECT SUM(Qty) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkReceiptCmd.Parameters.AddWithValue("@ItemName", itemName);

                        object result = checkReceiptCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            lblStatus.Text = $"No data found for item {itemName} in PresentStockMaster.";
                            continue;
                        }
                        else
                        {
                            decimal availableQty = Convert.ToDecimal(result);

                            if (availableQty < qtyIssued)
                            {
                                lblStatus.Text = $"Insufficient quantity for item.";
                                continue;
                            }
                        }

                        cmd.ExecuteNonQuery();
                        lblStatus.Text = "Data entered successfully.";


                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemNames[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denomination);
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", dateParse); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", itemNames[i]);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", dateParse); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", itemNames[i]);
                            updateCmd.Parameters.AddWithValue("@Quantity", qtyIssued);

                            updateCmd.ExecuteNonQuery();
                            
                           
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", dateParse); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", itemNames[i]);
                            insertCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                LoadGridView();
                Response.Redirect(Request.RawUrl);
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
            string query = "SELECT ILueItem, iLueDenom FROM BasicLieuItems WHERE ILueItem = @ItemId";

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

        private decimal GetQuantity(string itemId, string dropdownName)
        {
            switch (itemId)
            {
                case "Milk Fresh":
                    return (dropdownName == "Dropdown1") ? 0.2m : 0.15m;
                case "Milk Tinned":
                    return (dropdownName == "Dropdown1") ? 0.08m : 0.055m;
                case "Milk Powder":
                    return (dropdownName == "Dropdown1") ? 0.028m : 0.02m;
                case "Butter Tinned":
                    return (dropdownName == "Dropdown3") ? 0.05m : 0.05m;
                case "Butter Fresh":
                    return (dropdownName == "Dropdown3") ? 0.05m : 0.05m;
                case "Eggs":
                    return 2;
                case "Cheese Tinned":
                    return 0.05m;
                default:
                    throw new ArgumentException("Invalid itemId");
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
            if (e.Row.RowType == DataControlRowType.DataRow /*&& GridViewExtraIssueDivers.EditIndex == e.Row.RowIndex*/)
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
                if (Session["Role"] != null && Session["Role"].ToString() == "Store Keeper")
                {
                    // Find the delete button in the row and hide it
                    LinkButton deleteButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                    if (deleteButton != null)
                    {
                        deleteButton.Visible = false;
                    }
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