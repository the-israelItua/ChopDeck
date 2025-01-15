using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Validation
{
    public class EnumValueValidationAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValueValidationAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Provided type must be an enum.");
            }
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !_enumType.IsEnumDefined(Enum.Parse(_enumType, value.ToString())))
            {
                return new ValidationResult($"Invalid value. Allowed values are: {string.Join(", ", Enum.GetNames(_enumType))}");
            }

            return ValidationResult.Success;
        }
    }

}