using GoLA2.Admin.Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace GoLA2.Admin
{
    public partial class Templates : System.Web.UI.Page
    {
        /// <summary>
        /// On page load this method is called and retreives all the 
        /// templates from the database to display in the GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the templates from the database
            DataTable TemplatesTable = Database.GetAllTemplates();
            // Add the data to the grid
            gridTemplates.DataSource = TemplatesTable;
            gridTemplates.DataBind();

            // Sets the Session variable that determines the sort order of the grid
            if (Session["sort"] == null)
                Session["sort"] = "";
        }

        /// <summary>
        /// This method is called when the user clicks a page button to get
        /// the next page of templates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridTemplates_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // set the page index to the new page ndex and bind the data
            gridTemplates.PageIndex = e.NewPageIndex;
            gridTemplates.DataBind();
        }
        
        /// <summary>
        /// This method is called when the user clicks a table heading
        /// to change the sort order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridTemplates_SortChanging(object sender, GridViewSortEventArgs e)
        {
            // Get the data again
            DataTable TemplatesTable = Database.GetAllTemplates();

            string dir;
            // If the sort expression is the same as the current one flip the direction
            if (((string)Session["sort"]).Equals(e.SortExpression))
            {
                dir = " DESC";
            }
            // if not use the default ascending
            else
            {
                dir = "";
            }
            // set the session to the new sort expression
            Session["sort"] = e.SortExpression + dir;
            // sort the table and add the data to the grid
            TemplatesTable.DefaultView.Sort = (string)Session["sort"];
            gridTemplates.DataSource = TemplatesTable;
            gridTemplates.DataBind();
        }

        /// <summary>
        /// This method is called when the user clicks a row's delete button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridTemplate_Deleting(object sender, GridViewDeleteEventArgs e)
        {
            // get the UserID of the row and call the database delete function on it
            Database.DeleteTemplate((int)e.Keys[0]);
            // that doesn't seem to refresh the table so we just do a quick page transfer to this page to call page load again.
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }
    }

}