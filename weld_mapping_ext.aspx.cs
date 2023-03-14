using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

namespace gbe
{
    public partial class weld_mapping_ext : System.Web.UI.Page
    {
        const string SPOOL_ID = "spool_id";
        const string TXT_R1_MPI_FW = "txt_r1_mpi_fw";
        const string TXT_R2_MPI_BW = "txt_r2_mpi_bw";
        const string TXT_R3_UT_BW = "txt_r3_ut_bw";
        const string TXT_R4_XRAY_BW = "txt_r4_xray_bw";
        const string TXT_R5_DP_FW = "txt_r5_dp_fw";
        const string TXT_R6_DP_BW = "txt_r6_dp_bw";
        const string TXT_R7_VI_FW = "txt_r7_vi_fw";
        const string TXT_R8_VI_BW = "txt_r8_vi_bw";
        const string WELD_TESTS = "weld_tests";
        const string WELDER_TOTALS = "welder_totals";
        const string SPOOL_DATA = "spool_data";
        const string GBEBLUE = "#c4e2ed";
        const int COL1WIDTH = 330;

        SortedList m_sl_weld_tests = new SortedList();
        SortedList m_sl_welder_totals = new SortedList();
        SortedList m_sl_spool_data = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_sl_weld_tests = (SortedList)ViewState[WELD_TESTS];
                m_sl_welder_totals = (SortedList)ViewState[WELDER_TOTALS];
                m_sl_spool_data = (SortedList)ViewState[SPOOL_DATA];

