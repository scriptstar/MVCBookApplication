namespace MvcBookApplication.Validation
{
    public class ValidationError
    {
        public ValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }

    }
}