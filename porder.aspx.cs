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
    public partial class porder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sid = Request.QueryString["id"];

            if(sid != null)
            {
                int id  = 0;

                try { id = Convert.ToInt32(sid); }
                catch { }

                porder_data pod = null;

                if(id > 0)
                {
                    
                    SortedList sl = new SortedList();

                    sl.Add("id", id);

                    ArrayList a = new ArrayList();

                    using (porders p = new porders())
                    {
                        a = p.get_porder_data(sl);
                    }

                    if (a.Count > 0)
                    {
                        pod = (porder_data)a[0];
                    }
                }

                if (pod != null)
                {
                    string fn = pod.order_no.Replace('/', '_');

                    if (fn.Length == 0)
                        fn  = "gbe_po";

                    Response.Clear();

                    /*
                    if (pod.doc!= null && pod.doc.Length > 0)
                    {
                        Response.ContentType = "application/msword";
                        Response.AddHeader("Content-Type", "application/msword");
                        fn += ".doc";
                    }
                    else*/

                    if (pod.pdf == null || pod.pdf.Length == 0)
                    {
                        SortedList sl = new SortedList();

                        create_porder  crpo = new create_porder();

                        string contract_number = string.Empty;

                        try 
                        {
                            if (pod.order_no.StartsWith("GBE/"))
                                contract_number = pod.order_no.Split('/')[3]; 
                            else
                                contract_number = pod.order_no.Split('/')[0]; 
                        }
                        catch { }

                        ArrayList aorder_lines = new ArrayList();

                        sl.Add("porder_id", pod.id);
                        using (po_orderlines pol = new po_orderlines())
                        {
                            ArrayList apol = pol.get_po_orderlines_data(sl, pod.part_type, string.Empty);

                            foreach (po_orderlines_data pold in apol)
                            {
                                SortedList sl_pol = new SortedList();
                                sl_pol.Add("part_id", pold.part_id);
                                sl_pol.Add("qty", pold.qty);
                                aorder_lines.Add(sl_pol);
                            }
                        }

                        if (aorder_lines.Count > 0)
                        {
                            decimal total_value = 0;

                            pod.pdf = crpo.create_po_pdf(pod.order_no, contract_number, pod.delivery_address_id, pod.delivery_date, aorder_lines, ref total_value);

                            using (porders p = new porders())
                            {
                                sl.Clear();
                                sl.Add("id", pod.id);
                                sl.Add("pdf", pod.pdf);
                                sl.Add("total_value", total_value);
                                p.save_porder_data(sl);
                            }
                        }
                    }

                    if (pod.pdf != null && pod.pdf.Length > 0)
                    {
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Type", "application/pdf");
                    }

                    Response.AddHeader("Content-Disposition", "inline; filename=" + fn);

                    Response.Buffer = true;

                    /*if (pod.doc != null && pod.doc.Length > 0)
                        Response.BinaryWrite(pod.doc);
                    else*/
                    if (pod.pdf != null && pod.pdf.Length > 0)
                        Response.BinaryWrite(pod.pdf);

                    Response.Flush();

                    try { Response.End(); }
                    catch { }
                }
            }
        }
    }
}
