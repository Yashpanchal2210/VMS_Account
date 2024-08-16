using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Windows.Controls;

namespace VMS_1
{
    public partial class ItemReceipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack)
            {
                if (ViewState["DataTable"] == null)
                {
                    ViewState["DataTable"] = new DataTable();
                }
                BindGridView();
                BindFiles();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetItemNames()
        {
            HashSet<string> itemNames = new HashSet<string>();
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT InLieuItem FROM InLieuItems";
            //string itemquery = "SELECT DISTINCT BasicItem FROM BasicItems ORDER BY BasicItem ASC";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        itemNames.Add(reader["InLieuItem"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading item names: " + ex.Message);
            }

            return itemNames.ToArray();
        }

        [WebMethod]
        public static string GetItemDenom(string ItemVal)
        {
            string Denomination = "";

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Denomination, VegScale, NonVegScale  FROM InLieuItems WHERE InLieuItem = @BasicItem";
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
        private int checkVAStatus(string[] Date)
        {
            int status = 0;
            string []data=Date[0].Split('-');
            int selectedMonth = Convert.ToInt32(data[1]);
            int selectedYear = Convert.ToInt32(data[0]);
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "EXEC usp_GetVACurrentStatusByMonth " + selectedMonth + ","+ selectedYear + "";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            if (dt.Rows.Count > 0)
            {
                if (Session["NudId"].ToString()!= dt.Rows[0]["FrowardedTo"].ToString())
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

                string[] itemNames = Request.Form.GetValues("itemname");
                string[] quantities = Request.Form.GetValues("qty");
                string[] receivedFromArray = Request.Form.GetValues("rcvdfrom");
                string[] otherReceivedFromArray = Request.Form.GetValues("otherReceivedFrom");
                string[] referenceNosArray = Request.Form.GetValues("ref");
                string[] datesArray = Request.Form.GetValues("date");
                int status=checkVAStatus(datesArray);
                if(status>0)
                {
                    lblStatus.Text = "you can not enter the data in this visualling account month";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                if (itemNames == null || quantities == null || receivedFromArray == null || referenceNosArray == null || datesArray == null)
                {
                    lblStatus.Text = "Some form values are missing.";
                    return;
                }

                string receivedFrom = receivedFromArray.Length > 0 ? receivedFromArray[0] : null;
                //string otherReceivedFrom = otherReceivedFromArray.Length > 0 ? otherReceivedFromArray[0] : null;
                string referenceNo = referenceNosArray.Length > 0 ? referenceNosArray[0] : null;
                string dateStr = datesArray.Length > 0 ? datesArray[0] : null;

                if (itemNames.Length != quantities.Length)
                {
                    lblStatus.Text = "The number of items and quantities do not match.";
                    return;
                }

                string[] denominations = new string[itemNames.Length];

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        SqlCommand getDenominationCmd = new SqlCommand("SELECT Denomination FROM InLieuItems WHERE InLieuItem = @ItemName", conn);
                        getDenominationCmd.Parameters.AddWithValue("@ItemName", itemNames[i]);

                        object denomValue = getDenominationCmd.ExecuteScalar();
                        if (denomValue != null)
                        {
                            denominations[i] = denomValue.ToString();
                        }
                        else
                        {
                            lblStatus.Text = "Denomination not found for item: " + itemNames[i];
                            return;
                        }
                    }

                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        DateTime date;
                        if (!DateTime.TryParse(dateStr, out date))
                        {
                            lblStatus.Text = "Invalid date format for item: " + itemNames[i];
                            return;
                        }

                        string itemName = itemNames[i];
                        decimal quantity;
                        if (!decimal.TryParse(quantities[i], out quantity))
                        {
                            lblStatus.Text = "Invalid quantity format for item: " + itemNames[i];
                            return;
                        }

                        //string receivedFromValue = receivedFrom == "Others" ? otherReceivedFrom : receivedFrom;

                        SqlCommand cmd = new SqlCommand("INSERT INTO ReceiptMaster (itemNames, quantities, denominations, receivedFrom, referenceNos, Dates) VALUES (@ItemName, @Quantity, @Denomination, @ReceivedFrom, @ReferenceNo, @Date)", conn);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Denomination", i < denominations.Length ? denominations[i] : denominations[0]);
                        cmd.Parameters.AddWithValue("@ReceivedFrom", receivedFrom);
                        cmd.Parameters.AddWithValue("@ReferenceNo", referenceNo);
                        cmd.Parameters.AddWithValue("@Date", date);

                        cmd.ExecuteNonQuery();

                        if (FileUpload1.HasFile)
                        {
                            string fileName = Path.GetFileName(FileUpload1.FileName);
                            string fileDateString = Request.Form["fileDate"];
                            DateTime fileDate;

                            if (DateTime.TryParse(fileDateString, out fileDate))
                            {
                                lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
                            }
                            else
                            {
                                lblStatus.Text = "Invalid date format.";
                            }

                            SaveFileToDatabase(FileUpload1.PostedFile.InputStream, fileName, fileDate);
                        }
                        else
                        {
                            lblStatus.Text = "Please select a file to upload.";
                        }

                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand checkItemCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkItemCmd.Parameters.AddWithValue("@ItemName", itemName);

                        int itemCount = (int)checkItemCmd.ExecuteScalar();

                        if (itemCount > 0)
                        {
                            // If item exists, update the quantity
                            SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty + @Quantity WHERE ItemName = @ItemName", conn);
                            updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updatePresentStockCmd.Parameters.AddWithValue("@Quantity", quantity);

                            updatePresentStockCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If item does not exist, insert a new record
                            SqlCommand insertPresentStockCmd = new SqlCommand("INSERT INTO PresentStockMaster (ItemName, Qty, Denos) VALUES (@ItemName, @Quantity, @Denos)", conn);
                            insertPresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertPresentStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertPresentStockCmd.Parameters.AddWithValue("@Denos", denominations[i]);

                            insertPresentStockCmd.ExecuteNonQuery();
                        }

                        SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Month", date.Month);
                        checkCmd.Parameters.AddWithValue("@Year", date.Year);
                        checkCmd.Parameters.AddWithValue("@ItemName", itemName);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE MonthEndStockMaster SET Qty = Qty + @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Month", date.Month);
                            updateCmd.Parameters.AddWithValue("@Year", date.Year);
                            updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updateCmd.Parameters.AddWithValue("@Quantity", quantity);

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand insertCmd = new SqlCommand(
                                "INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", date); // Use the full date here
                            insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertCmd.Parameters.AddWithValue("@Type", "Receipt"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                //Response.Write("<script>alert('Data once submitted cannot be changed');</script>");
                //lblStatus.Text = "Data entered successfully.";
                BindGridView();
                BindFiles();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string fileName = Path.GetFileName(FileUpload1.FileName);
                string fileDateString = Request.Form["fileDate"];
                DateTime fileDate;

                if (DateTime.TryParse(fileDateString, out fileDate))
                {
                    lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
                }
                else
                {
                    lblStatus.Text = "Invalid date format.";
                }

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
                    cmd.Parameters.Add("@type", "ReceiptCRV");
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


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string[]> GetFilteredData(string monthYear)
        {
            List<string[]> data = new List<string[]>();
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM ReceiptMaster WHERE CONVERT(VARCHAR(7), Dates, 120) = @MonthYear", conn);
                    cmd.Parameters.AddWithValue("@MonthYear", monthYear);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new string[]
                        {
                            reader["itemNames"].ToString(),
                            reader["quantities"].ToString(),
                            reader["denominations"].ToString(),
                            reader["receivedFrom"].ToString(),
                            reader["referenceNos"].ToString(),
                            DateTime.Parse(reader["Dates"].ToString()).ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching data: " + ex.Message);
            }

            return data;
        }

        private void BindGridView()
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT itemid, Dates, referenceNos, receivedFrom, itemnames, denominations, quantities FROM ReceiptMaster ORDER By itemId DESC", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    //GridView1.DataSource = dt;
                    //GridView1.DataBind();
                    GridView.DataSource = dt;
                    GridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void FilesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                int fileId = Convert.ToInt32(e.CommandArgument);

                DeleteFileFromDatabase(fileId);

                BindFiles();
                BindGridView();
            }
        }

        private void DeleteFileFromDatabase(int Id)
        {
            // Assuming you are using SqlConnection and SqlCommand for SQL Server
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                // SQL command to delete a record from the database
                string sql = "DELETE FROM PDFFiles WHERE Id = @Id";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", Id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if any rows were affected (optional but good for validation)
                    if (rowsAffected > 0)
                    {
                        lblStatus.Text = "File deleted successfully.";
                    }
                    else
                    {
                        lblStatus.Text = "File not deleted.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the database operation
                    // Log the exception or display an error message as needed
                    throw new Exception("Error deleting file from database", ex);
                }
                finally
                {
                    // Ensure connection is properly closed
                    connection.Close();
                }
            }
        }

        protected void GridViewFileReceipt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Row Type: " + e.Row.RowType.ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["Role"] != null && Session["Role"].ToString() == "Store Keeper")
                {
                    // Find the delete button in the row and hide it
                    LinkButton deleteButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "DeleteFile");
                    if (deleteButton != null)
                    {
                        deleteButton.Visible = false;
                    }
                }
            }
        }

        protected void GridViewReceipt_RowDataBound(object sender, GridViewRowEventArgs e)
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

        private void BindFiles()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM PDFfiles Where Type = 'ReceiptCRV' ORDER By Id DESC", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    FilesGridView.DataSource = dt;
                    FilesGridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void GridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            GridViewRow row = GridView.Rows[e.RowIndex];
            int id = Convert.ToInt32(GridView.DataKeys[e.RowIndex].Values[0]);

            System.Web.UI.WebControls.TextBox dateTextBox = (System.Web.UI.WebControls.TextBox)row.FindControl("lblDate");
            string date = dateTextBox.Text;

            System.Web.UI.WebControls.TextBox dateTextBox1 = (System.Web.UI.WebControls.TextBox)row.FindControl("lblreferenceNos");
            string referenceNos = dateTextBox1.Text;
            System.Web.UI.WebControls.TextBox dateTextBox2 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtreceivedFrom");
            string receivedFrom = dateTextBox2.Text;
            System.Web.UI.WebControls.TextBox dateTextBox3 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtitemnames");
            string itemnames = dateTextBox3.Text;
            System.Web.UI.WebControls.TextBox dateTextBox4 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtDenomination");
            string denominations = dateTextBox4.Text;
            System.Web.UI.WebControls.TextBox dateTextBox5 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtquantities");
            string quantities = dateTextBox5.Text;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE ReceiptMaster SET Dates=@Date, referenceNos=@ReferenceNos, receivedFrom=@ReceivedFrom, itemnames=@ItemNames, denominations=@Denominations, quantities=@Quantities WHERE itemid=@ID", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@ReferenceNos", referenceNos);
                    cmd.Parameters.AddWithValue("@ReceivedFrom", receivedFrom);
                    cmd.Parameters.AddWithValue("@ItemNames", itemnames);
                    cmd.Parameters.AddWithValue("@Denominations", denominations);
                    cmd.Parameters.AddWithValue("@Quantities", quantities);

                    cmd.ExecuteNonQuery();
                }
                GridView.EditIndex = -1;
                BindGridView();
                lblStatus.Text = "Record updated successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while updating the record: " + ex.Message;
            }
        }

        protected void GridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView.EditIndex = -1;
            BindGridView();
        }

        protected void GridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;


            int id = Convert.ToInt32(GridView.DataKeys[e.RowIndex].Value);
            string quantities = "";
            string itemname = "";
            string date = "";



            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT quantities, itemnames, Dates FROM ReceiptMaster WHERE itemid=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantities = reader["quantities"].ToString();
                            itemname = reader["itemnames"].ToString();
                            date = reader["Dates"].ToString();
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
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
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
                            "UPDATE MonthEndStockMaster SET Qty = Qty - @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        updateCmd.Parameters.AddWithValue("@Month", month);
                        updateCmd.Parameters.AddWithValue("@Year", year);
                        updateCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updateCmd.Parameters.AddWithValue("@Quantity", quantities);

                        updateCmd.ExecuteNonQuery();
                    }
                }
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM ReceiptMaster WHERE itemid=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }


            BindGridView();
        }
    }
}