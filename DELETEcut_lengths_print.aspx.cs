using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gbe
{
    public partial class cut_length_print : System.Web.UI.Page
    {
        Dictionary<string, List<string>>  dict_cld = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            get_data();
            display();
        }

        void get_data()
        {
            dict_cld = new Dictionary<string, List<string>>();

            List<string> lst = new List<string>();

            dict_cld.Add("G00390-MSMA-NC-35-007-SP-001_C1" ,lst);
            lst.Add("250mm PIPE|300.00");
            lst.Add("250mm PIPE|700.00");
            lst.Add("250mm PIPE|4300.00");
            
            lst = new List<string>();

            dict_cld.Add("G00390-MSMA-NC-35-007-SP-004_C1" ,lst);
            lst.Add("250mm PIPE|700.00");
            lst.Add("250mm PIPE|400.00");
            lst.Add("250mm PIPE|4200.00");

            lst = new List<string>();
            dict_cld.Add("G00390-MSMA-NC-35-007-SP-002_C1" ,lst);
            lst.Add("150mm PIPE|600.00");
            lst.Add("150mm PIPE|800.00");
            lst.Add("150mm PIPE|4200.00");

            lst = new List<string>();
            dict_cld.Add("G00390-MSMA-NC-35-007-SP-003_C1" ,lst);
            lst.Add("150mm PIPE|600.00");
            lst.Add("150mm PIPE|700.00");
            lst.Add("150mm PIPE|4300.00");
        }

        void display()
        {
            tblMain.Rows.Clear();

            TableRow r;
            TableCell c;

            if (dict_cld != null)
            {
                foreach (KeyValuePair<string, List<string>> kvp in dict_cld)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName("LightGray");

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(kvp.Key));
                    r.Cells.Add(c);

                    tblMain.Rows.Add(r);

                    foreach (string part_len in kvp.Value)
                    {
                        string [] sa = part_len.Split('|');

                        string part = sa[0];
                        string len =  sa[1];

                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");

                        c = new TableCell();
                        c.Controls.Add(new LiteralControl(part));
                        r.Cells.Add(c);

                        c = new TableCell();
                        c.HorizontalAlign = HorizontalAlign.Right;
                        c.Controls.Add(new LiteralControl(len));
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_cut_2 = new TextBox();
                        c.Controls.Add(txt_cut_2);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_cut_3 = new TextBox();
                        c.Controls.Add(txt_cut_3);
                        r.Cells.Add(c);

                        c = new TableCell();
                        TextBox txt_cut_4 = new TextBox();
                        c.Controls.Add(txt_cut_4);
                        r.Cells.Add(c);

                        tblMain.Rows.Add(r);
                    }
                }
            }
        }
    }
}