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
    public partial class add_to_schedule : System.Web.UI.Page
    {
        const string FAB_TBL = "schedule_fab";
        const string DELIV_TBL = "schedule_delivery";

        const string VS_SCHEDULE_RECS = "vs_schedule_recs";
        const string VS_TBL = "vs_tbl";
        const string VS_HOLDING = "vs_holding";
        const string VS_QUARANTINE = "vs_quarantine";
        const string ATT_BC = "att_bc";

        SortedList m_sl_schedule_rec = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string tbl = string.Empty;

                string holding_area = string.Empty;
                bool bholding_area = false;

                string quarantine = string.Empty;
                bool bquarantine = false;

                tbl = Request.QueryString["t"];

                holding_area = Request.QueryString["h"];
                quarantine = Request.QueryString["q"];

                if(holding_area != null)
                    if(holding_area=="1")
                        bholding_area = true;

                if (!bholding_area)
                {
                    if (quarantine != null)
                        if (quarantine == "1")
                            bquarantine = true;
                }

                if (tbl != null)
                {
                    if (tbl == FAB_TBL)
                    {
                        lblTime.Visible = txtTime.Visible = lblVehicle.Visible = dlVehicle.Visible = false;

                        lblDate.Visible = txtDate.Visible = false;

                        if (bholding_area)
                        {
                            lblTitle.Text = "Add To Fabrication Holding Area";
                        }
                        else if (bquarantine)
                        {
                            lblTitle.Text = "Quarantine To Fabrication Schedule";

                            lblDate.Text = "Fabrication Date (dd/mm/yyyy):";
                        }
                        else
                        {
                            lblTitle.Text = "Holding Area To Fabrication Schedule";

                            lblDate.Text = "Fabrication Date (dd/mm/yyyy):";
                        }
                    }
                    else if (tbl == DELIV_TBL)
                    {
                        lblTitle.Text = "Add To Delivery Schedule";

                        lblDate.Text = "Delivery Date (dd/mm/yyyy):";

                        lblTime.Visible = txtTime.Visible = lblVehicle.Visible = dlVehicle.Visible = true;
                    }
                    else
                        Server.Transfer("default.aspx", true);

                    ViewState[VS_TBL] = tbl;
                    ViewState[VS_HOLDING] = bholding_area;
                    ViewState[VS_QUARANTINE] = bquarantine;
                }
                else
                    Server.Transfer("default.aspx", true);

                using (vehicles v = new vehicles())
                {
                    ArrayList avd = v.get_vehicle_data(new SortedList());

                    dlVehicle.Items.Add("TBC");

                    foreach (vehicle_data vd in avd)
                    {
                        dlVehicle.Items.Add(vd.registration.Trim());
                    }
                }

                get_schedule_recs();

                txtDate.Focus();
            }
            else
            {
                try { m_sl_schedule_rec = (SortedList)ViewState[VS_SCHEDULE_RECS]; }
                catch { }
            }

            lblMsg.Text = string.Empty;
            display();
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

                if (dt == DateTime.Today)
                {
                    DateTime dtnow = DateTime.Now;

                    DateTime midday = new DateTime(dtnow.Year, dtnow.Month, dtnow.Day, 12, 0, 0);

                    bret &= dtnow < midday;
                }
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

        void get_schedule_recs()
        {
            schedule sf = null;

            string dt = schedule_fab_data.INIT_DATE;
            
            string tbl = ViewState[VS_TBL].ToString();
            bool b_quarantine = (bool)ViewState[VS_QUARANTINE];

            if (tbl == FAB_TBL)
            {
                sf = new schedule_fab();

                bool b_add_to_holding = (bool)ViewState[VS_HOLDING];

                if (b_quarantine)
                    dt = schedule_fab_data.QUARANTINE_RECS_DATE;
                else if (!b_add_to_holding)
                    dt = schedule_fab_data.HOLDING_RECS_DATE;
            }
            else if (tbl == DELIV_TBL)
            {
                sf = new schedule_delivery();

                if (b_quarantine)
                    dt = schedule_fab_data.QUARANTINE_RECS_DATE;
            }

            ArrayList a_schd = sf.get_schedule_recs(dt);

            if (m_sl_schedule_rec == null)
                m_sl_schedule_rec = new SortedList();
            else
                m_sl_schedule_rec.Clear();

            foreach (c_schedule_rec sr in a_schd)
            {
                if (m_sl_schedule_rec == null)
                    m_sl_schedule_rec = new SortedList();

                if (!m_sl_schedule_rec.ContainsKey(sr.barcode))
                    m_sl_schedule_rec.Add(sr.barcode, sr);
            }

            ViewState[VS_SCHEDULE_RECS] = m_sl_schedule_rec;
        }

        void display()
        {
            string tbl = ViewState[VS_TBL].ToString();

            tblSpools.Rows.Clear();

            int i = 0;

            if (m_sl_schedule_rec != null)
            {
                if (m_sl_schedule_rec.Count > 0)
                {
                    TableRow r;
                    TableCell c;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGreen");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(string.Empty));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl("Spool"));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl("Status"));
                    r.Cells.Add(c);

                    if (tbl == FAB_TBL)
                    {
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl("Schedule Slot"));
                        r.Cells.Add(c);
                    }

                    bool b_add_to_holding = (bool)ViewState[VS_HOLDING];

                    if (tbl == DELIV_TBL || !b_add_to_holding)
                    {
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl("Batch"));
                        r.Cells.Add(c);
                    }

                    tblSpools.Rows.Add(r);

                    foreach (DictionaryEntry e0 in m_sl_schedule_rec)
                    {
                        c_schedule_rec sr = (c_schedule_rec)e0.Value;

                        r = new TableRow();

                        r.Attributes[ATT_BC] = sr.barcode;

                        System.Drawing.Color bc;
                        if (i++ % 2 == 0)
                            bc = System.Drawing.Color.FromName("White");
                        else
                            bc = System.Drawing.Color.FromName("LightGray");

                        r.BackColor = bc;

                        c = new TableCell();
                        CheckBox chk = new CheckBox();
                        chk.ID = "chk" + sr.schd.id.ToString();
                        c.Width = 10;
                        c.Controls.Add(chk);
                        r.Cells.Add(c);

                        c = new TableCell();

                        if (sr.schd.material.Trim() == "Stainless Steel")
                        {
                            c.ForeColor = System.Drawing.Color.FromName("Red");
                        }
                        else if (sr.schd.material.Trim() == "Screwed")
                        {
                            c.ForeColor = System.Drawing.Color.FromName("Lime");
                        }
                        else if (sr.schd.material.Trim() == "HS")
                        {
                            c.ForeColor = System.Drawing.Color.FromName("Purple");
                        }

                        c.Controls.Add(new LiteralControl(sr.barcode));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(sr.schd.status));
                        r.Cells.Add(c);

                        if (tbl == FAB_TBL)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(sr.schd.slots.ToString()));
                            r.Cells.Add(c);
                        }

                        if (tbl == DELIV_TBL || !b_add_to_holding)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;

                            string batch_no = string.Empty;

                            if (sr.schd.batch_number == 0)
                                batch_no = "Extra";
                            else
                                batch_no = sr.schd.batch_number.ToString("00");

                            c.Controls.Add(new LiteralControl(batch_no));
                            r.Cells.Add(c);
                        }

                        tblSpools.Rows.Add(r);
                    }
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (m_sl_schedule_rec != null)
            {
                string tbl = ViewState[VS_TBL].ToString();
                bool bholding_area = (bool)ViewState[VS_HOLDING];

                if (tbl == FAB_TBL && !bholding_area)
                {
                    txtDate.Text = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                    txtTime.Text = "00:00";

                    if (!is_valid_date(txtDate.Text))
                    {
                        lblMsg.Text = "Invalid fabrication date";
                        txtDate.Focus();
                        return;
                    }
                    else if (schedule_fab.is_date_locked(txtDate.Text))
                    {
                        lblMsg.Text = "This fabrication date is locked";
                        txtDate.Focus();
                        return;
                    }
                }
                else if (tbl == DELIV_TBL)
                {
                    if (!is_valid_date(txtDate.Text))
                    {
                        lblMsg.Text = "Invalid delivery date";
                        txtDate.Focus();
                        return;
                    }

                    if (!is_valid_time(txtTime.Text))
                    {
                        lblMsg.Text = "Invalid time";
                        txtTime.Focus();
                        return;
                    }
                }

                ArrayList a_schedule_id = new ArrayList();
                SortedList sl_selected_schedule_recs = new SortedList();
                int total_slots_selected = 0;

                foreach (TableRow r in tblSpools.Rows)
                {
                    string barcode = (r.Attributes[ATT_BC]);

                    if (barcode != null)
                    {
                        CheckBox chk = (CheckBox)(r.Cells[0].Controls[0]);

                        if (chk.Checked)
                        {
                            c_schedule_rec sr = (c_schedule_rec)m_sl_schedule_rec[barcode];

                            a_schedule_id.Add(sr.schd.id);
                            total_slots_selected += sr.schd.slots;

                            sl_selected_schedule_recs.Add(sr.schd.id, sr);
                        }
                    }
                }

                if (a_schedule_id.Count > 0)
                {
                    DateTime dt_fab = DateTime.ParseExact(schedule_fab_data.INIT_DATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime dt_deliv = DateTime.ParseExact(schedule_fab_data.INIT_DATE, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    if (tbl == FAB_TBL)
                    {
                        schedule_fab sf = new schedule_fab();

                        if (!bholding_area)
                        {
                            int allocated_slots = sf.get_allocated_slots(txtDate.Text);

                            if ((allocated_slots + total_slots_selected) > schedule.PROD_SLOTS)
                            {
                                lblMsg.Text = "Error. " + (schedule.PROD_SLOTS - allocated_slots).ToString() + " slot(s) available for the selected date";
                                return;
                            }

                            dt_fab = DateTime.ParseExact(txtDate.Text.Trim() + "\x20" + txtTime.Text.Trim(), "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            dt_fab = dt_fab.AddDays(1);

                            SortedList sl_batches = new SortedList();

                            const int MAX_BATCH_NO = 999;

                            using (settings setting = new settings())
                            {
                                settings_data sd = setting.get_settings_data();

                                int batch_number = sd.schedule_batch_number + 1;

                                foreach (int schedule_id in a_schedule_id)
                                {
                                    c_schedule_fab_rec sr = (c_schedule_fab_rec)sl_selected_schedule_recs[schedule_id];

                                    string project = sr.barcode.Split('-')[0];

                                    ArrayList a_batch = null;

                                    if (sl_batches.ContainsKey(project))
                                        a_batch = (ArrayList)sl_batches[project];
                                    else
                                    {
                                        a_batch = new ArrayList();

                                        if (sl_batches.Count > 0)
                                        {
                                            batch_number++;
                                        }

                                        sl_batches.Add(project, a_batch);
                                    }

                                    if (batch_number > MAX_BATCH_NO)
                                        batch_number = 1;

                                    sr.schd.batch_number = batch_number;

                                    a_batch.Add(sr);
                                }


                                SortedList sl = new SortedList();
                                sl.Add("id", sd.id);
                                sl.Add("schedule_batch_number", batch_number);
                                setting.save_settings_data(sl);
                            }
                        }
                    }
                    else if (tbl == DELIV_TBL)
                    {
                        dt_deliv = DateTime.ParseExact(txtDate.Text.Trim() + "\x20" + txtTime.Text.Trim(), "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    using (schedule_fab schdf = new schedule_fab())
                    {
                        using (schedule_delivery schdd = new schedule_delivery())
                        {
                            foreach (int schedule_id in a_schedule_id)
                            {
                                c_schedule_rec sr = (c_schedule_rec)sl_selected_schedule_recs[schedule_id];

                                SortedList sl_schd = new SortedList();

                                if (tbl == FAB_TBL)
                                {
                                    sl_schd.Add("id", schedule_id);
                                    sl_schd.Add("dt", dt_fab);
                                    sl_schd.Add("batch_number", sr.schd.batch_number);
                                    sl_schd.Add("seq", int.MaxValue);

                                    schdf.save_schedule_fab_data(sl_schd);

                                    if (bholding_area)
                                    {
                                        if (sl_selected_schedule_recs.ContainsKey(schedule_id))
                                        {
                                            sl_schd.Clear();
                                            sl_schd.Add("spool_id", sr.schd.spool_id);
                                            sl_schd.Add("dt", dt_deliv);
                                            sl_schd.Add("batch_number", sr.schd.batch_number);
                                            sl_schd.Add("vehicle", "TBC");

                                            schdd.save_schedule_delivery_data(sl_schd);
                                        }
                                    }
                                }
                                else if (tbl == DELIV_TBL)
                                {
                                    sl_schd.Clear();
                                    sl_schd.Add("id", schedule_id);
                                    sl_schd.Add("dt", dt_deliv);
                                    sl_schd.Add("vehicle", dlVehicle.Text);

                                    schdd.save_schedule_delivery_data(sl_schd);
                                }
                            }
                        }
                    }

                    txtDate.Text = txtTime.Text = txtDate.Text = string.Empty;
                    get_schedule_recs();
                    display();

                    lblMsg.Text = "The selected spools have been added";

                    txtDate.Focus();
                
                }
                else
                {
                    lblMsg.Text = "No spools selected";
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (m_sl_schedule_rec != null)
            {
                string confirmValue = Request.Form["confirm_value"];

                if (confirmValue == "Yes")
                {
                    //schedule_fab sf = new schedule_fab();
                    SortedList sl = new SortedList();
                    bool brefresh = false;
                    string tbl = ViewState[VS_TBL].ToString();

                    using (cdb_connection db = new cdb_connection())
                    {
                        foreach (TableRow r in tblSpools.Rows)
                        {
                            string barcode = (r.Attributes[ATT_BC]);

                            if (barcode != null)
                            {
                                CheckBox chk = (CheckBox)(r.Cells[0].Controls[0]);

                                if (chk.Checked)
                                {
                                    c_schedule_rec sr = (c_schedule_rec)m_sl_schedule_rec[barcode];

                                    sl.Clear();
                                    sl.Add("id", sr.schd.id);
                                    db.delete_record(tbl, sl);
                                    brefresh = true;
                                }
                            }
                        }
                    }

                    if (brefresh)
                    {
                        get_schedule_recs();
                        display();
                        txtDate.Focus();
                    }
                }
            }
        }
    }

    
}
