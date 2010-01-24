<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Contact/ContactMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MvcBookApplication.Controllers" %>
<%@ Import Namespace="MvcBookApplication.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Browse Contacts</h2>
    <table class="grid">
        <thead>
            <tr>
                <th>
                    <%=Html.ActionLink<ContactController>(
                                  c => c.Browse(ViewData.Model.PageNumber,
                                                "email",
                                                ViewData.GetSortDirection("email")),
                                  "Email")%>
                </th>
                <th>
                    <%=Html.ActionLink<ContactController>(
                                  c => c.Browse(ViewData.Model.PageNumber,
                                                "name",
                                                ViewData.GetSortDirection("name")),
                                  "Name")%>
                </th>
                <th>
                    Actions
                </th>
            </tr>
        </thead>
        <tbody>
            <%
                foreach (var contact in ViewData.Model)
                {%>
            <tr>
                <td>
                    <%=contact.Email%>
                </td>
                <td>
                    <%=contact.Name%>
                </td>
                <td>
                    <a href='/contact/edit/<%=contact.Id %>'>edit</a> <a href='/contact/delete/<%=contact.Id %>'>delete</a>
                </td>
            </tr>
            <%
                }%>
        </tbody>
    </table>
    <%
        Html.RenderPartial("pagination", new PaginationViewData
                                             {
                                                 PageActionLink =
                                                     "/contact/browse/{page}",
                                                 PageNumber = ViewData.Model.PageNumber,
                                                 PageSize = ViewData.Model.PageSize,
                                                 TotalItemCount = ViewData.Model.TotalItemCount,
                                                 PageCount = ViewData.Model.PageCount,
                                                 SortBy = (string) ViewData["sortby"],
                                                 SortDirection = (string) ViewData["sortdir"]
                                             });%>
</asp:Content>
