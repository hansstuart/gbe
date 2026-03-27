using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;

namespace gbe
{
    public partial class irisndt : System.Web.UI.Page
    {
        const string FILEUPLOAD1_SESSION = "FileUpload1_Session";
        const string VS_DOWNLOAD_DATA = "VS_DOWNLOAD_DATA";
        const string VS_UPLOAD_DATA = "VS_UPLOAD_DATA";
        const string LIGHTGREEN = "LightGreen";
        const string WHITE = "White";
        const string CLI_REF = "CLI_REF";
        const string UID = "uid";
        const int CW1 = 320;

        ArrayList m_uploaded_data = null;
        ArrayList m_download_data = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = string.Empty;
            
            if (IsPostBack)
            {
                if (FileUpload1.HasFile)
                {
                    Session[FILEUPLOAD1_SESSION] = FileUpload1;
                }
                else
                {
                    if (Session[FILEUPLOAD1_SESSION] != null)
                    {
                        FileUpload1 = (FileUpload)Session[FILEUPLOAD1_SESSION];
                    }
                }

                m_download_data = (ArrayList)ViewState[VS_DOWNLOAD_DATA];
                m_uploaded_data = (ArrayList)ViewState[VS_UPLOAD_DATA];

                display_download_data();
                display_uploaded_data();
            }
            else
            {
                txtUploadFile.Attributes.Add("readonly", "readonly");
                btnBrowse.Attributes.Add("onclick", "document.getElementById('" + FileUpload1.ClientID + "').click(); return false;");

                get_download_data();
                display_download_data();

                get_uploaded_data();
                display_uploaded_data();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (txtUploadFile.Text.Trim().Length > 0 && FileUpload1.HasFile)
            {
                if (Path.GetExtension(FileUpload1.FileName).ToLower() == ".pdf")
                {
                    string filename = Path.GetFileName(FileUpload1.FileName);

                    string dir = Server.MapPath("~/") + "\\temp";

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string save_name = get_unique_file_name(dir, ".pdf");

                    FileUpload1.SaveAs(save_name);

                    Session[FILEUPLOAD1_SESSION] = null;
                    txtUploadFile.Text = string.Empty;

                    using (irisndt_upload iu = new irisndt_upload())
                    {
                        SortedList sl = new SortedList();
                        sl.Add(irisndt_upload.DT, DateTime.Now);
                        sl.Add(irisndt_upload.FILENAME, filename);
                        sl.Add(irisndt_upload.FILE_CONTENT, File.ReadAllBytes(save_name));
                        sl.Add(irisndt_upload.PROCESSED, 0);

                        iu.save_irisndt_upload_data(sl);
                    }

                    File.Delete(save_name);

                    Response.Redirect("apave.aspx");
                }
            }
        }

        void get_download_data()
        {
            if(m_download_data == null)
                m_download_data = new ArrayList();

            m_download_data.Clear();

            irisndt_download idn = new irisndt_download();
            
            m_download_data = idn.get_download_data();
            
            ViewState[VS_DOWNLOAD_DATA] = m_download_data;
        }

