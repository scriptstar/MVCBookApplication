<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Contact/ContactMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h2>
    Import Contacts</h2>
<%=Html.ValidationSummary()%>
<form id="importcontacts" action="/contact/import" method="post" enctype="multipart/form-data">
<div>
    Enter email address below. One address per line.
    <label for="contacts">
        Email Addresses</label>
    <br />
    <textarea id="contacts" name="contacts" rows="10" cols="55"></textarea>
</div>
<div>
    <input type="file" id='file' name='file' /></div>
<div>
    <input type="submit" id="import" name="import" value="Import Email" /></div>
</form>
</asp:Content>
