<%@ Page Language="C#" MasterPageFile="~/gbe.Master" AutoEventWireup="true" CodeBehind="create_module.aspx.cs" Inherits="gbe.create_module" Title="Create Module" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 753px">
        <tr>
            <td style="font-family: Verdana">
            </td>
            <td style="width: 100px">
                <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red"></asp:Label></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="font-family: Verdana">
                Module Number:<br />
                eg.G00000-xx-xx-MOD</td>
            <td style="width: 100px">
                <asp:TextBox ID="txtModuleNumber" runat="server" AutoCompleteType="Disabled" MaxLength="50"
                    Width="381px"></asp:TextBox></td>
            <td style="width: 99px">
            </td>
        </tr>
        <tr>
            <td style="width: 172px; height: 21px">
                <span style="font-family: Verdana">Revision:</span></td>
            <td style="width: 100px; height: 21px">
                <asp:TextBox ID="txtRevision" runat="server" MaxLength="3" Width="64px"></asp:TextBox></td>
            <td style="width: 99px; height: 21px">
                <asp:Button ID="btnSave" runat="server" OnClick="btnProceed_Click" OnClientClick="Confirm() "
                    Text="Save" UseSubmitBehavior="False" Width="94px" /></td>
        </tr>
        <tr>
            <td style="width: 172px">
            </td>
            <td style="width: 100px">
            </td>
            <td style="width: 99px">
            </td>
        </tr>
    </table>
    <script type="text/javascript">
    
        function Confirm() 
        {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
        
            if (confirm("Save module?")) 
            {
                confirm_value.value = "Yes";
                
                scroll(0,0);
                
            } else 
            {
                confirm_value.value = "No";
            }
            
            document.forms[0].appendChild(confirm_value);
        }
    
    </script>
</asp:Content>
