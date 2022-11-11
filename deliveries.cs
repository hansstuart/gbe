using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class deliveries : cdb_connection
    {
        string m_tbl_deliveries = "deliveries";
        string m_tbl_spool_delivery = "spool_delivery";

        public bool save_delivery_data(SortedList sl)
        {
            bool bret = true;

             try
            {
                int delivery_id = save(sl, m_tbl_deliveries);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_delivery_data_data(1) \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public bool save_delivery_data(SortedList sl, ArrayList a_obj)
        {
            bool bret = true;

            try
            {
                int delivery_id = save(sl, m_tbl_deliveries);

                if (!sl.ContainsKey("id"))
                    sl.Add("id", delivery_id);

                if (delivery_id > 0)
                {
                    SortedList sl1 = new SortedList();

                    foreach (object obj in a_obj)
                    {
                        int spool_id = 0;
                        int assembly_type = 0;

                        if (obj.GetType() == typeof(spool_data))
                        {
                            spool_id = ((spool_data)obj).id;
                            assembly_type = 0;
                        }
                        else if (obj.GetType() == typeof(module_data))
                        {
                            spool_id = ((module_data)obj).id;
                            assembly_type = 1;
                        }
                        else if (obj.GetType() == typeof(int))
                        {
                            spool_id = ((int)obj);
                            assembly_type = 0;
                        }

                        sl1.Clear();

                        sl1.Add("spool_id", spool_id);
                        sl1.Add("delivery_id", delivery_id);
                        sl1.Add("assembly_type", assembly_type);
                        
                        save(sl1, m_tbl_spool_delivery);
                    }
                }
                else
                {
                    bret = false;
                    EventLog.WriteEntry("PCF gbe", "save_delivery_data_data(2) \n" + "save[1] returned 0", EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_delivery_data_data(3) \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }


        public ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();
            
            foreach (DataRow dr in dta.Rows)
            {
                delivery_data dd = new delivery_data();

                try
                {
                    dd.id = (int)dr["id"];
                }
                catch { }

                try
                {
                    dd.user_id = (int)dr["user_id"];
                }
                catch { }
                try
                {
                    dd.vehicle = dr["vehicle"].ToString();
                }
                catch { }

                try
                {
                    dd.driver = dr["driver"].ToString();
                }
                catch { }

                try
                {
                    dd.datetime_stamp = Convert.ToDateTime(dr["datetime_stamp"].ToString());
                }
                catch { }

                try
                {
                    dd.delivery_note = (byte[])dr["delivery_note"];
                }
                catch { }

                try
                {
                    dd.datetime_delivered = Convert.ToDateTime(dr["datetime_delivered"].ToString());
                }
                catch { }

                try
                {
                    dd.delivered = (bool)dr["delivered"];
                }
                catch { }

                try
                {
                    dd.receipt_notes = dr["receipt_notes"].ToString();
                }
                catch { }

                try
                {
                    dd.address_table = dr["address_table"].ToString();
                }
                catch { }

                try
                {
                    dd.address_id = (int)dr["address_id"];
                }
                catch { }

                try
                {
                    dd.shipped = (bool)dr["shipped"];
                }
                catch { }

                a.Add(dd);
            }

            return a;
        }

        public ArrayList get_delivery_data(DateTime dtfrom, DateTime dtto)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl_deliveries, dtfrom, dtto, "datetime_stamp");

                dta.DefaultView.Sort = "datetime_stamp ASC";
                dta = dta.DefaultView.ToTable();

                a = load_results_array(dta);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_delivery_data(dt) \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_delivery_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl_deliveries, search_params);
                dta.DefaultView.Sort = "datetime_stamp DESC";
                dta = dta.DefaultView.ToTable();

                a = load_results_array(dta);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_delivery_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_spool_delivery_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl_spool_delivery, search_params);

                SortedList sl = new SortedList();

                foreach (DataRow dr in dta.Rows)
                {
                    spool_delivery_data dd = new spool_delivery_data();

                    try
                    {
                        dd.id = (int)dr["id"];
                        dd.spool_id = (int)dr["spool_id"];
                        dd.delivery_id = (int)dr["delivery_id"];

                        try { dd.assembly_type = (int)dr["assembly_type"]; }
                        catch { }


                        sl.Clear();
                        sl.Add("id", dd.delivery_id);

                        ArrayList a1 = get_delivery_data(sl);

                        if (a1.Count > 0)
                            dd.delivery_data = (delivery_data)a1[0];
                    }
                    catch { }

                    a.Add(dd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_spool_delivery_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public static int get_checkDigit(string idWithoutCheckdigit)
        {

            // allowable characters within identifier
            const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVYWXZ-";

            // remove leading or trailing whitespace, convert to uppercase
            idWithoutCheckdigit = idWithoutCheckdigit.Trim().ToUpper();

            // this will be a running total
            int sum = 0;

            // loop through digits from right to left
            for (int i = 0; i < idWithoutCheckdigit.Length; i++)
            {

                //set ch to "current" character to be processed
                char ch = idWithoutCheckdigit[idWithoutCheckdigit.Length - i - 1];

                // throw exception for invalid characters
                if (validChars.IndexOf(ch) == -1)
                    throw new Exception(ch + " is an invalid character");

                // our "digit" is calculated using ASCII value - 48
                int digit = (int)ch - 48;

                // weight will be the current digit's contribution to
                // the running total
                int weight;
                if (i % 2 == 0)
                {

                    // for alternating digits starting with the rightmost, we
                    // use our formula this is the same as multiplying x 2 and
                    // adding digits together for values 0 to 9.  Using the
                    // following formula allows us to gracefully calculate a
                    // weight for non-numeric "digits" as well (from their
                    // ASCII value - 48).
                    weight = (2 * digit) - (int)(digit / 5) * 9;
                }
                else
                {
                    // even-positioned digits just contribute their ascii
                    // value minus 48
                    weight = digit;
                }

                // keep a running total of weights
                sum += weight;

            }

            // avoid sum less than 10 (if characters below "0" allowed,
            // this could happen)
            sum = Math.Abs(sum) + 10;

            // check digit is amount needed to reach next number
            // divisible by ten
            return (10 - (sum % 10)) % 10;

        }

        public void delete_spool_delivery(int delivery_id)
        {
            SortedList sl = new SortedList();
            sl.Add("delivery_id", delivery_id);
            delete_record(m_tbl_spool_delivery, sl);
        }
    }

    [Serializable]
    public class delivery_data
    {
        public int id = 0;
        public int user_id = 0;
        public string vehicle = string.Empty;
        public string driver = string.Empty;
        public DateTime datetime_stamp = DateTime.MinValue;
        public byte[] delivery_note = null;
        public bool delivered = false;
        public DateTime datetime_delivered = DateTime.MinValue;
        public string receipt_notes = string.Empty;
        public string address_table = string.Empty;
        public int address_id = 0;
        public bool shipped = false;
    }

    [Serializable]
    public class spool_delivery_data
    {
        public static int SPOOL = 0;
        public static int MODULE = 1;
        public int id = 0;
        public int spool_id = 0;
        public int delivery_id = 0;
        public int assembly_type = 0;
        public delivery_data delivery_data = null;
    }
}
