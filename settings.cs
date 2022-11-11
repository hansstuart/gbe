using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class settings : cdb_connection
    {
        string m_tbl = "settings";

        public settings_data get_settings_data()
        {
            settings_data sd = null;
            
            DataTable dta = get_data(m_tbl, new SortedList());

            foreach (DataRow dr in dta.Rows)
            {
                sd = new settings_data();
                sd.id = (int)dr["id"];

                try { sd.pin = dr["pin"].ToString(); }
                catch { }

                try { sd.schedule_batch_number = (int)dr["schedule_batch_number"]; }
                catch { }

                try { sd.gbe_barcode_team_email = dr["gbe_barcode_team_email"].ToString(); }
                catch { }

                try { sd.imsl_email = dr["imsl_email"].ToString(); }
                catch { }


                break;
            }

            return sd;
        }

        public void save_settings_data(SortedList sl)
        {
            save(sl, m_tbl);
        }
    }

    [Serializable]
    public class settings_data
    {
        public int id = 0;
        public int schedule_batch_number = 0;
        public string pin = string.Empty;
        public string gbe_barcode_team_email = string.Empty;
        public string imsl_email = string.Empty;
    }
}
