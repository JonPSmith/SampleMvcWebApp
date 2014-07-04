SampleMvcWebApp
===============

SampleMvcWebApp is a ASP.NET MVC5 web site designed to show number of useful methods for building enterprise grade web applications using ASP.NET MVC5 and Entity Framework 6. The code for this sample MVC web application, and the associated [GenericServices Framework](https://github.com/JonPSmith/GenericServices) are both an open source project by [Jon Smith](http://www.thereformedprogrammer.net/about-me/) under the [MIT licence](http://opensource.org/licenses/MIT).

This code is available as a [live web site](http://samplemvcwebapp.net/) which includes explanations of the code - see an example of this on the [Posts code explanation](http://samplemvcwebapp.net/Posts/CodeView) page.

The GenericService Framework is available on [GitHub](https://github.com/JonPSmith/GenericServices) and soon via NuGet (when the release is stable).

The specific features in this code are:

### 1. Simple, but robust database services

Database accesses are normally a big part of enterprise systems build with APS.NET MVC. However, my experience is that creating these services in a robust and comprehensive form can lead to a lot of repetative code that does the same thing, but for different data. My aim has been to produce a generic framework that handles most of the cases, and is easily extensible when special handling is required. Examples of there use on this web site are:

 - See normal, synchronous access using a DTO for shaping in the [Posts Controller](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/PostsController.cs)
 - See new EF6 async access using a DTO for shaping in the [PostsAsync Controller](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/PostsAsyncController.cs)
 - See normal, synchronous access directly via data class in the [Tags Controller](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/TagsController.cs)
 - See new EF6 async access directly via data class in the [TagsAsync Controller](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/TagsAsyncController.cs)

### 2. Handling long running tasks

Having long running tasks on a web site without feedback is not what users expect. My mathematic modelling applications have a lot of long running tasks so I have developed specific code for displaying progress messages and allowing the user to cancel a task etc.

 - See the [ActionController](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Controllers/ActionController.cs) for a number of action methods
 - You can find a [BBC Radio schedule searcher task](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/ServiceLayer/BBCScheduleService/Concrete/ScheduleSearcherAsync.cs) being called from [this view](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Views/Action/Radio4Search.cshtml)
 - Other variations of long running actions can be found in [this view](https://github.com/JonPSmith/SampleMvcWebApp/blob/master/SampleWebApp/Views/Action/Index.cshtml)

### 3. Use of Dependency Injection

 The GenericService framework is designed specifically to work with Dependency Injection (DI). DI is used throughout this web site, but specific examples are:

 - Inserting the required services into a controller by action parameter injection.
 - DI is also used for creating the GenericService etc. See Code Explanation for more information.

Note that the SampleMvcWebApp uses AutoFac dependency injection framework, but the framework allows you to replace AutoFac with your own favourite DI tool.
