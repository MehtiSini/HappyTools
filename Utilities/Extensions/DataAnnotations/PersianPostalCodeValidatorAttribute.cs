using HappyTools.Utilities.Extensions.PersianHelper;
using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    /// <summary>
    /// Validate persian postal code format
    /// </summary>
    public class PersianPostalCodeValidatorAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public PersianPostalCodeValidatorAttribute() 
        {
            ErrorMessage = null;
        }

        /// <summary>Checks that the value of the data field is valid.</summary>
        /// <param name="value">The data field value to validate.</param>
        /// <returns>true always.</returns>
        public override bool IsValid(object value)
        {
            var val = value?.ToString();
            if (val == null || val.Trim().Length == 0)
                return true;
            else
            {
                return val.IsValidIranianPostalCode();
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
                ErrorMessageResourceName = "InvalidPostalCodeError";
            }
        }

    }
}