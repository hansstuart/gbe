using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class spools : cdb_connection
    {
        string m_tbl = "spools";
        bool m_ex_call = false;

        public spools() { }

        public spools(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public ArrayList get_spool_data_ex(SortedList search_params)
        {
            m_ex_call = true;

            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            try
            {
                a = get_spool_data(search_params);

                using (weld_jobs wj = new weld_jobs(m_sql_connection))
                {
                    using (qa_jobs qaj = new qa_jobs(m_sql_connection))
                    {
                        foreach (spool_data sd in a)
                        {
                            sl.Clear();

                            sl.Add("spool_id", sd.id);
                            sl.Add("assembly_type", weld_job_data.SPOOL);

                            a2 = wj.get_weld_job_data(sl);

                            if (a2.Count > 0)
                                sd.weld_job_data = (weld_job_data)a2[0];

                            sd.qa_data = qaj.get_qa_job_data(sl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_data_ex() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_spool_data_short(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            DataTable dta = get_data(m_tbl, search_params);
            foreach (DataRow dr in dta.Rows)
            {
                spool_data sd = new spool_data();

                sd.id = (int)dr["id"];
                sd.spool = dr["spool"].ToString();
                sd.revision = dr["revision"].ToString();
                sd.barcode = dr["barcode"].ToString();

                try { sd.welder = (int)dr["welder"]; }
                catch { }

                try { sd.fitter = (int)dr["fitter"]; }
                catch { }

                try { sd.delivery_address = (int)dr["delivery_address"]; }
                catch { }

                try { sd.cad_user_id = (int)dr["cad_user_id"]; }
                catch { }

                if (dr["porder_created"].GetType() == typeof(bool))
                    sd.porder_created = (bool)dr["porder_created"];

                if (dr["on_hold"].GetType() == typeof(bool))
                    sd.on_hold = (bool)dr["on_hold"];

                try { sd.status = dr["status"].ToString(); }
                catch { }

                try { sd.cost_centre = (int)dr["cost_centre"]; }
                catch { }

                try { sd.imsl_cost_centre = (int)dr["imsl_cost_centre"]; }
                catch { }

                try { sd.picked = (bool)dr["picked"]; }
                catch { }

                try { sd.include_in_weld_map = (bool)dr["include_in_weld_map"]; }
                catch { }

                try { sd.site_fitter = (int)dr["site_fitter"]; }
                catch { }

                try { sd.date_created = (DateTime)dr["date_created"]; }
                catch { }

                try { sd.delivery_date = (DateTime)dr["delivery_date"]; }
                catch { }

                try { sd.material = dr["material"].ToString(); }
                catch { }

                try { sd.pipe_size = dr["pipe_size"].ToString(); }
                catch { }

                try { sd.cut_size1 = dr["cut_size1"].ToString(); }
                catch { }

                try { sd.cut_size2 = dr["cut_size2"].ToString(); }
                catch { }

                try { sd.cut_size3 = dr["cut_size3"].ToString(); }
                catch { }

                try { sd.cut_size4 = dr["cut_size4"].ToString(); }
                catch { }

                try { sd.drawing_id = (int)dr["drawing_id"]; }
                catch { }

                try { sd.checked_by = dr["checked_by"].ToString(); }
                catch { }

                try { sd.fab_order_id = (int)dr["fab_order_id"]; }
                catch { }

                a.Add(sd);
            }

            return a;
        }

        public ArrayList get_spool_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            using (users u = new users(m_sql_connection))
            {
                try
                {
                    a = get_spool_data_short(search_params);
                    DataTable dta = get_data(m_tbl, search_params);

                    using (spool_parts sp = new spool_parts(m_sql_connection))
                    {
                        using (schedule_fab sfab = new schedule_fab())
                        {
                            foreach (spool_data sd in a)
                            {
                                sl.Clear();
                                sl.Add("id", sd.welder);

                                a2 = u.get_user_data(sl);

                                if (a2.Count > 0)
                                    sd.welder_data = (user_data)a2[0];

                                sl.Clear();
                                sl.Add("id", sd.fitter);


                                a2 = u.get_user_data(sl);


                                if (a2.Count > 0)
                                    sd.fitter_data = (user_data)a2[0];

                                sl.Clear();
                                sl.Add("id", sd.site_fitter);

                                a2 = u.get_user_data(sl);

                                if (a2.Count > 0)
                                    sd.site_fitter_data = (user_data)a2[0];



                                sl.Clear();

                                sl.Add("spool_id", sd.id);

                                sd.spool_part_data = sp.get_spool_parts_data_ex(sl);

                                decimal rate = 0;
                                if (sd.spool_part_data != null)
                                {
                                    foreach (spool_part_data spd in sd.spool_part_data)
                                    {
                                        rate += spd.part_data.welder_rate;
                                    }
                                }

                                if (rate == 0)
                                    sd.weld_required = false;

                                rate = 0;

                                if (sd.spool_part_data != null)
                                {
                                    foreach (spool_part_data spd in sd.spool_part_data)
                                    {
                                        rate += spd.part_data.fitter_rate;
                                    }
                                }

                                if (rate == 0)
                                    sd.fit_required = false;


                                if (!m_ex_call)
                                {
                                    sd.spool_part_data.Clear();
                                    sd.spool_part_data = null;
                                }

                                sl.Clear();
                                sl.Add("spool_id", sd.id);

                                a2 = sfab.get_schedule_fab_data(sl);

                                if(a2.Count > 0)
                                    sd.schedule_fab_data = (schedule_fab_data)a2[a2.Count - 1];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("PCF gbe", "get_spool_data() \n" + ex.ToString(), EventLogEntryType.Error);
                }

            }

            return a;
        }

        public bool save_spool_details(ArrayList asd, string user_id)
        {
            bool bret = true;
            SortedList sl = new SortedList();

            foreach (key_value_container kvc in asd)
            {
                string audit_trail = string.Empty;

                sl.Clear();

                foreach (key_value kv in kvc.container)
                {
                    if (kv != null)
                    {
                        if (kv.key == "audit_trail")
                            audit_trail = kv.value.ToString();
                        else if (!sl.Contains(kv.key))
                            sl.Add(kv.key, kv.value);
                    }
                }

                bret &= save_spool_details(sl, audit_trail, user_id);
            }

            return bret;
        }

        public bool save_spool_details(SortedList sl, string audit_trail, string user_id)
        {
            bool bret = true;

            try
            {
                const string ID = "id";

                int spool_id = 0;

                if (sl.ContainsKey(ID))
                    spool_id = (int)sl[ID];

                if (spool_id == 0)
                    sl.Add("date_created", DateTime.Now);
                
                spool_id = save(sl, m_tbl);

                if (!sl.ContainsKey(ID))
                    sl.Add(ID, spool_id);

                if (spool_id > 0)
                {
                    using (spool_audit_trail sat = new spool_audit_trail())
                    {
                        SortedList sl_sat = new SortedList();
                        sl_sat.Add("spool_id", spool_id);
                        sl_sat.Add("status", audit_trail);
                        sl_sat.Add("user_id", user_id);

                        bret = sat.save_spool_audit_trail_details(sl_sat);
                    }

                    ////////
                    if (sl.ContainsKey("fitter") || sl.ContainsKey("welder"))
                    {
                        SortedList sl_fa = new SortedList();

                        sl_fa.Add("spool_id", spool_id);

                        using (weld_jobs wj = new weld_jobs(m_sql_connection))
                        {
                            ArrayList a_wj = wj.get_weld_job_data(sl_fa);

                            int wj_id = 0;

                            if (a_wj.Count > 0)
                            {
                                weld_job_data wjd = (weld_job_data)a_wj[0];
                                wj_id = wjd.id;
                            }

                            sl_fa.Clear();

                            DateTime dt_assigned = DateTime.Now;

                            sl_fa.Add("assigned_on", dt_assigned);

                            a_wj = wj.get_weld_job_data(sl_fa);

                            int seq = a_wj.Count + 1;

                            sl_fa.Clear();

                            sl_fa.Add("id", wj_id);

                            if (sl.ContainsKey("fitter"))
                            {
                                int fitter_id = (int)sl["fitter"];
                                sl_fa.Add("fitter_id", fitter_id);
                            }

                            if (sl.ContainsKey("welder"))
                            {
                                int welder_id = (int)sl["welder"];
                                sl_fa.Add("user_id", welder_id);
                            }
                            
                            sl_fa.Add("assigned_seq", seq);
                            sl_fa.Add("assigned_on", dt_assigned);
                            sl_fa.Add("spool_id", spool_id);

                            wj.save_weld_job_data(sl_fa);
                        }
                    }
                    ///////
                }
                else
                {
                    bret = false;
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_spool_details()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public bool save_drawing(SortedList sl)
        {
            bool bret = true;

            try
            {
                const string ID = "id";

                int id = save(sl, "drawings");

                if (!sl.ContainsKey(ID))
                    sl.Add(ID, id);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_drawing()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public drawing_data get_drawing_data(int drawing_id)
        {
            SortedList sl = new SortedList();
            sl.Add("id", drawing_id);

            DataTable dta = get_data("drawings", sl);

            drawing_data dd = new drawing_data();

            if(dta.Rows.Count > 0)
            {
                DataRow dr = dta.Rows[0];

                dd.id = (int)dr["id"];
                dd.spool_id = (int)dr["spool_id"];
                dd.pdf = (byte[])dr["pdf"];
            }

            return dd;
        }

        public ArrayList get_spools_for_sceduled_load(string contract, DateTime dt_delivery_date)
        {
            ArrayList a = new ArrayList();

            SortedList sl = new SortedList();
            sl.Add("barcode", contract + "%");
            sl.Add("delivery_date", dt_delivery_date.ToString("MMM").ToLower() + '\x20' + dt_delivery_date.ToString("dd") + '\x20' + dt_delivery_date.ToString("yyyy") + "%");


            return a;
        }
    }

    [Serializable]
    public class spool_data
    {
        public int id = 0;
        public string spool = string.Empty;
        public string revision = string.Empty;
        public string barcode = string.Empty;
        public int welder = 0;
        public int fitter = 0;
        public int delivery_address = 0;
        public bool porder_created = false;
        public int cad_user_id = 0;
        public string status = string.Empty;
        public bool on_hold = false;
        public int cost_centre = 0;
        public int imsl_cost_centre = 0;
        public string tag = string.Empty;
        public int site_fitter = 0;
        public string material = string.Empty;

        public int fab_order_id = 0;

        public string pipe_size = string.Empty;
        public string cut_size1 = string.Empty;
        public string cut_size2 = string.Empty;
        public string cut_size3 = string.Empty;
        public string cut_size4 = string.Empty;

        public int drawing_id = 0;

        public string checked_by = string.Empty;

        public user_data welder_data = null;
        public user_data fitter_data = null;
        public user_data site_fitter_data = null;
        public ArrayList spool_part_data = null;
        public weld_job_data weld_job_data = null;
        public ArrayList qa_data = null;
        public bool picked = false;

        public bool include_in_weld_map = true;

        public bool weld_required = true;
        public bool fit_required = true;

        public DateTime date_created = DateTime.MinValue;
        public DateTime delivery_date = DateTime.MinValue;

        public schedule_fab_data schedule_fab_data = null;
    }

    [Serializable]
    public class drawing_data
    {
        public int id = 0;
        public int spool_id = 0;
        public byte[] pdf = null;
    }
}
