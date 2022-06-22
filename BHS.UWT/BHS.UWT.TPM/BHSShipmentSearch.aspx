<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/BHSTPM.Master" CodeBehind="BHSShipmentSearch.aspx.cs" Inherits="BHS.UWT.TPM.BHSShipmentSearch" %>
<asp:Content ID="orderSummaryHeadContent" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        
        $(document).ready(function() {
        $('.datePicker').datepicker({
                dateFormat: "mm-dd-yy",
                changeYear: true,
                changeMonth: true
            });
            /*
            $("#tbScheduledShipDate").focus(function() {
                $("#tbScheduledShipDate").datepicker("show");
            });

            $("#tbScheduledShipDate").click(function() {
                $("#tbScheduledShipDate").showDatePicker();
            });

            //$("#tbScheduledShipDate").focus();
            */
        });
        
     </script>
</asp:Content>
     
<asp:Content ID="ShipmentDocumentsMainContent" ContentPlaceHolderID="MainContent" runat="server">
   <asp:Panel ID="ShipmentSelection" runat="server" Visible="true" DefaultButton="btnSearch">
        <h2>
            Shipment Search</h2>
        <div id="shipmentSearch" class="content-section">
            <div id="Shipments">
                <asp:Table ID="tblShipmentSearch" runat="server">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>
                            <asp:ValidationSummary ID="SearchValidationSummary" runat="server" ValidationGroup="SearchValidation"
                                ShowMessageBox="false" DisplayMode=BulletList ShowSummary="true"  />
                        </asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                    <asp:TableRow>
                        <asp:TableCell>Shipment</asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="tbShipmentId" runat="server" MaxLength="25"></asp:TextBox></asp:TableCell>                        
                    </asp:TableRow>
                     <asp:TableRow>
                        <asp:TableCell>BOL Number</asp:TableCell>
                        <asp:TableCell><asp:TextBox ID="tbBOL" runat="server" MaxLength="25"></asp:TextBox></asp:TableCell>                        
                    </asp:TableRow>
                     <asp:TableRow>
                        <asp:TableCell>Scheduled Ship Date</asp:TableCell>
                        <asp:TableCell><div id="datepicker"><asp:TextBox ID="tbScheduledShipDate" runat="server" class="datePicker"></asp:TextBox></div></asp:TableCell>                        
                    </asp:TableRow>                                      
                </asp:Table>
            </div>
        </div>
        
        <div style="text-align: center; clear: both">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="100" Font-Bold="true"
                CssClass="ReturnInput" CausesValidation="false" OnClick="btnCancel_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" Width="100" Font-Bold="true"
                CssClass="ReturnInput" CausesValidation="false" OnClick="btnClear_Click"/>
              <asp:Button ID="btnSearch" runat="server" Text="Search" Width="100" Font-Bold="true"
                CssClass="ReturnInput" CausesValidation="true" ValidationGroup = "SearchValidation" OnClick="btnSearch_Click" />
        </div>
    </asp:Panel>
</asp:Content>
    
    
