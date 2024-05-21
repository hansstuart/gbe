<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="cut_length.aspx.cs" Inherits="gbe.cut_length" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label>
    </div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div> 
        <asp:CheckBox ID="chkCutterView" runat="server" Text="Cutter's view" AutoPostBack="True" Font-Names="Verdana" OnCheckedChanged="chkCutterView_CheckedChanged" />
    </div>
    
    <div style="clear: both">&nbsp;</div>

    <div>
        <asp:Table ID="tblMain" runat="server" Font-Names="Verdana" Font-Size="11pt"> </asp:Table>
    </div>

</asp:Content>
