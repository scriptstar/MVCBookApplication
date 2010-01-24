using System.Web.Mvc;
using MvcBookApplication.Validation;

namespace MvcBookApplication
{
    public static class HtmlHelperExtensions
    {
        public static string JQueryGenerator(this HtmlHelper htmlHelper,
                                             string formName, object model)
        {
            return (new JQueryValidationGenerator()).Generate(formName, model);
        }
    }
}