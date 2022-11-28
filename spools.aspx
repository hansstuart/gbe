<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="spools.aspx.cs" Inherits="gbe.spools1" Title="gbe - Spools" %>
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
            <td style="width: 190px; height: 21px;">
            </td>
            <td style="width: 309px; height: 21px;">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Search Spools:"></asp:Label></td>
            <td style="width: 133px; height: 21px;">
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
                <asp:DropDownList ID="dlSearchFlds" runat="server" Width="128px">
                </asp:DropDownList></td>
            <td style="height: 21px">
                </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Status:"></asp:Label></td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:DropDownList ID="dlStatus" runat="server" Width="294px">
                </asp:DropDownList></td>
            <td style="width: 133px; height: 21px">
                <asp:CheckBox ID="chkSrchOnHold" runat="server" Font-Names="Verdana" Text="On Hold"
                    TextAlign="Left" /></td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
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
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
            </td>
            <td style="width: 133px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export"
                    Width="94px" /></td>
        </tr>
    </table>
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
            <td style="width: 192px; height: 64px;">
            </td>
            <td colspan="3" style="width: 1040px; height: 64px">
                <br />
                <asp:Table ID="tblMain" runat="server" EnableViewState="False" Font-Names="Verdana"
                    Width="1309px" Font-Size="11pt">
                </asp:Table>
            </td>
        </tr>
    </table>
    <script type = "text/javascript">
        function Confirm() 
        {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Delete this Spool?")) 
            {
                confirm_value.value = "Yes";
            } else 
            {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }

        function ConfirmChecked() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_checked_value";

            if (confirm("Confirm that the spool has been checked.")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
        
        function isNumberKey(evt)
        {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57) )
                return false;
            return true;
        }
    </script>
</asp:Content>
