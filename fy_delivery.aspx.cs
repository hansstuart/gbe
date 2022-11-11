using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace gbe
{
    public partial class fy_delivery : System.Web.UI.Page
    {
        const string VS_DELIVERY_DATA = "VS_DELIVERY_DATA";
        const string VS_ADDRESSES = "VS_ADDRESSES";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            set_default_button();

            if (!IsPostBack)
            {
                string user_name = System.Web.HttpContext.Current.User.Identity.Name;
                int user_id = fy.ws().is_driver_or_loader(fy.PASSKEY, user_name);

                object[] drv = fy.ws().get_driver_names(fy.PASSKEY, user_id);
                object[] veh = fy.ws().get_vehicle_reg(fy.PASSKEY, user_id);

                foreach (string sdrv in drv)
                {
                    cboDriver.Items.Add(sdrv);
                }

                foreach (string sveh in veh)
                {
                    cboVehicle.Items.Add(sveh);
                }

                MultiView1.ActiveViewIndex = 0;

                txtBarcode.Attributes.Add("onfocus", "this.select()");

                btnSave.OnClientClick = "_confirm(" + "'Save Delivery?'" + ")";
            }
        }

        protected void btnNewDelivery_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 1;
        }

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            if (txtBarcode.Text.Trim().Length > 0)
            {
                process_barcode();
            }

            if (lblMsg.Text.Trim().Length == 0)
                txtBarcode.Text = string.Empty;

            txtBarcode.Focus();
        }

        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList a_i = new ArrayList();

            for (int i = 0; i < lstBarcodes.Items.Count; i++)
            {
                if (lstBarcodes.Items[i].Selected)
                    a_i.Add(i);
            }

            for (int i = a_i.Count - 1; i >= 0; i--)
                lstBarcodes.Items.RemoveAt((int)a_i[i]);
        }

        protected void btnOpenDelivery_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDeliveryNumber.Text.Trim().Length > 0)
                {
                    int id = 0;

                    try { id = Convert.ToInt32(txtDeliveryNumber.Text.Trim()); }
                    catch
                    {
                        lblMsg.Text = "Invalid delivery number";
                        return;
                    }

                    gbe_ws.delivery_data dd = fy.ws().get_delivery_data(fy.PASSKEY, id);

                    if (dd != null)
                    {
                        if (dd.shipped)
                        {
                            lblMsg.Text = "Delivery has been shipped";
                        }
                        else
                        {
                            ViewState[VS_DELIVERY_DATA] = dd;

                            cboDriver.Text = dd.driver;
                            cboVehicle.Text = dd.vehicle;

                            object[] barcodes = fy.ws().get_spool_delivery_barcodes(fy.PASSKEY, dd.id);

                            foreach (string bc in barcodes)
                            {
                                add_to_list(bc);
                            }

                            txtBarcode.Focus();

                            MultiView1.ActiveViewIndex = 1;
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Delivery not found";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message.ToString() + "<br/><br/>" + ex.ToString();
            }
        }

        void add_to_list(string bc)
        {
            bool bAdd = true;

            foreach (ListItem li in lstBarcodes.Items)
            {
                if (li.Text.ToLower() == bc.ToLower())
                {
                    bAdd = false;
                }
            }

            if (bAdd)
            {
                if (update_status(bc, "LD") == 0)
                {
                    lstBarcodes.Items.Add(bc);

                    update_count();
                }
            }
        }

        int update_status(string bc, string status)
        {
            int ret = 0;

            try
            {
                fy.ws().upd_spool_status(fy.PASSKEY, bc, status);
            }
            catch (Exception ex)
            {
                lblMsg.Text = "update_status<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                ret = -1;
            }

            return ret;
        }

        void update_count()
        {
            lblCount.Text = "Count: " + lstBarcodes.Items.Count.ToString();
        }

        protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        {
            if (MultiView1.ActiveViewIndex == 0)
            {
                txtBarcode.Text = string.Empty;
                lstBarcodes.Items.Clear();
                update_count();
            }

            set_default_button();
            
        }

        void process_barcode()
        {
            try
            {
                gbe_ws.module_data md = fy.ws().get_module(fy.PASSKEY, txtBarcode.Text.Trim());

                if (md != null)
                    process_module(md);
                else
                    process_spool();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "process_barcode<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        void process_module(gbe_ws.module_data md)
        {
            try
            {
                object[] a = fy.ws().get_module_spool_barcodes(fy.PASSKEY, md.module);

                foreach (string bc in a)
                {
                    if (is_spool_marked_for_delivery(bc) == 0)
                    {
                        lblMsg.Text = ("One or more spools not marked for delivery.");
                        return;
                    }
                }

                add_to_list(txtBarcode.Text.Trim());

                foreach (string bc in a)
                    add_to_list(bc);
            }
            catch (Exception ex)
            {
                lblMsg.Text = "process_module<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        void process_spool()
        {
            if (is_spool_marked_for_delivery(txtBarcode.Text.Trim()) == 1)
                add_to_list(txtBarcode.Text.Trim());
            else
                lblMsg.Text = ("Spool not marked for delivery.");
        }

        int is_spool_marked_for_delivery(string bc)
        {
            int ret = 0;

            try
            {
                gbe_ws.spool_data sd = fy.ws().get_spool(fy.PASSKEY, bc.Trim());

                if (sd != null)
                    ret = (sd.status == "RD" || sd.status == "LD") ? 1 : 0;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "is_spool_marked_for_delivery<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                ret = -1;
            }

            return ret;
        }

        protected void btnBack3_Click(object sender, ImageClickEventArgs e)
        {
            MultiView1.ActiveViewIndex = MultiView1.ActiveViewIndex - 1;
        }

        protected void btnBack2_Click(object sender, ImageClickEventArgs e)
        {
            MultiView1.ActiveViewIndex = MultiView1.ActiveViewIndex - 1;
        }

        protected void btnBack4_Click(object sender, ImageClickEventArgs e)
        {
            MultiView1.ActiveViewIndex = MultiView1.ActiveViewIndex - 1;
        }

        protected void btnNext2_Click(object sender, ImageClickEventArgs e)
        {
            if (lstBarcodes.Items.Count > 0)
            {
                string contract_number = string.Empty;
                string[] sa_bc = lstBarcodes.Items[0].Text.Split('-');

                if (sa_bc.Length > 0)
                {
                    contract_number = sa_bc[0];

                    object [] a_addr = fy.ws().get_addresses(fy.PASSKEY, contract_number);

                    ViewState[VS_ADDRESSES] = a_addr;

                    foreach (gbe_ws.address addr in a_addr)
                    {
                        string saddr = string.Empty;

                        foreach (string addr_line in addr.address_line)
                        {
                            if (saddr.Trim().Length > 0)
                                saddr += ",";

                            saddr += addr_line;
                        }

                        cboDeliveryAddress.Items.Add(saddr);
                    }

                    if (cboDeliveryAddress.Items.Count > 0)
                    {
                        cboDeliveryAddress_SelectedIndexChanged(null, null);
                    }
                }

                MultiView1.ActiveViewIndex = MultiView1.ActiveViewIndex + 1;
            }
        }

        protected void btnNext3_Click(object sender, ImageClickEventArgs e)
        {
            MultiView1.ActiveViewIndex = MultiView1.ActiveViewIndex + 1;
        }

        protected void cboDeliveryAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDeliveryAddress.Text = string.Empty;

            object[] a_addr = (object[])ViewState[VS_ADDRESSES];

            if (cboDeliveryAddress.SelectedIndex >= 0)
            {
                gbe_ws.address addr = (gbe_ws.address)a_addr[cboDeliveryAddress.SelectedIndex];

                foreach (string addr_line in addr.address_line)
                    txtDeliveryAddress.Text += addr_line + "\r\n";
            }
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string confirmValue = Request.Form["confirm_value"];

                if (confirmValue == "Yes")
                {
                    if (cboVehicle.Text.Trim().Length == 0)
                    {
                        lblMsg.Text = "Vehicle missing";
                        cboVehicle.Focus();
                        return;
                    }

                    if (cboDriver.Text.Trim().Length == 0)
                    {
                        lblMsg.Text = "Driver missing";
                        cboDriver.Focus();
                        return;
                    }

                    string address_table = string.Empty;
                    int address_id = 0;

                    object[] a_addr = (object[])ViewState[VS_ADDRESSES];

                    gbe_ws.address addr = (gbe_ws.address)a_addr[cboDeliveryAddress.SelectedIndex];
                    address_table = addr.table;
                    address_id = addr.id;

                    object[] a = new object[lstBarcodes.Items.Count];

                    int i = 0;

                    foreach (ListItem li in lstBarcodes.Items)
                    {
                        string bc = li.Text;
                        a[i++] = bc;
                    }

                    int delivery_id = 0;

                    gbe_ws.delivery_data dd = null;

                    try
                    {
                        dd = (gbe_ws.delivery_data)ViewState[VS_DELIVERY_DATA];
                    }
                    catch { }

                    if (dd != null)
                    {
                        delivery_id = dd.id;
                    }

                    string user_name = System.Web.HttpContext.Current.User.Identity.Name;
                    int user_id = fy.ws().is_driver_or_loader(fy.PASSKEY, user_name);

                    if (fy.ws().save_delivery_3(fy.PASSKEY, user_id, cboVehicle.Text.Trim(), cboDriver.Text.Trim(), a, address_table, address_id, ref delivery_id, chkShip.Checked))
                    {
                        string msg = "Delivery saved successfully.<br/><br/>The Delivery Number is:<br/>" + delivery_id.ToString();
                        string return_url = "fy_delivery.aspx";

                        fy.show_msg(msg, return_url, this);
                    }
                    else
                    {
                        lblMsg.Text = ("Problem saving data");
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "btnSave_Click<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        void set_default_button()
        {
            if (MultiView1.ActiveViewIndex == 1)
                pnlMain.DefaultButton = "btnAdd";
            else
                pnlMain.DefaultButton = "btnNull";
        }
    }
}
