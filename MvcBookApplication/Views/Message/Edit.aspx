<%@ Page Language="C#" MasterPageFile="~/Views/Message/MessageMaster.Master" Inherits="System.Web.Mvc.ViewPage<Message>" %>

<%@ Import Namespace="MvcBookApplication.Data.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit Message</h2>
    <%= Html.ValidationSummary()%>
    <%= Html.JQueryGenerator("editmessage", ViewData.Model) %>
    <form id="editmessage" action="/message/edit" method="post">
    <div>
        <label for="name">
            Name</label>
        <br />
        <%=Html.TextBox("name") %>
        <br />
        <label for="subject">
            Subject</label>
        <br />
        <%=Html.TextBox("subject") %>
        <br />
        <label for="html">
            Html Body (optional)</label>
        <br />
        <%=Html.TextArea("html") %>
        <br />
        <label for="text">
            Text Body</label>
        <br />
        <%=Html.TextArea("text") %>
        <br />
        <%=Html.Hidden("id", ViewData.Model.Id) %>
        <input type="submit" value="Save Message" />
    </div>
    </form>

    <script type="text/javascript">
    $(document).ready(function() {
        $("#html").wysiwyg({
            controls: {
                html: { visible: true }
            }
        });
    });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="/Scripts/jquery-wysiwyg/jquery.wysiwyg.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery-wysiwyg/jquery.wysiwyg.js" type="text/javascript"></script>

</asp:Content>
