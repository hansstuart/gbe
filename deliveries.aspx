<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="deliveries.aspx.cs" Inherits="gbe.deliveries1" Title="gbe - Deliveries" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 746px">
        <tr>
            <td style="width: 280px">
            </td>
            <td style="width: 9px">
            </td>
            <td style="width: 372px">
            </td>
        </tr>
        <tr>
            <td style="width: 280px; height: 21px">
            </td>
            <td style="width: 9px; height: 21px">
            </td>
            <td style="width: 372px; height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 280px; height: 165px">
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
            <td style="width: 280px">
            </td>
            <td style="width: 9px">
            </td>
            <td style="width: 372px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
        </tr>
    </table>
    <br />
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <br />
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="885px">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
