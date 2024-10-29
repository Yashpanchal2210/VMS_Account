using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class EmtDashbord : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    admin.Visible = true;
                    audit.Visible = false;
                    conversation.Visible = false;
                    LoadGridView();
                }
                else
                {
                    admin.Visible = false;
                }
                if (Session["Role"].ToString() == "Accounting Officer")
                {
                    audit.Visible = true;
                    admin.Visible = false;
                    BindDemandGridView();
                    //BindConversation();
                }
                else
                {
                    audit.Visible = false;
                }
                if (Session["Role"].ToString() == "Auditor")
                {
                    audit.Visible = false;
                    admin.Visible = false;
                    //BindDemandGridView();
                    BindConversation();
                }            

            }
        }
        private void BindConversation()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("EXEC usp_GetConversationListNlao", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvvalist.DataSource = dt;
                    gvvalist.DataBind();
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT name, rank, Designation, NudId, Password, Role FROM usermaster WHERE NudId <> 'admin'";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewUser.DataSource = reader;
                GridViewUser.DataBind();
            }
        }
        private void BindDemandGridView()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand("EXEC usp_GetDemandIssueList '" + System.DateTime.Now + "' ", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}