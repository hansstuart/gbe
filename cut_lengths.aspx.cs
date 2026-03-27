using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Text;
using System.IO;

namespace gbe
{
    public partial class cut_lengths : System.Web.UI.Page
    {
        const string BARCODE = "barcode";
        const string CUTLENGTH = "cut_length";
        const string ROW_TYPE = "row_type";
        const string PART = "part";
        const string BATCHNO = "batch_no";
        const string CHK_ADD_TO_PRINT = "chk_add_to_print_list_";
        const string VS_CUT_DATA = "vs_cut_data";
        const int NUM_OF_ADDITIONAL_CUTS = 3;

        Dictionary<string, List<CCutLengthData>>  m_dict_cld = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtSearch.Focus();

            imgAddToPrint.Visible = false;

            if (IsPostBack)
            {
                m_dict_cld = (Dictionary<string, List<CCutLengthData>>) ViewState[VS_CUT_DATA];

                display_cut_data();
                populate_print_table();
            }
        }

        void get_cut_data()
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


                string spool_search = string.Empty;
 
                if (txtSearch.Text.Trim().Length > 2)
                {
                    spool_search = $"barcode like '{txtSearch.Text.Trim()}%' and ";
                }
 
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
                                , additional_cut1
                                , additional_cut2
                                , additional_cut3
                                , additional_info

	                            , f1.description as f1_part_description
                                , f1.fitting_size_mm as f1_fitting_size_mm
                                , f1.gap_mm  as f1_gap_mm

	                            , f2.description as f2_part_description
                                , f2.fitting_size_mm as f2_fitting_size_mm
                                , f2.gap_mm as f2_gap_mm
                                , batch_number
                            
                                from schedule_fab

                                join spools on spools.id = schedule_fab.spool_id
                                join spool_parts on spool_parts.spool_id = spools.id
                                join parts on parts.id = spool_parts.part_id
                                left join spool_pipe_fittings on spool_pipe_fittings.spool_part_id  = spool_parts.id
                                left join parts as f1 on f1.id = fitting_1_part_id
                                left join parts as f2 on f2.id = fitting_2_part_id

                                where 
                                {spool_search}
                                (status = 'SC' or status = 'RP')
                                and (dt >= @dt_from and  dt <= @dt_to)
                                and (parts.description like '%pipe%' or parts.part_type like '%pipe%')

                                order by schedule_fab.seq, schedule_fab.dt, schedule_fab.batch_number, barcode
                                
                            ";

                 m_dict_cld = new Dictionary<string, List<CCutLengthData>>();

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

                        /*
                        try { cld.additional_cut1 = (decimal)dr["additional_cut1"]; } catch { }
                        try { cld.additional_cut2 = (decimal)dr["additional_cut2"]; } catch { }
                        try { cld.additional_cut3 = (decimal)dr["additional_cut3"]; } catch { }
                        */

                        cld.batch_number = dr["batch_number"].ToString();

                        cld.additional_info = dr["additional_info"].ToString();

                        string key = cld.barcode;

                        List<CCutLengthData> lst_cld = null;

                        if ( m_dict_cld.ContainsKey(key))
                        {
                            lst_cld =  m_dict_cld[key];
                        }
                        else
                        {
                            lst_cld = new List<CCutLengthData>();
                            m_dict_cld.Add(key, lst_cld);
                            iseq = 0;
                        }

                        iseq++;

