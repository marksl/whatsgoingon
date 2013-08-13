<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Filtered.aspx.cs" Inherits="view.Filtered" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Release Manager</title>

<style type="text/css">
        .i {COLOR:black; BACKGROUND-COLOR:#80FF80}
        .d {COLOR:black; BACKGROUND-COLOR:#FF8080}
    </style>
    

    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="//netdna.bootstrapcdn.com/twitter-bootstrap/2.3.2/css/bootstrap-combined.min.css" rel="stylesheet">
</head>
<body style="margin: 20px">
    
    <script src="http://code.jquery.com/jquery.js"></script>
    <script src="//netdna.bootstrapcdn.com/twitter-bootstrap/2.3.2/js/bootstrap.min.js"></script>

    <form id="form1" runat="server">
    <div>
    <h1>Changes: <% Response.Write(PrevText); %>
        and 
        <% Response.Write(CurrText); %>
    </h1>

        <table class="table table-bordered">
        
        <% foreach (var category in Categories)
        {
            Response.Write("<tr><th>");
            Response.Write(Context.Server.HtmlEncode(category));
            Response.Write("</th><td>");
            Response.Write(GetHtml(category));
            Response.Write("</td></tr>");
        }  %>
            
        </table>

    </div>
    </form>
    
    <h3><a href="Default.aspx">Back</a></h3>
    
</body>
</html>
