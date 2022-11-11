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
    public partial class cust_delnote : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string sref_id = PCFsecure.PCFUtil.page_params(Request.QueryString["x"])["ref_id"].ToString();

                int ref_id = Convert.ToInt32(sref_id);

                using (consignment_delivery_notes cdn = new consignment_delivery_notes())
                {
                    SortedList sl = new SortedList();
                    sl.Add("consignment_reference_id", ref_id);

                    ArrayList a = cdn.get_consignment_delivery_notes_data(sl);

                    if (a.Count > 0)
                    {
                        consignment_delivery_notes_data cdnd = (consignment_delivery_notes_data)a[0];

                        if (cdnd.pdf != null)
                        {
                            Response.Clear();

                            Response.ContentType = "application/pdf";
                            Response.AddHeader("Content-Type", "application/pdf");
                            Response.AddHeader("Content-Disposition", "inline; filename=gbe_delivery_note");

                            Response.Buffer = true;

                            Response.BinaryWrite(cdnd.pdf);

                            Response.Flush();

                            try { Response.End(); }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
