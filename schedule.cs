using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class schedule : cdb_connection
    {
        public readonly static int PROD_SLOTS = int.MaxValue;  //50;  lockout removed
        public readonly static int EXTRA_SLOTS = int.MaxValue; //15; lockout removed

        virtual public ArrayList get_schedule_recs(string dt) { return null; }

        public DataTable get_schedule_recs(string dt, string tbl)
        {
            DataTable dtab = null;

            DateTime dt_from = DateTime.ParseExact(dt, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            dt_from = new DateTime(dt_from.Year, dt_from.Month, dt_from.Day, 0, 0, 0);
            DateTime dt_to = new DateTime(dt_from.Year, dt_from.Month, dt_from.Day, 23, 23, 59);

            string select = "select TBL.*, spools.barcode, spools.status, spools.welder, spools.fitter, spools.material from TBL ";
            select += " inner join spools on spools.id = TBL.spool_id ";
            select += " where dt >= @dt_from and  dt <= @dt_to";
            select += " and spools.status != 'SH' ";
            select += " and spools.status != 'IN' ";
            select += " and spools.status != 'OS' ";

            select = select.Replace("TBL", tbl);

            using (cdb_connection dbc = new cdb_connection())
            {
                SortedList sl_p = new SortedList();
                sl_p.Add("@dt_from", dt_from);
                sl_p.Add("@dt_to", dt_to);

                dtab = dbc.get_data_p(select, sl_p);
            }

            return dtab;
        }
    }

    public class schedule_fab : schedule
    {
        public virtual string m_tbl
        {
            get { return "schedule_fab"; }
        }

        public int get_extra_allocated_slots(string sdate)
        {
            int allocated_slots = 0;

            ArrayList a_schd = get_schedule_recs(sdate + " 00:00:00");

            foreach (c_schedule_fab_rec sr in a_schd)
            {
                if(sr.schd.batch_number == 0)
                    allocated_slots += sr.schd.slots;
            }

            return allocated_slots;
        }

        public int get_allocated_slots(string sdate)
        {
            int allocated_slots = 0;

            ArrayList a_schd = get_schedule_recs(sdate + " 00:00:00");

            foreach (c_schedule_fab_rec sr in a_schd)
            {
                if (sr.schd.status == "SH" || sr.schd.status == "OS" || sr.schd.status == "IN" || sr.schd.status == "RD" || sr.schd.status == "LD")
                    continue;

                if (sr.schd.batch_number != 0)
                    allocated_slots += sr.schd.slots;
            }

            return allocated_slots;
        }

        public override  ArrayList get_schedule_recs(string dt)
        {
            ArrayList a_schd = new ArrayList();

            DataTable dtab = get_schedule_recs(dt, m_tbl);

            foreach (DataRow dr in dtab.Rows)
            {
                c_schedule_rec sr = new c_schedule_fab_rec();
                schedule_fab_data schd = schedule_fab.get_schedule_fab_data(dr);
                sr.schd = schd;

                try { sr.barcode = dr["barcode"].ToString(); }
                catch { }

                a_schd.Add(sr);
            }

            return a_schd;
        }

        public bool save_schedule_fab_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_schedule_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_schedule_fab_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    schedule_fab_data vd = schedule_fab.get_schedule_fab_data(dr);

                    a.Add(vd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_schedule_fab_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public static schedule_fab_data get_schedule_fab_data(DataRow dr)
        {
            schedule_fab_data vd = new schedule_fab_data();

            try
            {
                vd.id = (int)dr["id"];
            }
            catch { }

            try
            {
                vd.spool_id = (int)dr["spool_id"];
            }
            catch { }

            try
            {
                vd.dt = (DateTime)dr["dt"];
            }
            catch { }

            try
            {
                vd.slots = (int)dr["slots"];
            }
            catch { }

            try
            {
                vd.batch_number = (int)dr["batch_number"];
            }
            catch { }

            try
            {
                vd.status = dr["status"].ToString();
            }
            catch { }

            try
            {
                vd.welder = dr["welder"].ToString();
            }
            catch { }

            try
            {
                vd.fitter = dr["fitter"].ToString();
            }
            catch { }

            try
            {
                vd.material = dr["material"].ToString();
            }
            catch { }

            try
            {
                vd.welder_rates = dr["WELDER_RATES"].ToString();
            }
            catch { }

            try
            {
                vd.fitter_rates = dr["FITTER_RATES"].ToString();
            }
            catch { }

            return vd;
        }

        public static int get_slots(part_data pd)
        {
            int slots = 0;

            if (pd.description.ToUpper().Contains("PIPE")|| pd.part_type.ToUpper().Contains("PIPE"))
            {
                int size = 0;
                char[] trim_chars = { '\x20', 'm' };
                try { size = Convert.ToInt32(pd.size_mm.ToLower().Trim(trim_chars)); }
                catch { }

                if (size >= 65 && size <= 150)
                    slots = 1;
                else if (size >= 200 && size <= 300)
                    slots = 2;
                else if (size >= 350)
                    slots = 3;
            }

            return slots;
        }

#if !NOT_WEB
        public static bool is_date_locked(string save_date)
        {
           
            bool bret = false;

            string users_exempt_to_schedule_time_based_lockout = string.Empty;

            try { users_exempt_to_schedule_time_based_lockout = System.Web.Configuration.WebConfigurationManager.AppSettings["users_exempt_to_schedule_time_based_lockout"].ToString(); }
            catch { }

            string [] sa_users = users_exempt_to_schedule_time_based_lockout.Split('|');
            
            if (sa_users.Length > 0)
            {
                string login_id = System.Web.HttpContext.Current.User.Identity.Name;

                foreach (string suser in sa_users)
                {
                    if (suser.Trim().ToLower() == login_id.Trim().ToLower())
                        return false;
                }
            }

            DayOfWeek day_today = DateTime.Now.DayOfWeek;

            DateTime dt_save;

            if (DateTime.TryParseExact(save_date, "dd/MM/yyyy",
              System.Globalization.CultureInfo.InvariantCulture,
              System.Globalization.DateTimeStyles.None, out dt_save))
            {
                DayOfWeek save_day = dt_save.DayOfWeek;

                if ((day_today == DayOfWeek.Monday && save_day == DayOfWeek.Wednesday)
                || (day_today == DayOfWeek.Tuesday && save_day == DayOfWeek.Thursday)
                || (day_today == DayOfWeek.Wednesday && save_day == DayOfWeek.Friday)
                || (day_today == DayOfWeek.Thursday && save_day == DayOfWeek.Monday)
                || (day_today == DayOfWeek.Friday && save_day == DayOfWeek.Tuesday)
                || (day_today == DayOfWeek.Saturday && save_day == DayOfWeek.Tuesday)
                || (day_today == DayOfWeek.Sunday && save_day == DayOfWeek.Tuesday))
                    bret = true;
            }
            else
                bret = false;

            return bret;
        }
#endif


    }

    public class schedule_fab_extras : schedule_fab
    {
        public override string m_tbl
        {
            get { return "schedule_fab_extras"; }
        }
    }

    public class schedule_sequence : cdb_connection
    {

    }

    [Serializable]
    public class schedule_data
    {
        public static string INIT_DATE = "01/01/1980 00:00:00";
        public static string HOLDING_RECS_DATE = "02/01/1980 00:00:00";
        public static string QUARANTINE_RECS_DATE = "03/01/1980 00:00:00";
        public static DateTime DT_HOLDING_RECS_DATE = new DateTime(1980, 01, 02, 0, 0, 0);
        public static DateTime DT_QUARANTINE_RECS_DATE = new DateTime(1980, 01, 03, 0, 0, 0);

        public static string DEF_ECODE = "00";

        public int id = 0;
        public int spool_id = 0;
        public DateTime dt = DateTime.MinValue;
        public int slots = 0;
        public int batch_number = 0;
        public string vehicle = string.Empty;
        public string status = string.Empty;
        public string welder = string.Empty;
        public string fitter = string.Empty;
        public string project = string.Empty;
        public int cut_and_clean = 0;
        public string site = string.Empty;
        public bool delivered = false;
        public string e_code = DEF_ECODE;
        public string material = string.Empty;
        public bool module = false;
        public string welder_rates = string.Empty;
        public string fitter_rates = string.Empty;
        public int seq = 0;

        public void set_cut_and_clean_bit_on(int pos)
        {
            cut_and_clean |= (1 << pos);
        }

        public void set_cut_and_clean_bit_off(int pos)
        {
            cut_and_clean &= ~(1 << pos);
        }

        public bool is_cut_and_clean_bit_set(int pos)
        {
            return (cut_and_clean & (1 << pos)) != 0;
        }
    }

    [Serializable]
    public class schedule_fab_data : schedule_data
    {
       
    }

    public class schedule_delivery : schedule
    {
        public virtual string m_tbl
        {
            get { return "schedule_delivery"; }
        }

        public override  ArrayList get_schedule_recs(string dt)
        {
            ArrayList a_schd = new ArrayList();

            DataTable dtab = get_schedule_recs(dt, m_tbl);

            foreach (DataRow dr in dtab.Rows)
            {
                c_schedule_rec sr = new c_schedule_deliv_rec();
                schedule_delivery_data schd = schedule_delivery.get_schedule_delivery_data(dr);
                sr.schd = schd;

                try { sr.barcode = dr["barcode"].ToString(); }
                catch { }

                a_schd.Add(sr);
            }

            return a_schd;
        }

        public bool save_schedule_delivery_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_schedule_delivery_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_schedule_delivery_data(SortedList search_params)
        {
            ArrayList a = new ArrayList();
            try
            {
                DataTable dta = get_data(m_tbl, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    schedule_delivery_data vd = schedule_delivery.get_schedule_delivery_data(dr);

                    a.Add(vd);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_schedule_delivery_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public static schedule_delivery_data get_schedule_delivery_data(DataRow dr)
        {
            schedule_delivery_data vd = new schedule_delivery_data();

            try
            {
                vd.id = (int)dr["id"];
            }
            catch { }

            try
            {
                vd.spool_id = (int)dr["spool_id"];
            }
            catch { }
            
            try
            {
                    vd.dt = (DateTime)dr["dt"];
            }catch{}

            try
            {
                vd.vehicle = dr["vehicle"].ToString();
            }
            catch{}

            try
            {
                vd.batch_number = (int)dr["batch_number"];
            }catch{}

            try
            {
                vd.status = dr["status"].ToString();
            }
            catch{}

            try
            {
                vd.welder = dr["welder"].ToString(); 
            }
            catch { }

            try
            {
                vd.fitter = dr["fitter"].ToString();
            }
            catch { }

            try
            {
                vd.material = dr["material"].ToString();
            }
            catch { }

            return vd;
        }

        
    }
    [Serializable]
    
    public class schedule_delivery_data : schedule_data
    {
       
    }

    [Serializable]
    public class c_schedule_rec
    {
        public string barcode;
        public schedule_data schd;
    }

    [Serializable]
    public class c_schedule_fab_rec : c_schedule_rec
    {


    }

    [Serializable]
    public class c_schedule_deliv_rec : c_schedule_rec
    {
    }
}
