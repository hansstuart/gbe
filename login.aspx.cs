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
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUser.Focus();
        }
        
        void log_in()
        {
            bool bsuccess = false;

            if (txtUser.Text.Trim().Length > 0 )
            {
                SortedList sl = new SortedList();
                sl.Add("login_id", txtUser.Text.Trim());

                ArrayList a = new ArrayList();

                using (users u = new users())
                {
                    a = u.get_user_data(sl);
                }

                if (a.Count > 0)
                {
                    user_data ud = (user_data)a[0];

                    if (txtPW.Text.Trim() == ud.password
                        || ud.role == "WELDER"
                        || ud.role == "FITTER"
                        || ud.role == "MODULE BUILDER")
                    {
                        bsuccess = true;
                    }
                }
            }

            if (bsuccess)
            {
                
                FormsAuthentication.RedirectFromLoginPage(txtUser.Text.Trim(), false);
            }
            else
                lblMsg.Text = "Login failure";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            log_in();
        }
    }
}
