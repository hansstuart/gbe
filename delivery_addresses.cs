using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class delivery_addresses : cdb_connection
    {
        string m_tbl = "delivery_addresses";

        public int save_delivery_address_data(SortedList sl)
        {
            int id = 0;

            try
            {
                id = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                id = -100;
                EventLog.WriteEntry("PCF gbe", "save_delivery_address_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return id;
        }

        public ArrayList get_delivery_address_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    delivery_address cd = new delivery_address();

                    try
                    {
                        cd.id = (int)dr["id"];
                        cd.contact_name = dr["contact_name"].ToString();
                        cd.address_line1 = dr["address_line1"].ToString();
                        cd.address_line2 = dr["address_line2"].ToString();
                        cd.address_line3 = dr["address_line3"].ToString();
                        cd.address_line4 = dr["address_line4"].ToString();
                        cd.contact_name = dr["contact_name"].ToString();
                        cd.telephone = dr["telephone"].ToString();
                        cd.contract_number = dr["contract_number"].ToString();
                    }
                    catch { }

                    a.Add(cd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_delivery_address_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    public class delivery_address
    {
        public int id = 0;
        public string contact_name = string.Empty;
        public string address_line1 = string.Empty;
        public string address_line2 = string.Empty;
        public string address_line3 = string.Empty;
        public string address_line4 = string.Empty;
        public string telephone = string.Empty;
        public string contract_number = string.Empty;
    }

}

