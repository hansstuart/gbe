using System;
using System.Data;
using System.Collections;
using System.Diagnostics;


namespace gbe
{
    public class locations : cdb_connection
    {
        string m_tbl = "locations";

        public bool save_location_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_location_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_location_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    location_data ld = new location_data();

                    try
                    {
                        ld.id = (int)dr["id"];
                        ld.location = dr["location"].ToString();
                    }
                    catch { }

                    a.Add(ld);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_location_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public string get_location(int loc_id)
        {
            string location =string.Empty;

            SortedList sl = new SortedList();

            sl.Add("id", loc_id);

            ArrayList a_ld = get_location_data(sl);

            if (a_ld.Count > 0)
            {
                location_data ld = (location_data)a_ld[0];

                location = ld.location;
            }

            return location;
        }

        public location_data get_location_data(int loc_id)
        {
            location_data ld = null;

            SortedList sl = new SortedList();

            sl.Add("id", loc_id);

            ArrayList a_ld = get_location_data(sl);

            if (a_ld.Count > 0)
            {
                location_data ld0 = (location_data)a_ld[0];

                ld = ld0;
            }

            return ld;
        }
    }

    

    [Serializable]
    public class location_data
    {
        public int id = 0;
        public string location = string.Empty;
    }
}
