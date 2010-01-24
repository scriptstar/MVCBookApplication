<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Account/AccountMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Reset Password</h2>
    <%= Html.ValidationSummary() %>
    <form method="post" action="<%= Html.AttributeEncode(Url.Action("ResetPasswordQuestion", ViewData.Model)) %>">
    <div>
        <table>
            <tr>
                <td>
                    Question:
                </td>
                <td>
                    <%= ViewData.Model.Question%>
                </td>
            </tr>
            <tr>
                <td>
                    Answer:
                </td>
                <td>
                    <%= Html.TextBox("answer")%>
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
