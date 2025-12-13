using System.ComponentModel.DataAnnotations;
using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    public class PufCannotBeDefaultAttribute : ValidationAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.PufRequiredAttribute" /> class by using a specified maximum length.</summary>
        public PufCannotBeDefaultAttribute() 
        {
        }

        /// <summary>Determines whether the specified value of the object is valid.</summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>true if the specified value is valid; otherwise, false.</returns>
        public override bool IsValid(object value)
        {
            var type = value.GetType();
            var defaultValue = type.GetDefaultValue();
            if (value.Equals(defaultValue))
                return false;
            return true;
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
                ErrorMessageResourceName = "FieldRequiredError";
            }
        }

    }
}