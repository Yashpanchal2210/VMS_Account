﻿using DocumentFormat.OpenXml.Bibliography;
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

namespace VMS_1
{
    public partial class DemandIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                BindGridView();
            }

        }
        private void BindGridView()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    SqlCommand cmd = new SqlCommand("SELECT ID,DemandNo,ItemCode,ItemName,ItemDeno,Qty,DemandDate,SupplyDate,dbo.usp_get_demandIssuedQty(DemandNo)IssuedQty,[dbo].[usp_get_demandReceiptdQty](ItemName,DemandNo)ReceiptQty FROM Demand WHERE Status=1 AND DemandNo IN(SELECT DemandNo FROM BVYardIssue) ORDER By Id desc", conn);
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