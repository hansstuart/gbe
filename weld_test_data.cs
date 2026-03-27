using System;
using System.Collections;
using System.Data;

namespace gbe
{
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
        public string report9PA_BW = string.Empty;
        public int version = 0;
        public bool pass  = false;
        public bool sent_to_iris  = false;
        public DateTime datetime_stamp = DateTime.MinValue;
        public ArrayList a_weld_test_ext_welder = new ArrayList();

        public void init(DataRow dr)
        {
            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

            id = dr_wm.i_gf("id");
            barcode = dr_wm.s_gf("barcode");
            report1MPI_FW = dr_wm.s_gf("report1MPI_FW");
            report2MPI_BW = dr_wm.s_gf("report2MPI_BW");
            report3UT_BW = dr_wm.s_gf("report3UT_BW");
            report4XRAY_BW = dr_wm.s_gf("report4XRAY_BW");
            report5DP_FW = dr_wm.s_gf("report5DP_FW");
            report6DP_BW = dr_wm.s_gf("report6DP_BW");
            report7VI_FW = dr_wm.s_gf("report7VI_FW");
            report8VI_BW = dr_wm.s_gf("report8VI_BW");
            report9PA_BW = dr_wm.s_gf("report9PA_BW");
            datetime_stamp = dr_wm.dt_gf("datetime_stamp");

            version = dr_wm.i_gf("version");
            pass = dr_wm.b_gf("pass");
            sent_to_iris = dr_wm.b_gf("sent_to_iris");
        }

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
       
        public int total_pa_bw()
        {
            int total = 0;

            foreach (weld_test_ext_welder_data wtewd in a_weld_test_ext_welder)
                total += wtewd.pa_bw;

            return total;
        }
    }

    [Serializable]
    public class weld_test_ext_fw_bw
    {
        public int fw = 0;
        public int bw = 0;

        public int fw_tested = 0;
        public int bw_tested = 0;



        public decimal pc_fw_tested() 
        {
            decimal fw_tested_pc = 0;

            if (fw > 0)
                fw_tested_pc = (Convert.ToDecimal(fw_tested) / Convert.ToDecimal(fw)) * 100;

            return fw_tested_pc;
        }

        public decimal pc_bw_tested()
        {
            decimal bw_tested_pc = 0;

            if (bw > 0)
                bw_tested_pc = (Convert.ToDecimal(bw_tested) / Convert.ToDecimal(bw)) * 100;

            return bw_tested_pc;
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
        public int pa_bw = 0;
        public DateTime datetime_stamp = DateTime.MinValue;
        public string welder = string.Empty;

        public void init(DataRow dr)
        {
            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

            id = dr_wm.i_gf("id");
            welder_id = dr_wm.i_gf("welder_id");
            weld_test_ext_id = dr_wm.i_gf("weld_test_ext_id");
            mpi_fw = dr_wm.i_gf("mpi_fw");
            ut_bw = dr_wm.i_gf("ut_bw");
            mpi_bw = dr_wm.i_gf("mpi_bw");
            xray_bw = dr_wm.i_gf("xray_bw");
            dp_bw = dr_wm.i_gf("dp_bw");
            dp_fw = dr_wm.i_gf("dp_fw");
            vi_bw = dr_wm.i_gf("vi_bw");
            vi_fw = dr_wm.i_gf("vi_fw");
            pa_bw = dr_wm.i_gf("pa_bw");
            welder = dr_wm.s_gf("welder");
            datetime_stamp = dr_wm.dt_gf("datetime_stamp");
        }

        public int total() { return mpi_bw + mpi_fw + ut_bw + xray_bw + dp_bw + dp_fw + vi_bw + vi_fw; }

        public int total_tested_fw() 
        {
            int total = 0;

            if (mpi_fw > 0)
                total = mpi_fw;

            if (dp_fw > 0)
                total = dp_fw;

            if (vi_fw > 0)
                total = vi_fw;

            return total;
        }

        public int total_tested_bw()
        {
            int total = 0;

            if (mpi_bw > 0)
                total = mpi_bw;

            if (ut_bw > 0)
                total = ut_bw;

            if (xray_bw > 0)
                total = xray_bw;

            if (dp_bw > 0)
                total = dp_bw;

            if (vi_bw > 0)
                total = vi_bw;

            if (pa_bw > 0)
                total = pa_bw;

            return total;
        }
    }

    [Serializable]
    public class weld_test_ext_welder_failure_codes_data:data_row
    {
        public int id = 0;
        public int weld_test_ext_id = 0;
        public int welder_id = 0;
        public int spool_id = 0;
        public string test_type = string.Empty;
        public string failure_code = string.Empty;
        public string weld_type = string.Empty;
        public int spool_part_id = 0;

        public void init(DataRow dr)
        {
            data_row_weld_mapping dr_wm = new data_row_weld_mapping(dr);

            id = dr_wm.i_gf("id");
            weld_test_ext_id = dr_wm.i_gf("weld_test_ext_id");
            welder_id = dr_wm.i_gf("welder_id");
            spool_id = dr_wm.i_gf("spool_id");
            test_type = dr_wm.s_gf("test_type");
            failure_code = dr_wm.s_gf("failure_code");
            weld_type = dr_wm.s_gf("weld_type");
            spool_part_id = dr_wm.i_gf("spool_part_id");
        }
    }

    public class data_row_weld_mapping : data_row
    {
        public data_row_weld_mapping(DataRow dr)
        {
            m_dr = dr;
        }

        public int i_gf(  string fld)
        {
            return base.i_gf(m_dr, fld);
        }

        public string s_gf( string fld)
        {
            return base.s_gf(m_dr, fld);
        }

        public decimal d_gf( string fld)
        {
            return base.d_gf(m_dr, fld);
        }

        public bool b_gf( string fld)
        {
            return base.b_gf(m_dr, fld);
        }

        public DateTime dt_gf(string fld)
        {
            return base.dt_gf(m_dr, fld);
        }
    }
}