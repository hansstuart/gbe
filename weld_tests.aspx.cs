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
    public partial class weld_tests1 : System.Web.UI.Page
    {
        SortedList m_results_by_spool = new SortedList();
        SortedList m_results_by_date = new SortedList();
        string PIPE = "|";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["results_by_spool"] = m_results_by_spool;
                ViewState["results_by_date"] = m_results_by_date;

                dlSort.Items.Add("Project");
                dlSort.Items.Add("Date");

                dtFrom.SelectedDate = DateTime.Today;
                dtTo.SelectedDate = DateTime.Today;

                txtSearch.Focus();
            }
            else
            {
                display();
            }
        }

        void search()
        {
            m_results_by_spool.Clear();
            m_results_by_date.Clear();

            ArrayList asd = new ArrayList();
            ArrayList awtj = new ArrayList();
            SortedList sl = new SortedList();

            DateTime dtfrom = dtFrom.SelectedDate;
            DateTime dtto = dtTo.SelectedDate;
            dtto = dtto.AddHours(23);
            dtto = dtto.AddMinutes(59);
            dtto = dtto.AddSeconds(59);

            using (weld_tests wt = new weld_tests())
            {
                awtj = wt.get_weld_test_data(dtfrom, dtto, "datetime_stamp");
            }

            using (spools spls = new spools())
            {
                using (users u = new users())
                {
                    foreach (weld_test_data wtd in awtj)
                    {
                        sl.Clear();
                        sl.Add("id", wtd.spool_id);

                        asd = spls.get_spool_data_short(sl);

                        if (asd.Count > 0)
                        {
                            spool_data sd = (spool_data)asd[0];
                            if (txtSearch.Text.Trim().Length == 0 || sd.spool.ToUpper().StartsWith(txtSearch.Text.Trim().ToUpper()))
                            {
                                string welder = "-";
                               
                                if (sd.welder > 0)
                                {
                                    user_data wud = u.get_user_data(sd.welder);
                                    if (wud != null)
                                        welder = wud.login_id;
                                }


                                string k = string.Empty;
                                k = sd.spool + " / " + sd.revision + PIPE + wtd.datetime_stamp.ToString("yyyyMMddHHmmss") + wtd.id + PIPE + welder;
                                m_results_by_spool.Add(k, wtd);

                                k = wtd.datetime_stamp.ToString("yyyyMMddHHmmss") + wtd.id + PIPE + sd.spool + " / " + sd.revision + PIPE + welder;
                                m_results_by_date.Add(k, wtd);
                            }
                        }
                    }
                }
            }

            ViewState["results_by_spool"] = m_results_by_spool;
            ViewState["results_by_date"] = m_results_by_date;

        }

        void display()
        {
            m_results_by_spool = (SortedList)ViewState["results_by_spool"];
            m_results_by_date = (SortedList)ViewState["results_by_date"];

            tblResults.Rows.Clear();

            if (m_results_by_spool == null || m_results_by_date == null)
                return;

            SortedList results = null;

            int idx_spool = 0;
            int idx_welder = 2;

            if (dlSort.Text == "Project")
            {
                results = m_results_by_spool;
                idx_spool = 0;
            }
            else
            {
                results = m_results_by_date;
                idx_spool = 1;
            }

            if (results.Count == 0)
                return;

            SortedList sl = new SortedList();
            TableRow r;
            TableCell c;

            string[] hdr = new string[] { "Spool", "Date/Time", "User", "Report 1", "Report 2", "FW", "BW", "Welder" };

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

            using (users u = new users())
            {
                foreach (DictionaryEntry e0 in results)
                {
                    weld_test_data wtd = (weld_test_data)e0.Value;
                    sl.Clear();

                    string spool = e0.Key.ToString().Split(PIPE[0])[idx_spool];

                    string username = string.Empty;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(spool));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(wtd.datetime_stamp.ToString("dd/MM/yyyy HH:mm:ss")));
                    r.Cells.Add(c);

                    sl.Clear();

                    sl.Add("id", wtd.user_id);
                    
                    ArrayList a2 = u.get_user_data(sl);

                    if (a2.Count > 0)
                    {
                        user_data ud = (user_data)a2[0];
                        username = ud.login_id;
                    }

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(username));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(wtd.report_number));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(wtd.report2_number));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(wtd.fw.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(wtd.bw.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(e0.Key.ToString().Split(PIPE[0])[idx_welder]));
                    r.Cells.Add(c);
                    

                    tblResults.Rows.Add(r);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }
    }
}
