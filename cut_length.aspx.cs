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
    public partial class cut_length : System.Web.UI.Page
    {
        const string VS_CL_DATA = "VS_CL_DATA";
        const string VS_SEQ = "VS_SEQ";
        const string BARCODE = "barcode";
        const string ROW_IDX = "row_idx";
        const string SEQ = "seq";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                get_data();

                string user_id = System.Web.HttpContext.Current.User.Identity.Name;

                using (users u = new users())
                {
                    user_data ud = u.get_user_data(user_id);

                    if (ud.role != "ADMIN")
                    {
                        chkCutterView.Checked = true;
                        chkCutterView.Visible = false;
                    }
                }
            }

            display();
        }

        void get_data()
        {
            try
            {
                DateTime dt_from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                DateTime dt_to = DateTime.MaxValue;

                // hs. 20231121. get from holding area instead
                dt_from = schedule_fab_data.DT_HOLDING_RECS_DATE;
                dt_to = dt_from.AddHours(1);

                dt_from = dt_from.AddMonths(-1); 

                SortedList sl_params = new SortedList();
                sl_params.Add("@dt_from", dt_from);
                sl_params.Add("@dt_to", dt_to);

                string sql = $@"select 
                                spools.id
                                , barcode
                                , spool_parts.qty
                                , spool_parts.seq
                                , spool_parts.id as spool_parts_id
                                , parts.description
                                , parts.part_type
                                , parts.fitting_size_mm
                                , parts.gap_mm
                            
                                from schedule_fab
                                join spools on spools.id = schedule_fab.spool_id
                                join spool_parts on spool_parts.spool_id = spools.id
                                join parts on parts.id = spool_parts.part_id

                                where 
                                (status = 'SC' or status = 'RP')
                                and (dt >= @dt_from and  dt <= @dt_to)

                                order by schedule_fab.seq, schedule_fab.dt, schedule_fab.batch_number
                            ";

                Dictionary<string, List<CCutLengthData>> dict_cld = new Dictionary<string, List<CCutLengthData>>();

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
                        cld.part_type = dr["part_type"].ToString();
                        try { cld.fitting_size_mm = (decimal)dr["fitting_size_mm"]; } catch { }
                        try { cld.gap_mm = (decimal)dr["gap_mm"]; } catch { }
                        try { cld.spool_part_seq = (int)dr["seq"]; } catch { }
                        try { cld.spool_parts_id = (int)dr["spool_parts_id"]; } catch { }

                        string key = cld.barcode;

                        List<CCutLengthData> lst_cld = null;

                        if (dict_cld.ContainsKey(key))
                        {
                            lst_cld = dict_cld[key];
                        }
                        else
                        {
                            lst_cld = new List<CCutLengthData>();
                            dict_cld.Add(key, lst_cld);
                            iseq = 0;
                        }

                        iseq++;

                        if(cld.spool_part_seq == 0)
                            cld.spool_part_seq = iseq;

                        lst_cld.Add(cld);
                    }
                }

                ViewState[VS_CL_DATA] = dict_cld;
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

            Dictionary<string, List<CCutLengthData>> dict_cld = (Dictionary<string, List<CCutLengthData>>)ViewState[VS_CL_DATA];

            if (dict_cld != null)
            {
                foreach (KeyValuePair<string, List<CCutLengthData>> kvp in dict_cld )
                {
                    string barcode = kvp.Key;
                    List<CCutLengthData> lst_cld = kvp.Value;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(barcode));
                    r.Cells.Add(c);

                    tblMain.Rows.Add(r);

                    for (int i = 0; i < lst_cld.Count; i++)
                    {
                        CCutLengthData cld = lst_cld[i];

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        string part = string.Empty; 

                        if (!chkCutterView.Checked)
                        {
                            part += cld.spool_part_seq.ToString("00") + ": ";
                        }

                        part += cld.part_description;
                           
                        if ((cld.is_pipe))
                        {
                            r.BackColor = System.Drawing.Color.FromName("Bisque");

                            c = new TableCell();
                            c.Controls.Add(new LiteralControl(part));
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
                            c.Controls.Add(new LiteralControl("Cut (mm): "+ get_cut_length(barcode, cld.spool_part_seq).ToString("0.00")) );
                            r.Cells.Add(c);
                        }
                        else
                        {
                            if (!chkCutterView.Checked)
                            {
                                c = new TableCell();
                                c.Controls.Add(new LiteralControl(part));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl("Fitting size: " + (cld.fitting_size_mm).ToString("0.00")));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl("Gap: " + (cld.gap_mm).ToString("0.00")));
                                r.Cells.Add(c);

                                c = new TableCell();
                                c.HorizontalAlign = HorizontalAlign.Right;
                                c.Controls.Add(new LiteralControl(string.Empty));
                                r.Cells.Add(c);
                            }
                        }

                        if (!chkCutterView.Checked)
                        {
                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            ImageButton btn_move_up = new ImageButton();
                            btn_move_up.ToolTip = "Move up";
                            btn_move_up.ImageUrl = "~/up.png";
                            btn_move_up.ID = "btn_move_up_" + barcode + cld.spool_part_seq.ToString() + cld.spool_parts_id.ToString();
                            btn_move_up.Attributes[BARCODE] = barcode;
                            btn_move_up.Attributes[SEQ] = cld.spool_part_seq.ToString();
                            btn_move_up.Click += btn_move_up_Click;
                            c.Controls.Add(btn_move_up);
                            r.Cells.Add(c);

                            c = new TableCell();
                            c.HorizontalAlign = HorizontalAlign.Center;
                            ImageButton btn_move_dn = new ImageButton();
                            btn_move_dn.ToolTip = "Move down";
                            btn_move_dn.ImageUrl = "~/down.png";
                            btn_move_dn.ID = "btn_move_dn_" + barcode + cld.spool_part_seq.ToString() + cld.spool_parts_id.ToString();
                            btn_move_dn.Attributes[BARCODE] = barcode;
                            btn_move_dn.Attributes[SEQ] = cld.spool_part_seq.ToString();
                            btn_move_dn.Click += btn_move_dn_Click;
                            c.Controls.Add(btn_move_dn);
                            r.Cells.Add(c);
                        }

                        tblMain.Rows.Add(r);
                    }

                    r = new TableRow();
                    r.Height = 10;
                    tblMain.Rows.Add(r);

                }
            }
        }

        void move_part(object sender, int direction)
        {
            ImageButton btn = (ImageButton)sender;
            string barcode = btn.Attributes[BARCODE];
            int seq = Convert.ToInt32(btn.Attributes[SEQ]);

            Dictionary<string, List<CCutLengthData>> dict_cld = (Dictionary<string, List<CCutLengthData>>)ViewState[VS_CL_DATA];

            if (dict_cld != null)
            {
                if (dict_cld.ContainsKey(barcode))
                {
                    List<CCutLengthData> lst_cld = dict_cld[barcode];
                    
                    int i = 0;
                    CCutLengthData cld = null;

                    for (; i < lst_cld.Count; i++)
                    {
                        cld = lst_cld[i];

                        if(seq == cld.spool_part_seq)
                            break;
                    }

                    bool bmove = false;

                    if (direction == 1) // down
                    {
                        if (i < lst_cld.Count - 1)
                        {
                            bmove = true;
                        }
                    }
                    else if (direction == -1) // up
                    {
                        if (i > 0)
                        {
                            bmove = true;
                        }
                    }

                    if (bmove)
                    {
                        lst_cld.RemoveAt(i);
                        lst_cld.Insert(i + direction, cld);
                        ViewState[VS_CL_DATA] = dict_cld;
                        display();
                    }
                }
            }
        }

        private void btn_move_dn_Click(object sender, ImageClickEventArgs e)
        {
            move_part( sender, 1);
        }

        private void btn_move_up_Click(object sender, ImageClickEventArgs e)
        {
            move_part( sender, -1);
        }

        TableRow populate_pipe_row(TableRow r, CCutLengthData cld)
        {
            return r;

        }

        decimal get_cut_length(string barcode, int seq)
        {
            decimal cut_length = 0M;

            Dictionary<string, List<CCutLengthData>> dict_cld = (Dictionary<string, List<CCutLengthData>>)ViewState[VS_CL_DATA];

            if (dict_cld != null)
            {
                if (dict_cld.ContainsKey(barcode))
                {
                    List<CCutLengthData> lst_cld = dict_cld[barcode];

                    int i = 0;
                    CCutLengthData cld = null;

                    for (; i < lst_cld.Count; i++)
                    {
                        cld = lst_cld[i];

                        if (seq == cld.spool_part_seq)
                            break;
                    }

                    cut_length = cld.length;

                    if (i > 0)
                    {
                        CCutLengthData cld_above = lst_cld[i-1];
                        cut_length -= cld_above.fitting_size_mm;
                        cut_length -= cld_above.gap_mm;

                    }

                    if (i < lst_cld.Count-1)
                    {
                        CCutLengthData cld_below = lst_cld[i+1];
                        cut_length -= cld_below.fitting_size_mm;
                        cut_length -= cld_below.gap_mm;
                    }
                }
            }

            return cut_length;
        }

        [Serializable]
        class CCutLengthData
        {
            public int spool_id { get; set; }
            public string barcode { get; set; }
            public decimal length { get; set; }
            public string part_description { get; set; }
            public string part_type { get; set; }
            public decimal fitting_size_mm { get; set; }
            public decimal gap_mm { get; set; }
            public int spool_part_seq { get; set; }
            public int spool_parts_id { get; set; }

            public string label { get { return spool_part_seq.ToString("00") + ": " + part_description; } }

            static string PIPE = "PIPE";
            public bool is_pipe{get {return (part_description.ToUpper().Contains(PIPE)|| part_type.ToUpper().Contains(PIPE));} }
        }

        protected void chkCutterView_CheckedChanged(object sender, EventArgs e)
        {
            display();
        }
    }
}