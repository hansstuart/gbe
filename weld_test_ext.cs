﻿using System;
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

        public void update_reports(string r1, string r2, string r3, string r4, string r5, string r6, string r7, string r8, int id)
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

            update(sl, m_tbl_weld_test_ext, id);
        }
    }

    [Serializable]
    public class weld_test_ext_data
    {
        public int id = 0;
        public int weld_tester_id = 0;
        public int spool_id = 0;
        public string barcode = string.Empty;
        public string report1MPI_FW = string.Empty;
        public string report2MPI_BW = string.Empty;
        public string report3UT_BW = string.Empty;
        public string report4XRAY_BW = string.Empty;
        public string report5DP_FW = string.Empty;
        public string report6DP_BW = string.Empty;
        public string report7VI_FW = string.Empty;
        public string report8VI_BW = string.Empty;
        public DateTime datetime_stamp = DateTime.MinValue;
        public ArrayList a_weld_test_ext_welder = new ArrayList();
        public int fw = 0;
        public int bw = 0;

        public int total_mpi_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.mpi_bw;

            return total;
        }

        public int total_mpi_fw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.mpi_fw;

            return total;
        }

        public int total_ut_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.ut_bw;

            return total;
        }

        public int total_xray_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.xray_bw;

            return total;
        }

        public int total_dp_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.dp_bw;

            return total;
        }

        public int total_dp_fw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.dp_fw;

            return total;
        }

        public int total_vi_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.vi_bw;

            return total;
        }

        public int total_vi_fw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.vi_fw;

            return total;
        }
       
    }

    [Serializable]
    public class weld_test_ext_welder_data
    {
        public int id = 0;
        public int weld_test_ext_id = 0;
        public int welder_id = 0;
        public int mpi_bw = 0;
        public int mpi_fw = 0;
        public int ut_bw = 0;
        public int xray_bw = 0;
        public int dp_bw = 0;
        public int dp_fw = 0;
        public int vi_bw = 0;
        public int vi_fw = 0;

        public string welder = string.Empty;

        public int total() { return mpi_bw + mpi_fw + ut_bw + xray_bw + dp_bw + dp_fw + vi_bw + vi_fw; }
    }


}