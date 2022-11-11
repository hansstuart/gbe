using System;
using System.Data;

namespace gbe
{
    public class customer_fab_mat : data_row
    {
        public readonly static string TBL = "customer_fab_mat";
        public readonly static string ID = "id";
        public readonly static string NAME = "name";
        public readonly static string BFAB = "bfab";
        public readonly static string BMAT = "bmat";

        public customer_fab_mat(DataRow dr)
        {
            m_dr = dr;
        }

        public Int32 id
        {
            get
            {
                return i_gf(m_dr, ID);
            }
        }

        public string name
        {
            get
            {
                return s_gf(m_dr, NAME);
            }
        }

        public bool bfab
        {
            get
            {
                return b_gf(m_dr, BFAB);
            }
        }

        public bool bmat
        {
            get
            {
                return b_gf(m_dr, BMAT);
            }
        }
    }

    public class customer_fab_mat_rates : data_row
    {
        public readonly static string TBL = "customer_fab_mat_rates";
        public readonly static string ID = "id";
        public readonly static string CUSTOMER_FAB_MAT_ID = "customer_fab_mat_id";
        public readonly static string DT = "dt";
        public readonly static string MATERIAL = "material";
        public readonly static string FAB = "fab";        

        public customer_fab_mat_rates(DataRow dr)
        {
            m_dr = dr;
        }

        public Int32 id
        {
            get
            {
                return i_gf(m_dr, ID);
            }
        }

        public Int32 customer_fab_mat_id
        {
            get
            {
                return i_gf(m_dr, CUSTOMER_FAB_MAT_ID);
            }
        }

        public DateTime dt
        {
            get
            {
                return dt_gf(m_dr, DT);
            }
        }

        public decimal material
        {
            get
            {
                return d_gf(m_dr, MATERIAL);
            }
        }

        public decimal fab
        {
            get
            {
                return d_gf(m_dr, FAB);
            }
        }
    }
}
