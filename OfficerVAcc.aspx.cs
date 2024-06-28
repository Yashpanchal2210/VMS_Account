using DocumentFormat.OpenXml.Wordprocessing;
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
    public partial class OfficerVAcc : System.Web.UI.Page
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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Officers Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                //Set column widths
                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 15;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 15;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 15;
                //worksheet.Row(1).Height = 0;

                // Apply borders to all cells                

                worksheet.Cells["A2"].Value = "";
                worksheet.Cells["B2"].Value = "";
                worksheet.Cells["C2"].Value = " 'S'";
                worksheet.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C2"].Style.Font.Bold = true;
                worksheet.Cells["D2"].Value = "";
                worksheet.Cells["E2"].Value = "'V'";
                worksheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E2"].Style.Font.Bold = true;
                worksheet.Cells["F2"].Value = "";
                worksheet.Cells["G2"].Value = " Total";
                worksheet.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G2"].Style.Font.Bold = true;
                worksheet.Cells["H2"].Value = "";
                worksheet.Cells["I2"].Value = "";

                worksheet.Cells["B3"].Value = "GEN";
                worksheet.Cells["B4"].Value = "RIK + HON. OFFICER";
                worksheet.Cells["B5"].Value = "NON ENTITLED MESSING";
                worksheet.Cells["B6"].Value = "Total";
                DataTable OfficerGEN = GetOfficerGENData(selectedDate);
                if (OfficerGEN.Rows.Count > 0)
                {
                    worksheet.Cells["C3"].Value = OfficerGEN.Rows[0]["nonVegOfficers"];
                    worksheet.Cells["D3"].Value = "";
                    worksheet.Cells["E3"].Value = OfficerGEN.Rows[0]["vegOfficers"];
                    worksheet.Cells["F3"].Value = "=";
                    worksheet.Cells["G3"].Value = OfficerGEN.Rows[0]["Total"];
                }
                DataTable OfficerRik = GetOfficerRikData(selectedDate);
                if (OfficerRik.Rows.Count > 0)
                {
                    worksheet.Cells["C4"].Value = OfficerRik.Rows[0]["nonVegrikOfficers"];
                    worksheet.Cells["D4"].Value = "";
                    worksheet.Cells["E4"].Value = OfficerRik.Rows[0]["vegrikOfficers"];
                    worksheet.Cells["F4"].Value = "=";
                    worksheet.Cells["G4"].Value = OfficerRik.Rows[0]["Total"];
                }
                DataTable OfficerEnti = GetOfficerEntitledData(selectedDate);
                if (OfficerEnti.Rows.Count > 0)
                {
                    worksheet.Cells["C5"].Value = OfficerEnti.Rows[0]["nonVegNonEntitledOfficer"];
                    worksheet.Cells["D5"].Value = "";
                    worksheet.Cells["E5"].Value = OfficerEnti.Rows[0]["vegNonEntitledOfficer"];
                    worksheet.Cells["F5"].Value = "=";
                    worksheet.Cells["G5"].Value = OfficerEnti.Rows[0]["Total"];
                }
                DataTable OfficerTotal = GetOfficeTotalData(selectedDate);
                if (OfficerTotal.Rows.Count > 0)
                {
                    worksheet.Cells["C6"].Value = OfficerTotal.Rows[0]["TotalNonVeg"];
                    worksheet.Cells["D6"].Value = "";
                    worksheet.Cells["E6"].Value = OfficerTotal.Rows[0]["TotalVeg"];
                    worksheet.Cells["F6"].Value = "=";
                    worksheet.Cells["G6"].Value = OfficerTotal.Rows[0]["Total"];
                }


                // Add static labels to cells
                worksheet.Cells["A7"].Value = "Ser.";
                worksheet.Cells["B7"].Value = "Item";
                worksheet.Cells["C7"].Value = "Strength";
                worksheet.Cells["D7"].Value = "";
                worksheet.Cells["E7"].Value = "Scale";
                worksheet.Cells["F7"].Value = "";
                worksheet.Cells["G7"].Value = "Qty Entitled";
                worksheet.Cells["H7"].Value = "";
                worksheet.Cells["I7"].Value = "Qty Issued";

                DataTable OfficeBasicItems = GetOfficeBasicItemsData();
                int srno = 1;
                int row = 8, col = 1;
                foreach (DataRow item in OfficeBasicItems.Rows)
                {
                    col = 1;
                    worksheet.Cells[row, col].Value = srno; col++;
                    worksheet.Cells[row, col].Value = item["BasicItem"];
                    worksheet.Cells[row, col].Style.Font.Bold = true;
                    col++;
                    worksheet.Cells[row, col].Value = OfficerTotal.Rows[0]["Total"];
                    col++;
                    worksheet.Cells[row, col].Value = "x"; col++;
                    worksheet.Cells[row, col].Value = item["VegScale"]; col++;
                    worksheet.Cells[row, col].Value = "="; col++;
                    worksheet.Cells[row, col].Value = Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"]);

                    col++;
                    row++;
                    DataTable OfficeInLieuItems = GetOfficeInLieuItemsData(Convert.ToInt32(item["Id"]));
                    foreach (DataRow itemIn in OfficeInLieuItems.Rows)
                    {
                        col = 2;
                        worksheet.Cells[row, col].Value = itemIn["InLieuItem"]; col++;
                        worksheet.Cells[row, col].Value = ""; col++;
                        worksheet.Cells[row, col].Value = "x"; col++;
                        worksheet.Cells[row, col].Value = ""; col++;
                        worksheet.Cells[row, col].Value = "="; col++;
                        worksheet.Cells[row, col].Value = 0.00; col++;
                        row++;
                    }
                    //row=15;
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "";
                    worksheet.Cells[row, 3].Value = "";
                    worksheet.Cells[row, 4].Value = "Total";
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Value = "";
                    worksheet.Cells[row, 6].Value = "=";
                    worksheet.Cells[row, 7].Value = Convert.ToDecimal(OfficerTotal.Rows[0]["Total"]) * Convert.ToDecimal(item["VegScale"]);
                    worksheet.Cells[row, 7].Style.Font.Bold = true;
                    row++;

                    DataTable OfficeIssueItems = GetOfficeIssueItemsData(Convert.ToInt32(item["Id"]));
                    decimal TotalQtyIssued = 0M;
                    foreach (DataRow itemIn in OfficeIssueItems.Rows)
                    {
                        col = 6;
                        worksheet.Cells[row, col].Value = itemIn["ItemName"]; col++;
                        worksheet.Cells[row, col].Value = ""; col++;
                        worksheet.Cells[row, col].Value = "="; col++;
                        worksheet.Cells[row, col].Value = itemIn["QtyIssued"];
                        TotalQtyIssued += Convert.ToDecimal(itemIn["QtyIssued"]);
                        row++;
                    }
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "";
                    worksheet.Cells[row, 3].Value = "";
                    worksheet.Cells[row, 4].Value = "";
                    worksheet.Cells[row, 5].Value = "";
                    worksheet.Cells[row, 6].Value = "";
                    worksheet.Cells[row, 7].Value = "";
                    worksheet.Cells[row, 8].Value = "Total";
                    worksheet.Cells[row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 9].Value = TotalQtyIssued;
                    worksheet.Cells[row, 9].Style.Font.Bold = true;
                    srno++;
                    row++;
                }

                int columnLength = row;
                using (ExcelRange range = worksheet.Cells[$"A1:I{columnLength}"])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    //range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                var DateNow = DateTime.Now.Month + "-" + DateTime.Now.Year;
                string fileName = $"Officers_{DateNow}.xlsx";
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
    }
}