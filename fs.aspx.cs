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
    public partial class fs : System.Web.UI.Page
    {
        const string VS_FS_DATA = "VS_FS_DATA";

        protected void Page_Load(object sender, EventArgs e)
        {
            Table tbl = (Table)Master.FindControl("tblMenu");

            tbl.Visible = false;

            tbl = (Table)Master.FindControl("tblMenuStores");

            tbl.Visible = false;

            if(!IsPostBack)
            {
                get_spools();
            }

            display_spools();
        }

        void get_spools()
        {
            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl = new SortedList();

            sl.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl);
            }

            sl.Clear();

            if (a.Count > 0)
            {
                user_data ud = (user_data)a[0];

                if (ud.role == "FITTER")
                {
                    string sql = "select spools.id, spool, revision, date_created, spools.status, on_hold, spool_status.order_of_production, schedule_fab.batch_number, weld_jobs.assigned_on,  weld_jobs.assigned_seq, drawing_id ";
                    sql += " from spools ";
                    sql += " left join schedule_fab on spools.id = schedule_fab.spool_id ";
                    sql += " left join spool_status on spools.status = spool_status.status ";
                    sql += " left join weld_jobs on spools.id = weld_jobs.spool_id ";
                    sql += " where ";
                    sql += " spool_status.order_of_production < 40 ";
                    sql += " and  ";
                    sql += " fitter = " + ud.id;
                    sql += " order by weld_jobs.assigned_on, weld_jobs.assigned_seq ";

                    try
                    {
                        SortedList sl_fsd = new SortedList();

                        using (cdb_connection dbc = new cdb_connection())
                        {
                            DataTable dtab = dbc.get_data(sql);

                            foreach (DataRow dr in dtab.Rows)
                            {
                                CFitterScheduleData fsd = new CFitterScheduleData();
                                fsd.spool_id = (int)dr["id"];
                                fsd.spool = dr["spool"].ToString();
                                fsd.revision = dr["revision"].ToString();
                                try { fsd.date_created = (DateTime)dr["date_created"]; }
                                catch { }
                                fsd.status = dr["status"].ToString();
                                fsd.on_hold = (bool)dr["on_hold"];
                                fsd.batch_number = dr["batch_number"].ToString();

                                DateTime dt_assigned = DateTime.MinValue;

                                int seq = 0;
                                try { seq = (int)dr["assigned_seq"]; }
                                catch { };

                                try { dt_assigned = (DateTime)dr["assigned_on"]; }
                                catch { }
                                
                                try{fsd.drawing_id = (int)dr["drawing_id"];}catch{}

                                fsd.key = dt_assigned.ToString("yyyyMMdd_HHmmssfff") + "_" + seq.ToString("0000") + "_" + fsd.spool_id;

                                sl_fsd.Add(fsd.key, fsd);
                            }
                        }

                        ViewState[VS_FS_DATA] = sl_fsd;
                    }
                    catch (Exception ex)
                    {
                        lblMsg.Text = ex.Message.ToString();
                    }
                }
            }
        }

        void display_spools()
        {
            SortedList sl_fsd = null;

            try { sl_fsd = (SortedList)ViewState[VS_FS_DATA]; }
            catch { };

            tblMain.Rows.Clear();

            if (sl_fsd != null)
            {
                string[] hdr = new string[] { "", "Spool", "Revision", "Batch", "Created", "Status", "On Hold" };

                TableRow r;
                TableCell c;

                foreach (DictionaryEntry e0 in sl_fsd)
                {
                    CFitterScheduleData fsd = (CFitterScheduleData)e0.Value;

                    Panel pnlResults = new Panel();
                    Table tblResults = new Table();

                    tblResults.ID = "tblResults" + fsd.spool_id.ToString();

                    pnlResults.Attributes["uid"] = fsd.key.ToString();
                    tblResults.Attributes["uid"] = fsd.key.ToString();

                    pnlResults.Controls.Add(tblResults);

                    r = new TableRow();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.Controls.Add(pnlResults);
                    tblMain.Rows.Add(r);

                    r = new TableRow();

                    r.Attributes["uid"] = fsd.key.ToString();

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

                    r.Attributes["uid"] = fsd.key.ToString();

                    r.BackColor = System.Drawing.Color.FromName("White");


                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    ImageButton btn_show_parts = new ImageButton();
                    btn_show_parts.Click += new ImageClickEventHandler(btn_show_parts_Click);
                    btn_show_parts.ID = "btn_show_parts_" + fsd.spool_id.ToString();
                    btn_show_parts.Attributes["uid"] = fsd.key.ToString();
                    c.Controls.Add(btn_show_parts);
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(fsd.spool));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(fsd.revision));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(fsd.batch_number));
                    r.Cells.Add(c);

                    c = new TableCell();
                    string date_created = string.Empty;
                    if (fsd.date_created > DateTime.MinValue)
                        date_created = fsd.date_created.ToString("dd/MM/yyyy HH:mm:ss");
                    c.Controls.Add(new LiteralControl(date_created));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    c.Controls.Add(new LiteralControl(fsd.status));
                    r.Cells.Add(c);

                    c = new TableCell();

                    if(fsd.on_hold)
                        c.Controls.Add(new LiteralControl("Yes"));

                    r.Cells.Add(c);

                    if (fsd.drawing_id > 0)
                    {
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        ImageButton btn_view_drawing = new ImageButton();
                        btn_view_drawing.ToolTip = "View Drawing";
                        btn_view_drawing.ImageUrl = "~/pdf.png";
                        btn_view_drawing.Click += new ImageClickEventHandler(btn_view_drawing_Click);
                        btn_view_drawing.ID = "btn_view_drawing" + fsd.spool_id.ToString();
                        btn_view_drawing.Attributes["uid"] = fsd.drawing_id.ToString();

                        c.Controls.Add(btn_view_drawing);
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                    if (fsd.bshowing_parts)
                    {
                        show_part_data(fsd.key);
                    }

                    set_show_hide_button_properties(fsd.bshowing_parts, btn_show_parts);
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

        CFitterScheduleData get_fitter_schedule_data(string key)
        {
            CFitterScheduleData fsd = null;

            SortedList sl_fsd = null;

            try { sl_fsd = (SortedList)ViewState[VS_FS_DATA]; }
            catch { };

            if (sl_fsd != null)
            {
                if (sl_fsd.ContainsKey(key))
                {
                    fsd = (CFitterScheduleData)sl_fsd[key];
                }
            }

            return fsd;
        }

        void btn_show_parts_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string key = (b.Attributes["uid"]);

            CFitterScheduleData fsd = get_fitter_schedule_data(key);

            if (fsd != null)
            {
                fsd.bshowing_parts = !fsd.bshowing_parts;

                if (fsd.bshowing_parts)
                {
                    show_part_data(fsd.key);
                }
                else
                {
                    hide_part_data(fsd.key);
                }

                set_show_hide_button_properties(fsd.bshowing_parts, b);

                SortedList sl_fsd = (SortedList)ViewState[VS_FS_DATA];
                ViewState[VS_FS_DATA] = sl_fsd;
            
            }
        }
        void show_part_data(string key)
        {
            CFitterScheduleData fsd = get_fitter_schedule_data(key);

            if (fsd != null)
            {
                using (spool_parts sp = new spool_parts())
                {
                    SortedList sl = new SortedList();

                    sl.Add("spool_id", fsd.spool_id);

                    ArrayList a_spd = sp.get_spool_parts_data_ex(sl);

                    SortedList sl_parts_data = new SortedList();

                    foreach (spool_part_data spd in a_spd)
                    {
                        if (spd.part_data != null)
                            sl_parts_data.Add(spd.part_data.description + sl_parts_data.Count.ToString(), spd);
                    }

                    Panel pnlResults = get_pnlResults(key);
                    Table tblParts = new Table();
                    tblParts.ID = "tblParts" + key;

                    pnlResults.Controls.Add(tblParts);

                    foreach (DictionaryEntry e1 in sl_parts_data)
                    {
                        TableRow r;
                        TableCell c;

                        spool_part_data spd = (spool_part_data)e1.Value;

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
                        c.Width = 16;
                        c.BackColor = tblMain.BackColor;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Width = 500;
                        c.Controls.Add(new LiteralControl(part_desc));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(part_no));
                        c.Width = 150;
                        r.Cells.Add(c);

                        c = new TableCell();

                        if (part_desc.ToUpper().Contains("PIPE"))
                            c.Controls.Add(new LiteralControl("Len:" + spd.qty.ToString("0.00")));
                        else
                            c.Controls.Add(new LiteralControl("Qty:" + spd.qty.ToString("0")));

                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl("FW:" + spd.fw.ToString()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl("BW:" + spd.bw.ToString()));
                        r.Cells.Add(c);

                        tblParts.Rows.Add(r);
                    }
                }
            }
        }

        void hide_part_data(string key)
        {
            Panel pnl = get_pnlResults(key);

            foreach (Control cntrl in pnl.Controls)
            {
                if (cntrl.GetType() == typeof(Table))
                {
                    Table tbl = (Table)cntrl;

                    if (tbl.ID.StartsWith("tblSizes"))
                    {
                        tbl.Rows.Clear();
                        pnl.Controls.Remove(cntrl);
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
                    }
                }
            }
        }

        Panel get_pnlResults(string key)
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

                            if (pnl.Attributes["uid"] == key)
                            {
                                return pnl;
                            }
                        }
                    }
                }
            }

            return ret_pnl;
        }

        [Serializable]
        class CFitterScheduleData
        {
            public int spool_id = 0;
            public string spool = string.Empty;
            public string revision = string.Empty;
            public DateTime date_created = DateTime.MinValue;
            public string status = string.Empty;
            public bool on_hold = false;
            public string batch_number;
            public int drawing_id = 0;

            public string key = string.Empty;

            public bool bshowing_parts = false;
        }
    }
}
