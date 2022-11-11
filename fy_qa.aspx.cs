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
    public partial class fy_qa : System.Web.UI.Page
    {
        const string VS_USER_ID = "VS_USER_ID";
        const string VS_SPOOL_DATA = "VS_SPOOL_DATA";
        const string VS_MODULE_DATA = "VS_MODULE_DATA";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            txtBarcode.Attributes.Add("onfocus", "this.select()");
            txtBarcode.Focus();
        }

        protected void btnPass_Click(object sender, ImageClickEventArgs e)
        {
            update("PASS");
        }

        protected void btnFail_Click(object sender, ImageClickEventArgs e)
        {
            update("FAIL");
        }

        private void update(string result)
        {
            lblMsg.Text = string.Empty;

            if (is_valid_input())
            {
                try
                {
                    if (!fy.ws().is_alive(fy.PASSKEY))
                    {
                        lblMsg.Text = ("Unable to contact webservice");
                        return;
                    }
                    
                    int assembly_type = -1;

                    ArrayList a_spool_ids =  (ArrayList) ViewState[VS_SPOOL_DATA];
                    ArrayList a_module_ids = (ArrayList)ViewState[VS_MODULE_DATA];
                    ArrayList a_ids = null;

                    int user_id = (int)ViewState[VS_USER_ID];

                    if (a_spool_ids != null)
                    {
                        a_ids = a_spool_ids;
                        assembly_type = 0;
                    }
                    else if (a_module_ids != null)
                    {
                        a_ids = a_module_ids;
                        assembly_type = 1;
                    }

                    foreach (int id in a_ids)
                    {
                        if (id > 0)
                        {
                            if (!fy.ws().save_qa_job_2(fy.PASSKEY, id, assembly_type, user_id, result))
                            {
                                lblMsg.Text = ("Problem saving data");
                                return;
                            }
                        }
                        else
                        {
                            lblMsg.Text = ("Problem with data");
                            return;
                        }
                    }

                    lblMsg.Text = ("QA complete");
                    txtBarcode.Text = string.Empty;
                    lstBarcodes.Items.Clear();
                    ViewState.Clear();
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "update()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                }
            }
        }

        bool is_valid_input()
        {
            bool bret = false;

            ArrayList a_spool_ids = new ArrayList();
            ArrayList a_module_ids = new ArrayList();
            
            gbe_ws.spool_data spool_data = null;
            gbe_ws.module_data module_data = null;

            try
            {
                if (lstBarcodes.Items.Count == 0)
                {
                    lblMsg.Text = ("Spool missing");
                    txtBarcode.Focus();

                    return bret;
                }

                int user_id = fy.ws().is_qa_user(fy.PASSKEY, System.Web.HttpContext.Current.User.Identity.Name.Trim());

                if (user_id == 0)
                {
                    lblMsg.Text = ("Incorrect ID");
                    return bret;
                }
                else
                    ViewState[VS_USER_ID] = user_id;

                
                for(int i = 0; i <lstBarcodes.Items.Count; i++)
                {
                    lstBarcodes.Items[i].Selected = true;

                    string barcode = lstBarcodes.Items[i].Text;

                    spool_data = fy.ws().get_spool(fy.PASSKEY, barcode.Trim());

                    if (spool_data == null)
                    {
                        module_data = fy.ws().get_module(fy.PASSKEY, txtBarcode.Text.Trim());

                        if (module_data == null)
                        {
                            lblMsg.Text = ("Spool/Module not found.<br/>" + barcode);
                            txtBarcode.Focus();

                            return bret;
                        }
                    }

                    if (spool_data != null)
                    {
                        a_spool_ids.Add(spool_data.id);
                        
                        bool bfit_started, bfit_finished, bweld_started, bweld_finished;
                        bfit_started = bfit_finished = bweld_started = bweld_finished = false;

                        fy.ws().is_fitted_and_welded(fy.PASSKEY, spool_data.id, ref bfit_started, ref bfit_finished, ref bweld_started, ref bweld_finished);

                        bret = bfit_started && bfit_finished && bweld_started && bweld_finished;

                        if (!bret)
                        {
                            if (!bfit_started)
                                lblMsg.Text += "Fit not started.<br/>";

                            if (!bfit_finished)
                                lblMsg.Text += "Fit not finished.<br/>";

                            if (!bweld_started)
                                lblMsg.Text += "Weld not started.<br/>";

                            if (!bweld_finished)
                                lblMsg.Text += "Weld not finished.<br/>";

                            lblMsg.Text += barcode;

                            return bret;
                        }
                        else
                            bret = true;
                    }
                    else if (module_data != null)
                    {
                        a_module_ids.Add(module_data.id);
                                                

                        bool bbuild_started, bbuild_finished;
                        bbuild_started = bbuild_finished = false;

                        fy.ws().is_module_built(fy.PASSKEY, module_data.id, ref bbuild_started, ref bbuild_finished);

                        bret = bbuild_started && bbuild_finished;

                        if (!bret)
                        {
                            if (!bbuild_started)
                                lblMsg.Text += "Build not started.<br/>";

                            if (!bbuild_finished)
                                lblMsg.Text += "Build not finished.<br/>";

                            lblMsg.Text += barcode;

                            return bret;
                        }
                        else
                            bret = true;
                    }
                    lstBarcodes.Items[i].Selected = false;
                }

                ViewState[VS_SPOOL_DATA] = a_spool_ids;
                ViewState[VS_MODULE_DATA] = a_module_ids;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "is_valid_input()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }

            return bret;
        }

        protected void btnDelete_Click(object sender, ImageClickEventArgs e)
        {
            ArrayList a_i = new ArrayList();

            for (int i = 0; i < lstBarcodes.Items.Count; i++)
            {
                if (lstBarcodes.Items[i].Selected)
                    a_i.Add(i);
            }

            for (int i = a_i.Count - 1; i >= 0; i--)
                lstBarcodes.Items.RemoveAt((int)a_i[i]);

            txtBarcode.Focus();
        }

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();

            if (barcode.Length > 0)
            {
                bool bAdd = true;

                foreach (ListItem li in lstBarcodes.Items)
                {
                    if (li.Text.ToLower() == barcode.ToLower())
                    {
                        bAdd = false;
                        break;
                    }
                }

                if (bAdd)
                {
                    lstBarcodes.Items.Add(barcode);
                    txtBarcode.Text = string.Empty;
                }
            }

            txtBarcode.Focus();
        }

        protected void btnPass_Click1(object sender, ImageClickEventArgs e)
        {

        }
    }
}
