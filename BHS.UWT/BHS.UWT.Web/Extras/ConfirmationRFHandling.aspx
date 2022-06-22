<%@ Page language="C#" AutoEventWireup="false" Inherits="Manh.WMFW.RF.ConfirmationRFHandling" %>

<% 
    string js = "";

    js += "<script type='text/javascript' src='js/jquery-1.4.2.min.js'></script>\r\n";
    js += "<script type='text/javascript' src='js/json2.js'></script>\r\n";
    //js += "<script type='text/javascript' src='js/BHSLotConfirmOrderPicking.js'></script>\r\n";
    js += "<script type='text/javascript' src='js/BhsCountBack.js'></script>\r\n";
    
    Response.Write(js); 
%>

<%
    string content = "";

    // add hidden inputs
    string LastInternalInstructionNum;
    if (Session["LastInternalInstructionNum"] != null)
    {
        LastInternalInstructionNum = Session["LastInternalInstructionNum"].ToString();
        content += "<input type='hidden' id='LastInternalInstructionNum' value='" + LastInternalInstructionNum + "' />";
    }
    Response.Write(content);
%>