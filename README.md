<h1 align="center">Flighta Project ReadMe 🫡</h1>

Flighta is a fictional flight agency project made in .NET Core consisting of an MVC website and an API. The MVC website offers several controllers such as Flights, Users, Bookings, and SendEmail API to communicate with the website developers. Users can view available flights and book them through the website, and admins can manage flights, users, and bookings. The API is used to perform CRUD operations for flights, users, and bookings.

Some of the notable features of Flighta include:

- User authentication using encrypted cookies to manage the user's state.
- JWT authentication in the API to secure endpoints and ensure only admins can access them.
- Usage of ADO.NET to communicate with the SQL database, which stores all the information.
- Implementation of Bootstrap 5 with jQuery, Toastr, Bootstrap modals and dropdowns to create a responsive UI.
- Contact Us page and SendEmail API controller that uses SMTP client to send feedback to website developers.
- The repository contains a postman collection export which is used to test the API and it's authentication and can be linked to a monitor

Overall, Flighta is a well-designed, secure, and user-friendly flight booking website that can be used as a base for building more complex flight booking systems. The use of modern technologies such as .NET Core, JWT authentication, and Bootstrap 5 ensures that the website is fast, reliable, and responsive.
<hr/>

<h2 align="center"> Technologies used 🧪</h2>


<div align="center">
<a href="https://dotnet.microsoft.com/">
<img height="32" width="32" src="https://cdn.simpleicons.org/dotnet/purple"/>&nbsp;&nbsp;</a>
<a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads">
<img height="32" width="32" src="https://cdn.simpleicons.org/microsoftsqlserver/red"/>&nbsp;&nbsp;</a>
<a href="https://getbootstrap.com/">
<img height="32" width="32" src="https://cdn.simpleicons.org/bootstrap/#7952B3"/>&nbsp;&nbsp;</a>
<a href="https://en.wikipedia.org/wiki/HTML5">
<img height="32" width="32" src="https://cdn.simpleicons.org/html5/#E34F26"/>&nbsp;&nbsp;</a>
<a href="https://dotnet.microsoft.com/en-us/languages/csharp">
<img height="32" width="32" src="https://cdn.simpleicons.org/csharp/#239120"/>&nbsp;&nbsp;</a>
<a href="https://developer.mozilla.org/en-US/docs/Web/CSS">
<img height="32" width="32" src="https://cdn.simpleicons.org/css3/#1572B6"/>&nbsp;&nbsp;</a>
<a href="https://developer.mozilla.org/en-US/docs/Web/javascript">
<img height="32" width="32" src="https://cdn.simpleicons.org/javascript/#F7DF1E"/>&nbsp;&nbsp;</a>
<a href="https://jquery.com/">
<img height="32" width="32" src="https://cdn.simpleicons.org/jquery/#0769AD"/>&nbsp;&nbsp;</a>
<a href="https://www.postman.com/">
<img height="32" width="32" src="https://cdn.simpleicons.org/postman/#FF6C37"/>&nbsp;&nbsp;</a>
<a href="https://github.com/CodeSeven/toastr">
<img height="32" width="32" src="https://nuget.org/Content/gallery/img/default-package-icon-256x256.png"/></a>
</div>
<hr/>
<h2 align="center"> Screenshots 🌠 </h2>


<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236459622-77b96e5e-9027-469b-9c6f-c4611fd24ec3.png" width="80%">
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236459702-4ecfd70b-460d-4910-b524-b6317e352f0c.png" width="80%">
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236459764-40edb947-e084-4ff1-811b-083f7535e297.png" width="80%">
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236459838-013026f9-749d-4406-83c3-ea48d22f587d.png" width="80%">
</p>

<p align="center">
  <img src="(https://user-images.githubusercontent.com/25421570/236459799-857c8722-c745-4274-b412-5daa492677c2.png" width="80%">
</p>

<p align="center">
  <img src="(https://user-images.githubusercontent.com/25421570/236459883-61f0556f-ac81-46e9-acd9-7b15c802a999.png" width="80%">
</p>

<p align="center">
  <img src="(https://user-images.githubusercontent.com/25421570/236459899-d3629be5-50d0-45a1-a7aa-55d1e9174230.png" width="80%">
</p>

<p align="center">
  <img src="(https://user-images.githubusercontent.com/25421570/236465253-2939bb45-5292-4dd4-bf56-fd04308b2ea9.png" width="80%">
</p>
<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236465253-2939bb45-5292-4dd4-bf56-fd04308b2ea9.png" width="80%">
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/25421570/236465737-8ad88a42-e293-4082-b9e0-9b2de81a624c.png" width="80%">
</p>


</hr>
<h2 align="center"> The MVC part of the project 🦸‍♂️</h2>

<h3 align="center"> Flights </h3>
<p align="center">
The Flights controller offers a page for clients to view available flights to book with flight number, flight time, and number of seats. If an admin is logged in, it shows an edit button, a delete button, and a create flight button.
</p>

<h3 align="center"> Users </h3>
<p align="center">
The Users controller offers a page for only admins to view current users with their user ID, username, email, and role (client or admin).
</p>

<h3 align="center"> Bookings </h3>
<p align="center">
The Bookings controller will be automatically populated for clients, showing only their booked flights and a delete button. For admins, it shows all bookings made by all users. There is a delete button to delete bookings for which users have to provide a reason, and those deleted bookings will appear in red on the bookings list with their reason for deletion.
</p>

<h3 align="center"> Contact Us </h3>
<p align="center">
The Contact Us page allows users to send feedback to the website developers.
</p>
</hr>
<h2 align="center"> The API ⚒️ </h2>
<p align="center">
The API uses JWT authentication, and almost all controllers are locked down only for users with the role of "Admin." Here's a breakdown of what each controller offers:
</p>

<h3 align="center"> Auth </h3>
<p align="center">
The Auth controller takes in a username and password and generates a JWT token.
</p>

<h3 align="center"> Flights </h3>
<p align="center">
The Flights controller offers CRUD operations for flights.
</p>

<h3 align="center"> Users </h3>
</p align="center">
The Users controller offers CRUD operations for users.
</p>

<h3 align="center"> Bookings </h3>
<p align="center">
The Bookings controller offers CRUD operations for bookings.
</p>

<h3 align="center"> Send Email </h3>
<p align="center">
The SendEmail API controller uses an SMTP client to send feedback to the website developers.
</p>
