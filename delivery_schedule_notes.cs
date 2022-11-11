using System;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Data.SqlClient;

namespace gbe
{
    public class delivery_schedule_notes : cdb_connection
    {
        string m_tbl = "delivery_schedule_notes";

        public void save_delivery_schedule_notes_data(SortedList sl)
        {
            save(sl, m_tbl);
        }
    }

    [Serializable]
    public class delivery_schedule_notes_data
    {
        public int id = 0;
        public string note_key = string.Empty;
        public string note = string.Empty;
        public DateTime dt = DateTime.MinValue;
    }
}
