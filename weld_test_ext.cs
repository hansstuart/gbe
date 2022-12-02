using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class weld_test_ext : cdb_connection
    {
        string m_tbl_weld_test_ext = "weld_test_ext";
        string m_tbl_weld_test_ext_welder = "weld_test_ext_welder";

        public bool save_weld_test_ext_data(SortedList sl_weldtest, ArrayList a_welder_tests)
        {
            bool bret = true;

            try
            {
                const string SPOOL_ID = "spool_id";

                SortedList sl_del = new SortedList();
                
                sl_del.Add(SPOOL_ID, sl_weldtest[SPOOL_ID].ToString());
                delete_record(m_tbl_weld_test_ext, sl_del);
                delete_record(m_tbl_weld_test_ext_welder, sl_del);

                int weld_test_ext_id = save(sl_weldtest, m_tbl_weld_test_ext);

                foreach (SortedList sl in a_welder_tests)
                {
                    sl.Add("weld_test_ext_id", weld_test_ext_id);

                    save(sl, m_tbl_weld_test_ext_welder);
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_weld_test_ext_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class weld_test_ext_data
    {
        public int id = 0;
        public int weld_tester_id = 0;
        public int spool_id = 0;
        public string report1 = string.Empty;
        public string report2 = string.Empty;
        public DateTime datetime_stamp = DateTime.MinValue;
        public ArrayList a_weld_test_ext_welder = new ArrayList();
    }

    [Serializable]
    public class weld_test_ext_welder
    {
        public int id = 0;
        public int weld_test_ext_id = 0;
        public int welder = 0;
        public int mpi_fw = 0;
        public int ut_bw = 0;
        public int mpi_bw = 0;
        public int xray_bw = 0;
        public int dp_bw = 0;
        public int dp_fw = 0;
        public int vi_bw = 0;
        public int vi_fw = 0;
    }
}