namespace MvcBookApplication.Data.Models
{
    public class ResetPasswordModel
    {
        public string Username { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}