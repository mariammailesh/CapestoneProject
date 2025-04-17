using System.ComponentModel.DataAnnotations;

namespace CapestoneProject.DTOs
{
    public class SignupDTO
    {
        [Required]
        public string Full_Name { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone_Number { get; set; }

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPasswordHash { get; set; }
    }
}