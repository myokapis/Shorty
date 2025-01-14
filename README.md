# Shorty
A fun .Net API demo project that shortens URLs

## Overview
Shorty is a .Net Core web API that exposes endpoints for working with URLs.
- /Url/ShortenUrl: accepts a URL and generates a unique, shortened URL.
- /Url/ExpandUrl: accepts a shortened URL and returns the original URL.



## Running the Application
### Assumptions
This documentation assumes that the user is running a current version of Visual Studio on Windows
and has .Net 9.0 installed. The code should run fine in other IDEs on other operating systems;
however, it has not been tested on any platform other than Visual Studio on Windows.

### Starting the Application
The project has configuration to listen for http or https requests on designated ports and to accept
requests through IIS Express on another port. Select one of those options in the Visual Studio
dropdown. Then start the project without the debugger enabled either by clicking the hollow green
arrow in the Visual Sudio toolbar or by typing Ctl + F5.

### Submitting Requests via Swagger
When the project starts, a browser window will open to /swagger/index.html and
display the available endpoints. Each endpoint will have a dropdown arrow on the far right
hand side. Clicking that dropdown arrow will expand the details for the endpoint.

Once the endpoint details have been expanded, a "Try it out" button will be visible just
below the expansion arrow. Clicking the button will make the request body writable. Enter
the request data into the request body. Both endpoints are configured to accept a string
parameter from the request body, so be sure to enclose the data in double quotes.

*Example: "https://my.domain.com/some/path"*

Click the "Execute" button to submit the data to the endpoint. When the response is returned,
it will be displayed in the Response Body under the Response section.

### URL Format
The application accepts properly formatted URLs containing protocol, domain, path, and optional 
query string. It isn't designed to parse partial or relative URLs. See the examples below.

__GOOD__ *"https://ferrets.com/products?id=ahr5"*

__BAD__ *"ferrets.com/products?id=ahr5"* (protocol is missing)

__BAD__ *"/products?id=ahr5"* (relative URL)