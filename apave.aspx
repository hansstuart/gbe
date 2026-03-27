<%@ Page Title="" Language="C#" MasterPageFile="~/gbe_irisndt.Master" AutoEventWireup="true" CodeBehind="apave.aspx.cs" Inherits="gbe.irisndt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 75%; margin-left: 0px;" border="1">
        <tr>
            <td style="height: 23px;  " colspan="2">
                 <asp:Label ID="lblMsg" runat="server" Font-Names="Verdana" ForeColor="Red" Width="440px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="height: 23px;  ">
                <asp:Label ID="Label1" runat="server" Font-Names="Verdana" Text="Download" Font-Bold="True"></asp:Label>
            </td>
            <td style="height: 23px;  ">
                <asp:Label ID="Label2" runat="server" Font-Names="Verdana" Text="Upload" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="  height: 36px;">
                &nbsp;</td>
            <td style="height: 36px;  ">
                <table style="">
                    <tr>
                        <td style=" ">
                            <asp:TextBox ID="txtUploadFile" runat="server" Width="321px"></asp:TextBox>
                        </td>
                        <td style=" ">
                <asp:Button ID="btnBrowse" runat="server" Text="Browse..."
                    UseSubmitBehavior="False" Width="93px" />
            
                         
                        </td>
                        <td style=" ">
                            <asp:Button ID="btnUpload" runat="server" Text="Upload" Width="93px" OnClick="btnUpload_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top; height: 30px;">
                <asp:Table ID="tblDownloads" runat="server" Font-Names="Verdana">
                </asp:Table>
            </td>
            <td style="vertical-align: top; height: 30px;  ">
                <asp:Table ID="tblUploads" runat="server" Font-Names="Verdana">
                </asp:Table>
            </td>
        </tr>
        <tr>
            <td style="  height: 30px;">
                &nbsp;</td>
            <td style="height: 30px;  ">
                &nbsp;</td>
        </tr>
    </table>

    <script type="text/javascript">
        function get_filename() {
            document.getElementById('<%=txtUploadFile.ClientID%>').value = document.getElementById('<%=FileUpload1.ClientID%>').value.split(/(\\|\/)/g).pop();
        }
    </script>

    <asp:FileUpload ID="FileUpload1" runat="server" onchange="get_filename()" style="display: none;"/> 

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
</asp:Content>
