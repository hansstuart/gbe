<%@ Page Language="C#" MasterPageFile="~/gbe_fy.Master" AutoEventWireup="true" CodeBehind="fy_delivery.aspx.cs" Inherits="gbe.fy_delivery" Title="Delivery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Panel ID="pnlMain" runat="server" >
<div style="clear:both">&nbsp;</div>

<div>
         <asp:Label ID="lblMsg" runat="server" CssClass="FY_LabelErrorMsg" style="width: 100%"></asp:Label>
</div>

<asp:Button ID="btnNull" runat="server" Width="0px" BorderStyle="None" Height="0px" BackColor="LightBlue" />

<div style="clear:both">&nbsp;</div>

<asp:MultiView ID="MultiView1" runat="server" OnActiveViewChanged="MultiView1_ActiveViewChanged">

<asp:View ID="View1" runat="server">

<div style="float:left; width: 20%">&nbsp;</div>

<div style="float:left" >
    <asp:Button ID="btnNewDelivery" runat="server" Text="NEW DELIVERY" CssClass="FY_Button" UseSubmitBehavior="False" OnClick="btnNewDelivery_Click"/>  
</div>

<div style="clear:both"><br/><br/><br/><br/></div>

<div style="float:left; width: 20%; ">&nbsp;</div>

<div>
    <asp:Label id="lblDeliveryNumber" runat="server" Text="Delivery Number:" CssClass="FY_Label"></asp:Label>
</div>

<div style="float:left; width: 20%;">&nbsp;</div>

<div style="float:left">
        <asp:TextBox ID="txtDeliveryNumber" runat="server" Width="230px" CssClass="FY_TextBox"></asp:TextBox>
</div>

<div style="clear:both">&nbsp;</div>

<div style="float:left; width: 20%;">&nbsp;</div>

<div style="float:left" >
    <asp:Button ID="btnOpenDelivery" runat="server" Text="OPEN DELIVERY" CssClass="FY_Button" UseSubmitBehavior="False" OnClick="btnOpenDelivery_Click"/>  
</div>

</asp:View>

<asp:View ID="View2" runat="server">

<div style="clear:both">&nbsp;</div>

<asp:Label id="lblVehicle" runat="server" Text="Vehicle:" CssClass="FY_Label"></asp:Label>
<br/>
<asp:DropDownList ID="cboVehicle" runat="server" CssClass="FY_Label" Width="300px"></asp:DropDownList>

<br/>
<asp:Label id="lblDriver" runat="server" Text="Driver:" CssClass="FY_Label"></asp:Label>
<br/>
<asp:DropDownList ID="cboDriver" runat="server" CssClass="FY_Label" Width="300px"></asp:DropDownList>

<div style="clear:both">&nbsp;</div>

<div>
    <asp:Label id="lblBarcode" runat="server" Text="Spool Barcode:" CssClass="FY_Label"></asp:Label>
</div>

<div>
    
    <div style="float:left">
        <asp:TextBox ID="txtBarcode" runat="server" Width="600px" CssClass="FY_TextBox"></asp:TextBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnAdd" runat="server" ImageUrl="~/add32.png" ToolTip="Add to list" OnClick="btnAdd_Click" />
    </div>
</div>

<div style="clear:both">&nbsp;</div>

<div>
    <div style="float:left">
        <asp:ListBox ID="lstBarcodes" runat="server" Width="606px" CssClass="FY_Label" Height="220px" SelectionMode="Multiple"></asp:ListBox>
    </div>
    
    <div style="float:left; width:10px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/delete32.png" ToolTip="Delete from list" OnClick="btnDelete_Click" />
    </div>
</div>

<div style="clear:both">&nbsp;</div>
    
<asp:Label id="lblCount" runat="server" Text="Count: " CssClass="FY_Label"></asp:Label>


<div style="clear:both">&nbsp;</div>

<div style="clear:both">
    <div style="float:left; width:280px">&nbsp;</div>
        
    <div style="float:left">
        <asp:ImageButton ID="btnBack2" runat="server" ImageUrl="~/left32.png" ToolTip="Back" OnClick="btnBack2_Click"   />
    </div>
    
    <div style="float:left; width:20px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnNext2" runat="server" ImageUrl="~/right32.png" ToolTip="Next" OnClick="btnNext2_Click"  />
    </div>
</div>  

</asp:View>

<asp:View ID="View3" runat="server">

<div style="clear:both">&nbsp;</div>

<asp:Label id="lblDeliveryAddress" runat="server" Text="Delivery Address:" CssClass="FY_Label"></asp:Label>
<br/>
<asp:DropDownList ID="cboDeliveryAddress" runat="server" CssClass="FY_Label" Width="608px" OnSelectedIndexChanged="cboDeliveryAddress_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
<br/>
<asp:TextBox ID="txtDeliveryAddress" runat="server" CssClass="FY_TextBox" Width="600px" Height="220px" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>

<br/><br/><br/>

<div style="clear:both">
    <div style="float:left; width:280px">&nbsp;</div>
        
    <div style="float:left">
        <asp:ImageButton ID="btnBack3" runat="server" ImageUrl="~/left32.png" ToolTip="Back" OnClick="btnBack3_Click"   />
    </div>
    
    <div style="float:left; width:20px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnNext3" runat="server" ImageUrl="~/right32.png" ToolTip="Next" OnClick="btnNext3_Click"  />
    </div>
</div>  

</asp:View>

<asp:View ID="View4" runat="server">

<div style="clear:both">&nbsp;</div>
<br/><br/>

<asp:CheckBox ID="chkShip" runat="server" Text="Delivery is ready to go. Update delivery and spool status to 'SHIPPED'." CssClass="FY_Label" />

<br/><br/>
    
<div style="clear:both">
    <div style="float:left; width:280px">&nbsp;</div>
        
    <div style="float:left">
        <asp:ImageButton ID="btnBack4" runat="server" ImageUrl="~/left32.png" ToolTip="Back" OnClick="btnBack4_Click"   />
    </div>
    
    <div style="float:left; width:20px">&nbsp;</div>
    
    <div style="float:left">
        <asp:ImageButton ID="btnSave" runat="server" ImageUrl="~/save32.png" ToolTip="Save" OnClick="btnSave_Click"  />
    </div>
</div>  
    
</asp:View>
    
</asp:MultiView></asp:Panel>  
</asp:Content>
