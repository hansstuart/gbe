<%@ Page Title="" Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_weld_test_ext.aspx.cs" Inherits="gbe.fy_weld_test_ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnGetSpool">

<div style="clear:both">&nbsp;</div>

<div >
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<div style="clear:both">&nbsp;</div>
<div>
    <table style="width: 1096px">
        <tr>
            <td style="width: 247px">
                <asp:Label ID="lblReport1" runat="server" CssClass="FY_Label" Text="Report MPI FW:"></asp:Label>
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport1MPI_FW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
            <td style="width: 289px">
                <asp:Label ID="lblReport2" runat="server" CssClass="FY_Label" Text="Report MPI BW:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtReport2MPI_BW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 247px">
                <asp:Label ID="lblReport3" runat="server" CssClass="FY_Label" Text="Report UT BW:"></asp:Label>
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport3UT_BW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
            <td style="width: 289px">
                <asp:Label ID="lblReport4" runat="server" CssClass="FY_Label" Text="Report XRAY BW:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtReport4XRAY_BW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 247px">
                <asp:Label ID="lblReport5" runat="server" CssClass="FY_Label" Text="Report DP FW:"></asp:Label>
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport5DP_FW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
            <td style="width: 289px">
                <asp:Label ID="lblReport6" runat="server" CssClass="FY_Label" Text="Report DP BW:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtReport6DP_BW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 247px">
                <asp:Label ID="lblReport7" runat="server" CssClass="FY_Label" Text="Report VI FW:"></asp:Label>
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport7VI_FW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
            <td style="width: 289px">
                <asp:Label ID="lblReport8" runat="server" CssClass="FY_Label" Text="Report VI BW:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtReport8VI_BW" runat="server" CssClass="FY_TextBox" Width="284px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 247px">&nbsp;</td>
            <td style="width: 309px">&nbsp;</td>
            <td style="width: 289px">&nbsp;</td>
            <td align="center">
                <asp:Button ID="btnResetReports" runat="server" CssClass="FY_Button" OnClick="btnResetReports_Click" Text="Reset Reports" />
            </td>
        </tr>

    </table>
</div>
<div style="clear:both">
    <div style="float:left">
    </div>
    &nbsp;<div style="float:left">
    </div>
    <div style="float:left">
    </div>
    <div>
        <asp:Label ID="lblBarcode" runat="server" CssClass="FY_Label" Text="Barcode:"></asp:Label>
    </div>
    <div style="float:left">
        <asp:TextBox ID="txtBarcode" runat="server" CssClass="FY_Label" Width="528px"></asp:TextBox>
    </div>
    <div style="float:left; width:10px">
        &nbsp;</div>
    <div style="float:left">
        <asp:ImageButton ID="btnGetSpool" runat="server" ImageUrl="~/search32.png" OnClick="btnGetSpool_Click" ToolTip="Get spool" />
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
          
    <asp:Table ID="tblWeldTest" runat="server" CssClass="FY_Label">
    </asp:Table>
</div>

<div>
    
    <div style="clear:both">&nbsp;</div>

    <div style="float:left; width:268px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/save32.png" ToolTip="Save" OnClick="btnSave_Click" />
    </div>

</div>
</asp:Panel>

<script type="text/javascript">

    function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
</script>

</asp:Content>

