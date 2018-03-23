using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoLA2.Models.Database;
using PagedList;

namespace GoLA2.Controllers
{
    /// <summary>
    /// This controller handles Template related views, access
    /// to template models and any other template related methods
    /// </summary>
    public class TemplatesController : Controller
    {
        // Database access
        private DatabaseEntities db = new DatabaseEntities();

        /// <summary>
        /// This method provides a view with a list of all templates in the 
        /// database. The method also supports paging, searching and sorting 
        /// of that data.
        /// GET: UserTemplates
        /// </summary>
        /// <param name="sortOrder">The order string to use</param>
        /// <param name="searchString">The string to search for</param>
        /// <param name="currentFilter">The string that was previously searched</param>
        /// <param name="page">The page the user is on</param>
        /// <returns>The templates view</returns>
        // NOTE: This method is large, the vast majority is repeated in MyIndex
        // and a lot is similar to Games/Index. Thus this method and those 
        // should be refactored to remove as much repetition as possible in the 
        // next code iteration. Should be relatively easy but not worth doing
        // the day before deadline in case of issues.
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            // save the sort order
            ViewBag.CurrentSort = sortOrder;
            // save the various other sorts too
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.HeightSortParm = sortOrder == "height" ? "height_desc" : "height";
            ViewBag.WidthSortParm = sortOrder == "width" ? "width_desc" : "width";
            ViewBag.CellSortParm = sortOrder == "cell" ? "cell_desc" : "cell";
            ViewBag.UserSortParm = sortOrder == "user" ? "user_desc" : "user";

            // if there is a new search set the page to one
            if (searchString != null)
            {
                page = 1;
            }
            // if not set the new search to the old search so we can page through
            else
            {
                searchString = currentFilter;
            }

            // save the new/old search (even if blank)
            ViewBag.CurrentFilter = searchString;
            
            // Get the list of templates from the db
            var userTemplates = db.UserTemplates.Include(u => u.User);

            // If there is a search to be made discard any templates that don't match it
            if (!String.IsNullOrEmpty(searchString))
            {
                userTemplates = userTemplates.Where(x => x.Name.Contains(searchString));
            }

            // now we have the templates sort the list based on sortOrder or default
            switch (sortOrder)
            {
                case "name_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Name);
                    break;
                case "height_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Height);
                    break;
                case "width_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Width);
                    break;
                case "cell_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Cells);
                    break;
                case "user_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.User.FirstName);
                    break;
                case "height":
                    userTemplates = userTemplates.OrderBy(x => x.Height);
                    break;
                case "width":
                    userTemplates = userTemplates.OrderBy(x => x.Width);
                    break;
                case "cell":
                    userTemplates = userTemplates.OrderBy(x => x.Cells);
                    break;
                case "user":
                    userTemplates = userTemplates.OrderBy(x => x.User.FirstName);
                    break;
                default:
                    userTemplates = userTemplates.OrderBy(x => x.Name);
                    break;
            }

