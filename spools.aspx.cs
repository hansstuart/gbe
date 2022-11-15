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
    public partial class spools1 : System.Web.UI.Page
    {
        const int REC_PER_PG = 15;
        const string SHOW_PARTS_INSERTION_POINT = "SHOW_PARTS_INSERTION_POINT";
        const string PART_DATA_ROW = "PART_DATA_ROW";
        
        const string VS_SPOOLS = "VS_SPOOLS";
        const string VS_SEARCH_PARAMS = "VS_SEARCH_PARAMS";
        const string VS_CURRENT_PAGE = "VS_CURRENT_PAGE";
        const string VS_RECORD_COUNT = "VS_RECORD_COUNT";

        SortedList m_spools = null;
        SortedList m_welders = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_welders = (SortedList)ViewState["welders"];
                m_spools = (SortedList)ViewState[VS_SPOOLS];
                display();
            }
            else
            {
                get_welders();

                using (spool_status ss = new spool_status())
                {
                    ArrayList a = ss.get_spool_status_data(new SortedList());

                    dlStatus.Items.Add(string.Empty);

                    foreach (spool_status_data ssd in a)
                        dlStatus.Items.Add(ssd.status + " - " + ssd.description);

                    dlSearchFlds.Items.Add("Project");
                    dlSearchFlds.Items.Add("Spool");
                    dlSearchFlds.Items.Add("Barcode");
                    txtSearch.Focus();
                }

                if (Request.Url.Query.Trim().Length > 0)
                {
                    return_from_modify();
                }
            }
        }

        void return_from_modify()
        {
            string v = Request.QueryString["s"];
            if (v != null)
            {
                txtSearch.Text = v;
            }

            v = Request.QueryString["u"];

            if (v != null)
            {
                try
                {
                    dlStatus.SelectedIndex = Convert.ToInt32(v);
                }
                catch { }
            }

            v = Request.QueryString["f"];

            if (v != null)
            {
                try
                {
                    dlSearchFlds.SelectedIndex = Convert.ToInt32(v);
                }
                catch { }
            }

            v = Request.QueryString["h"];

            if (v != null)
            {
                chkSrchOnHold.Checked = (v == "1" ? true : false);
            }

            int pg = 1;

            v = Request.QueryString["pg"];

            if (v != null)
            {
                try { pg = Convert.ToInt32(v);}
                catch { }
            }

            first_search(pg);

            v = Request.QueryString["c"];

            if (v != null)
            {
                v = "ctl00_ContentPlaceHolder1_" + v;

                Page.RegisterClientScriptBlock("CtrlFocus",
                  @"<script> 
                          function ScrollView()
                          {
                             var el = document.getElementById('" + v + @"')
                             if (el != null)
                             {        
                                el.scrollIntoView();
                                el.focus();
                             }
                          }
                          window.onload = ScrollView;
                          </script>");
            }
        }

        void upd_record_count()
        {
            if (ViewState[VS_SEARCH_PARAMS] != null)
            {
                SortedList sl = (SortedList)ViewState[VS_SEARCH_PARAMS];

                int record_count = 0;

                using (cdb_connection dbc = new cdb_connection())
                {
                    record_count = dbc.get_record_count("spools", sl);
                }

                ViewState[VS_RECORD_COUNT] = record_count;
            }
        }

        void init_search()
        {
            SortedList sl = new SortedList();

            if (txtSearch.Text.Trim().Length > 3)
            {
                string fld, val;
                fld = val = string.Empty;

                val = txtSearch.Text.Trim();

                if (dlSearchFlds.Text == "Spool")
                    fld = "spool";
                else if (dlSearchFlds.Text == "Barcode")
                    fld = "barcode";
                else if (dlSearchFlds.Text == "Project")
                {
                    fld = "spool";
                    val += "%";
                }

                sl.Add(fld, val);
            }

            if (dlStatus.Text.Trim().Length > 0)
            {
                string[] sa = dlStatus.Text.Split('-');

                if (sa.Length > 0)
                {
                    sl.Add("status", sa[0].Trim());
                }
            }

            if (chkSrchOnHold.Checked)
                sl.Add("on_hold", true);

            ViewState[VS_SEARCH_PARAMS] = sl;

            upd_record_count();
        }

        void search(int pg)
        {
            SortedList sl = (SortedList)ViewState[VS_SEARCH_PARAMS];

            using (spools spls = new spools())
            {
                spls.pg = pg;
                spls.recs_per_pg = REC_PER_PG;
                spls.order_by = "barcode";

                if (m_spools != null)
                    m_spools.Clear();
                
                if (sl.Count > 0)
                {
                    ArrayList asd = spls.get_spool_data_ex(sl);

                    m_spools = new SortedList();

                    foreach (spool_data sd in asd)
                    {
                        sd.tag = sd.status;
                        spool_data_ex sdx = new spool_data_ex();
                        sdx.sd = sd;
                        
                        m_spools.Add(sd.id, sdx);
                    }
                }
            }

            if (get_last_page() == 0)
                pg = 0;

            ViewState[VS_SPOOLS] = m_spools;
            ViewState[VS_CURRENT_PAGE] = pg;

            lblPage.Text = get_current_page().ToString() + " / " + get_last_page().ToString();
        }

        user_data get_user_data()
        {
            user_data ud = new user_data();

            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl0 = new SortedList();

            sl0.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);

                if(a.Count > 0)
                    ud = (user_data)a[0];
            }

            return ud;
        }

        bool is_admin(user_data ud)
        {
            return ud.role.ToUpper() == "ADMIN";
        }

        bool is_supervisor(user_data ud)
        {
            return ud.role.ToUpper() == "SUPERVISOR";
        }
        
        bool is_super_user()
        {
            return (System.Web.HttpContext.Current.User.Identity.Name.ToUpper() == "SB")
                    ||(System.Web.HttpContext.Current.User.Identity.Name.ToUpper() == "DU")
                    || (System.Web.HttpContext.Current.User.Identity.Name.ToUpper() == "PCF");
        }

        void display()
        {
            tblMain.Rows.Clear();

            if (m_spools != null)
            {
                user_data ud = get_user_data();
                bool badmin = is_admin(ud);
                bool bsupervisor = is_supervisor(ud);

                string[] hdr = new string[] { "WM", "Spool", "Revision", "Batch", "Created", "Fitter", "Start", "Finish", /*"Fit Time", "Weld Time",*/ "Installed By", "Status", "On Hold" };

                SortedList sl_barcode_order = new SortedList();

                foreach (DictionaryEntry e0 in m_spools)
                {
                    spool_data sd = ((spool_data_ex)e0.Value).sd;

                    sl_barcode_order.Add(sd.barcode+sd.id.ToString("0000000"), e0.Value);
                }

                foreach (DictionaryEntry e0 in sl_barcode_order)
                {
                    spool_data_ex sdx = (spool_data_ex)e0.Value;
                    spool_data sd = sdx.sd;

                    Panel pnlResults = new Panel();
                    Table tblResults = new Table();

                    tblResults.ID = "tblResults" + sd.id.ToString();

                    pnlResults.Attributes["uid"] = sd.id.ToString();
                    tblResults.Attributes["uid"] = sd.id.ToString();

                    pnlResults.Controls.Add(tblResults);

                    TableRow r;
                    TableCell c;

                    r = new TableRow();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.Controls.Add(pnlResults);
                    tblMain.Rows.Add(r);


                    r = new TableRow();

                    r.Attributes["uid"] = sd.id.ToString();

                    r.BackColor = System.Drawing.Color.FromName("SeaGreen");

                    foreach (string sh in hdr)
                    {
                        c = new TableCell();
                        if (sh == "Spool")
                            c.Width = 500;
                        c.Controls.Add(new LiteralControl(sh));
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                    r = new TableRow();
                    r.Attributes["uid"] = sd.id.ToString();

                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    CheckBox chk_include_spool_in_weld_map = new CheckBox();
                    chk_include_spool_in_weld_map.AutoPostBack = true;
                    chk_include_spool_in_weld_map.CheckedChanged += new EventHandler(chk_include_spool_in_weld_map_CheckedChanged);
                    chk_include_spool_in_weld_map.ID = "chk_include_spool_in_weld_map" + sd.id.ToString();
                    chk_include_spool_in_weld_map.Checked = sd.include_in_weld_map;
                    chk_include_spool_in_weld_map.Attributes["uid"] = sd.id.ToString();
                    chk_include_spool_in_weld_map.ToolTip = "Include spool in weld map";
                    
                    if (!badmin)
                        chk_include_spool_in_weld_map.Enabled = false;

                    c.Controls.Add(chk_include_spool_in_weld_map);
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sd.spool));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sd.revision));
                    r.Cells.Add(c);

                    c = new TableCell();

                    c.HorizontalAlign = HorizontalAlign.Center;
                    string batch_no = "-";

                    if (sd.schedule_fab_data != null)
                        if (sd.schedule_fab_data.batch_number > 0)
                            batch_no = sd.schedule_fab_data.batch_number.ToString();

                    c.Controls.Add(new LiteralControl(batch_no));
                    r.Cells.Add(c);

                    c = new TableCell();
                    string date_created = string.Empty;
                    if (sd.date_created > DateTime.MinValue)
                        date_created = sd.date_created.ToString("dd/MM/yyyy HH:mm:ss");
                    c.Controls.Add(new LiteralControl(date_created));
                    r.Cells.Add(c);

                    c = new TableCell();

                    string welder = string.Empty;
                    string fitter = string.Empty;
                    string site_fitter = string.Empty;

                    if (sd.welder_data != null)
                        welder = sd.welder_data.login_id;

                    if (sd.fitter_data != null)
                        fitter = sd.fitter_data.login_id;

                    if (sd.site_fitter_data != null)
                        site_fitter = sd.site_fitter_data.login_id;

                    string sfw = string.Empty;

                    /* hs. 20221114
                    if (sd.weld_job_data != null)
                    {
                        if (sd.weld_job_data.robot > 0)
                            sfw = "Robot";
                    }

                    if (welder.Trim().Length > 0)
                    {
                        if (sfw.Trim().Length > 0)
                            sfw += "/";

                        sfw += welder;
                    }
                    */

                    if (fitter.Trim().Length > 0)
                    {
                        if (sfw.Trim().Length > 0)
                            sfw += "/";

                        sfw += fitter;
                    }

                    c.Controls.Add(new LiteralControl(sfw));
                    r.Cells.Add(c);

                    string dts_fab = string.Empty;
                    string dte_fab = string.Empty;
                    
                    string dte_i = string.Empty;
                   
                    if (sd.weld_job_data != null)
                    {
                        ArrayList a_start = new ArrayList();
                        ArrayList a_finish = new ArrayList();

                        if (sd.weld_job_data.start > DateTime.MinValue)
                            a_start.Add(sd.weld_job_data.start);

                        if (sd.weld_job_data.start_fit > DateTime.MinValue)
                            a_start.Add(sd.weld_job_data.start_fit);


                        if (sd.weld_job_data.finish > DateTime.MinValue)
                            a_finish.Add(sd.weld_job_data.finish);
                            
                        if (sd.weld_job_data.finish_fit > DateTime.MinValue)
                            a_finish.Add(sd.weld_job_data.finish_fit);

                        a_start.Sort();
                        a_finish.Sort();

                        if (a_start.Count > 0)
                        {
                            DateTime dt = (DateTime)a_start[0];
                            dts_fab = dt.ToString("dd/MM/yyyy HH:mm:ss");
                        }

                        if (a_finish.Count > 0)
                        {
                            DateTime dt = (DateTime)a_finish[a_finish.Count - 1];
                            dte_fab = dt.ToString("dd/MM/yyyy HH:mm:ss");
                        }

                        if (sd.weld_job_data.installed_on > DateTime.MinValue)
                            dte_i = sd.weld_job_data.installed_on.ToString("dd/MM/yyyy");
                    }

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dts_fab));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dte_fab));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(site_fitter));
                    r.Cells.Add(c);

                    // status start
                    c = new TableCell();
                    string status = sd.status;

                    if (status == "SH" || status == "OS" || status == "IN" || status == "LD" || !badmin)
                    {
                        SortedList sl = new SortedList();
                        sl.Add("spool_id", sd.id);

                        ArrayList asdd = new ArrayList();

                        using (deliveries deliv = new deliveries())
                        {
                            asdd = deliv.get_spool_delivery_data(sl);
                        }

                        if (asdd.Count > 0)
                        {
                            spool_delivery_data sdd = (spool_delivery_data)asdd[0];

                            if (sdd.delivery_data != null)
                            {
                                if (status == "SH")
                                    status += " " + sdd.delivery_data.datetime_stamp.ToString("dd/MM/yyyy");
                                else if(status == "OS")
                                    status += " " + sdd.delivery_data.datetime_delivered.ToString("dd/MM/yyyy HH:mm:ss");
                                else if (status == "IN")
                                    status += " " + dte_i;
                            }
                        }
                        c.Controls.Add(new LiteralControl(status));
                    }
                    else
                    {
                        DropDownList dlstatus = new DropDownList();
                        dlstatus.AutoPostBack = true;
                        dlstatus.SelectedIndexChanged += new EventHandler(dlstatus_SelectedIndexChanged);
                        dlstatus.ID = "dlstatus" + sd.id.ToString();
                        dlstatus.Attributes["uid"] = sd.id.ToString();

                        dlstatus.Items.Add("SC");
                        dlstatus.Items.Add("RP");
                        dlstatus.Items.Add("IP");
                        dlstatus.Items.Add("WT");
                        dlstatus.Items.Add("QA");
                        dlstatus.Items.Add("PC");
                        dlstatus.Items.Add("RD");
                        dlstatus.Items.Add("LD");
                        dlstatus.Items.Add("SH");
                        dlstatus.Items.Add("OS");
                        dlstatus.Items.Add("IN");

                        dlstatus.Text = status;

                        if (!badmin)
                            dlstatus.Enabled = false;

                        c.Controls.Add(dlstatus);
                    }
                    
                    r.Cells.Add(c);
                    // status end

                    c = new TableCell();
                    CheckBox chk_onhold = new CheckBox();
                    chk_onhold.AutoPostBack = true;
                    chk_onhold.CheckedChanged += new EventHandler(chk_onhold_CheckedChanged);
                    chk_onhold.ID = "chk_onhold" + sd.id.ToString();
                    chk_onhold.Checked = sd.on_hold;
                    chk_onhold.Attributes["uid"] = sd.id.ToString();
                    if (!badmin)
                        chk_onhold.Enabled = false;

                    c.Controls.Add(chk_onhold);

                    r.Cells.Add(c);

                    if (sd.drawing_id > 0)
                    {
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        ImageButton btn_view_drawing = new ImageButton();
                        btn_view_drawing.ToolTip = "View Drawing";
                        btn_view_drawing.ImageUrl = "~/pdf.png";
                        btn_view_drawing.Click += new ImageClickEventHandler(btn_view_drawing_Click);
                        btn_view_drawing.ID = "btn_view_drawing" + sd.id.ToString();
                        btn_view_drawing.Attributes["uid"] = sd.drawing_id.ToString();

                        c.Controls.Add(btn_view_drawing);
                        r.Cells.Add(c);
                    }

                    if (badmin)
                    {
                        //if (status == "SC" || status == "RP")
                        //{
                            ImageButton btn_modify_spool = new ImageButton();
                            btn_modify_spool.Click += new ImageClickEventHandler(btn_modify_spool_Click);
                            btn_modify_spool.ImageUrl = "~/modify.png";
                            btn_modify_spool.ToolTip = "Modify spool";
                            btn_modify_spool.ID = "btn_modify_spool_" + sd.id.ToString();
                            btn_modify_spool.Attributes["uid"] = sd.id.ToString();

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            c.Controls.Add(btn_modify_spool);
                            r.Cells.Add(c);
                        //}
                            
                        if (!sd.picked || is_super_user())
                        {
                            ImageButton btn_remove_spool = new ImageButton();
                            btn_remove_spool.OnClientClick = "Confirm()";
                            btn_remove_spool.Click += new ImageClickEventHandler(btn_remove_spool_Click);
                            btn_remove_spool.ImageUrl = "~/delete.png";
                            btn_remove_spool.ToolTip = "Delete spool";
                            btn_remove_spool.ID = "btn_remove_spool_" + sd.id.ToString();
                            btn_remove_spool.Attributes["uid"] = sd.id.ToString();

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            c.Controls.Add(btn_remove_spool);
                            r.Cells.Add(c);
                        }
                    }

                    if (badmin || bsupervisor)
                    {
                        if (status == "QA" || status == "RD" || status == "WT")
                        {
                            c = new TableCell();
                            CheckBox chk_dispatch = new CheckBox();
                            chk_dispatch.Text = "Despatch";
                            chk_dispatch.Font.Name = "verdana";
                            chk_dispatch.AutoPostBack = true;
                            chk_dispatch.CheckedChanged += new EventHandler(chk_dispatch_CheckedChanged);
                            chk_dispatch.ID = "chk_dispatch" + sd.id.ToString();
                            chk_dispatch.Checked = sd.tag == "RD";
                            chk_dispatch.Attributes["uid"] = sd.id.ToString();
                            c.Controls.Add(chk_dispatch);
                            r.Cells.Add(c);
                        }
                    }

                    if (sd.picked)
                    {
                        c = new TableCell();
                        Image img_picked = new Image();
                        img_picked.ToolTip = "Picked";
                        img_picked.ImageUrl = "~/picked.png";
                        c.Controls.Add(img_picked);
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                    r = new TableRow();
                    r.Attributes["uid"] = sd.id.ToString();
                    r.Attributes[SHOW_PARTS_INSERTION_POINT] = sd.id.ToString();

                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    ImageButton btn_show_parts = new ImageButton();
                    btn_show_parts.Click += new ImageClickEventHandler(btn_show_parts_Click);
                    btn_show_parts.ID = "btn_show_parts_" + sd.id.ToString();
                    btn_show_parts.Attributes["uid"] = sd.id.ToString();
                    c.Controls.Add(btn_show_parts);
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl("Delivery Date (dd/mm/yyyy):"));
                    
                    c.Controls.Add(new LiteralControl("</br>"));

                    TextBox txt_delivery_date = new TextBox();
                    txt_delivery_date.ID = "txt_delivery_date_" + sd.id.ToString();
                    txt_delivery_date.Attributes["uid"] = sd.id.ToString();

                    if (sd.delivery_date > DateTime.MinValue)
                        txt_delivery_date.Text = sd.delivery_date.ToString("dd/MM/yyyy");
                    
                    c.Controls.Add(txt_delivery_date);
                    c.Controls.Add(new LiteralControl("  "));
                    
                    ImageButton btn_save_delivery_date = new ImageButton();
                    btn_save_delivery_date.ToolTip = "Save delivery date";
                    btn_save_delivery_date.ImageUrl = "~/disk.png";
                    btn_save_delivery_date.Click += new ImageClickEventHandler(btn_save_delivery_date_Click);
                    btn_save_delivery_date.ID = "btn_save_delivery_date_" + sd.id.ToString();
                    btn_save_delivery_date.Attributes["uid"] = sd.id.ToString();
                    c.Controls.Add(btn_save_delivery_date);

                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    if (sdx.bshowing_parts)
                    {
                        show_size_and_part_data(sd.id);
                    }

                    set_show_hide_button_properties(sdx.bshowing_parts, btn_show_parts);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r = new TableRow();
                    r.Cells.Add(c);
                    tblResults.Rows.Add(r);
                }
            }
        }

        void btn_view_drawing_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);
            string url = string.Empty;

            url = "view_drawing.aspx?id=" + uid;

            Response.Write("<script>");
            Response.Write("window.open('" + url + "','_blank','','false')");
            Response.Write("</script>");
        }

        Panel get_pnlResults(int uid)
        {
            Panel ret_pnl = null;

            foreach (TableRow r in tblMain.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.GetType() == typeof(Panel))
                        {
                            Panel pnl = ((Panel)cntrl);

                            if (pnl.Attributes["uid"] == uid.ToString())
                            {
                                return pnl;
                            }
                        }
                    }
                }
            }

            return ret_pnl;
        }

        Table get_tblResults(int id)
        {
            return get_tbl(id, "tblResults");
        }

        Table get_tblParts(int id)
        {
            return get_tbl(id, "tblParts");
        }

        Table get_tblSizes(int id)
        {
            return get_tbl(id, "tblSizes");
        }

        Table get_tbl(int id, string tbl_name)
        {
            Table ret_tbl = null;

            Panel pnl = get_pnlResults(id);

            foreach (Control cntrl in pnl.Controls)
            {
                if (cntrl.GetType() == typeof(Table))
                {
                    Table tbl = (Table)cntrl;

                    if (tbl.ID.StartsWith(tbl_name))
                    {
                        ret_tbl = tbl;
                        break;
                    }
                }
            }

            return ret_tbl;
        }

        void btn_show_parts_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);
                
            int id = Convert.ToInt32(uid);

            if (m_spools.ContainsKey(id))
            {
                spool_data_ex sdx = ((spool_data_ex)m_spools[id]);

                if (!sdx.bshowing_parts)
                {
                    show_size_and_part_data(id);
                }
                else
                {
                    hide_size_and_part_data(id);
                }

                sdx.bshowing_parts = !sdx.bshowing_parts;

                set_show_hide_button_properties(sdx.bshowing_parts, b);

                ViewState[VS_SPOOLS] = m_spools;
            }
        }

        void set_show_hide_button_properties(bool bshowing_parts, ImageButton b)
        {
            if (bshowing_parts)
            {
                b.ToolTip = "Hide parts";
                b.ImageUrl = "~/up.png";
            }
            else
            {
                b.ToolTip = "Show parts";
                b.ImageUrl = "~/down.png";
            }
        }

        void hide_size_and_part_data(int id)
        {
            hide_parts(id);
        }

        void hide_parts(int id)
        {
            Panel pnl = get_pnlResults(id);

            foreach (Control cntrl in pnl.Controls)
            {
                if (cntrl.GetType() == typeof(Table))
                {
                    Table tbl = (Table)cntrl;

                    if (tbl.ID.StartsWith("tblSizes"))
                    {
                        tbl.Rows.Clear();
                        pnl.Controls.Remove(cntrl);
                        break;
                    }
                }
            }

            foreach (Control cntrl in pnl.Controls)
            {
                if (cntrl.GetType() == typeof(Table))
                {
                    Table tbl = (Table)cntrl;

                    if (tbl.ID.StartsWith("tblHdrParts"))
                    {
                        tbl.Rows.Clear();
                        pnl.Controls.Remove(cntrl);
                        break;
                    }
                }
            }

            foreach (Control cntrl in pnl.Controls)
            {
                if (cntrl.GetType() == typeof(Table))
                {
                    Table tbl = (Table)cntrl;

                    if (tbl.ID.StartsWith("tblParts"))
                    {
                        tbl.Rows.Clear();
                        pnl.Controls.Remove(cntrl);
                        break;
                    }
                }
            }
        }

        void show_size_and_part_data(int id)
        {
            show_size_data(id);
            show_part_data(id);
        }

        void show_size_data(int id)
        {
            if (m_spools.ContainsKey(id))
            {
                user_data ud = get_user_data();
                bool badmin = is_admin(ud);
                bool bsupervisor = is_supervisor(ud);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;

                Panel pnlResults = get_pnlResults(id);
                Table tblSizes = new Table();
                tblSizes.ID = "tblSizes" + id;

                pnlResults.Controls.Add(tblSizes);

                TableRow r;
                TableCell c;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("Silver");

                TextBox txt_bx = null;

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Pipe Size:"));

                txt_bx = new TextBox();
                txt_bx.ID = "txtPipeSize" + sd.id;
                txt_bx.MaxLength = 20;
                txt_bx.Text = sd.pipe_size;
                c.Controls.Add(txt_bx);
                
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Cut Size:"));

                txt_bx = new TextBox();
                txt_bx.ID = "txtCutSize1" + sd.id;
                txt_bx.MaxLength = 20;
                txt_bx.Text = sd.cut_size1;
                c.Controls.Add(txt_bx);

                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Cut Size:"));

                txt_bx = new TextBox();
                txt_bx.ID = "txtCutSize2" + sd.id;
                txt_bx.MaxLength = 20;
                txt_bx.Text = sd.cut_size2;
                c.Controls.Add(txt_bx);

                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Cut Size:"));

                txt_bx = new TextBox();
                txt_bx.ID = "txtCutSize3" + sd.id;
                txt_bx.MaxLength = 20;
                txt_bx.Text = sd.cut_size3;
                c.Controls.Add(txt_bx);

                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Cut Size:"));

                txt_bx = new TextBox();
                txt_bx.ID = "txtCutSize4" + sd.id;
                txt_bx.MaxLength = 20;
                txt_bx.Text = sd.cut_size4;
                c.Controls.Add(txt_bx);

                r.Cells.Add(c);

                c = new TableCell();
                c.HorizontalAlign = HorizontalAlign.Center;
                ImageButton btn_save_size_data = new ImageButton();
                btn_save_size_data.ToolTip = "Save changes to sizes";
                btn_save_size_data.ImageUrl = "~/disk.png";
                btn_save_size_data.Click += new ImageClickEventHandler(btn_save_size_data_Click);
                btn_save_size_data.ID = "btn_save_part_" + sd.id.ToString();
                btn_save_size_data.Attributes["spool_id"] = sd.id.ToString();
                c.Controls.Add(btn_save_size_data);

                r.Cells.Add(c);

                tblSizes.Rows.Add(r);
            }
        }

        void btn_save_size_data_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string spool_id = (b.Attributes["spool_id"]);

            Table tblSizes = get_tblSizes(Convert.ToInt32(spool_id));

            string pipe_size, cut_size1, cut_size2, cut_size3, cut_size4;
            pipe_size = cut_size1 = cut_size2 = cut_size3 = cut_size4 = string.Empty;

            foreach (TableCell c in tblSizes.Rows[0].Cells)
            {
                foreach (Control cntrl in c.Controls)
                {
                    if (cntrl.ID != null)
                    {
                        if (cntrl.ID.StartsWith("txtPipeSize"))
                            pipe_size = ((TextBox)cntrl).Text.Trim();

                        if (cntrl.ID.StartsWith("txtCutSize1"))
                            cut_size1 = ((TextBox)cntrl).Text.Trim();

                        if (cntrl.ID.StartsWith("txtCutSize2"))
                            cut_size2 = ((TextBox)cntrl).Text.Trim();

                        if (cntrl.ID.StartsWith("txtCutSize3"))
                            cut_size3 = ((TextBox)cntrl).Text.Trim();

                        if (cntrl.ID.StartsWith("txtCutSize4"))
                            cut_size4 = ((TextBox)cntrl).Text.Trim();
                    }
                }
            }

            SortedList sl = new SortedList();

            int id = Convert.ToInt32(spool_id);

            sl.Add("id", id);
            sl.Add("pipe_size", pipe_size);
            sl.Add("cut_size1", cut_size1);
            sl.Add("cut_size2", cut_size2);
            sl.Add("cut_size3", cut_size3);
            sl.Add("cut_size4", cut_size4);

            using (spools spls = new spools())
            {
                spls.save_spool_details(sl, "Sizes change", System.Web.HttpContext.Current.User.Identity.Name);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;

                sd.pipe_size = pipe_size;
                sd.cut_size1 = cut_size1;
                sd.cut_size2 = cut_size2;
                sd.cut_size3 = cut_size3;
                sd.cut_size4 = cut_size4;
            }
        }

        void show_part_data(int id)
        {
            if (m_spools.ContainsKey(id))
            {
                user_data ud = get_user_data();
                bool badmin = is_admin(ud);
                bool bsupervisor = is_supervisor(ud);

                spool_data sd = ((spool_data_ex) m_spools[id]).sd;

                using (porders po = new porders())
                {
                    if (sd.spool_part_data != null)
                    {
                        if (sd.spool_part_data.Count > 0)
                        {
                            /*
                            SortedList sl_parts_data = new SortedList();

                            foreach (spool_part_data spd in sd.spool_part_data)
                            {
                                if (spd.part_data != null)
                                    sl_parts_data.Add(spd.seq.ToString("000000") + "_" + spd.id.ToString(new string('0', 64)), spd);
                            }
                            */

                            TableRow r;
                            TableCell c;

                            Panel pnlResults = get_pnlResults(id);

                            
                            //hs. 20221114
                            Table tblPartsHdr = new Table();
                            tblPartsHdr.ID = "tblHdrParts" + id;
                            pnlResults.Controls.Add(tblPartsHdr);

                            r = new TableRow();
                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(string.Empty));
                            r.Cells.Add(c);
                            tblPartsHdr.Rows.Add(r);

                            r = new TableRow();
                            r.Attributes["uid"] = sd.id.ToString();

                            r.BackColor = System.Drawing.Color.FromName("LightGray");

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Parts"));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            ImageButton btn_save_parts = new ImageButton();
                            btn_save_parts.ToolTip = "Save changes to all parts for this spool " + sd.barcode;
                            btn_save_parts.ImageUrl = "~/disk.png";
                            btn_save_parts.Click += btn_save_parts_Click;
                            btn_save_parts.ID = "btn_save_parts_" + sd.id.ToString();
                            btn_save_parts.Attributes["uid"] = sd.id.ToString();
                            btn_save_parts.Attributes["spool_id"] = sd.id.ToString();
                            if (!badmin)
                                btn_save_parts.Visible = false;

                            c.Controls.Add(btn_save_parts);
                            r.Cells.Add(c);

                            tblPartsHdr.Rows.Add(r);
                            
                            Table tblParts = new Table();
                            tblParts.ID = "tblParts" + id;

                            pnlResults.Controls.Add(tblParts);

                            //foreach (DictionaryEntry e1 in sl_parts_data)
                            foreach(spool_part_data spd in sd.spool_part_data)
                            {
                                //spool_part_data spd = (spool_part_data)e1.Value;

                                r = new TableRow();
                                r.Attributes["uid"] = sd.id.ToString();
                                r.Attributes["PART_DATA_ROW"] = sd.id.ToString();
                                r.Attributes["spool_part_id"] = spd.id.ToString();

                                r.BackColor = System.Drawing.Color.FromName("LightGray");

                                c = new TableCell();
                                CheckBox chk_include_spool_part_in_weld_map = new CheckBox();
                                chk_include_spool_part_in_weld_map.AutoPostBack = true;
                                chk_include_spool_part_in_weld_map.CheckedChanged += new EventHandler(chk_include_spool_part_in_weld_map_CheckedChanged);
                                chk_include_spool_part_in_weld_map.ID = "chk_include_spool_part_in_weld_map" + spd.id.ToString();
                                chk_include_spool_part_in_weld_map.Checked = spd.include_in_weld_map;
                                chk_include_spool_part_in_weld_map.Attributes["uid"] = spd.id.ToString();
                                chk_include_spool_part_in_weld_map.Attributes["spool_id"] = sd.id.ToString();
                                chk_include_spool_part_in_weld_map.ToolTip = "Include spool part in weld map";

                                if (!badmin)
                                    chk_include_spool_part_in_weld_map.Enabled = false;

                                c.Controls.Add(chk_include_spool_part_in_weld_map);
                                r.Cells.Add(c);

                                string part_no, part_desc;
                                part_no = part_desc = string.Empty;

                                if (spd.part_data != null)
                                {
                                    part_no = spd.part_data.part_number;
                                    part_desc = spd.part_data.description;
                                }

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl((tblParts.Rows.Count + 1).ToString()));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Width = 500;
                                c.Controls.Add(new LiteralControl(part_desc));
                                r.Cells.Add(c);

                                /*
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(part_no));
                                c.Width = 150;
                                r.Cells.Add(c);
                                */

                                c = new TableCell();

                                if (part_desc.ToUpper().Contains("PIPE"))
                                    c.Controls.Add(new LiteralControl("Len:"));
                                else
                                    c.Controls.Add(new LiteralControl("Qty:"));

                                TextBox qtb = null;

                                if (part_desc.ToUpper().Contains("PIPE"))
                                {
                                    qtb = create_decimal_textbox("qty_" + spd.id.ToString());
                                    qtb.Text = spd.qty.ToString("0.00");
                                }
                                else
                                {
                                    qtb = create_numeric_textbox("qty_" + spd.id.ToString());
                                    qtb.Text = spd.qty.ToString("0");
                                    qtb.Enabled = false;
                                }

                                qtb.MaxLength = 7;

                                if (!badmin)
                                    qtb.ReadOnly = true;
                                c.Controls.Add(qtb);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("FW:"));
                                TextBox tb = create_numeric_textbox("fw_" + spd.id.ToString());
                                tb.MaxLength = 3;
                                tb.Text = spd.fw.ToString();
                                if (!badmin)
                                    tb.ReadOnly = true;
                                c.Controls.Add(tb);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("BW:"));
                                tb = create_numeric_textbox("bw_" + spd.id.ToString());
                                tb.MaxLength = 3;
                                tb.Text = spd.bw.ToString();
                                if (!badmin)
                                    tb.ReadOnly = true;
                                c.Controls.Add(tb);

                                r.Cells.Add(c);

                                /*
                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Center;
                                ImageButton btn_save_part = new ImageButton();
                                btn_save_part.ToolTip = "Save changes to part";
                                btn_save_part.ImageUrl = "~/disk.png";
                                btn_save_part.Click += new ImageClickEventHandler(btn_save_part_Click);
                                btn_save_part.ID = "btn_save_part_" + spd.id.ToString();
                                btn_save_part.Attributes["uid"] = spd.id.ToString();
                                btn_save_part.Attributes["spool_id"] = sd.id.ToString();
                                if (!badmin)
                                    btn_save_part.Visible = false;

                                c.Controls.Add(btn_save_part);
                                r.Cells.Add(c);
                                */

                                if (spd.porder > 0)
                                {
                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Center;

                                    HyperLink h = new HyperLink();
                                    h.Text = "PO";
                                    h.NavigateUrl = "porders.aspx?id=" + spd.porder.ToString();
                                    //h.Target = "_blank";

                                    c.Controls.Add(h);

                                    r.Cells.Add(c);
                                }

                                // hs. 20221114
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Welder:"));
                                DropDownList dl_welder = new DropDownList();
                                dl_welder.ID = "dl_welder" + spd.id.ToString();
                                dl_welder.Items.Add(string.Empty);
                                dl_welder.Attributes["uid"] = spd.id.ToString();

                                foreach (DictionaryEntry e0 in m_welders)
                                    dl_welder.Items.Add(e0.Key.ToString());

                                if (!m_welders.ContainsKey(spd.welder))
                                    dl_welder.Items.Add(spd.welder);

                                if (spd.welder.Trim().Length > 0)
                                    dl_welder.Text = spd.welder;
                                else
                                {
                                    string welder = string.Empty;

                                    if (sd.welder_data != null)
                                        welder = sd.welder_data.login_id;

                                    string sfw = string.Empty;
                                    
                                    if (sd.weld_job_data != null)
                                    {
                                        if (sd.weld_job_data.robot > 0)
                                            sfw = "Robot";
                                    }

                                    if (welder.Trim().Length > 0)
                                    {
                                        if (sfw.Trim().Length > 0)
                                            sfw += "/";

                                        sfw += welder;
                                    }

                                    if (sfw.Trim().Length > 0)
                                    {
                                        dl_welder.Items.Add(sfw);
                                        dl_welder.Text = sfw;
                                    }
                                }

                                c.Controls.Add(dl_welder);
                                r.Cells.Add(c);

                                tblParts.Rows.Add(r);
                            }
                        }
                    }
                }
            }
        }

        private void btn_save_parts_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            ImageButton b = (ImageButton)sender;
            
            int id, fw, bw;
            decimal qty = 0;
            int nfound = 0;
            string welder = string.Empty;

            string spool_id = (b.Attributes["spool_id"]);

            Table tblParts = get_tblParts(Convert.ToInt32(spool_id));

            foreach (TableRow r in tblParts.Rows)
            {
                id = fw = bw = 0;
                qty = 0;
                nfound = 0;
                welder = string.Empty;

                try { id = Convert.ToInt32((r.Attributes["spool_part_id"])); }
                catch
                {
                    continue;
                }

                if (id == 0)
                    return;

                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID != null)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.StartsWith("qty_"))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { qty = Convert.ToDecimal(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }

                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.StartsWith("fw_"))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { fw = Convert.ToInt32(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }

                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.StartsWith("bw_"))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { bw = Convert.ToInt32(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }

                            if (cntrl.GetType() == typeof(DropDownList))
                            {
                                if (cntrl.ID.StartsWith("dl_welder"))
                                {
                                    DropDownList dl_welder = (DropDownList)cntrl;
                                    welder = dl_welder.Text;
                                    nfound++;
                                }
                            }
                        }
                    }
                }

                if (nfound > 3)
                {
                    SortedList slp = new SortedList();

                    using (spool_parts sp = new spool_parts())
                    {
                        slp.Add("id", id);
                        slp.Add("qty", qty);
                        slp.Add("fw", fw);
                        slp.Add("bw", bw);
                        slp.Add("welder", welder);

                        sp.save_spool_parts_data(slp);

                        int sd_id = Convert.ToInt32(r.Attributes["uid"]);

                        if (m_spools.ContainsKey(sd_id))
                        {
                            spool_data sd = ((spool_data_ex)m_spools[sd_id]).sd;

                            foreach (spool_part_data spd in sd.spool_part_data)
                            {
                                if (spd.part_data != null)
                                {
                                    if (spd.id == id)
                                    {
                                        spd.qty = qty;
                                        spd.fw = fw;
                                        spd.bw = bw;
                                        spd.welder = welder;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
        }

        private void dl_welder_TextChanged(object sender, EventArgs e)
        {
            int id = 0;

            DropDownList dl = (DropDownList)sender;
            string uid = (dl.Attributes["uid"]);

            try { id = Convert.ToInt32(uid); }
            catch
            {
                lblMsg.Text = "Failed to identify part id";
                return;
            }

            SortedList slp = new SortedList();

            using (spool_parts sp = new spool_parts())
            {
                slp.Add("id", id);
                slp.Add("welder", dl.Text);
                
                sp.save_spool_parts_data(slp);
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

            foreach (user_data ud in a)
            {
                if (ud.login_id.Trim().Length > 0)
                    if (!m_welders.ContainsKey(ud.login_id.Trim()))
                        m_welders.Add(ud.login_id.Trim(), ud);
            }

            

            ViewState["welders"] = m_welders;
        }

        void btn_save_delivery_date_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);
            int id = Convert.ToInt32(uid);

            Table tblResults = get_tblResults(id);

            foreach (TableRow r in tblResults.Rows)
            {
                string ruid = string.Empty;

                try { ruid = r.Attributes["uid"]; }
                catch { }
                if (ruid == uid)
                {
                    foreach (TableCell c in r.Cells)
                    {
                        foreach (Control cntrl in c.Controls)
                        {
                            if (cntrl.ID != null)
                            {
                                if (cntrl.GetType() == typeof(TextBox))
                                {
                                    if (cntrl.ID.EndsWith("txt_delivery_date_" + uid))
                                    {
                                        TextBox tb = (TextBox)cntrl;
                                        DateTime dt = DateTime.MaxValue;

                                        try { dt = Convert.ToDateTime(tb.Text); }
                                        catch { tb.Text = "** INVALID DATE **"; return; }

                                        SortedList sl = new SortedList();
                                        sl.Add("delivery_date", dt);

                                        sl.Add("id", Convert.ToInt32(uid));

                                        using (spools sp = new spools())
                                        {
                                            using (users u = new users())
                                            {
                                                user_data ud = u.get_user_data(System.Web.HttpContext.Current.User.Identity.Name);
                                                sp.save_spool_details(sl, string.Empty, ud.login_id);
                                                return;
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

        void btn_refresh_spool_Click(object sender, ImageClickEventArgs e)
        {
            btnSearch_Click(sender, null);
        }

        void btn_modify_spool_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);

            string url = "create_spool.aspx?uid=" + uid;
            url += "&s=" + txtSearch.Text.Trim();
            url += "&u=" + dlStatus.SelectedIndex.ToString();
            url += "&f=" + dlSearchFlds.SelectedIndex.ToString();
            url += "&h=" + (chkSrchOnHold.Checked?"1":"0");
            url += "&p=1";
            url += "&c=" + b.ID;
            url += "&pg=" + get_current_page().ToString();
            
            Response.Redirect(url);
            
            /*
            Response.Write("<script>");
            Response.Write("window.open('" + url + "','_blank','','false')");
            Response.Write("</script>");
            */
        }

        void btn_save_part_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            ImageButton b = (ImageButton)sender;
            int id,  fw, bw;
            id = fw = bw = 0;
            decimal qty = 0;
            
            int nfound = 0;

            string spool_id = (b.Attributes["spool_id"]);
            string uid = (b.Attributes["uid"]);

            try { id = Convert.ToInt32(uid); }
            catch 
            {
                lblMsg.Text = "Failed to identify part id";
                return;
            }

            Table tblParts = get_tblParts(Convert.ToInt32(spool_id));

            foreach (TableRow r in tblParts.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID != null)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.EndsWith("qty_" + uid))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { qty = Convert.ToDecimal(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }

                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.EndsWith("fw_" + uid))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { fw = Convert.ToInt32(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }

                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                if (cntrl.ID.EndsWith("bw_" + uid))
                                {
                                    TextBox tb = (TextBox)cntrl;
                                    try { bw = Convert.ToInt32(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }
                        }

                        if (nfound > 2)
                        {
                            SortedList slp = new SortedList();

                            using (spool_parts sp = new spool_parts())
                            {
                                slp.Add("id", id);
                                slp.Add("qty", qty);
                                slp.Add("fw", fw);
                                slp.Add("bw", bw);

                                sp.save_spool_parts_data(slp);

                                int sd_id = Convert.ToInt32(r.Attributes["uid"]);

                                if (m_spools.ContainsKey(sd_id))
                                {
                                    spool_data sd = ((spool_data_ex)m_spools[sd_id]).sd;

                                    foreach (spool_part_data spd in sd.spool_part_data)
                                    {
                                        if (spd.part_data != null)
                                        {
                                            if (spd.id == id)
                                            {
                                                spd.qty = qty;
                                                spd.fw = fw;
                                                spd.bw = bw;
                                                break;
                                            }
                                        }
                                    }
                                }

                                return;
                            }
                        }
                    }
                }
            }
        }

        void dlstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dl = (DropDownList)sender;

            string uid = (dl.Attributes["uid"]);

            SortedList sl = new SortedList();

            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            sl.Add("id", id);
            sl.Add("status", dl.Text);

            using (spools spls = new spools())
            {
                if (dl.Text == "RD")
                {
                    sl.Add("drawing_id", 0);

                    SortedList sl_drw = new SortedList();
                    sl_drw.Add("spool_id", id);

                    spls.delete_record("drawings", sl_drw);
                }

                spls.save_spool_details(sl, "Status change", System.Web.HttpContext.Current.User.Identity.Name);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;

                sd.status = dl.Text;
                sd.drawing_id = 0;
            }
        }

        void btn_remove_spool_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                ImageButton b = (ImageButton)sender;

                string uid = (b.Attributes["uid"]);

                ArrayList del_i = new ArrayList();
                int i = 0;

                Table tblResults = get_tblResults(Convert.ToInt32(uid));

                foreach (TableRow r in tblResults.Rows)
                {
                    if (r.Attributes["uid"] == uid)
                        del_i.Add(i);

                    i++;
                }

                del_i.Reverse();

                foreach (int n in del_i)
                {
                    tblResults.Rows.RemoveAt(n);
                }

                int sd_id = 0;

                try { sd_id = Convert.ToInt32(uid); }
                catch { }

                if (m_spools.ContainsKey(sd_id))
                    m_spools.Remove(sd_id);

                ViewState[VS_SPOOLS] = m_spools;

                using (cdb_connection db = new cdb_connection())
                {
                    SortedList sl = new SortedList();
                    sl.Add("id", sd_id);

                    db.delete_record("spools", sl);

                    sl.Clear();

                    sl.Add("spool_id", sd_id);

                    db.delete_record("spool_parts", sl);

                    db.delete_record("new_spools", sl);

                    db.delete_record("schedule_delivery", sl);
                    db.delete_record("schedule_fab", sl);

                    db.delete_record("drawings", sl);

                    db.delete_record("weld_jobs", sl);
                }

                upd_record_count();

                if (get_current_page() > get_last_page())
                    ViewState[VS_CURRENT_PAGE] = get_last_page();

                search(get_current_page());
                display();
            }
        }

        void chk_dispatch_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["uid"]);

            SortedList sl = new SortedList();

            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            sl.Add("id", id);
            sl.Add("status", chk.Checked?"RD":"QA");

            using (spools spls = new spools())
            {
                spls.save_spool_details(sl, "status", System.Web.HttpContext.Current.User.Identity.Name);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;
                sd.tag = chk.Checked ? "RD" : "QA";
            }
        }

        void chk_include_spool_in_weld_map_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["uid"]);
            
            SortedList sl = new SortedList();

            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            sl.Add("id", id);
            sl.Add("include_in_weld_map", chk.Checked);

            using (spools spls = new spools())
            {
                spls.save_spool_details(sl, "include_in_weld_map", System.Web.HttpContext.Current.User.Identity.Name);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;

                sd.include_in_weld_map = chk.Checked;
            }

            foreach (DictionaryEntry e0 in m_spools)
            {
                 spool_data sd = ((spool_data_ex)e0.Value).sd;

                 if (id == sd.id)
                 {
                     sd.include_in_weld_map = chk.Checked;
                     break;
                 }
            }
        }

        void chk_include_spool_part_in_weld_map_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["uid"]);
            string spool_id = (chk.Attributes["spool_id"]);

            int id = 0;
            int spoolid = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            try { spoolid = Convert.ToInt32(spool_id); }
            catch { }

            SortedList sl = new SortedList();

            sl.Add("id", id);
            sl.Add("include_in_weld_map", chk.Checked);

            using (spool_parts sp = new spool_parts())
            {
                sp.save_spool_parts_data(sl);
            }

            spool_data sd = ((spool_data_ex)m_spools[spoolid]).sd;

            foreach (spool_part_data spd in sd.spool_part_data)
            {
                if(spd.id == id)
                {
                    spd.include_in_weld_map = chk.Checked;
                    break;
                }
            }
        }

        void chk_onhold_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["uid"]);

            SortedList sl = new SortedList();

            int id = 0;

            try{id = Convert.ToInt32(uid);}catch{}

            sl.Add("id", id);
            sl.Add("on_hold", chk.Checked);

            using (spools spls = new spools())
            {
                spls.save_spool_details(sl, "On Hold", System.Web.HttpContext.Current.User.Identity.Name);

                spool_data sd = ((spool_data_ex)m_spools[id]).sd;

                sd.on_hold = chk.Checked;
            }

            foreach (DictionaryEntry e0 in m_spools)
            {
                spool_data sd = ((spool_data_ex)e0.Value).sd;

                if (id == sd.id)
                {
                    sd.on_hold = chk.Checked;
                    break;
                }
            }
        }

        void first_search(int pg)
        {
            init_search();
            search(pg);
            display();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            first_search(1);
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
        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (m_spools != null)
            {
                if (m_spools.Count > 0)
                {
                    string CRLF = "\r\n";
                    string DC = "\"";
                    string CM = ",";


                    string[] hdr = new string[12] { "spool", "revision", "welder", "start", "finish", "status", "on_hold", "part_number", "description", "qty", "fw", "bw" };

                    Response.ContentType = "text/csv";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=spool_data_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");

                    string shdr = string.Empty;

                    foreach (string s in hdr)
                    {
                        if (shdr.Length > 0)
                            shdr += CM;

                        shdr += DC + s + DC;
                    }

                    Response.Write(shdr);
                    Response.Write(CRLF);

                    string line_spool = string.Empty;

                    SortedList sl_barcode_order = new SortedList();

                    foreach (DictionaryEntry e0 in m_spools)
                    {
                        spool_data sd = ((spool_data_ex)e0.Value).sd;

                        sl_barcode_order.Add(sd.barcode + sd.id.ToString("0000000"), sd);
                    }

                    foreach (DictionaryEntry e0 in sl_barcode_order)
                    {
                        spool_data sd = (spool_data)e0.Value;

                        line_spool = DC + sd.spool + DC;
                        line_spool += CM;

                        line_spool += DC + sd.revision + DC;
                        line_spool += CM;

                        string welder = string.Empty;

                        if (sd.welder_data != null)
                            welder = sd.welder_data.login_id;

                        line_spool += DC + welder + DC;
                        line_spool += CM;

                        string dts = string.Empty;
                        string dte = string.Empty;

                        if (sd.weld_job_data != null)
                        {
                            if (sd.weld_job_data.start > DateTime.MinValue)
                                dts = sd.weld_job_data.start.ToString("yyyy-MM-dd HH:mm:ss");

                            if (sd.weld_job_data.finish > DateTime.MinValue)
                                dte = sd.weld_job_data.finish.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        line_spool += dts;
                        line_spool += CM;

                        line_spool += dte;
                        line_spool += CM;

                        line_spool += DC + sd.status + DC;
                        line_spool += CM;

                        if (sd.on_hold)
                            line_spool += DC + "Y" + DC;
                        else
                            line_spool += DC + "N" + DC;

                        if (sd.spool_part_data != null)
                        {
                            if (sd.spool_part_data.Count > 0)
                            {
                                string line_part = string.Empty;
                                foreach (spool_part_data spd in sd.spool_part_data)
                                {
                                    line_part = DC + spd.part_data.part_number + DC;
                                    line_part += CM;

                                    line_part += DC + spd.part_data.description + DC;
                                    line_part += CM;


                                    line_part += spd.qty.ToString("0.00");
                                    line_part += CM;
                                    line_part += spd.fw.ToString();
                                    line_part += CM;
                                    line_part += spd.bw.ToString();

                                    Response.Write(line_spool + CM + line_part);
                                    Response.Write(CRLF);
                                }
                            }
                            else
                            {
                                Response.Write(line_spool);
                                Response.Write(CRLF);
                            }
                        }
                        else
                        {
                            Response.Write(line_spool);
                            Response.Write(CRLF);
                        }
                    }

                    Response.End();
                }
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

            if (ViewState[VS_CURRENT_PAGE] != null)
                current_page = (int)ViewState[VS_CURRENT_PAGE];

            return current_page;
        }

        int get_last_page()
        {
            int last_page = 0;

            if (ViewState[VS_RECORD_COUNT] != null)
            {
                int record_count = (int)ViewState[VS_RECORD_COUNT];

                last_page = record_count / REC_PER_PG;

                if (record_count % REC_PER_PG > 0)
                    last_page++;
            }
            return last_page;
        }
    }

    [Serializable]
    class spool_data_ex
    {
        public spool_data sd;
        public bool bshowing_parts = false;
    }
}
