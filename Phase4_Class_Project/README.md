This contains 7 projects:

Group 1: .net C# application
EHealth.Shared: .net common code for accessing the database
EHealth.Admin.Web: .net web client front-end for administrators
EHealth.User.Web: .net web client front-end for regular users

Group 2: .net webapi backend and react frontend samples
EHealth.Api: not used?
EHealth.Api.Shared: .net code for accessing the database
EHealth.Web.React: REACT frontend
EHealth.Web.ReactWithAuth: REACT frontend

The EHealth_empty_db.zip file contains a backup of the initial database. Unzip and restore to use.
The db_create.txt contains SQL statements to create the application tables.  It will not create the identity tables.

Check all appsettings.json files to verify that the DefaultConnection is correct.
