<%@ Page Language="C#" MasterPageFile="~/MANHRF.Master" AutoEventWireup="true" CodeBehind="UWTUMCCWorkTypeSelection.aspx.cs" Inherits="BHS.UWT.WEB.UWTUMCCWorkTypeSelection" Title="UM Cycle Count Work Type Selection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div  id="header">
       <h3>Select Work Type</h3>
    </div>
    
    <asp:Panel ID="panelDef" runat="server" DefaultButton="butStart">
    <hr class="formDivider"/>
     <table>
            <tr><td colspan="2">
                <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label></td></tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblWorkType" runat="server" Text="Work Type:"></asp:Label>
                </td>
                <td >
                    <asp:DropDownList ID="ddlWorkTypes" runat="server" Width="125px">
                    </asp:DropDownList><asp:TextBox ID="tbHid" runat="server" Width="1px" Visible="false"></asp:TextBox>
                </td>               
                <td>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblLocation" runat="server" Text="Location:"></asp:Label>
                </td>
                <td class="textInputColumn">
                    <asp:TextBox ID="tbLocation"  runat="server" Width="125px"></asp:TextBox>
                </td>               
                <td>
                </td>
            </tr>
            
     </table>
     <hr class="formDivider"/>
      <div id="buttons">
            <asp:Button ID="butStart"  runat="server" Text="Start" OnClick="butStart_Click"/>
            &nbsp;<asp:Button ID="butCancel" runat="server" Text="Cancel" CausesValidation="False" UseSubmitBehavior="False" OnClick="butCancel_Click" />
      </div>
    </asp:Panel>
    </asp:Content>
