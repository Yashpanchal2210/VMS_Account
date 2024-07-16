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
using System.IO;

namespace VMS_1
{
    public partial class CurrentStockList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGridView();
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            /*Verifies that the control is rendered */
        }
        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT Id,ItemName,Qty,Denos  FROM PresentStockMaster", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvStock.DataSource = dt;
                    gvStock.DataBind();
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        protected void PrintGridView(object sender, EventArgs e)
        {
            //Disable Paging if all Pages need to be Printed.
            if ((sender as Button).CommandArgument == "All")
            {
                //Disable Paging.
                gvStock.AllowPaging = false;

                //Re-bind the GridView.
                this.LoadGridView();

                //For Printing Header on each Page.
                gvStock.UseAccessibleHeader = true;
                gvStock.HeaderRow.TableSection = TableRowSection.TableHeader;
                gvStock.FooterRow.TableSection = TableRowSection.TableFooter;
                gvStock.Attributes["style"] = "border-collapse:separate";
                foreach (GridViewRow row in gvStock.Rows)
                {
                    if ((row.RowIndex + 1) % gvStock.PageSize == 0 && row.RowIndex != 0)
                    {
                        row.Attributes["style"] = "page-break-after:always;";
                    }
                }
            }
            else
            {
                //Hide the Pager.
                gvStock.PagerSettings.Visible = false;
                this.LoadGridView();
            }

            using (StringWriter sw = new StringWriter())
            {
                //Render GridView to HTML.
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvStock.RenderControl(hw);

                //Enable Paging.
                gvStock.AllowPaging = true;
                this.LoadGridView();

                //Remove single quotes to avoid JavaScript error.
                string gridHTML = sw.ToString().Replace(Environment.NewLine, "");
                string gridCSS = gridStyles.InnerText.Replace("\"", "'").Replace(Environment.NewLine, "");


                //Print the GridView.
                string script = "window.onload = function() { PrintGrid('" + gridHTML + "', '" + gridCSS + "'); }";
                ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", script, true);
            }
        }
    }
}