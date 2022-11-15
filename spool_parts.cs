using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class spool_parts : cdb_connection
    {
        string m_tbl = "spool_parts";

        public spool_parts() { }

        public spool_parts(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public bool save_spool_parts_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_spool_parts_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_spool_parts_data_ex(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            try
            {
                using (parts p = new parts(m_sql_connection))
                {
                    a = get_spool_parts_data(search_params);

                    foreach (spool_part_data spd in a)
                    {
                        sl.Clear();

                        sl.Add("id", spd.part_id);
                        a2 = p.get_part_data(sl);

                        if (a2.Count > 0)
                        {
                            spd.part_data = (part_data)a2[0];

                            if (spd.part_data.description.EndsWith("*") || spd.part_data.description.EndsWith(".")) // ashworth/imsl. get the non */. part and populate 'part_id_alt'
                            {
                                search_params.Clear();
                                char[] charsToTrim = {'*', ' ', '.'};
                                search_params.Add("description", spd.part_data.description.TrimEnd(charsToTrim));

                                ArrayList a_pd_alt = p.get_part_data(search_params);

                                if (a_pd_alt.Count > 0)
                                {
                                    part_data pd_alt = (part_data)a_pd_alt[0];
                                    spd.part_id_alt = pd_alt.id;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_parts_data_ex() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;

        }
        public ArrayList get_spool_parts_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            order_by = " seq, id ";

            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    spool_part_data sd = new spool_part_data();

                    try
                    {
                        try {sd.id = (int)dr["id"];}
                        catch { }
                        try {sd.spool_id = (int)dr["spool_id"];}
                        catch { }
                        try { sd.part_id = (int)dr["part_id"]; }
                        catch { }
                        try {sd.qty = (decimal)dr["qty"];}
                        catch { }
                        try {sd.fw = (int)dr["fw"];}
                        catch { }
                        try {sd.bw = (int)dr["bw"];}
                        catch { }
                        try { sd.porder = (int)dr["porder"]; }
                        catch { }

                        try { sd.picked = (bool)dr["picked"]; }
                        catch { }

                        try { sd.include_in_weld_map = (bool)dr["include_in_weld_map"]; }
                        catch { }

                        try { sd.welder = dr["welder"].ToString(); } catch { }

                        try { sd.seq = (int)dr["seq"]; }
                        catch { }

                    }
                    catch { }

                    a.Add(sd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_parts_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public void delete_parts(int spool_id)
        {
            SortedList sl = new SortedList();
            sl.Add("spool_id", spool_id);

            delete_record(m_tbl, sl);
        }

        int get_sufficed_part_id(int part_id, string suffix)
        {
            // eg * and . ashworth and imsp respectively

            int id = 0;
            using (parts p = new parts(m_sql_connection))
            {
                SortedList sl = new SortedList();

                sl.Add("id", part_id);

                ArrayList a = p.get_part_data(sl);

                if (a.Count > 0)
                {
                    part_data pd = (part_data)a[0];

                    sl.Clear();

                    sl.Add("description", pd.description + suffix);
                    a = p.get_part_data(sl);

                    if (a.Count > 0)
                    {
                        pd = (part_data)a[0];

                        id = pd.id;
                    }
                }
            }

            return id;
        }

        public SortedList get_project_requirement(int part_id)
        {
            SortedList sl = new SortedList();

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = @"select spools.spool, spools.status, spool_parts.qty, spool_parts.part_id  from spool_parts 
                                inner join spools 
                                on spool_parts.spool_id = spools.id where spool_parts.picked <> @picked and (spool_parts.part_id=@part_id";

            int ashworth_part_id = get_sufficed_part_id(part_id, "*");
            int imsl_part_id = get_sufficed_part_id(part_id, ".");

            if (ashworth_part_id > 0)
                cmd.CommandText += " or spool_parts.part_id=@ashworth_part_id";

            if (imsl_part_id > 0)
                cmd.CommandText += " or spool_parts.part_id=@imsl_part_id";

            cmd.CommandText += ") order by spool";

            cmd.Parameters.AddWithValue("@picked", 1);
            cmd.Parameters.AddWithValue("@part_id", part_id);

            if (ashworth_part_id > 0)
                cmd.Parameters.AddWithValue("@ashworth_part_id", ashworth_part_id);

            if (imsl_part_id > 0)
                cmd.Parameters.AddWithValue("@imsl_part_id", imsl_part_id);

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            string project = string.Empty;
            string spool = string.Empty;
            decimal qty = 0; 

            foreach (DataRow dr in dta.Rows)
            {
                project = string.Empty;
                spool = string.Empty;
                qty = 0; 

                try
                {
                    try { spool = dr["spool"].ToString(); }
                    catch { }

                    try { qty = (decimal)dr["qty"]; }
                    catch { }

                    project = spool.Split('-')[0].ToUpper();

                    if (sl.ContainsKey(project))
                    {
                        qty += ((decimal)sl[project]);
                        sl[project] = qty;
                    }
                    else
                        sl.Add(project, qty);

                }
                catch { }
            }

            return sl;
        }
    }

    [Serializable]
    public class spool_part_data
    {
        public int id = 0;
        public int spool_id = 0;
        public int part_id = 0;
        public int part_id_alt = 0;
        public decimal qty = 0;
        public int fw = 0;
        public int bw = 0;
        public int porder = 0;
        public bool picked = false;
        public part_data part_data = null;
        public bool include_in_weld_map = false;
        public int seq = 0;

        public string welder = string.Empty;
    }
}
