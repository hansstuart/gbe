using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gbe
{
    public partial class view_weld_test_report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sid = Request.QueryString["id"];

            if (sid != null)
            {
                int id = 0;

                try { id = Convert.ToInt32(sid); }
                catch { }

                ArrayList a_iu = null;

                using (irisndt_upload iu = new irisndt_upload())
                {
                    SortedList sl = new SortedList();
                    sl.Add(irisndt_upload.ID,id);
                    a_iu = iu.get_upload_data($"{irisndt_upload.ID}, {irisndt_upload.FILE_CONTENT}, {irisndt_upload.FILENAME}", sl);
                }

                if (a_iu != null && a_iu.Count > 0)
                {
                    irisndt_upload_data upldd = (irisndt_upload_data)a_iu[0];

                    pdf_doc_data dd = new pdf_doc_data();

                    dd.id = upldd.id;
                    dd.pdf = upldd.file_content;

                    if (dd.pdf != null && dd.pdf.Length > 0)
                    {
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("Content-Type", "application/pdf");

                        Response.AddHeader("Content-Disposition", "inline; filename=WeldTestReport.pdf");

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