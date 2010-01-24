using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace MvcBookApplication.Validation
{
    public class ValidationRunner : IValidationRunner
    {
        public List<ValidationError> ValidationErrors { get; private set; }

        public List<ValidationError> Run(object modelToValidate)
        {
            ValidationErrors = new List<ValidationError>();
            var props = TypeDescriptor.GetProperties(modelToValidate.GetType());
            foreach (PropertyDescriptor prop in props)
            {
                var value = prop.GetValue(modelToValidate);

                foreach (var attrib in prop.Attributes
                    .OfType<ValidationAttribute>())
                {

                    if (!attrib.IsValid(value))
                    {
                        ValidationErrors.Add(
                            new ValidationError(prop.Name,
                                                attrib.ErrorMessage));
                    }
                }

            }
            return ValidationErrors;
        }
    }
}