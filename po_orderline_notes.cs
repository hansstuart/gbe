using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
namespace gbe
{
    public class po_orderline_notes : cdb_connection
    {
        string m_tbl = "po_orderline_notes";

        public bool save_po_orderline_notes_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_po_orderline_notes_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_po_orderline_notes_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    po_orderline_notes_data ld = new po_orderline_notes_data();

                    try
                    {
                        ld.id = (int)dr["id"];
                        ld.note = dr["note"].ToString();
                        ld.po_orderline_id = (int)dr["consignment_id"];
                    }
                    catch { }

                    a.Add(ld);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_po_orderline_notes_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class po_orderline_notes_data
    {
        public int id = 0;
        public string note = string.Empty;
        public int po_orderline_id = 0;
    }
}
