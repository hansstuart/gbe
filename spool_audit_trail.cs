using System;
using System.Data;
using System.Configuration;
#if !NOT_WEB
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
#endif
using System.Collections;
using System.Diagnostics;


namespace gbe
{
    public class spool_audit_trail : cdb_connection
    {
        string m_tbl = "spool_audit_trail";

        public ArrayList get_spool_audit_trail_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    spool_audit_trail_data sd = new spool_audit_trail_data();

                    try
                    {
                        sd.id = (int)dr["id"];
                        sd.spool_id = (int)dr["spool_id"];
                        sd.status = dr["status"].ToString();
                        sd.datetime_stamp = Convert.ToDateTime( dr["datetime_stamp"].ToString());
                        sd.user_id = dr["user_id"].ToString();

                    }
                    catch { }

                    a.Add(sd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_audit_trail_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_spool_audit_trail_details(SortedList sl)
        {
            bool bret = true;
            
            return bret; // can't see the value of this at the mo. HS20140108

            /*
            try
            {
                if (sl.ContainsKey("datetime_stamp"))
                    sl.Remove("datetime_stamp");

                sl.Add("datetime_stamp", DateTime.Now);

                int spool_id = save(sl, m_tbl);

                if (spool_id == 0)
                {
                    bret = false;
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_spool_audit_trail_details()\n" + ex.ToString(), EventLogEntryType.Error);
            }
            
             
            return bret;
             */
        }

    }

    [Serializable]
    public class spool_audit_trail_data
    {
        public int id = 0;
        public int spool_id = 0;
        public string status = string.Empty;
        public DateTime datetime_stamp = DateTime.MinValue;
        public string user_id = string.Empty; 
    }
}
