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
    public partial class totals : System.Web.UI.Page
    {
        const string VS_RESULTS = "results";

        SortedList m_results = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                search();
                display();
            }
            else
            {
                m_results = (SortedList)ViewState[VS_RESULTS];
                display();
            }
        }

        void search()
        {
            m_results.Clear();

            using (cdb_connection dbc = new cdb_connection())
            {
                DateTime dt_from = DateTime.Now.AddMonths(-3);
                dt_from = new DateTime(dt_from.Year, dt_from.Month, dt_from.Day);

                string sql_select = "select barcode, date_created as dt, material from spools where date_created > @dt_from order by date_created, barcode";

                SortedList sl_p = new SortedList();
                sl_p.Add("@dt_from", dt_from);

                DataTable dtab = dbc.get_data_p(sql_select, sl_p);

                foreach (DataRow dr in dtab.Rows)
                {
                    ctotals ctr = get_ctotals_rec(dr);

                    if (ctr != null)
                    {
                        increment_created_totals(dr, ctr);
                    }
                }

                sql_select = "select barcode, datetime_stamp as dt, material from qa_jobs  ";
                sql_select += " left join spools on qa_jobs.spool_id = spools.id ";
                sql_select += " where datetime_stamp  > @dt_from ";

                sl_p.Clear();
                sl_p.Add("@dt_from", dt_from);

                dtab = dbc.get_data_p(sql_select, sl_p);

                foreach (DataRow dr in dtab.Rows)
                {
                    ctotals ctr = get_ctotals_rec(dr);

                    if (ctr != null)
                    {
                        increment_qa_totals(dr, ctr);
                    }
                }
            }

            ViewState[VS_RESULTS] = m_results;
        }

        void display()
        {
            tblResults.Rows.Clear();

            if (m_results.Count == 0)
                return;

            TableRow r;
            TableCell c;

            Table tbl = null;
            TableRow r0 = null;
            TableCell c0 = null;

            string[] hdr = new string[] { "Date", "Project", "Spools Created", "Spools QA'd" };
            string[] hdr2 = new string[] { "HS", "Carbon", "Stainless", "All" };

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

            /////////////
            r = new TableRow();

            c = new TableCell();
            c.Controls.Add(new LiteralControl(string.Empty));
            r.Cells.Add(c);

            c = new TableCell();
            c.Controls.Add(new LiteralControl(string.Empty));
            r.Cells.Add(c);

            for (int i0 = 0; i0 < 2; i0++)
            {
                c = new TableCell();
                c.BackColor = System.Drawing.Color.FromName("LightGreen");
                r.Cells.Add(c);

                tbl = new Table();
                tbl.Style.Add("width", "100%");
                c.Controls.Add(tbl);

                r0 = new TableRow();
                r0.HorizontalAlign = HorizontalAlign.Right;

                tbl.Rows.Add(r0);

                foreach (string sh in hdr2)
                {
                    c0 = new TableCell();
                    c0.Style.Add("width", "25%");
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(sh));
                    r0.Cells.Add(c0);
                }

            }

            tblResults.Rows.Add(r);


            ////////////

            int i = 0;

            for(int n = m_results.Count-1; n>=0; n--)
            {
                SortedList sl = (SortedList)m_results.GetByIndex(n);

                int day_total_spools, day_total_qa, day_total_carbon_created, day_total_carbon_qa, day_total_stainless_created, day_total_stainless_qa, day_total_hs_created, day_total_hs_qa;
                day_total_spools = day_total_qa = day_total_carbon_created = day_total_carbon_qa = day_total_stainless_created = day_total_stainless_qa = day_total_hs_created = day_total_hs_qa =0;

                System.Drawing.Color bc;
                if (i++ % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("LightGray");

                foreach (DictionaryEntry e0 in sl)
                {
                    ctotals ctr = (ctotals)e0.Value;

                    day_total_spools += ctr.spools_created;
                    day_total_qa += ctr.spools_QAd;

                    day_total_carbon_created += ctr.carbon_created;
                    day_total_carbon_qa += ctr.carbon_QAd;
                    
                    day_total_stainless_created += ctr.stainless_created;
                    day_total_stainless_qa += ctr.stainless_QAd;

                    day_total_hs_created += ctr.hs_created;
                    day_total_hs_qa += ctr.hs_QAd;

                    r = new TableRow();

//                    r.BackColor = bc;

                    c = new TableCell();
                    c.BackColor = bc;
                    c.Controls.Add(new LiteralControl(ctr.dt.ToString("dd/MM/yyyy")));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.BackColor = bc;
                    c.Controls.Add(new LiteralControl(ctr.project));
                    r.Cells.Add(c);

                    // Created
                                                            
                    c = new TableCell();
                                        
                    r.Cells.Add(c);

                    tbl = new Table();
                    tbl.Style.Add("width", "100%"); 
                    c.Controls.Add(tbl);

                    r0 = new TableRow();
                    r0.HorizontalAlign = HorizontalAlign.Right;

                    tbl.Rows.Add(r0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%"); 
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.hs_created.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%"); 
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.carbon_created.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%"); 
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.stainless_created.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%"); 
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.spools_created.ToString()));
                    r0.Cells.Add(c0);

                    // QA
                    
                    c = new TableCell();

                    r.Cells.Add(c);

                    tbl = new Table();
                    tbl.Style.Add("width", "100%");
                    c.Controls.Add(tbl);

                    r0 = new TableRow();
                    r0.HorizontalAlign = HorizontalAlign.Right;

                    tbl.Rows.Add(r0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%");
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.hs_QAd.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%");
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.carbon_QAd.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%");
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.stainless_QAd.ToString()));
                    r0.Cells.Add(c0);

                    c0 = new TableCell();
                    c0.BackColor = bc;
                    c0.Style.Add("width", "25%");
                    c0.HorizontalAlign = HorizontalAlign.Right;
                    c0.Controls.Add(new LiteralControl(ctr.spools_QAd.ToString()));
                    r0.Cells.Add(c0);

                    tblResults.Rows.Add(r);
                }

                r = new TableRow();
                r.Font.Bold = true;
                c = new TableCell();
                c.Controls.Add(new LiteralControl(string.Empty));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl(string.Empty));
                r.Cells.Add(c);

                // Created

                c = new TableCell();
                c.BackColor = System.Drawing.Color.FromName("LightPink");
                r.Cells.Add(c);

                tbl = new Table();
                tbl.Style.Add("width", "100%");
                c.Controls.Add(tbl);

                r0 = new TableRow();
                r0.HorizontalAlign = HorizontalAlign.Right;

                tbl.Rows.Add(r0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_hs_created.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_carbon_created.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_stainless_created.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_spools.ToString()));
                r0.Cells.Add(c0);

                // QA

                c = new TableCell();
                c.BackColor = System.Drawing.Color.FromName("LightPink");
                r.Cells.Add(c);

                tbl = new Table();
                tbl.Style.Add("width", "100%");
                c.Controls.Add(tbl);

                r0 = new TableRow();
                r0.HorizontalAlign = HorizontalAlign.Right;

                tbl.Rows.Add(r0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_hs_qa.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_carbon_qa.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_stainless_qa.ToString()));
                r0.Cells.Add(c0);

                c0 = new TableCell();
                c0.Style.Add("width", "25%");
                c0.HorizontalAlign = HorizontalAlign.Right;
                c0.Controls.Add(new LiteralControl(day_total_qa.ToString()));
                r0.Cells.Add(c0);

                tblResults.Rows.Add(r);
            }
        }

        void increment_created_totals(DataRow dr, ctotals ctr)
        {
            try
            {
                ctr.spools_created++;

                string material = dr["material"].ToString();

                if (material.ToLower().StartsWith("carbon"))
                    ctr.carbon_created++;
                else if (material.ToLower().StartsWith("stainless"))
                    ctr.stainless_created++;
                else if (material.ToLower().StartsWith("hs"))
                    ctr.hs_created++; ;
            }
            catch { }
        }

        void increment_qa_totals(DataRow dr, ctotals ctr)
        {
            try
            {
                ctr.spools_QAd++;

                string material = dr["material"].ToString();

                if (material.ToLower().StartsWith("carbon"))
                    ctr.carbon_QAd++;
                else if (material.ToLower().StartsWith("stainless"))
                    ctr.stainless_QAd++;
                else if (material.ToLower().StartsWith("hs"))
                    ctr.hs_QAd++;
            }
            catch { }
        }

        ctotals get_ctotals_rec(DataRow dr)
        {
            ctotals ctr = null;
            string sdt = string.Empty;
            string project = string.Empty;
            DateTime dt = DateTime.MinValue;

            try
            { 
                dt = (DateTime)dr["dt"];
                sdt =  dt.ToString("yyyyMMdd"); 
            }
            catch { }

            try { project = dr["barcode"].ToString().Split('-')[0]; }
            catch { }

            if (sdt.Length > 0 && project.Length > 0)
            {
                SortedList sl = null;

                if (m_results.ContainsKey(sdt))
                {
                    sl = (SortedList)m_results[sdt];
                }
                else
                {
                    sl = new SortedList();
                    m_results.Add(sdt, sl);
                }

                if (sl.ContainsKey(project))
                {
                    ctr = (ctotals)sl[project];
                }
                else
                {
                    ctr = new ctotals();
                    ctr.project = project;
                    ctr.dt = dt;
                    sl.Add(project, ctr);
                }
            }

            return ctr;
        }

        [Serializable]
        class ctotals
        {
            public string project = string.Empty;
            public DateTime dt = DateTime.MaxValue;
            public int spools_created = 0;
            public int spools_QAd = 0;
            public int carbon_created = 0;
            public int stainless_created = 0;
            public int hs_created = 0;
            public int carbon_QAd = 0;
            public int stainless_QAd = 0;
            public int hs_QAd = 0;
        }
    }
}
