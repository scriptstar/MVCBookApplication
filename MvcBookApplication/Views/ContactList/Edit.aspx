<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ContactList/ContactListMaster.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit Contact List</h2>
    <%=Html.ValidationSummary()%>
    <%=Html.JQueryGenerator("editcontact", ViewData.Model)%>
    <form id="editcontact" action="/contactlist/edit/<%=ViewData.Model.Id %>" method="post">
    <div>
        <%Html.RenderPartial("_contactlistform"); %>
        <input type="submit" value="Save Contact List" />
    </div>
    </form>
</asp:Content>
