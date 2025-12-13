using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    /// <summary>
    /// Custom Validator
    /// </summary>
    public class CustomValidatorAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>Gets a value that indicates whether the attribute requires validation context.</summary>
        /// <returns>true if the attribute requires validation context; otherwise, false.</returns>
        public override bool RequiresValidationContext {
            get { return true; }
        }
        public Type CustomValidatorType { get; set; }
        public ICustomValidator CustomValidator { get; set; }
        public CustomValidatorAttribute(Type customValidatorType) 
        {
            CustomValidatorType = customValidatorType;
            ErrorMessage = null;
            CustomValidator = customValidatorType.CreateInstance<ICustomValidator>();
        }

        /// <summary>Checks that the value of the data field is valid.</summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>true always.</returns>
        public override bool IsValid(object value)
        {
            CustomValidator.Value = value;
            return CustomValidator.IsValid(value);
        }
        public override string FormatErrorMessage(string name)
        {

            var error = CustomValidator.FormatErrorMessage(name);
            if (!string.IsNullOrEmpty(error))
                return error;
            EnsureErrorMessageResource();
            return base.FormatErrorMessage(name);
        }

        public void EnsureErrorMessageResource()
        {
            if (ErrorMessageResourceType == null)
            {
                ErrorMessageResourceType = typeof(FrameworkValidationMessages);
            }
            if (string.IsNullOrEmpty(ErrorMessageResourceName))
            {
                ErrorMessageResourceName = "InvalidCodingError";
            }
        }

    }
    public interface ICustomValidator
    {
        object Value { get; set; }
        /// <summary>
        /// Determine whether value is valid or not
        /// </summary>
        /// <param name="value">the value to validate</param>
        /// <returns></returns>
        bool IsValid(object value);

        /// <summary>
        /// Generating error message
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string FormatErrorMessage(string name);

    }

    public abstract class CustomValidator : ICustomValidator
    {
        public object Value { get; set; }

        /// <inheritdoc />
        public abstract bool IsValid(object value);

        /// <summary>
        /// Generating error message
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string FormatErrorMessage(string name)
        {
            var errorMessage = string.Format(FrameworkValidationMessages.InvalidCodingError, Value, name);
            return errorMessage;
        }
    }
}