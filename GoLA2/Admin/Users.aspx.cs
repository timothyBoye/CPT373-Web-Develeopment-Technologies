using GoLA2.Admin.Model;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace GoLA2.Admin
{
    public partial class Users : System.Web.UI.Page
    {
        /// <summary>
        /// Called on page load, gets the users and populates the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the users from the database
            DataTable UsersTable = Database.GetAllUsers();
            // Populate the grid with the users
            gridUsers.DataSource = UsersTable;
            gridUsers.DataBind();

            // Sets the session variable for the sort order of the grid
            if (Session["sort"] == null)
                Session["sort"] = "";
        }

        /// <summary>
        /// Method is called when the user clicks the paging buttons
        /// to change page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // set the page to the new page index and bind the data
            gridUsers.PageIndex = e.NewPageIndex;
            gridUsers.DataBind();
        }

        /// <summary>
        /// Method called when the user clicks one of the grid headings
        /// to change the sort order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridUsers_SortChanging(object sender, GridViewSortEventArgs e)
        {
            // Get the data
            DataTable UsersTable = Database.GetAllUsers();
            string dir;
            // If the sortexpression matches the current one flip the order
            if (((string)Session["sort"]).Equals(e.SortExpression))
            {
                dir = " DESC";
            }
            //otherwise go with default ascending
            else
            {
                dir = "";
            }
            // save the new sort expression
            Session["sort"] = e.SortExpression + dir;
            // sort the data and fill the grid with the data
            UsersTable.DefaultView.Sort = (string)Session["sort"];
            gridUsers.DataSource = UsersTable;
            gridUsers.DataBind();
        }

        /// <summary>
        /// Method called when the user clicks a row's delete button
        /// and deletes that item from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridUser_Deleting(object sender, GridViewDeleteEventArgs e)
        {
            // tell the database to delete the User at the provided UserID
            Database.DeleteUser((int)e.Keys[0]);
            // Doesn't refresh the data so refresh the page
            Server.TransferRequest(Request.Url.AbsolutePath, false);
        }
    }
}