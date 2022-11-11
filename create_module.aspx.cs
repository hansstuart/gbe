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
    public partial class create_module : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtModuleNumber.Focus();
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                if (is_valid_module_number())
                {
                    if (txtRevision.Text.Trim().Length > 0)
                    {
                        if (!module_exists())
                        {
                            try
                            {
                                save_module();
                                lblMsg.Text = txtModuleNumber.Text + "/" + txtRevision.Text + " saved.";
                                txtModuleNumber.Text = txtRevision.Text = string.Empty;
                                txtModuleNumber.Focus();
                            }
                            catch (Exception ex)
                            {
                                lblMsg.Text = "Error\r\n" + ex.ToString();
                            }
                        }
                        else
                        {
                            lblMsg.Text = "Module already exists";
                            txtModuleNumber.Focus();
                        }
                    }
                    else
                    {
                        lblMsg.Text = "Revision missing";
                        txtRevision.Focus();
                    }
                }
                else
                {
                    lblMsg.Text = "Invalid Module Number";
                    txtModuleNumber.Focus();
                }
            }
        }

        bool is_valid_module_number()
        {
            bool bret = false;
            
            string mn = txtModuleNumber.Text.Trim();

            if (mn.Length > 0)
            {
                string[] sa_mn = mn.Split('-');

                if (sa_mn.Length > 2)
                {
                    if (sa_mn[sa_mn.Length - 1] == "MOD")
                    {
                        using(customers c = new customers())
                        {
                            SortedList sl = new SortedList();
                            sl.Add("contract_number", sa_mn[0]);

                            if (c.get_customer_data(sl).Count > 0)
                                bret = true;
                        }
                    }
                }
            }

            return bret;
        }

        bool module_exists()
        {
            bool bret = false;

            using (modules m = new modules())
            {
                SortedList sl = new SortedList();
                sl.Add("module", txtModuleNumber.Text);
                sl.Add("revision", txtRevision.Text);

                bret = (m.get_module_data(sl).Count > 0);
            }
            return bret;
        }

        void save_module()
        {
            using (modules m = new modules())
            {
                SortedList sl = new SortedList();
                sl.Add("module", txtModuleNumber.Text);
                sl.Add("revision", txtRevision.Text);
                sl.Add("barcode", txtModuleNumber.Text+"_"+txtRevision.Text);

                m.save_module_data(sl);

                using (new_spools b = new new_spools())
                {
                    SortedList slb = new SortedList();
                    slb.Add("spool_id", sl["id"].ToString());
                    slb.Add("printed", false);
                    slb.Add("assembly_type", new_spool_data.MODULE);

                    b.save_new_spool_data(slb);
                }
            }
        }
    }
}
