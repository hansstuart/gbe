<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="cut_lengths.aspx.cs" Inherits="gbe.cut_lengths" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label>
    </div>

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">

        <asp:View ID="View1" runat="server">
            
            <div style="clear: both">&nbsp;</div>
            <div style="clear: both">&nbsp;</div>
            <div style="clear: both">&nbsp;</div>
            
             <div >
                <table >
                <tr>
                    <td style="width: 74px">&nbsp;</td>
                    <td style="width: 309px">
                        <asp:TextBox ID="txtSearch" runat="server" style="margin-left: 0px" Width="288px"></asp:TextBox>
                    </td>
                    <td style="width: 21px">
                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" Width="94px" />
                    </td>
                </tr>
                </table>
            </div>
            
            <div style="clear: both">&nbsp;</div>

            <div style="clear: both">
                <div style="float:left; ">
                    <asp:Table ID="tblMain" runat="server" Font-Names="Verdana" Font-Size="11pt"> </asp:Table>
                </div>

                <div style="float:left; ">&nbsp;</div>

                <div style="float:left; width: 1% ">
                    <asp:ImageButton ID="imgAddToPrint" runat="server" ImageUrl="~/import.png"  
                        style="position:fixed;"  ToolTip="Add to print" OnClick="imgAddToPrint_Click"  />
                </div> 
            </div>

        </asp:View>

        <asp:View ID="View2" runat="server">
            
            <div style="clear: both">&nbsp;</div>
            <div style="clear: both">&nbsp;</div>
            <div style="clear: both">&nbsp;</div>

            <div>
                <div style="float:left; ">
                    <asp:Table ID="tblPrint" runat="server" Font-Names="Verdana" Font-Size="11pt"> </asp:Table>
                </div>

                <div style="float:left; ">&nbsp;</div>

                <div style="float:left; width: 1% ">
                    <asp:ImageButton ID="imgPrint" runat="server" ImageUrl="~/printer.png"  
                        style="position:fixed;" ToolTip="Print" OnClick="imgPrint_Click"  />
                </div> 
            </div>

        </asp:View>

    </asp:MultiView>
</asp:Content>
