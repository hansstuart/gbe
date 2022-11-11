using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class modules : cdb_connection
    {
        string m_tbl = "modules";

         public modules() { }

        public modules(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }


        public ArrayList get_module_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();

            DataTable dta = get_data(m_tbl, search_params);

            foreach (DataRow dr in dta.Rows)
            {
                module_data md = new module_data();

                try { md.id = (int)dr["id"]; }
                catch { }
                try { md.module = dr["module"].ToString();}
                catch { }
                try { md.revision = dr["revision"].ToString();}
                catch { }
                try { md.barcode = dr["barcode"].ToString(); }
                catch { }
                try { md.status = dr["status"].ToString(); }
                catch { }
                try { md.builder = (int)dr["builder"]; }
                catch { }

                a.Add(md);
            }

            return a;
        }

        public ArrayList get_module_data_ex(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            ArrayList a2 = new ArrayList();
            SortedList sl = new SortedList();

            try
            {
                a = get_module_data(search_params);

                using (weld_jobs wj = new weld_jobs())
                {
                    using (qa_jobs qaj = new qa_jobs())
                    {
                        using (users u = new users())
                        {
                            foreach (module_data sd in a)
                            {
                                sl.Clear();

                                sl.Add("spool_id", sd.id);
                                sl.Add("assembly_type", weld_job_data.MODULE);

                                a2 = wj.get_weld_job_data(sl);

                                if (a2.Count > 0)
                                    sd.weld_job_data = (weld_job_data)a2[0];

                                sd.qa_data = qaj.get_qa_job_data(sl);

                                sl.Clear();
                                sl.Add("id", sd.builder);
                                a2 = u.get_user_data(sl);

                                if (a2.Count > 0)
                                    sd.builder_data = (user_data)a2[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_module_data_ex() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public bool save_module_data(SortedList sl)
        {
            bool bret = true;
            
            int spool_id = save(sl, m_tbl);
            
            if (spool_id <= 0)
            {
                bret = false;
            }
        
            return bret;
        }
    }

    [Serializable]
    public class module_data
    {
        public int id = 0;
        public string module = string.Empty;
        public string revision = string.Empty;
        public string barcode = string.Empty;
        public string status = string.Empty;
        public int builder = 0;

        public user_data builder_data = null;
        public weld_job_data weld_job_data = null;
        public ArrayList qa_data = null; 
    }
}