                display();
            }
            else
            {
                txtSearch.Focus();
            }
        }

        bool get_spool_part_data(string project)
        {
            bool bret = true;

            if (m_sl_spool_data != null)
                m_sl_spool_data.Clear();
            else
                m_sl_spool_data = new SortedList();

            if (m_sl_welder_totals != null)
                m_sl_welder_totals.Clear();
            else
                m_sl_welder_totals = new SortedList();

            string weld_map_part_type_excludes = string.Empty;

            try { weld_map_part_type_excludes = System.Web.Configuration.WebConfigurationManager.AppSettings["weld_map_part_type_excludes"].ToString().Trim(); }
            catch { weld_map_part_type_excludes = string.Empty; }

            string sql_weld_map_part_type_excludes = string.Empty;

            if (weld_map_part_type_excludes.Length > 0)
            {
                sql_weld_map_part_type_excludes = " and parts.part_type not in " + weld_map_part_type_excludes;
            }

            string select = " select barcode "
                + " , users_spool_welder.login_id as spool_welder "
                + " , parts.description as spool_part "
                + " , spool_parts.welder as spool_part_welder "
                + " , spool_parts.fw "
                + " , spool_parts.bw "
                + ""
                + " from spools "
                + " left join users as users_spool_welder on users_spool_welder.id = spools.welder "
                + " left join spool_parts on spool_parts.spool_id = spools.id "
                + " left join parts on parts.id = spool_parts.part_id "
                + ""
                + " where "
                + ""
                + " spools.barcode like '" + project + "%' "
                + sql_weld_map_part_type_excludes
                //+ " and "
                //+ " spools.checked_by is not null "
                + " and "
                + " spools.include_in_weld_map=1 "
                + " and "
                + " spool_parts.include_in_weld_map=1 "
                + ""
                + " order by "
                + " spools.barcode "
                + " , parts.description "
                + "";

            try
            {
                using (cdb_connection dbc = new cdb_connection())
                {
                    
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

                            spool_data_weld_mapping_ext sd = new spool_data_weld_mapping_ext();

                            sd.spool_part = dr_wm.s_gf("spool_part");
                            sd.spool_welder = dr_wm.s_gf("spool_welder");
                            sd.spool_part_welder = dr_wm.s_gf("spool_part_welder");
                            sd.fw = dr_wm.i_gf("fw");
                            sd.bw = dr_wm.i_gf("bw");

                            string barcode = dr_wm.s_gf("barcode");

                            ArrayList a_sd = null;

                            if (m_sl_spool_data.ContainsKey(barcode))
                            {
                                a_sd = (ArrayList)m_sl_spool_data[barcode];
                            }
                            else
                            {
                                a_sd = new ArrayList();
                                m_sl_spool_data.Add(barcode, a_sd);
                            }

                            a_sd.Add(sd);

                            string welder = string.Empty;

                            if (sd.spool_part_welder.Trim().Length > 0)
                                welder = sd.spool_part_welder.Trim();
                            else
                                welder = sd.spool_welder.Trim();

                            if (welder.Length > 0)
                            {
                                weld_test_ext_fw_bw fw_bw = null;

                                if (m_sl_welder_totals.ContainsKey(welder))
                                {
                                    fw_bw = (weld_test_ext_fw_bw)m_sl_welder_totals[welder];
                                }
                                else
                                {
                                    fw_bw = new weld_test_ext_fw_bw();
                                    m_sl_welder_totals.Add(welder, fw_bw);
                                }

                                fw_bw.fw += sd.fw;
                                fw_bw.bw += sd.bw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
                bret = false;
            }

            ViewState[SPOOL_DATA] = m_sl_spool_data;
            ViewState[WELDER_TOTALS] = m_sl_welder_totals;

            return bret;
        }

        bool get_weld_test_data(string project)
        {
            bool bret = true;

            if (m_sl_weld_tests != null)
                m_sl_weld_tests.Clear();
            else
                m_sl_weld_tests = new SortedList();

            

            string select = "select distinct weld_test_ext_welder.id  as weld_test_ext_welder_id, "
                    + " barcode, "
                    + " users.login_id as welder, "
                    + " weld_test_ext.*, "
                    + " weld_test_ext_welder.* "

                    + " from weld_test_ext "

                    + " left join weld_test_ext_welder on weld_test_ext_welder.weld_test_ext_id = weld_test_ext.id "
                    + " left join users on users.id = weld_test_ext_welder.welder_id "
                    + " left join spools on spools.id = weld_test_ext.spool_id "

                    + " where spools.barcode like '" + project + "%' "
                    
                    + " order by spools.barcode  "
                    + "";

            try
            {
                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

                            string barcode = dr_wm.s_gf("barcode");

                            weld_test_ext_data wted = null;

                            if (m_sl_weld_tests.ContainsKey(barcode))
                            {
                                wted = (weld_test_ext_data)m_sl_weld_tests[barcode];
                            }
                            else
                            {
                                wted = new weld_test_ext_data();

                                wted.id = dr_wm.i_gf("id");
                                wted.barcode = dr_wm.s_gf("barcode");
                                wted.report1MPI_FW = dr_wm.s_gf("report1MPI_FW");
                                wted.report2MPI_BW = dr_wm.s_gf("report2MPI_BW");
                                wted.report3UT_BW = dr_wm.s_gf("report3UT_BW");
                                wted.report4XRAY_BW = dr_wm.s_gf("report4XRAY_BW");
                                wted.report5DP_FW = dr_wm.s_gf("report5DP_FW");
                                wted.report6DP_BW = dr_wm.s_gf("report6DP_BW");
                                wted.report7VI_FW = dr_wm.s_gf("report7VI_FW");
                                wted.report8VI_BW = dr_wm.s_gf("report8VI_BW");
                                wted.datetime_stamp = dr_wm.dt_gf("datetime_stamp");

                                m_sl_weld_tests.Add(barcode, wted);
                            }

                            weld_test_ext_welder_data wtewd = new weld_test_ext_welder_data();

                            wtewd.mpi_fw = dr_wm.i_gf("mpi_fw");
                            wtewd.ut_bw = dr_wm.i_gf("ut_bw");
                            wtewd.mpi_bw = dr_wm.i_gf("mpi_bw");
                            wtewd.xray_bw = dr_wm.i_gf("xray_bw");
                            wtewd.dp_bw = dr_wm.i_gf("dp_bw");
                            wtewd.dp_fw = dr_wm.i_gf("dp_fw");
                            wtewd.vi_bw = dr_wm.i_gf("vi_bw");
                            wtewd.vi_fw = dr_wm.i_gf("vi_fw");
                            wtewd.welder = dr_wm.s_gf("welder");
                               
                            if (wtewd.welder.ToLower() == "n/a")  // rolls eyes
                                continue;

                            wted.a_weld_test_ext_welder.Add(wtewd);
                        }
                    }
                }

                foreach (DictionaryEntry e0 in m_sl_weld_tests)
                {
                    weld_test_ext_data wted = (weld_test_ext_data)e0.Value;

                    foreach (weld_test_ext_welder_data wtewd in wted.a_weld_test_ext_welder)
                    {
                        weld_test_ext_fw_bw fw_bw = null;

                        if (m_sl_welder_totals.ContainsKey(wtewd.welder))
                        {
                            fw_bw = (weld_test_ext_fw_bw)m_sl_welder_totals[wtewd.welder];
                            
                            fw_bw.fw_tested += wtewd.total_tested_fw();
                            fw_bw.bw_tested += wtewd.total_tested_bw();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
                bret = false;
            }

            ViewState[WELD_TESTS] = m_sl_weld_tests;
            ViewState[WELDER_TOTALS] = m_sl_welder_totals;

            return bret;
        }

        void search()
        {
            string project = txtSearch.Text.Trim();

            if (project.Length > 5)
            {
                if(get_spool_part_data(project))
                    get_weld_test_data(project);
            }
        }

        void display()
        {
            int spool_total_mpi_fw = 0;
            int spool_total_mpi_bw = 0;
            int spool_total_ut_bw = 0;
            int spool_total_xray_bw = 0;
            int spool_total_dp_fw = 0;
            int spool_total_dp_bw = 0;
            int spool_total_vi_fw = 0;
            int spool_total_vi_bw = 0;

            int grand_total_mpi_fw = 0;
            int grand_total_mpi_bw = 0;
            int grand_total_ut_bw = 0;
            int grand_total_xray_bw = 0;
            int grand_total_dp_fw = 0;
            int grand_total_dp_bw = 0;
            int grand_total_vi_fw = 0;
            int grand_total_vi_bw = 0;

            int grand_total_fw = 0;
            int grand_total_bw = 0;

            ArrayList a_hdr_fld_names = new ArrayList()
            {
                new key_value("Welder", "welder_id"),
                new key_value("MPI FW", "mpi_fw"),
                new key_value("MPI BW", "mpi_bw"),
                new key_value("UT BW", "ut_bw"),
                new key_value("XRAY BW", "xray_bw"),
                new key_value("DP FW", "dp_fw"),
                new key_value("DP BW", "dp_bw"),
                new key_value("VI FW", "vi_fw"),
                new key_value("VI BW", "vi_bw")
                /*new key_value("TOTAL", "total")*/
            };

            tblResults.Rows.Clear();

            if (m_sl_weld_tests != null)
            {
                TableRow r;
                TableCell c;

                string[] hdr_welder = new string[] { "Welder", "FW", "BW", "FW tested", "BW tested", "% FW tested", "% BW tested" };

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");

                foreach (string sh in hdr_welder)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblResults.Rows.Add(r);

                int fw_total, bw_total, fw_total_tested, bw_total_tested;
                fw_total = bw_total = fw_total_tested = bw_total_tested = 0;

                foreach (DictionaryEntry e0 in m_sl_welder_totals)
                {
                    string welder = e0.Key.ToString();
                    weld_test_ext_fw_bw fw_bw = (weld_test_ext_fw_bw)e0.Value;

                    fw_total += fw_bw.fw;
                    bw_total += fw_bw.bw;
                    fw_total_tested += fw_bw.fw_tested;
                    bw_total_tested += fw_bw.bw_tested;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(welder));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.fw.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.bw.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.fw_tested.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.bw_tested.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.pc_fw_tested().ToString("0.00")));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(fw_bw.pc_bw_tested().ToString("0.00")));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);
                }

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGray");
                r.Font.Bold = true;

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
                c.Controls.Add(new LiteralControl(fw_total_tested.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(bw_total_tested.ToString()));
                r.Cells.Add(c);

                decimal pc_fw_total_tested = 0;
                decimal pc_bw_total_tested = 0;

                if (fw_total > 0)
                    pc_fw_total_tested = (Convert.ToDecimal(fw_total_tested) / Convert.ToDecimal(fw_total)) * 100;

                if (bw_total > 0)
                    pc_bw_total_tested = (Convert.ToDecimal(bw_total_tested) / Convert.ToDecimal(bw_total)) * 100;

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(pc_fw_total_tested.ToString("0.00")));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(pc_bw_total_tested.ToString("0.00")));
                r.Cells.Add(c);

                tblResults.Rows.Add(r);

                for (int i = 0; i < 3; i++)
                {
                    r = new TableRow();
                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);
                }

                foreach (DictionaryEntry e0 in m_sl_spool_data)
                {
                    string barcode = e0.Key.ToString();
                    ArrayList a_sd = (ArrayList)e0.Value;

                    spool_total_mpi_fw = 0;
                    spool_total_mpi_bw = 0;
                    spool_total_ut_bw = 0;
                    spool_total_xray_bw = 0;
                    spool_total_dp_fw = 0;
                    spool_total_dp_bw = 0;
                    spool_total_vi_fw = 0;
                    spool_total_vi_bw = 0;

                    string[] hdr1 = new string[] { "Spool" };
                    string[] hdr2 = new string[] { string.Empty, "Report MPI FW", "Report MPI BW", "Report UT BW", "Report XRAY BW", "Report DP FW", "Report DP BW", "Report VI FW", "Report VI BW" };

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGreen");

                    foreach (string sh in hdr1)
                    {
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(sh));
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(barcode));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.BackColor = System.Drawing.Color.FromName("LightGreen");
                    c.Controls.Add(new LiteralControl("Welder"));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.BackColor = System.Drawing.Color.FromName("LightGreen");
                    c.Controls.Add(new LiteralControl("FW"));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.BackColor = System.Drawing.Color.FromName("LightGreen");
                    c.Controls.Add(new LiteralControl("BW"));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    int total_fw_spool = 0;
                    int total_bw_spool = 0;

                    foreach (spool_data_weld_mapping_ext sd in a_sd)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(sd.spool_part));
                        r.Cells.Add(c);

                        string welder = string.Empty;

                        if (sd.spool_part_welder.Trim().Length > 0)
                            welder = sd.spool_part_welder.Trim();
                        else
                            welder = sd.spool_welder.Trim();

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(welder));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(sd.fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(sd.bw.ToString()));
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);

                        total_fw_spool += sd.fw;
                        total_bw_spool += sd.bw;

                        

                        grand_total_fw += sd.fw;
                        grand_total_bw += sd.bw;
                    }

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                    c = new TableCell();
                    c.BackColor = System.Drawing.Color.FromName(GBEBLUE);
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.BackColor = System.Drawing.Color.FromName(GBEBLUE);
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(total_fw_spool.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(total_bw_spool.ToString()));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    //
                    if (m_sl_weld_tests.ContainsKey(barcode))
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            r = new TableRow();
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(string.Empty));
                            r.Cells.Add(c);

                            tblResults.Rows.Add(r);
                        }

                        weld_test_ext_data wted = (weld_test_ext_data)m_sl_weld_tests[barcode];
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("deepskyblue");

                        foreach (string sh in hdr2)
                        {
                            c = new TableCell();

                            if (sh.Length == 0)
                                c.BackColor = System.Drawing.ColorTranslator.FromHtml(GBEBLUE);

                            c.Controls.Add(new LiteralControl(sh));
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.BackColor = System.Drawing.ColorTranslator.FromHtml(GBEBLUE);
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r1_mpi_fw = new TextBox();
                        txt_r1_mpi_fw.ID = TXT_R1_MPI_FW + wted.id;
                        txt_r1_mpi_fw.Text = wted.report1MPI_FW;
                        txt_r1_mpi_fw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r1_mpi_fw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r2_mpi_bw = new TextBox();
                        txt_r2_mpi_bw.ID = TXT_R2_MPI_BW + wted.id;
                        txt_r2_mpi_bw.Text = wted.report2MPI_BW;
                        txt_r2_mpi_bw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r2_mpi_bw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r3_ut_bw = new TextBox();
                        txt_r3_ut_bw.ID = TXT_R3_UT_BW + wted.id;
                        txt_r3_ut_bw.Text = wted.report3UT_BW;
                        txt_r3_ut_bw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r3_ut_bw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r4_xray_bw = new TextBox();
                        txt_r4_xray_bw.ID = TXT_R4_XRAY_BW + wted.id;
                        txt_r4_xray_bw.Text = wted.report4XRAY_BW;
                        txt_r4_xray_bw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r4_xray_bw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r5_dp_fw = new TextBox();
                        txt_r5_dp_fw.ID = TXT_R5_DP_FW + wted.id;
                        txt_r5_dp_fw.Text = wted.report5DP_FW;
                        txt_r5_dp_fw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r5_dp_fw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r6_dp_bw = new TextBox();
                        txt_r6_dp_bw.ID = TXT_R6_DP_BW + wted.id;
                        txt_r6_dp_bw.Text = wted.report6DP_BW;
                        txt_r6_dp_bw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r6_dp_bw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r7_vi_fw = new TextBox();
                        txt_r7_vi_fw.ID = TXT_R7_VI_FW + wted.id;
                        txt_r7_vi_fw.Text = wted.report7VI_FW;
                        txt_r7_vi_fw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r7_vi_fw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_r8_vi_bw = new TextBox();
                        txt_r8_vi_bw.ID = TXT_R8_VI_BW + wted.id;
                        txt_r8_vi_bw.Text = wted.report8VI_BW;
                        txt_r8_vi_bw.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(txt_r8_vi_bw);
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Left;
                        c.BackColor = System.Drawing.ColorTranslator.FromHtml(GBEBLUE);
                        ImageButton btn_save_report_details = new ImageButton();
                        btn_save_report_details.ToolTip = "Save report details";
                        btn_save_report_details.ImageUrl = "~/disk.png";
                        btn_save_report_details.Click += btn_save_report_details_Click;
                        btn_save_report_details.ID = "btn_save_report_details" + wted.id.ToString();
                        btn_save_report_details.Attributes[ID] = wted.id.ToString();
                        c.Controls.Add(btn_save_report_details);

                        r.Cells.Add(c);

                        r.Attributes[ID] = wted.id.ToString();

                        tblResults.Rows.Add(r);

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("deepskyblue");

                        foreach (key_value kv in a_hdr_fld_names)
                        {
                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(kv.key));
                            r.Cells.Add(c);

                            if (kv.key == "Welder")
                            {
                                c.Width = COL1WIDTH;
                            }
                            else
                            {
                                c.Width = 120;
                            }
                        }

                        tblResults.Rows.Add(r);

                        foreach (weld_test_ext_welder_data wtewd in wted.a_weld_test_ext_welder)
                        {
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("White");

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(wtewd.welder));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.mpi_fw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.mpi_bw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.ut_bw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.xray_bw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.dp_fw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.dp_bw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.vi_fw.ToString()));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.vi_bw.ToString()));
                            r.Cells.Add(c);

                            /*
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wtewd.total().ToString()));
                            r.Cells.Add(c);
                            */

                            tblResults.Rows.Add(r);

                            spool_total_mpi_fw += wtewd.mpi_fw;
                            spool_total_mpi_bw += wtewd.mpi_bw;
                            spool_total_ut_bw += wtewd.ut_bw;
                            spool_total_xray_bw += wtewd.xray_bw;
                            spool_total_dp_fw += wtewd.dp_fw;
                            spool_total_dp_bw += wtewd.dp_bw;
                            spool_total_vi_fw += wtewd.vi_fw;
                            spool_total_vi_bw += wtewd.vi_bw;

                            grand_total_mpi_fw += wtewd.mpi_fw;
                            grand_total_mpi_bw += wtewd.mpi_bw;
                            grand_total_ut_bw += wtewd.ut_bw;
                            grand_total_xray_bw += wtewd.xray_bw;
                            grand_total_dp_fw += wtewd.dp_fw;
                            grand_total_dp_bw += wtewd.dp_bw;
                            grand_total_vi_fw += wtewd.vi_fw;
                            grand_total_vi_bw += wtewd.vi_bw;
                        }

                        // spool totals
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("LightGray");
                        r.Font.Bold = true;

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl("Total"));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_mpi_fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_mpi_bw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_ut_bw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_xray_bw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_dp_fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_dp_bw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_vi_fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total_vi_bw.ToString()));
                        r.Cells.Add(c);

                        int spool_total = 0;
                        spool_total += spool_total_mpi_fw;
                        spool_total += spool_total_mpi_bw;
                        spool_total += spool_total_ut_bw;
                        spool_total += spool_total_xray_bw;
                        spool_total += spool_total_dp_fw;
                        spool_total += spool_total_dp_bw;
                        spool_total += spool_total_vi_fw;
                        spool_total += spool_total_vi_bw;

                        /*
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spool_total.ToString()));
                        r.Cells.Add(c);
                        */

                        tblResults.Rows.Add(r);
                    }
