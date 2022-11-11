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
    public partial class deliveries1 : System.Web.UI.Page
    {
        ArrayList m_results = new ArrayList();
        SortedList m_barcodes = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dtFrom.SelectedDate = DateTime.Today;
                dtTo.SelectedDate = DateTime.Today;
            }
            else
            {
                m_results = (ArrayList)ViewState["deliveries"];
                m_barcodes = (SortedList)ViewState["barcodes"];

                if (m_barcodes == null)
                    m_barcodes = new SortedList();

                display();
            }
        }

        void search()
        {
            m_barcodes.Clear();

            using (deliveries deliv = new deliveries())
            {
                deliv.order_by = "datetime_stamp ASC";

                DateTime dtfrom = dtFrom.SelectedDate;
                DateTime dtto = dtTo.SelectedDate;
                dtto = dtto.AddHours(23);
                dtto = dtto.AddMinutes(59);
                dtto = dtto.AddSeconds(59);

                m_results = deliv.get_delivery_data(dtfrom, dtto);

                SortedList sl = new SortedList();

                foreach (delivery_data dd in m_results)
                {
                    sl.Clear();

                    sl.Add("delivery_id", dd.id);

                    ArrayList asdd = new ArrayList();

                    using (deliveries dsdl = new deliveries())
                    {
                        asdd = dsdl.get_spool_delivery_data(sl);
                    }

                    ArrayList al_d = new ArrayList();

                    using (spools spls = new spools())
                    {
                        using (modules m = new modules())
                        {
                            foreach (spool_delivery_data sdd in asdd)
                            {
                                sl.Clear();
                                sl.Add("id", sdd.spool_id);

                                if (sdd.assembly_type == spool_delivery_data.SPOOL)
                                {
                                    ArrayList asd = spls.get_spool_data_short(sl);

                                    foreach (spool_data sd in asd)
                                    {
                                        al_d.Add(sd.barcode);
                                    }
                                }
                                else if (sdd.assembly_type == spool_delivery_data.MODULE)
                                {
                                    ArrayList amd = m.get_module_data(sl);

                                    foreach (module_data md in amd)
                                        al_d.Add(md.barcode);
                                }
                            }
                        }
                    }

                    al_d.Sort();

                    m_barcodes.Add(dd.id, al_d);
                }
            }

            ViewState["deliveries"] = m_results;
            ViewState["barcodes"] = m_barcodes;
        }

        void display()
        {
            tblResults.Rows.Clear();

            if (m_results == null)
                return;

            if (m_results.Count == 0)
                return;

            using (users u = new users())
            {
                SortedList sl = new SortedList();

                string[] hdr = new string[] { "Date/Time", "User", "Driver", "Vehicle", "Delivery No.", "Shipped" };

                TableRow r;
                TableCell c;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");

                foreach (string sh in hdr)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblResults.Rows.Add(r);

                string username = string.Empty;

                foreach (delivery_data dd in m_results)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("White");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dd.datetime_stamp.ToString("dd/MM/yyyy HH:mm:ss")));
                    r.Cells.Add(c);

                    sl.Clear();

                    sl.Add("id", dd.user_id);
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
                    c.Controls.Add(new LiteralControl(dd.driver));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dd.vehicle));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(dd.id.ToString()));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    CheckBox chk = new CheckBox();
                    chk.AutoPostBack = true;
                    chk.ID = "chk_shipped_" + dd.id.ToString();
                    chk.Attributes["dd_id"] = dd.id.ToString();
                    chk.Checked = dd.shipped;
                    chk.ToolTip = "Shipped";
                    chk.CheckedChanged += new EventHandler(chk_shipped_CheckedChanged);

                    c.Controls.Add(chk);
                    r.Cells.Add(c);

                    c = new TableCell();
                    HyperLink h = new HyperLink();
                    h.Text = "Delivery Note";
                    h.NavigateUrl = "delivery_note.aspx/delivery_note_" + dd.driver.Replace(" ", "_") + "_" + dd.vehicle.Replace(" ", "_") + "_" + dd.datetime_stamp.ToString("yyyyMMdd_HHmmss") + "?id=" + dd.id.ToString();
                    h.Target = "_blank";

                    c.Controls.Add(h);

                    r.Cells.Add(c);

                    tblResults.Rows.Add(r);

                    if (dd.receipt_notes.Trim().Length > 0)
                    {
                        r = new TableRow();

                        c = new TableCell();
                        c.BackColor = System.Drawing.Color.FromName("LightPink");
                        c.Controls.Add(new LiteralControl(dd.receipt_notes.Trim()));
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);
                    }
                    /*
                    sl.Clear();

                    sl.Add("delivery_id", dd.id);

                    ArrayList asdd = new ArrayList();

                    using (deliveries dsdl = new deliveries())
                    {
                        asdd = dsdl.get_spool_delivery_data(sl);
                    }

                    ArrayList al_d = new ArrayList();


                    using (spools spls = new spools())
                    {
                        using (modules m = new modules())
                        {
                            foreach (spool_delivery_data sdd in asdd)
                            {
                                sl.Clear();
                                sl.Add("id", sdd.spool_id);

                                if (sdd.assembly_type == spool_delivery_data.SPOOL)
                                {
                                    ArrayList asd = spls.get_spool_data(sl);

                                    foreach (spool_data sd in asd)
                                    {
                                        al_d.Add(sd.barcode);
                                    }
                                }
                                else if (sdd.assembly_type == spool_delivery_data.MODULE)
                                {
                                    ArrayList amd = m.get_module_data(sl);

                                    foreach (module_data md in amd)
                                        al_d.Add(md.barcode);
                                }
                            }
                        }
                    }

                    al_d.Sort();
                    */

                    if (m_barcodes.ContainsKey(dd.id))
                    {
                        ArrayList al_d = (ArrayList)m_barcodes[dd.id];

                        foreach (string bc in al_d)
                        {
                            r = new TableRow();
                            r.BackColor = System.Drawing.Color.FromName("LightGray");

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(bc));
                            r.Cells.Add(c);

                            tblResults.Rows.Add(r);
                        }
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        void chk_shipped_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            string uid = (chk.Attributes["dd_id"]);

            SortedList sl = new SortedList();

            int id = 0;

            try { id = Convert.ToInt32(uid); }
            catch { }

            sl.Add("id", id);
            sl.Add("shipped", chk.Checked);

            using (deliveries d = new deliveries())
            {
                d.save_delivery_data(sl);
            }

            foreach (delivery_data dd in m_results)
            {
                if (dd.id == id)
                {
                    dd.shipped = chk.Checked;
                    break;
                }
            }

            ViewState["deliveries"] = m_results;
        }
    }
}
