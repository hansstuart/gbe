using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;


namespace gbe
{
    public class consignment : cdb_connection
    {
        string m_tbl = "consignment";

        public ArrayList get_consignment_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            SortedList sl = new SortedList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                using (locations l = new locations())
                {
                    foreach (DataRow dr in dta.Rows)
                    {
                        consignment_data pd = new consignment_data();

                        try
                        {
                            try { pd.id = (int)dr["id"]; }
                            catch { }
                            try { pd.description = dr["description"].ToString(); }
                            catch { }
                            try { pd.additional_description = dr["additional_description"].ToString(); }
                            catch { }
                            try { pd.part_number = dr["part_number"].ToString(); }
                            catch { }
                            try { pd.size = dr["size"].ToString(); }
                            catch { }
                            try { pd.manufacturer = dr["manufacturer"].ToString(); }
                            catch { }
                            
                            try { pd.owner = dr["owner"].ToString(); }
                            catch { }
                            try { pd.project = dr["project"].ToString(); }
                            catch { }
                            try { pd.qty_in_stock = (decimal)dr["qty_in_stock"]; }
                            catch { }

                            try { pd.location_id = (int)dr["location_id"]; }
                            catch { }

                            if (pd.location_id > 0)
                            {
                                sl.Clear();

                                sl.Add("id", pd.location_id);

                                ArrayList a_ld = l.get_location_data(sl);

                                if (a_ld.Count > 0)
                                {
                                    location_data ld = (location_data)a_ld[0];

                                    pd.location = ld;
                                }
                            }

                            try { pd.barcode = dr["barcode"].ToString(); }
                            catch { }

                        }
                        catch { }

                        a.Add(pd);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consignment_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_consignment_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_consignment_data()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }
    [Serializable]
    public class consignment_data
    {
        public int id = 0;
        public string description = string.Empty;
        public string additional_description = string.Empty;
        public string part_number = string.Empty;
        public string size = string.Empty;
        public string manufacturer = string.Empty;
        
        public string owner = string.Empty;
        public string project = string.Empty;       
        public decimal qty_in_stock = 0;

        public int location_id = 0;
        public location_data location = null;
        public string barcode = string.Empty;       
    }

}
