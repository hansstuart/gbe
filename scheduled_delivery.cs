using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace gbe
{
    public class scheduled_delivery : cdb_connection
    {
        string m_tbl = "scheduled_delivery";
        public static string ID = "id";
        public static string CONTRACT = "contract";
        public static string SITE = "site";
        public static string DATETIME_OF_DELIVERY = "datetime_of_delivery";
        public static string DATETIME_OF_LOAD = "datetime_of_load";
        public static string DRIVER = "driver";
        public static string VEHICLE = "vehicle";
        public static string LOADING_REQUIREMENTS = "loading_requirements";
        public static string ADDITIONAL_ITEMS = "additional_items";
        public static string ACTIVE = "active";

        public ArrayList get_current_scheduled_delivery_data(DateTime dttoday)
        {
            ArrayList a = new ArrayList();
        
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = "select * from " + m_tbl;

            string where = " where " + ACTIVE + " =1 and " + DATETIME_OF_LOAD + "<=@dt";

            string dt = dttoday.ToString("yyyy-MM-dd 23:59:59");
            cmd.Parameters.AddWithValue("@dt", Convert.ToDateTime(dt));

            select += where;

            select += " order by " + DATETIME_OF_LOAD + " ASC";

            cmd.CommandText = select;

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            foreach (DataRow dr in dta.Rows)
            {
                scheduled_delivery_data sd = null;

                try
                {
                    sd = load_scheduled_delivery_data(dr);
                }
                catch { }

                a.Add(sd);
            }

            return a;
        }

        public ArrayList get_scheduled_delivery_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            
            DataTable dta = get_data(m_tbl, search_params);

            foreach (DataRow dr in dta.Rows)
            {
                scheduled_delivery_data sd = null;

                try
                {
                    sd = load_scheduled_delivery_data(dr);
                }
                catch { }

                a.Add(sd);
            }

            return a;
        }

        scheduled_delivery_data load_scheduled_delivery_data(DataRow dr)
        {
            scheduled_delivery_data sd = new scheduled_delivery_data();

            sd.id = (int)dr[ID];
            sd.contract = dr[CONTRACT].ToString();
            sd.site = dr[SITE].ToString();
            
            try { sd.datetime_of_delivery = (DateTime)dr[DATETIME_OF_DELIVERY]; }
            catch { }

            try { sd.datetime_of_load = (DateTime)dr[DATETIME_OF_LOAD]; }
            catch { }

            sd.driver = dr[DRIVER].ToString();
            sd.vehicle = dr[VEHICLE].ToString();
            sd.loading_requirements = dr[LOADING_REQUIREMENTS].ToString();
            sd.additional_items = dr[ADDITIONAL_ITEMS].ToString();
            sd.active = (bool)dr[ACTIVE];

            return sd;
        }

        public int save_scheduled_delivery_data(SortedList sl)
        {
            return save(sl, m_tbl);
        }

        public void delete_scheduled_delivery(int id)
        {
            SortedList sl = new SortedList();

            sl.Clear();
            sl.Add(ID, id);

            delete_record(m_tbl, sl);
        }
    }

    [Serializable]
    public class scheduled_delivery_data
    {
        public int id = 0;
        public string contract = string.Empty;
        public string site = string.Empty;
        public DateTime datetime_of_delivery = DateTime.Today;
        public DateTime datetime_of_load = DateTime.Today;
        public string driver = string.Empty;
        public string vehicle = string.Empty;
        public string loading_requirements = string.Empty;
        public string additional_items = string.Empty;
        public bool active = false;
    }
}
