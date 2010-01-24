using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Data;
using MvcBookApplication.Services;
using MvcBookApplication.Tests;

namespace MvcBookApplication.Tests.Services
{
    [TestFixture]
    public class PayPalServiceTests
    {
        [Test]
        public void handshake_should_create_request_to_paypal()
        {
            var url = "https://www.paypal.com/cgi-bin/webscr";

            //mock the paypal response
            var mockPaypalResponse = new Mock<WebResponse>();
            mockPaypalResponse.Expect(r => r.GetResponseStream())
                .Returns(new MemoryStream());

            //mock the request to paypal
            var mockPaypalRequest = new Mock<WebRequest>();
            mockPaypalRequest.Expect(r => r.GetRequestStream())
                .Returns(new MemoryStream());
            mockPaypalRequest.Expect(r => r.GetResponse())
                .Returns(mockPaypalResponse.Object);

            //mock the service helper
            var mockHelper = new Mock<IPayPalServiceHelper>();
            mockHelper.Expect(h => h.CreateRequest(url))
                .Returns(mockPaypalRequest.Object);

            var requestcontent = "five";
            MyMocks.Request
                .Expect(r => r.ContentLength).Returns(requestcontent.Length);
            MyMocks.Request
                .Expect(r => r.BinaryRead(requestcontent.Length))
                .Returns((new ASCIIEncoding()).GetBytes(requestcontent));
            var service = new PayPalService(null, mockHelper.Object);
            service.PerformHandShake(MyMocks.Request.Object);

            //verify mocks
            long length = (requestcontent + "&cmd=_notify-validate").Length;
            mockPaypalRequest.VerifySet(r => r.Method, "POST");
            mockPaypalRequest.VerifySet(r => r.ContentType,
                                        "application/x-www-form-urlencoded");
            mockPaypalRequest.VerifySet(r => r.ContentLength, length);
            mockPaypalRequest.VerifyAll();
            mockHelper.VerifyAll();
        }

        [Test]
        public void proccesspayment_should_call_repository_save_when_subscribing()
        {
            var email = "payer@test.com";
            var plan = "Personal Plan";
            var mockRepo = new Mock<ISubscriptionPlanRepository>();
            mockRepo.Expect(s => s.Save(email, plan))
                .Returns(1);

            var service = new PayPalService(mockRepo.Object, null);
            var formCollection = new NameValueCollection
                             {
                                 {"txn_type", "subscr_signup"},
                                 {"payer_email", email},
                                 {"item_name", plan}
                             };

            service.ProcessPayment(formCollection);
            mockRepo.VerifyAll();
        }

[Test]
public void processpayment_should_call_repositor_save_when_unsubscribing_with_free_plan()
{
   var email = "payer@test.com";
    var plan = "Free Plan";
    var mockRepo = new Mock<ISubscriptionPlanRepository>();
    mockRepo.Expect(s => s.Save(email, plan))
        .Returns(1);

    var service = new PayPalService(mockRepo.Object, null);
    var formCollection = new NameValueCollection
                     {
                         {"txn_type", "subscr_cancel"},
                         {"payer_email", email}
                     };

    service.ProcessPayment(formCollection);
    mockRepo.VerifyAll();
}
            
    }
}