

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EntityFrameworkApi.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredParameterAttributes : ValidationAttribute
    {
        private int? _minLength, _maxLength, _length;
        public RequiredParameterAttributes()
        {

        }

        public RequiredParameterAttributes(int length)
        {
            _length = length;
        }

        public RequiredParameterAttributes(int minLength, int maxLength)
        {
            _maxLength = maxLength;
            _minLength = minLength;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo? property = validationContext.ObjectInstance.GetType().GetProperty(validationContext.DisplayName);
            if (property is null)
                return ValidationResult.Success;

            if (value is null)
                throw new NullReferenceException(validationContext.DisplayName + " Cannot be null.");

            if (property.PropertyType.Name.ToLower() == "string")
                StringValidation(value, validationContext, property);
            else if (property.PropertyType.Name.ToLower().Replace("ı", "i").Contains("int") || property.PropertyType.Name.ToLower().Replace("ı", "i").Contains("decimal"))
                IntegerValidation(value, validationContext);
            else if (property.PropertyType.Name.ToLower() == "guid")
                GuidValidation(value, validationContext);

            return ValidationResult.Success;
        }

        private void StringValidation(object value, ValidationContext validationContext, PropertyInfo property)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                throw new NullReferenceException(validationContext.DisplayName + " Cannot be null.");

            if (_minLength.HasValue && value.ToString().Length < _minLength.Value)
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");

            if (_maxLength.HasValue && value.ToString().Length > _maxLength.Value)
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");

            if (_length.HasValue && value.ToString().Length != _length.Value)
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");
        }

        private void IntegerValidation(object value, ValidationContext validationContext)
        {
            if (!int.TryParse(value.ToString(), out int intValue))
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");

            if (intValue == 0)
                throw new NullReferenceException(validationContext.DisplayName + " Cannot be null.");

            if (_length.HasValue)
                _minLength = _length;

            if (_minLength.HasValue && intValue < _minLength.Value)
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");

            if (_maxLength.HasValue && intValue > _maxLength.Value)
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");
        }

        private void GuidValidation(object value, ValidationContext validationContext)
        {
            if (!Guid.TryParse(value.ToString(), out Guid guidValue))
                throw new ArgumentOutOfRangeException(validationContext.DisplayName + " Cannot be null.");

            if (guidValue == Guid.Empty)
                throw new NullReferenceException(validationContext.DisplayName + " Cannot be null.");
        }
    }
}
