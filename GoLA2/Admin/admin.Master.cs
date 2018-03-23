using GoLA2.Admin.Model;
using System;
using System.Web.UI.HtmlControls;

namespace GoLA2.Admin
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        /// <summary>
        /// The string for accessing the admin user stored in the Session variable
        /// </summary>
        public static string WebFormsUser = "WebFormsUser";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Setup an unordered list for the navbar (login/logout)
            HtmlGenericControl ul = new HtmlGenericControl("ul");
            HtmlGenericControl li = new HtmlGenericControl("li");
            HtmlGenericControl liBack = new HtmlGenericControl("li");
            ul.Attributes["class"] = "nav navbar-nav navbar-right";

            // If the user is logged in show their name and the logout button
            if (Session[WebFormsUser] != null &&
                ((User)Session[WebFormsUser]).IsAdmin)
            {
                LogBtn.NavigateUrl = "Logout.aspx";
                LogBtn.Text = "Logout";
                li.InnerHtml = "<a href=\"Logout.aspx\">Logout</a>";
                FirstName.Text = ((Model.User)Session[WebFormsUser]).FirstName;
                LastName.Text = ((Model.User)Session[WebFormsUser]).LastName;
            }
            // Not logged in AND trying to access a page other than login redirect to login
            else if ( ! this.ContentPlaceHolder1.Page.GetType().Name.Equals("admin_login_aspx"))
            {
                Server.Transfer("Login.aspx", false);
            }
            // Not logged in and on the login page, turn the logout buttons into login buttons
            else
            {
                LogBtn.NavigateUrl = "Login.aspx";
                LogBtn.Text = "Login";
                li.InnerHtml = "<a href=\"Login.aspx\">Login</a>";
            }
            // Add the nav login/logout buttons made above to the menu
            ul.Controls.Add(li);
            // Add a back to main site button to nav
            liBack.InnerHtml = "<a href=\"../ \">Back to Site</a>";
            ul.Controls.Add(liBack);
            // Add the login/logout nav to the site
            loginMenu.Controls.Add(ul);
        }
    }
}