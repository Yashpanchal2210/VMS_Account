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

namespace VMS_1.Audit
{
    public partial class AuditUnit : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            string Date = monthYearPicker.Value;
            string url = "VictuallingManagement.aspx?date=" + Server.UrlEncode(Date);
            string script = $"<script type='text/javascript'>window.open('{url}', '_blank');</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "OpenInNewTab", script);
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();

                    string updateQuery = "UPDATE ApproveVMS SET AuditApprovedDate=@AuditApprovedDate,AuditObservation=@AuditObservation WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) = @Year";

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

                        //updateCommand.Parameters.AddWithValue("@Month", selectedMonth);
                        //updateCommand.Parameters.AddWithValue("@Year", selectedYear);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //lblMessage.Text = "Report has been updated for the Logistic officer.";
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            string updateQuery = "UPDATE ApproveVMS SET SktoLogo = @SktoLogo, Logo = @Logo, LogoReject = @LogoReject, " +
                                                 "LogoApprove = @LogoApprove, LogotoCo = @LogotoCo, Co = @Co, " +
                                                 "CoReject = @CoReject, CoApprove = @CoApprove, IsApproved = @IsApproved " +
                                                 "WHERE MONTH(AddedDate) = @Month AND YEAR(AddedDate) = @Year";
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
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

                    //updateCommand.Parameters.AddWithValue("@Month", selectedMonth);
                    //updateCommand.Parameters.AddWithValue("@Year", selectedYear);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        //lblMessage.Text = "Report has been updated for the Logistic officer.";
                    }
                }
            }
        }
    }
}