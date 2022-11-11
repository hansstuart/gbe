<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="view_schedule.aspx.cs" Inherits="gbe.view_schedule" Title="Fabrication Schedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width: 969px">
        <tr>
            <td style="width: 14px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 14px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3" style="height: 7px">
                &nbsp;<asp:Label ID="lblTitle" runat="server" Font-Names="Verdana"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3" style="height: 58px">
                <asp:Button ID="btnView" runat="server" Width="185px" Height="28px" OnClick="btnView_Click" /></td>
        </tr>
    </table>
    
<asp:MultiView ID="MultiView1" runat="server" OnActiveViewChanged="MultiView1_ActiveViewChanged">
<asp:View ID="view_view_schedule" runat="server">
    <table style="width: 1353px">
        <tr>
                            <td colspan="7">
                                <asp:Label ID="lblMaterial" runat="server" Font-Names="Verdana" Text="Material:"></asp:Label></td>
                            <td colspan="1" style="width: 23px">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="7">
                                <asp:DropDownList ID="dlMaterial" runat="server" OnSelectedIndexChanged="dlMaterial_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList></td>
                            <td colspan="1" style="width: 23px">
                            </td>
                        </tr>
        
        <tr>
            <td colspan="3" style="height: 54px; width: 964px;">
                <asp:Panel ID="pnlFilter" style="width: 100%" runat="server">
                    <table style="width: 1109px">
                        
                        <tr>
                            <td colspan="7">
                                <asp:Label ID="lblFilter" runat="server" Font-Names="Verdana" Text="Filter:"></asp:Label></td>
                            <td colspan="1" style="width: 23px">
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 34px; height: 26px;" bgcolor="red">
                                <asp:CheckBox ID="chkRed" runat="server" Text=" SC, RP" Font-Names="Verdana" Width="91px" /></td>
                            <td style="width: 40px; height: 26px;" bgcolor="darkorange"><asp:CheckBox ID="chkOrange" runat="server" Text="IP, PC" Font-Names="Verdana" Width="78px" /></td>
                            <td style="width: 40px; height: 26px;" bgcolor="deepskyblue"><asp:CheckBox ID="chkBlue" runat="server" Text=" QA, WT" Font-Names="Verdana" Width="91px" /></td>
                            <td style="width: 40px; height: 26px;" bgcolor="green"><asp:CheckBox ID="chkGreen" runat="server" Text="RD, LD" Font-Names="Verdana" Width="78px" /></td>
                            <td style="width: 10px; height: 26px">
                                <asp:CheckBox ID="chkSpools" runat="server" Font-Names="Verdana" Text="Spools" Width="82px" /></td>
                            <td style="width: 10px; height: 26px">
                                <asp:CheckBox ID="chkModules" runat="server" Font-Names="Verdana" Text="Modules" Width="91px"  /></td>
                            <td style="width: 77px; height: 26px">
                                <asp:TextBox ID="txtFitter" runat="server" Width="82px" placeholder="Fitter"></asp:TextBox></td>
                            <td style="height: 26px; width: 220px;">
                                <asp:Button ID="btnApplyFilter" runat="server" Text="Apply" Width="95px" OnClick="btnApplyFilter_Click" /></td>
                        </tr>
                       
                    </table>
                    <br />
                    &nbsp;</asp:Panel>
            </td>
            <td colspan="1" style="width: 964px; height: 54px">
            </td>
        </tr>
        
    </table>
    
     <div style="clear:both">&nbsp;</div>
     <div>
        
        
        <div style="float:left; ">
            <asp:Table ID="tblSchedule" runat="server" Font-Names="Verdana">
            </asp:Table>
        </div>
        
        <div style="float:left; width: 1% ">
            <asp:ImageButton ID="imgReSeq" runat="server" ImageUrl="~/Up-down.png"  
                style="position:fixed;" ToolTip="Re-sequence or Quarantine" OnClick="imgReSeq_Click"  />
        </div> 
        
    </div>
    
</asp:View>

