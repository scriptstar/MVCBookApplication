using System;
using System.Collections.Generic;

namespace MvcBookApplication.Validation
{
    public class ValidationException : Exception
    {
        public ValidationException(List<ValidationError> errors)
        {
            ValidationErrors = errors;
        }

        public List<ValidationError> ValidationErrors { get; private set; }
    }
}