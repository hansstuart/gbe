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
using System.Collections.Generic;

namespace gbe
{
    public partial class maint1 : System.Web.UI.Page
    {
        const int ADD_NEW = 1;
        const int REC_PER_PG = 25;

        string m_tbl = string.Empty;
        string m_order_by = string.Empty;

        DataTable m_dta = null;
        int m_state = 0;

        List<cfields> m_flds;

        bool is_admin()
        {
            bool badmin = false;

            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl0 = new SortedList();

            sl0.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);
            }

            if (a.Count > 0)
            {
                user_data ud = (user_data)a[0];

                if (ud.role.ToUpper() == "ADMIN")
                    badmin = true;
            }

            return badmin;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string t = Request.QueryString["t"];

            m_flds = new List<cfields>();

            bool badmin = is_admin();

            if(t != null)
            {
                m_tbl = t;
                lblSearch.Text = "Search " + m_tbl;

                if (m_tbl == "users")
                {
                    m_order_by = "name";

                    if (!badmin)
                    {
                        Response.Redirect("default.aspx");
                    }

                    m_flds.Add(new cfields("Name", "name", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Login ID", "login_id", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Password", "password", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Role", "role", cfields.STR, "PROJECT MANAGER|WELDER|FITTER|MODULE BUILDER|SITE FITTER|QA|WELD_TESTER|DRIVER|LOADER|ADMIN|SUPERVISOR|USER|CUSTOMER|STOREMAN|LABOURER|FABRICATOR|PLANT"));
                    m_flds.Add(new cfields("Job title", "job_title", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("email", "email", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("IMSL User", "imsl_username", cfields.STR, "Unassigned|gbefab|gbe-services|gbe-ldn"));
                    m_flds.Add(new cfields("Special Permissions", "special_permissions", cfields.INT, string.Empty));
                }
                else if (m_tbl == "parts")
                {
                    m_order_by = "description";

                    m_flds.Add(new cfields("Active", "active", cfields.BOOL, "Yes|No"));
                    m_flds.Add(new cfields("Source", "source", cfields.INT, "External|In house"));
                    m_flds.Add(new cfields("Part no.", "part_number", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Description", "description", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 100;
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Add. description", "additional_description", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Size", "size", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 20;
                    m_flds.Add(new cfields("Type", "part_type", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 50;
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Material cost", "material_cost", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Supplier", "supplier", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Welder rate", "welder_rate", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Fitter rate", "fitter_rate", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Site Fitter rate", "site_fitter_rate", cfields.DEC, string.Empty));
                    
                    m_flds.Add(new cfields("GBE Sale cost", "gbe_sale_cost", cfields.DEC, string.Empty));

                    m_flds.Add(new cfields("Apollo Sale cost", "Apollo", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("C.Watkins Sale cost", "C_Watkins", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("CPS Sale cost", "CPS", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Excel Sale cost", "Excel", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Shawston Sale cost", "Shawston", cfields.DEC, string.Empty));
                    
                    m_flds.Add(new cfields("Pipe Center Sale cost", "pipecenter_sale_cost", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("OLMAT Sale cost", "olmat_group_sale_cost", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Buxton & McNulty Sale cost", "buxton_mcnulty_sale_cost", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Associated Pipework cost", "associated_pipework_fab_only", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Generic Sale cost", "generic_sale_cost", cfields.DEC, string.Empty));

                    m_flds.Add(new cfields("Watkins cost", "watkins", cfields.DEC, string.Empty));

                    m_flds.Add(new cfields("DGR Fab & Mat", "dgr_fab_and_mat", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("DGR Fab only", "dgr_fab_only", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Rates Fab & Mat", "rates_materials_and_fabrication", cfields.DEC, string.Empty));

                    m_flds.Add(new cfields("Size(mm)", "size_mm", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 20;
                   
                    m_flds.Add(new cfields("Manufacturer", "manufacturer", cfields.STR, string.Empty));
                }
                if (m_tbl == "customers")
                {
                    m_order_by = "name";

                    m_flds.Add(new cfields("Name", "name", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("email", "email", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 100;
                    m_flds.Add(new cfields("Address line 1", "address_line1", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 2", "address_line2", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 3", "address_line3", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 4", "address_line4", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Contact name", "contact_name", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Telephone", "telephone", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Contract number", "contract_number", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 10;
                    
                }
                if (m_tbl == "vehicles")
                {
                    m_order_by = "registration";

                    m_flds.Add(new cfields("Registration", "registration", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Vehicle Type", "vehicle_type", cfields.STR, string.Empty));
                }

                if (m_tbl == "consumable_parts")
                {
                    m_order_by = "description";

                    m_flds.Add(new cfields("Description", "description", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Additional description", "additional_description", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Part no.", "part_number", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Unit cost", "unit_cost", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Qty in stock", "qty_in_stock", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Min stock", "min_stock_holding", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Re-order qty", "reorder_qty", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Location", "location_id", cfields.STR, string.Empty, "locations", "location"));
                    m_flds.Add(new cfields("Supplier", "supplier_id", cfields.STR, string.Empty, "suppliers", "name"));
                    m_flds.Add(new cfields("Status", "order_status", cfields.INT, string.Empty));
                }

                if (m_tbl == "consignment")
                {
                    m_order_by = "description";

                    m_flds.Add(new cfields("Description", "description", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Additional description", "additional_description", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Part no.", "part_number", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Size", "size", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].text_len = 15;
                    m_flds.Add(new cfields("Manufacturer", "manufacturer", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Location", "location_id", cfields.STR, string.Empty, "locations", "location"));
                    m_flds.Add(new cfields("Qty in stock", "qty_in_stock", cfields.DEC, string.Empty));
                    m_flds.Add(new cfields("Owner", "owner", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Barcode", "barcode", cfields.STR, string.Empty));
                }

                if (m_tbl == "suppliers")
                {
                    m_order_by = "name";

                    m_flds.Add(new cfields("Name", "name", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("email address", "email_address", cfields.STR, string.Empty));
                }

                if (m_tbl == "locations")
                {
                    m_order_by = "location";

                    m_flds.Add(new cfields("Location", "location", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                }

                if (m_tbl == "delivery_addresses")
                {
                    m_order_by = "contract_number";

                    m_flds.Add(new cfields("Contract number", "contract_number", cfields.STR, string.Empty));
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Address line 1", "address_line1", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 2", "address_line2", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 3", "address_line3", cfields.STR, string.Empty));
                    m_flds.Add(new cfields("Address line 4", "address_line4", cfields.STR, string.Empty));
                }

                if (m_tbl == "settings")
                {
                    m_order_by = "id";

                    m_flds.Add(new cfields("PIN", "pin", cfields.STR, string.Empty));
                    //m_flds[m_flds.Count - 1].bsearch_fld = true;
                }

                if (m_tbl == "customer_fab_mat")
                {
                    m_order_by = "name";

                    cfields cfi = new cfields("Name", "name", cfields.STR, string.Empty);
                    cfi.bread_only = true;
                    m_flds.Add(cfi);
                    m_flds[m_flds.Count - 1].bsearch_fld = true;
                    m_flds.Add(new cfields("Material", "bmat", cfields.BOOL, "Yes|No"));
                    m_flds.Add(new cfields("Fab.", "bfab", cfields.BOOL, "Yes|No"));
                }

                dlActive.Visible = (m_tbl == "parts");
                if (IsPostBack)
                {
                    m_state = (int)ViewState["state"];

                    m_dta = (DataTable)ViewState["dta"];
                    display();

                    if(m_state == ADD_NEW)
                        add_new();
                }
                else
                {
                    m_state = 0;
                    ViewState["state"] = m_state;

                    foreach (cfields cf in m_flds)
                    {
                        if(cf.bsearch_fld)
                            dlSearchFlds.Items.Add(cf._display_label);
                    }

                    if (m_tbl == "parts")
                    {
                        using (parts p = new parts())
                        {
                            ArrayList pt = p.get_distinct_part_types();
                            ViewState["part_types"] = pt;
                        }

                        dlActive.Items.Add("Active");
                        dlActive.Items.Add("Inactive");
                    }

                    txtSearch.Focus();
                }
            }

            if (!is_admin())
            {
                btnSave.Visible = btnDelete.Visible = btnAdd.Visible = false;
            }

        }

        cfields find_cfields_by_display_label(string display_label)
        {
            cfields cf = new cfields();

            foreach (cfields c in m_flds)
                if (c._display_label == display_label)
                    cf = c;

            return cf;
        }

        cfields find_cfields_by_field_name(string field_name)
        {
            cfields cf = new cfields();

            foreach (cfields c in m_flds)
                if (c._db_field_name == field_name)
                    cf = c;

            return cf;
        }

        void display_tbl_hdr()
        {
            TableRow r;
            TableCell c;

            r = new TableRow();

            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            c = new TableCell();
            c.Controls.Add(new LiteralControl(string.Empty));
            r.Cells.Add(c);

            foreach (cfields cf0 in m_flds)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(cf0._display_label));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

        }

        void display()
        {
            if (m_dta != null)
            {
                tblResults.Rows.Clear();

                TableRow r;
                TableCell c;

                if (m_dta.Rows.Count > 0)
                {
                    display_tbl_hdr();

                    foreach (DataRow dr in m_dta.Rows)
                    {
                        if (m_tbl == "users")
                        {
                            string role = dr["Role"].ToString().ToUpper();

                             if(role == "MASTER")
                                 continue;
                        }

                        System.Drawing.Color bc;

                        if (tblResults.Rows.Count % 2 == 0)
                            bc = System.Drawing.Color.FromName("LightGray");
                        else
                            bc = System.Drawing.Color.FromName("White");

                        r = new TableRow();
                        r.BackColor = bc;

                        c = new TableCell();
                        CheckBox chk = new CheckBox();
                        chk.ID = "chk" + dr["id"].ToString();
                        c.Controls.Add(chk);

                        r.Cells.Add(c);
                        
                        foreach (cfields cf1 in m_flds)
                        {
                            c = new TableCell();

                            if (cf1.sl_from_tbl_fld.Count > 0)
                            {
                                DropDownList dl = new DropDownList();

                                dl.Items.Add(string.Empty);

                                foreach (DictionaryEntry e in cf1.sl_from_tbl_fld)
                                    dl.Items.Add(e.Key.ToString());

                                if(dr[cf1._db_field_name].GetType() == typeof(int))
                                {
                                    try
                                    {
                                        dl.Text = cf1.sl_from_tbl_id[(int)dr[cf1._db_field_name]].ToString();
                                    }
                                    catch { }
                                }

                                dl.Attributes["db_field_name"] = cf1._db_field_name;
                                dl.ID = cf1._db_field_name + dr["id"].ToString();
                                c.Controls.Add(dl);
                            }
                            else if (cf1._values.Count == 0)
                            {
                                TextBox tb = new TextBox();

                                if (m_tbl.ToLower() == "parts" && cf1._db_field_name.ToLower() == "description")
                                {
                                    tb.Width = new Unit(700, UnitType.Pixel);
                                }

                                tb.Text = dr[cf1._db_field_name].ToString();
                                tb.Attributes["db_field_name"] = cf1._db_field_name;
                                tb.ID = cf1._db_field_name + dr["id"].ToString();
                                tb.MaxLength = cf1.text_len;
                                tb.ReadOnly = cf1.bread_only;

                                if(cf1._db_field_type == cfields.DEC)
                                    tb.Attributes.Add("onkeypress", "return isDecimal(event)");
                                c.Controls.Add(tb);
                            }
                            else
                            {
                                DropDownList dl = new DropDownList();

                                foreach (string s in cf1._values)
                                    dl.Items.Add(s);

                                string dltext = dr[cf1._db_field_name].ToString();

                                if (cf1._db_field_type == cfields.BOOL)
                                {
                                    if (dltext.ToLower() == "true")
                                        dl.SelectedIndex = 0;
                                    else
                                        dl.SelectedIndex = 1;
                                }
                                else if (cf1._db_field_type == cfields.INT)
                                {
                                    try { dl.SelectedIndex = Convert.ToInt32(dltext); }
                                    catch { }
                                }
                                else
                                    dl.Text = dltext;

                                dl.Attributes["db_field_name"] = cf1._db_field_name;
                                dl.ID = cf1._db_field_name+dr["id"].ToString();
                                c.Controls.Add(dl);
                            }

                            r.Cells.Add(c);
                        }

                        r.Attributes["uid"] = dr["id"].ToString();

                        tblResults.Rows.Add(r);
                    }
                }
            }
        }

        void init_search()
        {
            SortedList sl = new SortedList();

            cfields cf = find_cfields_by_display_label(dlSearchFlds.Text);

            if (m_tbl == "parts")
            {
                string srch = string.Empty;

                if (txtSearch.Text.Trim().Length > 0)
                    srch = txtSearch.Text.Trim();
                else if (cf._db_field_name == "part_type")
                    srch = dlPartType.Text;

                sl.Add(cf._db_field_name, srch);

                bool bactive = true;
                if (dlActive.Text == "Inactive")
                    bactive = false;

                sl.Add("active", bactive);
            }
            else
            {
                if (txtSearch.Text.Trim().Length > 0)
                {
                    sl.Add(cf._db_field_name, txtSearch.Text.Trim());
                }
            }

            ViewState["search_params"] = sl;

            upd_record_count();
        }

        void upd_record_count()
        {
            if (ViewState["search_params"] != null)
            {
                SortedList sl = (SortedList)ViewState["search_params"];

                int record_count = 0;

                using (cdb_connection dbc = new cdb_connection())
                {
                    record_count = dbc.get_record_count(m_tbl, sl);
                }

                ViewState["record_count"] = record_count;
            }
        }

        void search(int pg)
        {
            SortedList sl = (SortedList)ViewState["search_params"];

            using (cdb_connection dbc = new cdb_connection())
            {
                dbc.pg = pg;
                dbc.recs_per_pg = REC_PER_PG;
                dbc.order_by = m_order_by;

                m_dta = dbc.get_data(m_tbl, sl);
            }

            ViewState["dta"] = m_dta;

            if (get_last_page() == 0)
                pg = 0;
            
            ViewState["current_page"] = pg;

            lblPage.Text = get_current_page().ToString() + " / " + get_last_page().ToString();
        }

        void btn_delete_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (m_tbl.Length > 0)
            {
                init_search();
                search(1);
                display();
            }
        }

        bool part_used(int part_id)
        {
            SortedList sl = new SortedList();

            sl.Add("part_id", part_id);

            ArrayList a = new ArrayList();

            using (spool_parts sp = new spool_parts())
            {
                sp.select_top = 1;
                a = sp.get_spool_parts_data(sl);
            }

            return a.Count > 0;
        }

        bool user_activity(int user_id)
        {
            bool bret = false;

            SortedList sl = new SortedList();

            sl.Add("user_id", user_id);

            ArrayList a = new ArrayList();

            using (weld_jobs wj = new weld_jobs())
            {
                a = wj.get_weld_job_data(sl);
            }

            if (a.Count == 0)
            {
                using (deliveries d = new deliveries())
                {
                    a = d.get_delivery_data(sl);
                }
            }

            if (a.Count == 0)
            {
                using (qa_jobs qa = new qa_jobs())
                {
                    a = qa.get_qa_job_data(sl);
                }
            }

            if (a.Count == 0)
            {
                using (weld_tests wt = new weld_tests())
                {
                    a = wt.get_weld_test_data(sl);
                }
            }

            bret = a.Count > 0;

            return bret;
        }

        protected bool ok_to_delete(string uid)
        {
            bool bret = true;

            if (m_tbl == "users")
                bret = !user_activity(Convert.ToInt32(uid));
            else if (m_tbl == "parts")
                bret = !part_used(Convert.ToInt32(uid));

            return bret;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;

            if (get_last_page() == 0)
                return;

            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                ArrayList del_i = new ArrayList();
                ArrayList del_uid = new ArrayList();
                int i = 0;

                int not_ok_to_delete = 0;

                foreach (TableRow r in tblResults.Rows)
                {
                    string uid = (r.Attributes["uid"]);

                    if (uid != null)
                    {
                        CheckBox chk = (CheckBox)(r.Cells[0].Controls[0]);

                        if (chk.Checked)
                        {
                            if (ok_to_delete(uid))
                            {
                                del_i.Add(i);
                                del_uid.Add(Convert.ToInt32(uid));
                            }
                            else
                                not_ok_to_delete++;
                        }

                        i++;
                    }
                }

                using (cdb_connection cdb = new cdb_connection())
                {
                    SortedList sl = new SortedList();

                    foreach (int id in del_uid)
                    {
                        sl.Clear();
                        sl.Add("id", id);
                        cdb.delete_record(m_tbl, sl);
                    }
                }

                for (int n = del_i.Count - 1; n >= 0; n--)
                {
                    int ii = (int)del_i[n];
                    m_dta.Rows.RemoveAt(ii);
                }

                if (not_ok_to_delete == 1)
                    lblMsg.Text = "Unable to delete a record as it forms part of the system's historical record.";
                else if (not_ok_to_delete > 1)
                    lblMsg.Text = "Unable to delete some records as they form part of the system's historical record.";

                upd_record_count();

                if (get_current_page() > get_last_page())
                    ViewState["current_page"] = get_last_page();

                search(get_current_page());
                display();
                
            }
        }

        void display_blank_record()
        {
            TableRow r;
            TableCell c;

            r = new TableRow();

            c = new TableCell();
            c.Controls.Add(new LiteralControl(string.Empty));
            //c.Width = 10;
            r.Cells.Add(c);

            foreach (cfields cf1 in m_flds)
            {
                c = new TableCell();
                if (cf1._values.Count == 0)
                {
                    TextBox tb = new TextBox();
                    tb.Text = string.Empty;
                    tb.Attributes["db_field_name"] = cf1._db_field_name;
                    c.Controls.Add(tb);
                    tb.ID = cf1._db_field_name;
                }
                else
                {
                    DropDownList dl = new DropDownList();

                    foreach (string s in cf1._values)
                         dl.Items.Add(s);
 
                    dl.Attributes["db_field_name"] = cf1._db_field_name;
                    dl.ID = cf1._db_field_name;
                    c.Controls.Add(dl);
                }

                r.Cells.Add(c);
            }

            r.Attributes["uid"] = "0";

            tblResults.Rows.Add(r);
        }

        void add_new()
        {
            tblResults.Rows.Clear();

            display_tbl_hdr();
            display_blank_record();

            m_state = ADD_NEW;
            ViewState["state"] = m_state;

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            add_new();
        }

        void save()
        {
            bool badd_new = false;

            using (cdb_connection cdb = new cdb_connection())
            {
                int saved_id = 0;
                SortedList sl = new SortedList();

                foreach (TableRow r in tblResults.Rows)
                {
                    string uid = (r.Attributes["uid"]);

                    badd_new = (uid == "0");

                    if (uid != null)
                    {
                        sl.Clear();

                        foreach (TableCell cell in r.Cells)
                        {
                            foreach (Control c in cell.Controls)
                            {
                                string db_field_name = null;
                                string db_field_value = string.Empty;

                                if (c.GetType() == typeof(TextBox))
                                {
                                    db_field_name = ((TextBox)c).Attributes["db_field_name"];
                                    db_field_value = ((TextBox)c).Text.Trim();
                                }
                                else if (c.GetType() == typeof(DropDownList))
                                {
                                    db_field_name = ((DropDownList)c).Attributes["db_field_name"];
                                    db_field_value = ((DropDownList)c).Text.Trim();
                                }

                                if (db_field_name != null)
                                {
                                    cfields cf = find_cfields_by_field_name(db_field_name);

                                    if (cf._db_field_type == cfields.BOOL)
                                    {
                                        bool b = false;

                                        if (db_field_value.ToLower() == "yes" || db_field_value.ToLower() == "true")
                                            b = true;

                                        sl.Add(db_field_name, b);
                                    }
                                    else if (cf._db_field_type == cfields.STR)
                                    {
                                        if (cf.sl_from_tbl_fld.Count > 0) // foreign key
                                        {
                                            int db_val = 0;

                                            if (db_field_value.Length > 0)
                                            {
                                                db_val = Convert.ToInt32(cf.sl_from_tbl_fld[db_field_value]);
                                            }

                                            sl.Add(db_field_name, db_val);
                                        }
                                        else
                                            sl.Add(db_field_name, db_field_value);
                                    }
                                    else
                                    {
                                        if (c.GetType() == typeof(DropDownList))
                                        {
                                            // use selected index
                                            sl.Add(db_field_name, ((DropDownList)c).SelectedIndex);
                                        }
                                        else
                                        {
                                            decimal dec = 0.00M;

                                            try { dec = Convert.ToDecimal(db_field_value); }
                                            catch { }

                                            sl.Add(db_field_name, dec);
                                        }
                                    }
                                }
                            }
                        }

                        if (sl.Count > 0)
                        {
                            if (!badd_new)
                                sl.Add("id", Convert.ToInt32(uid));

                            saved_id = cdb.save(sl, m_tbl);

                            if (badd_new)
                            {
                                break;
                            }
                        }
                    }
                }

                if (!badd_new)
                    search(get_current_page());
                else
                {
                    sl.Clear();
                    sl.Add("id", saved_id);

                    ViewState["record_count"] = 1;
                    ViewState["search_params"] = sl;

                    search(1);

                    m_state = 0;
                    ViewState["state"] = m_state;

                    display();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            save();
        }

        protected void dlSearchFlds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_tbl == "parts" && dlSearchFlds.Text == "Type")
            {
                dlPartType.Visible = true;

                if (dlPartType.Items.Count == 0)
                {
                    ArrayList pt = (ArrayList)ViewState["part_types"];

                    if (pt != null)
                    {
                        foreach (string s in pt)
                        {
                            dlPartType.Items.Add(s);
                        }
                    }
                }
            }
            else
                dlPartType.Visible = false;
        }

        protected void btnFirstPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                search(1);
                display();
            }
        }

        protected void btnPreviousPage_Click(object sender, ImageClickEventArgs e)
        {
            int pg = get_current_page();

            if (pg > 0)
            {
                if (pg > 1)
                {
                    search(pg - 1);
                    display();
                }
            }
        }

        protected void btnNextPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                int pg = get_current_page();

                if (pg > 0)
                {
                    if (pg < get_last_page())
                    {
                        search(pg + 1);
                        display();
                    }
                }
            }
        }

        protected void btnLastPage_Click(object sender, ImageClickEventArgs e)
        {
            if (get_last_page() > 0)
            {
                search(get_last_page());
                display();
            }
        }

        int get_current_page()
        {
            int current_page = 0;

            if (ViewState["current_page"] != null)
                current_page = (int)ViewState["current_page"];

            return current_page;
        }

        int get_last_page()
        {
            int last_page = 0;

            if (ViewState["record_count"] != null)
            {
                int record_count = (int)ViewState["record_count"];

                last_page = record_count / REC_PER_PG;

                if (record_count % REC_PER_PG > 0)
                    last_page++;
            }
            return last_page;
        }
    }

    class cfields
    {
        public static int INT = 0;
        public static int STR = 1;
        public static int DEC = 2;
        public static int BOOL = 3;

        public string _display_label = string.Empty;
        public string _db_field_name = string.Empty;
        public int _db_field_type;
        public ArrayList _values = new ArrayList();
        public string _from_table = string.Empty;
        public SortedList sl_from_tbl_id = new SortedList();
        public SortedList sl_from_tbl_fld = new SortedList();

        public bool bsearch_fld = false;
        public int text_len = 50;
        public bool bread_only = false;

        public cfields() { }

        public cfields(string display_label, string db_field_name, int db_field_type, string values, string from_table, string from_fld)
        {
            _display_label = display_label;
            _db_field_name = db_field_name;
            _db_field_type = db_field_type;

            try
            {
                cdb_connection db = new cdb_connection();

                DataTable dta = db.get_data(from_table, new SortedList());

                foreach (DataRow dr in dta.Rows)
                {
                    try {
                        sl_from_tbl_id.Add(dr["id"], dr[from_fld].ToString());
                        sl_from_tbl_fld.Add(dr[from_fld].ToString(), dr["id"]);
                    }
                    catch { }
                }
            }
            catch { }

            _from_table = from_table;
        }

        public cfields(string display_label, string db_field_name, int db_field_type, string values)
        {
            _display_label = display_label;
            _db_field_name = db_field_name;
            _db_field_type = db_field_type;

            if (values.Trim().Length > 0)
            {
                string[] v = values.Split('|');

                foreach (string s in v)
                    _values.Add(s);
            }
        }
    }
}
