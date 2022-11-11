<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="stock_movement.aspx.cs" Inherits="gbe.stock_movement" Title="Stock Movement Audit Trail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;<table style="width: 818px">
        <tr>
            <td style="width: 64px">
            </td>
            <td style="width: 295px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Search:"></asp:Label></td>
            <td style="width: 96px">
            </td>
            <td style="width: 285px">
            </td>
        </tr>
        <tr>
            <td style="width: 64px">
            </td>
            <td style="width: 295px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 96px">
                <asp:DropDownList ID="dlSearchFlds" runat="server" Width="128px">
                </asp:DropDownList></td>
            <td style="width: 285px">
            </td>
        </tr>
        <tr>
            <td style="width: 64px">
            </td>
            <td style="width: 295px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Date:"></asp:Label></td>
            <td style="width: 96px">
            </td>
            <td style="width: 285px">
            </td>
        </tr>
        <tr>
            <td style="width: 64px; height: 141px">
            </td>
            <td style="width: 295px; height: 141px">
                <asp:Calendar ID="dtFrom" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px"></asp:Calendar>
            </td>
            <td style="width: 96px; height: 141px" valign="bottom">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
            <td style="width: 285px; height: 141px">
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 602px">
        <tr>
            <td style="width: 56px; height: 21px">
            </td>
            <td style="width: 140px; height: 21px">
            </td>
            <td style="width: 13px; height: 21px">
            </td>
            <td style="width: 125px; height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 56px; height: 21px">
            </td>
            <td align="right" style="width: 140px; height: 21px">
                <asp:ImageButton ID="btnFirstPage" runat="server" ImageUrl="~/first.png" OnClick="btnFirstPage_Click"
                    ToolTip="First page" />
                <asp:ImageButton ID="btnPreviousPage" runat="server" ImageUrl="~/back.png" OnClick="btnPreviousPage_Click"
                    ToolTip="Previous page" /></td>
            <td align="center" style="width: 13px; height: 21px">
                <asp:Label ID="lblPage" runat="server" Font-Names="Verdana" Font-Size="Smaller" Width="183px"></asp:Label></td>
            <td align="left" style="width: 125px; height: 21px">
                &nbsp;<asp:ImageButton ID="btnNextPage" runat="server" ImageUrl="~/forward.png" OnClick="btnNextPage_Click"
                    ToolTip="Next page" />
                <asp:ImageButton ID="btnLastPage" runat="server" ImageUrl="~/last.png" OnClick="btnLastPage_Click"
                    ToolTip="Last page" /></td>
        </tr>
        <tr>
            <td style="width: 56px">
            </td>
            <td style="width: 140px">
            </td>
            <td style="width: 13px">
            </td>
            <td style="width: 125px">
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 1095px">
        <tr>
            <td style="width: 67px">
            </td>
            <td colspan="3">
                <br />
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="885px">
                </asp:Table>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
