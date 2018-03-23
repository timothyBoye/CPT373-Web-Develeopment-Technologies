using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace GoLA2.Models.Database
{
    /// <summary>
    /// Validation metadata for the EF Users class/table
    /// </summary>
    public class UsersMetadata
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    // Adds the Metadata class to the EF User class
    [MetadataType(typeof(UsersMetadata))]
    public partial class User
    {

    }


    /// <summary>
    /// Validation metadata for the EF UserTemplates class/table
    /// </summary>
    public class TemplateMetadata
    {
        [Required]
        public string Name { get; set; }

        [Required, Range(1, Int32.MaxValue)]
        public int Height { get; set; }

        [Required, Range(1, Int32.MaxValue)]
        public int Width { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Cells { get; set; }
    }

    /// <summary>
    /// Validator class for the more complex properties of the UserTemplate class
    /// </summary>
    public class TemplateValidator : AbstractValidator<UserTemplate>
    {
        /// <summary>
        /// Checks that a given Template meets all the complex rules provided in
        /// the game of life template class (Uses Template.ValidTemplate()).
        /// </summary>
        public TemplateValidator()
        {
            RuleFor(x => x).Must(x =>
                {
                    // Try to validate the the template
                    try
                    {
                        Logic.Template.ValidTemplate(x.Height, x.Width, x.Cells.Split(new[] {"\n", "\r\n" }, StringSplitOptions.None));
                        return true;
                    }
                    // Invalid template
                    catch
                    {
                        return false;
                    }
                }
                // Returned message if the template is wrong
                ).WithMessage("Cells must contain only X's and O's and have a height and width that match those supplied.");
        }
    }

    // Attachs the metadata validation and the complex validator class to the EF template class
    [MetadataType(typeof(TemplateMetadata))]
    [Validator(typeof(TemplateValidator))]
    public partial class UserTemplate
    {

    }



    /// <summary>
    /// Validation metadata for the EF UserGames class/table
    /// </summary>
    public class GameMetadata
    { 
        public int UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Range(1, Int32.MaxValue)]
        public int Height { get; set; }

        [Required, Range(1, Int32.MaxValue)]
        public int Width { get; set; }

        public string Cells { get; set; }

    }

    // Attaches the metadata validation to the EF UserGame class
    [MetadataType(typeof(GameMetadata))]
    public partial class UserGame
    {

    }
}