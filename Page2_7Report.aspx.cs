﻿using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Page2_7Report : System.Web.UI.Page
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

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            // Get the current date and calculate the previous month
            string[] selectedDate = monthYearPicker.Value.Split('-');


            DataTable monthEndStockData = GetPreviousMonthData(selectedDate);

            // Fetch data from the PresentStockMaster table
            DataTable presentStockData = GetPresentStockData();

            //Fetch Fresh data
            DataTable presentFreshData = GetFreshStockData(selectedDate);

            //DataTable receiptData = GetReceiptData();
            DataTable presentReceiptData = GetReceiptData(selectedDate);

            //Fetch IssueData Officer
            DataTable presentIssueOfficer = GetOfficerIssueData(selectedDate);

            //Fetch IssueData Sailor
            DataTable presentIssueSailor = GetSailorIssueData(selectedDate);

            //Fetch Wastage
            DataTable presentWastage = GetWastageData(selectedDate);

            //Fetch Divers Data
            DataTable presentDivers = GetDiversIssueData(selectedDate);

            //Fetch OtherShips
            DataTable presentOtherShips = GetOtherShipData(selectedDate);

            //Fetch Patients
            DataTable presentPatients = GetPatientsData(selectedDate);

            //Fetch ExtraIssue
            DataTable presentExtraIssue = GetExtraIssueData(selectedDate);

            //Fetch Ration Payemnt
            DataTable presentRationPayement = GetRationPaymentData(selectedDate);


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page2-7 Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                // Set column widths
                worksheet.Column(1).Width = 0;
                worksheet.Column(2).Width = 11;
                worksheet.Column(3).Width = 62.71;
                worksheet.Row(1).Height = 0;



                // Add header rows and merge cells
                worksheet.Cells["B2:L2"].Merge = true;
                worksheet.Cells["B2:L2"].Value = "PAGE 02";
                worksheet.Cells["B2:L2"].Style.Font.Bold = true;
                worksheet.Cells["B2:L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2:L2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B4"].Value = "LINE";
                worksheet.Cells["B5"].Value = "NO.";

                worksheet.Cells["N2:AA2"].Merge = true;
                worksheet.Cells["N2:AA2"].Value = "PAGE 03";
                worksheet.Cells["N2:AA2"].Style.Font.Bold = true;
                worksheet.Cells["N2:AA2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["N2:AA2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["AC2:AP2"].Merge = true;
                worksheet.Cells["AC2:AP2"].Value = "PAGE 04";
                worksheet.Cells["AC2:AP2"].Style.Font.Bold = true;
                worksheet.Cells["AC2:AP2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["AC2:AP2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["AR2:BE2"].Merge = true;
                worksheet.Cells["AR2:BE2"].Value = "PAGE 05";
                worksheet.Cells["AR2:BE2"].Style.Font.Bold = true;
                worksheet.Cells["AR2:BE2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["AR2:BE2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["BG2:BT2"].Merge = true;
                worksheet.Cells["BG2:BT2"].Value = "PAGE 06";
                worksheet.Cells["BG2:BT2"].Style.Font.Bold = true;
                worksheet.Cells["BG2:BT2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["BG2:BT2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["BV2:CI2"].Merge = true;
                worksheet.Cells["BV2:CI2"].Value = "PAGE 07";
                worksheet.Cells["BV2:CI2"].Style.Font.Bold = true;
                worksheet.Cells["BV2:CI2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["BV2:CI2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Add numbering from 1 to 28 in column B (B7 to B34)
                int number = 1;
                int columnLength = 28;
                if (presentReceiptData != null && presentReceiptData.Rows != null && presentReceiptData.Rows.Count > 0)
                {
                    columnLength = columnLength + presentReceiptData.Rows.Count;
                }
                for (int row = 7; row <= columnLength; row++)
                {
                    worksheet.Cells[row, 2].Value = number;
                    number++;
                }

                // Apply borders to all cells
                using (ExcelRange range = worksheet.Cells["B2:CC" + columnLength])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }

                // Add static labels to cells
                worksheet.Cells["C7"].Value = "DENOS OF QTY ->";
                worksheet.Cells["C8"].Value = "FULL STOWAGE";
                worksheet.Cells["C9"].Value = "STOCK NOT TO FALL BELOW";
                worksheet.Cells["C9"].Style.Font.Bold = true;
                worksheet.Cells["C10"].Value = "BALANCE AS PER ACCOUNT";
                worksheet.Cells["C10"].Style.Font.Bold = true;


                int dataStartRowReceiptCRV = 11;
                int startRowReceiptCRV = dataStartRowReceiptCRV;
                foreach (DataRow rowReceiptCRV in presentReceiptData.Rows)
                {
                    string CellNo = "C" + dataStartRowReceiptCRV;
                    worksheet.Cells[CellNo].Value = rowReceiptCRV["referenceNos"];
                    int dataStartRowReceiptCRVNOS1 = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowReceiptCRV["itemnames"]))
                        {
                            worksheet.Cells[dataStartRowReceiptCRV, dataStartRowReceiptCRVNOS1].Value = rowReceiptCRV["quantities"]; // Start from column C
                            //break;
                        }
                        else
                        {
                            worksheet.Cells[dataStartRowReceiptCRV, dataStartRowReceiptCRVNOS1].Value = 0;
                        }
                        dataStartRowReceiptCRVNOS1++;
                    }

                    dataStartRowReceiptCRV++;
                }

                int endRowReceiptCRV = dataStartRowReceiptCRV;
                

                //int startColumnSub = 4;
                //int dataEndColumnSub = startColumnSub + presentStockData.Rows.Count - 1;
                //int endColumnSub = dataEndColumnSub; // Assuming dataEndColumnSub is the last column index
                //int resultRowSub1 = 32; // Row to display the first result
                //int resultRowSub2 = 33; // Row to display the second result

                //for (int col = startColumnSub; col <= endColumnSub; col++)
                //{
                //    string startCell = ExcelCellBase.GetAddress(21, col); // D21
                //    string endCell = ExcelCellBase.GetAddress(31, col); // D31
                //    string resultCell1 = ExcelCellBase.GetAddress(resultRowSub1, col); // Result in D32
                //    string resultCell2 = ExcelCellBase.GetAddress(resultRowSub2, col); // Result in D33

                //    // Set the formula to subtract D31 from D21 and place the result in the result cells
                //    worksheet.Cells[resultCell1].Formula = $"{startCell}-{endCell}";
                //    worksheet.Cells[resultCell2].Formula = resultCell1;
                //}

                int freshDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + freshDataRow].Value = "FRESH RECEIPTS FROM  Pg - 12";
                int TotalReceipt = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + TotalReceipt].Value = "TOTAL RECEIPTS";
                worksheet.Cells["C" + dataStartRowReceiptCRV++].Style.Font.Bold = true;
                int issueSailorDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueSailorDataRow].Value = "ISSUE TO SHIP'S COY";
                int issueOfficerDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueOfficerDataRow].Value = "ISSUE TO OFFICERS";
                int issueExtraIssueDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueExtraIssueDataRow].Value = "EXTRA ISSUES from  Pg - 09";
                int issuePaymentIssueDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issuePaymentIssueDataRow].Value = "PAYMENT ISSUE from Pg.10/11";
                int issuePatientsDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issuePatientsDataRow].Value = "ISSUE TO PATIENTS";
                int issueOtherShipsDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueOtherShipsDataRow].Value = "ISSUE TO OTHER SHIPS";
                //worksheet.Cells["C" + dataStartRowReceiptCRV++].Value = "ISSUE TO CAT SCHOOL";
                int issueDiversDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueDiversDataRow].Value = "ISSUE TO DIVERS";
                int issueWastageDataRow = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueWastageDataRow].Value = "WASTAGE";
                int issueGrandIssue = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + issueGrandIssue].Value = "ISSUE GRAND TOTAL";
                worksheet.Cells["C" + dataStartRowReceiptCRV++].Style.Font.Bold = true;
                int BalanceAsPerAccount = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + BalanceAsPerAccount].Value = "BALANCE AS PER ACCOUNT";
                int BalanceAsPerMuster = dataStartRowReceiptCRV++;
                worksheet.Cells["C" + BalanceAsPerMuster].Value = "BALANCE AS PER MUSTER";
                worksheet.Cells["C" + dataStartRowReceiptCRV++].Value = "CRV / LOSS STATEMENT";


                //FreshData
                int dataStartRowFresh = freshDataRow;
                foreach (DataRow rowFresh in presentFreshData.Rows)
                {
                    int dataStartRowReceiptCRVNOS1 = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowFresh["itemnames"]))
                        {
                            worksheet.Cells[dataStartRowFresh, dataStartRowReceiptCRVNOS1].Value = rowFresh["quantities"]; // Start from column C
                            //break;
                        }
                        dataStartRowReceiptCRVNOS1++;
                    }
                }

                //ISSUE OFFICER
                //int dataStartRowIssueOfficer = 23;
                foreach (DataRow rowIssueOfficer in presentIssueOfficer.Rows)
                {
                    int dataStartRowIssueOfficer1 = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowIssueOfficer["ItemName"]))
                        {
                            worksheet.Cells[issueOfficerDataRow, dataStartRowIssueOfficer1].Value = Convert.ToDouble(rowIssueOfficer["quantities"]); // Start from column C
                            //break;
                        }
                        dataStartRowIssueOfficer1++;
                    }
                }

                //ISSUE SAILOR
                foreach (DataRow rowIssueSailor in presentIssueSailor.Rows)
                {
                    int dataStartRowIssueSailor = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowIssueSailor["ItemName"]))
                        {
                            worksheet.Cells[issueSailorDataRow, dataStartRowIssueSailor].Value = Convert.ToDouble(rowIssueSailor["QtyIssued"]); // Start from column C
                            //break;
                        }
                        dataStartRowIssueSailor++;
                    }
                }

                //Extra Issue
                foreach (DataRow rowExtraIssue in presentExtraIssue.Rows)
                {
                    int dataStartRowExtraIssue = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowExtraIssue["ItemName"]))
                        {
                            worksheet.Cells[issueExtraIssueDataRow, dataStartRowExtraIssue].Value = Convert.ToDouble(rowExtraIssue["Qty"]); // Start from column C
                            worksheet.Cells[issueExtraIssueDataRow, dataStartRowExtraIssue].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowExtraIssue++;
                    }
                }

                //Wastage
                foreach (DataRow rowWastage in presentWastage.Rows)
                {
                    int dataStartRowWastage = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowWastage["ItemName"]))
                        {
                            worksheet.Cells[issueWastageDataRow, dataStartRowWastage].Value = Convert.ToDouble(rowWastage["Qty"]); // Start from column C
                            worksheet.Cells[issueWastageDataRow, dataStartRowWastage].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowWastage++;
                    }
                }

                //Divers
                foreach (DataRow rowDivers in presentDivers.Rows)
                {
                    int dataStartRowDivers = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowDivers["ItemName"]))
                        {
                            worksheet.Cells[issueDiversDataRow, dataStartRowDivers].Value = Convert.ToDouble(rowDivers["Qty"]); // Start from column C
                            worksheet.Cells[issueDiversDataRow, dataStartRowDivers].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowDivers++;
                    }
                }

                //OtherShips
                foreach (DataRow rowShips in presentOtherShips.Rows)
                {
                    int dataStartRowShips = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowShips["ItemName"]))
                        {
                            worksheet.Cells[issueOtherShipsDataRow, dataStartRowShips].Value = Convert.ToDouble(rowShips["Qty"]); // Start from column C
                            worksheet.Cells[issueOtherShipsDataRow, dataStartRowShips].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowShips++;
                    }
                }

                //Patients
                foreach (DataRow rowPatients in presentPatients.Rows)
                {
                    int dataStartRowPatients = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowPatients["ItemName"]))
                        {
                            worksheet.Cells[issuePatientsDataRow, dataStartRowPatients].Value = Convert.ToDouble(rowPatients["Qty"]); // Start from column C
                            worksheet.Cells[issuePatientsDataRow, dataStartRowPatients].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowPatients++;
                    }
                }

                //Payment Issue
                foreach (DataRow rowPayemtnIssue in presentRationPayement.Rows)
                {
                    int dataStartRowPayemtnIssue = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowPayemtnIssue["ItemName"]))
                        {
                            worksheet.Cells[issuePaymentIssueDataRow, dataStartRowPayemtnIssue].Value = Convert.ToDouble(rowPayemtnIssue["Qty"]); // Start from column C
                            worksheet.Cells[issuePaymentIssueDataRow, dataStartRowPayemtnIssue].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //break;
                        }
                        dataStartRowPayemtnIssue++;
                    }
                }


                int dataStartColumnSum = 4;
                int dataEndColumn = dataStartColumnSum + presentStockData.Rows.Count;
                for (int col = dataStartColumnSum; col < dataEndColumn; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(startRowReceiptCRV, col);
                    string endCell = ExcelCellBase.GetAddress(endRowReceiptCRV, col);
                    string sumCell = ExcelCellBase.GetAddress(TotalReceipt, col);
                    worksheet.Cells[sumCell].Formula = $"SUM({startCell}:{endCell})";
                }

                int dataStartColumnEndSum = 4;
                int dataEndColumnEnd = dataStartColumnEndSum + presentStockData.Rows.Count;
                for (int col = dataStartColumnEndSum; col < dataEndColumnEnd; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(issueSailorDataRow, col);
                    string endCell = ExcelCellBase.GetAddress(issueWastageDataRow, col);
                    string sumCell = ExcelCellBase.GetAddress(issueGrandIssue, col);
                    worksheet.Cells[sumCell].Formula = $"SUM({startCell}:{endCell})";
                }

                int dataStartColumnEndSub = 4;
                int dataEndColumnEndSub = dataStartColumnEndSub + presentStockData.Rows.Count;
                for (int col = dataStartColumnEndSub; col < dataEndColumnEndSub; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(TotalReceipt, col);
                    string endCell = ExcelCellBase.GetAddress(issueGrandIssue, col);
                    string sumCell = ExcelCellBase.GetAddress(BalanceAsPerAccount, col);
                    worksheet.Cells[sumCell].Formula = $"MIN({startCell}-{endCell})";

                    string sumCell1 = ExcelCellBase.GetAddress(BalanceAsPerMuster, col);
                    worksheet.Cells[sumCell1].Formula = $"MIN({startCell}-{endCell})";
                }

                // Bind data from the PresentStockMaster table
                int dataStartColumn = 4; // Column D
                foreach (DataRow row in presentStockData.Rows)
                {
                    worksheet.Cells[4, dataStartColumn].Value = row["InLieuItem"];
                    worksheet.Cells[7, dataStartColumn].Value = row["Denomination"];
                    dataStartColumn++;

                    int dataStartRowReceipt = 4;
                    foreach (DataRow rowReceipt in monthEndStockData.Rows)
                    {
                        bool matchfound = false;
                        if (row["InLieuItem"].Equals(rowReceipt["ItemName"]))
                        {
                            worksheet.Cells[10, dataStartRowReceipt].Value = rowReceipt["Qty"]; // Start from column C
                            matchfound = true;
                        }
                        if (!matchfound)
                        {
                            worksheet.Cells[10, dataStartRowReceipt].Value = 0.00;
                        }
                        dataStartRowReceipt++;
                    }

                    //int dataStartRowReceiptCRVNOS = 4;
                    //foreach (DataRow rowReceiptCRV in presentReceiptData.Rows)
                    //{
                    //    if (row["ItemName"].Equals(rowReceiptCRV["itemnames"]))
                    //    {
                    //        worksheet.Cells[10, dataStartRowReceiptCRVNOS].Value = rowReceiptCRV["quantities"]; // Start from column C
                    //    }

                    //    dataStartRowReceiptCRVNOS++;
                    //}
                }

                //int startRow = 11; // Starting from B11 downwards
                //foreach (DataRow row in receiptData.Rows)
                //{
                //    // Assuming ReferenceNos is the column name from ReceiptMaster table
                //    string referenceNos = row["ReferenceNos"].ToString();
                //    worksheet.Cells[startRow, 2].Value = referenceNos; // Populate in column B (2)
                //    startRow++;
                //}

                // Save and download

                var DateNow = DateTime.Now.Month + "-" + DateTime.Now.Year;
                string fileName = $"PAGE2-7_{DateNow}.xlsx";
                FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                excelPackage.SaveAs(excelFile);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(excelFile.FullName);
                Response.Flush();
                Response.End();
            }
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

        private DataTable GetOfficerIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ItemName, SUM(CONVERT(INT, QtyIssued)) AS quantities FROM IssueMaster Where Role = @role AND MONTH(Date) = @Month AND Year(Date) = @Year GROUP By ItemName", conn))
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
    }
}
