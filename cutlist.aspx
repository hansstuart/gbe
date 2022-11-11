<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="cutlist.aspx.cs" Inherits="gbe.cutlist" Title="Cut List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div>
        <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label>
    </div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div style="font-family: Verdana; height: 24px;">
        Sort By:  <asp:DropDownList ID="dlSortBy" runat="server" Width="263px" AutoPostBack="True" OnSelectedIndexChanged="dlSortBy_SelectedIndexChanged"></asp:DropDownList>
    </div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div>
        <asp:Table ID="tblMain" runat="server" Font-Names="Verdana" Font-Size="11pt"> </asp:Table>
    </div>
</asp:Content>
