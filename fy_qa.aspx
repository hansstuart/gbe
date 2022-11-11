<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_qa.aspx.cs" Inherits="gbe.fy_qa" Title="QA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnAdd">
<div style="clear:both">&nbsp;</div>

<div >
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Label id="lblBarcode" runat="server" Text="Spool Barcode:" CssClass="FY_Label"></asp:Label>
</div>

<div style="clear:both">
    
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
    <div style="float:left">
        <asp:ListBox ID="lstBarcodes" runat="server" Width="606px" CssClass="FY_Label" Height="450px" SelectionMode="Multiple"></asp:ListBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <div>
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete32.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
        </div>
        
        <div>&nbsp;</div>
        <div>&nbsp;</div>
        
    </div>
    
    <div style="clear:both" >&nbsp;</div>

    <div style="float:left; width:180px" >&nbsp;</div>
    
     <div style="float:left">
            <asp:ImageButton ID="btnPass" runat="server" ImageUrl="~/apply32.png" ToolTip="Pass" OnClick="btnPass_Click" />
     </div>
    <div style="float:left; width:180px" >&nbsp;</div>
    
    <div style="float:left">
            <asp:ImageButton ID="btnFail" runat="server" ImageUrl="~/fail32.png" ToolTip="Fail" OnClick="btnFail_Click" />
     </div>
     
</div>

</asp:Panel>    
</asp:Content>
