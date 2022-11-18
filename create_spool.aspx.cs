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
using System.Text.RegularExpressions;
using System.IO;

namespace gbe
{
    public partial class create_spool : System.Web.UI.Page
    {
        const int STATE_ENTER_CONTRACT_NO = 0;
        const int STATE_ENTER_CONTACT = 1;
        const int STATE_CONTRACT_FOUND_DISPLAY_CONTACT = 2;
        const int STATE_ENTER_PARTS = 3;
        const string CUST_DELIVERY_ADDR = "Use customer address";
        const string NEW_DELIVERY_ADDR = "Enter new address";
        const string GBE_MEDWAY = "GBE, Whitewall Rd, Medway City Estate";
        const string VS_IMSL_COST_CENTRES = "vs_imsl_cost_centres";

        const string MAT_NA = "";
        const string MAT_CARBON_STEEL = "Carbon Steel";
        const string MAT_STAINLESS_STEEL = "Stainless Steel";
        const string MAT_SCREWED = "Screwed";
        const string MAT_HS = "HS";

        int m_state = 0;
        ArrayList m_parts_list = new ArrayList();
        ArrayList m_customer_controls = new ArrayList();
        ArrayList m_parts_controls = new ArrayList();
        ArrayList m_new_delivery_address_controls = new ArrayList();

        SortedList m_sl_delivery_addresses = new SortedList();
        SortedList m_sl_imsl_cost_centres = new SortedList();

        string m_user_id = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");

            lblMsg.Text = lblMsg2.Text = string.Empty;

