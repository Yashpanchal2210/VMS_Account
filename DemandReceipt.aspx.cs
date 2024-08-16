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


namespace VMS_1
{
    public partial class DemandReceipt : System.Web.UI.Page
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
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                //string[] itemNames = Request.Form.GetValues("itemname");
                //string[] quantities = Request.Form.GetValues("qty");
                //string[] receivedFromArray = Request.Form.GetValues("rcvdfrom");
                //string[] otherReceivedFromArray = Request.Form.GetValues("otherReceivedFrom");
                //string[] referenceNosArray = Request.Form.GetValues("ref");
                //string[] datesArray = Request.Form.GetValues("date");

                //if (itemNames == null || quantities == null || receivedFromArray == null || referenceNosArray == null || datesArray == null)
                //{
                //    lblStatus.Text = "Some form values are missing.";
                //    return;
                //}

                //string receivedFrom = receivedFromArray.Length > 0 ? receivedFromArray[0] : null;
                ////string otherReceivedFrom = otherReceivedFromArray.Length > 0 ? otherReceivedFromArray[0] : null;
                //string referenceNo = referenceNosArray.Length > 0 ? referenceNosArray[0] : null;
                //string dateStr = datesArray.Length > 0 ? datesArray[0] : null;

                //if (itemNames.Length != quantities.Length)
                //{
                //    lblStatus.Text = "The number of items and quantities do not match.";
                //    return;
                //}

                //string[] denominations = new string[itemNames.Length];

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    foreach (GridViewRow itm in gvDemandIssue.Rows)
                    {
                        string denominations = "";
                        if (((CheckBox)itm.FindControl("chk")).Checked)
                        {
                            //SqlCommand getDenominationCmd = new SqlCommand("SELECT Denomination FROM InLieuItems WHERE InLieuItem = @ItemName", conn);
                            //getDenominationCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);

                            //object denomValue = getDenominationCmd.ExecuteScalar();
                            //if (denomValue != null)
                            //{
                            //    denominations = denomValue.ToString();
                            //}
                            //else
                            //{
                            //    lblStatus.Text = "Denomination not found for item: " + itm.Cells[3].Text;
                            //    return;
                            //}
                        

                        //for (int i = 0; i < itemNames.Length; i++)
                        //{
                        //    DateTime date;
                        //    if (!DateTime.TryParse(dateStr, out date))
                        //    {
                        //        lblStatus.Text = "Invalid date format for item: " + itemNames[i];
                        //        return;
                        //    }

                        //    string itemName = itemNames[i];
                        //    decimal quantity;
                        //    if (!decimal.TryParse(quantities[i], out quantity))
                        //    {
                        //        lblStatus.Text = "Invalid quantity format for item: " + itemNames[i];
                        //        return;
                        //    }

                            //string receivedFromValue = receivedFrom == "Others" ? otherReceivedFrom : receivedFrom;

                            SqlCommand cmd = new SqlCommand("INSERT INTO ReceiptMaster (itemNames, quantities, denominations, receivedFrom, referenceNos, Dates) VALUES (@ItemName, @Quantity, @Denomination, @ReceivedFrom, @ReferenceNo, @Date)", conn);
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);
                            cmd.Parameters.AddWithValue("@Quantity", itm.Cells[5].Text);
                            cmd.Parameters.AddWithValue("@Denomination", itm.Cells[4].Text);
                            cmd.Parameters.AddWithValue("@ReceivedFrom", "BVYARD");
                            cmd.Parameters.AddWithValue("@ReferenceNo", itm.Cells[7].Text);
                            cmd.Parameters.AddWithValue("@Date", itm.Cells[6].Text);

                            cmd.ExecuteNonQuery();

                            //if (FileUpload1.HasFile)
                            //{
                            //    string fileName = Path.GetFileName(FileUpload1.FileName);
                            //    string fileDateString = Request.Form["fileDate"];
                            //    DateTime fileDate;

                            //    if (DateTime.TryParse(fileDateString, out fileDate))
                            //    {
                            //        lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
                            //    }
                            //    else
                            //    {
                            //        lblStatus.Text = "Invalid date format.";
                            //    }

                            //    SaveFileToDatabase(FileUpload1.PostedFile.InputStream, fileName, fileDate);
                            //}
                            //else
                            //{
                            //    lblStatus.Text = "Please select a file to upload.";
                            //}

                            // Update PresentStockMaster table if ItemName exists
                            SqlCommand checkItemCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                            checkItemCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);

