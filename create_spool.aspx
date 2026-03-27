<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="create_spool.aspx.cs" Inherits="gbe.create_spool" Title="gbe - Create Spool" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.js" type="text/javascript"></script>  

    <script type="text/javascript"> 

        function calcCutLength(id) {

            var len = (document.getElementById("ContentPlaceHolder1_qty_" + id).value);
            var f1 = (document.getElementById("ContentPlaceHolder1_f1_" + id).value);
            var f2 = (document.getElementById("ContentPlaceHolder1_f2_" + id).value);

            $.ajax(
                {
                    type: "POST",
                    url: "create_spool.aspx/CalcCutLength",
                    data: "{ len: '" + len + "', f1: '" + f1 + "', f2: '" + f2 + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {

                        $('#' + "ContentPlaceHolder1_cut_" + id).text(msg.d);
                    },
                    failure: function (response) {
                        alert('Failure in calcCutLength');
                    },
                    error: function (response) {
                        alert('Error in calcCutLength');
                    }
                });
        }
    </script> 

    <asp:FileUpload ID="FileUpload1" runat="server" onchange="get_filename_drawing()" style="display: none;"/> 
      

    <table style="width: 753px">
        <tr>
            <td style="font-family: Verdana">
            </td>
            <td align="center" colspan="2">
                <asp:HyperLink ID="hlSpoolsView" runat="server" Font-Names="Verdana" Width="384px" NavigateUrl="~/spools.aspx">Return to Spools View</asp:HyperLink></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                &nbsp;</td>
            <td style="width: 10px">
                <asp:HyperLink ID="hlPrevSpool" runat="server" Font-Names="Verdana" Width="174px" NavigateUrl="~/spools.aspx">Previous Spool</asp:HyperLink>
                </td>
            <td style="width: 64px" align="right">
                <asp:HyperLink ID="hlNextSpool" runat="server" Font-Names="Verdana" Width="152px" NavigateUrl="~/spools.aspx">Next Spool</asp:HyperLink></td>
            <td style="width: 99px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                &nbsp;</td>
            <td style="width: 10px">
                &nbsp;</td>
            <td style="width: 64px" align="right">
                &nbsp;</td>
            <td style="width: 99px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                Contract Number:</td>
            <td colspan="2">
                <asp:TextBox ID="txtContractNumber" runat="server" Width="255px" MaxLength="50"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana; height: 24px;">
                Cost Centre:</td>
            <td style="height: 24px;" colspan="2">
                <asp:DropDownList ID="dlCostCentre" runat="server" Width="263px">
                </asp:DropDownList></td>
            <td style="width: 99px; height: 24px;">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                IMSL Cost Centre:</td>
            <td colspan="2">
                <asp:DropDownList ID="dlIMSLCostCentre" runat="server" Width="263px">
                </asp:DropDownList></td>
            <td style="width: 99px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <span style="font-family: Verdana"></span></td>
            <td style="height: 21px" colspan="2">
                </td>
            <td style="width: 99px; height: 21px">
                <asp:Button ID="btnProceed" runat="server" Text="Next" OnClick="btnProceed_Click" UseSubmitBehavior="False" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 172px">
            </td>
            <td colspan="2">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblCustomer" runat="server" Font-Names="Verdana" Text="Customer:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtCustomer" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 26px;">
                <asp:Label ID="lblAddr1" runat="server" Font-Names="Verdana" Text="Address Line 1:"></asp:Label></td>
            <td style="height: 26px;" colspan="2">
                <asp:TextBox ID="txtAddr1" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px; height: 26px;">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr2" runat="server" Font-Names="Verdana" Text="Address Line 2:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtAddr2" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr3" runat="server" Font-Names="Verdana" Text="Address Line 3:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtAddr3" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblAddr4" runat="server" Font-Names="Verdana" Text="Address Line 4:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtAddr4" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblContactName" runat="server" Font-Names="Verdana" Text="Contact Name:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtContactName" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblPhone" runat="server" Font-Names="Verdana" Text="Telephone:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px">
                <asp:Label ID="lblEmail" runat="server" Font-Names="Verdana" Text="Primary Email:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="379px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px" valign="top">
                <asp:Label ID="lblEmail2" runat="server" Font-Names="Verdana" Text="Additional Emails:"></asp:Label></td>
            <td colspan="2">
                <asp:TextBox ID="txtEmail2" runat="server" Width="379px" Height="48px" TextMode="MultiLine"></asp:TextBox></td>
            <td style="width: 99px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
            </td>
            <td style="height: 21px" colspan="2">
                </td>
            <td style="width: 99px; height: 21px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <asp:Label ID="lblDelAddr" runat="server" Font-Names="Verdana" Text="Delivery Address:"></asp:Label></td>
            <td style="height: 21px" colspan="2">
                <asp:DropDownList ID="dlDelAddr" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dlDelAddr_SelectedIndexChanged"
                    Width="385px">
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
            <td style="width: 195px; height: 26px;">
                <asp:Label ID="lbl_new_deliv_addr_line2" runat="server" Font-Names="Verdana" Text="Address Line 2:"></asp:Label></td>
            <td style="width: 438px; height: 26px;">
                <asp:TextBox ID="txt_new_deliv_addr_line2" runat="server" MaxLength="50" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px;">
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

    <table id="tbl_spl_and_parts"  runat="server" style="width: 809px">
        <tr>
            <td style="width: 241px; height: 26px">
                <asp:Label ID="lblFabNumber" runat="server" Font-Names="Verdana" Text="Fab. Number:"></asp:Label>
            </td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtFabNumber" runat="server" MaxLength="150" Width="175px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnFabNumberLookup" runat="server" Text="Look-up" Width="91px" OnClick="btnFabNumberLookup_Click" />
            </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 35px">
                <asp:Label ID="lblFabPO" runat="server" Font-Names="Verdana" Text="Fab. PO:"></asp:Label>
            </td>
            <td style="width: 438px; height: 35px">
                <asp:DropDownList ID="dlFabPO" runat="server" Height="22px" Width="385px">
                </asp:DropDownList>
            </td>
            <td style="width: 106px; height: 35px">
                </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                &nbsp;</td>
            <td style="width: 438px; height: 26px">
                <asp:Label ID="lblFabDetails" runat="server" Font-Names="Verdana" ></asp:Label></td>
            <td style="width: 106px; height: 26px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                &nbsp;</td>
            <td style="width: 438px; height: 26px">
                <asp:Label ID="lblMsg2" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 106px; height: 26px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Spool Number:<br />
                    <span style="font-size: 10pt">(excluding Contract number)</span></span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtSpoolNumber" runat="server" Width="381px" MaxLength="50"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Revision:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtRevision" runat="server" Width="70px" MaxLength="3"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Status:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:DropDownList ID="dlStatus" runat="server">
                </asp:DropDownList></td>
            <td style="width: 106px; height: 26px">
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Weld Map:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:DropDownList ID="dlWeldMapping" runat="server">
                </asp:DropDownList></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Material:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:DropDownList ID="dlMaterial" runat="server" Width="215px">
                </asp:DropDownList></td>
            <td style="width: 106px; height: 26px">
            </td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana">Drawing:</span></td>
            <td style="width: 438px; height: 26px">
                <asp:TextBox ID="txtDrawing" runat="server" MaxLength="500" Width="381px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnBrowse" runat="server" Text="Browse..."
                    UseSubmitBehavior="False" Width="93px" />
            
                         
            </td>
                  
        </tr>
        
        <tr>
            <td style="width: 241px; height: 26px">
                &nbsp;</td>
            <td style="width: 438px; height: 26px">
                &nbsp;</td>
            <td style="width: 106px; height: 26px">
                &nbsp;</td>
                  
        </tr>
        
        <tr>
            <td style="width: 241px; height: 26px">
                <span style="font-family: Verdana"></span>
            </td>
            <td style="width: 438px; height: 26px">
                </td>
            <td style="width: 106px; height: 26px">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save Spool"
                    UseSubmitBehavior="False" Width="93px" OnClientClick="Confirm() " /></td>
            <td>
                <asp:Button ID="btnNewSpool" runat="server" OnClick="btnNewSpool_Click" Text="New Spool"
                    UseSubmitBehavior="False" Width="93px" /></td>
            <td>
                <asp:Button ID="btnCopySpool" runat="server" OnClick="btnCopySpool_Click" Text="Copy Spool"
                    UseSubmitBehavior="False" Width="93px" /></td>
        </tr>
        <tr>
            <td style="width: 241px; height: 26px;">
                <asp:Label ID="lblPartNo" runat="server" Font-Names="Verdana" Text="Part:"></asp:Label></td>
            <td style="width: 438px; height: 26px;">
                <asp:TextBox ID="txtPartNo" runat="server" MaxLength="150" Width="379px"></asp:TextBox></td>
            <td style="width: 106px; height: 26px;">
                <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Add" Width="91px" /></td>
        </tr>
    </table>

    <table style="width: 2000px">
        <tr>
            <td style="width: 88px; height: 23px;">
            </td>
            <td style="width: 1302px; height: 23px;">
                <asp:Label ID="lblSelected" runat="server" Font-Names="Verdana" Text="Selected Parts:"
                    Width="173px" ForeColor="Black" Font-Bold="False"></asp:Label></td>
            
        </tr>

        <tr>
            <td style="width: 88px">
            </td>
            <td style="width: 1900px" >
                <asp:Table ID="tblParts" runat="server" Font-Names="Verdana" CellPadding="2" CellSpacing="2" EnableViewState="False">
                </asp:Table>
            </td>
        </tr>
    </table>
    <br />
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
                /*            
                if (confirm("Create another spool like this one?")) 
                {
                    confirm_create_another_value.value = "Yes";
                }
                else
                {
                    confirm_create_another_value.value = "No";
                }
                
                scroll(0,0);
                */
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
            if ((txtlen - dotpos) > 3)
                return false;
        }

        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
     }
     
      function get_filename_drawing() 
     {
        document.getElementById('<%=txtDrawing.ClientID%>').value = document.getElementById('<%=FileUpload1.ClientID%>').value.split(/(\\|\/)/g).pop();
        }

        
     
    </script>

    <table style="width:100%;">
        <tr>
            <td>
                <asp:TextBox ID="txtPipeSize" runat="server" MaxLength="20" Width="206px" Visible="False"></asp:TextBox></td>
             
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtCutSize1" runat="server" MaxLength="20" Width="206px" Visible="False"></asp:TextBox>
                <asp:TextBox ID="txtCutSize2" runat="server" MaxLength="20" Width="206px" Visible="False"></asp:TextBox>
                <asp:TextBox ID="txtCutSize3" runat="server" MaxLength="20" Width="206px" Visible="False"></asp:TextBox>
                <asp:TextBox ID="txtCutSize4" runat="server" MaxLength="20" Width="206px" Visible="False"></asp:TextBox></td>
            
        </tr>
        <tr>
            <td>&nbsp;</td>
             
        </tr>
    </table>

</asp:Content>
