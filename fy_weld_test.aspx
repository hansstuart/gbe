<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_weld_test.aspx.cs" Inherits="gbe.fy_weld_test" Title="Weld Test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnAdd">

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
        <asp:Label id="lblFW" runat="server" Text="FW:" CssClass="FY_Label"></asp:Label>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:TextBox ID="txtFW" runat="server" Width="232" CssClass="FY_TextBox"></asp:TextBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:Label id="lblBW" runat="server" Text="BW:" CssClass="FY_Label"></asp:Label>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:TextBox ID="txtBW" runat="server" Width="232" CssClass="FY_TextBox"></asp:TextBox>
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
        <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="~/add32.png" ToolTip="Add to list" OnClick="btnAdd_Click" />
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <div style="float:left; height:220px; overflow: auto">
        <asp:ListBox ID="lstBarcodes" runat="server" Width="606px" CssClass="FY_Label" Height="220px" SelectionMode="Multiple" ></asp:ListBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete32.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <div style="float:left; width:268px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/save32.png" ToolTip="Save" OnClick="btnSave_Click" />
    </div>

</div>
</asp:Panel>
</asp:Content>
