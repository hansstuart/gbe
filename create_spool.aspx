<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="create_spool.aspx.cs" Inherits="gbe.create_spool" Title="gbe - Create Spool" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:FileUpload ID="FileUpload1" runat="server" onchange="get_filename()" style="display: none;"/> 

    <table style="width: 753px">
        <tr>
            <td style="font-family: Verdana">
            </td>
            <td style="width: 100px">
                <asp:HyperLink ID="hlSpoolsView" runat="server" Font-Names="Verdana" Width="384px" NavigateUrl="~/spools.aspx">Return to Spools View</asp:HyperLink></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                Contract Number:</td>
            <td style="width: 100px">
                <asp:TextBox ID="txtContractNumber" runat="server" Width="381px" MaxLength="50"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana; height: 24px;">
                Cost Centre:</td>
            <td style="width: 100px; height: 24px;">
                <asp:DropDownList ID="dlCostCentre" runat="server" Width="263px">
                </asp:DropDownList></td>
            <td style="width: 99px; height: 24px;">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                IMSL Cost Centre:</td>
            <td style="width: 100px">
                <asp:DropDownList ID="dlIMSLCostCentre" runat="server" Width="263px">
                </asp:DropDownList></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <span style="font-family: Verdana"></span></td>
            <td style="width: 100px; height: 21px">
                </td>
            <td style="width: 99px; height: 21px">
                <asp:Button ID="btnProceed" runat="server" Text="Next" OnClick="btnProceed_Click" UseSubmitBehavior="False" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 172px">
            </td>
            <td style="width: 100px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblCustomer" runat="server" Font-Names="Verdana" Text="Customer:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtCustomer" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 26px;">
                <asp:Label ID="lblAddr1" runat="server" Font-Names="Verdana" Text="Address Line 1:"></asp:Label></td>
            <td style="width: 100px; height: 26px;">
                <asp:TextBox ID="txtAddr1" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px; height: 26px;">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr2" runat="server" Font-Names="Verdana" Text="Address Line 2:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtAddr2" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr3" runat="server" Font-Names="Verdana" Text="Address Line 3:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtAddr3" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr4" runat="server" Font-Names="Verdana" Text="Address Line 4:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtAddr4" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblContactName" runat="server" Font-Names="Verdana" Text="Contact Name:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtContactName" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblPhone" runat="server" Font-Names="Verdana" Text="Telephone:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblEmail" runat="server" Font-Names="Verdana" Text="Email Address:"></asp:Label></td>
            <td style="width: 100px">
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
            </td>
            <td style="width: 100px; height: 21px">
                </td>
            <td style="width: 99px; height: 21px">
                </td>
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
    <table id="tbl_new_delivery_address" runat="server" style="width: 751px; height: 97px;">
        <tr>
            <td style="width: 195px ; height: 3px;">
                <asp:Label ID="lbl_new_deliv_addr_line1" runat="server" Font-Names="Verdana" Text="Address Line 1:"></asp:Label></td>
            <td style="width: 438px ; height: 3px;">
                <asp:TextBox ID="txt_new_deliv_addr_line1" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 3px;">
            </td>
        </tr>
        <tr>
            <td style="width: 195px;">
                <asp:Label ID="lbl_new_deliv_addr_line2" runat="server" Font-Names="Verdana" Text="Address Line 2:"></asp:Label></td>
            <td style="width: 438px;">
                <asp:TextBox ID="txt_new_deliv_addr_line2" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px;">
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
            <td style="width: 195px; height: 10px;">
                <asp:Label ID="lbl_new_deliv_addr_phone" runat="server" Font-Names="Verdana" Text="Telephone:"></asp:Label></td>
            <td style="width: 438px; height: 10px;">
                <asp:TextBox ID="txt_new_deliv_addr_phone" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 10px;">
            </td>
        </tr>
    </table>
    <br />
    <table id="tbl_spl_and_parts"  runat="server" style="width: 751px">
        <tr>
            <td style="width: 195px; height: 26px">
            </td>
            <td style="width: 438px; height: 26px">
                <asp:Label ID="lblMsg2" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Spool Number:<br />
                    <span style="font-size: 10pt">(excluding Contract number)</span></span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtSpoolNumber" runat="server" Width="381px" MaxLength="50"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Revision:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtRevision" runat="server" Width="70px" MaxLength="3"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Weld Mapping:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:DropDownList ID="dlWeldMapping" runat="server">
                </asp:DropDownList></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Material:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:DropDownList ID="dlMaterial" runat="server" Width="215px">
                </asp:DropDownList></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Pipe Size:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtPipeSize" runat="server" MaxLength="20" Width="206px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Cut Size:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtCutSize1" runat="server" MaxLength="20" Width="206px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Cut Size:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtCutSize2" runat="server" MaxLength="20" Width="206px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Cut Size:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtCutSize3" runat="server" MaxLength="20" Width="206px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Cut Size:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtCutSize4" runat="server" MaxLength="20" Width="206px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana">Drawing:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtDrawing" runat="server" MaxLength="500" Width="381px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnBrowse" runat="server" Text="Browse..."
                    UseSubmitBehavior="False" Width="93px" />
            
                         
            </td>
                  
        </tr>
        <tr>
            <td style="width: 195px; height: 26px">
                <span style="font-family: Verdana"></span>
            </td>
            <td style="width: 438px; height: 26px">
                </td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save Spool"
                    UseSubmitBehavior="False" Width="93px" OnClientClick="Confirm() " /></td>
        </tr>
        <tr>
            <td style="width: 195px; height: 26px;">
                <asp:Label ID="lblPartNo" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
            <td style="width: 438px; height: 26px;">
                <asp:TextBox ID="txtPartNo" runat="server" MaxLength="150" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px;">
                <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" Width="91px" /></td>
        </tr>
    </table>

    <table>
        <tr>
            <td style="width: 195px">
            </td>
            <td style="width: 438px">
                <asp:Label ID="lblSelected" runat="server" Font-Names="Verdana" Text="Selected Parts:"
                    Width="173px" ForeColor="Black" Font-Bold="False"></asp:Label></td>
            <td style="width: 106px">
            </td>
        </tr>

        <tr>
            <td style="width: 195px">
            </td>
            <td >
                <asp:Table ID="tblParts" runat="server" Font-Names="Verdana" CellPadding="2" CellSpacing="2" Width="745px" EnableViewState="False">
                </asp:Table>
            </td>
        </tr>
    </table>
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
        
            if (confirm("Save spool?")) 
            {
                confirm_value.value = "Yes";
                            
                if (confirm("Create another spool like this one?")) 
                {
                    confirm_create_another_value.value = "Yes";
                }
                else
                {
                    confirm_create_another_value.value = "No";
                }
                
                scroll(0,0);
                
            } else 
            {
                confirm_value.value = "No";
            }
            
            document.forms[0].appendChild(confirm_value);
            document.forms[0].appendChild(confirm_create_another_value);
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
     
     function get_filename() 
     {
        document.getElementById('<%=txtDrawing.ClientID%>').value = document.getElementById('<%=FileUpload1.ClientID%>').value.split(/(\\|\/)/g).pop();
     }
     
      </script>
</asp:Content>
