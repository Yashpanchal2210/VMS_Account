using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ItemMaster : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                LoadGridView();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                {
                    btnSubmit.Visible = true;
                    entry.Visible = true;
                }
                else
                {
                    btnSubmit.Visible = false;
                    entry.Visible = false;
                }
                //LoadBasicItems();
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            SELECT 
    b.Id AS ID,
    b.BasicItem AS BasicItem, 
    b.Category AS Category, 
    REPLACE(b.Denomination, ',', '') AS Denomination, 
    FORMAT(b.VegScale, 'N4') AS VegScale, 
    FORMAT(b.NonVegScale, 'N4') AS NonVegScale, 
    i.Id AS InlIueId,
    i.InLieuItem AS InLieuItem, 
    i.Category AS InLieuItemCategory, 
    REPLACE(i.Denomination, ',', '') AS InLieuItemDenomination, 
    FORMAT(i.VegScale, 'N4') AS InLieuItemVegScale, 
    FORMAT(i.NonVegScale, 'N4') AS InLieuItemNonVegScale
FROM 
    BasicItems b
LEFT JOIN 
    InLieuItems i ON b.Id = i.BasicItemId
ORDER BY 
    b.AddDate DESC, b.Id DESC
";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridView1.DataSource = reader;
                GridView1.DataBind();
            }
        }

        //private void LoadBasicItems()
        //{
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        string query = "SELECT MIN(Id) AS Id, BasicItem, BasicDenom FROM BasicLieuItems GROUP BY BasicItem, BasicDenom";
        //        SqlCommand cmd = new SqlCommand(query, conn);

        //        conn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        basicItem.DataSource = reader;
        //        basicItem.DataTextField = "BasicItem";
        //        basicItem.DataValueField = "Id";
        //        basicItem.DataBind();
        //    }
        //    basicItem.Items.Insert(0, new ListItem("Select", ""));
        //}

        [WebMethod]
        public static List<string> GetCategoryWiseDataItems(string basicItem)
        {
            List<string> items = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string idQuery = "SELECT BasicItem FROM BasicItems WHERE Category = @Category";
                using (SqlCommand cmd = new SqlCommand(idQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", basicItem);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(reader["BasicItem"].ToString());
                        }
                    }
                }
            }

            return items;
        }

        [WebMethod]
        public static string GetInLieuBasicDenom(string basicItem)
        {
            string basicDenom = string.Empty;
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "select Top 1 Denomination from inLieuItems where InLieuItem = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", basicItem);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    basicDenom = reader["Denomination"].ToString();
                }
            }

            return basicDenom;
        }


        [WebMethod]
        public static List<string> GetInLieuItems(string basicItem, string categoryVal)
        {
            List<string> items = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string basicitemId = "select Id from BasicItems where BasicItem = @ItemName And Category = @Category";
                string basicId = null;
                using (SqlCommand cmd = new SqlCommand(basicitemId, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemName", basicItem);
                    cmd.Parameters.AddWithValue("@Category", categoryVal);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            basicId = reader["Id"].ToString();
                        }
                    }
                }


                string idQuery = "SELECT InlieuItem FROM InLieuItems WHERE BasicItemId = @Id";
                string basicItemValue = "";
                using (SqlCommand cmd = new SqlCommand(idQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", basicId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(reader["InlieuItem"].ToString());
                        }
                    }
                }
            }

            return items;
        }

        [WebMethod]
        public static string GetBasicDenom(string basicItem)
        {
            string basicDenom = string.Empty;
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "select Top 1 Denomination from BasicItems where BasicItem = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", basicItem);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    basicDenom = reader["Denomination"].ToString();
                }
            }

            return basicDenom;
        }

        [WebMethod]
        public static string GetDenominationForInLieuItem(string inlieuItem)
        {
            string ilueDenom = string.Empty;
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "select iLueDenom from BasicLieuItems where iLueItem = @ilueItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ilueItem", inlieuItem);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ilueDenom = reader["iLueDenom"].ToString();
                }
            }

            return ilueDenom;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Response.Write("<script>alert('Data once submitted cannot be changed');</script>");
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            LoadGridView();
            ScriptManager.RegisterStartupScript(this, GetType(), "FetchInLieuItems", $"onRowEdit({e.NewEditIndex});", true);
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            LoadGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;
                if (rowIndex >= 0 && rowIndex < GridView1.Rows.Count)
                {
                    GridViewRow row = GridView1.Rows[rowIndex];
                    int id = Convert.ToInt32(GridView1.DataKeys[rowIndex].Value);

                    decimal vegScale = decimal.Parse(((TextBox)row.FindControl("txtVegScale")).Text);
                    decimal nonVegScale = decimal.Parse(((TextBox)row.FindControl("txtNonVegScale")).Text);

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE InLieuItems SET VegScale = @VegScale, NonVegScale = @NonVegScale WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@VegScale", vegScale);
                            cmd.Parameters.AddWithValue("@NonVegScale", nonVegScale);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    GridView1.EditIndex = -1;
                    LoadGridView();
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly
                Response.Write("Error: " + ex.Message);
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM InLieuItems WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView();
        }
        protected bool IsLogisticOfficer()
        {
            // Check if Session["Role"] exists and equals "Regulating Officer"
            if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
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
