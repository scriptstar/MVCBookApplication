using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MvcBookApplication.Validation
{
    public class JQueryValidationGenerator : IValidationGenerator
    {
        public string Generate(string formToValidate, object modelToValidate)
        {
            var props = TypeDescriptor.GetProperties(modelToValidate.GetType());
            var rules = string.Empty;
            var script = string.Empty;
            var messages = string.Empty;
            foreach (PropertyDescriptor prop in props)
            {
                var rule = string.Empty;
                var message = string.Empty;
                foreach (var attrib in prop.Attributes
                    .OfType<ValidationAttribute>())
                {
                    var subrule = string.Empty;
                    var ruletype = string.Empty;
                    if (attrib.GetType() == typeof(RequiredAttribute))
                    {
                        subrule = "required:true";
                        ruletype = "required";
                    }
                    if (attrib.GetType() == typeof(EmailAttribute))
                    {
                        subrule = "email:true";
                        ruletype = "email";
                    }
                    if (attrib.GetType() == typeof(RangeAttribute))
                    {
                        subrule = string.Format("range:[{0},{1}]",
                                                ((RangeAttribute)attrib).Minimum,
                                                ((RangeAttribute)attrib).Maximum);
                        ruletype = "range";
                    }
                    if (attrib.GetType() == typeof(DateAttribute))
                    {
                        subrule = "date:true";
                        ruletype = "date";
                    }

                    rule += string.Format("{0}{1}",
                                          string.IsNullOrEmpty(rule) ? "" : ",",
                                          subrule);
                    message += string.Format("{0}{1}:'{2}'",
                                             string.IsNullOrEmpty(message) ? "" : ",",
                                             ruletype,
                                             attrib.ErrorMessage);
                }

                if (!string.IsNullOrEmpty(rule))
                {
                    rule = string.Format("{0}:{{{1}}}",
                                         prop.Name.ToLower(),
                                         rule);
                    rules += string.Format("{0}{1}",
                                           string.IsNullOrEmpty(rules) ? "" : ",",
                                           rule);

                    message = string.Format("{0}:{{{1}}}",
                                            prop.Name.ToLower(),
                                            message);
                    messages += string.Format("{0}{1}",
                                              string.IsNullOrEmpty(messages) ? "" : ",",
                                              message);
                }


            }

            if (!string.IsNullOrEmpty(rules))
            {
                script = string.Format("<script type=\"text/javascript\">\r\n" +
                                       "$().ready(function() {{\r\n" +
                                       "$('#{0}')." +
                                       "validate({{rules:{{{1}}},\r\n" +
                                       "messages:{{{2}}}}});}});" +
                                       "\r\n</script>",
                                       formToValidate, rules, messages);
            }

            return script;
        }
    }
}