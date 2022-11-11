<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="cust_delivery.aspx.cs" Inherits="gbe.cust_delivery" Title="Create Delivery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    &nbsp;
    
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
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Submit Delivery?")) 
            {
                confirm_value.value = "Yes";
            } else 
            {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
        
        function DisableBackButton() 
        {
            window.history.forward()
        }
     DisableBackButton();
     window.onload = DisableBackButton;
     window.onpageshow = function(evt) { if (evt.persisted) DisableBackButton() }
     window.onunload = function() { void (0) }
     
   </script>

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="View1" runat="server">
            <table style="width: 928px">
        <tr>
            <td style="width: 440px">
            </td>
            <td style="width: 9611px">
            </td>
            <td style="width: 2225px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 440px">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Delivery date:"></asp:Label><asp:Calendar
                    ID="dtDelivery" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="112px"
                    Width="216px">
                    <SelectedDayStyle BorderStyle="Solid" />
                </asp:Calendar>
            </td>
            <td style="width: 9611px">
            </td>
            <td style="width: 2225px">
            </td>
            <td style="width: 352px">
            </td>
        </tr>
                <tr>
                    <td align="left" style="width: 440px">
                    </td>
                    <td style="width: 9611px">
                    </td>
                    <td style="width: 2225px">
                    </td>
                    <td style="width: 352px">
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 440px">
                        <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Text="Project (G00xxx):"></asp:Label></td>
                    <td style="width: 9611px">
                    </td>
                    <td style="width: 2225px">
                    </td>
                    <td style="width: 352px">
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 440px">
                        <asp:TextBox ID="txtProject" runat="server" MaxLength="6"></asp:TextBox></td>
                    <td style="width: 9611px">
                    </td>
                    <td style="width: 2225px">
                    </td>
                    <td style="width: 352px">
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 440px">
                    </td>
                    <td style="width: 9611px">
                    </td>
                    <td style="width: 2225px">
                    </td>
                    <td style="width: 352px">
                    </td>
                </tr>
        <tr>
            <td align="right" style="width: 440px">
                <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit Delivery" OnClientClick="Confirm()" UseSubmitBehavior="False" /></td>
            <td style="width: 9611px">
            </td>
            <td style="width: 2225px">
            </td>
            <td style="width: 352px">
            </td>
        </tr>
        <tr>
            <td align="left" style="width: 440px" valign="top">
                <asp:Label ID="Label3" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
            <td style="width: 9611px">
            </td>
            <td align="left" style="width: 2225px" valign="top">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 440px; height: 45px;" valign="top">
                <asp:TextBox ID="txtPartNo" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 9611px; height: 45px;" valign="top">
                <asp:Button ID="btnAll" runat="server" OnClick="btnAdd_Click" Text="Add >>" ToolTip="Select part for delivery" />
            </td>
            <td style="width: 2225px; height: 45px;" valign="top">
                <asp:Table ID="tblSelected" runat="server" Font-Names="Verdana" Width="522px">
                </asp:Table>
            </td>
        </tr>
    </table>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <table style="width: 773px">
                <tr>
                    <td style="width: 369px">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td style="width: 369px">
                        <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="Delivery submitted"></asp:Label></td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 369px; height: 26px">
                        <asp:Label ID="lblRef" runat="server" Font-Names="Verdana" Width="351px"></asp:Label></td>
                    <td align="right" style="height: 26px">
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 369px; height: 26px">
                        <asp:HyperLink ID="hlDelNote" runat="server" Font-Names="Verdana" Target="_blank">View Delivery Note</asp:HyperLink></td>
                    <td align="right" style="height: 26px">
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 369px; height: 26px">
                        <asp:Button ID="btnOK" runat="server" OnClick="btnOK_Click" Text="OK" Width="72px" UseSubmitBehavior="False" /></td>
                    <td align="right" style="height: 26px">
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Content>
