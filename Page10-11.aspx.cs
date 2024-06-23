using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;

namespace VMS_1
{
    public partial class Page10_11 : System.Web.UI.Page
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

        protected void PaymentExportToExcelButton_Click(object sender, EventArgs e)
        {
            // Get the current date and calculate the previous month
            string[] selectedDate = monthYearPicker.Value.Split('-');

            //Fetch ItemNames
            DataTable presentItemStock = GetPresentStockData(selectedDate);

            //Fetch Ration Payemnt
            DataTable presentRationPayement = GetRationPaymentData(selectedDate);


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page2-7 Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                worksheet.Column(1).Width = 62.71;

                // Add header rows and merge cells
                worksheet.Cells["A1:L1"].Merge = true;
                worksheet.Cells["A1:L1"].Value = "PAGE 10";
                worksheet.Cells["A1:L1"].Style.Font.Bold = true;
                worksheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:L1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A2:L2"].Merge = true;

                worksheet.Cells["A3:L3"].Merge = true;
                worksheet.Cells["A3:L3"].Value = "DAILY EXTRACT OF RATION ISSUED ON PAYMENT";
                worksheet.Cells["A3:L3"].Style.Font.Bold = true;
                worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A4:L4"].Merge = true;

                worksheet.Cells["A6"].Value = "VR. NO.";

                //Get Item Names
                int dataStartColumn = 2; // Column B
                foreach (DataRow row in presentItemStock.Rows)
                {
                    worksheet.Cells[6, dataStartColumn].Value = row["ItemName"];
                    worksheet.Cells[8, dataStartColumn].Value = row["Denom"];
                    worksheet.Cells[10, dataStartColumn].Value = row["Rate"];
                    dataStartColumn++;
                }

                worksheet.Cells["A8"].Value = "DENOM->";
                worksheet.Cells["A10"].Value = "RATE->";

                //Get CRV Data
                int dataStartRowPaymentIssue = 12;
                foreach (DataRow rowPaymentIssue in presentRationPayement.Rows)
                {
                    string CellNo = "A" + dataStartRowPaymentIssue;
                    worksheet.Cells[CellNo].Value = rowPaymentIssue["ReferenceCRVNo"];
                    int dataStartRowPaymentIssue1 = 2;
                    foreach (DataRow row in presentItemStock.Rows)
                    {
                        if (row["ItemName"].Equals(rowPaymentIssue["ItemName"]))
                        {
                            worksheet.Cells[dataStartRowPaymentIssue, dataStartRowPaymentIssue1].Value = rowPaymentIssue["Qty"]; // Start from column C
                            break;
                        }
                        dataStartRowPaymentIssue1++;
                    }

                    dataStartRowPaymentIssue++;
                }

                dataStartRowPaymentIssue++;
                dataStartRowPaymentIssue++;
                dataStartRowPaymentIssue++;
                worksheet.Cells["A" + dataStartRowPaymentIssue].Value = "T/QTY";
                int dataStartColumnSum = 2;
                int dataEndColumn = dataStartColumnSum + presentItemStock.Rows.Count;
                for (int col = dataStartColumnSum; col < dataEndColumn; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(11, col);
                    string endCell = ExcelCellBase.GetAddress(dataStartRowPaymentIssue - 1, col);
                    string sumCell = ExcelCellBase.GetAddress(dataStartRowPaymentIssue, col);
                    worksheet.Cells[sumCell].Formula = $"SUM({startCell}:{endCell})";
                }
                dataStartRowPaymentIssue++;
                int B = 2;
                int A = 10;
                int dataStartColumnMultiply = 2;
                int dataEndColumnMul = dataStartColumnMultiply + presentItemStock.Rows.Count;
                for (int col = dataStartColumnMultiply; col < dataEndColumnMul; col++)
                {
                    string rateB = ExcelCellBase.GetAddress(10, B);
                    string endCell = ExcelCellBase.GetAddress(dataStartRowPaymentIssue - 1, col);
                    string sumCell = ExcelCellBase.GetAddress(dataStartRowPaymentIssue, col);
                    worksheet.Cells[sumCell].Formula = $"{rateB}*{endCell}";
                    B++;
                    A++;
                }
                worksheet.Cells["A" + dataStartRowPaymentIssue].Value = "VALUE";




                // Apply borders to all cells
                using (ExcelRange range = worksheet.Cells["A2:L" + dataStartRowPaymentIssue])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }


                // Save and download

                var DateNow = DateTime.Now.Month + "-" + DateTime.Now.Year;
                string fileName = $"PAGE10-11_{DateNow}.xlsx";
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

        private DataTable GetRationPaymentData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ReferenceCRVNo, ItemName, SUM(CONVERT(decimal(18, 3), Qty)) AS Qty FROM RationIssuePayment WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ReferenceCRVNo, Qty, ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetPresentStockData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT ItemName, Denom, Rate  FROM RationIssuePayment WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ItemName, Denom, Rate", conn))
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