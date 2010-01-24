<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Contact/ContactMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MvcBookApplication.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Delete Contact</h2>
    <br />
    <strong style='color:Red'>Are you sure you want to delete this contact?</strong>
    <br />
    <br />
    <div>
        Email:
        <%=ViewData.Model.Email %><br />
        Name:
        <%= ViewData.Model.Name  %><br />
    </div>
    <br />
    <div>
        <form id="deletecontact" action="/contact/delete/<%=ViewData.Model.Id %>" method="post">
        <input type="submit" value="Yes, delete the contact" />
        <%=Html.ActionLink<ContactController>(c=>c.Browse(null,null,null),"No, cancel and go back") %>
        </form>
    </div>
</asp:Content>
