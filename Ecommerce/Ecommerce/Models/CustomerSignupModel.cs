using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class CustomerSignupModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z. -]*$", ErrorMessage = "Name can only contain letters, spaces, dots, and dashes.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [UniqueEmail(ErrorMessage = "Email already in use. Please use a different email.")]
        [RegularExpression(@"^[a-zA-Z0-9]+@gmail.com$", ErrorMessage = "Email must follow the format '@gmail.com' where x is a number.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=(.*[A-Za-z]){2})(?=.*\d)(?=(.*[^A-Za-z0-9\s]){2})[A-Za-z0-9!@#$%^&*()_+{}[\]:;<>,.?~\\-]+$", ErrorMessage = "Password must have at least 2 alphabets, 1 number, and 2 special characters, and no spaces.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User Type is required.")]
        public string UserType { get; set; }
    }
}
