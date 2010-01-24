using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using MvcBookApplication.Data;
using Ninject.Core;
using System.Linq;

namespace MvcBookApplication.Services
{
    public interface IPayPalServiceHelper
    {
        WebRequest CreateRequest(string url);
    }

    public class PayPalServiceHelper : IPayPalServiceHelper
    {
        public WebRequest CreateRequest(string url)
        {
            return WebRequest.Create(url);
        }
    }

    public class PayPalService : IPaymentService
    {
public ISubscriptionPlanRepository SubscriptionPlanRepository { get; set; }
public IPayPalServiceHelper PayPalServiceHelper { get; set; }

[Inject]
public PayPalService(ISubscriptionPlanRepository subscriptionPlanRepository, IPayPalServiceHelper payPalServiceHelper)
{
    SubscriptionPlanRepository = subscriptionPlanRepository;
    PayPalServiceHelper = payPalServiceHelper;
}

public void ProcessPayment(NameValueCollection formCollection)
{
    //subscribe site
    var payerEmail = "";
    if (formCollection.AllKeys.Contains("payer_email"))
    {
        payerEmail = formCollection["payer_email"];
    }
    var txn_type = "";
    if (formCollection.AllKeys.Contains("txn_type"))
    {
        txn_type = formCollection["txn_type"];
    }
    var itemName = "";
    if (formCollection.AllKeys.Contains("item_name"))
    {
        itemName = formCollection["item_name"];
    }

    if (txn_type == "subscr_cancel")
    {
        SubscriptionPlanRepository.Save(payerEmail, "Free Plan");
       
    }
    else if (txn_type == "subscr_signup")
    {
        SubscriptionPlanRepository.Save(payerEmail, itemName);
    }
}

        public void PerformHandShake(HttpRequestBase Request)
        {
            //Read the PayPals' Instant Pay Notification (IPN) POST 
            var strFormValues = Encoding.ASCII
                .GetString(Request.BinaryRead(Request.ContentLength));

            // Create the request back
            var req = PayPalServiceHelper
                .CreateRequest("https://www.paypal.com/cgi-bin/webscr");

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var strNewValue = strFormValues + "&cmd=_notify-validate";
            req.ContentLength = strNewValue.Length;

            //// Write the request back IPN strings
            var stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();

            //send the request, read the response
            var strResponse = req.GetResponse();
            var IPNResponseStream = strResponse.GetResponseStream();
            var encode = System.Text.Encoding.GetEncoding("utf-8");
            var readStream = new StreamReader(IPNResponseStream, encode);


            var read = new char[256];
            var count = readStream.Read(read, 0, 256);
            string IPNResponse = new string(read, 0, count);
            if (IPNResponse == "VERIFIED")
            {
                //IPN is valid
            }
            else
            {
                //IPN is INVALID
            }

            //tidy up, close streams
            if (readStream != null) readStream.Close();
            if (strResponse != null) strResponse.Close();
        }
    }
}