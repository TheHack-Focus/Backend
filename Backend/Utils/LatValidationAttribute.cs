using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Utils
{
    public class LatValidationAttribute : ValidationAttribute
    {
        public LatValidationAttribute()
        {

        }

        public override bool IsValid(object value)
        {
            if (typeof(double) != value.GetType()) return false;

            double lat = (double) value;
            if (lat > 90 || lat < -90) return false;

            return true;
        }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            if (IsValid(value))
            {
                return null;
            }
            else
            {
                return new ValidationResult("Not a valid latitude value.");
            }
        }
    }
}
