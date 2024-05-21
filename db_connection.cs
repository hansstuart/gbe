using System;
using System.Data;
using System.Configuration;

#if !NOT_WEB
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
#endif
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections;

namespace gbe
{
    [Serializable]
    public class cdb_connection : IDisposable
    {
        public string m_and_or_op = "and";
        public int select_top = 0;
        public string order_by = string.Empty;
        public int pg = 0;
        public int recs_per_pg = 0;

        public SqlConnection m_sql_connection = null;

        protected bool m_b_keep_open = false;

        public bool connect()
        {
            bool bret = true;

            try
            {
                 bool bconnect = false;

                if (m_sql_connection == null)
                    bconnect = true;
                else if(m_sql_connection.State == ConnectionState.Closed)
                    bconnect = true;

                if (bconnect)
                {
#if NOT_WEB
                    Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
                    string connection_string = config.AppSettings.Settings["connection_string"].Value.ToString();
#else
                    string connection_string = System.Web.Configuration.WebConfigurationManager.AppSettings["connection_string"].ToString();
#endif
                    m_sql_connection = new SqlConnection(connection_string);
                    m_sql_connection.Open();
                }
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "cdb_connection::connect() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public void disconnect()
        {
            try
            {
                if (m_sql_connection != null)
                {
                    m_sql_connection.Close();
                    m_sql_connection = null;
                }
            }
            catch { }
        }

        public int save(SortedList sl, string tbl)
        {
            int id = 0;

            try
            {
               if(sl.ContainsKey("id"))
                    id = Convert.ToInt32(sl["id"].ToString());

                if (id > 0)
                {
                    update(sl, tbl, id);
                }
                else
                {
                    id = insert(tbl, sl);
                }
            }
            catch (Exception ex)
            {
                id = 0;
                EventLog.WriteEntry("PCF gbe", "cdb_connection()::save \ntable=" + tbl + "\n" +  ex.ToString(), EventLogEntryType.Error);
            }

            return id;
        }

        public int save(SortedList sl, string tbl, string identity_field)
        {
            int id = 0;

            try
            {
               if(sl.ContainsKey(identity_field))
                    id = Convert.ToInt32(sl[identity_field].ToString());

                if (id > 0)
                {
                    update(sl, tbl, id, identity_field);
                }
                else
                {
                    id = insert(tbl, sl);
                }
            }
            catch (Exception ex)
            {
                id = 0;
                EventLog.WriteEntry("PCF gbe", "cdb_connection()::save (2) \ntable=" + tbl + "\n" +  ex.ToString(), EventLogEntryType.Error);
            }

            return id;
        }

        public bool update(SortedList sl, string tbl, int id, string identity_field)
        {
            bool bret = true;

            try
            {
                if (!connect())
                    return false;

                if (sl.ContainsKey(identity_field))
                    sl.Remove(identity_field);

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                string update = "update " + tbl + " set ";
                string flds = string.Empty;

                foreach (DictionaryEntry e in sl)
                {
                    if (flds.Length > 0)
                        flds += ",";

                    string f = "@" + e.Key;

                    flds +=  e.Key + "=" + f;

                    cmd.Parameters.AddWithValue(f, e.Value);
                }

                update += flds;
                update += $" where {identity_field} = " + id.ToString();

                cmd.CommandText = update;

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "cdb_connection()::update (2) \ntable=" + tbl + "\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public bool update(SortedList sl, string tbl, int id)
        {
            bool bret = true;

            try
            {
                if (!connect())
                    return false;

                if (sl.ContainsKey("id"))
                    sl.Remove("id");

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = m_sql_connection;

                string update = "update " + tbl + " set ";
                string flds = string.Empty;

                foreach (DictionaryEntry e in sl)
                {
                    if (flds.Length > 0)
                        flds += ",";

                    string f = "@" + e.Key;

                    flds +=  e.Key + "=" + f;

                    cmd.Parameters.AddWithValue(f, e.Value);
                }

                update += flds;
                update += " where id = " + id.ToString();

                cmd.CommandText = update;

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "cdb_connection()::update \ntable=" + tbl + "\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public int insert(string tbl, SortedList sl)
        {
            if (!connect())
                return 0;

            if (sl.ContainsKey("id"))
                sl.Remove("id");

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string insert = "insert into " + tbl + "(" ;
            string flds = string.Empty;

            foreach (DictionaryEntry e in sl)
            {
                if (flds.Length > 0)
                    flds += ",";

                flds += e.Key;
            }

            insert += flds;

            insert += ") values (";

            flds = string.Empty;

            foreach (DictionaryEntry e in sl)
            {
                if (flds.Length > 0)
                    flds += ",";

                string f = "@" + e.Key;

                flds += f;

                cmd.Parameters.AddWithValue(f, e.Value);
            }

            insert += flds;

            insert += "); SELECT SCOPE_IDENTITY()";

            cmd.CommandText = insert;

            int id = Convert.ToInt32(cmd.ExecuteScalar());
            sl.Add("id", id);

            return id;
        }

        public int get_record_count(string tbl, SortedList search_params)
        {
            int record_count = 0;

            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = "select count(*) from " + tbl;
            
            select += gen_where(search_params, cmd);

            cmd.CommandText = select;

            record_count = (int)cmd.ExecuteScalar();

            return record_count;
        }

        string gen_where(SortedList search_params, SqlCommand cmd)
        {
            string where = string.Empty;
            string where_op = string.Empty;

            if (search_params != null)
            {
                foreach (DictionaryEntry e in search_params)
                {
                    if (where.Length == 0)
                        where = " where ";
                    else
                        where += " " + m_and_or_op + " ";

                    if (e.Value == DBNull.Value)
                        where_op = " is null ";
                    else if (e.Value.ToString().StartsWith("%") || e.Value.ToString().EndsWith("%"))
                        where_op = " like @";
                    else
                        where_op = "=@";

                    where += e.Key + where_op;

                    if (e.Value != DBNull.Value)
                        where += e.Key;

                    cmd.Parameters.AddWithValue("@" + e.Key, e.Value);
                }
            }

            if (pg == 0)
            {
                if (order_by.Trim().Length > 0)
                    where += " ORDER BY " + order_by;
            }

            return where;
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

            select += " * from " + tbl;

            return select;
        }

        protected string gen_page()
        {
            string s = string.Empty;

            if (pg > 0)
            {
                int ito = pg * recs_per_pg;
                int ifrom = (ito - recs_per_pg) + 1;
                s = ") as result_page where row between " + ifrom.ToString() + " and " + ito.ToString();

            }
            return s;
        }

        public DataTable get_data(string tbl, DateTime dtfrom, DateTime dtto, string dt_fld)
        {
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = gen_select(tbl);

            select += " where " + dt_fld + " >= @dtfrom and " + dt_fld + " <= @dtto";

            cmd.Parameters.AddWithValue("@dtfrom", dtfrom);
            cmd.Parameters.AddWithValue("@dtto", dtto);

            select += gen_page();

            cmd.CommandText = select;

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            return dta;
        }

        public DataTable get_data(string tbl, SortedList search_params)
        {
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            string select = gen_select(tbl);

            select += gen_where(search_params, cmd);

            select += gen_page();

            /*
            if(order_by.Trim().Length> 0)
            {
                select += " ORDER BY " + order_by;
            }
              */
            

            cmd.CommandText = select;

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            return dta;
        }

        public DataTable get_data(string select)
        {
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = select;

            SqlDataAdapter ad = new SqlDataAdapter(cmd);

            ad.SelectCommand.CommandTimeout = 600;

            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            return dta;
        }

        public DataTable get_data_p(string select, SortedList sl_params)
        {
            connect();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = m_sql_connection;

            cmd.CommandText = select;

            if (sl_params != null)
            {
                foreach (DictionaryEntry e0 in sl_params)
                {
                    cmd.Parameters.AddWithValue(e0.Key.ToString(), e0.Value);
                }
            }

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            const string R_LIST = "r_list";
            ad.Fill(ds, R_LIST);

            DataTable dta = ds.Tables[R_LIST];

            return dta;
        }

        public void delete_record(string tbl, SortedList search_params)
        {
            if (search_params.Count > 0)
            {
                if (connect())
                {
                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = m_sql_connection;

                    string delete = "delete  from " + tbl;
                    string where = string.Empty;

                    if (search_params != null)
                    {
                        foreach (DictionaryEntry e in search_params)
                        {
                            if (where.Length == 0)
                                where = " where ";
                            else
                                where += " and ";

                            where += e.Key + "=@" + e.Key;
                            cmd.Parameters.AddWithValue("@" + e.Key, e.Value);
                        }
                    }

                    delete += where;

                    cmd.CommandText = delete;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            if (!m_b_keep_open)
                disconnect();

            GC.SuppressFinalize(this);
        }
    }
}
