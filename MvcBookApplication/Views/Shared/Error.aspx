<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<h2>
    Sorry, an error occurred while processing your request.
</h2>
<%if (TempData.ContainsKey("error"))
  {%>
<div class="error" style='font-size:large'>
    <%= TempData["error"] %></div>
<%} %>
</asp:Content>
