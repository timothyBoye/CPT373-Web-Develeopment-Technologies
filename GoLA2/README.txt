All work is my own unless otherwise specified however I 
have taken inspiration from other sources for example
sorting and paging were inspired by tutorial materials and
microsoft samples.

All dot points on the marking guide should be present in this 
assignment including bonus marks 'stored procedures' for part B
and 'upload template file' also in part B.

There are however a few sections I would refactor in future as code
is repeated or to meet spec I've had to rewrite code that would otherwise
be provided by Entity Framework (in Part B) but the code all works so 
should meet spec.

Game:
- Active games are NEVER saved back to db/session UNLESS the user manually
clicks the save button this is deliberate as the user may not wish to overwrite
the previous save.
- Session games are AUTOMATICALLY saved to the db when a user logs in (and
removed from the session), this is deliberate as a now logged in user would
want their games associated with their account.
- Logged in users New Games are saved AUTOMATICALLY to the db upon creation
it is assumed a logged in user would expect their games to not disappear with
the session.
- Games are sortable, searchable and paged

Users:
- Email addresses must be unique in db when registering.
- p-admin@gmail.com / u-rmit already exists and is an admin user feel free to 
use this to test the admin site etc rather than edit the database manually to
create an admin user.
- If you do edit the database note paswords are hashed, so creating a user 
manually would be problematic
- When an ADMIN user is logged in a link to the Admin site will appear in the top
right nav, this only takes you to the page (can also manually go to admin/index.aspx)
it doesn't log you in (I could easily have transfered the user to the admin site
logged in but it would have entailed checking if the MVC EF user was an admin and 
was concerned this would count as using EF in part 2, instead the user must login
again and a different session variable/User class is used).

Templates:
- Templates page is identical when logged in or not except when logged in a create
button will appear in the top right (under the nav)
- Link to my templates only shows for logged in users and is in the top right of 
the nav
- Templates are sortable, searchable and paged

Part B:
- Site redirects users to the login page if they hit any other page if not logged in
(appears to be secure, but I don't know much about webforms security)
- BONUS stored procedures used for every function no matter how trivial
- BONUS a well formatted text document can be uploaded on the template page to the 
db
- BONUS BONUS with help from microsoft samples and a lot of rage the templates and
users pages on the WebForms site are also sortable searchable and paged!!! 

I think that's everything...



TL;DR:
I did ALL the things... I think... "Please don't break, please don't break..."
