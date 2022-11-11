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
    public partial class stock_movement : System.Web.UI.Page
    {
        const int REC_PER_PG = 25;
        const string FLD_USER = "User ID";
        const string FLD_DEST = "Destintaion";
        const string FLD_DESC = "Part Description";
        const string SEARCH_FIELD = "search_field";
        const string SEARCH_STRING = "search_string";
        const string SEARCH_DATE = "search_date";
        
        string[] srch_flds = { FLD_USER, FLD_DEST, FLD_DESC };
        
        ArrayList m_results = new ArrayList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (string sf in srch_flds)
                    dlSearchFlds.Items.Add(sf);

                dtFrom.SelectedDate = DateTime.Today;

                new_search();

                txtSearch.Focus();
            }
            else
            {
                m_results = (ArrayList)ViewState["results"];
                display();
            }
        }

        void new_search()
        {
            init_search();
            search(1);
            display();
        }

        void get_search_params(ref DateTime dt, ref string search_field, ref string search_string)
        {
            SortedList sl = (SortedList)ViewState["search_params"];

            dt = (DateTime)sl[SEARCH_DATE];

            if (sl.ContainsKey(SEARCH_FIELD) && sl.ContainsKey(SEARCH_STRING))
            {
                search_field = sl[SEARCH_FIELD].ToString();
                search_string = sl[SEARCH_STRING].ToString();
            }
        }

        void upd_record_count()
        {
            int record_count = 0;
                        
            DateTime dt = DateTime.Now;
            string search_field = string.Empty;
            string search_string = string.Empty;

            get_search_params(ref  dt, ref  search_field, ref  search_string);

            using (stock_movement_audit_trail smat = new stock_movement_audit_trail())
            {
                record_count = smat.get_record_count(dt, search_field, search_string);
            }

            ViewState["record_count"] = record_count;
        }

        void init_search()
        {
            SortedList sl = new SortedList();

            sl.Add(SEARCH_DATE, dtFrom.SelectedDate);

            if (txtSearch.Text.Trim().Length > 0)
            {
                string fld, val;
                fld = val = string.Empty;

                val = txtSearch.Text.Trim();

                if (dlSearchFlds.Text == FLD_USER)
                    fld = "user_id";
                else if (dlSearchFlds.Text == FLD_DEST)
                    fld = "destination";
                else if (dlSearchFlds.Text == FLD_DESC)
                    fld = "part";

                sl.Add(SEARCH_FIELD, fld);
                sl.Add(SEARCH_STRING, val);
            }

            ViewState["search_params"] = sl;

            upd_record_count();
        }

        void search(int pg)
        {
            ArrayList a0 = null;
            ArrayList a1 = null;
            SortedList sl = new SortedList();

            m_results.Clear();

            using (stock_movement_audit_trail smat = new stock_movement_audit_trail())
            {
                smat.pg = pg;
                smat.recs_per_pg = REC_PER_PG;
                smat.order_by = "datetime_stamp DESC";

                DateTime dt = DateTime.Now;
                string search_field = string.Empty;
                string search_string = string.Empty;

                get_search_params(ref  dt, ref  search_field, ref  search_string);

                a0 = smat.get_stock_movement_audit_trail_data(dt, search_field, search_string);
            }

            using (parts m = new parts())
            {
                using (consumable_parts c = new consumable_parts())
                {
                    using (consignment_instance n = new consignment_instance())
                    {
                        foreach (stock_movement_audit_trail_data smatd in a0)
                        {
                            if (a1 != null)
                                a1.Clear();

                            sl.Clear();
                            sl.Add("id", smatd.part_id);

                            stock_movement_entry sm = new stock_movement_entry();
                            sm.smatd = smatd;

                            if (smatd.part_type == "M" || smatd.part_type == "N")
                            {
                                a1 = m.get_part_data(sl);

                                if (a1.Count > 0)
                                {
                                    part_data pd = (part_data)a1[0];
                                    sm.desc = pd.description;
                                }

                            }
                            else if (smatd.part_type == "C")
                            {
                                a1 = c.get_consumable_part_data(sl);

                                if (a1.Count > 0)
                                {
                                    consumable_part_data cpd = (consumable_part_data)a1[0];
                                    sm.desc = cpd.description;
                                    if (cpd.additional_description.Trim().Length > 0)
                                        sm.desc += " / " + cpd.additional_description.Trim();
                                }
                            }
                            else if (smatd.part_type == "oldN")
                            {
                                a1 = n.get_consignment_data(sl);

                                if (a1.Count > 0)
                                {
                                    consignment_instance_data cd = (consignment_instance_data)a1[0];
                                    sm.desc = cd.description;
                                    if (cd.additional_description.Trim().Length > 0)
                                        sm.desc += " / " + cd.additional_description.Trim();
                                    if (cd.size.Trim().Length > 0)
                                        sm.desc += " / " + cd.size.Trim();
                                }
                            }

                            m_results.Add(sm);
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

        void display()
        {
            tblResults.Rows.Clear();

            if (m_results.Count == 0)
                return;

            TableRow r;
            TableCell c;

            string[] hdr = new string[] { "Date/Time", "Part", "Qty", "Event", "Destination", "User" };

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

            int i = 0;

            foreach(stock_movement_entry sm in m_results)
            {
                r = new TableRow();

                System.Drawing.Color bc;
                if (i++ % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("LightGray");

                r.BackColor = bc;

                c = new TableCell();
                c.Controls.Add(new LiteralControl(sm.smatd.datetime_stamp.ToString("dd/MM/yyyy HH:mm:ss")));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(sm.desc));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(sm.smatd.quantity.ToString("0.00")));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(sm.smatd.movement_event));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(sm.smatd.destination));
                r.Cells.Add(c);


                c = new TableCell();
                c.Controls.Add(new LiteralControl(sm.smatd.user_id));
                r.Cells.Add(c);

                tblResults.Rows.Add(r);
            }
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            new_search();
        }
    }

    [Serializable]
    public class stock_movement_entry
    {
        public string desc = string.Empty;
        public stock_movement_audit_trail_data smatd = null;
    }
}
