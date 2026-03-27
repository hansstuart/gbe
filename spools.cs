using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;

namespace gbe
{
    public class spools : cdb_connection
    {
        const int RECS_PER_PG = 10;
        public static int REC_PER_PG = RECS_PER_PG;

        string m_tbl = "spools";
        bool m_ex_call = false;

        public spools() { }

        public spools(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public ArrayList get_spool_data_ex(SortedList search_params, int recs_per_page=RECS_PER_PG)
        {
            return get_spool_data_new(search_params, recs_per_page);

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

        void populate_spool_data(spool_data sd, DataRow dr, string prefix="")
        {
            try {sd.id = (int)dr[prefix+"id"];}
            catch { }

            if (sd.id == 0)
            {
                try {sd.id = (int)dr[prefix+"spools_id"];}
                catch { }
            }

            sd.spool = dr[prefix+"spool"].ToString();
            sd.revision = dr[prefix+"revision"].ToString();
            sd.barcode = dr[prefix+"barcode"].ToString();

            try { sd.welder = (int)dr[prefix+"welder"]; }
            catch { }

            try { sd.fitter = (int)dr[prefix+"fitter"]; }
            catch { }

            try { sd.delivery_address = (int)dr[prefix+"delivery_address"]; }
            catch { }

            try { sd.cad_user_id = (int)dr[prefix+"cad_user_id"]; }
            catch { }

            if (dr[prefix+"porder_created"].GetType() == typeof(bool))
                sd.porder_created = (bool)dr[prefix+"porder_created"];

            if (dr[prefix+"on_hold"].GetType() == typeof(bool))
                sd.on_hold = (bool)dr[prefix+"on_hold"];

            try { sd.status = dr[prefix+"status"].ToString(); }
            catch { }

            try { sd.cost_centre = (int)dr[prefix+"cost_centre"]; }
            catch { }

            try { sd.imsl_cost_centre = (int)dr[prefix+"imsl_cost_centre"]; }
            catch { }

            try { sd.picked = (bool)dr[prefix+"picked"]; }
            catch { }

            try { sd.include_in_weld_map = (bool)dr[prefix+"include_in_weld_map"]; }
            catch { }

            try { sd.site_fitter = (int)dr[prefix+"site_fitter"]; }
            catch { }

            try { sd.date_created = (DateTime)dr[prefix+"date_created"]; }
            catch { }

            try { sd.delivery_date = (DateTime)dr[prefix+"delivery_date"]; }
            catch { }

            try { sd.material = dr[prefix+"material"].ToString(); }
            catch { }

            try { sd.pipe_size = dr[prefix+"pipe_size"].ToString(); }
            catch { }

            try { sd.cut_size1 = dr[prefix+"cut_size1"].ToString(); }
            catch { }

            try { sd.cut_size2 = dr[prefix+"cut_size2"].ToString(); }
            catch { }

            try { sd.cut_size3 = dr[prefix+"cut_size3"].ToString(); }
            catch { }

            try { sd.cut_size4 = dr[prefix+"cut_size4"].ToString(); }
            catch { }

            try { sd.drawing_id = (int)dr[prefix+"drawing_id"]; }
            catch { }

            try { sd.weld_test_report_count = (int)dr[prefix+"weld_test_report_count"]; }
            catch { }

            try { sd.checked_by = dr[prefix+"checked_by"].ToString(); }
            catch { }

            try { sd.fab_order_id = (int)dr[prefix+"fab_order_id"]; }
            catch { }
        }

        public ArrayList get_spool_data_short(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            DataTable dta = get_data(m_tbl, search_params);
            foreach (DataRow dr in dta.Rows)
            {
                spool_data sd = new spool_data();

                populate_spool_data(sd, dr);

                a.Add(sd);
            }

            return a;
        }

        public ArrayList get_spool_data_new(SortedList search_params, int recs_per_page=RECS_PER_PG)
        {
            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            if (pg == 0)
            {
                pg = 1;
                recs_per_pg = recs_per_page;
            }

            order_by = "barcode";

            string select = string.Empty;

            select = $@"select  * from (select ROW_NUMBER() over (order by {order_by} ) as row 
                                ,spools.id as spools_id
                                from 
                                spools";

            if (search_params != null)
            {
                string where = string.Empty;

                foreach (DictionaryEntry e0 in search_params)
                {
                    if(where.Trim().Length > 0)
                        where += " and ";
                    
                    string op = "=";
                    
                    if(e0.Value.ToString().Contains("%"))
                        op = "like";
                    
                    where += $" {e0.Key} {op} @{e0.Key} ";
                }

                if(where.Trim().Length > 0)
                    select += $" where {where} "; 
            }

            select += gen_page();

            DataTable dtab = get_data_p(select, search_params);

            logit("dtab.Rows.Count == " + dtab.Rows.Count.ToString());

            if(dtab.Rows.Count == 0)
                return a;

            string where_id = string.Empty;

            foreach (DataRow dr in dtab.Rows)
            {
                if(where_id.Trim().Length > 0)
                    where_id += " or ";

                int spool_id = (int)dr["spools_id"];

                where_id += " spools.id=" + spool_id.ToString();
            }


            order_by = "barcode, spool_parts_seq, spool_parts_id";

            select = $@"select
                                  spools.id as spools_id
								  ,spools.spool as spools_spool
								  ,spools.revision as spools_revision
								  ,spools.barcode as spools_barcode
								  ,spools.welder as spools_welder
								  ,spools.delivery_address as spools_delivery_address
								  ,spools.cad_user_id as spools_cad_user_id
								  ,spools.porder_created as spools_porder_created
								  ,spools.status as spools_status
								  ,spools.on_hold as spools_on_hold
								  ,spools.fitter as spools_fitter
								  ,spools.cost_centre as spools_cost_centre
								  ,spools.picked as spools_picked
								  ,spools.site_fitter as spools_site_fitter
								  ,spools.include_in_weld_map as spools_include_in_weld_map
								  ,spools.date_created as spools_date_created
								  ,spools.delivery_date as spools_delivery_date
								  ,spools.imsl_cost_centre as spools_imsl_cost_centre
								  ,spools.material as spools_material
								  ,spools.pipe_size as spools_pipe_size
								  ,spools.cut_size1 as spools_cut_size1
								  ,spools.cut_size2 as spools_cut_size2
								  ,spools.cut_size3 as spools_cut_size3
								  ,spools.cut_size4 as spools_cut_size4
								  ,spools.drawing_id as spools_drawing_id
								  ,spools.checked_by as spools_checked_by
								  ,spools.fab_order_id as spools_fab_order_id
								  
                                  ,(select COUNT(weld_test_reports.id) from weld_test_reports where weld_test_reports.spool_id = spools.id) spools_weld_test_report_count
                                
								, parts.*
                                
								, spool_parts.id as spool_parts_id
                                , spool_parts.bw as spool_parts_bw
								, spool_parts.completed as  spool_parts_completed
								, spool_parts.fw as spool_parts_fw
								, spool_parts.include_in_weld_map  as spool_parts_include_in_weld_map 
								, spool_parts.part_id as spool_parts_part_id
								, spool_parts.picked as spool_parts_picked
								, spool_parts.porder as spool_parts_porder 
								, spool_parts.qty as spool_parts_qty 
								, spool_parts.seq as spool_parts_seq
								, spool_parts.spool_id as spool_parts_spool_id
								, spool_parts.welder as spool_parts_welder 
                                    
                                , spool_pipe_fittings_id
								, spool_pipe_fittings.additional_cut1  as spool_pipe_fittings_additional_cut1
								, spool_pipe_fittings.additional_cut2 as spool_pipe_fittings_additional_cut2
								, spool_pipe_fittings.additional_cut3 as spool_pipe_fittings_additional_cut3
								, spool_pipe_fittings.additional_info as spool_pipe_fittings_additional_info
								, spool_pipe_fittings.completed as spool_pipe_fittings_completed
								, spool_pipe_fittings.fitting_1_part_id as spool_pipe_fittings_fitting_1_part_id
								, spool_pipe_fittings.fitting_1_seq_no as spool_pipe_fittings_fitting_1_seq_no
								, spool_pipe_fittings.fitting_2_part_id as spool_pipe_fittings_fitting_2_part_id
								, spool_pipe_fittings.fitting_2_seq_no as spool_pipe_fittings_fitting_2_seq_no
								, spool_pipe_fittings.spool_id as spool_pipe_fittings_spool_id
								, spool_pipe_fittings.spool_part_id as spool_pipe_fittings_spool_part_id
								, spool_pipe_fittings.spool_pipe_fittings_id as spool_pipe_fittings_spool_pipe_fittings_id

                                , fitter_data.id as fitter_data_id
                                , fitter_data.name as fitter_data_name
                                , fitter_data.login_id as fitter_data_login_id
                                , fitter_data.password as fitter_data_password
                                , fitter_data.role as fitter_data_role
                                , fitter_data.job_title as fitter_data_job_title
                                , fitter_data.email as fitter_data_email
                                , fitter_data.imsl_username as fitter_data_imsl_username
                                , fitter_data.special_permissions as fitter_data_special_permissions

                                , welder_data.id as welder_data_id
                                , welder_data.name as welder_data_name
                                , welder_data.login_id as welder_data_login_id
                                , welder_data.password as welder_data_password
                                , welder_data.role as welder_data_role
                                , welder_data.job_title as welder_data_job_title
                                , welder_data.email as welder_data_email
                                , welder_data.imsl_username as welder_data_imsl_username
                                , welder_data.special_permissions as welder_data_special_permissions
                                
								, weld_jobs.id as weld_jobs_id 
								, weld_jobs.assembly_type as  weld_jobs_assembly_type 
								, weld_jobs.assigned_on as weld_jobs_assigned_on 
								, weld_jobs.assigned_seq as  weld_jobs_assigned_seq
								, weld_jobs.finish as  weld_jobs_finish 
								, weld_jobs.finish_dt as  weld_jobs_finish_dt 
								, weld_jobs.finish_fit as weld_jobs_finish_fit 
								, weld_jobs.fitter_id as weld_jobs_fitter_id 
								, weld_jobs.installed_on as  weld_jobs_installed_on 
								, weld_jobs.robot as  weld_jobs_robot
								, weld_jobs.site_fitter_id as  weld_jobs_site_fitter_id
								, weld_jobs.spool_id as  weld_jobs_spool_id 
								, weld_jobs.start as  weld_jobs_start 
								, weld_jobs.start_fit as  weld_jobs_start_fit
								, weld_jobs.user_id as  weld_jobs_user_id 
								 
								
								, qa_jobs.id as qa_jobs_id
								, qa_jobs.assembly_type as qa_jobs_assembly_type
								, qa_jobs.datetime_stamp as qa_jobs_datetime_stamp
								, qa_jobs.result as qa_jobs_result
								, qa_jobs.spool_id as qa_jobs_spool_id
								, qa_jobs.user_id as qa_jobs_user_id
                                    
                                
                                , parts.id as parts_id

                                , f1.description as f1_part_description
                                , f1.fitting_size_mm as f1_fitting_size_mm
                                , f1.gap_mm  as f1_gap_mm
                                , f2.description as f2_part_description
                                , f2.fitting_size_mm as f2_fitting_size_mm
                                , f2.gap_mm as f2_gap_mm
                            
                            from 
                                spools

                            left join users fitter_data on fitter_data.id = spools.fitter 
                            left join users welder_data on welder_data.id = spools.welder 
                            left join spool_parts on spool_parts.spool_id = spools.id
                            left join parts on parts.id = spool_parts.part_id
                            left join qa_jobs on qa_jobs.spool_id = spools.id
                            left join weld_jobs on weld_jobs.spool_id = spools.id

                            left join spool_pipe_fittings on spool_parts.id = spool_pipe_fittings.spool_part_id  
                            left join parts as f1 on f1.id = fitting_1_part_id
                            left join parts as f2 on f2.id = fitting_2_part_id
                            
                            where
                            {where_id}
                            
                            order by
                            {order_by}
                ";

            logit("get_data - start");

            dtab = get_data(select);

            logit("get_data - end");

            SortedList sl_spool_data = new SortedList();

            logit("populate - start");
            foreach (DataRow dr in dtab.Rows)
            {
                dr["id"] = 0;

                int spool_id = (int)dr["spools_id"];

                spool_data sd = null;

                if (!sl_spool_data.ContainsKey(spool_id))
                {
                    sd = new spool_data();

                    populate_spool_data(sd, dr, "spools_");

                    sl_spool_data.Add(spool_id, sd);

                    populate_user_data(sd.fitter_data = new user_data(),  dr, "fitter_data_");
                    populate_user_data(sd.welder_data = new user_data(),  dr, "welder_data_");

                    populate_weld_job_data(sd.weld_job_data = new weld_job_data(), dr, "weld_jobs_");

                    using (schedule_fab sfab = new schedule_fab())
                    {
                        sl.Clear();
                        sl.Add("spool_id", sd.id);

                        a2 = sfab.get_schedule_fab_data(sl);

                        if (a2.Count > 0)
                            sd.schedule_fab_data = (schedule_fab_data)a2[a2.Count - 1];
                    }
                }
                else
                {
                    sd = (spool_data)sl_spool_data[spool_id];
                }

                if(sd.spool_part_data == null)
                    sd.spool_part_data = new ArrayList();

                populate_spool_part_data(sd.spool_part_data, dr, "spool_parts_", "spool_pipe_fittings_");

                if(sd.qa_data == null)
                    sd.qa_data = new ArrayList();

                populate_qa_data(sd.qa_data, dr, "qa_jobs_");
            }

            logit("populate - end");

            foreach (DictionaryEntry e0 in sl_spool_data)
            {
                spool_data sd = (spool_data) e0.Value;

                decimal rate = 0;

                if (sd.spool_part_data != null)
                {
                    foreach (spool_part_data spd in sd.spool_part_data)
                    {
                        if(spd.part_data != null)
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
                        if(spd.part_data != null)
                            rate += spd.part_data.fitter_rate;
                    }
                }

                if (rate == 0)
                    sd.fit_required = false;

                a.Add(sd);
            }

            return a;
        }

        void populate_qa_data(ArrayList a_qad, DataRow dr, string prefix="")
        {
            int qa_jobs_id  = 0;

            try { qa_jobs_id = (int)dr[prefix + "id"];} catch {return; };

            foreach (qa_job_data qajd0 in a_qad)
            {
                if (qajd0.id == qa_jobs_id)
                {
                    return;
                }
            }

            qa_job_data qajd = new qa_job_data();

                try { qajd.id = qa_jobs_id; }
                catch { }
                try { qajd.user_id = (int)dr[prefix+"user_id"]; }
                catch { }
                try { qajd.spool_id = (int)dr[prefix+"spool_id"]; }
                catch { }
                try { qajd.datetime_stamp = Convert.ToDateTime(dr[prefix+"datetime_stamp"].ToString()); }
                catch { }
                try { qajd.result = dr[prefix+"result"].ToString(); }
                catch { }

                try
                {
                    qajd.assembly_type = (int)dr[prefix+"assembly_type"];
                }
                catch { }

            a_qad.Add(qajd);
        }

        void populate_weld_job_data(weld_job_data wjd, DataRow dr, string prefix="")
        {
            try
            {
                wjd.id = (int)dr[prefix+"id"];
            }
            catch {return; }

            try
            {
                wjd.spool_id = (int)dr[prefix+"spool_id"];
            }
            catch { }
                        
            try
            {
                wjd.user_id = (int)dr[prefix+"user_id"]; }
            catch { }

            try
            {
                wjd.fitter_id = (int)dr[prefix+"fitter_id"];
            }
            catch { }

            try
            {
                wjd.site_fitter_id = (int)dr[prefix+"site_fitter_id"];
            }
            catch { }

            try
            { wjd.start = Convert.ToDateTime(dr[prefix+"start"].ToString()); }
            catch { }

            try
            { wjd.finish = Convert.ToDateTime(dr[prefix+"finish"].ToString()); }
            catch { }

            try
            { wjd.start_fit = Convert.ToDateTime(dr[prefix+"start_fit"].ToString()); }
            catch { }
                        
            try
            { wjd.finish_fit = Convert.ToDateTime(dr[prefix+"finish_fit"].ToString()); }
            catch { }
                        
            try
            { wjd.installed_on = Convert.ToDateTime(dr[prefix+"installed_on"].ToString()); }
            catch { }
                        
            try
            {
                wjd.assembly_type = (int)dr[prefix+"assembly_type"];
            }
            catch { }

            try
            {
                wjd.robot = (int)dr[prefix+"robot"];
            }
            catch { }
        }

        void populate_spool_part_data(ArrayList a_spd, DataRow dr, string prefix="", string spool_pipe_fittings_prefix="")
        {
            int spool_parts_id = 0;


            try { spool_parts_id = (int)dr[prefix + "id"]; }
            catch {return; }

            foreach (spool_part_data spd0 in a_spd)
            {
                if (spd0.id == spool_parts_id)
                {
                    return;
                }
            }

            spool_part_data spd = new spool_part_data();
            spool_parts.populate_spool_part_data(spd, dr, prefix, spool_pipe_fittings_prefix);

            spd.part_data = new part_data();
            populate_spool_part_data_part_data(spd.part_data, dr);

            a_spd.Add(spd);
        }

        void populate_spool_part_data_part_data(part_data pd, DataRow dr)
        {
                try{pd.id = (int)dr["parts_id"];}catch{}
                try{pd.part_number = dr["part_number"].ToString();}catch{}
                try{pd.description = dr["description"].ToString();}catch{}
                try{pd.size = dr["size"].ToString();}catch{}
                try{pd.part_type = dr["part_type"].ToString();}catch{}
                try{pd.size_mm = dr["size_mm"].ToString();}catch{}
                try{pd.welder_rate = (decimal)dr["welder_rate"];}catch{}
                try{pd.fitter_rate = (decimal)dr["fitter_rate"];}catch{}
                try{pd.gbe_sale_cost = (decimal)dr["gbe_sale_cost"];}catch{}
                try{pd.pipecenter_sale_cost = (decimal)dr["pipecenter_sale_cost"];}catch{}
                try { pd.olmat_group_sale_cost = (decimal)dr["olmat_group_sale_cost"]; }
                catch { }

                try { pd.buxton_mcnulty_sale_cost = (decimal)dr["buxton_mcnulty_sale_cost"]; }
                catch { }

                try { pd.associated_pipework_fab_only = (decimal)dr["associated_pipework_fab_only"]; }
                catch { }

                try { pd.generic_sale_cost = (decimal)dr["generic_sale_cost"]; }
                catch { }

                try { pd.dgr_fab_and_mat = (decimal)dr["dgr_fab_and_mat"]; }
                catch { }
                try { pd.dgr_fab_only = (decimal)dr["dgr_fab_only"]; }
                catch { }
                try { pd.rates_materials_and_fabrication = (decimal)dr["rates_materials_and_fabrication"]; }
                catch { }

                try{pd.material_cost = (decimal)dr["material_cost"];}catch{}
                try{pd.supplier = dr["supplier"].ToString();}catch{}
                try { pd.additional_description = dr["additional_description"].ToString(); }
                catch { }
                try { pd.manufacturer = dr["manufacturer"].ToString(); }
                catch { }
                try { pd.site_fitter_rate = (decimal)dr["site_fitter_rate"]; }
                catch { }
                try { pd.source = (int)dr["source"]; }
                catch { }
                try { pd.active = (bool)dr["active"]; }
                catch { }

                try { pd.watkins = (decimal)dr["watkins"]; }
                catch { }

                try { pd.apollo = (decimal)dr["apollo"]; }
                catch { }


                try { pd.cps = (decimal)dr["CPS"]; }
                catch { }

                try { pd.excel = (decimal)dr["Excel"]; }
                catch { }

                try { pd.shawston = (decimal)dr["Shawston"]; }
                catch { }

            try { pd.fitting_size_mm = (decimal)dr["fitting_size_mm"]; }
                catch { }

            try { pd.gap_mm = (decimal)dr["gap_mm"]; }
                catch { }
        }

        void populate_user_data(user_data ud, DataRow dr, string prefix="")
        {

            try { ud.id = (int)dr[prefix + "id"]; } catch { }
            try{ud.name = dr[prefix + "name"].ToString();} catch { }
            try{ud.login_id = dr[prefix + "login_id"].ToString();} catch { }
            try{ud.password = dr[prefix + "password"].ToString();} catch { }
            try{ud.role = dr[prefix + "role"].ToString();} catch { }
            try{ud.job_title = dr[prefix + "job_title"].ToString();} catch { }
            try{ud.email = dr[prefix + "email"].ToString();} catch { }
            try{ud.imsl_username = dr[prefix + "imsl_username"].ToString();} catch { }
            try{ud.special_permissions = (int)dr[prefix + "special_permissions"];} catch { }
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
                    //DataTable dta = get_data(m_tbl, search_params);

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

                                sd.spool_part_data = sp.get_spool_parts_data_ex(sl); //plop

                                decimal rate = 0;
                                if (sd.spool_part_data != null)
                                {
                                    foreach (spool_part_data spd in sd.spool_part_data)
                                    {
                                        if(spd.part_data != null)
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
                                        if(spd.part_data != null)
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

        public ArrayList get_spool_data(string spool_ids)
        {
            ArrayList a = new ArrayList();
            
            try
            {
                string sql_select = @"
                        
                    select 

                    spools.[id] as spools_id
                            ,[spool]
                            ,[revision]
                            ,[barcode]
                            ,spools.[welder] as spools_welder
                            ,[delivery_address]
                            ,[cad_user_id]
                            ,[porder_created]
                            ,[status]
                            ,[on_hold]
                            ,[fitter]
                            ,[cost_centre]
                            ,spools.[picked] as spools_picked
                            ,[site_fitter]
                            ,spools.[include_in_weld_map] as spools_include_in_weld_map
                            ,[date_created]
                            ,[delivery_date]
                            ,[imsl_cost_centre]
                            ,[material]
                            ,[pipe_size]
                            ,[cut_size1]
                            ,[cut_size2]
                            ,[cut_size3]
                            ,[cut_size4]
                            ,[drawing_id]
                            ,[checked_by]
                            ,[fab_order_id]

	                        ,spool_parts.[id] as spool_parts_id
                            ,[part_id]
                            ,[spool_id]
                            ,[qty]
                            ,[fw]
                            ,[bw]
                            ,[porder]
                            ,spool_parts.[picked] as spool_parts_picked
                            ,spool_parts.[include_in_weld_map] as spool_parts_include_in_weld_map
                            ,spool_parts.[welder] as spool_parts_welder
                            ,[seq]

                    from spools

                    join spool_parts on spools.id = spool_parts.spool_id

                    where spools.id in (
                        
                    " + spool_ids + ")  ";
                    
                DataTable dta = get_data(sql_select);

                spool_data sd = null;
                int current_spool_id = int.MinValue;
                    
                foreach (DataRow dr in dta.Rows)
                {
                    int next_spool_id = (int)dr["spools_id"];

                    if (next_spool_id != current_spool_id)
                    {
                        current_spool_id = next_spool_id;

                        sd = new spool_data();
                        sd.spool_part_data = new ArrayList();

                        populate_spool_data(sd, dr);

                        a.Add(sd);
                    }

                    spool_part_data spd = new spool_part_data();

                    spool_parts.populate_spool_part_data(spd, dr);

                    sd.spool_part_data.Add(spd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_data(spool_ids) \n" + ex.ToString(), EventLogEntryType.Error);
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

        public bool save_weld_test_report(SortedList sl)
        {
            bool bret = true;

            try
            {
                const string ID = "id";

                int id = save(sl, "weld_test_reports");

                if (!sl.ContainsKey(ID))
                    sl.Add(ID, id);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_weld_test_report()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
        public pdf_doc_data get_drawing_data(int drawing_id)
        {
            SortedList sl = new SortedList();
            sl.Add("id", drawing_id);

            DataTable dta = get_data("drawings", sl);

            pdf_doc_data dd = new pdf_doc_data();

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

        static void logit(string s)
        {
           // File.AppendAllText("c:\\temp\\log.txt", DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fff") + " " + s + "\r\n");
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

        public int weld_test_report_count = 0;

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
    public class pdf_doc_data
    {
        public int id = 0;
        public int spool_id = 0;
        public byte[] pdf = null;
    }
}
