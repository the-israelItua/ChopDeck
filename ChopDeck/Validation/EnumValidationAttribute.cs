using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ChopDeck.Validation
{
    public class EnumValueValidationAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValueValidationAttribute(Type enumType)
        {
            _enumType = enumType ?? throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enumeration");
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"Value is required.");
            }

            if (value is string stringValue)
            {
                bool isValid = Enum.GetNames(_enumType).Contains(stringValue);
                if (!isValid)
                {
                    return new ValidationResult($"Invalid status. Allowed values are: {string.Join(", ", Enum.GetNames(_enumType))}.");
                }
            }
            else
            {
                return new ValidationResult($"Invalid input type. Must be a string representing an enum value.");
            }

            return ValidationResult.Success;
        }
    }
}
