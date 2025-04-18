using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyTasks.Helpers.Validations;
using RMS_API.DTOs.AuthDTO.Request;
using RMS_API.DTOs.AuthDTO.Response;
using System.Data;
using System.Data.SqlClient;


namespace RMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> SignIn(LoginDTO input)
        {
            var response = new LoginResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
                    throw new Exception("Email and Password are required");

                string connectionString = "Data Source=DESKTOP-D89OGE0\\SQLEXPRESS;Initial Catalog=\"RMS Data_Tire\";Integrated Security=True;Trust Server Certificate=True";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("User_Login", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Email", input.Email);
                command.Parameters.AddWithValue("@Password", input.Password);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                    throw new Exception("Invalid Email / Password");

                if (dataTable.Rows.Count > 1)
                    throw new Exception("Query Contains More Than One Element");

           
                foreach (DataRow row in dataTable.Rows)
                {
                    response.UserId = Convert.ToInt32(row["UserId"]);
                    response.Username = row["Username"].ToString();
                }

                return Ok(response);    
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error Was Occurred {ex.Message}");
            }
        }

        [HttpPost("Rest")]
        public async Task<IActionResult> Rest(LoginDTO Input)
        {
            string message = " ";
            try
            {

                if (ValidationHelper.IsValidEmail(Input.Email))
                {
                    string connectionString = "Data Source=DESKTOP-D89OGE0\\SQLEXPRESS;Initial Catalog=\"RMS Data_Tire\";Integrated Security=True;Trust Server Certificate=True";

                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Rest_Password", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Email", Input.Email);
                    command.Parameters.AddWithValue("@NewPassword1", Input.Password);
                    command.Parameters.AddWithValue("@NewPassword2", Input.Password);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read(); 
                        connection.Close();
                        return StatusCode(200, "Update Successful");
                    }
                    else
                    {
                        connection.Close();
                        return StatusCode(401, "Update failed");
                    }
                }

                return StatusCode(400, message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("signup")]
public async Task<IActionResult> SignUp(SignUpDTO input)
{
    try
    {
        if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password) ||
            string.IsNullOrWhiteSpace(input.FirstName) || string.IsNullOrWhiteSpace(input.Username))
        {
            throw new Exception("All required fields must be provided");
        }

        string connectionString = "Data Source=DESKTOP-D89OGE0\\SQLEXPRESS;Initial Catalog=\"RMS Data_Tire\";Integrated Security=True;Trust Server Certificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand("CREATE_NEW_USER", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@FirstName", input.FirstName);
                command.Parameters.AddWithValue("@LastName", input.LastName);
                command.Parameters.AddWithValue("@UserName", input.Username);
                command.Parameters.AddWithValue("@Password", input.Password);
                command.Parameters.AddWithValue("@UserRoleId", input.UserRoleId);
                command.Parameters.AddWithValue("@Email", input.Email);
                command.Parameters.AddWithValue("@PhoneNumber", input.PhoneNumber);
                command.Parameters.AddWithValue("@Image", (object?)input.Image ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", input.CreatedBy);
              

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        return Ok(new { message = "User created successfully" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}


    }
}
