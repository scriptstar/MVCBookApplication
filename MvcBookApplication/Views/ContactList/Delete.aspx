<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ContactList/ContactListMaster.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MvcBookApplication.Controllers"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
        Delete Contact List</h2>
    <br />
    <strong style='color:Red'>Are you sure you want to delete this contact list?</strong>
    <br />
    <br />
    <div>
        Name:
        <%=ViewData.Model.Name %><br />
        Description:
        <%= ViewData.Model.Description  %><br />
    </div>
    <br />
    <div>
        <form id="deletecontact" action="/contactlist/delete/<%=ViewData.Model.Id %>" method="post">
        <input type="submit" value="Yes, delete the contact list" />
        <%=Html.ActionLink<ContactListController>(c=>c.Browse(null,null,null),"No, cancel and go back") %>
        </form>
    </div>
</asp:Content>
