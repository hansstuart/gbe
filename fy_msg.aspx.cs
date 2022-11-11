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
    public partial class fy_msg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string x = Request.QueryString["x"];

            lblMsg.Text = get_param("msg");
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            string url = get_param("return_url");

            if (url.Trim().Length == 0)
                url = "fy.aspx";

            Response.Redirect(url);
        }

        string get_param(string k)
        {
            string sret = string.Empty;

            string x = Request.QueryString["x"];

            if (x != null)
            {
                SortedList sl_p = PCFsecure.PCFUtil.page_params(x);

                if (sl_p.ContainsKey(k))
                    sret = sl_p[k].ToString();

            }

            return sret;
        }
    }
}
