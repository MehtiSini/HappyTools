using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    /// <summary>
    /// Validate codings 
    /// </summary>
    public class CodeValidatorAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public List<string> ValidCodes { get; set; }
        public CodeValidatorAttribute(params string[] validCodes) 
        {
            if (validCodes != null)
                ValidCodes = validCodes.ToList(p => p);
            else
            {
                ValidCodes = new List<string>();
            }
            ErrorMessage = null;
        }

        /// <summary>Checks that the value of the data field is valid.</summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>true always.</returns>
        public override bool IsValid(object value)
        {
            var val = value?.ToString();
            if (val == null || val.Trim().Length == 0)
                return false;
            else
            {
                return ValidCodes.Contains(val);
            }
        }

        public override string FormatErrorMessage(string name)
        {
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
}