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

        const string TXT_MPI_FW = "txt_mpi_fw";
        const string TXT_MPI_BW = "txt_mpi_bw";
        const string TXT_UT_BW = "txt_ut_bw";
        const string TXT_XRAY_BW = "txt_xray_bw";
        const string TXT_DP_FW = "txt_dp_fw";
        const string TXT_DP_BW = "txt_dp_bw";
        const string TXT_VI_FW = "txt_vi_fw";
        const string TXT_VI_BW = "txt_vi_bw";

        const string WELD_TESTS = "weld_tests";
        const string WELDER_TOTALS = "welder_totals";
        const string SPOOL_DATA = "spool_data";
        const string GBEBLUE = "#c4e2ed";
        const string PURPLE = "#FF90EE90";
        const string WHITE = "White";
        const string LIGHTGRAY = "LightGray";
        const string LIGHTGREEN = "LightGreen";
        const int COL1WIDTH = 330;
        const int COLWIDTH = 120;
        const string ROW_TYPE = "row_type";
        const string ROW_TYPE_REPORT_DETAILS = "row_type_report_details";
        const string ROW_TYPE_WELDER_TESTS = "row_type_welder_tests";
        const string BARCODE = "barcode";
        const string WELDER = "welder";
        const string DSB = "deepskyblue";
        const string FAILURE_CODES = "failure_codes";

        SortedList m_sl_weld_tests = new SortedList();
        SortedList m_sl_welder_totals = new SortedList();
        SortedList m_sl_spool_data = new SortedList();
        SortedList m_sl_weld_test_failure_codes = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_sl_weld_tests = (SortedList)ViewState[WELD_TESTS];
                m_sl_welder_totals = (SortedList)ViewState[WELDER_TOTALS];
                m_sl_spool_data = (SortedList)ViewState[SPOOL_DATA];
                m_sl_weld_test_failure_codes = (SortedList)ViewState[FAILURE_CODES];

                display();
            }
            else
            {
                txtSearch.Focus();
            }
        }

        void search()
        {
            lblMsg.Text = string.Empty;

            string project = txtSearch.Text.Trim();
            string fromDate = txtDTFrom.Text.Trim();
            string toDate = txtDTTo.Text.Trim();
            DateTime dtFrom = DateTime.MinValue;
            DateTime dtTo = DateTime.MaxValue;

            if (project.Length > 5)
            {
                if (fromDate.Length > 0)
                {
                    try
                    {
                        fromDate += " 00:00:00";
                        dtFrom = DateTime.ParseExact(fromDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    }
                    catch
                    {
                        lblMsg.Text = "Invalid from date";
                        return;
                    }
                }

                if (toDate.Length > 0)
                {
                    try
                    {
                        toDate += " 23:59:59";
                        dtTo = DateTime.ParseExact(toDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    }
                    catch
                    {
                        lblMsg.Text = "Invalid to date";
                        return;
                    }
                }
                try
                {
                    weld_mapping_ext_tables wmet = new weld_mapping_ext_tables();

                    wmet.get_spool_part_data(project, ref m_sl_spool_data, ref m_sl_welder_totals);
                    wmet.get_weld_test_data(project, ref m_sl_weld_tests, ref m_sl_welder_totals, m_sl_spool_data, dtFrom, dtTo);
                    wmet.get_weld_test_failure_codes(project, ref m_sl_weld_test_failure_codes);

                    ViewState[SPOOL_DATA] = m_sl_spool_data;
                    ViewState[WELD_TESTS] = m_sl_weld_tests;
                    ViewState[WELDER_TOTALS] = m_sl_welder_totals;
                    ViewState[FAILURE_CODES] = m_sl_weld_test_failure_codes;
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                }
            }
            else
            {
                lblMsg.Text = "At least the first 6 characters from a spool number required";
            }
        }

        void display()
        {
            weld_mapping_ext_tables wmet = new weld_mapping_ext_tables();

            wmet.populate_table(tblResults, m_sl_weld_tests, m_sl_spool_data, m_sl_welder_totals, m_sl_weld_test_failure_codes, txtSearch.Text.Trim().ToUpper(), 
                btn_save_report_details_Click, btn_save_test_details_Click);

        }

        private void btn_save_test_details_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            weld_mapping_ext_tables wmet = new weld_mapping_ext_tables();

            wmet.save_test_details( b, tblResults, ref m_sl_weld_tests, ref m_sl_welder_totals, ref m_sl_weld_test_failure_codes);
            
            ViewState[WELD_TESTS] = m_sl_weld_tests;
            ViewState[WELDER_TOTALS] = m_sl_welder_totals;
            display();
        }

        int s2i(string s)
        {
             int i = 0;

            try {i = Convert.ToInt32(s); } catch { }

            return i;
        }

        private void btn_save_report_details_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            weld_mapping_ext_tables wmet = new weld_mapping_ext_tables();

            wmet.save_report_details(b, tblResults, m_sl_weld_tests);
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

                        //Control cntrl = c.Controls[0];

                        foreach (Control cntrl in c.Controls)
                        {
                            if (cntrl.GetType() == typeof(LiteralControl))
                                line += ((LiteralControl)(cntrl)).Text;
                            else if (cntrl.GetType() == typeof(TextBox))
                                line += ((TextBox)(cntrl)).Text;
                            else if (cntrl.GetType() == typeof(DropDownList))
                                line += "\x20" + ((DropDownList)(cntrl)).Text;
                        }

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