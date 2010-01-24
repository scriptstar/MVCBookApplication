namespace MvcBookApplication.Data
{
    public interface ISubscriptionPlanRepository
    {
        int Save(string email, string plan);
    }
}