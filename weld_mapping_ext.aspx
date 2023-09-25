<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="weld_mapping_ext.aspx.cs" Inherits="gbe.weld_mapping_ext" %>
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
                    Width="90px" /></td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                &nbsp;</td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                &nbsp;</td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnExport" runat="server" Text="Export" Width="90px" OnClick="btnExport_Click" /></td>
        </tr>
        
    </table><table style="width: 1239px">
        <tr>
            <td style="width: 192px">
            </td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <table style="width: 1239px">
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <asp:Table ID="tblResults" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="2048px">
                </asp:Table>
            </td>
        </tr>
    </table>


    <script type="text/javascript">

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>
