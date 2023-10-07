using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lab2.Models
{
    public class IndexModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z. -]*$", ErrorMessage = "Name can only contain letters, spaces, dots, and dashes.")]
        public string name { get; set; }

/*----------------------------------------------------------------------------------------------------------------*/

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(7, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 7 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "Username can only contain letters, numbers, dashes, and underscores.")]
        public string username { get; set; }

        /*----------------------------------------------------------------------------------------------------------------*/

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^\d{2}-\d{5}-\d@student.aiub.edu$", ErrorMessage = "Email must follow the format 'xx-xxxxx-x@student.aiub.edu' where x is a number.")]
        public string email { get; set; }

        /*----------------------------------------------------------------------------------------------------------------*/

        [Required(ErrorMessage = "Student ID is required.")]
        [RegularExpression(@"^\d{2}-\d{5}-\d$", ErrorMessage = "Student ID must follow the format 'xx-xxxxx-x' where x is a number.")]
        public string studentid { get; set; }

        /*----------------------------------------------------------------------------------------------------------------*/

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=(.*[A-Za-z]){2})(?=.*\d)(?=(.*[^A-Za-z0-9\s]){2})[A-Za-z0-9!@#$%^&*()_+{}[\]:;<>,.?~\\-]+$", ErrorMessage = "Password must have at least 2 alphabets, 1 number, and 2 special characters, and no spaces.")]
        public string password { get; set; }

        /*----------------------------------------------------------------------------------------------------------------*/

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DateOfBirth(18, ErrorMessage = "You must be at least 18 years old.")]
        public DateTime? dob { get; set; }

    }
}