using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecommerce.EF;

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var db = new ProductEntities3(); // You might want to consider a more efficient way to access your database context.

        if (value != null)
        {
            var email = value.ToString();
            var existingUser = db.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                return new ValidationResult("Email already in use. Please use a different email.");
            }
        }

        return ValidationResult.Success;
    }
}
