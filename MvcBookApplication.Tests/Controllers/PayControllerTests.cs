using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class PayControllerTests
    {
        [Test]
        public void callback_should_return_null()
        {
            var con = new PayController();
            var result = con.Callback();
            Assert.IsNull(result);
        }

        [Test]
        public void callback_should_perform_hand_shake()
        {
            var mockService = new Mock<IPaymentService>();
            mockService
                .Expect(s => s.PerformHandShake(MyMocks.Request.Object));

            var con = new PayController(mockService.Object);
            con.SetFakeControllerContext();

            con.Callback();

            mockService.VerifyAll();
        }

        [Test]
        public void callback_should_process_payment()
        {
            var mockService = new Mock<IPaymentService>();
            mockService
                .Expect(s => s.ProcessPayment(MyMocks.Request.Object.Form));

            var con = new PayController(mockService.Object);
            con.SetFakeControllerContext();

            con.Callback();

            mockService.VerifyAll();
        }

        [Test]
        public void subscribe_should_return_view()
        {
            var controller = new PayController();
            var result = controller.Subscribe();
            result.AssertViewResult(controller, "Subscribe", "subscribe");
        }
    }
}