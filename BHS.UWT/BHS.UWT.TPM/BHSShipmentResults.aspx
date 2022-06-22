<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/BHSTPM.Master" CodeBehind="BHSShipmentResults.aspx.cs" Inherits="BHS.UWT.TPM.BHSShipmentResults" %>
<asp:Content ID="orderSummaryHeadContent" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">

     </script>
</asp:Content>
     
<asp:Content ID="ShipmentDocumentsMainContent" ContentPlaceHolderID="MainContent" runat="server">
   <asp:Panel ID="ShipmentSelection" runat="server" Visible="true">
        <h2>
            Shipment Selection</h2>
        <div id="ShipmentList" class="content-section">
            <div id="ShipmentList">
                <asp:Table ID="tblShipments" runat="server">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell Width="300px">Shipment</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="300px">Bill of Lading Number</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="300px">Scheduled Ship Date</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="50px"></asp:TableHeaderCell>
		            	<asp:TableHeaderCell Width="75px"></asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                <table>
                    <tbody>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnFirst" OnClick="pager_Click" runat="server" Text="<< First" CssClass="ReturnInput"
                                    Width="80" CommandArgument="First"></asp:Button>
                                <asp:Button ID="btnPrev" OnClick="pager_Click" runat="server" Text="< Previous" CssClass="ReturnInput"
                                    Width="80" CommandArgument="Prev"></asp:Button>
                                Page
                                <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="True" CssClass="ReturnInput"
                                    OnSelectedIndexChanged="ddlPages_SelectedIndexChanged">
                                </asp:DropDownList>
                                of
                                <asp:Label ID="lblPageCount" runat="server"></asp:Label>
                                <asp:Button ID="btnNext" OnClick="pager_Click" runat="server" Text="Next >" CssClass="ReturnInput"
                                    Width="80" CommandArgument="Next"></asp:Button>
                                <asp:Button ID="btnLast" OnClick="pager_Click" runat="server" Text="Last >>" CssClass="ReturnInput"
                                    Width="80" CommandArgument="Last"></asp:Button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <input type="hidden"  runat=server />
        <input id="Hidden1" type="hidden"  runat=server />
        
        <div style="text-align: center; clear: both">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="100" Font-Bold="true"
                CssClass="ReturnInput" CausesValidation="false" OnClick="tbCancel_Click" />
        </div>
    </asp:Panel>
</asp:Content>
    
    
