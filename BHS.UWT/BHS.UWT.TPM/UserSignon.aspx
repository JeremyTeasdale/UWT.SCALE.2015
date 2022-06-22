<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserSignon.aspx.cs" Inherits="BHS.UWT.TPM.UserSignon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>UWT Document Search</title>
    <link href="<%= AppHelper.CSSUrl("BHSTPM.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= AppHelper.CSSUrl("ui.datepicker.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= AppHelper.CSSUrl("slimbox2.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= AppHelper.CSSUrl("timepicker.css") %>" rel="stylesheet" type="text/css" />

    <script src="<%= AppHelper.ScriptUrl("jquery-1.3.2.min.js") %>" type="text/javascript"></script>

    <script src="<%= AppHelper.ScriptUrl("ui.datepicker.js") %>" type="text/javascript"></script>

    <script src="<%= AppHelper.ScriptUrl("jquery.qtip.js") %>" type="text/javascript"></script>

    <script src="<%= AppHelper.ScriptUrl("slimbox2.js") %>" type="text/javascript"></script>

    <script src="<%= AppHelper.ScriptUrl("jquery.timePicker.js") %>" type="text/javascript"></script>

    <script src="<%= AppHelper.ScriptUrl("json2.min.js") %>" type="text/javascript"></script>

    <script type="text/javascript">
       

    </script>

</head>
<body>
    <form id="FormMain" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
    </asp:ScriptManager>
    <a name="top"></a>
    <div id="header">
        <table border="0" cellpadding="2" cellspacing="0" width="100%">
            <tr>
                <td colspan="3" style="height: 24px; background-color: #0C3A84">
                    <asp:Literal runat="server" ID="FailureText" EnableViewState="False"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td width="100%" colspan="3" style="height: 5px" />
            </tr>
            <tr>
                <td align="left">
                    <asp:HyperLink ID="linkLeft" runat="server">
                        <asp:Image ID="imgLeft" runat="server" />
                    </asp:HyperLink>
                </td>
                <td align="center" width="100%">
                    <asp:HyperLink ID="linkCenter" runat="server">
                        <asp:Image ID="imgCenter" runat="server" />
                    </asp:HyperLink>
                </td>
                <td align="right">
                    <asp:HyperLink ID="linkRight" runat="server">
                        <asp:Image ID="imgRight" runat="server" />
                    </asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="SignOnPanel" runat="server" Visible="true" DefaultButton="btnSignOn">
        <h2>
            Sign On</h2>
        <div id="SignOn" class="content-section">
            <asp:Table ID="tblShipmentSearch" runat="server">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>
                        <asp:ValidationSummary ID="LogonValidationSummary" runat="server" ValidationGroup="Login1"
                            ShowMessageBox="false" DisplayMode=BulletList ShowSummary="true"  />
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>User Name:</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>Password:</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        <div style="text-align: center; clear: both">
            <asp:Button ID="btnSignOn" runat="server" Text="Sign On" Width="100" Font-Bold="true"
                CssClass="ReturnInput" CausesValidation="true" ValidationGroup="Login1" OnClick="LoginButton_Click" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>