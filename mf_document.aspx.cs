using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;

namespace gbe
{
    public partial class mf_document : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string filename = Request.QueryString["fn"];
            string doc_name = Request.QueryString["dn"];
            bool delete_file = Request.QueryString["df"]=="1";

            if (filename != null && doc_name != null)
            {
                string pdf_file = string.Empty;

                string dir = Path.Combine(Server.MapPath("~/"), "temp");

                pdf_file = Path.Combine(dir, filename);

                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Type", "application/pdf");

                Response.AddHeader("Content-Disposition", $"inline; filename={doc_name}.pdf");

                Response.Buffer = true;

                Response.BinaryWrite(File.ReadAllBytes(pdf_file));

                if (delete_file)
                {
                    try { File.Delete(pdf_file); } catch { }
                }

                Response.Flush();

                try { Response.End(); }
                catch { }

                
            }
        }
    }
}