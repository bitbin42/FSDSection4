This contains 2 projects:

MMStoreServer: aspnet webapi project; see the api.txt file for documentation on the calls
MMStoreClient: REACT client app; see the CreateReactApp.txt file for info on starting it

MMStore_initial_db.zip contains a backup of the initial database which can be unzipped and restored
The CreateTables.txt file contains SQL statements for creating the tables and initial data as an alternative.

Note that the client app starts chrome with --disable-web-security and --user-data-dir=C:/ChromeDevSession
This is needed to allow cross-origin api calls when the client is on port 3000 and the server on port 5000
The default chrome started by npm won't do that -- exit that browsr and use Visual Studio code to test the app

The MMStoreClient\src\api.js file specifies the sever and port to use for maing calls to the backend server
