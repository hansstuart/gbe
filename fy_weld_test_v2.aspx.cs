using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace gbe
{
    public partial class fy_weld_test_v2 : System.Web.UI.Page
    {
        const string SPOOL_ID = "spool_id";
        const string WELDER_ID = "welder_id";
        const string WELDERS = "welders";
        const string BARCODE = "barcode";
        const string FLD_NAME = "fld_name";
        const string CLIENT_REF  = "client_ref";
        const string SPOOL_PARTS = "spool_parts";
        const string SPOOL_PART_ID = "spool_part_id";
        const string PASS = "PASS";
        const string FAIL = "FAIL";
        const string DLFAILCODE = "dlFailCode";
        const string WELD_TYPE = "WELD_TYPE";
        const string FW = "FW";
        const string BW = "BW";
        const string PROJECT = "project";
        
        const string MPI = "MPI";
        const string MPI_FW = "MPI_FW";
        const string MPI_BW = "MPI_BW";
        const string UT_BW = "UT_BW";
        const string XRAY_BW = "XRAY_BW";
        const string DP = "DP";
        const string DP_FW = "DP_FW";
        const string DP_BW = "DP_BW";
        const string VI = "VI";
        const string VI_FW = "VI_FW";
        const string VI_BW = "VI_BW";
        const string PA_BW = "PA_BW";
        const int MAX_WELD_TESTS = 15;

        const string TXTREPORT = "txtReport";

        int m_spool_id = 0;
        string m_barcode = string.Empty;

        SortedList m_sl_wedlers = new SortedList();
        ArrayList m_a_spool_parts = new ArrayList();
        ArrayList m_a_controls = new ArrayList();

        weld_test_ext_data m_wted = null;
        ArrayList m_a_wtewfc = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            getOpenWeldTestProjects();

            retrieveAllControls(this.Page);

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
                try { m_sl_wedlers = (SortedList)ViewState[WELDERS]; } catch { };
                try { m_barcode = ViewState[BARCODE].ToString(); } catch { };
                try { m_a_spool_parts = (ArrayList)ViewState[SPOOL_PARTS]; } catch { };

                display();
            }
            else
            {
                

                foreach (TextBox tb in getReportTextBoxes())
                {
                    tb.Enabled = false;
                }

                txtReport1MPI_FW.Attributes[CLIENT_REF ] = MPI;  
                txtReport2MPI_BW.Attributes[CLIENT_REF] = MPI_BW;
                txtReport3UT_BW.Attributes[CLIENT_REF] = UT_BW;
                txtReport4XRAY_BW.Attributes[CLIENT_REF] = XRAY_BW;
                txtReport5DP_FW.Attributes[CLIENT_REF] = DP;  
                txtReport6DP_BW.Attributes[CLIENT_REF] = DP_BW;
                txtReport7VI_FW.Attributes[CLIENT_REF] = VI;  
                txtReport8VI_BW.Attributes[CLIENT_REF] = VI_BW;
                txtReport9PA_BW.Attributes[CLIENT_REF] = PA_BW;

                chkReport1MPI_FW.Attributes[FLD_NAME] =  MPI_FW;
                chkReport2MPI_BW.Attributes[FLD_NAME] = MPI_BW;
                chkReport3UT_BW.Attributes[FLD_NAME] = UT_BW;
                chkReport4XRAY_BW.Attributes[FLD_NAME] = XRAY_BW;
                chkReport5DP_FW.Attributes[FLD_NAME] =  DP_FW;
                chkReport6DP_BW.Attributes[FLD_NAME] = DP_BW;
                chkReport7VI_FW.Attributes[FLD_NAME] =  VI_FW;
                chkReport8VI_BW.Attributes[FLD_NAME] = VI_BW;
                chkReport9PA_BW.Attributes[FLD_NAME] = PA_BW;

                chkReport1MPI_FW.Attributes[CLIENT_REF ] = MPI;  
                chkReport2MPI_BW.Attributes[CLIENT_REF] = MPI_BW;
                chkReport3UT_BW.Attributes[CLIENT_REF] = UT_BW;
                chkReport4XRAY_BW.Attributes[CLIENT_REF] = XRAY_BW;
                chkReport5DP_FW.Attributes[CLIENT_REF] = DP;  
                chkReport6DP_BW.Attributes[CLIENT_REF] = DP_BW;
                chkReport7VI_FW.Attributes[CLIENT_REF] = VI;  
                chkReport8VI_BW.Attributes[CLIENT_REF] = VI_BW;
                chkReport9PA_BW.Attributes[CLIENT_REF] = PA_BW;

                dlPassFail.Items.Add(PASS);
                dlPassFail.Items.Add(FAIL);
            }
        }

        void getOpenWeldTestProjects()
        {
            tblActiveTestProjects.Rows.Clear();

            string user_name = System.Web.HttpContext.Current.User.Identity.Name;
            int weld_tester_id = fy.ws().get_user_id(fy.PASSKEY, user_name);
            ArrayList a_projects = new ArrayList();

            string select = $@" select barcode 
                            FROM weld_test_ext
                            join spools on spool_id = spools.id

                            where weld_tester_id = {weld_tester_id}
                            and                            
                            sent_to_iris = 0
                            and
                            pass = 1";

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
                            string barcode = dr_s.s_gf("barcode").ToUpper();

                            string [] sa = barcode.Split('-');

                            if (sa.Length > 0)
                            {
                                string project = sa[0];

                                if (!a_projects.Contains(project))
                                {
                                    a_projects.Add(project);
                                }
                            }
                        }
                    }
                }

                if (a_projects.Count > 0)
                {
                    TableRow r;
                    TableCell c;

                    ArrayList a_hdr_sp_fld_names = new ArrayList()
                    {
                        "Active Weld Test Projects|500", "Close|0"
                    };

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGreen");

                    foreach (string s in a_hdr_sp_fld_names)
                    {
                        string [] sa = s.Split('|');

                        c = new TableCell();
                        c.Width = Convert.ToInt32(sa[1]);
                        c.Controls.Add(new LiteralControl(sa[0]));
                        r.Cells.Add(c);
                    }

                    tblActiveTestProjects.Rows.Add(r);

                    foreach (string project in a_projects)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("LightGray");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(project));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        CheckBox chk = new CheckBox();
                        chk.ID = "chk"+project;
                        chk.Attributes[PROJECT] = project;
                        c.Controls.Add(chk);
                        r.Cells.Add(c);


                        tblActiveTestProjects.Rows.Add(r);
                    }

                    r = new TableRow();

                    c = new TableCell();
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    Button btnClose = new Button();
                    btnClose.ID = "btnClose";
                    btnClose.Text = "Close";
                    btnClose.CssClass = "FY_TableTextBox";
                    btnClose.Click += btnClose_Click;
                    c.Controls.Add(btnClose);
                    r.Cells.Add(c);

                    tblActiveTestProjects.Rows.Add(r);
                }
            }
            catch { }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList a_projects = new ArrayList();

                foreach (TableRow r in tblActiveTestProjects.Rows)
                {
                    foreach (TableCell c in r.Cells)
                    {
                        foreach (Control cntrl in c.Controls)
                        {
                            if (cntrl.GetType() == typeof(CheckBox))
                            {
                                CheckBox chk = (CheckBox)cntrl;

                                if (chk.Checked)
                                {
                                    string project = chk.Attributes[PROJECT];
                                    a_projects.Add(project.Trim());
                                }
                            }
                        }
                    }
                }

                if (a_projects.Count > 0)
                {
                    string user_name = System.Web.HttpContext.Current.User.Identity.Name;
                    int weld_tester_id = fy.ws().get_user_id(fy.PASSKEY, user_name);

                    using (cdb_connection dbc = new cdb_connection())
                    {
                        foreach (string project in a_projects)
                        {
                            if (project.Length > 0)
                            {
                                string select = $@" select weld_test_ext.id 
                                FROM weld_test_ext
                                join spools on spool_id = spools.id

                                where 
                                sent_to_iris = 0
                                and
                                pass = 1
                                and
                                weld_tester_id = {weld_tester_id}
                                and
                                barcode like '{project}%'";

                                DataTable dtab = dbc.get_data(select);

                                if (dtab.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtab.Rows)
                                    {
                                        weld_test_ext_data wted = new weld_test_ext_data();
                                        wted.init(dr);

                                        string update = $@" update
                                                        weld_test_ext
                                                        set sent_to_iris = 1
                                                        where id = {wted.id}";

                                        SqlCommand cmd = new SqlCommand();

                                        cmd.Connection = dbc.m_sql_connection;
                                        cmd.CommandText = update;
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "btnClose_Click()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }

            getOpenWeldTestProjects();
        }

        List<TextBox> getReportTextBoxes()
        {
            List<TextBox> lb = new List<TextBox>();

            foreach (Control control in m_a_controls)
                {
                    if (control.GetType() == typeof(TextBox))
                    {
                        TextBox tb = (TextBox)control;
                        
                        if(tb.ID.StartsWith(TXTREPORT))
                            lb.Add(tb);
                    }
                }

            return lb;
        }

        void retrieveAllControls(Control control)
        {
            foreach (Control ctr in control.Controls)
            {
                if (ctr != null)
                {
                    m_a_controls.Add(ctr);             

                    if (ctr.HasControls())
                    {
                        retrieveAllControls(ctr);
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            foreach (TableRow r in tblWeldTest.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.GetType() == typeof(TextBox))
                        {
                            TextBox tb = (TextBox)cntrl;

                            string fld = tb.Attributes[FLD_NAME];
                            string welder = tb.Attributes[WELDER_ID];

                            if (fld != null && fld.Trim().Length > 0)
                            {
                                if (tb.Text.Trim().Length > 0)
                                {
                                    int num_of_welds = 0;

                                    try {num_of_welds = Convert.ToInt32(tb.Text.Trim()); } catch { }
                                    

                                    for (int i = 0; i < MAX_WELD_TESTS; i++)
                                    {
                                        DropDownList dl = (DropDownList)FindControlRecursive(Page, DLFAILCODE + "_" + fld + "_" + welder + "_" + i.ToString("000"));

                                        if(dl != null)
                                            dl.CssClass = "invisible";  
                                    }

                                    for (int i = 0; i < num_of_welds; i++)
                                    {
                                        if(i >= MAX_WELD_TESTS)
                                            break;

                                        DropDownList dl = (DropDownList)FindControlRecursive(Page, DLFAILCODE + "_" + fld + "_" + welder + "_" + i.ToString("000"));

                                        if(dl != null)
                                            dl.CssClass = "FY_TableTextBox";
                                    }
                                }
                            }
                        }
                    }
                }
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
                else if (ret == 1)
                {
                    lblMsg.Text = "Spool or welder data not found";
                }
            }
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            save();
            getOpenWeldTestProjects();
        }

        void save()
        {
            try
            {
                string all_reports = txtReport1MPI_FW.Text.Trim() + txtReport2MPI_BW.Text.Trim() + txtReport3UT_BW.Text.Trim() + 
                    txtReport4XRAY_BW.Text.Trim() + txtReport5DP_FW.Text.Trim() + txtReport6DP_BW.Text.Trim() + txtReport7VI_FW.Text.Trim() +
                    txtReport8VI_BW.Text.Trim() + txtReport9PA_BW.Text.Trim();

                if (all_reports.Length == 0)
                {
                    lblMsg.Text = "Client Reference(s) missing";
                    return;
                }

                ArrayList a_welder_tests = new ArrayList();
                ArrayList a_fail_codes = new ArrayList();

                foreach (TableRow r in tblSpoolParts.Rows /*tblWeldTest.Rows*/)
                {
                    SortedList sl_welder_test = null;
                    SortedList sl_fail_code = null;

                    foreach (TableCell c in r.Cells)
                    {
                        //welder_id = r.Attributes[WELDER_ID];

                        foreach (Control cntrl in c.Controls)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                TextBox tb = (TextBox)cntrl;

                                string fld = tb.Attributes[FLD_NAME];

                                if (fld != null && fld.Trim().Length > 0)
                                {
                                    if (sl_welder_test == null)
                                        sl_welder_test = new SortedList();

                                    if (!sl_welder_test.ContainsKey(fld))
                                        sl_welder_test.Add(fld, tb.Text);
                                }
                            }

                            if (cntrl.GetType() == typeof(DropDownList))
                            {
                                DropDownList dl = (DropDownList)cntrl;

                                string weld_type = dl.Attributes[WELD_TYPE];
                                string welder_id = dl.Attributes[WELDER_ID];
                                string spool_part_id = dl.Attributes[SPOOL_PART_ID];

                                if (dl.Text.Trim().Length > 0)
                                {
                                    sl_fail_code = new SortedList();
                                    sl_fail_code.Add("weld_type", weld_type);
                                    sl_fail_code.Add("failure_code", dl.Text);
                                    sl_fail_code.Add("spool_part_id", spool_part_id);
                                    sl_fail_code.Add(WELDER_ID, welder_id);
                                    sl_fail_code.Add(SPOOL_ID, m_spool_id);

                                    a_fail_codes.Add(sl_fail_code);
                                }
                            }
                        }
                    }

                    /*
                    if (sl_welder_test != null)
                    {
                        sl_welder_test.Add(WELDER_ID, welder_id);
                        sl_welder_test.Add(SPOOL_ID, m_spool_id);
                        a_welder_tests.Add(sl_welder_test);
                    }
                    */

                    /*
                    if (sl_fail_code != null)
                    {
                        sl_fail_code.Add(WELDER_ID, welder_id);
                        sl_fail_code.Add(SPOOL_ID, m_spool_id);
                        a_fail_codes.Add(sl_fail_code);
                    }
                    */
                }

                //if (a_welder_tests.Count > 0)
                //{
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
                    sl_weldtest.Add("report9PA_BW", txtReport9PA_BW.Text.Trim());
                    sl_weldtest.Add("version", "2");
                    sl_weldtest.Add("pass", dlPassFail.Text==PASS?1:0);
                    sl_weldtest.Add("sent_to_iris", 0);

                    weld_test_ext wte = new weld_test_ext();
                    wte.save_weld_test_ext_data(sl_weldtest, a_welder_tests, a_fail_codes);
                //}

                lblMsg.Text = "Weld test saved for " + txtBarcode.Text;

                tblWeldTest.Rows.Clear();
                tblSpoolParts.Rows.Clear();

                m_spool_id = 0;
                m_barcode = string.Empty;
                m_sl_wedlers.Clear();
                m_a_spool_parts.Clear();

                ViewState[SPOOL_ID] = m_spool_id;
                ViewState[BARCODE] = m_barcode;
                ViewState[WELDERS] = m_sl_wedlers;
                ViewState[SPOOL_PARTS] = m_a_spool_parts;

                txtBarcode.Text = string.Empty;
                btnSave.Visible = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "save()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        void save_old()
        {
            try
            {
                string all_reports = txtReport1MPI_FW.Text.Trim() + txtReport2MPI_BW.Text.Trim() + txtReport3UT_BW.Text.Trim() + 
                    txtReport4XRAY_BW.Text.Trim() + txtReport5DP_FW.Text.Trim() + txtReport6DP_BW.Text.Trim() + txtReport7VI_FW.Text.Trim() +
                    txtReport8VI_BW.Text.Trim() + txtReport9PA_BW.Text.Trim();

                if(all_reports.Length == 0)
                    return;

                ArrayList a_welder_tests = new ArrayList();
                ArrayList a_fail_codes = new ArrayList();

                foreach (TableRow r in tblWeldTest.Rows)
                {
                    SortedList sl_welder_test = null;
                    SortedList sl_fail_code = null;

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

                                if (fld != null && fld.Trim().Length > 0)
                                {
                                    if (sl_welder_test == null)
                                        sl_welder_test = new SortedList();

                                    if (!sl_welder_test.ContainsKey(fld))
                                        sl_welder_test.Add(fld, tb.Text);
                                }
                            }

                            if (cntrl.GetType() == typeof(DropDownList))
                            {
                                DropDownList dl = (DropDownList)cntrl;

                                string fld = dl.Attributes[FLD_NAME];

                                if (fld != null && fld.Trim().Length > 0 && dl.Text.Trim().Length > 0)
                                {
                                    sl_fail_code = new SortedList();
                                    sl_fail_code.Add("test_type", fld);
                                    sl_fail_code.Add("failure_code", dl.Text);
                                    sl_fail_code.Add(WELDER_ID, welder_id);
                                    sl_fail_code.Add(SPOOL_ID, m_spool_id);
                                    a_fail_codes.Add(sl_fail_code);
                                }
                            }
                        }
                    }

                    if (sl_welder_test != null)
                    {
                        sl_welder_test.Add(WELDER_ID, welder_id);
                        sl_welder_test.Add(SPOOL_ID, m_spool_id);
                        a_welder_tests.Add(sl_welder_test);
                    }

                    /*
                    if (sl_fail_code != null)
                    {
                        sl_fail_code.Add(WELDER_ID, welder_id);
                        sl_fail_code.Add(SPOOL_ID, m_spool_id);
                        a_fail_codes.Add(sl_fail_code);
                    }
                    */
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
                    sl_weldtest.Add("report9PA_BW", txtReport9PA_BW.Text.Trim());
                    sl_weldtest.Add("version", "2");
                    sl_weldtest.Add("pass", dlPassFail.Text==PASS?1:0);
                    sl_weldtest.Add("sent_to_iris", 0);

                    weld_test_ext wte = new weld_test_ext();
                    wte.save_weld_test_ext_data(sl_weldtest, a_welder_tests, a_fail_codes);
                }

                lblMsg.Text = "Weld test saved for " + txtBarcode.Text;

                tblWeldTest.Rows.Clear();
                tblSpoolParts.Rows.Clear();

                m_spool_id = 0;
                m_barcode = string.Empty;
                m_sl_wedlers.Clear();
                m_a_spool_parts.Clear();

                ViewState[SPOOL_ID] = m_spool_id;
                ViewState[BARCODE] = m_barcode;
                ViewState[WELDERS] = m_sl_wedlers;
                ViewState[SPOOL_PARTS] = m_a_spool_parts;

                txtBarcode.Text = string.Empty;
                btnSave.Visible = false;
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

            if (m_a_spool_parts == null)
                m_a_spool_parts = new ArrayList();
            else
                m_a_spool_parts.Clear();

            m_barcode = string.Empty;

            m_spool_id = 0;

            // welders from spool_parts
            string select_welders = "select barcode, spools.id as spool_id, spool_parts.welder as spool_parts_welder, users.id as welder_id "
                            + " from spools "
                            + "  join spool_parts on spool_parts.spool_id = spools.id "
                            + "  join users on users.login_id = REPLACE(spool_parts.welder,'Robot/','') "
                            + " where barcode = '" + barcode + "' ";
            try
            {
                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select_welders);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            data_row_spool dr_s = new data_row_spool(dr);

                            if (m_spool_id == 0)
                                m_spool_id = dr_s.i_gf("spool_id");

                            m_barcode = dr_s.s_gf("barcode").ToUpper();

                            int welder_id = dr_s.i_gf("welder_id");
                            string welder = dr_s.s_gf("spool_parts_welder");

                            if (!m_sl_wedlers.ContainsKey(welder))
                                m_sl_wedlers.Add(welder, welder_id);
                        }

                        string select_spool_parts = $@"select spool_parts.id as spool_parts_id, fw, bw, welder spool_parts_welder, description
                                        from spool_parts
                                        join parts on parts.id = part_id
                                        where 
                                        spool_id = {m_spool_id} 
                                        and include_in_weld_map = 1
                                        order by seq";

                        dtab = dbc.get_data(select_spool_parts);

                        if (dtab.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtab.Rows)
                            {
                                m_a_spool_parts.Add(new spool_part(dr));
                            }

                            string select_weld_test_ext = $@"select * 
                                                    from weld_test_ext
                                                    where 
                                                    spool_id = {m_spool_id} 
                                                    and 
                                                    version=2
                                                    and 
                                                    sent_to_iris=0
                                                    order by id desc";

                            dtab = dbc.get_data(select_weld_test_ext);

                            if (dtab.Rows.Count > 0)
                            {
                                m_wted = new weld_test_ext_data();
                                m_wted.init(dtab.Rows[0]);

                                if (!m_wted.pass)
                                {
                                    string select_weld_test_failure_codes = $@"select * 
                                                    from weld_test_ext_welder_failure_codes
                                                    where 
                                                    weld_test_ext_id = {m_wted.id} order by id";

                                    dtab = dbc.get_data(select_weld_test_failure_codes);

                                    if (dtab.Rows.Count > 0)
                                    {
                                        m_a_wtewfc = new ArrayList();

                                        foreach (DataRow dr in dtab.Rows)
                                        {
                                            weld_test_ext_welder_failure_codes_data wtewfc = new weld_test_ext_welder_failure_codes_data();
                                            wtewfc.init(dr);
                                            m_a_wtewfc.Add(wtewfc);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ret = 2;
                        }
                    }
                    else
                    {
                        ret = 1;
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
            ViewState[SPOOL_PARTS] = m_a_spool_parts;

            return ret;
        }

        void display()
        {
            if (m_wted != null) 
            {
                txtReport1MPI_FW.Text = m_wted.report1MPI_FW;
                txtReport2MPI_BW.Text = m_wted.report2MPI_BW;
                txtReport3UT_BW.Text = m_wted.report3UT_BW;
                txtReport4XRAY_BW.Text = m_wted.report4XRAY_BW;
                txtReport5DP_FW.Text = m_wted.report5DP_FW;
                txtReport6DP_BW.Text = m_wted.report6DP_BW;
                txtReport7VI_FW.Text = m_wted.report7VI_FW;
                txtReport8VI_BW.Text = m_wted.report8VI_BW;
                txtReport9PA_BW.Text = m_wted.report9PA_BW;

                chkReport1MPI_FW.Checked = m_wted.report1MPI_FW.Trim().Length > 0;
                chkReport2MPI_BW.Checked = m_wted.report2MPI_BW.Trim().Length > 0;
                chkReport3UT_BW.Checked = m_wted.report3UT_BW.Trim().Length > 0;
                chkReport4XRAY_BW.Checked = m_wted.report4XRAY_BW.Trim().Length > 0;
                chkReport5DP_FW.Checked = m_wted.report5DP_FW.Trim().Length > 0;
                chkReport6DP_BW.Checked = m_wted.report6DP_BW.Trim().Length > 0;
                chkReport7VI_FW.Checked = m_wted.report7VI_FW.Trim().Length > 0;
                chkReport9PA_BW.Checked = m_wted.report9PA_BW.Trim().Length > 0;
            }

            update_report_textboxes_with_project_code();

            if (m_sl_wedlers != null && m_sl_wedlers.Count > 0 && m_a_spool_parts != null && m_a_spool_parts.Count > 0)
            {
                btnSave.Visible = true;

                tblWeldTest.Visible = false;

                tblWeldTest.Rows.Clear();

                ArrayList a_hdr_wt_fld_names = new ArrayList()
                {
                    new key_value("Welder", "welder_id"),
                    new key_value("MPI FW", MPI_FW),
                    new key_value("MPI BW", MPI_BW),
                    new key_value("UT BW", UT_BW),
                    new key_value("XRAY BW", XRAY_BW),
                    new key_value("DP FW", DP_FW),
                    new key_value("DP BW", DP_BW),
                    new key_value("VI FW", VI_FW),
                    new key_value("VI BW", VI_BW),
                    new key_value("PA BW", PA_BW),
                };

                TableRow r;
                TableCell c;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");

                foreach (key_value kv in a_hdr_wt_fld_names)
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

                    if (welder == "N/A")
                        continue;

                    int welder_id = (int)e0.Value;
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");
                    r.Attributes[WELDER_ID] = welder_id.ToString();

                    foreach (key_value kv in a_hdr_wt_fld_names)
                    {
                        c = new TableCell();

                        if (kv.key == "Welder")
                        {
                            c.Controls.Add(new LiteralControl(welder));
                        }
                        else
                        {
                            c.VerticalAlign = VerticalAlign.Top;

                            string sid = kv.value + "_" + welder_id.ToString();
                            TextBox tb_welds_tested = create_numeric_textbox(sid);
                            tb_welds_tested.CssClass = "FY_TableTextBox";
                            tb_welds_tested.Attributes[FLD_NAME] = kv.value.ToString();
                            tb_welds_tested.Attributes[WELDER_ID] = welder_id.ToString();
                            tb_welds_tested.Enabled = false;

                            //tb_welds_tested.AutoPostBack = true;
                            //tb_welds_tested.TextChanged += Tb_welds_tested_TextChanged;
                            tb_welds_tested.Attributes.Add("onkeyup", $"showFailCodeDroplists('{sid}', '{MAX_WELD_TESTS}')");
                            
                            c.Controls.Add(tb_welds_tested);

                            for (int i = 0; i < MAX_WELD_TESTS; i++)
                            {
                                DropDownList dlFailCode = new DropDownList();
                                dlFailCode.ID = DLFAILCODE + "_" + sid + "_" + i.ToString("000");
                                dlFailCode.CssClass = "invisible"; //"FY_TableTextBox";
                                dlFailCode.Attributes[FLD_NAME] = kv.value.ToString();
                                dlFailCode.Attributes[WELDER_ID] = welder_id.ToString();
                                dlFailCode.Items.Add(string.Empty);
                                dlFailCode.Items.Add("F1");
                                dlFailCode.Items.Add("F2");
                                dlFailCode.Items.Add("F3");
                                dlFailCode.AutoPostBack = true;
                                dlFailCode.SelectedIndexChanged += DlFailCode_SelectedIndexChanged;
                                c.Controls.Add(dlFailCode);

                                //dlFailCode.Enabled = false;
                                //dlFailCode.Visible = false;
                            }
                        }

                        r.Cells.Add(c);
                    }

                    tblWeldTest.Rows.Add(r);
                }

                enableAllWeldTestControls();
                

                ArrayList a_hdr_sp_fld_names = new ArrayList()
                {
                    "Part|500", "Welder|150", "FW|0", "BW|0"
                };

                tblSpoolParts.Rows.Clear();

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");

                foreach (string s in a_hdr_sp_fld_names)
                {
                    string [] sa = s.Split('|');

                    c = new TableCell();
                    c.Width = Convert.ToInt32(sa[1]);
                    c.Controls.Add(new LiteralControl(sa[0]));
                    r.Cells.Add(c);
                }

                tblSpoolParts.Rows.Add(r);

                foreach (spool_part sp in m_a_spool_parts)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");
                    r.Attributes[SPOOL_PART_ID] = sp.spool_parts_id.ToString();

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    c.Controls.Add(new LiteralControl(sp.part_description));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    c.Controls.Add(new LiteralControl(sp.spool_parts_welder));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    TextBox tb = create_numeric_textbox(FW + sp.spool_parts_id.ToString());
                    tb.CssClass = "FY_TableTextBox";
                    tb.Attributes[FLD_NAME] = FW;
                    tb.Text = sp.fw.ToString();
                    tb.Enabled = false;
                    tb.Style[HtmlTextWriterStyle.TextAlign] = "right";
                    c.Controls.Add(tb);
                    r.Cells.Add(c);

                    ArrayList a_wtewfc = new ArrayList();

                    if (m_a_wtewfc != null)
                    {
                        foreach (weld_test_ext_welder_failure_codes_data wtewfc in m_a_wtewfc)
                        {
                            if (wtewfc.spool_part_id == sp.spool_parts_id
                                && wtewfc.weld_type == FW)
                            {
                                a_wtewfc.Add(wtewfc);
                            }
                        }
                    }

                    for (int i = 0; i < sp.fw; i++)
                    {
                        DropDownList dlFailCode = new DropDownList();
                        dlFailCode.ID = DLFAILCODE + FW + sp.spool_parts_id.ToString() + "_" + i.ToString("000");
                        dlFailCode.CssClass = "FY_TableTextBox";
                        dlFailCode.Attributes[SPOOL_PART_ID] = sp.spool_parts_id.ToString();
                        dlFailCode.Attributes[WELDER_ID] = ((int)m_sl_wedlers[sp.spool_parts_welder]).ToString();
                        dlFailCode.Attributes[WELD_TYPE] = FW; 
                        dlFailCode.Items.Add(string.Empty);
                        dlFailCode.Items.Add("F1");
                        dlFailCode.Items.Add("F2");
                        dlFailCode.Items.Add("F3");
                        dlFailCode.AutoPostBack = true;
                        dlFailCode.SelectedIndexChanged += DlFailCode_SelectedIndexChanged;
                        c.Controls.Add(dlFailCode);

                        if(a_wtewfc.Count > i)
                            dlFailCode.Text = ((weld_test_ext_welder_failure_codes_data)a_wtewfc[i]).failure_code;
                    }

                    

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    tb = create_numeric_textbox(BW + sp.spool_parts_id.ToString());
                    tb.CssClass = "FY_TableTextBox";
                    tb.Attributes[FLD_NAME] = BW;
                    tb.Text = sp.bw.ToString();
                    tb.Enabled = false;
                    tb.Style[HtmlTextWriterStyle.TextAlign] = "right";
                    c.Controls.Add(tb);
                    r.Cells.Add(c);

                    a_wtewfc = new ArrayList();

                    if (m_a_wtewfc != null)
                    {
                        foreach (weld_test_ext_welder_failure_codes_data wtewfc in m_a_wtewfc)
                        {
                            if (wtewfc.spool_part_id == sp.spool_parts_id
                                && wtewfc.weld_type == BW)
                            {
                                a_wtewfc.Add(wtewfc);
                            }
                        }
                    }

                    for (int i = 0; i < sp.bw; i++)
                    {
                        DropDownList dlFailCode = new DropDownList();
                        dlFailCode.ID = DLFAILCODE + BW + sp.spool_parts_id.ToString() + "_" + i.ToString("000");
                        dlFailCode.CssClass = "FY_TableTextBox";
                        dlFailCode.Attributes[SPOOL_PART_ID] = sp.spool_parts_id.ToString();
                        dlFailCode.Attributes[WELDER_ID] = ((int)m_sl_wedlers[sp.spool_parts_welder]).ToString();
                        dlFailCode.Attributes[WELD_TYPE] = BW; 
                        dlFailCode.Items.Add(string.Empty);
                        dlFailCode.Items.Add("F1");
                        dlFailCode.Items.Add("F2");
                        dlFailCode.Items.Add("F3");
                        dlFailCode.AutoPostBack = true;
                        dlFailCode.SelectedIndexChanged += DlFailCode_SelectedIndexChanged;
                        c.Controls.Add(dlFailCode);

                        if(a_wtewfc.Count > i)
                            dlFailCode.Text = ((weld_test_ext_welder_failure_codes_data)a_wtewfc[i]).failure_code;
                    }  

                    tblSpoolParts.Rows.Add(r);
                }

                set_pass_fail();
            }
        }

        private void Tb_welds_tested_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox) sender;
            int num_of_welds = 0;

            try {num_of_welds = Convert.ToInt32(tb.Text.Trim()); } catch { }

            if (num_of_welds > 0)
            {
                string fld = tb.Attributes[FLD_NAME];
                string welder = tb.Attributes[WELDER_ID];

                for (int i = 0; i < MAX_WELD_TESTS; i++)
                {
                    DropDownList dl = (DropDownList)FindControlRecursive(Page, DLFAILCODE + "_" + fld + "_" + welder + "_" + i.ToString("000"));

                    if(dl != null)
                        dl.Visible = false;
                }

                for (int i = 0; i < num_of_welds; i++)
                {
                    DropDownList dl = (DropDownList)FindControlRecursive(Page, DLFAILCODE + "_" + fld + "_" + welder + "_" + i.ToString("000"));

                    if(dl != null)
                        dl.Visible = true;
                }
            }

            //dlFailCode.ID = DLFAILCODE + "_" + kv.value + "_" + welder_id.ToString()+ "_" + i.ToString("000");
        }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID) return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null) return controlToReturn;
            }
            return null;
        }

        private void DlFailCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            set_pass_fail();
        }

        void set_pass_fail()
        {
            bool bFail = false;

            foreach (TableRow row in tblSpoolParts.Rows) // was tblWeldTest
            {
                foreach (TableCell cell in row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        if (control.GetType() == typeof(DropDownList))
                        {
                            DropDownList dl = (DropDownList)control;

                            if (dl.ID.StartsWith(DLFAILCODE))
                            {
                                if (dl.Text.Trim().Length > 0)
                                    bFail = true;
                            }
                        }
                    }
                }
            }

            if (bFail)
                dlPassFail.Text = FAIL;
            else
                dlPassFail.Text = PASS;
        }

        TextBox create_numeric_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 90;
            tb.MaxLength = 3;
            tb.Text = "";
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

            public decimal d_gf(string fld)
            {
                return base.d_gf(m_dr, fld);
            }
        }

        [Serializable]
        public class spool_part
        {
            public int spool_parts_id { get; set; }
            public int fw { get; set; }
            public int bw { get; set; }
            public int seq { get; set; }
            public string part_description { get; set; }
            public string spool_parts_welder { get; set; }
            public decimal qty { get; set; }

            public spool_part(DataRow dr)
            {
                data_row_spool dr_s = new data_row_spool(dr);

                spool_parts_id = dr_s.i_gf("spool_parts_id");
                fw = dr_s.i_gf("fw");
                bw = dr_s.i_gf("bw");
                seq = dr_s.i_gf("seq");
                spool_parts_welder = dr_s.s_gf("spool_parts_welder");
                part_description = dr_s.s_gf("description");
                qty = dr_s.d_gf("qty");
            }
        }

        void update_report_textboxes_with_project_code()
        {
            string sep = "-";

            if (m_barcode.Contains(sep))
            {
                string project = m_barcode.Split(sep[0])[0] + "/";

                foreach (TextBox tb in getReportTextBoxes())
                {
                    update_report_textbox_with_project_code(tb);
                }
            }
        }

        void update_report_textbox_with_project_code(TextBox tb)
        {
            string sep = "-";

            if (m_barcode.Contains(sep))
            {
                string project = m_barcode.Split(sep[0])[0] + "/";
                
                if (tb.Text.Length > 0)
                {
                    if (!tb.Text.StartsWith(project))
                        tb.Text = project + generateDefaultReportName(tb.Attributes[CLIENT_REF]);
                }
            }
        }

        protected void btnResetReports_Click(object sender, EventArgs e)
        {
            chkReport9PA_BW.Checked = chkReport1MPI_FW.Checked = chkReport2MPI_BW.Checked = chkReport3UT_BW.Checked = chkReport4XRAY_BW.Checked =
                chkReport5DP_FW.Checked = chkReport6DP_BW.Checked = chkReport7VI_FW.Checked = chkReport8VI_BW.Checked = false;

            txtReport1MPI_FW.Text = txtReport2MPI_BW.Text = txtReport3UT_BW.Text = txtReport4XRAY_BW.Text = txtReport5DP_FW.Text =
                txtReport6DP_BW.Text = txtReport7VI_FW.Text = txtReport8VI_BW.Text = txtReport9PA_BW.Text = string.Empty;

            

            enableAllWeldTestControls();
            set_pass_fail();
        }

        protected void chkReport1MPI_FW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport1MPI_FW, txtReport1MPI_FW);
        }

        protected void chkReport2MPI_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport2MPI_BW, txtReport2MPI_BW);
        }

        protected void chkReport3UT_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport3UT_BW, txtReport3UT_BW);
        }

        protected void chkReport4XRAY_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport4XRAY_BW, txtReport4XRAY_BW);
        }

        protected void chkReport5DP_FW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport5DP_FW, txtReport5DP_FW);
        }

        protected void chkReport6DP_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport6DP_BW, txtReport6DP_BW);
        }

        protected void chkReport7VI_FW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport7VI_FW, txtReport7VI_FW);
        }

        protected void chkReport8VI_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport8VI_BW, txtReport8VI_BW);
        }

        protected void chkReport9PA_BW_CheckedChanged(object sender, EventArgs e)
        {
            reportCheckBoxChanged(chkReport9PA_BW, txtReport9PA_BW);
        }

        string generateDefaultReportName(string client_ref)
        {
            string userName = System.Web.HttpContext.Current.User.Identity.Name;

            return $"{DateTime.Now.ToString("yyyyMMdd")}/{client_ref}/{userName}";
        }

        void reportCheckBoxChanged(CheckBox chkbox, TextBox txtReport)
        {
            if (chkbox.Checked)
            {
                //txtRport.Enabled = true;

                if (txtReport.Text.Trim().Length == 0)
                {
                    txtReport.Text = generateDefaultReportName(chkbox.Attributes[CLIENT_REF]);
                    update_report_textbox_with_project_code(txtReport);
                }
            }
            else
            {
                txtReport.Text = string.Empty;
                txtReport.Enabled = false;
            }

            enableWeldTestControls(chkbox);
            set_pass_fail();
        }

        void enableWeldTestControls(CheckBox chkbox)
        {
            string fld_name = chkbox.Attributes[FLD_NAME];

            foreach (TableRow row in tblWeldTest.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    foreach (Control control in cell.Controls)
                    {
                        if (control.GetType() == typeof(DropDownList))
                        {
                            DropDownList dl = (DropDownList)control;

                            if (dl.ID.StartsWith(DLFAILCODE) && dl.Attributes[FLD_NAME] == fld_name)
                            {
                                if (chkbox.Checked)
                                {
                                    dl.Enabled = true;
                                }
                                else
                                {
                                    dl.Text = string.Empty;
                                    dl.Enabled = false;
                                }
                            }
                        }

                        if (control.GetType() == typeof(TextBox))
                        {
                            TextBox tb = (TextBox)control;

                            if (tb.ID.StartsWith(fld_name))
                            {
                                if (chkbox.Checked)
                                {
                                    tb.Enabled = true;
                                }
                                else
                                {
                                    tb.Text = string.Empty;
                                    tb.Enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        void enableAllWeldTestControls()
        {
            enableWeldTestControls(chkReport1MPI_FW);
            enableWeldTestControls(chkReport2MPI_BW);
            enableWeldTestControls(chkReport3UT_BW);
            enableWeldTestControls(chkReport4XRAY_BW);
            enableWeldTestControls(chkReport5DP_FW);
            enableWeldTestControls(chkReport6DP_BW);
            enableWeldTestControls(chkReport7VI_FW);
            enableWeldTestControls(chkReport8VI_BW);
            enableWeldTestControls(chkReport9PA_BW);
        }
    }
}