<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (ViewContext.TempData.ContainsKey("flash"))
    {
%>
<div class="flash">
    <%= Html.Encode(ViewContext.TempData["flash"])%>
</div>
<%
    }
%>