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
using System.Text;
using System.IO;

namespace gbe
{
    public partial class cust_delivery : System.Web.UI.Page
    {
        ArrayList m_selected = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_selected = (ArrayList)ViewState["selected"];
            }

            display();
        }

        void display()
        {
            tblSelected.Rows.Clear();

            TableRow r;
            TableCell c;

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");
            c = new TableCell();
            c.Controls.Add(new LiteralControl("Selected for Delivery"));
            r.Cells.Add(c);
            tblSelected.Rows.Add(r);

            if (m_selected != null)
            {
                foreach (part_data pd in m_selected)
                {
                    create_selected_table_entry(pd);
                }
            }
        }

        protected void paint_selected_table()
        {
            System.Drawing.Color bc;
            int i = 0;
            foreach (TableRow r in tblSelected.Rows)
            {
                if (i++ == 0)
                    continue;

                if (i % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("LightGray");

                r.BackColor = bc;
            }
        }

        void btn_remove_part_Click(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string key  = (b.Attributes["key"]);

            ArrayList del_i = new ArrayList();
            int i = 0;

            foreach (TableRow r in tblSelected.Rows)
            {
                if (r.Attributes["key"] == key)
                    del_i.Add(i);

                i++;
            }

            del_i.Reverse();

            foreach (int n in del_i)
            {
                tblSelected.Rows.RemoveAt(n);
            }

            del_i.Clear();
            i = 0;

            foreach (part_data pd in m_selected)
            {
                if (pd.attributes["key"].ToString() == key)
                    del_i.Add(i);

                i++;
            }

            del_i.Reverse();

            foreach (int n in del_i)
            {
                m_selected.RemoveAt(n);
            }

            paint_selected_table();

            ViewState["selected"] = m_selected;
        }

        void btn_select_Click(object sender, EventArgs e)
        {/*
            ImageButton b = (ImageButton)sender;

            string key = (b.Attributes["key"]);

            consignment_data cd = (consignment_data)m_available[key];

            if (m_selected == null)
                m_selected = new SortedList();

            if (!m_selected.ContainsKey(key))
            {
                m_selected.Add(key, cd);

                ViewState["selected"] = m_selected;
                create_selected_table_entry(cd, key);
            }
           */
        }

        void create_selected_table_entry(part_data pd)
        {
            string key = pd.id.ToString();

            TableRow r;
            TableCell c;

            r = new TableRow();

            c = new TableCell();

            c.Controls.Add(new LiteralControl(create_table_entry_desc(pd)));
            r.Cells.Add(c);

            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Right;
            c.Controls.Add(new LiteralControl("Qty:"));
            TextBox qtb = null;

            qtb = create_numeric_textbox("qty_" + key);
            qtb.Text = "1";

            c.Controls.Add(qtb);
            r.Cells.Add(c);

            ImageButton btn_remove_part = new ImageButton();
            btn_remove_part.Click += new ImageClickEventHandler(btn_remove_part_Click);
            btn_remove_part.ImageUrl = "~/delete.png";
            btn_remove_part.ToolTip = "Delete";
            btn_remove_part.ID = "btn_remove_part_" + pd.id.ToString();
            btn_remove_part.Attributes["key"] = key;


            c = new TableCell();
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Controls.Add(btn_remove_part);
            r.Cells.Add(c);

            r.Attributes["key"] = key;

            tblSelected.Rows.Add(r);

            paint_selected_table();
        }

        string create_table_entry_desc(part_data pd)
        {
            string desc = pd.description;

            

            return desc;
        }

       

        TextBox create_numeric_textbox(string id)
        {
            TextBox tb = new TextBox();
            tb.Width = 45;
            tb.MaxLength = 3;
            tb.Text = "0";
            tb.Attributes.Add("onkeypress", "return isNumberKey(event)");
            tb.ID = id;

            return tb;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (tblSelected.Rows.Count < 2)
            {
                lblMsg.Text = "Nothing selected for delivery.";
                return;
            }

            string project = txtProject.Text.Trim().ToUpper();

            if (project.Length != 6 || !project.StartsWith("G00"))
            {
                lblMsg.Text = "Invalid project number";
                txtProject.Focus();
                return;
            }

             string confirmValue = Request.Form["confirm_value"];

             if (confirmValue == "Yes")
             {
                 DateTime dt = dtDelivery.SelectedDate;

                 if (dt.Date < DateTime.Now.Date)
                 {
                     lblMsg.Text = "Please enter a valid delivery date.";
                     return;
                 }

                 int ref_id = 0;
                 string login_id = System.Web.HttpContext.Current.User.Identity.Name;
                 string sref = string.Empty;

                 Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
                 string gsbin = System.Web.Configuration.WebConfigurationManager.AppSettings["gsbin"].ToString();
                 string cust_deliv_note_email_recips = System.Web.Configuration.WebConfigurationManager.AppSettings["cust_deliv_note_email_recips"].ToString();

                 using (consignment_reference cr = new consignment_reference())
                 {
                     int reference_number = 1;
                     SortedList sl = new SortedList();
                     sl.Add("owner", login_id);
                     cr.order_by = "id DESC";
                     cr.select_top = 1;
                     ArrayList a = cr.get_consignment_reference_data(sl);

                     sl.Clear();

                     if (a.Count > 0)
                     {
                         consignment_reference_data crd = (consignment_reference_data)a[0];

                         reference_number = crd.reference_number + 1;
                     }

                     sl.Add("owner", login_id);
                     sl.Add("reference_number", reference_number);
                     sref = login_id + "-"+ DateTime.Now.ToString("yy")+ "-" + reference_number.ToString("000000");
                     sl.Add("reference", sref);
                     sl.Add("delivery_date", dt);
                     sl.Add("delivered", false);
                     sl.Add("project", project);

                     lblRef.Text = "Ref: " + sref;

                     ref_id = cr.save(sl, consignment_reference.get_tbl_name());
                 }

                
                 const string ENDL = "\r\n";
                 const string FF = "\f";
                 const string SPC = "\x20";

                 StringBuilder S = new StringBuilder();
                 string hdr = sref + ENDL;
                 hdr += dt.ToString("dd/MM/yyyy") + ENDL;
                 hdr += cust_deliv_note_email_recips + ENDL;
                 hdr += project + ENDL;
                 hdr += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;
                 hdr += ENDL;

                 int line_cnt = 1;

                 S.Append(hdr);

                 ArrayList a_cdld = new ArrayList();

                 int i = 0;

                 foreach (TableRow r in tblSelected.Rows)
                 {
                     string key = (r.Attributes["key"]);

                     if (key !=  null)
                     {
                         decimal qty = 0;

                         foreach (TableCell cell in r.Cells)
                         {
                             foreach (Control c in cell.Controls)
                             {
                                 if (c.ID != null)
                                 {
                                     if (c.ID.StartsWith("qty"))
                                     {
                                         TextBox tb = ((TextBox)c);

                                         try { qty = Convert.ToDecimal(tb.Text.Trim()); }
                                         catch { }
                                     }
                                 }
                             }
                         }

                         part_data cd = (part_data)m_selected[i];

                         consignment_delivery_line_data cdld = new consignment_delivery_line_data();
                         cdld.part_id = cd.id;
                         cdld.qty_dispatched = qty;
                         a_cdld.Add(cdld);

                         string desc = create_table_entry_desc(cd);
                         desc = desc.Insert(0, qty.ToString("0").PadLeft(8) +SPC);

                        if (line_cnt++ > 80)
                        {
                            if (S.Length > 0)
                                S.Append(FF);

                            S.Append(hdr);

                            line_cnt = 1;
                        }

                        S.Append(desc+ENDL);

                        i++;
                     }
                 }

                 string psFile = Server.MapPath("temp");
                 
                 psFile += "\\";
                 psFile += DateTime.Now.ToString("yyyyMMddHHmmssfff") + ref_id.ToString();
                 string pdfFile = psFile + ".pdf";
                 psFile += ".ps";

                 string delnote = Server.MapPath("forms") + "\\GBE_CONS";

                 if (File.Exists(delnote))
                     File.Copy(delnote, psFile);

                 FileStream fs = new FileStream(psFile, FileMode.Append, FileAccess.Write);
                 StreamWriter sw = new StreamWriter(fs, System.Text.ASCIIEncoding.Default);

                 sw.Write(S.ToString());

                 sw.Close();

                 PCFUtil util = new PCFUtil();
                 int ret = util.ps_convert(gsbin, psFile, pdfFile, "pdfwrite", true, "plop", 0);

                 if (File.Exists(pdfFile))
                 {
                     using (consignment_delivery_notes cdn = new consignment_delivery_notes())
                     {
                         byte[] pdf = File.ReadAllBytes(pdfFile);

                         SortedList sl = new SortedList();

                         sl.Add("pdf", pdf);
                         sl.Add("consignment_reference_id", ref_id);
                         

                         int dn_id = cdn.save(sl, consignment_delivery_notes.get_tbl_name());

                         hlDelNote.NavigateUrl = "cust_delnote.aspx?x=" + PCFsecure.PCFUtil.tohex(PCFsecure.PCFUtil.EncryptData("ref_id=" + ref_id.ToString(), PCFsecure.PCFUtil.PW));
                     }

                     using (consignment_delivery_line cdl = new consignment_delivery_line())
                     {
                         foreach (consignment_delivery_line_data cdld in a_cdld)
                         {
                             SortedList sl = new SortedList();
                             
                             sl.Add("qty_dispatched", cdld.qty_dispatched);
                             sl.Add("part_id", cdld.part_id);
                             sl.Add("consignment_reference_id", ref_id);
                             cdl.save_consignment_delivery_line_data(sl);
                         }
                     }

                     try
                     {
                         File.Copy(psFile, Server.MapPath("email")+"\\" + Path.GetFileName(psFile));
                         File.Delete(psFile);
                         File.Delete(pdfFile);
                     }
                     catch { }
                 }
             
                 MultiView1.ActiveViewIndex = 1;
             }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPartNo.Text.Trim().Length > 0)
            {
                SortedList sl = new SortedList();
                sl.Add("description", txtPartNo.Text.Trim());

                ArrayList a = new ArrayList();

                using (parts p = new parts())
                {
                    a = p.get_part_data(sl);
                }

                if (a.Count > 0)
                {
                    txtPartNo.Text = string.Empty;

                    part_data pd = (part_data)a[0];

                    if (m_selected == null)
                        m_selected = new ArrayList();

                    bool b_already_there = false;
                    foreach (part_data pd0 in m_selected)
                    {
                        if (pd0.description == pd.description)
                        {
                            b_already_there = true;
                            break;
                        }
                    }

                    if (!b_already_there)
                    {
                        pd.attributes.Add("key", pd.id.ToString());
                        m_selected.Add(pd);

                        ViewState["selected"] = m_selected;
                        create_selected_table_entry(pd);
                    }
                }
            }

            txtPartNo.Focus();
        }
    }
}
