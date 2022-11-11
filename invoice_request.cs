using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class invoice_request : cdb_connection
    {
        string m_tbl = "invoice_request";

        public bool save_invoice_request_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_invoice_request_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_invoice_request_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                a = load_results_array(dta);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_invoice_request_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();

            dta.DefaultView.Sort = "id ASC";
            dta = dta.DefaultView.ToTable();

            foreach (DataRow dr in dta.Rows)
            {
                invoice_request_data ird = new invoice_request_data();

                try { ird.id = (int)dr["id"]; }
                catch { }

                try { ird.dt_from = Convert.ToDateTime(dr["dt_from"].ToString()); }
                catch { }

                try { ird.dt_to = Convert.ToDateTime(dr["dt_to"].ToString()); }
                catch { }

                try { ird.spool = dr["spool"].ToString(); }
                catch { }

                try { ird.email_address = dr["email_address"].ToString(); }
                catch { }

                try { ird.bprocessed = (bool)dr["bprocessed"]; }
                catch { }

                a.Add(ird);
            }
            return a;
        }
    }

    [Serializable]
    public class invoice_request_data
    {
        public int id = 0;
        public string spool = string.Empty;
        public string email_address = string.Empty;
        public DateTime dt_from = DateTime.MaxValue;
        public DateTime dt_to = DateTime.MaxValue;
        public bool bprocessed = false;
    }
}
