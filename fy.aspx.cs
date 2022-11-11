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
    public partial class fy : System.Web.UI.Page
    {
        public static string PASSKEY = "33b33g33b33";
        static string ACCESS_DENIED = "Access Denied";

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageButton btnBack = (ImageButton)Master.FindControl("btnBack");
            btnBack.Visible = false;
        }

        public static gbe_ws.cgbe ws()
        {
            string ws_url = System.Web.Configuration.WebConfigurationManager.AppSettings["ws_url"].ToString();

            gbe_ws.cgbe ws = new gbe_ws.cgbe();

            ws.Url = ws_url;
            ws.Timeout = 1 * 60 * 1000;

            return ws;
        }

        public static void show_msg(string msg, string return_url, Page page)
        {
            string url = "fy_msg.aspx?x=" + PCFsecure.PCFUtil.tohex(PCFsecure.PCFUtil.EncryptData("msg=" + msg + "&return_url=" + return_url, PCFsecure.PCFUtil.PW));

            page.Response.Redirect(url);
        }

        protected void btnAssignWelder_Click(object sender, EventArgs e)
        {
            string user_name = System.Web.HttpContext.Current.User.Identity.Name;
            int user_id = fy.ws().is_admin_user(fy.PASSKEY, user_name);

            if (user_id == 0)
            {
                lblMsg.Text = (ACCESS_DENIED);
                return ;
            }
            else
                Response.Redirect("fy_assign_job.aspx");
        }

        public static int is_super_privileged_user()
        {
            int iret = 0;

            string user_id = System.Web.HttpContext.Current.User.Identity.Name;

            using (users u = new users())
            {
                user_data ud = u.get_user_data(user_id);

                if (ud.is_special_permission_set(1))
                    iret = ud.id;
            }

            return iret;
        }

        public static int is_privileged_user()
        {
            int iret = 0;

            string user_id = System.Web.HttpContext.Current.User.Identity.Name;

            using (users u = new users())
            {
                user_data ud = u.get_user_data(user_id);

                if (ud.is_special_permission_set(0))
                    iret = ud.id;
            }

            return iret;
        }

        public static bool can_start_stop_job()
        {
            bool bcan_start_stop_job = false;

            string user_id = System.Web.HttpContext.Current.User.Identity.Name;

            bcan_start_stop_job = is_privileged_user() > 0;

            bcan_start_stop_job |= ((fy.ws().is_welder(fy.PASSKEY, user_id) > 0)
                                    || (fy.ws().is_fitter(fy.PASSKEY, user_id) > 0)
                                    || (fy.ws().is_module_builder(fy.PASSKEY, user_id) > 0));

            return bcan_start_stop_job;
        }

        protected void btnWeld_Click(object sender, EventArgs e)
        {
            if (can_start_stop_job())
            {
                Response.Redirect("fy_weld.aspx");
            }
            else
            {
                lblMsg.Text = (ACCESS_DENIED);
                return ;
            }
        }

        protected void btnQA_Click(object sender, EventArgs e)
        {
            string user_name = System.Web.HttpContext.Current.User.Identity.Name;

            int user_id = fy.ws().is_qa_user(fy.PASSKEY, user_name);

            if (user_id == 0)
            {
                lblMsg.Text = (ACCESS_DENIED);
                return ;
            }
            else
                Response.Redirect("fy_qa.aspx");
        }

        protected void btnWeldTest_Click(object sender, EventArgs e)
        {
            string user_name = System.Web.HttpContext.Current.User.Identity.Name;

            int user_id = fy.ws().is_weld_tester(fy.PASSKEY, user_name);

            if (user_id == 0)
            {
                lblMsg.Text = (ACCESS_DENIED);
                return;
            }
            else
                Response.Redirect("fy_weld_test.aspx");
        }

        protected void btnDelivery_Click(object sender, EventArgs e)
        {
            string user_name = System.Web.HttpContext.Current.User.Identity.Name;

            int user_id = fy.ws().is_driver_or_loader(fy.PASSKEY, user_name);

            if (user_id == 0)
            {
                lblMsg.Text = (ACCESS_DENIED);
                return;
            }
            else
                Response.Redirect("fy_delivery.aspx");
        }
    }
}
