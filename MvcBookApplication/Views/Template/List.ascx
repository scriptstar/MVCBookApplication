<%@ Import Namespace="MvcBookApplication.Data.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Template>>" %>
<% foreach (var template in Model)
   {%>
<a href='<%=template.Path %>' class='templatelink'>
    <img src='<%=template.Thumbnail %>' /></a>
<%} %>

