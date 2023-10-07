using System;
using System.ComponentModel.DataAnnotations;

public class DateOfBirthAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public DateOfBirthAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            if (age < _minimumAge)
            {
                return new ValidationResult($"You must be at least {_minimumAge} years old.");
            }
        }

        return ValidationResult.Success;
    }
}
