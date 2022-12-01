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
    public partial class fy_weld_test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (!IsPostBack)
            {
                txtReport1.Focus();
                txtBW.Text = txtFW.Text = "0";
            }
        }

        protected void btnAdd_Click(object sender, ImageClickEventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            int fw, bw;
            fw = bw = 0;

            try
            {
                fw = Convert.ToInt32(txtFW.Text.Trim());
            }
            catch
            {
                lblMsg.Text = "Enter a valid numeric FW";
                txtFW.Focus();
                return;
            }

            try
            {
                bw = Convert.ToInt32(txtBW.Text.Trim());
            }
            catch
            {
                lblMsg.Text = "Enter a valid numeric BW";
                txtBW.Focus();
                return;
            }

            if (barcode.Length > 0)
            {
                bool bAdd = true;

                foreach (ListItem li in lstBarcodes.Items)
                {
                    if (li.Text.ToLower().StartsWith(barcode.ToLower() + "\x01"))
                    {
                        bAdd = false;
                        break;
                    }
                }

                if (bAdd)
                {
                    lstBarcodes.Items.Add(barcode + "\x01, FW = " + fw.ToString() + "\x01, BW = " + bw.ToString());
                    txtBarcode.Text = string.Empty;
                }
            }

            txtBarcode.Focus();
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
        }

        protected void btnSave_Click(object sender, ImageClickEventArgs e)
        {
            save();
        }

        void save()
        {
            try
            {
                if (lstBarcodes.Items.Count > 0)
                {
                    if (txtReport1.Text.Trim().Length == 0)
                    {
                        lblMsg.Text = "Report 1 missing";
                        txtReport1.Focus();
                        return;
                    }

                    if (txtReport2.Text.Trim().Length == 0)
                    {
                        lblMsg.Text = "Report 2 missing";
                        txtReport2.Focus();
                        return;
                    }

                    ArrayList a_bad_spools_idx = new ArrayList();

                    int idx = 0;

                    foreach (ListItem li in lstBarcodes.Items)
                    {
                        string[] sa = li.Text.Split('\x01');

                        gbe_ws.spool_data spool_data = fy.ws().get_spool(fy.PASSKEY, sa[0].Trim());

                        if (spool_data == null)
                        {
                            a_bad_spools_idx.Add(idx);
                        }
                        idx++;
                    }

                    if (a_bad_spools_idx.Count > 0)
                    {
                        foreach (int i in a_bad_spools_idx)
                        {
                            lstBarcodes.Items[i].Selected = true;
                        }

                        lblMsg.Text = "One or more spools could not be found. <br/>Please remove from the list.";

                        return;
                    }

                    foreach (ListItem li in lstBarcodes.Items)
                    {
                        string[] sa = li.Text.Split('\x01');

                        int fw = Convert.ToInt32(sa[1].Split('=')[1].Trim());
                        int bw = Convert.ToInt32(sa[2].Split('=')[1].Trim());

                        gbe_ws.spool_data spool_data = fy.ws().get_spool(fy.PASSKEY, sa[0].Trim());

                        if (spool_data != null)
                        {
                            string user_name = System.Web.HttpContext.Current.User.Identity.Name;

                            int user_id = fy.ws().get_user_id(fy.PASSKEY, user_name);
                            fy.ws().save_weld_test(fy.PASSKEY, spool_data.id, user_id, txtReport1.Text.Trim(), txtReport2.Text.Trim(), fw, bw);
                        }
                    }

                    lstBarcodes.Items.Clear();

                    string msg = "Weld test complete";
                    string return_url = "fy_weld_test.aspx";

                    fy.show_msg(msg, return_url, this);
                    
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "save()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }
    }
}
