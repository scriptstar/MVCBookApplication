<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Pay/PayMaster.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form action="https://www.paypal.com/cgi-bin/webscr" method="post">
    <input type="hidden" name="cmd" value="_s-xclick">
    <input type="hidden" name="hosted_button_id" value="1445233">
    <table>
        <tr>
            <td>
                <input type="hidden" name="on0" value="Site Url">Site Url
            </td>
        </tr>
        <tr>
            <td>
                <input type="text" name="os0" maxlength="60">
    </table>
    <input type="image" src="https://www.paypal.com/en_US/i/btn/btn_subscribeCC_LG.gif"
        border="0" name="submit" alt="">
    <img alt="" border="0" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1"
        height="1">
    </form>
</asp:Content>
