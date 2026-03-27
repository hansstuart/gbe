using System;
using System.Collections;
using System.Diagnostics;
using System.Data;

namespace gbe
{
    public class delivery_schedule_email : cdb_connection
    {
        string m_tbl = "delivery_schedule_email";

        public void save_delivery_schedule_email_data(SortedList sl)
        {
            save(sl, m_tbl);
        }

        public ArrayList get_delivery_schedule_email_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();

            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    delivery_schedule_email_data dsed = new delivery_schedule_email_data();

                    dsed.id = (int)dr[delivery_schedule_email_data.ID];
                    dsed.delivery_key = dr[delivery_schedule_email_data.DELIVERY_KEY].ToString();
                    dsed.email_sent = (bool)dr[delivery_schedule_email_data.EMAIL_SENT];
                    dsed.record_datetime = (DateTime)dr[delivery_schedule_email_data.RECORD_DATETIME];
                    try { dsed.sent_datetime = (DateTime)dr[delivery_schedule_email_data.SENT_DATETIME]; } catch { }
                    dsed.user_id = (int)dr[delivery_schedule_email_data.USER_ID];

                    a.Add(dsed);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_delivery_schedule_email_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }
            return a;
        }
        
    }

    [Serializable]
    public class delivery_schedule_email_data
    {
        public static string ID = "id";
        public static string DELIVERY_KEY = "delivery_key";
        public static string EMAIL_SENT = "email_sent";
        public static string SENT_DATETIME = "sent_datetime";
        public static string USER_ID = "user_id";
        public static string RECORD_DATETIME = "record_datetime";

        public int id = 0;
        public string delivery_key = string.Empty;
        public bool email_sent = false;
        public DateTime sent_datetime = DateTime.MinValue;
        public int user_id = 0;
        public DateTime record_datetime = DateTime.MinValue;
    }
}