                            int itemCount = (int)checkItemCmd.ExecuteScalar();

                            if (itemCount > 0)
                            {
                                // If item exists, update the quantity
                                SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty + @Quantity WHERE ItemName = @ItemName", conn);
                                updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);
                                updatePresentStockCmd.Parameters.AddWithValue("@Quantity", itm.Cells[5].Text);

                                updatePresentStockCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                // If item does not exist, insert a new record
                                SqlCommand insertPresentStockCmd = new SqlCommand("INSERT INTO PresentStockMaster (ItemName, Qty, Denos) VALUES (@ItemName, @Quantity, @Denos)", conn);
                                insertPresentStockCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);
                                insertPresentStockCmd.Parameters.AddWithValue("@Quantity", itm.Cells[5].Text);
                                insertPresentStockCmd.Parameters.AddWithValue("@Denos", itm.Cells[4].Text);

                                insertPresentStockCmd.ExecuteNonQuery();
                            }

                            SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                            checkCmd.Parameters.AddWithValue("@Month", Convert.ToDateTime(itm.Cells[6].Text).Month);
                            checkCmd.Parameters.AddWithValue("@Year", Convert.ToDateTime(itm.Cells[6].Text).Year);
                            checkCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);

                            int count = (int)checkCmd.ExecuteScalar();

                            if (count > 0)
                            {
                                SqlCommand updateCmd = new SqlCommand(
                                    "UPDATE MonthEndStockMaster SET Qty = Qty + @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                                updateCmd.Parameters.AddWithValue("@Month", Convert.ToDateTime(itm.Cells[6].Text).Month);
                                updateCmd.Parameters.AddWithValue("@Year", Convert.ToDateTime(itm.Cells[6].Text).Year);
                                updateCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);
                                updateCmd.Parameters.AddWithValue("@Quantity", itm.Cells[5].Text);

                                updateCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                SqlCommand insertCmd = new SqlCommand(
                                    "INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                                insertCmd.Parameters.AddWithValue("@Date", itm.Cells[6].Text); // Use the full date here
                                insertCmd.Parameters.AddWithValue("@ItemName", itm.Cells[3].Text);
                                insertCmd.Parameters.AddWithValue("@Quantity", itm.Cells[5].Text);
                                insertCmd.Parameters.AddWithValue("@Type", "Receipt"); // Set the correct parameter name for Type

                                insertCmd.ExecuteNonQuery();
                            }
                            SqlCommand updateissueCmd = new SqlCommand("UPDATE BVYardIssue SET Status =1 WHERE Itemcode = @Itemcode And DemandNo=@DemandNo", conn);
                            updateissueCmd.Parameters.AddWithValue("@Itemcode", itm.Cells[2].Text);
                            updateissueCmd.Parameters.AddWithValue("@DemandNo", itm.Cells[1].Text);

                            updateissueCmd.ExecuteNonQuery();
                            lblStatus.Text = "Demand Received Successfully";
                        }
                    }
                }
                BindGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            //if (FileUpload1.HasFile)
            //{
            //    string fileName = Path.GetFileName(FileUpload1.FileName);
            //    string fileDateString = Request.Form["fileDate"];
            //    DateTime fileDate;

            //    if (DateTime.TryParse(fileDateString, out fileDate))
            //    {
            //        lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
            //    }
            //    else
            //    {
            //        lblStatus.Text = "Invalid date format.";
            //    }

            //    SaveFileToDatabase(FileUpload1.PostedFile.InputStream, fileName, fileDate);
            //}
            //else
            //{
            //    lblStatus.Text = "Please select a file to upload.";
            //}
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

                    SqlDataAdapter da = new SqlDataAdapter("select DemandNo,BIY.ItemCode,iLueItem,BasicDenom,QtyIssued,DateIssued,IssueRefNo from BVYardIssue BIY INNER JOIN BasicLieuItems BS ON BIY.ItemCode=BS.ItemCode WHERE BIY.Status=0", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDemandIssue.DataSource = dt;
                    gvDemandIssue.DataBind();
                    //GridView.DataSource = dt;
                    //GridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}