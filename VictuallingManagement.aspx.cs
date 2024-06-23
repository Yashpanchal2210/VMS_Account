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
            int lineNumber = 1;
            foreach (DataRow receiptRow in receiptMasterTable.Rows)
            {
                htmlTables += "<tr>";
                htmlTables += $"<td>{lineNumber}</td>"; // Display line number in the first column
                htmlTables += $"<td>{receiptRow["referenceNos"]}</td>"; // Display referenceNos in the second column

                foreach (DataRow row in inLieuItemsTable.Rows)
                {
                    string itemName = row["InLieuItem"].ToString();
                    // Find the row in receiptMasterTable where ItemName matches InLieuItem and referenceNos matches
                    DataRow[] matchingRows = receiptMasterTable.Select($"itemnames = '{itemName}' AND referenceNos = '{receiptRow["referenceNos"]}'");
                    if (matchingRows.Length > 0)
                    {
                        // Assuming Quantity is the column name for the quantity data
                        htmlTables += $"<td>{matchingRows[0]["quantities"]}</td>";
                    }
                    else
                    {
                        htmlTables += "<td></td>"; // If no match found, display empty cell
                    }
                }

                htmlTables += "</tr>";
                lineNumber++;
            }



            htmlTables += "</table>";
            //}
            htmlTables += "</div>";

            tablesContainerPage2to7.InnerHtml = htmlTables;
        }

        //protected void GenerateHTMLViewP2_7(string[] selectedDate)
        //{
        //    DataTable monthEndStockData = GetPreviousMonthData(selectedDate);

        //    // Fetch data from the PresentStockMaster table
        //    DataTable presentStockData = GetPresentStockData();

        //    // DataTable receiptData = GetReceiptData();
        //    DataTable presentReceiptData = GetReceiptData(selectedDate);

        //    // Fetch IssueData Officer
        //    DataTable presentIssueOfficer = GetOfficerIssueData(selectedDate);

        //    // Fetch IssueData Sailor
        //    DataTable presentIssueSailor = GetSailorIssueData(selectedDate);

        //    // Fetch Wastage
        //    DataTable presentWastage = GetWastageData(selectedDate);

        //    // Fetch Divers Data
        //    DataTable presentDivers = GetDiversIssueData(selectedDate);

        //    // Fetch OtherShips
        //    DataTable presentOtherShips = GetOtherShipData(selectedDate);

        //    // Fetch Patients
        //    DataTable presentPatients = GetPatientsData(selectedDate);

        //    // Fetch ExtraIssue
        //    DataTable presentExtraIssue = GetExtraIssueData(selectedDate);

        //    // Fetch Ration Payment
        //    DataTable presentRationPayement = GetRationPaymentData(selectedDate);

        //    // Your existing code to fetch data from the database and create the Excel package
        //    DataTable inLieuItemsTable = new DataTable();

        //    using (SqlConnection connection = new SqlConnection(connStr))
        //    {
        //        string query = "SELECT inlieuitem, Denomination FROM InLieuItems";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            connection.Open();
        //            SqlDataAdapter adapter = new SqlDataAdapter(command);
        //            adapter.Fill(inLieuItemsTable);
        //        }
        //    }

        //    int itemCount = inLieuItemsTable.Rows.Count;
        //    // Generate HTML tables
        //    string htmlTables = "<div class='table-container' style='overflow-x: auto; display: flex; flex-wrap: nowrap;'>";
        //    int itemsPerPage = 32; // Number of items to display per page
        //    int itemsPerPageMinusOne = itemsPerPage - 1;
        //    int totalPages = 7;
        //    int totalItems = itemCount > itemsPerPage * (totalPages - 1) ? itemsPerPage * (totalPages - 1) : itemCount;
        //    int remainingItems = itemCount - totalItems;
        //    int itemIndex = 0;

        //    for (int i = 2; i < 8; i++) // Loop to create tables for each page
        //    {
        //        htmlTables += "<table border='1' width='100%' style='text-align:center; margin-right:10px;'>";
        //        htmlTables += $"<tr><th colspan='11'>Page {i.ToString().PadLeft(2, '0')}</th></tr>";
        //        htmlTables += "<tr><th style='width:40px;'>Line No.</th>";

        //        if (i == 2) // Set width only for the first page
        //        {
        //            htmlTables += "<th style='width: 450px;'>Column 1</th>";
        //        }

        //        for (int h = 1; h < (i == 2 ? 10 : 11); h++)
        //        {
        //            string itemHeader = itemIndex < totalItems ? inLieuItemsTable.Rows[itemIndex]["inlieuitem"].ToString() : $"Column {h}";
        //            string itemDenomination = itemIndex < totalItems ? inLieuItemsTable.Rows[itemIndex]["Denomination"].ToString() : "";

        //            htmlTables += $"<th>{itemHeader}<br/>{itemDenomination}</th>";
        //            itemIndex++;
        //        }
        //        htmlTables += "</tr>";

        //        int rowsToDisplay = i == 8 ? remainingItems : itemsPerPage;
        //        for (int j = 0; j < rowsToDisplay; j++)
        //        {
        //            htmlTables += "<tr>";
        //            int lineNumber = j + 1;
        //            htmlTables += $"<td>{lineNumber}</td>";

        //            if (i == 2 && j == 0) // Check if it's page 2 and row 3
        //            {
        //                // Show "FULL STOWAGE" in column 1, row 3
        //                htmlTables += "<td>FULL STOWAGE</td>";

        //                // Skip the rest of the loop for this row
        //                for (int k = 1; k < 10; k++)
        //                {
        //                    htmlTables += "<td>1</td>";
        //                }
        //            }
        //            else if (i == 2 && j == 1)
        //            {
        //                htmlTables += "<td style=''>BALANCE AS PER ACCOUNT</td>";

        //                for (int k = 1; k < 10; k++)
        //                {
        //                    htmlTables += "<td>2</td>";
        //                }
        //            }
        //            else if (i == 2 && j == 2)
        //            {
        //                htmlTables += "<td style=''>STOCK NOT TO FALL BELOW</td>";

        //                for (int k = 1; k < 10; k++)
        //                {
        //                    htmlTables += "<td>2</td>";
        //                }
        //            }
        //            else
        //            {
        //                for (int k = 0; k < 10; k++)
        //                {
        //                    htmlTables += "<td></td>";
        //                }
        //            }

        //            htmlTables += "</tr>";
        //        }

        //        htmlTables += "</table>";
        //        if (itemIndex >= totalItems)
        //        {
        //            break;
        //        }
        //    }
        //    htmlTables += "</div>";

        //    tablesContainerPage2to7.InnerHtml = htmlTables;
        //}




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
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ExtraIssue WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
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

        #endregion 

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {

        }

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


    }
}