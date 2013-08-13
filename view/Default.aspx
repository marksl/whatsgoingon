<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="view.Default" %>
<%@ Import Namespace="view" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Release Manager</title>
    
    <style type="text/css">
        .i {COLOR:black; BACKGROUND-COLOR:#80FF80}
        .d {COLOR:black; BACKGROUND-COLOR:#FF8080}
        .g {COLOR:lightgrey;}
    
        </style>
    
    

    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="http://netdna.bootstrapcdn.com/twitter-bootstrap/2.3.2/css/bootstrap-combined.min.css" rel="stylesheet" media="screen">

</head>
<body style="margin: 20px">
    
    <script src="http://code.jquery.com/jquery.js"></script>
    <script src="http://netdna.bootstrapcdn.com/twitter-bootstrap/2.3.2/js/bootstrap.min.js"></script>
    
    <form id="form1" runat="server">
    <div>
    
        <div >
  <div >
      <h1>Last <% Response.Write(NumBuilds); %>  Builds</h1>

        <table id="changetable" class="table table-bordered">
            <colgroup>
                <col span="1" width="100px"/>
                <col span="1" width="90px"/>
            </colgroup>
            <thead >
                <tr>
                    <th>Build</th> <% foreach (var category in Categories)
                       {
                           Response.Write("<th>");
                           Response.Write(Context.Server.HtmlEncode(category));
                           Response.Write("</th>");
                       }  %>
                </tr>
            </thead>
            <tbody >
            <%  foreach (BuildData build in Builds)
                {
                    Response.Write("<tr><td>");
                    Response.Write(build.Version);
                    Response.Write("</td>");

                    foreach (var category in Categories)
                    {
                        string html = build.GetHtml(category);
                        if (html != null)
                        {
                            Response.Write("<td>");
                            Response.Write(html);
                            Response.Write("</td>");
                        }
                        else
                        {
                            Response.Write("<td></td>");
                        }
                    }
                    Response.Write("</tr>");
                } %>
            </tbody>
        </table>
            </div>
            </div>
        
        <asp:Button ID="ShowEmpties" runat="server" Text="ShowEmpties" OnClick="ShowEmpties_Click" />

        <h1>Filter</h1>
        
        Previous: <asp:DropDownList ID="PreviousDropDownList" DataValueField="Val" DataTextField="Txt" runat="server"></asp:DropDownList> Current: <asp:DropDownList ID="CurrentDropDownList" DataValueField="Val" DataTextField="Txt" runat="server"></asp:DropDownList> <asp:Button ID="ViewReport" runat="server" Text="View" OnClick="ViewReport_Click" />

        

        
    </div>
    </form>
</body>
</html>
