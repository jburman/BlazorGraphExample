# Overview
BlazorGraphExample demonstrates a self contained [Blazor](https://blazor.net) application that connects to Microsoft's Graph API, queries the user's information and lets them browse through their OneDrive. There is not any server side code and you can try a [demo](https://blazorgraph.z20.web.core.windows.net/) hosted in an Azure Static Web site.

**Example**

<img src="https://github.com/jburman/BlazorGraphExample/raw/master/Docs/onedrive.png" />

Authentication calls are handled by the [AuthService](https://github.com/jburman/BlazorGraphExample/blob/master/BlazorGraphExample/Services/AuthService.cs), 
which provides a thin wrapper over the [MSAL.js library](https://github.com/AzureAD/microsoft-authentication-library-for-js) 
that is designed for SPA applications. Once a token is acquired through MSAL, all of the Graph API calls are made 
from the [GraphService](https://github.com/jburman/BlazorGraphExample/blob/master/BlazorGraphExample/Services/GraphService.cs) 
using a few hand coded classes (just to keep things simple).

To try the sample out yourself:
- Clone the repository
- [Register a new app](https://apps.dev.microsoft.com/) with Microsoft. Be sure to select Web Platform and check Allow Implicit Flow.
- Fill in the Redirect URI (the project uses https://localhost:44395/ by default)
- Copy the Client ID and update Program.cs to use it.
  
You can now either run the project from Visual Studio using IISExpress or with something like dotnet-serve. 

**Run with [dotnet-serve](https://www.nuget.org/packages/dotnet-serve/)**

1. From the folder with the BlazorGraphExample.sln file run a dotnet publish.
Example: `dotnet publish -o c:\publish`
2. Change to the dist folder (for example: c:\publish\BlazorGraphExample\dist)
3. Launch the index.html: `dotnet-serve -S -p 44395 -o`
