<%@ Page Title="" Language="C#" MasterPageFile="~/Views/ContactList/ContactListMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--<script type="text/javascript">
    $(document).ready(function() {
        $("#createcontact").submit(function() {
            var f = $("#createcontact");
            f.validate();
            var action = f.attr("action");
            var data = f.serialize();
            $.post(action, data, function() {
                alert("done");
            });
            return false;
        });

        $.ajaxStart(function() {
            $("#createcontact").fadeOut("slow");
        });
        $.ajaxStop(function() {
            $("#createcontact").fadeIn("slow");
        });
    });
</script>--%>
    <h2>
        Create New Contact List</h2>
    <%=Html.ValidationSummary()%>
    <%=Html.JQueryGenerator("createcontact", ViewData.Model)%>
    <form id="createcontact" action="/contactlist/create" method="post">
    <div>
        <%Html.RenderPartial("_contactlistform"); %>
        <input type="submit" value="Create Contact List" />
    </div>
    </form>
</asp:Content>
