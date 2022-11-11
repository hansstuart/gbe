using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class part_stock : cdb_connection
    {
        string m_tbl = "part_stock";

        public ArrayList get_part_stock_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            
            DataTable dta = get_data(m_tbl, search_params);

            a = load_results_array(dta);
            
            return a;
        }

        protected string gen_select(string tbl)
        {
            string select = string.Empty;

            select += "select ";

            if (select_top > 0)
                select += " top " + select_top.ToString();

            if (pg > 0)
            {
                select += " * from (select ROW_NUMBER() over (order by ";
                select += order_by;
                select += " ) as row, ";
            }

            select += " part_stock.id, part_id, location_id, part_stock.qty_in_stock from " + tbl;

            return select;
        }
        public ArrayList get_part_stock_data(string search_field, string search_string)
        {
            ArrayList a = new ArrayList();

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = gen_select(m_tbl);
            cmd.CommandText += " inner join parts on part_stock.part_id = parts.id ";
            cmd.CommandText += gen_where(search_field, search_string, cmd);
            cmd.CommandText += gen_page();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            a = load_results_array(dta);

            return a;
        }

        ArrayList load_results_array(DataTable dta)
        {
            ArrayList a = new ArrayList();

            foreach (DataRow dr in dta.Rows)
            {
                part_stock_data psd = new part_stock_data();

                try
                {
                    try { psd.id = (int)dr["id"]; }
                    catch { }
                    try { psd.part_id = (int)dr["part_id"]; }
                    catch { }
                    try { psd.location_id = (int)dr["location_id"]; }
                    catch { }
                    try { psd.qty_in_stock = (decimal)dr["qty_in_stock"]; }
                    catch { }
                }
                catch { }

                a.Add(psd);
            }
            return a;

        }
        public int save_part_stock_data(SortedList sl)
        {
            int id = 0;
            id = save(sl, m_tbl);

            return id;
        }

        public int get_record_count(string search_field, string search_string)
        {
            int record_count = 0;

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = "select count(*) from " + m_tbl + gen_where(search_field, search_string, cmd);

            cmd.CommandText = select;

            record_count = (int)cmd.ExecuteScalar();

            return record_count;
        }

        string gen_where(string srch_fld, string srch_string, SqlCommand cmd)
        {
            string where = " where ";

            if (srch_fld == "part")
            {
                string where_part = string.Empty;

                if (srch_string.Trim() == "%")
                {
                    where_part = "part_id>@part_id";
                    cmd.Parameters.AddWithValue("@part_id", 0);
                }
                else
                {
                    using (parts p = new parts())
                    {
                        SortedList sl0 = new SortedList();
                        sl0.Add("description", srch_string + "%");
                        ArrayList ap = p.get_part_data(sl0);

                        if (ap.Count > 0)
                        {
                            int i = 0;
                            foreach (part_data pd in ap)
                            {
                                if (i > 0)
                                    where_part += "  or ";
                                where_part += " part_id=@part_id_" + i++.ToString();

                            }

                            i = 0;
                            foreach (part_data pd in ap)
                                cmd.Parameters.AddWithValue("@part_id_" + i++.ToString(), pd.id);
                        }
                    }
                }

                if (where_part.Trim().Length > 0)
                {
                    where += "(";
                    where += where_part;
                    where += ")";
                }
                else
                {
                    // force 0 results
                    where += " id is null";
                }
            }
            else if (srch_fld == "location")
            {
                string where_loc = string.Empty;

                using (locations l = new locations())
                {
                    SortedList sl0 = new SortedList();
                    sl0.Add("location", srch_string + "%");
                    ArrayList al = l.get_location_data(sl0);

                    if (al.Count > 0)
                    {
                        int i = 0;
                        foreach (location_data ld in al)
                        {
                            if (i > 0)
                                where_loc += "  or ";
                            where_loc += " location_id=@location_id_" + i++.ToString();

                        }

                        i = 0;
                        foreach (location_data ld in al)
                            cmd.Parameters.AddWithValue("@location_id_" + i++.ToString(), ld.id);
                    }
                }

                if (where_loc.Trim().Length > 0)
                {
                    where += "(";
                    where += where_loc;
                    where += ")";
                }
                else
                {
                    // force 0 results
                    where += " id is null";
                }
            }
            

            return where;
        }

        public void delete_record(int id)
        {
            SortedList sl = new SortedList();
            sl.Add("id", id);
            delete_record(m_tbl, sl);
        }
    }
    
    [Serializable]
    public class part_stock_data
    {
        public int id = 0;
        public int part_id = 0;
        public int location_id = 0;
        public decimal qty_in_stock = 0;
        public bool generate_back_order = false;
    }
}
