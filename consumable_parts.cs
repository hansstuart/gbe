using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class consumable_parts : cdb_connection
    {
        string m_tbl = "consumable_parts";

        public ArrayList get_consumable_part_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            SortedList sl = new SortedList();

            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                using (locations l = new locations())
                {
                    using (suppliers sup = new suppliers())
                    {
                        foreach (DataRow dr in dta.Rows)
                        {
                            consumable_part_data pd = new consumable_part_data();

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

                                try { pd.unit_cost = (decimal)dr["unit_cost"]; }
                                catch { }

                                try { pd.qty_in_stock = (decimal)dr["qty_in_stock"]; }
                                catch { }
                                try { pd.min_stock_holding = (int)dr["min_stock_holding"]; }
                                catch { }
                                try { pd.reorder_qty = (int)dr["reorder_qty"]; }
                                catch { }
                                try { pd.order_status = (int)dr["order_status"]; }
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

                                try { pd.supplier_id = (int)dr["supplier_id"]; }
                                catch { }

                                if (pd.supplier_id > 0)
                                {
                                    sl.Clear();

                                    sl.Add("id", pd.supplier_id);

                                    ArrayList a_sd = sup.get_supplier_data(sl);

                                    if (a_sd.Count > 0)
                                    {
                                        supplier_data sd = (supplier_data)a_sd[0];

                                        pd.supplier = sd;
                                    }
                                }
                            }
                            catch { }

                            a.Add(pd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consumable_part_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_consumable_part_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                int consumable_part_id = save(sl, m_tbl);

                if (consumable_part_id > 0)
                {
                    SortedList sl_pd = new SortedList();
                    sl.Add("id", consumable_part_id);

                    ArrayList a_pd = get_consumable_part_data(sl_pd);

                    if (a_pd.Count > 1)
                    {
                        consumable_part_data pd = (consumable_part_data)a_pd[0];
                    }
                }
                else
                {
                    bret = false;
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_consumable_part_details()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }
    }

        [Serializable]
        public class consumable_part_data
        {
            public int id = 0;
            public string description = string.Empty;
            public string additional_description = string.Empty;
            public string part_number = string.Empty;
            public decimal unit_cost = 0;
            public int supplier_id = 0;
            public decimal qty_in_stock = 0;
            public int min_stock_holding = 0;
            public int reorder_qty = 0;
            public int order_status = 0;
            public int location_id = 0;
            public location_data location = null;
            public supplier_data supplier = null;
        }
    }

