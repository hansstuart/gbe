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
    public partial class order_parts : System.Web.UI.Page
    {
        const string NEW_DELIVERY_ADDR = "Enter new address";
        const string GBE_MEDWAY = "gbe, Whitewall Rd, Medway City Estate";
        const string RETURN_ORDER = "RETURN ORDER";

        const string DESCRIPTION = "Description";
        const string TYPE = "Type";
        const string PART_NO = "Part No.";
        const string SUPPLIER = "Supplier";

        const string FLD_DESCRIPTION = "description";
        const string FLD_TYPE = "part_type";
        const string VS_SEARCH_RESULTS = "vs_search_results";

        const string IMG_ADD_BTN = "~/add.png";
        const string IMG_DELETE_BTN = "~/delete.png";
        const string TT_ADD = "Add part to order";
        const string TT_DELETE = "Remove part from order";

        const string PART_ID = "part_id";
        
        string m_user_id = string.Empty;
        
        ArrayList m_parts_list = new ArrayList();
        ArrayList m_delivery_address_controls = new ArrayList();
        ArrayList m_new_delivery_address_controls = new ArrayList();
        ArrayList m_parts_controls = new ArrayList();
        SortedList m_sl_delivery_addresses = new SortedList();
        user_data m_ud = null;
        ArrayList m_search_results = new ArrayList();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");

            m_delivery_address_controls.Add(lblDelAddr);
            m_delivery_address_controls.Add(dlDelAddr);

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
            
            m_parts_controls.Add(lblPartNo);
            m_parts_controls.Add(txtPartNo);
            m_parts_controls.Add(btnAdd);
            m_parts_controls.Add(lblSelected);
            m_parts_controls.Add(tblParts);

            lblMsg.Text = string.Empty;

            if (IsPostBack)
            {
                m_parts_list = (ArrayList)ViewState["parts_list"];
                m_sl_delivery_addresses = (SortedList)ViewState["delivery_addresses"];

                show_parts_list();

                if (MultiView1.ActiveViewIndex == 1)
                {
                    display();
                }
            }
            else
            {
                dlSearchFlds.Items.Add(DESCRIPTION);
                dlSearchFlds.Items.Add(TYPE);
                using (parts p = new parts())
                {
                    ArrayList pt = p.get_distinct_part_types();
                    ViewState["part_types"] = pt;
                }

                init_screen();

                MultiView1.ActiveViewIndex = 0;
            }
        }

        void init_screen()
        {
            tblParts.Rows.Clear();

            if (m_parts_list == null)
                m_parts_list = new ArrayList();

            m_parts_list.Clear();
            ViewState["parts_list"] = m_parts_list;

            display_delivery_address_controls(false);
            display_parts_controls(false);
            display_new_delivery_address_controls(false);

            txtContractNumber.ReadOnly = false;
            txtContractNumber.Text = string.Empty;
            txtContractNumber.Focus();

            btnProceed.Visible = true;
            
            txtContractNumber.Focus();

            btnSave.Text = "Create Order";
        }

        protected void show_parts_list()
        {
            tblParts.Rows.Clear();

            int i = 0;

            SortedList sl_parts_list = new SortedList();

            foreach (part_data pd in m_parts_list)
            {
                sl_parts_list.Add(pd.description + sl_parts_list.Count.ToString(), pd);
            }

            foreach (DictionaryEntry e0 in sl_parts_list)
            {
                part_data pd = (part_data)e0.Value;
                add_part_to_table(pd, null, false);
                i++;
            }
        }

        protected void add_part_to_table(part_data pd, spool_part_data spd, bool b_add_to_top)
        {
            if (!pd.attributes.ContainsKey("uid"))
                pd.attributes.Add("uid", pd.part_number + Guid.NewGuid().ToString("N"));

            foreach (TableRow r0 in tblParts.Rows)
            {
                if (pd.description.ToUpper() == ((LiteralControl)r0.Cells[1].Controls[0]).Text.ToUpper())
                {
                    if (pd.description.ToUpper().Contains("PIPE")||pd.part_type.ToUpper().Contains("PIPE"))
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

            TableRow r;
            TableCell c;

            r = new TableRow();

            r.Attributes["part_id"] = pd.id.ToString();

            //r.BackColor = bc;

            c = new TableCell();
            c.Controls.Add(new LiteralControl(pd.part_number));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(pd.description));
            r.Cells.Add(c);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Right;
            c.Controls.Add(new LiteralControl((pd.description.ToUpper().Contains("PIPE")|| pd.part_type.ToUpper().Contains("PIPE")) ? "Len (m):" : "Qty:"));
            TextBox qtb = null;

            if (pd.description.ToUpper().Contains("PIPE")||pd.part_type.ToUpper().Contains("PIPE"))
                qtb = create_decimal_textbox("qty_" + pd.attributes["uid"].ToString());
            else
            {
                qtb = create_numeric_textbox("qty_" + pd.attributes["uid"].ToString());
                qtb.Text = "1";
            }

            if (spd != null)
                qtb.Text = spd.qty.ToString();

            c.Controls.Add(qtb);
            r.Cells.Add(c);

            ImageButton btn_remove_part = new ImageButton();
            btn_remove_part.Click += new ImageClickEventHandler(btn_remove_part_Click);
            // btn_remove_part.Text = "X";
            btn_remove_part.ImageUrl = IMG_DELETE_BTN;
            btn_remove_part.ToolTip = "Remove part";
            btn_remove_part.ID = "btn_remove_part_" + pd.attributes["uid"].ToString(); // tblParts.Rows.Count.ToString();
            btn_remove_part.Attributes["uid"] = pd.id.ToString();

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Controls.Add(btn_remove_part);
            r.Cells.Add(c);

            r.Attributes["uid"] = pd.id.ToString();

            if (pd.attributes.ContainsKey("spd_id"))
                r.Attributes["spd_id"] = pd.attributes["spd_id"].ToString();

            string uid = Request.QueryString["uid"];

            if (b_add_to_top)
                tblParts.Rows.AddAt(0, r); // put on top
            else
                tblParts.Rows.Add(r);

            paint_part_table();
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
                    bc = System.Drawing.Color.FromName("LightGray");

                r.BackColor = bc;
            }
        }

        void display_delivery_address_controls(bool bdisplay)
        {
            foreach (Control c in m_delivery_address_controls)
                c.Visible = bdisplay;
        }

        void display_new_delivery_address_controls(bool bdisplay)
        {
            tbl_new_delivery_address.Visible = bdisplay;

            foreach (Control c in m_new_delivery_address_controls)
                c.Visible = bdisplay;
        }

        void display_parts_controls(bool bdisplay)
        {
            tbl_spl_and_parts.Visible = bdisplay;

            foreach (Control c in m_parts_controls)
                c.Visible = bdisplay;

            btnSave.Visible = bdisplay;
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

        void remove_part(string uid)
        {
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
                if (pd.id.ToString() == uid)
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
        void btn_remove_part_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);

            remove_part(uid);

            
        }

        void add_part(string part_desc)
        {
            SortedList sl = new SortedList();
            sl.Add("description", part_desc);

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
                    foreach (part_data pd0 in m_parts_list)
                    {
                        if (pd0.description == pd.description)
                        {
                            pd = pd0;
                            b_already_there = true;
                            break;
                        }
                    }

                    /* moved to add_part_to_table
                    if (!pd.attributes.ContainsKey("uid"))
                        pd.attributes.Add("uid", pd.part_number + Guid.NewGuid().ToString("N"));
                    */

                    add_part_to_table(pd, null, true);

                    if (!b_already_there)
                        m_parts_list.Add(pd);

                    ViewState["parts_list"] = m_parts_list;
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPartNo.Text.Trim().Length > 0)
            {
                add_part(txtPartNo.Text.Trim());
            }

            txtPartNo.Focus();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (tblParts.Rows.Count > 0)
                {
                    string confirmValue = Request.Form["confirm_value"];

                    if (confirmValue == "Yes")
                    {
                        using (users u = new users())
                        {
                            m_ud = u.get_user_data(System.Web.HttpContext.Current.User.Identity.Name);
                        }

                        create_order(dlDelAddr.Text == RETURN_ORDER);

                        init_screen();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error\r\n" + ex.ToString();
            }
        }

        protected void dlDelAddr_SelectedIndexChanged(object sender, EventArgs e)
        {
            display_new_delivery_address_controls(dlDelAddr.Text == NEW_DELIVERY_ADDR);

            if (dlDelAddr.Text == RETURN_ORDER)
                btnSave.Text = "Return Order";
            else
                btnSave.Text = "Create Order";
        }

        void create_order(bool breturn_order)
        {
            int delivery_address_id = get_delivery_address_id();

            SortedList sl_ss = get_supplier_split(delivery_address_id);

            ArrayList aorderlines = new ArrayList();

            create_porder cpo = new create_porder();

            foreach (DictionaryEntry e0 in sl_ss)
            {
                string[] sa = e0.Key.ToString().Split('_');
                string supplier = string.Empty;

                if(sa.Length > 0)
                    supplier = sa[0];
                
                aorderlines.Clear();

                int order_number = 0;

                using (po_numbers po_n = new po_numbers())
                {
                    order_number = po_n.get_next_po_number(txtContractNumber.Text.Trim().ToUpper(), m_ud.login_id);
                }
                string order_type = breturn_order ? "GBE/RETURN/" : "GBE/PO/";

                string sorder_no = order_type +  m_ud.login_id.Trim() + "/" + txtContractNumber.Text.Trim().ToUpper() + "/" +order_number.ToString("000");

                SortedList sl_p = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_p)
                {
                    SortedList sl_pol = new SortedList();
                    sl_pol.Add("part_id", e1.Key);
                    sl_pol.Add("qty", e1.Value);
                    aorderlines.Add(sl_pol);
                }

                porder_data pod = new porder_data();

                if (aorderlines.Count > 0)
                {
                    string delivery_date = breturn_order ? string.Empty : "ASAP";

                    decimal total_value = 0;

                    pod.pdf = cpo.create_po_pdf(sorder_no, txtContractNumber.Text.Trim().ToUpper(), delivery_address_id, delivery_date, aorderlines, ref total_value);

                    SortedList sl = new SortedList();

                    sl.Clear();

                    sl.Add("pdf", pod.pdf);
                    sl.Add("order_no", sorder_no);
                    sl.Add("active", true);
                    sl.Add("part_type", "M");
                    sl.Add("delivery_address_id", delivery_address_id);
                    sl.Add("delivery_date", delivery_date);
                    sl.Add("supplier", supplier);
                    sl.Add("total_value", total_value);

                    int po_id = 0;

                    using (porders p = new porders())
                    {
                        po_id = p.save_porder_data(sl);
                    }

                    using (po_orderlines pol = new po_orderlines())
                    {
                        pol.save_po_orderlines_data(aorderlines, po_id);
                    }

                    string redirect = "<script>window.open('porder.aspx/" + sorder_no + "/?id=" + po_id.ToString() + "');</script>";
                    Response.Write(redirect);
                }
            }
        }

        SortedList get_supplier_split(int delivery_address_id)
        {
            SortedList sl_supplier_split = new SortedList();
            SortedList slp = new SortedList();
            ArrayList a = null;
            decimal qty;
            TextBox tb = null;
            int part_id = 0;

            using (parts p = new parts())
            {
                foreach (TableRow r in tblParts.Rows)
                {
                    if (r.Attributes["part_id"] != null)
                    {
                        try { part_id = Convert.ToInt32(r.Attributes["part_id"]); }
                        catch { };
                        qty = 0;

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
                                        catch {  }
                                    }
                                }
                            }
                        }

                        slp.Clear();
                        slp.Add("id", part_id);

                        a = p.get_part_data(slp);

                        if (a.Count > 0)
                        {
                            part_data pd = (part_data)a[0];

                            if (qty > 0)
                            {
                                string k = pd.supplier.ToLower().Trim() + "_" + delivery_address_id.ToString().Trim();

                                if (!sl_supplier_split.ContainsKey(k))
                                {
                                    sl_supplier_split.Add(k, new SortedList());
                                }

                                ((SortedList)sl_supplier_split[k]).Add(part_id, qty);
                            }
                        }
                    }
                }
            }

            return sl_supplier_split;
        }

        int get_delivery_address_id()
        {
            int delivery_address_id = 0;

            if (dlDelAddr.Text == GBE_MEDWAY)
                delivery_address_id = 0;
            else if(dlDelAddr.Text == RETURN_ORDER)
                delivery_address_id = -2;
            else if (dlDelAddr.Text == NEW_DELIVERY_ADDR)
            {
                if (new_delivery_address_entered())
                    delivery_address_id = save_new_delivery_address();
                else
                {
                    lblMsg.Text = "New delivery address details missing.";
                    
                }
                if (delivery_address_id == -100)
                {
                    lblMsg.Text = "Error saving new delivery address.";
                    
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
                    delivery_address_id = -100;
                }
            }

            return delivery_address_id;
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

        int save_new_delivery_address()
        {
            int id = 0;

            SortedList sl = new SortedList();

            sl.Add("id", "0");
            sl.Add("contract_number", txtContractNumber.Text.Trim().ToUpper());
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

        string create_address_line(string al1, string al2, string al3, string al4)
        {
            string sda = al1;

            if (al2.Length > 0)
            {
                if (sda.Trim().Length > 0)
                    sda += ", ";

                sda += al2;
            }

            if (al3.Length > 0)
            {
                if (sda.Trim().Length > 0)
                    sda += ", ";

                sda += al3;
            }

            if (al4.Length > 0)
            {
                if (sda.Trim().Length > 0)
                    sda += ", ";

                sda += al4;
            }

            return sda;
        }

        void get_delivery_addresses()
        {
            dlDelAddr.Items.Clear();

            dlDelAddr.Items.Add(GBE_MEDWAY);
            dlDelAddr.Items.Add(NEW_DELIVERY_ADDR);

            if (m_sl_delivery_addresses == null)
                m_sl_delivery_addresses = new SortedList();
            else
                m_sl_delivery_addresses.Clear();

            customer_data cd = get_customer_data(txtContractNumber.Text.Trim());

            if (cd != null)
            {
               string sda =  create_address_line(cd.address_line1, cd.address_line2, cd.address_line3, cd.address_line4);

               if (sda.Length > 0)
               {
                   m_sl_delivery_addresses.Add(sda, -1);
                   dlDelAddr.Items.Add(sda);
               }
            }

            using (delivery_addresses da = new delivery_addresses())
            {
                SortedList slda = new SortedList();

                slda.Add("contract_number", txtContractNumber.Text.Trim());

                ArrayList ada = da.get_delivery_address_data(slda);

                foreach (delivery_address d in ada)
                {
                    string sda = create_address_line(d.address_line1, d.address_line2, d.address_line3, d.address_line4);

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

            dlDelAddr.Items.Add(RETURN_ORDER);

            ViewState["delivery_addresses"] = m_sl_delivery_addresses;
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            if (is_contract_number(txtContractNumber.Text.Trim()))
            {
                display_delivery_address_controls(true);
                get_delivery_addresses();

                display_parts_controls(true);

                txtContractNumber.ReadOnly = true;
                btnProceed.Visible = false;
            }
            else
            {
                lblMsg.Text = "Contract number does not exist.";
            }
        }

        customer_data get_customer_data(string contract_number)
        {
            customer_data cd = null;

            SortedList sl = new SortedList();

            sl.Clear();
            sl.Add("contract_number", contract_number);

            ArrayList acd = new ArrayList();

            using (customers cust = new customers())
            {
                acd = cust.get_customer_data(sl);

                if (acd.Count > 0)
                    cd = (customer_data) acd[0];
            }

            return cd;
        }

        bool is_contract_number(string contract_number)
        {
            return get_customer_data(contract_number) != null;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 1;
            m_search_results = new ArrayList();
            ViewState[VS_SEARCH_RESULTS] = m_search_results;
            txtSearch.Focus();
        }

        protected void dlSearchFlds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlSearchFlds.Text == "Type")
            {
                dlPartType.Visible = true;

                if (dlPartType.Items.Count == 0)
                {
                    ArrayList pt = (ArrayList)ViewState["part_types"];

                    if (pt != null)
                    {
                        foreach (string s in pt)
                        {
                            dlPartType.Items.Add(s);
                        }
                    }
                }
            }
            else
                dlPartType.Visible = false;
        }

        protected void lnkPrevPage_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
        }

        protected void btnSearchParts_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        protected void search()
        {
            using (parts p = new parts())
            {
                p.order_by = FLD_DESCRIPTION;

                SortedList sl = new SortedList();

                string srch = string.Empty;
                string field_name = string.Empty;

                if (dlSearchFlds.Text == DESCRIPTION)
                    field_name = FLD_DESCRIPTION;
                else if (dlSearchFlds.Text == TYPE)
                    field_name = FLD_TYPE;

                if (txtSearch.Text.Trim().Length > 0)
                    srch = txtSearch.Text.Trim();
                else if (field_name == FLD_TYPE)
                    srch = dlPartType.Text;

                sl.Add(field_name, srch+"%");

                sl.Add("active", true);

                m_search_results = p.get_part_data(sl);
                ViewState[VS_SEARCH_RESULTS] = m_search_results;
            }
        }

        protected void display()
        {
            tblResults.Rows.Clear();
            m_search_results = (ArrayList)ViewState[VS_SEARCH_RESULTS];

            if (m_search_results == null)
                return;

            if (m_search_results.Count == 0)
                return;

            TableRow r;
            TableCell c;

            r = new TableRow();

            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            c = new TableCell();
            c.Controls.Add(new LiteralControl(string.Empty));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(DESCRIPTION));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(PART_NO));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(SUPPLIER));
            r.Cells.Add(c);

            tblResults.Rows.Add(r);
            
            int i = 0;

            foreach (part_data pd in m_search_results)
            {
                r = new TableRow();
                
                System.Drawing.Color bc;
                if (i++ % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("LightGray");

                r.BackColor = bc;

                r.Attributes[PART_ID] = pd.id.ToString();

                bool balreadythere = false;
                foreach (part_data pd0 in m_parts_list)
                {
                    if (pd0.id == pd.id)
                    {
                        balreadythere = true;
                        break;
                    }
                }

                ImageButton btn_add_part_to_po = new ImageButton();
                btn_add_part_to_po.Click += new ImageClickEventHandler(btn_add_part_to_po_Click);
                btn_add_part_to_po.ImageUrl = balreadythere?IMG_DELETE_BTN:IMG_ADD_BTN;
                btn_add_part_to_po.ToolTip = balreadythere?TT_DELETE:TT_ADD;
                btn_add_part_to_po.ID = "btn_add_part_to_po" + pd.id;
                btn_add_part_to_po.Attributes["uid"] = pd.id.ToString();

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Center;
                c.Controls.Add(btn_add_part_to_po);
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(pd.description));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(pd.part_number));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(pd.supplier));
                r.Cells.Add(c);

                tblResults.Rows.Add(r);
            }
        }

        void btn_add_part_to_po_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);
            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            foreach (part_data pd in m_search_results)
            {
                if (id == pd.id)
                {
                    if (b.ImageUrl == IMG_ADD_BTN)
                    {
                        add_part(pd.description);
                        b.ImageUrl = IMG_DELETE_BTN;
                        b.ToolTip = TT_DELETE;
                    }
                    else
                    {
                        remove_part(uid);
                        b.ImageUrl = IMG_ADD_BTN;
                        b.ToolTip = TT_ADD;
                    }

                    break;
                }
            }
        }

        protected void dlPartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
        }
    }
}
