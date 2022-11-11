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
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class users : cdb_connection
    {
        string m_tbl = "users";
        
        public users()
        {
        }
       
        public users(SqlConnection sql_connection)
        {
            m_sql_connection = sql_connection;
            m_b_keep_open = true;
        }
        

        public ArrayList get_user_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    user_data ud = new user_data();

                    try
                    {
                        ud.id = (int)dr["id"];
                        ud.name = dr["name"].ToString();
                        ud.login_id = dr["login_id"].ToString();
                        ud.password = dr["password"].ToString();
                        ud.role = dr["role"].ToString();
                        ud.job_title = dr["job_title"].ToString();
                        ud.email = dr["email"].ToString();
                        ud.imsl_username = dr["imsl_username"].ToString();
                        ud.special_permissions = (int)dr["special_permissions"];
                    }
                    catch { }

                    a.Add(ud);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_user_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public user_data get_user_data(string username)
        {
            user_data ud = null;

            SortedList sl0 = new SortedList();

            sl0.Add("login_id", username);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);
            }

            if (a.Count > 0)
            {
                ud = (user_data)a[0];
            }

            return ud;
        }

        public user_data get_user_data(int id)
        {
            user_data ud = null;

            SortedList sl0 = new SortedList();

            sl0.Add("id", id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);
            }

            if (a.Count > 0)
            {
                ud = (user_data)a[0];
            }

            return ud;
        }
    }

    [Serializable]
    public class user_data
    {
        public int id = 0;
        public string name = string.Empty;
        public string login_id = string.Empty;
        public string password = string.Empty;
        public string role = string.Empty;
        public string job_title = string.Empty;
        public string email = string.Empty;
        public string imsl_username = string.Empty;
        public int special_permissions = 0;

        public void set_special_permission_on(int pos)
        {
            special_permissions |= (1 << pos);
        }

        public void set_special_permission_off(int pos)
        {
            special_permissions &= ~(1 << pos);
        }

        public bool is_special_permission_set(int pos)
        {
            return (special_permissions & (1 << pos)) != 0;
        }
    }
}
