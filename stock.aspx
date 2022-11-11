<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="stock.aspx.cs" Inherits="gbe.stock" Title="gbe - Stock" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 1096px">
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
            <td style="width: 190px">
            </td>
            <td colspan="3">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="499px"></asp:Label></td>
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
            <td style="width: 190px">
            </td>
            <td style="width: 309px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Search:"></asp:Label></td>
            <td style="width: 133px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 133px; height: 21px">
                <asp:DropDownList ID="dlSearchFlds" runat="server" AutoPostBack="True"
                    Width="128px">
                </asp:DropDownList></td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
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
    <script type="text/javascript">
    function onlyDotsAndNumbers(txt, event) 
      {
        var charCode = (event.which) ? event.which : event.keyCode
        
        if (charCode == 46) 
        {
            if (txt.value.indexOf(".") < 0)
                return true;
            else
                return false;
        }

        if (txt.value.indexOf(".") > 0) 
        {
            var txtlen = txt.value.length;
            var dotpos = txt.value.indexOf(".");
            if ((txtlen - dotpos) > 2)
                return false;
        }

        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
     }
     
        function Confirm() 
        {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Delete?")) 
            {
                confirm_value.value = "Yes";
            } else 
            {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
      </script>
</asp:Content>
