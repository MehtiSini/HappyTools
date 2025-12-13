using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HappyTools.Utilities.Extensions
{
    public static class ModelValidationExtensions
    {

        /// <summary>
        /// Determine weather specified object instance is valid using data annotation attribiutes
        /// </summary>
        /// <param name="objSource"></param>
        public static void Validate(this object objSource)
        {
            ICollection<ValidationResult> results = new List<ValidationResult>();
            var resValidation = objSource.TryValidate(out results);
            if (!resValidation)
            {
                throw new ModelValidationException(results.ToList());
            }
        }
        /// <summary>
        /// Determine weather specified object instance is valid using data annotation attribiutes
        /// </summary>
        /// <param name="results">the result of validation</param>
        /// <param name="objSource"></param>
        public static bool TryValidate(this object objSource, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(objSource, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                objSource, context, results,
                validateAllProperties: true
            );
        }
    }
    public class ModelValidationException : Exception
    {
        public ModelValidationException(List<ValidationResult> validationResults) : base("Error in validating model!")
        {
            ValidationResults = validationResults;

        }
        public ModelValidationException(List<ValidationResult> validationResults, string message) : base(message)
        {
            ValidationResults = validationResults;
        }
        public List<ValidationResult> ValidationResults { get; set; }

        /// <summary>Gets a message that describes the current exception.</summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string ("").</returns>
        public override string Message {
            get {
                var res = base.Message;
                var validationStr = GetValidationErrors();
                if (!string.IsNullOrEmpty(res))
                {
                    if (!string.IsNullOrEmpty(validationStr))
                        res = string.Format(res, validationStr);
                }
                else
                {
                    if (!string.IsNullOrEmpty(validationStr))
                        res = validationStr;
                }
                return res;
            }
        }

        private string GetValidationErrors()
        {
            var res = string.Empty;
            if (ValidationResults == null || ValidationResults.Count == 0)
                return res;
            for (var i = 0; i < ValidationResults.Count; i++)
            {
                var item = ValidationResults[i];
                res += item.ErrorMessage;
                if (i != ValidationResults.Count - 1)
                    res += ",";
            }
            return res;
        }
    }
}