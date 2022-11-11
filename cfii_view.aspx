<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="cfii_view.aspx.cs" Inherits="gbe.consignment_view" Title="Consignment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;
    <br />
    
     <script type = "text/javascript">
        function Confirm() 
        {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Delete this line?")) 
            {
                confirm_value.value = "Yes";
            } else 
            {
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
    </script>

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
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
            <td style="width: 190px; height: 21px">
            </td>
            <td colspan="3" style="height: 21px">
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
                <asp:DropDownList ID="dlSearchFlds" runat="server" Width="128px">
                </asp:DropDownList></td>
            <td style="height: 21px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                    Width="94px" /></td>
        </tr>
    </table>
    <table style="width: 1095px">
        <tr>
            <td style="width: 192px">
            </td>
            <td style="width: 34px">
                <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add New" Width="94px" /></td>
            <td style="width: 32px">
                </td>
            <td>
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
            <br />
        </asp:View>
        &nbsp;
        <asp:View ID="View2" runat="server" OnActivate="View2_Activate">
            <table style="width: 1089px">
                <tr>
                    <td style="height: 21px">
                        <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
                    <td style="height: 21px">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="dlparts" runat="server" Width="641px">
                        </asp:DropDownList></td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnAddNew" runat="server" Text="Add" Width="90px" OnClick="btnAddNew_Click" /></td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
    
</asp:Content>
