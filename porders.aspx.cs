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
    public partial class porders1 : System.Web.UI.Page
    {
        SortedList m_porders = null;
        SortedList m_added_parts = new SortedList();
        SortedList m_added_parts_display = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");
            */

            if (IsPostBack)
            {
                m_porders = (SortedList)ViewState["porders"];
                m_added_parts = (SortedList)ViewState["added_parts"];
                m_added_parts_display = (SortedList)ViewState["added_parts_display"];
                display();
            }
            else
            {
                ViewState["added_parts"] = m_added_parts;
                ViewState["added_parts_display"] = m_added_parts_display;
                
                txtQty.Attributes.Add("onkeypress", "return onlyDotsAndNumbers(this, event)");
                MultiView1.ActiveViewIndex = 0;

                string sid = Request.QueryString["id"];

                if (sid != null)
                {
                    int id = 0;

                    try { id = Convert.ToInt32(sid); }
                    catch { }

                    porder_data pod = null;

                    if (id > 0)
                    {
                        SortedList sl = new SortedList();

                        sl.Add("id", id);

                        ArrayList a = new ArrayList();

                        using (porders p = new porders())
                        {
                            a = p.get_porder_data(sl);
                        }

                        if (a.Count > 0)
                        {
                            pod = (porder_data)a[0];
                            txtSearch.Text = pod.order_no;
                            btnSearch_Click(null, null);
                        }
                    }
                }
            }

            txtSearch.Focus();
        }

        void get_active_returns()
        {
            using (porders po = new porders())
            {
                po.no_pdf = true;

                SortedList sl = new SortedList();

                if (m_porders != null)
                    m_porders.Clear();
                else
                    m_porders = new SortedList();

            
                sl.Add("order_no", "GBE/RETURN%");
                sl.Add("active", true);

                ArrayList apords = po.get_porder_data(sl);

                if (apords.Count > 0)
                {
                    foreach (porder_data pd in apords)
                    {
                        if (!m_porders.ContainsKey(pd.order_no))
                            m_porders.Add(pd.order_no, pd);
                    }
                }
            }

            ViewState["porders"] = m_porders;
        }

        void search()
        {
            lblMsg.Text = string.Empty;

            using(porders po = new porders())
            {
                po.no_pdf = true;

                SortedList sl = new SortedList();

                if (m_porders != null)
                    m_porders.Clear();
                else
                    m_porders = new SortedList();

                if (txtSearch.Text.Trim().Length > 3)
                {
                    sl.Add("order_no", txtSearch.Text.Trim() + "%");
                }

                if (txtSupplier.Text.Trim().Length > 3)
                {
                    sl.Add("supplier", txtSupplier.Text.Trim() + "%");
                }

                if (txtDate.Text.Trim().Length > 0)
                {
                    DateTime dt = DateTime.Now;

                    try {dt =  Convert.ToDateTime(txtDate.Text.Trim()); }
                    catch { lblMsg.Text = "Invalid date"; return; }

                    sl.Add("order_date", dt.ToString("MMM").ToLower() + '\x20' + dt.ToString("dd") + '\x20' + dt.ToString("yyyy") + "%");
                }

                if (sl.Count > 0)
                {
                    po.m_and_or_op = "and";

                    ArrayList apords = po.get_porder_data(sl);

                    if (apords.Count > 0)
                    {
                        foreach (porder_data pd in apords)
                        {
                            if (!m_porders.ContainsKey(pd.order_no))
                                m_porders.Add(pd.order_no, pd);
                        }
                    }
                }
            }

            ViewState["porders"] = m_porders;
        }

        void display()
        {
            tblResults.Rows.Clear();
            
            SortedList sl = new SortedList();
            SortedList sl_ol = new SortedList();
            TextBox qtb;

            if (m_porders != null)
            {
                using (po_orderlines plines = new po_orderlines())
                {
                    imsl_orders imslorders = new imsl_orders(plines.m_sql_connection);

                    foreach (DictionaryEntry e0 in m_porders)
                    {
                        porder_data pd = (porder_data)e0.Value;

                        bool breturn = pd.order_no.ToUpper().StartsWith("GBE/RETURN");
                        
                        TableRow r;
                        TableCell c;

                        r = new TableRow();

                        r.Attributes["po_id"] = pd.id.ToString();

                        r.BackColor = System.Drawing.Color.FromName("LightGreen");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(pd.order_no));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        if (pd.part_type.Length > 0)
                        {
                            if (pd.part_type == "M")
                            {
                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Center;
                                ImageButton btn_add_part = new ImageButton();
                                btn_add_part.ToolTip = "Add part to PO";
                                btn_add_part.ImageUrl = "~/add.png";
                                btn_add_part.Click += new ImageClickEventHandler(btn_add_part_Click);
                                btn_add_part.ID = "btn_add_part" + pd.id.ToString();
                                btn_add_part.Attributes["po_id"] = pd.id.ToString();

                                c.Controls.Add(btn_add_part);
                                r.Cells.Add(c);


                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Center;
                                ImageButton btn_save_po = new ImageButton();
                                btn_save_po.ToolTip = "Save changes to PO";
                                btn_save_po.ImageUrl = "~/disk.png";
                                btn_save_po.Click += new ImageClickEventHandler(btn_save_po_Click);
                                btn_save_po.ID = "btn_save_po_" + pd.id.ToString();
                                btn_save_po.Attributes["po_id"] = pd.id.ToString();

                                c.Controls.Add(btn_save_po);
                                r.Cells.Add(c);
                            }
                        }

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        ImageButton btn_view_po = new ImageButton();
                        btn_view_po.ToolTip = "View PO";
                        btn_view_po.ImageUrl = "~/pdf.png";
                        btn_view_po.Click += new ImageClickEventHandler(btn_view_po_Click);
                        btn_view_po.ID = "btn_view_po" + pd.id.ToString();
                        btn_view_po.Attributes["po_id"] = pd.id.ToString();

                        c.Controls.Add(btn_view_po);
                        r.Cells.Add(c);

                        ImageButton btn_remove_po = new ImageButton();
                        btn_remove_po.Click += new ImageClickEventHandler(btn_remove_po_Click);
                        btn_remove_po.ImageUrl = "~/delete.png";
                        btn_remove_po.ToolTip = "Delete";
                        btn_remove_po.ID = "btn_remove_part_" + pd.id.ToString();
                        btn_remove_po.Attributes["po_id"] = pd.id.ToString();
                        btn_remove_po.OnClientClick = "Confirm()";

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.Controls.Add(btn_remove_po);
                        r.Cells.Add(c);

                        if (breturn)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            CheckBox chk = new CheckBox();
                            chk.AutoPostBack = true;
                            chk.ID = "chk_active_" + pd.id.ToString();
                            chk.Attributes["po_id"] = pd.id.ToString();
                            chk.Checked = pd.active;
                            chk.ToolTip = pd.active?"Active, awaiting credit":"In-active, credit received";
                            chk.CheckedChanged += new EventHandler(chk_CheckedChanged);

                            c.Controls.Add(chk);
                            r.Cells.Add(c);
                        }

                        if (pd.supplier.ToLower() == "imsl")
                        {
                            sl.Clear();

                            sl.Add("porder_id", pd.id);
                            ArrayList a_io = imslorders.get_imsl_order_data(sl);

                            ImageButton btn_imsl = new ImageButton();
                            btn_imsl.ID = "btn_imsl_" + pd.id.ToString();
                            btn_imsl.Attributes["po_id"] = pd.id.ToString();

                            if (a_io.Count > 0)
                            {
                                imsl_order_data io = (imsl_order_data)a_io[0];

                                btn_imsl.ImageUrl = "~/upload_grey.png";
                                btn_imsl.ToolTip = "Sent to IMSL on " + io.dt_sent.ToString("dd/MM/yyyy hh:mm") + " by " + io.ud.login_id;
                            }
                            else
                            {
                                btn_imsl.ImageUrl = "~/upload.png";
                                btn_imsl.ToolTip = "Send to IMSL";
                                btn_imsl.Click += new ImageClickEventHandler(btn_imsl_Click);
                                btn_imsl.OnClientClick = "ConfirmSendToIMSL()";
                            }

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            c.Controls.Add(btn_imsl);
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);

                        r = new TableRow();
                        r.Attributes["po_id"] = pd.id.ToString();

                        r.BackColor = System.Drawing.Color.FromName("LightGreen");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(pd.supplier.ToUpper()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        LiteralControl lit_total_value = new LiteralControl();
                        lit_total_value.ID = "lit_total_value_" + pd.id.ToString();
                        lit_total_value.Text = "£" + pd.total_value.ToString("0.00");
                        c.Controls.Add(lit_total_value);
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);

                        if (pd.part_type.Length > 0)
                        {
                            if (m_added_parts_display.ContainsKey(pd.id.ToString()))
                            {
                                using (parts p = new parts())
                                {
                                    ArrayList a = (ArrayList)m_added_parts_display[pd.id.ToString()];

                                    foreach (string s in a)
                                    {
                                        string[] sa = s.Split('|');

                                        if (sa.Length == 2)
                                        {
                                            sl.Clear();

                                            sl.Add("id", sa[0]);

                                            ArrayList ap = p.get_part_data(sl);

                                            if (ap.Count > 0)
                                            {
                                                part_data ptd = (part_data)ap[0];

                                                insert_added_part(ptd.description, sa[1], tblResults.Rows.Count, pd.id.ToString(), ptd.id.ToString());
                                            }
                                        }
                                    }
                                }
                            }   
                        
                            sl.Clear();
                            sl_ol.Clear();

                            sl.Add("porder_id", pd.id);

                            ArrayList a_pol = plines.get_po_orderlines_data(sl, pd.part_type, string.Empty);

                            foreach (po_orderlines_data pol in a_pol)
                            {
                                if (!sl_ol.ContainsKey(pol.part_desc))
                                    sl_ol.Add(pol.part_desc, pol);
                            }

                            foreach (DictionaryEntry e1 in sl_ol)
                            {
                                po_orderlines_data pol = (po_orderlines_data)e1.Value;

                                r = new TableRow();

                                r.Attributes["po_id"] = pd.id.ToString();
                                r.Attributes["pol_uid"] = pol.id.ToString();

                                r.BackColor = System.Drawing.Color.FromName("LightGray");
                                
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(pol.part_desc));
                                r.Cells.Add(c);

                                qtb = create_decimal_textbox("qty_" + pol.id.ToString());
                                qtb.Text = pol.qty.ToString("0.00");
                                qtb.Attributes["po_id"] = pd.id.ToString();
                                qtb.Attributes["pol_uid"] = pol.id.ToString();
                                                                
                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(qtb);
                                r.Cells.Add(c);

                                if (pol.note != null)
                                {
                                    if (pol.note.note.Trim().Length > 0)
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            c = new TableCell();
                                            c.Controls.Add(new LiteralControl(string.Empty));
                                            c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                            r.Cells.Add(c);
                                        }

                                        c = new TableCell();
                                        c.BackColor = System.Drawing.Color.FromName("LightGray");
                                        c.Controls.Add(new LiteralControl(pol.note.note.Trim()));
                                        r.Cells.Add(c);
                                    }
                                }

                                tblResults.Rows.Add(r);
                            }
                        }
                    }
                }
            }
        }

        void btn_imsl_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            user_data ud = null;

            using (users u = new users())
            {
                ud = u.get_user_data(System.Web.HttpContext.Current.User.Identity.Name);
            }

            if (ud.imsl_username.Trim().Length == 0 || ud.imsl_username.ToLower() == "unassigned")
            {
                lblMsg.Text = "IMSL username has not been assigned";
                return;
            }
            
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                using (imsl_orders imslorders = new imsl_orders())
                {
                    SortedList sl = new SortedList();
                    ImageButton b = (ImageButton)sender;

                    string po_id = (b.Attributes["po_id"]);
                   
                    sl.Add("porder_id", po_id);
                    sl.Add("bsent", false);
                    sl.Add("dt_sent", DateTime.Now);
                    sl.Add("user_id", ud.id);
                    sl.Add("imsl_username", ud.imsl_username);

                    imslorders.save_imsl_order_data(sl);

                    display();
                }
            }
        }

        void btn_remove_po_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                SortedList sl = new SortedList();
                ImageButton b = (ImageButton)sender;
                ArrayList rows_to_delete = new ArrayList();

                string po_id = (b.Attributes["po_id"]);

                if (po_id != null)
                {
                    using (porders po = new porders())
                    {
                        int id = 0;
                        try
                        {
                            id = Convert.ToInt32(po_id);
                        }
                        catch { }

                        string order_number = po.get_order_number(id);

                        if (id > 0)
                        {
                            if (m_porders.ContainsKey(order_number))
                                m_porders.Remove(order_number);
                        }

                        using (po_orderlines plines = new po_orderlines())
                        {
                            foreach (TableRow r in tblResults.Rows)
                            {
                                string r_po_id = r.Attributes["po_id"];
                                string r_pol_id = r.Attributes["pol_uid"];

                                if (po_id == r_po_id)
                                {
                                    if (r_pol_id != null)
                                    {
                                        sl.Clear();
                                        sl.Add("id", r_pol_id);

                                        plines.delete_orderline(sl);
                                    }

                                    if (r_po_id != null)
                                    {
                                        sl.Clear();
                                        sl.Add("id", r_po_id);

                                        po.delete_porder(sl);

                                        if (r_po_id == po_id)
                                        {
                                            rows_to_delete.Add(r);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (TableRow r in rows_to_delete)
                    tblResults.Rows.Remove(r);
            }
        }

        void chk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["po_id"]);

            SortedList sl = new SortedList();

            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            sl.Add("id", id);
            sl.Add("active", chk.Checked);

            using (porders p = new porders())
            {
                p.save_porder_data(sl);
            }

            foreach (DictionaryEntry e0 in m_porders)
            {
                porder_data pd = (porder_data)e0.Value;

                if (pd.id == id)
                {
                    pd.active = chk.Checked;
                    break;
                }
            }

            chk.ToolTip = chk.Checked ? "Active, awaiting credit" : "In-active, credit received";
        }

        void btn_add_part_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["po_id"]);

            ViewState["add_to_order_uid"] = uid;

            MultiView1.ActiveViewIndex = 1;

            txtPart.Text = txtQty.Text = string.Empty;
            txtPart.Focus();
        }

        void view_po(int id)
        {
            string url = string.Empty;

            using (porders po = new porders())
            {
                url = "porder.aspx/PO_" + po.get_order_number(id).Replace('/', '_') + "?id=" + id.ToString();

                Response.Write("<script>");
                Response.Write("window.open('" + url + "','_blank','','false')");
                Response.Write("</script>");
            }
        }

        void btn_view_po_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["po_id"]);
            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            if (id > 0)
            {
                view_po(id);
            }
        }

        void btn_save_po_Click(object sender, ImageClickEventArgs e)
        {
            SortedList sl = new SortedList();
            ImageButton b = (ImageButton)sender;
            ArrayList rows_to_delete = new ArrayList();

            string uid = (b.Attributes["po_id"]);
            using (porders po = new porders())
            {
                using (po_orderlines plines = new po_orderlines())
                {
                    foreach (TableRow r in tblResults.Rows)
                    {
                        foreach (TableCell cell in r.Cells)
                        {
                            foreach (Control c in cell.Controls)
                            {
                                if (c.ID != null)
                                {
                                    if (c.ID.StartsWith("qty"))
                                    {
                                        TextBox tb = ((TextBox)c);

                                        string po_id = (tb.Attributes["po_id"]);

                                        if (po_id != null)
                                        {
                                            string pol_uid = (tb.Attributes["pol_uid"]);

                                            if (po_id == uid)
                                            {
                                                decimal qty = 0;
                                                try { qty = Convert.ToDecimal(tb.Text.Trim()); }
                                                catch { }

                                                sl.Clear();

                                                if (pol_uid != null)
                                                {
                                                    sl.Add("id", pol_uid);
                                                }
                                                else
                                                {
                                                    string part_id = tb.Attributes["part_id"];
                                                    if (part_id != null)
                                                    {
                                                        sl.Add("porder_id", po_id);
                                                        sl.Add("part_id", part_id);
                                                    }

                                                    m_added_parts.Remove(po_id);
                                                    m_added_parts_display.Remove(po_id);

                                                    ViewState["added_parts"] = m_added_parts;
                                                    ViewState["added_parts_display"] = m_added_parts_display;

                                                    r.BackColor = System.Drawing.Color.FromName("LightGray");
                                                }

                                                if (qty > 0)
                                                {
                                                    sl.Add("qty", qty);
                                                    plines.save_po_orderlines_data(sl);
                                                }
                                                else 
                                                {
                                                    if (pol_uid != null)
                                                        plines.delete_orderline(sl);
                                                    
                                                    rows_to_delete.Add(r);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // update porder record
                    int id = 0;

                    try { id = Convert.ToInt32(uid); }
                    catch { }

                    if (id > 0)
                    {
                        po.null_pdf(id);    // null so that it's created next time it's viewed
                        view_po(id);

                        sl.Clear();
                        sl.Add("id", id);

                        ArrayList a_po = po.get_porder_data(sl);

                        if (a_po.Count > 0)
                        {
                            porder_data pod = (porder_data)a_po[0];

                            sl.Clear();
                            sl.Add("porder_id", uid);

                            ArrayList a_pol = plines.get_po_orderlines_data(sl, "M", string.Empty);

                            decimal total_value = 0;
                            foreach (po_orderlines_data pold in a_pol)
                            {
                                total_value += pold.qty * pold.unit_cost;
                            }

                            foreach (TableRow r in tblResults.Rows)
                            {
                                string po_id = (r.Attributes["po_id"]);

                                if (po_id != null)
                                {
                                    if (po_id == uid)
                                    {
                                        foreach (TableCell cell in r.Cells)
                                        {
                                            foreach (Control c in cell.Controls)
                                            {
                                                if (c.ID == "lit_total_value_" + po_id)
                                                {
                                                    ((LiteralControl)c).Text = "£" + total_value.ToString("0.00");

                                                    if (m_porders.ContainsKey(pod.order_no))
                                                    {
                                                        pod = (porder_data)m_porders[pod.order_no];
                                                        pod.total_value = total_value;
                                                        ViewState["porders"] = m_porders;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (TableRow r in rows_to_delete)
                    tblResults.Rows.Remove(r);

            }
        }

        TextBox create_decimal_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 100;
            tb.MaxLength = 7;
            tb.Text = string.Empty;
            tb.Attributes.Add("onkeypress", "return onlyDotsAndNumbers(this, event)");
            tb.ID = id;

            return tb;
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            string uid = ViewState["add_to_order_uid"].ToString();
            SortedList sl = new SortedList();
            ArrayList al = new ArrayList();

            string part_desc = txtPart.Text.Trim();
            string qty = txtQty.Text.Trim();

            if(part_desc.Length > 0 && qty.Length > 0)
            {
                using (parts p = new parts())
                {
                    sl.Clear();
                    sl.Add("description", part_desc);

                    al = p.get_part_data(sl);

                    if (al.Count > 0)
                    {
                        part_data pd = (part_data)al[0];

                        using (po_orderlines pol = new po_orderlines())
                        {
                            sl.Clear();

                            sl.Add("part_id", pd.id);
                            sl.Add("porder_id", uid);

                            al = pol.get_po_orderlines_data(sl, "M", string.Empty);

                            if (al.Count == 0)
                            {
                                bool b_already_added = false;

                                if (m_added_parts.ContainsKey(uid))
                                {
                                    if (((SortedList)m_added_parts[uid]).ContainsKey(pd.id))
                                        b_already_added = true;
                                }

                                if (!b_already_added)
                                {
                                    SortedList sl_parts = null;

                                    if (m_added_parts.ContainsKey(uid))
                                        sl_parts = (SortedList)m_added_parts[uid];
                                    else
                                    {
                                        sl_parts = new SortedList();
                                        m_added_parts.Add(uid, sl_parts);
                                        m_added_parts_display.Add(uid, new ArrayList());
                                    }

                                    sl_parts.Add(pd.id, qty);

                                    ((ArrayList)m_added_parts_display[uid]).Add(pd.id.ToString() +"|"+ qty);

                                    ViewState["added_parts"] = m_added_parts;
                                    ViewState["added_parts_display"] = m_added_parts_display;

                                    int i = 0;
                                    foreach (TableRow r in tblResults.Rows)
                                    {
                                        i++;

                                        if (r.Attributes["po_id"] == uid)
                                        {
                                            insert_added_part(pd.description, qty, i, uid, pd.id.ToString());
                                            MultiView1.ActiveViewIndex = 0;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    lblMsg2.Text = "Part already added.";
                                    txtPart.Focus();
                                }
                            }
                            else
                            {
                                lblMsg2.Text = "Part already exists on PO.";
                                txtPart.Focus();
                            }
                        }
                    }
                    else
                    {
                        lblMsg2.Text = "Part not found.";
                        txtPart.Focus();
                    }
                }
            }
        }

        void insert_added_part(string description, string qty, int insert_at, string po_id, string part_id)
        {
            TableRow r = new TableRow();
            TextBox qtb;
            TableCell c;

            r.BackColor = System.Drawing.Color.FromName("LightPink");

            c = new TableCell();
            c.Controls.Add(new LiteralControl(description));
            r.Cells.Add(c);

            qtb = create_decimal_textbox("qty_" + po_id + "_" + part_id);
            qtb.Text = qty;
            qtb.Attributes["po_id"] = po_id;
            qtb.Attributes["part_id"] = part_id;

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Right;
            c.Controls.Add(qtb);
            r.Cells.Add(c);
            tblResults.Rows.AddAt(insert_at, r);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
        }

        protected void btnActiveReturns_Click(object sender, EventArgs e)
        {
            get_active_returns();
            display();
        }
    }
}
