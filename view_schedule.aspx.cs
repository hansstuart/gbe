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
using System.IO;

namespace gbe
{
    public partial class view_schedule : System.Web.UI.Page
    {
        const string VS_SCHEDULE_RECS = "vs_schedule_recs";
        const string VS_SCHEDULE_RECS_DT_TIME = "vs_schedule_recs_dt_time";
        const string VS_CURR_VIEW = "vs_curr_view";
        const string ATT_BC = "att_bc";
        const string LIST_VIEW = "List View";
        const string CALENDER_VIEW = "Calender View";
        const string K_DATE = "K_DATE";
        const string DT_FMT = "yyyyMMdd";
        const string DT_FMT2 = "yyyyMMdd_HH:mm";
        const string FAB_TBL = "schedule_fab";
        const string DELIV_TBL = "schedule_delivery";
        const string VS_TBL = "vs_tbl";
        const string CHK0 = "chk0";
        const string VS_EDIT_VIEW_RECS = "vs_edit_view_recs";
        const string UID = "uid";
        const string BARCODE = "barcode";
        const string DATE = "date";
        const string BATCH = "batch";
        const string TXT_TIME =  "txt_time_";
        const string TXT_DATE = "txt_date_";
        const string DL_VEH = "dl_veh_";
        const string CHK = "chk_";
        const string VS_DATE_FROM = "vs_date_from";
        const string INVALID_DATE = "**INVALID DATE**";
        const string INVALID_TIME = "**INVALID TIME**";
        const string VS_WELDERS_AND_FITTERS = "vs_welders_and_fitters";
        const string VS_CHANGE_CONFIRM_DATA = "vs_change_confirm_data";
        const string VS_HOLDING = "vs_holding";
        const string VS_NOTES_EDIT_DATA = "vs_notes_edit_data";
        const string VS_NOTES = "vs_notes";
        const string VS_NOTES_BY_ID = "vs_notes_by_id";
        const string VS_0 = "vs_0";
        const string EXTRAS = "Extras";
        
        string SEQ_FMT = new string('0', 16);
        string BATCH_FMT = new string('0', 8);
        
        SortedList m_sl_schedule_rec = null;
        SortedList m_sl_dt_time_schedule_rec = null;
        SortedList m_sl_welders_and_fitters = null;
        string m_curr_view = string.Empty;
        string m_edit_view_recs = string.Empty;
        SortedList m_sl_codes = null;
        SortedList m_sl_notes = null;
        SortedList m_sl_notes_by_id = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            if(System.Web.HttpContext.Current.User.Identity.Name.ToUpper() == "PCF")
                btn_hs_upd.Visible = true;
            */

            if (m_sl_codes == null)
                m_sl_codes = new SortedList();
            else
                m_sl_codes.Clear();

            m_sl_codes.Add("00", "OK");
            m_sl_codes.Add("E1", "Material shortage");
            m_sl_codes.Add("E2", "Material incorrect");
            m_sl_codes.Add("E3", "Spool drawing missing measurements");
            m_sl_codes.Add("E4", "Spool drawing unreadable");

            if (!IsPostBack)
            {
                Label3.Visible = Label4.Visible = Label5.Visible = Label6.Visible = txtDeliveryDate.Visible = txtDeliveryTime.Visible = dlDeliveryVehicle.Visible = false;

                chkRed.Checked = chkOrange.Checked = chkBlue.Checked = chkGreen.Checked = true;

                chkSpools.Checked = true;
                chkModules.Checked = false;

                dlMaterial.Items.Add("Carbon");
                dlMaterial.Items.Add("Stainless");
                dlMaterial.Items.Add("Screwed");
                dlMaterial.Items.Add("HS");

                DateTime dt_from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                DateTime dt_to = DateTime.MaxValue;

                dt_from = dt_from.AddMonths(-1);
                
                ViewState[VS_DATE_FROM] = dt_from;

                string tbl = string.Empty;
                string holding_area = string.Empty;
                bool bholding_area = is_holding_area();

                tbl = Request.QueryString["t"];
                
                if (tbl != null)
                {
                    if (tbl == FAB_TBL)
                    {
                        if (bholding_area)
                            lblTitle.Text = "Fabrication Holding Area";
                        else
                            lblTitle.Text = "Fabrication Schedule";
                    }
                    else if (tbl == DELIV_TBL)
                    {
                        lblTitle.Text = "Delivery Schedule";

                        lblInsertAtSeq.Visible = txtSeq.Visible = false;
                        chkQuarantineSpools.Text = imgReSeq.ToolTip = "Quarantine";
                        chkQuarantineSpools.Enabled = false;
                        chkQuarantineSpools.Checked = true;
                    }
                    else
                        Server.Transfer("default.aspx", true);

                    ViewState[VS_TBL] = tbl;
                    ViewState[VS_HOLDING] = bholding_area;
                }
                else
                    Server.Transfer("default.aspx", true);

                ena_save_button();

                if (bholding_area)
                {
                    dt_from = schedule_fab_data.DT_HOLDING_RECS_DATE;
                    dt_to = dt_from.AddHours(1);
                }

                get_welders_and_fitters();

                get_schedule_recs(dt_from, dt_to, tbl);

                if(is_delivery_schedule())
                    display_calender();
                else
                    display_list();

                using (vehicles v = new vehicles())
                {
                    ArrayList avd = v.get_vehicle_data(new SortedList());

                    dlVehicle.Items.Add(string.Empty);
                    dlVehicle.Items.Add("TBC");
                    dlDeliveryVehicle.Items.Add(string.Empty);

                    foreach (vehicle_data vd in avd)
                    {
                        dlVehicle.Items.Add(vd.registration.Trim());
                        dlDeliveryVehicle.Items.Add(vd.registration.Trim());
                    }
                }

                MultiView1.ActiveViewIndex = 0;
            }
            else
            {
                try { m_sl_schedule_rec = (SortedList)ViewState[VS_SCHEDULE_RECS]; }
                catch { }

                try { m_sl_dt_time_schedule_rec = (SortedList)ViewState[VS_SCHEDULE_RECS_DT_TIME]; }
                catch { }

                try { m_curr_view = ViewState[VS_CURR_VIEW].ToString(); }
                catch { }

                try { m_edit_view_recs = ViewState[VS_EDIT_VIEW_RECS].ToString(); }
                catch { }

                try { m_sl_welders_and_fitters = (SortedList)ViewState[VS_WELDERS_AND_FITTERS]; }
                catch { }

                try { m_sl_notes = (SortedList)ViewState[VS_NOTES]; }
                catch { }

                try { m_sl_notes_by_id = (SortedList)ViewState[VS_NOTES_BY_ID]; }
                catch { }

                if (MultiView1.ActiveViewIndex == 0 || MultiView1.ActiveViewIndex == 3 || MultiView1.ActiveViewIndex == 4 || MultiView1.ActiveViewIndex == 5)
                {
                    if (m_curr_view == CALENDER_VIEW)
                        display_calender();
                    else
                        display_list();
                }
                else if (MultiView1.ActiveViewIndex == 2)
                {
                    display_edit_view();
                }
            }

            lblMsg.Text = string.Empty;
        }

        bool is_fab_schedule()
        {
            bool bret = false;

            try { bret = ViewState[VS_TBL].ToString() == FAB_TBL; }
            catch { }

            return bret;
        }

        bool is_delivery_schedule()
        {
            bool bret = false;

            try { bret = ViewState[VS_TBL].ToString() == DELIV_TBL; }
            catch { }

            return bret;
        }

        void get_welders_and_fitters()
        {
            m_sl_welders_and_fitters = new SortedList();

            string select = "select id, login_id from users where role = 'fitter' or role = 'welder'";

            using (cdb_connection dbc = new cdb_connection())
            {
                DataTable dtab = dbc.get_data_p(select, null);

                foreach (DataRow dr in dtab.Rows)
                {
                    string id = string.Empty;
                    string login_id = string.Empty;

                    id = dr["id"].ToString();
                    login_id = dr["login_id"].ToString();

                    if (!m_sl_welders_and_fitters.ContainsKey(id))
                        m_sl_welders_and_fitters.Add(id, login_id);
                }
            }

            ViewState[VS_WELDERS_AND_FITTERS] = m_sl_welders_and_fitters;
        }

