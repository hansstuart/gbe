using System;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace gbe
{
    public class irisndt_download
    {
        public ArrayList get_download_data()
        {
            ArrayList a = new ArrayList();

            try
            {
                string select = $@" select * 
                            FROM weld_test_ext

                            where                          
                            sent_to_iris =  1
                            and
                            pass = 1
                            order by datetime_stamp desc";

                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            weld_test_ext_data wted = new weld_test_ext_data();
                            wted.init(dr);

                            if(!a.Contains(wted.report1MPI_FW))
                                a.Add(wted.report1MPI_FW);

                            if(!a.Contains(wted.report2MPI_BW))
                                a.Add(wted.report2MPI_BW);

                            if(!a.Contains(wted.report3UT_BW))
                                a.Add(wted.report3UT_BW);

                            if(!a.Contains(wted.report4XRAY_BW))
                                a.Add(wted.report4XRAY_BW);

                            if(!a.Contains(wted.report5DP_FW))
                                a.Add(wted.report5DP_FW);

                            if(!a.Contains(wted.report6DP_BW))
                                a.Add(wted.report6DP_BW);

                            if(!a.Contains(wted.report7VI_FW))
                                a.Add(wted.report7VI_FW);

                            if(!a.Contains(wted.report8VI_BW))
                                a.Add(wted.report8VI_BW);

                            if(!a.Contains(wted.report9PA_BW))
                                a.Add(wted.report9PA_BW);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "irisndt_download::get_download_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }

        public ArrayList get_weld_test_save_data(string cli_ref)
        {
            string DC = "\"";
            string CM = ",";

            ArrayList a = new ArrayList();

            try 
            {
                string select = $@" select 
                                    spool 
                                    ,revision
                                    ,seq
                                    ,parts.description
                                    ,spool_parts.welder spool_parts_welder
                                    ,qty
                                    ,fw
                                    ,bw
                                    
                                    FROM weld_test_ext
  
                                    join spools on weld_test_ext.spool_id = spools.id
                                    join spool_parts on weld_test_ext.spool_id = spool_parts.spool_id
                                    join parts on spool_parts.part_id = parts.id

                                    where 
                                    (
                                    report1MPI_FW = '{cli_ref}'
                                    or
                                    report2MPI_BW = '{cli_ref}'
                                    or
                                    report3UT_BW = '{cli_ref}'
                                    or
                                    report4XRAY_BW = '{cli_ref}'
                                    or
                                    report5DP_FW = '{cli_ref}'
                                    or
                                    report6DP_BW = '{cli_ref}'
                                    or
                                    report7VI_FW = '{cli_ref}'
                                    or
                                    report8VI_BW = '{cli_ref}'
                                    or
                                    report9PA_BW = '{cli_ref}'
                                ) 
                                and
                                sent_to_iris =  1
                                and
                                pass = 1
                                and
                                spool_parts.include_in_weld_map=1
                                order by barcode ";

                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        string[] hdr = new string[] { "SPOOL", "REVISION", "ITEM", "DESCRIPTION", "WELDER", "QTY", "FW", "BW" };

                        string shdr = string.Empty;

                        foreach (string s in hdr)
                        {
                            if (shdr.Length > 0)
                                shdr += CM;

                            shdr += DC + s + DC;
                        }

                        a.Add(shdr);

                        foreach (DataRow dr in dtab.Rows)
                        {
                            fy_weld_test_v2.data_row_spool spool = new fy_weld_test_v2.data_row_spool(dr);
                            fy_weld_test_v2.spool_part spool_part = new fy_weld_test_v2.spool_part(dr);
                                 
                            string sline = DC + spool.s_gf("spool") + DC;
                            sline += CM;
                            sline += DC + spool.s_gf("revision") + DC;
                            sline += CM;
                            sline += DC + spool_part.seq + DC;
                            sline += CM;
                            sline += DC + spool_part.part_description + DC;
                            sline += CM;
                            sline += DC + spool_part.spool_parts_welder + DC;
                            sline += CM;
                            sline += DC + spool_part.qty + DC;
                            sline += CM;
                            sline += DC + spool_part.fw+ DC;
                            sline += CM;
                            sline += DC + spool_part.bw+ DC;

                            a.Add(sline);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("PCF gbe", "irisndt_download::save_weld_test_data() \n" + ex.ToString(), EventLogEntryType.Error);
            }

            return a;
        }
    }
}