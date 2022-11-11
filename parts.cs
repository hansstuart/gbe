using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;


namespace gbe
{
    public class parts : cdb_connection
    {
        string m_tbl = "parts";

        public parts() { }

        public parts(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }

        public ArrayList get_part_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    part_data pd = new part_data();

                    try
                    {
                         try{pd.id = (int)dr["id"];}catch{}
                         try{pd.part_number = dr["part_number"].ToString();}catch{}
                         try{pd.description = dr["description"].ToString();}catch{}
                         try{pd.size = dr["size"].ToString();}catch{}
                         try{pd.part_type = dr["part_type"].ToString();}catch{}
                         try{pd.size_mm = dr["size_mm"].ToString();}catch{}
                         try{pd.welder_rate = (decimal)dr["welder_rate"];}catch{}
                         try{pd.fitter_rate = (decimal)dr["fitter_rate"];}catch{}
                         try{pd.gbe_sale_cost = (decimal)dr["gbe_sale_cost"];}catch{}
                         try{pd.pipecenter_sale_cost = (decimal)dr["pipecenter_sale_cost"];}catch{}
                         try { pd.olmat_group_sale_cost = (decimal)dr["olmat_group_sale_cost"]; }
                         catch { }

                         try { pd.buxton_mcnulty_sale_cost = (decimal)dr["buxton_mcnulty_sale_cost"]; }
                         catch { }

                         try { pd.associated_pipework_fab_only = (decimal)dr["associated_pipework_fab_only"]; }
                            catch { }

                         try { pd.generic_sale_cost = (decimal)dr["generic_sale_cost"]; }
                         catch { }

                         try { pd.dgr_fab_and_mat = (decimal)dr["dgr_fab_and_mat"]; }
                         catch { }
                         try { pd.dgr_fab_only = (decimal)dr["dgr_fab_only"]; }
                         catch { }
                         try { pd.rates_materials_and_fabrication = (decimal)dr["rates_materials_and_fabrication"]; }
                         catch { }

                         try{pd.material_cost = (decimal)dr["material_cost"];}catch{}
                         try{pd.supplier = dr["supplier"].ToString();}catch{}
                         try { pd.additional_description = dr["additional_description"].ToString(); }
                         catch { }
                         try { pd.manufacturer = dr["manufacturer"].ToString(); }
                         catch { }
                         try { pd.site_fitter_rate = (decimal)dr["site_fitter_rate"]; }
                         catch { }
                         try { pd.source = (int)dr["source"]; }
                         catch { }
                         try { pd.active = (bool)dr["active"]; }
                         catch { }

                         try { pd.watkins = (decimal)dr["watkins"]; }
                         catch { }

                         try { pd.apollo = (decimal)dr["apollo"]; }
                         catch { }


                         try { pd.cps = (decimal)dr["CPS"]; }
                         catch { }

                         try { pd.excel = (decimal)dr["Excel"]; }
                         catch { }

                         try { pd.shawston = (decimal)dr["Shawston"]; }
                         catch { }

                    }

                    catch { }

                    a.Add(pd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_part_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_distinct_part_types()
        {
            ArrayList a = new ArrayList();
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = "select distinct part_type from parts where active = 1 order by part_type asc";

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            foreach (DataRow dr in dta.Rows)
            {
                try { a.Add(dr["part_type"].ToString()); }
                catch { }
            }


            return a;
        }

        public int get_record_count(SortedList search_params)
        {
            return get_record_count(m_tbl, search_params);
        }
    }

    [Serializable]
    public class part_data
    {
        public int id = 0;
        public string part_number = string.Empty;
        public string description = string.Empty;
        public string size = string.Empty;
        public string part_type = string.Empty;
        public decimal welder_rate = 0;
        public decimal fitter_rate = 0;
        public decimal gbe_sale_cost = 0;
        public decimal pipecenter_sale_cost = 0;
        public decimal olmat_group_sale_cost = 0;
        public decimal buxton_mcnulty_sale_cost = 0;
        public decimal associated_pipework_fab_only = 0;
        public decimal generic_sale_cost = 0;
        public decimal watkins = 0;

        public decimal apollo = 0;

        public decimal cps = 0;
        public decimal excel = 0;
        public decimal shawston = 0;

        public decimal dgr_fab_and_mat = 0;
        public decimal dgr_fab_only = 0;
        public decimal rates_materials_and_fabrication = 0;

        public string size_mm = string.Empty;
        public decimal material_cost = 0;
        public string supplier = string.Empty;
        public string additional_description = string.Empty;
        public string manufacturer = string.Empty;
        public decimal site_fitter_rate = 0;
        public int source = 0;
        public bool active = true;

        public SortedList attributes = new SortedList();
    }
}
