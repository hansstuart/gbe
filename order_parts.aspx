<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="order_parts.aspx.cs" Inherits="gbe.order_parts" Title="gbe - Order Parts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <script type="text/javascript">
    
      function isNumberKey(evt)
      {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57) )
            return false;
        return true;
       }

        function Confirm() 
        {
        
            var confirm_create_another_value = document.createElement("INPUT");
            confirm_create_another_value.type = "hidden";
            confirm_create_another_value.name = "confirm_create_another_value";
            
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Are you sure you wish to proceed?")) 
            {
                confirm_value.value = "Yes";
                            
                scroll(0,0);
                
            } else 
            {
                confirm_value.value = "No";
            }
            
            document.forms[0].appendChild(confirm_value);

        }
        
      function OnRemove(r)
      {
        var tblPartsID = '<%=tblParts.ClientID %>';
        
        var tblParts = document.getElementById(tblPartsID);
                
        alert(r);        
        tblParts.deleteRow(r);
        
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

    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="view_order_parts" runat="server">
    <table style="width: 753px">
        <tr>
            <td style="width: 172px; height: 21px">
            </td>
            <td style="width: 100px; height: 21px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 99px; height: 21px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <span style="font-family: Verdana">Contract Number:</span></td>
            <td style="width: 100px; height: 21px">
                <asp:TextBox ID="txtContractNumber" runat="server" MaxLength="50" Width="374px"></asp:TextBox></td>
            <td style="width: 99px; height: 21px">
                <asp:Button ID="btnProceed" runat="server" OnClick="btnProceed_Click" Text="Next"
                    UseSubmitBehavior="False" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <asp:Label ID="lblDelAddr" runat="server" Font-Names="Verdana" Text="Delivery Address:"></asp:Label></td>
            <td style="width: 100px; height: 21px">
                <asp:DropDownList ID="dlDelAddr" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlDelAddr_SelectedIndexChanged"
                    Width="383px">
                </asp:DropDownList></td>
            <td style="width: 99px; height: 21px">
            </td>
        </tr>
    </table>
    <table id="tbl_new_delivery_address" runat="server" style="width: 751px; height: 97px">
        <tr>
            <td style="width: 195px; height: 3px">
                <asp:Label ID="lbl_new_deliv_addr_line1" runat="server" Font-Names="Verdana" Text="Address Line 1:"></asp:Label></td>
            <td style="width: 438px; height: 3px">
                <asp:TextBox ID="txt_new_deliv_addr_line1" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 3px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px">
                <asp:Label ID="lbl_new_deliv_addr_line2" runat="server" Font-Names="Verdana" Text="Address Line 2:"></asp:Label></td>
            <td style="width: 438px">
                <asp:TextBox ID="txt_new_deliv_addr_line2" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px">
                <asp:Label ID="lbl_new_deliv_addr_line3" runat="server" Font-Names="Verdana" Text="Address Line 3:"></asp:Label></td>
            <td style="width: 438px">
                <asp:TextBox ID="txt_new_deliv_addr_line3" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px">
                <asp:Label ID="lbl_new_deliv_addr_line4" runat="server" Font-Names="Verdana" Text="Address Line 4:"></asp:Label></td>
            <td style="width: 438px">
                <asp:TextBox ID="txt_new_deliv_addr_line4" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px">
                <asp:Label ID="lbl_new_deliv_addr_name" runat="server" Font-Names="Verdana" Text="Contact Name:"></asp:Label></td>
            <td style="width: 438px">
                <asp:TextBox ID="txt_new_deliv_addr_name" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 10px">
                <asp:Label ID="lbl_new_deliv_addr_phone" runat="server" Font-Names="Verdana" Text="Telephone:"></asp:Label></td>
            <td style="width: 438px; height: 10px">
                <asp:TextBox ID="txt_new_deliv_addr_phone" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 10px">
            </td>
        </tr>
    </table>
    <table id="tbl_spl_and_parts" runat="server" style="width: 909px">
        <tr>
            <td style="width: 736978px; height: 26px">
            </td>
            <td style="width: 438px; height: 26px">
                </td>
            <td style="width: 106px; color: #000000; height: 26px">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" OnClientClick="Confirm() "
                    Text="Create Order" UseSubmitBehavior="False" Width="93px" /></td>
            <td style="width: 292419px; color: #000000; height: 26px">
            </td>
        </tr>
        <tr style="font-size: 12pt">
            <td style="width: 736978px; height: 26px">
                <asp:Label ID="lblPartNo" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtPartNo" runat="server" MaxLength="150" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" Width="91px" /></td>
            <td style="width: 292419px; height: 26px">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search..."
                    Width="91px" /></td>
        </tr>
        <tr style="font-size: 12pt">
            <td style="width: 736978px">
            </td>
            <td style="width: 438px">
                <asp:Label ID="lblSelected" runat="server" Font-Bold="False" Font-Names="Verdana"
                    ForeColor="Black" Text="Selected Parts:" Width="173px"></asp:Label></td>
            <td style="width: 106px">
            </td>
            <td style="width: 292419px">
            </td>
        </tr>
        <tr style="font-size: 12pt">
            <td style="width: 736978px">
            </td>
            <td colspan="2" style="width: 550px">
                <asp:Table ID="tblParts" runat="server" CellPadding="2" CellSpacing="2" EnableViewState="False"
                    Font-Names="Verdana" Width="543px">
                </asp:Table>
            </td>
            <td colspan="1" style="width: 292419px">
            </td>
        </tr>
    </table>
            <br />
        </asp:View>
        <asp:View ID="view_search_parts" runat="server">
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
                        <asp:Label ID="lblMsg2" runat="server" Font-Names="Verdana" ForeColor="Red" Width="499px"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                        <asp:LinkButton ID="lnkPrevPage" runat="server" Font-Names="Verdana" Font-Size="Smaller"
                            OnClick="lnkPrevPage_Click">Return to purchase order</asp:LinkButton></td>
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
                        <asp:DropDownList ID="dlSearchFlds" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlSearchFlds_SelectedIndexChanged"
                            Width="128px">
                        </asp:DropDownList></td>
                    <td style="height: 21px">
                        <asp:Button ID="btnSearchParts" runat="server" OnClick="btnSearchParts_Click" Text="Search"
                            Width="94px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px; height: 21px">
                    </td>
                    <td style="width: 309px; height: 21px">
                        <asp:DropDownList ID="dlPartType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlPartType_SelectedIndexChanged"
                            Visible="False" Width="294px">
                        </asp:DropDownList></td>
                    <td style="width: 133px; height: 21px">
                    </td>
                    <td style="height: 21px">
                    </td>
                </tr>
            </table>
            <table style="width: 1094px">
            <tr>
                <td style="width: 196px">
                </td>
                <td>
                    </td>
            </tr>
                <tr>
                    <td style="width: 196px; height: 16px">
                    </td>
                    <td style="height: 16px">
                    <asp:Table id="tblResults" runat="server" Font-Names="Verdana" Width="890px">
                    </asp:Table>
                    </td>
                </tr>
        </table>
        </asp:View>
        
    </asp:MultiView>
    
</asp:Content>
