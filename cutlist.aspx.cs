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
    public partial class cutlist : System.Web.UI.Page
    {
        const string VS_CL_DATA = "VS_CL_DATA";
        const string VS_SORT = "VS_SORT";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SortedList sl_sort = new SortedList();
                const string SPOOL = "Spool";

                sl_sort.Add(SPOOL, " barcode ASC ");
                sl_sort.Add("Pipe Size", " pipe_size ASC ");
                sl_sort.Add("Date Created", " date_created ASC ");

                ViewState[VS_SORT] = sl_sort;

                foreach (DictionaryEntry e0 in sl_sort)
                    dlSortBy.Items.Add(e0.Key.ToString());

                dlSortBy.Text = SPOOL;
                
                get_cust_list_data(sl_sort[SPOOL].ToString());
            }

            display();
        }

        void get_cust_list_data(string order_by)
        {
            try
            {
                string sql = "select spools.id, spool, revision, date_created, spools.status, pipe_size, cut_size1, cut_size2, cut_size3, cut_size4 ";
                sql += " from spools ";
                sql += " left join spool_status on spools.status = spool_status.status ";
                sql += " where ";

                sql += " ( ";
                sql += " (pipe_size is not null and datalength(pipe_size) > 0) ";
                sql += " or ";
                sql += " (cut_size1 is not null and datalength(cut_size1) > 0) ";
                sql += " or ";
                sql += " (cut_size2 is not null and datalength(cut_size2) > 0) ";
                sql += " or ";
                sql += " (cut_size3 is not null and datalength(cut_size3) > 0) ";
                sql += " or ";
                sql += " (cut_size4 is not null and datalength(cut_size4) > 0) ";
                sql += " ) ";

                sql += " and ";
                sql += " spool_status.order_of_production < 40 ";
                sql += " order by " + order_by;

                ArrayList a_cld = new ArrayList();

                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(sql);

                    foreach (DataRow dr in dtab.Rows)
                    {
                        CCutListData cld = new CCutListData();

                        cld.spool_id = (int)dr["id"];
                        cld.spool = dr["spool"].ToString();
                        cld.revision = dr["revision"].ToString();

                        try { cld.date_created = (DateTime)dr["date_created"]; }
                        catch { }
                        
                        cld.status = dr["status"].ToString();

                        cld.pipe_size = dr["pipe_size"].ToString();
                        cld.cut_size1 = dr["cut_size1"].ToString();
                        cld.cut_size2 = dr["cut_size2"].ToString();
                        cld.cut_size3 = dr["cut_size3"].ToString();
                        cld.cut_size4 = dr["cut_size4"].ToString();

                        a_cld.Add(cld);
                    }
                }

                ViewState[VS_CL_DATA] = a_cld;

            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message.ToString();
            }
        }

        void display()
        {
            tblMain.Rows.Clear();

            string[] hdr = new string[] { "Spool", "Revision", "Created", "Status", "Pipe Size", "Cut Size", "Cut Size", "Cut Size", "Cut Size" };

            TableRow r;
            TableCell c;

            r = new TableRow();

            r.BackColor = System.Drawing.Color.FromName("SeaGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblMain.Rows.Add(r);

            ArrayList a_cld = (ArrayList)ViewState[VS_CL_DATA];

            if (a_cld != null)
            {
                System.Drawing.Color bc;
                int i = 0;

                foreach (CCutListData cld in a_cld)
                {
                    r = new TableRow();

                    if (i++ % 2 == 0)
                        bc = System.Drawing.Color.FromName("White");
                    else
                        bc = System.Drawing.Color.FromName("LightGray");

                    r.BackColor = bc;

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.spool));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.revision));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.date_created.ToString("dd/MM/yyyy")));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    c.Controls.Add(new LiteralControl(cld.status));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.pipe_size));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.cut_size1));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.cut_size2));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.cut_size3));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cld.cut_size4));
                    r.Cells.Add(c);
                    tblMain.Rows.Add(r);
                }

                tblMain.Rows.Add(r);
            }
        }

        protected void dlSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortedList sl_sort = (SortedList)ViewState[VS_SORT];
            get_cust_list_data(sl_sort[dlSortBy.Text].ToString());
            display();
        }

        [Serializable]
        class CCutListData
        {
            public int spool_id = 0;
            public string spool = string.Empty;
            public string revision = string.Empty;
            public DateTime date_created = DateTime.MinValue;
            public string status = string.Empty;
            public string pipe_size = string.Empty;
            public string cut_size1 = string.Empty;
            public string cut_size2 = string.Empty;
            public string cut_size3 = string.Empty;
            public string cut_size4 = string.Empty;
        }
    }
}