        void display_download_data()
        {
            TableRow r;
            TableCell c;

            tblUploads.Rows.Clear();

            if (m_download_data.Count > 0)
            {
                string[] hdr = new string[] { "Client Reference" };

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName(LIGHTGREEN);

                foreach (string sh in hdr)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblDownloads.Rows.Add(r);
            }

            foreach (string cli_ref in m_download_data)
            {
                if (cli_ref.Trim().Length > 0)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName(WHITE);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(cli_ref));
                    c.Width = CW1;
                    r.Cells.Add(c);

                    c = new TableCell();

                    c.HorizontalAlign = HorizontalAlign.Center;
                    ImageButton btn_save = new ImageButton();
                    btn_save.ToolTip = "Save";
                    btn_save.ImageUrl = "~/disk.png";
                    btn_save.Click += new ImageClickEventHandler(btn_save_Click);
                    btn_save.ID = "btn_save" + cli_ref;
                    btn_save.Attributes[CLI_REF] = cli_ref;

                    c.Controls.Add(btn_save);
                    r.Cells.Add(c);

                    tblDownloads.Rows.Add(r);
                }
            }
        }

        private void btn_save_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn_save = (ImageButton)sender;

            string cli_ref = btn_save.Attributes[CLI_REF];

            if (cli_ref.Trim().Length > 0)
            {
                string CRLF = "\r\n";

                irisndt_download idn = new irisndt_download();
                ArrayList a = idn.get_weld_test_save_data(cli_ref);

                if (a.Count > 0)
                {
                    Response.ContentType = "text/csv";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + cli_ref.Replace('/', '-') + ".csv");

                    foreach (string sline in a)
                    {
                        Response.Write(sline + CRLF);
                    }

                    Response.End();
                }
                else
                {
                    lblMsg.Text = "Empty file";
                }
            }
        }

        void get_uploaded_data()
        {
            if(m_uploaded_data == null)
                m_uploaded_data = new ArrayList();

            m_uploaded_data.Clear();

            using (irisndt_upload iu = new irisndt_upload())
            {
                m_uploaded_data = iu.get_upload_data($"{irisndt_upload.ID}, {irisndt_upload.DT}, {irisndt_upload.FILENAME}", null);
            }

            ViewState[VS_UPLOAD_DATA] = m_uploaded_data;
        }

        void display_uploaded_data()
        {
            tblUploads.Rows.Clear();

            if (m_uploaded_data.Count > 0)
            {
                TableRow r;
                TableCell c;

                string[] hdr = new string[] { "File", "Date" };

                r = new TableRow();
                r.BackColor = System.Drawing.Color.FromName(LIGHTGREEN);

                foreach (string sh in hdr)
                {
                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(sh));
                    r.Cells.Add(c);
                }

                tblUploads.Rows.Add(r);

                foreach (irisndt_upload_data iud in m_uploaded_data)
                {
                    r = new TableRow();
                    r.BackColor = System.Drawing.Color.FromName(WHITE);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(iud.filename));
                    c.Width = CW1;
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.Controls.Add(new LiteralControl(iud.dt.ToString("dd/MM/yyyy HH:mm")));
                    r.Cells.Add(c);

                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    ImageButton btn_view_report  = new ImageButton();
                    btn_view_report.ToolTip = "View";
                    btn_view_report.ImageUrl = "~/pdf.png";
                    btn_view_report.Click += new ImageClickEventHandler(btn_view_report_Click);
                    btn_view_report.ID = "btn_view_report" + iud.id.ToString();
                    btn_view_report.Attributes[UID] = iud.id.ToString();

                    c.Controls.Add(btn_view_report);
                    r.Cells.Add(c);

                    /*
                    c = new TableCell();
                    c.HorizontalAlign = HorizontalAlign.Center;
                    ImageButton btn_delete_report  = new ImageButton();
                    btn_delete_report.OnClientClick = "Confirm()";
                    btn_delete_report.ToolTip = "Delete";
                    btn_delete_report.ImageUrl = "~/delete.png";
                    btn_delete_report.Click +=  new ImageClickEventHandler(btn_delete_report_Click);
                    btn_delete_report.ID = "btn_delete_report" + iud.id.ToString();
                    btn_delete_report.Attributes[UID] = iud.id.ToString();

                    c.Controls.Add(btn_delete_report);
                    r.Cells.Add(c);
                    */

                    tblUploads.Rows.Add(r);
                }
            }
        }

        private void btn_delete_report_Click(object sender, ImageClickEventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];

            if (confirmValue == "Yes")
            {
                ImageButton b = (ImageButton)sender;

                int id = 0;

                try { id = Convert.ToInt32(b.Attributes[UID]); } catch { }

                irisndt_upload upld = new irisndt_upload();

                upld.delete_upload(id);

                get_uploaded_data();
                display_uploaded_data();
            }
        }

        private void btn_view_report_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton b = (ImageButton)sender;

            string uid = (b.Attributes[UID]);
            string url = string.Empty;

            url = "view_weld_test_report.aspx?id=" + uid;

            Response.Write("<script>");
            Response.Write("window.open('" + url + "','_blank','','false')");
            Response.Write("</script>");
        }

        string get_unique_file_name(string path, string ext)
        {
            string fnname = string.Empty;

            if (!path.EndsWith("\\"))
                path += "\\";

            Random r = new Random();

            do
            {
                fnname = path + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + "_" + r.Next(1, 10000).ToString("00000") + ext;
            } while (File.Exists(fnname));

            return fnname;
        }
    }
}