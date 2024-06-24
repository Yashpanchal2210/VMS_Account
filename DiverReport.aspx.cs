using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class DiverReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGridViewDivers();
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        private void LoadGridViewDivers()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssue", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueDivers.DataSource = dt;
                    GridViewExtraIssueDivers.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void ExportDiversStockButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                DataTable dt = new DataTable();

                // Add only the columns you want to include in the Excel file
                dt.Columns.Add("Name");
                dt.Columns.Add("Rank");
                dt.Columns.Add("P.No.");
                dt.Columns.Add("NO. OF DAYS ENTILED");
                dt.Columns.Add("CHOCOLATE");
                dt.Columns.Add("HORLICKS");
                dt.Columns.Add("EGGS");
                dt.Columns.Add("MILK");
                dt.Columns.Add("G/NUT");
                dt.Columns.Add("BUTTER");
                dt.Columns.Add("SUGAR");

                foreach (GridViewRow row in GridViewExtraIssueDivers.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        DataRow dr = dt.NewRow();

                        dr["Name"] = row.Cells[1].Text;
                        dr["Rank"] = row.Cells[2].Text;
                        dr["P.No."] = row.Cells[3].Text;
                        dr["NO. OF DAYS ENTILED"] = row.Cells[4].Text;
                        dr["CHOCOLATE"] = row.Cells[5].Text;
                        dr["HORLICKS"] = row.Cells[6].Text;
                        dr["EGGS"] = row.Cells[7].Text;
                        dr["MILK"] = row.Cells[8].Text;
                        dr["G/NUT"] = row.Cells[9].Text;
                        dr["BUTTER"] = row.Cells[10].Text;
                        dr["SUGAR"] = row.Cells[11].Text;

                        dt.Rows.Add(dr);
                    }
                }

                DataRow totalRow = dt.NewRow();
                totalRow["Name"] = "Total";

                for (int i = 4; i < dt.Columns.Count; i++)
                {
                    decimal total = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (decimal.TryParse(dr[i].ToString(), out decimal value))
                        {
                            total += value;
                        }
                    }
                    totalRow[i] = total;
                }

                dt.Rows.Add(totalRow);

                DataRow qtyRow = dt.NewRow();
                qtyRow["Name"] = "QTY ISSUED";

                for (int i = 4; i < dt.Columns.Count; i++)
                {
                    decimal total = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Name"].ToString() == "Total")
                            continue;

                        if (decimal.TryParse(dr[i].ToString(), out decimal value))
                        {
                            total += value;
                        }
                    }
                    qtyRow[i] = total;
                }
                dt.Rows.Add(qtyRow);

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Divers");

                    // Set the title
                    worksheet.Cells["A1"].Value = $"SUMMARY OF RATION ISSUED TO DIVER FOR THE MONTH OF {DateTime.Now.ToString("MMMM yyyy")}";
                    worksheet.Cells["A1:K1"].Merge = true; // Merge cells for the title
                    worksheet.Cells["A1:K1"].Style.Font.Bold = true; // Bold font for the title

                    // Add empty row
                    worksheet.InsertRow(2, 1);

                    // Add authority
                    worksheet.Cells["A3"].Value = "AUTHORITY: GOI LETTER DD/1655/RATION/IHQ MOD (NAVY)/1396/D(N-IV)08 DATED 07 OCT 08";
                    worksheet.Cells["A3:K3"].Merge = true; // Merge cells for the authority
                    worksheet.Cells["A3:K3"].Style.Font.Bold = true; // Bold font for the authority

                    // Add empty row
                    worksheet.InsertRow(4, 1);

                    // Add the DataTable
                    worksheet.Cells["A5"].LoadFromDataTable(dt, true);

                    // Set borders for the entire table
                    var borderStyle = ExcelBorderStyle.Thin;
                    var borderColor = System.Drawing.Color.Black;

                    var range = worksheet.Cells[5, 1, dt.Rows.Count + 5, dt.Columns.Count];
                    range.Style.Border.Top.Style = borderStyle;
                    range.Style.Border.Bottom.Style = borderStyle;
                    range.Style.Border.Left.Style = borderStyle;
                    range.Style.Border.Right.Style = borderStyle;
                    range.Style.Border.Top.Color.SetColor(borderColor);
                    range.Style.Border.Bottom.Color.SetColor(borderColor);
                    range.Style.Border.Left.Color.SetColor(borderColor);
                    range.Style.Border.Right.Color.SetColor(borderColor);

                    // Set bold font for the total row
                    worksheet.Cells[dt.Rows.Count + 5, 1, dt.Rows.Count + 5, dt.Columns.Count].Style.Font.Bold = true;
                    worksheet.Cells[dt.Rows.Count + 6, 1, dt.Rows.Count + 6, dt.Columns.Count].Style.Font.Bold = true;

                    // Check user role and add watermark if the user role is "user"
                    //if (Session["Role"] != null && Session["Role"].ToString() == "User")
                    //{
                    //    var watermark = worksheet.Drawings.AddShape("Watermark", eShapeStyle.Rect);
                    //    watermark.Text = "User";
                    //    watermark.Font.Size = 40;
                    //    watermark.Font.Bold = true;
                    //    watermark.Fill.Style = eFillStyle.SolidFill;

                    //    watermark.Fill.Color.SetColor(System.Drawing.Color.FromArgb(150, 255, 255, 255));

                    //    watermark.SetPosition(5, 5);
                    //    watermark.SetSize(800, 200);

                    //    watermark.TextAlignment = OfficeOpenXml.Drawing.eTextAlignment.Center;
                    //    watermark.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
                    //    watermark.Rotation = -45;
                    //}

                    // Save the file
                    string fileName = $"Divers_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
                    FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                    excelPackage.SaveAs(excelFile);

                    // Provide download link
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    Response.TransmitFile(excelFile.FullName);
                    Response.Flush();
                }
            }
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

    }
}