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
    public partial class weld_mapping : System.Web.UI.Page
    {
        SortedList m_spools = null;
        SortedList m_weld_tests = null;
        SortedList m_welders = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_spools = (SortedList)ViewState["spools"];
                m_weld_tests = (SortedList)ViewState["weld_tests"];
                m_welders = (SortedList)ViewState["welders"];

                display();
            }
            else
            {
                get_welders();

                txtSearch.Focus();
                lblBreakdown.Visible = lblSummary.Visible = false;
            }
        }

        void get_welders()
        {
            m_welders = new SortedList();

            ArrayList a = new ArrayList();

            SortedList sl = new SortedList();
            sl.Add("role", "WELDER");

            using (users u = new users())
            {
                a = u.get_user_data(sl);
            }

            dlWelder.Items.Add(string.Empty);

            foreach (user_data ud in a)
            {
                if (ud.login_id.Trim().Length > 0)
                    if (!m_welders.ContainsKey(ud.login_id.Trim()))
                        m_welders.Add(ud.login_id.Trim(), ud);
            }

            foreach(DictionaryEntry e0 in m_welders)
                dlWelder.Items.Add(e0.Key.ToString());

            ViewState["welders"] = m_welders;
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        void search()
        {
            SortedList sl = new SortedList();

            if (m_spools != null)
                m_spools.Clear();
            else
                m_spools = new SortedList();

            if (m_weld_tests != null)
                m_weld_tests.Clear();
            else
                m_weld_tests = new SortedList();


            if (txtSearch.Text.Trim().Length > 3)
            {
                string select = "select spools.id, spool, barcode, revision, users.login_id, spool_parts.id as spool_parts_id, spool_parts.qty, spool_parts.fw, spool_parts.bw, spool_parts.include_in_weld_map, parts.part_number, parts.description, weld_tests.spool_id, weld_tests.report_number, weld_tests.report2_number, weld_tests.fw as wtfw, weld_tests.bw as wtbw, weld_jobs.robot ";
                select += " from spools ";
                select += " inner join spool_parts on spool_parts.spool_id=spools.id ";
                select += " inner join parts on spool_parts.part_id=parts.id ";
                select += " left join users on users.id = spools.welder ";
                select += " left join weld_tests on weld_tests.spool_id = spools.id ";
                select += " left join weld_jobs on spools.id=weld_jobs.spool_id ";
                select += " where barcode like '" + txtSearch.Text.Trim() + "%' ";
                select += " and spools.include_in_weld_map=1";
                select += " and spool_parts.include_in_weld_map=1";
                select += " order by spools.id";

                try
                {
                    using (cdb_connection dbc = new cdb_connection())
                    {
                        DataTable dtab = dbc.get_data(select);

                        int prev_spool_id = -1;

                        spool_data sd = null;

                        foreach (DataRow dr in dtab.Rows)
                        {
                            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

                            int spool_id = dr_wm.i_gf("id");

                            if (spool_id != prev_spool_id)
                            {
                                sd = null;

                                prev_spool_id = spool_id;

                                string welder = string.Empty;

                                if (dr_wm.i_gf("robot") > 0)
                                    welder = "Robot";

                                string sw = dr_wm.s_gf("login_id");

                                if (sw.Trim().Length > 0)
                                {
                                    if (welder.Trim().Length > 0)
                                    {
                                        welder += "/";
                                    }
                                }
                                
                                welder += sw;

                                if (welder.Trim().Length > 0)
                                {
                                    sd = new spool_data();
                                    sd.id = spool_id;
                                    sd.barcode = dr_wm.s_gf("barcode");
                                    sd.spool = dr_wm.s_gf("spool");
                                    sd.revision = dr_wm.s_gf("revision");
                                    sd.welder_data = new user_data();

                                    sd.welder_data.login_id = welder;

                                    m_spools.Add(sd.welder_data.login_id.PadRight(10) + sd.barcode + sd.id, sd);

                                    sd.spool_part_data = new ArrayList();

                                    weld_test_data wtd = new weld_test_data();
                                    wtd.bw = dr_wm.i_gf("wtbw");
                                    wtd.fw = dr_wm.i_gf("wtfw");
                                    wtd.report_number = dr_wm.s_gf("report_number");
                                    wtd.report2_number = dr_wm.s_gf("report2_number");

                                    m_weld_tests.Add(sd.id, wtd);
                                }
                            }

                            if (sd != null)
                            {
                                bool badd = true;
                                int spd_id = dr_wm.i_gf("spool_parts_id");

                                foreach (spool_part_data spd in sd.spool_part_data)
                                {
                                    if (spd_id == spd.id)
                                    {
                                        badd = false;
                                        break;
                                    }
                                }

                                if (badd)
                                {
                                    spool_part_data spd = new spool_part_data();
                                    spd.id = dr_wm.i_gf("spool_parts_id");

                                    spd.bw = dr_wm.i_gf("bw");
                                    spd.fw = dr_wm.i_gf("fw");
                                    spd.include_in_weld_map = dr_wm.b_gf("include_in_weld_map");
                                    spd.part_data = new part_data();
                                    spd.part_data.part_number = dr_wm.s_gf("part_number");
                                    spd.part_data.description = dr_wm.s_gf("description");
                                    spd.qty = dr_wm.d_gf("qty");
                                    sd.spool_part_data.Add(spd);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                }
            }

            ViewState["spools"] = m_spools;
            ViewState["weld_tests"] = m_weld_tests;
        }

        void search_old()
        {
            SortedList sl = new SortedList();

            if (m_spools != null)
                m_spools.Clear();

            if (txtSearch.Text.Trim().Length > 3)
            {
                string fld, val;
                fld = val = string.Empty;

                val = txtSearch.Text.Trim();

                fld = "barcode";
                val += "%";

                sl.Add(fld, val);

                sl.Add("include_in_weld_map", true);

                if (dlWelder.Text.Trim().Length > 0)
                {
                    user_data ud = (user_data)m_welders[dlWelder.Text.Trim()];
                    sl.Add("welder", ud.id);
                }

                ArrayList a = new ArrayList();

                using (spools spls = new spools())
                {
                    a = spls.get_spool_data_ex(sl);
                }
                               

                m_spools = new SortedList();
                m_weld_tests = new SortedList();

                using (weld_tests wt = new weld_tests())
                {
                    foreach (spool_data sd in a)
                    {
                        string welder = string.Empty;

                        if (sd.welder_data != null)
                            welder = sd.welder_data.login_id;

                        m_spools.Add(welder.PadRight(10) + sd.barcode + sd.id, sd);

                        sl.Clear();

                        sl.Add("spool_id", sd.id);


                        ArrayList awt = wt.get_weld_test_data(sl);

                        if (awt.Count > 0)
                            m_weld_tests.Add(sd.id, (weld_test_data)awt[0]);

                    }
                }

                ViewState["spools"] = m_spools;
                ViewState["weld_tests"] = m_weld_tests;
            }
        }

        void display()
        {
            lblBreakdown.Visible = lblSummary.Visible = false;

            if (m_spools != null)
            {
                SortedList sl_summary_totals = new SortedList();

                tblResults.Rows.Clear();
                tblSummary.Rows.Clear();

                TableRow r;
                TableCell c;

                string[] hdr = new string[] { "Spool", "Revision", "Welder", "Test Report 1", "Test Report 2" };
                string[] hdr2 = new string[] { "Part", "Qty", "FW", "BW", "Total FW", "Total BW" };

                int fw_total_welder = 0;
                int bw_total_welder = 0;
                int fw_tested = 0;
                int bw_tested = 0;
                int spool_for_welder = 0;
                decimal fw_pc_tested = 0;
                decimal bw_pc_tested = 0;

                for (int i = 0; i < m_spools.Count; i++ )
                {
                    spool_data sd = (spool_data)m_spools.GetByIndex(i);
                  
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGreen");

                    foreach (string sh in hdr)
                    {
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(sh));
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sd.spool));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sd.revision));
                    r.Cells.Add(c);

                    c = new TableCell();

                    string welder = string.Empty;

                    if (sd.welder_data != null)
                        welder = sd.welder_data.login_id;

                    c.Controls.Add(new LiteralControl(welder));
                    r.Cells.Add(c);

                    string r1, r2;
                    r1 = r2 = string.Empty;

                    if (m_weld_tests.ContainsKey(sd.id))
                    {
                        weld_test_data wtd = (weld_test_data)m_weld_tests[sd.id];

                        r1 = wtd.report_number;
                        r2 = wtd.report2_number;

                        fw_tested += wtd.fw;
                        bw_tested += wtd.bw;
                    }

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(r1));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(r2));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    if (sd.spool_part_data != null)
                    {
                        spool_for_welder++;

                        int fw_total_spool = 0;
                        int bw_total_spool = 0;

                        if (sd.spool_part_data.Count > 0)
                        {
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightGreen");

                            c = new TableCell();
                            c.BackColor = System.Drawing.Color.FromName("LightBlue");
                            c.Controls.Add(new LiteralControl(string.Empty));
                            r.Cells.Add(c);

                            foreach (string sh in hdr2)
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(sh));
                                r.Cells.Add(c);
                            }

                            tblResults.Rows.Add(r);

                            foreach (spool_part_data spd in sd.spool_part_data)
                            {
                                if (spd.include_in_weld_map)
                                {
                                    r = new TableRow();
                                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                                    string part_no, part_desc;
                                    part_no = part_desc = string.Empty;

                                    if (spd.part_data != null)
                                    {
                                        part_no = spd.part_data.part_number;
                                        part_desc = spd.part_data.description;
                                    }



                                    c = new TableCell();
                                    c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.Controls.Add(new LiteralControl(part_desc));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Right;
                                    c.Controls.Add(new LiteralControl(spd.qty.ToString("0.00")));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Right;
                                    c.Controls.Add(new LiteralControl(spd.fw.ToString()));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Right;
                                    c.Controls.Add(new LiteralControl(spd.bw.ToString()));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Right;
                                    c.Controls.Add(new LiteralControl((spd.fw).ToString()));
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Right;
                                    c.Controls.Add(new LiteralControl((spd.bw).ToString()));
                                    r.Cells.Add(c);

                                    fw_total_spool += spd.fw;
                                    bw_total_spool += spd.bw;

                                    tblResults.Rows.Add(r);
                                }
                            }

                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightPink");

                            for (int n = 0; n < 4; n++)
                            {
                                c = new TableCell();
                                c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);
                            }

                            c = new TableCell();
                            c.BackColor = System.Drawing.Color.FromName("LightBlue");
                            c.Controls.Add(new LiteralControl("Spool Total"));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(fw_total_spool.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(bw_total_spool.ToString()));
                            r.Cells.Add(c);

                            tblResults.Rows.Add(r);

                            fw_total_welder += fw_total_spool;
                            bw_total_welder += bw_total_spool;

                            bool b_output_welder_total  = false;

                            if (i == m_spools.Count - 1)
                                b_output_welder_total = true;
                            else
                            {
                                spool_data sd_next = (spool_data)m_spools.GetByIndex(i+1);
                                string next_welder = string.Empty;

                                if (sd_next.welder_data != null)
                                    next_welder = sd_next.welder_data.login_id;

                                if(welder != next_welder)
                                    b_output_welder_total = true;
                            }

                            if(b_output_welder_total)
                            {
                                r = new TableRow();
                                r.BackColor = System.Drawing.Color.FromName("PaleVioletRed");

                                for (int n = 0; n < 4; n++)
                                {
                                    c = new TableCell();
                                    c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                }

                                c = new TableCell();
                                c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                c.Controls.Add(new LiteralControl("Welder Total"));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(fw_total_welder.ToString()));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(bw_total_welder.ToString()));
                                r.Cells.Add(c);

                                tblResults.Rows.Add(r);

                                // qty tested for welder

                                int fw_qty_tested = 0;
                                int bw_qty_tested = 0;

                                try { fw_qty_tested = Convert.ToInt32(fw_tested); }
                                catch { }

                                try { bw_qty_tested = Convert.ToInt32(bw_tested); }
                                catch { }

                                r = new TableRow();
                                r.BackColor = System.Drawing.Color.FromName("PaleVioletRed");

                                for (int n = 0; n < 4; n++)
                                {
                                    c = new TableCell();
                                    c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                }

                                c = new TableCell();
                                c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                c.Controls.Add(new LiteralControl("Qty Tested"));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(fw_qty_tested.ToString()));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(bw_qty_tested.ToString()));
                                r.Cells.Add(c);

                                tblResults.Rows.Add(r);

                                // % tested for welder

                                fw_pc_tested = 0;
                                bw_pc_tested = 0;

                                try { fw_pc_tested = (Convert.ToDecimal(fw_tested) / Convert.ToDecimal(fw_total_welder) * 100); }
                                catch { }

                                try { bw_pc_tested = (Convert.ToDecimal(bw_tested) / Convert.ToDecimal(bw_total_welder) * 100); }
                                catch { }

                                r = new TableRow();
                                r.BackColor = System.Drawing.Color.FromName("PaleVioletRed");

                                for (int n = 0; n < 4; n++)
                                {
                                    c = new TableCell();
                                    c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                }

                                c = new TableCell();
                                c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                c.Controls.Add(new LiteralControl("% Tested"));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(fw_pc_tested.ToString("0.00") + "%"));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(bw_pc_tested.ToString("0.00") + "%"));
                                r.Cells.Add(c);

                                tblResults.Rows.Add(r);

                                // save welder totals
                                welder_totals wt = null;

                                if (sl_summary_totals.ContainsKey(welder))
                                    wt = (welder_totals)sl_summary_totals[welder];
                                else
                                {
                                    wt = new welder_totals();
                                    sl_summary_totals.Add(welder, wt);
                                }

                                wt.bw += bw_total_welder;
                                wt.fw += fw_total_welder;
                                wt.bw_tested += bw_tested;
                                wt.fw_tested += fw_tested;

                                fw_total_welder = bw_total_welder = fw_tested = bw_tested = spool_for_welder = 0;
                            }
                        }
                    }
                }

                if (m_spools.Count > 0)
                {
                    lblBreakdown.Visible = lblSummary.Visible = true;

                    string[] summary_hdr = new string[] { "Welder", "Total FW", "Total BW", "FW Tested", "BW Tested", "% FW Tested", "% BW Tested" };

                     r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGreen");

                    foreach (string sh in summary_hdr)
                    {
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(sh));
                        r.Cells.Add(c);
                    }

                    tblSummary.Rows.Add(r);

                    int fw_total, bw_total;
                    fw_total = bw_total = fw_tested = bw_tested = 0;

                    foreach(DictionaryEntry e0 in sl_summary_totals)
                    {
                        welder_totals wt = (welder_totals)e0.Value;

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("LightPink");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(e0.Key.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(wt.fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(wt.bw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(wt.fw_tested.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(wt.bw_tested.ToString()));
                        r.Cells.Add(c);

                        fw_pc_tested = 0;
                        bw_pc_tested = 0;

                        try { fw_pc_tested = (Convert.ToDecimal(wt.fw_tested) / Convert.ToDecimal(wt.fw) * 100); }
                        catch { }

                        try { bw_pc_tested = (Convert.ToDecimal(wt.bw_tested) / Convert.ToDecimal(wt.bw) * 100); }
                        catch { }

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(fw_pc_tested.ToString("0.00") + "%"));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(bw_pc_tested.ToString("0.00") + "%"));
                        r.Cells.Add(c);

                        tblSummary.Rows.Add(r);

                        bw_total += wt.bw;
                        fw_total += wt.fw;
                        bw_tested += wt.bw_tested;
                        fw_tested += wt.fw_tested;
                    }

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("PaleVioletRed");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl("Total"));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_total.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(bw_total.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_tested.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(bw_tested.ToString()));
                    r.Cells.Add(c);

                    fw_pc_tested = 0;
                    bw_pc_tested = 0;

                    try { fw_pc_tested = (Convert.ToDecimal(fw_tested) / Convert.ToDecimal(fw_total) * 100); }
                    catch { }

                    try { bw_pc_tested = (Convert.ToDecimal(bw_tested) / Convert.ToDecimal(bw_total) * 100); }
                    catch { }

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_pc_tested.ToString("0.00") + "%"));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(bw_pc_tested.ToString("0.00") + "%"));
                    r.Cells.Add(c);

                    tblSummary.Rows.Add(r);

                    /*
                    decimal pc_tested = (Convert.ToDecimal(m_weld_tests.Count) / Convert.ToDecimal(m_spools.Count)) * 100;

                    r = new TableRow();

                    for (int n = 0; n < 7; n++)
                    {
                        c = new TableCell();
                        c.BackColor = System.Drawing.Color.FromName("LightBlue");
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);
                    }

                    c = new TableCell();
                    //c.Font.Bold = true;
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.BackColor = System.Drawing.Color.FromName("Brown");
                    c.Controls.Add(new LiteralControl(pc_tested.ToString("0.00") + "% spools tested."));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);
                     */
                }
            }
        }

        void export(Table tbl)
        {
            if (tbl.Rows.Count > 0)
            {
                string CRLF = "\r\n";
                string DC = "\"";
                string CM = ",";

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=weld_mapping_" + txtSearch.Text.Trim() + "_" + tbl.ID + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");

                string line = string.Empty;

                foreach (TableRow r in tbl.Rows)
                {
                    line = string.Empty;

                    foreach (TableCell c in r.Cells)
                    {
                        if (line.Length > 0)
                            line += CM;

                        if (c.HorizontalAlign != HorizontalAlign.Right)
                            line += DC;

                        line += ((LiteralControl)(c.Controls[0])).Text;

                        if (c.HorizontalAlign != HorizontalAlign.Right)
                            line += DC;
                    }

                    line += CRLF;

                    Response.Write(line);
                }

                Response.End();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            export(tblResults);
        }

        protected void btnExportSummary_Click(object sender, EventArgs e)
        {
            export(tblSummary);
        }
    }

    public class welder_totals
    {
        public int fw = 0;
        public int bw = 0;
        public int bw_tested = 0;
        public int fw_tested = 0;
    }

    public class data_row_weld_mapping : data_row
    {
        public data_row_weld_mapping(DataRow dr)
        {
            m_dr = dr;
        }

        public int i_gf(  string fld)
        {
            return base.i_gf(m_dr, fld);
        }

        public string s_gf( string fld)
        {
            return base.s_gf(m_dr, fld);
        }

        public decimal d_gf( string fld)
        {
            return base.d_gf(m_dr, fld);
        }

        public bool b_gf( string fld)
        {
            return base.b_gf(m_dr, fld);
        }
    }
}
