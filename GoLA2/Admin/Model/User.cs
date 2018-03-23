namespace GoLA2.Admin.Model
{
    /// <summary>
    /// This class denotes the structure of a User for storing
    /// the Admin user when it is pulled from the database. 
    /// This effectively is a duplication of the auto generated
    /// entity framework class but wasn't sure I could use it in
    /// the WebForms section for the assignment.
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}