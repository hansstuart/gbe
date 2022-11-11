<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_assign_job.aspx.cs" Inherits="gbe.fy_assign_job" Title="Assign Job" %>
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
    <div style="float:left;  width:606px; overflow: auto ">
        <asp:ListBox ID="lstBarcodes" runat="server" Width="1000px" CssClass="FY_Label" Height="220px" SelectionMode="Multiple"></asp:ListBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <div>
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete32.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
        </div>
        
        <div>&nbsp;</div>
        <div>&nbsp;</div>
        
        <div>
            <asp:ImageButton ID="btnMoveUp" runat="server" ImageUrl="~/Up32.png" ToolTip="Move up" OnClick="btnMoveUp_Click"  />
        </div>
        
        <div>&nbsp;</div>
        
        <div>
            <asp:ImageButton ID="btnMoveDown" runat="server" ImageUrl="~/Down32.png" ToolTip="Move down" OnClick="btnMoveDown_Click"  />
        </div>
    </div>
    
    
    
    
</div>

<div style="clear:both">&nbsp;</div>

<div>

    <div style="float:left">
        <asp:CheckBox id="chkRobot" runat="server" Text="Use Robot" CssClass="FY_Label" TextAlign="Left"></asp:CheckBox>
    </div>

</div>

<div style="clear:both">&nbsp;</div>

<div>
    
    <div style="float:left">
        <asp:Label id="lblWelder" runat="server" Text="Welder:" CssClass="FY_Label"></asp:Label>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:DropDownList ID="cboWelder" runat="server" CssClass="FY_Label" Width="200px"> </asp:DropDownList>
    </div>

    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:Label id="lblFitter" runat="server" Text="Fitter:" CssClass="FY_Label"></asp:Label>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:DropDownList ID="cboFitter" runat="server" CssClass="FY_Label" Width="200px"> </asp:DropDownList>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnAssign" runat="server" ImageUrl="~/apply32.png" ToolTip="Assign" OnClick="btnAssign_Click" />
    </div>
</div>
</asp:Panel>
</asp:Content>
