using System.Collections.Generic;

namespace MvcBookApplication.Validation
{
    public interface IValidationRunner
    {
        List<ValidationError> Run(object modelToValidate);
    }
}