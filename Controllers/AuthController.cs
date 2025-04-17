using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CapestoneProject.DTOs.SignUp.Request;
using CapestoneProject.Helpers.Validations;
using Microsoft.Data.SqlClient;
using CapestoneProject.DTOs.Login.Request;
using CapestoneProject.DTOs.Login.Response;
using CapestoneProject.DTOs.ResetPassword.Request;


namespace CapestoneProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("signup")] //static route
        public async Task<IActionResult> Signup([FromBody] SignUpInputDTO input)
        {
            string message = "";
            try
            {
                //validate input 
                if (ValidationHelper.IsValidPassword(input.Password) && ValidationHelper.IsValidEmail(input.Email) &&
                    ValidationHelper.IsValidName(input.FullName) && ValidationHelper.IsValidBirthDate(input.BirthDate) &&
                    ValidationHelper.IsValidPhoneNumber(input.PhoneNumber))
                {
                    string connectionString = "Data Source=DESKTOP-7QLOIQ2\\SQLEXPRESS01;Initial Catalog=\"E-Food Management System\";Integrated Security=True;Trust Server Certificate=True";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("sp_RegisterUser", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Full_Name", input.FullName);
                    command.Parameters.AddWithValue("@UserName", input.UserName);
                    command.Parameters.AddWithValue("@Birth_Date", input.BirthDate);
                    command.Parameters.AddWithValue("@Email", input.Email);
                    command.Parameters.AddWithValue("@Phone_Number", input.PhoneNumber);
                    command.Parameters.AddWithValue("@PasswordHash", input.Password);

                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    connection.Close();

                    if (result > 0)
                        return StatusCode(201, "Account Created!");
                    else
                        return StatusCode(400, "Failed to Create Account");

                }
                return StatusCode(400, "Failed to Create Account");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error Was Occured {ex.Message}");
            }
        }
        [HttpPost]
        [Route("login")] //static route
        public async Task<IActionResult> Login([FromBody] LoginInputDTO input)
        {
            var User = new LoginOutputDTO();
            try
            {
                if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
                    throw new Exception("Email and Password are required ");
                string connectionString = "Data Source=DESKTOP-7QLOIQ2\\SQLEXPRESS01;Initial Catalog=\"E-Food Management System\";Integrated Security=True;Trust Server Certificate=True";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("sp_LoginUser", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", input.Email);
                command.Parameters.AddWithValue("@PasswordHash", input.Password);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User.UserId = Convert.ToInt32(reader["UserId"]);
                        User.Role_Id = Convert.ToInt32(reader["Role_Id"]);
                        User.FullName = reader["Full_Name"].ToString();
                        User.Email = reader["Email"].ToString();
                        User.PhoneNumber = reader["Phone_Number"].ToString();
                        User.UserName = reader["UserName"].ToString();
                    }
                    else
                    {
                        throw new Exception("Failed To Login!"); // Invalid login
                    }
                }
                connection.Close();
                return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error Was Occured {ex.Message}");
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.Email))
                    throw new Exception("Email and Password are required ");
                string connectionString = "Data Source=DESKTOP-7QLOIQ2\\SQLEXPRESS01;Initial Catalog=\"E-Food Management System\";Integrated Security=True;Trust Server Certificate=True";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("sp_ResetPassword", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                if (ValidationHelper.IsValidPassword(input.NewPasswordHash))
                {
                    command.Parameters.AddWithValue("@NewPasswordHash", input.NewPasswordHash);
                }
                connection.Open();
                var result = command.ExecuteNonQuery();
                connection.Close();

                if (result > 0) // 1 row affected
                    return StatusCode(201, "Password reset successfully!");
                else
                    return StatusCode(400, "Failed to reset password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error Was Occured {ex.Message}");
            }
        }
    }
}
