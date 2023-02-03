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
using System.Diagnostics;

namespace gbe
{
    public partial class create_porder : System.Web.UI.Page
    {
        SortedList m_sl_po = new SortedList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (users u = new users())
                {
                    u.order_by = "login_id";

                    ArrayList a = u.get_user_data(new SortedList());

                    foreach (user_data ud in a)
                    {
                        if (ud.role == "ADMIN")
                            dlUser.Items.Add(ud.login_id.ToUpper());
                    }
                }

                dlUser.Text = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();

                dlUser.Enabled = (System.Web.HttpContext.Current.User.Identity.Name.ToUpper() == "PM");
            }

            txtProject.Focus();
        }

        protected void btnCreatePO_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            
            if (txtProject.Text.Trim().Length >= 6)
            {
                user_data ud = null;

                using (users u = new users())
                {
                    ud = u.get_user_data(dlUser.Text);
                }

                bool b_po_produced = false;

                if (ud != null)
                {
                    SortedList sl = new SortedList();

                    sl.Add("spool", txtProject.Text.Trim() + "%");
                    sl.Add("porder_created", false);
                    sl.Add("cad_user_id", ud.id);

                    using (spools spls = new spools())
                    {
                        ArrayList a = spls.get_spool_data_ex(sl);

                        if (a.Count > 0)
                        {
                            SortedList sl_supplier_split = new SortedList();

                            foreach (spool_data sd in a)
                            {
                                foreach (spool_part_data spd in sd.spool_part_data)
                                {
                                    if (spd.porder == 0)
                                    {
                                        string k = spd.part_data.supplier.ToLower().Trim() + "_" + sd.delivery_address.ToString().Trim();

                                        if (!sl_supplier_split.ContainsKey(k))
                                        {
                                            sl_supplier_split.Add(k, new ArrayList());
                                        }

                                        ((ArrayList)sl_supplier_split[k]).Add(spd);
                                    }
                                }
                            }

                            foreach (DictionaryEntry e0 in sl_supplier_split)
                            {
                                ArrayList aspd = (ArrayList)e0.Value;
                                string[] sa = e0.Key.ToString().Split('_');

                                string supplier = string.Empty;
                                int deliv_addr_id = 0;

                                if (sa.Length > 0)
                                {
                                    supplier = sa[0];

                                    try { deliv_addr_id = Convert.ToInt32(sa[sa.Length - 1]); }
                                    catch { }
                                }

                                create_po2(aspd, txtProject.Text.Trim(), ud, deliv_addr_id, supplier);

                                display_po();

                                b_po_produced = true;
                            }
                        }

                        if (!b_po_produced)
                        {
                            lblMsg.Text = "There are no orders outstanding for this project.";
                        }
                    }
                }
                else
                    lblMsg.Text = "Error retrieving user data";
            }
        }

        public byte[] create_po_pdf(string order_number, string contract_number, int delivery_address_id, string delivery_date, ArrayList aorder_lines, ref decimal total_value)
        {
            const string ENDL = "\r\n";
            const string FF = "\f";
            const string SPC = "\x20";

            ArrayList adelivery_address = new ArrayList();

            SortedList sl = new SortedList();

            sl.Clear();
            sl.Add("contract_number", contract_number);

            ArrayList acd = new ArrayList();

            using (customers cust = new customers())
            {
                acd = cust.get_customer_data(sl);
            }

            string customer_name = string.Empty;
            string customer_contact = string.Empty;
            string customer_telephone = string.Empty;

            string delivery_contact = string.Empty;
            string delivery_telephone = string.Empty;

            customer_data cd = null;

            if (acd.Count > 0)
            {
                cd = (customer_data)acd[0];
                customer_name = cd.name;
                customer_contact = cd.contact_name;
                customer_telephone = cd.telephone;
            }

            if (delivery_address_id == 0)
            {
                adelivery_address.Add("GBE Fabrications Ltd");
                adelivery_address.Add("1 Whitewall Road");
                adelivery_address.Add("Medway City Estate");
                adelivery_address.Add("Rochester");
                adelivery_address.Add("ME2 4EW");
            }
            else if (delivery_address_id == -1)
            {
                if (cd != null)
                {
                    adelivery_address.Add(cd.address_line1);
                    adelivery_address.Add(cd.address_line2);
                    adelivery_address.Add(cd.address_line3);
                    adelivery_address.Add(cd.address_line4);
                }
            }
            else if (delivery_address_id == -2)
            {
                adelivery_address.Add("RETURN ORDER");
            }
            else if (delivery_address_id > 0)
            {
                sl.Clear();
                sl.Add("id", delivery_address_id);

                ArrayList adad = new ArrayList();

                using (delivery_addresses da = new delivery_addresses())
                {
                    adad = da.get_delivery_address_data(sl);
                }

                if (adad.Count > 0)
                {
                    delivery_address dadt = (delivery_address)adad[0];

                    adelivery_address.Add(dadt.address_line1);
                    adelivery_address.Add(dadt.address_line2);
                    adelivery_address.Add(dadt.address_line3);
                    adelivery_address.Add(dadt.address_line4);

                    delivery_contact = dadt.contact_name;
                    delivery_telephone = dadt.telephone;
                }
            }

            while (adelivery_address.Count < 8)
                adelivery_address.Add(string.Empty);

            user_data ud = null;
            string login_id, name, job_title, supplier;
            login_id = name = job_title = supplier = string.Empty;

            string username = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();

            try { username = dlUser.Text; }
            catch { }

            using (users u = new users())
            {
                ud = u.get_user_data(username);
            }

            if (ud != null)
            {
                login_id = ud.login_id;
                name = ud.name;
                job_title = ud.job_title;
            }

            SortedList sl_pol0 = (SortedList) aorder_lines[0];

            int part_id = 0;

            if (sl_pol0.ContainsKey("part_id"))
                part_id = Convert.ToInt32(sl_pol0["part_id"].ToString());

            using (parts p = new parts())
            {
                sl.Clear();
                sl.Add("id", part_id);

                ArrayList ap = p.get_part_data(sl);

                if (ap.Count > 0)
                {
                    part_data pd = (part_data) ap[0];
                    supplier = pd.supplier;
                }
            }

            StringBuilder S = new StringBuilder();
            string hdr = order_number.PadRight(50);
            hdr += login_id.PadRight(50);
            hdr += name.PadRight(50);
            hdr += job_title;
            hdr += ENDL;
            hdr += DateTime.Now.ToString("dd/MM/yyyy").PadRight(20) + contract_number.ToUpper();
            hdr += ENDL;
            hdr += supplier;
            hdr += ENDL;

            foreach (string addr_line in adelivery_address)
                hdr += addr_line + ENDL;

            hdr += delivery_date + ENDL;

            hdr += customer_name + ENDL;

            if (customer_contact.Length > 0)
            {

            }

            hdr += ENDL;

            if (delivery_contact.Length > 0)
            {

            }

            hdr += ENDL;

            decimal total = 0;

            int line_cnt = 0;

            S.Append(hdr);

            ArrayList a_notes = new ArrayList();

            using (parts p = new parts())
            {
                foreach (SortedList sl_pol in aorder_lines)
                {
                    decimal qty = 0;

                    if (sl_pol.ContainsKey("qty"))
                    {
                        try { qty = (decimal)sl_pol["qty"]; }
                        catch { };
                    }

                    if (sl_pol.ContainsKey("part_id"))
                    {
                        part_id = Convert.ToInt32(sl_pol["part_id"].ToString());
                        sl.Clear();
                        sl.Add("id", part_id);

                        ArrayList ap = p.get_part_data(sl);

                        if (ap.Count > 0)
                        {
                            part_data pd = (part_data)ap[0];

                            if (line_cnt == 38)
                            {
                                if (S.Length > 0)
                                    S.Append(FF);

                                S.Append(hdr);

                                line_cnt = 0;
                            }

                            string qty_len = qty.ToString("0.00");

                            if (pd.description.ToUpper().Contains("PIPE")|| pd.part_type.ToUpper().Contains("PIPE"))
                                qty_len += "m";

                            S.Append(qty_len.PadLeft(10));
                            S.Append(SPC);

                            string part_no, part_desc;
                            part_no = part_desc = string.Empty;

                            decimal price = 0;
                        
                            part_no = pd.part_number;

                            if (part_no.Trim().Length == 0)
                                part_no = "TBC";

                            part_desc = pd.description;
                            price = pd.material_cost; //pd.gbe_sale_cost;

                            S.Append(part_no.PadRight(50));
                            S.Append(string.Empty.PadRight(55));
                            

                            S.Append(price.ToString("0.00").PadLeft(25));
                            
                            S.Append(SPC);

                            decimal line_total = qty * price;

                            S.Append(line_total.ToString("0.00").PadLeft(25));
                            S.Append(ENDL);
                            total += line_total;

                            S.Append(string.Empty.PadRight(11));
                            S.Append("(" + part_desc + ")");
                            S.Append(ENDL);

                            line_cnt += 2;

                            if (sl_pol.ContainsKey("note"))
                            {
                                string note_line = string.Empty;
                                note_line += part_desc.PadRight(50);
                                note_line += SPC;
                                note_line += sl_pol["note"].ToString();

                                a_notes.Add(note_line);
                            }
                        }
                    }
                }
            }

            for (int i = line_cnt; line_cnt < 45; line_cnt++)
                S.Append(ENDL);

            S.Append("Total = " + total.ToString("0.00").PadLeft(25));

            if (a_notes.Count > 0)
            {
                S.Append(FF);
                hdr += "notes";
                hdr += ENDL;

                S.Append(hdr);

                line_cnt = 0;

                foreach (string note_line in a_notes)
                {
                    if (line_cnt++ > 30)
                    {
                        if (S.Length > 0)
                            S.Append(FF);

                        S.Append(hdr);

                        line_cnt = 1;
                    }

                    S.Append(note_line);
                    S.Append(ENDL);
                }
            }

            string psFile = Server.MapPath("temp");
            DateTime dt = DateTime.Now;
            psFile += "\\";
            psFile += dt.ToString("yyyyMMddHHmmssfff") + contract_number + delivery_address_id.ToString();
            string pdfFile = psFile + ".pdf";
            psFile += ".ps";

            string FORMS_DIR = "forms_dir";
            string forms_dir = string.Empty;

            try { forms_dir = System.Web.Configuration.WebConfigurationManager.AppSettings[FORMS_DIR].ToString(); }
            catch { forms_dir = Server.MapPath("forms"); }

            string po_form = forms_dir + "\\gbe_po";

            File.Copy(po_form, psFile);

            FileStream fs = new FileStream(psFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.ASCIIEncoding.Default);

            sw.Write(S.ToString());

            sw.Close();
                        
            string gsbin = System.Web.Configuration.WebConfigurationManager.AppSettings["gsbin"].ToString();

            PCFUtil util = new PCFUtil();
            int ret = util.ps_convert(gsbin, psFile, pdfFile, "pdfwrite", true, "plop", 0);

            byte[] pdf = null;

            try { pdf = File.ReadAllBytes(pdfFile); }
            catch { }

            try { File.Delete(psFile); }
            catch { }

            try { File.Delete(pdfFile); }
            catch { }

            total_value = total;

            return pdf;
        }

        void create_po2(ArrayList aspd, string contract_number, user_data ud, int delivery_address_id, string supplier)
        {
            if (aspd.Count == 0)
                return;

            ArrayList aorderlines = new ArrayList();

            int order_number = 0;

            using (po_numbers po_n = new po_numbers())
            {
                order_number = po_n.get_next_po_number(contract_number, ud.login_id);
            }

            string sorder_no = contract_number.Trim() + "/" + ud.login_id.Trim() + "/" + order_number.ToString("000");

            SortedList sl_spd = new SortedList();

            foreach (spool_part_data spd in aspd)
            {
                if (sl_spd.Contains(spd.part_data.description))
                {
                    spool_part_data spd2 = (spool_part_data)sl_spd[spd.part_data.description];
                    spd2.qty += spd.qty;
                }
                else
                {
                    if (spd.part_data.source == 0)
                        sl_spd.Add(spd.part_data.description, spd);
                }
            }

            foreach (DictionaryEntry e in sl_spd)
            {
                spool_part_data spd = (spool_part_data)e.Value;

                SortedList sl_pol = new SortedList();
                sl_pol.Add("part_id", spd.part_id);
                sl_pol.Add("qty", spd.qty);
                aorderlines.Add(sl_pol);
            }
            
            porder_data pod = new porder_data();

            if (aorderlines.Count > 0)
            {
                decimal total_value = 0;

                pod.pdf = create_po_pdf(sorder_no, contract_number, delivery_address_id, txtDeliveryDate.Text.Trim(), aorderlines, ref total_value);

                SortedList sl = new SortedList();

                sl.Clear();

                sl.Add("pdf", pod.pdf);
                sl.Add("order_no", sorder_no);
                sl.Add("active", true);
                sl.Add("part_type", "M");
                sl.Add("delivery_address_id", delivery_address_id);
                sl.Add("delivery_date", txtDeliveryDate.Text.Trim());
                sl.Add("supplier", supplier);
                sl.Add("total_value", total_value);

                int po_id = 0;

                using (porders p = new porders())
                {
                    po_id = p.save_porder_data(sl);
                }

                m_sl_po.Add(po_id, sorder_no);

                using (spool_parts sp = new spool_parts())
                {
                    foreach (spool_part_data spd in aspd)
                    {
                        sl.Clear();

                        sl.Add("id", spd.id);
                        sl.Add("porder", po_id);

                        sp.save_spool_parts_data(sl);

                        using (spools spls = new spools())
                        {
                            sl.Clear();
                            sl.Add("id", spd.spool_id);
                            sl.Add("porder_created", true);

                            spls.save_spool_details(sl, "PO CREATED", ud.login_id);
                        }
                    }
                }

                using (po_orderlines pol = new po_orderlines())
                {
                    pol.save_po_orderlines_data(aorderlines, po_id);
                }
            }
        }

        void create_po(ArrayList aspd, string contract_number, user_data ud, int delivery_address_id)
        {
            const string ENDL = "\r\n";
            const string FF = "\f";
            const string SPC = "\x20";

            if (aspd.Count == 0)
                return;

            ArrayList aorderlines = new ArrayList();

            spool_part_data spd0 = (spool_part_data)aspd[0];

            ArrayList adelivery_address = new ArrayList();

            SortedList sl = new SortedList();
            
            sl.Clear();
            sl.Add("contract_number", contract_number);

            ArrayList acd = new ArrayList();

            using (customers cust = new customers())
            {
                acd = cust.get_customer_data(sl);
            }

            string customer_name = string.Empty;
            string customer_contact = string.Empty;
            string customer_telephone = string.Empty;

            string delivery_contact = string.Empty;
            string delivery_telephone = string.Empty;

            if (acd.Count > 0)
            {
                customer_data cd = (customer_data)acd[0];
                customer_name = cd.name;
                customer_contact = cd.contact_name;
                customer_telephone = cd.telephone;
            }

            if (delivery_address_id == 0)
            {
                adelivery_address.Add("GBE Fabrications Ltd");
                adelivery_address.Add("1 Whitewall Road");
                adelivery_address.Add("Medway City Estate");
                adelivery_address.Add("Rochester");
                adelivery_address.Add("ME2 4EW");
            }
            else if (delivery_address_id > 0)
            {
                sl.Clear();
                sl.Add("id", delivery_address_id);

                ArrayList adad = new ArrayList();

                using (delivery_addresses da = new delivery_addresses())
                {
                    adad = da.get_delivery_address_data(sl);
                }

                if (adad.Count > 0)
                {
                    delivery_address dadt = (delivery_address)adad[0];

                    adelivery_address.Add(dadt.address_line1);
                    adelivery_address.Add(dadt.address_line2);
                    adelivery_address.Add(dadt.address_line3);
                    adelivery_address.Add(dadt.address_line4);

                    delivery_contact = dadt.contact_name;
                    delivery_telephone = dadt.telephone;
                }
            }

            while (adelivery_address.Count < 8)
                adelivery_address.Add(string.Empty);

            int order_number = 0;

            using (po_numbers po_n = new po_numbers())
            {
                order_number = po_n.get_next_po_number(contract_number, ud.login_id);
            }

            StringBuilder S = new StringBuilder();
            string hdr = contract_number.PadRight(10);
            hdr += order_number.ToString("000").PadRight(5);
            hdr += ud.login_id.PadRight(50);
            hdr += ud.name.PadRight(50);
            hdr += ud.job_title;
            hdr += ENDL;
            hdr += DateTime.Now.ToString("dd/MM/yyyy");
            hdr += ENDL;
            hdr += spd0.part_data.supplier;
            hdr += ENDL;

            foreach (string addr_line in adelivery_address)
                hdr += addr_line + ENDL;

            hdr += txtDeliveryDate.Text.Trim() + ENDL;
            
            hdr += customer_name + ENDL;

            if (customer_contact.Length > 0)
            {

            }

            hdr += ENDL;

            if (delivery_contact.Length > 0)
            {

            }

            hdr += ENDL;


            decimal total = 0;

            int line_cnt = 0;
            
            S.Append(hdr);

            SortedList sl_spd = new SortedList();

            foreach (spool_part_data spd in aspd)
            {
                if (sl_spd.Contains(spd.part_data.description))
                {
                    spool_part_data spd2 = (spool_part_data)sl_spd[spd.part_data.description];
                    spd2.qty += spd.qty;
                }
                else
                {
                    if(spd.part_data.source == 0)
                        sl_spd.Add(spd.part_data.description, spd);
                }
            }

            foreach (DictionaryEntry e in sl_spd)
            {
                spool_part_data spd = (spool_part_data)e.Value;
                if (line_cnt++ > 30)
                {
                    if (S.Length > 0)
                        S.Append(FF);

                    S.Append(hdr);

                    line_cnt = 1;
                }

                string qty_len = spd.qty.ToString("0.00");

                if (spd.part_data.description.ToUpper().Contains("PIPE")|| spd.part_data.part_type.ToUpper().Contains("PIPE"))
                    qty_len += "m";

                S.Append(qty_len.PadLeft(10));
                S.Append(SPC);

                string part_no, part_desc;
                part_no = part_desc = string.Empty;

                decimal price = 0;

                if (spd.part_data != null)
                {
                    part_no = spd.part_data.part_number;
                    part_desc = spd.part_data.description;
                    price = spd.part_data.material_cost; //spd.part_data.gbe_sale_cost;
                }

                SortedList sl_pol = new SortedList();
                sl_pol.Add("part_id", spd.part_id);
                sl_pol.Add("qty", spd.qty);
                aorderlines.Add(sl_pol);

                S.Append(part_no.PadRight(50));
                S.Append(SPC);
                S.Append(part_desc.PadRight(50));

                S.Append(price.ToString("0.00").PadLeft(25));

                S.Append(SPC);

                decimal line_total = spd.qty * price;

                S.Append(line_total.ToString("0.00").PadLeft(25));
                S.Append(ENDL);
                total += line_total;
            }

            for (int i = line_cnt; line_cnt < 45; line_cnt++)
                S.Append(ENDL);

            S.Append("Total = " + total.ToString("0.00").PadLeft(25));

            string psFile = Server.MapPath("temp");
            DateTime dt = DateTime.Now;
            psFile += "\\";
            psFile += dt.ToString("yyyyMMddHHmmssfff") + contract_number + delivery_address_id.ToString();
            string pdfFile = psFile + ".pdf";
            psFile += ".ps";

            string FORMS_DIR = "forms_dir";
            string forms_dir = string.Empty;

            try { forms_dir = System.Web.Configuration.WebConfigurationManager.AppSettings[FORMS_DIR].ToString(); }
            catch { forms_dir = Server.MapPath("forms"); }

            string po_form = forms_dir + "\\gbe_po";

            File.Copy(po_form, psFile);

            FileStream fs = new FileStream(psFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.ASCIIEncoding.Default);

            sw.Write(S.ToString());

            sw.Close();

            string gsbin = System.Web.Configuration.WebConfigurationManager.AppSettings["gsbin"].ToString();

            PCFUtil util = new PCFUtil();
            int ret = util.ps_convert(gsbin, psFile, pdfFile, "pdfwrite", true, "plop", 0);

            if (File.Exists(pdfFile))
            {
                porder_data pod = new porder_data();

                /*
                string pdf2doc = string.Empty;

                try
                {
                    pdf2doc = System.Web.Configuration.WebConfigurationManager.AppSettings["pdf2doc"].ToString();
                }
                catch { }

                
                sl = new SortedList();

                if (pdf2doc.Trim().Length > 0)
                {
                    string docFile = Server.MapPath("temp");
                    docFile += "\\";
                    docFile += dt.ToString("yyyyMMddHHmmssfff") + contract_number + delivery_address_id.ToString();
                    docFile += ".doc";

                    Process pdf2doc_proc = new Process();

                    pdf2doc_proc.StartInfo.FileName = pdf2doc;

                    string args = "--src=\"" + pdfFile + "\" ";
                    args += " --dest=\"" + docFile + "\" ";
                    args += " --mode=1";
                    pdf2doc_proc.StartInfo.Arguments = args;
                    pdf2doc_proc.StartInfo.UseShellExecute = false;
                    pdf2doc_proc.StartInfo.RedirectStandardInput = true;
                    pdf2doc_proc.StartInfo.RedirectStandardOutput = true;
                    pdf2doc_proc.StartInfo.CreateNoWindow = true;

                    pdf2doc_proc.Start();

                    pdf2doc_proc.WaitForExit();

                    if (File.Exists(docFile))
                    {
                        pod.doc = File.ReadAllBytes(docFile);
                        sl.Add("doc", pod.doc);
                        File.Delete(docFile);
                    }
                }
                */

                pod.pdf = File.ReadAllBytes(pdfFile);

                sl.Clear();

                sl.Add("pdf", pod.pdf);
                string sorder_no = contract_number.Trim() + "/" + ud.login_id.Trim() + "/" + order_number.ToString("000");
                sl.Add("order_no", sorder_no);
                sl.Add("active", true);
                sl.Add("part_type", "M");
                int po_id = 0;

                using (porders p = new porders())
                {
                    po_id = p.save_porder_data(sl);
                }

                m_sl_po.Add(po_id, sorder_no);

                using (spool_parts sp = new spool_parts())
                {
                    foreach (spool_part_data spd in aspd)
                    {
                        sl.Clear();

                        sl.Add("id", spd.id);
                        sl.Add("porder", po_id);

                        sp.save_spool_parts_data(sl);

                        using (spools spls = new spools())
                        {
                            sl.Clear();
                            sl.Add("id", spd.spool_id);
                            sl.Add("porder_created", true);

                            spls.save_spool_details(sl, "PO CREATED", ud.login_id);
                        }
                    }
                }

                File.Delete(psFile);
                File.Delete(pdfFile);

                using (po_orderlines pol = new po_orderlines())
                {
                    pol.save_po_orderlines_data(aorderlines, po_id);
                }
            }
        }

        void display_po()
        {
            tblPO.Rows.Clear();

            TableRow r;
            TableCell c;

            string[] hdr = new string[] { "Purchase Order", "Supplier"};

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblPO.Rows.Add(r);

            using (porders po = new porders())
            {
                SortedList sl = new SortedList();

                foreach (DictionaryEntry e0 in m_sl_po)
                {
                    sl.Clear();
                    sl.Add("id", e0.Key.ToString());
                    ArrayList a_po = po.get_porder_data(sl);

                    if (a_po.Count > 0)
                    {
                        porder_data pod = (porder_data)a_po[0];

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        HyperLink h = new HyperLink();
                        h.Text = e0.Value.ToString();
                        h.NavigateUrl = "porder.aspx/PO_" + e0.Value.ToString().Replace('/', '_') + "?id=" + e0.Key.ToString();
                        h.Target = "_blank";

                        c.Controls.Add(h);
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(pod.supplier.ToUpper()));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Center;

                        h = new HyperLink();
                        h.Text = "Edit PO";
                        h.NavigateUrl = "porders.aspx?id=" + e0.Key.ToString();
                        //h.Target = "_blank";

                        c.Controls.Add(h);

                        r.Cells.Add(c);

                        tblPO.Rows.Add(r);
                    }
                }
            }
        }
    }
}