//

                    for (int i = 0; i < 3; i++)
                    {
                        r = new TableRow();
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);
                    }
                }

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");
                r.Font.Bold = true;

                foreach (key_value kv in a_hdr_fld_names)
                {
                    c = new TableCell();

                    string shdr_vale = string.Empty;

                    if (kv.key == "Welder")
                    {
                        shdr_vale = "Total tests for " + txtSearch.Text.Trim();
                        c.Width = COL1WIDTH;
                    }
                    else
                    {
                        shdr_vale = kv.key;
                        c.Width = 120;
                    }

                    
                    c.Controls.Add(new LiteralControl(shdr_vale));
                    r.Cells.Add(c);
                    tblResults.Rows.Add(r);
                }

                // grand total
                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("PaleVioletRed");
                r.Font.Bold = true;

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(string.Empty));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_mpi_fw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_mpi_bw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_ut_bw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_xray_bw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_dp_fw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_dp_bw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_vi_fw.ToString()));
                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(grand_total_vi_bw.ToString()));
                r.Cells.Add(c);

                int total = 0;

                total += grand_total_mpi_fw;
                total += grand_total_mpi_bw;
                total += grand_total_ut_bw;
                total += grand_total_xray_bw;
                total += grand_total_dp_fw;
                total += grand_total_dp_bw;
                total += grand_total_vi_fw;
                total += grand_total_vi_bw;

                /*
                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Right;
                c.Controls.Add(new LiteralControl(total.ToString()));
                r.Cells.Add(c);
                */

                tblResults.Rows.Add(r);

                int fw_tested, bw_tested;

                fw_tested =  bw_tested = 0;

                if (fw_tested == 0 && grand_total_mpi_fw > 0)
                    fw_tested = grand_total_mpi_fw;
                if (fw_tested == 0 && grand_total_dp_fw > 0)
                    fw_tested = grand_total_dp_fw;
                if (fw_tested == 0 && grand_total_vi_fw > 0)
                    fw_tested = grand_total_vi_fw;

                if (bw_tested == 0 && grand_total_mpi_bw > 0)
                    bw_tested = grand_total_mpi_bw;
                if (bw_tested == 0 && grand_total_ut_bw > 0)
                    bw_tested = grand_total_ut_bw;
                if (bw_tested == 0 && grand_total_xray_bw > 0)
                    bw_tested = grand_total_xray_bw;
                if (bw_tested == 0 && grand_total_dp_bw > 0)
                    bw_tested = grand_total_dp_bw;
                if (bw_tested == 0 && grand_total_vi_bw > 0)
                    bw_tested = grand_total_vi_bw;

                decimal fw_tested_pc, bw_tested_pc;
                fw_tested_pc = bw_tested_pc = 0;

                if(grand_total_fw > 0)
                    fw_tested_pc = (fw_tested / grand_total_fw) * 100;

                if(grand_total_bw > 0)
                    bw_tested_pc = (bw_tested / grand_total_bw) * 100;

                for (int i = 0; i < 3; i++)
                {
                    r = new TableRow();
                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);
                }

                
              
            }
        }

        private void btn_save_report_details_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string weld_test_id = (b.Attributes[ID]);

            string r1, r2, r3, r4, r5, r6, r7, r8;
            r1=  r2= r3= r4= r5= r6= r7= r8= string.Empty;

            foreach (TableRow r in tblResults.Rows)
            {
                if (r.Attributes[ID] == weld_test_id)
                {
                    foreach (TableCell c in r.Cells)
                    {
                        foreach (Control ctrl in c.Controls)
                        {
                            if (ctrl.GetType() == typeof(TextBox))
                            {
                                TextBox tb = (TextBox)ctrl;

                                if (tb.ID.StartsWith(TXT_R1_MPI_FW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r1 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R2_MPI_BW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r2 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R3_UT_BW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r3 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R4_XRAY_BW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r4 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R5_DP_FW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r5 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R6_DP_BW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r6 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R7_VI_FW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r7 = tb.Text;
                                    }
                                }
                                else if (tb.ID.StartsWith(TXT_R8_VI_BW))
                                {
                                    if (tb.Attributes[ID] == weld_test_id)
                                    {
                                        r8 = tb.Text;
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
            }

            int iweld_test_id = 0;

            try { iweld_test_id = Convert.ToInt32(weld_test_id); } catch { }

            if (iweld_test_id > 0)
            {
                using (cdb_connection db = new cdb_connection())
                {
                    using (weld_test_ext wt = new weld_test_ext())
                    {
                        wt.update_reports(r1, r2, r3, r4, r5, r6, r7, r8, iweld_test_id);
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            export(tblResults);
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

                        Control cntrl = c.Controls[0];

                        if (cntrl.GetType() == typeof(LiteralControl))
                            line += ((LiteralControl)(cntrl)).Text;
                        else if (cntrl.GetType() == typeof(TextBox))
                            line += ((TextBox)(cntrl)).Text;

                        if (c.HorizontalAlign != HorizontalAlign.Right)
                            line += DC;
                    }

                    line += CRLF;

                    Response.Write(line);
                }

                Response.End();
            }
        }


    }
}