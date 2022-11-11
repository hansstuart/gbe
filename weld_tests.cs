using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class weld_tests : cdb_connection
    {
        string m_tbl = "weld_tests";

        public bool save_weld_test_data(ArrayList akvc)
        {
            bool bret = true;
            SortedList sl = new SortedList();

            foreach (key_value_container kvc in akvc)
            {
                sl.Clear();

                foreach (key_value kv in kvc.container)
                    if (!sl.Contains(kv.key))
                        sl.Add(kv.key, kv.value);

                bret &= save_weld_test_data(sl);
            }

            return bret;
        }

        public bool save_weld_test_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_weld_test_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_weld_test_data(DateTime dtfrom, DateTime dtto, string dt_fld)
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
                weld_test_data wtd = new weld_test_data();

                try
                {
                    try {wtd.id = (int)dr["id"];}catch{}
                    try {wtd.user_id = (int)dr["user_id"];}catch{}
                    try {wtd.spool_id = (int)dr["spool_id"];}catch{}
                    try {wtd.report_number = dr["report_number"].ToString();}catch{}
                    try {wtd.report2_number = dr["report2_number"].ToString();}catch{}
                    try {wtd.fw = (int)dr["fw"];}catch{}
                    try {wtd.bw = (int)dr["bw"];}catch{}
                    try { wtd.datetime_stamp = Convert.ToDateTime(dr["datetime_stamp"].ToString()); }
                    catch { }
                }
                catch { }

                a.Add(wtd);
            }

            return a;
        }

        public ArrayList get_weld_test_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                a = load_results_array(dta);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_weld_test_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class weld_test_data
    {
        public int id = 0;
        public int spool_id = 0;
        public int user_id = 0;
        public string report_number = string.Empty;
        public string report2_number = string.Empty;
        public int fw = 0;
        public int bw = 0;
        public DateTime datetime_stamp = DateTime.MinValue;
    }
}
