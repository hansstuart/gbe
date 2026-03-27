using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

namespace gbe
{
    public partial class weld_test_report_admin : System.Web.UI.Page
    {
        const string LIGHTGREEN = "LightGreen";
        const string WHITE = "White";
        const string RED = "LightPink";

        const string VS_REPORT_DATA = "VS_REPORT_DATA";
        const string VS_IRISNDT_UPLOAD_ID = "VS_IRISNDT_UPLOAD_ID";
        const int CW1 = 320;
        const string UID = "uid";

        ArrayList m_report_data = null;
        int m_current_irisndt_upload_id = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (IsPostBack)
            {
                m_report_data = (ArrayList)ViewState[VS_REPORT_DATA];
                try { m_current_irisndt_upload_id = (int)ViewState[VS_IRISNDT_UPLOAD_ID]; } catch { }
                display_report_data();
            }
            else
            {
                get_report_data();
                display_report_data();
            }
        }

        void get_report_data()
        {
            if(m_report_data == null)
                m_report_data = new ArrayList();

            m_report_data.Clear();

            using (irisndt_upload iu = new irisndt_upload())
            {
                m_report_data = iu.get_upload_data($"{irisndt_upload.ID}, {irisndt_upload.DT}, {irisndt_upload.FILENAME}, {irisndt_upload.PROCESSED}, {irisndt_upload.BARCODE}", null);
            }

            ViewState[VS_REPORT_DATA] = m_report_data;
        }
        
