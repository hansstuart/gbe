using System;
using System.Data;

namespace gbe
{
    public class data_row
    {
        protected DataRow m_dr = null;

        protected string s_gf(DataRow dr, string fld)
        {
            string s = string.Empty;

            try { s = o_gf(dr, fld).ToString(); }
            catch { }

            return s;
        }

        protected int i_gf(DataRow dr, string fld)
        {
            int i = 0;

            try { i = (int)o_gf(dr, fld); }
            catch { }

            return i;
        }

        protected bool b_gf(DataRow dr, string fld)
        {
            bool bret = false;

            try { bret = (bool)o_gf(dr, fld); }
            catch { }

            return bret;
        }

        protected long l_gf(DataRow dr, string fld)
        {
            long i = 0;

            try { i = (long)o_gf(dr, fld); }
            catch { }

            return i;
        }

        protected decimal d_gf(DataRow dr, string fld)
        {
            decimal d = 0;

            try { d = (decimal)o_gf(dr, fld); }
            catch { }

            return d;
        }

        protected DateTime dt_gf(DataRow dr, string fld)
        {
            DateTime dt = DateTime.MinValue;

            try { dt = (DateTime)o_gf(dr, fld); }
            catch { }

            return dt;
        }

        object o_gf(DataRow dr, string fld)
        {
            try
            {
                return dr[fld];
            }
            catch { return string.Empty; }
        }

        public object o_gf(string fld)
        {
            return o_gf(m_dr, fld);
        }

        protected byte[] ba_gf(DataRow dr, string fld)
        {
            byte[] ba = null;

            try { ba = (byte[])o_gf(dr, fld); }
            catch { }
            return ba;
        }
    }
}