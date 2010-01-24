<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Message/MessageMaster.Master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="/Scripts/jquery-wysiwyg/jquery.wysiwyg.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery-wysiwyg/jquery.wysiwyg.js" type="text/javascript"></script>

    <% if (false){%>
<script src="../../Scripts/jquery-1.3.1.min-vsdoc.js" type="text/javascript"></script>
<%}%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create New Message</h2>
    <%=Html.ValidationSummary()%>
    <%=Html.JQueryGenerator("createmessage", ViewData.Model)%>
    <% if ((ViewData["mytemplates"] as IList) != null && (ViewData["mytemplates"] as IList).Count > 0)
       {%>
    <h3>
        My Templates</h3>
    <div id='myTemplates'>
        <%
            Html.RenderPartial("/views/template/list.ascx", ViewData["mytemplates"]);%>
    </div>
    <%}%>
    <h3>
        System Template</h3>
    <div id='templatesContainer'>
        <%
            Html.RenderPartial("/views/template/list.ascx", ViewData["templates"]);%>
    </div>
    <form id="createmessage" action="/message/create" method="post">
    <div>
        <label for="name">
            Name</label>
        <br />
        <%=Html.TextBox("name")%>
        <br />
        <label for="subject">
            Subject</label>
        <br />
        <%=Html.TextBox("subject")%>
        <br />
        <label for="html">
            Html Body (optional)</label>
        <br />
        <%=Html.TextArea("html", new Dictionary<string, object> {{"cols", "80"},{"rows","20"}})%>
        <br />
        <label for="text">
            Text Body</label>
        <br />
        <%=Html.TextArea("text")%>
        <br />
        <input type="submit" value="Create Message" />
    </div>
    </form>
    <div id="dialog" title="Image Gallery" style="display: none">
        <iframe id='frameuploader' src="/gallery/uploader" style='width: 100%; height: 64px;'
            frameborder="0"></iframe>
        <div id='galleryImages'>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function() {
            $myDialog = $("#dialog").dialog({
                modal: true,
                width: 500,
                height: 300,
                autoOpen: false
            });

            $("#html").wysiwyg({
                controls: {
                    html: { visible: true },
                    insertImage: {
                        visible: true,
                        exec: function() { showImageDialog(); }
                    },
                    saveTemplate: {
                        visible: true,
                        exec: function() { saveTemplate(); }
                    }
                }
            });

            $("#frameuploader").load(function() {
                $.get('/gallery/getallimages',
                    null,
                    function(data, textStatus) {
                        $("#galleryImages").empty();
                        $(data).each(function() {
                            var imgid = this.id;
                            var img = $("<img height='32px' src='/gallery/getimage/"
                                + imgid + "'/>");
                            img.click(function() {
                                $("#html").wysiwyg("insertImage", "http://localhost:4452/gallery/getimage/" + imgid);
                                $myDialog.dialog("close");
                            });
                            $("#galleryImages")
                                .append(img);
                        });
                    },
                    'json');
            });

            $(".templatelink").click(function() {
                $.get($(this).attr("href"), null, function(data) {
                    $("#html").wysiwyg('setContent', data);
                });
                return false;
            });


        });

        function showImageDialog() {
            $("#dialog").show();
            $myDialog.dialog("open");
        }

function saveTemplate() {
    var content = $("#html").wysiwyg('getContent').html();
    var name = prompt("What is the name of the template?");
    $.ajax({
        type: "POST",
        url: "/template/save",
        dataType: 'json',
        data: { name: name, content: content },
        success: function(data) {
            if (data.success == true)
                alert("Template saved");
            else
                alert("Error saving");
        } 
    });
}
    </script>

</asp:Content>
