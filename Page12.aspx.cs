using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

namespace VMS_1
{
    public partial class Page12 : System.Web.UI.Page
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

        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber - 1;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
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

                worksheet.Column(2).Width = 30;

                //Get Item Names
                int dataStartColumn = 3;// Column C
                foreach (DataRow row in presentItemStock.Rows)
                {
                    worksheet.Cells[3, dataStartColumn].Value = row["iLueItem"];
                    worksheet.Cells[5, dataStartColumn].Value = row["iLueDenom"];
                    dataStartColumn++;
                }

                string columnLetter = GetExcelColumnName(dataStartColumn);
                // Add header rows and merge cells
                worksheet.Cells["A1:" + columnLetter + "1"].Merge = true;
                worksheet.Cells["A1:" + columnLetter + "1"].Value = "PAGE 12";
                worksheet.Cells["A1:" + columnLetter + "1"].Style.Font.Bold = true;
                worksheet.Cells["A1:" + columnLetter + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:" + columnLetter + "1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A2:" + columnLetter + "2"].Merge = true;
                worksheet.Cells["A2:" + columnLetter + "2"].Value = $"DAILY RECEIPT  OF FRESH RATION FOR THE MONTH OF {selectedDate[1] + " " + selectedDate[0]}";
                worksheet.Cells["A2:" + columnLetter + "2"].Style.Font.Bold = true;
                worksheet.Cells["A2:" + columnLetter + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:" + columnLetter + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["B3"].Value = "Voucher No";

                worksheet.Cells["A5"].Value = "DENO.";
                worksheet.Cells["A6"].Value = "DATE";

                //int dataStartColumnDate = 7;// Column C
                //foreach (DataRow row in presentItemStock.Rows)
                //{
                //    worksheet.Cells[dataStartColumnDate, 1].Value = row["iLueItem"];
                //    dataStartColumn++;
                //}

                int dataStartRowReceiptCRV = 7;
                foreach (DataRow rowReceiptCRV in presentRationPayement.Rows)
                {
                    string CellNoA = "A" + dataStartRowReceiptCRV;
                    string CellNoB = "B" + dataStartRowReceiptCRV;

                    // Convert Dates value to DateTime
                    if (DateTime.TryParse(rowReceiptCRV["Dates"].ToString(), out DateTime dateValue))
                    {
                        worksheet.Cells[CellNoA].Value = dateValue.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        worksheet.Cells[CellNoA].Value = rowReceiptCRV["Dates"]; // In case it's not a valid date, assign the raw value
                    }

                    worksheet.Cells[CellNoB].Value = rowReceiptCRV["referenceNos"];

                    int dataStartRowReceiptCRVNOS1 = 4;
                    foreach (DataRow row in presentItemStock.Rows)
                    {
                        if (row["iLueItem"].Equals(rowReceiptCRV["itemnames"]))
                        {
                            worksheet.Cells[dataStartRowReceiptCRV, dataStartRowReceiptCRVNOS1].Value = rowReceiptCRV["quantities"]; // Start from column C
                            break;
                        }
                        dataStartRowReceiptCRVNOS1++;
                    }

                    dataStartRowReceiptCRV++;
                }

                worksheet.Cells[dataStartRowReceiptCRV, 1].Value = "T/QTY";
                worksheet.Cells[dataStartRowReceiptCRV, 1].Style.Font.Bold = true;

                for (int col = 3; col < dataStartColumn; col++)
                {
                    worksheet.Cells[dataStartRowReceiptCRV, col].Formula = $"SUM({worksheet.Cells[7, col].Address}:{worksheet.Cells[dataStartRowReceiptCRV - 1, col].Address})";
                    worksheet.Cells[dataStartRowReceiptCRV, col].Style.Font.Bold = true;
                }

                using (ExcelRange rng = worksheet.Cells[1, 1, dataStartRowReceiptCRV, dataStartColumn - 1])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                //Fit Width
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save and download

                var DateNow = DateTime.Now.Month + "-" + DateTime.Now.Year;
                string fileName = $"PAGE_12_{DateNow}.xlsx";
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
                using (SqlCommand cmd = new SqlCommand("select Dates, itemnames, quantities, referenceNos from ReceiptMaster Where MONTH(Dates) = @Month AND YEAR(Dates) = @Year order by Dates ASC", conn))
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
                using (SqlCommand cmd = new SqlCommand("select iLueItem, iLueDenom from BasicLieuItems where Fresh = 'True'", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
    }
}