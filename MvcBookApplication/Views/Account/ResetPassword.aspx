<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Reset Password</h2>
    <%= Html.ValidationSummary() %>
    <form method="post" action="<%= Html.AttributeEncode(Url.Action("ResetPassword")) %>">
    <div>
        <table>
            <tr>
                <td>
                    Username or Email:
                </td>
                <td>
                    <%= Html.TextBox("usernameOrEmail")%>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <input type="submit" value="Reset Password" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</asp:Content>
