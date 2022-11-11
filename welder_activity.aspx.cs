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
    public partial class welder_activity : System.Web.UI.Page
    {
        ArrayList m_welders = new ArrayList();
        ArrayList m_fitters = new ArrayList();
        ArrayList m_site_fitters = new ArrayList();
        ArrayList m_results = new ArrayList();

        const string ALL_WELDERS = "All welders";
        const string ALL_FITTERS = "All fitters";
        const string ALL_SITE_FITTERS = "All site fitters";
        const string ROBOT = "Robot";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_welders = (ArrayList)ViewState["welders"];
                m_fitters = (ArrayList)ViewState["fitters"];
                m_site_fitters = (ArrayList)ViewState["site_fitters"];
                m_results = (ArrayList)ViewState["results"];
                display();
            }
            else
            {
                dlSearchFlds.Items.Add("Project");
                dlSearchFlds.Items.Add("Spool");
                dlSearchFlds.Items.Add("Barcode");

                dlWelders.Items.Add(ROBOT);

                ArrayList a = new ArrayList();
                
                SortedList sl = new SortedList();
                sl.Add("role", "WELDER");
                
                using (users u = new users())
                {
                    u.order_by = "name";
                    m_welders = u.get_user_data(sl);
                }

                foreach (user_data ud in m_welders)
                    dlWelders.Items.Add(ud.name + " (" + ud.login_id + ")");

                sl.Clear();

                sl.Add("role", "FITTER");

                using (users u = new users())
                {
                    u.order_by = "name";
                    m_fitters = u.get_user_data(sl);
                }

                foreach (user_data ud in m_fitters)
                    dlWelders.Items.Add(ud.name + " (" + ud.login_id + ")");

                sl.Clear();

                sl.Add("role", "SITE FITTER");

                using (users u = new users())
                {
                    u.order_by = "name";
                    m_site_fitters = u.get_user_data(sl);
                }

                foreach (user_data ud in m_site_fitters)
                    dlWelders.Items.Add(ud.name + " (" + ud.login_id + ")");

                dlWelders.Items.Add(ALL_WELDERS);
                dlWelders.Items.Add(ALL_FITTERS);
                dlWelders.Items.Add(ALL_SITE_FITTERS);
                
                m_welders.Add(new user_data());

                ViewState["fitters"] = m_fitters;
                ViewState["welders"] = m_welders;
                ViewState["site_fitters"] = m_site_fitters;
                ViewState["results"] = m_results;
            }
        }

        user_data get_user_data()
        {
            user_data ud = null;
            
            foreach (user_data w in m_welders)
            {
                if (w.name + " (" + w.login_id + ")" == dlWelders.Text)
                {
                    ud = w;
                }
            }

            if (ud == null)
            {
                foreach (user_data w in m_fitters)
                {
                    if (w.name + " (" + w.login_id + ")" == dlWelders.Text)
                    {
                        ud = w;
                    }
                }
            }

            if (ud == null)
            {
                foreach (user_data w in m_site_fitters)
                {
                    if (w.name + " (" + w.login_id + ")" == dlWelders.Text)
                    {
                        ud = w;
                    }
                }
            }

            return ud;
        }

        void search()
        {
            tblResults.Rows.Clear();
            m_results.Clear();

            ViewState["results"] = m_results;

            if (txtSpool.Text.Trim().Length > 0)
                if (txtSpool.Text.Trim().Length < 4)
                    return;

            DateTime dtfrom = dtFrom.SelectedDate;
            DateTime dtto = dtTo.SelectedDate;

            user_data ud = get_user_data();

            int uid = 0;
            int by_welder_or_fitter_or_site_fitter = 0;

            if (dlWelders.Text == ROBOT)
            {
                by_welder_or_fitter_or_site_fitter = 3;
            }
            else
            {
                if (ud != null)
                {
                    if (ud.role == "WELDER")
                        by_welder_or_fitter_or_site_fitter = 0;
                    else if (ud.role == "FITTER")
                        by_welder_or_fitter_or_site_fitter = 1;
                    else if (ud.role == "SITE FITTER")
                        by_welder_or_fitter_or_site_fitter = 2;

                    uid = ud.id;
                }
                else
                {
                    if (dlWelders.Text == ALL_WELDERS)
                        by_welder_or_fitter_or_site_fitter = 0;
                    if (dlWelders.Text == ALL_FITTERS)
                        by_welder_or_fitter_or_site_fitter = 1;
                    if (dlWelders.Text == ALL_SITE_FITTERS)
                        by_welder_or_fitter_or_site_fitter = 2;
                }
            }

            using (weld_jobs wj = new weld_jobs())
            {
                m_results = wj.get_weld_job_data(uid, by_welder_or_fitter_or_site_fitter, dtfrom, dtto, txtSpool.Text.Trim());
            }
            
            ViewState["results"] = m_results;
        }

        int welder_or_fitter_view()
        {
            int by_welder_or_fitter = 0;

            if (dlWelders.Text == ROBOT)
            {
                by_welder_or_fitter = 3;
            }
            else
            {
                user_data ud = get_user_data();

                if (dlWelders.Text == ALL_WELDERS)
                    by_welder_or_fitter = 0;
                else if (dlWelders.Text == ALL_FITTERS)
                    by_welder_or_fitter = 1;
                else if (dlWelders.Text == ALL_SITE_FITTERS)
                    by_welder_or_fitter = 2;
                else if (ud != null)
                {
                    if (ud.role == "WELDER")
                        by_welder_or_fitter = 0;
                    else if (ud.role == "FITTER")
                        by_welder_or_fitter = 1;
                    else if (ud.role == "SITE FITTER")
                        by_welder_or_fitter = 2;
                }
            }

            return by_welder_or_fitter;
        }
        void display()
        {
            TableRow r;
            TableCell c;

            int current_welder_fitter_id = -1;

            int by_welder_or_fitter = welder_or_fitter_view();
            
            decimal welder_total = 0;
            decimal fitter_total = 0;
            decimal site_fitter_total = 0;
            decimal robot_total = 0;

            decimal welder_spool_total = 0;
            decimal fitter_spool_total = 0;
            decimal site_fitter_spool_total = 0;

            string[] hdr_w = new string[] { "Robot", "Welder",  "Spool", "Part", "Qty", "Start", "Finish", "Welder Rate", "Welder Total"};
            string[] hdr_f = new string[] { "Robot", "Fitter", "Spool", "Part", "Qty", "Start", "Finish", "Fitter Rate", "Fitter Total" };
            string[] hdr_sf = new string[] { "Site Fitter", "Spool", "Part", "Qty", "Installed On", "Site Fitter Rate", "Site Fitter Total" };

            string[] hdr;

            if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                hdr = hdr_w;
            else if (by_welder_or_fitter == 1)
                hdr = hdr_f;
            else
                hdr = hdr_sf;

            string current_spool = string.Empty;

            foreach (weld_job_data wjd in m_results)
            {
                if (wjd.spool_data != null)
                {
                    string this_spool = wjd.spool_data.barcode;
            
                    // compare welder or fitter or site fitter
                    int this_welder_fitter_id = -2;

                    if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                    {
                        this_welder_fitter_id = wjd.user_id;
                    }
                    else if (by_welder_or_fitter == 1)
                    {
                        this_welder_fitter_id = wjd.fitter_id;
                    }
                    else
                    {
                        this_welder_fitter_id = wjd.site_fitter_id;
                    }

                    if (current_welder_fitter_id != this_welder_fitter_id)
                    {
                        current_welder_fitter_id = this_welder_fitter_id;

                        if (tblResults.Rows.Count > 1)
                        {
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightGray");

                            string s = string.Empty;

                            foreach (string sh in hdr)
                            {
                                if (sh == "Welder Total")
                                    s = welder_spool_total.ToString("0.00");
                                else if (sh == "Fitter Total")
                                    s = fitter_spool_total.ToString("0.00");
                                else if (sh == "Site Fitter Total")
                                    s = site_fitter_spool_total.ToString("0.00");
                                else
                                    s = string.Empty;

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(s));
                                r.Cells.Add(c);

                                current_spool = this_spool;
                            }

                            tblResults.Rows.Add(r);

                            welder_spool_total = 0;
                            fitter_spool_total = 0;
                            site_fitter_spool_total = 0;

                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("Silver");

                            s = string.Empty;

                            foreach (string sh in hdr)
                            {
                                if (sh == "Welder Total")
                                {
                                    s = welder_total.ToString("0.00");
                                }
                                else if (sh == "Fitter Total")
                                    s = fitter_total.ToString("0.00");
                                else if (sh == "Site Fitter Total")
                                    s = site_fitter_total.ToString("0.00");
                                else
                                    s = string.Empty;

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Font.Bold = true;
                                c.Controls.Add(new LiteralControl(s));
                                r.Cells.Add(c);
                            }
                            tblResults.Rows.Add(r);

                            welder_total = 0;
                            fitter_total = 0;

                            current_spool = string.Empty;
                        }

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("LightGreen");

                        foreach (string sh in hdr)
                        {
                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(sh));
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);
                    }

                    foreach (spool_part_data spd in wjd.spool_data.spool_part_data)
                    {
                        if (current_spool.Length == 0)
                            current_spool = this_spool;
                        else if (current_spool != this_spool)
                        {
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightGray");

                            string s = string.Empty;

                            foreach (string sh in hdr)
                            {
                                if (sh == "Welder Total")
                                    s = welder_spool_total.ToString("0.00");
                                else if (sh == "Fitter Total")
                                    s = fitter_spool_total.ToString("0.00");
                                else if (sh == "Site Fitter Total")
                                    s = site_fitter_spool_total.ToString("0.00");
                                else
                                    s = string.Empty;

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(s));
                                r.Cells.Add(c);

                                current_spool = this_spool;
                            }

                            tblResults.Rows.Add(r);

                            welder_spool_total = 0;
                            fitter_spool_total = 0;
                            site_fitter_spool_total = 0;
                        }

                        if (wjd.robot > 0)
                        {
                            if (by_welder_or_fitter == 0)
                            {
                                if (spd.fw == 0)
                                {
                                    continue;
                                }
                            }
                            else if (by_welder_or_fitter == 3)
                            {
                                if (spd.bw == 0)
                                {
                                    continue;
                                }
                            }
                        }

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        if (by_welder_or_fitter == 0 || by_welder_or_fitter == 1 || by_welder_or_fitter == 3)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;

                            if (wjd.robot > 0)
                            {
                                LiteralControl lc_y = new LiteralControl("Y");
                                lc_y.Visible = false;

                                c.Controls.Add(lc_y);

                                Image img_picked = new Image();
                                img_picked.ToolTip = "Robot";
                                img_picked.ImageUrl = "~/picked.png";
                                
                                c.Controls.Add(img_picked);
                            }

                            r.Cells.Add(c);

                            if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(wjd.spool_data.welder_data.login_id));
                                r.Cells.Add(c);
                            }
                            else if (by_welder_or_fitter == 1)
                            {
                                string fitter = string.Empty;

                                if (wjd.spool_data.fitter_data != null)
                                    fitter = wjd.spool_data.fitter_data.login_id;

                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(fitter));
                                r.Cells.Add(c);
                            }
                        }
                        else if (by_welder_or_fitter == 2)
                        {
                            string site_fitter = string.Empty;

                            if (wjd.spool_data.site_fitter_data != null)
                                site_fitter = wjd.spool_data.site_fitter_data.login_id;

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(site_fitter));
                            r.Cells.Add(c);
                        }

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(wjd.spool_data.spool + " / " + wjd.spool_data.revision));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(spd.part_data.description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(spd.qty.ToString("0.00")));
                        r.Cells.Add(c);

                        decimal line_total;

                        if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wjd.start.ToString("dd:MM:yyyy HH:mm:ss")));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wjd.finish.ToString("dd:MM:yyyy HH:mm:ss")));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(spd.part_data.welder_rate.ToString("0.00")));
                            r.Cells.Add(c);

                            line_total = (spd.qty * spd.part_data.welder_rate);
                            welder_total += line_total;
                            welder_spool_total += line_total;

                            if (dlWelders.Text == ROBOT)
                            {
                                robot_total += line_total;
                            }

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(line_total.ToString("0.00")));
                            r.Cells.Add(c);
                        }
                        else if (by_welder_or_fitter == 1)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wjd.start_fit.ToString("dd:MM:yyyy HH:mm:ss")));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wjd.finish_fit.ToString("dd:MM:yyyy HH:mm:ss")));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(spd.part_data.fitter_rate.ToString("0.00")));
                            r.Cells.Add(c);
                            line_total = (spd.qty * spd.part_data.fitter_rate);
                            fitter_total += line_total;
                            fitter_spool_total += line_total;
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(line_total.ToString("0.00")));
                            r.Cells.Add(c);
                        }
                        else if (by_welder_or_fitter == 2)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(wjd.installed_on.ToString("dd:MM:yyyy HH:mm:ss")));
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(spd.part_data.site_fitter_rate.ToString("0.00")));
                            r.Cells.Add(c);
                            line_total = (spd.qty * spd.part_data.site_fitter_rate);
                            site_fitter_total += line_total;
                            site_fitter_spool_total += line_total;
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Right;
                            c.Controls.Add(new LiteralControl(line_total.ToString("0.00")));
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);
                    }
                }
            }

            if (tblResults.Rows.Count > 1)
            {
                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGray");

                string s = string.Empty;

                foreach (string sh in hdr)
                {
                    if (sh == "Welder Total")
                        s = welder_spool_total.ToString("0.00");
                    else if (sh == "Fitter Total")
                        s = fitter_spool_total.ToString("0.00");
                    else if (sh == "Site Fitter Total")
                        s = site_fitter_spool_total.ToString("0.00");
                    else
                        s = string.Empty;

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(s));
                    r.Cells.Add(c);
                }

                tblResults.Rows.Add(r);

                welder_spool_total = 0;
                fitter_spool_total = 0;
                site_fitter_spool_total = 0;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("Silver");

                s = string.Empty;

                foreach (string sh in hdr)
                {
                    if (sh == "Welder Total")
                        s = welder_total.ToString("0.00");
                    else if (sh == "Fitter Total")
                        s = fitter_total.ToString("0.00");
                    else if (sh == "Site Fitter Total")
                        s = site_fitter_total.ToString("0.00");
                    else
                        s = string.Empty;

                    c = new TableCell();
                    c.Font.Bold = true;
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(new LiteralControl(s));
                    r.Cells.Add(c);
                }

                tblResults.Rows.Add(r);

                welder_total = 0;
                fitter_total = 0;

                if (dlWelders.Text == ROBOT)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("DarkGray");

                    s = string.Empty;

                    foreach (string sh in hdr)
                    {
                        if (sh == "Welder Total")
                            s = robot_total.ToString("0.00");
                        else
                            s = string.Empty;

                        c = new TableCell();
                        c.Font.Bold = true;
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(s));
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

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
            int by_welder_or_fitter = welder_or_fitter_view();

            string[] hdr_w = new string[] { "robot", "welder", "spool", "part", "qty", "start", "finish", "welder_rate Rate", "welder_total Total" };
            string[] hdr_f = new string[] { "robot", "fitter", "spool", "part", "qty", "start", "finish", "fitter_rate", "fitter_total" };
            string[] hdr_sf = new string[] { "", "site_fitter", "spool", "part", "qty", "installed_on", "site_fitter_rate", "site_fitter_total" };

            string[] hdr;

            if (by_welder_or_fitter == 0)
                hdr = hdr_w;
            else if(by_welder_or_fitter == 1)
                hdr = hdr_f;
            else 
                hdr = hdr_sf;

            if (tblResults.Rows.Count > 0)
            {
                string CRLF = "\r\n";
                string DC = "\"";
                string CM = ",";

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=welder_fitter_activity_" + dlWelders.Text + "_" + dtFrom.SelectedDate.ToString("yyyyMMdd") + "-" + dtTo.SelectedDate.ToString("yyyyMMdd") + ".csv");

                string shdr = string.Empty;

                foreach (string s in hdr)
                {
                    if (shdr.Length > 0)
                        shdr += CM;

                    shdr += DC + s + DC;
                }

                Response.Write(shdr);
                Response.Write(CRLF);

                string line = string.Empty;

                foreach (TableRow r in tblResults.Rows)
                {
                    if (r.BackColor == System.Drawing.Color.FromName("LightGreen"))
                        continue;

                    line = DC + get_tbl_row_cell_val(r, 0) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 1) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 2) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 3) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 4) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 5) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 6) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 7) + DC + CM;
                    line += DC + get_tbl_row_cell_val(r, 8) + DC;
                    
                    line += CRLF;

                    Response.Write(line);
                }

                Response.End();

            }
        }

        string get_tbl_row_cell_val(TableRow r, int i)
        {
            string sret = string.Empty;

            try { sret = ((LiteralControl)(r.Cells[i].Controls[0])).Text; }
            catch { }

            return sret;
        }
    }
}
