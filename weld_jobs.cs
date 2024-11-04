using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class weld_jobs : cdb_connection
    {
        string m_tbl = "weld_jobs";

        public weld_jobs()
        {
        }

        public weld_jobs(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public ArrayList get_weld_job_data_rev(int user_id, int by_welder_or_fitter, DateTime dtfrom, DateTime dtto, string spool)
        {
            //by_welder_or_fitter. 0 == welder, 1 == fitter, site fitter == 2

            ArrayList a = new ArrayList();
            try
            {
                connect();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;
                
                string select = "select spools.barcode ";
                select += " ,spools.spool";
                select += " ,spools.revision";
                select += " ,spools.date_created";
                select += " ,spools.status";
                select += " ,parts.id as part_id";
                select += " ,parts.description";
                select += " ,spools.cost_centre";
                select += " ,spools.imsl_cost_centre";
                select += " ,spool_parts.qty";
                select += " ,parts.material_cost";
                select += " ,parts.gbe_sale_cost";

                select += " ,parts.pipecenter_sale_cost";
                select += " ,parts.olmat_group_sale_cost";
                select += " ,parts.dgr_fab_and_mat";
                select += " ,parts.dgr_fab_only";
                select += " ,parts.buxton_mcnulty_sale_cost";
                select += " ,parts.associated_pipework_fab_only";
                select += " ,parts.generic_sale_cost";
                select += " ,parts.watkins";
                select += " ,parts.Apollo";
                select += " ,parts.CPS";
                select += " ,parts.Excel";
                select += " ,parts.Shawston";

                select += " ,weld_jobs.* ";

                select += " from " + m_tbl;

                select += " inner join spools on weld_jobs.spool_id=spools.id ";
                select += " inner join spool_parts on weld_jobs.spool_id = spool_parts.spool_id ";
                select += " inner join parts on spool_parts.part_id = parts.id ";

                string where = " where spools.barcode like @spool and (spools.status='QA' or spools.status='RD' or spools.status='WT' or spools.status='SH' or spools.status='OS' or spools.status='IN' or spools.status='LD') and ";

                if (by_welder_or_fitter < 2)
                    //where += "finish >= @from and finish <= @to ";
                    where += "( (finish >= @from and finish <= @to) OR (finish is null AND finish_fit >= @from AND finish_fit <= @to) ) ";
                else
                    where += "installed_on >= @from and installed_on <= @to ";

                string welder_or_fitter_fld = string.Empty;

                if (by_welder_or_fitter == 0)
                    welder_or_fitter_fld = "user_id";
                else if (by_welder_or_fitter == 1)
                    welder_or_fitter_fld = "fitter_id";
                else if (by_welder_or_fitter == 2)
                    welder_or_fitter_fld = "site_fitter_id";


                if (user_id > 0)
                {
                    where += " and ";

                    where += welder_or_fitter_fld;
                    where += "=@user_id ";

                }

                select += where;

                select += " order by spools.barcode " ;

                string from = dtfrom.ToString("yyyy-MM-dd 00:00:00");
                string to = dtto.ToString("yyyy-MM-dd 23:59:59");

                cmd.Parameters.AddWithValue("@from", Convert.ToDateTime(from));
                cmd.Parameters.AddWithValue("@to", Convert.ToDateTime(to));
                cmd.Parameters.AddWithValue("@spool", spool + "%");

                if (user_id > 0)
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                cmd.CommandText = select;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                const string R_LIST = "r_list";
                ad.Fill(ds, R_LIST);

                DataTable dta = ds.Tables[R_LIST];

                SortedList sl = new SortedList();

                string curr_bc = "!�$%";
                weld_job_data wjd = null;

        
                foreach (DataRow dr in dta.Rows)
                {
                    try
                    {
                        string bc = dr["barcode"].ToString();

                        if (bc != curr_bc)
                        {
                            wjd = new weld_job_data();
                            wjd.id = (int)dr["id"];
                            wjd.spool_id = (int)dr["spool_id"];
                            try
                            {
                                wjd.user_id = (int)dr["user_id"];
                            }
                            catch { }

                            try
                            {
                                wjd.fitter_id = (int)dr["fitter_id"];
                            }
                            catch { }

                            try
                            { wjd.start = Convert.ToDateTime(dr["start"].ToString()); }
                            catch { }

                            try
                            { wjd.finish = Convert.ToDateTime(dr["finish"].ToString()); }
                            catch { }
                            try
                            { wjd.start_fit = Convert.ToDateTime(dr["start_fit"].ToString()); }
                            catch { }
                            try
                            { wjd.finish_fit = Convert.ToDateTime(dr["finish_fit"].ToString()); }
                            catch { }

                            try
                            { wjd.installed_on = Convert.ToDateTime(dr["installed_on"].ToString()); }
                            catch { }

                            try
                            {
                                wjd.assembly_type = (int)dr["assembly_type"];
                            }
                            catch { }

                            try
                            {
                                wjd.robot = (int)dr["robot"];
                            }
                            catch { }

                            wjd.spool_data = new spool_data();

                            wjd.spool_data.barcode = bc;

                            try
                            {
                                wjd.spool_data.spool = dr["spool"].ToString();
                            }
                            catch { }

                            try
                            {
                                wjd.spool_data.revision = dr["revision"].ToString();
                            }
                            catch { }

                            try
                            {
                                wjd.spool_data.date_created = (DateTime)dr["date_created"];
                            }
                            catch { }

                            try
                            {
                                wjd.spool_data.cost_centre = (int)dr["cost_centre"];
                            }
                            catch { }

                            try
                            {
                                wjd.spool_data.imsl_cost_centre = (int)dr["imsl_cost_centre"];
                            }
                            catch { }

                            a.Add(wjd);

                            curr_bc = bc;

                        }

                        spool_part_data spd = new spool_part_data();

                        try
                        {
                            spd.qty = (decimal)dr["qty"];
                        }
                        catch { }
                         
                        part_data pd = new part_data();

                        try
                        {
                            pd.id = (int)dr["part_id"];
                        }
                        catch { }

                        try
                        {
                            pd.description = dr["description"].ToString();
                        }
                        catch { }

                        try
                        {
                            pd.material_cost = (decimal)dr["material_cost"];
                        }
                        catch { }

                        try
                        {
                            pd.gbe_sale_cost = (decimal)dr["gbe_sale_cost"];
                        }
                        catch { }
                        
                        try
                        {
                            pd.pipecenter_sale_cost = (decimal)dr["pipecenter_sale_cost"];
                        }
                        catch { }

                        try
                        {
                            pd.olmat_group_sale_cost = (decimal)dr["olmat_group_sale_cost"];
                        }
                        catch { }

                        try
                        {
                            pd.dgr_fab_and_mat = (decimal)dr["dgr_fab_and_mat"];
                        }
                        catch { }

                        try
                        {
                            pd.dgr_fab_only = (decimal)dr["dgr_fab_only"];
                        }
                        catch { }

                        try
                        {
                            pd.buxton_mcnulty_sale_cost = (decimal)dr["buxton_mcnulty_sale_cost"];
                        }
                        catch { }

                        try
                        {
                            pd.associated_pipework_fab_only = (decimal)dr["associated_pipework_fab_only"];
                        }
                        catch { }

                        try
                        {
                            pd.generic_sale_cost = (decimal)dr["generic_sale_cost"];
                        }
                        catch { }

                        try
                        {
                            pd.watkins = (decimal)dr["watkins"];
                        }
                        catch { }

                        try
                        {
                            pd.apollo = (decimal)dr["Apollo"];
                        }
                        catch { }

                        try
                        {
                            pd.cps = (decimal)dr["CPS"];
                        }
                        catch { }

                        try
                        {
                            pd.excel = (decimal)dr["Excel"];
                        }
                        catch { }

                        try
                        {
                            pd.shawston = (decimal)dr["Shawston"];
                        }
                        catch { }

                        spd.part_data = pd;

                        if (wjd.spool_data.spool_part_data == null)
                            wjd.spool_data.spool_part_data = new ArrayList();

                        wjd.spool_data.spool_part_data.Add(spd);
                       
                    }
                    catch { }

                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_weld_job_data(int user_id, int by_welder_or_fitter, DateTime dtfrom, DateTime dtto, string spool, string order_by)
        {
            //by_welder_or_fitter. 0 == welder, 1 == fitter, site fitter == 2

            ArrayList a = new ArrayList();
            try
            {
                connect();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                string select = "select * from " + m_tbl;

                select += " inner join spools on weld_jobs.spool_id=spools.id ";
                string where = " where spools.barcode like @spool and (spools.status='QA' or spools.status='RD' or spools.status='WT' or spools.status='SH' or spools.status='OS' or spools.status='IN' or spools.status='LD') and ";

                if (by_welder_or_fitter < 2)
                    //where += "finish >= @from and finish <= @to ";
                    where += "( (finish >= @from and finish <= @to) OR (finish is null AND finish_fit >= @from AND finish_fit <= @to) ) ";
                else
                    where += "installed_on >= @from and installed_on <= @to ";

                string welder_or_fitter_fld = string.Empty;

                if (by_welder_or_fitter == 0)
                    welder_or_fitter_fld = "user_id";
                else if (by_welder_or_fitter == 1)
                    welder_or_fitter_fld = "fitter_id";
                else if (by_welder_or_fitter == 2)
                    welder_or_fitter_fld = "site_fitter_id";


                if (user_id > 0)
                {
                    where += " and ";

                    where += welder_or_fitter_fld;
                    where += "=@user_id ";

                }

                select += where;

                if (order_by.Length > 0)
                    select += " order by " + welder_or_fitter_fld;

                string from = dtfrom.ToString("yyyy-MM-dd 00:00:00");
                string to = dtto.ToString("yyyy-MM-dd 23:59:59");

                cmd.Parameters.AddWithValue("@from", Convert.ToDateTime(from));
                cmd.Parameters.AddWithValue("@to", Convert.ToDateTime(to));
                cmd.Parameters.AddWithValue("@spool", spool+"%");

                if (user_id > 0)
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                cmd.CommandText = select;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                const string R_LIST = "r_list";
                ad.Fill(ds, R_LIST);

                DataTable dta = ds.Tables[R_LIST];

                SortedList sl = new SortedList();

                using (modules module = new modules(m_sql_connection))
                {
                    using (spools spls = new spools(m_sql_connection))
                    {
                        foreach (DataRow dr in dta.Rows)
                        {
                            weld_job_data wjd = new weld_job_data();

                            try
                            {
                                wjd.id = (int)dr["id"];
                                wjd.spool_id = (int)dr["spool_id"];
                                try
                                {
                                    wjd.user_id = (int)dr["user_id"];
                                }
                                catch { }

                                try
                                {
                                    wjd.fitter_id = (int)dr["fitter_id"];
                                }
                                catch { }

                                try
                                { wjd.start = Convert.ToDateTime(dr["start"].ToString()); }
                                catch { }

                                try
                                { wjd.finish = Convert.ToDateTime(dr["finish"].ToString()); }
                                catch { }
                                try
                                { wjd.start_fit = Convert.ToDateTime(dr["start_fit"].ToString()); }
                                catch { }
                                try
                                { wjd.finish_fit = Convert.ToDateTime(dr["finish_fit"].ToString()); }
                                catch { }

                                try
                                { wjd.installed_on = Convert.ToDateTime(dr["installed_on"].ToString()); }
                                catch { }

                                try
                                {
                                    wjd.assembly_type = (int)dr["assembly_type"];
                                }
                                catch { }

                                try
                                {
                                    wjd.robot = (int)dr["robot"];
                                }
                                catch { }

                                sl.Clear();
                                sl.Add("id", wjd.spool_id);

                                if (wjd.assembly_type == weld_job_data.SPOOL)
                                {
                                    ArrayList asd = spls.get_spool_data_ex(sl);

                                    if (asd.Count > 0)
                                        wjd.spool_data = (spool_data)asd[0];
                                }
                                else if (wjd.assembly_type == weld_job_data.MODULE)
                                {
                                    ArrayList asd = module.get_module_data(sl);

                                    if (asd.Count > 0)
                                        wjd.module_data = (module_data)asd[0];
                                }
                            }
                            catch { }

                            a.Add(wjd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public DataTable get_weld_job_activity_data(string login_id, int by_welder_or_fitter, DateTime dtfrom, DateTime dtto, string spool)
        {
            DataTable dta = null;

            // by_welder_or_fitter. 0 == welder, 1 == fitter, robot == 3
            try
            {
                connect();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                string select = @"
                    
                    select 
                    COALESCE(NULLIF(spool_parts.welder, ''), spools_welder_user.login_id)  as welder_login_id
                    , qty
                    , fw
                    , bw
                    , barcode
                    , description
                    , qty
                    , start
                    , finish
                    , start_fit
                    , finish_fit
                    , robot
                    , welder_rate
                    , fitter_rate
                    , fitter_user.login_id as fitter_login_id

                    from spool_parts

                    inner join  spools on spools.id = spool_parts.spool_id  
                    inner join parts on parts.id = spool_parts.part_id
                    inner join weld_jobs on weld_jobs.spool_id = spool_parts.spool_id  
                    inner join users as fitter_user on fitter_user.id = weld_jobs.fitter_id  
                    inner join users as spools_welder_user on spools_welder_user.id = spools.welder

                    where

                    ( ISNULL(spool_parts.welder,'') != 'N/A' and ISNULL(spools_welder_user.login_id ,'') != 'N/A')
                    and                        
                    finish_dt >= @from and finish_dt <= @to
                    and  
                    spools.barcode like @barcode

                ";

                if (by_welder_or_fitter == 0 )
                {
                    if(login_id.Trim().Length > 0)
                        select += " and ( spool_parts.welder = @login_id or (spool_parts.welder is null and spools_welder_user.login_id = @login_id ) ) ";

                    select += " order by welder_login_id, barcode ";    
                }
                else if (by_welder_or_fitter == 1   )
                {
                    if(login_id.Trim().Length > 0)
                        select += " and login_id = @login_id ";

                    select += " order by fitter_login_id, barcode ";    
                }
                else if (by_welder_or_fitter == 3)
                {
                     select += " and robot > 0 ";
                    select += " order by welder_login_id, barcode ";    
                }
                
                string from = dtfrom.ToString("yyyyMMdd");
                string to = dtto.ToString("yyyyMMdd");

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@login_id", login_id);

                if (spool.Trim().Length > 0)
                    cmd.Parameters.AddWithValue("@barcode", spool+"%");

                cmd.CommandText = select;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                const string R_LIST = "r_list";
                ad.Fill(ds, R_LIST);

                dta = ds.Tables[R_LIST];
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_job_activity_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }
            
            return dta;
        }

        public ArrayList get_weld_job_data(int user_id, int by_welder_or_fitter, DateTime dtfrom, DateTime dtto, string spool)
        {
            // by_welder_or_fitter. 0 == welder, 1 == fitter, site fitter == 2, robot == 3

            ArrayList a = new ArrayList();
            try
            {
                connect();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                //string select = "select * "; // HS. 20240806
                string select = "select distinct weld_jobs.id  ,weld_jobs.* "; // HS. 20240806

                select += " , spools.id, spools.barcode ";

                if (by_welder_or_fitter < 3)
                {
                    select += " , users.name ";
                }

                select +=  " from " + m_tbl;

                string join = string.Empty;
                join = " inner join  spools on spools.id = weld_jobs.spool_id ";
                
                join += " inner join users on users.id = weld_jobs.";

                if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                    join += "user_id ";
                else if (by_welder_or_fitter == 1)
                    join += "fitter_id ";
                else if (by_welder_or_fitter == 2)
                    join += "site_fitter_id ";
            
                // HS. 20240806
                join += " inner join spool_parts on spool_parts.spool_id = spools.id ";

                string where = " where ";

                if (by_welder_or_fitter == 0 || by_welder_or_fitter == 3)
                    where += "finish_dt >= @from and finish_dt <= @to ";
                else if (by_welder_or_fitter == 1)
                    where += "finish_fit >= @from and finish_fit <= @to ";
                else if (by_welder_or_fitter == 1)
                    where += "installed_on >= @from and installed_on <= @to ";

                // HS. 20240806
                ///////////////
                if (by_welder_or_fitter == 0)
                {
                    if (user_id > 0)
                    {
                        where += " and spool_parts.welder = @welder  ";

                        string welder = string.Empty;

                        using (users u = new users(m_sql_connection))
                        {
                            welder = u.get_user_data(user_id).login_id;
                        }

                        cmd.Parameters.AddWithValue("@welder ", welder);
                    }
                }
                else if (by_welder_or_fitter == 1 
                         || by_welder_or_fitter == 2)
                {
                    string welder_or_fitter_fld = string.Empty;

                    if (by_welder_or_fitter == 1)
                        welder_or_fitter_fld = "fitter_id";
                    else if (by_welder_or_fitter == 2)
                        welder_or_fitter_fld = "site_fitter_id";

                    where += " and ";

                    where += welder_or_fitter_fld;

                    if (user_id > 0)
                        where += " = ";
                    else
                        where += " > ";

                    where += "@user_id ";
                }
                else if (by_welder_or_fitter == 3)
                {
                    where += " and robot > 0 ";
                }
                ///////////

                if (spool.Trim().Length > 0)
                    where += " and spools.barcode like @barcode ";

                select += join;
                select += where;

                select += " order by   ";

                if (by_welder_or_fitter < 3) // HS. 20240806
                {
                    select += " users.name,  ";
                }
                
                select += " spools.barcode ";

                string from = dtfrom.ToString("yyyyMMdd");
                string to = dtto.ToString("yyyyMMdd");

                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);

                cmd.Parameters.AddWithValue("@user_id", user_id);

                if (spool.Trim().Length > 0)
                    cmd.Parameters.AddWithValue("@barcode", spool+"%");

                cmd.CommandText = select;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                const string R_LIST = "r_list";
                ad.Fill(ds, R_LIST);

                DataTable dta = ds.Tables[R_LIST];

                SortedList sl = new SortedList();

                using (modules module = new modules(m_sql_connection))
                {
                    using (spools spls = new spools(m_sql_connection))
                    {
                        foreach (DataRow dr in dta.Rows)
                        {
                            weld_job_data wjd = new weld_job_data();

                            try
                            {
                                wjd.id = (int)dr["id"];
                                wjd.spool_id = (int)dr["spool_id"];
                                try
                                {
                                    wjd.user_id = (int)dr["user_id"];
                                }
                                catch { }

                                try
                                {
                                    wjd.fitter_id = (int)dr["fitter_id"];
                                }
                                catch { }

                                try
                                { wjd.start = Convert.ToDateTime(dr["start"].ToString()); }
                                catch { }

                                try
                                { wjd.finish = Convert.ToDateTime(dr["finish"].ToString()); }
                                catch { }
                                try
                                { wjd.start_fit = Convert.ToDateTime(dr["start_fit"].ToString()); }
                                catch { }
                                try
                                { wjd.finish_fit = Convert.ToDateTime(dr["finish_fit"].ToString()); }
                                catch { }

                                try
                                { wjd.installed_on = Convert.ToDateTime(dr["installed_on"].ToString()); }
                                catch { }

                                try
                                {
                                    wjd.assembly_type = (int)dr["assembly_type"];
                                }
                                catch { }

                                try
                                {
                                    wjd.robot = (int)dr["robot"];
                                }
                                catch { }

                                sl.Clear();
                                sl.Add("id", wjd.spool_id);

                                if (wjd.assembly_type == weld_job_data.SPOOL)
                                {
                                    ArrayList asd = spls.get_spool_data_ex(sl);

                                    if (asd.Count > 0)
                                        wjd.spool_data = (spool_data)asd[0];
                                }
                                else if (wjd.assembly_type == weld_job_data.MODULE)
                                {
                                    ArrayList asd = module.get_module_data(sl);

                                    if (asd.Count > 0)
                                        wjd.module_data = (module_data)asd[0];
                                }
                            }
                            catch { }

                            a.Add(wjd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_weld_job_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);
                
                foreach (DataRow dr in dta.Rows)
                {
                    weld_job_data wjd = new weld_job_data();

                    try
                    {
                        wjd.id = (int)dr["id"];
                        wjd.spool_id = (int)dr["spool_id"];
                        
                        try
                        {
                            wjd.user_id = (int)dr["user_id"]; }
                        catch { }

                        try
                        {
                            wjd.fitter_id = (int)dr["fitter_id"];
                        }
                        catch { }

                        try
                        {
                            wjd.site_fitter_id = (int)dr["site_fitter_id"];
                        }
                        catch { }

                        try
                        { wjd.start = Convert.ToDateTime(dr["start"].ToString()); }
                        catch { }

                        try
                        { wjd.finish = Convert.ToDateTime(dr["finish"].ToString()); }
                        catch { }

                        try
                        { wjd.start_fit = Convert.ToDateTime(dr["start_fit"].ToString()); }
                        catch { }
                        
                        try
                        { wjd.finish_fit = Convert.ToDateTime(dr["finish_fit"].ToString()); }
                        catch { }
                        
                        try
                        { wjd.installed_on = Convert.ToDateTime(dr["installed_on"].ToString()); }
                        catch { }
                        
                        try
                        {
                            wjd.assembly_type = (int)dr["assembly_type"];
                        }
                        catch { }

                        try
                        {
                            wjd.robot = (int)dr["robot"];
                        }
                        catch { }
                    }
                    catch { }

                    a.Add(wjd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_weld_job_data(ArrayList akvc)
        {
            bool bret = true;
            SortedList sl = new SortedList();

            foreach (key_value_container kvc in akvc)
            {
                sl.Clear();

                foreach (key_value kv in kvc.container)
                    if (!sl.Contains(kv.key))
                        sl.Add(kv.key, kv.value);

                bret &= save_weld_job_data(sl);
            }

            return bret;
        }

        public bool save_weld_job_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_weld_jobs() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class weld_job_data
    {
        public static int SPOOL = 0;
        public static int MODULE = 1;
        public int id = 0;
        public int spool_id = 0;
        public DateTime start = DateTime.MinValue;
        public DateTime finish = DateTime.MinValue;
        public DateTime start_fit = DateTime.MinValue;
        public DateTime finish_fit = DateTime.MinValue;
        public int user_id = 0;
        public int fitter_id = 0;
        public int site_fitter_id = 0;
        public DateTime installed_on = DateTime.MinValue;
        public int assembly_type = 0; // 0 = spool, 1 = module
        public int robot = 0;

        public spool_data spool_data = null;
        public module_data module_data = null;
    }

    [Serializable]
    public class data_row_weld_activity : data_row
    {
        public readonly static string WELDER_LOGIN_ID = "welder_login_id";
        public readonly static string FITTER_LOGIN_ID = "fitter_login_id";
        public readonly static string BARCODE = "barcode";
        public readonly static string START = "start";
        public readonly static string FINISH = "finish";
        public readonly static string START_FIT = "start_fit";
        public readonly static string FINISH_FIT = "finish_fit";
        public readonly static string FITTER_ID = "fitter_id";
        public readonly static string DESCRIPTION = "description";
        public readonly static string QTY = "qty";
        public readonly static string FW = "fw";
        public readonly static string BW = "bw";
        public readonly static string ROBOT = "robot";
        public readonly static string WELDER_RATE = "welder_rate";
        public readonly static string FITTER_RATE = "fitter_rate";

        public data_row_weld_activity(DataRow dr)
        {
            m_dr = dr;
        }

        public decimal qty
        {
            get
            {
                return d_gf(m_dr, QTY);
            }
        }

        public decimal welder_rate
        {
            get
            {
                return d_gf(m_dr, WELDER_RATE);
            }
        }

        public decimal fitter_rate
        {
            get
            {
                return d_gf(m_dr, FITTER_RATE);
            }
        }

        public int fw
        {
            get
            {
                return i_gf(m_dr, FW);
            }
        }

        public int bw
        {
            get
            {
                return i_gf(m_dr, BW);
            }
        }

         public int robot
        {
            get
            {
                return i_gf(m_dr, ROBOT);
            }
        }

        public string barcode
        {
            get
            {
                return s_gf(m_dr, BARCODE);
            }
        }

        public DateTime start
        {
            get
            {
                return dt_gf(m_dr, START);
            }
        }

        public DateTime finish
        {
            get
            {
                return dt_gf(m_dr, FINISH);
            }
        }

        public DateTime start_fit
        {
            get
            {
                return dt_gf(m_dr, START_FIT);
            }
        }

        public DateTime finish_fit
        {
            get
            {
                return dt_gf(m_dr, FINISH_FIT);
            }
        }

        public string description
        {
            get
            {
                return s_gf(m_dr, DESCRIPTION);
            }
        }

        public string welder_login_id
        {
            get
            {
                return s_gf(m_dr, WELDER_LOGIN_ID);
            }
        }

        public string fitter_login_id
        {
            get
            {
                return s_gf(m_dr, FITTER_LOGIN_ID);
            }
        }
    }
}
