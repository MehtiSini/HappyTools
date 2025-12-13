using System.ComponentModel.DataAnnotations;
using Nozhan.Utility.Core.Resources;

namespace HappyTools.Utilities.Extensions.DataAnnotations
{
    public class PufStringLengthAttribute : StringLengthAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.StringLengthAttribute" /> class by using a specified maximum length.</summary>
        /// <param name="maximumLength">The maximum length of a string. </param>
        public PufStringLengthAttribute(int maximumLength):base(maximumLength) 
        {
            
        }
        public override string FormatErrorMessage(string name)
        {
            EnsureErrorMessageResource();
            return base.FormatErrorMessage(name);
        }

        public void EnsureErrorMessageResource()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                if (ErrorMessageResourceType == null)
                {
                    ErrorMessageResourceType = typeof(FrameworkValidationMessages);
                }

                if (string.IsNullOrEmpty(ErrorMessageResourceName))
                {
                    ErrorMessageResourceName =
                        MinimumLength != 0 ? "StringLengthError" : "StringLengthErrorIncludingMinimum";
                }
            }

        }

    }
    public class PufDataTypeAttribute : DataTypeAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.PufDataTypeAttribute" /> class by using a specified maximum length.</summary>
        public PufDataTypeAttribute(DataType dataType) : base(dataType)
        {
            ErrorMessage = null;
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
                var errorMessageResourceName = "InvalidFormatError";
                //switch (this.DataType)
                //{
                //    case DataType.DateTime:
                //        errorMessageResourceName = "InvalidDateTimeError";
                //        break;
                //    case DataType.Date:
                //        errorMessageResourceName = "InvalidDateError";
                //        break;
                //}
                ErrorMessageResourceName = errorMessageResourceName;
                
            }
        }

    }
}
