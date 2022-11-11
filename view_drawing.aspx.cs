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
    public partial class view_drawing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sid = Request.QueryString["id"];

            if (sid != null)
            {
                int id = 0;

                try { id = Convert.ToInt32(sid); }
                catch { }

                using (spools s = new spools())
                {
                    drawing_data dd = s.get_drawing_data(id);

                    if (dd.pdf != null && dd.pdf.Length > 0)
                    {
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Type", "application/pdf");

                        Response.AddHeader("Content-Disposition", "inline; filename=GBE_Drawing.pdf");

                        Response.Buffer = true;

                        if (dd.pdf != null && dd.pdf.Length > 0)
                            Response.BinaryWrite(dd.pdf);

                        Response.Flush();

                        try { Response.End(); }
                        catch { }
                    }
                }
            }
        }
    }
}
