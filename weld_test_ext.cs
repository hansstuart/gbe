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
        public static string m_tbl_weld_test_ext_welder_failure_codes = "weld_test_ext_welder_failure_codes";

        public bool save_weld_test_ext_data(SortedList sl_weldtest, ArrayList a_welder_tests, ArrayList a_fail_codes)
        {
            bool bret = true;

            try
            {
                const string SPOOL_ID = "spool_id";

                SortedList sl_del = new SortedList();
                
                sl_del.Add(SPOOL_ID, sl_weldtest[SPOOL_ID].ToString());
                delete_record(m_tbl_weld_test_ext, sl_del);
                delete_record(m_tbl_weld_test_ext_welder, sl_del);
                delete_record(m_tbl_weld_test_ext_welder_failure_codes, sl_del);

                int weld_test_ext_id = save(sl_weldtest, m_tbl_weld_test_ext);

                foreach (SortedList sl in a_welder_tests)
                {
                    sl.Add("weld_test_ext_id", weld_test_ext_id);

                    save(sl, m_tbl_weld_test_ext_welder);
                }

                if (a_fail_codes != null)
                {
                    foreach (SortedList sl in a_fail_codes)
                    {
                        sl.Add("weld_test_ext_id", weld_test_ext_id);

                        save(sl, m_tbl_weld_test_ext_welder_failure_codes);
                    }
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_weld_test_ext_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public void update_reports(string r1, string r2, string r3, string r4, string r5, string r6, string r7, string r8, string r9, int id, string pass_fail)
        {
            SortedList sl = new SortedList();

            sl.Add("report1MPI_FW", r1);
            sl.Add("report2MPI_BW", r2);
            sl.Add("report3UT_BW", r3);
            sl.Add("report4XRAY_BW", r4);
            sl.Add("report5DP_FW", r5);
            sl.Add("report6DP_BW", r6);
            sl.Add("report7VI_FW", r7);
            sl.Add("report8VI_BW", r8);
            sl.Add("report9PA_BW", r9);

            if(pass_fail.Trim().Length > 0)
                sl.Add("pass", pass_fail=="PASS"?true:false);

            update(sl, m_tbl_weld_test_ext, id);
        }

        public void update_weld_test_ext_welder(string mpi_fw, string mpi_bw, string ut_bw, string xray_bw, string dp_fw, string dp_bw, string vi_fw, string vi_bw, string pa_bw, int id)
        {
            SortedList sl = new SortedList();
            sl.Add("mpi_fw", mpi_fw);
            sl.Add("mpi_bw", mpi_bw);
            sl.Add("ut_bw", ut_bw);
            sl.Add("xray_bw", xray_bw);
            sl.Add("dp_fw", dp_fw);
            sl.Add("dp_bw", dp_bw);
            sl.Add("vi_fw", vi_fw);
            sl.Add("vi_bw", vi_bw);
            sl.Add("pa_bw", pa_bw);

            update(sl, m_tbl_weld_test_ext_welder, id); 
        }
    }

    

    [Serializable]
    public class spool_data_weld_mapping_ext
    {
        public int spool_part_id = 0;
        public string spool_part = string.Empty;
        public string spool_welder = string.Empty; // from spools record (old mapping method) 
        public string spool_part_welder = string.Empty; // from spool_parts record (new method)
        public int fw = 0;
        public int bw = 0;
        public string spool_welder_name = string.Empty;
        public string spool_part_welder_name = string.Empty;
    }

    
}