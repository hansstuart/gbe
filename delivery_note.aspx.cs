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
    public partial class delivery_note : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sid = Request.QueryString["id"];

            if (sid != null)
            {
                int id = 0;

                try { id = Convert.ToInt32(sid); }
                catch { }

                if (id > 0)
                {
                    SortedList sl = new SortedList();

                    sl.Clear();

                    sl.Add("id", id);

                    ArrayList add = new ArrayList();

                    using (deliveries deliv = new deliveries())
                    {
                        add = deliv.get_delivery_data(sl);
                    }

                    if (add.Count > 0)
                    {
                        string contract_number = string.Empty;
                        delivery_data dd = (delivery_data)add[0];

                        sl.Clear();

                        sl.Add("id", dd.user_id);
                        string loader_username = string.Empty;

                        ArrayList a = new ArrayList();

                        using (users u = new users())
                        {
                            a = u.get_user_data(sl);
                        }

                        if (a.Count > 0)
                        {
                            user_data ud = (user_data)a[0];
                            loader_username = ud.name;
                        }

                        a.Clear();
                        sl.Clear();

                        sl.Add("login_id", dd.driver);
                        string driver_username = string.Empty;

                        using (users u = new users())
                        {
                            a = u.get_user_data(sl);
                        }

                        if (a.Count > 0)
                        {
                            user_data ud = (user_data)a[0];
                            driver_username = ud.name;
                        }

                        if (dd.delivery_note == null || dd.delivery_note.Length == 0)
                        {
                            sl.Clear();

                            sl.Add("delivery_id", dd.id);
                            
                            ArrayList asdd = new ArrayList();

                            using (deliveries dsdl = new deliveries())
                            {
                                asdd = dsdl.get_spool_delivery_data(sl);
                            }

                            ArrayList aspl_data = new ArrayList();

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
                                                aspl_data.Add(sd.barcode);
                                            }
                                        }
                                        else if (sdd.assembly_type == spool_delivery_data.MODULE)
                                        {
                                            ArrayList amd = m.get_module_data(sl);

                                            foreach (module_data md in amd)
                                            {
                                                aspl_data.Add(md.barcode);
                                            }
                                        }
                                    }
                                }
                            }

                            if (aspl_data.Count > 0)
                            {
                                aspl_data.Sort();

                                contract_number = aspl_data[0].ToString().Split('-')[0];
                                
                                sl.Clear();
                                sl.Add("contract_number", contract_number);

                                ArrayList acd = new ArrayList();

                                using (customers cust = new customers())
                                {
                                    acd = cust.get_customer_data(sl);
                                }

                                if(acd.Count > 0)
                                {
                                    const string ENDL = "\r\n";
                                    const string FF = "\f";
                                    
                                    customer_data cd = (customer_data)acd[0];

                                    StringBuilder S = new StringBuilder();

                                    string bc = "gbe-" + dd.id.ToString();

                                    bc += "-" + deliveries.get_checkDigit(dd.id.ToString()).ToString();
                                    
                                    string hdr = bc.PadRight(50) +  cd.contract_number + ENDL;
                                    hdr += cd.name + ENDL;

                                    ArrayList a_addr = get_address(dd.address_table, dd.address_id, cd);

                                    foreach (string addr_line in a_addr)
                                    {
                                        hdr += addr_line + ENDL;
                                    }
                                    
                                    hdr += cd.contact_name + ENDL;
                                    hdr += cd.telephone + ENDL;

                                    hdr += dd.datetime_stamp.ToString("dd/MM/yyyy HH:mm:ss") + ENDL;
                                    hdr += driver_username + ENDL;
                                    hdr += dd.vehicle + ENDL;
                                    hdr += loader_username + ENDL;
                                    hdr += ENDL;

                                    int line_cnt = 1;

                                    S.Append(hdr);

                                    foreach (string bc0 in aspl_data)
                                    {
                                        string bc1 = bc0;

                                        int p = bc0.LastIndexOf('_');

                                        if (p > 0)
                                        {
                                            bc1 = bc0.Substring(0, p);

                                            if (p + 1 < bc0.Length)
                                            {
                                                bc1 += "/";
                                                bc1 += bc0.Substring(p + 1);
                                            }
                                        }

                                        if (line_cnt++ > 80)
                                        {
                                            if (S.Length > 0)
                                                S.Append(FF);

                                            S.Append(hdr);

                                            line_cnt = 1;
                                        }

                                        S.Append(bc1 + " "  + ENDL);
                                    }

                                    string psFile = Server.MapPath("temp");
                                    DateTime dt = DateTime.Now;
                                    psFile += "\\";
                                    psFile += dt.ToString("yyyyMMddHHmmssfff") + dd.id.ToString();
                                    string pdfFile = psFile + ".pdf";
                                    psFile += ".ps";

                                    string delnote = Server.MapPath("forms") + "\\GBE_DELNOTE";

                                    if (File.Exists(delnote))
                                        File.Copy(delnote, psFile);

                                    FileStream fs = new FileStream(psFile, FileMode.Append, FileAccess.Write);
                                    StreamWriter sw = new StreamWriter(fs, System.Text.ASCIIEncoding.Default);

                                    sw.Write(S.ToString());

                                    sw.Close();

                                    Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
                                    string gsbin = System.Web.Configuration.WebConfigurationManager.AppSettings["gsbin"].ToString();

                                    PCFUtil util = new PCFUtil();
                                    int ret = util.ps_convert(gsbin, psFile, pdfFile, "pdfwrite", true, "plop", 0);

                                    if (File.Exists(pdfFile))
                                    {
                                        dd.delivery_note  = File.ReadAllBytes(pdfFile);

                                        sl.Clear();

                                        sl.Add("id", dd.id);
                                        sl.Add("delivery_note", dd.delivery_note);

                                        using (deliveries deliv = new deliveries())
                                        {
                                            deliv.save_delivery_data(sl, new ArrayList());
                                        }

                                        File.Delete(psFile);
                                        File.Delete(pdfFile);
                                    }
                                }
                            }
                        }

                        if (dd.delivery_note != null)
                        {
                            Response.Clear();

                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Type", "application/pdf");
                            Response.AddHeader("Content-Disposition", "inline; filename=" + "delnote_" + contract_number+ "_" + dd.datetime_stamp.ToString("yyyyMMdd_HHmmss"));

                            Response.Buffer = true;

                            Response.BinaryWrite(dd.delivery_note);

                            Response.Flush();

                            try { Response.End(); }
                            catch { }
                        }
                    }
                }
            }
        }

        ArrayList get_address(string address_table, int address_id, customer_data cd0)
        {
            ArrayList a = new ArrayList();
            
            for (int i = 0; i < 4; i++)
                a.Add(string.Empty);

            SortedList sl = new SortedList();

            sl.Add("id", address_id);

            ArrayList acd = new ArrayList();

            if (address_table == "customer_data")
            {
                using (customers cust = new customers())
                {
                    acd = cust.get_customer_data(sl);
                }

                if (acd.Count > 0)
                {
                    customer_data cd = (customer_data)acd[0];
                
                    a[0] = (cd.address_line1);
                    a[1] = (cd.address_line2);
                    a[2] = (cd.address_line3);
                    a[3] = (cd.address_line4);
                }
            }
            else if (address_table == "delivery_address")
            {
                acd.Clear();

                using (delivery_addresses da = new delivery_addresses())
                {
                    acd = da.get_delivery_address_data(sl);
                }

                if (acd.Count > 0)
                {
                    delivery_address cd = (delivery_address)acd[0];
                    a[0] = (cd.address_line1);
                    a[1] = (cd.address_line2);
                    a[2] = (cd.address_line3);
                    a[3] = (cd.address_line4);
                }
            }
            else
            {
                a[0] = (cd0.address_line1);
                a[1] = (cd0.address_line2);
                a[2] = (cd0.address_line3);
                a[3] = (cd0.address_line4);
            }

            return a;
        }
    }
}
