using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;


namespace gbe
{
    public class spool_status : cdb_connection
    {
        string m_tbl = "spool_status";

        public ArrayList get_spool_status_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    spool_status_data ssd = new spool_status_data();

                    try
                    {
                        ssd.id = (int)dr["id"];

                        if (dr["status"].GetType() == typeof(string))
                            ssd.status = dr["status"].ToString();

                        if (dr["description"].GetType() == typeof(string))
                            ssd.description = dr["description"].ToString();
                    }
                    catch { }

                    a.Add(ssd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_status_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class spool_status_data
    {
        public int id = 0;
        public string status = string.Empty;
        public string description = string.Empty;
    }
}
