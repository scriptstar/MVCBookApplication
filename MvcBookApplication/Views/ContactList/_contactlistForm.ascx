<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<label for="name">
    Name</label>
<br />
<%=Html.TextBox("name")%>
<br />
<label for="description">
    Description</label>
<br />
<%=Html.TextArea("description")%>
<br />
