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
using System.Web.Services;

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
        const string VS_IMSL_FAB_ORDERS = "vs_imsl_fab_orders";
        const string VS_STATE = "vs_state";
        const string PARTS_LIST = "parts_list";
        const string UID = "uid";
        const string MODIFY_ID = "modify_id";
        const string FILEUPLOAD_SESSION = "FileUploadSession";
                
        /*
        const string ADDITIONAL_CUT1 = "additional_cut1_";
        const string ADDITIONAL_CUT2 = "additional_cut2_";
        const string ADDITIONAL_CUT3 = "additional_cut3_";
        */

        const string ADDITIONAL_INFO = "additional_info_";
        
        const string PIPE_CUT_LENGTH_PART_NO = "pipe_cut_length_part_no";
        const string IS_PIPE = "is_pipe";
        const string QTY = "qty_";
        const string F1 = "f1_";
        const string F2 = "f2_";
        
        int m_state = 0;
        ArrayList m_parts_list = new ArrayList();
        ArrayList m_customer_controls = new ArrayList();
        ArrayList m_parts_controls = new ArrayList();
        ArrayList m_new_delivery_address_controls = new ArrayList();

        SortedList m_sl_delivery_addresses = new SortedList();
        SortedList m_sl_imsl_cost_centres = new SortedList();

        SortedList m_sl_cut_length_data = new SortedList();  

        string m_user_id = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        
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
            m_customer_controls.Add(txtEmail2); 
            m_customer_controls.Add(txtContactName);
            m_customer_controls.Add(lblCustomer);
            m_customer_controls.Add(lblAddr1);
            m_customer_controls.Add(lblAddr2);
            m_customer_controls.Add(lblAddr3);
            m_customer_controls.Add(lblAddr4);
            m_customer_controls.Add(lblPhone);
            m_customer_controls.Add(lblEmail);
            m_customer_controls.Add(lblEmail2);
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
            m_parts_controls.Add(dlStatus);
            m_parts_controls.Add(lblPartNo);
            m_parts_controls.Add(txtPartNo);
            m_parts_controls.Add(btnAdd);
            m_parts_controls.Add(lblSelected);
            m_parts_controls.Add(tblParts);

            m_parts_controls.Add(txtDrawing);
             

            m_parts_controls.Add(lblFabNumber);
            m_parts_controls.Add(txtFabNumber);

            if (IsPostBack)
            {
                m_state = (int)ViewState[VS_STATE];
                m_parts_list = (ArrayList)ViewState[PARTS_LIST];
                m_sl_delivery_addresses = (SortedList)ViewState["delivery_addresses"];
                m_sl_imsl_cost_centres = (SortedList)ViewState[VS_IMSL_COST_CENTRES];

                show_parts_list();

                if (FileUpload1.HasFile)
                {
                    Session[FILEUPLOAD_SESSION] = FileUpload1;
                }
                else
                {
                    if (Session[FILEUPLOAD_SESSION] != null)
                    {
                        FileUpload1 = (FileUpload)Session[FILEUPLOAD_SESSION];
                    }
                }

                
            }
            else
            {
                clear_controls();

                using (spool_status ss = new spool_status())
                {
                    ArrayList a = ss.get_spool_status_data(new SortedList());

                    foreach (spool_status_data ssd in a)
                        dlStatus.Items.Add(ssd.status);
                }

                dlStatus.Text = "SC";

                txtDrawing.Attributes.Add("readonly", "readonly");
                 

                dlWeldMapping.Items.Add(string.Empty);
                dlWeldMapping.Items.Add("Yes");
                dlWeldMapping.Items.Add("No");

                dlMaterial.Items.Add(material.MAT_NA);
                dlMaterial.Items.Add(material.MAT_CARBON_STEEL);
                dlMaterial.Items.Add(material.MAT_STAINLESS_STEEL);
                dlMaterial.Items.Add(material.MAT_SCREWED);
                dlMaterial.Items.Add(material.MAT_HS);

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

                        if (!m_sl_imsl_cost_centres.ContainsKey(dr_cfm.name))
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
                    btnNewSpool.Visible = true;
                    btnCopySpool.Visible = true;

                    hlSpoolsView.NavigateUrl += Request.Url.Query;
                    ViewState["search_string"] = Request.QueryString["s"];
                    ViewState["status_index"] = Request.QueryString["u"];
                    ViewState["field_index"] = Request.QueryString["f"];
                    ViewState["on_hold"] = Request.QueryString["h"];
                    ViewState["page_no"] = Request.QueryString["p"];

                    string prev_uid = get_prev_id(uid);
                    string next_uid = get_next_id(uid);

                    hlPrevSpool.NavigateUrl = "~/create_spool.aspx" + Request.Url.Query.Replace("uid=" + uid, "uid=" + prev_uid);
                    hlNextSpool.NavigateUrl = "~/create_spool.aspx" + Request.Url.Query.Replace("uid=" + uid, "uid=" + next_uid);

                    populate_for_edit(uid);
                }
                else
                {
                    btnNewSpool.Visible = false;
                    btnCopySpool.Visible = false;

                    hlSpoolsView.Visible = false;
                    hlNextSpool.Visible = false;
                    hlPrevSpool.Visible = false;

                    m_state = STATE_ENTER_CONTRACT_NO;
                    ViewState[VS_STATE] = m_state;

                    m_parts_list = new ArrayList();
                    store_parts_list();

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

        void store_parts_list()
        {
            ViewState[PARTS_LIST] = m_parts_list;
            HttpContext.Current.Session[PARTS_LIST] = m_parts_list;
        }

        string get_cad_user_login_id(int spool_id)
        {
            string cad_user_login_id = string.Empty;

            using (cdb_connection dbc = new cdb_connection())
            {
                string sql = $@"
                            select 
                            login_id 
                            from spools 
                            join users on cad_user_id = users.id
                            where spools.id = {spool_id} 
                            ";

                DataTable dtab = dbc.get_data(sql);

                if (dtab != null && dtab.Rows.Count > 0)
                {
                    DataRow dr = dtab.Rows[0];
                    cad_user_login_id = dr["login_id"].ToString();
                }
            }

            return cad_user_login_id;
        }

        string get_prev_id(string current_id)
        {
            string id = string.Empty;

            using (cdb_connection dbc = new cdb_connection())
            {
                string sql = $@"select top 1 id 
                            from spools 
                            where barcode < 
                            (select barcode from spools where id = {current_id})
                            ORDER BY barcode desc ";
                                
                DataTable dtab = dbc.get_data(sql);

                if (dtab != null && dtab.Rows.Count > 0)
                {
                    DataRow dr = dtab.Rows[0];
                    id = dr["id"].ToString();
                }
                else
                {
                    id = current_id;
                }
            }

            return id;
        }

        string get_next_id(string current_id)
        {
            string id = string.Empty;

            using (cdb_connection dbc = new cdb_connection())
            {
                string sql = $@"select top 1  id 
                            from spools 
                            where barcode > 
                            (select barcode from spools where id = {current_id})
                            ORDER BY barcode asc ";
                                
                DataTable dtab = dbc.get_data(sql);

                if (dtab != null && dtab.Rows.Count > 0)
                {
                    DataRow dr = dtab.Rows[0];
                    id = dr["id"].ToString();;
                }
                else
                {
                    id = current_id;
                }
            }
            return id;
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

                            if (m_state == STATE_ENTER_CONTACT)
                            {
                                ViewState[VS_STATE] = m_state;
                                return;
                            }

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

                            dlStatus.Text = sd.status;

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

                            if (sd.fab_order_id > 0)
                            {
                                imsl_ws.fab_order_details fod0 = ws().get_fab_order("eye_emm_ess_ell", sd.fab_order_id);

                                txtFabNumber.Text = fod0.fab_number.ToString();

                                get_fab_details(fod0.fab_number.ToString());

                                object[] a_fod = (object[])ViewState[VS_IMSL_FAB_ORDERS];

                                int i = 0;
                                foreach (imsl_ws.fab_order_details fod in a_fod)
                                {
                                    if (fod.fab_order_id == fod0.fab_order_id)
                                    {
                                        dlFabPO.SelectedIndex = i;
                                    }

                                    i++;
                                }
                            }

                            show_parts_list(sd.spool_part_data);

                            m_state = STATE_ENTER_PARTS;
                            ViewState[VS_STATE] = m_state;

                            ViewState[MODIFY_ID] = id;

                            paint_part_table();
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
            dlMaterial.Text = material.MAT_NA;
            dlFabPO.Items.Clear();
            lblFabDetails.Text = string.Empty;

            txtContractNumber.Text = string.Empty;
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
                    txtEmail2.Text = cd.email2;
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
            sl.Add("email_additional", txtEmail2.Text.Trim());
            sl.Add("active", true);

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


                string uid = Request.QueryString["uid"];

                if (uid != null)
                {
                    populate_for_edit(uid);
                }

                txtFabNumber.Focus();
            }

            ViewState[VS_STATE] = m_state;
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

            try { id = (int)ViewState[MODIFY_ID]; }
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
                            lblMsg2.Text = lblMsg.Text;

                            txtDrawing.Text = string.Empty;
                             
                            Session[FILEUPLOAD_SESSION] = null;
                             

                            btnNewSpool.Visible = true;
                            btnCopySpool.Visible = true;

                            int spool_id = (int)ViewState[MODIFY_ID];

                            ArrayList a_spd = get_spool_part_data(spool_id);
                
                            show_parts_list(a_spd);

                            paint_part_table();
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
               

        void init_for_new_spool()
        {
            Response.Redirect("create_spool.aspx");
        }

        void init_for_copy_spool()
        {
            btnNewSpool.Visible = false;
            btnCopySpool.Visible = false;

            ViewState[MODIFY_ID] = 0; // if the previous was a modify, this is now a new spool

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
            
            store_parts_list();

            if(dlStatus.Text == "A0")
                dlStatus.Text = "SC";

            lblMsg2.Text = "Spool number has been incremented. Amend if necessary and save.";
        }

        bool save_spool(int delivery_address)
        {
            int id = 0;

            try { id = (int)ViewState[MODIFY_ID]; }
            catch { }

            SortedList sl = new SortedList();

            sl.Add("id", id);
            sl.Add("spool", txtContractNumber.Text.Trim() + "-" + txtSpoolNumber.Text.Trim());
            sl.Add("revision", txtRevision.Text.Trim());
            sl.Add("barcode", txtContractNumber.Text.Trim() + "-" + txtSpoolNumber.Text.Trim() + "_" + txtRevision.Text.Trim());
            sl.Add("delivery_address", delivery_address);
            sl.Add("cost_centre", dlCostCentre.SelectedIndex);

            sl.Add("status", dlStatus.Text);

            sl.Add("material", dlMaterial.Text);

            int imsl_cost_centre = 0;

            if(m_sl_imsl_cost_centres.ContainsKey(dlIMSLCostCentre.Text))
            {
                imsl_cost_centre = (int)m_sl_imsl_cost_centres[dlIMSLCostCentre.Text];
            }

            sl.Add("imsl_cost_centre", imsl_cost_centre);

            bool binclude_in_weld_map = dlWeldMapping.Text.ToLower() == "yes";

            sl.Add("include_in_weld_map", binclude_in_weld_map);

            sl.Add("pipe_size", txtPipeSize.Text.Trim());
            sl.Add("cut_size1", txtCutSize1.Text.Trim());
            sl.Add("cut_size2", txtCutSize2.Text.Trim());
            sl.Add("cut_size3", txtCutSize3.Text.Trim());
            sl.Add("cut_size4", txtCutSize4.Text.Trim());

            if (dlFabPO.Items.Count > 0)
            {
                object[] a_fod = (object[])ViewState[VS_IMSL_FAB_ORDERS];

                imsl_ws.fab_order_details fod = (imsl_ws.fab_order_details)a_fod[dlFabPO.SelectedIndex];

                sl.Add("fab_order_id", fod.fab_order_id);
            }
            else
                sl.Add("fab_order_id", 0);

            int schedule_id = 0;

            user_data ud = null;

            using (users u = new users())
            {
                ud = u.get_user_data(System.Web.HttpContext.Current.User.Identity.Name);
            }

            if (id == 0) // new spool
            {
                sl.Add("porder_created", false);
                sl.Add("on_hold", false);
                sl.Add("picked", false);

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
                string cad_user_login_id = get_cad_user_login_id(id);

                if(cad_user_login_id == "A0" || cad_user_login_id == "A0Fab")
                    sl.Add("cad_user_id", ud.id);

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
                int  fw, bw, f1, f2;
                decimal qty;
                //decimal additional_cut1, additional_cut2, additional_cut3;
                string additional_info;

                TextBox tb = null;

                int saved_spool_id = (int)sl["id"];

                using (spool_parts sp = new spool_parts())
                {
                    if (id != 0)
                    {
                        sp.delete_parts(id);
                    }

                    bool b_all_inhouse = true;

                    int sched_slots = 1;

                    int iseq = 0;

                    SortedList sl_seq_partid = new SortedList();

                    foreach (TableRow r in tblParts.Rows)
                    {
                        if (r.Attributes["part_id"] != null)
                        {
                            foreach (TableCell cell in r.Cells)
                            {
                                foreach (Control cntrl in cell.Controls)
                                {
                                    if (cntrl.ID != null)
                                    {
                                        if (cntrl.GetType() == typeof(LiteralControl))
                                        {
                                            LiteralControl lc = (LiteralControl)cntrl;

                                            if (lc.ID.StartsWith("seq"))
                                            {
                                                try
                                                {
                                                    sl_seq_partid.Add(Convert.ToInt32(lc.Text), r.Attributes["part_id"]);
                                                }
                                                catch { }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

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

                            if (r.Attributes["spd_id"] == null)
                                slp.Add("include_in_weld_map", true);

                            slp.Add("spool_id", saved_spool_id);
                            slp.Add("part_id", r.Attributes["part_id"]);

                            qty = fw = bw = f1 = f2 = 0;
                            //additional_cut1 = additional_cut2 = additional_cut3 = 0;
                            additional_info = string.Empty;

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
                                        
                                        if (c.ID.StartsWith("f1"))
                                        {
                                            tb = ((TextBox)c);

                                            try { f1 = Convert.ToInt32(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith("f2"))
                                        {
                                            tb = ((TextBox)c);

                                            try { f2 = Convert.ToInt32(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith(ADDITIONAL_INFO))
                                        {
                                            tb = ((TextBox)c);
                                            additional_info = tb.Text.Trim();
                                        }

                                        /*
                                        if (c.ID.StartsWith(ADDITIONAL_CUT1))
                                        {
                                            tb = ((TextBox)c);

                                            try { additional_cut1 = Convert.ToDecimal(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith(ADDITIONAL_CUT2))
                                        {
                                            tb = ((TextBox)c);

                                            try { additional_cut2 = Convert.ToDecimal(tb.Text.Trim()); }
                                            catch { }
                                        }

                                        if (c.ID.StartsWith(ADDITIONAL_CUT3))
                                        {
                                            tb = ((TextBox)c);

                                            try { additional_cut3 = Convert.ToDecimal(tb.Text.Trim()); }
                                            catch { }
                                        }
                                        */
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

                                const string INCLUDE_IN_WELD_MAP = "include_in_weld_map";
                                if (r.Attributes[INCLUDE_IN_WELD_MAP] != null)
                                {
                                    if(slp.ContainsKey(INCLUDE_IN_WELD_MAP))
                                        slp[INCLUDE_IN_WELD_MAP] = r.Attributes[INCLUDE_IN_WELD_MAP];
                                    else
                                        slp.Add(INCLUDE_IN_WELD_MAP, r.Attributes[INCLUDE_IN_WELD_MAP]);
                                }

                                const string WELDER = "welder";
                                if (r.Attributes[WELDER] != null)
                                {
                                    if(slp.ContainsKey(WELDER))
                                        slp[WELDER] = r.Attributes[WELDER];
                                    else
                                        slp.Add(WELDER, r.Attributes[WELDER]);
                                }

                                sp.save_spool_parts_data(slp);

                                if (r.Attributes[IS_PIPE] != null)
                                {
                                    int spool_part_id = Convert.ToInt32(slp["id"]);
                                    slp.Clear();

                                    slp.Add("spool_part_id", spool_part_id);
                                    slp.Add("spool_id", saved_spool_id);
                                    slp.Add("fitting_1_seq_no", f1);
                                    slp.Add("fitting_2_seq_no", f2);

                                    if (sl_seq_partid.ContainsKey(f1))
                                    {
                                        slp.Add("fitting_1_part_id", sl_seq_partid[f1]);
                                    }

                                    if (sl_seq_partid.ContainsKey(f2))
                                    {
                                        slp.Add("fitting_2_part_id", sl_seq_partid[f2]);
                                    }     

                                    /*
                                    slp.Add("additional_cut1", additional_cut1);
                                    slp.Add("additional_cut2", additional_cut2);
                                    slp.Add("additional_cut3", additional_cut3);
                                    */

                                     slp.Add("additional_info", additional_info);

                                    sp.save_spool_pipe_fittings_data(slp);
                                }
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
                        sl.Add("id", saved_spool_id);
                        sl.Add("picked", true);
                        s.save_spool_details(sl, "UPDATED", m_user_id);
                    }
                }

                

                ViewState[MODIFY_ID] = saved_spool_id;
            }

            return bret;
        }

        ArrayList get_spool_part_data(int spool_id)
        {
            ArrayList a = new ArrayList();

            using (spool_parts sp = new spool_parts())
            {
                SortedList sl = new SortedList();

                sl.Add("spool_id", spool_id);

                a = sp.get_spool_parts_data_ex(sl);
            }

            return a;
        }

        protected void paint_part_table()
        {
            ListItem [] seq_range = new ListItem[m_parts_list.Count];
            
            for(int iseq = 0; iseq < m_parts_list.Count; iseq++)
                seq_range[iseq] = new ListItem((iseq + 1).ToString());

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

            TableRow r;
            TableCell c;

            r = new TableRow();

            r.Attributes["part_id"] = pd.id.ToString();
            r.Attributes["source"] = pd.source.ToString();

            ImageButton btn_move_part_down = new ImageButton();
            btn_move_part_down.ImageUrl = "~/down.png";
            btn_move_part_down.ToolTip = "Move part down";
            btn_move_part_down.ID = "btn_move_part_down_" + pd.attributes[UID].ToString();  
            btn_move_part_down.Attributes[UID] = pd.attributes[UID].ToString();
            btn_move_part_down.Click +=  new ImageClickEventHandler(btn_move_part_down_Click);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Controls.Add(btn_move_part_down);
            r.Cells.Add(c);

            ImageButton btn_move_part_up = new ImageButton();
            btn_move_part_up.ImageUrl = "~/up.png";
            btn_move_part_up.ToolTip = "Move part up";
            btn_move_part_up.ID = "btn_move_part_up_" + pd.attributes[UID].ToString();  
            btn_move_part_up.Attributes[UID] = pd.attributes[UID].ToString();
            btn_move_part_up.Click += new ImageClickEventHandler(btn_move_part_up_Click);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Controls.Add(btn_move_part_up);
            r.Cells.Add(c);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Right;

            string guid = Guid.NewGuid().ToString("N");

            LiteralControl lc = new LiteralControl();
            lc.ID = "seq_" + guid;
            lc.Text = (tblParts.Rows.Count + 1).ToString("00");

            c.Controls.Add(lc);
            r.Cells.Add(c);

            c = new TableCell();
            Label lbldesc = new Label();
            lbldesc.ID = "part_desc_" + guid;
            lbldesc.Width = 500;
            lbldesc.Text = pd.description;
            c.Controls.Add(lbldesc);
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl((pd.description.ToUpper().Contains("PIPE")|| pd.part_type.ToUpper().Contains("PIPE")) ?"Len (m):":"Qty:"));
            TextBox qtb = null;

            bool bpipe = is_pipe(pd);

            if (bpipe)
            {
                qtb = create_decimal_textbox_calc_cut_length(QTY, pd.attributes[UID].ToString());

                r.Attributes[IS_PIPE] = "1";
            }
            else
            {
                qtb = create_numeric_textbox(QTY + pd.attributes[UID].ToString());
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
                
                c.Controls.Add(new LiteralControl("Stock:" + stock_count.ToString("0")));
                
                if(stock_count == 0)
                    c.BackColor = System.Drawing.Color.FromName("Red");
            }

            r.Cells.Add(c);

            if (bpipe)
            {
                TextBox f1tb = create_numeric_textbox_calc_cut_length(F1, pd.attributes[UID].ToString());

                if (spd != null)
                    if(spd.spool_pipe_fittings_data != null)
                        f1tb.Text = spd.spool_pipe_fittings_data.fitting_1_seq_no.ToString();

                c = new TableCell();
                c.Controls.Add(new LiteralControl("F1:"));
                c.Controls.Add(f1tb);
                r.Cells.Add(c);

                TextBox f2tb = create_numeric_textbox_calc_cut_length(F2, pd.attributes[UID].ToString());

                if (spd != null)
                    if(spd.spool_pipe_fittings_data != null)
                        f2tb.Text = spd.spool_pipe_fittings_data.fitting_2_seq_no.ToString();

                c = new TableCell();
                c.Controls.Add(new LiteralControl("F2:"));
                c.Controls.Add(f2tb);
                r.Cells.Add(c);

                // _cutlen 
                Label lbl_cut = new Label();
                lbl_cut.ID =  "cut_" + pd.attributes[UID].ToString();
                
                c = new TableCell();
                c.Controls.Add(new LiteralControl("Cut:"));
                c.Controls.Add(lbl_cut);
                r.Cells.Add(c);

                if (spd != null)
                    lbl_cut.Text = CCutLengthData.get_cut_length(spd.cut_length_data).ToString("0.00");
                else
                {
                    
                    lbl_cut.Text = $"<script type='text/javascript'>calcCutLength('{pd.attributes[UID].ToString()}');</script>";
                    
                }
                
                /*
                // additional cut lengths
                TextBox cuttb1 = create_decimal_textbox(ADDITIONAL_CUT1 + pd.attributes[UID].ToString());

                if (spd != null)
                    cuttb1.Text = spd.spool_pipe_fittings_data.additional_cut1.ToString("0.00");

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(cuttb1);
                r.Cells.Add(c);
                    
                TextBox cuttb2 = create_decimal_textbox(ADDITIONAL_CUT2 + pd.attributes[UID].ToString());

                if (spd != null)
                    cuttb2.Text = spd.spool_pipe_fittings_data.additional_cut2.ToString("0.00");

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(cuttb2);
                r.Cells.Add(c);

                TextBox cuttb3 = create_decimal_textbox(ADDITIONAL_CUT3 + pd.attributes[UID].ToString());

                if (spd != null)
                    cuttb3.Text = spd.spool_pipe_fittings_data.additional_cut3.ToString("0.00");

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(cuttb3);
                r.Cells.Add(c);
                */
                
                TextBox tbaddinfo = new TextBox();
                tbaddinfo.ID = ADDITIONAL_INFO + pd.attributes[UID].ToString();
                tbaddinfo.MaxLength= 50;

                if (spd != null)
                    tbaddinfo.Text = spd.spool_pipe_fittings_data.additional_info;

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(tbaddinfo);
                r.Cells.Add(c);
            }
            else
            {
                // cell padding
                for (int i = 0; i < 4; i++)
                {
                    c = new TableCell();
                    r.Cells.Add(c);
                }
            }

            ImageButton btn_remove_part = new ImageButton();
            btn_remove_part.Click+= new ImageClickEventHandler(btn_remove_part_Click);
            btn_remove_part.ImageUrl = "~/delete.png";
            btn_remove_part.ToolTip = "Remove part";
            btn_remove_part.ID = "btn_remove_part_" + pd.attributes[UID].ToString();  
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

            tblParts.Rows.Add(r);
        }

        bool is_pipe(part_data pd) 
        {
            return pd.description.ToUpper().Contains("PIPE") || pd.part_type.ToUpper().Contains("PIPE");
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

        TextBox create_numeric_textbox_calc_cut_length(string prefix, string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 45;
            tb.MaxLength = 3;
            tb.Text = "0";
            tb.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tb.Attributes.Add("onkeyup", $"calcCutLength('{id}')");
            tb.ID = prefix+id;;

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

        TextBox create_decimal_textbox_calc_cut_length(string prefix, string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 50;
            tb.MaxLength = 8;
            tb.Text = string.Empty;
            tb.Attributes.Add("onkeypress", "return onlyDotsAndNumbers(this, event)");
            tb.Attributes.Add("onkeyup", $"calcCutLength('{id}')");
            tb.ID = prefix+id;

            return tb;
        }

        protected void show_parts_list(ArrayList a_spool_part_data)
        {
            tblParts.Rows.Clear();

            if(m_parts_list == null)
                m_parts_list = new ArrayList();
            else
                m_parts_list.Clear();

            foreach (spool_part_data spd in a_spool_part_data)
            {
                if (spd.part_data == null)
                {
                    spd.part_data = new part_data();
                }

                spd.part_data.attributes.Add("spd_id", spd.id);
                spd.part_data.attributes.Add("include_in_weld_map", spd.include_in_weld_map.ToString());
                spd.part_data.attributes.Add("welder", spd.welder);

                add_part_to_table(spd.part_data, spd);

                m_parts_list.Add(spd.part_data);
            }

            store_parts_list();
        }

        protected void show_parts_list()
        {
            tblParts.Rows.Clear();

            if (m_parts_list != null)
            {
                int i = 0;

                foreach (part_data pd in m_parts_list)
                {
                    add_part_to_table(pd, null);
                    i++;
                }

                paint_part_table();
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
                        m_parts_list.Add(pd);

                        store_parts_list();

                        add_part_to_table(pd, null);

                        paint_part_table();
                    }
                }
            }

            txtPartNo.Focus();
        }

        void btn_move_part_down_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes[UID]);

            int i = 0;

            foreach (TableRow r in tblParts.Rows)
            {
                if (r.Attributes[UID] == uid)
                    break;

                i++;
            }

            if (i + 1 < m_parts_list.Count)
            {
                part_data pd = (part_data)m_parts_list[i];
                m_parts_list.RemoveAt(i);
                m_parts_list.Insert(i + 1, pd);
                
                TableRow r = tblParts.Rows[i];
                tblParts.Rows.RemoveAt(i);
                tblParts.Rows.AddAt(i + 1, r);
            }

            store_parts_list();

            paint_part_table();
        }

        void btn_move_part_up_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes[UID]);
            int i = 0;
            foreach (TableRow r in tblParts.Rows)
            {
                if (r.Attributes[UID] == uid)
                    break;

                i++;
            }

            if (i > 0)
            {
                part_data pd = (part_data)m_parts_list[i];
                m_parts_list.RemoveAt(i);
                m_parts_list.Insert(i - 1 , pd);
                
                TableRow r = tblParts.Rows[i];
                tblParts.Rows.RemoveAt(i);
                tblParts.Rows.AddAt(i - 1, r);
            }

            store_parts_list();

            paint_part_table();
        }

        void btn_remove_part_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes[UID]);

            ArrayList del_i = new ArrayList();
            int i = 0;

            foreach (TableRow r in tblParts.Rows)
            {
                if (r.Attributes[UID] == uid)
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

           store_parts_list();

            paint_part_table();
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

        protected void btnFabNumberLookup_Click(object sender, EventArgs e)
        {
            dlFabPO.Items.Clear();
            lblFabDetails.Text = string.Empty;

            if (txtFabNumber.Text.Trim().Length > 0)
            {
                try
                {
                    int ifn = Convert.ToInt32(txtFabNumber.Text.Trim());
                }
                catch
                {
                    lblMsg.Text = "Invalid Fab. number. Numeric only";
                    txtFabNumber.Focus();
                    return;
                }

                get_fab_details(txtFabNumber.Text.Trim());
            }
        }

        void get_fab_details(string fab_number)
        {
            object[] a_fod = ws().get_fab_orders("eye_emm_ess_ell", txtFabNumber.Text.Trim());

            if (a_fod != null && a_fod.Length > 0)
            {
                ViewState[VS_IMSL_FAB_ORDERS] = a_fod;

                imsl_ws.fab_order_details fod0 = (imsl_ws.fab_order_details)a_fod[0];
                    
                lblFabDetails.Text = "Customer: " + fod0.customer + "<br />";
                lblFabDetails.Text += "Project: " + fod0.customer_project + "<br />";

                foreach (imsl_ws.fab_order_details fod in a_fod)
                {
                    if(fod.customer_po_number.Length > 0)
                        dlFabPO.Items.Add(fod.customer_po_number);
                    else
                        dlFabPO.Items.Add("-");
                }
            }
            else
            {
                lblFabDetails.Text = "Not found";
            }
        }

        protected void btnNewSpool_Click(object sender, EventArgs e)
        {
            init_for_new_spool();
        }

        protected void btnCopySpool_Click(object sender, EventArgs e)
        {
            init_for_copy_spool();
        }

        [WebMethod]
        public static string CalcCutLength(string len, string f1, string f2)
        {
            string sret = string.Empty;

            ArrayList parts_list = (ArrayList)HttpContext.Current.Session[PARTS_LIST];
            
            decimal cutLen = 0;

            try { cutLen = Convert.ToDecimal(len); } catch { }

            if (cutLen > 0)
            {
                cutLen *= 1000;

                int iF1, iF2;
                iF1 =  iF2 = -1;

                try {iF1 = Convert.ToInt32(f1)-1; } catch { }
                try {iF2 = Convert.ToInt32(f2)-1; } catch { }

                if (iF1 >= 0 && iF1 < parts_list.Count)
                {
                    part_data pd1 = (part_data)parts_list[iF1];
                    cutLen -= pd1.fitting_size_mm;
                    cutLen -= pd1.gap_mm;
                }

                if (iF2 >= 0 && iF2 < parts_list.Count)
                {
                    part_data pd2 = (part_data)parts_list[iF2];
                    cutLen -= pd2.fitting_size_mm;
                    cutLen -= pd2.gap_mm;
                }
            }

            sret = cutLen.ToString("0.00");

            return sret;
        }

        protected void btnBrowse_Click(object sender, EventArgs e)
        {

        }

        protected void btnBrowseTestRpt_Click(object sender, EventArgs e)
        {

        }
    }
}
