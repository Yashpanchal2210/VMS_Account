﻿using System;
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
using System.IO;

namespace VMS_1
{
    public partial class OtherShips : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            GetItems();
            LoadGridView();
        }
        private int checkVAStatus(string[] Date)
        {
            int status = 0;
            string[] data = Date[0].Split('-');
            int selectedMonth = Convert.ToInt32(data[1]);
            int selectedYear = Convert.ToInt32(data[0]);
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "EXEC usp_GetVACurrentStatusByMonth " + selectedMonth + "," + selectedYear + "";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            if (dt.Rows.Count > 0)
            {
                if (Session["NudId"].ToString() != dt.Rows[0]["FrowardedTo"].ToString())
                {
                    status = 1;
                }
            }
            return status;
        }
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] itemnameId = Request.Form.GetValues("itemname");
                string[] denom = Request.Form.GetValues("denom");
                string[] qty = Request.Form.GetValues("qty");
                string[] refno = Request.Form.GetValues("refno");
                int status = checkVAStatus(date);
                if (status > 0)
                {
                    lblStatus.Text = "you can not enter the data in this visualling account month";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        string itemname = GetItemNameById(conn, itemnameId[i]);

                        SqlCommand cmd = new SqlCommand("INSERT INTO otherships (Date, Itemname, Denom, Qty, Reference) VALUES (@Date, @Itemname, @Denom, @Qty, @Reference)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@Itemname", itemname);
                        cmd.Parameters.AddWithValue("@Denom", denom[i]);
                        cmd.Parameters.AddWithValue("@Qty", qty[i]);
                        cmd.Parameters.AddWithValue("@Reference", refno[i]);

                        decimal qtyIssued = decimal.Parse(qty[i]);
                        SqlCommand checkReceiptCmd = new SqlCommand(
                            "SELECT SUM(Qty) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkReceiptCmd.Parameters.AddWithValue("@ItemName", itemname[i]);

                        object result = checkReceiptCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            lblStatus.Text = $"No data found for item {itemname} in PresentStockMaster.";
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
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qty[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denom[i]);
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", itemname);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", itemname);
                            updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", itemname);
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

        private string GetItemNameById(SqlConnection conn, string itemId)
        {
            string itemName = string.Empty;
            string query = "SELECT ILueItem FROM BasicLieuItems WHERE Id = @ItemId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemName = reader["ILueItem"].ToString();
                    }
                }
            }

            return itemName;
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string fileName = Path.GetFileName(FileUpload1.FileName);
                string fileDateString = Request.Form["fileDate"];  // Assuming fileDateTextBox is the ID of your input element for date selection
                DateTime fileDate;

                if (DateTime.TryParse(fileDateString, out fileDate))
                {
                    // Successfully parsed the date
                    // Proceed with your logic
                    lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
                }
                else
                {
                    // Handle invalid date format
                    lblStatus.Text = "Invalid date format.";
                }

                // Save the file to the database
                SaveFileToDatabase(FileUpload1.PostedFile.InputStream, fileName, fileDate);
            }
            else
            {
                lblStatus.Text = "Please select a file to upload.";
            }
        }

        private void SaveFileToDatabase(Stream fileStream, string fileName, DateTime fileDate)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    BinaryReader br = new BinaryReader(fileStream);
                    Byte[] bytes = br.ReadBytes((Int32)fileStream.Length);

                    string query = "INSERT INTO PDFFiles (Name, type, Date, Data) VALUES (@Name, @type, @FileDate, @Data)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = fileName;
                    cmd.Parameters.Add("@type", "OtherShips");
                    cmd.Parameters.Add("@FileDate", SqlDbType.VarChar).Value = fileDate;
                    cmd.Parameters.Add("@Data", SqlDbType.Binary).Value = bytes;

                    cmd.ExecuteNonQuery();
                }
                lblStatus.Text = "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while uploading the file: " + ex.Message;
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

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM otherships order by Id desc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewOtherShips.DataSource = dt;
                    GridViewOtherShips.DataBind();
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
            string ItemName = "";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string itemNameQuery = "Select ItemName from RationScale Where ItemId = @Id";
                SqlCommand cmditem = new SqlCommand(itemNameQuery, conn);
                cmditem.Parameters.AddWithValue("@Id", ItemVal);
                SqlDataReader reader1 = cmditem.ExecuteReader();
                if (reader1.Read())
                {
                    ItemName = reader1["ItemName"].ToString();
                }
                conn.Close();

                conn.Open();
                string query = "SELECT Denomination, VegScale, NonVegScale  FROM InLieuItems WHERE InLieuItem = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", ItemName);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Denomination = reader["Denomination"].ToString();
                }
            }

            return Denomination;
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

        protected void GridViewExtraIssueOtherShip_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = Convert.ToInt32(GridViewOtherShips.DataKeys[e.RowIndex].Value);
            string quantities = "";
            string itemname = "";
            string date = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Qty, Itemname, Date FROM OtherShips WHERE Id=@Id", conn))
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



                using (SqlCommand cmd = new SqlCommand("DELETE FROM OtherShips WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView(); // Rebind the GridView to show the updated data
        }

        protected void GridViewOS_RowDataBound(object sender, GridViewRowEventArgs e)
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