<asp:View ID="view_add_extras" runat="server">
    <table style="width: 969px">
        <tr>
            <td colspan="2" style="height: 21px">
            </td>
        </tr>
        <tr>
            <td colspan="3" style="height: 6px">
                &nbsp;<asp:Label ID="lblAddTitle" runat="server" Font-Names="Verdana" Text="Add To Fabrication Schedule : "></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3" style="height: 21px">
            </td>
        </tr>
    </table>
    <table style="width: 699px">
        <tr>
            <td style="width: 310px">
                <asp:Label id="Label1" runat="server" Font-Names="Verdana" Text="Spool Barcode:"></asp:Label></td>
            <td style="width: 141px">
                <asp:TextBox id="txtBarcode" runat="server" Width="381px">
                </asp:TextBox></td>
            <td style="width: 148px" align="center">
                <asp:Button id="btnAddSpool" runat="server" OnClick="btnAddSpool_Click" Text="Add"
                    Width="95px" /></td>
        </tr>
        <tr>
            <td style="width: 310px">
                <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Text="Delivery:"></asp:Label></td>
            <td style="width: 141px">
            </td>
            <td style="width: 148px">
            </td>
        </tr>
        <tr>
            <td style="width: 310px">
                <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Text="Date (dd/mm/yyyy):"></asp:Label></td>
            <td style="width: 141px">
                <asp:TextBox ID="txtDeliveryDate" runat="server" MaxLength="10"></asp:TextBox></td>
            <td style="width: 148px">
            </td>
        </tr>
        <tr>
            <td style="width: 310px">
                <asp:Label ID="Label5" runat="server" Font-Names="Verdana" Text="Time (hh:mm):"></asp:Label></td>
            <td style="width: 141px">
                <asp:TextBox ID="txtDeliveryTime" runat="server" MaxLength="5"></asp:TextBox></td>
            <td style="width: 148px">
            </td>
        </tr>
        <tr>
            <td style="width: 310px">
                <asp:Label ID="Label6" runat="server" Font-Names="Verdana" Text="Vehicle:"></asp:Label></td>
            <td style="width: 141px">
                <asp:DropDownList ID="dlDeliveryVehicle" runat="server" Width="155px">
                </asp:DropDownList></td>
            <td style="width: 148px">
                </td>
        </tr>
    </table>
</asp:View>

<asp:View ID="view_edit" runat="server">
    
        <table style="width: 553px">
            <tr>
                <td style="width: 222px">
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="Apply To All Selected Spools: " Width="251px"></asp:Label></td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:CheckBox ID="chkDate" runat="server" AutoPostBack="True" Font-Names="Verdana"
                        OnCheckedChanged="chkDate_CheckedChanged" Text="Date (dd/mm/yyyy):" /></td>
                <td>
                    <asp:TextBox ID="txtDate" runat="server" MaxLength="10" Enabled="False"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:CheckBox ID="chkAddToExtras" runat="server" AutoPostBack="True" Font-Names="Verdana"
                        OnCheckedChanged="chkAddToExtras_CheckedChanged" Text="Add To Extras" /></td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:CheckBox ID="chkQuarantine" runat="server" AutoPostBack="True" Font-Names="Verdana"
                        OnCheckedChanged="chkQuarantine_CheckedChanged" Text="Quarantine" /></td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:CheckBox ID="chkTime" runat="server" AutoPostBack="True" Font-Names="Verdana"
                        OnCheckedChanged="chkTime_CheckedChanged" Text="Time (hh:mm):" /></td>
                <td>
                    <asp:TextBox ID="txtTime" runat="server" MaxLength="5" Enabled="False"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 222px">
                    <asp:CheckBox ID="chkVehicle" runat="server" AutoPostBack="True" Font-Names="Verdana"
                        OnCheckedChanged="chkVehicle_CheckedChanged" Text="Vehicle:" /></td>
                <td>
                    <asp:DropDownList ID="dlVehicle" runat="server" Width="155px" Enabled="False">
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td style="width: 222px">
                </td>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Save" Width="95px" OnClick="btnSave_Click" OnClientClick="Confirm() " /></td>
            </tr>
        </table>
    

<br />

<asp:Table ID="tblRecs" runat="server" Font-Names="Verdana">

</asp:Table>

</asp:View>

<asp:View ID="view_confirm_code_change" runat="server">
    <table style="width: 563px">
        <tr>
            <td style="width: 297px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label7" runat="server" Font-Names="Verdana" Text="Confirm Code Change"
                    Width="247px"></asp:Label></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label8" runat="server" Font-Names="Verdana" Text="Previous Code:"
                    Width="184px"></asp:Label></td>
            <td>
                <asp:Label ID="lblPrevCode" runat="server" Font-Names="Verdana" Width="348px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label9" runat="server" Font-Names="Verdana" Text="New Code:" Width="184px"></asp:Label></td>
            <td>
                <asp:Label ID="lblNewCode" runat="server" Font-Names="Verdana" Width="349px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label10" runat="server" Font-Names="Verdana" Text="EMail Recipients:"
                    Width="184px"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtEmailAddr" runat="server" ReadOnly="True" TextMode="MultiLine"
                    Width="292px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 297px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="height: 51px; width: 297px;">
            </td>
            <td style="height: 51px">
                <table style="width: 195px">
                    <tr>
                        <td style="width: 15px">
                            <asp:Button ID="btnCancelCodeChange" runat="server" OnClick="btnCancelCodeChange_Click"
                                Text="Cancel" Width="75px" /></td>
                        <td style="width: 4px">
                            <asp:Button ID="btnConfirmCodeChange" runat="server" Text="Confirm" Width="75px" OnClick="btnConfirmCodeChange_Click" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
