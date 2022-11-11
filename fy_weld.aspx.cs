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
    public partial class fy_weld : System.Web.UI.Page
    {
        const string VS_BWELDER = "VS_BWELDER";
        const string VS_BFITTER = "VS_BFITTER";
        const string VS_BMODULE_BUILDER = "VS_BMODULE_BUILDER";
        const string VS_USER_ID = "VS_USER_ID";
        const string VS_SPOOL_DATA = "VS_SPOOL_DATA";
        const string VS_MODULE_DATA = "VS_MODULE_DATA";
        const string VS_WELD_JOB_DATA = "VS_WELD_JOB_DATA";

        const string PROCESS_WELD = "WELD";
        const string PROCESS_FIT = "FIT";
        const string PROCESS_MODULE = "MODULE";

        const string DELIM = "\t";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (!IsPostBack)
            {
                txtBarcode.Attributes.Add("onfocus", "this.select()");
                txtBarcode.Focus();

                if (fy.is_privileged_user() > 0)
                {
                    pnlProcess.Visible = true;
                    dlProcess.Items.Add(PROCESS_WELD);
                    dlProcess.Items.Add(PROCESS_FIT);
                    dlProcess.Items.Add(PROCESS_MODULE);
                }
                else
                    pnlProcess.Visible = false;
            }
        }

        protected void btnStart_Click(object sender, ImageClickEventArgs e)
        {
            ViewState[VS_BWELDER] = false;
            ViewState[VS_BFITTER] = false;
            ViewState[VS_BMODULE_BUILDER] = false;

            try
            {
                if (!fy.ws().is_alive(fy.PASSKEY))
                {
                    lblMsg.Text = ("Unable to contact webservice");
                    return;
                }

                start_process();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "btnStart_Click()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        protected void btnStop_Click(object sender, ImageClickEventArgs e)
        {
            ViewState[VS_BWELDER] = false;
            ViewState[VS_BFITTER] = false;
            ViewState[VS_BMODULE_BUILDER] = false;

            try
            {
                if (!fy.ws().is_alive(fy.PASSKEY))
                {
                    lblMsg.Text = ("Unable to contact webservice");
                    return;
                }

                finish_process();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "btnStop_Click()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        void start_process()
        {
            if (is_valid_input())
            {
                bool berrors_found = false;

                bool bwelder = (bool)ViewState[VS_BWELDER];
                bool bmodule_builder = (bool)ViewState[VS_BMODULE_BUILDER];
                bool bfitter = (bool)ViewState[VS_BFITTER];
                int user_id = (int)ViewState[VS_USER_ID];
                                
                ArrayList a_idx_processed = new ArrayList();
                int idx = -1;

                try
                {
                    bool bprivileged_user = fy.is_privileged_user() > 0;

                    foreach (ListItem li in lstBarcodes.Items)
                    {
                        idx++;

                        string barcode = li.Text.Split(DELIM[0])[0].Trim();

                        gbe_ws.weld_job_data weld_job_data = null;
                        gbe_ws.spool_data spool_data = null;
                        gbe_ws.module_data module_data = null;

                        try { weld_job_data = (gbe_ws.weld_job_data)((ArrayList)ViewState[VS_WELD_JOB_DATA])[idx]; }
                        catch { }
                        try { spool_data = (gbe_ws.spool_data)((ArrayList)ViewState[VS_SPOOL_DATA])[idx]; }
                        catch { }
                        try { module_data = (gbe_ws.module_data)((ArrayList)ViewState[VS_MODULE_DATA])[idx]; }
                        catch { }
                        
                        int weld_job_id = 0;

                        if (weld_job_data != null)
                        {
                            weld_job_id = weld_job_data.id;

                            if ((weld_job_data.start > DateTime.MinValue && (bwelder || bmodule_builder))
                                || (weld_job_data.start_fit > DateTime.MinValue && bfitter))
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** ALREADY STARTED **";

                                continue;
                            }
                        }

                        bool b_process_required = true;

                        string process = string.Empty;

                        if (bwelder)
                        {
                            if (spool_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** SPOOL NOT FOUND **";
                                continue;
                            }

                            if (!spool_data.weld_required)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** WELD NOT REQUIRED **";
                                continue;
                            }

                            if (spool_data.welder == 0 && weld_job_data.robot == 0)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** WELDER UNASSIGNED **";
                                continue;
                            }

                            if (!bprivileged_user && user_id != spool_data.welder)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** INCORRECT USER ID **";
                                continue;
                            }
                        }
                        else if (bfitter)
                        {
                            if (spool_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** SPOOL NOT FOUND **";
                                continue;
                            }

                            if (!spool_data.fit_required)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** FIT NOT REQUIRED **";
                                continue;
                            }

                            if ( spool_data.fitter == 0)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** FITTER UNASSIGNED **";
                                continue;
                            }

                            if (!bprivileged_user && user_id != spool_data.fitter)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** INCORRECT USER ID **";
                                continue;
                            }
                        }
                        else if (bmodule_builder)
                        {
                            if (module_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** MODULE NOT FOUND **";
                                continue;
                            }

                            process = "BUILD";
                            b_process_required = true;
                        }

                        if (b_process_required)
                        {
                            int id = 0;
                            int assembly_type = -1;

                            if (bwelder || bfitter)
                            {
                                id = spool_data.id;
                                assembly_type = 0;
                            }
                            else if (bmodule_builder)
                            {
                                id = module_data.id;
                                assembly_type = 1;
                            }

                            if (fy.ws().save_weld_job_2(fy.PASSKEY, id, assembly_type, weld_job_id, (bwelder || bmodule_builder?spool_data.welder:spool_data.fitter), (bwelder || bmodule_builder ? 100 : 102)))
                            {
                                a_idx_processed.Add(idx);
                            }
                            else
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** PROBLEM SAVING DATA **";
                            }
                        }
                    } 

                    a_idx_processed.Reverse();

                    foreach (int i in a_idx_processed)
                    {
                        lstBarcodes.Items.RemoveAt(i);
                    }

                    if (a_idx_processed.Count > 0)
                    {
                        if (bwelder)
                            lblMsg.Text = ("Started");
                        else if (bfitter)
                            lblMsg.Text = ("Started");
                        else if (bmodule_builder)
                            lblMsg.Text = ("Module build started");

                        txtBarcode.Text = string.Empty;
                        txtBarcode.Focus();
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "frmWeld::start_process()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                }
            }
        }

        void finish_process()
        {
            if (is_valid_input())
            {
                bool berrors_found = false;

                bool bwelder = (bool)ViewState[VS_BWELDER];
                bool bmodule_builder = (bool)ViewState[VS_BMODULE_BUILDER];
                bool bfitter = (bool)ViewState[VS_BFITTER];
                int user_id = (int)ViewState[VS_USER_ID];

                bool bprivileged_user = fy.is_privileged_user() > 0;

                ArrayList a_idx_processed = new ArrayList();
                int idx = -1;

                foreach (ListItem li in lstBarcodes.Items)
                {
                    idx++;

                    string barcode = li.Text.Split(DELIM[0])[0].Trim();

                    try
                    {
                        gbe_ws.weld_job_data weld_job_data = null;
                        gbe_ws.spool_data spool_data = null;
                        gbe_ws.module_data module_data = null;

                        try { weld_job_data = (gbe_ws.weld_job_data)((ArrayList)ViewState[VS_WELD_JOB_DATA])[idx]; }
                        catch { }
                        try { spool_data = (gbe_ws.spool_data)((ArrayList)ViewState[VS_SPOOL_DATA])[idx]; }
                        catch { }
                        try { module_data = (gbe_ws.module_data)((ArrayList)ViewState[VS_MODULE_DATA])[idx]; }
                        catch { }

                        bool b_process_required = true;

                        if (bwelder)
                        {
                            if (spool_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** SPOOL NOT FOUND **";
                                continue;
                            }

                            if (!spool_data.weld_required)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** WELD NOT REQUIRED **";
                                continue;
                            }

                            if (spool_data.welder == 0 && weld_job_data.robot == 0)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** WELDER UNASSIGNED **";
                                continue;
                            }

                            if (!bprivileged_user && user_id != spool_data.welder)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** INCORRECT USER ID **";
                                continue;
                            }
                        }
                        else if (bfitter)
                        {
                            if (spool_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** SPOOL NOT FOUND **";
                                continue;
                            }

                            if (!spool_data.fit_required)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** FIT NOT REQUIRED **";
                                continue;
                            }

                            if (spool_data.fitter == 0)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** FITTER UNASSIGNED **";
                                continue;
                            }

                            if (!bprivileged_user && user_id != spool_data.fitter)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** INCORRECT USER ID **";
                                continue;
                            }
                        }
                        else if (bmodule_builder)
                        {
                            if (module_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** MODULE NOT FOUND **";
                                continue;
                            }

                            b_process_required = true;
                        }

                        if (b_process_required)
                        {
                            if (weld_job_data == null)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** NOT STARTED **";

                                continue;
                            }
                        }

                        if ((bwelder && spool_data.weld_required) || bmodule_builder)
                        {
                            if (weld_job_data.start == DateTime.MinValue)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** NOT STARTED **";

                                continue;
                            }

                            if (weld_job_data.finish != DateTime.MinValue)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** ALREADY COMPLETED **";

                                continue;
                            }
                        }

                        if (bfitter && spool_data.fit_required)
                        {
                            if (weld_job_data.start_fit == DateTime.MinValue)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** NOT STARTED **";

                                continue;
                            }

                            if (weld_job_data.finish_fit != DateTime.MinValue)
                            {
                                lstBarcodes.Items[idx].Text = barcode + DELIM + "** ALREADY COMPLETED **";

                                continue;
                            }
                        }
                        int id = 0;
                        int assembly_type = -1;

                        if (bwelder || bfitter)
                        {
                            id = spool_data.id;
                            assembly_type = 0;
                        }
                        else if (bmodule_builder)
                        {
                            id = module_data.id;
                            assembly_type = 1;
                        }

                        if (fy.ws().save_weld_job_2(fy.PASSKEY, id, assembly_type, weld_job_data.id, (bwelder || bmodule_builder ? spool_data.welder : spool_data.fitter), (bwelder || bmodule_builder ? 101 : 103)))
                        {
                            a_idx_processed.Add(idx);
                        }
                        else
                        {
                            lstBarcodes.Items[idx].Text = barcode + DELIM + "** PROBLEM SAVING DATA **";
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMsg.Text = "frmWeld::finish_process()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                    }
                }

                a_idx_processed.Reverse();

                foreach (int i in a_idx_processed)
                {
                    lstBarcodes.Items.RemoveAt(i);
                }

                if (a_idx_processed.Count > 0)
                {
                    if (bwelder)
                        lblMsg.Text = ("Completed");
                    else if (bfitter)
                        lblMsg.Text = ("Completed");
                    else if (bmodule_builder)
                        lblMsg.Text = ("Module build completed");

                    txtBarcode.Text = string.Empty;
                    txtBarcode.Focus();
                }
            }
        }

        bool is_valid_input()
        {
            bool bret = true;

            try
            {
                if (lstBarcodes.Items.Count == 0)
                {
                    lblMsg.Text = ("Spool(s) missing");
                    txtBarcode.Focus();
                    return false;
                }

                string user_id = System.Web.HttpContext.Current.User.Identity.Name;

                if (!fy.can_start_stop_job())
                {
                    lblMsg.Text = ("Incorrect User ID");
                    return false;
                }

                bool bwelder, bfitter, bmodule_builder, bprivileged_user;
                bwelder = bfitter = bmodule_builder = bprivileged_user = false;

                if (fy.is_privileged_user() > 0)
                    bprivileged_user = true;
                
                if (is_welder(user_id))
                    bwelder = true;
                else if (is_fitter(user_id))
                    bfitter = true;
                else if (is_module_builder(user_id))
                    bmodule_builder = true;

                ViewState[VS_USER_ID] = fy.ws().get_user_id(fy.PASSKEY, user_id);

                int idx = 0;

                ArrayList a_vs_spool_data = new ArrayList();
                ArrayList a_vs_weld_job_data = new ArrayList();
                ArrayList a_vs_module_data = new ArrayList();

                foreach (ListItem li in lstBarcodes.Items)
                {
                    string barcode = li.Text.Split(DELIM[0])[0].Trim();

                    if (bwelder || bfitter || bprivileged_user)
                    {
                        gbe_ws.spool_data spool_data = fy.ws().get_spool(fy.PASSKEY, barcode);

                        a_vs_spool_data.Add(spool_data);

                        if (spool_data != null)
                        {
                            gbe_ws.weld_job_data weld_job_data = fy.ws().get_weld_job_data_2(fy.PASSKEY, spool_data.id, 0);
                            a_vs_weld_job_data.Add(weld_job_data);
                        }
                        else
                        {
                            a_vs_weld_job_data.Add(null);
                        }
                    }
                    else if (bmodule_builder || bprivileged_user)
                    {
                        gbe_ws.module_data module_data = fy.ws().get_module(fy.PASSKEY, barcode);
                        a_vs_module_data.Add(module_data);

                        if (module_data != null)
                        {
                            gbe_ws.weld_job_data weld_job_data = fy.ws().get_weld_job_data_2(fy.PASSKEY, module_data.id, 1);
                            a_vs_weld_job_data.Add(weld_job_data);
                        }
                        else
                        {
                            a_vs_weld_job_data.Add(null);
                        }
                    }
                    else
                    {
                        lblMsg.Text = ("Unable to process due to invalid user ID");
                        return false;
                    }

                    idx++;
                }

                ViewState[VS_SPOOL_DATA] = a_vs_spool_data;
                ViewState[VS_WELD_JOB_DATA] = a_vs_weld_job_data;
                ViewState[VS_MODULE_DATA] = a_vs_module_data;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "is_valid_input()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                bret = false;
            }

            return bret;
        }

        bool is_fitter(string id)
        {
            bool bret = false;
            ViewState[VS_BFITTER] = false;

            int privileged_user_id = fy.is_privileged_user();

            if (privileged_user_id > 0)
            {
                if (dlProcess.Text == PROCESS_FIT)
                {
                    bret = true;
                    ViewState[VS_BFITTER] = true;
                }
            }
            else
            {
                try
                {
                    if (fy.ws().is_fitter(fy.PASSKEY, id) > 0)
                    {
                        bret = true;
                        ViewState[VS_BFITTER] = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "is_fitter()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                }
            }

            return bret;
        }

        bool is_welder(string id)
        {
            bool bret = false;
            ViewState[VS_BWELDER] = false;

            int privileged_user_id = fy.is_privileged_user();

            if (privileged_user_id > 0)
            {
                if (dlProcess.Text == PROCESS_WELD)
                {
                    bret = true;
                    ViewState[VS_BWELDER] = true;
                }
            }
            else
            {
                try
                {
                    if ( fy.ws().is_welder(fy.PASSKEY, id) > 0)
                    {
                        bret = true;
                        ViewState[VS_BWELDER] = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "is_welder()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                }
            }

            return bret;
        }

        bool is_module_builder(string id)
        {
            bool bret = false;
            ViewState[VS_BMODULE_BUILDER] = false;

            int privileged_user_id = fy.is_privileged_user();

            if (privileged_user_id > 0)
            {
                if (dlProcess.Text == PROCESS_MODULE)
                {
                    bret = true;
                    ViewState[VS_BMODULE_BUILDER] = true;
                }
            }
            else
            {
                try
                {
                    if (fy.ws().is_module_builder(fy.PASSKEY, id) > 0)
                    {
                        bret = true;
                        ViewState[VS_BMODULE_BUILDER] = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "is_module_builder()<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
                }
            }
            return bret;
        }

        void update_spool(bool bwelder, gbe_ws.spool_data spool_data)
        {
            ArrayList asd = new ArrayList();

            gbe_ws.key_value kv = null;

            gbe_ws.key_value_container kvc2 = new gbe_ws.key_value_container();

            kvc2.container = new object[3];

            kv = new gbe_ws.key_value();
            
            int user_id = (int)ViewState[VS_USER_ID];

            kv.key = "id";
            kv.value = spool_data.id;

            kvc2.container[0] = kv;

            kv = new gbe_ws.key_value();

            kv.key = bwelder ? "welder" : "fitter";

            kv.value = user_id;

            kvc2.container[1] = kv;

            kv = new gbe_ws.key_value();

            kv.key = "audit_trail";
            kv.value = "WELDER ASSIGNED";

            kvc2.container[2] = kv;

            asd.Add(kvc2);

            string user_name = System.Web.HttpContext.Current.User.Identity.Name;
            fy.ws().save_spool_data(fy.PASSKEY, asd.ToArray(), user_name);
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
    }
}
