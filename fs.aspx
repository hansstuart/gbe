<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="fs.aspx.cs" Inherits="gbe.fs" Title="Fitter Schedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px; height: 64px">
            </td>
            <td colspan="3" style="width: 1040px; height: 64px">
                 <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 192px; height: 64px">
            </td>
            <td colspan="3" style="width: 1040px; height: 64px">
                <br />
                <asp:Table ID="tblMain" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Font-Size="11pt" Width="1309px">
                </asp:Table>
            </td>
        </tr>
    </table>
</asp:Content>
