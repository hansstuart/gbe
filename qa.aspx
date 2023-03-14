<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="qa.aspx.cs" Inherits="gbe.qa" Title="gbe - QA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 603px">
        <tr>
            <td style="width: 60437px; height: 21px;">
            </td>
            <td style="width: 40002px; height: 21px;">
            </td>
            <td colspan="4" style="width: 1015px; height: 21px;">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="360px"></asp:Label></td>
        </tr>
    </table>
    <table style="width: 746px">
        <tr>
            <td style="width: 181px">
            </td>
            <td style="width: 9px">
            </td>
            <td style="width: 372px">
            </td>
        </tr>
        <tr>
            <td style="width: 181px; height: 21px;">
            </td>
            <td style="width: 9px; height: 21px;">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Project:"></asp:Label></td>
            <td style="width: 372px; height: 21px;">
            </td>
        </tr>
        <tr>
            <td style="width: 181px; height: 26px;">
            </td>
            <td style="width: 9px; height: 26px;">
                <asp:TextBox ID="txtSearch" runat="server" Width="250px"></asp:TextBox></td>
            <td style="width: 372px; height: 26px;">
                </td>
        </tr>
        <tr>
            <td style="width: 181px; height: 21px">
            </td>
            <td style="width: 9px; height: 21px">
            </td>
            <td style="width: 372px; height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 181px; height: 165px">
            </td>
            <td style="width: 9px; height: 165px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="From:"></asp:Label><asp:Calendar
                    ID="dtFrom" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px"></asp:Calendar>
            </td>
            <td style="width: 372px; height: 165px">
                <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="To:"></asp:Label><asp:Calendar
                    ID="dtTo" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px"></asp:Calendar>
            </td>
        </tr>
        <tr>
            <td style="width: 181px">
            </td>
            <td style="width: 9px">
            </td>
            <td style="width: 372px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
        </tr>
    </table>
    <br />
    <table style="width: 1243px">
        <tr>
            <td style="width: 141px">
            </td>
            <td colspan="3">
                <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Text="Sort:"></asp:Label><br />
                <asp:DropDownList ID="dlSort" runat="server" 
                    Width="205px" AutoPostBack="True">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td style="width: 141px">
            </td>
            <td colspan="3">
                <br />
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="1078px">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
