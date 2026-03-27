<%@ Page Title="" Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_weld_test_v2.aspx.cs" Inherits="gbe.fy_weld_test_v2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnGetSpool">

<div style="clear:both">&nbsp;</div>

<div >
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<div style="clear:both">&nbsp;</div>
<div>
    <table border="1" style="width: 1096px">
        <tr>
            <td style="width: 165px">
                <asp:Label ID="lblReport9" runat="server" CssClass="FY_Label" Text="Client Reference:"></asp:Label>
            </td>
            <td style="width: 309px">
                <asp:CheckBox ID="chkReport2MPI_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport2MPI_BW_CheckedChanged" Text="MPI BW:" Visible="False" />
                <asp:TextBox ID="txtReport2MPI_BW" runat="server" CssClass="FY_Label" Visible="False" Width="16px"></asp:TextBox>
            </td>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport6DP_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport6DP_BW_CheckedChanged" Text="DP BW:" Visible="False" />
                <asp:TextBox ID="txtReport6DP_BW" runat="server" CssClass="FY_Label" Visible="False" Width="19px"></asp:TextBox>
            </td>
            <td>
                <asp:CheckBox ID="chkReport8VI_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport8VI_BW_CheckedChanged" Text="VI BW:" Visible="False" />
                <asp:TextBox ID="txtReport8VI_BW" runat="server" CssClass="FY_Label" Visible="False" Width="16px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport1MPI_FW" runat="server" CssClass="FY_Label" Text="MPI:" AutoPostBack="True" OnCheckedChanged="chkReport1MPI_FW_CheckedChanged" />
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport1MPI_FW" runat="server" CssClass="FY_TextBox" Width="375px"></asp:TextBox>
            </td>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport3UT_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport3UT_BW_CheckedChanged" Text="UT BW:" />
            </td>
            <td>
                <asp:TextBox ID="txtReport3UT_BW" runat="server" CssClass="FY_TextBox" Width="375px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport5DP_FW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport5DP_FW_CheckedChanged" Text="DP:" />
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport5DP_FW" runat="server" CssClass="FY_TextBox" Width="375px"></asp:TextBox>
            </td>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport9PA_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport9PA_BW_CheckedChanged" Text="PA BW:" />
            </td>
            <td>
                <asp:TextBox ID="txtReport9PA_BW" runat="server" CssClass="FY_TextBox" Width="375px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport7VI_FW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport7VI_FW_CheckedChanged" Text="VI:" />
            </td>
            <td style="width: 309px">
                <asp:TextBox ID="txtReport7VI_FW" runat="server" CssClass="FY_TextBox" Width="375px"></asp:TextBox>
            </td>
            <td style="width: 165px">
                <asp:CheckBox ID="chkReport4XRAY_BW" runat="server" AutoPostBack="True" CssClass="FY_Label" OnCheckedChanged="chkReport4XRAY_BW_CheckedChanged" Text="XRAY BW:" />
            </td>
            <td>
                <asp:TextBox ID="txtReport4XRAY_BW" runat="server" CssClass="FY_Label" Width="375px"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td style="width: 165px">
                &nbsp;</td>
            <td style="width: 309px">
                &nbsp;</td>
            <td style="width: 165px">
                &nbsp;</td>
            <td align="right">
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
    <asp:Table ID="tblSpoolParts" runat="server" CssClass="FY_Label">
    </asp:Table>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Table ID="tblWeldTest" runat="server" CssClass="FY_Label">
    </asp:Table>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:DropDownList ID="dlPassFail" runat="server" CssClass="FY_Label"></asp:DropDownList>
</div>

<div>
    
    <div style="clear:both">&nbsp;</div>

    <div style="float:left; width:547px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/save32.png" ToolTip="Save" OnClick="btnSave_Click" />
    </div>

</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Table ID="tblActiveTestProjects" runat="server" CssClass="FY_Label">
    </asp:Table>
</div>

</asp:Panel>


<script type="text/javascript">

    function isNumberKey(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

    function showFailCodeDroplists(id, MAX_WELD_TESTS) {

        //alert('plop ' + String(MAX_WELD_TESTS).padStart(3, '0') );

        for (i = 0; i < MAX_WELD_TESTS; i++)
        {
            document.getElementById("ContentPlaceHolder1_dlFailCode_" + id + "_" + String(i).padStart(3, '0')).className = 'invisible';
        }

        var num_of_welds = (document.getElementById("ContentPlaceHolder1_" + id).value);

        for (i = 0; i < num_of_welds; i++)
        {
            if (i == MAX_WELD_TESTS)
                break;

            document.getElementById("ContentPlaceHolder1_dlFailCode_" + id + "_" + String(i).padStart(3, '0')).className = 'FY_TableTextBox';
        }
    }

</script>

</asp:Content>

