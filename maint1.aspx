<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="maint1.aspx.cs" Inherits="gbe.maint1" Title="gbe - maint" %>
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
            <td style="width: 190px; height: 21px;">
            </td>
            <td style="width: 309px; height: 21px;">
                &nbsp;</td>
            <td style="width: 133px; height: 21px;">
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
                <asp:DropDownList ID="dlSearchFlds" runat="server" Width="128px" AutoPostBack="True" OnSelectedIndexChanged="dlSearchFlds_SelectedIndexChanged">
                </asp:DropDownList></td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 190px; height: 21px">
            </td>
            <td style="width: 309px; height: 21px">
                <asp:DropDownList ID="dlPartType" runat="server" Visible="False" Width="294px">
                </asp:DropDownList></td>
            <td style="width: 133px; height: 21px">
                <asp:DropDownList ID="dlActive" runat="server" Width="128px">
                </asp:DropDownList></td>
            <td style="height: 21px">
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
        
      function isDecimal(evt)
      {
        var charCode = (evt.which) ? evt.which : event.keyCode
      
        if(charCode == 46 && evt.srcElement.value.indexOf('.') > -1)
          return false;
      
        if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46 )
            return false;
      
        return true;
       }
       
    </script>

    <table style="width: 1095px">
        <tr>
            <td style="width: 192px">
            </td>
            <td style="width: 34px">
                <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" OnClientClick="Confirm()"
                    Text="Delete" Width="94px" /></td>
            <td style="width: 32px">
                <asp:Button ID="btnAdd" runat="server" Text="Add New" Width="94px" OnClick="btnAdd_Click" /></td>
            <td>
                <asp:Button ID="btnSave" runat="server" Text="Save" Width="94px" OnClick="btnSave_Click" /></td>
        </tr>
    </table>
    <table style="width: 602px">
        <tr>
            <td style="width: 56px; height: 21px">
            </td>
            <td style="width: 140px; height: 21px;">
            </td>
            <td style="width: 13px; height: 21px;">
            </td>
            <td style="width: 125px; height: 21px;">
            </td>
        </tr>
        <tr>
            <td style="width: 56px; height: 21px">
            </td>
            <td style="width: 140px; height: 21px" align="right">
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
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px">
            </td>
            <td colspan="3">
                <br />
                &nbsp;</td>
        </tr>
    </table>
                <asp:Table ID="tblResults" runat="server" Font-Names="Verdana" EnableViewState="False">
                </asp:Table>
    <br />
    
</asp:Content>
