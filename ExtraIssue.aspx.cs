using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class ExtraIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridViewExtraIssue();
        }

        private void LoadGridViewExtraIssue()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("Select * from ExtraIssueCategory order by Date ASC", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssue.DataSource = dt;
                    GridViewExtraIssue.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void ExportExtraIssueButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                DataTable strengthData = new DataTable();
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from ExtraIssueCategory", conn);
                    dataAdapter.Fill(strengthData);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("STATEMENT OF EXTRA ISSUE");

                    // Set the title
                    worksheet.Cells["A2"].Value = $"STATEMENT OF EXTRA ISSUE - {DateTime.Now.ToString("MMMM yyyy")}";
                    worksheet.Cells["A2:N2"].Merge = true;
                    worksheet.Cells["A2:N2"].Style.Font.Bold = true;
                    worksheet.Cells["A2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    using (var range = worksheet.Cells["A2:N29"])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    worksheet.Cells["A5"].Value = "DENOM- >";
                    worksheet.Cells["A6"].Value = "SCALE- >";
                    worksheet.Cells["A7"].Value = "DATE";

                    worksheet.Cells["B3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["B3"].Style.Font.Bold = true;
                    worksheet.Cells["C3"].Value = "TEA";
                    worksheet.Cells["C3"].Style.Font.Bold = true;
                    worksheet.Cells["D3"].Value = "MILK FRESH";
                    worksheet.Cells["D3"].Style.Font.Bold = true;
                    worksheet.Cells["E3"].Value = "SUGAR";
                    worksheet.Cells["E3"].Style.Font.Bold = true;

                    worksheet.Cells["F3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["F3"].Style.Font.Bold = true;
                    worksheet.Cells["G3"].Value = "LIME FRESH";
                    worksheet.Cells["G3"].Style.Font.Bold = true;

                    worksheet.Cells["H3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["H3"].Style.Font.Bold = true;
                    worksheet.Cells["I3"].Value = "LIME JUICE";
                    worksheet.Cells["I3"].Style.Font.Bold = true;
                    worksheet.Cells["J3"].Value = "SUGAR";
                    worksheet.Cells["J3"].Style.Font.Bold = true;

                    worksheet.Cells["K3"].Value = "NO. OF PERSONS PEST CONTROL";
                    worksheet.Cells["K3"].Style.Font.Bold = true;
                    worksheet.Cells["L3"].Value = "MILK FRESH";
                    worksheet.Cells["L3"].Style.Font.Bold = true;

                    worksheet.Cells["M3"].Value = "NO. OF PERSONS LEAD POISIONING";
                    worksheet.Cells["M3"].Style.Font.Bold = true;
                    worksheet.Cells["N3"].Value = "MILK FRESH";
                    worksheet.Cells["N3"].Style.Font.Bold = true;

                    worksheet.Cells["B2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells["C5"].Value = "KGS";
                    worksheet.Cells["C6"].Value = "0.007";
                    worksheet.Cells["D5"].Value = "LTRS";
                    worksheet.Cells["D6"].Value = "0.070";
                    worksheet.Cells["E5"].Value = "KGS";
                    worksheet.Cells["E6"].Value = "0.030";
                    worksheet.Cells["G5"].Value = "KGS";
                    worksheet.Cells["G6"].Value = "0.070";
                    worksheet.Cells["I5"].Value = "KGS";
                    worksheet.Cells["I6"].Value = "0.028";
                    worksheet.Cells["J5"].Value = "KGS";
                    worksheet.Cells["J6"].Value = "0.040";
                    worksheet.Cells["L5"].Value = "LTRS";
                    worksheet.Cells["L6"].Value = "0.250";
                    worksheet.Cells["N5"].Value = "LTRS";
                    worksheet.Cells["N6"].Value = "0.280";

                    int currentRow = 8;
                    double totalStrengthTeaMilkSugar = 0;
                    double totalTea = 0;
                    double totalMilkTeaMilkSugar = 0;
                    double totalSugarTeaMilkSugar = 0;

                    double totalStrengthLimeFresh = 0;
                    double totalLimeFresh = 0;

                    double totalStrengthLimeAndSugar = 0;
                    double totalLimeJuice = 0;
                    double totalSugarLimeAndSugar = 0;

                    double totalStrengthPestControl = 0;
                    double totalMilkPestControl = 0;

                    double totalStrengthLeadPoisoning = 0;
                    double totalMilkLeadPoisoning = 0;

                    foreach (DataRow row in strengthData.Rows)
                    {
                        worksheet.Cells[$"A{currentRow}"].Value = Convert.ToDateTime(row["Date"]).ToString("MM/dd/yyyy");

                        if (row["Type"].ToString() == "MilkSugarAndTea")
                        {
                            double strength = 0.00, tea = 0.00, milk = 0.00, sugar = 0.00;

                            if (row["itemName"].ToString() == "Tea")
                            {
                                tea = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }
                            else if (row["itemName"].ToString() == "Milk Fresh")
                            {
                                milk = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }
                            else if (row["itemName"].ToString() == "Sugar")
                            {
                                sugar = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }

                            strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);

                            worksheet.Cells[$"B{currentRow}"].Value = strength;
                            worksheet.Cells[$"C{currentRow}"].Value = tea;
                            worksheet.Cells[$"D{currentRow}"].Value = milk;
                            worksheet.Cells[$"E{currentRow}"].Value = sugar;

                            totalStrengthTeaMilkSugar += strength;
                            totalTea += tea;
                            totalMilkTeaMilkSugar += milk;
                            totalSugarTeaMilkSugar += sugar;
                        }
                        else if (row["Type"].ToString() == "LimeFresh")
                        {
                            double strength = 0.00, limeFresh = 0.00;

                            if (row["itemName"].ToString() == "Lime Fresh")
                            {
                                limeFresh = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }

                            strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);

                            worksheet.Cells[$"F{currentRow}"].Value = strength;
                            worksheet.Cells[$"G{currentRow}"].Value = limeFresh;

                            totalStrengthLimeFresh += strength;
                            totalLimeFresh += limeFresh;
                        }
                        else if (row["Type"].ToString() == "LimeJuiceandSugar")
                        {
                            double strength = 0.00, limeJuice = 0.00, sugar = 0.00;

                            if (row["itemName"].ToString() == "Lime Fresh")
                            {
                                limeJuice = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }
                            else if (row["itemName"].ToString() == "Sugar")
                            {
                                sugar = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }

                            strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);

                            worksheet.Cells[$"H{currentRow}"].Value = strength;
                            worksheet.Cells[$"I{currentRow}"].Value = limeJuice;
                            worksheet.Cells[$"J{currentRow}"].Value = sugar;

                            totalStrengthLimeAndSugar += strength;
                            totalLimeJuice += limeJuice;
                            totalSugarLimeAndSugar += sugar;
                        }
                        else if (row["Type"].ToString() == "PestControl")
                        {
                            double strength = 0.00, milk = 0.00;

                            if (row["itemName"].ToString() == "Milk Fresh")
                            {
                                milk = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }

                            strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);

                            worksheet.Cells[$"K{currentRow}"].Value = strength;
                            worksheet.Cells[$"L{currentRow}"].Value = milk;

                            totalStrengthPestControl += strength;
                            totalMilkPestControl += milk;
                        }
                        else if (row["Type"].ToString() == "LeadPoisioning")
                        {
                            double strength = 0.00, milk = 0.00;

                            if (row["itemName"].ToString() == "Milk Fresh")
                            {
                                milk = row["Qty"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Qty"]);
                            }

                            strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);

                            worksheet.Cells[$"M{currentRow}"].Value = strength;
                            worksheet.Cells[$"N{currentRow}"].Value = milk;

                            totalStrengthLeadPoisoning += strength;
                            totalMilkLeadPoisoning += milk;
                        }

                        currentRow++;
                    }

                    // Add the totals row
                    worksheet.Cells[$"A{currentRow}"].Value = "TOTAL";
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;

                    worksheet.Cells[$"B{currentRow}"].Value = totalStrengthTeaMilkSugar;
                    worksheet.Cells[$"C{currentRow}"].Value = totalTea;
                    worksheet.Cells[$"D{currentRow}"].Value = totalMilkTeaMilkSugar;
                    worksheet.Cells[$"E{currentRow}"].Value = totalSugarTeaMilkSugar;

                    worksheet.Cells[$"F{currentRow}"].Value = totalStrengthLimeFresh;
                    worksheet.Cells[$"G{currentRow}"].Value = totalLimeFresh;

                    worksheet.Cells[$"H{currentRow}"].Value = totalStrengthLimeAndSugar;
                    worksheet.Cells[$"I{currentRow}"].Value = totalLimeJuice;
                    worksheet.Cells[$"J{currentRow}"].Value = totalSugarLimeAndSugar;

                    worksheet.Cells[$"K{currentRow}"].Value = totalStrengthPestControl;
                    worksheet.Cells[$"L{currentRow}"].Value = totalMilkPestControl;

                    worksheet.Cells[$"M{currentRow}"].Value = totalStrengthLeadPoisoning;
                    worksheet.Cells[$"N{currentRow}"].Value = totalMilkLeadPoisoning;

                    // Add the new section
                    currentRow += 2;
                    worksheet.Cells[$"A{currentRow}"].Value = "S.NO.";
                    worksheet.Cells[$"B{currentRow}"].Value = "ITEM";
                    worksheet.Cells[$"C{currentRow}"].Value = "ENTITLED";
                    worksheet.Cells[$"E{currentRow}"].Value = "S.NO.";
                    worksheet.Cells[$"F{currentRow}"].Value = "ITEM";
                    worksheet.Cells[$"G{currentRow}"].Value = "ENTITLED";
                    worksheet.Cells[$"A{currentRow + 1}"].Value = 1;
                    worksheet.Cells[$"B{currentRow + 1}"].Value = "TEA";
                    worksheet.Cells[$"C{currentRow + 1}"].Value = totalTea;
                    worksheet.Cells[$"A{currentRow + 2}"].Value = 2;
                    worksheet.Cells[$"B{currentRow + 2}"].Value = "SUGAR";
                    worksheet.Cells[$"C{currentRow + 2}"].Value = totalSugarTeaMilkSugar;
                    worksheet.Cells[$"A{currentRow + 3}"].Value = 3;
                    worksheet.Cells[$"B{currentRow + 3}"].Value = "MILK FRESH";
                    worksheet.Cells[$"C{currentRow + 3}"].Value = totalMilkTeaMilkSugar;
                    worksheet.Cells[$"E{currentRow + 1}"].Value = 4;
                    worksheet.Cells[$"F{currentRow + 1}"].Value = "LIME FRESH";
                    worksheet.Cells[$"G{currentRow + 1}"].Value = 0;
                    worksheet.Cells[$"E{currentRow + 2}"].Value = 5;
                    worksheet.Cells[$"F{currentRow + 2}"].Value = "LIME JUICE";
                    worksheet.Cells[$"G{currentRow + 2}"].Value = totalLimeJuice;

                    worksheet.Cells[$"A{currentRow}:N{currentRow}"].Style.Font.Bold = true;

                    // Auto-fit columns for all cells
                    worksheet.Cells.AutoFitColumns(0);

                    // Save the Excel package to a file
                    string fileName = $"STATEMENT_OF_EXTRA_ISSUE_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
                    FileInfo fileInfo = new FileInfo(Server.MapPath($"~/{fileName}"));
                    excelPackage.SaveAs(fileInfo);

                    // Provide the file for download
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                    Response.WriteFile(Server.MapPath($"~/{fileName}"));
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                // Handle the error
                Response.Write($"Error: {ex.Message}");
            }
        }
    }
}