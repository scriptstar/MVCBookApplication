using System.Collections.Specialized;
using System.Web;

namespace MvcBookApplication.Services
{
    public interface IPaymentService
    {
        void ProcessPayment(NameValueCollection formCollection);
        void PerformHandShake(HttpRequestBase Request);
    }
}