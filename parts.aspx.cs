using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace gbe
{
    public partial class parts1 : System.Web.UI.Page
    {
        const string VS_DATA = "VS_DATA";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                search_and_display();
            }
            else
            {
                dlMaterial.Items.Add(string.Empty);
                dlPartType.Items.Add(string.Empty);

                using (parts p = new parts())
                {
                    ArrayList a = p.get_imsl_part_types();
                    
                    foreach (string s in a)
                    {
                        if(s.Trim().Length > 0)
                            dlPartType.Items.Add(s);
                    }

                    a = p.get_material();

                    foreach (string s in a)
                    {
                        if(s.Trim().Length > 0)
                            dlMaterial.Items.Add(s);
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dlPartType.Text = dlMaterial.Text = string.Empty;

            search_and_display();
        }

        protected void dlMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;

            search_and_display();
        }

        protected void dlPartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;

            search_and_display();
        }

        void search_and_display()
        {
            ViewState[VS_DATA] = get_part_data();
            display();
        }

        void display()
        {
            List<CPartRec> a_data = null;
            
            tblResults.Rows.Clear();

            a_data = (List<CPartRec>)ViewState[VS_DATA];

            if(a_data == null || a_data.Count == 0)
                return;

            TableRow r;
            TableCell c;

            string[] hdr = new string[] { "Part", "Material", "Type" };

            r = new TableRow();
            r.BackColor = System.Drawing.Color.FromName("LightGreen");

            foreach (string sh in hdr)
            {
                c = new TableCell();
                c.Controls.Add(new LiteralControl(sh));
                r.Cells.Add(c);
            }

            tblResults.Rows.Add(r);

            int i = 0;

            foreach (CPartRec pr in a_data)
            {
                r = new TableRow();

                System.Drawing.Color bc;
                if (i++ % 2 == 0)
                    bc = System.Drawing.Color.FromName("White");
                else
                    bc = System.Drawing.Color.FromName("LightGray");

                r.BackColor = bc;

                c = new TableCell();
                c.Width = 600;
                c.Controls.Add(new LiteralControl(pr.description));
                r.Cells.Add(c);

                c = new TableCell();
                c.Width = 200;
                c.Controls.Add(new LiteralControl(pr.material));
                r.Cells.Add(c);

                c = new TableCell();
                c.Width = 200;
                c.Controls.Add(new LiteralControl(pr.part_type));
                r.Cells.Add(c);

                tblResults.Rows.Add(r);
            }
        }

        List<CPartRec> get_part_data()
        {
            const string DESCRIPTION = "description";
            const string MATERIAL = "material";
            const string PART_TYPE = "part_type";

            var a = new List<CPartRec>();

            string desc, material, part_type;
            desc = material = part_type = string.Empty;

            desc = txtSearch.Text.Trim();
            material = dlMaterial.Text.Trim();
            part_type = dlPartType.Text.Trim();

            if ((desc + material + part_type).Length > 0)
            {
                string select = $"select {DESCRIPTION},{MATERIAL},{PART_TYPE}  from parts where active = 1 and ";

                if (txtSearch.Text.Trim().Length > 1)
                {
                    select += $" {DESCRIPTION} like '{txtSearch.Text.Trim()}%' ";
                }
                else
                {
                    string where = string.Empty;

                    if (dlMaterial.Text.Trim().Length > 0)
                    {
                        where = $" {MATERIAL}='{dlMaterial.Text.Trim()}'";
                    }

                    if (dlPartType.Text.Trim().Length > 0)
                    {
                        if (where.Trim().Length > 0)
                            where += " and ";

                        where += $" {PART_TYPE}='{dlPartType.Text.Trim()}' ";
                    }

                    select += where;
                }

                select += $" order by {DESCRIPTION} ";

                using (cdb_connection dbc = new cdb_connection())
                {
                    DataTable dtab = dbc.get_data(select);

                    if (dtab.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtab.Rows)
                        {
                            CPartRec pr = new CPartRec();

                            try { pr.description = (dr[DESCRIPTION].ToString()); }
                            catch { }

                            try { pr.material = (dr[MATERIAL].ToString()); }
                            catch { }

                            try { pr.part_type = (dr[PART_TYPE].ToString()); }
                            catch { }

                            a.Add(pr);
                        }
                    }
                }
            }

            return a;
        }

        [Serializable]
        class CPartRec
        {
            public string description { get; set; }
            public string material { get; set; }
            public string part_type { get; set; }
        }
    }
}