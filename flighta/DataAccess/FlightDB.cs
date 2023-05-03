﻿using Flighta.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static Flighta.ApiControllers.BookingController;
using static Flighta.ApiControllers.FlightsController;
using static Flighta.ApiControllers.UsersController;

namespace Flighta.DataAccess
{
    public class FlightDB
    {
        public readonly IConfiguration _config;
        public FlightDB(IConfiguration config)
        {
            _config = config;
        }
        public async Task<List<User>> GetUsers()
        {
            List<User> users = new List<User>();
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand("SELECT userId,username,hashedPass,roleId,email FROM Users", conn);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var role = reader.GetInt32(3);
                User user = new User(
                    reader.GetInt32(0), //userid
                    reader.GetString(1), //username
                    reader.GetString(2), //password
                    reader.GetString(4), //email
                    (role == 1) ? Roles.Client : Roles.Admin //role
                );
                users.Add(user);
            }

            reader.Close();
            return users;
        }
        public async Task<List<FlightM>> GetFlights()
        {
            List<FlightM> flights = new List<FlightM>();
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [flightId],[from],[to],[num_of_seats],[flight_time],[cost] FROM [Flights]"

                , conn);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                FlightM flight = new FlightM(
                    reader.GetInt32(0), //flightid
                    reader.GetString(1), //from
                    reader.GetString(2), //to
                    reader.GetInt32(3), //seats
                    reader.GetDateTime(4), //flight_time
                    reader.GetDecimal(5) //cost
                );
                flights.Add(flight);
            }

            reader.Close();
            return flights;
        }
        public async Task<bool> LoginUser(string emailOrUsername, string pass)
        {
            bool result = false;
            var hash = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pass)));
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand($"EXEC LoginUser @emailOrUsername,@hash", conn);
            cmd.Parameters.AddWithValue("@emailOrUsername", emailOrUsername);
            cmd.Parameters.AddWithValue("@hash", hash);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result = reader.GetInt32(0) == 1 ? true : false; //check user
            }

            reader.Close();
            return result;
        }
        public async Task<User?> GetUser(string term)
        {
            User user = null!;
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC GetUser @term", conn);
                cmd.Parameters.AddWithValue("@term", term);
                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    user = new User(
                       reader.GetInt32(0), //userid
                       reader.GetString(1), //username
                       (reader.GetInt32(2) == 1) ? Roles.Client : Roles.Admin //role
                   );
                }

                reader.Close();
            }
            catch
            {
                return null;
            }
            return user;
        }
        public async Task<bool> CreateUser(User user)
        {
            try
            {
                var hashedpassword = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC Admin_CreateUser @username,@password,@role,@email", conn);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", hashedpassword);
                cmd.Parameters.AddWithValue("@role", user.Role == Roles.Client ? 1 : 2);
                cmd.Parameters.AddWithValue("@email", user.Email);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var hashedpassword = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC Admin_UpdateUser @userid,@username,@password,@role,@email", conn);
                cmd.Parameters.AddWithValue("@userid", user.userId);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", hashedpassword);
                cmd.Parameters.AddWithValue("@role", user.Role == Roles.Client ? 1 : 2);
                cmd.Parameters.AddWithValue("@email", user.Email);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE userId = @userid", conn);
                cmd.Parameters.AddWithValue("@userid", id);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch { return false; }
        }
        public async Task<FlightM?> GetFlight(int id)
        {
            FlightM flight = null;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [flightId],[from],[to],[num_of_seats],[flight_time],[cost] FROM [Flights] WHERE flightId = @id"

                , conn);
            cmd.Parameters.AddWithValue("@id", id);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flight = new FlightM(
                    reader.GetInt32(0), //flightid
                    reader.GetString(1), //from
                    reader.GetString(2), //to
                    reader.GetInt32(3), //seats
                    reader.GetDateTime(4), //flight_time
                    reader.GetDecimal(5) //cost
                );
            }

            reader.Close();
            return flight;
        }
        public async Task<bool> AddFLight(FlightM flight)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC Admin_AddFlight @from,@to,@seats,@time,@cost", conn);
                cmd.Parameters.AddWithValue("@from", flight.From);
                cmd.Parameters.AddWithValue("@to", flight.To);
                cmd.Parameters.AddWithValue("@seats", flight.seats);
                cmd.Parameters.AddWithValue("@time", flight.flightTime);
                cmd.Parameters.AddWithValue("@cost", flight.cost);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateFlight(FlightM flight)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC Admin_UpdateFlight @flightid,@from,@to,@seats,@time,@cost", conn);
                cmd.Parameters.AddWithValue("@flightid", flight.flightId);
                cmd.Parameters.AddWithValue("@from", flight.From);
                cmd.Parameters.AddWithValue("@to", flight.To);
                cmd.Parameters.AddWithValue("@seats", flight.seats);
                cmd.Parameters.AddWithValue("@time", flight.flightTime);
                cmd.Parameters.AddWithValue("@cost", flight.cost);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteFlight(int id)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("DELETE FROM Flights WHERE FlightId = @flightid", conn);
                cmd.Parameters.AddWithValue("@flightid", id);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch { return false; }
        }
        public async Task<bool> BookFlight(BookingDetailsDto details)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("EXEC BookFlight @userid,@flightid", conn);
                cmd.Parameters.AddWithValue("@userid", details.userid);
                cmd.Parameters.AddWithValue("@flightid", details.flightid);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<BookingDetails>> GetBookings()
        {
            List<BookingDetails> bookings = new List<BookingDetails>();
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand("SELECT UserId,FlightId,book_time FROM BookedFlights", conn);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                BookingDetails booking = new BookingDetails(
                    reader.GetInt32(0), //userid
                    reader.GetInt32(1), //flightid
                    reader.GetDateTime(2) //Book_time
                );
                bookings.Add(booking);
            }

            reader.Close();
            return bookings;
        }
        public async Task<BookingDetails?> GetBooking(int userid)
        {
            BookingDetails booking = null;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand("SELECT UserId,FlightId,book_time FROM BookedFlights WHERE userId = @userid", conn);
            cmd.Parameters.AddWithValue("@userid", userid);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                booking = new BookingDetails(
                    reader.GetInt32(0), //flightId
                    reader.GetInt32(1), //UserId
                    reader.GetDateTime(2) //Book_time
                );
            }

            reader.Close();
            return booking;
        }
        public async Task<bool> DeleteBooking(BookingDetailsDto details)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("DELETE FROM BookedFlights WHERE userId = @userid AND flightId = @flightid", conn);
                cmd.Parameters.AddWithValue("@userid", details.userid);
                cmd.Parameters.AddWithValue("@flightid", details.flightid);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch { return false; }
        }
        public async Task<bool> DeleteBookingWithReason(BookingDetailsDto details,string reason)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
                SqlCommand cmd = new SqlCommand("DELETE FROM BookedFlights WHERE userId = @userid AND flightId = @flightid", conn);
                SqlCommand cmd2 = new SqlCommand("INSERT INTO CancelledBookings (flightid,userid,Reason) VALUES (@flightid,@userid,@reason)", conn);
                cmd.Parameters.AddWithValue("@userid", details.userid);
                cmd.Parameters.AddWithValue("@flightid", details.flightid);
                cmd2.Parameters.AddWithValue("@userid", details.userid);
                cmd2.Parameters.AddWithValue("@flightid", details.flightid);
                cmd2.Parameters.AddWithValue("@reason", reason);
                await conn.OpenAsync();
                await cmd2.ExecuteNonQueryAsync();
                return await cmd.ExecuteNonQueryAsync() == 0 ? false : true;
            }
            catch { return false; }
        }
        public async Task<FlightM?> GetFlightid(CreateFlightDto dto)
        {
            FlightM flight = null;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [flightId],[from],[to],[num_of_seats],[flight_time],[cost] FROM [Flights] WHERE [from] = @from AND [to] = @to AND [num_of_seats] = @seats AND [flight_time] = @time AND [cost] = @cost"

                , conn);
            cmd.Parameters.AddWithValue("@from", dto.from);
            cmd.Parameters.AddWithValue("@to", dto.to);
            cmd.Parameters.AddWithValue("@seats", dto.seats);
            cmd.Parameters.AddWithValue("@time", dto.time);
            cmd.Parameters.AddWithValue("@cost", dto.cost);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                flight = new FlightM(
                    reader.GetInt32(0), //flightid
                    reader.GetString(1), //from
                    reader.GetString(2), //to
                    reader.GetInt32(3), //seats
                    reader.GetDateTime(4), //flight_time
                    reader.GetDecimal(5) //cost
                );
            }

            reader.Close();
            return flight;
        }
        public async Task<User?> GetUserId(CreateUserDto dto)
        {
            User user = null;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [userId],[username],[hashedPass],[roleid],email FROM [Users] WHERE [username] = @username"

                , conn);
            cmd.Parameters.AddWithValue("@username", dto.username);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                user = new User(
                    reader.GetInt32(0), //userid
                    reader.GetString(1), //username
                    reader.GetString(2), //pass
                    reader.GetString(4), //email
                    reader.GetInt32(3) == 1 ? Roles.Client : Roles.Admin //roleid
                );
            }

            reader.Close();
            return user;
        }
        public async Task<User?> GetUserIdbyUsername(string username)
        {
            User user = null;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [userId],[username],[hashedPass],[roleid],email FROM [Users] WHERE [username] = @username"

                , conn);
            cmd.Parameters.AddWithValue("@username", username);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                user = new User(
                    reader.GetInt32(0), //userid
                    reader.GetString(1), //username
                    reader.GetString(2), //pass
                    reader.GetString(4), //email
                    reader.GetInt32(3) == 1 ? Roles.Client : Roles.Admin //roleid
                );
            }

            reader.Close();
            return user;
        }
        public async Task<Roles> GetRole(string username)
        {
            Roles role = Roles.None;
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand(

                "SELECT [roleid] FROM [Users] WHERE [username] = @username"

                , conn);
            cmd.Parameters.AddWithValue("@username", username);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                role = reader.GetInt32(0) == 1 ? Roles.Client : Roles.Admin;
            }

            reader.Close();
            return role;
        }
        public async Task<List<BookingDetails>> GetUserBookings(int userid)
        {
            List<BookingDetails> bookings = new List<BookingDetails>();
            using SqlConnection conn = new SqlConnection(_config.GetConnectionString("Default"));
            SqlCommand cmd = new SqlCommand("SELECT UserId,FlightId,book_time FROM BookedFlights WHERE [UserId] = @userid", conn);
            cmd.Parameters.AddWithValue("@userid", userid);
            await conn.OpenAsync();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                BookingDetails booking = new BookingDetails(
                    reader.GetInt32(0), //userid
                    reader.GetInt32(1), //flightid
                    reader.GetDateTime(2) //Book_time
                );
                bookings.Add(booking);
            }

            reader.Close();
            return bookings;
        }
    }
}
