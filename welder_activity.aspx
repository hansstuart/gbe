<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="welder_activity.aspx.cs" Inherits="gbe.welder_activity" Title="gbe - Welder Activity" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 1096px">
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 223px">
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
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="440px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 223px">
                &nbsp;</td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 223px">
                <asp:Label ID="lblW" runat="server" Font-Names="Verdana" Text="Welders:"></asp:Label><br />
                <asp:DropDownList ID="dlWelders" runat="server" Width="190px">
                </asp:DropDownList></td>
            <td style="width: 133px">
                </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 223px">
                <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Text="Spool/Project:"></asp:Label></td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 223px">
                <asp:TextBox ID="txtSpool" runat="server" Width="215px"></asp:TextBox></td>
            <td style="width: 133px">
                <asp:DropDownList ID="dlSearchFlds" runat="server" Width="128px" Visible="False">
                </asp:DropDownList></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="From:"></asp:Label><br />
                <asp:Calendar ID="dtFrom" runat="server" Height="112px" Font-Names="Verdana" Font-Size="8pt" Width="216px"></asp:Calendar>
            </td>
            <td style="width: 133px; height: 21px">
                &nbsp;<asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="To:"></asp:Label><br />
                <asp:Calendar ID="dtTo" runat="server" Height="112px" Font-Names="Verdana" Font-Size="8pt" Width="216px"></asp:Calendar>
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
                </td>
            <td style="width: 133px; height: 21px">
                &nbsp;</td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
            </td>
            <td style="width: 133px; height: 21px" align="right">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
            </td>
            <td align="right" style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
            </td>
            <td align="right" style="width: 133px; height: 21px">
                <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export"
                    Width="94px" /></td>
            <td style="height: 21px">
            </td>
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
