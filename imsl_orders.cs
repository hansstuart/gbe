using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class imsl_orders : cdb_connection
    {
        string m_tbl = "imsl_orders";

        public imsl_orders() { }

        public imsl_orders(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public ArrayList get_imsl_order_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    imsl_order_data iod = new imsl_order_data();

                    try
                    {
                        try { iod.id = (int)dr["id"]; }
                        catch { }

                        try { iod.porder_id = (int)dr["porder_id"]; }
                        catch { }

                        try { iod.bsent = (bool)dr["bsent"]; }
                        catch { }

                        try { iod.dt_sent = (DateTime)dr["dt_sent"]; }
                        catch { }

                        try { iod.user_id = (int)dr["user_id"]; }
                        catch { }

                        try { iod.imsl_username = dr["imsl_username"].ToString(); }
                        catch { }

                        users u = new users(m_sql_connection);
                        iod.ud = u.get_user_data(iod.user_id);
                    }

                    catch { }

                    a.Add(iod);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_imsl_order_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_imsl_order_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_imsl_order_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class imsl_order_data
    {
        public int id = 0;
        public int porder_id = 0;
        public bool bsent = false;
        public DateTime dt_sent = DateTime.MinValue;
        public int user_id = 0;
        public user_data ud = null;
        public string imsl_username = string.Empty;
    }
}
