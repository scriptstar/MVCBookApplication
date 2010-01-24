using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data;
using MvcBookApplication.Data.InMemory;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;
using Ninject.Core.Behavior;
 
namespace MvcBookApplication
{

    public class InMemoryModule : StandardModule
    {
        public override void Load()
        {
            //authentication & memebership
            Bind<IFormsAuthentication>().To<FormsAuthenticationWrapper>();
            Bind<MembershipProvider>().ToConstant(Membership.Provider);

            //services
            Bind<IMessageService>().To<InMemoryMessageService>();
            Bind<IContactService>().To<InMemoryContactService>();
            Bind<IContactListService>().To<InMemoryContactListService>();
            Bind<IGalleryService>().To<InMemoryGalleryService>();
            Bind<ITemplateService>().To<InMemoryTemplateService>();

            Bind<IPaymentService>().To<PayPalService>();
            Bind<IPayPalServiceHelper>().To<PayPalServiceHelper>();
            Bind<ISubscriptionPlanRepository>().To<InMemorySubscriptionPlanRepository>()
                .Using<SingletonBehavior>();

            //repositories
            Bind<IMessageRepository>().To<InMemoryMessageRepository>()
                .Using<SingletonBehavior>();
            Bind<IContactRepository>().To<InMemoryContactRepository>()
                .Using<SingletonBehavior>();
            Bind<IContactListRepository>().To<InMemoryContactListRepository>()
                .Using<SingletonBehavior>();
            Bind<IGalleryRepository>().To<InMemoryGalleryRepository>()
                .Using<SingletonBehavior>();
Bind<ITemplateRepository>().To<InMemoryTemplateRepository>()
    .Using<SingletonBehavior>();
            //misc
            Bind<IValidationRunner>().To<ValidationRunner>();
            Bind<IParserFactory>().To<ParserFactory>();

            PopulateRepositoriesWithTestData();
        }

        private void PopulateRepositoriesWithTestData()
        {
            var provider = (MembershipProvider)Kernel.Get(typeof(MembershipProvider));
            var contactrepo = (IContactRepository)Kernel.Get(typeof(IContactRepository));
            var contactlistrepo = (IContactListRepository)Kernel.Get(typeof(IContactListRepository));
            var messagerepo = (IMessageRepository)Kernel.Get(typeof(IMessageRepository));

            for (var i = 0; i < 50; i++)
            {
                contactrepo.Add(
                    new Contact
                        {
                            Email = ("user" + i + "@test.com"),
                            Name = string.Format("First{0} Last{1}", i, i),
                            Dob = (new DateTime(1967, 10, 28)).AddDays(i),
                            Sex = (i % 3 == 0
                                       ?
                                           Sex.Undefined
                                       :
                                           (i % 2 == 0
                                                ?
                                                    Sex.Female
                                                :
                                                    Sex.Male)),
                            User = i % 2 == 0
                                       ? new User
                                             {
                                                 UserId = (Guid)provider
                                                             .GetUser("test", false)
                                                             .ProviderUserKey,
                                                 Username = provider
                                                             .GetUser("test", false)
                                                             .UserName
                                             }
                                       :
                                           new User
                                               {
                                                   UserId = (Guid)provider
                                                               .GetUser("test2", false)
                                                               .ProviderUserKey,
                                                   Username = provider
                                                               .GetUser("test2", false)
                                                               .UserName
                                               }
                        });

                contactlistrepo.Add(new ContactList
                                        {
                                            Name = "list " + i,
                                            Description = "this is the description for list " + i,
                                            User = i % 2 == 0
                                                       ? new User
                                                             {
                                                                 UserId = (Guid)provider
                                                                                     .GetUser("test", false)
                                                                                     .ProviderUserKey,
                                                                 Username = provider
                                                                     .GetUser("test", false)
                                                                     .UserName
                                                             }
                                                       :
                                                           new User
                                                               {
                                                                   UserId = (Guid)provider
                                                                                       .GetUser("test2", false)
                                                                                       .ProviderUserKey,
                                                                   Username = provider
                                                                       .GetUser("test2", false)
                                                                       .UserName
                                                               }
                                        });

messagerepo.Add(new Message
       {
           Html = "random <b>html</b> " + i,
           Name = "Message " + i,
           Subject = "Subject " + i,
           Text = "random text " + i,
           User = i % 2 == 0
                            ? new User
                            {
                                UserId = (Guid)provider
                                                    .GetUser("test", false)
                                                    .ProviderUserKey,
                                Username = provider
                                    .GetUser("test", false)
                                    .UserName
                            }
                            :
                                new User
                                {
                                    UserId = (Guid)provider
                                                        .GetUser("test2", false)
                                                        .ProviderUserKey,
                                    Username = provider
                                        .GetUser("test2", false)
                                        .UserName
                                }
       });

            }
        }
    }
}
