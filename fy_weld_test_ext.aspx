<%@ Page Title="" Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_weld_test_ext.aspx.cs" Inherits="gbe.fy_weld_test_ext" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel ID="pnlMain" runat="server" DefaultButton="btnGetSpool">

<div style="clear:both">&nbsp;</div>

<div >
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Label id="lblReport1" runat="server" Text="Report 1:" CssClass="FY_Label"></asp:Label>
</div>

<div style="clear:both">
    <div style="float:left">
        <asp:TextBox ID="txtReport1" runat="server" Width="600px" CssClass="FY_TextBox"></asp:TextBox>
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Label id="lblReport2" runat="server" Text="Report 2:" CssClass="FY_Label"></asp:Label>
</div>

<div style="clear:both">
    <div style="float:left">
        <asp:TextBox ID="txtReport2" runat="server" Width="600px" CssClass="FY_TextBox"></asp:TextBox>
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    
    <div style="float:left">
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
    </div>
    
</div>

<div style="clear:both">&nbsp;</div>

<div style="clear:both">
    
    <div>
        <asp:Label id="lblBarcode" runat="server" Text="Barcode:" CssClass="FY_Label"></asp:Label>
    </div>

    <div style="float:left">
        <asp:TextBox ID="txtBarcode" runat="server" Width="600px" CssClass="FY_TextBox"></asp:TextBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnGetSpool" runat="server" ImageUrl="~/search32.png" ToolTip="Get spool" OnClick="btnGetSpool_Click" />
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

