using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class po_orderlines : cdb_connection
    {
        string m_tbl = "po_orderlines";

        public int save_po_orderlines_data(ArrayList al, int porder_id)
        {
            int ret = 0;

            try
            {
                foreach (SortedList sl in al)
                {
                    sl.Add("porder_id", porder_id);
                    save_po_orderlines_data(sl);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "save_po_orderlines_data(2) \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return ret;
        }

        public int save_po_orderlines_data(SortedList sl)
        {
            int ret = 0;

            try
            {
                if (!sl.ContainsKey("id") && !sl.ContainsKey("active"))
                    sl.Add("active", true);

                ret = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "save_po_orderlines_data(1) \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return ret;
        }

        public ArrayList get_po_orderlines_data(SortedList search_params, string part_type, string project)
        {
            ArrayList a = new ArrayList();
            
            try
            {
                using (part_stock ps = new part_stock())
                {
                    using (po_orderline_notes n = new po_orderline_notes())
                    {
                        using (locations loc = new locations())
                        {
                            DataTable dta = get_data(m_tbl, search_params);

                            foreach (DataRow dr in dta.Rows)
                            {
                                po_orderlines_data pod = new po_orderlines_data();

                                try
                                {
                                    try { pod.id = (int)dr["id"]; }
                                    catch { }
                                    try { pod.part_id = (int)dr["part_id"]; }
                                    catch { }
                                    try { pod.qty = (decimal)dr["qty"]; }
                                    catch { }
                                    try { pod.porder_id = (int)dr["porder_id"]; }
                                    catch { }
                                    try { pod.qty_delivered = (decimal)dr["qty_delivered"]; }
                                    catch { }
                                    try { pod.active = (bool)dr["active"]; }
                                    catch { }
                                    try { pod.status = (int)dr["status"]; }
                                    catch { }


                                    SortedList sl = new SortedList();
                                    sl.Add("id", pod.part_id);

                                    if (part_type == "M")
                                    {
                                        using (parts p = new parts())
                                        {
                                            ArrayList apd = p.get_part_data(sl);

                                            foreach (part_data pd in apd)
                                            {
                                                pod.part_desc = pd.description;
                                                pod.unit_cost = pd.material_cost;
                                                pod.supplier = pd.supplier;
                                                pod.project = project;
                                                
                                                // get location info 
                                                sl.Clear();
                                                int part_id =  pd.id;

                                                if (pod.part_desc.EndsWith("*") || pod.part_desc.EndsWith(".")) // ashworth/imsl. get the non */. part
                                                {
                                                    sl.Clear();

                                                    char[] charsToTrim = { '*', ' ', '.' };
                                                    sl.Add("description", pod.part_desc.TrimEnd(charsToTrim));

                                                    ArrayList a_pd_alt = p.get_part_data(sl);

                                                    if (a_pd_alt.Count > 0)
                                                    {
                                                        part_data pd_alt = (part_data)a_pd_alt[0];
                                                        part_id = pd_alt.id;
                                                    }
                                                }

                                                sl.Clear();
                                                
                                                sl.Add("part_id", part_id);

                                                ArrayList aps = ps.get_part_stock_data(sl);

                                                decimal qty = 0;

                                                foreach (part_stock_data psd in aps)
                                                {
                                                    location_data ld = loc.get_location_data(psd.location_id);

                                                    if (psd.qty_in_stock > qty || pod.location == null)
                                                    {
                                                        pod.location = ld;
                                                        qty = psd.qty_in_stock;
                                                    }

                                                    pod.existing_locations_for_this_part.Add(ld);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                    else if (part_type == "C")
                                    {
                                        using (consumable_parts cp = new consumable_parts())
                                        {
                                            ArrayList acpd = cp.get_consumable_part_data(sl);

                                            foreach (consumable_part_data cpd in acpd)
                                            {
                                                string desc = cpd.description;

                                                if (cpd.additional_description.Length > 0)
                                                    desc += " / " + cpd.additional_description;

                                                pod.part_desc = desc;

                                                if (pod.location == null)
                                                    pod.location = cpd.location;

                                                pod.existing_locations_for_this_part.Add(cpd.location);

                                                pod.unit_cost = cpd.unit_cost;
                                                pod.supplier = cpd.supplier.name;
                                            }
                                        }
                                    }

                                    sl.Clear();
                                    sl.Add("po_orderline_id", pod.id);

                                    ArrayList an = n.get_po_orderline_notes_data(sl);

                                    if (an.Count > 0)
                                        pod.note = (po_orderline_notes_data)an[0];

                                }
                                catch { }


                                a.Add(pod);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_po_orderlines_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public void delete_orderline(SortedList sl)
        {
            delete_record(m_tbl, sl);
        }
    }
    [Serializable]
    public class po_orderlines_data
    {
        public int id = 0;
        public int part_id = 0;
        public string part_desc = string.Empty;
        public decimal qty = 0;
        public int porder_id = 0;
        public string project = string.Empty;
        public decimal qty_delivered = 0;
        public decimal unit_cost = 0;

        public string supplier = string.Empty;

        public location_data location = null;
        public ArrayList existing_locations_for_this_part = new ArrayList();
        public po_orderline_notes_data note = null;

        public bool active = false;
        public int status = 0;

        public bool generate_back_order = false;

    }
}
