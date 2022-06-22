<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BHS.UWT.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    
        Internal Instruction Number: <input type="text" id="InternalInstructionNumber" /> <input type="button" value="Validate" id="Validate" />
        
        <br /><br />
    
        <input type="text" id="AllowKeyPress" />
 
    </form>
</body>
</html>

<% 
    string js = "";

    js += "<script type='text/javascript' src='content/js/jquery-1.4.2.min.js'></script>\r\n";
    js += "<script type='text/javascript' src='content/js/json2.js'></script>\r\n";
    js += "<script type='text/javascript' src='content/js/BHSLotConfirmOrderPicking.js'></script>\r\n";
    
    Response.Write(js); 
%>


