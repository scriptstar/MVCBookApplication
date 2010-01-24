using System.Collections.Generic;
using System.Web.Mvc;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;

namespace MvcBookApplication.Tests.Controllers
{
	[TestFixture]
	public class MessageControllerTest
	{
		private MessageController controller;
		private Mock<IMessageService> service;

		[SetUp]
		public void Setup()
		{
			service = new Mock<IMessageService>();
			service.Expect( s => s.GetById( It.IsAny<int>() ) ).Returns( new Message{ Id = 1 } );
			controller = new MessageController( service.Object );
		}

		[Test]
		public void getting_create_returns_viewresult()
		{
			//act
			var result = controller.Create();

			//assert
			Assert.IsAssignableFrom( typeof(ViewResult), result );
		}

		[Test]
		public void posting_to_create_returns_redirect_to_edit_action()
		{
			//arrange
			var name = "Mister Burns";
			var subject = "Some Subject";
			var text = "Some Text";
			var form = new FormCollection{ { "Name", name }, { "Subject", subject }, { "Text", text } };
			controller.ValueProvider = form.ToValueProvider();

			//act
			var result = controller.Create_Post();

			//assert
			Assert.IsAssignableFrom( typeof(RedirectToRouteResult), result );
			Assert.AreEqual( "Edit", ( (RedirectToRouteResult)result ).RouteValues["action"] );
		}

		[Test]
		public void posting_to_create_creates_and_saves_a_new_message_object()
		{
			//arrange
			var name = "Mister Burns";
			var subject = "Some Subject";
			var text = "Some Text";
			var message = new Message{ Name = name };
			var svc = new Mock<IMessageService>();
			svc.Expect( s => s.Add( It.IsAny<Message>() ) ).Returns( 1 );
			controller.MessageService = svc.Object;
			var form = new FormCollection{ { "Name", name }, { "Subject", subject }, { "Text", text } };
			controller.ValueProvider = form.ToValueProvider();

			//act
			var result = controller.Create_Post();

			//assert
			svc.VerifyAll();
		}

		[Test]
		public void getting_edit_returns_the_edit_viewresult()
		{
			//act
			var result = controller.Edit( 1 );

			//assert
			Assert.IsAssignableFrom( typeof(ViewResult), result );
		}

		[Test]
		public void getting_edit_calls_msgservice_to_get_message_and_pass_to_viewdata()
		{
			//arrange
			var id = 1;
			var name = "Mr. Burn's Message";
			var message = new Message{ Id = id, Name = name };
			var svc = new Mock<IMessageService>();
			svc
				.Expect( s => s.GetById( It.Is<int>( i => i == id ) ) )
				.Returns( message );
			controller.MessageService = svc.Object;

			//act
			var result = controller.Edit( id );

			//assert
			Assert.AreSame( message, result.ViewData.Model );
		}

		[Test]
		public void posting_invalid_data_to_edit_returns_view_and_adds_modelerror()
		{
			//arrange
			var id = 1;
			var name = string.Empty;
			var subject = "Welcome to Springfield!";
			var text = "Lorem ipsum dolor sit amet.";
			var html = "<p>Lorem <b>ipsum</b> dolor sit amet.</p>";
			var form = new FormCollection{
			                             	{ "Name", name },
			                             	{ "Subject", subject },
			                             	{ "Text", text },
			                             	{ "Html", html }
			                             };
			controller.ValueProvider = form.ToValueProvider();
			var ex = new ValidationException(
				new List<ValidationError>{
				                         	new ValidationError( "Name", "Name is a required field." )
				                         }
				);
			service.Expect( x => x.Update( It.IsAny<Message>() ) ).Throws( ex );

			//act
			var result = controller.Edit_Post( id );

			//assert
			Assert.IsAssignableFrom( typeof(ViewResult), result );
			var viewResult = (ViewResult)result;
			Assert.AreEqual( "", viewResult.ViewName );
			Assert.IsFalse( viewResult.ViewData.ModelState.IsValid );
		}

		[Test]
		public void posting_to_edit_returns_redirect_to_selectcontacts_action()
		{
			//arrange
			var id = 1;
			var name = "Mister Burns";
			var subject = "Welcome to Springfield!";
			var text = "Lorem ipsum dolor sit amet.";
			var html = "<p>Lorem <b>ipsum</b> dolor sit amet.</p>";
			var form = new FormCollection{
			                             	{ "Name", name },
			                             	{ "Subject", subject },
			                             	{ "Text", text },
			                             	{ "Html", html }
			                             };
			controller.ValueProvider = form.ToValueProvider();

			//act
			var result = controller.Edit_Post( id );

			//assert
			Assert.IsAssignableFrom( typeof(RedirectToRouteResult), result );
			Assert.AreEqual( "SelectContacts", ( (RedirectToRouteResult)result ).RouteValues["action"] );
		}

		[Test]
		public void posting_to_edit_gets_message_and_updates_with_input()
		{
			//arrange
			var id = 1;
			var name = "Not Mister Burns";
			var subject = "Welcome to Springfield!";
			var text = "Lorem ipsum dolor sit amet.";
			var html = "<p>Lorem <b>ipsum</b> dolor sit amet.</p>";
			var form = new FormCollection{
			                             	{ "Name", name },
			                             	{ "Subject", subject },
			                             	{ "Text", text },
			                             	{ "Html", html }
			                             };
			controller.ValueProvider = form.ToValueProvider();
			var message = new Message{ Id = id, Name = "Mister Burns" };
			var svc = new Mock<IMessageService>();
			svc
				.Expect( s => s.GetById( It.Is<int>( i => i == id ) ) )
				.Returns( message );
			svc
				.Expect( s => s.Update( It.Is<Message>( m =>
				                                        m.Id == id &&
				                                        m.Name == name &&
				                                        m.Subject == subject &&
				                                        m.Text == text
				                        	) ) ).Returns( true );
			controller.MessageService = svc.Object;

			//act
			controller.Edit_Post( id );

			//assert
			svc.VerifyAll();
		}
	}
}