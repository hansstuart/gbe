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
    public partial class consignment_view : System.Web.UI.Page
    {
        const string CDVS = "cd_vs";
        const string LDVSID = "ld_vs_id";
        const string LDVSLOC = "ld_vs_loc";
        const string ADDATVS = "add_at_vs";
        const string PARTSVS = "parts_vs";
        const string TB_PROJECT = "txtproject_";
        const string TB_OWNER = "txtowner_";
        const string TB_QTY = "txtqty_";
        const string TB_BARCODE = "txtbarcode_";
        const string DL_LOC = "dlloc_";
        const string BTN_SAVE = "btnsave_";
        const string BTN_SAVE_NEW = "btnsavenew_";
        const string BTN_ADD_INST = "btnaddinst_";

        SortedList m_results = new SortedList();
        SortedList m_locations_id = new SortedList();
        SortedList m_locations_loc = new SortedList();
        SortedList m_add_at = new SortedList();
        SortedList m_parts = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_locations_id = (SortedList)ViewState[LDVSID];
                m_locations_loc = (SortedList)ViewState[LDVSLOC];
                m_results = (SortedList)ViewState[CDVS];
                m_add_at = (SortedList)ViewState[ADDATVS];
                m_parts = (SortedList)ViewState[PARTSVS];

                display();
            }
            else
            {
                dlSearchFlds.Items.Add("description");
                dlSearchFlds.Items.Add("owner");
                dlSearchFlds.Items.Add("project");

                m_locations_id.Add(0, string.Empty);
                m_locations_loc.Add(string.Empty, 0);

                using (locations l = new locations())
                {
                    ArrayList al = l.get_location_data(new SortedList());

                    foreach (location_data ld in al)
                    {
                        m_locations_id.Add(ld.id, ld.location);

                        if (!m_locations_loc.ContainsKey(ld.location))
                            m_locations_loc.Add(ld.location, ld.id);    
                    }
                }

                ViewState[LDVSID] = m_locations_id;
                ViewState[LDVSLOC] = m_locations_loc;
            }

            txtSearch.Focus();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            search();
            display();
        }

        void display()
        {
            if (m_results == null)
                return;

            TableRow r;
            TableCell c;

            tblResults.Rows.Clear();

            foreach (DictionaryEntry e0 in m_results)
            {
                SortedList sl_c = (SortedList)e0.Value;

                int i = 0;
                foreach (DictionaryEntry e1 in sl_c)
                {
                    consignment_instance_data cd = (consignment_instance_data)e1.Value;

                    if (i == 0)
                    {
                        i++;

                        // hdr
                        string[] hdr = new string[] { "Description", "Additional Description", "Part No.", "Size", "Manufacturer" };
                        string[] sub_hdr = new string[] { string.Empty, "Project No. (G00xxx)", "Owner", "Quantity", "Location", "Barcode" };

                        r = new TableRow();

                        r.BackColor = System.Drawing.Color.FromName("LightGreen");

                        foreach (string sh in hdr)
                        {
                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(sh));
                            r.Cells.Add(c);
                        }

                        tblResults.Rows.Add(r);

                        // part info
                        r = new TableRow();

                        r.BackColor = System.Drawing.Color.FromName("LightGray");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cd.description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cd.additional_description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cd.part_number));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cd.size));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cd.manufacturer));
                        r.Cells.Add(c);

                        tblResults.Rows.Add(r);

                        r = new TableRow();

                        //subhdr
                        r.BackColor = System.Drawing.Color.FromName("LightPink");

                        int n = 0;
                        foreach (string sh in sub_hdr)
                        {
                            c = new TableCell();

                            if (n == 0)
                            {
                                c.BackColor = System.Drawing.Color.FromName("LightBlue");
                                n++;
                            }

                            c.Controls.Add(new LiteralControl(sh));
                            r.Cells.Add(c);
                        }

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;
                        ImageButton btn_add_inst = new ImageButton();
                        btn_add_inst.ToolTip = "Add CFII instance";
                        btn_add_inst.ImageUrl = "~/add.png";
                        btn_add_inst.Click += new ImageClickEventHandler(btn_add_consigment_instance_Click);
                        btn_add_inst.ID = BTN_ADD_INST + cd.part_id.ToString();
                        btn_add_inst.Attributes["uid"] = cd.part_id.ToString();

                        r.ID = cd.part_id.ToString();

                        c.Controls.Add(btn_add_inst);
                        r.Cells.Add(c);
                        tblResults.Rows.Add(r);

                        
                    }

                    add_controls(cd, -1);

                }
            }

            if (m_add_at != null)
            {
                foreach (DictionaryEntry e0 in m_add_at)
                {
                    new_consignment_instance nci = (new_consignment_instance)e0.Value;
                    add_controls(nci.ci, nci.row);
                }
            }
        }

        void add_controls(consignment_instance_data cd, int add_at)
        {
            string sid = string.Empty;
            string btn_save_id = string.Empty;

            if (cd.id == 0)
            {
                sid = cd.guid; 
                btn_save_id = BTN_SAVE_NEW + sid;
            }
            else
            {
                sid = cd.id.ToString();
                btn_save_id = BTN_SAVE + sid;
            }

            TableRow r;
            TableCell c;

            // consignment instances
            r = new TableRow();

            c = new TableCell();
            c.BackColor = System.Drawing.Color.FromName("LightBlue");
            c.Controls.Add(new LiteralControl(string.Empty));
            r.Cells.Add(c);

            TextBox textbox = new TextBox();

            // project
            c = new TableCell();
            textbox.MaxLength = 50;
            textbox.Text = cd.project;
            textbox.ID = TB_PROJECT + sid;
            c.Controls.Add(textbox);
            r.Cells.Add(c);

            // owner
            textbox = new TextBox();
            textbox.MaxLength = 50;
            textbox.ID = TB_OWNER + sid;
            c = new TableCell();
            textbox.Text = cd.owner;
            c.Controls.Add(textbox);
            r.Cells.Add(c);

            // qty
            textbox = create_decimal_textbox(TB_QTY + sid);
            c = new TableCell();
            textbox.Text = cd.qty_in_stock.ToString("0.00");
            c.Controls.Add(textbox);
            r.Cells.Add(c);

            // loc
            DropDownList dl = new DropDownList();
            dl.ID = DL_LOC + sid;

            foreach (DictionaryEntry e2 in m_locations_loc)
                dl.Items.Add(e2.Key.ToString());

            if (m_locations_id.ContainsKey(cd.location_id))
                dl.Text = m_locations_id[cd.location_id].ToString();

            c = new TableCell();
            c.Controls.Add(dl);
            r.Cells.Add(c);

            // barcode
            textbox = new TextBox();
            textbox.MaxLength = 50;
            textbox.ID = TB_BARCODE + sid;
            c = new TableCell();
            textbox.Text = cd.barcode;
            c.Controls.Add(textbox);
            r.Cells.Add(c);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            ImageButton btn_save_part = new ImageButton();
            btn_save_part.ToolTip = "Save changes";
            btn_save_part.ImageUrl = "~/disk.png";
            btn_save_part.Click += new ImageClickEventHandler(btn_save_part_Click);
            btn_save_part.ID = btn_save_id;
            btn_save_part.Attributes["uid"] = sid;
            btn_save_part.Attributes["part_id"] = cd.part_id.ToString();

            c.Controls.Add(btn_save_part);
            r.Cells.Add(c);

            if(add_at>0)
                tblResults.Rows.AddAt(add_at, r);
            else
                tblResults.Rows.Add(r);

        }

        void search()
        {
            if (m_add_at == null)
                m_add_at = new SortedList();
            else
                m_add_at.Clear();

            SortedList sl = new SortedList();

            if (txtSearch.Text.Trim().Length > 0)
            {
                if (dlSearchFlds.Text == "description")
                {
                    using (parts p = new parts())
                    {
                        sl.Add("description", txtSearch.Text.Trim()+"%");

                        ArrayList ap = p.get_part_data(sl);

                        if (ap.Count > 0)
                        {
                            ArrayList a_part_ids = new ArrayList();

                            foreach(part_data pd in ap)
                            {
                                a_part_ids.Add(pd.id);
                            }
                            
                            sl.Clear();
                            sl.Add("part_id", a_part_ids);
                        }
                        else
                            return;
                    }
                }
                else
                {
                    sl.Add(dlSearchFlds.Text, txtSearch.Text.Trim());
                }
            }

            get_and_store_results(sl);
        }

        void get_and_store_results(SortedList sl)
        {
            if (m_results == null)
                m_results = new SortedList();
            else
                m_results.Clear();

            if (sl.Count == 1)
            {
                using (consignment_instance c = new consignment_instance())
                {
                    ArrayList a = null;

                    if (sl.GetKey(0).ToString() == "part_id")
                    {
                        ArrayList a_part_ids = (ArrayList)sl["part_id"];
                        a = c.get_consignment_data(a_part_ids);
                    }
                    else
                        a = c.get_consignment_data(sl);

                    foreach (consignment_instance_data cd in a)
                    {
                        if (!m_results.ContainsKey(cd.description))
                            m_results.Add(cd.description, new SortedList());

                        SortedList sl_c = (SortedList)m_results[cd.description];
                        sl_c.Add(cd.project + cd.id.ToString(), cd);
                    }
                }
            }

            ViewState[CDVS] = m_results;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 1;
        }

        TextBox create_decimal_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 50;
            tb.MaxLength = 8;
            tb.Text = string.Empty;
            tb.Attributes.Add("onkeypress", "return onlyDotsAndNumbers(this, event)");
            tb.ID = id;

            return tb;
        }

        void btn_save_part_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            ImageButton btn_save = (ImageButton)sender;

            int id = 0;
            int part_id = 0;
            int location_id = 0;
            decimal qty = 0;
            string project = string.Empty;
            string owner = string.Empty;
            string barcode = string.Empty;
            int nfound = 0;

            string uid = (btn_save.Attributes["uid"]);
            bool bsave_new = false;

            if (btn_save.ID.StartsWith(BTN_SAVE))
            {
                try { id = Convert.ToInt32(uid); }
                catch
                {
                    lblMsg.Text = "Failed to identify instance id";
                    return;
                }
            }
            else if (btn_save.ID.StartsWith(BTN_SAVE_NEW))
            {
                bsave_new = true;

                string spartid = (btn_save.Attributes["part_id"]);

                try { part_id = Convert.ToInt32(spartid); }
                catch
                {
                    lblMsg.Text = "Failed to identify part id";
                    return;
                }
            }
            TextBox txt_project, txt_owner, txt_barcode, txt_qty;
            DropDownList dl_loc = null;
            txt_project =  txt_owner =  txt_barcode = txt_qty = null;
            foreach (TableRow r in tblResults.Rows)
            {
                foreach (TableCell c in r.Cells)
                {
                    foreach (Control cntrl in c.Controls)
                    {
                        if (cntrl.ID != null)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                TextBox tb = (TextBox)cntrl;

                                if (cntrl.ID.EndsWith(TB_PROJECT + uid))
                                {
                                    txt_project = tb;
                                    project = tb.Text.Trim().ToUpper();
                                    nfound++;
                                }
                                else if (cntrl.ID.EndsWith(TB_OWNER + uid))
                                {
                                    txt_owner = tb;
                                    owner = tb.Text.Trim();
                                    nfound++;
                                }
                                else if (cntrl.ID.EndsWith(TB_BARCODE + uid))
                                {
                                    txt_barcode = tb;
                                    barcode = tb.Text.Trim();
                                    nfound++;
                                }
                                else if (cntrl.ID.EndsWith(TB_QTY + uid))
                                {
                                    txt_qty = tb;
                                    try { qty = Convert.ToDecimal(tb.Text); }
                                    catch { }
                                    nfound++;
                                }
                            }
                            else if (cntrl.GetType() == typeof(DropDownList))
                            {
                                DropDownList dl = (DropDownList)cntrl;

                                dl_loc = dl;

                                if (cntrl.ID.EndsWith(DL_LOC + uid))
                                {
                                    location_id = (int)m_locations_loc[dl.Text];
                                    nfound++;
                                }
                            }
                        }

                        if (nfound == 5)
                        {
                            SortedList sl = new SortedList();

                            if (!bsave_new)
                                sl.Add("id", id);
                            else
                            {
                                sl.Add("part_id", part_id);
                                new_consignment_instance nci = (new_consignment_instance)m_add_at[uid];
                                if(nci.ci.id > 0)
                                    sl.Add("id", nci.ci.id);
                            }

                            sl.Add("project", project);
                            sl.Add("owner", owner);
                            sl.Add("barcode", barcode);
                            sl.Add("qty_in_stock", qty);
                            sl.Add("location_id", location_id);

                            int ci_id = 0;

                            using (consignment_instance ci = new consignment_instance())
                            {
                                ci_id = ci.save_consignment_data(sl);
                            }

                            if (bsave_new)
                            {
                                new_consignment_instance nci = (new_consignment_instance)m_add_at[uid];
                                nci.ci.id = ci_id;
                                nci.ci.project = project;
                                nci.ci.owner = owner;
                                nci.ci.barcode = barcode;
                                nci.ci.qty_in_stock = qty;
                                nci.ci.location_id = location_id;
                                
                                foreach (DictionaryEntry e0 in m_results)
                                {
                                    SortedList sl_c = (SortedList)e0.Value;

                                    if (sl_c.Count > 0)
                                    {
                                        consignment_instance_data ci = (consignment_instance_data)sl_c.GetByIndex(0); ;

                                        if (nci.ci.part_id == ci.part_id)
                                        {
                                            sl_c.Add(nci.ci.project + nci.ci.id.ToString(), nci.ci);

                                            m_add_at.Remove(nci.ci.guid);

                                            ViewState[CDVS] = m_results;
                                            ViewState[ADDATVS] = m_add_at;

                                            display();
                                            break;
                                        }
                                    }
                                         
                                }

                                
                            }

                            return;
                        }
                    }
                }
            }
        }

        void btn_add_consigment_instance_Click(object sender, ImageClickEventArgs e)
        {
            lblMsg.Text = string.Empty;

            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes["uid"]);
            int part_id = 0;

            try { part_id = Convert.ToInt32(uid); }
            catch
            {
                lblMsg.Text = "Failed to identify part id";
                return;
            }

            int i = 0;
            
            foreach (TableRow r0 in tblResults.Rows)
            {
                i++;

                if (r0.ID == uid)
                {
                    if (m_add_at == null)
                        m_add_at = new SortedList();


                    // only one at a time 
                    if (m_add_at.Count > 0)
                        return; 
                    
                    new_consignment_instance nci = new new_consignment_instance();
                    nci.row = i;
                    nci.ci.part_id = part_id;
                    nci.ci.guid = Guid.NewGuid().ToString("N");

                    m_add_at.Add(nci.ci.guid, nci);

                    add_controls(nci.ci, i);

                    ViewState[ADDATVS] = m_add_at;
                    return;
                }
            }
        }

        protected void View2_Activate(object sender, EventArgs e)
        {
            m_parts = (SortedList)ViewState[PARTSVS];

            if (m_parts == null)
            {
                m_parts = new SortedList();

                using(parts p = new parts())
                {
                    p.order_by = "description";

                    ArrayList ap = p.get_part_data(new SortedList());

                    foreach (part_data pd in ap)
                    {
                        string desc = pd.description.Trim();
                        if (desc.Length > 0)
                        {
                            if (pd.additional_description.Trim().Length > 0)
                                desc += " / " + pd.additional_description.Trim();
                            if (pd.size.Trim().Length > 0)
                                desc += " / " + pd.size.Trim();

                            if (!m_parts.ContainsKey(desc))
                            {
                                m_parts.Add(desc, pd.id);
                                dlparts.Items.Add(desc);
                            }
                        }
                    }
                }

                ViewState[PARTSVS] = m_parts;
            }

            dlparts.Focus();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            int part_id = (int)m_parts[dlparts.Text];

            SortedList sl = new SortedList();
            sl.Add("part_id", part_id);

            get_and_store_results(sl);

            if (m_results.Count == 0)
            {
                using (consignment_instance c = new consignment_instance())
                {
                    int id = c.save_consignment_data(sl);

                    get_and_store_results(sl);
                }
            }

            display();
            MultiView1.ActiveViewIndex = 0;
        }
    }
    [Serializable]
    public class new_consignment_instance
    {
        public int row = -1;
        public consignment_instance_data ci = new consignment_instance_data();
    }
}

