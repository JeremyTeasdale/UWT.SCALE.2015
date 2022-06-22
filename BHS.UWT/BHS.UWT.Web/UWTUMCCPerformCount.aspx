<%@ Page Language="C#" MasterPageFile="~/MANHRF.Master" AutoEventWireup="true" CodeBehind="UWTUMCCPerformCount.aspx.cs" Inherits="BHS.UWT.WEB.UWTUMCCPerformCount" Title="Perform CC Count" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
function UpdateQTY()
{
    window.status = document.getElementById('ctl00_ContentPlaceHolder1_tbUMQty1').value;
    var qty = aspnetForm.ctl00_ContentPlaceHolder1_tbUMQty1.value * aspnetForm.ctl00_ContentPlaceHolder1_hfUMConvQty1.value;
    
    if(aspnetForm.ctl00_ContentPlaceHolder1_tbUMQty2 != undefined)
        qty += aspnetForm.ctl00_ContentPlaceHolder1_tbUMQty2.value * aspnetForm.ctl00_ContentPlaceHolder1_hfUMConvQty2.value;
    
    if(aspnetForm.ctl00_ContentPlaceHolder1_tbUMQty3 != undefined)
        qty += aspnetForm.ctl00_ContentPlaceHolder1_tbUMQty3.value * aspnetForm.ctl00_ContentPlaceHolder1_hfUMConvQty3.value;
    
    document.getElementById('lblBaseUMQty').innerHTML = qty;
}
</script>
 <div id="header">
        <asp:Label ID="lblHeading" runat="server" Text="Cycle Count"></asp:Label>
                      
    </div>  
<asp:Panel ID="panelDef" runat="server" DefaultButton="butStart" Width="230">
    <hr class="formDivider"/>
     <table>
            <tr class="labelColumn">
                <td colspan="2"><asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label></td>
            </tr>
            <tr>
                <td class="labelColumn">Loc:</td>
                <td class="displayOnlyTextContentColumn">
                    <asp:Label ID="lblLocation" runat="server" Text=""></asp:Label>                   
                    
                </td>               
                <td class="textInputColumn">
                    <asp:TextBox ID="tbHid" runat="server" Width="1px" Visible="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">Item:</td>
                <td class="displayOnlyTextContentColumn">
                    <asp:Label ID="lblItem" runat="server" Text=""></asp:Label>
                    <asp:TextBox ID="txtItem" runat="server" Width="100px"></asp:TextBox>                     
                </td>               
                <td>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblLot" runat="server" Text=""></asp:Label></td>
                <td class="displayOnlyTextContentColumn">
                    &nbsp;<asp:TextBox ID="txtLot" runat="server" Width="100px"></asp:TextBox>                     
                </td>               
                <td>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">Desc:</td>
                <td class="displayOnlyTextContentColumn">
                    <asp:Label ID="lblDesc" runat="server" Text=""></asp:Label>                   
                </td>               
                <td>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblUM1" runat="server" Text=""></asp:Label>
                </td>
                <td class="textInputColumn">
                    <asp:TextBox ID="tbUMQty1" runat="server" Width="55px"></asp:TextBox>                
                </td>               
                <td>
                    <asp:HiddenField ID="hfUMConvQty1" runat="server" Value="0" />
                    <asp:RegularExpressionValidator ID="revUM1Qty" runat="server" ControlToValidate="tbUMQty1"
                        ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblUM2" runat="server" Text=""></asp:Label>
                </td>
                <td class="textInputColumn">
                    <asp:TextBox ID="tbUMQty2" runat="server" Width="55px"></asp:TextBox>             
                </td>               
                <td>
                    <asp:HiddenField ID="hfUMConvQty2" runat="server" Value="0" />
                    &nbsp;
                    <asp:RegularExpressionValidator ID="revUM2Qty" runat="server" ControlToValidate="tbUMQty2"
                        ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">
                    <asp:Label ID="lblUM3" runat="server" Text=""></asp:Label>
                </td>
                <td class="textInputColumn">
                    <asp:TextBox ID="tbUMQty3" runat="server" Width="55px"></asp:TextBox>               
                </td>               
                <td>
                    <asp:HiddenField ID="hfUMConvQty3" runat="server" Value="0" />
                    &nbsp;
                    <asp:RegularExpressionValidator ID="revUM3Qty" runat="server" ControlToValidate="tbUMQty3"
                        ErrorMessage="*" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="labelColumn">Base UM Qty:</td>
                <td class="labelColumn">
                    <div id="lblBaseUMQty"></div>
                </td>
                <td></td>
            </tr> 
     </table>
     <hr class="formDivider"/>
      <div id="buttons">
        <asp:Button ID="butStart"  runat="server" Text="OK" OnClick="butStart_Click"/>
        &nbsp;<asp:Button ID="butDone" runat="server" Text="Done" CausesValidation="False" UseSubmitBehavior="False"  OnClick="butDone_Click" />
        &nbsp;<asp:Button ID="but_Add" runat="server" Text="Add" CausesValidation="False" UseSubmitBehavior="False" OnClick="but_AddClick" />
   
      </div>

    </asp:Panel>
</asp:Content>
