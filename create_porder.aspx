<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="create_porder.aspx.cs" Inherits="gbe.create_porder" Title="Create Purchase Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 887px">
        <tr>
            <td style="width: 147px; height: 23px">
                <span style="font-family: Verdana">Contract No.:<br />
                    <span style="font-size: 10pt"></span>
                </span>
            </td>
            <td style="width: 291px; height: 23px">
                <asp:TextBox ID="txtProject" runat="server" Width="381px"></asp:TextBox></td>
            <td style="width: 267px; height: 23px">
            </td>
        </tr>
        <tr>
            <td style="width: 147px; height: 23px">
                <span style="font-family: Verdana">Delivery date:</span></td>
            <td style="width: 291px; height: 23px">
                <asp:TextBox ID="txtDeliveryDate" runat="server" MaxLength="50" Width="381px"></asp:TextBox></td>
            <td style="width: 267px; height: 23px">
            </td>
        </tr>
        <tr>
            <td style="width: 147px">
                <span style="font-family: Verdana">Spools created by:</span></td>
            <td style="width: 291px">
                <asp:DropDownList ID="dlUser" runat="server" Font-Names="Verdana" Width="108px">
                </asp:DropDownList></td>
            <td style="width: 267px">
                <asp:Button ID="btnCreatePO" runat="server" OnClick="btnCreatePO_Click" Text="Create PO "
                    UseSubmitBehavior="False" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 147px">
            </td>
            <td style="width: 291px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 267px">
            </td>
        </tr>
        <tr>
            <td style="width: 147px">
            </td>
            <td style="width: 291px">
            </td>
            <td style="width: 267px">
            </td>
        </tr>
        <tr>
            <td style="width: 147px">
            </td>
            <td style="width: 291px">
                <asp:Table ID="tblPO" runat="server" Font-Names="Verdana" Width="361px">
                </asp:Table>
            </td>
            <td style="width: 267px">
            </td>
        </tr>
    </table>
</asp:Content>
