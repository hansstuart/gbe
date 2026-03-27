using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class irisndt_upload : cdb_connection
    {
        const string m_tbl = "irisndt_upload";
        public static readonly string ID = "id";
        public static readonly string DT = "dt";
        public static readonly string FILENAME = "filename";
        public static readonly string FILE_CONTENT = "file_content";
        public static readonly string PROCESSED = "processed";
        public static readonly string BARCODE = "barcode";

        public static int PROCESS_STATUS_PROCESSING_PENDING = 0;
        public static int PROCESS_STATUS_PROCESSED = 1;
        public static int PROCESS_STATUS_NO_BARCODES_IN_PDF = -2;


        const string ID_ALIAS = "irisndt_upload_id";

        public bool save_irisndt_upload_data(SortedList sl)
        {
            bool bret = true;

            try
            {
                save(sl, m_tbl);
            }
            catch (Exception ex)
            {
                bret = false;
                EventLog.WriteEntry("PCF gbe", "save_irisndt_upload_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return bret;
        }

        public ArrayList get_upload_data(string flds, SortedList search_params, int top=0)
        {
            flds = flds.Replace(ID, $"{m_tbl}.{ID} as {ID_ALIAS}");

            string s_top = string.Empty;

            if(top > 0)
                s_top = $" top({top}) ";

            string select = $"select {s_top} {flds} from {m_tbl} " ;

            if (flds.Contains(BARCODE))
            {
                select += @" left join weld_test_reports on weld_test_reports.irisndt_upload_id = [irisndt_upload].[id]
                            left join spools on spools.id = weld_test_reports.spool_id ";
            }

            if (search_params != null)
            {
                string where = string.Empty;

                foreach (DictionaryEntry e0 in search_params)
                {
                    if(where.Trim().Length > 0)
                        where += " and ";

                    where += $" {e0.Key}=@{e0.Key} ";
                }

                if(where.Trim().Length > 0)
                    select += $" where {where} "; 
            }

            select += $" order by dt desc";

            return get_irisndt_upload_data(select, search_params);
        }

        ArrayList get_irisndt_upload_data(string select, SortedList search_params)
        {
            ArrayList a = new ArrayList();

            try
            {
                DataTable dta = get_data_p(select, search_params);

                foreach (DataRow dr in dta.Rows)
                {
                    irisndt_upload_data ld = null;

                    int id = (int)dr[ID_ALIAS];

                    foreach (irisndt_upload_data ld0 in a)
                    {
                        if (ld0.id == id)
                        {
                            ld = ld0;
                            break;
                        }
                    }

                    if (ld == null)
                    {
                        ld = new irisndt_upload_data();
                        try{ld.id = (int)dr[ID_ALIAS];}catch{}
                        try{ld.dt = (DateTime)dr[DT];}catch{}
                        try{ld.filename = dr[FILENAME].ToString();}catch{}
                        try { ld.file_content = ( byte [])dr[FILE_CONTENT]; }catch{}
                        try { ld.processed = (int)dr[PROCESSED]; }catch{}

                        a.Add(ld);
                    }

                    try {ld.barcodes.Add(dr[BARCODE].ToString());}catch{}
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "get_irisndt_upload_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public void delete_upload(int id)
        {
            SortedList sl = new SortedList();
            sl.Add(ID, id);
            delete_record(m_tbl, sl);
        }
    }

    [Serializable]
    public class irisndt_upload_data
    {
        public int id = 0;
        public DateTime dt = DateTime.MinValue;
        public string filename = string.Empty;
        public byte [] file_content = null;
        public int processed = 0;
        public ArrayList barcodes = new ArrayList();
    }
}