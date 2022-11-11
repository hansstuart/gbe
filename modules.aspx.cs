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
    public partial class modules1 : System.Web.UI.Page
    {
        SortedList m_modules = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_modules = (SortedList)ViewState["modules"];
                display();
            }
            else
            {
                txtSearch.Focus();
            }
        }

        void search()
        {
            using (modules m = new modules())
            {
                SortedList sl = new SortedList();

                if (m_modules != null)
                    m_modules.Clear();

                if (txtSearch.Text.Trim().Length > 3)
                {
                    sl.Add("barcode", txtSearch.Text.Trim() + "%");

                    ArrayList amd = m.get_module_data_ex(sl);

                    m_modules = new SortedList();

                    foreach (module_data md in amd)
                    {
                        m_modules.Add(md.barcode+"_"+md.id, md);
                    }
                }
            }

            ViewState["modules"] = m_modules;
        }

        void display()
        {
            if (m_modules != null)
            {
                tblResults.Rows.Clear();

                TableRow r;
                TableCell c;

                string[] hdr = new string[] { "Module", "Revision", "Builder", "Start Build", "Finish Build", "Status" };

                r = new TableRow();

                r.BackColor = System.Drawing.Color.FromName("SeaGreen");

                foreach (string sh in hdr)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblResults.Rows.Add(r);

                foreach (DictionaryEntry e0 in m_modules)
                {
                    module_data md = (module_data)e0.Value;

                    r = new TableRow();

                    r.Attributes["uid"] = e0.Key.ToString();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(md.module));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(md.revision));
                    r.Cells.Add(c);

                    c = new TableCell();

                    string builder = string.Empty;

                    if (md.builder_data != null)
                        builder = md.builder_data.login_id;

                    c.Controls.Add(new LiteralControl(builder));
                    r.Cells.Add(c);

                    string dts_b = string.Empty;
                    string dte_b = string.Empty;

                    if (md.weld_job_data != null)
                    {
                        if (md.weld_job_data.start > DateTime.MinValue)
                            dts_b = md.weld_job_data.start.ToString("dd/MM/yyyy HH:mm:ss");

                        if (md.weld_job_data.finish > DateTime.MinValue)
                            dte_b = md.weld_job_data.finish.ToString("dd/MM/yyyy HH:mm:ss");
                    }

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dts_b));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dte_b));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(md.status));
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
