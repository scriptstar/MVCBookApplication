<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Uploader</title>
    <style>
        .field-validation-error
        {
            color: #ff0000;
        }
    </style>
</head>
<body>
    <div>
        <form id="frmImageUpload" enctype="multipart/form-data" action="/gallery/upload"
        method='post'>
        <input type="file" id='imageuploader' name='imageuploader' />
        <input type="submit" value="Upload" /><%=Html.ValidationMessage("imageuploader") %>
        </form>
    </div>
</body>
</html>
