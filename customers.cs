using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class customers : cdb_connection
    {
        string m_tbl = "customers";

        public bool save_customer_details(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_customer_details() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_customer_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    customer_data cd = new customer_data();

                    try
                    {
                        cd.id = (int)dr["id"];
                        cd.name = dr["name"].ToString();
                        cd.address_line1 = dr["address_line1"].ToString();
                        cd.address_line2 = dr["address_line2"].ToString();
                        cd.address_line3 = dr["address_line3"].ToString();
                        cd.address_line4 = dr["address_line4"].ToString();
                        cd.contact_name = dr["contact_name"].ToString();
                        cd.telephone = dr["telephone"].ToString();
                        cd.contract_number = dr["contract_number"].ToString();
                        cd.email = dr["email"].ToString();
                    }
                    catch { }

                    a.Add(cd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_customer_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    public class customer_data
    {
        public int id = 0;
        public string name = string.Empty;
        public string address_line1 = string.Empty;
        public string address_line2 = string.Empty;
        public string address_line3 = string.Empty;
        public string address_line4 = string.Empty;
        public string contact_name = string.Empty;
        public string telephone = string.Empty;
        public string contract_number = string.Empty;
        public string email = string.Empty;
    }

}

