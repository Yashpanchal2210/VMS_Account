using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VMS_1
{
    public partial class VictuallingManagement : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

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
                bool hasData = !string.IsNullOrEmpty(HTMLContentLiteralP2.Text);
                if (hasData)
                {
                    // Determine which buttons to show based on the role (assuming role logic from previous example)
                    string role = Session["Role"] as string;

                    if (role == "Store Keeper")
                    {
                        btnSendToLogistic.Visible = true;
                    }
                    else if (role == "Logistic Officer")
                    {
                        btnApprove1.Visible = true;
                        btnReject1.Visible = true;
                    }
                    else if (role == "Commanding Officer")
                    {
                        btnApprove2.Visible = true;
                        btnReject2.Visible = true;
                    }
                    // else handle other roles or default visibility as needed
                }
            }
        }

        protected void SendToLogoButton_Click(object sender, EventArgs e)
        {
            string selectedMonthYear = monthYearPicker.Value;
            UpdateSendToLogo(selectedMonthYear);
        }

        private void UpdateSendToLogo(string monthYear)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();

                    // Extract the month and year from the provided string
                    DateTime selectedDate = DateTime.ParseExact(monthYear, "yyyy-MM", null);
                    int selectedMonth = selectedDate.Month;
                    int selectedYear = selectedDate.Year;

                    // Check if there's existing data for the selected month and year
                    string checkQuery = "SELECT COUNT(*) FROM ApproveVMS WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) = @Year";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Month", selectedMonth);
                        checkCommand.Parameters.AddWithValue("@Year", selectedYear);

                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            // Update the existing record
                            string updateQuery = "UPDATE ApproveVMS SET SktoLogo = @SktoLogo, Logo = @Logo, LogoReject = @LogoReject, " +
                                                 "LogoApprove = @LogoApprove, LogotoCo = @LogotoCo, Co = @Co, " +
                                                 "CoReject = @CoReject, CoApprove = @CoApprove, IsApproved = @IsApproved " +
                                                 "WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) = @Year";

                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@SktoLogo", 1);
                                updateCommand.Parameters.AddWithValue("@Logo", 1);
                                updateCommand.Parameters.AddWithValue("@LogoReject", 0);
                                updateCommand.Parameters.AddWithValue("@LogoApprove", 0);
                                updateCommand.Parameters.AddWithValue("@LogotoCo", 0);
                                updateCommand.Parameters.AddWithValue("@Co", 0);
                                updateCommand.Parameters.AddWithValue("@CoReject", 0);
                                updateCommand.Parameters.AddWithValue("@CoApprove", 0);
                                updateCommand.Parameters.AddWithValue("@IsApproved", 0);

                                updateCommand.Parameters.AddWithValue("@Month", selectedMonth);
                                updateCommand.Parameters.AddWithValue("@Year", selectedYear);

                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "Report has been updated for the Logistic officer.";
                                }
                            }
                        }
                        else
                        {
                            // Insert new record
                            string reportNo = $"{selectedYear}{selectedMonth}{DateTime.Now.Day}"; // Adjust as needed

                            string insertQuery = "INSERT INTO ApproveVMS (SK, SktoLogo, Logo, LogoReject, LogoApprove, LogotoCo, Co, CoReject, CoApprove, IsApproved, ReportNumber, AddedDate) " +
                                                 "VALUES (@SK, @SktoLogo, @Logo, @LogoReject, @LogoApprove, @LogotoCo, @Co, @CoReject, @CoApprove, @IsApproved, @ReportNumber, @Date)";

                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@SK", 0);
                                insertCommand.Parameters.AddWithValue("@SktoLogo", 1);
                                insertCommand.Parameters.AddWithValue("@Logo", 1);
                                insertCommand.Parameters.AddWithValue("@LogoReject", 0);
                                insertCommand.Parameters.AddWithValue("@LogoApprove", 0);
                                insertCommand.Parameters.AddWithValue("@LogotoCo", 0);
                                insertCommand.Parameters.AddWithValue("@Co", 0);
                                insertCommand.Parameters.AddWithValue("@CoReject", 0);
                                insertCommand.Parameters.AddWithValue("@CoApprove", 0);
                                insertCommand.Parameters.AddWithValue("@IsApproved", 0);
                                insertCommand.Parameters.AddWithValue("@ReportNumber", reportNo);
                                insertCommand.Parameters.AddWithValue("@Date", selectedDate);

                                int rowsAffected = insertCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    lblMessage.Text = "Report has been forwarded to Logistic officer.";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle it appropriately
                lblMessage.Text = "An error occurred: " + ex.Message;
                throw; // Rethrow exception or handle as needed
            }
        }


        protected void GenerateHTMLViewButton_Click(object sender, EventArgs e)
        {
            string[] selectedDateP27 = monthYearPicker.Value.Split('-');
            string monthYear = monthYearPicker.Value;
            string COName = Request.Form["coVal"];
            string LOName = Request.Form["loVal"];
            string AOName = Request.Form["aoVal"];

            DateTime selectedDate = DateTime.ParseExact(monthYear, "yyyy-MM", null);
            string formattedMonthYear = selectedDate.ToString("MMMM yyyy");

            string commandingOfficer = COName;
            string logisticsOfficer = LOName;
            string accountingOfficer = AOName;
            string currentUser = HttpContext.Current.User.Identity.Name;
            string currentUserRank = Session["Rank"].ToString();
            string currentUserNudID = Session["NudId"].ToString();

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append("<table style='font-family: Arial; width: 100%; margin-top:10px;'>");

            // Title and content based on provided image
            htmlBuilder.Append($"<tr><td colspan='13' style='font-size: 20px; font-weight: bold; text-align: center;'>ACCOUNT OF VICTUALLING STORES FOR THE MONTH OF {formattedMonthYear}</td></tr>");

            htmlBuilder.Append("<tr><td>Name and Rank of the Commanding Officer:</td><td colspan='2'>" + commandingOfficer + "</td></tr>");
            htmlBuilder.Append("<tr><td>Name and the Rank of the Logistics Officer:</td><td colspan='2'>" + logisticsOfficer + "</td></tr>");
            htmlBuilder.Append("<tr><td>Name and Rank of the officer directly responsible for keeping the Account:</td><td colspan='2'>" + accountingOfficer + "</td></tr>");

            // Instructions
            htmlBuilder.Append("<tr><td colspan='13' style='font-weight: bold; text-align: center;'>INSTRUCTIONS</td></tr>");

            htmlBuilder.Append("<tr><td colspan='13'>1. The working or rough copy (form IN - 213) of this account is to be written up in ink daily. On completion of the account the following action is to be taken:-</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>On completion of the account the following action is to be taken:-</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>Ships and establishments carrying their own accounts are to check the working or rough copy and prepare a fair copy for audit. All relevant vouchers and certificates are to accompany the account.</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>All relevant vouchers and certificates are to accompany the account.</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>Ships and establishments NOT carrying their own accounts are to forward the working OR rough copy of the account to BLOGO together with all relevant vouchers. </td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>After checking and completion of the account the BLOGO is to prepare a fair copy for audit. All relevant vouchers and certificates are to accompany the account.</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>All relevant vouchers and certificates are to accompany the account.</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>2. The quantities remaining as per muster, at the end of the each month in line 28 to be carried forward to the new account in line 4.</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>Differences discovered between the actual quantities after muster and the balance as per account are to be entered on line 29, Pages 2 to 7. The certificates on</td></tr>");

            // Details of Enclosures
            htmlBuilder.Append("<tr><td colspan='13' style='font-weight: bold; text-align: center;'>DETAILS OF ENCLOSURES</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13'>AS PER ATTACHED LIST</td></tr>");

            // Footer
            htmlBuilder.Append("<tr><td>Account written up by (Name):</td><td colspan='2'>" + Session["UserName"] + "</td><td>Rank:</td><td>" + Session["Rank"] + "</td><td>No:</td><td>" + Session["NudId"] + "</td></tr>");
            htmlBuilder.Append("<tr><td>Account Checked up by (Name):</td><td colspan='2'></td><td>Rank:</td><td></td><td>No:</td><td></td></tr>");

            htmlBuilder.Append("</table>");

            // Render the HTML content on the frontend
            HTMLContentLiteralP2.Text = htmlBuilder.ToString();
            GenerateHTMLViewP18(selectedDateP27);
            GenerateHTMLViewP8();
            GenerateHTMLViewP2_7(selectedDateP27);
            GenerateHTMLViewSailorWorksheet(selectedDateP27);
            GenerateHTMLViewOfficerWorksheet(selectedDateP27);
            //BindData(selectedDateP27);

            bool hasData = !string.IsNullOrEmpty(HTMLContentLiteralP2.Text);
            if (hasData)
            {
                // Determine which buttons to show based on the role (assuming role logic from previous example)
                string role = Session["Role"] as string;

                if (role == "Store Keeper")
                {
                    btnSendToLogistic.Visible = true;
                }
                else if (role == "Logistic Officer")
                {
                    btnApprove1.Visible = true;
                    btnReject1.Visible = true;
                }
                else if (role == "Commanding Officer")
                {
                    btnApprove2.Visible = true;
                    btnReject2.Visible = true;
                }
                // else handle other roles or default visibility as needed
            }
        }


        #region Page2To7
        protected void GenerateHTMLViewP2_7(string[] selectedDate)
        {
            // Fetch InLieuItem and Denomination data
            DataTable inLieuItemsTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "SELECT InLieuItem, Denomination FROM InLieuItems";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(inLieuItemsTable);
                }
            }

            // Fetch ReceiptMaster data month-wise
            DataTable receiptMasterTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM ReceiptMaster WHERE MONTH(Dates) = @Month AND YEAR(Dates) = @Year";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Month", selectedDate[1]); // Assuming selectedDate contains month and year
                    command.Parameters.AddWithValue("@Year", selectedDate[0]);
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(receiptMasterTable);
                }
            }
            //FEtch Month End stock
            DataTable monthEndStockData = GetPreviousMonthData(selectedDate);

            //Fetch Fresh Data
            DataTable presentFreshStock = GetFreshStockData(selectedDate);

            // Generate HTML tables
            string htmlTables = "<div class='table-container' style='overflow-x: auto; display: flex; flex-wrap: nowrap;'>";
            int itemsPerPage = 32; // Number of items to display per page
            int totalPages = 7; // Adjust as needed

            //for (int page = 2; page < 9; page++) // Loop to create tables for each page
            //{
            htmlTables += "<table border='1' width='100%' style='text-align:center; margin-right:10px;'>";
            //htmlTables += $"<tr><th colspan='11'>Page {page.ToString().PadLeft(2, '0')}</th></tr>";

            // Row for InLieuItem
            htmlTables += "<tr><th>Line No.</th><th></th>"; // Added two empty headers for the first two columns
            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                htmlTables += $"<th>{row["InLieuItem"]}</th>";
            }
            htmlTables += "</tr>";

            // Row for Denomination
            //htmlTables += "<tr><th>Denomination</th>";
            htmlTables += "<tr><th></th><th></th>";
            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                htmlTables += $"<th>{row["Denomination"]}</th>";
            }
            htmlTables += "</tr>";

            // Rows for ReceiptMaster data

            htmlTables += "<tr><th>1</th><th>FULL STOWAGE</th>";
            foreach (DataRow receiptRow in inLieuItemsTable.Rows)
            {
                htmlTables += "<th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr><th>2</th><th>STOCK NOT TO FALL BELOW</th>";
            foreach (DataRow receiptRow in inLieuItemsTable.Rows)
            {
                htmlTables += "<th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr><th>3</th><th>BALANCE AS PER ACCOUNT</th>";
            foreach (DataRow receiptRow in inLieuItemsTable.Rows)
            {
                if (monthEndStockData.Rows.Count > 0)
                {
                    foreach (DataRow row in monthEndStockData.Rows)
                    {
                        string itemName = row["ItemName"].ToString();
                        // Find the row in receiptMasterTable where ItemName matches InLieuItem and referenceNos matches
                        if (row["ItemName"].Equals(receiptRow["InLieuItem"]))
                        {
                            htmlTables += $"<th>{receiptRow["quantities"]}</th>";
                        }
                        else
                        {
                            htmlTables += "<th>0</th>"; // If no match found, display empty cell
                        }
                    }
                }
                else
                {
                    htmlTables += "<th>0</th>"; // If no match found, display empty cell
                }
            }
            htmlTables += "</tr>";


            int lineNumber = 4;
            foreach (DataRow receiptRow in receiptMasterTable.Rows)
            {
                htmlTables += "<tr>";
                htmlTables += $"<td>{lineNumber}</td>"; // Display line number in the first column
                htmlTables += $"<td>{receiptRow["referenceNos"]}</td>"; // Display referenceNos in the second column

                foreach (DataRow row in inLieuItemsTable.Rows)
                {
                    string itemName = row["InLieuItem"].ToString();
                    // Find the row in receiptMasterTable where ItemName matches InLieuItem and referenceNos matches
                    if (row["InLieuItem"].Equals(receiptRow["itemnames"]))
                    {
                        htmlTables += $"<td>{receiptRow["quantities"]}</td>";
                    }
                    else
                    {
                        htmlTables += "<td></td>"; // If no match found, display empty cell
                    }
                }

                htmlTables += "</tr>";
                lineNumber++;
            }

            foreach (DataRow receiptRow in receiptMasterTable.Rows)
            {
                htmlTables += "<tr>";
                htmlTables += $"<td>{lineNumber}</td>"; // Display line number in the first column
                htmlTables += $"<td>{receiptRow["referenceNos"]}</td>"; // Display referenceNos in the second column

                foreach (DataRow row in inLieuItemsTable.Rows)
                {
                    string itemName = row["InLieuItem"].ToString();
                    // Find the row in receiptMasterTable where ItemName matches InLieuItem and referenceNos matches
                    if (row["InLieuItem"].Equals(receiptRow["itemnames"]))
                    {
                        htmlTables += $"<td>{receiptRow["quantities"]}</td>";
                    }
                    else
                    {
                        htmlTables += "<td></td>"; // If no match found, display empty cell
                    }
                }

                htmlTables += "</tr>";
                lineNumber++;
            }

            htmlTables += $"<tr><th>{lineNumber}</th><th>FRESH RECEIPTS FROM Pg - 12</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow receiptRow in presentFreshStock.Rows)
                {
                    if (row["InLieuItem"].Equals(receiptRow["itemnames"]))
                    {
                        int quantity = Convert.ToInt32(receiptRow["quantities"]);
                        htmlTables += $"<th>{quantity}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th>0</th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // Add the sum row for "receiptMasterTable" and "presentFreshStock"
            htmlTables += $"<tr><th>{lineNumber}</th><th>TOTAL RECEIPTS</th>";
            for (int i = 0; i < inLieuItemsTable.Rows.Count; i++)
            {
                decimal totalval = 0;
                foreach (DataRow row in receiptMasterTable.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalval += Convert.ToDecimal(row["quantities"]);
                    }
                }
                foreach (DataRow row in presentFreshStock.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalval += Convert.ToDecimal(row["quantities"]);
                    }
                }

                htmlTables += $"<td>{totalval}</td>";
            }
            htmlTables += "</tr>";
            lineNumber++;
            //Fetch Fresh Data
            DataTable SailorIssueData = GetSailorIssueData(selectedDate);

            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE TO SHIP'S COY</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow sailorRow in SailorIssueData.Rows)
                {
                    if (row["InLieuItem"].Equals(sailorRow["ItemName"]))
                    {
                        htmlTables += $"<th>{sailorRow["QtyIssued"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;


            //Officer Issue Data
            DataTable officerIssueData = GetOfficerIssueData(selectedDate);

            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE TO OFFICERS</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow officerIssue in officerIssueData.Rows)
                {
                    if (row["InLieuItem"].Equals(officerIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{officerIssue["QtyIssued"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            //Officer Issue Data
            DataTable extraIssueData = GetExtraIssueData(selectedDate);

            htmlTables += $"<tr><th>{lineNumber}</th><th>EXTRA ISSUES from  Pg - 09</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow extraIssue in extraIssueData.Rows)
                {
                    if (row["InLieuItem"].Equals(extraIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{extraIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // RationPaymentIssue
            DataTable PaymentIssue = GetRationPaymentIssueData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>PAYMENT ISSUE from Pg.10/11</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow paymentIssue in PaymentIssue.Rows)
                {
                    if (row["InLieuItem"].Equals(paymentIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{paymentIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // GetPatientsData
            DataTable PatientsData = GetPatientsData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE TO PATIENTS</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow patientIssue in PatientsData.Rows)
                {
                    if (row["InLieuItem"].Equals(patientIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{patientIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // GetOtherShipData
            DataTable OtherShipData = GetOtherShipData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE TO OTHER SHIPS</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow otherShipIssue in OtherShipData.Rows)
                {
                    if (row["InLieuItem"].Equals(otherShipIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{otherShipIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // GetDiversIssueData
            DataTable DiversIssueData = GetDiversIssueData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE TO DIVERS</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow diverShipIssue in DiversIssueData.Rows)
                {
                    if (row["InLieuItem"].Equals(diverShipIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{diverShipIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;

            // WASTAGE
            DataTable WastageData = GetWastageData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>WASTAGE</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                bool matchFound = false;
                foreach (DataRow wastageIssue in WastageData.Rows)
                {
                    if (row["InLieuItem"].Equals(wastageIssue["ItemName"]))
                    {
                        htmlTables += $"<th>{wastageIssue["Qty"]}</th>";
                        matchFound = true;
                        break;
                    }
                }
                if (!matchFound)
                {
                    htmlTables += "<th></th>";
                }
            }
            htmlTables += "</tr>";
            lineNumber++;
            /// Total Issue 
            htmlTables += $"<tr><th>{lineNumber}</th><th>ISSUE GRAND TOTAL</th>";
            for (int i = 0; i < inLieuItemsTable.Rows.Count; i++)
            {
                decimal totalIssue = 0;
                foreach (DataRow row in SailorIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in officerIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in extraIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PaymentIssue.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PatientsData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in OtherShipData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in DiversIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in WastageData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalIssue += Convert.ToDecimal(row["Qty"]);
                    }
                }

                htmlTables += $"<td>{totalIssue}</td>";
            }
            htmlTables += "</tr>";
            lineNumber++;


            /// BALANCE AS PER ACCOUNT
            htmlTables += $"<tr><th>{lineNumber}</th><th>BALANCE AS PER ACCOUNT</th>";
            for (int i = 0; i < inLieuItemsTable.Rows.Count; i++)
            {
                decimal total = 0, totalReceipt = 0;
                foreach (DataRow row in SailorIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in officerIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in extraIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PaymentIssue.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PatientsData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in OtherShipData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in DiversIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in WastageData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }


                //---------------//total Issue

                totalReceipt = 0;
                foreach (DataRow row in receiptMasterTable.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalReceipt += Convert.ToDecimal(row["quantities"]);
                    }
                }
                foreach (DataRow row in presentFreshStock.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalReceipt += Convert.ToDecimal(row["quantities"]);
                    }
                }
                htmlTables += $"<td>{totalReceipt - total}</td>";

            }
            htmlTables += "</tr>";
            lineNumber++;

            /// BALANCE AS PER ACCOUNT
            htmlTables += $"<tr><th>{lineNumber}</th><th>BALANCE AS PER MUSTER</th>";
            for (int i = 0; i < inLieuItemsTable.Rows.Count; i++)
            {
                decimal total = 0; decimal totalReceipt = 0;
                foreach (DataRow row in SailorIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in officerIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["QtyIssued"]);
                    }
                }
                foreach (DataRow row in extraIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PaymentIssue.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in PatientsData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in OtherShipData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in DiversIssueData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }
                foreach (DataRow row in WastageData.Rows)
                {
                    if (row["ItemName"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        total += Convert.ToDecimal(row["Qty"]);
                    }
                }


                //---------------//total Issue


                foreach (DataRow row in receiptMasterTable.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalReceipt += Convert.ToDecimal(row["quantities"]);
                    }
                }
                foreach (DataRow row in presentFreshStock.Rows)
                {
                    if (row["itemnames"].ToString() == inLieuItemsTable.Rows[i]["InLieuItem"].ToString())
                    {
                        totalReceipt += Convert.ToDecimal(row["quantities"]);
                    }
                }
                htmlTables += $"<td>{totalReceipt - total}</td>";

            }
            htmlTables += "</tr>";
            lineNumber++;

            // CRV / LOSS STATEMENT
            DataTable LossSttement = GetWastageData(selectedDate);
            htmlTables += $"<tr><th>{lineNumber}</th><th>CRV / LOSS STATEMENT</th>";

            foreach (DataRow row in inLieuItemsTable.Rows)
            {
                htmlTables += "<th></th>";
            }
            htmlTables += "</tr>";
            lineNumber++;



            // Close the table and div tags
            htmlTables += "</table></div>";

            // Assign the generated HTML to the container
            tablesContainerPage2to7.InnerHtml = htmlTables;


            //tablesContainerPage2to7.InnerHtml = htmlTables;
        }


        private DataTable GetPreviousMonthData(string[] previousMonthDate)
        {
            int preveiousMonth = DateTime.Now.Month;
            int previousYear = DateTime.Now.Year;
            if (previousMonthDate != null && previousMonthDate.Length > 1)
            {
                int month = Convert.ToInt32(previousMonthDate[1]);
                if (month > 1)
                {
                    preveiousMonth = month - 1;
                    previousYear = Convert.ToInt32(previousMonthDate[0]);
                }
                else
                {
                    preveiousMonth = 12;
                    previousYear = previousYear - 1;
                }
            }


            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", Convert.ToString(preveiousMonth));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToString(previousYear));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetPresentStockData()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 72 InLieuItem, Denomination FROM InLieuItems", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOfficerIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM IssueMaster Where Role = @role AND MONTH(Date) = @Month AND Year(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    cmd.Parameters.AddWithValue("@role", "Wardroom");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetSailorIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM IssueMaster Where Role = @role AND MONTH(Date) = @Month AND Year(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    cmd.Parameters.AddWithValue("@role", "Galley");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetReceiptData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ReceiptMaster WHERE MONTH(Dates) = @Month AND YEAR(Dates) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetWastageData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Wastage WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetRationPaymentIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Wastage WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetDiversIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ItemName,SUM(Qty)Qty FROM ExtraIssue WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOtherShipData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM OtherShips WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetPatientsData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetExtraIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ItemName, SUM(CONVERT(decimal(18, 3), Qty)) AS Qty FROM ExtraIssueCategory WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetRationPaymentData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ItemName, SUM(CONVERT(decimal(18, 3), Qty)) AS Qty FROM RationIssuePayment WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetFreshStockData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("select Dates, itemnames, quantities, referenceNos from ReceiptMaster Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year order by Dates ASC", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    cmd.Parameters.AddWithValue("@role", "Wardroom");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        #endregion 

        #region Page 8
        protected void GenerateHTMLViewP8()
        {
            StringBuilder htmlBuilder = new StringBuilder();

            // Start the table with Arial font
            htmlBuilder.Append("<table style='font-family: Arial; width: 100%; border-collapse: collapse;'>");

            // Title row
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td colspan='13' style='font-size: 12px; font-weight: bold; text-align: center; border: 1px solid black;'>PAGE - 8</td>");
            htmlBuilder.Append("</tr>");

            // Second title row
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td colspan='13' style='font-size: 12px; font-weight: bold; text-align: center; border: 1px solid black;'>PACKING MATERIAL ACCOUNT</td>");
            htmlBuilder.Append("</tr>");

            // Header row
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>TINS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>TINS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>TEA</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>CARTON</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>PLASTIC</td>");
            htmlBuilder.Append("</tr>");

            // Sub-header row
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>LINE</td>");
            htmlBuilder.Append("<td colspan='3' style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>2MD</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>2MD</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>ASSOR-</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>1 MD</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>Misc</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>15 KG</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BAGS</td>");
            htmlBuilder.Append("</tr>");

            // Denominations and initial data
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NO.</td>");
            htmlBuilder.Append("<td colspan='3' style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>BTSA</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>ATSA</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>TED</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>ATSA</td>");
            htmlBuilder.Append("</tr>");

            // Denomination header row
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<td colspan='3' style='border: 1px solid black;'>DENOM-></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
            htmlBuilder.Append("<td style='border: 1px solid black;'>NOS</td>");
            htmlBuilder.Append("</tr>");

            // Line number rows
            for (int i = 1; i <= 29; i++)
            {
                htmlBuilder.Append("<tr>");
                htmlBuilder.Append($"<td style='border: 1px solid black;'>{i}</td>");
                for (int j = 0; j < 12; j++)
                {
                    htmlBuilder.Append("<td style='border: 1px solid black;'></td>");
                }
                htmlBuilder.Append("</tr>");
            }

            // Footer rows
            htmlBuilder.Append("<tr><td colspan='2' style='font-weight: bold; border: 1px solid black;'>BALANCE  B/F FROM LAST A/C</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>RECEIPTS (VR. NOS.)</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='font-weight: bold; border: 1px solid black;'>TOTAL RECEIPTS</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>ISSUE TO OTHER SHIPS</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>USED FOR SANITARY PURPOSE</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='font-weight: bold; border: 1px solid black;'>ISSUES GRAND TOTAL</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>BALANCE AS PER ACCOUNT</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>QTY AS PER MUSTER</td><td colspan='11' style='border: 1px solid black;'></td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='border: 1px solid black;'>DIFFERENCES</td><td colspan='11' style='border: 1px solid black;'></td></tr>");

            // End the table
            htmlBuilder.Append("</table>");

            // Render the HTML content on the frontend
            HTMLContentLiteralP8.Text = htmlBuilder.ToString();
        }
        #endregion

        #region Page18
        protected void GenerateHTMLViewP18(string[] selectedDate)
        {
            string monthYear = monthYearPicker.Value;
            string COName = Request.Form["coVal"];
            string LOName = Request.Form["loVal"];
            string AOName = Request.Form["aoVal"];

            string[] formattedMonthYear = selectedDate;

            string commandingOfficer = COName;
            string logisticsOfficer = LOName;
            string accountingOfficer = AOName;
            string currentUser = HttpContext.Current.User.Identity.Name;
            string currentUserRank = Session["Rank"].ToString();
            string currentUserNudID = Session["NudId"].ToString();

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append("<table style='font-family: Arial; width: 100%; border-collapse: collapse;'>");

            // Title and content based on provided image
            htmlBuilder.Append("<tr><td colspan='13' style='font-size: 12px; font-weight: bold; text-align: center;'>Page - 14</td></tr>");

            htmlBuilder.Append("<tr><td colspan='13' style='font-size: 12px; font-weight: bold; text-decoration: underline; text-align: center;'>CERTIFICATE NO. 1 OF MUSTER</td></tr>");
            htmlBuilder.Append($"<tr><td colspan='8' style='text-align: center;'>Victualling stores have been mustered by the Officer-in-Charge on {formattedMonthYear} and found correct.</td></tr>");

            htmlBuilder.Append("<tr><td colspan='3' style='text-align: center;'>Signature and Rank of Accounting Officer.</td><td colspan='2' style='text-align: left;'>(" + logisticsOfficer + ")</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Lieutenant Commander</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Logistics Officer</td></tr>");

            htmlBuilder.Append("<tr><td colspan='13' style='font-size: 12px; font-weight: bold; text-decoration: underline; text-align: center;'>CERTIFICATE NO. 2 OF CREDIT OF GOVT. CASH RECOVERIES</td></tr>");
            htmlBuilder.Append("<tr><td colspan='13' style='text-align: left;'>Certified that the sum of Rs.Nil on account of payment issues as details on page 10 & 11 has been credited by the Logistics Officer of the Ship through MRO vide MRO No. Nil as per details given therein.</td></tr>");

            htmlBuilder.Append("<tr><td colspan='8' style='text-align: left;'>Signature and Rank of Officer maintaining cash A/c</td><td colspan='2' style='text-align: left;'>(" + logisticsOfficer + ")</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Lieutenant Commander</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Logistics Officer</td></tr>");

            htmlBuilder.Append("<tr><td colspan='4' style='text-align: left; font-weight: bold;'>APPROVED AND CERTIFIED THAT :-</td></tr>");
            htmlBuilder.Append("<tr><td>(i)</td><td colspan='10'>The extra issues shown on page 9 have been made under Commanding Officer's authority.</td></tr>");
            htmlBuilder.Append("<tr><td>(ii)</td><td colspan='10'>The quantity of packing material charged off for sanitary purposes were issued under Commanding Officer's authority.</td></tr>");
            htmlBuilder.Append("<tr><td>(iii)</td><td colspan='10'>The remains as shown in this account are correct to the best of my knowledge and belief.</td></tr>");
            htmlBuilder.Append("<tr><td>(iv)</td><td colspan='10'>The quarterly muster of sailors has been carried out in accordance with Reg. No.0353 of Regs Navy.</td></tr>");
            htmlBuilder.Append("<tr><td>(v)</td><td colspan='10'>The periodicity of issue of fish has been restricted in accordance with NI 2/98 as amended from time to time.</td></tr>");

            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Signature of the Commanding Officer</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>(" + commandingOfficer + ")</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Commodore</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Commanding Officer</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>INS Hamla</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Marve Malad(W)</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Mumbai-95</td></tr>");
            htmlBuilder.Append("<tr><td colspan='2' style='text-align: left;'>Date:</td></tr>");

            htmlBuilder.Append("</table>");

            // Render the HTML content on the frontend
            HTMLContentLiteralP14.Text = htmlBuilder.ToString();
        }
        #endregion

        #region Sailor

        private DataTable GetSailorGENData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegSailor)vegSailor,SUM(nonVegSailor)nonVegSailor,(SUM(vegSailor)+SUM(nonVegSailor)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetSailorRikData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegSailorRik)vegSailorRik,SUM(nonVegSailorRik)nonVegSailorRik,(SUM(vegSailorRik)+SUM(nonVegSailorRik)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetSailorEntitledData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegNonEntitledSailor)vegNonEntitledSailor,SUM(NonVegNonEntitledSailor)NonVegNonEntitledSailor,(SUM(vegNonEntitledSailor)+SUM(NonVegNonEntitledSailor)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetSailorTotalData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT (SUM(vegSailor)+SUM(vegSailorRik)+SUM(vegNonEntitledOfficer))TotalNonVeg, (SUM(nonVegSailor) + SUM(nonVegSailorRik) + SUM(nonVegNonEntitledOfficer))TotalVeg,(SUM(vegSailor) + SUM(vegSailorRik) + SUM(vegNonEntitledOfficer) + SUM(nonVegSailor) + SUM(nonVegSailorRik) + SUM(nonVegNonEntitledOfficer))Total  FROM[dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetSailorBasicItemsData()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from BasicItems WHERE Category='Officer'", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetSailorInLieuItemsData(int BasicItemId)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from InLieuItems WHERE Category='Officer' AND BasicItemId=@BasicItemId", conn))
                {
                    cmd.Parameters.AddWithValue("@BasicItemId", BasicItemId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetSailorIssueItemsData(int BasicItemId)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IM.ItemName,SUM(convert(decimal,IM.QtyIssued))QtyIssued FROM IssueMaster IM INNER JOIN InLieuItems INL ON  IM.ItemId=INL.Id WHERE INL.BasicItemId=@BasicItemId AND IM.Role='Wardroom' GROUP BY month(IM.Date),IM.ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@BasicItemId", BasicItemId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        protected void GenerateHTMLViewSailorWorksheet(string[] selectedDate)
        {

            // Generate HTML tables
            string htmlTables = "<div class='table-container' style='overflow-x: auto; display: flex; flex-wrap: nowrap;'>";
            htmlTables += "<table border='1' width='100%' style='text-align:center; margin-right:10px;'>";

            // Row for InLieuItem
            htmlTables += "<tr><th></th><th></th><th>'S'</th><th></th><th>'V'</th><th></th><th> Total</th><th></th><th></th>"; // Added two empty headers for the first two columns
            htmlTables += "</tr>";
            htmlTables += "<tr>";
            DataTable OfficerGEN = GetSailorGENData(selectedDate);
            if (OfficerGEN.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>GEN</th><th>" + OfficerGEN.Rows[0]["nonVegSailor"] + "</th><th></th><th>" + OfficerGEN.Rows[0]["vegSailor"] + "</th><th></th><th>" + OfficerGEN.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>GEN</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";
            htmlTables += "<tr>";
            DataTable OfficerRik = GetSailorRikData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>RIK + HON. OFFICER</th><th>" + OfficerRik.Rows[0]["nonVegSailorRik"] + "</th><th></th><th>" + OfficerRik.Rows[0]["vegSailorRik"] + "</th><th></th><th>" + OfficerRik.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>RIK + HON. OFFICER</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr>";
            DataTable OfficerEnti = GetSailorEntitledData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>NON ENTITLED MESSING</th><th>" + OfficerEnti.Rows[0]["NonVegNonEntitledSailor"] + "</th><th></th><th>" + OfficerEnti.Rows[0]["vegNonEntitledSailor"] + "</th><th></th><th>" + OfficerEnti.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>NON ENTITLED MESSING</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr>";
            DataTable OfficerTotal = GetSailorTotalData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>Total</th><th>" + OfficerTotal.Rows[0]["TotalNonVeg"] + "</th><th></th><th>" + OfficerTotal.Rows[0]["TotalVeg"] + "</th><th></th><th>" + OfficerTotal.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>Total</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr><th>Ser.</th><th>Item</th><th>Strength</th><th></th><th>Scale</th><th></th><th>Qty Entitled</th><th></th><th>Qty Issued</th>";
            htmlTables += "</tr>";

            DataTable OfficeBasicItems = GetSailorBasicItemsData();
            int srno = 1;
            foreach (DataRow item in OfficeBasicItems.Rows)
            {
                htmlTables += "<tr>";
                htmlTables += $"<td>{srno}</td>";
                htmlTables += $"<td>{item["BasicItem"]}</td>";
                htmlTables += $"<td>{OfficerTotal.Rows[0]["Total"]}</td>";
                htmlTables += $"<td>x</td>";
                htmlTables += $"<td>{item["VegScale"]}</td>";
                htmlTables += $"<td>=</td>";
                htmlTables += $"<td>{Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"])}</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += "</tr>";
                DataTable OfficeInLieuItems = GetSailorInLieuItemsData(Convert.ToInt32(item["Id"]));
                foreach (DataRow itemIn in OfficeInLieuItems.Rows)
                {
                    htmlTables += "<tr>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>{itemIn["InLieuItem"]}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>=</td>";
                    htmlTables += $"<td>{0.00}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += "</tr>";
                }
                //row=15;
                htmlTables += "<tr>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>Total</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>=</td>";
                htmlTables += $"<td>{Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"])}</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += "</tr>";

                DataTable OfficeIssueItems = GetSailorIssueItemsData(Convert.ToInt32(item["Id"]));
                decimal TotalQtyIssued = 0M;
                foreach (DataRow itemIn in OfficeIssueItems.Rows)
                {
                    htmlTables += "<tr>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>{itemIn["ItemName"]}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>=</td>";
                    htmlTables += $"<td>{itemIn["QtyIssued"]}</td>";
                    htmlTables += "</tr>";
                }
                htmlTables += "<tr>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>Total</td>";
                htmlTables += $"<td>{TotalQtyIssued}</td>";
                htmlTables += "</tr>";
                srno++;
            }

            htmlTables += "</table></div>";

            // Assign the generated HTML to the container
            tablesContainerPageSailorWorksheet.InnerHtml = htmlTables;

        }

        #endregion

        #region Officer

        private DataTable GetOfficerGENData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegOfficers)vegOfficers,SUM(nonVegOfficers)nonVegOfficers,(SUM(vegOfficers)+SUM(nonVegOfficers)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetOfficerRikData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegrikOfficers)vegrikOfficers,SUM(nonVegrikOfficers)nonVegrikOfficers,(SUM(vegrikOfficers)+SUM(nonVegrikOfficers)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetOfficerEntitledData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegNonEntitledOfficer)vegNonEntitledOfficer,SUM(nonVegNonEntitledOfficer)nonVegNonEntitledOfficer,(SUM(vegNonEntitledOfficer)+SUM(nonVegNonEntitledOfficer)) Total FROM [dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOfficeTotalData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT (SUM(vegOfficers)+SUM(vegrikOfficers)+SUM(vegNonEntitledOfficer))TotalNonVeg, (SUM(nonVegOfficers) + SUM(nonVegrikOfficers) + SUM(nonVegNonEntitledOfficer))TotalVeg,(SUM(vegOfficers) + SUM(vegrikOfficers) + SUM(vegNonEntitledOfficer) + SUM(nonVegOfficers) + SUM(nonVegrikOfficers) + SUM(nonVegNonEntitledOfficer))Total  FROM[dbo].[strength]  Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year GROUP BY Month(Dates)", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOfficeBasicItemsData()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from BasicItems WHERE Category='Officer'", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetOfficeInLieuItemsData(int BasicItemId)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("select * from InLieuItems WHERE Category='Officer' AND BasicItemId=@BasicItemId", conn))
                {
                    cmd.Parameters.AddWithValue("@BasicItemId", BasicItemId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        private DataTable GetOfficeIssueItemsData(int BasicItemId)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IM.ItemName,SUM(convert(decimal,IM.QtyIssued))QtyIssued FROM IssueMaster IM INNER JOIN InLieuItems INL ON  IM.ItemId=INL.Id WHERE INL.BasicItemId=@BasicItemId AND IM.Role='Wardroom' GROUP BY month(IM.Date),IM.ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@BasicItemId", BasicItemId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        protected void GenerateHTMLViewOfficerWorksheet(string[] selectedDate)
        {

            // Generate HTML tables
            string htmlTables = "<div class='table-container' style='overflow-x: auto; display: flex; flex-wrap: nowrap;'>";
            htmlTables += "<table border='1' width='100%' style='text-align:center; margin-right:10px;'>";

            // Row for InLieuItem
            htmlTables += "<tr><th></th><th></th><th>'S'</th><th></th><th>'V'</th><th></th><th> Total</th><th></th><th></th>"; // Added two empty headers for the first two columns
            htmlTables += "</tr>";
            htmlTables += "<tr>";
            DataTable OfficerGEN = GetOfficerGENData(selectedDate);
            if (OfficerGEN.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>GEN</th><th>" + OfficerGEN.Rows[0]["nonVegOfficers"] + "</th><th></th><th>" + OfficerGEN.Rows[0]["vegOfficers"] + "</th><th></th><th>" + OfficerGEN.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>GEN</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";
            htmlTables += "<tr>";
            DataTable OfficerRik = GetOfficerRikData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>RIK + HON. OFFICER</th><th>" + OfficerRik.Rows[0]["nonVegrikOfficers"] + "</th><th></th><th>" + OfficerRik.Rows[0]["vegrikOfficers"] + "</th><th></th><th>" + OfficerRik.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>RIK + HON. OFFICER</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr>";
            DataTable OfficerEnti = GetOfficerEntitledData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>NON ENTITLED MESSING</th><th>" + OfficerEnti.Rows[0]["nonVegNonEntitledOfficer"] + "</th><th></th><th>" + OfficerEnti.Rows[0]["vegNonEntitledOfficer"] + "</th><th></th><th>" + OfficerEnti.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>NON ENTITLED MESSING</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr>";
            DataTable OfficerTotal = GetOfficeTotalData(selectedDate);
            if (OfficerRik.Rows.Count > 0)
            {
                htmlTables += "<th></th><th>Total</th><th>" + OfficerTotal.Rows[0]["TotalNonVeg"] + "</th><th></th><th>" + OfficerTotal.Rows[0]["TotalVeg"] + "</th><th></th><th>" + OfficerTotal.Rows[0]["Total"] + "</th><th></th><th></th>";
            }
            else
            {
                htmlTables += "<th></th><th>Total</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th>";
            }
            htmlTables += "</tr>";

            htmlTables += "<tr><th>Ser.</th><th>Item</th><th>Strength</th><th></th><th>Scale</th><th></th><th>Qty Entitled</th><th></th><th>Qty Issued</th>";
            htmlTables += "</tr>";

            DataTable OfficeBasicItems = GetOfficeBasicItemsData();
            int srno = 1;
            foreach (DataRow item in OfficeBasicItems.Rows)
            {
                htmlTables += "<tr>";
                htmlTables += $"<td>{srno}</td>";
                htmlTables += $"<td>{item["BasicItem"]}</td>";
                htmlTables += $"<td>{OfficerTotal.Rows[0]["Total"]}</td>";
                htmlTables += $"<td>x</td>";
                htmlTables += $"<td>{item["VegScale"]}</td>";
                htmlTables += $"<td>=</td>";
                htmlTables += $"<td>{Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"])}</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += "</tr>";
                DataTable OfficeInLieuItems = GetOfficeInLieuItemsData(Convert.ToInt32(item["Id"]));
                foreach (DataRow itemIn in OfficeInLieuItems.Rows)
                {
                    htmlTables += "<tr>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>{itemIn["InLieuItem"]}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>=</td>";
                    htmlTables += $"<td>{0.00}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += "</tr>";
                }
                //row=15;
                htmlTables += "<tr>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>Total</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>=</td>";
                htmlTables += $"<td>{Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"])}</td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += "</tr>";

                DataTable OfficeIssueItems = GetOfficeIssueItemsData(Convert.ToInt32(item["Id"]));
                decimal TotalQtyIssued = 0M;
                foreach (DataRow itemIn in OfficeIssueItems.Rows)
                {
                    htmlTables += "<tr>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>{itemIn["ItemName"]}</td>";
                    htmlTables += $"<td></td>";
                    htmlTables += $"<td>=</td>";
                    htmlTables += $"<td>{itemIn["QtyIssued"]}</td>";
                    htmlTables += "</tr>";
                }
                htmlTables += "<tr>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td></td>";
                htmlTables += $"<td>Total</td>";
                htmlTables += $"<td>{TotalQtyIssued}</td>";
                htmlTables += "</tr>";
                srno++;
            }

            htmlTables += "</table></div>";

            // Assign the generated HTML to the container
            tablesContainerPageOfficerWorksheet.InnerHtml = htmlTables;

        }

        #endregion


    }
}