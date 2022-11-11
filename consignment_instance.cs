using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class consignment_instance : cdb_connection
    {
        string m_tbl = "consignment_instance";

        public ArrayList get_consignment_data(ArrayList a_part_ids)
        {
            ArrayList a = new ArrayList();

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = gen_select(m_tbl);
            cmd.CommandText += gen_where(a_part_ids, cmd);
            cmd.CommandText += gen_page();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            a = load_results_array(dta);

            return a;
        }

        string gen_where(ArrayList a_part_ids, SqlCommand cmd)
        {
            string where = string.Empty;

            if (a_part_ids.Count > 0)
            {
                where = " where (";

                int i = 0;
                foreach (int part_id in a_part_ids)
                {
                    if (i > 0)
                        where += "  or ";
                    where += " part_id=@part_id_m" + i++.ToString();

                }

                where  += ")";

                i = 0;
                foreach (int part_id in a_part_ids)
                    cmd.Parameters.AddWithValue("@part_id_m" + i++.ToString(), part_id);
            
            }
            else
            {
                // force 0 results
                where = " id is null";
            }
        

            return where;
        }

        ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();
            SortedList sl = new SortedList();

            using (parts p = new parts())
            {
                using (locations l = new locations())
                {
                    foreach (DataRow dr in dta.Rows)
                    {
                        consignment_instance_data pd = new consignment_instance_data();

                        try
                        {
                            try { pd.id = (int)dr["id"]; }
                            catch { }

                            try { pd.part_id = (int)dr["part_id"]; }
                            catch { }
                            sl.Clear();
                            sl.Add("id", pd.part_id);

                            ArrayList a_pd = p.get_part_data(sl);

                            if (a_pd.Count > 0)
                            {
                                part_data pd1 = (part_data)a_pd[0];

                                try { pd.description = pd1.description; }
                                catch { }
                                try { pd.additional_description = pd1.additional_description; }
                                catch { }
                                try { pd.part_number = pd1.part_number; }
                                catch { }
                                try { pd.size = pd1.size; }
                                catch { }
                                try { pd.manufacturer = pd1.manufacturer; }
                                catch { }
                            }

                            try { pd.owner = dr["owner"].ToString(); }
                            catch { }
                            try { pd.project = dr["project"].ToString(); }
                            catch { }
                            try { pd.qty_in_stock = (decimal)dr["qty_in_stock"]; }
                            catch { }
                            try { pd.barcode = dr["barcode"].ToString(); }
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

                        }
                        catch { }

                        a.Add(pd);
                    }
                }
            }

            return a;
        }

        public ArrayList get_consignment_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                a = load_results_array( dta);


            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_consignment_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public int save_consignment_data(SortedList sl)
        {
            int ret = 0;

            try
            {
                ret = save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "save_consignment_data()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return ret;
        }
    }
    [Serializable]
    public class consignment_instance_data
    {
        public int id = 0;
        public int part_id = 0;
        public string description = string.Empty;
        public string additional_description = string.Empty;
        public string part_number = string.Empty;
        public string size = string.Empty;
        public string manufacturer = string.Empty;

        public string owner = string.Empty;
        public string project = string.Empty;
        public string barcode = string.Empty;
        public decimal qty_in_stock = 0;

        public int location_id = 0;
        public location_data location = null;

        public string guid = string.Empty;

    }

}

