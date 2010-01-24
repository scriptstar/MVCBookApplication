<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Login</h2>
    <p>
        Please enter your username and password below. If you don't have an account, please
        <%= Html.ActionLink("register", "Register") %>.
    </p>
    <%= Html.ValidationSummary()%>
    
    <form method="post" action="<%= Html.AttributeEncode(Url.Action("Login")) %>">
    <div>
        <table>
            <tr>
                <td>
                    Username:
                </td>
                <td>
                    <%= Html.TextBox("Username")%>
                </td>
            </tr>
            <tr>
                <td>
                    Password:
                </td>
                <td>
                    <%= Html.Password("Password")%>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <input type="checkbox" name="rememberMe" value="true" />
                    Remember me?
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <input type="submit" value="Login" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</asp:Content>
