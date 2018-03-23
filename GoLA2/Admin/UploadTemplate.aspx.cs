using GoLA2.Admin.Model;
using GoLA2.Models.Logic;
using System;
using System.IO;

namespace GoLA2.Admin
{
    public partial class UploadTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called when the user clicks the upload button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpload_click(object sender, EventArgs e)
        {
            // If the user has selected a file try and create a template
            if (fileUpload.HasFile)
            {
                // create the template variables
                string name = fileUpload.FileName.Split(new[] { "." }, StringSplitOptions.None)[0];
                string cells;
                int height, width;
                int userID = ((User)Session[Site1.WebFormsUser]).UserID;

                try
                {
                    // set the template variables by reading the file
                    using (StreamReader inputStreamReader = new StreamReader(fileUpload.PostedFile.InputStream))
                    {
                        height = int.Parse(inputStreamReader.ReadLine());
                        width = int.Parse(inputStreamReader.ReadLine());
                        cells = inputStreamReader.ReadToEnd();
                    }
                    // Create a template to call its automatic validation will throw exception if there is an error in the file
                    Template template = new Template(name, height, width, cells);
                    // Template is valid if we get here so add it to the database (use the template classes cell string for consistency)
                    Database.UploadTemplate(userID, name, height, width, template.ToString());
                    
                    // Let the user know the upload worked
                    Validation.Text = name + " has been uploaded";
                    Validation.CssClass = "text-success";
                }
                // Issue with the file or the format is wrong
                catch (Exception ex)
                {
                    // Tell the user off for procided a bad file (is an admin so the exception messages are ok)
                    Validation.Text = ex.Message;
                    Validation.CssClass = "text-danger";
                }
            }
            // Odd they clicked upload but didn't select a file
            else
            {
                // Tell the user off
                Validation.Text = "No file uploaded";
                Validation.CssClass = "text-danger";
            }
        }

    }
}