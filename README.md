# Creating a Url Shortener Service From Scratch with .Net Core

In this tutorial we will be creating a url shortener service like bit.ly. 
We are going to create anonymous API endpoints for generating short urls, and redirecting to original url if generated short url is called.

![Image Credit: Mulesoft](https://cdn-images-1.medium.com/max/800/0*LuC2zX2ROv6v_WDE.png)

The following technologies will be utilized;

- .Net Core 3.0 Web API for backend
- Mongo Db Cloud Atlas for data storage
- Docker for containerization

---

## Here are the steps we will follow

1. Create a Mongo DB Cloud Atlas Cluster.
2. Create .Net Core Web API
3. Dockerize the API.
4. Publish API to Heroku for free.

---

# Create a Mongo DB Cloud Atlas Cluster

Go to [https://cloud.mongodb.com/user#/atlas/login](https://cloud.mongodb.com/user#/atlas/login) page, register if you are not registered before, or log in if you already registered.

After you logged in, click the "Build a Cluster" button under Clusters. If you are on the free tier, you can only create one cluster, so you will have to delete the previous cluster(s) if you have another previously and plan to stay on the free tier.

![Free-Tier Cluster](https://cdn-images-1.medium.com/max/800/1*1ixNfE3_HUaPcTBEQjaYUw.png)

Click the "Create a cluster" button, give your cluster an informative name for this project, select a cloud provider of your liking, and a region closest to you.

![Select Cloud Provider & Region](https://cdn-images-1.medium.com/max/800/1*fAA7cMCPJptaddcfPpTlbQ.png)

After you select a cloud provider and region, click "Create Cluster". This may take between 7 to 10 minutes. While waiting for the cluster to be ready, let’s create a user to connect to this database. Go to **Database Access** under **Security**. Generally, it is not a good practice to use the admin account for applications, so we will be creating a user for this application. Click the **ADD NEW USER** button. Enter a username of your liking and a password, or you can auto-generate a secure password. Normally, it is a better practice to customize this user to access only the dedicated database for this user, but for now, select **Read and Write to Any Database**. Click **Add User** when you are ready.

![MongoDB Users](https://cdn-images-1.medium.com/max/800/1*NGgdsUbk8szhnHpzA62dqg.png)

![Add User Window](https://cdn-images-1.medium.com/max/800/1*eFaFgoGdjBDcgfxZMHILXQ.png)

Before connecting to the cluster, we need to complete one more step. Click **Network Access** from the left menu. Click **Add IP Address** button.

![Network Access Menu](https://cdn-images-1.medium.com/max/800/1*wWyK4ElLZ_SdXawS5-FvaA.png)

Click **Allow Access From Anywhere** on the modal window and click **Confirm**. This enables this cluster to be accessed from any IP.

![Allow Access From Anywhere](https://cdn-images-1.medium.com/max/800/1*GgCyPYfKPYZZ9eQrSamBjQ.png)

By now, your cluster should be ready. Click the **CONNECT** button to make configurations for connection.

![Your cluster is ready!](https://cdn-images-1.medium.com/max/800/1*Kq6eDJz-RfGNMBQo6b6LLg.png)

Click the **Connect Your Application** tab, select **C#** as the driver, and **2.5 or Later** as the version. Note the connection string somewhere, we will be using it later in our API project.

![Connection Options](https://cdn-images-1.medium.com/max/800/1*TPDy6cogXWthLiOdRnwpEA.png)

![Connection String](https://cdn-images-1.medium.com/max/800/1*MG0y4BHMLGYaPV8Dm14-7Q.png)

---

# Create API

#### Prequisites

[Install .Net Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0) if you do not have it.

[Install Visual Studio 2019](https://visualstudio.microsoft.com/vs/) as I find it more convenient for .NET Projects rather than VS Code.

[Creating a Mongo Db Cloud Atlas cluster](https://medium.com/@bugrasitemkar/create-a-mongo-db-cloud-atlas-cluster-9d711b483e66) before starting the project will be handy. You can also create at the final step.

#### Create Solution and API Project

From the Visual Studio 2019 Splash Screen, select Create a New Project option. Select ASP.NET Core Web Application C#, click Next.

Enter Project Name: ShortUrl.API

Solution Name: ShortUrl

Click Create. In the next screen Select ASP.NET Core 3.0 and API. Click Create.

![](https://cdn-images-1.medium.com/max/800/1*ElwxpXWyOCyuzyH2ou_LrA.png)

Delete WeatherForecastController under Controllers folder, and WeatherForecast.cs class file.

Although actual development is more back and forth process, and not as straight forward as this tutorial is going to be, I will go through each project in solution one by one for the sake of simplicity.

#### Create Model Project

Let’s create model project which we will be using across all other projects. Right click on ShortUrl Solution, Add, New Project. Select Class Library (.NET Standart) and name is as “Model”.

We need to include MongoDB.Bson library in this project as we are going to add Base model and DTO model to this project and add Mongo DB properties. Type the following command to Package Manager Console. Please be sure you have selected “Model” project on the Default Project dropdown of Package Manager Console.

Install-Package MongoDB.Bson -Version 2.9.3

Before adding models, we need a custom validation attribute to check the url send.

Right click on model, select “Add Folder”, name the new folder as Validation. Right click on validation folder and add a new class file named as “CustomValidation.cs”. In custom validation class, we are accepting full urls as valid (either http or https).

Finally two entity models to use with Mongo Db, a request and a response model.

File Structure of Model project is as follows:

— Model Project

— —Validation Folder

— — — CustomValidation.cs

— —MongoBaseModel.cs

— —ShortURLModel.cs

— — ShortURLRequestModel.cs

— — ShortUrlResponseModel.cs

[**bugrasitemkar/UrlShortenerService**  
github.com](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Model "https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Model")[](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Model)

#### Create Repository Project

We are going to move developing the project from repository to service to API finally. Create a Repository project in the same fashion as Model project under ShortUrl solution.

Before adding any class, first include the dependencies for the repository project.

From the package manager, select Repository project as default project and execute the following command to include MongoDb driver package.

Install-Package MongoDB.Driver.Core -Version 2.9.3

Also right click on Dependencies, Click Add Reference, from the projects, select Model project and add as dependency.

After the dependencies are added, first add an Abstract folder under Repository project. This folder is going to contain our interface for ShortUrl repository. This IRepository, interface is going to contain necessary methods we will be using through our service.

Then we can include a generic repository for MongoDb and inherit from this generic repository to produce more specialized repositories for task needed. I have named generic repository as MongoRepository.

Finally we are going to add a specialized Repository, I name it as ShortUrlRepository. This class inherits from MongoRepository and implements IRepository interface.

Here is the file structure for repository project:

— Repository Project (.NET Core Class Library)

—— Abstract

— — — IRepository.cs

— — MongoRepository.cs

— — ShortUrlRepository.cs

[**bugrasitemkar/UrlShortenerService**  
github.com](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Repository "https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Repository")[](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Repository)

#### Create Service Project

Next, create a service layer. Controller layer should only receive the request, call the service and return the response as its single responsibility. Service layer will be used for mapping Request/Response models to DTO’s and back as well as any business we need to implement.

Before adding classes, include models and repository projects as dependency just like we did in repository project.

Create another class library project same as Repository project. Name it Service. Create Abstract, Map, and Utilities folders in the project. The file structure going to be as follows:

— Service Project (.NET Core Class Library)

—— Abstract

— —— IShortUrlService.cs

— — Map

— — — ShortUrlModelMapper.cs

— — Utilities

— — — TokenGenerator.cs

— — ShortUrlService.cs

The Short Url Service Interface, contains necessary service methods.

Add token generator under utilities folder, which will generate short url tokens.

Then add Short Url Model Mapper class handles the mapping operation and generates short url from token generator while mapping request model to dto model.

Finally let’s add Service class.

[**bugrasitemkar/UrlShortenerService**  
github.com](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Services "https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Services")[](https://github.com/bugrasitemkar/UrlShortenerService/tree/master/Services)

#### API Project

Now we have our model, repository and service projects are ready, we continue implementing the API project.

**Add Dependencies**

First let’s add necessary Nuget Packages.

In the Package Manager Console tab, Paste following commands.

This package is for easy API versioning:

Install-Package Microsoft.AspNetCore.Mvc.Versioning -Version 4.0.0

This package is for logging:

Install-Package NLog.Web -Version 4.9.0

Include all three Model, Service, and Repository projects as dependencies.

**Create Controller**

Right click on controller folder, Add, Controller. Select API Controller — Empty. Name it as ShortUrlController.

_Redirecting or returning the short url model !_

If you notice, the Get method takes a redirect parameter from query string, which has default value set as true. If redirect query parameter is not send or send as true, the method redirects to the original url found using short url, and if redirect method is false, Get method returns the model with found short url and does not redirect. Overloading a method in this manner may not be the best practice in a real production environment. I did this way, for the sake of simplicity but normally you should create another controller to handle redirect functionality.

_Versioning the controller_

I like Url versioning, found it more readable and understandable.

\[ApiVersion(“1”)\] indicates the version of this controller.

\[Route(“api/v{version:apiVersion}/\[controller\]”)\] enables us to inject ApiVersion property above to controller route such as api/v1/shorturl in this example.

_Logging_

We are defining logger (ILogger<ShortUrlController> logger) and injecting the logging interface which enables us to use any logging solution. We are using NLog in this project.

Right click on ShortUrl.API project, Select Add, New Item. Search for config in the search box top right. Select Web Configuration File and name it as “nlog.config”. We will be using this file to configure logging.

One last step is necessary to enable logging with NLog, change the Program.cs class as follows which sets the NLog as the default logging provider to this application with a log level of trace.

**Adding Services to Container / Configuring request Pipeline**

The middlewares can be added directly under ConfigureServices and request pipeline can be configured directly under Configure method in the Startup.cs file. I like creating Extension methods and calling them from ConfigureServices and Configure methods more.

Let’s create an Extensions folder under ShortUrl.API project.

_Global Error Handling_

Implementing a global error handling is very userful. The following extension method is going to return 500 HTTP status code with the exception message in case of any unhandled exception occurs within the project.

Create Error Model class under Extension directory. Then create a Configure Extension class.

We will be calling this method later from Startup.cs class.

_Configuring Cors and Registering Service and Repository classes._

We need to configure Cross Origin access to not to limit API requests to a single origin. If you decide to not to give access to this API openly but to consume from just a frontend application, you need to configure Cross Origin access accordingly. We will talk about it later when building frontend application so you do not need to worry about that now.

Also registering Repository and Service classes for injection in this Extension too.

Create a class named ServiceExtensions under Extensions folder and add following methods.

We will have to call this methods from Startup.cs too.

Finally, call the middlewares from Startup class.

Notice we are also adding API Versioning as a service in the ConfigureServices method (services.AddApiVersioning()).

**Adding Development Settings**

Add the connection string from connect to short-url section of your created mongo db cloud cluster to appsettings.Development.json

**Adding Production Settings**

I recommend always adding a Production settings file in order to keep environmental configuration separate.

Right click on ShortURL.API project, select Add, New Item. Search for appsettings in the right top search box. Select App Settings File and name it as appsettings.Production.json. Visual studio is going to automatically add Connection String and Logging sections. We are going to edit this file prior to deploying API to production.

[**bugrasitemkar/UrlShortenerService**  
github.com](https://github.com/bugrasitemkar/UrlShortenerService "https://github.com/bugrasitemkar/UrlShortenerService")[](https://github.com/bugrasitemkar/UrlShortenerService)

We have created the backend application with following features;

*   Core functionality: Creating a short url link from a url, retrieving previously created short url link from a url, redirecting to a url using created short url.
*   API Versioning
*   Logging with NLog
*   Global Exception Handling

I omit API documentation with swagger or hateoas links intentionally to keep the project simpler. Please keep in mind, API documentation is one of the necessary steps to develop a proper API.


How to Dockerize .Net Core 3.0 Application \* { font-family: Georgia, Cambria, "Times New Roman", Times, serif; } html, body { margin: 0; padding: 0; } h1 { font-size: 50px; margin-bottom: 17px; color: #333; } h2 { font-size: 24px; line-height: 1.6; margin: 30px 0 0 0; margin-bottom: 18px; margin-top: 33px; color: #333; } h3 { font-size: 30px; margin: 10px 0 20px 0; color: #333; } header { width: 640px; margin: auto; } section { width: 640px; margin: auto; } section p { margin-bottom: 27px; font-size: 20px; line-height: 1.6; color: #333; } section img { max-width: 640px; } footer { padding: 0 20px; margin: 50px 0; text-align: center; font-size: 12px; } .aspectRatioPlaceholder { max-width: auto !important; max-height: auto !important; } .aspectRatioPlaceholder-fill { padding-bottom: 0 !important; } header, section\[data-field=subtitle\], section\[data-field=description\] { display: none; }

How to Dockerize .Net Core 3.0 Application
==========================================

Prequisite for this step:

* * *

### How to Dockerize .Net Core 3.0 Application

![](https://cdn-images-1.medium.com/max/800/0*LbT0_vVM3DLpruSg.png)

**Prequisite for this step:**

*   Install Docker to your development environment

[**Docker Documentation**  
_Try our multi-part walkthrough that covers writing your first app, data storage, networking, and swarms, and ends with…_docs.docker.com](https://docs.docker.com/ "https://docs.docker.com/")[](https://docs.docker.com/)

*   If you do not have a sample asp.net core 3.0 project, you can clone the following repo to use as a sample.

[**bugrasitemkar/UrlShortenerService**  
github.com](https://github.com/bugrasitemkar/UrlShortenerService "https://github.com/bugrasitemkar/UrlShortenerService")[](https://github.com/bugrasitemkar/UrlShortenerService)

**Create a Dockerfile**

Create a file on your solution folder (where ShourtURL.sln is if you are using the Repository above), name the file as Dockerfile. Copy the code piece to your Dockerfile and save.

Each dockerfile should start with FROM keyword. FROM sets the base image for dockerfile, you can either start from scratch or set a previous image for base image. Here we are setting an .Net Core 3.0 image as base.

WORKDIR command sets the current working directory.

EXPOSE command exposes the ports we are going to use. Here we will be using port 80 and going to map to another port when running container.

In the second block, we are setting working directory as “src”, copying ShortUrl.API project (replace with your main project in the solution if you are not using the repository as example) to working directory running restore command to restore dependencies. Then building the API project by calling RUN dotnet build.

Next block, we are publishing the project. And In the final block, setting the entrypoint of the container.

**Build and Run the Container**

Open a terminal or command prompt and navigate to folder where dockerfile is.

_First build the container by running following command:_

“docker build -t shorturl:v1 .”

“-t” parameter sets the tag name of the container, we name it as “shorturl” and “:v1” part is the version number, which I always suggest using. If you do not provide a version number, docker automatically adds “:latest”.

_After container builds, you can run the following command to run the container:_

“docker run -p 8080:80 shorturl:v1”

“-p” parameter sets the port to be mapped, in this case we are mapping 80 (whi ch is exposed in dockerfile) to 8080.

Up to this point if everything is fine, you should have the container up and running.

**Test your application**

This part is only applicable If you are following [Creating a Url Shortener Service From Scratch with .Net Core 3.0](https://medium.com/@bugrasitemkar/creating-a-url-shortener-service-from-scratch-with-net-core-e8ebacad12c1) series, or used the sample github repo above.

You can use the following requests to test endpoints running in docker container.

_Get all entries:_

GET: [http://localhost:8080/api/v1/shorturl](http://localhost:8080/api/v1/shorturl)

_Post a long url and get a shorturl:_

POST: [http://localhost:8080/api/v1/shorturl](http://urlnet.xyz:8080/api/v1/shorturl)

Add to header → Content-Type: application/json

Add to body →

{  
 “LongURL”: “[http://www.medium.com](http://www.medium.com)"  
}

Example Response:

_Redirect to a shorturl:_

Redirects to medium.com;

GET: [http://localhost:8080/api/v1/shorturl](http://urlnet.xyz:8080/api/v1/shorturl)/E30Ff

Return related object from DB;

GET: [http://localhost:8080/api/v1/shorturl](http://urlnet.xyz:8080/api/v1/shorturl)/E30Ff?redirect=false

![](https://cdn-images-1.medium.com/max/800/0*Piks8Tu6xUYpF4DU)
