using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;


namespace gbe
{
    public class consignment_reference : cdb_connection
    {
        static string m_tbl = "consignment_reference";

        public static string get_tbl_name() { return m_tbl; }

        public ArrayList get_consignment_reference_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    consignment_reference_data pd = new consignment_reference_data();

                    try
                    {
                        try { pd.id = (int)dr["id"]; }
                        catch { }
                        try { pd.owner = dr["owner"].ToString(); }
                        catch { }
                        try { pd.reference_number = (int)dr["reference_number"]; }
                        catch { }
                        try { pd.reference = dr["reference"].ToString(); }
                        catch { }
                        try { pd.delivery_date = (DateTime)dr["delivery_date"]; }
                        catch { }
                        try { pd.delivered = (bool)dr["delivered"]; }
                        catch { }
                        try { pd.project = dr["project"].ToString(); }
                        catch { }
                    }
                    catch { }

                    a.Add(pd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consignment_reference_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_consignment_reference_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_consignment_reference_data()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class consignment_reference_data
    {
        public int id = 0;
        public string owner = string.Empty;
        public int reference_number = 0;
        public string reference = string.Empty;
        public DateTime delivery_date = DateTime.MinValue;
        public bool delivered = false;
        public string project = string.Empty;

    }
}
