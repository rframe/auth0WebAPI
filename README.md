# auth0WebAPI


## Project Requirements
- [Dot Net Core](https://www.microsoft.com/net/learn/get-started/windows)

## Configure Application
- Update the appsettings files `(\Auth0WebAPI\appsettings.Development.json, \Auth0WebAPI\appsettings.json)`
- Files should have the following values set
- Cors.Origin
- Cors.DevlopmentOrigin
- Auth0.ApiIdentifier
- Auth0.Domain
- Auth0.ManagementDomain
- Auth0.ManagementToken


## Run Project Locally
- From root/Auth0WebAPI
- Run `dotnet build`
- Run `dotnet run`
- Update the API Location in the Angular Application to point to the server that this process started