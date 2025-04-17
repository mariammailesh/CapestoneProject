using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CapestoneProject.DTOs;
using CapestoneProject.Helpers;
using System.Data;
using CapestoneProject.Models;
using CapestoneProject.Helpers.Validations;

namespace CapestoneProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseHelper readOnlyDbHelper;

        public AuthController(DatabaseHelper dbHelper)
        {
            readOnlyDbHelper = dbHelper;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO signupDto)
        {
            try
            {
                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@Full_Name", signupDto.Full_Name),
                    new Microsoft.Data.SqlClient.SqlParameter("@UserName", signupDto.UserName),
                    new Microsoft.Data.SqlClient.SqlParameter("@Email", signupDto.Email),
                    new Microsoft.Data.SqlClient.SqlParameter("@Phone_Number", signupDto.Phone_Number),
                    new Microsoft.Data.SqlClient.SqlParameter("@PasswordHash", signupDto.PasswordHash),
                    new Microsoft.Data.SqlClient.SqlParameter("@Role_Id", 12), // Default role for regular users
                    new Microsoft.Data.SqlClient.SqlParameter("@IsActive", true)
                };

                var result = await readOnlyDbHelper.ExecuteScalarAsync<int>("sp_RegisterUser", parameters);

                if (result > 0)
                {
                    return Ok(new
                    {
                        message = "User registered successfully",
                        userId = result
                    });
                }
                else
                {
                    return BadRequest(new { message = "Registration failed. Email or username might already exist." });
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return StatusCode(500, new { message = "Database error occurred during registration", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@Email", loginDto.Email),
                    new Microsoft.Data.SqlClient.SqlParameter("@PasswordHash", loginDto.Password) // Note: In production, you should hash the password before sending
                };

                var result = await readOnlyDbHelper.ExecuteReaderAsync("sp_LoginUser", parameters);

                if (result.Rows.Count > 0)
                {
                    var user = new User
                    {
                        UserId = Convert.ToInt32(result.Rows[0]["UserId"]),
                        Full_Name = result.Rows[0]["Full_Name"].ToString(),
                        UserName = result.Rows[0]["UserName"].ToString(),
                        Email = result.Rows[0]["Email"].ToString(),
                        Role_Id = Convert.ToInt32(result.Rows[0]["Role_Id"])
                    };

                    // You might want to generate a JWT token here
                    return Ok(new
                    {
                        message = "Login successful",
                        user = user
                    });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return StatusCode(500, new { message = "Database error occurred during login", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetDto)
        {
            try
            {
                var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@Email", resetDto.Email),
                    new Microsoft.Data.SqlClient.SqlParameter("@NewPasswordHash", resetDto.NewPasswordHash)
                };

                try
                {
                    await readOnlyDbHelper.ExecuteNonQueryAsync("sp_ResetPassword", parameters);
                    return Ok(new { message = "Password reset successful" });
                }
                catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Message.Contains("Email not found"))
                {
                    return NotFound(new { message = "Email not found" });
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return StatusCode(500, new { message = "Database error occurred during password reset", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during password reset", error = ex.Message });
            }
        }
    }
}