        void display_report_data()
        {
            tblReports.Rows.Clear();

            if (m_report_data != null && m_report_data.Count > 0)
            {
                TableRow r;
                TableCell c;

                string[] hdr = new string[] { "File", "Date" };

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName(LIGHTGREEN);

                foreach (string sh in hdr)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblReports.Rows.Add(r);

                foreach (irisndt_upload_data iud in m_report_data)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName(WHITE);

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    c.Controls.Add(new LiteralControl(iud.filename));
                    c.Width = CW1;
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.VerticalAlign = VerticalAlign.Top;
                    c.Controls.Add(new LiteralControl(iud.dt.ToString("dd/MM/yyyy HH:mm")));
                    r.Cells.Add(c);

                    if (iud.processed == irisndt_upload.PROCESS_STATUS_PROCESSING_PENDING)
                    {
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.VerticalAlign = VerticalAlign.Top;
                        c.Controls.Add(new LiteralControl("** Update pending **"));
                        r.Cells.Add(c);
                    }
                    else if (iud.processed == irisndt_upload.PROCESS_STATUS_NO_BARCODES_IN_PDF)
                    {
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.BackColor = System.Drawing.Color.FromName(RED);
                        c.VerticalAlign = VerticalAlign.Top;
                        c.Controls.Add(new LiteralControl("** Barcodes missing **"));
                        r.Cells.Add(c);
                    }
                    else if (iud.barcodes.Count > 0)
                    {
                        c = new TableCell();
                         
                        Table tblBarcode = new Table();
                        tblBarcode.GridLines = GridLines.Both;
                        tblBarcode.Font.Size = 10;
                        
                        TableRow r1 = null;
                        TableCell c1;

                        iud.barcodes.Sort();

                        const int COLS = 2;

                        for (int i = 0; i < iud.barcodes.Count; i++)
                        {
                            if (i % COLS == 0)
                            {
                                r1 = new TableRow();
                                tblBarcode.Rows.Add(r1);
                            }

                            c1 = new TableCell();
                            c1.Controls.Add(new LiteralControl(iud.barcodes[i].ToString()));
                            r1.Cells.Add(c1);
                        }

                        c.Controls.Add(tblBarcode);
                        r.Cells.Add(c);
                    }

                    if (iud.processed != irisndt_upload.PROCESS_STATUS_PROCESSING_PENDING)
                    {
                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        c.VerticalAlign = VerticalAlign.Top;
                        ImageButton btn_add_remove_spools = new ImageButton();
                        btn_add_remove_spools.ToolTip = "Add/remove spools";
                        btn_add_remove_spools.ImageUrl = "~/modify.png";
                        btn_add_remove_spools.Click += new ImageClickEventHandler(btn_add_remove_spools_Click);
                        btn_add_remove_spools.ID = "btn_add_remove_spools" + iud.id.ToString();
                        btn_add_remove_spools.Attributes[UID] = iud.id.ToString();

                        c.Controls.Add(btn_add_remove_spools);
                        r.Cells.Add(c);
                    }

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    c.VerticalAlign = VerticalAlign.Top;
                    ImageButton btn_view_report  = new ImageButton();
                    btn_view_report.ToolTip = "View";
                    btn_view_report.ImageUrl = "~/pdf.png";
                    btn_view_report.Click += new ImageClickEventHandler(btn_view_report_Click);
                    btn_view_report.ID = "btn_view_report" + iud.id.ToString();
                    btn_view_report.Attributes[UID] = iud.id.ToString();

                    c.Controls.Add(btn_view_report);
                    r.Cells.Add(c);
                    
                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    c.VerticalAlign = VerticalAlign.Top;
                    ImageButton btn_delete_report  = new ImageButton();
                    btn_delete_report.OnClientClick = "Confirm()";
                    btn_delete_report.ToolTip = "Delete";
                    btn_delete_report.ImageUrl = "~/delete.png";
                    btn_delete_report.Click +=  new ImageClickEventHandler(btn_delete_report_Click);
                    btn_delete_report.ID = "btn_delete_report" + iud.id.ToString();
                    btn_delete_report.Attributes[UID] = iud.id.ToString();

                    c.Controls.Add(btn_delete_report);
                    r.Cells.Add(c);

                    tblReports.Rows.Add(r);
                }
            }
        }

        private void btn_add_remove_spools_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            try { m_current_irisndt_upload_id = Convert.ToInt32(b.Attributes[UID]); } catch {m_current_irisndt_upload_id = 0; }

            ViewState[VS_IRISNDT_UPLOAD_ID] = m_current_irisndt_upload_id;

            if (m_current_irisndt_upload_id > 0)
            {
                lstBarcodes.Items.Clear();

                foreach (irisndt_upload_data iud in m_report_data)
                {
                    if (iud.id == m_current_irisndt_upload_id)
                    {
                        lblReport.Text = $"Report: {iud.filename}";

                        foreach(string barcode in iud.barcodes)
                            lstBarcodes.Items.Add(barcode);
                    }
                }

                MultiView1.ActiveViewIndex = 1;
            }
        }

        private void btn_delete_report_Click(object sender, ImageClickEventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                ImageButton b = (ImageButton)sender;

                try { m_current_irisndt_upload_id = Convert.ToInt32(b.Attributes[UID]); } catch {m_current_irisndt_upload_id = 0; }
                ViewState[VS_IRISNDT_UPLOAD_ID] = m_current_irisndt_upload_id;
                
                irisndt_upload upld = new irisndt_upload();

                upld.delete_upload(m_current_irisndt_upload_id);

                using (cdb_connection dbc = new cdb_connection())
                {
                    const string WELD_TEST_REPORTS_TBL = "weld_test_reports";
                    const string IRISNDT_UPLOAD_ID = "irisndt_upload_id";
                    SortedList sl_p = new SortedList();
                    sl_p.Clear();
                    sl_p.Add(IRISNDT_UPLOAD_ID, m_current_irisndt_upload_id);
                    dbc.delete_record(WELD_TEST_REPORTS_TBL, sl_p);
                }

                get_report_data();
                display_report_data();
            }
        }

        private void btn_view_report_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes[UID]);
            string url = string.Empty;

            url = "view_weld_test_report.aspx?id=" + uid;

            Response.Write("<script>");
            Response.Write("window.open('" + url + "','_blank','','false')");
            Response.Write("</script>");
        }

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();

            if (barcode.Length > 0)
            {
                bool bAdd = true;

                foreach (ListItem li in lstBarcodes.Items)
                {
                    if (li.Text.ToLower() == barcode.ToLower())
                    {
                        bAdd = false;
                        break;
                    }
                }

                if (bAdd)
                {
                    lstBarcodes.Items.Add(barcode);
                    txtBarcode.Text = string.Empty;
                }
            }

            txtBarcode.Focus();
        }

        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList a_i = new ArrayList();

            for (int i = 0; i < lstBarcodes.Items.Count; i++)
            {
                if (lstBarcodes.Items[i].Selected)
                    a_i.Add(i);
            }

            for (int i = a_i.Count - 1; i >= 0; i--)
                lstBarcodes.Items.RemoveAt((int)a_i[i]);

            txtBarcode.Focus();
        }

        protected void btnSaveSpools_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (lstBarcodes.Items.Count == 0)
            {
                lblMsg.Text = "Spool barcode(s) missing";
                return;
            }

            ArrayList a_spool_ids = new ArrayList();
            ArrayList a_barcodes = new ArrayList();

            using (cdb_connection dbc = new cdb_connection())
            {
                int i = 0;
                SortedList sl_p = new SortedList();

                foreach (ListItem li in lstBarcodes.Items)
                {
                    string barcode = li.Text.Trim();

                    if(!a_barcodes.Contains(barcode))
                        a_barcodes.Add(barcode);
                
                    string sql_select = "select id from spools where barcode = @barcode  ";
                        
                    sl_p.Clear();
                    sl_p.Add("@barcode", barcode);

                    DataTable dtab = dbc.get_data_p(sql_select, sl_p);

                    if (dtab.Rows.Count == 0)
                    {
                        lblMsg.Text = "Spool not found";
                        lstBarcodes.SelectedIndex = i;
                        return;
                    }
                    else
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            int id = 0;

                            try {id = (int)dr["id"]; } catch { }

                            if (id > 0)
                            {
                                if (!a_spool_ids.Contains(id))
                                {
                                    a_spool_ids.Add(id);
                                }
                            }
                        }
                    }

                    i++;
                }

                const string WELD_TEST_REPORTS_TBL = "weld_test_reports";
                const string IRISNDT_UPLOAD_ID = "irisndt_upload_id";
                const string SPOOL_ID = "spool_id";

                sl_p.Clear();
                sl_p.Add(IRISNDT_UPLOAD_ID, m_current_irisndt_upload_id);
                dbc.delete_record(WELD_TEST_REPORTS_TBL, sl_p);

                foreach (int spool_id in a_spool_ids)
                {
                    sl_p.Clear();
                    sl_p.Add(IRISNDT_UPLOAD_ID, m_current_irisndt_upload_id);
                    sl_p.Add(SPOOL_ID, spool_id);
                    dbc.insert(WELD_TEST_REPORTS_TBL, sl_p);
                }

                using (irisndt_upload iu = new irisndt_upload())
                {
                    sl_p.Clear();
                    sl_p.Add(irisndt_upload.ID, m_current_irisndt_upload_id);
                    sl_p.Add(irisndt_upload.PROCESSED, irisndt_upload.PROCESS_STATUS_PROCESSED);

                    iu.save_irisndt_upload_data(sl_p);
                }

                foreach (irisndt_upload_data iud in m_report_data)
                {
                    if (iud.id == m_current_irisndt_upload_id)
                    {
                        iud.processed = irisndt_upload.PROCESS_STATUS_PROCESSED;

                        iud.barcodes.Clear();

                        foreach (string barcode in a_barcodes)
                        {
                            iud.barcodes.Add(barcode);
                        }

                        ViewState[VS_REPORT_DATA] = m_report_data;

                        break;
                    }
                }
                
                
            }

            MultiView1.ActiveViewIndex = 0;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
        }

        protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        {
            if (MultiView1.ActiveViewIndex == 0)
            {
                txtBarcode.Text = string.Empty;
                lstBarcodes.Items.Clear();

                display_report_data();
            }
            else if (MultiView1.ActiveViewIndex == 1)
            {
                txtBarcode.Focus();
            }
        }
    }
}

