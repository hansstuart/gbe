using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class vehicles : cdb_connection
    {
        string m_tbl = "vehicles";

        public bool save_vehicle_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_vehicle_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_vehicle_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                dta.DefaultView.Sort = "registration ASC";
                dta = dta.DefaultView.ToTable();

                foreach (DataRow dr in dta.Rows)
                {
                    vehicle_data vd = new vehicle_data();

                    try
                    {
                        vd.id = (int)dr["id"];
                        vd.registration = dr["registration"].ToString();
                        vd.vehicle_type = dr["vehicle_type"].ToString();
                    }
                    catch { }

                    a.Add(vd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_vehicle_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class vehicle_data
    {
        public int id = 0;
        public string registration = string.Empty;
        public string vehicle_type = string.Empty;
    }
}
