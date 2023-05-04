# Flighta

Flighta is a fictional flight agency project made in .NET Core which consists of two project types: an MVC website and an API.



## MVC Website

The MVC website has several controllers:

### Flights

The Flights controller offers a page for clients to view available flights to book with flight number, flight time, and number of seats. If an admin is logged in, it shows an edit button, a delete button, and a create flight button.

### Users

The Users controller offers a page for only admins to view current users with their user ID, username, email, and role (client or admin).

### Bookings

The Bookings controller will be automatically populated for clients, showing only their booked flights and a delete button. For admins, it shows all bookings made by all users. There is a delete button to delete bookings for which users have to provide a reason, and those deleted bookings will appear in red on the bookings list with their reason for deletion.

### Contact Us

The Contact Us page allows users to send feedback to the website developers.

## API

The API uses JWT authentication, and almost all controllers are locked down only for users with the role of "Admin." Here's a breakdown of what each controller offers:

### Auth

The Auth controller takes in a username and password and generates a JWT token.

### Flights

The Flights controller offers CRUD operations for flights.

### Users

The Users controller offers CRUD operations for users.

### Bookings

The Bookings controller offers CRUD operations for bookings.

### SendEmail

The SendEmail API controller uses an SMTP client to send feedback to the website developers.

## Technologies Used

The project uses ADO.NET to communicate with the database with queries or stored procedures and implements a SQL database to store the information. The website uses Bootstrap 5 with jQuery, and a JavaScript library called Toastr to display notifications. There are also Bootstrap modals and dropdown buttons.
