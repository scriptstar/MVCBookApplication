<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Contact/ContactMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create New Contact</h2>
    <%=Html.ValidationSummary()%>
    <%=Html.JQueryGenerator("createcontact", ViewData.Model)%>
    <form id="createcontact" action="/contact/create" method="post">
    <div>
        <label for="email">
            Email</label>
        <br />
        <%=Html.TextBox("email")%>
        <br />
        <label for="name">
            Name</label>
        <br />
        <%=Html.TextBox("name")%>
        <br />
        <label for="sex">
            Sex</label>
        <br />
        <%=Html.DropDownList("sex" )%>
        <br />
        <label for="dob">
            Date of Birth</label>
        <br />
        <%=Html.TextBox("dob")%>
        <br />
        <input type="submit" value="Create Contact" />
    </div>
    </form>
</asp:Content>
