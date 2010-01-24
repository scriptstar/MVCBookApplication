using System.Collections.Generic;
using MvcBookApplication.Data.Models;
using System.Linq;

namespace MvcBookApplication.Data
{
public class InMemorySubscriptionPlanRepository :ISubscriptionPlanRepository
{
    private List<SubscriptionPlan> SubscriptionPlans { get; set; }

    public InMemorySubscriptionPlanRepository()
    {
        SubscriptionPlans = new List<SubscriptionPlan>();
    }

    private int _autoId;
    private int AutoId
    {
        get
        {
            _autoId += 1;
            return _autoId;
        }
    }

    public int Save(string email, string plan)
    {
        var subscriptionPlan = SubscriptionPlans.SingleOrDefault(
            p => p.Email == email);
        if(subscriptionPlan == null)
        {
            subscriptionPlan = new SubscriptionPlan();
            subscriptionPlan.Id = AutoId;
            SubscriptionPlans.Add(subscriptionPlan);
        }
        subscriptionPlan.Email = email;
        subscriptionPlan.Plan = plan;
        return subscriptionPlan.Id;
    }
}
}