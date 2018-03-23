using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoLA2.Models.Database;
using PagedList;
using GoLA2.Models.Logic;

namespace GoLA2.Controllers
{

    /// <summary>
    /// Provides access to all game functionality and thus also all Game views
    /// </summary>
    public class GamesController : Controller
    {
        // Database connection
        private DatabaseEntities db = new DatabaseEntities();
        /// <summary>String to access the games stored in the Session variable for non logged in users</summary>
        public static String SessionTempGames = "TempGames";
        /// <summary>String to access the current active game stored in the 
        /// Session variable for all users. This variable gets clobbered by
        /// new games. Players MUST use the save function to save the state
        /// of their game.</summary>
        public static String SessionActiveGame = "ActiveGame";


        /// <summary>
        /// This method provides the view of the list of the users games,
        /// if the user is not logged in it shows the session games if they
        /// are logged in it gets their games from the database. This method
        /// also provides sorting, searching and paging of the games list.
        /// GET: Games
        /// </summary>
        /// <param name="sortOrder">String representing the sort order</param>
        /// <param name="searchString">The string the user wants to search for in their games</param>
        /// <param name="currentFilter">The string the user searched for previously to be rendered on the page as the current search</param>
        /// <param name="page">The index of the page of data to display</param>
        /// <returns>The Index view</returns>
        // NOTE: See Templates/Index for refactoring notes
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            // Save the sort string for later
            ViewBag.CurrentSort = sortOrder;

            // save the state of the various sorts
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.HeightSortParm = sortOrder == "height" ? "height_desc" : "height";
            ViewBag.WidthSortParm = sortOrder == "width" ? "width_desc" : "width";
            ViewBag.CellSortParm = sortOrder == "cell" ? "cell_desc" : "cell";

            // if there is a search to be made we goto page one
            if (searchString != null)
            {
                page = 1;
            }
            // if not then set the search to the previous search
            // (this allows us to page through search results without
            // resetting the search the second we go to page 2)
            else
            {
                searchString = currentFilter;
            }

            // Set the currentfilter to the searchstring so we can render it on the page
            ViewBag.CurrentFilter = searchString;

            // If the session games list doesn't exist make one or we'll get null errors
            if (Session[SessionTempGames] == null)
                Session[SessionTempGames] = new List<UserGame>();

            // Prepare the list to be rendered on the page
            List<UserGame> userGames;
            // Check if the user is logged in and get thier games if they are
            if (Session[UsersController.SessionUser] != null)
            {
                // Get the user object then get all their games from the db
                User user = (User)Session[UsersController.SessionUser];
                var dbGames = db.UserGames.Include(u => u.User).Where(x => x.UserID == user.UserID);
                // If there is a search to be done save only games matching it 
                if (!String.IsNullOrEmpty(searchString))
                {
                    userGames = dbGames.Where(x => x.Name.Contains(searchString)).ToList();
                }
                // else save all games
                else
                    userGames = dbGames.ToList();
            }
            // The user isnt logged in so get the session games
            else
            {
                // get the games from the session
                userGames = (List<UserGame>)Session[SessionTempGames];
                // if their is a search throw away any games that don't match
                if (!String.IsNullOrEmpty(searchString))
                {
                    userGames = userGames.FindAll(x => x.Name.Contains(searchString));
                }
            }

            // Now we have the games sort them based on the SortOrder the user 
            // provided or default if none
            switch (sortOrder)
            {
                case "name_desc":
                    userGames = userGames.OrderByDescending(x => x.Name).ToList();
                    break;
                case "height_desc":
                    userGames = userGames.OrderByDescending(x => x.Height).ToList();
                    break;
                case "width_desc":
                    userGames = userGames.OrderByDescending(x => x.Width).ToList();
                    break;
                case "cell_desc":
                    userGames = userGames.OrderByDescending(x => x.Cells).ToList();
                    break;
                case "height":
                    userGames = userGames.OrderBy(x => x.Height).ToList();
                    break;
                case "width":
                    userGames = userGames.OrderBy(x => x.Width).ToList();
                    break;
                case "cell":
                    userGames = userGames.OrderBy(x => x.Cells).ToList();
                    break;
                default:
                    userGames = userGames.OrderBy(x => x.Name).ToList();
                    break;
            }

