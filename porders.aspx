<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="porders.aspx.cs" Inherits="gbe.porders1" Title="Purchase Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;<asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="view_porders" runat="server">
    <table style="width: 1096px">
        <tr>
            <td style="width: 189px">
            </td>
            <td colspan="3">
            </td>
        </tr>
        <tr>
            <td style="width: 189px">
            </td>
            <td colspan="3">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="430px"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 189px">
            </td>
            <td colspan="3">
            </td>
        </tr>
        <tr>
            <td style="width: 189px">
                <asp:Label ID="lblSearch" runat="server" Font-Names="Verdana" Text="Order Number:"
                    Width="131px"></asp:Label></td>
            <td style="width: 309px">
                <asp:TextBox ID="txtSearch" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 165px">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 189px; height: 21px">
                <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Text="Supplier:" Width="131px"></asp:Label></td>
            <td style="width: 309px; height: 21px">
                <asp:TextBox ID="txtSupplier" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 165px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 189px; height: 21px">
                <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Text="Date (dd/mm/yyy):"
                    Width="184px"></asp:Label></td>
            <td style="width: 309px; height: 21px">
                <asp:TextBox ID="txtDate" runat="server" Width="288px"></asp:TextBox></td>
            <td style="width: 165px; height: 21px">
                <asp:Button ID="Button1" runat="server" OnClick="btnSearch_Click" Text="Search" Width="94px" /></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 189px; height: 21px">
                </td>
            <td style="width: 309px; height: 21px">
                </td>
            <td style="width: 165px; height: 21px">
            <asp:Button ID="btnActiveReturns" runat="server" OnClick="btnActiveReturns_Click" Text="Get Active Returns" Width="130px" /></td>
            <td style="height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 189px; height: 21px">
                </td>
            <td style="width: 309px; height: 21px">
                </td>
            <td style="width: 165px; height: 21px">
            </td>
            <td style="height: 21px">
            </td>
        </tr>
    </table>
    <asp:Table ID="tblResults" runat="server" Height="9px" Width="373px" Font-Names="Verdana" EnableViewState="False">
    </asp:Table>
        </asp:View>
        <asp:View ID="view_add_part" runat="server">
            <table style="width: 1096px">
                <tr>
                    <td style="width: 190px; height: 21px;">
                    </td>
                    <td style="width: 309px; height: 21px;">
                    </td>
                    <td style="width: 133px; height: 21px;">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 190px">
                    </td>
                    <td colspan="3">
                        <asp:Label ID="lblMsg2" runat="server" Font-Names="Verdana" ForeColor="Red" Width="448px"></asp:Label></td>
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
                        <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
                    <td style="width: 133px; height: 21px">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                    </td>
                    <td style="width: 309px; height: 21px">
                        <asp:TextBox ID="txtPart" runat="server" MaxLength="50" Width="288px"></asp:TextBox></td>
                    <td style="width: 133px; height: 21px">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                    </td>
                    <td style="width: 309px; height: 21px">
                        <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Quantity:"></asp:Label></td>
                    <td style="width: 133px; height: 21px">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                    </td>
                    <td style="width: 309px; height: 21px">
                        <asp:TextBox ID="txtQty" runat="server" MaxLength="8" Width="109px"></asp:TextBox></td>
                    <td style="width: 133px; height: 21px">
                        <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" Width="94px" /></td>
                    <td style="height: 21px">
                    </td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                    </td>
                    <td style="width: 309px; height: 21px">
                    </td>
                    <td style="width: 133px; height: 21px">
                        <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel"
                            Width="94px" /></td>
                    <td style="height: 21px">
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView><br />
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
        
        function ConfirmSendToIMSL() 
        {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Send order to IMSL?")) 
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
