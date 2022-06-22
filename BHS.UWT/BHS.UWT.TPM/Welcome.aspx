<%@ Page Title="" Language="C#" MasterPageFile="~/BHSTPM.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="BHS.UWT.TPM.Welcome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="WelcomePage" runat="server" Visible="true" DefaultButton="btnDocumentSearch">
        <div id="shipmentSearch" class="content-section">
            <h2 style="text-align: center;">
            Document Retrieval Home Page
            </h2>
            <br />
            <div style="text-align: center; clear: both">
                <asp:Button ID="btnDocumentSearch" runat="server" Text="Document Search" Width="150" Font-Bold="true"
                    CssClass="ReturnInput" CausesValidation="true" ValidationGroup="Login1" OnClick="DocumentSearch_Click" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>