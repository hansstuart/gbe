<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="totals.aspx.cs" Inherits="gbe.totals" Title="Totals" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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
