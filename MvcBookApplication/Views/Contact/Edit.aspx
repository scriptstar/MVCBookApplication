<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Contact/ContactMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Import Namespace="MvcBookApplication.Services"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(document).ready(function() {
        $("#sex option[value=<%= (int) ViewData.Model.Sex %>]")
        .attr("selected", "selected");
    });
</script>

<h2>
    Edit Contact</h2>
<%=Html.ValidationSummary()%>
<%=Html.JQueryGenerator("editcontact", ViewData.Model)%>
<form id="editcontact" action="/contact/edit/<%=ViewData.Model.Id %>" method="post">
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
    <input id="dob" name="dob" type="text" value='<%=ViewData.Model.Dob.ToShortDateString()  %>' />
    <br />
    <input type="submit" value="Save Contact" />
</div>
</form>
</asp:Content>
