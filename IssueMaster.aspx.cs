﻿using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;

namespace VMS_1
{
    public partial class IssueMaster : System.Web.UI.Page
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
                LoadGridView();
                GetItemCategories();
            }
        }

        [WebMethod]
        public static string GetInLiueItemsByCategory(string Category)
        {
            string categoryVal = string.Empty;

            if (Category == "Wardroom")
            {
                categoryVal = "Officer";
            }
            else if (Category == "Galley")
            {
                categoryVal = "Sailor";
            }

            List<string> itemNames = new List<string>();

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT Id, InLieuItem FROM InLieuItems WHERE Category = @category";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", categoryVal);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemNames.Add(reader["InLieuItem"].ToString());
                            itemNames.Add(reader["Id"].ToString());
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(itemNames); ;
        }

        [WebMethod]
        public static string GetItemDenom(string ItemVal)
        {

            var result = new
            {
                Denomination = string.Empty,
                VegScale = string.Empty,
                NonVegScale = string.Empty,
                CurrentStock=string.Empty,
                YearMarkStock=string.Empty
            };

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Denomination, VegScale, NonVegScale,[dbo].[usp_GetPresetStock](InLieuItem)CurrentStock,[dbo].[usp_GetYearMarkStock](InLieuItem)YearMarkStock  FROM InLieuItems WHERE Id = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", ItemVal);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    result = new
                    {
                        Denomination = reader["Denomination"].ToString(),
                        VegScale = reader["VegScale"].ToString(),
                        NonVegScale = reader["NonVegScale"].ToString(),
                        CurrentStock= reader["CurrentStock"].ToString(),
                        YearMarkStock=reader["YearMarkStock"].ToString()
                    };
                }
            }

            return new JavaScriptSerializer().Serialize(result);
        }        
        [WebMethod]
        public static List<object> GetItemCategories()
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM Items";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            var categories = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new
                {
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemID"].ToString(),
                    ScaleAmount = row["RationScaleOfficer"].ToString()
                });
            }

            return categories;
        }
        private int checkVAStatus(string[] Date)
        {
            int status = 0;
            string[] data = Date[0].Split('-');
            int selectedMonth = Convert.ToInt32(data[1]);
            int selectedYear = Convert.ToInt32(data[0]);
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "EXEC usp_GetVACurrentStatusByMonth " + selectedMonth + "," + selectedYear + "";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }
            if (dt.Rows.Count > 0)
            {
                if (Session["NudId"].ToString() != dt.Rows[0]["FrowardedTo"].ToString())
                {
                    status = 1;
                }
            }
            return status;
        }
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] role = Request.Form.GetValues("userrole");
                string[] itemname = Request.Form.GetValues("itemname");
                string[] enterstrength = Request.Form.GetValues("Strength");
                string[] qtyissued = Request.Form.GetValues("Qtyissued");


                //string[] itemcategory = Request.Form.GetValues("itemcategory");
                //string[] denomination = Request.Form.GetValues("denom");
                //string[] qtyentitled = Request.Form.GetValues("entitledstrength");

                string[] denomination = new string[itemname.Length];
                string[] itemid = new string[itemname.Length];
                int status = checkVAStatus(date);
                if (status > 0)
                {
                    lblStatus.Text = "you can not enter the data in this visualling account month";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    //int maxLength = Math.Max(date.Length, Math.Max(itemname.Length, Math.Max(enterstrength.Length, Math.Max(qtyissued.Length, Math.Max(denomination.Length, role.Length)))));

                    for (int i = 0; i < itemname.Length; i++)
                    {
                        DateTime parsedDate = DateTime.Parse(i < date.Length ? date[i] : date[0]);
                        string itemName = itemname[i];
                        decimal qtyIssued = decimal.Parse(qtyissued[i]);

                        // Retrieve the Denomination value from the AlternateItem table 
                        SqlCommand getDenominationCmd = new SqlCommand(
                            "SELECT Denomination FROM InLieuItems WHERE Id = @ItemName", conn);
                        getDenominationCmd.Parameters.AddWithValue("@ItemName", itemName);

                        object denomResult = getDenominationCmd.ExecuteScalar();
                        if (denomResult != null && denomResult != DBNull.Value)
                        {
                            denomination[i] = denomResult.ToString();
                        }
                        else
                        {
                            denomination[i] = string.Empty;
                        }

                        SqlCommand getIdCmd = new SqlCommand(
                            "SELECT InLieuItem FROM InLieuItems WHERE Id = @ItemName", conn);
                        getIdCmd.Parameters.AddWithValue("@ItemName", itemName);

                        object idResult = getIdCmd.ExecuteScalar();
                        if (idResult != null && idResult != DBNull.Value)
                        {
                            itemid[i] = idResult.ToString();
                        }
                        else
                        {
                            itemid[i] = string.Empty;
                        }

                        SqlCommand checkReceiptCmd = new SqlCommand(
                            "SELECT SUM(Qty) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkReceiptCmd.Parameters.AddWithValue("@ItemName", itemid[i]);

                        object result = checkReceiptCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            lblStatus.Text = $"No data found for item {itemName} in PresentStockMaster.";
                            continue;
                        }
                        else
                        {
                            decimal availableQty = Convert.ToDecimal(result);

                            if (availableQty < qtyIssued)
                            {
                                lblStatus.Text = $"Insufficient quantity for item.";
                                continue;
                            }
                        }

                        SqlCommand cmd = new SqlCommand("InsertIssue", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Date", i < date.Length ? date[i] : date[0]);
                        cmd.Parameters.AddWithValue("@ItemName", itemname[i]);
                        //cmd.Parameters.AddWithValue("@ItemId", itemname[i]);
                        cmd.Parameters.AddWithValue("@Strength", enterstrength[i]);
                        //cmd.Parameters.AddWithValue("@QtyEntitled", qtyentitled[i]);
                        cmd.Parameters.AddWithValue("@QtyIssued", qtyissued[i]);
                        cmd.Parameters.AddWithValue("@Denomination", denomination[i]);
                        cmd.Parameters.AddWithValue("@Role", i < role.Length ? role[i] : role[0]);
                        cmd.ExecuteNonQuery();

                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand(
                        "UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemid[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denomination[i]);
                        updatePresentStockCmd.ExecuteNonQuery();

                        // Extract month and year from the date
                        int month = parsedDate.Month;
                        int year = parsedDate.Year;

                        // Check if a record exists for the same item in the same month and year
                        SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Month", month);
                        checkCmd.Parameters.AddWithValue("@Year", year);
                        checkCmd.Parameters.AddWithValue("@ItemName", itemid[i]);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END " +
                                "WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Month", month);
                            updateCmd.Parameters.AddWithValue("@Year", year);
                            updateCmd.Parameters.AddWithValue("@ItemName", itemid[i]);
                            updateCmd.Parameters.AddWithValue("@Quantity", qtyIssued);

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand(
                                "INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", parsedDate); // Use the full date here
                            insertCmd.Parameters.AddWithValue("@ItemName", itemid[i]);
                            insertCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }

                        //lblStatus.Text = "Data entered successfully.";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "showSuccessToast('Data submitted successfully');", true);
                    }
                }

                LoadGridView();
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGridView();
        }

        private void LoadGridView()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM IssueMaster ORDER By Id Desc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    GridViewIssue.DataSource = dt;
                    GridViewIssue.DataBind();
                    //string[] date = new string[] { Convert.ToDateTime(dt.Rows[0]["Date"]).ToString("yyyy-MM-dd") };
                    //int status = checkVAStatus(date);
                    //if (status > 0)
                    //{
                    //    GridViewIssue.DataSource = dt;
                    //    GridViewIssue.DataBind();
                    //}
                    //else
                    //{
                    //    this.GridViewIssue.Columns[7].Visible = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridViewIssue_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewIssue.EditIndex = e.NewEditIndex;
            LoadGridView();
        }

        protected void GridViewIssue_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridViewIssue.Rows[e.RowIndex];
            int id = Convert.ToInt32(GridViewIssue.DataKeys[e.RowIndex].Value);

            System.Web.UI.WebControls.TextBox dateTextBox2 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtQtyIssued");
            string qty = dateTextBox2.Text;
            System.Web.UI.WebControls.TextBox dateTextBox3 = (System.Web.UI.WebControls.TextBox)row.FindControl("txtstrength");
            string strength = dateTextBox3.Text;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE IssueMaster SET EntitledStrength = @strength, QtyIssued = @QtyIssued WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@strength", strength);
                    cmd.Parameters.AddWithValue("@QtyIssued", qty);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            GridViewIssue.EditIndex = -1; // Exit edit mode
            LoadGridView(); // Rebind the GridView to show the updated data
        }

        protected void GridViewIssue_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewIssue.EditIndex = -1; // Exit edit mode
            LoadGridView(); // Rebind the GridView to show the original data
        }

        protected void GridViewIssue_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = Convert.ToInt32(GridViewIssue.DataKeys[e.RowIndex].Value);
            string quantities = "";
            string itemname = "";
            string date = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT QtyIssued, ItemName, Date FROM IssueMaster WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantities = reader["QtyIssued"].ToString();
                            itemname = reader["ItemName"].ToString();
                            date = reader["Date"].ToString();
                        }
                    }
                }

                if (quantities != null)
                {
                    DateTime dateTime = DateTime.Parse(date);
                    int month = dateTime.Month;
                    int year = dateTime.Year;

                    SqlCommand checkItemCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                    checkItemCmd.Parameters.AddWithValue("@ItemName", itemname);

                    int itemCount = (int)checkItemCmd.ExecuteScalar();

                    if (itemCount > 0)
                    {
                        // If item exists, update the quantity
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty + @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", quantities);

                        updatePresentStockCmd.ExecuteNonQuery();
                    }

                    SqlCommand checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                    checkCmd.Parameters.AddWithValue("@Month", month);
                    checkCmd.Parameters.AddWithValue("@Year", year);
                    checkCmd.Parameters.AddWithValue("@ItemName", itemname);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        SqlCommand updateCmd = new SqlCommand(
                            "UPDATE MonthEndStockMaster SET Qty = Qty + @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        updateCmd.Parameters.AddWithValue("@Month", month);
                        updateCmd.Parameters.AddWithValue("@Year", year);
                        updateCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updateCmd.Parameters.AddWithValue("@Quantity", quantities);

                        updateCmd.ExecuteNonQuery();
                    }
                }


                int rowsAffected;
                using (SqlCommand cmd = new SqlCommand("DELETE FROM IssueMaster WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    rowsAffected = cmd.ExecuteNonQuery();
                }

                if (rowsAffected != 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "showSuccessToast('Data Deleted Successfully');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "showErrorToast('Error deleting row!');", true);
                }

            }

            LoadGridView(); // Rebind the GridView to show the updated data
        }

        protected void GridViewIssue_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Row Type: " + e.Row.RowType.ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["Role"] != null && Session["Role"].ToString() == "Store Keeper")
                {
                    // Find the delete button in the row and hide it
                    //LinkButton deleteButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                    //if (deleteButton != null)
                    //{
                    //    deleteButton.Visible = false;
                    //}
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        string str = e.Row.Cells[1].Text;
                        LinkButton deleteButton = cell.Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                        LinkButton EditButton = cell.Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Edit");
                        if (deleteButton != null)
                        {
                            deleteButton.Visible = false;                            
                        }
                        if (EditButton != null)
                        {
                            EditButton.Visible = false;
                        }

                    }
                }
                else if(Session["Role"] != null && Session["Role"].ToString() == "Logistic Officer")
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {                      
                        string[] date = new string[] { Convert.ToDateTime(e.Row.Cells[1].Text).ToString("yyyy-MM-dd") };                       
                        int status = checkVAStatus(date);
                        if (status > 0)
                        {
                            LinkButton deleteButton = cell.Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                            LinkButton EditButton = cell.Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Edit");
                            if (deleteButton != null)
                            {
                                deleteButton.Visible = false;
                            }
                            if (EditButton != null)
                            {
                                EditButton.Visible = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
