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
    public partial class fy_assign_job : System.Web.UI.Page
    {
        const string VS_WELDERS = "VS_WELDERS";
        const string VS_FITTERS = "VS_FITTERS";
        const string DELIM = "\t";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (!IsPostBack)
            {
                cboWelder.Items.Add(string.Empty);
                cboFitter.Items.Add(string.Empty);

                object[] a = fy.ws().get_welders(fy.PASSKEY);

                ViewState[VS_WELDERS] = a;

                ArrayList al = new ArrayList();

                if (a != null)
                {
                    foreach (gbe_ws.user_data ud in a)
                    {
                        if (ud.login_id.Trim().Length > 0)
                            al.Add(ud.login_id);
                    }

                    al.Sort();

                    foreach (string s in al)
                    {
                        cboWelder.Items.Add(s);
                    }
                }

                a = fy.ws().get_fitters(fy.PASSKEY);

                ViewState[VS_FITTERS] = a;

                if (a != null)
                {
                    al.Clear();

                    foreach (gbe_ws.user_data ud in a)
                    {
                        if (ud.login_id.Trim().Length > 0)
                            al.Add(ud.login_id);
                    }

                    al.Sort();

                    foreach (string s in al)
                    {
                        cboFitter.Items.Add(s);
                    }
                }

                txtBarcode.Attributes.Add("onfocus", "this.select()");
                txtBarcode.Focus();
            }
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

        protected void btnAssign_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (lstBarcodes.Items.Count > 0
                    && (cboWelder.Text.Length > 0
                        ||cboFitter.Text.Length > 0))
                {
                    if (!fy.ws().is_alive(fy.PASSKEY))
                    {
                        lblMsg.Text = "Unable to contact webservice";
                        return;
                    }

                    ArrayList aspool_ids = new ArrayList();
                    ArrayList a_bad_spools_idx = new ArrayList();
                    SortedList sl_spool_status = new SortedList();
                    ArrayList a_welder_required_for_robot = new ArrayList();

                    int idx = 0;
                    foreach (ListItem li in lstBarcodes.Items)
                    {
                        string bc = li.Text.Split(DELIM[0])[0].Trim();

                        gbe_ws.spool_data sd = fy.ws().get_spool(fy.PASSKEY, bc);

                        if (sd != null)
                        {
                            if ((sd.fitter == 0 && sd.welder == 0) || fy.is_privileged_user() > 0)
                            {
                                if (sd.status == "SC" || sd.status == "RP" || fy.is_super_privileged_user() > 0)
                                {
                                    if (is_welder_required(sd.id))
                                    {
                                        lstBarcodes.Items[idx].Text = bc + DELIM + "** WELDER REQUIRED FOR FILLET WELDS **";

                                        a_welder_required_for_robot.Add(idx);
                                    }
                                    else
                                    {
                                        aspool_ids.Add(sd.id);

                                        if (!sl_spool_status.ContainsKey(sd.id))
                                            sl_spool_status.Add(sd.id, sd.status);
                                    }
                                }
                                else
                                {
                                    lstBarcodes.Items[idx].Text = bc + DELIM + "** STATUS MUST BE SC OR RP **";

                                    a_bad_spools_idx.Add(idx);
                                }
                            }
                            else
                            {
                                lstBarcodes.Items[idx].Text = bc + DELIM + "** FITTER/WELDER ALREADY ASSIGNED **";

                                a_bad_spools_idx.Add(idx);
                            }
                        }
                        else
                        {
                            lstBarcodes.Items[idx].Text = bc + DELIM + "** SPOOL NOT FOUND **";

                            a_bad_spools_idx.Add(idx);
                        }
                        
                        idx++;
                    }

                    if(a_bad_spools_idx.Count > 0)
                    {
                        lblMsg.Text = "One or more spools could not be assigned. <br/>Please remove from the list.";
                    }

                    if (a_welder_required_for_robot.Count > 0)
                    {
                        if (lblMsg.Text.Trim().Length > 0)
                            lblMsg.Text += "<br/>";


                        lblMsg.Text += "Please select a welder. The welder will be assigned to the spools with fillet welds.";

                        cboWelder.Focus();
                    }

                    if (a_bad_spools_idx.Count > 0 || a_welder_required_for_robot.Count > 0)
                    {
                        return;
                    }

                    if (aspool_ids.Count > 0)
                    {
                        ArrayList ansd = new ArrayList();
                        ArrayList asd = new ArrayList();

                        object[] onsd = fy.ws().get_spools(fy.PASSKEY, true, string.Empty, string.Empty);

                        foreach (int spool_id in aspool_ids)
                        {
                            int nsd_id = 0;

                            foreach (gbe_ws.new_spool_data nsd in onsd)
                            {
                                if (nsd.spool_id == spool_id)
                                    nsd_id = nsd.id;
                            }

                            gbe_ws.key_value kv = null;

                            if (nsd_id > 0)
                            {
                                if (cboWelder.Text.Trim().Length > 0)
                                {
                                    gbe_ws.key_value_container kvc = new gbe_ws.key_value_container();

                                    kvc.container = new object[2];

                                    kv = new gbe_ws.key_value();

                                    kv.key = "id";
                                    kv.value = nsd_id;

                                    kvc.container[0] = kv;

                                    kv = new gbe_ws.key_value();

                                    kv.key = "welder_assigned";
                                    kv.value = true;

                                    kvc.container[1] = kv;

                                    ansd.Add(kvc);
                                }

                                if (cboFitter.Text.Trim().Length > 0)
                                {
                                    gbe_ws.key_value_container kvc = new gbe_ws.key_value_container();

                                    kvc.container = new object[2];

                                    kv = new gbe_ws.key_value();

                                    kv.key = "id";
                                    kv.value = nsd_id;

                                    kvc.container[0] = kv;

                                    kv = new gbe_ws.key_value();

                                    kv.key = "fitter_assigned";
                                    kv.value = true;

                                    kvc.container[1] = kv;

                                    ansd.Add(kvc);
                                }
                            }

                            gbe_ws.key_value_container kvc2 = new gbe_ws.key_value_container();

                            kvc2.container = new object[5];

                            gbe_ws.user_data ud = null;

                            int welder_id = 0;

                            if (cboWelder.Text.Trim().Length > 0 )
                            {
                                ud = get_user_data(cboWelder.Text, VS_WELDERS);
                                
                                welder_id = ud.id;
                            }

                            kv = new gbe_ws.key_value();

                            kv.key = "id";
                            kv.value = spool_id;

                            kvc2.container[0] = kv;

                            kv = new gbe_ws.key_value();

                            kv.key = "welder";
                            kv.value = welder_id;

                            kvc2.container[1] = kv;

                            if (cboFitter.Text.Trim().Length > 0)
                            {
                                ud = get_user_data(cboFitter.Text, VS_FITTERS);

                                kv = new gbe_ws.key_value();

                                kv.key = "id";
                                kv.value = spool_id;

                                kvc2.container[0] = kv;

                                kv = new gbe_ws.key_value();

                                kv.key = "fitter";
                                kv.value = ud.id;

                                kvc2.container[2] = kv;
                            }

                            kv = new gbe_ws.key_value();

                            kv.key = "audit_trail";
                            kv.value = "WELDER ASSIGNED";

                            kvc2.container[3] = kv;

                            string s_spool_status = string.Empty;

                            if (sl_spool_status.ContainsKey(spool_id))
                                s_spool_status = sl_spool_status[spool_id].ToString();

                            if (s_spool_status == "SC")
                            {
                                kv = new gbe_ws.key_value();

                                kv.key = "status";
                                kv.value = "RP";

                                kvc2.container[4] = kv;
                            }

                            asd.Add(kvc2);
                        }

                        fy.ws().save_new_spool_data(fy.PASSKEY, ansd.ToArray());

                        string user_id = System.Web.HttpContext.Current.User.Identity.Name;
                        fy.ws().save_spool_data(fy.PASSKEY, asd.ToArray(), user_id);

                        //if (chkRobot.Checked)
                        //{
                            foreach (int spool_id in aspool_ids)
                            {
                                SortedList sl_wj = new SortedList();

                                sl_wj.Add("spool_id", spool_id);

                                using (weld_jobs wj = new weld_jobs())
                                {
                                    ArrayList a_wjd = wj.get_weld_job_data(sl_wj);

                                    foreach (weld_job_data wjd in a_wjd)
                                    {
                                        sl_wj.Clear();

                                        sl_wj.Add("id", wjd.id);
                                        sl_wj.Add("robot", chkRobot.Checked?1:0);

                                        wj.save_weld_job_data(sl_wj);
                                    }
                                }
                            }
                        //}
                    }

                    string msg = "Jobs assigned";
                    string return_url = "fy_assign_job.aspx";

                    fy.show_msg(msg, return_url, this);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "btnAssign_Click<br/>" + ex.Message.ToString() + "<br/>" + ex.ToString();
            }
        }

        bool is_welder_required(int spool_id)
        {
            bool bret = false;

            if (chkRobot.Checked && cboWelder.Text.Trim().Length == 0)
            {
                bret = has_fillet_welds(spool_id);
            }

            return bret;
        }

        bool has_fillet_welds(int spool_id)
        {
            bool bret = false;

            string sql = "select id from spool_parts where fw > 0 and spool_id = " + spool_id.ToString();

            using (cdb_connection dbc = new cdb_connection())
            {
                DataTable dtab = dbc.get_data(sql);

                bret = dtab.Rows.Count > 0;
            }

            return bret;
        }

        gbe_ws.user_data get_user_data(string login_id, string vs)
        {
            gbe_ws.user_data ud = null;

            object[] a_ud = (object[])ViewState[vs];

            foreach (gbe_ws.user_data ud0 in a_ud)
            {
                if (ud0.login_id == login_id)
                {
                    ud = ud0;
                    break;
                }
            }

            return ud;
        }

        protected void btnMoveUp_Click(object sender, ImageClickEventArgs e)
        {
            move_list_item(-1);
        }

        protected void btnMoveDown_Click(object sender, ImageClickEventArgs e)
        {
            move_list_item(1);
        }

        void move_list_item(int idir)
        {
            if (lstBarcodes.SelectedIndex >= 0)
            {
                int idx = lstBarcodes.SelectedIndex + idir;

                if (idx >= 0 && idx < lstBarcodes.Items.Count)
                {
                    string bc = lstBarcodes.Items[lstBarcodes.SelectedIndex].Text;

                    lstBarcodes.Items.RemoveAt(lstBarcodes.SelectedIndex);

                    lstBarcodes.Items.Insert(idx, bc);
                }
            }
        }
    }
}
