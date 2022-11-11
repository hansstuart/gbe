using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace gbe
{
    public partial class consignment_deliveries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl = new SortedList();

            sl.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl);
            }

            sl.Clear();

            if (a.Count > 0)
            {
                user_data ud = (user_data)a[0];

                if (ud.role.ToUpper() == "CUSTOMER")
                    sl.Add("owner", login_id);

                using (consignment_reference cr = new consignment_reference())
                {
                    cr.order_by = "id DESC";
                    a = cr.get_consignment_reference_data(sl);
                }
                
                TableRow r;
                TableCell c;

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName("LightGreen");
                
                c = new TableCell();
                c.Controls.Add(new LiteralControl("Reference"));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Delivery Date"));
                r.Cells.Add(c);

                c = new TableCell();
                c.Controls.Add(new LiteralControl("Delivery Note"));
                r.Cells.Add(c);
                
                tblCD.Rows.Add(r);

                int i = 0;

                foreach (consignment_reference_data crd in a)
                {
                    r = new TableRow();

                    System.Drawing.Color bc;
                    if (i++ % 2 == 0)
                        bc = System.Drawing.Color.FromName("White");
                    else
                        bc = System.Drawing.Color.FromName("LightGray");

                    r.BackColor = bc;

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(crd.reference));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(crd.delivery_date.ToString("dd/MM/yyyy")));
                    r.Cells.Add(c);

                    c = new TableCell();
                    HyperLink h = new HyperLink();
                    h.Text = "Delivery Note";
                    h.NavigateUrl = "cust_delnote.aspx?x=" + PCFsecure.PCFUtil.tohex(PCFsecure.PCFUtil.EncryptData("ref_id=" + crd.id.ToString(), PCFsecure.PCFUtil.PW));
                    h.Target = "_blank";

                    c.Controls.Add(h);
                    r.Cells.Add(c);

                    tblCD.Rows.Add(r);
                }
            }
        }
    }
}
