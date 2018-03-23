using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GoLA2.Models.Database;
using CryptSharp;

namespace GoLA2.Controllers
{
    /// <summary>
    /// Custom Authotization attribute as the site does not use the inbuilt
    /// IdentityAPI
    /// </summary>
    // This is placed here as it's such a tiny class and closely related to the
    // UsersController, was keeping as much User stuff in one place as possible.
    // Could probably be moved to its own file though.
    public class AuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Checks to see if there is a user logged in if not redirect to home.
        /// This is called with the [Authorization] attribute on page controllers,
        /// if there's no sessionuser the user can't access that page.
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            // No User? No access, redirect to home
            if (filterContext.HttpContext.Session[UsersController.SessionUser] == null)
            {
                filterContext.Result = new RedirectResult("/");
            }
        }
    }



    /// <summary>
    /// This Controller is incharge of logging users in and out (and thus
    /// the Login and register views).
    /// </summary>
    public class UsersController : Controller
    {
        /// <summary>
        /// The string for accessing the logged in user object in the Session
        /// Variable
        /// </summary>
        public static String SessionUser = "User";

        /// <summary>
        /// Database access variables (Entity Framework)
        /// </summary>
        private DatabaseEntities db = new DatabaseEntities();

        /// <summary>
        /// Provides the Register user page/form/view
        /// GET: Users/Create
        /// </summary>
        /// <returns>Register View</returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles PostBacks for the register form. This method attempts
        /// to add a new user to the database, should only fail if the data
        /// is not valid or the email address is already taken
        /// POST: Users/Create
        /// </summary>
        /// <param name="user">The user data the user entered into the form</param>
        /// <returns>If there were issues returns the data entered and validation 
        ///          errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Email,Password,FirstName,LastName")] User user)
        {
            // Perform basic validation check
            if (ModelState.IsValid)
            {
                // If email address is already taken return to form with error
                if (db.Users.Any(x => x.Email == user.Email))
                {
                    ViewBag.UserExists = "That email address has already been used to create an account please login or create a new account.";
                    return View(user);
                }
                // Email is not taken create user and redirect to login
                else
                {
                    // LOL not an admin
                    user.IsAdmin = false;
                    // Hash the password
                    user.Password = Crypter.Blowfish.Crypt(user.Password);

                    //create the user and save to db
                    db.Users.Add(user);
                    db.SaveChanges();

                    //Go to login
                    return RedirectToAction("Login", "Users");
                }
            }
            // Invalid data return to form
            else
            {
                return View(user);
            }
        }

        /// <summary>
        /// Provides the login page
        /// </summary>
        /// <returns>Login page</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles the PostBack for the login page/form/view.
        /// Attempts to log the user in using the credentials 
        /// provided.
        /// </summary>
        /// <param name="user">Users Credentials (NOT A FULL USER OBJECT)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        public ActionResult LoginPost([Bind(Include = "Email,Password")] User user)
        {
            // Call the Db to check creds and log the user in
            using (var database = new DatabaseEntities())
            {
                // Get the user with the provided username.
                User login = database.Users.FirstOrDefault(u => u.Email == user.Email);

                // if the user exists check the password and login
                if (login != null)
                {
                    // Check the password by using the hash function then login
                    if (Crypter.CheckPassword(user.Password, login.Password))
                    {
                        // Set the session to login
                        Session[SessionUser] = login;

                        // If there are session games save them to the database as belonging to the logged in user
                        if (Session[GamesController.SessionTempGames] != null)
                        {
                            // Get the list of games, loop through, add the user info and then save
                            List<UserGame> games = (List<UserGame>)Session[GamesController.SessionTempGames];
                            foreach (UserGame u in games)
                            {
                                u.UserID = login.UserID;
                                u.User = db.Users.First(x => x.UserID == u.UserID);
                                db.UserGames.Add(u);
                                db.SaveChanges();
                            }
                            // Games saved so remove from the session
                            Session[GamesController.SessionTempGames] = null;
                        }
                        // Redirect logged in user to home.
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Otherwise return them to the login page
                        ViewBag.FailedLogin = "Incorrect email address or password";
                        return View(user);
                    }
                    
                }
            }

            // Otherwise return them to the login page
            ViewBag.FailedLogin = "Incorrect email address or password";
            return View(user);
        }

        /// <summary>
        /// Logs the user out by clearing the ENTIRE session.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            // Kill the session variables & return to login page
            Session.Clear();
            // Send the logged out user to the home page
            return RedirectToAction("Index", "Home");
        }
       
        // Auto-Generated
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
