using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace VMS_1
{
    public partial class CreateDemand : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                LoadGridView();
                GetItems();
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                string itemNameQuery = "SELECT iLueItem FROM BasicLieuItems WHERE ItemCode = @ItemId";

                string[] date = Request.Form.GetValues("date");
                string[] itemIds = Request.Form.GetValues("item");
                string[] itemcode = Request.Form.GetValues("itemcode");
                string[] qty = Request.Form.GetValues("qty");
                string[] denom = Request.Form.GetValues("denoms");
                string demandno = "17G0001";
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("EXEC [usp_GetDemandNo]'4008'", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if(dt.Rows.Count>0)
                    {
                        demandno = dt.Rows[0][0].ToString();
                    }
                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < itemIds.Length; i++)
                    {
                        string itemName;
                        using (SqlCommand itemNameCmd = new SqlCommand(itemNameQuery, conn))
                        {
                            itemNameCmd.Parameters.AddWithValue("@ItemId", itemIds[i]);
                            itemName = itemNameCmd.ExecuteScalar()?.ToString();
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO Demand (DemandNo,ItemCode,ItemName,ItemDeno,Qty,ReqDate,SupplyDate,Status) VALUES (@DemandNo,@ItemCode,@ItemName,@ItemDeno,@Qty,@ReqDate,@SupplyDate,0)", conn);

                        cmd.Parameters.AddWithValue("@DemandNo", demandno);
                        cmd.Parameters.AddWithValue("@ItemCode", itemcode[i]);
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@ItemDeno", denom[i]);
                        cmd.Parameters.AddWithValue("@Qty", qty[i]);
                        cmd.Parameters.AddWithValue("@SupplyDate", date[0]);
                        cmd.Parameters.AddWithValue("@ReqDate", System.DateTime.Now);
                        cmd.ExecuteNonQuery();
                        lblStatus.Text = "Data entered successfully.";
                    }
                }

                LoadGridView();
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,ReqDate,SupplyDate FROM Demand order by id asc", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDemandIssue.DataSource = dt;
                    gvDemandIssue.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
        [WebMethod]
        public static string GetItemDenom(string ItemVal)
        {
            string Denomination = "";
            string ItemName = "";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                //string itemNameQuery = "Select ItemName from RationScale Where ItemId = @Id";
                //SqlCommand cmditem = new SqlCommand(itemNameQuery, conn);
                //cmditem.Parameters.AddWithValue("@Id", ItemVal);
                //SqlDataReader reader1 = cmditem.ExecuteReader();
                //if (reader1.Read())
                //{
                //    ItemName = reader1["ItemName"].ToString();
                //}
                //conn.Close();

                //conn.Open();
                string query = "SELECT *  FROM BasicLieuItems WHERE ItemCode = @ItemCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemCode", ItemVal);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Denomination = reader["BasicDenom"].ToString();
                   
                }
            }

            return Denomination;
        }
        [WebMethod]
        public static string GetItemCodem(string ItemVal)
        {
            string Itemcode = "";
            string ItemName = "";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();                
                string query = "SELECT *  FROM BasicLieuItems WHERE ItemCode = @ItemCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemCode", ItemVal);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Itemcode = reader["Itemcode"].ToString();

                }
            }

            return Itemcode;
        }
        [WebMethod]
        public static List<object> GetItems()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "select distinct iLueItem AS ItemName,itemcode AS ItemId from BasicLieuItems";
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
            var items = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new
                {
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemId"].ToString(),
                });
            }
            return items;
        }

        //[WebMethod]
        //public static string GetItemDataByItemId(string Id)
        //{
        //    ItemData itemData = new ItemData();
        //    itemData.Rates = new List<string>();
        //    itemData.ReferenceNos = new List<string>();

        //    string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        //    string query = "SELECT * FROM RationScale WHERE ItemId = @ItemId";

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@ItemId", Id);
        //            conn.Open();
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    itemData.Rates.Add(reader["Rate"].ToString());
        //                }
        //            }
        //        }
        //    }

        //    return JsonConvert.SerializeObject(itemData);
        //}

        public class ItemData
        {
            public List<string> Rates { get; set; }
            public List<string> ReferenceNos { get; set; }
        }

        protected void gvDemandIssue_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = Convert.ToInt32(gvDemandIssue.DataKeys[e.RowIndex].Value);
             
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();                
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Demand WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView(); // Rebind the GridView to show the updated data
        }

        protected void gvDemandIssue_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Row Type: " + e.Row.RowType.ToString());
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["Role"] != null && Session["Role"].ToString() == "Store Keeper")
                {
                    // Find the delete button in the row and hide it
                    LinkButton deleteButton = e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().FirstOrDefault(btn => btn.CommandName == "Delete");
                    if (deleteButton != null)
                    {
                        deleteButton.Visible = false;
                    }
                }
            }
        }
    }
}