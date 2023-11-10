using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mid.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9]+@gmail.com$", ErrorMessage = "Email must follow the format '@gmail.com' where x is a number.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=(.*[A-Za-z]){2})(?=.*\d)(?=(.*[^A-Za-z0-9\s]){2})[A-Za-z0-9!@#$%^&*()_+{}[\]:;<>,.?~\\-]+$", ErrorMessage = "Password must have at least 2 alphabets, 1 number, and 2 special characters, and no spaces.")]
        public string Password { get; set; }
    }
}