<%@ Import Namespace="MvcBookApplication.Models"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<PaginationViewData>" %>
<div class="pagination">
    <ul>
        <%
            if (ViewData.Model.HasPreviousPage)
            {%>
        <li class="previous"><a href='<%=ViewData.Model.PageActionLink
                                      .Replace("{page}",
                                               (ViewData.Model.PageNumber - 1).ToString()) +
                                  (string.IsNullOrEmpty(ViewData.Model.SortBy)
                                       ? ""
                                       : "?sortby=" + ViewData.Model.SortBy
                                         + "&sortdir=" + ViewData.Model.SortDirection)
                    %>'>« Previous</a></li>
        <%
            }
            else
            {%>
        <li class="previous-off">« Previous</li>
        <%
            }%>
        <%
            for (int page = 1; page <= ViewData.Model.PageCount; page++)
            {
                if (page == ViewData.Model.PageNumber)
                {%>
        <li class="active">
            <%=page.ToString()%></li>
        <%
                }
                else
                {%>
        <li><a href="<%=ViewData.Model.PageActionLink
                                          .Replace("{page}", page.ToString() +
                                                             (string.IsNullOrEmpty(ViewData.Model.SortBy)
                                                                  ? ""
                                                                  : "?sortby=" +
                                                                    ViewData.Model.SortBy
                                                                    + "&sortdir=" +
                                                                    ViewData.Model.
                                                                        SortDirection))%>">
            <%=page.ToString()%></a></li>
        <%
                }
            }

            if (ViewData.Model.HasNextPage)
            {%>
        <li class="next"><a href="<%=ViewData.Model.PageActionLink
                                      .Replace("{page}",
                                               (ViewData.Model.PageNumber + 1).ToString()) +
                                  (string.IsNullOrEmpty(ViewData.Model.SortBy)
                                       ? ""
                                       : "?sortby=" + ViewData.Model.SortBy
                                         + "&sortdir=" + ViewData.Model.SortDirection)%>">Next
            »</a></li>
        <%
            }
            else
            {%>
        <li class="next-off">Next »</li>
        <%
            }%>
    </ul>
</div>
