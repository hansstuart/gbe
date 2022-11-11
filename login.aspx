<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="gbe.login" Title="Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 1230px">
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
                <asp:Label ID="lblUser" runat="server" Font-Names="Verdana" Text="User ID:"></asp:Label></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
                <asp:TextBox ID="txtUser" runat="server" Width="282px"></asp:TextBox></td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px">
            </td>
            <td style="width: 659px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 371px; height: 21px">
            </td>
            <td style="width: 659px; height: 21px">
                <asp:Label ID="lblPW" runat="server" Font-Names="Verdana" Text="Password:"></asp:Label></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 371px; height: 21px">
            </td>
            <td style="width: 659px; height: 21px">
                <asp:TextBox ID="txtPW" runat="server" TextMode="Password" Width="282px"></asp:TextBox></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 371px; height: 21px">
            </td>
            <td style="width: 659px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 371px; height: 21px">
            </td>
            <td style="width: 659px; height: 21px">
                <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" Width="94px" /></td>
            <td style="height: 21px">
            </td>
        </tr>
    </table>
</asp:Content>
