<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="add_to_schedule.aspx.cs" Inherits="gbe.add_to_schedule" Title="Add To Schedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 969px">
        <tr>
            <td style="width: 14px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 14px; height: 21px">
            </td>
            <td style="height: 21px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3" style="height: 21px">
                &nbsp;<asp:Label ID="lblTitle" runat="server" Font-Names="Verdana" Text="Schedule:"></asp:Label></td>
        </tr>
    </table>
    <table style="width: 776px">
        <tr>
            <td style="width: 288px; height: 26px;">
                <asp:Label ID="lblDate" runat="server" Font-Names="Verdana" Text="Fabrication Date (dd/mm/yyyy):" Width="273px"></asp:Label></td>
            <td colspan="2" style="width: 198px; height: 26px;">
                <asp:TextBox ID="txtDate" runat="server" MaxLength="10"></asp:TextBox></td>
                
                <td colspan="1" style="width: 304px; height: 26px" align="right">
                <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" Width="125px" /></td>
                
                <td colspan="1" style="width: 10728px; height: 26px">
            </td>
            <td colspan="1" style="width: 5539px; height: 26px">
                <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" Width="125px" OnClientClick="Confirm()" /></td>
                
            <td colspan="1" style="width: 304px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 288px">
                <asp:Label ID="lblTime" runat="server" Font-Names="Verdana" Text="Time (hh:mm):" Visible="False"></asp:Label></td>
            <td colspan="2" style="width: 198px">
                <asp:TextBox ID="txtTime" runat="server" MaxLength="5" Visible="False"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 288px">
                <asp:Label ID="lblVehicle" runat="server" Font-Names="Verdana" Text="Vehicle:" Visible="False"></asp:Label></td>
            <td colspan="2" style="width: 198px">
                <asp:DropDownList ID="dlVehicle" runat="server" Width="155px" Visible="False">
                </asp:DropDownList></td>
            <td colspan="1" style="width: 304px">
                </td>
        </tr>
        <tr>
            <td style="width: 288px">
            </td>
            <td colspan="2" style="width: 198px">
            </td>
        </tr>
    </table>
    <table style="width: 705px">
        <tr>
            <td colspan="3" style="height: 23px">
                <asp:Table ID="tblSpools" runat="server" Font-Names="Verdana">
                </asp:Table>
            </td>
        </tr>
    </table>
    <br />
    
    <script type = "text/javascript">
    function Confirm() 
    {
        var confirm_value = document.createElement("INPUT");
        confirm_value.type = "hidden";
        confirm_value.name = "confirm_value";
    
        if (confirm("Delete the selected records?")) 
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
