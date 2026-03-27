<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="DELETEcut_lengths_print.aspx.cs" Inherits="gbe.cut_length_print" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label>
    </div>
    
    <div style="clear: both">&nbsp;</div>
    
    <div style="clear: both">&nbsp;</div>
        
    <div style="clear: both">&nbsp;</div>

    <div>
        <div style="float:left; ">

            <asp:Table ID="tblMain" runat="server" Font-Names="Verdana" Font-Size="11pt"> </asp:Table>
        </div>

        <div style="float:left; ">&nbsp;</div>

        <div style="float:left; width: 1% ">
            <asp:ImageButton ID="imgPrint" runat="server" ImageUrl="~/printer.png"  
                style="position:fixed;" ToolTip="Print"  />
        </div> 
    </div>

</asp:Content>
