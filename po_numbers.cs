using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class po_numbers : cdb_connection
    {
        string m_tbl = "po_numbers";


        private int save_po_number_data(SortedList sl)
        {
            int id = 0;

            try
            {
                id = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                id = -100;
                EventLog.WriteEntry("PCF gbe", "save_po_number_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return id;
        }

        private ArrayList get_po_number_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    po_number cd = new po_number();

                    try
                    {
                        cd.id = (int)dr["id"];
                        cd.contract_number = dr["contract_number"].ToString();
                        cd.user_name = dr["user_name"].ToString();
                        cd.order_number = (int)dr["order_number"];
                    }
                    catch { }

                    a.Add(cd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_po_number_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public int get_next_po_number(string contract_number, string user_name)
        {
            int order_number = 0;

            SortedList sl = new SortedList();

            sl.Add("contract_number", contract_number);
            sl.Add("user_name", user_name);

            ArrayList a = get_po_number_data(sl);

            po_number po_n = new po_number();

            if (a.Count > 0)
            {
                po_n = (po_number)a[0];
                order_number = po_n.order_number+1;
                sl.Add("id", po_n.id);
            }
            else
            {
                order_number = 1;
            }

            sl.Add("order_number", order_number);

            save_po_number_data(sl);
                        
            return order_number;
        }
    }

    public class po_number
    {
        public int id = 0;
        public string contract_number = string.Empty;
        public string user_name = string.Empty;
        public int order_number = 0;
    }

}

