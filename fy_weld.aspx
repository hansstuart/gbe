<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_weld.aspx.cs" Inherits="gbe.fy_weld" Title="Weld/Fit/Build" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel ID="pnlMain" runat="server" DefaultButton="btnAdd">
<div style="clear:both">&nbsp;</div>

<div >
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<div style="clear:both">&nbsp;</div>

<div>

<asp:Panel ID="pnlProcess" runat="server">

<div>
    <asp:Label id="lblProcess" runat="server" Text="Select Process:" CssClass="FY_Label" Visible="False"></asp:Label>
</div>

<div>
    <asp:DropDownList ID="dlProcess" runat="server" CssClass="FY_Label" Visible="False"> </asp:DropDownList>
</div>

</asp:Panel>
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
    <div style="float:left; width:606px; overflow: auto">
    
        <asp:ListBox ID="lstBarcodes" runat="server" Width="1000px" CssClass="FY_Label" Height="220px" SelectionMode="Multiple" ></asp:ListBox>
    
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete32.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
    </div>
    
</div>

<div style="clear:both">
    <asp:Button ID="btnNull" runat="server" Width="0px" BorderStyle="None" Height="0px" BackColor="LightBlue" />&nbsp;</div>

<div style="clear:both">
    <div style="float:left; width:280px">&nbsp;</div>
        
    <div style="float:left">
        <asp:ImageButton ID="btnStart" runat="server" ImageUrl="~/start32.png" ToolTip="Start" OnClick="btnStart_Click"  />
    </div>
    
    <div style="float:left; width:20px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnStop" runat="server" ImageUrl="~/stop32.png" ToolTip="Complete" OnClick="btnStop_Click"  />
    </div>
</div>  
</asp:Panel>
</asp:Content>
