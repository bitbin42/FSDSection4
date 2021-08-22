FSD .Net training capstone project

Phase4_Personal_project:
Code created as an exploration of React/webapi

MMStoreClient: REACT front-end code
MMStoreServer: C# web API back-end code

API.txt: defines the back-end API call formats
CreateReactApp.txt: commands for initializing the react project
CreateTables.txt: SQL commands to create tables, initial data, and sample users/passwords in the database
MMStore.postman_collection.json: sample API calls for use in postman
MMStore_initial_db.zip: backup of initialized SQL database


The client app 
Note that the client app starts chrome with --disable-web-security and --user-data-dir=C:/ChromeDevSession
This is needed to allow cross-origin api calls when the client is on port 3000 and the server on port 5000
The default chrome started by npm won't do that -- exit that browsr and use Visual Studio code to test the app

The MMStoreClient\src\api.js file specifies the sever and port to use for making calls to the backend server

The MMStoreServer\MSTests folder contains a basic NUinit test configuration for validating API calls to the back-end code