                        lst_cld.Add(cld);
                    }
                }

                ViewState[VS_CUT_DATA] = m_dict_cld;
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message.ToString();
            }
        }

        void display_cut_data()
        {
            tblMain.Rows.Clear();

            TableRow r;
            TableCell c;

            if (m_dict_cld != null)
            {
                if(m_dict_cld.Count > 0)
                    imgAddToPrint.Visible = true;

                foreach (KeyValuePair<string, List<CCutLengthData>> kvp in m_dict_cld)
                {
                    string barcode = kvp.Key;
                    List<CCutLengthData> lst_cld = kvp.Value;

                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(barcode));
                    r.Cells.Add(c);

                    CheckBox chk_add_to_print_list = new CheckBox();

                    chk_add_to_print_list.ID = CHK_ADD_TO_PRINT + barcode;
                    chk_add_to_print_list.Attributes[BARCODE] = barcode;

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Right;
                    c.Controls.Add(chk_add_to_print_list);
                    r.Cells.Add(c);

                    tblMain.Rows.Add(r);

                    if (lst_cld.Count > 0)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("LightGray");
                        c = new TableCell();
                        c.Controls.Add(new LiteralControl($"Batch no. {lst_cld[0].batch_number}"));
                        r.Cells.Add(c);

                        tblMain.Rows.Add(r);
                    }

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

        protected void imgAddToPrint_Click(object sender, ImageClickEventArgs e)
        {
            populate_print_table();

            if(tblPrint.Rows.Count > 0)
                MultiView1.ActiveViewIndex = 1;
        }

        void populate_print_table()
        {
            tblPrint.Rows.Clear();

            foreach (TableRow r in tblMain.Rows)
            {
                foreach (TableCell cell in r.Cells)
                {
                    foreach (Control cntrl in cell.Controls)
                    {
                        if (cntrl.ID != null)
                        {
                            if (cntrl.GetType() == typeof(CheckBox))
                            {
                                CheckBox chk = (CheckBox)cntrl;

                                if (chk.ID.StartsWith(CHK_ADD_TO_PRINT))
                                {
                                    if (chk.Checked)
                                    {
                                        string barcode = chk.Attributes[BARCODE];

                                        if (m_dict_cld.ContainsKey(barcode))
                                        {
                                            TableRow row = new TableRow();
                                            row.Attributes[ROW_TYPE] = BARCODE;
                                            row.Attributes[BARCODE] = barcode;
                                            row.BackColor = System.Drawing.Color.FromName("LightGray");

                                            TableCell c = new TableCell();
                                            c.Controls.Add(new LiteralControl(barcode));
                                            row.Cells.Add(c);

                                            string batch_number = string.Empty;

                                            if (m_dict_cld[barcode].Count > 0)
                                            {
                                                batch_number = m_dict_cld[barcode][0].batch_number.Trim();

                                                row.Attributes[BATCHNO] = batch_number;
                                            }

                                            tblPrint.Rows.Add(row);

                                            if (batch_number.Length > 0)
                                            {
                                                row = new TableRow();
                                                row.Attributes[ROW_TYPE] = BATCHNO;
                                                row.Attributes[BATCHNO] = batch_number;
                                                row.BackColor = System.Drawing.Color.FromName("LightGray");

                                                c = new TableCell();
                                                c.Controls.Add(new LiteralControl($"Batch no. {batch_number}"));
                                                row.Cells.Add(c);

                                                tblPrint.Rows.Add(row);
                                            }
                                            
                                            foreach (CCutLengthData cld in m_dict_cld[barcode])
                                            {
                                                string cut_length = CCutLengthData.get_cut_length(cld).ToString("0.00");

                                                row = new TableRow();
                                                row.BackColor = System.Drawing.Color.FromName("White");
                                                row.Attributes[ROW_TYPE] = CUTLENGTH;
                                                row.Attributes[PART] = cld.part_description;
                                                row.Attributes[CUTLENGTH] = cut_length;

                                                c = new TableCell();
                                                c.Controls.Add(new LiteralControl(cld.part_description));
                                                row.Cells.Add(c);

                                                c = new TableCell();
                                                c.HorizontalAlign = HorizontalAlign.Right;
                                                c.Controls.Add(new LiteralControl(cut_length));
                                                row.Cells.Add(c);

                                                /*
                                                List<decimal> additional_cuts = new List<decimal>();
                                                additional_cuts.Add(cld.additional_cut1);
                                                additional_cuts.Add(cld.additional_cut2);
                                                additional_cuts.Add(cld.additional_cut3);

                                                for (int i = 0; i < NUM_OF_ADDITIONAL_CUTS; i++)
                                                {
                                                    c = new TableCell();
                                                    TextBox txt_cut = new TextBox();
                                                    c.Controls.Add(txt_cut);
                                                    row.Cells.Add(c);

                                                    decimal additional_cut = additional_cuts[i];

                                                    if(additional_cut > 0)
                                                        txt_cut.Text = additional_cut.ToString("0.00");
                                                }
                                                */

                                                c = new TableCell();
                                                TextBox txt_info = new TextBox();
                                                c.Controls.Add(txt_info);
                                                row.Cells.Add(c);

                                                txt_info.Text = cld.additional_info;

                                                tblPrint.Rows.Add(row);
                                            }

                                            row = new TableRow();
                                            row.Height = 5;
                                            tblPrint.Rows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void imgPrint_Click(object sender, ImageClickEventArgs e)
        {
            //if (valid_cut_lengths())
            //{
                List<KeyValuePair<string, List<KeyValuePair<string, List<string>>>>> lst_barcode_parts_lengths = store_print_data();

                string batch_number = get_batch_number();

                string customer_name = get_customer_name();

                create_cut_length_pdf(lst_barcode_parts_lengths, batch_number, customer_name);
            //}
        }

        string get_batch_number()
        {
            string batch_number = string.Empty;

            foreach (TableRow r in tblPrint.Rows)
            {
                if (r.Attributes[ROW_TYPE] == BARCODE)
                {
                    batch_number = r.Attributes[BATCHNO];
                    break;
                }
            }
            return batch_number;
        }

        string get_customer_name()
        {
            string customer_name = string.Empty;

            string barcode = string.Empty;

            foreach (TableRow r in tblPrint.Rows)
            {
                if (r.Attributes[ROW_TYPE] == BARCODE)
                {
                    barcode = r.Attributes[BARCODE];
                    break;
                }
            }

            if (barcode.Length > 0)
            {
                string[] sa = barcode.Split('-');

                if (sa.Length > 2)
                {
                    using(customers c = new customers())
                    {
                        SortedList sl = new SortedList();
                        sl.Add("contract_number", sa[0]);

                        ArrayList a = c.get_customer_data(sl);

                        if (a.Count > 0)
                        {
                            customer_data cd = (customer_data)a[0];
                            customer_name = cd.name;
                        }
                    }
                }
            }

            return customer_name;
        }

        List<KeyValuePair<string, List<KeyValuePair<string, List<string>>>>> store_print_data()
        {
            List<KeyValuePair<string, List<KeyValuePair<string, List<string>>>>> lst_barcode_parts_lengths = new List<KeyValuePair<string, List<KeyValuePair<string, List<string>>>>>();
            List<KeyValuePair<string, List<string>>> lst_parts_lengths = null;
            List<string> lst_cut_lengths = null;
            
            foreach (TableRow r in tblPrint.Rows)
            {
                if (r.Attributes[ROW_TYPE] == BARCODE)
                {
                    lst_parts_lengths = new List<KeyValuePair<string, List<string>>>();
                    lst_barcode_parts_lengths.Add(new KeyValuePair<string, List<KeyValuePair<string, List<string>>>>(r.Attributes[BARCODE], lst_parts_lengths));
                    continue;
                }
                else if (r.Attributes[ROW_TYPE] == CUTLENGTH)
                {
                    lst_cut_lengths = new List<string>();
                    lst_parts_lengths.Add(new KeyValuePair<string, List<string>>(r.Attributes[PART], lst_cut_lengths));
                    lst_cut_lengths.Add(r.Attributes[CUTLENGTH]);

                    foreach (TableCell cell in r.Cells)
                    {
                        foreach (Control cntrl in cell.Controls)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                TextBox txt_box = (TextBox)cntrl;
                                lst_cut_lengths.Add(txt_box.Text.Trim());
                            }
                        }
                    }
                }
            }

           return lst_barcode_parts_lengths;
        }

        bool valid_cut_lengths()
        {
            bool bret = true;

            foreach (TableRow r in tblPrint.Rows)
            {
                if (r.Attributes[ROW_TYPE] == CUTLENGTH)
                {
                    foreach (TableCell cell in r.Cells)
                    {
                        foreach (Control cntrl in cell.Controls)
                        {
                            if (cntrl.GetType() == typeof(TextBox))
                            {
                                TextBox txt_box = (TextBox)cntrl;
                                txt_box.BackColor = System.Drawing.Color.White;

                                string cut_len = txt_box.Text.Trim();

                                if (cut_len.Length > 0)
                                {
                                    decimal dlen;

                                    bool bvalid = decimal.TryParse(cut_len, out dlen);

                                    if (!bvalid)
                                    {
                                        txt_box.BackColor = System.Drawing.Color.Red;
                                        bret = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return bret;
        }

        void create_cut_length_pdf(List<KeyValuePair<string, List<KeyValuePair<string, List<string>>>>> lst_barcode_parts_lengths
            , string batch_number, string customer_name)
        {
            const string ENDL = "\r\n";
            const string FF = "\f";
            const int LPP = 20;

            StringBuilder data_file = new StringBuilder();

            string username = System.Web.HttpContext.Current.User.Identity.Name.ToUpper();

            string hdr = username + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ENDL + batch_number + ENDL + customer_name + ENDL + ENDL + ENDL;

            int line_cnt = 0;

            data_file.Append(hdr);

            foreach (KeyValuePair<string, List<KeyValuePair<string, List<string>>>> kvp1 in lst_barcode_parts_lengths)
            {
                string barcode = kvp1.Key;

                int idx_barcode = 0;
                foreach (KeyValuePair<string, List<string>> kvp2 in kvp1.Value)
                {
                    if (idx_barcode == 0)
                    {
                        line_cnt++; // allow for barcode line in advance
                    }

                    if (line_cnt >= LPP)
                    {
                        if (idx_barcode == 0)
                            line_cnt = 1;
                        else
                            line_cnt = 0;

                        data_file.Append(FF);
                        data_file.Append(hdr);
                    }

                    if (idx_barcode == 0)
                    {
                        data_file.Append(barcode + ENDL);
                    }

                    idx_barcode++;

                    data_file.Append(kvp2.Key.PadRight(50).Substring(0,50));

                    foreach (string cut_length in kvp2.Value)
                    {
                        /*
                        decimal dcut_length = 0;
                        string cut_length_out = string.Empty;

                        if (cut_length.Trim().Length > 0)
                        {
                            try {dcut_length = Convert.ToDecimal(cut_length); } catch { }
                        }

                        if(dcut_length > 0)
                            cut_length_out = dcut_length.ToString("0.00");

                        data_file.Append(cut_length_out.PadLeft(12));
                        */

                        data_file.Append(cut_length.PadRight(12));
                    }

                    data_file.Append(ENDL);
                    line_cnt++;
                }
            }

            string psFile = Server.MapPath("temp");
            DateTime dt = DateTime.Now;
            psFile += "\\";
            psFile += dt.ToString("yyyyMMddHHmmssfff") + "_cutlist";
            string pdfFile = psFile + ".pdf";
            psFile += ".ps";

            string FORMS_DIR = "forms_dir";
            string forms_dir = string.Empty;

            try { forms_dir = System.Web.Configuration.WebConfigurationManager.AppSettings[FORMS_DIR].ToString(); }
            catch { forms_dir = Server.MapPath("forms"); }

            string po_form = forms_dir + "\\gbe_cut_list";

            File.Copy(po_form, psFile);

            FileStream fs = new FileStream(psFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.ASCIIEncoding.Default);

            sw.Write(data_file.ToString());

            sw.Close();
                        
            string gsbin = System.Web.Configuration.WebConfigurationManager.AppSettings["gsbin"].ToString();

            PCFUtil util = new PCFUtil();
            int ret = util.ps_convert(gsbin, psFile, pdfFile, "pdfwrite", true, "plop", 0);

            byte[] pdf = null;

            try { pdf = File.ReadAllBytes(pdfFile); }
            catch { }

            try { File.Delete(psFile); }
            catch { }

            try { File.Delete(pdfFile); }
            catch { }

            if (pdf != null && pdf.Length > 0)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Type", "application/pdf");
            }

            Response.AddHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(pdfFile));

            Response.Buffer = true;

            if (pdf != null && pdf.Length > 0)
                Response.BinaryWrite(pdf);

            Response.Flush();

            try { Response.End(); }
            catch { }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            get_cut_data();
            display_cut_data();
        }
    }

    [Serializable]
    public class CCutLengthDataOLD
    {
        public int spool_id { get; set; }
        public string barcode { get; set; }
        public decimal length { get; set; }
        public string part_description { get; set; }
        
        public string batch_number { get; set; }

        public int spool_pipe_fittings_id { get; set; }
        public int spool_parts_id { get; set; }
        public string f1_part_description { get; set; }
        public decimal f1_fitting_size_mm { get; set; }
        public decimal f1_gap_mm { get; set; }
            
        public string f2_part_description { get; set; }
        public decimal f2_fitting_size_mm { get; set; }
        public decimal f2_gap_mm { get; set; }

        /*
        public decimal additional_cut1{ get; set; }
        public decimal additional_cut2{ get; set; }
        public decimal additional_cut3{ get; set; }
        */

        public string additional_info{ get; set; }

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