<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="modules.aspx.cs" Inherits="gbe.modules1" Title="Modules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <table style="width: 587px">
        <tr>
            <td style="width: 853px">
            </td>
            <td colspan="2" style="width: 98px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="386px"></asp:Label></td>
        </tr>
    </table>
    <br />
    <table style="width: 1096px">
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Search Modules:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
                &nbsp;</td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 133px; height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
            </td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px; height: 64px">
            </td>
            <td colspan="3" style="width: 1040px; height: 64px">
                <br />
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="1309px">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
