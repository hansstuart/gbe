<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="weld_mapping.aspx.cs" Inherits="gbe.weld_mapping" Title="Weld Mapping" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 1096px">
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 309px">
            </td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td colspan="3">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="499px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 309px">
                &nbsp;</td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Project:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Welder (optional):" Visible="False"></asp:Label></td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:DropDownList ID="dlWelder" runat="server" Width="294px" Visible="False">
                </asp:DropDownList></td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnExportSummary" runat="server" Text="Export Summary" Width="131px" OnClick="btnExportSummary_Click" /></td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
            </td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnExport" runat="server" Text="Export Breakdown" Width="131px" OnClick="btnExport_Click" /></td>
        </tr>
    </table><table style="width: 1239px">
        <tr>
            <td style="width: 192px">
                <asp:Label ID="lblSummary" runat="server" Font-Bold="True" Font-Names="Verdana" Text="Summary"
                    Width="177px"></asp:Label></td>
            <td colspan="3">
            </td>
        </tr>
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <asp:Table ID="tblSummary" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="1032px">
                </asp:Table>
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 1239px">
        <tr>
            <td style="width: 192px">
                <asp:Label ID="lblBreakdown" runat="server" Font-Bold="True" Font-Names="Verdana"
                    Text="Breakdown" Width="177px"></asp:Label></td>
            <td colspan="3">
            </td>
        </tr>
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="1032px">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
