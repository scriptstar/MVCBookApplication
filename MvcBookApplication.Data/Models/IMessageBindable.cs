namespace MvcBookApplication.Data.Models
{
	public interface IMessageBindable
	{
		string Name { get; set; }
		string Subject { get; set; }
		string Text { get; set; }
		string Html { get; set; }
	}
}