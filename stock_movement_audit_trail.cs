using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class stock_movement_audit_trail : cdb_connection
    {
        string m_tbl = "stock_movement_audit_trail";

        string gen_where(DateTime dt, string srch_fld, string srch_string, SqlCommand cmd)
        {
            string where = " where datetime_stamp < @dt";

            string sdt = dt.ToString("yyyy-MM-dd 23:59:59");
            DateTime dt0 = Convert.ToDateTime(sdt);
            cmd.Parameters.AddWithValue("@dt", dt0);

            if (srch_fld == "part")
            {
                string where_part = string.Empty;

                using (parts p = new parts())
                {
                    SortedList sl0 = new SortedList();
                    sl0.Add("description", srch_string + "%");
                    ArrayList ap = p.get_part_data(sl0);
                    
                    if (ap.Count > 0)
                    {
                        where_part += "((part_type=@part_type_m or part_type=@part_type_n) and (";

                        int i = 0;
                        foreach (part_data pd in ap)
                        {
                            if (i > 0)
                                where_part += "  or ";
                            where_part += " part_id=@part_id_m" + i++.ToString();

                        }
                        where_part += "))";

                        cmd.Parameters.AddWithValue("@part_type_m", "M");
                        cmd.Parameters.AddWithValue("@part_type_n", "N");

                        i = 0;
                        foreach (part_data pd in ap)
                            cmd.Parameters.AddWithValue("@part_id_m" + i++.ToString(), pd.id);
                    }
                }

                using (consumable_parts c = new consumable_parts())
                {
                    SortedList sl0 = new SortedList();
                    sl0.Add("description", srch_string + "%");
                    ArrayList ap = c.get_consumable_part_data(sl0);

                    if (ap.Count > 0)
                    {
                        if(where_part.Length > 0)
                        {
                            where_part += " or ";
                        }

                        where_part += "(part_type=@part_type_c and (";

                        int i = 0;
                        foreach (consumable_part_data pd in ap)
                        {
                            if (i > 0)
                                where_part += "  or ";
                            where_part += " part_id=@part_id_c" + i++.ToString();

                        }
                        where_part += "))";

                        cmd.Parameters.AddWithValue("@part_type_c", "C");

                        i = 0;
                        foreach (consumable_part_data pd in ap)
                            cmd.Parameters.AddWithValue("@part_id_c" + i++.ToString(), pd.id);
                    }
                }

                if (where_part.Trim().Length > 0)
                {
                    where += " and ";
                    where += "(";
                    where += where_part;
                    where += ")";
                }
                else
                {
                    // force 0 results
                    where += " and id is null";
                }
            }
            else if (srch_fld == "user_id")
            {
                where += " and user_id=@user_id";
                cmd.Parameters.AddWithValue("@user_id", srch_string);
            }
            else if (srch_fld == "destination")
            {
                where += " and destination like @destination";
                cmd.Parameters.AddWithValue("@destination", srch_string+"%");
            }

            return where;
        }

       

        public int get_record_count(DateTime dt, string srch_fld, string srch_string)
        {
            int record_count = 0;

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = "select count(*) from " + m_tbl + gen_where(dt, srch_fld, srch_string, cmd);

            cmd.CommandText = select;

            record_count = (int)cmd.ExecuteScalar();

            return record_count;
        }

        public ArrayList get_stock_movement_audit_trail_data(DateTime dt, string srch_fld, string srch_string)
        {
            ArrayList a = new ArrayList();
         
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = gen_select(m_tbl);
            cmd.CommandText += gen_where(dt, srch_fld, srch_string, cmd);
            cmd.CommandText += gen_page();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            a = load_results_array(dta);

            return a;
        }

        ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();

            dta.DefaultView.Sort = "datetime_stamp DESC";
            dta = dta.DefaultView.ToTable();

            foreach (DataRow dr in dta.Rows)
            {
                stock_movement_audit_trail_data pd = new stock_movement_audit_trail_data();

                try
                {
                    try { pd.id = (int)dr["id"]; }
                    catch { }
                    try { pd.user_id = dr["user_id"].ToString(); }
                    catch { }
                    try { pd.part_type = dr["part_type"].ToString(); }
                    catch { }
                    try { pd.part_id = (int)dr["part_id"]; }
                    catch { }
                    try { pd.datetime_stamp = Convert.ToDateTime(dr["datetime_stamp"].ToString()); }
                    catch { }
                    try { pd.movement_event = dr["movement_event"].ToString(); }
                    catch { }
                    try { pd.quantity = (decimal)dr["quantity"]; }
                    catch { }
                    try { pd.destination = dr["destination"].ToString(); }
                    catch { }

                }
                catch { }

                a.Add(pd);
            }

            return a;
        }

        public ArrayList get_stock_movement_audit_trail_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                a = load_results_array(dta);
                
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_stock_movement_audit_trail_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_stock_movement_audit_trail_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                int id = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_stock_movement_audit_trail_data()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class stock_movement_audit_trail_data
    {
        public int id = 0;
        public string user_id = string.Empty;
        public string part_type = string.Empty;
        public int part_id = 0;
        public DateTime datetime_stamp = DateTime.Now;
        public string movement_event = string.Empty;
        public decimal quantity = 0;
        public string destination = string.Empty;
    }
}
