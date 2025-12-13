using System.ComponentModel.DataAnnotations;
using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    public class PufNullableGuidAttribute : ValidationAttribute
    {
        private readonly bool _allowEmpty;

        /// <summary>Initializes a new instance of the <see cref="T:PufGuidAttribute" /> class</summary>
        /// <param name="allowEmpty">whether empty guid is allowed or not. </param>
        public PufNullableGuidAttribute(bool allowEmpty) 
        {
            _allowEmpty = allowEmpty;
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
                Guid res = Guid.Empty;
                var guidParseRes = Guid.TryParse(val.ToString(), out res);
                if (!guidParseRes || !_allowEmpty && res == default)
                    return false;
                else
                {
                    return true;
                }
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
                ErrorMessageResourceName = "InvalidGuidError";
            }
        }

    }
}