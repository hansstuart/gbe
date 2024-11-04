using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

namespace gbe
{
    public partial class cut_lengths : System.Web.UI.Page
    {
        Dictionary<string, List<CCutLengthData>>  dict_cld = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            get_data();

            display();
        }

        void get_data()
        {
            try
            {
                DateTime dt_from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                DateTime dt_to = DateTime.MaxValue;

                dt_from = schedule_fab_data.DT_HOLDING_RECS_DATE;
                dt_to = dt_from.AddHours(1);

                SortedList sl_params = new SortedList();
                sl_params.Add("@dt_from", dt_from);
                sl_params.Add("@dt_to", dt_to);

                string sql = $@"select 
                                spools.id
                                , barcode
                                , spool_parts.qty 
                                , spool_parts.seq
                                , spool_parts.completed
                                , spool_parts.id as spool_parts_id
                                , parts.description
                                , parts.part_type

                                , spool_pipe_fittings_id
                                
	                            , fitting_1_part_id
	                            , fitting_2_part_id

	                            , f1.description as f1_part_description
                                , f1.fitting_size_mm as f1_fitting_size_mm
                                , f1.gap_mm  as f1_gap_mm

	                            , f2.description as f2_part_description
                                , f2.fitting_size_mm as f2_fitting_size_mm
                                , f2.gap_mm as f2_gap_mm
                            
                                from schedule_fab

                                join spools on spools.id = schedule_fab.spool_id
                                join spool_parts on spool_parts.spool_id = spools.id
                                join parts on parts.id = spool_parts.part_id
                                left join spool_pipe_fittings on spool_pipe_fittings.spool_part_id  = spool_parts.id
                                left join parts as f1 on f1.id = fitting_1_part_id
                                left join parts as f2 on f2.id = fitting_2_part_id

                                where 
                                (status = 'SC' or status = 'RP')
                                and (dt >= @dt_from and  dt <= @dt_to)
                                and (parts.description like '%pipe%' or parts.part_type like '%pipe%')

                                order by schedule_fab.seq, schedule_fab.dt, schedule_fab.batch_number
                            ";

                 dict_cld = new Dictionary<string, List<CCutLengthData>>();

                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data_p(sql, sl_params);
                    
                    int iseq = 0;
                    
                    foreach (DataRow dr in dtab.Rows)
                    {
                        CCutLengthData cld = new CCutLengthData();

                        cld.spool_id = (int)dr["id"];
                        cld.barcode =  dr["barcode"].ToString();
                        try { cld.length = (decimal)dr["qty"]*1000; } catch { }
                        cld.part_description = dr["description"].ToString();

                        try { cld.spool_pipe_fittings_id = (int)dr["spool_pipe_fittings_id"]; } catch { }
                        try { cld.completed = (bool)dr["completed"]; } catch { }

                        cld.f1_part_description = dr["f1_part_description"].ToString();
                        try { cld.f1_fitting_size_mm = (decimal)dr["f1_fitting_size_mm"]; } catch { }
                        try { cld.f1_gap_mm = (decimal)dr["f1_gap_mm"]; } catch { }

                        cld.f2_part_description = dr["f2_part_description"].ToString();
                        try { cld.f2_fitting_size_mm = (decimal)dr["f2_fitting_size_mm"]; } catch { }
                        try { cld.f2_gap_mm = (decimal)dr["f2_gap_mm"]; } catch { }
                        
                        string key = cld.barcode;

                        List<CCutLengthData> lst_cld = null;

                        if ( dict_cld.ContainsKey(key))
                        {
                            lst_cld =  dict_cld[key];
                        }
                        else
                        {
                            lst_cld = new List<CCutLengthData>();
                            dict_cld.Add(key, lst_cld);
                            iseq = 0;
                        }

                        iseq++;

                        lst_cld.Add(cld);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message.ToString();
            }
        }

        void display()
        {
            tblMain.Rows.Clear();

            TableRow r;
            TableCell c;

            if (dict_cld != null)
            {
                foreach (KeyValuePair<string, List<CCutLengthData>> kvp in dict_cld)
                {
                    string barcode = kvp.Key;
                    List<CCutLengthData> lst_cld = kvp.Value;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(barcode));
                    r.Cells.Add(c);

                    tblMain.Rows.Add(r);

                    foreach (CCutLengthData cld in lst_cld)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cld.f1_part_description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Fitting size: " + (cld.f1_fitting_size_mm).ToString("0.00")));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Gap: " + (cld.f1_gap_mm).ToString("0.00")));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        tblMain.Rows.Add(r);

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("Bisque");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cld.part_description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Len (mm): " + cld.length.ToString("0.00")));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.Font.Bold = true;
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Cut (mm): " + CCutLengthData.get_cut_length(cld).ToString("0.00")) );
                        r.Cells.Add(c);

                        tblMain.Rows.Add(r);

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(cld.f2_part_description));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Fitting size: " + (cld.f2_fitting_size_mm).ToString("0.00")));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl("Gap: " + (cld.f2_gap_mm).ToString("0.00")));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(string.Empty));
                        r.Cells.Add(c);

                        tblMain.Rows.Add(r);

                        r = new TableRow();
                        r.Height = 5;
                        tblMain.Rows.Add(r);
                    }

                    r = new TableRow();
                    r.Height = 10;
                    tblMain.Rows.Add(r);
                }
            }
        }

        decimal OLDget_cut_length(CCutLengthData cld)
        {
            decimal cut_length = 0M;

            cut_length = cld.length;

            cut_length -= cld.f1_fitting_size_mm;
            cut_length -= cld.f1_gap_mm;

            cut_length -= cld.f2_fitting_size_mm;
            cut_length -= cld.f2_gap_mm;

            return cut_length;
        }
    }

    [Serializable]
    class CCutLengthData
    {
        public int spool_id { get; set; }
        public string barcode { get; set; }
        public decimal length { get; set; }
        public string part_description { get; set; }
            
        public int spool_pipe_fittings_id { get; set; }
        public int spool_parts_id { get; set; }
        public string f1_part_description { get; set; }
        public decimal f1_fitting_size_mm { get; set; }
        public decimal f1_gap_mm { get; set; }
            
        public string f2_part_description { get; set; }
        public decimal f2_fitting_size_mm { get; set; }
        public decimal f2_gap_mm { get; set; }

        public bool completed { get; set; }
            
        public static decimal get_cut_length(CCutLengthData cld)
        {
            decimal cut_length = 0M;

            cut_length = cld.length;

            cut_length -= cld.f1_fitting_size_mm;
            cut_length -= cld.f1_gap_mm;

            cut_length -= cld.f2_fitting_size_mm;
            cut_length -= cld.f2_gap_mm;

            return cut_length;
        }
    }
}