        void get_schedule_recs(DateTime dt_from, DateTime dt_to, string tbl)
        {
            if (m_sl_schedule_rec == null)
                m_sl_schedule_rec = new SortedList();
            else
                m_sl_schedule_rec.Clear();

            if (m_sl_dt_time_schedule_rec == null)
                m_sl_dt_time_schedule_rec = new SortedList();
            else
                m_sl_dt_time_schedule_rec.Clear();
            
            if (tbl.Trim().Length > 0)
            {
                string select = "select !TBL!.*, spools.id, spools.barcode, spools.status, spools.welder, spools.fitter, spools.material, customers.* ";

                /*
                select += " , stuff ";
                select += " ( ";
                select += "    (";
                select += "       select '; ' + CAST( (parts.welder_rate * spool_parts.qty ) as  nvarchar)";
                select += "       from spool_parts";
                select += "       inner join parts on parts.id = spool_parts.part_id";
                select += "       where spool_parts.spool_id = spools.id";
                select += "       and parts.part_type not like '%pipe%'";
                select += "       FOR XML PATH('')";
                select += "    )";
                select += "    , 1";
                select += "    , 1";
                select += "    , ''";
                select += ") WELDER_RATES";
                select += ",";

                select += " stuff ";
                select += " ( ";
                select += "    (";
                select += "       select '; ' + CAST( (parts.fitter_rate * spool_parts.qty) as  nvarchar)";
                select += "       from spool_parts";
                select += "       inner join parts on parts.id = spool_parts.part_id";
                select += "       where spool_parts.spool_id = spools.id";
                select += "       and parts.part_type not like '%pipe%'";
                select += "       FOR XML PATH('')";
                select += "    )";
                select += "    , 1";
                select += "    , 1";
                select += "    , ''";
                select += ") FITTER_RATES";
                */

                select += " from !TBL! ";
                select += " inner join spools on spools.id = !TBL!.spool_id ";
                select += " left  join customers on  REVERSE(PARSENAME(REPLACE(REVERSE(LEFT(spools.barcode,12)), '-', '.'), 1))  = customers.contract_number ";

                select += " where (spools.status !=  'SH' and spools.status !=  'OS' and spools.status !=  'IN' ";
                
                if (is_fab_schedule())
                    select += " and spools.status !=  'RD' and spools.status !=  'LD' ";

                if (is_fab_schedule() )
                    select += " and spools.material like @material + '%' ";

                select += " ) and (dt >= @dt_from and  dt <= @dt_to) ";

                if (is_fab_schedule())
                    select += " order by seq, dt, batch_number";

                select = select.Replace("!TBL!", tbl);

                string delivery_notes_select = "select * from delivery_schedule_notes where note_key = @note_key ";

                using (cdb_connection dbc = new cdb_connection())
                {
                    SortedList sl_p = new SortedList();
                    sl_p.Add("@dt_from", dt_from);
                    sl_p.Add("@dt_to", dt_to);

                    if (is_fab_schedule())
                        sl_p.Add("@material", dlMaterial.Text);

                    DataTable dtab = dbc.get_data_p(select, sl_p);

                    foreach (DataRow dr in dtab.Rows)
                    {
                        c_schedule_rec sr = null;

                        if (is_fab_schedule())
                        {
                            sr = new c_schedule_fab_rec();

                            schedule_data schd = schedule_fab.get_schedule_fab_data(dr);
                            sr.schd = schd;
                        }
                        else if (is_delivery_schedule())
                        {
                            sr = new c_schedule_deliv_rec();

                            schedule_data schd = schedule_delivery.get_schedule_delivery_data(dr);
                            sr.schd = schd;
                        }

                        if (sr.schd.status == "SH" || sr.schd.status == "OS" || sr.schd.status == "IN")
                            continue;

                        try { sr.barcode = dr["barcode"].ToString(); }
                        catch { }

                        try { sr.schd.project = dr["name"].ToString(); }
                        catch { }

                        try { sr.schd.cut_and_clean = (int)dr["cut_and_clean"]; }
                        catch { }

                        try { sr.schd.delivered = (bool)dr["delivered"]; }
                        catch { }

                        try { sr.schd.material = dr["material"].ToString(); }
                        catch { }

                        try { sr.schd.module = (bool)dr["module"]; }
                        catch { }

                        try { sr.schd.seq = (int)dr["seq"];}
                        catch { sr.schd.seq = dtab.Rows.Count; }

                        try 
                        {
                            sr.schd.site = dr["address_line1"].ToString();
                            sr.schd.site += ",\x20" + dr["address_line2"].ToString(); 
                        }
                        catch { }

                        try { sr.schd.e_code = dr["e_code"].ToString(); }
                        catch { }

                        if (sr.schd.e_code.Trim().Length == 0)
                            sr.schd.e_code = schedule_data.DEF_ECODE;

                        bool b_holding = (bool)ViewState[VS_HOLDING];

                        string k0 = string.Empty;

                        /*
                        if(b_holding)
                            k0 = sr.schd.batch_number.ToString();
                        else if(is_fab_schedule())
                            k0 = "FAB";
                        else
                            k0 = sr.schd.dt.ToString(DT_FMT);
                        */

                        if (is_fab_schedule())
                            k0 = "FAB";
                        else
                            k0 = sr.schd.dt.ToString(DT_FMT);

                        SortedList sl0 = null;

                        if (!m_sl_schedule_rec.ContainsKey(k0))
                        {
                            sl0 = new SortedList();
                            m_sl_schedule_rec.Add(k0, sl0);
                        }
                        else
                            sl0 = (SortedList)m_sl_schedule_rec[k0];

                        string fab_seq = string.Empty;

                        if (is_fab_schedule())
                        {
                            sr.schd.seq = sl0.Count + 1;
                            fab_seq = sr.schd.seq.ToString(SEQ_FMT)+ "~";
                        }

                        string k1 = fab_seq;

                        if (sr.schd.batch_number > 0)
                            k1 += (sr.schd.batch_number.ToString(BATCH_FMT) + "|" + sr.barcode.Split('-')[0]).ToUpper();
                        else
                            k1 += EXTRAS + "|";

                        SortedList sl1 = null;

                        if (!sl0.ContainsKey(k1))
                        {
                            sl1 = new SortedList();
                            sl0.Add(k1, sl1);
                        }
                        else
                            sl1 = (SortedList)sl0[k1];

                        string k2 = (sr.schd.dt.ToString("HHmm") + sr.barcode).ToUpper();

                        if (!sl1.ContainsKey(k2))
                            sl1.Add(k2, sr);

                        k0 = sr.schd.dt.ToString(DT_FMT2 + "_" + sr.schd.vehicle);

                        if (!m_sl_dt_time_schedule_rec.ContainsKey(k0))
                        {
                            sl0 = new SortedList();
                            m_sl_dt_time_schedule_rec.Add(k0, sl0);

                            SortedList sl_notes_p = new SortedList();

                            sl_notes_p.Add("@note_key", k0);
                            
                            DataTable dtab_notes = dbc.get_data_p(delivery_notes_select, sl_notes_p);

                            foreach (DataRow dr_n in dtab_notes.Rows)
                            {
                                string note = dr_n["note"].ToString();
                                int note_id = (int)dr_n["id"];

                                if (m_sl_notes == null)
                                    m_sl_notes = new SortedList();
                                
                                if (!m_sl_notes.ContainsKey(k0))
                                {
                                    m_sl_notes.Add(k0, note);
                                }

                                if (m_sl_notes_by_id == null)
                                    m_sl_notes_by_id = new SortedList();

                                if (!m_sl_notes_by_id.ContainsKey(k0))
                                {
                                    m_sl_notes_by_id.Add(k0, note_id);
                                }
                            }
                        }
                        else
                            sl0 = (SortedList)m_sl_dt_time_schedule_rec[k0];

                        k1 = fab_seq;

                        if (sr.schd.batch_number > 0)
                            k1 += (sr.schd.batch_number.ToString(BATCH_FMT) + "|" + sr.barcode.Split('-')[0]).ToUpper();
                        else
                            k1 += EXTRAS + "|";

                        sl1 = null;

                        if (!sl0.ContainsKey(k1))
                        {
                            sl1 = new SortedList();
                            sl0.Add(k1, sl1);
                        }
                        else
                            sl1 = (SortedList)sl0[k1];

                        k2 = (fab_seq + sr.schd.dt.ToString("HHmm") + sr.barcode).ToUpper();

                        if (!sl1.ContainsKey(k2))
                            sl1.Add(k2, sr);
                    }
                }
            }

            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;
            ViewState[VS_SCHEDULE_RECS_DT_TIME] = m_sl_dt_time_schedule_rec;
            ViewState[VS_NOTES] = m_sl_notes;
            ViewState[VS_NOTES_BY_ID] = m_sl_notes_by_id;
        }

