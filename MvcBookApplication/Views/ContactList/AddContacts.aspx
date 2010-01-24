<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ContactList/ContactListMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MvcBookApplication.Data.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%=ViewData.Model.Name %>
        >> Add Contacts</h2>
    There are 3 ways to add contacts to your list
    <h3>
        1. Manually</h3>
    Enter the contact information to add:<br />
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
        <select id="sex" name="sex">
            <option value="0"></option>
            <option value="1">Male</option>
            <option value="2">Female</option>
        </select>
        <br />
        <label for="dob">
            Date of Birth</label>
        <br />
        <%=Html.TextBox("dob")%>
        <br />
        <input type="submit" value="Create Contact" />
    </div>
    </form>
    <h3>
        2. Copy Another List</h3>
    Select the contact list(s) you want to copy:<br />
    <%
        foreach (var contactList in ((List<ContactList>)ViewData["contactlists"]))
        {%>
    <input type="checkbox" />
    <%=contactList.Name %><br />
    <%
        }%>
    <h3>
        3. Import From File</h3>
    Upload a text file to import contacts into the list
</asp:Content>
