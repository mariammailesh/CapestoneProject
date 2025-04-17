using System.Text.RegularExpressions;

namespace CapestoneProject.Helpers.Validations
{
    public static class ValidationHelper
    {
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) && password.Length >= 6)
                throw new Exception("Password Is Required");
            return true;
        }
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new Exception("Name Is Required And Should not be more than 50 character");
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    throw new Exception("Name Is Required To Contais of character and white spaces ");
            }
            return true;
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email Is  Required");

            int atIndex = email.IndexOf('@');
            int dotIndex = email.LastIndexOf('.');

            if (atIndex < 1 || dotIndex < atIndex + 2 || dotIndex >= email.Length - 2)
                throw new Exception("Email Is  Required");

            string domain = email.Substring(atIndex + 1, dotIndex - atIndex - 1);
            string extension = email.Substring(dotIndex + 1);

            if (domain.Length < 2 || extension.Length < 2)
                throw new Exception("Email Is  Required");

            foreach (char c in email.Substring(0, atIndex))
            {
                if (!char.IsLetterOrDigit(c) && c != '.' && c != '_' && c != '%' && c != '+' && c != '-')
                    throw new Exception("Email Is  Required");
            }
            return true;
        }
        public static bool IsValidBirthDate(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return false;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age)) age--; // adjust if birthday hasn’t occurred yet this year

            return (birthDate.Value <= today? true : throw new Exception("BirthDate should be less than today date"));
        }
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrEmpty(phone))
                throw new Exception("Phone Number Is Required");

            // Basic regex for international and local numbers
            var phoneRegex = new Regex(@"^\+?[0-9]{7,15}$");
            return (phoneRegex.IsMatch(phone)? true : throw new Exception("Phone number should contain basic regex for international and local numbers"));
        }

        //// Optional: check uniqueness in DB
        //public static bool IsPhoneNumberUnique(string phone)
        //{
        //    using (var context = new YourDbContext())
        //    {
        //        return !context.Users.Any(u => u.Phone_Number == phone);
        //    }
        //}


    }
}
