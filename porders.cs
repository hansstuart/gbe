using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class porders : cdb_connection
    {
        string m_tbl = "porders";

        public bool no_pdf = false;

        public int save_porder_data(SortedList sl)
        {
            int ret = 0;

            try
            {
                if (!sl.ContainsKey("id"))
                {
                    sl.Add("order_date", DateTime.Now);
                }

                ret = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "save_porder_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return ret;
        }

        public ArrayList get_porder_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    porder_data pod = new porder_data();

                    try
                    {
                        pod.id = (int)dr["id"];

                        if (!no_pdf)
                        {
                            try { pod.pdf = (byte[])dr["pdf"]; }
                            catch { }
                            try { pod.doc = (byte[])dr["doc"]; }
                            catch { }
                        }

                        if (dr["order_no"].GetType() == typeof(string))
                            pod.order_no = dr["order_no"].ToString();

                        try{pod.active = (bool)dr["active"];}
                        catch { }

                        try{pod.part_type = dr["part_type"].ToString();}
                        catch { }

                        try {pod.delivery_address_id = (int)dr["delivery_address_id"]; }
                        catch { }

                        try { pod.delivery_date = dr["delivery_date"].ToString(); }
                        catch { }

                        try { pod.order_date = (DateTime) dr["order_date"]; }
                        catch { }

                        try { pod.supplier = dr["supplier"].ToString(); }
                        catch { }

                        try { pod.total_value = (decimal)dr["total_value"]; }
                        catch { }
                    }
                    catch { }

                    a.Add(pod);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_porder_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public string get_order_number(int id)
        {
            string order_number = string.Empty;

             try
            {
            if (connect())
            {
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                string sql = "select order_no from " + m_tbl + " where id=@id";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandText = sql;

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                const string R_LIST = "r_list";
                ad.Fill(ds, R_LIST);

                DataTable dta = ds.Tables[R_LIST];

                foreach (DataRow dr in dta.Rows)
                {
                    try
                    {
                        if (dr["order_no"].GetType() == typeof(string))
                            order_number = dr["order_no"].ToString();
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry("PCF gbe", "get_order_number() \n" + ex.ToString(), EventLogEntryType.Error);
        }

            return order_number;
        }

        public void null_pdf(int id)
        {
            try
            {
                if (connect())
                {
                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = m_sql_connection;

                    string sql = "update " + m_tbl + " set pdf = NULL  where id=@id";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "null_pdf() \n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        public void delete_porder(SortedList sl)
        {
            delete_record(m_tbl, sl);
        }
    }

    [Serializable]
    public class porder_data
    {
        public int id = 0;
        public byte[] pdf = null;
        public byte[] doc = null;
        public string order_no = string.Empty;
        public bool active = false;
        public string part_type = string.Empty;
        public int delivery_address_id = 0;
        public string delivery_date = string.Empty;
        public DateTime order_date = DateTime.MinValue;
        public string supplier = string.Empty;
        public decimal total_value = 0;
    }
}
