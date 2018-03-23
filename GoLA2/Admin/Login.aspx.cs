using GoLA2.Admin.Model;
using System;

namespace GoLA2.Admin
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Attempts to login with the provided credentials.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            // Call the database with the credientals
            User User = Database.GetAdmin(Email.Text, Password.Text);
            // if the database returns a User login success set the Session variable
            if (User != null)
            {
                Session[Site1.WebFormsUser] = User;
                Server.Transfer("Index.aspx", false);
            }
            // if the database returns null return an error message to the view
            else
            {
                ValidationError.Text = "Username/Password is incorrect or not an admin.";
            }
        }
    }
}