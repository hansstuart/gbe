<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy.aspx.cs" Inherits="gbe.fy" Title="Fab Yard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Panel ID="pnlMain" runat="server">
    
<div style="clear:both">&nbsp;</div>

<div>
    <div style="float:left; width: 10%">&nbsp;</div>
    <div style="float:left; width: 80%">
             <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
    </div>
</div>

<div>
 
    <div style="clear:both">&nbsp;</div>
    
    <div style="float:left; width: 20%">&nbsp;</div>
    <div style="float:left" >
        <asp:Button ID="btnAssignWelder" runat="server" Text="ASSIGN JOB" CssClass="FY_Button" OnClick="btnAssignWelder_Click" UseSubmitBehavior="False"/>  
    </div>
    
    <div style="clear:both">&nbsp;</div>
    
    <div style="float:left; width: 20%">&nbsp;</div>
    
    <div style="float:left" >
        <asp:Button ID="btnWeld" runat="server" Text="WELD/FIT/BUILD" CssClass="FY_Button" OnClick="btnWeld_Click" UseSubmitBehavior="False"/>  
    </div>
    
    <div style="clear:both">&nbsp;</div>
    
    <div style="float:left; width: 20%">&nbsp;</div>
    
    <div style="float:left" >
        <asp:Button ID="btnQA" runat="server" Text="QA" CssClass="FY_Button" UseSubmitBehavior="False" OnClick="btnQA_Click"/>  
    </div>
    
    <div style="clear:both">&nbsp;</div>
    
    <div style="float:left; width: 20%">&nbsp;</div>
    
    <div style="float:left" >
        <asp:Button ID="btnWeldTest" runat="server" Text="WELD TEST" CssClass="FY_Button" UseSubmitBehavior="False" OnClick="btnWeldTest_Click"/>  
    </div>
    
    <div style="clear:both">&nbsp;</div>
    
    <div style="float:left; width: 20%">&nbsp;</div>
    
    <div style="float:left" >
        <asp:Button ID="btnDelivery" runat="server" Text="DELIVERY" CssClass="FY_Button" UseSubmitBehavior="False" OnClick="btnDelivery_Click"/>  
    </div>
    
    <div style="clear:both">&nbsp;</div>
    
    
</div>
</asp:Panel>
</asp:Content>