            // set the size of a page and the page number we're on
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            // Return the list of games as a paged list thus only return the
            // items on the page we're currently on
            return View(userGames.ToPagedList(pageNumber, pageSize));
        }


        /// <summary>
        /// This method provides the play game view, the game it displays is
        /// the game matching the id variable.
        /// GET: Games/Play
        /// </summary>
        /// <param name="id">id of game</param>
        /// <returns>Play view</returns>
        public ActionResult Play(int? id)
        {
            // Bad request if no id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the UserGame from either the DB or session depending on user
            UserGame userGame;
            if (Session[UsersController.SessionUser] != null)
                userGame = db.UserGames.Find(id);
            else
                userGame = ((List<UserGame>)Session[SessionTempGames]).Find(x => x.UserGameID == id);

            // If the game is null it doesn't exist return not found
            if (userGame == null)
            {
                return HttpNotFound();
            }

            // create a GameOfLife and write it to the session
            Session[SessionActiveGame] = new GameOfLife(userGame, (int)id, userGame.UserID, userGame.User);

            // return the play view
            return View();
        }

        /// <summary>
        /// This method responds to post calls from the play game view (AJAX calls).
        /// The game is retreived from the session, a game tick is played and the 
        /// new state of the cells is returned (as a string).
        /// POST: Games/PlayGameTick
        /// </summary>
        /// <returns>Multiline string of '█'s and ' 's representing alive and dead cells</returns>
        [HttpPost]
        public string PlayGameTick()
        {
            // get the game and play a turn on it
            ((GameOfLife)Session[SessionActiveGame]).TakeTurn();

            // Return the new game state as a string
            return 
                ((GameOfLife)Session[SessionActiveGame])
                .ToString('█', ' ');
        }

        /// <summary>
        /// This method responds to post calls from play game view (AJAX).
        /// The game is retrieved from the session and the current state of
        /// that game is used to overwrite the game in the session or database
        /// based on user.
        /// </summary>
        [HttpPost]
        public void SaveGame()
        {
            // get the game id
            int id = ((GameOfLife)Session[SessionActiveGame]).UserGameID;
            // create a new usergame by calling SaveGame
            UserGame uGame = ((GameOfLife)Session[SessionActiveGame]).SaveGame();
            // If the user is logged in delete the old game,
            // update the usergame with the users details, then save the game to the db
            if (Session[UsersController.SessionUser] != null)
            {
                var dbgame = db.UserGames.Remove(db.UserGames.First(x => x.UserGameID == id));
                uGame.UserID = ((User)Session[UsersController.SessionUser]).UserID;
                uGame.User = db.Users.First(x => x.UserID == uGame.UserID);
                db.UserGames.Add(uGame);
                db.SaveChanges();
            }
            // If not a logged in user get the session game list and overwrite the old game
            else
            {
                List<UserGame> list = (List<UserGame>)Session[SessionTempGames];
                list[list.FindIndex(x => x.UserGameID == id)] = uGame;
            }
        }

        /// <summary>
        /// This method returns the create game form/view with the
        /// templates that exist in the database as a list the user 
        /// can choose from.
        /// GET: Games/Create
        /// </summary>
        /// <returns>The create game view</returns>
        public ActionResult Create()
        {
            // if there is no session games list create it or we'll get null errors
            if (Session[SessionTempGames] == null)
                Session[SessionTempGames] = new List<UserGame>();
            // Get the templates and provide them to the view
            ViewData["Templates"] = new SelectList(db.UserTemplates.ToList(), "UserTemplateID", "Name");
            // return the view
            return View();
        }

