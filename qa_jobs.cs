using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class qa_jobs : cdb_connection
    {
        string m_tbl = "qa_jobs";

        public qa_jobs()
        {
        }

        public qa_jobs(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public bool save_qa_job_data(ArrayList akvc)
        {
            bool bret = true;
            SortedList sl = new SortedList();

            foreach (key_value_container kvc in akvc)
            {
                sl.Clear();

                foreach (key_value kv in kvc.container)
                    if (!sl.Contains(kv.key))
                        sl.Add(kv.key, kv.value);

                bret &= save_qa_job_data(sl);
            }

            return bret;
        }

        public bool save_qa_job_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_qa_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_qa_job_data(DateTime dtfrom, DateTime dtto, string dt_fld)
        {
            ArrayList a = new ArrayList();

            DataTable dta = get_data(m_tbl, dtfrom, dtto, dt_fld);

            a = load_results_array(dta);

            return a;
        }

        public ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();

            dta.DefaultView.Sort = "datetime_stamp DESC";
            dta = dta.DefaultView.ToTable();

            foreach (DataRow dr in dta.Rows)
            {
                qa_job_data qajd = new qa_job_data();

                try { qajd.id = (int)dr["id"]; }
                catch { }
                try { qajd.user_id = (int)dr["user_id"]; }
                catch { }
                try { qajd.spool_id = (int)dr["spool_id"]; }
                catch { }
                try { qajd.datetime_stamp = Convert.ToDateTime(dr["datetime_stamp"].ToString()); }
                catch { }
                try { qajd.result = dr["result"].ToString(); }
                catch { }

                try
                {
                    qajd.assembly_type = (int)dr["assembly_type"];
                }
                catch { }


                a.Add(qajd);
            }
            return a;
        }

        public ArrayList get_qa_job_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                a = load_results_array( dta);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_qa_job_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class qa_job_data
    {
        public static int SPOOL = 0;
        public static int MODULE = 1;
        public int id = 0;
        public int spool_id = 0;
        public int user_id = 0;
        public DateTime datetime_stamp = DateTime.MinValue;
        public string result = string.Empty;
        public int assembly_type = 0; // 0 = spool, 1 = module
    }
}
