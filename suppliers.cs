using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class suppliers : cdb_connection
    {
        string m_tbl = "suppliers";

        public bool save_supplier_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_supplier_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_supplier_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    supplier_data ld = new supplier_data();

                    try
                    {
                        try{ld.id = (int)dr["id"];}catch{}
                        try{ld.name = dr["name"].ToString();}catch{}
                        try{ld.email_address = dr["email_address"].ToString();}catch{}
                        try { ld.po_number = (int)dr["po_number"]; }
                        catch { }
                    }
                    catch { }

                    a.Add(ld);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_supplier_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class supplier_data
    {
        public int id = 0;
        public string name = string.Empty;
        public string email_address = string.Empty;
        public int po_number = 0;
    }
}
