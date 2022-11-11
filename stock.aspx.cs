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
    public partial class stock : System.Web.UI.Page
    {
        const int REC_PER_PG = 25;
        
        const string FLD_DESC = "Part Description";
        const string FLD_LOC = "Location";
        const string SEARCH_FIELD = "search_field";
        const string SEARCH_STRING = "search_string";

        SortedList m_results = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dlSearchFlds.Items.Add(FLD_DESC);
                dlSearchFlds.Items.Add(FLD_LOC);

                txtSearch.Focus();
            }
            else
            {
                m_results = (SortedList)ViewState["results"];
                display();
            }
        }

        void new_search()
        {
            init_search();
            search(1);
            display();
        }

        void init_search()
        {
            SortedList sl = new SortedList();

            if (txtSearch.Text.Trim().Length > 0)
            {
                string fld, val;
                fld = val = string.Empty;

                val = txtSearch.Text.Trim();

                if (dlSearchFlds.Text == FLD_DESC)
                    fld = "part";
                else if (dlSearchFlds.Text == FLD_LOC)
                    fld = "location";

                sl.Add(SEARCH_FIELD, fld);
                sl.Add(SEARCH_STRING, val);
                
                ViewState["search_params"] = sl;

                upd_record_count();
            }
        }

        void search(int pg)
        {
            ArrayList a0 = null;
            ArrayList a1 = null;
            SortedList sl = new SortedList();

            if (m_results != null)
                m_results.Clear();
            else
                m_results = new SortedList();

            string search_field = string.Empty;
            string search_string = string.Empty;

            get_search_params(ref  search_field, ref  search_string);

            if (search_string.Trim().Length > 0)
            {
                using (part_stock ps = new part_stock())
                {
                    ps.pg = pg;
                    ps.recs_per_pg = REC_PER_PG;
                    ps.order_by = "parts.description"; //"id";

                    a0 = ps.get_part_stock_data(search_field, search_string);

                    using (parts p = new parts())
                    {
                        using (locations l = new locations())
                        {
                            foreach (part_stock_data psd in a0)
                            {
                                if (a1 != null)
                                    a1.Clear();

                                sl.Clear();
                                sl.Add("id", psd.part_id);

                                part_stock_entry pse = new part_stock_entry();
                                pse.psd = psd;

                                a1 = p.get_part_data(sl);

                                if (a1.Count > 0)
                                {
                                    part_data pd = (part_data)a1[0];
                                    pse.desc = pd.description;

                                    sl.Clear();
                                    sl.Add("id", psd.location_id);

                                    a1 = l.get_location_data(sl);

                                    if (a1.Count > 0)
                                    {
                                        location_data ld = (location_data)a1[0];
                                        pse.location = ld.location;
                                    }

                                    m_results.Add(gen_results_key(pse), pse);
                                }
                            }
                        }
                    }
                }
            }

            ViewState["results"] = m_results;

            if (get_last_page() == 0)
                pg = 0;

            ViewState["current_page"] = pg;

            lblPage.Text = get_current_page().ToString() + " / " + get_last_page().ToString();
        }

        string gen_results_key(part_stock_entry pse)
        {
            return (pse.desc+pse.location+pse.psd.id);
        }

        void display()
        {
            TextBox qtb;

            tblResults.Rows.Clear();

            if (m_results != null)
            {
                if (m_results.Count == 0)
                    return;

                TableRow r;
                TableCell c;

                string[] hdr = new string[] { "Part", "Location", "Qty in Stock" };
                string[] hdr2 = new string[] { "Project", "Requirement"};

                string desc = string.Empty;
                bool b_show_total = false;
                decimal total_qty = 0;

                string end_mrk = "\xFF";

                if (m_results.Count > 0)
                {
                    if (!m_results.ContainsKey(end_mrk))
                        m_results.Add(end_mrk, new part_stock_entry());
                }

                SortedList sl_part_req = null;

                using (spool_parts sp = new spool_parts())
                {
                    foreach (DictionaryEntry e0 in m_results)
                    {
                        part_stock_entry pse = (part_stock_entry)e0.Value;

                        System.Drawing.Color bc;

                        r = new TableRow();

                        if (pse.desc != desc)
                        {
                            if (b_show_total)
                            {
                                /*
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);

                                bc = System.Drawing.Color.FromName("White");

                                c = new TableCell();
                                c.BackColor = bc;
                                c.Font.Bold = true;
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(total_qty.ToString("0.00")));
                                r.Cells.Add(c);
                                tblResults.Rows.Add(r);

                                r = new TableRow();
                                */
                            }

                            if (sl_part_req != null)
                            {
                                if (sl_part_req.Count > 0)
                                {
                                    c = new TableCell();
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                    tblResults.Rows.Add(r);

                                    bc = System.Drawing.Color.FromName("LightGreen");

                                    foreach (string sh in hdr2)
                                    {
                                        c = new TableCell();
                                        c.BackColor = bc;
                                        c.Controls.Add(new LiteralControl(sh));
                                        r.Cells.Add(c);
                                    }

                                    tblResults.Rows.Add(r);

                                    decimal total_qty_req = 0;
                                    decimal qty_req = 0;

                                    foreach (DictionaryEntry e1 in sl_part_req)
                                    {
                                        r = new TableRow();

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(string.Empty));
                                        r.Cells.Add(c);
                                        tblResults.Rows.Add(r);

                                        bc = System.Drawing.Color.FromName("LightPink");

                                        c = new TableCell();
                                        c.BackColor = bc;
                                        c.Controls.Add(new LiteralControl(e1.Key.ToString()));
                                        r.Cells.Add(c);
                                        tblResults.Rows.Add(r);

                                        qty_req = (decimal)e1.Value;

                                        c = new TableCell();
                                        c.BackColor = bc;
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(qty_req.ToString("0.00")));
                                        r.Cells.Add(c);

                                        tblResults.Rows.Add(r);

                                        total_qty_req += qty_req;
                                    }

                                    if (sl_part_req.Count > 1)
                                    {
                                        r = new TableRow();
                                        c = new TableCell();

                                        c.Controls.Add(new LiteralControl(string.Empty));
                                        r.Cells.Add(c);
                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(string.Empty));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.BackColor = bc;
                                        c.Font.Bold = true;
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(total_qty_req.ToString("0.00")));
                                        r.Cells.Add(c);
                                        tblResults.Rows.Add(r);

                                    }
                                
                                    r = new TableRow();
                                }
                            }

                            if (e0.Key.ToString() == end_mrk)
                                continue;

                            //if (pse.psd.qty_in_stock == 0)
                            //    continue;

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(string.Empty));
                            r.Cells.Add(c);
                            tblResults.Rows.Add(r);

                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("SeaGreen");

                            foreach (string sh in hdr)
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(sh));
                                r.Cells.Add(c);
                            }

                            tblResults.Rows.Add(r);

                            r = new TableRow();

                            bc = System.Drawing.Color.FromName("LightGray");

                            c = new TableCell();
                            c.BackColor = bc;
                            c.Controls.Add(new LiteralControl(pse.desc));
                            r.Cells.Add(c);

                            desc = pse.desc;
                            tblResults.Rows.Add(r);

                            total_qty = pse.psd.qty_in_stock;
                            b_show_total = false;

                            sl_part_req = sp.get_project_requirement(pse.psd.part_id);
                        }
                        else
                        {
                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(string.Empty));
                            r.Cells.Add(c);

                            total_qty += pse.psd.qty_in_stock;
                            b_show_total = true;
                        }

                        bc = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.BackColor = bc;
                        c.Controls.Add(new LiteralControl(pse.location));
                        r.Cells.Add(c);

                        qtb = create_decimal_textbox("qty_" + pse.psd.id.ToString());
                        qtb.Style["TEXT-ALIGN"] = TextAlign.Right.ToString();
                        qtb.Text = pse.psd.qty_in_stock.ToString("0.00");
                        qtb.Attributes["uid"] = pse.psd.id.ToString();

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.BackColor = System.Drawing.Color.FromName("White");
                        c.Controls.Add(qtb);
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.BackColor = System.Drawing.Color.FromName("White");
                        ImageButton btn_save_stock = new ImageButton();
                        btn_save_stock.ToolTip = "Save changes to stock";
                        btn_save_stock.ImageUrl = "~/disk.png";
                        btn_save_stock.Click += new ImageClickEventHandler(btn_save_stock_Click);
                        btn_save_stock.ID = "btn_save_stock_" + pse.psd.id.ToString();
                        btn_save_stock.Attributes["uid"] = pse.psd.id.ToString();

                        c.Controls.Add(btn_save_stock);
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.BackColor = System.Drawing.Color.FromName("White");
                        ImageButton btn_remove_stock = new ImageButton();
                        btn_remove_stock.Click += new ImageClickEventHandler(btn_remove_stock_Click);
                        btn_remove_stock.ImageUrl = "~/delete.png";
                        btn_remove_stock.ToolTip = "Delete";
                        btn_remove_stock.ID = "btn_remove_stock" + pse.psd.id.ToString();
                        btn_remove_stock.Attributes["uid"] = pse.psd.id.ToString();
                        btn_remove_stock.Attributes["results_key"] = gen_results_key(pse);
                        btn_remove_stock.OnClientClick = "Confirm()";

                        c.Controls.Add(btn_remove_stock);
                        r.Cells.Add(c);

                        r.Attributes["uid"] = pse.psd.id.ToString();
                        r.Attributes["results_key"] = gen_results_key( pse);

                        tblResults.Rows.Add(r);
                    }
                }
            }
        }

        void btn_remove_stock_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                try
                {
                    SortedList sl = new SortedList();
                    ImageButton b = (ImageButton)sender;
                    ArrayList rows_to_delete = new ArrayList();

                    string uid = (b.Attributes["uid"]);
                    string results_key = b.Attributes["results_key"];

                    if (uid != null)
                    {

                        using (part_stock ps = new part_stock())
                        {
                            int id = 0;
                            try
                            {
                                id = Convert.ToInt32(uid);
                            }
                            catch { }

                            if (id > 0)
                            {
                                sl.Clear();
                                sl.Add("id", uid);

                                ArrayList a_sd = ps.get_part_stock_data(sl);

                                part_stock_data psd = new part_stock_data();

                                if (a_sd.Count > 0)
                                {
                                    psd = (part_stock_data)a_sd[0];
                                }

                                ps.delete_record(id);

                                if (m_results.ContainsKey(results_key))
                                    m_results.Remove(results_key);

                                foreach (TableRow r in tblResults.Rows)
                                {
                                    string r_results_key = r.Attributes["results_key"];


                                    if (r_results_key == results_key)
                                    {
                                        rows_to_delete.Add(r);
                                    }
                                }

                                using (stock_movement_audit_trail smat = new stock_movement_audit_trail())
                                {
                                    using (locations l = new locations())
                                    {
                                        sl.Clear();

                                        sl.Add("user_id", System.Web.HttpContext.Current.User.Identity.Name);
                                        sl.Add("part_type", "M");
                                        sl.Add("part_id", psd.part_id);
                                        sl.Add("datetime_stamp", DateTime.Now);
                                        sl.Add("movement_event", "STOCK DELETED FROM " + l.get_location(psd.location_id));
                                        sl.Add("quantity", psd.qty_in_stock);
                                        sl.Add("destination", string.Empty);

                                        smat.save_stock_movement_audit_trail_data(sl);
                                    }
                                }
                            }
                        }
                    }

                    foreach (TableRow r in rows_to_delete)
                        tblResults.Rows.Remove(r);
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "Error\r\n" + ex.ToString();
                }
            }
        }

        void btn_save_stock_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ImageButton b = (ImageButton)sender;

                string uid = (b.Attributes["uid"]);

                using (part_stock ps = new part_stock())
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

                                        string tb_uid = (tb.Attributes["uid"]);

                                        if (tb_uid == uid)
                                        {
                                            decimal qty_old = 0;
                                            decimal qty = 0;
                                            try { qty = Convert.ToDecimal(tb.Text.Trim()); }
                                            catch { }

                                            SortedList sl = new SortedList();
                                            sl.Add("id", uid);

                                            ArrayList a_sd = ps.get_part_stock_data(sl);

                                            part_stock_data psd = new part_stock_data();

                                            if (a_sd.Count > 0)
                                            {
                                                psd = (part_stock_data)a_sd[0];

                                                qty_old = psd.qty_in_stock;
                                            }

                                            sl.Add("qty_in_stock", qty);

                                            ps.save_part_stock_data(sl);

                                            string location  = string.Empty;

                                            using (locations loc = new locations())
                                            {
                                                location = loc.get_location(psd.location_id);
                                            }
                                            
                                            using (stock_movement_audit_trail smat = new stock_movement_audit_trail())
                                            {
                                                sl.Clear();

                                                sl.Add("user_id", System.Web.HttpContext.Current.User.Identity.Name);
                                                sl.Add("part_type", "M");
                                                sl.Add("part_id", psd.part_id);
                                                sl.Add("datetime_stamp", DateTime.Now);
                                                sl.Add("movement_event", "QTY ADJ FROM " + qty_old.ToString("0.00"));
                                                sl.Add("quantity", qty);
                                                sl.Add("destination", location);

                                                smat.save_stock_movement_audit_trail_data(sl);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error\r\n" + ex.ToString();
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

        void upd_record_count()
        {
            int record_count = 0;

            string search_field = string.Empty;
            string search_string = string.Empty;

            get_search_params(ref  search_field, ref  search_string);

            using (part_stock ps = new part_stock())
            {
                record_count = ps.get_record_count(search_field, search_string);
            }

            ViewState["record_count"] = record_count;
        }

        void get_search_params(ref string search_field, ref string search_string)
        {
            SortedList sl = (SortedList)ViewState["search_params"];

            if (sl != null)
            {
                if (sl.ContainsKey(SEARCH_FIELD) && sl.ContainsKey(SEARCH_STRING))
                {
                    search_field = sl[SEARCH_FIELD].ToString();
                    search_string = sl[SEARCH_STRING].ToString();
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            new_search();
        }

        protected void btnFirstPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                search(1);
                display();
            }
        }

        protected void btnPreviousPage_Click(object sender, ImageClickEventArgs e)
        {
            int pg = get_current_page();

            if (pg > 0)
            {
                if (pg > 1)
                {
                    search(pg - 1);
                    display();
                }
            }
        }

        protected void btnNextPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                int pg = get_current_page();

                if (pg > 0)
                {
                    if (pg < get_last_page())
                    {
                        search(pg + 1);
                        display();
                    }
                }
            }
        }

        protected void btnLastPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                search(get_last_page());
                display();
            }
        }

        int get_current_page()
        {
            int current_page = 0;

            if (ViewState["current_page"] != null)
                current_page = (int)ViewState["current_page"];

            return current_page;
        }

        int get_last_page()
        {
            int last_page = 0;

            if (ViewState["record_count"] != null)
            {
                int record_count = (int)ViewState["record_count"];

                last_page = record_count / REC_PER_PG;

                if (record_count % REC_PER_PG > 0)
                    last_page++;
            }
            return last_page;
        }
    }

    [Serializable]
    public class part_stock_entry
    {
        public string desc = string.Empty;
        public string location = string.Empty;
        public part_stock_data psd = null;
    }
}
