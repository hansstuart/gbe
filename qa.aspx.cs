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
    public partial class qa : System.Web.UI.Page
    {
        SortedList m_results_by_spool = new SortedList();
        SortedList m_results_by_date = new SortedList();
        SortedList m_spool_data = new SortedList();
        string PIPE = "|";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["results_by_spool"] = m_results_by_spool;
                ViewState["results_by_date"] = m_results_by_date;
                ViewState["spool_data"] = m_spool_data;

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
            ArrayList aqaj = new ArrayList();
            SortedList sl = new SortedList();

            DateTime dtfrom = dtFrom.SelectedDate;
            DateTime dtto = dtTo.SelectedDate;
            dtto = dtto.AddHours(23);
            dtto = dtto.AddMinutes(59);
            dtto = dtto.AddSeconds(59);

            using (qa_jobs qaj = new qa_jobs())
            {
                aqaj = qaj.get_qa_job_data(dtfrom, dtto, "datetime_stamp");
            }
            
            using (spools spls = new spools())
            {
            using (modules m = new modules())
            {
                foreach (qa_job_data qajd in aqaj)
                {
                    sl.Clear();
                    sl.Add("id", qajd.spool_id);

                    if (qajd.assembly_type == qa_job_data.SPOOL)
                    {
                        asd = spls.get_spool_data_ex(sl);

                        if (asd.Count > 0)
                        {
                            spool_data sd = (spool_data)asd[0];

                            if (txtSearch.Text.Trim().Length == 0 || sd.spool.ToUpper().StartsWith(txtSearch.Text.Trim().ToUpper()))
                            {
                                string k = string.Empty;

                                k = sd.spool + " / " + sd.revision + PIPE + qajd.datetime_stamp.ToString("yyyyMMddHHmmss") + qajd.id;
                                m_results_by_spool.Add(k, qajd);

                                k = qajd.datetime_stamp.ToString("yyyyMMddHHmmss") + qajd.id + PIPE + sd.spool + " / " + sd.revision;
                                m_results_by_date.Add(k, qajd);

                                if (!m_spool_data.ContainsKey(qajd.id))
                                    m_spool_data.Add(qajd.id, sd);
                            }
                        }
                    }
                    else if (qajd.assembly_type == qa_job_data.MODULE)
                    {
                        asd = m.get_module_data(sl);

                        if (asd.Count > 0)
                        {
                            module_data sd = (module_data)asd[0];
                            if (txtSearch.Text.Trim().Length == 0 || sd.module.ToUpper().StartsWith(txtSearch.Text.Trim().ToUpper()))
                            {
                                string k = string.Empty;
                                k = sd.module + " / " + sd.revision + PIPE + qajd.datetime_stamp.ToString("yyyyMMddHHmmss") + qajd.id;
                                m_results_by_spool.Add(k, qajd);

                                k = qajd.datetime_stamp.ToString("yyyyMMddHHmmss") + qajd.id + PIPE + sd.module + " / " + sd.revision;
                                m_results_by_date.Add(k, qajd);
                            }
                        }
                    }
                }
            }
            }

            ViewState["results_by_spool"] = m_results_by_spool;
            ViewState["results_by_date"] = m_results_by_date;
            ViewState["spool_data"] = m_spool_data;
        }

        void display()
        {
            m_results_by_spool = (SortedList)ViewState["results_by_spool"];
            m_results_by_date = (SortedList)ViewState["results_by_date"];
            m_spool_data = (SortedList)ViewState["spool_data"];

            tblResults.Rows.Clear();

            if (m_results_by_spool == null || m_results_by_date == null)
                return;

            SortedList results = null;

            int idx_spool = 0;

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

            string[] hdr = new string[] { "Spool", "Weld Finish", "Fit Finish", "QA Date/Time", "QA User", "QA Result" };

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

            //using (spools spls = new spools() )
            //{
                using (users u = new users())
                {
                    foreach (DictionaryEntry e0 in results)
                    {
                        qa_job_data qaj = (qa_job_data)e0.Value;
                        sl.Clear();

                        spool_data sd = null;

                        if(m_spool_data.ContainsKey(qaj.id))
                            sd = (spool_data)m_spool_data[qaj.id];

                        string weld_info, fit_info;
                        weld_info = fit_info = string.Empty;

                        if (sd != null)
                        {
                            try { weld_info = sd.weld_job_data.finish.ToString("dd/MM/yy HH:mm:ss") + " by " + sd.welder_data.login_id; } catch { }
                            try { fit_info = sd.weld_job_data.finish_fit.ToString("dd/MM/yy HH:mm:ss") + " by " + sd.fitter_data.login_id;} catch { }
                        }
                        else
                        {
                            weld_info = fit_info = "-";
                        }
                        
                        string spool = e0.Key.ToString().Split(PIPE[0])[idx_spool];

                        string username = string.Empty;

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");
                        
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(spool));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(weld_info));
                        r.Cells.Add(c);


                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(fit_info));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(qaj.datetime_stamp.ToString("dd/MM/yy HH:mm:ss")));
                        r.Cells.Add(c);

                        sl.Clear();
                        
                        sl.Add("id", qaj.user_id);
                        username = string.Empty;

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
                        c.Controls.Add(new LiteralControl(qaj.result));
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);
                    }
                }
            //}
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

       
    }
}
