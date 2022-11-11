<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_msg.aspx.cs" Inherits="gbe.fy_msg" Title="Fab Yard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div style="clear:both">&nbsp;</div>

<div>
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_Label" style="width: 100%"></asp:Label>
</div>

<br/> <br/> <br/> <br/>

<div style="clear:both">&nbsp;</div>
    
<div style="float:left; width: 20%">&nbsp;</div>

<div style="float:left" >
    <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="FY_Button" OnClick="btnOK_Click" />  
</div>

</asp:Content>