        void display_calender()
        {
            MultiView1.ActiveViewIndex = 0;

            m_curr_view = CALENDER_VIEW;
            ViewState[VS_CURR_VIEW] = m_curr_view;

            lblMaterial.Visible = false;
            dlMaterial.Visible = false;
            pnlFilter.Visible = false;
            imgReSeq.Visible = false;

            bool bcan_add_to_schedule = false;

            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl0 = new SortedList();

            sl0.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);
            }

            if (a.Count > 0)
            {
                user_data ud = (user_data)a[0];

                bcan_add_to_schedule = ud.is_special_permission_set(0); //ud.special_permissions == 1;
            }

            tblSchedule.Rows.Clear();
            
            if (m_sl_schedule_rec != null)
            {
                if (m_sl_schedule_rec.Count > 0)
                {
                    DateTime start_date = DateTime.ParseExact(m_sl_schedule_rec.GetKey(0).ToString(), DT_FMT, System.Globalization.CultureInfo.InvariantCulture);
                    DateTime end_date = DateTime.ParseExact(m_sl_schedule_rec.GetKey(m_sl_schedule_rec.Count - 1).ToString(), DT_FMT, System.Globalization.CultureInfo.InvariantCulture);

                    while (start_date.DayOfWeek != DayOfWeek.Monday)
                        start_date = start_date.AddDays(-1);

                    TimeSpan t = (end_date - start_date);
                    double num_of_days = t.TotalDays;
                    num_of_days++;

                    double num_of_week_days = 0;

                    for (double d = 0; d < num_of_days; d++)
                    {
                        DateTime dt = start_date.AddDays(d);

                        if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                            num_of_week_days++;
                    }

                    const int SQ_DIM = 250;
                    TableRow r;
                    TableCell c;

                    int ncells = Convert.ToInt32(num_of_days);

                    r = new TableRow();

                    for (int i = 0, n = 1; i < ncells; i++, n++)
                    {
                        DateTime dt = start_date.AddDays(i);

                        c = new TableCell();

                        c.Height = c.Width = SQ_DIM;
                        c.VerticalAlign = VerticalAlign.Top;

                        System.Drawing.Color bc = new System.Drawing.Color();

                        if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                            bc = System.Drawing.Color.FromName("Silver");
                        else
                            bc = System.Drawing.Color.FromName("LightGray");

                        c.BackColor = bc;
                        c.BorderStyle = BorderStyle.Solid;
                        c.BorderWidth = 1;
                        c.Style["OVERFLOW"] = "auto";
                        double font_size = 9;
                        FontUnit fu = new FontUnit(font_size);
                        c.Font.Size = fu;

                        Panel p = new Panel();
                        p.ScrollBars = ScrollBars.Both;
                        p.Height = p.Width = SQ_DIM;

                        string k = dt.ToString(DT_FMT);

                        if (m_sl_schedule_rec.ContainsKey(k))
                        {
                            SortedList sl_batches = (SortedList)m_sl_schedule_rec[k];

                            foreach (DictionaryEntry e0 in sl_batches)
                            {
                                string [] sa0 = e0.Key.ToString().Split('|');

                                string batch_no = sa0[0].Trim();
                                string project = sa0[1].Trim();

                                SortedList sl_batch = (SortedList)e0.Value;

                                //if (batch_no == EXTRAS)
                                //    project = sl_batch.Count.ToString();

                                TreeView treeview = new TreeView();

                                treeview.ID = "tv_" + k + "_" + batch_no + "_" + project;

                                treeview.ImageSet = TreeViewImageSet.Simple;
                                treeview.SelectedNodeChanged += new EventHandler(treeview_SelectedNodeChanged);

                                TreeNode n0 = new TreeNode(batch_no + "\x20" + project);
                                n0.Value = "b|" + batch_no + "^" + project + "|" + k;

                                treeview.Nodes.Add(n0);

                                foreach (DictionaryEntry e1 in sl_batch)
                                {
                                    c_schedule_rec sr = (c_schedule_rec)e1.Value;

                                    string node_text = string.Empty;

                                    if (is_fab_schedule())
                                    {
                                        node_text = sr.barcode;
                                    }
                                    else if (is_delivery_schedule())
                                    {
                                        node_text = sr.schd.dt.ToString("HH:mm");
                                        node_text += "\x20";
                                        node_text += sr.barcode;
                                        node_text += "\x20";
                                        node_text += sr.schd.vehicle;
                                    }

                                    TreeNode n1 = new TreeNode(node_text);
                                    n1.Value = "s|" + batch_no + "^" + project + "|" + k + "|" + sr.barcode;

                                    n0.ChildNodes.Add(n1);
                                }

                                treeview.CollapseAll();

                                p.Controls.Add(treeview);
                            }
                        }

                        Label l = new Label();
                        l.Text = dt.DayOfWeek.ToString() + "\x20" + dt.ToLongDateString();
                        l.Width = SQ_DIM;
                        l.ForeColor = System.Drawing.Color.FromName("DarkGreen");
                        l.Font.Bold = true;

                        if (is_fab_schedule())
                        {
                            Label l2 = new Label();
                            l2.ForeColor = l.ForeColor;
                            l2.Font.Bold = l.Font.Bold;

                            ArrayList a_slots_available = get_available_slots(dt.ToString("dd/MM/yyyy"));

                            /*
                            l2.Text += "[" + a_slots_available[0].ToString() + "]";
                            l2.Text += "[" + a_slots_available[1].ToString() + "]";
                            */
                        }

                        ImageButton btn_add = null;

                        if (is_fab_schedule())
                        {
                            if (bcan_add_to_schedule)
                            {
                                btn_add = new ImageButton();
                                btn_add.ID = "btn" + n.ToString("0000");
                                btn_add.ImageUrl = "~/add.png";
                                btn_add.ToolTip = "Add To Schedule";
                                btn_add.Attributes[K_DATE] = k;
                                btn_add.Click += new ImageClickEventHandler(btn_add_Click);

                                l.Width = (int)(SQ_DIM - 16);
                            }
                        }
                        
                        c.Controls.Add(l);

                        if (is_fab_schedule())
                        {
                            Label l2 = new Label();
                            l2.ForeColor = l.ForeColor;
                            l2.Font.Bold = l.Font.Bold;
                            l2.Width = l.Width;

                            ArrayList a_slots_available = get_available_slots(dt.ToString("dd/MM/yyyy"));

                            /*
                            l2.Text += "[" + a_slots_available[0].ToString() + "]";
                            l2.Text += "[" + a_slots_available[1].ToString() + "]";
                            c.Controls.Add(l2);
                            */
                        }

                        if (is_fab_schedule())
                        {
                            if (bcan_add_to_schedule)
                            {
                                c.Controls.Add(btn_add);
                            }
                        }

                        c.Controls.Add(p);

                        r.Cells.Add(c);

                        if (n % 7 == 0 || n == ncells)
                        {
                            tblSchedule.Rows.Add(r);

                            r = new TableRow();
                        }
                    }
                }
            }
        }

        void display_edit_view()
        {
            if (m_edit_view_recs.Trim().Length > 0)
            {
                string[] sa = m_edit_view_recs.Split('|');

                if (sa.Length > 0)
                {
                    bool bbatch = sa[0] == "b";
                    string sbatch = sa[1].Replace("^", "|");
                    string sdt = sa[2];
                    string sbarcode = string.Empty;

                    if (sa.Length > 3)
                        sbarcode = sa[3].ToUpper();

                    if (is_fab_schedule())
                    {
                        txtTime.Visible = dlVehicle.Visible = chkTime.Visible = chkVehicle.Visible = false;
                    }
                    else if (is_delivery_schedule())
                    {
                        txtTime.Visible = dlVehicle.Visible = chkTime.Visible = chkVehicle.Visible = true;
                        chkAddToExtras.Visible = false;
                    }

                    if (m_sl_schedule_rec.ContainsKey(sdt))
                    {
                        SortedList sl_batches = (SortedList)m_sl_schedule_rec[sdt];

                        if (sl_batches.ContainsKey(sbatch))
                        {
                            SortedList sl_batch = (SortedList)sl_batches[sbatch];

                            TableRow r;
                            TableCell c;
                            CheckBox chk;
                            
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightGreen");

                            chk = new CheckBox();

                            chk.ID = CHK0;
                            chk.Checked = true;
                            chk.AutoPostBack = true;
                            chk.CheckedChanged += new EventHandler(chk_CheckedChanged);
                            c = new TableCell();
                            c.Controls.Add(chk);
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Spool Barcode"));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Date"));
                            r.Cells.Add(c);

                            if (is_delivery_schedule())
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Time"));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Vehicle"));
                                r.Cells.Add(c);
                            }

                            tblRecs.Rows.Add(r);
                        
                            int i = 0;

                            foreach (DictionaryEntry e1 in sl_batch)
                            {
                                c_schedule_rec sr = (c_schedule_rec)e1.Value;

                                if (sbarcode.Length != 0)
                                    if (sr.barcode.ToLower() != sbarcode.ToLower())
                                        continue;

                                r = new TableRow();
                                r.Attributes[UID] = sr.schd.id.ToString();
                                r.Attributes[BARCODE] = sr.barcode.ToUpper();
                                r.Attributes[DATE] = sdt;
                                r.Attributes[BATCH] = sbatch;

                                System.Drawing.Color bc;
                                if (i++ % 2 == 0)
                                    bc = System.Drawing.Color.FromName("White");
                                else
                                    bc = System.Drawing.Color.FromName("LightGray");
                                
                                r.BackColor = bc;

                                chk = new CheckBox();
                                chk.Checked = true;
                                chk.ID = CHK + sr.schd.id.ToString();
                                c = new TableCell();
                                c.Controls.Add(chk);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(sr.barcode));
                                r.Cells.Add(c);

                                c = new TableCell();
                                TextBox txt_date = new TextBox();
                                txt_date.ID = TXT_DATE + sr.schd.id.ToString();
                                txt_date.Attributes[UID] = sr.schd.id.ToString();
                                txt_date.Attributes[BARCODE] = sr.barcode.ToUpper();
                                txt_date.Text = sr.schd.dt.ToString("dd/MM/yyyy");
                                c.Controls.Add(txt_date);
                                r.Cells.Add(c);

                                if (is_fab_schedule())
                                {
                                 
                                }
                                else if (is_delivery_schedule())
                                {
                                    c = new TableCell();
                                    TextBox txt_time = new TextBox();
                                    txt_time.ID = TXT_TIME + sr.schd.id.ToString();
                                    txt_time.Attributes[UID] = sr.schd.id.ToString();
                                    txt_time.Attributes[BARCODE] = sr.barcode.ToUpper();
                                    txt_time.Text = sr.schd.dt.ToString("HH:mm");
                                    c.Controls.Add(txt_time);
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    DropDownList dl = new DropDownList();
                                    dl.ID = DL_VEH + sr.schd.id.ToString();
                                    dl.Attributes[UID] = sr.schd.id.ToString();
                                    dl.Attributes[BARCODE] = sr.barcode.ToUpper();
                                    
                                    foreach (ListItem li in dlVehicle.Items)
                                        dl.Items.Add(li.Text);

                                    dl.Text = sr.schd.vehicle;

                                    c.Controls.Add(dl);

                                    r.Cells.Add(c);
                                }

                                c = new TableCell();
                                
                                c.HorizontalAlign = HorizontalAlign.Center;
                                ImageButton btn_save = new ImageButton();
                                btn_save.ToolTip = "Save";
                                btn_save.ImageUrl = "~/disk.png";
                                btn_save.Click += new ImageClickEventHandler(btn_save_Click);
                                btn_save.ID = "btn_save" + sr.schd.id.ToString();
                                btn_save.Attributes[UID] = sr.schd.id.ToString();
                                btn_save.Attributes[BARCODE] = sr.barcode.ToUpper();
                                btn_save.Attributes[DATE] = sdt;
                                btn_save.Attributes[BATCH] = sbatch;
                                c.Controls.Add(btn_save);
                                r.Cells.Add(c);

                                tblRecs.Rows.Add(r);
                            }
                        }
                    }
                }
            }
        }

        void treeview_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeView t = (TreeView)sender;

            TreeNode n = t.SelectedNode;

            m_edit_view_recs = n.Value;
            ViewState[VS_EDIT_VIEW_RECS] = m_edit_view_recs;

            display_edit_view();

            if (tblRecs.Rows.Count > 0)
            {
                TableRow r = tblRecs.Rows[0];

                CheckBox chk = get_tbl_row_checkbox(r, CHK0);
                chk.Checked = true;
                chk_CheckedChanged(chk, null);
            }

            n.Selected = false;

            MultiView1.ActiveViewIndex = 2;

            set_view_btn_text();
        }

        void chk_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            foreach (TableRow r in tblRecs.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.GetType() == typeof(CheckBox))
                        {
                            CheckBox cb = (CheckBox)cntrl;

                            cb.Checked = chk.Checked;
                        }
                    }
                }
            }
        }

        void btn_add_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            ImageButton b = (ImageButton)sender;

            string k_date = (b.Attributes[K_DATE]);

            DateTime dt = DateTime.ParseExact(k_date, DT_FMT, System.Globalization.CultureInfo.InvariantCulture);

            lblAddTitle.Text = "Add To Fabrication Schedule : " + dt.DayOfWeek.ToString() + "\x20" + dt.ToLongDateString(); ;

            ViewState[K_DATE] = dt;

            MultiView1.ActiveViewIndex = 1;
            txtBarcode.Focus();
            
        }

        bool display_list_record(schedule_data sd, string fitter_id)
        {
            bool bret = false;

            if (sd.status == "SC" || sd.status == "RP")
            {
                bret = chkRed.Checked;
            }
            else if (sd.status == "IP" || sd.status == "PC")
            {
                bret = chkOrange.Checked;
            }
            else if (sd.status == "QA" || sd.status == "WT")
            {
                bret = chkBlue.Checked;
            }
            else if (sd.status == "RD" || sd.status == "LD")
            {
                bret = chkGreen.Checked;
            }

            if (!is_holding_area() && !is_delivery_schedule())
            {
                if (sd.module)
                    bret &= chkModules.Checked;
                else
                    bret &= chkSpools.Checked;
            }

            if (fitter_id.Trim().Length > 0)
            {
                bret = sd.fitter == fitter_id;
            }

            return bret;
        }

        void display_list()
        {
            imgReSeq.Visible = true;

            string fitter_id = string.Empty;

            if (txtFitter.Text.Trim().Length > 0)
            {
                using (users u = new users())
                {
                    user_data ud = u.get_user_data(txtFitter.Text.Trim());

                    if (ud != null)
                        fitter_id = ud.id.ToString();
                    else
                        fitter_id = "0";
                }
            }

            m_curr_view = LIST_VIEW;
            ViewState[VS_CURR_VIEW] = m_curr_view;

            bool b_holding = (bool)ViewState[VS_HOLDING];

            pnlFilter.Visible = !b_holding;
            btnView.Visible = is_delivery_schedule();

            tblSchedule.Rows.Clear();

            System.Drawing.Color HDR_ROW_BC = System.Drawing.Color.FromName("DarkSlateBlue");
            System.Drawing.Color HDR_ROW_FC = System.Drawing.Color.FromName("White");
            System.Drawing.Color ROW_DL1 = System.Drawing.Color.FromName("White");
            System.Drawing.Color ROW_DL2 = System.Drawing.Color.FromName("LightGray");

            SortedList sl_schedule_rec = null;

            if (is_delivery_schedule())
            {
                sl_schedule_rec = m_sl_dt_time_schedule_rec;

                chkSpools.Visible = chkModules.Visible = dlMaterial.Visible = false;
            }
            else
                sl_schedule_rec = m_sl_schedule_rec;

            if (sl_schedule_rec != null)
            {
                if (sl_schedule_rec.Count > 0)
                {
                    TableRow r;
                    TableCell c;

                    int n = 0;

                    foreach (DictionaryEntry e0 in sl_schedule_rec)
                    {
                        int i = 0;

                        SortedList sl0 = (SortedList)e0.Value;

                        if (sl0.Count > 0)
                        {
                            if (is_delivery_schedule())
                            {
                                DateTime dt_date = DateTime.ParseExact(e0.Key.ToString().Split('_')[0], DT_FMT, System.Globalization.CultureInfo.InvariantCulture);

                                if (tblSchedule.Rows.Count > 0)
                                {
                                    r = new TableRow();
                                    c = new TableCell();
                                    c.Height = 10;
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                    tblSchedule.Rows.Add(r);
                                }

                                r = new TableRow();

                                if (is_delivery_schedule())
                                {
                                    c = new TableCell();
                                    c.BackColor = (ROW_DL1);
                                    c.HorizontalAlign = HorizontalAlign.Center;

                                    ImageButton btn_notes = new ImageButton();

                                    btn_notes.ToolTip = "Notes";
                                    btn_notes.ImageUrl = "~/list.png";
                                    btn_notes.Click += new ImageClickEventHandler(btn_notes_Click);
                                    btn_notes.ID = "BTN_NOTES_" + n++.ToString();
                                    btn_notes.Attributes[UID] = e0.Key.ToString();

                                    r.ID = e0.Key.ToString();

                                    c.Controls.Add(btn_notes);
                                    r.Cells.Add(c);
                                }
                                else
                                {
                                    c = new TableCell();
                                    c.Controls.Add(new LiteralControl(string.Empty));
                                    r.Cells.Add(c);
                                }

                                c = new TableCell();

                                string sdt = dt_date.ToString("dd/MM/yy ") + dt_date.DayOfWeek;

                                if (is_delivery_schedule())
                                    sdt += "\x20@" + e0.Key.ToString().Split('_')[1] + "\x20" + e0.Key.ToString().Split('_')[2];

                                c.Controls.Add(new LiteralControl(sdt));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);

                                tblSchedule.Rows.Add(r);

                                if (is_delivery_schedule())
                                {
                                    r = new TableRow();

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Center;
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.HorizontalAlign = HorizontalAlign.Left;

                                    LiteralControl lc_note = new LiteralControl();
                                    lc_note.ID = "lc_note_" + e0.Key.ToString();

                                    if (m_sl_notes != null)
                                        if (m_sl_notes.ContainsKey(e0.Key.ToString()))
                                            lc_note.Text = m_sl_notes[e0.Key.ToString()].ToString();

                                    c.Controls.Add(lc_note);

                                    r.Cells.Add(c);

                                    tblSchedule.Rows.Add(r);
                                }
                            }

                            Panel pnl = new Panel();
                            Table tbl = new Table();

                            r = new TableRow();

                            if (!b_holding)
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);
                            }

                            if (is_fab_schedule())
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Seq"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);
                            }

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Batch"));
                            c.BackColor = (HDR_ROW_BC);
                            c.ForeColor = (HDR_ROW_FC);
                            r.Cells.Add(c);

                            if (is_delivery_schedule())
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Time"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);
                            }

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Spool"));
                            c.BackColor = (HDR_ROW_BC);
                            c.ForeColor = (HDR_ROW_FC);
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl("Project"));
                            c.BackColor = (HDR_ROW_BC);
                            c.ForeColor = (HDR_ROW_FC);
                            r.Cells.Add(c);

                            if (is_fab_schedule())
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Cut & Clean"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);

                                if (!b_holding)
                                {
                                    c = new TableCell();
                                    c.Controls.Add(new LiteralControl("Code"));
                                    c.BackColor = (HDR_ROW_BC);
                                    c.ForeColor = (HDR_ROW_FC);
                                    r.Cells.Add(c);
                                }

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Module"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Fit Time"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Weld Time"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);
                            }

                            if (is_delivery_schedule())
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Vehicle"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl("Site"));
                                c.BackColor = (HDR_ROW_BC);
                                c.ForeColor = (HDR_ROW_FC);
                                r.Cells.Add(c);
                            }

                            tbl.Rows.Add(r);

                            SortedList sl_batches = (SortedList)e0.Value;

                            foreach (DictionaryEntry e in sl_batches)
                            {
                                string[] sa = e.Key.ToString().Split('|');

                                SortedList sl_batch = (SortedList)e.Value;

                                foreach (DictionaryEntry e1 in sl_batch)
                                {
                                    c_schedule_rec sr = (c_schedule_rec)e1.Value;

                                    if (sr.schd.status == "SH" || sr.schd.status == "OS" || sr.schd.status == "IN" || !display_list_record(sr.schd, fitter_id))
                                        continue;

                                    r = new TableRow();

                                    r.Attributes[UID] = sr.schd.id.ToString();

                                    r.Attributes[ATT_BC] = sr.barcode;

                                    System.Drawing.Color bc;
                                    if (i++ % 2 == 0)
                                        bc = ROW_DL1;
                                    else
                                        bc = ROW_DL2;

                                    r.BackColor = bc;

                                    if (!b_holding)
                                    {
                                        c = new TableCell();

                                        const string TL = "^^";

                                        string tl = TL;

                                        if (sr.schd.status == "SC" || sr.schd.status == "RP")
                                        {
                                            c.BackColor = System.Drawing.Color.FromName("Red");
                                        }
                                        else if (sr.schd.status == "IP" || sr.schd.status == "PC")
                                        {
                                            c.BackColor = System.Drawing.Color.FromName("DarkOrange");

                                            if (sr.schd.status == "IP")
                                            {
                                                /*
                                                if (m_sl_welders_and_fitters.ContainsKey(sr.schd.welder))
                                                    tl = m_sl_welders_and_fitters[sr.schd.welder].ToString();
                                                else if (m_sl_welders_and_fitters.ContainsKey(sr.schd.fitter))
                                                    tl = m_sl_welders_and_fitters[sr.schd.fitter].ToString();
                                                 */

                                                string sfitter, swelder;
                                                sfitter = swelder = "-";

                                                if (m_sl_welders_and_fitters.ContainsKey(sr.schd.fitter))
                                                    sfitter = m_sl_welders_and_fitters[sr.schd.fitter].ToString();

                                                if (m_sl_welders_and_fitters.ContainsKey(sr.schd.welder))
                                                    swelder = m_sl_welders_and_fitters[sr.schd.welder].ToString();

                                                tl = sfitter + "/" + swelder;

                                            }
                                        }
                                        else if (sr.schd.status == "QA" || sr.schd.status == "WT")
                                        {
                                            c.BackColor = System.Drawing.Color.FromName("DeepSkyBlue");
                                        }
                                        else if (sr.schd.status == "RD" || sr.schd.status == "LD")
                                        {
                                            c.BackColor = System.Drawing.Color.FromName("Green");
                                        }

                                        if (is_fab_schedule())
                                        {
                                            if (sr.schd.delivered)
                                            {
                                                c.HorizontalAlign = HorizontalAlign.Left;
                                                Image img_delivered = new Image();
                                                img_delivered.ToolTip = "Parts delivered";
                                                img_delivered.ImageUrl = "~/picked.png";
                                                c.Controls.Add(img_delivered);
                                            }
                                        }

                                        if (tl == TL)
                                            c.ForeColor = c.BackColor;

                                        c.Controls.Add(new LiteralControl(tl));

                                        r.Cells.Add(c);
                                    }

                                    if (is_fab_schedule())
                                    {
                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(sr.schd.seq.ToString()));

                                        r.Cells.Add(c);
                                    }

                                    c = new TableCell();

                                    if (sr.schd.batch_number > 0)
                                        c.Controls.Add(new LiteralControl(sr.schd.batch_number.ToString()));
                                    else
                                        c.Controls.Add(new LiteralControl("Extra"));

                                    r.Cells.Add(c);

                                    if (is_delivery_schedule())
                                    {
                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(sr.schd.dt.ToString("HH:mm")));
                                        r.Cells.Add(c);
                                    }

                                    c = new TableCell();

                                    if (sr.schd.material.Trim() == "Stainless Steel")
                                        c.ForeColor = System.Drawing.Color.FromName("Red");
                                    else if (sr.schd.material.Trim() == "Screwed")
                                    {
                                        c.ForeColor = System.Drawing.Color.FromName("Lime");
                                    }
                                    else if (sr.schd.material.Trim() == "HS")
                                    {
                                        c.ForeColor = System.Drawing.Color.FromName("Purple");
                                    }

                                    Table tblSpool = new Table();
                                    tblSpool.Width = 500;
                                    TableCell cellSpool = new TableCell();
                                    TableRow rowSpool = new TableRow();
                                    cellSpool.Controls.Add(new LiteralControl(sr.barcode));
                                    rowSpool.Cells.Add(cellSpool);

                                    tblSpool.Rows.Add(rowSpool);
                                    c.Controls.Add(tblSpool);

                                    //c.Controls.Add(new LiteralControl(sr.barcode));  
                                    r.Cells.Add(c);

                                    c = new TableCell();
                                    c.Controls.Add(new LiteralControl(sr.schd.project));
                                    r.Cells.Add(c);

                                    if (is_fab_schedule())
                                    {
                                        c = new TableCell();

                                        CheckBox chk_cut = new CheckBox();

                                        chk_cut.ID = "cut_" + sr.schd.id.ToString();
                                        chk_cut.Attributes[UID] = sr.schd.id.ToString();
                                        chk_cut.Checked = sr.schd.is_cut_and_clean_bit_set(0);
                                        chk_cut.AutoPostBack = true;
                                        chk_cut.CheckedChanged += new EventHandler(chk_cut_CheckedChanged);
                                        chk_cut.BackColor = sr.schd.is_cut_and_clean_bit_set(0) ? System.Drawing.Color.FromName("Green") : System.Drawing.Color.FromName("Red");
                                        chk_cut.ToolTip = "Cut";

                                        CheckBox chk_clean = new CheckBox();

                                        chk_clean.ID = "clean_" + sr.schd.id.ToString();
                                        chk_clean.Attributes[UID] = sr.schd.id.ToString();
                                        chk_clean.Checked = sr.schd.is_cut_and_clean_bit_set(1);
                                        chk_clean.AutoPostBack = true;
                                        chk_clean.CheckedChanged += new EventHandler(chk_clean_CheckedChanged);
                                        chk_clean.BackColor = sr.schd.is_cut_and_clean_bit_set(1) ? System.Drawing.Color.FromName("Green") : System.Drawing.Color.FromName("Red");
                                        chk_clean.ToolTip = "Clean";

                                        c.HorizontalAlign = HorizontalAlign.Center;

                                        c.Controls.Add(chk_cut);
                                        c.Controls.Add(chk_clean);

                                        r.Cells.Add(c);

                                        if (!b_holding)
                                        {
                                            c = new TableCell();

                                            DropDownList dl_code = new DropDownList();
                                            dl_code.ID = "dl_code_" + sr.schd.id.ToString();
                                            dl_code.Attributes[UID] = sr.schd.id.ToString();
                                            dl_code.AutoPostBack = true;

                                            foreach (DictionaryEntry e2 in m_sl_codes)
                                                dl_code.Items.Add(e2.Key.ToString());

                                            dl_code.Text = sr.schd.e_code;

                                            dl_code.SelectedIndexChanged += new EventHandler(dl_code_SelectedIndexChanged);

                                            c.Controls.Add(dl_code);

                                            r.Cells.Add(c);
                                        }

                                        c = new TableCell();

                                        CheckBox chk_mod = new CheckBox();

                                        chk_mod.ID = "module_" + sr.schd.id.ToString();
                                        chk_mod.Attributes[UID] = sr.schd.id.ToString();
                                        chk_mod.Checked = sr.schd.module;
                                        chk_mod.AutoPostBack = true;
                                        chk_mod.CheckedChanged += new EventHandler(chk_mod_CheckedChanged);

                                        c.HorizontalAlign = HorizontalAlign.Center;
                                        c.Controls.Add(chk_mod);

                                        r.Cells.Add(c);

                                        c = new TableCell();

                                        c.HorizontalAlign = HorizontalAlign.Right;

                                        decimal rate_per_minute_welder = 0;
                                        decimal rate_per_minute_fitter = 0;

                                        try { rate_per_minute_welder = Convert.ToDecimal(System.Web.Configuration.WebConfigurationManager.AppSettings["rate_per_minute_welder"].ToString()); }
                                        catch { }

                                        try { rate_per_minute_fitter = Convert.ToDecimal(System.Web.Configuration.WebConfigurationManager.AppSettings["rate_per_minute_fitter"].ToString()); }
                                        catch { }

                                        decimal dtime = 0;

                                        string[] sa_rates = sr.schd.fitter_rates.Split(';');

                                        foreach (string srate in sa_rates)
                                        {
                                            decimal drate = 0;

                                            try { drate = Convert.ToDecimal(srate); }
                                            catch { }

                                            dtime += Math.Ceiling(drate / rate_per_minute_fitter);
                                        }

                                        TimeSpan ts = new TimeSpan(0, Convert.ToInt32(dtime), 0);

                                        string s_ts = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
                                        c.Controls.Add(new LiteralControl(s_ts));

                                        r.Cells.Add(c);

                                        c = new TableCell();

                                        c.HorizontalAlign = HorizontalAlign.Right;

                                        dtime = 0;

                                        sa_rates = sr.schd.welder_rates.Split(';');

                                        foreach (string srate in sa_rates)
                                        {
                                            decimal drate = 0;

                                            try { drate = Convert.ToDecimal(srate); }
                                            catch { }

                                            dtime += Math.Ceiling(drate / rate_per_minute_welder);
                                        }

                                        ts = new TimeSpan(0, Convert.ToInt32(dtime), 0);

                                        s_ts = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00");
                                        c.Controls.Add(new LiteralControl(s_ts));

                                        r.Cells.Add(c);
                                    }

                                    if (is_delivery_schedule())
                                    {
                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(sr.schd.vehicle));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(sr.schd.site));
                                        r.Cells.Add(c);
                                    }

                                    CheckBox chk_sched_tbl_rec = new CheckBox();
                                    chk_sched_tbl_rec.ID = "chk_sched_tbl_rec_" + e.Key.ToString()+ "|" + sr.barcode;
                                    chk_sched_tbl_rec.Attributes[UID] = e.Key.ToString();
                                    chk_sched_tbl_rec.Attributes[BARCODE] = sr.barcode;

                                    c = new TableCell();
                                    c.Controls.Add(chk_sched_tbl_rec);
                                    r.Cells.Add(c);

                                    tbl.Rows.Add(r);
                                }
                            }

                            pnl.Controls.Add(tbl);

                            r = new TableRow();

                            c = new TableCell();
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.Controls.Add(pnl);
                            r.Cells.Add(c);
                            tblSchedule.Rows.Add(r);

                        }
                    }
                }
                else
                {
                    imgReSeq.Visible = false;
                }
            }
        }

        void chk_clean_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            chk.BackColor = chk.Checked ? System.Drawing.Color.FromName("Green") : System.Drawing.Color.FromName("Red");

            string uid = (chk.Attributes[UID]);

            int id = Convert.ToInt32(uid);

            foreach (DictionaryEntry e0 in m_sl_schedule_rec)
            {
                SortedList sl_batches = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_batches)
                {
                    string[] sa = e1.Key.ToString().Split('|');

                    SortedList sl_batch = (SortedList)e1.Value;

                    foreach (DictionaryEntry e2 in sl_batch)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e2.Value;

                        if (sr.schd.id == id)
                        {
                            if (chk.Checked)
                                sr.schd.set_cut_and_clean_bit_on(1);
                            else
                                sr.schd.set_cut_and_clean_bit_off(1);

                            SortedList sl = new SortedList();

                            sl.Add("id", uid);
                            sl.Add("cut_and_clean", sr.schd.cut_and_clean);

                            using (schedule_fab schdf = new schedule_fab())
                            {
                                schdf.save_schedule_fab_data(sl);
                            }

                            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;

                            return;
                        }
                    }
                }
            }
        }

        void chk_cut_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            chk.BackColor = chk.Checked ? System.Drawing.Color.FromName("Green") : System.Drawing.Color.FromName("Red");

            string uid = (chk.Attributes[UID]);

            int id = Convert.ToInt32(uid);

            foreach (DictionaryEntry e0 in m_sl_schedule_rec)
            {
                SortedList sl_batches = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_batches)
                {
                    string[] sa = e1.Key.ToString().Split('|');

                    SortedList sl_batch = (SortedList)e1.Value;

                    foreach (DictionaryEntry e2 in sl_batch)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e2.Value;

                        if (sr.schd.id == id)
                        {
                            if (chk.Checked)
                                sr.schd.set_cut_and_clean_bit_on(0);
                            else
                                sr.schd.set_cut_and_clean_bit_off(0);

                            SortedList sl = new SortedList();

                            sl.Add("id", uid);
                            sl.Add("cut_and_clean", sr.schd.cut_and_clean);

                            using (schedule_fab schdf = new schedule_fab())
                            {
                                schdf.save_schedule_fab_data(sl);
                            }

                            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;
                            
                            return;
                        }
                    }
                }
            }
        }

        void remove_schd_rec_from_tbl(string uid)
        {
            bool bdone = false;
            Table tbl = null; 
            TableRow r_to_delete = null;

            foreach (TableRow r0 in tblSchedule.Rows)
            {
                foreach (TableCell c0 in r0.Cells)
                {
                    foreach (Control cntrl0 in c0.Controls)
                    {
                        if (cntrl0.GetType() == typeof(Panel))
                        {
                            Panel pnl = (Panel)cntrl0;

                            foreach (Control cntrl1 in pnl.Controls)
                            {
                                if (cntrl1.GetType() == typeof(Table))
                                {
                                    tbl = (Table)cntrl1;

                                    foreach (TableRow r1 in tbl.Rows)
                                    {
                                        string ruid = string.Empty;

                                        try { ruid = r1.Attributes[UID]; }
                                        catch { }

                                        if (ruid != null)
                                        {
                                            if (ruid == uid)
                                            {
                                                r_to_delete = r1;
                                                bdone = true;
                                            }
                                        }
                                        if (bdone)
                                            break;
                                    }
                                }
                            }
                            if (bdone)
                                break;
                        }
                        if (bdone)
                            break;
                    }
                    if (bdone)
                        break;
                }
                if (bdone)
                    break;
            }

            if(tbl != null)
                if (r_to_delete != null)
                    tbl.Rows.Remove(r_to_delete);
        }

        void chk_mod_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes[UID]);

            SortedList sl = new SortedList();

            sl.Add("id", uid);
            sl.Add("module", chk.Checked);

            using (schedule_fab schdf = new schedule_fab())
            {
                schdf.save_schedule_fab_data(sl);
            }

            bool bremove = !is_holding_area();

            bremove &= (chk.Checked && !chkModules.Checked) || (!chk.Checked && !chkSpools.Checked);

            if (bremove)
            {
                remove_schd_rec_from_tbl(uid);
            }

            int id = Convert.ToInt32(uid);

            foreach (DictionaryEntry e0 in m_sl_schedule_rec)
            {
                SortedList sl_batches = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_batches)
                {
                    string[] sa = e1.Key.ToString().Split('|');

                    SortedList sl_batch = (SortedList)e1.Value;

                    foreach (DictionaryEntry e2 in sl_batch)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e2.Value;

                        if (sr.schd.id == id)
                        {
                            sr.schd.module = chk.Checked;
                            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;
                            return;
                        }
                    }
                }
            }
        }

        void btn_notes_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn_notes = (ImageButton)sender;

            string key = btn_notes.Attributes[UID];

            display_notes_edit(key);

            js_scroll_to_top();
        }

        void dl_code_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dl_code = (DropDownList)sender;

            string uid = (dl_code.Attributes[UID]);

            int id = Convert.ToInt32(uid);

            foreach (DictionaryEntry e0 in m_sl_schedule_rec)
            {
                SortedList sl_batches = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_batches)
                {
                    string[] sa = e1.Key.ToString().Split('|');

                    SortedList sl_batch = (SortedList)e1.Value;

                    foreach (DictionaryEntry e2 in sl_batch)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e2.Value;

                        if (sr.schd.id == id)
                        {
                            if (sr.schd.e_code != dl_code.Text)
                            {
                                display_confirm_status_change(sr, dl_code.ID, dl_code.Text);
                                return;
                            }
                        }
                    }
                }
            }
        }

        ArrayList get_e_code_email_addrs()
        {
            ArrayList a = new ArrayList();

            change_confirm_data ccd = (change_confirm_data)ViewState[VS_CHANGE_CONFIRM_DATA];

            using (settings setting = new settings())
            {
                settings_data sd = setting.get_settings_data();
                const char SC = ';';

                if (ccd.new_e_code == "E1" || ccd.new_e_code == "E2")
                {
                    string[] sa = sd.gbe_barcode_team_email.Split(SC);

                    foreach (string s in sa)
                    {
                        if(s.Trim().Length > 0)
                            a.Add(s);
                    }

                    sa = sd.imsl_email.Split(SC);

                    foreach (string s in sa)
                    {
                        if (s.Trim().Length > 0)
                            a.Add(s);
                    }
                }

                SortedList sl = new SortedList();

                sl.Clear();

                string contract_number = string.Empty;

                try
                {
                    contract_number = ccd.sr.barcode.Split('-')[0];
                }
                catch { }

                if (contract_number.Trim().Length > 0)
                {
                    sl.Add("contract_number", contract_number);

                    ArrayList acd = new ArrayList();

                    using (customers cust = new customers())
                    {
                        acd = cust.get_customer_data(sl);

                        if (acd.Count > 0)
                        {
                            customer_data cd = (customer_data)acd[acd.Count - 1];

                            if (cd.email.Trim().Length > 0)
                                a.Add(cd.email);
                        }
                    }
                }
            }

            return a;
        }

        void display_confirm_status_change(c_schedule_rec sr, string dl_code_id, string new_e_code)
        {
            HiddenField h = (HiddenField)Master.FindControl("hdnScroll");

            change_confirm_data ccd = new change_confirm_data();

            if (h.Value.Contains("|"))
            {
                ccd.scroll_left = h.Value.Split('|')[0];
                ccd.scroll_top = h.Value.Split('|')[1];
            }

            ccd.dl_code_id = dl_code_id;
            ccd.new_e_code = new_e_code;
            ccd.sr = sr;

            //btnView.Visible = false;

            MultiView1.ActiveViewIndex = 3;

            lblPrevCode.Text = sr.schd.e_code + " - " + m_sl_codes[sr.schd.e_code];
            lblNewCode.Text = new_e_code + " - " + m_sl_codes[new_e_code];

            ViewState[VS_CHANGE_CONFIRM_DATA] = ccd;

            ArrayList a_e_addr = get_e_code_email_addrs();

            txtEmailAddr.Text = string.Empty;

            foreach (string s_e_addr in a_e_addr)
                txtEmailAddr.Text += s_e_addr + "\r\n";

            js_scroll_to_top();
        }

        void display_notes_edit(string key)
        {
            HiddenField h = (HiddenField)Master.FindControl("hdnScroll");

            notes_edit_data ned = new notes_edit_data();

            if (h.Value.Contains("|"))
            {
                ned.scroll_left = h.Value.Split('|')[0];
                ned.scroll_top = h.Value.Split('|')[1];
            }

            ned.key = key;

            //btnView.Visible = false;
            
            string[] sa = key.Split('_');
            
            lblDate.Text = sa[0];
            lblTime.Text = sa[1];
            lblVehicle.Text = sa[2];

            txtNote.Text = string.Empty;

            if (m_sl_notes != null)
                if (m_sl_notes.ContainsKey(key))
                    txtNote.Text = m_sl_notes[key].ToString();

            ViewState[VS_NOTES_EDIT_DATA] = ned;

            js_scroll_to_top();

            MultiView1.ActiveViewIndex = 4;
            txtNote.Focus();
        }

        protected void returnFromNoteEdit()
        {
            //btnView.Visible = true;

            notes_edit_data ned = (notes_edit_data)ViewState[VS_NOTES_EDIT_DATA];
            MultiView1.ActiveViewIndex = 0;

            js_scroll(ned.scroll_left, ned.scroll_top);
        }

        void js_scroll_to_top()
        {
            Page.RegisterClientScriptBlock("scroll_to_top",
                  @"<script> 
                          function ScrollView()
                          {
                             scroll(0,0);
                          }
                          window.onload = ScrollView;
                          </script>");
            
        }

        ArrayList get_available_slots(string sdate)
        {
            ArrayList a = new ArrayList();

            schedule_fab sf = new schedule_fab();

            int allocated_slots = sf.get_allocated_slots(sdate);

            int available_slots_prod = schedule.PROD_SLOTS - sf.get_allocated_slots(sdate);

            if (available_slots_prod < 0)
                available_slots_prod = 0;

            int available_slots_extra = schedule.EXTRA_SLOTS - sf.get_extra_allocated_slots(sdate);

            if (available_slots_extra < 0)
                available_slots_extra = 0;

            a.Add(available_slots_prod);
            a.Add(available_slots_extra);

            return a;
        }

        void set_view_btn_text()
        {
            btnView.Text = "Switch to ";

            if (MultiView1.ActiveViewIndex == 0)
            {
                if (m_curr_view == LIST_VIEW)
                {
                    btnView.Text += CALENDER_VIEW;
                }
                else
                {
                    btnView.Text += LIST_VIEW;
                }
            }
            else if (MultiView1.ActiveViewIndex == 1)
            {
                btnView.Text += CALENDER_VIEW;
            }
            else if (MultiView1.ActiveViewIndex == 2)
            {
                btnView.Text += CALENDER_VIEW;
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            if (MultiView1.ActiveViewIndex == 0)
            {
                if (m_curr_view == LIST_VIEW)
                {
                    display_calender();
                }
                else
                {
                    display_list();
                }
            }
            else if (MultiView1.ActiveViewIndex == 1)
            {
                DateTime dt_from = (DateTime)ViewState[VS_DATE_FROM];
                string tbl = ViewState[VS_TBL].ToString();
                get_schedule_recs(dt_from, dt_from.AddMonths(2), tbl);

                display_calender();
            }
            else if (MultiView1.ActiveViewIndex == 2)
            {
                display_calender();
            }

            set_view_btn_text();
        }

        protected void btnAddSpool_Click(object sender, EventArgs e)
        {
            add_spool();
            txtBarcode.Focus();
        }

        void add_spool()
        {
            SortedList sl = new SortedList();

            sl.Add("barcode", txtBarcode.Text.Trim());

            using (spools spls = new spools())
            {
                ArrayList a = spls.get_spool_data_ex(sl);

                if (a.Count > 0)
                {
                    DateTime dt = (DateTime)ViewState[K_DATE];

                    spool_data sd = (spool_data)a[0];

                    sl.Clear();
                    sl.Add("spool_id", sd.id);

                    using (schedule_fab schd = new schedule_fab())
                    {
                        ArrayList a_schd = schd.get_schedule_fab_data(sl);

                        if (a_schd.Count > 0)
                        {
                            foreach (schedule_fab_data sfd in a_schd)
                            {
                                if (sfd.dt >= dt)
                                {
                                    lblMsg.Text = "Spool already in fab. schedule on " + dt.DayOfWeek.ToString() + "\x20" + dt.ToLongDateString();
                                    return;
                                }
                            }
                        }
                    }

                    string sdate = null;
                    string stime = null;

                    /*
                    if (!validate_date(txtDeliveryDate))
                    {
                        return;
                    }
                    else
                        sdate = txtDeliveryDate.Text.Trim();

                    if (!validate_time(txtDeliveryTime))
                    {
                        return;
                    }
                    else
                        stime = txtDeliveryTime.Text.Trim();
                    */

                    using (schedule_delivery schd = new schedule_delivery())
                    {
                        ArrayList a_schd = schd.get_schedule_delivery_data(sl);

                        if (a_schd.Count > 0)
                        {
                            foreach (schedule_fab_data sfd in a_schd)
                            {
                                if (sfd.dt >= dt)
                                {
                                    lblMsg.Text = "Spool already in delivery schedule on " + dt.DayOfWeek.ToString() + "\x20" + dt.ToLongDateString();
                                    return;
                                }
                            }
                        }
                    }

                    int sched_slots = 1;

                    foreach (spool_part_data spd in sd.spool_part_data)
                    {
                        int slots = schedule_fab.get_slots(spd.part_data);

                        if (slots > sched_slots)
                            sched_slots = slots;
                    }

                    schedule_fab sfe = new schedule_fab();

                    int allocated_slots = sfe.get_extra_allocated_slots(dt.ToString("dd/MM/yyyy"));

                    if ((allocated_slots + sched_slots) > schedule.EXTRA_SLOTS)
                    {
                        lblMsg.Text = "Error. " + (schedule.EXTRA_SLOTS - allocated_slots).ToString() + " slot(s) available for the selected date";
                    }
                    else
                    {
                        using (schedule_fab schd = new schedule_fab())
                        {
                            using (schedule_delivery schd_deliv = new schedule_delivery())
                            {
                                SortedList sl_schd = new SortedList();
                                sl_schd.Add("spool_id", sd.id);
                                sl_schd.Add("dt", dt);
                                sl_schd.Add("slots", sched_slots);
                                sl_schd.Add("batch_number", 0);

                                schd.save_schedule_fab_data(sl_schd);

                                string vehicle = dlDeliveryVehicle.Text.Trim();

                                sl_schd.Clear();

                                DateTime dt_deliv = DateTime.ParseExact(schedule_fab_data.INIT_DATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                                sl_schd.Add("spool_id", sd.id);
                                sl_schd.Add("dt", dt_deliv);
                                sl_schd.Add("vehicle", "TBC");
                                sl_schd.Add("batch_number", 0);

                                schd_deliv.save_schedule_delivery_data(sl_schd);

                                lblMsg.Text = txtBarcode.Text + " added";

                                txtBarcode.Text = txtDeliveryDate.Text = txtDeliveryTime.Text = dlDeliveryVehicle.Text = string.Empty;
                                txtBarcode.Focus();
                            }
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Spool not found";
                }
            }
        }

        protected void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox) sender;
            txtDate.Enabled = chk.Checked;
            chkQuarantine.Checked = false;

            if (txtDate.Enabled)
                txtDate.Focus();
            else
                txtDate.Text = string.Empty;

            ena_save_button();
        }

        protected void chkTime_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            txtTime.Enabled = chk.Checked;

            if (txtTime.Enabled)
                txtTime.Focus();
            else
                txtTime.Text = string.Empty;

            ena_save_button();
        }

        protected void chkVehicle_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dlVehicle.Enabled = chk.Checked;

            if (dlVehicle.Enabled)
                dlVehicle.Focus();
            else
                dlVehicle.Text = string.Empty;

            ena_save_button();
        }

        void ena_save_button()
        {
            btnSave.Enabled = (chkTime.Checked || chkDate.Checked || chkVehicle.Checked || chkAddToExtras.Checked || chkQuarantine.Checked) && (tblRecs.Rows.Count>0);
        }

        bool is_valid_date(string date)
        {
            bool bret = false;

            DateTime dt;

            bret = DateTime.TryParseExact(date, "dd/MM/yyyy",
              System.Globalization.CultureInfo.InvariantCulture,
              System.Globalization.DateTimeStyles.None, out dt);

            if (bret)
            {
                bret = dt >= DateTime.Now.AddMonths(-1) && dt < DateTime.Now.AddMonths(2);
            }

            return bret;
        }

        bool is_valid_time(string time)
        {
            DateTime dt;

            return DateTime.TryParseExact(time, "HH:mm",
              System.Globalization.CultureInfo.InvariantCulture,
              System.Globalization.DateTimeStyles.None, out dt);
        }

        bool is_holding_area()
        {
            string holding_area = string.Empty;
            bool bholding_area = false;

            holding_area = Request.QueryString["h"];

            if (holding_area != null)
                if (holding_area == "1")
                    bholding_area = true;

            return bholding_area;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (chkTime.Checked || chkDate.Checked || chkVehicle.Checked || chkQuarantine.Checked)
            {
                string confirm_value = Request.Form["confirm_save_value"];

                if (confirm_value == "Yes")
                {
                    string sdate = null;
                    string stime = null;

                    if (chkDate.Checked)
                    {
                        if (!validate_date(txtDate))
                        {
                            return;
                        }
                        else
                        {
                            sdate = txtDate.Text.Trim();

                            if (is_fab_schedule() && schedule_fab.is_date_locked(sdate))
                            {
                                lblMsg.Text = "This fabrication date is locked";
                                return;
                            }
                        }
                    }

                    if (chkTime.Checked)
                    {
                        if (is_delivery_schedule())
                        {
                            if (!validate_time(txtTime))
                            {
                                return;
                            }
                            else
                                stime = txtTime.Text.Trim();
                        }
                        else
                            sdate = "00:00";
                    }

                    string vehicle = null;

                    if (chkVehicle.Checked)
                    {
                        vehicle = dlVehicle.Text.Trim();
                    }

                    ArrayList a_quarantined_tbl_idx = new ArrayList();
                    int idx_to_remove = 0;

                    foreach (TableRow r in tblRecs.Rows)
                    {
                        string uid = string.Empty;

                        try {uid = r.Attributes[UID]; }
                        catch { }

                        if (uid != null)
                        {
                            if (uid.Trim().Length > 0)
                            {
                                CheckBox chk = get_tbl_row_checkbox(r, CHK+uid);

                                if (chk.Checked)
                                {
                                    string barcode = r.Attributes[BARCODE];
                                    string sdt = r.Attributes[DATE];
                                    string sbatch = r.Attributes[BATCH];

                                    c_schedule_rec sr = get_update_record(barcode, sdt, sbatch);

                                    if (sdate != null && stime == null)
                                        stime = sr.schd.dt.ToString("HH:mm");

                                    if (sdate == null && stime != null)
                                        sdate = sr.schd.dt.ToString("dd/MM/yyyy");

                                    if (chkQuarantine.Checked)
                                    {
                                        sdate = schedule_data.DT_QUARANTINE_RECS_DATE.ToString("dd/MM/yyyy");
                                        stime = ("00:00");

                                        a_quarantined_tbl_idx.Add(idx_to_remove);
                                    }

                                    update_record(uid, sr, sdate, stime, vehicle, chkAddToExtras.Checked);

                                    TextBox txtbox = null;

                                    if (chkDate.Checked)
                                    {
                                        txtbox = get_tbl_row_textbox(r, TXT_DATE + uid);
                                        txtbox.Text = txtDate.Text;
                                    }

                                    if (is_delivery_schedule())
                                    {
                                        if (chkTime.Checked)
                                        {
                                            txtbox = get_tbl_row_textbox(r, TXT_TIME + uid);
                                            txtbox.Text = txtTime.Text;
                                        }
                                    }

                                    if (chkVehicle.Checked)
                                    {
                                        DropDownList dl_veh = get_tbl_row_dropdownlist(r, DL_VEH + uid);
                                        dl_veh.Text = vehicle;
                                    }
                                }
                            }
                        }

                        idx_to_remove++;
                    }

                    a_quarantined_tbl_idx.Reverse();

                    foreach (int idx in a_quarantined_tbl_idx)
                    {
                        tblRecs.Rows.RemoveAt(idx);
                    }

                    if (tblRecs.Rows.Count == 1)
                        tblRecs.Rows.Clear();

                    chkDate.Checked = chkTime.Checked = chkVehicle.Checked = chkAddToExtras.Checked = chkQuarantine.Checked = false;

                    txtDate.Text = txtTime.Text = dlVehicle.Text = string.Empty;

                    chkDate_CheckedChanged(chkDate, null);
                    chkTime_CheckedChanged(chkTime, null);
                    chkVehicle_CheckedChanged(chkVehicle, null);

                    DateTime dt_from = (DateTime)ViewState[VS_DATE_FROM];
                    
                    string tbl = ViewState[VS_TBL].ToString();

                    get_schedule_recs(dt_from, DateTime.MaxValue, tbl);
                }
            }
        }

        bool validate_date(TextBox txtbox)
        {
            bool bret = true;

            if (!is_valid_date(txtbox.Text.Trim()))
            {
                txtbox.Text = INVALID_DATE;
                bret = false;
            }

            return bret;
        }

        bool validate_time(TextBox txtbox)
        {
            bool bret = true;

            if (!is_valid_time(txtbox.Text.Trim()))
            {
                txtbox.Text = INVALID_TIME;
                bret = false;
            }

            return bret;
        }

        void btn_save_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn_save = (ImageButton)sender;

            string uid = btn_save.Attributes[UID];
            string barcode = btn_save.Attributes[BARCODE];
            string sdt = btn_save.Attributes[DATE];
            string sbatch = btn_save.Attributes[BATCH];

            TableRow tbl_row = get_table_row(uid);

            TextBox txt_date = get_tbl_row_textbox(uid, TXT_DATE + uid);
            TextBox txt_time = get_tbl_row_textbox(uid, TXT_TIME + uid);

            string sdate = string.Empty;
            string stime = "00:00";

            if (!validate_date(txt_date))
            {
                return;
            }
            else
            {
                sdate = txt_date.Text.Trim();
                
                if (is_fab_schedule() && schedule_fab.is_date_locked(sdate))
                {
                    lblMsg.Text = "This fabrication date is locked";
                    return;
                }
            }
            if (is_delivery_schedule())
            {
                if (!validate_time(txt_time))
                {
                    return;
                }
                else
                    stime = txt_time.Text.Trim();
            }

            string vehicle = string.Empty;

            if (is_delivery_schedule())
            {
                DropDownList dl_veh = get_tbl_row_dropdownlist(uid, DL_VEH + uid);
                vehicle = dl_veh.Text.Trim();
            }

            c_schedule_rec sr = get_update_record(barcode, sdt, sbatch);

            update_record(uid, sr, sdate, stime, vehicle, false);

            DateTime dt_from = (DateTime)ViewState[VS_DATE_FROM];

            string tbl = ViewState[VS_TBL].ToString();

            get_schedule_recs(dt_from, DateTime.MaxValue, tbl);
        }

        AttributeCollection get_table_row_attributes(string uid)
        {
            AttributeCollection ac = null;

            TableRow r = get_table_row(uid);

            if (r != null)
                ac = r.Attributes;

            return ac;
        }

        TableRow get_table_row(string uid)
        {
            TableRow tbl_row = null;

            foreach (TableRow r in tblRecs.Rows)
            {
                string ruid = r.Attributes[UID];

                if (ruid == uid)
                    tbl_row = r;
            }

            return tbl_row;
        }

        c_schedule_rec get_update_record(string barcode, string sdt, string sbatch)
        {
            c_schedule_rec sr = null;

            if (m_sl_schedule_rec.ContainsKey(sdt))
            {
                SortedList sl_batches = (SortedList)m_sl_schedule_rec[sdt];

                if (sl_batches.ContainsKey(sbatch))
                {
                    SortedList sl_batch = (SortedList)sl_batches[sbatch];

                    foreach (DictionaryEntry e0 in sl_batch)
                    {
                        if (e0.Key.ToString().ToUpper().Contains(barcode))
                        {
                            sr = (c_schedule_rec)e0.Value;
                            break;
                        }
                    }
                }
            }

            return sr;
        }

        TextBox get_tbl_row_textbox(string uid, string textbox_id)
        {
            return (TextBox)get_tbl_row_control(uid, textbox_id);
        }

        TextBox get_tbl_row_textbox(TableRow r, string textbox_id)
        {
            return (TextBox)get_tbl_row_control(r, textbox_id);
        }

        DropDownList get_tbl_row_dropdownlist(string uid, string dropdownlist_id)
        {
            return (DropDownList)get_tbl_row_control(uid, dropdownlist_id);
        }

        DropDownList get_tbl_row_dropdownlist(TableRow r, string dropdownlist_id)
        {
            return (DropDownList)get_tbl_row_control(r, dropdownlist_id);
        }

        CheckBox get_tbl_row_checkbox(TableRow r, string checkbox_id)
        {
            return (CheckBox)get_tbl_row_control(r, checkbox_id);
        }

        Control get_tbl_row_control(string uid, string control_id)
        {
            Control control = null;

            TableRow r = get_table_row(uid);

            control = get_tbl_row_control(r, control_id);
            
            return control;
        }

        Control get_tbl_row_control(TableRow r, string control_id)
        {
            Control control = null;

            if (r != null)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID == control_id)
                        {
                            control = cntrl;
                            break;
                        }
                    }

                    if (control != null)
                        break;
                }
            }

            return control;
        }

        void update_record(string uid, c_schedule_rec sr, string sdate, string stime, string vehicle, bool badd_to_extras)
        {
            SortedList sl = new SortedList();

            sl.Add("id", sr.schd.id);

            if (sdate != null)
            {
                DateTime dt = DateTime.ParseExact(sdate + "\x20" + stime, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture); ;
                sl.Add("dt", dt);
            }

            if (vehicle != null)
            {
                if (is_delivery_schedule())
                {
                    sl.Add("vehicle", vehicle);
                }
            }

            if (badd_to_extras)
                sl.Add("batch_number", 0);

            if (is_delivery_schedule())
            {
                using (schedule_delivery schdd = new schedule_delivery())
                {
                    schdd.save_schedule_delivery_data(sl);
                }
            }
            else if (is_fab_schedule())
            {
                using (schedule_fab schdf = new schedule_fab())
                {
                    schdf.save_schedule_fab_data(sl);
                }
            }
        }

        protected void btn_hs_upd_Click(object sender, EventArgs e)
        {
            string select = "select schedule_fab.*, spools.id as spools_ID, spools.barcode  from schedule_fab ";
            select += " inner join spools on spool_id = spools.id ";
            select += " order by  schedule_fab.dt, spools.barcode";

            using (cdb_connection dbc = new cdb_connection())
            {

                DataTable dtab = dbc.get_data(select);

                SortedList sl_upd = new SortedList();

                foreach (DataRow dr in dtab.Rows)
                {
                    c_schedule_rec sr = new c_schedule_fab_rec();

                    schedule_data schd = schedule_fab.get_schedule_fab_data(dr);
                    sr.schd = schd;

                    try { sr.barcode = dr["barcode"].ToString(); }
                    catch { }

                    string k = sr.schd.dt.ToString("yyyyMMdd") + sr.barcode.Split('-')[0];

                    ArrayList a = null;

                    if (!sl_upd.ContainsKey(k))
                    {
                        a = new ArrayList();
                        sl_upd.Add(k, a);
                    }
                    {
                        a = (ArrayList)sl_upd[k];
                    }

                    a.Add(sr.schd.id);
                }

                using (schedule_fab sf = new schedule_fab())
                {
                    using (settings setting = new settings())
                    {
                        settings_data sd = setting.get_settings_data();

                        int batch_number = sd.schedule_batch_number;
                        
                        foreach (DictionaryEntry e0 in sl_upd)
                        {
                            ArrayList a = (ArrayList)e0.Value;

                            if(a.Count > 0)
                            {
                                batch_number++;
                                
                                if (batch_number > 99)
                                    batch_number = 1;

                                foreach (int id in a)
                                {
                                    SortedList sl_sf = new SortedList();

                                    sl_sf.Add("id", id);
                                    sl_sf.Add("batch_number", batch_number);

                                    sf.save_schedule_fab_data(sl_sf);
                                }
                            }
                        }

                        SortedList sl_s = new SortedList();
                        sl_s.Add("id", sd.id);
                        sl_s.Add("schedule_batch_number", batch_number);
                        setting.save_settings_data(sl_s);
                    }
                }
            }
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            display_list();
        }

        protected void btnCancelCodeChange_Click(object sender, EventArgs e)
        {
            change_confirm_data ccd = (change_confirm_data)ViewState[VS_CHANGE_CONFIRM_DATA];

            bool bdone = false;

            foreach (TableRow row in tblSchedule.Rows)
            {
                if (bdone)
                    break;

                foreach (TableCell cell in row.Cells)
                {
                    if (bdone)
                        break;

                    foreach (Control c in cell.Controls)
                    {
                        if (bdone)
                            break;

                        if (c.ID == ccd.dl_code_id)
                        {
                            ((DropDownList)c).Text = ccd.sr.schd.e_code;
                            bdone = true;
                        }
                    }
                }
            }

            returnFromCodeChange();
        }

        protected void returnFromCodeChange()
        {
            //btnView.Visible = true;

            change_confirm_data ccd = (change_confirm_data)ViewState[VS_CHANGE_CONFIRM_DATA];
            MultiView1.ActiveViewIndex = 0;

            js_scroll(ccd.scroll_left, ccd.scroll_top);
        }

        void js_scroll(string left, string top)
        {
            Page.RegisterClientScriptBlock("scroll",
                  @"<script> 
                          function ScrollView()
                          {
                             scroll(" + left + " ," + top + @");
                          }
                          window.onload = ScrollView;
                          </script>");
        }

        protected void btnConfirmCodeChange_Click(object sender, EventArgs e)
        {
            change_confirm_data ccd = (change_confirm_data)ViewState[VS_CHANGE_CONFIRM_DATA];

            SortedList sl = new SortedList();

            sl.Add("id", ccd.sr.schd.id);
            sl.Add("e_code", ccd.new_e_code);

            using (schedule_fab schdf = new schedule_fab())
            {
                schdf.save_schedule_fab_data(sl);
            }

            int id = Convert.ToInt32(ccd.sr.schd.id);

            foreach (DictionaryEntry e0 in m_sl_schedule_rec)
            {
                SortedList sl_batches = (SortedList)e0.Value;

                foreach (DictionaryEntry e1 in sl_batches)
                {
                    string[] sa = e1.Key.ToString().Split('|');

                    SortedList sl_batch = (SortedList)e1.Value;

                    foreach (DictionaryEntry e2 in sl_batch)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e2.Value;

                        if (sr.schd.id == id)
                        {
                            sr.schd.e_code = ccd.new_e_code;
                            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;
                        }
                    }
                }
            }

            string transaction_file_dir = string.Empty;

            try { transaction_file_dir = System.Web.Configuration.WebConfigurationManager.AppSettings["transaction_file_dir"].ToString(); }
            catch { transaction_file_dir = string.Empty; }

            if (transaction_file_dir.Trim().Length > 0)
            {
                if (Directory.Exists(transaction_file_dir))
                {
                    DateTime dt = DateTime.Now;
            
                    string tf = transaction_file_dir;
                    tf += "\\e_code_change_";
                    tf += dt.ToString("yyyyMMddHHmmssfff");
                    tf += ccd.sr.schd.id.ToString();
                    tf += ".csv";

                    const string CM = ",";
                    const string SC = ";";
                    const string CRLF_SUB = "¬";

                    string strans = "E_CODE_CHANGE_02";
                    strans += CM;

                    string [] sa_e_addr = txtEmailAddr.Text.Split('\n');

                    string email_recip = string.Empty;

                    foreach (string s_e_addr in sa_e_addr)
                    {
                        string s = s_e_addr.Trim(new char[] { '\r', '\x20' });

                        if (s.Length > 0)
                            email_recip += s + SC;
                    }

                    if (email_recip.Length > 0)
                    {
                        strans += email_recip ;
                    }

                    strans += CM;

                    string msg = string.Empty;
                    msg += CRLF_SUB + "Spool:" + CRLF_SUB;
                    msg += ccd.sr.barcode + CRLF_SUB;

                    if(ccd.new_e_code == "00")
                    {
                        strans += "GBE Fabrication Resumed" + CM;
                    }
                    else
                    {
                        strans += "GBE Fabrication Paused" + CM;

                        msg += CRLF_SUB + "Reason:" + CRLF_SUB;
                        msg += m_sl_codes[ccd.new_e_code].ToString();
                    }

                    strans += msg;

                    File.WriteAllText(tf, strans, System.Text.Encoding.Default);
                }
            }

            returnFromCodeChange();
        }

        protected void btnCancelNote_Click(object sender, EventArgs e)
        {
            returnFromNoteEdit();
        }

        protected void btnOKNote_Click(object sender, EventArgs e)
        {
            saveNote();
            returnFromNoteEdit();
        }

        void saveNote()
        {
            notes_edit_data ned = (notes_edit_data)ViewState[VS_NOTES_EDIT_DATA];

            try { m_sl_notes = (SortedList)ViewState[VS_NOTES]; }
            catch { }

            if (m_sl_notes == null)
                m_sl_notes = new SortedList();

            string note = txtNote.Text.Trim().PadRight(100).Substring(0,100).Trim();

            if (m_sl_notes.ContainsKey(ned.key))
                m_sl_notes[ned.key] = note;
            else
                m_sl_notes.Add(ned.key, note);

            bool bdone = false;

            foreach (TableRow r in tblSchedule.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID == "lc_note_" + ned.key)
                        {
                            ((LiteralControl)cntrl).Text = note;

                            using(delivery_schedule_notes dsn = new delivery_schedule_notes())
                            {
                                SortedList sl = new SortedList();
                                int id = 0;

                                if (m_sl_notes_by_id != null)
                                    if (m_sl_notes_by_id.ContainsKey(ned.key))
                                        id = (int)m_sl_notes_by_id[ned.key];

                                if (id > 0)
                                    sl.Add("id", id);

                                sl.Add("note_key", ned.key);
                                sl.Add("note", note);
                                sl.Add("dt", DateTime.Now);

                                dsn.save_delivery_schedule_notes_data(sl);

                                if (sl.ContainsKey("id"))
                                {
                                    if (m_sl_notes_by_id == null)
                                        m_sl_notes_by_id = new SortedList();

                                    if (!m_sl_notes_by_id.ContainsKey(ned.key))
                                        m_sl_notes_by_id.Add(ned.key, id);
                                }
                            }

                            bdone = true;

                            break;
                        }
                    }

                    if (bdone)
                        break;
                }
                if (bdone)
                    break;
            }

            ViewState[VS_NOTES] = m_sl_notes;
            ViewState[VS_NOTES_BY_ID] = m_sl_notes_by_id;
        }

        [Serializable]
        protected class change_confirm_data
        {
            public string scroll_top = string.Empty;
            public string scroll_left = string.Empty;
            public string dl_code_id = string.Empty;
            public string new_e_code = string.Empty;
            public c_schedule_rec sr = null;
        }

        [Serializable]
        protected class notes_edit_data
        {
            public string scroll_top = string.Empty;
            public string scroll_left = string.Empty;
            public string key = string.Empty;
            public string notes = string.Empty;
        }

        protected void chkAddToExtras_CheckedChanged(object sender, EventArgs e)
        {
            chkQuarantine.Checked = false;
            ena_save_button();
        }

        protected void chkQuarantine_CheckedChanged(object sender, EventArgs e)
        {
            chkDate.Checked = chkAddToExtras.Checked = false;
            txtDate.Text = string.Empty;
            txtDate.Enabled = false;
            
            ena_save_button();
        }

        protected void imgReSeq_Click(object sender, ImageClickEventArgs e)
        {
            txtSeq.Text = txtReseqSpools.Text = string.Empty;
            
            chkQuarantineSpools.Checked = is_delivery_schedule();
            
            ArrayList a_chk = get_req_seq_checked_chkboxes();
            
            if (a_chk.Count > 0)
            {
                foreach (CheckBox chk in a_chk)
                {
                    string k = chk.Attributes[UID];
                    string barcode = chk.Attributes[BARCODE];

                    txtReseqSpools.Text += barcode + "\n";
                }

                if (txtReseqSpools.Text.Trim().Length > 0)
                {
                    txtSeq.Focus();
                    MultiView1.ActiveViewIndex = 5;
                }
            }
        }

        protected void btnReSeqCancel_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
        }

        protected void btnReSeqOK_Click(object sender, EventArgs e)
        {
            if (chkQuarantineSpools.Checked)
            {
                quarantine();
            }
            else
            {
                re_seq();
            }
        }

        void re_seq()
        {
            if (txtSeq.Text.Trim().Length > 0)
            {
                int seq = 0;

                try { seq = Convert.ToInt32(txtSeq.Text.Trim()); }
                catch { }

                if (seq > 0)
                {
                    if (m_sl_schedule_rec.Count > 0)
                    {
                        ArrayList a_chk = get_req_seq_checked_chkboxes();

                        ArrayList a_keys_to_reseq = new ArrayList();

                        foreach (CheckBox chk in a_chk)
                            a_keys_to_reseq.Add(chk.Attributes[UID]);

                        SortedList sl0 = (SortedList)m_sl_schedule_rec.GetByIndex(0);
                        SortedList sl_new = new SortedList();

                        foreach (DictionaryEntry e0 in sl0)
                        {
                            if (!a_keys_to_reseq.Contains(e0.Key.ToString()))
                            {
                                string[] sa = e0.Key.ToString().Split('~');

                                if (sa.Length == 2)
                                {
                                    if (seq == sl_new.Count + 1)
                                    {
                                        foreach (string k in a_keys_to_reseq)
                                        {
                                            string[] sa2 = k.Split('~');

                                            sl_new.Add((sl_new.Count + 1).ToString(SEQ_FMT) + "~" + sa2[1], sl0[k]);
                                        }
                                    }

                                    sl_new.Add((sl_new.Count + 1).ToString(SEQ_FMT) + "~" + sa[1], e0.Value);
                                }
                            }
                        }

                        if (seq > sl_new.Count)
                        {
                            foreach (string k in a_keys_to_reseq)
                            {
                                string[] sa2 = k.Split('~');

                                sl_new.Add((sl_new.Count + 1).ToString(SEQ_FMT) + "~" + sa2[1], sl0[k]);
                            }
                        }

                        SortedList sl = new SortedList();
                        SortedList sl_upd = new SortedList();

                        using (schedule_fab schdf = new schedule_fab())
                        {
                            int i = 0;
                            foreach (DictionaryEntry e0 in sl_new)
                            {
                                i++;
                                sl = (SortedList)e0.Value;

                                c_schedule_rec sr = (c_schedule_rec)sl.GetByIndex(0);
                                sr.schd.seq = i;

                                sl_upd.Clear();
                                sl_upd.Add("id", sr.schd.id);
                                sl_upd.Add("seq", sr.schd.seq);

                                schdf.save_schedule_fab_data(sl_upd);
                            }
                        }

                        m_sl_schedule_rec["FAB"] = sl_new;

                        foreach (CheckBox chk in a_chk)
                            chk.Checked = false;

                        ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;

                        display_list();
                    }

                    MultiView1.ActiveViewIndex = 0;
                }
                else
                {
                    lblMsg.Text = "Enter a number greater than 0.";
                    txtSeq.Focus();
                }
            }
            else
                txtSeq.Focus();
        }

        void quarantine()
        {
            ArrayList a_chk = get_req_seq_checked_chkboxes();

            ArrayList a_keys_to_quarantine = new ArrayList();
            ArrayList a_barcodes_to_quarantine = new ArrayList();
            ArrayList keys_to_delete = new ArrayList();
            SortedList sl0, sl1;

            sl0 = sl1 = null;

            foreach (CheckBox chk in a_chk)
                a_keys_to_quarantine.Add(chk.Attributes[UID]);

            foreach (CheckBox chk in a_chk)
                a_barcodes_to_quarantine.Add(chk.Attributes[BARCODE]);

            using (schedule_fab schdf = new schedule_fab())
            {
                using (schedule_delivery schdd = new schedule_delivery())
                {
                    foreach (DictionaryEntry e0 in m_sl_schedule_rec)
                    {
                        sl0 = (SortedList)e0.Value;

                        foreach (string k0 in a_keys_to_quarantine)
                        {
                            if (sl0.ContainsKey(k0))
                            {
                                sl1 = (SortedList)sl0[k0];

                                foreach (DictionaryEntry e1 in sl1)
                                {
                                    c_schedule_rec sr = (c_schedule_rec)e1.Value;

                                    if (a_barcodes_to_quarantine.Contains(sr.barcode))
                                    {
                                        SortedList sl = new SortedList();

                                        sl.Add("id", sr.schd.id);
                                        sl.Add("dt", schedule_fab_data.DT_QUARANTINE_RECS_DATE);

                                        if (is_fab_schedule())
                                        {
                                            sl.Add("seq", int.MaxValue);
                                            schdf.save_schedule_fab_data(sl);
                                        }
                                        else
                                        {
                                            schdd.save_schedule_delivery_data(sl);
                                        }

                                        keys_to_delete.Add(e1.Key);
                                    }
                                }

                                foreach (string k1 in keys_to_delete)
                                {
                                    if (sl1 != null)
                                        if (sl1.ContainsKey(k1))
                                            sl1.Remove(k1);
                                }
                            }
                        }

                        delete_empty_sl(sl0);
                    }
                }
            }

            delete_empty_sl(m_sl_schedule_rec);

            if (is_fab_schedule())
            {
                if (m_sl_schedule_rec.Count > 0)
                {
                    SortedList sl_upd = new SortedList();

                    sl0 = (SortedList)m_sl_schedule_rec.GetByIndex(0);

                    using (schedule_fab schdf = new schedule_fab())
                    {
                        int i = 0;
                        foreach (DictionaryEntry e0 in sl0)
                        {
                            i++;
                            SortedList sl = (SortedList)e0.Value;

                            c_schedule_rec sr = (c_schedule_rec)sl.GetByIndex(0);
                            sr.schd.seq = i;

                            sl_upd.Clear();
                            sl_upd.Add("id", sr.schd.id);
                            sl_upd.Add("seq", sr.schd.seq);

                            schdf.save_schedule_fab_data(sl_upd);
                        }
                    }
                }
            }
            else if (is_delivery_schedule())
            {
                foreach (DictionaryEntry e0 in m_sl_dt_time_schedule_rec)
                {
                    sl0 = (SortedList)e0.Value;

                    foreach (string k0 in a_keys_to_quarantine)
                    {
                        if (sl0.ContainsKey(k0))
                        {
                            sl1 = (SortedList)sl0[k0];

                            foreach (DictionaryEntry e1 in sl1)
                            {
                                c_schedule_rec sr = (c_schedule_rec)e1.Value;

                                if (a_barcodes_to_quarantine.Contains(sr.barcode))
                                {
                                    keys_to_delete.Add(e1.Key);
                                }
                            }

                            foreach (string k1 in keys_to_delete)
                            {
                                if (sl1 != null)
                                    if (sl1.ContainsKey(k1))
                                        sl1.Remove(k1);
                            }
                        }
                    }

                    delete_empty_sl(sl0);
                }

                delete_empty_sl(m_sl_dt_time_schedule_rec);

                ViewState[VS_SCHEDULE_RECS_DT_TIME] = m_sl_dt_time_schedule_rec;
            }
            
            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;

            display_list();

            MultiView1.ActiveViewIndex = 0;
        }

        void delete_empty_sl(SortedList sl0)
        {
            ArrayList keys_to_delete = new ArrayList();

            foreach (DictionaryEntry e0 in sl0)
            {
                SortedList sl = (SortedList)e0.Value;

                if (sl.Count == 0)
                    keys_to_delete.Add(e0.Key);
            }

            foreach (string k in keys_to_delete)
            {
                if (sl0 != null)
                    if (sl0.ContainsKey(k))
                        sl0.Remove(k);
            }
        }

        ArrayList get_req_seq_checked_chkboxes()
        {
            ArrayList a = new ArrayList();

            foreach (TableRow r0 in tblSchedule.Rows)
            {
                foreach (TableCell c0 in r0.Cells)
                {
                    foreach (Control cntrl0 in c0.Controls)
                    {
                        if (cntrl0.GetType() == typeof(Panel))
                        {
                            Panel pnl = (Panel)cntrl0;

                            foreach (Control cntrl1 in pnl.Controls)
                            {
                                if (cntrl1.GetType() == typeof(Table))
                                {
                                    Table tbl = (Table)cntrl1;

                                    foreach (TableRow r1 in tbl.Rows)
                                    {
                                        foreach (TableCell cell in r1.Cells)
                                        {
                                            foreach (Control cntrl in cell.Controls)
                                            {
                                                if (cntrl.GetType() == typeof(CheckBox))
                                                {
                                                    CheckBox chk = (CheckBox)cntrl;

                                                    if (chk.ID.StartsWith("chk_sched_tbl_rec_"))
                                                    {
                                                        if (chk.Checked)
                                                            a.Add(chk);
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
            }

            return a;
        }

        protected void dlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime dt_from = (DateTime)ViewState[VS_DATE_FROM];
            DateTime dt_to = dt_from.AddMonths(2);

            if (is_holding_area())
            {
                dt_from = schedule_fab_data.DT_HOLDING_RECS_DATE;
                dt_to = dt_from.AddHours(1);
            }

            string tbl = ViewState[VS_TBL].ToString();
            get_schedule_recs(dt_from, dt_to, tbl);
            display_list();
        }

        protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        {
            btnView.Visible = is_delivery_schedule() && MultiView1.ActiveViewIndex<3;
            set_view_btn_text();
        }

        SortedList get_invoice_amounts()
        {
            SortedList sl_invoice_ammouts = new SortedList();

            SortedList sl_schedule_rec = m_sl_schedule_rec;
            string spool_ids = string.Empty;

            foreach (DictionaryEntry e0 in sl_schedule_rec)
            {
                SortedList sl0 = (SortedList)e0.Value;
                            
                if (sl0.Count > 0)
                {
                    SortedList sl_batches = (SortedList)e0.Value;

                    foreach (DictionaryEntry e in sl_batches)
                    {
                        string[] sa = e.Key.ToString().Split('|');

                        SortedList sl_batch = (SortedList)e.Value;

                        foreach (DictionaryEntry e1 in sl_batch)
                        {
                            c_schedule_rec sr = (c_schedule_rec)e1.Value;

                            if(spool_ids.Length > 0)
                                spool_ids += ",";

                            spool_ids += sr.schd.spool_id;
                        }
                    }
                }
            }

            if (spool_ids.Length > 0)
            {
                spools sp = new spools();

                ArrayList a = sp.get_spool_data(spool_ids);

                if (a.Count > 0)
                {
                    foreach (spool_data sd in a)
                    {
                        if (!sl_invoice_ammouts.ContainsKey(sd.id))
                        {
                            decimal material_amount, fab_amount;
                            material_amount = fab_amount = 0;
                            sl_invoice_ammouts.Add(sd.id, CSpoolInvoiceAmount.get_invoice_amount(sd, ref material_amount, ref fab_amount));
                        }
                    }
                }
            }

            return sl_invoice_ammouts;
        }
    }
}