            // set the page size, the page we're on and return the page we're on's templates
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(userTemplates.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// This method provides a view with a list of templates belonging to
        /// the logged in user from the database. The method also supports 
        /// paging, searching and sorting of that data. A user MUST be logged in.
        /// GET: User's UserTemplates
        /// </summary>
        /// <param name="sortOrder">The order string to use</param>
        /// <param name="searchString">The string to search for</param>
        /// <param name="currentFilter">The string that was previously searched</param>
        /// <param name="page">The page the user is on</param>
        /// <returns>The my templates view</returns>
        // NOTE: See Index for refactoring notes
        [Authorization]
        public ActionResult MyIndex(string sortOrder, string searchString, string currentFilter, int? page)
        {
            // get the user from the session
            User user = (User)Session[UsersController.SessionUser];
            // save the sort order
            ViewBag.CurrentSort = sortOrder;
            // save the various other sort parametres
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.HeightSortParm = sortOrder == "height" ? "height_desc" : "height";
            ViewBag.WidthSortParm = sortOrder == "width" ? "width_desc" : "width";
            ViewBag.CellSortParm = sortOrder == "cell" ? "cell_desc" : "cell";

            // if there is a NEW search set page to one
            if (searchString != null)
            {
                page = 1;
            }
            // otherwise set the search to the old search so we can page through it
            else
            {
                searchString = currentFilter;
            }

            // save the new/old search (even if blank)
            ViewBag.CurrentFilter = searchString;

            // discard any templates that aren't by this user
            var userTemplates = db.UserTemplates.Where(x => x.UserID == user.UserID);

            //if there is a search discard any templates that don't match the string
            if (!String.IsNullOrEmpty(searchString))
            {
                userTemplates = userTemplates.Where(x => x.Name.Contains(searchString));
            }

            // Now we have our list sort it based on SortOrder or default
            switch (sortOrder)
            {
                case "name_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Name);
                    break;
                case "height_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Height);
                    break;
                case "width_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Width);
                    break;
                case "cell_desc":
                    userTemplates = userTemplates.OrderByDescending(x => x.Cells);
                    break;
                case "height":
                    userTemplates = userTemplates.OrderBy(x => x.Height);
                    break;
                case "width":
                    userTemplates = userTemplates.OrderBy(x => x.Width);
                    break;
                case "cell":
                    userTemplates = userTemplates.OrderBy(x => x.Cells);
                    break;
                default:
                    userTemplates = userTemplates.OrderBy(x => x.Name);
                    break;
            }

            // set page size, the page number and return the current pages data
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(userTemplates.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// This method provides a summary view of the template that matches
        /// the id provided
        /// GET: UserTemplates/Details/5
        /// </summary>
        /// <param name="id">id of template to summarise</param>
        /// <returns>The details view</returns>
        public ActionResult Details(int? id)
        {
            // No id? NO PAGE FOR YOU!
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Get the template from the db
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            // if the template is null the id asked for doesnt exist, not found
            if (userTemplate == null)
            {
                return HttpNotFound();
            }

            // Return the details view with the template asked for
            return View(userTemplate);
        }

        /// <summary>
        /// This method provides the create template view/form.
        /// This form can only be accessed by a logged in user.
        /// GET: UserTemplates/Create
        /// </summary>
        /// <returns>the create template form/view</returns>
        [Authorization]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// This method handles postback from the create template form
        /// and can only be used by a logged in user. The method check the
        /// form data is valid then saves the new template to the database.
        /// POST: UserTemplates/Create
        /// </summary>
        /// <param name="userTemplate"></param>
        /// <returns>Redirect or the create form with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization]
        public ActionResult Create([Bind(Include = "Name,Height,Width,Cells")] UserTemplate userTemplate)
        {
            // Check the form data is a valid template (Validator class handles this)
            if (ModelState.IsValid)
            {
                // set the userid and user to that of the creator
                userTemplate.UserID = ((User)Session[GoLA2.Controllers.UsersController.SessionUser]).UserID;
                userTemplate.User = db.Users.First(x => x.UserID == userTemplate.UserID);

                // add the template to the database, save and redirect to the list
                db.UserTemplates.Add(userTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // errors in the form, return to the form with validation errors marked
            return View(userTemplate);
        }


        /// <summary>
        /// This method provides the delete confirmation view for the template
        /// matching the given id. The user who created the template must be
        /// logged in for this method to work.
        /// GET: UserTemplates/Delete/5
        /// </summary>
        /// <param name="id">Id of the template to be deleted</param>
        /// <returns>the delete confirmation view</returns>
        [Authorization]
        public ActionResult Delete(int? id)
        {
            // No id? NO PAGE FOR YOU!
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get the template matching the id
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            // No template? mustn't exist return not found
            if (userTemplate == null)
            {
                return HttpNotFound();
            }
            // if the userid matches the user logged in return the view
            else if (userTemplate.UserID == ((User)Session[UsersController.SessionUser]).UserID)
            {
                return View(userTemplate);
            }
            // the user id mustn't match the user logged in, fail silently.
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// This method handles the postback for the delete confirmation
        /// view, if the user calls this method they must be confirming 
        /// deletion and thus the template matching the id given will be 
        /// deleted.
        /// POST: UserTemplates/Delete/5
        /// </summary>
        /// <param name="id">Id of template to delete</param>
        /// <returns>redirect to list</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorization]
        public ActionResult DeleteConfirmed(int id)
        {
            // Get the template again
            UserTemplate userTemplate = db.UserTemplates.Find(id);
            // Double check the user owns this template (postback could be corrupt/hacked maybe?)
            if (userTemplate.UserID == ((User)Session[UsersController.SessionUser]).UserID)
            {
                // remove template, save then redirect to list
                db.UserTemplates.Remove(userTemplate);
                db.SaveChanges();
                return RedirectToAction("MyIndex");
            }
            // THIS SHOULD NOT BE HAPPENING OMG NO! Oh well no harm done, fail silently.
            else
            {
                return RedirectToAction("Index", "Home");
            }
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
