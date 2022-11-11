using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;


namespace gbe
{
    public class consignment_delivery_notes : cdb_connection
    {
        static string m_tbl = "consignment_delivery_notes";

        public static string get_tbl_name() { return m_tbl; }

        public ArrayList get_consignment_delivery_notes_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    consignment_delivery_notes_data pd = new consignment_delivery_notes_data();

                    try
                    {
                        try { pd.id = (int)dr["id"]; }
                        catch { }

                        try { pd.consignment_reference_id = (int)dr["consignment_reference_id"]; }
                        catch { }

                        try
                        {
                            pd.pdf = (byte[])dr["pdf"];
                        }
                        catch { }

                        
                    }
                    catch { }

                    a.Add(pd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consignment_delivery_notes_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_consignment_delivery_notes_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_consignment_delivery_notes_data()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

    [Serializable]
    public class consignment_delivery_notes_data
    {
        public int id = 0;
        public int consignment_reference_id = 0;
        public byte[] pdf = null;
        
    }
}