</asp:View>

<asp:View ID="view_notes" runat="server">

<table style="width: 563px">
        <tr>
            <td style="width: 297px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label11" runat="server" Font-Names="Verdana" Text="Notes"
                    Width="247px"></asp:Label></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label12" runat="server" Font-Names="Verdana" Text="Date:"
                    Width="184px"></asp:Label></td>
            <td>
                <asp:Label ID="lblDate" runat="server" Font-Names="Verdana" Width="348px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label14" runat="server" Font-Names="Verdana" Text="Time:" Width="184px"></asp:Label></td>
            <td>
                <asp:Label ID="lblTime" runat="server" Font-Names="Verdana" Width="349px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 297px">
                <asp:Label ID="Label16" runat="server" Font-Names="Verdana" Text="Vehicle:"
                    Width="184px"></asp:Label></td>
            <td>
                <asp:Label ID="lblVehicle" runat="server" Font-Names="Verdana" Width="349px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 297px; height: 98px;">
                <asp:Label ID="Label13" runat="server" Font-Names="Verdana" Text="Note:" Width="247px"></asp:Label></td>
            <td style="height: 98px">
                <asp:TextBox ID="txtNote" runat="server" Height="76px" TextMode="MultiLine" Width="100%" MaxLength="100"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="height: 51px; width: 297px;">
            </td>
            <td style="height: 51px">
                <table style="width: 195px">
                    <tr>
                        <td style="width: 15px">
                            <asp:Button ID="btnCancelNote" runat="server" 
                                Text="Cancel" Width="75px" OnClick="btnCancelNote_Click" /></td>
                        <td style="width: 4px">
                            <asp:Button ID="btnOKNote" runat="server" Text="OK" Width="75px" OnClick="btnOKNote_Click"  /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:View>

<asp:View ID="re_sequence" runat="server">

<table style="width: 700px">
        <tr>
            <td style="width: 25px">
                </td>
            <td style="width: 1592px">
            </td>
        </tr>
        <tr>
            <td style="width: 25px">
                </td>
            <td style="width: 1592px">
                <asp:Label ID="lblInsertAtSeq" runat="server" Font-Names="Verdana" Text="Insert at sequence:"
                    Width="163px"></asp:Label>
                <asp:TextBox ID="txtSeq" runat="server" MaxLength="6"></asp:TextBox></td>
        </tr>
    <tr>
        <td style="width: 25px">
        </td>
        <td style="width: 1592px">
            <asp:CheckBox ID="chkQuarantineSpools" runat="server" Font-Names="Verdana" Text="or Quarantine"
                TextAlign="Left" /></td>
    </tr>
        <tr>
            <td style="width: 25px; height: 98px;">
                <asp:Label ID="lblReseqSpools" runat="server" Font-Names="Verdana" Text="Spools:"></asp:Label></td>
            <td style="height: 98px; width: 1592px;">
                <asp:TextBox ID="txtReseqSpools" runat="server" Height="165px" TextMode="MultiLine" Width="100%" MaxLength="100"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="height: 51px; width: 25px;">
            </td>
            <td style="height: 51px; width: 1592px;">
                <table style="width: 195px">
                    <tr>
                        <td style="width: 15px">
                            <asp:Button ID="btnReSeqCancel" runat="server" 
                                Text="Cancel" Width="75px" OnClick="btnReSeqCancel_Click" /></td>
                        <td style="width: 4px">
                            <asp:Button ID="btnReSeqOK" runat="server" Text="OK" Width="75px" OnClick="btnReSeqOK_Click"  /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:View>
    
</asp:MultiView>

<script type="text/javascript">
    function Confirm() 
    {
        var confirm_save_value = document.createElement("INPUT");
        confirm_save_value.type = "hidden";
        confirm_save_value.name = "confirm_save_value";
    
        if (confirm("Save?")) 
        {
            confirm_save_value.value = "Yes";
        } 
        else 
        {
            confirm_save_value.value = "No";
        }
        
        document.forms[0].appendChild(confirm_save_value);
    }
</script>

</asp:Content>
