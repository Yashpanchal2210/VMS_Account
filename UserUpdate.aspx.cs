﻿using System;
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
    public partial class UserUpdate : System.Web.UI.Page
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
                string nudId = Session["NudId"]?.ToString();
                string query = "SELECT name, rank, Designation, NudId, Password, Role FROM usermaster WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewUser.DataSource = reader;
                GridViewUser.DataBind();
            }
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
                    string password = ((TextBox)row.FindControl("txtPassword")).Text;
                    //string secretQuestion = ((TextBox)row.FindControl("txtSecretQuestion")).Text;
                    //string answer = ((TextBox)row.FindControl("txtAnswer")).Text;
                    //DropDownList ddlRole = (DropDownList)row.FindControl("ddlRole");
                    //string role = ddlRole.SelectedValue;

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        string query = "UPDATE usermaster SET rank = @Rank, Designation = @Designation, name = @Name, Password = @Password WHERE NudId = @NudId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Rank", rank);
                        cmd.Parameters.AddWithValue("@Designation", designation);
                        cmd.Parameters.AddWithValue("@NudId", nuid);
                        cmd.Parameters.AddWithValue("@Password", password);
                        //cmd.Parameters.AddWithValue("@SecretQuestion", secretQuestion);
                        //cmd.Parameters.AddWithValue("@Answer", answer);
                        //cmd.Parameters.AddWithValue("@Role", role);
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