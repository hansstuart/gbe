using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
namespace gbe
{
    public class installation_report : cdb_connection
    {
        string m_tbl = "installation_report";

        public bool save_installation_report_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_installation_report_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_installation_report_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    installation_report_data ird = new installation_report_data();

                    try
                    {
                        ird.id = (int)dr["id"];
                        ird.barcode = dr["barcode"].ToString();
                        ird.datetime_stamp = (DateTime)dr["datetime_stamp"];
                        ird.login_id = dr["login_id"].ToString();
                    }
                    catch { }

                    a.Add(ird);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_installation_report_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public void delete_installation_report_data(SortedList search_params)
        {
            delete_record(m_tbl, search_params);
        }
    }

    public class installation_report_data
    {
        public int id = 0;
        public string barcode = string.Empty;
        public DateTime datetime_stamp = DateTime.Now;
        public string login_id = string.Empty;
    }
}
