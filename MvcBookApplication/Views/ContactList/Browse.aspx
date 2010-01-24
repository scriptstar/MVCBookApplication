<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ContactList/ContactListMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="MvcBookApplication.Controllers"%>
<%@ Import Namespace="MvcBookApplication.Models"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Browse Contacts</h2>
    <table class="grid">
        <thead>
            <tr>
                <th>
                    <%=Html.ActionLink<ContactListController>(
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
                foreach (var contactList in ViewData.Model)
                {%>
            <tr>
                <td>
                    <%=contactList.Name%>
                </td>
                <td>
                    <a href='/contactlist/edit/<%=contactList.Id %>'>edit</a> <a href='/contactlist/delete/<%=contactList.Id %>'>
                        delete</a>
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
                                                     "/contactlist/browse/{page}",
                                                 PageNumber = ViewData.Model.PageNumber,
                                                 PageSize = ViewData.Model.PageSize,
                                                 TotalItemCount = ViewData.Model.TotalItemCount,
                                                 PageCount = ViewData.Model.PageCount,
                                                 SortBy = (string)ViewData["sortby"],
                                                 SortDirection = (string)ViewData["sortdir"]
                                             });%>
</asp:Content>
