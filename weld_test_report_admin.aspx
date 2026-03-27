<%@ Page Title="" Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="weld_test_report_admin.aspx.cs" Inherits="gbe.weld_test_report_admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="clear:both">&nbsp;</div>
    
    <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="440px"></asp:Label>
    
    <div style="clear:both">&nbsp;</div>

    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0" OnActiveViewChanged="MultiView1_ActiveViewChanged">

    <asp:View ID="View1" runat="server">
        
    <div style="clear:both">&nbsp;</div>

    <table style="width: 75%; margin-left: 0px;" border="1">
        <tr>
            <td style="vertical-align: top; height: 30px;  " align="center">
                <asp:Table ID="tblReports" runat="server" Font-Names="Verdana">
                </asp:Table>
            </td>
        </tr>
    </table>
    
    </asp:View>

    <asp:View ID="View2" runat="server">

   <div style="clear:both">&nbsp;</div>
   <div style="clear:both">&nbsp;</div>

    <div style="float:left">
        <asp:Label ID="lblReport" runat="server" Font-Names="Verdana" ></asp:Label>
    </div>

    <div style="clear:both">&nbsp;</div>

    <div style="float:left">
        <asp:Label ID="lblBarcode" runat="server" Font-Names="Verdana" Text="Barcode"></asp:Label>
        <asp:TextBox ID="txtBarcode" runat="server" Width="530px" CssClass=""></asp:TextBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left; height: 16px;">
        <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="~/add.png" ToolTip="Add to list" OnClick="btnAdd_Click" />
    </div>
 

<div style="clear:both">&nbsp;</div>

<div>
    <div style="float:left;  width:606px; overflow: auto ">
        <asp:ListBox ID="lstBarcodes" runat="server" Width="600px" CssClass="" Height="400px" SelectionMode="Multiple"></asp:ListBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <div>
            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
        </div>
    </div> 

    <div style="clear:both">&nbsp;</div>
    <div style="clear:both">&nbsp;</div>
        
    
    <div>
        <asp:Button class="floated" ID="btnSaveSpools" runat="server" Text="Save" OnClick="btnSaveSpools_Click" Width="102px" />
    
    </div>
    
    <div>
            <asp:Button class="floated" ID="btnCancel" runat="server" Text="Cancel"   Width="102px" OnClick="btnCancel_Click" />
    </div>

    <div>&nbsp;</div>
        

  </div>      

    </asp:View>

    </asp:MultiView>


  
    <script type = "text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";

            if (confirm("Delete?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
    </script>

    <style>

        .floated {
           float:left;
           margin-right:5px;
        }
     </style>

</asp:Content>
