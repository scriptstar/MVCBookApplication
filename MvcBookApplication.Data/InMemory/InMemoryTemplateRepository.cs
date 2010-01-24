using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data.InMemory
{
    public class InMemoryTemplateRepository : ITemplateRepository
    {
        private List<Template> Templates { get; set; }

        public InMemoryTemplateRepository()
        {
            Templates = new List<Template>
                        {
                            new Template
                                {
                                    Path = "/content/templates/template1.htm",
                                    Thumbnail = "/content/templates/images/template1.jpg"
                                },
                            new Template
                                {
                                    Path = "/content/templates/template2.htm",
                                    Thumbnail = "/content/templates/images/template2.jpg"
                                },
                            new Template
                                {
                                    Path = "/content/templates/template3.htm",
                                    Thumbnail = "/content/templates/images/template3.jpg",
                                    Username = "test"
                                },
                            new Template
                                {
                                    Path = "/content/templates/template4.htm",
                                    Thumbnail = "/content/templates/images/template4.jpg",
                                    Username = "test"
                                }
                        };
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

        public IQueryable<Template> Get()
        {
            return Templates.AsQueryable();
        }

        public int Save(string username, string name, string content)
        {

            if (!Directory.Exists(HttpContext.Current.Server.MapPath(string.Format("~/content/Templates/{0}", username))))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(string.Format("~/content/Templates/{0}", username)));
            }

            using (TextWriter tw = new StreamWriter(HttpContext.Current.Server.MapPath(string.Format("~/content/Templates/{0}/{1}", username, name))))
            {
                tw.Write(HttpUtility.HtmlDecode(content));
                tw.Close();
            }

            var template = new Template
                               {
                                   Id = AutoId,
                                   Path = string.Format("/content/Templates/{0}/{1}", username, name),
                                   Thumbnail = "/content/templates/images/default.jpg",
                                   Username = username
                               };
            Templates.Add(template);
            return template.Id;
        }
    }
}