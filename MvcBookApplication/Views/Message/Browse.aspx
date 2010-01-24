<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Message/MessageMaster.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Message>>" %>

<%@ Import Namespace="MvcBookApplication.Data.Models" %>
<%@ Import Namespace="MvcBookApplication.Models" %>
<%@ Import Namespace="MvcBookApplication.Services" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Browse Messages</h2>
    <table class="grid">
        <thead>
            <tr>
                <th>
                    Name
                </th>
                <th>
                    Actions
                </th>
            </tr>
        </thead>
        <tbody>
            <%
                foreach (var message in ViewData.Model)
                {%>
            <tr>
                <td>
                    <%=message.Name%>
                </td>
                <td>
                    <a href='/message/edit/<%=message.Id %>'>edit</a>
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
                                                     "/message/browse/{page}",
                                                 PageNumber = ViewData.Model.PageNumber,
                                                 PageSize = ViewData.Model.PageSize,
                                                 TotalItemCount = ViewData.Model.TotalItemCount,
                                                 PageCount = ViewData.Model.PageCount
                                             });%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
