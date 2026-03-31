<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="parts.aspx.cs" Inherits="gbe.parts1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 1096px">
        <tr>
            <td style="width: 190px">
            </td>
            <td style="width: 152px">
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
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 152px; height: 21px">
                &nbsp;</td>
            <td style="width: 133px; height: 21px">
                &nbsp;</td>
            <td style="height: 21px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
                &nbsp;</td>
            <td style="width: 152px; height: 21px">
                <asp:Label ID="lblSearch1" runat="server" Font-Names="Verdana" Text="Material:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
                <asp:DropDownList ID="dlMaterial" runat="server" Width="294px" AutoPostBack="True" OnSelectedIndexChanged="dlMaterial_SelectedIndexChanged">
                </asp:DropDownList></td>
            <td style="height: 21px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 152px; height: 21px">
                <asp:Label ID="lblSearch0" runat="server" Font-Names="Verdana" Text="Part Type:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
                <asp:DropDownList ID="dlPartType" runat="server" Width="294px" AutoPostBack="True" OnSelectedIndexChanged="dlPartType_SelectedIndexChanged">
                </asp:DropDownList></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
                &nbsp;</td>
            <td style="width: 152px; height: 21px">
                &nbsp;</td>
            <td style="width: 133px; height: 21px">
                &nbsp;</td>
            <td style="height: 21px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
                &nbsp;</td>
            <td style="width: 152px; height: 21px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Description:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox>
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" Width="94px" />
            </td>
        </tr>
    </table>
    <br />
                <asp:Table ID="tblResults" runat="server" Font-Names="Verdana" EnableViewState="False">
                </asp:Table>
    



</asp:Content>
