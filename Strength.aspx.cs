using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Strength : System.Web.UI.Page
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
                // Initialize ViewState["DataTable"] if it's null
                if (ViewState["DataTable"] == null)
                {
                    ViewState["DataTable"] = new DataTable();
                }
                BindGridView();
                GridViewStrength.Visible = false;
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] dates = Request.Form.GetValues("date");
                string[] vegOfficers = Request.Form.GetValues("VegOfficer");
                string[] nonVegOfficers = Request.Form.GetValues("NonVegOfficer");
                string[] vegrikOfficers = Request.Form.GetValues("VegrikOfficer");
                string[] nonVegrikOfficers = Request.Form.GetValues("NonVegRikOfficer");
                string[] vegSailor = Request.Form.GetValues("vegSailor");
                string[] nonVegSailor = Request.Form.GetValues("nonVegSailor");
                string[] vegSailorRik = Request.Form.GetValues("VegSailorRik");
                string[] nonVegSailorRik = Request.Form.GetValues("NonVegSailorRik");
                string[] vegNonEntitledOfficer = Request.Form.GetValues("VegNonEntitledOfficer");
                string[] nonVegNonEntitledOfficer = Request.Form.GetValues("NonVegNonEntitledOfficer");
                string[] vegNonEntitledSailor = Request.Form.GetValues("VegNonEntitledSailor");
                string[] NonVegNonEntitledSailor = Request.Form.GetValues("NonVegNonEntitledSailor");
                string[] civilians = Request.Form.GetValues("Civilian");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < dates.Length; i++)
                    {
                        SqlCommand cmd = new SqlCommand("InsertOrUpdateStrength", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Date", dates[i]);
                        cmd.Parameters.AddWithValue("@VegOfficers", vegOfficers[i]);
                        cmd.Parameters.AddWithValue("@NonVegOfficers", nonVegOfficers[i]);
                        cmd.Parameters.AddWithValue("@VegrikOfficers", vegrikOfficers[i]);
                        cmd.Parameters.AddWithValue("@NonVegrikOfficers", nonVegrikOfficers[i]);
                        cmd.Parameters.AddWithValue("@VegSailor", vegSailor[i]);
                        cmd.Parameters.AddWithValue("@NonVegSailor", nonVegSailor[i]);
                        cmd.Parameters.AddWithValue("@VegSailorRik", vegSailorRik[i]);
                        cmd.Parameters.AddWithValue("@NonVegSailorRik", nonVegSailorRik[i]);
                        cmd.Parameters.AddWithValue("@VegNonEntitledOfficer", vegNonEntitledOfficer[i]);
                        cmd.Parameters.AddWithValue("@NonVegNonEntitledOfficer", nonVegNonEntitledOfficer[i]);
                        cmd.Parameters.AddWithValue("@VegNonEntitledSailor", vegNonEntitledSailor[i]);
                        cmd.Parameters.AddWithValue("@NonVegNonEntitledSailor", NonVegNonEntitledSailor[i]);
                        cmd.Parameters.AddWithValue("@Civilians", civilians[i]);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblStatus.Text = "Data entered successfully.";
                BindGridView();
                BindTotalGridView((DataTable)ViewState["DataTable"]);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        private void BindGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                string query = string.Empty;
                string role = Session["Role"].ToString();

                if (role == "Regulating Officer")
                {
                    query = "SELECT * FROM Strength WHERE IsApproved = 0 OR IsApproved IS NULL;";
                }else if (role == "Regulating Office")
                {
                    query = "SELECT * FROM Strength where IsApproved = 1";
                }
                else
                {
                    query = "SELECT * FROM Strength";
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ViewState["DataTable"] = dt;
                    GridViewStrength.DataSource = dt;
                    GridViewStrength.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while fetching data: " + ex.Message;
            }
        }

        private void BindTotalGridView(DataTable dt)
        {
            DataRow totalRow = dt.NewRow();
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(int) && column.ColumnName != "dates")
                {
                    totalRow[column.ColumnName] = dt.Compute($"SUM([{column.ColumnName}])", "");
                }
            }

            dt.Rows.Add(totalRow);
            GridViewStrength.DataSource = dt;
            GridViewStrength.DataBind();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string filterDate = txtDate.Text.Trim();

            if (DateTime.TryParseExact(filterDate, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                DataTable dt = (DataTable)ViewState["DataTable"];
                DataView dv = new DataView(dt);

                // Format the filter to match the month and year
                dv.RowFilter = $"dates >= '{date.ToString("yyyy-MM-01")}' AND dates < '{date.AddMonths(1).ToString("yyyy-MM-01")}'";

                if (dv.Count > 0)
                {
                    GridViewStrength.DataSource = dv;
                    GridViewStrength.DataBind();
                    GridViewStrength.Visible = true;
                }
                else
                {
                    lblStatus.Text = "No data found for the selected date.";
                    GridViewStrength.Visible = false;
                }
            }
            else
            {
                lblStatus.Text = "Invalid date format. Please enter a valid date in yyyy-MM format.";
            }
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string commandName = btn.CommandName;
            int id = Convert.ToInt32(btn.CommandArgument);

            if (commandName == "DeleteRecord")
            {
                DeleteRecord(id);
            }
            else if (commandName == "ApproveRecord")
            {
                ApproveRecord(id);
            }

            // Rebind the GridView after the action
            BindGridView();
            BindTotalGridView((DataTable)ViewState["DataTable"]);
        }

        private void DeleteRecord(int id)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Strength WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error deleting record: " + ex.Message;
            }
        }

        private void ApproveRecord(int id)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Strength SET IsApproved = 1 WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error approving record: " + ex.Message;
            }
        }

        protected bool IsRegulatingOfficer()
        {
            // Check if Session["Role"] exists and equals "Regulating Officer"
            if (Session["Role"] != null && Session["Role"].ToString() == "Regulating Officer")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
