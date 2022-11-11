<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="invoicing.aspx.cs" Inherits="gbe.invoicing" Title="Invoicing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;<table style="width: 1096px">
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
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
                &nbsp;</td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
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
                </td>
            <td>
            </td>
        </tr>
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
            <td style="width: 223px">
            </td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="From:"></asp:Label><br />
                <asp:Calendar ID="dtFrom" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px"></asp:Calendar>
            </td>
            <td style="width: 133px; height: 21px">
                &nbsp;<asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="To:"></asp:Label><br />
                <asp:Calendar ID="dtTo" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px"></asp:Calendar>
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 20px">
            </td>
            <td style="width: 223px; height: 20px" valign="middle">
                <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Text="Email Address:"></asp:Label></td>
            <td style="width: 133px; height: 20px" valign="middle">
                &nbsp;<asp:TextBox ID="txtEmailAddress" runat="server" Width="208px"></asp:TextBox></td>
            <td style="height: 20px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 223px; height: 21px">
            </td>
            <td align="right" style="width: 133px; height: 21px">
                <asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" Text="Send Invoice"
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
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" Visible="False" /></td>
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
                <asp:CheckBox ID="chkExportSummary" runat="server" Font-Names="Verdana" Text="Export Summary" Visible="False" /></td>
            <td align="left" style="width: 133px; height: 21px">
                <asp:CheckBox ID="chkExportBreakdown" runat="server" Font-Names="Verdana" Text="Export Breakdown" Visible="False" /></td>
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
                    Width="94px" Visible="False" /></td>
            <td style="height: 21px">
            </td>
        </tr>
    </table>
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <br />
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="885px" Visible="False">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