            m_customer_controls.Add(txtCustomer);
            m_customer_controls.Add(txtAddr1);
            m_customer_controls.Add(txtAddr2);
            m_customer_controls.Add(txtAddr3);
            m_customer_controls.Add(txtAddr4);
            m_customer_controls.Add(txtPhone);
            m_customer_controls.Add(txtEmail);
            m_customer_controls.Add(txtContactName);
            m_customer_controls.Add(lblCustomer);
            m_customer_controls.Add(lblAddr1);
            m_customer_controls.Add(lblAddr2);
            m_customer_controls.Add(lblAddr3);
            m_customer_controls.Add(lblAddr4);
            m_customer_controls.Add(lblPhone);
            m_customer_controls.Add(lblEmail);
            m_customer_controls.Add(lblContactName);
            m_customer_controls.Add(lblDelAddr);
            m_customer_controls.Add(dlDelAddr);

            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_line1);
            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_line2);
            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_line3);
            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_line4);
            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_name);
            m_new_delivery_address_controls.Add(lbl_new_deliv_addr_phone);

            m_new_delivery_address_controls.Add(txt_new_deliv_addr_line1);
            m_new_delivery_address_controls.Add(txt_new_deliv_addr_line2);
            m_new_delivery_address_controls.Add(txt_new_deliv_addr_line3);
            m_new_delivery_address_controls.Add(txt_new_deliv_addr_line4);
            m_new_delivery_address_controls.Add(txt_new_deliv_addr_name);
            m_new_delivery_address_controls.Add(txt_new_deliv_addr_phone);

            m_parts_controls.Add(txtSpoolNumber);
            m_parts_controls.Add(txtRevision);
            m_parts_controls.Add(lblPartNo);
            m_parts_controls.Add(txtPartNo);
            m_parts_controls.Add(btnAdd);
            m_parts_controls.Add(lblSelected);
            m_parts_controls.Add(tblParts);

            m_parts_controls.Add(txtPipeSize);
            m_parts_controls.Add(txtCutSize1);
            m_parts_controls.Add(txtCutSize2);
            m_parts_controls.Add(txtCutSize3);
            m_parts_controls.Add(txtCutSize4);
            m_parts_controls.Add(txtDrawing);

            if (IsPostBack)
            {
                m_state = (int)ViewState["state"];
                m_parts_list = (ArrayList)ViewState["parts_list"];
                m_sl_delivery_addresses = (SortedList)ViewState["delivery_addresses"];
                m_sl_imsl_cost_centres = (SortedList)ViewState[VS_IMSL_COST_CENTRES];
                
                show_parts_list();

                if (FileUpload1.HasFile)
                {
                    Session["FileUpload1"] = FileUpload1;
                }
                else
                {
                    if (Session["FileUpload1"] != null)
                    {
                        FileUpload1 = (FileUpload)Session["FileUpload1"];
                    }
                }
            }
            else
            {
                txtDrawing.Attributes.Add("readonly", "readonly");

                dlWeldMapping.Items.Add(string.Empty);
                dlWeldMapping.Items.Add("Yes");
                dlWeldMapping.Items.Add("No");

                dlMaterial.Items.Add(MAT_NA);
                dlMaterial.Items.Add(MAT_CARBON_STEEL);
                dlMaterial.Items.Add(MAT_STAINLESS_STEEL);
                dlMaterial.Items.Add(MAT_SCREWED);
                dlMaterial.Items.Add(MAT_HS);

                cost_centre cc = new cost_centre();

                dlCostCentre.Items.Add(string.Empty);

                foreach (DictionaryEntry e0 in cc.m_sl_cost_centre)
                {
                    dlCostCentre.Items.Add(e0.Value.ToString());
                }

                using (cdb_connection dbc = new cdb_connection())
                {
                    dbc.order_by = customer_fab_mat.NAME;

                    DataTable dtab = dbc.get_data(customer_fab_mat.TBL, null);

                    if (m_sl_imsl_cost_centres == null)
                        m_sl_imsl_cost_centres = new SortedList();
                    else
                        m_sl_imsl_cost_centres.Clear();

                    foreach (DataRow dr in dtab.Rows)
                    {
                        customer_fab_mat dr_cfm = new customer_fab_mat(dr);

                        m_sl_imsl_cost_centres.Add(dr_cfm.name, dr_cfm.id);
                    }

                    dlIMSLCostCentre.Items.Add(string.Empty);
                    dlIMSLCostCentre.Items.Add("NONE");

                    foreach (DictionaryEntry e0 in m_sl_imsl_cost_centres)
                    {
                        dlIMSLCostCentre.Items.Add(e0.Key.ToString());
                    }

                    ViewState[VS_IMSL_COST_CENTRES] = m_sl_imsl_cost_centres;
                }

                string uid = Request.QueryString["uid"];

                if (uid != null)
                {
                    hlSpoolsView.NavigateUrl += Request.Url.Query;
                    ViewState["search_string"] = Request.QueryString["s"];
                    ViewState["status_index"] = Request.QueryString["u"];
                    ViewState["field_index"] = Request.QueryString["f"];
                    ViewState["on_hold"] = Request.QueryString["h"];
                    ViewState["page_no"] = Request.QueryString["p"];

                    populate_for_edit(uid);
                }
                else
                {
                    hlSpoolsView.Visible = false;

                    m_state = STATE_ENTER_CONTRACT_NO;
                    ViewState["state"] = m_state;

                    m_parts_list = new ArrayList();
                    ViewState["parts_list"] = m_parts_list;

                    m_sl_delivery_addresses = new SortedList();
                    ViewState["delivery_addresses"] = m_sl_delivery_addresses;

                    display_customer_controls(false);
                    display_new_delivery_address_controls(false);
                    txtContractNumber.Focus();
                }

                btnBrowse.Attributes.Add("onclick", "document.getElementById('" + FileUpload1.ClientID + "').click(); return false;");
            }

            if (m_state == STATE_ENTER_PARTS)
            {
                display_parts_controls(true);
                txtPartNo.Focus();
            }
            else
                display_parts_controls(false);
        }

        void populate_for_edit(string uid)
        {
            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            if (id > 0)
            {
                using (spools spls = new spools())
                {
                    SortedList sl = new SortedList();

                    sl.Add("id", id);

                    ArrayList asd = spls.get_spool_data_ex(sl);

                    if (asd.Count > 0)
                    {
                        spool_data sd = (spool_data)asd[0];

                        string[] spn = sd.spool.Split('-');

                        if (spn.Length > 0)
                        {
                            txtContractNumber.Text = spn[0];

                            show_customer_details();

                            dlCostCentre.SelectedIndex = sd.cost_centre;

                            if (sd.imsl_cost_centre == 0)
                            {
                                dlIMSLCostCentre.Text = "NONE";
                            }
                            else
                            {
                                foreach (DictionaryEntry e0 in m_sl_imsl_cost_centres)
                                {
                                    if ((int)e0.Value == sd.imsl_cost_centre)
                                        dlIMSLCostCentre.Text = e0.Key.ToString();
                                }
                            }

                            if (sd.delivery_address == 0)
                                dlDelAddr.Text = GBE_MEDWAY;
                            else if (sd.delivery_address == -1)
                                dlDelAddr.Text = CUST_DELIVERY_ADDR;
                            else
                            {
                                foreach (DictionaryEntry e0 in m_sl_delivery_addresses)
                                {
                                    if ((int)e0.Value == sd.delivery_address)
                                        dlDelAddr.Text = e0.Key.ToString();
                                }
                            }

                            display_new_delivery_address_controls(false);

                            txtSpoolNumber.Text = txtRevision.Text = string.Empty;

                            for (int i = 1; i < spn.Length; i++)
                            {
                                if (txtSpoolNumber.Text.Trim().Length > 0)
                                    txtSpoolNumber.Text += "-";

                                txtSpoolNumber.Text += spn[i];
                            }

                            txtRevision.Text = sd.revision;

                            if (sd.include_in_weld_map)
                                dlWeldMapping.Text = "Yes";
                            else
                                dlWeldMapping.Text = "No";

                            dlMaterial.Text = sd.material;

                            txtPipeSize.Text = sd.pipe_size;
                            txtCutSize1.Text = sd.cut_size1;
                            txtCutSize2.Text = sd.cut_size2;
                            txtCutSize3.Text = sd.cut_size3;
                            txtCutSize4.Text = sd.cut_size4;

                            foreach (spool_part_data spd in sd.spool_part_data)
                            {
                                spd.part_data.attributes.Add("spd_id", spd.id);
                                spd.part_data.attributes.Add("include_in_weld_map", spd.include_in_weld_map.ToString());
                                spd.part_data.attributes.Add("welder", spd.welder);

                                add_part_to_table(spd.part_data, spd);

                                m_parts_list.Add(spd.part_data);
                            }

                            ViewState["parts_list"] = m_parts_list;
                            
                            m_state = STATE_ENTER_PARTS;
                            ViewState["state"] = m_state;

                            ViewState["modify_id"] = id;
                        }
                    }
                }
            }
        }

        void display_parts_controls(bool bdisplay)
        {
            tbl_spl_and_parts.Visible = bdisplay;

            foreach (Control c in m_parts_controls)
                c.Visible = bdisplay;

            btnProceed.Visible = !bdisplay;
            btnSave.Visible = bdisplay;
        }

        void display_customer_controls(bool bdisplay)
        {
            foreach (Control c in m_customer_controls)
                c.Visible = bdisplay;
        }

        void display_new_delivery_address_controls(bool bdisplay)
        {
            tbl_new_delivery_address.Visible = bdisplay;

            foreach (Control c in m_new_delivery_address_controls)
                c.Visible = bdisplay;
        }

        void enable_customer_controls(bool bena)
        {
            foreach (Control c in m_customer_controls)
            {
                if(c.GetType() == typeof(TextBox))
                {
                    TextBox t = (TextBox)c;
                    t.ReadOnly = !bena;
                }
            }
        }

        void clear_controls()
        {
            dlCostCentre.Text = string.Empty;
            dlIMSLCostCentre.Text = string.Empty;
            dlWeldMapping.Text = string.Empty;
            dlMaterial.Text = MAT_NA;

            txtSpoolNumber.Text = string.Empty;
            txtRevision.Text = string.Empty;

            foreach (Control c in m_customer_controls)
                if (c.GetType() == typeof(TextBox))
                    ((TextBox)c).Text = string.Empty;

            foreach (Control c in m_parts_controls)
                if (c.GetType() == typeof(TextBox))
                    ((TextBox)c).Text = string.Empty;

            foreach (Control c in m_new_delivery_address_controls)
                if (c.GetType() == typeof(TextBox))
                    ((TextBox)c).Text = string.Empty;

            tblParts.Rows.Clear();
        }

        void show_customer_details()
        {
            txtContractNumber.ReadOnly = true;
            
            display_customer_controls(true);

            SortedList sl = new SortedList();
                       
            sl.Add("contract_number", txtContractNumber.Text);

            using (customers c = new customers())
            {
                ArrayList a = c.get_customer_data(sl);

                if (a.Count > 0)
                {
                    enable_customer_controls(false);
                    m_state = STATE_CONTRACT_FOUND_DISPLAY_CONTACT;

                    customer_data cd = (customer_data)(a[0]);

                    txtCustomer.Text = cd.name;
                    txtAddr1.Text = cd.address_line1;
                    txtAddr2.Text = cd.address_line2;
                    txtAddr3.Text = cd.address_line3;
                    txtAddr4.Text = cd.address_line4;
                    txtContactName.Text = cd.contact_name;
                    txtPhone.Text = cd.telephone;
                    txtEmail.Text = cd.email;
                }
                else
                {
                    enable_customer_controls(true);
                    txtCustomer.Focus();

                    lblMsg.Text = "Contract not found. Enter customer details:";
                    m_state = STATE_ENTER_CONTACT;
                }
            }

            dlDelAddr.Items.Clear();

            dlDelAddr.Items.Add(GBE_MEDWAY);
            dlDelAddr.Items.Add(CUST_DELIVERY_ADDR);
            dlDelAddr.Items.Add(NEW_DELIVERY_ADDR);

            m_sl_delivery_addresses.Clear();

            using (delivery_addresses da = new delivery_addresses())
            {
                SortedList slda = new SortedList();

                slda.Add("contract_number", txtContractNumber.Text.Trim());

                ArrayList ada = da.get_delivery_address_data(slda);

                foreach (delivery_address d in ada)
                {
                    string sda = d.address_line1;

                    if (sda.Trim().Length > 0)
                        sda += ", ";

                    sda += d.address_line2;

                    if (sda.Trim().Length > 0)
                        sda += ", ";

                    sda += d.address_line3;

                    if (sda.Trim().Length > 0)
                        sda += ", ";

                    sda += d.address_line4;

                    if (sda.Length > 0)
                    {
                        if (!m_sl_delivery_addresses.ContainsKey(sda))
                        {
                            m_sl_delivery_addresses.Add(sda, d.id);
                            dlDelAddr.Items.Add(sda);
                        }
                    }
                }
            }

            ViewState["delivery_addresses"] = m_sl_delivery_addresses;
        }

        void save_customer_details()
        {
            SortedList sl = new SortedList();

            sl.Add("id", "0");
            sl.Add("contract_number", txtContractNumber.Text.Trim());
            sl.Add("name", txtCustomer.Text.Trim());
            sl.Add("address_line1", txtAddr1.Text.Trim());
            sl.Add("address_line2", txtAddr2.Text.Trim());
            sl.Add("address_line3", txtAddr3.Text.Trim());
            sl.Add("address_line4", txtAddr4.Text.Trim());
            sl.Add("contact_name", txtContactName.Text.Trim());
            sl.Add("telephone", txtPhone.Text.Trim());
            sl.Add("email", txtEmail.Text.Trim());

            using (customers c = new customers())
            {
                c.save_customer_details(sl);
            }
        }

        bool is_contract_number(string cn)
        {
            bool bret = false;

            try
            {
                if (cn.Trim().Length > 1)
                {
                    Regex r = new Regex("^[a-zA-Z0-9]*$");
                    if (r.IsMatch(cn))
                    {
                        bret = true;
                    }
                }
            }
            catch { bret = false; }

            return bret;
        }


        bool is_valid_spool_number()
        {
            bool bret = false;

            try
            {
                int l = txtSpoolNumber.Text.Trim().Length;
                if (l > 0)
                {
                    Regex r = new Regex("[0-9]");

                    if (r.IsMatch(txtSpoolNumber.Text.Trim(), l - 1))
                        bret = true;
                }
            }
            catch { bret = false; }

            return bret;
        }

        bool is_spool_number_unique(string spn)
        {
            SortedList sl = new SortedList();
            sl.Add("spool", spn);

            bool bret = false;

            using (spools s = new spools())
            {
                bret = (s.get_spool_data(sl).Count == 0);
            }

            return bret;
        }

        bool is_revsion_number_unique(string spn, string rev)
        {
            SortedList sl = new SortedList();
            sl.Add("spool", spn);
            sl.Add("revision", rev);

            bool bret = false;

            using (spools s = new spools())
            {
                bret =  (s.get_spool_data(sl).Count == 0);
            }

            return bret;
        }

        bool is_modified_spool_number_unique(string spn, string rev, int modify_id)
        {
            SortedList sl = new SortedList();
            sl.Add("spool", spn);
            sl.Add("revision", rev);

            bool bret = true;

            using (spools s = new spools())
            {
                ArrayList sl_sd = s.get_spool_data(sl);

                foreach (spool_data sd in sl_sd)
                {
                    if (sd.id != modify_id)
                    {
                        bret = false;
                        break;
                    }
                }
            }

            return bret;
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            if (m_state == STATE_ENTER_CONTRACT_NO)
            {
                if (dlCostCentre.Text == string.Empty)
                {
                    lblMsg.Text = "Select a cost centre";
                    dlCostCentre.Focus();

                }
                else if (dlIMSLCostCentre.Text == string.Empty)
                {
                    lblMsg.Text = "Select an IMSL cost centre";
                    dlIMSLCostCentre.Focus();

                }
                else if (is_contract_number(txtContractNumber.Text.Trim()))
                {
                    show_customer_details();
                }
                else
                {
                    lblMsg.Text = "Invalid contract number. Alphanumerics only";
                    txtSpoolNumber.Focus();
                }
            }
            else if (m_state == STATE_ENTER_CONTACT)
            {
                if (txtCustomer.Text.Trim().Length == 0)
                {
                    lblMsg.Text = "Customer name missing.";
                    txtCustomer.Focus();
                }
                else
                {
                    save_customer_details();
                    show_customer_details();
                }
            }
            else if (m_state == STATE_CONTRACT_FOUND_DISPLAY_CONTACT)
            {
                if (dlDelAddr.Text == NEW_DELIVERY_ADDR)
                {
                    if (!new_delivery_address_entered())
                    {
                        lblMsg.Text = "New delivery address details missing.";
                        return;
                    }
                }

                m_state = STATE_ENTER_PARTS;
                display_parts_controls(true);
                txtSpoolNumber.Focus();
            }

            ViewState["state"] = m_state;
        }

        int save_new_delivery_address()
        {
            int id = 0;

            SortedList sl = new SortedList();

            sl.Add("id", "0");
            sl.Add("contract_number", txtContractNumber.Text.Trim());
            sl.Add("address_line1", txt_new_deliv_addr_line1.Text.Trim());
            sl.Add("address_line2", txt_new_deliv_addr_line2.Text.Trim());
            sl.Add("address_line3", txt_new_deliv_addr_line3.Text.Trim());
            sl.Add("address_line4", txt_new_deliv_addr_line4.Text.Trim());
            sl.Add("contact_name", txt_new_deliv_addr_name.Text.Trim());
            sl.Add("telephone", txt_new_deliv_addr_phone.Text.Trim());

            using (delivery_addresses da = new delivery_addresses())
            {
                id = da.save_delivery_address_data(sl);
            }

            return id;
        }

        bool new_delivery_address_entered()
        {
            int new_addr_len = 0;

            new_addr_len += txt_new_deliv_addr_line1.Text.Trim().Length;
            new_addr_len += txt_new_deliv_addr_line2.Text.Trim().Length;
            new_addr_len += txt_new_deliv_addr_line3.Text.Trim().Length;
            new_addr_len += txt_new_deliv_addr_line4.Text.Trim().Length;

            return (new_addr_len > 0);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (dlCostCentre.Text == string.Empty)
            {
                lblMsg.Text = "Select a cost centre";
                dlCostCentre.Focus();
                return;

            }

            if (dlIMSLCostCentre.Text == string.Empty)
            {
                lblMsg.Text = "Select an IMSL cost centre";
                dlIMSLCostCentre.Focus();
                return;

            }
            
            if (dlWeldMapping.Text == string.Empty)
            {
                lblMsg2.Text = "Select Weld Mapping Yes/No";
                dlWeldMapping.Focus();
                return;
            }

            if (!is_valid_spool_number())
            {
                lblMsg2.Text = "Invalid spool number. Must end in a number";
                txtSpoolNumber.Focus();
                return;
            }

            string rev = txtRevision.Text.Trim();

            if(rev.Length == 0)
             {
                lblMsg2.Text = "Revision missing";
                txtRevision.Focus();
                return;
            }

            string spool_number = txtContractNumber.Text.Trim()+ "-" + txtSpoolNumber.Text.Trim();

            int id = 0;

            try { id = (int)ViewState["modify_id"]; }
            catch { }

            if (id == 0)
            {
                if (!is_revsion_number_unique(spool_number, rev))
                {
                    lblMsg2.Text = "Revision number for this spool already exists.";
                    txtRevision.Focus();
                    return;
                }
            }
            else
            {
                if (!is_modified_spool_number_unique(spool_number, rev, id))
                {
                    lblMsg2.Text = "This spool/revision already exists.";
                    txtSpoolNumber.Focus();
                    return;
                }
            }

            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                if (m_state == STATE_ENTER_PARTS)
                {
                    int delivery_address_id = 0;

                    if (dlDelAddr.Text == GBE_MEDWAY)
                        delivery_address_id = 0;
                    else if (dlDelAddr.Text == CUST_DELIVERY_ADDR)
                        delivery_address_id = -1;
                    else if (dlDelAddr.Text == NEW_DELIVERY_ADDR)
                    {
                        if (new_delivery_address_entered())
                            delivery_address_id = save_new_delivery_address();
                        else
                        {
                            lblMsg.Text = "New delivery address details missing.";
                            return;
                        }
                        if (delivery_address_id == -100)
                        {
                            lblMsg.Text = "Error saving new delivery address.";
                            return;
                        }
                    }
                    else
                    {
                        if (m_sl_delivery_addresses.ContainsKey(dlDelAddr.Text))
                        {
                            delivery_address_id = (int)(m_sl_delivery_addresses[dlDelAddr.Text]);
                        }
                        else
                        {
                            lblMsg.Text = "Error with delivery address.";
                            return;
                        }
                    }

                    if (tblParts.Rows.Count > 0)
                    {
                        if (save_spool(delivery_address_id))
                        {
                            lblMsg.Text = "Spool: " + spool_number + " Revision: " + rev + " saved.";

                            string confirm_create_another_value = Request.Form["confirm_create_another_value"];

                            if (confirm_create_another_value == "Yes")
                            {
                                ViewState["modify_id"] = 0; // if the previous was a modify, this is now a new spool

                                string spn = txtSpoolNumber.Text.Trim();

                                string [] a_spn = new  string[2] {string.Empty, string.Empty};
                                int n = 0;

                                for (int i = spn.Length - 1; i >= 0; i--)
                                {
                                    if (n == 0 && !Char.IsDigit(spn[i]))
                                        n++;

                                    a_spn[n] = spn[i] + a_spn[n];
                                }

                                int next = 0;

                                int l = a_spn[0].Length;
                                string f = string.Empty;

                                for (int i = 0; i < l; i++)
                                    f += "0";

                                try { next = Convert.ToInt32(a_spn[0]); next++; }
                                catch { }

                                txtSpoolNumber.Text = a_spn[1] + next.ToString(f);

                                foreach (TableRow r in tblParts.Rows)
                                {
                                    if (r.Attributes["spd_id"] != null)
                                        r.Attributes.Remove("spd_id");
                                }
                    
                                foreach (part_data pd in m_parts_list)
                                {
                                    if (pd.attributes.ContainsKey("spd_id"))
                                        pd.attributes.Remove("spd_id");
                                }
            
                                ViewState["parts_list"] = m_parts_list;

                                lblMsg2.Text = "Spool number has been incremented. Amend if necessary and save.";
                            }
                            else
                            {
                                clear_controls();
                                display_parts_controls(false);
                                display_customer_controls(false);
                                display_new_delivery_address_controls(false);
                                m_state = STATE_ENTER_CONTRACT_NO;
                                txtSpoolNumber.ReadOnly = false;
                                txtRevision.ReadOnly = false;
                                txtSpoolNumber.Focus();
                                ViewState["state"] = m_state;
                                m_parts_list.Clear();
                                ViewState["parts_list"] = m_parts_list;
                            }

                            txtDrawing.Text = string.Empty;
                            Session["FileUpload1"] = null;
                        }
                        else
                            lblMsg.Text = "Error saving spool.";
                    }
                    else
                    {
                        lblMsg2.Text = "Parts missing.";
                        txtPartNo.Focus();
                    }
                }
            }
        }

        bool save_spool(int delivery_address)
        {
            int id = 0;

            try { id = (int)ViewState["modify_id"]; }
            catch { }

            SortedList sl = new SortedList();

            sl.Add("id", id);
            sl.Add("spool", txtContractNumber.Text.Trim() + "-" + txtSpoolNumber.Text.Trim());
            sl.Add("revision", txtRevision.Text.Trim());
            sl.Add("barcode", txtContractNumber.Text.Trim() + "-" + txtSpoolNumber.Text.Trim() + "_" + txtRevision.Text.Trim());
            sl.Add("delivery_address", delivery_address);
            sl.Add("cost_centre", dlCostCentre.SelectedIndex);

            sl.Add("material", dlMaterial.Text);

            int imsl_cost_centre = 0;

            if(m_sl_imsl_cost_centres.ContainsKey(dlIMSLCostCentre.Text))
            {
                imsl_cost_centre = (int)m_sl_imsl_cost_centres[dlIMSLCostCentre.Text];
            }

            sl.Add("imsl_cost_centre", imsl_cost_centre);

            bool binclude_in_weld_map = dlWeldMapping.Text.ToLower() == "yes";

            sl.Add("include_in_weld_map", binclude_in_weld_map);

            sl.Add("porder_created", false);

            sl.Add("pipe_size", txtPipeSize.Text.Trim());
            sl.Add("cut_size1", txtCutSize1.Text.Trim());
            sl.Add("cut_size2", txtCutSize2.Text.Trim());
            sl.Add("cut_size3", txtCutSize3.Text.Trim());
            sl.Add("cut_size4", txtCutSize4.Text.Trim());

            int schedule_id = 0;

            if (id == 0) // new spool
            {
                sl.Add("on_hold", false);
                sl.Add("status", "SC");
                sl.Add("picked", false);

                user_data ud = null;

                using (users u = new users())
                {
                    ud = u.get_user_data(System.Web.HttpContext.Current.User.Identity.Name);
                }

                if (ud != null)
                {
                    sl.Add("cad_user_id", ud.id);
                }
                else
                {
                    lblMsg2.Text = "Error retrieving user data";
                }
            }
            else
            {
                using (schedule_fab schd = new schedule_fab())
                {
                    SortedList sl_schd = new SortedList();

                    sl_schd.Add("spool_id", id);

                    ArrayList a_schd = schd.get_schedule_fab_data(sl_schd);

                    if (a_schd.Count > 0)
                    {
                        schedule_fab_data schd_rec = (schedule_fab_data)a_schd[0];

                        schedule_id = schd_rec.id;
                    }
                }
            }
            
            bool bret = true;

            using (spools s = new spools())
            {
                bret = s.save_spool_details(sl, "CREATED", m_user_id);

                if (id == 0) // new spool
                {
                    using (new_spools b = new new_spools())
                    {
                        SortedList slb = new SortedList();
                        slb.Add("spool_id", sl["id"].ToString());
                        slb.Add("printed", false);
                        slb.Add("assembly_type", new_spool_data.SPOOL);

                        bret &= b.save_new_spool_data(slb);
                    }

                    using (schedule_fab schd = new schedule_fab())
                    {
                        SortedList sl_schd = new SortedList();
                        
                        sl_schd.Add("spool_id", sl["id"].ToString());
                        DateTime dt_fab = DateTime.ParseExact(schedule_fab_data.INIT_DATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        sl_schd.Add("dt", dt_fab);

                        int slots = 0;

                        sl_schd.Add("slots", slots);

                        schd.save_schedule_fab_data(sl_schd);

                        try { schedule_id = (int)sl_schd["id"]; }
                        catch { }
                    }
                }

                if (txtDrawing.Text.Trim().Length > 0 && FileUpload1.HasFile)
                {
                    int spool_id = (int)sl["id"];
                    SortedList sl_drawing = new SortedList();

                    sl_drawing.Add("spool_id", spool_id);

                    if (Path.GetExtension(FileUpload1.FileName).ToLower() == ".pdf")
                    {
                        string filename = Path.GetFileName(FileUpload1.FileName);

                        string dir = Server.MapPath("~/") + "\\temp";

                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        string save_name = get_unique_file_name(dir, ".pdf");

                        FileUpload1.SaveAs(save_name);

                        sl_drawing.Add("pdf", File.ReadAllBytes(save_name));

                        s.save_drawing(sl_drawing);

                        int drawing_id = (int)sl_drawing["id"];

                        SortedList sl_spool = new SortedList();

                        sl_spool.Add("id", spool_id);
                        sl_spool.Add("drawing_id", drawing_id);

                        s.save_spool_details(sl_spool, "UPD DRAWING ID", m_user_id);

                        File.Delete(save_name);
                    }
                }

                SortedList slp = new SortedList();
                int  fw, bw;
                decimal qty;
                TextBox tb = null;

                using (spool_parts sp = new spool_parts())
                {
                    if (id != 0)
                    {
                        sp.delete_parts(id);
                    }

                    bool b_all_inhouse = true;

                    int spool_id = (int)sl["id"];

                    int sched_slots = 1;

                    int iseq = 0;

                    foreach (TableRow r in tblParts.Rows)
                    {
                        iseq++;

                        if (r.Attributes["slots"] != null)
                        {
                            int slots = 0;

                            try { slots = Convert.ToInt32(r.Attributes["slots"].ToString()); }
                            catch { }

                            if (slots > sched_slots)
                                sched_slots = slots;
                        }

                        if (r.Attributes["part_id"] != null)
                        {
                            slp.Clear();

                            /*
                            if (r.Attributes["spd_id"] != null)
                                slp.Add("id", r.Attributes["spd_id"]);
                            else
                                slp.Add("include_in_weld_map", true);
                             */

                            if (r.Attributes["spd_id"] == null)
                                slp.Add("include_in_weld_map", true);

                            slp.Add("spool_id", spool_id);
                            slp.Add("part_id", r.Attributes["part_id"]);

                            qty = fw = bw = 0;

                            foreach (TableCell cell in r.Cells)
                            {
                                foreach (Control c in cell.Controls)
                                {
                                    if (c.ID != null)
                                    {
                                        if (c.ID.StartsWith("qty"))
                                        {
                                            tb = ((TextBox)c);

                                            try { qty = Convert.ToDecimal(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith("fw"))
                                        {
                                            tb = ((TextBox)c);

                                            try { fw = Convert.ToInt32(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith("bw"))
                                        {
                                            tb = ((TextBox)c);

                                            try { bw = Convert.ToInt32(tb.Text.Trim()); }
                                            catch { }
                                        }
                                    }
                                }
                            }

                            if (qty > 0)
                            {
                                bool b_picked = false;

                                if (r.Attributes["source"] != null)
                                    b_picked = r.Attributes["source"].ToString() == "1";

                                b_all_inhouse &= b_picked;

                                slp.Add("qty", qty);
                                slp.Add("fw", fw);
                                slp.Add("bw", bw);
                                slp.Add("picked", b_picked);
                                slp.Add("seq", iseq);

                                if (r.Attributes["include_in_weld_map"] != null)
                                    slp.Add("include_in_weld_map", r.Attributes["include_in_weld_map"]);

                                if(r.Attributes["welder"] != null)
                                    slp.Add("welder", r.Attributes["welder"]);

                                sp.save_spool_parts_data(slp);
                            }
                        }
                    }

                    if (schedule_id > 0)
                    {
                        using (schedule_fab schd = new schedule_fab())
                        {
                            SortedList sl_schd = new SortedList();

                            sl_schd.Add("id", schedule_id);

                            if (dlMaterial.Text.Trim() == "Stainless Steel")
                                sched_slots = 0;

                            sl_schd.Add("slots", sched_slots);

                            schd.save_schedule_fab_data(sl_schd);
                        }
                    }

                    if (b_all_inhouse)
                    {
                        sl.Clear();
                        sl.Add("id", spool_id);
                        sl.Add("picked", true);
                        s.save_spool_details(sl, "UPDATED", m_user_id);
                    }
                }
               
            }

            return bret;
        }

        protected void paint_part_table()
        {
            System.Drawing.Color bc;
            int i = 0;
            foreach (TableRow r in tblParts.Rows)
            {
                if (i++ % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("WhiteSmoke");

                r.BackColor = bc;

                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID != null)
                        {
                            if (cntrl.GetType() == typeof(LiteralControl))
                            {
                                LiteralControl lc = (LiteralControl)cntrl;

                                if (lc.ID.StartsWith("seq"))
                                {
                                    lc.Text = i.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void add_part_to_table(part_data pd, spool_part_data spd)
        {
            const string UID = "uid";

            if (!pd.attributes.ContainsKey(UID))
                pd.attributes.Add(UID, pd.part_number + Guid.NewGuid().ToString("N"));

            /* hs 20221116
            foreach (TableRow r0 in tblParts.Rows)
            {
                if(pd.description.ToUpper() == ((LiteralControl)r0.Cells[1].Controls[0]).Text.ToUpper())
                {
                    if (pd.description.ToUpper().Contains("PIPE"))
                        return;

                    
                    string sqty = ((TextBox)r0.Cells[2].Controls[1]).Text;

                    int qty = 0;

                    try { qty = Convert.ToInt32(sqty); }
                    catch { }

                    qty++;

                    ((TextBox)r0.Cells[2].Controls[1]).Text = qty.ToString();

                    return;
                    
                }
            }
            */
    
            

            TableRow r;
            TableCell c;

            r = new TableRow();

            r.Attributes["part_id"] = pd.id.ToString();
            r.Attributes["source"] = pd.source.ToString();

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Right;

            LiteralControl lc = new LiteralControl();
            lc.ID = "seq_" + Guid.NewGuid().ToString("N");
            lc.Text = (tblParts.Rows.Count + 1).ToString();

            c.Controls.Add(lc);
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(pd.description));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(pd.description.ToUpper().Contains("PIPE")?"Len (m):":"Qty:"));
            TextBox qtb = null;

            if (pd.description.ToUpper().Contains("PIPE"))
                qtb = create_decimal_textbox("qty_" + pd.attributes[UID].ToString());
            else
            {
                qtb = create_numeric_textbox("qty_" + pd.attributes[UID].ToString());
                qtb.Text = "1";
                qtb.Enabled = false;
            }

            if (spd != null)
            {
                qtb.Text = spd.qty.ToString();
            }

            c.Controls.Add(qtb);
            r.Cells.Add(c);

            TextBox fwtb = create_numeric_textbox("fw_" + pd.attributes[UID].ToString());
            if (spd != null)
                fwtb.Text = spd.fw.ToString();

            c = new TableCell();
            c.Controls.Add(new LiteralControl("FW:"));
            c.Controls.Add(fwtb);
            r.Cells.Add(c);

            TextBox bwtb = create_numeric_textbox("bw_" + pd.attributes[UID].ToString());
            if (spd != null)
                bwtb.Text = spd.bw.ToString();

            c = new TableCell();
            c.Controls.Add(new LiteralControl("BW:"));
            c.Controls.Add(bwtb);
            r.Cells.Add(c);

            c = new TableCell();

            if (pd.supplier.Trim().ToLower() == "imsl")
            {
                decimal stock_count = ws().get_stock_count("eye_emm_ess_ell", pd.description);
                
                c.Controls.Add(new LiteralControl("IMSL Stock:" + stock_count.ToString("0")));
                
                if(stock_count == 0)
                    c.BackColor = System.Drawing.Color.FromName("Red");
            }

            r.Cells.Add(c);

            ImageButton btn_remove_part = new ImageButton();
            btn_remove_part.Click+= new ImageClickEventHandler(btn_remove_part_Click);
           // btn_remove_part.Text = "X";
            btn_remove_part.ImageUrl = "~/delete.png";
            btn_remove_part.ToolTip = "Remove part";
            btn_remove_part.ID = "btn_remove_part_" + pd.attributes[UID].ToString(); // tblParts.Rows.Count.ToString();
            btn_remove_part.Attributes[UID] = pd.attributes[UID].ToString();
                                    
            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Controls.Add(btn_remove_part);
            r.Cells.Add(c);

            r.Attributes[UID] = pd.attributes[UID].ToString();

            r.Attributes["slots"] = schedule_fab.get_slots(pd).ToString();

            if (pd.attributes.ContainsKey("spd_id"))
                r.Attributes["spd_id"] = pd.attributes["spd_id"].ToString();

            if (pd.attributes.ContainsKey("include_in_weld_map"))
                r.Attributes["include_in_weld_map"] = pd.attributes["include_in_weld_map"].ToString();

            if (pd.attributes.ContainsKey("welder"))
                r.Attributes["welder"] = pd.attributes["welder"].ToString();

            string uid = Request.QueryString[UID];

            tblParts.Rows.Add(r);
                
            paint_part_table();
        }

        TextBox create_numeric_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 45;
            tb.MaxLength = 3;
            tb.Text = "0";
            tb.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tb.ID = id;

            return tb;
        }

        TextBox create_decimal_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 50;
            tb.MaxLength = 8;
            tb.Text = string.Empty;
            tb.Attributes.Add("onkeypress", "return onlyDotsAndNumbers(this, event)");
            tb.ID = id;

            return tb;
        }

        protected void show_parts_list()
        {
            tblParts.Rows.Clear();

            int i = 0;

            // hs. 20221115
            /*
            SortedList sl_parts_list = new SortedList();

            foreach (part_data pd in m_parts_list)
            {
                sl_parts_list.Add(pd.description+sl_parts_list.Count.ToString(), pd);
            }
            */

            //foreach(DictionaryEntry e0 in sl_parts_list)
            foreach(part_data pd in m_parts_list)
            {
                //part_data pd = (part_data)e0.Value;
                add_part_to_table(pd, null);
                i++;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPartNo.Text.Trim().Length > 0)
            {
                SortedList sl = new SortedList();
                sl.Add("description", txtPartNo.Text.Trim());
                sl.Add("active", true);

                ArrayList a = new ArrayList();

                using (parts p = new parts())
                {
                    a = p.get_part_data(sl);
                }

                if (a.Count > 0)
                {
                    txtPartNo.Text = string.Empty;

                    part_data pd = (part_data)a[0];

                    if (pd.active)
                    {

                        bool b_already_there = false;

                        /* hs 20221114
                        foreach (part_data pd0 in m_parts_list)
                        {
                            if (pd0.description == pd.description)
                            {
                                pd = pd0;
                                b_already_there = true;
                                break;
                            }
                        }
                        */

                        add_part_to_table(pd, null);

                        if (!b_already_there)
                            m_parts_list.Add(pd);

                        ViewState["parts_list"] = m_parts_list;
                    }
                }
            }

            txtPartNo.Focus();
        }

        void btn_remove_part_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);

            ArrayList del_i = new ArrayList();
            int i = 0;

            foreach (TableRow r in tblParts.Rows)
            {
                if (r.Attributes["uid"] == uid)
                    del_i.Add(i);

                i++;
            }

            del_i.Reverse();

            foreach (int n in del_i)
            {
                tblParts.Rows.RemoveAt(n);
            }

            del_i.Clear();
            i = 0;

            foreach (part_data pd in m_parts_list)
            {
                if(pd.attributes["uid"].ToString() == uid)
                    del_i.Add(i);

                 i++;
            }

            del_i.Reverse();

            foreach (int n in del_i)
            {
                m_parts_list.RemoveAt(n);
            }

            paint_part_table();

            ViewState["parts_list"] = m_parts_list;
        }

        protected void dlDelAddr_SelectedIndexChanged(object sender, EventArgs e)
        {
            display_new_delivery_address_controls(dlDelAddr.Text == NEW_DELIVERY_ADDR);
        }

        string get_unique_file_name(string path, string ext)
        {
            string fnname = string.Empty;

            if (!path.EndsWith("\\"))
                path += "\\";

            Random r = new Random();

            do
            {
                fnname = path + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + "_" + r.Next(1, 10000).ToString("00000") + ext;
            } while (File.Exists(fnname));

            return fnname;
        }

        static imsl_ws.imsl_ws ws()
        {
            string imsl_ws_url = System.Web.Configuration.WebConfigurationManager.AppSettings["imsl_ws_url"].ToString();

            imsl_ws.imsl_ws ws = new imsl_ws.imsl_ws();

            ws.Url = imsl_ws_url;
            ws.Timeout = 1 * 60 * 1000;

            return ws;
        }
    }
}
