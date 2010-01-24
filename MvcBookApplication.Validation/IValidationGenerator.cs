using System;
using System.Collections.Generic;
using System.Text;

namespace MvcBookApplication.Validation
{
    interface IValidationGenerator
    {
        string Generate(string formToValidate, object modelToValidate);
    }
}
