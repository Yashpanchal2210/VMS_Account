using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class LeadPoisioning_ExtraIssue : System.Web.UI.Page
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
                string[] strength = Request.Form.GetValues("strength");
                string[] milk = Request.Form.GetValues("milk");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    int itemId = 0;
                    string query = "SELECT Id FROM BasicLieuItems WHERE IlueItem = @ItemName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                itemId = Convert.ToInt32(reader["Id"]);
                            }
                        }
                    }

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, ItemId, ItemName, Type, Qty) VALUES (@Date, @Strength, @ItemId, @ItemName, @Type, @Qty)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@Strength", strength[i]);
                        cmd.Parameters.AddWithValue("@ItemId", itemId);
                        cmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        cmd.Parameters.AddWithValue("@Type", "LeadPoisioning");
                        cmd.Parameters.AddWithValue("@Qty", milk[i]);

                        decimal qtyIssued = decimal.Parse(milk[i]);
                        SqlCommand checkReceiptCmd = new SqlCommand(
                            "SELECT SUM(Qty) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkReceiptCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");

                        object result = checkReceiptCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            lblStatus.Text = $"No data found for item Milk Fresh in PresentStockMaster.";
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
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", milk[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", "Ltr");
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                            updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(milk[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                            insertCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(milk[i]));
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

        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssueCategory where Type = 'LeadPoisioning' Order By Id Desc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueCategory2.DataSource = dt;
                    GridViewExtraIssueCategory2.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridViewExtraIssueLead_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = Convert.ToInt32(GridViewExtraIssueCategory2.DataKeys[e.RowIndex].Value);
            string quantities = "";
            string itemname = "";
            string date = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Qty, ItemName, Date FROM ExtraIssueCategory WHERE Id=@Id AND Type = @Type", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Type", "LeadPoisioning");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantities = reader["Qty"].ToString();
                            itemname = reader["ItemName"].ToString();
                            date = reader["Date"].ToString();
                        }
                    }
                }

                if (quantities != null)
                {
                    DateTime dateTime = DateTime.Parse(date);
                    int month = dateTime.Month;
                    int year = dateTime.Year;

                    SqlCommand checkItemCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                    checkItemCmd.Parameters.AddWithValue("@ItemName", itemname);

                    int itemCount = (int)checkItemCmd.ExecuteScalar();

                    if (itemCount > 0)
                    {
                        // If item exists, update the quantity
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty + @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", quantities);

                        updatePresentStockCmd.ExecuteNonQuery();
                    }

                    SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                    checkCmd.Parameters.AddWithValue("@Month", month);
                    checkCmd.Parameters.AddWithValue("@Year", year);
                    checkCmd.Parameters.AddWithValue("@ItemName", itemname);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        SqlCommand updateCmd = new SqlCommand(
                            "UPDATE MonthEndStockMaster SET Qty = Qty + @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        updateCmd.Parameters.AddWithValue("@Month", month);
                        updateCmd.Parameters.AddWithValue("@Year", year);
                        updateCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updateCmd.Parameters.AddWithValue("@Quantity", quantities);

                        updateCmd.ExecuteNonQuery();
                    }
                }



                using (SqlCommand cmd = new SqlCommand("DELETE FROM ExtraIssueCategory WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView(); // Rebind the GridView to show the updated data
        }

        protected void GridViewLead_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Row Type: " + e.Row.RowType.ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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
    }
}