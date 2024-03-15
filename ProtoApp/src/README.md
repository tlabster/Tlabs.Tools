# ProtoApp
Tlabs Library based application prototype to demonstrate a typical design of the
* Application configuration
* Module structure layout
* Data store persistence
* Web API implementation

### General project structure
The application prototype comprises of three sub-projects

1. The main executable `ProtoApp.exe` is implemented in the `Server` folder.  
(Server, since it is forming a hosted web-server.)  
This project notably contains the prototype applications configuration and all of its file system based resources (like a Sqlite database file in `rsc/store` or static web resources in `rsc/webroot`).  

2. An application's prototype `Module`.  
(Such a module would typically structure some functional related services and APIs of a large application...)  

3. A storage provider (and database type) specific `Store` module.  
(This is to isolate storage specific dependencies. In the prototype case dependencies to EF Core [via Tlabs.EfDataStore]
and a Sqlite database provider...)  


```
ProtoApp
  +---Server
  |   +---API
  |   +---Controller
  |   |   \---About
  |   +---Model
  |   +---Properties
  |   \---rsc
  |       +---store
  |       \---webroot
  +---Module
  |   +---API
  |   +---Model
  |   \---Service
  \---Store
      +---Migrations
      \---Store
```
### Configuration
The *ProtoApp* configuration can be found in the `Server/appsettings.json` file.  
This is where all configurable aspects of the application are defined.
