using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class UnapprovedUsers : System.Web.UI.Page
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
                RejectedGridView();
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT name, rank, Designation, NudId, Password, Role, IsApproved FROM usermaster WHERE NudId <> 'admin' AND (IsApproved = 0)";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewUser.DataSource = reader;
                GridViewUser.DataBind();
            }
        }

        private void RejectedGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT name, rank, Designation, NudId, Password, Role, IsApproved FROM usermaster WHERE NudId <> 'admin' AND (IsRejected = 1)";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewReject.DataSource = reader;
                GridViewReject.DataBind();
            }
        }

        protected void GridViewUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve")
            {
                string nudId = e.CommandArgument.ToString();
                ApproveUser(nudId);
            }
            else if (e.CommandName == "Reject")
            {
                string nudId = e.CommandArgument.ToString();
                RejectUser(nudId);
            }
        }

        //protected void GridViewReject_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    int rowIndex = e.RowIndex;

        //    // Retrieve the key value of the row being deleted (NudId in this case)
        //    string nudId = GridViewReject.DataKeys[rowIndex]["NudId"].ToString();

        //    // Call a method to perform the deletion
        //    DeleteUser(nudId);

        //    LoadGridView();
        //    RejectedGridView();
        //}

        private void ApproveUser(string nudId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE usermaster SET IsApproved = 1 WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadGridView();
            RejectedGridView();
        }

        private void DeleteUser(string nudId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM usermaster WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void RejectUser(string nudId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "update usermaster Set IsRejected = 1, IsApproved = null WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadGridView();
            RejectedGridView();
        }

        protected void GridViewUser_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewUser.EditIndex = e.NewEditIndex;
            LoadGridView();
        }

        protected void GridViewUser_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewUser.EditIndex = -1;
            LoadGridView();
        }

        protected void GridViewUser_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < GridViewUser.Rows.Count)
            {
                GridViewRow row = GridViewUser.Rows[e.RowIndex];

                if (row != null)
                {
                    string name = ((TextBox)row.FindControl("txtName")).Text;
                    string rank = ((TextBox)row.FindControl("txtRank")).Text;
                    string designation = ((TextBox)row.FindControl("txtDesignation")).Text;
                    string nuid = ((TextBox)row.FindControl("txtNuid")).Text;
                    DropDownList ddlRole = (DropDownList)row.FindControl("ddlRole");
                    //string password = ((TextBox)row.FindControl("txtPassword")).Text;
                    //string secretQuestion = ((TextBox)row.FindControl("txtSecretQuestion")).Text;
                    //string answer = ((TextBox)row.FindControl("txtAnswer")).Text;
                    //DropDownList ddlRole = (DropDownList)row.FindControl("ddlRole");
                    //string role = ddlRole.SelectedValue;

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        string query = "UPDATE usermaster SET rank = @Rank, Designation = @Designation, name = @Name, Role = @Role, NudId = @NudId WHERE NudId = @NudId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Rank", rank);
                        cmd.Parameters.AddWithValue("@Designation", designation);
                        cmd.Parameters.AddWithValue("@NudId", nuid);
                        //cmd.Parameters.AddWithValue("@Password", password);
                        //cmd.Parameters.AddWithValue("@SecretQuestion", secretQuestion);
                        //cmd.Parameters.AddWithValue("@Answer", answer);
                        cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedValue);
                        cmd.Parameters.AddWithValue("@Name", name);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }

                    GridViewUser.EditIndex = -1;
                    LoadGridView();
                }
            }
        }
    }
}