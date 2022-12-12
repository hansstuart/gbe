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
    public partial class fy_weld_test_ext : System.Web.UI.Page
    {
        const string SPOOL_ID = "spool_id";
        const string WELDER_ID = "welder_id";
        const string WELDERS = "welders";
        const string BARCODE = "barcode";
        const string FLD_NAME = "fld_name";

        int m_spool_id = 0;
        string m_barcode = string.Empty;

        SortedList m_sl_wedlers = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            m_sl_wedlers = new SortedList();

            string user_name = System.Web.HttpContext.Current.User.Identity.Name;

            int user_id = fy.ws().is_weld_tester(fy.PASSKEY, user_name);

            if (user_id == 0)
            {
                Response.Redirect("fy.aspx");
            }

            btnSave.Visible = false;

            if (IsPostBack)
            {
                try { m_spool_id = (int)ViewState[SPOOL_ID]; } catch { };
                try { m_sl_wedlers = (SortedList)ViewState[WELDERS];} catch {  };
                try { m_barcode = ViewState[BARCODE].ToString();} catch { };

                display();
            }
        }

        protected void btnGetSpool_Click(object sender, ImageClickEventArgs e)
        {
            btnSave.Visible = false;

            if (txtBarcode.Text.Trim().Length > 0)
            {
                tblWeldTest.Rows.Clear();

                int ret = search(txtBarcode.Text.Trim());
                if (ret == 0)
                {
                    display();
                    btnSave.Visible = true;
                }
                else if(ret == 1)
                {
                    lblMsg.Text = "Spool or welder data not found";
                }
                

            }
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            save();
        }

        void save()
        {
            try
            {
                ArrayList a_welder_tests = new ArrayList();

                foreach (TableRow r in tblWeldTest.Rows)
                {
                    SortedList sl = null;

                    string welder_id = string.Empty;

                    foreach (TableCell c in r.Cells)
                    {
                        welder_id = r.Attributes[WELDER_ID];

                        foreach (Control cntrl in c.Controls)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                TextBox tb = (TextBox)cntrl;

                                string fld = tb.Attributes[FLD_NAME];

                                if (fld != null)
                                {
                                    if (fld.Trim().Length > 0)
                                    {
                                        if (sl == null)
                                            sl = new SortedList();

                                        if (!sl.ContainsKey(fld))
                                            sl.Add(fld, tb.Text);
                                    }
                                }

                            }
                        }
                    }

                    if (sl != null)
                    {
                        sl.Add(WELDER_ID, welder_id);
                        sl.Add(SPOOL_ID, m_spool_id);
                        a_welder_tests.Add(sl);
                    }
                }

                if (a_welder_tests.Count > 0)
                {
                    SortedList sl_weldtest = new SortedList();

                    string user_name = System.Web.HttpContext.Current.User.Identity.Name;
                    int weld_tester_id = fy.ws().get_user_id(fy.PASSKEY, user_name);

                    sl_weldtest.Add("weld_tester_id", weld_tester_id);
                    sl_weldtest.Add(SPOOL_ID, m_spool_id);
                    sl_weldtest.Add("datetime_stamp", DateTime.Now);
                    sl_weldtest.Add("report1MPI_FW", txtReport1MPI_FW.Text.Trim());
                    sl_weldtest.Add("report2MPI_BW", txtReport2MPI_BW.Text.Trim());
                    sl_weldtest.Add("report3UT_BW", txtReport3UT_BW.Text.Trim());
                    sl_weldtest.Add("report4XRAY_BW", txtReport4XRAY_BW.Text.Trim());
                    sl_weldtest.Add("report5DP_FW", txtReport5DP_FW.Text.Trim());
                    sl_weldtest.Add("report6DP_BW", txtReport6DP_BW.Text.Trim());
                    sl_weldtest.Add("report7VI_FW", txtReport7VI_FW.Text.Trim());
                    sl_weldtest.Add("report8VI_BW", txtReport8VI_BW.Text.Trim());

                    weld_test_ext wte = new weld_test_ext();
                    wte.save_weld_test_ext_data(sl_weldtest, a_welder_tests);
                }

                string msg = "Weld test complete";
                string return_url = "fy_weld_test_ext.aspx";

                fy.show_msg(msg, return_url, this);

                
            }
            catch (Exception ex)
            {
                lblMsg.Text = "save()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        int search(string barcode)
        {
            int ret = 0;

            lblMsg.Text = string.Empty;

            if (m_sl_wedlers == null)
                m_sl_wedlers = new SortedList();
            else
                m_sl_wedlers.Clear();

            m_barcode = barcode;
            m_spool_id = 0;

            // welders from spool_parts
            string select = "select spools.id as spool_id, spool_parts.welder as welder, users.id as welder_id "
                            + " from spools "
                            + "  join spool_parts on spool_parts.spool_id = spools.id "
                            + "  join users on users.login_id = spool_parts.welder "
                            + " where barcode = '" + barcode + "' ";

            try
            {
                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            data_row_spool dr_s = new data_row_spool(dr);

                            if (m_spool_id == 0)
                                m_spool_id = dr_s.i_gf("spool_id");

                            int welder_id = dr_s.i_gf("welder_id");
                            string welder = dr_s.s_gf("welder");

                            if (!m_sl_wedlers.ContainsKey(welder))
                                m_sl_wedlers.Add(welder, welder_id);
                        }
                       
                    }
                    else
                    {
                        ret = 1;

                        /*
                        // if no welders from spool_parts, user welder from spools
                        select = "select spools.id as spool_id, users.id as welder_id, users.login_id as welder "
                            + " from spools "
                            + "  join users on users.id = spools.welder "
                            + " where barcode = '" + barcode + "' ";

                        dtab = dbc.get_data(select);

                        if (dtab.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtab.Rows)
                            {
                                data_row_spool dr_s = new data_row_spool(dr);

                                if (m_spool_id == 0)
                                    m_spool_id = dr_s.i_gf("spool_id");

                                int welder_id = dr_s.i_gf("welder_id");
                                string welder = dr_s.s_gf("welder");

                                if (!m_sl_wedlers.ContainsKey(welder_id))
                                    m_sl_wedlers.Add(welder, welder_id);
                            }

                            bret = true;
                        }
                        else
                            bret = false;
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
                ret = 2;
            }

            ViewState[SPOOL_ID] = m_spool_id;
            ViewState[WELDERS] = m_sl_wedlers;
            ViewState[BARCODE] = m_barcode;

            return ret;
        }

        void display()
        {
            if (m_sl_wedlers != null && m_sl_wedlers.Count > 0)
            {
                btnSave.Visible = true;

                tblWeldTest.Rows.Clear();

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
                new key_value("VI BW", "vi_bw"),
            };

                TableRow r;
                TableCell c;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");

                foreach (key_value kv in a_hdr_fld_names)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(kv.key));
                    r.Cells.Add(c);

                    if (kv.key != "Welder")
                    {
                        c.Width = 120;
                    }
                }

                tblWeldTest.Rows.Add(r);

                foreach (DictionaryEntry e0 in m_sl_wedlers)
                {
                    string welder = e0.Key.ToString();
                    int welder_id = (int)e0.Value;
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");
                    r.Attributes[WELDER_ID] = welder_id.ToString();

                    foreach (key_value kv in a_hdr_fld_names)
                    {
                        c = new TableCell();

                        if (kv.key == "Welder")
                        {
                            c.Controls.Add(new LiteralControl(welder));
                        }
                        else
                        {
                            TextBox tb = create_numeric_textbox(kv.value + "_" + welder_id.ToString());
                            tb.CssClass = "FY_TableTextBox";
                            tb.Attributes[FLD_NAME] = kv.value.ToString();
                            c.Controls.Add(tb);
                        }
                        r.Cells.Add(c);
                    }

                    tblWeldTest.Rows.Add(r);
                }
            }
        }

        TextBox create_numeric_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 90;
            tb.MaxLength = 3;
            tb.Text = "0";
            tb.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tb.ID = id;

            return tb;
        }

        public class data_row_spool : data_row
        {
            public data_row_spool(DataRow dr)
            {
                m_dr = dr;
            }

            public int i_gf(string fld)
            {
                return base.i_gf(m_dr, fld);
            }

            public string s_gf(string fld)
            {
                return base.s_gf(m_dr, fld);
            }

        }
    }
}