using GoLA2.Models.Logic;
using System.Data;
using System.Data.SqlClient;

namespace GoLA2.Admin.Model
{
    /// <summary>
    /// Contains static classes for working with the database using
    /// an ADO.NET implementation. This class is used by the WebForms
    /// Admin site only.
    /// </summary>
    public class Database
    {
        // Connection string for connecting to the database
        private static string DBConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"\\\\Mac\\Home\\Documents\\Visual Studio 2015\\Projects\\GoLA2\\GoLA2\\App_Data\\Database.mdf\";Integrated Security=True;MultipleActiveResultSets=True";

        /// <summary>
        /// Gets an SqlConnection object preloaded with the database 
        /// connection string.
        /// </summary>
        /// <returns>SqlConnection preloaded with the database string</returns>
        private static SqlConnection DatabaseConnection()
        {
            return new SqlConnection(DBConnectionString);
        }

        /// <summary>
        /// Gets the admin user assosiated with the provided email and
        /// password, if the email and password do not match a user or 
        /// the user is not an admin null is returned.
        /// </summary>
        /// <param name="email">Email address of the admin user to find</param>
        /// <param name="password">Associated (not hashed) password, this function will hash it automatically</param>
        /// <returns>User object of the admin user or null if not an admin</returns>
        public static User GetAdmin(string email, string password)
        {
            var User = new User();
            using (var db = DatabaseConnection())
            {
                // Setup the sql stored procedure 
                var command = new SqlCommand("Login", db);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter emailsql = command.Parameters.Add("@email", SqlDbType.VarChar);
                emailsql.Direction = ParameterDirection.Input;
                emailsql.Value = email;

                // call the procedure and fill the datatable
                var DataTable = new DataTable();
                var Adapter = new SqlDataAdapter(command);
                Adapter.Fill(DataTable);
                // If a user was returned fill in the User object data
                if (DataTable.Rows.Count > 0)
                {
                    DataRow row = DataTable.Rows[0];

                    User.UserID = (int)row["UserID"];
                    User.Password = (string)row["Password"];
                    User.FirstName = (string)row["FirstName"];
                    User.LastName = (string)row["LastName"];
                    User.Email = (string)row["Email"];
                    User.IsAdmin = (bool)row["IsAdmin"];
                }
                // no user found at email address return null
                else
                    return null;
            }

            // User was found check password and admin status return User
            // If BOTH are true
            if (CryptSharp.Crypter.CheckPassword(password, User.Password) && User.IsAdmin)
                return User;
            // Password or Admin status invalid return null either way
            // Don't bother providing better context why help a hacker
            // brute force even just a little.
            else
                return null;
        }

        /// <summary>
        /// Returns a DataTable of all the users in the database, 
        /// NOTE: This does NOT include passwords
        /// </summary>
        /// <returns>DataTable with columns UserID,Email,FirstName,LastName,IsAdmin</returns>
        public static DataTable GetAllUsers()
        {
            using (var db = DatabaseConnection())
            {
                // Set up stored procedure
                var command = new SqlCommand("GetAllUsers", db);
                command.CommandType = CommandType.StoredProcedure;

                // Run the command
                var DataTable = new DataTable();
                var Adapter = new SqlDataAdapter(command);
                Adapter.Fill(DataTable);

                // If there are any users return the table
                if(DataTable.Rows.Count > 0)
                    return DataTable;
                // If not null will do
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns a DataTable of all the templates in the database.
        /// </summary>
        /// <returns>DataTable with columns UserTemplateID, UserID, Name, Height, Width, Cells</returns>
        public static DataTable GetAllTemplates()
        {
            using (var db = DatabaseConnection())
            {
                // Setup the stored procedure
                var command = new SqlCommand("GetAllTemplates", db);
                command.CommandType = CommandType.StoredProcedure;

                //Run the command
                var DataTable = new DataTable();
                var Adapter = new SqlDataAdapter(command);
                Adapter.Fill(DataTable);

                // if there are any templates return the table
                if (DataTable.Rows.Count > 0)
                    return DataTable;
                // if not just null
                else
                    return null;
            }
        }

        /// <summary>
        /// Deletes the user at id if they exist.
        /// WARNING: there is no confirmation, 
        /// this method WILL delete the user immediately
        /// </summary>
        /// <param name="id">UserID of the user to be deleted</param>
        /// <returns>Number of rows affected (should be 1 hopefullly)</returns>
        public static int DeleteUser(int id)
        {
            using (var db = DatabaseConnection())
            {
                // setup the stored procedure
                db.Open();
                var command = new SqlCommand("DeleteUser", db);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter UserID = command.Parameters.Add("@userID", SqlDbType.Int);
                UserID.Direction = ParameterDirection.Input;
                UserID.Value = id;
                //Execute the command and return the rows affected
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates and adds to the database a new template after it has been
        /// validated by the Template class. This method will throw an exception
        /// if the template is invalid.
        /// </summary>
        /// <param name="userID">The UserID of the user submitting the template</param>
        /// <param name="name">The name of the template</param>
        /// <param name="height">The height of the template</param>
        /// <param name="width">The width of the template</param>
        /// <param name="cells">A string containing \n or \n\r to delimit the grid rows</param>
        /// <returns>Number of rows affected (should be 1)</returns>
        public static int UploadTemplate(int userID, string name, int height, int width, string cells)
        {
            // try make template to ensure valid
            Template template = new Template(name, height, width, cells);

            using (var db = DatabaseConnection())
            {
                // setup the stored procedure with all the variables
                db.Open();
                var command = new SqlCommand("UploadTemplate", db);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter UserID = command.Parameters.Add("@userID", SqlDbType.Int);
                UserID.Direction = ParameterDirection.Input;
                UserID.Value = userID;
                SqlParameter Name = command.Parameters.Add("@name", SqlDbType.VarChar);
                Name.Direction = ParameterDirection.Input;
                Name.Value = name;
                SqlParameter Height = command.Parameters.Add("@height", SqlDbType.Int);
                Height.Direction = ParameterDirection.Input;
                Height.Value = height;
                SqlParameter Width = command.Parameters.Add("@width", SqlDbType.Int);
                Width.Direction = ParameterDirection.Input;
                Width.Value = width;
                SqlParameter Cells = command.Parameters.Add("@cells", SqlDbType.VarChar);
                Cells.Direction = ParameterDirection.Input;
                Cells.Value = cells;

                // Execute the command and return rows affected
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the template at id if they exist.
        /// WARNING: there is no confirmation, 
        /// this method WILL delete the template immediately
        /// </summary>
        /// <param name="id">TemplateID of the template to be deleted</param>
        /// <returns>Number of rows affected (should be 1 hopefullly)</returns>
        public static int DeleteTemplate(int id)
        {
            using (var db = DatabaseConnection())
            {
                // Steup the stored procedure
                db.Open();
                var command = new SqlCommand("DeleteTemplate", db);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter TemplateID = command.Parameters.Add("@templateID", SqlDbType.Int);
                TemplateID.Direction = ParameterDirection.Input;
                TemplateID.Value = id;

                // Execute command and return rows affected
                return command.ExecuteNonQuery();
            }
        }
    }
}