using System;

namespace GoLA2.Admin
{
    public partial class Logout : System.Web.UI.Page
    {
        /// <summary>
        /// If this page is loaded the user will automatically be logged out
        /// by clearing the session and then forwarded to the login page. Could
        /// in future change this page to be a confirmation page but this is fine
        /// for now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // clear and redirect
            Session.Clear();
            Server.Transfer("Login.aspx", false);
        }
    }
}