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
    public partial class invoicing : System.Web.UI.Page
    {
        ArrayList m_results = new ArrayList();

        string[][] table_title = new string[][] 
        {
            new string[] { "Summary:" },
            new string[] { "Detailed breakdown:" }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_results = (ArrayList)ViewState["results"];
                display();
            }
            else
            {
                txtSpool.Focus();
                
                chkExportSummary.Checked = chkExportBreakdown.Checked = true;

                dtFrom.SelectedDate = DateTime.Now;
                dtTo.SelectedDate = DateTime.Now;

                ViewState["results"] = m_results;
            }
        }

        void display()
        {
            TableRow r;
            TableCell c;

            string total_sale_cost_hdr = "Total Sale Cost";
            string total_material_cost_hdr = "Total Material Cost";

            string[][] hdr = new string[][] 
            {
                new string[] { "Project", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, total_material_cost_hdr, total_sale_cost_hdr },
                new string[] { "Spool", "Status", "Completed", "Part", "Cost Centre", "Qty", "Material Cost", "Sale Cost" , total_material_cost_hdr, total_sale_cost_hdr }
            };

            tblResults.Rows.Clear();

            if (m_results.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    decimal total_sale_cost = 0;
                    decimal inv_total_sale_cost = 0;

                    decimal total_material_cost = 0;
                    decimal inv_total_material_cost = 0;

                    string contract = string.Empty;
                    string spool  = string.Empty;

                    r = new TableRow();

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(table_title[i][0]));
                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    int n = 0;

                    SortedList sl = new SortedList();
                    foreach (weld_job_data wjd in m_results)
                    {
                        sl.Add(wjd.spool_data.barcode + sl.Count.ToString("0000000"), wjd);
                    }

                    foreach (DictionaryEntry e0 in sl)
                    {
                        weld_job_data wjd = (weld_job_data)e0.Value;
                        if (wjd.spool_data != null)
                        {
                            if (wjd.spool_data.welder_data != null || wjd.spool_data.fitter_data != null)
                            {
                                DateTime dt_completed = wjd.finish;

                                if (wjd.finish_fit > wjd.finish)
                                    dt_completed = wjd.finish_fit;

                                string[] sa = wjd.spool_data.spool.Split('-');

                                string this_contract = string.Empty;

                                if (sa.Length > 0)
                                {
                                    this_contract = sa[0].ToUpper();
                                }

                                if (contract != this_contract)
                                {
                                    

                                    if (n++ > 0)
                                    {
                                        r = new TableRow();
                                        r.BackColor = System.Drawing.Color.FromName((i == 1 ? "LightGray" : "White"));

                                        string s = string.Empty;

                                        HorizontalAlign ha;

                                        foreach (string sh in hdr[0])
                                        {
                                            if (sh == total_sale_cost_hdr)
                                            {
                                                s = total_sale_cost.ToString("0.00");
                                                ha = HorizontalAlign.Right;
                                            }
                                            else
                                            {
                                                if (i == 0 && sh == "Project")
                                                    s = contract;
                                                else
                                                    s = string.Empty;
                                                ha = HorizontalAlign.Left;
                                            }

                                            c = new TableCell();
                                            c.HorizontalAlign = ha;
                                            c.Controls.Add(new LiteralControl(s));
                                            r.Cells.Add(c);
                                        }

                                        tblResults.Rows.Add(r);

                                        total_sale_cost = 0;
                                        total_material_cost = 0;
                                    }

                                    if ((i == 0 && tblResults.Rows.Count == 1) || i > 0)
                                    {
                                        r = new TableRow();
                                        r.BackColor = System.Drawing.Color.FromName("LightGreen");

                                        foreach (string sh in hdr[i])
                                        {
                                            c = new TableCell();
                                            c.Controls.Add(new LiteralControl(sh));
                                            r.Cells.Add(c);
                                        }

                                        tblResults.Rows.Add(r);
                                    }

                                    contract = this_contract;
                                }

                                string cc = string.Empty;

                                foreach (spool_part_data spd in wjd.spool_data.spool_part_data)
                                {
                                    decimal sale_cost = 0;

                                    if (wjd.spool_data.cost_centre == 0)
                                        sale_cost = spd.part_data.gbe_sale_cost;
                                    else if (wjd.spool_data.cost_centre == 1)
                                        sale_cost = spd.part_data.pipecenter_sale_cost;
                                    else if (wjd.spool_data.cost_centre == 2)
                                        sale_cost = spd.part_data.olmat_group_sale_cost;
                                    else if (wjd.spool_data.cost_centre == 3)
                                        sale_cost = spd.part_data.dgr_fab_and_mat;
                                    else if (wjd.spool_data.cost_centre == 4)
                                        sale_cost = spd.part_data.dgr_fab_only;
                                    else if (wjd.spool_data.cost_centre == 5)
                                        sale_cost = spd.part_data.buxton_mcnulty_sale_cost;
                                    else if (wjd.spool_data.cost_centre == 6)
                                        sale_cost = spd.part_data.associated_pipework_fab_only;
                                    else if (wjd.spool_data.cost_centre == 7)
                                        sale_cost = spd.part_data.generic_sale_cost;
                                    else if (wjd.spool_data.cost_centre == 8)
                                        sale_cost = spd.part_data.watkins;
                                    
                                    decimal line_total_sale_cost = (spd.qty * sale_cost);
                                    total_sale_cost += line_total_sale_cost;
                                    inv_total_sale_cost += line_total_sale_cost;

                                    decimal line_total_material_cost = (spd.qty * spd.part_data.material_cost);
                                    total_material_cost += line_total_material_cost;
                                    inv_total_material_cost += line_total_material_cost;

                                    spool = wjd.spool_data.spool + " / " + wjd.spool_data.revision;

                                    if (i == 1)
                                    {
                                        r = new TableRow();
                                        r.BackColor = System.Drawing.Color.FromName("White");

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(spool));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(wjd.spool_data.status));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(dt_completed.ToString("dd/MM/yy")));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.Controls.Add(new LiteralControl(spd.part_data.description));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Center;

                                        cost_centre costcentre = new cost_centre();

                                        cc = costcentre.m_sl_cost_centre[wjd.spool_data.cost_centre].ToString();
                                        
                                        c.Controls.Add(new LiteralControl(cc));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(spd.qty.ToString("0.00")));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(spd.part_data.material_cost.ToString("0.00")));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(sale_cost.ToString("0.00")));
                                        r.Cells.Add(c);

                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(line_total_material_cost.ToString("0.00")));
                                        r.Cells.Add(c);
                                        
                                        c = new TableCell();
                                        c.HorizontalAlign = HorizontalAlign.Right;
                                        c.Controls.Add(new LiteralControl(line_total_sale_cost.ToString("0.00")));
                                        r.Cells.Add(c);

                                        tblResults.Rows.Add(r);
                                    }
                                }
                            }
                        }
                    }
                

                    if (tblResults.Rows.Count > 1)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName((i == 1 ? "LightGray" : "White"));

                        string s = string.Empty;

                        HorizontalAlign ha;

                        foreach (string sh in hdr[0])
                        {
                            if (sh == total_sale_cost_hdr)
                            {
                                s = total_sale_cost.ToString("0.00");
                                ha = HorizontalAlign.Right;
                            }
                            else if(sh == total_material_cost_hdr)
                            {
                                s = total_material_cost.ToString("0.00");
                                ha = HorizontalAlign.Right;
                            }
                            else
                            {
                                if (i == 0 && sh == "Project")
                                    s = contract;
                                else
                                    s = string.Empty;
                                ha = HorizontalAlign.Left;
                            }

                            c = new TableCell();
                            c.HorizontalAlign = ha;
                            c.Controls.Add(new LiteralControl(s));
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);

                        total_sale_cost = 0;
                    }

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightPink");

                    string s1 = string.Empty;

                    HorizontalAlign ha1;

                    foreach (string sh in hdr[0])
                    {
                        if (sh == total_sale_cost_hdr)
                        {
                            s1 = inv_total_sale_cost.ToString("0.00");
                            ha1 = HorizontalAlign.Right;
                        }
                        else if (sh == total_material_cost_hdr)
                        {
                            s1 = inv_total_material_cost.ToString("0.00");
                            ha1 = HorizontalAlign.Right;
                        }
                        else
                        {
                            s1 = string.Empty;
                            ha1 = HorizontalAlign.Left;
                        }

                        c = new TableCell();
                        c.HorizontalAlign = ha1;
                        c.Controls.Add(new LiteralControl(s1));
                        r.Cells.Add(c);
                    }

                    tblResults.Rows.Add(r);

                }
            }
        }

        void search()
        {
            using (cdb_connection db_connection = new cdb_connection())
            {
                ArrayList spool_ids = new ArrayList();

                tblResults.Rows.Clear();
                m_results.Clear();
                db_connection.connect();

                ViewState["results"] = m_results;

                if (txtSpool.Text.Trim().Length > 3)
                {
                    SortedList sl = new SortedList();

                    string fld, val;
                    fld = val = string.Empty;

                    val = txtSpool.Text.Trim();

                    /*
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

                    if (chkShipped.Checked)
                    {
                        sl.Add("status", "SH");
                    }

                    using (spools spls = new spools())
                    {
                        foreach (spool_data sd in spls.get_spool_data(sl))
                        {
                            if (sd.status == "QA" || sd.status == "RD" || sd.status == "WT" || sd.status == "SH" || sd.status == "OS" || sd.status == "IN")
                                spool_ids.Add(sd.id);
                        }
                    }

                    if (spool_ids.Count == 0)
                        return;
                     */
                }
                else if (txtSpool.Text.Trim().Length != 0)
                {
                    db_connection.disconnect();
                    return;
                }

                DateTime dtfrom = dtFrom.SelectedDate;
                DateTime dtto = dtTo.SelectedDate;

                using (weld_jobs wj = new weld_jobs(db_connection.m_sql_connection))
                {
                    m_results = wj.get_weld_job_data(0, 0, dtfrom, dtto, txtSpool.Text.Trim(), "spool_id");
                }

                ViewState["results"] = m_results;

                db_connection.disconnect();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (!chkExportBreakdown.Checked && !chkExportSummary.Checked)
                return;

            string[] hdr = new string[5] { "spool", "part", "qty", "sale_cost", "total" };
            
            if (tblResults.Rows.Count > 0)
            {
                string CRLF = "\r\n";
                string DC = "\"";
                string CM = ",";

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=invoice_" + txtSpool.Text.Trim() + "_" + dtFrom.SelectedDate.ToString("yyyyMMdd") + "-" + dtTo.SelectedDate.ToString("yyyyMMdd") + ".csv");

                string line = string.Empty;

                bool bexport = false;

                foreach (TableRow r in tblResults.Rows)
                {
                    line = string.Empty;

                    foreach (TableCell c in r.Cells)
                    {
                        if (((LiteralControl)(c.Controls[0])).Text == table_title[0][0])
                        {
                            if (chkExportSummary.Checked)
                                bexport = true;
                            else
                                bexport = false;

                        }
                        if (((LiteralControl)(c.Controls[0])).Text == table_title[1][0])
                        {
                            if (chkExportBreakdown.Checked)
                                bexport = true;
                            else
                                bexport = false;
                        }

                        if (bexport)
                        {
                            if (line.Length > 0)
                                line += CM;

                            if (c.HorizontalAlign != HorizontalAlign.Right)
                                line += DC;

                            line += ((LiteralControl)(c.Controls[0])).Text;

                            if (c.HorizontalAlign != HorizontalAlign.Right)
                                line += DC;
                        }
                    }

                    if (bexport)
                    {
                        line += CRLF;

                        Response.Write(line);
                    }
                }
                /*
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

                    line = DC + ((LiteralControl)(r.Cells[0].Controls[0])).Text + DC + CM;
                    line += DC + ((LiteralControl)(r.Cells[1].Controls[0])).Text + DC + CM;
                    line += ((LiteralControl)(r.Cells[2].Controls[0])).Text + CM;
                    line += ((LiteralControl)(r.Cells[3].Controls[0])).Text + CM;
                    line += ((LiteralControl)(r.Cells[4].Controls[0])).Text + CM + CRLF;

                    Response.Write(line);
                }
                */
                Response.End();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtEmailAddress.Text.Trim().Length > 0)
                {
                    DateTime dt_from = dtFrom.SelectedDate;
                    DateTime dt_to = dtTo.SelectedDate;

                    SortedList sl = new SortedList();

                    sl.Add("spool", txtSpool.Text.Trim());
                    sl.Add("email_address", txtEmailAddress.Text.Trim());
                    sl.Add("dt_from", dt_from);
                    sl.Add("dt_to", dt_to);
                    sl.Add("bprocessed", false);

                    using (invoice_request ir = new invoice_request())
                    {
                        ir.save_invoice_request_data(sl);
                    }

                    lblMsg.Text = "The invoice spreadsheet will be emailed shortly.";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();
            }
        }
    }
}
