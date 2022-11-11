using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class new_spools : cdb_connection
    {
        string m_tbl = "new_spools";

        public string get_tbl()
        {
            return m_tbl;
        }

        public ArrayList get_new_spool_data(string project)
        {
            delete_completed_new_spools();

            ArrayList a = new ArrayList();
            ArrayList asd = new ArrayList();
            SortedList sl = new SortedList();

            sl.Add("printed", false);

            m_and_or_op = "or";

            DataTable dta = get_data(m_tbl, sl);

            try
            {
                foreach (DataRow dr in dta.Rows)
                {
                    new_spool_data bd = new new_spool_data();

                    try
                    {
                        bd.id = (int)dr["id"];
                        bd.spool_id = (int)dr["spool_id"];

                        if (dr["printed"].GetType() == typeof(bool))
                            bd.printed = (bool)dr["printed"];

                        if (dr["assembly_type"].GetType() == typeof(int))
                            bd.assembly_type = (int)dr["assembly_type"];

                        sl.Clear();
                        sl.Add("id", bd.spool_id.ToString());

                        if (bd.assembly_type == new_spool_data.SPOOL)
                        {
                            using (spools s = new spools())
                            {
                                asd = s.get_spool_data_short(sl);
                            }

                            if (asd.Count > 0)
                            {
                                spool_data sd = (spool_data)asd[0];

                                if (project.Length > 0)
                                    if (sd.spool.ToUpper().StartsWith(project.ToUpper()))
                                        bd.barcode = sd.barcode;
                            }
                        }
                        else if (bd.assembly_type == new_spool_data.MODULE)
                        {
                            using (modules s = new modules())
                            {
                                asd = s.get_module_data(sl);
                            }

                            if (asd.Count > 0)
                            {
                                module_data sd = (module_data)asd[0];

                                if (project.Length > 0)
                                    if (sd.module.ToUpper().StartsWith(project.ToUpper()))
                                        bd.barcode = sd.barcode;
                            }
                        }
                    }
                    catch { }

                    a.Add(bd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_barcodes() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_new_spool_data(ArrayList akvc)
        {
            bool bret = true;
            SortedList sl = new SortedList();

            foreach (key_value_container kvc in akvc)
            {
                sl.Clear();

                foreach (key_value kv in kvc.container)
                {
                    if (kv != null)
                    {
                        if (!sl.Contains(kv.key))
                            sl.Add(kv.key, kv.value);
                    }
                }
                bret &= save_new_spool_data(sl);
            }

            return bret;
        }

        public bool save_new_spool_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                int id = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_new_spool_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        void delete_completed_new_spools()
        {
            SortedList sl = new SortedList();

            sl.Clear();
            sl.Add("printed", true);
            
            delete_record(m_tbl, sl);
        }
    }

    [Serializable]
    public class new_spool_data
    {
        public static int SPOOL = 0;
        public static int MODULE = 1;
        public int id = 0;
        public int spool_id = 0;
        public bool printed = false;
        public int assembly_type = 0; // 0 = spool, 1 = module
        public string barcode = string.Empty;
        //public spool_data sd = null;
        //public module_data md = null;
    }
}
