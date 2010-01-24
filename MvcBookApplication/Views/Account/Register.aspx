<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage"%>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<h2>Account Creation</h2>
<p>
    Use the form below to create a new account.
</p>
<%= Html.ValidationSummary()%>
<form method="post" action="<%=Html.AttributeEncode(Url.Action("Register"))%>">
    <div>
        <label for="username">Username</label>
        <br />
        <%=Html.TextBox("username") %>
        <br />
        <label for="email">Email</label>
        <br />
        <%=Html.TextBox("email") %>
        <br />
        <label for="password">Password</label>
        <br />
        <%=Html.Password("password") %>
        <br />
        <label for="question">Secret Question</label>
        <br />
        <%=Html.TextBox("question") %>
        <br />
        <label for="answer">Secret Answer</label>
        <br />
        <%=Html.TextBox("answer") %>
        <br />
        <input type="submit" value="Register" />
    </div>
</form>
</asp:Content>