        /// <summary>
        /// Handles the posted create game form. When a form is posted the validity
        /// of the forms data is checked and if valid a game is created and saved
        /// to the session or database depending on the user.
        /// POST: Games/Create
        /// </summary>
        /// <param name="Templates">The id of the template</param>
        /// <param name="uGame">the usergame object filled out by the form</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Templates, [Bind(Include = "Name,Height,Width")] UserGame uGame)
        {
            // check the session has a games list otherwise we'll get null errors
            if (Session[SessionTempGames] == null)
                Session[SessionTempGames] = new List<UserGame>();

            // Validate the form
            if (ModelState.IsValid)
            {
                // Make a GameOfLife out of the form data to further validate it
                // This could be done with another Validator class in future but
                // it works here too for now.
                
                // Create the game
                GameOfLife game = new GameOfLife(uGame.Height, uGame.Width, uGame.Name);
                //get the template id
                int usertemplateIDint = int.Parse(Templates);
                //Get the template from the db based on the id
                UserTemplate usertemplate = db.UserTemplates.First(x => x.UserTemplateID == usertemplateIDint);
                // Check the template will fit
                if (game.Height >= usertemplate.Height && game.Width >= usertemplate.Width)
                {
                    // make a GameOfLife Template from the EF Template
                    Template template = new Template(usertemplate);
                    // Insert the template into the game at 0,0
                    // Spec did not require ability for user to
                    // choose the insertion location, so the user gets it
                    // in the origin.
                    game.InsertTemplate(template, 0, 0);
                    // Create a UserGame from the GameOfLife
                    UserGame userGame = game.SaveGame();

                    // If the user is logged in save game to db
                    if (Session[UsersController.SessionUser] != null)
                    {
                        userGame.UserID = ((User)Session[UsersController.SessionUser]).UserID;
                        userGame.User = db.Users.First(x => x.UserID == userGame.UserID);
                        db.UserGames.Add(userGame);
                    }
                    // if not save to session (null the db IDs they are invalid if any)
                    else
                    {
                        userGame.User = null;
                        userGame.UserID = 0;
                        userGame.UserGameID = ((List<UserGame>)Session[SessionTempGames]).Count()+1;
                        ((List<UserGame>)Session[SessionTempGames]).Add(userGame);
                    }

                    // Save the databse and redirect to the game list
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                // Model was invalide set the validation label to tell the user they done goofed
                else
                {
                    ModelState.AddModelError("", "The size must be bigger than the template.");
                }
            }
            // There must be errors, repopulate the template list and return
            // to the form to tell the user they done goofed
            ViewData["Templates"] = new SelectList(db.UserTemplates.ToList(), "UserTemplateID", "Name");
            return View(uGame);
        }

        /// <summary>
        /// Provides a delete confirmation view for the id passed to the method.
        /// GET: Games/Delete/5
        /// </summary>
        /// <param name="id">id of game</param>
        /// <returns>delete game view</returns>
        public ActionResult Delete(int? id)
        {
            // No id? NO PAGE FOR YOU
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the usergame object from the db or session
            UserGame userGame;
            if (Session[UsersController.SessionUser] != null)
                userGame = db.UserGames.Find(id);
            else
                userGame = ((List<UserGame>)Session[SessionTempGames]).Find(x => x.UserGameID == id);

            // if usergame is null then it wasn't found, return not found
            if (userGame == null)
            {
                return HttpNotFound();
            }
            
            // returns the delete confirm view for the usergame found above
            return View(userGame);
        }

        /// <summary>
        /// This method accepts posts from the delete confirmation view.
        /// When called it assumed that the user has now confirmed they
        /// wish to delete the game corresponding to the provided id.
        /// POST: Games/Delete/5
        /// </summary>
        /// <param name="id">Id of game to delete</param>
        /// <returns>Redirect to game list</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // If the user is logged in call remove on the db
            UserGame userGame;
            if (Session[UsersController.SessionUser] != null)
            {
                userGame = db.UserGames.Find(id);
                db.UserGames.Remove(userGame);
                db.SaveChanges();
            }
            // otherwise remove on the the session list
            else
            {
                ((List<UserGame>)Session[SessionTempGames]).RemoveAll(x => x.UserGameID == id);
            }

            // Redirect to the game list which hould now have one less game
            return RedirectToAction("Index");
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
