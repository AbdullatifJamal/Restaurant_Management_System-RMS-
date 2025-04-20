using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RMS_API.DTOs.AuthDTO.Request;
using RMS_API.DTOs.AuthDTO.Response;
using System.Data;

namespace login.Controllers
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
    }
}
