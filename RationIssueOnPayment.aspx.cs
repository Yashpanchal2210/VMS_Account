using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace VMS_1
{
    public partial class RationIssueOnPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                LoadGridView();
                GetItems();
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                string itemNameQuery = "SELECT ItemName FROM RationScale WHERE ItemId = @ItemId";

                string[] date = Request.Form.GetValues("date");
                string[] itemIds = Request.Form.GetValues("item");
                string[] rate = Request.Form.GetValues("rate");
                string[] qty = Request.Form.GetValues("qty");
                string[] denom = Request.Form.GetValues("denom");
                string[] refno = Request.Form.GetValues("refno");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {

                        string itemName;
                        using (SqlCommand itemNameCmd = new SqlCommand(itemNameQuery, conn))
                        {
                            itemNameCmd.Parameters.AddWithValue("@ItemId", itemIds[i]);
                            itemName = itemNameCmd.ExecuteScalar()?.ToString();
                        }


                        SqlCommand cmd = new SqlCommand("INSERT INTO RationIssuePayment (Date, ItemName, Rate, Denom, Qty, ReferenceCRVNo) VALUES (@Date, @ItemName, @Rate, @Denom, @Qty, @ReferenceCRVNo)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Rate", rate[i]);
                        cmd.Parameters.AddWithValue("@Denom", denom[i]);
                        cmd.Parameters.AddWithValue("@Qty", qty[i]);
                        cmd.Parameters.AddWithValue("@ReferenceCRVNo", refno[i]);

                        decimal qtyIssued = decimal.Parse(qty[i]);
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
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qty[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denom[i]);
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

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM RationIssuePayment order by id desc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewRationPaymentIssue.DataSource = dt;
                    GridViewRationPaymentIssue.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        [WebMethod]
        public static List<object> GetItems()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM RationScale";
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
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemId"].ToString(),
                });
            }

            return items;
        }

        [WebMethod]
        public static string GetItemDataByItemId(string Id)
        {
            ItemData itemData = new ItemData();
            itemData.Rates = new List<string>();
            itemData.ReferenceNos = new List<string>();

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM RationScale WHERE ItemId = @ItemId";
            string itemNameQuery = "SELECT ItemName FROM RationScale WHERE ItemId = @ItemId";
            string referenceQuery = "SELECT DISTINCT referenceNos FROM ReceiptMaster";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemId", Id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemData.Rates.Add(reader["Rate"].ToString());
                        }
                    }
                }

                //string itemName;
                //using (SqlCommand itemNameCmd = new SqlCommand(itemNameQuery, conn))
                //{
                //    itemNameCmd.Parameters.AddWithValue("@ItemId", Id);
                //    itemName = itemNameCmd.ExecuteScalar()?.ToString();
                //}

                //if (!string.IsNullOrEmpty(itemName))
                //{
                //    using (SqlCommand referenceCmd = new SqlCommand(referenceQuery, conn))
                //    {
                //        referenceCmd.Parameters.AddWithValue("@itemnames", itemName);
                //        using (SqlDataReader referenceReader = referenceCmd.ExecuteReader())
                //        {
                //            while (referenceReader.Read())
                //            {
                //                itemData.ReferenceNos.Add(referenceReader["referenceNos"].ToString());
                //            }
                //        }
                //    }
                //}
            }

            return JsonConvert.SerializeObject(itemData);
        }

        public class ItemData
        {
            public List<string> Rates { get; set; }
            public List<string> ReferenceNos { get; set; }
        }

        protected void GridViewExtraIssuePayment_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = Convert.ToInt32(GridViewRationPaymentIssue.DataKeys[e.RowIndex].Value);
            string quantities = "";
            string itemname = "";
            string date = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Qty, ItemName, Date FROM RationIssuePayment WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
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



                using (SqlCommand cmd = new SqlCommand("DELETE FROM RationIssuePayment WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView(); // Rebind the GridView to show the updated data
        }


    }
}