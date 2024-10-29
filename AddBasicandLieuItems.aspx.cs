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

namespace VMS_1
{
    public partial class AddBasicandLieuItems : System.Web.UI.Page
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
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    select * from BasicLieuItems ORDER BY Id desc";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewBasicInlieuItems.DataSource = reader;
                GridViewBasicInlieuItems.DataBind();
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string[] basicitem = Request.Form.GetValues("basicitem");
                string[] basicdenom = Request.Form.GetValues("basicdenom");
                string[] ilieuitem = Request.Form.GetValues("ilieuitem");
                string[] ilieudenom = Request.Form.GetValues("ilieudenom");
                string[] ItemCode = Request.Form.GetValues("patternno");
                string[] cate = Request.Form.GetValues("ilieudecat");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < ilieuitem.Length; i++)
                    {
                        string basicItemValue = i < basicitem.Length ? basicitem[i] : basicitem[0];
                        string basicDenomValue = i < basicdenom.Length ? basicdenom[i] : basicdenom[0];
                        string ilieuItemValue = ilieuitem[i];
                        string ilieuDenomValue = ilieudenom[i];

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM BasicLieuItems WHERE BasicItem = @BasicItem AND iLueItem = @iLueItem", conn);
                        checkCmd.Parameters.AddWithValue("@BasicItem", basicItemValue);
                        checkCmd.Parameters.AddWithValue("@iLueItem", ilieuItemValue);

                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblStatus.Text = $"The combination of Basic Item '{basicItemValue}' and In-lieu Item '{ilieuItemValue}' already exists. Please try to change the basic item and then try again.";
                            return;
                        }


                        SqlCommand cmd = new SqlCommand("INSERT INTO BasicLieuItems (BasicItem, BasicDenom, iLueItem, iLueDenom,ItemCode,Category) VALUES (@BasicItem, @BasicDenom, @iLueItem, @iLueDenom,@ItemCode,@Category)", conn);

                        cmd.Parameters.AddWithValue("@BasicItem", i < basicitem.Length ? basicitem[i] : basicitem[0]);
                        cmd.Parameters.AddWithValue("@BasicDenom", i < basicdenom.Length ? basicdenom[i] : basicdenom[0]);
                        cmd.Parameters.AddWithValue("@iLueItem", ilieuitem[i]);
                        cmd.Parameters.AddWithValue("@iLueDenom", ilieudenom[i]);
                        cmd.Parameters.AddWithValue("@ItemCode", ItemCode[i]);
                        cmd.Parameters.AddWithValue("@Category", cate[i]);
                        cmd.ExecuteNonQuery();
                    }
                }
                lblStatus.Text = "Data entered successfully.";

                LoadGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        protected void GridViewBasicInlieuItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewBasicInlieuItems.EditIndex = e.NewEditIndex;
            LoadGridView();
        }

        protected void GridViewBasicInlieuItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = (int)e.Keys["ID"];
            string basicItem = (string)e.NewValues["BasicItem"];
            string basicDenom = (string)e.NewValues["BasicDenom"];
            string ilueItem = (string)e.NewValues["iLueItem"];
            string ilueDenom = (string)e.NewValues["iLueDenom"];
            string ItemCode = (string)e.NewValues["ItemCode"];
            string cate = (string)e.NewValues["Category"];
            //using (SqlConnection conn = new SqlConnection(connStr))
            //{
            //    string query = "UPDATE BasicLieuItems SET BasicItem=@BasicItem, BasicDenom=@BasicDenom, iLueItem=@iLueItem, iLueDenom=@iLueDenom,ItemCode=@ItemCode,Category=@Category WHERE ID=@ID";
            //    SqlCommand cmd = new SqlCommand(query, conn);

            //    cmd.Parameters.AddWithValue("@BasicItem", basicItem);
            //    cmd.Parameters.AddWithValue("@BasicDenom", basicDenom);
            //    cmd.Parameters.AddWithValue("@iLueItem", ilueItem);
            //    cmd.Parameters.AddWithValue("@iLueDenom", ilueDenom);
            //    if (!string.IsNullOrEmpty(ItemCode))
            //    {
            //        cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
            //    }
            //    cmd.Parameters.AddWithValue("@ID", id);
            //    if (!string.IsNullOrEmpty(cate))
            //        cmd.Parameters.AddWithValue("@Category", cate);
            //    conn.Open();
            //    cmd.ExecuteNonQuery();
            //}
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE BasicLieuItems SET BasicItem = @BasicItem, BasicDenom = @BasicDenom, iLueItem = @iLueItem, iLueDenom = @iLueDenom, ItemCode = @ItemCode, Category = @Category WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters explicitly
                    cmd.Parameters.Add(new SqlParameter("@BasicItem", SqlDbType.NVarChar) { Value = basicItem });
                    cmd.Parameters.Add(new SqlParameter("@BasicDenom", SqlDbType.NVarChar) { Value = basicDenom });
                    cmd.Parameters.Add(new SqlParameter("@iLueItem", SqlDbType.NVarChar) { Value = ilueItem });
                    cmd.Parameters.Add(new SqlParameter("@iLueDenom", SqlDbType.NVarChar) { Value = ilueDenom });

                    // Check for null or empty values and handle appropriately
                    cmd.Parameters.Add(new SqlParameter("@ItemCode", SqlDbType.NVarChar) { Value = string.IsNullOrEmpty(ItemCode) ? DBNull.Value : (object)ItemCode });
                    cmd.Parameters.Add(new SqlParameter("@Category", SqlDbType.NVarChar) { Value = string.IsNullOrEmpty(cate) ? DBNull.Value : (object)cate });
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = id });

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            GridViewBasicInlieuItems.EditIndex = -1;
            LoadGridView();
        }

        protected void GridViewBasicInlieuItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewBasicInlieuItems.EditIndex = -1;
            LoadGridView();
        }

        protected void GridViewBasicInlieuItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = (int)e.Keys["ID"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM BasicLieuItems WHERE ID=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadGridView();
        }
    }
}