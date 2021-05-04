using System;
using System.ComponentModel.DataAnnotations;


namespace ToDoList.DateValidation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;
            if (dt >= DateTime.UtcNow)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? "Date isn't equal or greater than current date.");
        }
    }
}
