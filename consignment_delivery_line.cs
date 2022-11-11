using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class consignment_delivery_line : cdb_connection
    {
        string m_tbl = "consignment_delivery_line";

        public bool save_consignment_delivery_line_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                if (!sl.ContainsKey("id") && !sl.ContainsKey("active"))
                    sl.Add("active", true);

                int id = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_consignment_delivery_line_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_consignment_delivery_line_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                using (consignment_reference cr = new consignment_reference())
                {
                    using (locations l = new locations())
                    {
                        using (consignment_instance c = new consignment_instance())
                        {
                            using (parts p = new parts())
                            {
                                foreach (DataRow dr in dta.Rows)
                                {
                                    consignment_delivery_line_data ld = new consignment_delivery_line_data();

                                    try
                                    {
                                        try { ld.id = (int)dr["id"]; }
                                        catch { }
                                        try { ld.part_id = (int)dr["part_id"]; }
                                        catch { }
                                        try { ld.consignment_reference_id = (int)dr["consignment_reference_id"]; }
                                        catch { }
                                        try { ld.qty_dispatched = (decimal)dr["qty_dispatched"]; }
                                        catch { }
                                        try { ld.qty_delivered = (decimal)dr["qty_delivered"]; }
                                        catch { }
                                        try { ld.active = (bool)dr["active"]; }
                                        catch { }
                                    }
                                    catch { }

                                    SortedList sl = new SortedList();
                                    sl.Add("id", ld.part_id);
                                    ArrayList ap = p.get_part_data(sl);

                                    if (ap.Count > 0)
                                    {
                                        part_data cd = (part_data)ap[0];
                                        ld.description = cd.description;
                                    }

                                    sl.Clear();
                                    sl.Add("part_id", ld.part_id);
                                    ArrayList ac = c.get_consignment_data(sl);

                                    foreach (consignment_instance_data cd in ac)
                                    {
                                        if (ld.location == null)
                                            ld.location = cd.location;

                                        ld.existing_locations_for_this_part.Add(cd.location);
                                    }

                                    sl.Clear();

                                    sl.Add("id", ld.consignment_reference_id);

                                    ArrayList acr = cr.get_consignment_reference_data(sl);

                                    if (acr.Count > 0)
                                    {
                                        ld.consignment_reference_data = (consignment_reference_data)acr[0];
                                    }

                                    sl.Clear();
                                    sl.Add("consignment_id", ld.id);

                                    ArrayList an = new ArrayList();
                                    using (consignment_delivery_line_notes n = new consignment_delivery_line_notes())
                                    {
                                        an = n.get_consignment_delivery_line_notes_data(sl);
                                    }

                                    if (an.Count > 0)
                                    {
                                        ld.note = (consignment_delivery_line_notes_data)an[0];
                                    }

                                    a.Add(ld);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consignment_delivery_line_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }

    [Serializable]
    public class consignment_delivery_line_data
    {
        public int id = 0;
        public int part_id = 0;
        public int consignment_reference_id = 0;
        public decimal qty_dispatched = 0;
        public decimal qty_delivered = 0;
        public string description = string.Empty;
        public consignment_reference_data consignment_reference_data = null;
        public location_data location = null;
        public ArrayList existing_locations_for_this_part = new ArrayList();
        public consignment_delivery_line_notes_data note = null;
        public bool active = false;
    }
}
