using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net.Mail;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Management;
using System.Collections.Specialized;
using System.Xml;

namespace PCFsecure
{
    public abstract class PCFUtil
    {
        [DllImport("PCFsecure")]
        private static extern int sendEMail(string recips, string subject, string msg, string smtp_server,
            string smtp_user, string smtp_password, string smtp_from);

        [DllImport("PCFsecure")]
        private static extern int prevInstance(string win_class, string win_text);

        public static string PW = "1911VNS";
        public static string LIC_PW = "af95c6";

        public static SortedList page_params(string qs)
        {
            SortedList a = new SortedList();

            if (qs != string.Empty && qs != null)
            {
                try
                {
                    string dqs = PCFsecure.PCFUtil.DecryptData(PCFsecure.PCFUtil.toasc(qs), PW);

                    string[] f2 = dqs.Split('&');

                    foreach (string s in f2)
                    {
                        try
                        {
                            string[] f3 = s.Split('=');
                            a.Add(f3[0], f3[1]);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            return a;
        }

        public static int licenceManager(string productName)
        {
            string outLicenceType = string.Empty;
            return licenceManager(productName, out outLicenceType);

        }
        public static int licenceManager(string productName, out string outLicenceType)
        {
            int ret = 0;
            outLicenceType = "Licenced";

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty) // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");

            string machineName = System.Environment.MachineName;

            string filename = Application.StartupPath + "\\pcf_sys_info.txt";
            

            if (!File.Exists(filename))
            {
                FileStream file = new FileStream(filename, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file, Encoding.Default);

                string s = PCFsecure.PCFUtil.tohex(PCFsecure.PCFUtil.EncryptData(MACAddress + "_" + machineName, LIC_PW));

                sw.Write(s.ToString());

                sw.Close();
                file.Close();
            }

            filename = Application.StartupPath + "\\pcf_licence.txt";

            if (File.Exists(filename))
            {
                StreamReader reader = new StreamReader(filename);
                string lic = reader.ReadToEnd();
                reader.Close();

                string s = PCFsecure.PCFUtil.DecryptData(PCFsecure.PCFUtil.toasc(lic), LIC_PW);

                if (!s.StartsWith(productName + "_" + MACAddress))
                {
                    ret = 2;
                    outLicenceType = "Licence invalid";
                }
            }
            else
            {
                filename = Application.StartupPath + "\\pcf_temp.txt";

                if (File.Exists(filename))
                {
                    StreamReader reader = new StreamReader(filename);
                    string lic = reader.ReadToEnd();
                    reader.Close();

                    string s = PCFsecure.PCFUtil.DecryptData(PCFsecure.PCFUtil.toasc(lic), LIC_PW);

                    DateTime dt = DateTime.Parse(s);

                    if (dt < DateTime.Now)
                    {
                        ret = 3;
                        outLicenceType = "Licence expired on " + dt.ToShortDateString();
                    }
                    else
                    {
                        outLicenceType = "Temporary licence expires on " + dt.ToShortDateString();
                    }
                }
                else
                {
                    outLicenceType = "Licence not found";
                    ret = 1;
                }
            }
            return ret;
        }

        public static string tohex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;

        }

        public static string toasc(string Data)
        {
            string Data1 = "";
            string sData = "";

            while (Data.Length > 0)
            {
                Data1 = System.Convert.ToChar(System.Convert.ToUInt32(Data.Substring(0, 2), 16)).ToString();
                sData = sData + Data1;
                Data = Data.Substring(2, Data.Length - 2);
            }

            return sData;
        }

        public static byte[] EncryptData(byte[] data, string password, System.Security.Cryptography.PaddingMode paddingMode)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");
            if (password == null)
                throw new ArgumentNullException("password"); PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = paddingMode;
            ICryptoTransform encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16)); using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                encStream.Write(data, 0, data.Length);
                encStream.FlushFinalBlock();
                return msEncrypt.ToArray();
            }
        }
        public static byte[] DecryptData(byte[] data, string password, System.Security.Cryptography.PaddingMode paddingMode)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data");
            if (password == null)
                throw new ArgumentNullException("password"); PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = paddingMode;
            ICryptoTransform decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16)); using (MemoryStream msDecrypt = new MemoryStream(data))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                // Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
                byte[] fromEncrypt = new byte[data.Length];                // Read as many bytes as possible.
                int read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                if (read < fromEncrypt.Length)
                {
                    // Return a byte array of proper size.
                    byte[] clearBytes = new byte[read];
                    Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
                    return clearBytes;
                }
                return fromEncrypt;
            }
        }
        public static string EncryptData(string data, string password)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (password == null)
                throw new ArgumentNullException("password"); byte[] encBytes = EncryptData(Encoding.UTF8.GetBytes(data), password, System.Security.Cryptography.PaddingMode.ISO10126);
            return Convert.ToBase64String(encBytes);
        }        /// <summary>
        /// Decrypt the data string to the original string.  The data must be the base64 string
        /// returned from the EncryptData method.
        /// </summary>
        /// <param name="data">Encrypted data generated from EncryptData method.</param>
        /// <param name="password">Password used to decrypt the string.</param>
        /// <returns>Decrypted string.</returns>
        public static string DecryptData(string data, string password)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (password == null)
                throw new ArgumentNullException("password"); byte[] encBytes = Convert.FromBase64String(data);
            byte[] decBytes = DecryptData(encBytes, password, System.Security.Cryptography.PaddingMode.ISO10126);
            return Encoding.UTF8.GetString(decBytes);
        }

        public static SortedList get_encrypt_appsettings(string config_file)
        {
            SortedList appsettings = new SortedList();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(config_file);

            XmlNode appSettingsNode = xDoc.SelectSingleNode("/configuration/appSettings");

            if (appSettingsNode != null)
            {
                XmlNodeList nodeList = xDoc.GetElementsByTagName("appSettings");
                XmlNodeList nodeAppSettings = nodeList[0].ChildNodes;

                foreach (XmlNode n in nodeAppSettings)
                {
                    XmlAttributeCollection xmlAttCollection = n.Attributes;

                    string key, value;
                    key = value = string.Empty;
                    foreach (XmlAttribute a in xmlAttCollection)
                    {
                        string name = a.Name.Trim().ToLower();
                        
                        if (name == "key")
                            key = DecryptData(toasc(a.Value), PW);

                        if (name == "value")
                        {
                            if (a.Value.Trim().Length > 0)
                                value = DecryptData(toasc(a.Value), PW);

                            if (key.Length > 0)
                            {
                                if (!appsettings.ContainsKey(key))
                                    appsettings.Add(key, value);
                            }
                        }
                    }
                }

                xDoc.Save(config_file);
            }

            return appsettings;
        }

        public static void encrypt_appsettings(string config_file, bool bencr)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(config_file);

            XmlNode appSettingsNode = xDoc.SelectSingleNode("/configuration/appSettings");

            if (appSettingsNode != null)
            {
                XmlNodeList nodeList = xDoc.GetElementsByTagName("appSettings");
                XmlNodeList nodeAppSettings = nodeList[0].ChildNodes;

                foreach (XmlNode n in nodeAppSettings)
                {
                    XmlAttributeCollection xmlAttCollection = n.Attributes;

                    foreach (XmlAttribute a in xmlAttCollection)
                    {
                        string name = a.Name.Trim().ToLower();
                        if (name == "key" || name == "value")
                            if (a.Value.Length > 0)
                                if (bencr)
                                    a.Value = tohex(EncryptData(a.Value, PW));
                                else
                                    a.Value = DecryptData(toasc(a.Value), PW);
                    }
                }

                xDoc.Save(config_file);
            }
        }

        public static int prev_inst(string win_class, string win_text)
        {
            return prevInstance(win_class, win_text);
        }

        public static int send_EMailToUsers(string sql_server, ArrayList users, string subject, string msg)
        {
            ArrayList pcf_users = getUsers(sql_server);
            PCFsecureSettings pcfs = getSettings(sql_server);
            string email_addrs = "";

            foreach (string su in users)
            {
                foreach (PCFsecure.User pu in pcf_users)
                {
                    if (su == pu.Username)
                    {
                        if (pu.Email_address.Trim().Length > 0)
                        {
                            if (email_addrs.Length > 0)
                                email_addrs += ";";

                            if (pu.Email_address[0] != '<')
                                email_addrs += "<";

                            email_addrs += pu.Email_address;

                            if (pu.Email_address[pu.Email_address.Length - 1] != '>')
                                email_addrs += ">";
                        }
                    }
                }
            }

            if (email_addrs.Length > 0)
            {
                return send_EMail(email_addrs, subject, msg, pcfs.smtp_server, pcfs.smtp_user, pcfs.smtp_password, pcfs.smtp_from);
            }
            else
                return 0;
        }

        public static int send_EMail(string recips, string subject, string msg, string smtp_server, 
            string smtp_user, string smtp_password, string smtp_from)
        {
            if (smtp_from.Length > 0)
            {
                if (smtp_from[0] != '<')
                    smtp_from = "<" + smtp_from;

                if (smtp_from[smtp_from.Length -1] != '>')
                    smtp_from =  smtp_from + ">"; 

            }
            return sendEMail( recips,  subject,  msg,  smtp_server, 
             smtp_user,  smtp_password,  smtp_from);
        }

        public static PCFsecureSettings getSettings(string sql_server)
        {
            PCFsecureSettings settings = new PCFsecureSettings();
            
            string connStr = "Data Source=" + sql_server + ";Initial Catalog=PCFsecure;Integrated Security=True;";

            CapEncrypt ce = new CapEncrypt();
            SqlConnection connection = new SqlConnection(connStr);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandText = "if not exists (select * from syscolumns "
                      + "  where id=object_id('settings') and name='reprint_printer')"
                      + " alter table settings add reprint_printer nvarchar(50) NULL;";


            cmd.ExecuteNonQuery();

            
            cmd.CommandText = "SELECT * FROM settings WHERE id = 1";
            SqlDataAdapter ad_items = new SqlDataAdapter(cmd);
            DataSet ds_items = new DataSet();

            const string ITEMS_LIST = "settings_list";
            ad_items.Fill(ds_items, ITEMS_LIST);

            DataTable dta_items = ds_items.Tables[ITEMS_LIST];

            foreach (DataRow dr_items in dta_items.Rows)
            {
                settings.gsbin = dr_items["gsbin"].ToString().Trim();

                string [] sa = dr_items["ldapserver"].ToString().Trim().Split('|');

                if (sa.Length > 0)
                {
                    settings.ldapserver = sa[0];

                    if (sa.Length > 1)
                    {
                        settings.bldap_over_ssl = (sa[1] == "1");
                    }
                }
                
                settings.chq_printers = dr_items["chq_printers"].ToString();

                settings.rep_printers = dr_items["rep_printers"].ToString();

                if (dr_items["multi_auths"].ToString().Length > 0)
                    settings.multi_users = Convert.ToInt32(dr_items["multi_auths"].ToString());

                settings.smtp_server = dr_items["smtp_server"].ToString().Trim();

                settings.smtp_user = dr_items["smtp_user"].ToString().Trim();

                settings.smtp_password = ce.decrypt(dr_items["smtp_password"].ToString().Trim());

                settings.smtp_from = dr_items["smtp_from"].ToString().Trim();

                settings.smtp_admin = dr_items["smtp_admin_address"].ToString().Trim();

                settings.web_pages_dir = dr_items["web_pages_dir"].ToString().Trim();

                settings.web_service_dir = dr_items["web_service_dir"].ToString().Trim();

                settings.username = dr_items["cap_username"].ToString().Trim();

                ce.init();
                settings.password = ce.decrypt(dr_items["cap_password"].ToString().Trim());

                settings.domain = dr_items["cap_domain"].ToString().Trim();

                settings.reprint_printer = dr_items["reprint_printer"].ToString().Trim();

                
            }

            if (settings.sqlncli == string.Empty)
            {
                string cap_ini = Application.StartupPath + "\\PCFCapture.ini";

                IniFile ini = new IniFile(cap_ini);

                string cs = ini.IniReadValue("DEFAULT", "ConnectionString");

                string[] f1 = cs.Split(';');

                foreach (string s in f1)
                {
                    if (s.ToUpper().StartsWith("PROVIDER"))
                    {
                        string[] f2 = s.Split('=');

                        if (f2.Length > 1)
                            settings.sqlncli = f2[1].ToUpper();

                        break;
                    }
                }

                if (settings.sqlncli == string.Empty)
                {
                    settings.sqlncli = "SQLNCLI";
                }
            }

            connection.Close();

            return settings;
        }

        public static ArrayList getUsers(string sql_server)
        {
            ArrayList users = new ArrayList();

            string connStr = "Data Source=" + sql_server + ";Initial Catalog=PCFsecure;Integrated Security=True;";

            CapEncrypt ce = new CapEncrypt();
            SqlConnection connection = new SqlConnection(connStr);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM users";
            SqlDataAdapter ad_items = new SqlDataAdapter(cmd);
            DataSet ds_items = new DataSet();

            const string ITEMS_LIST = "users_list";
            ad_items.Fill(ds_items, ITEMS_LIST);

            DataTable dta_items = ds_items.Tables[ITEMS_LIST];

            foreach (DataRow dr_items in dta_items.Rows)
            {
                User u = new User();
                u.Id = Convert.ToInt32(dr_items["id"]);
                u.Username = dr_items["username"].ToString().Trim();
                u.Email_address = dr_items["email_address"].ToString().Trim();
                u.Rights = dr_items["rights"].ToString().Trim();

                users.Add(u);
            }

            connection.Close();

            return users;
        }

        public static ArrayList getGroups(string sql_server)
        {
            ArrayList groups = new ArrayList();

            string connStr = "Data Source=" + sql_server + ";Initial Catalog=PCFsecure;Integrated Security=True;";
            CapEncrypt ce = new CapEncrypt();
            SqlConnection connection = new SqlConnection(connStr);
            connection.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM groups";

            SqlDataAdapter ad_items = new SqlDataAdapter(cmd);
            DataSet ds_items = new DataSet();

            const string ITEMS_LIST = "groups_list";
            ad_items.Fill(ds_items, ITEMS_LIST);

            DataTable dta_items = ds_items.Tables[ITEMS_LIST];
            
            foreach (DataRow dr_items in dta_items.Rows)
            {
                Group g = new Group();
                g.Id = Convert.ToInt32(dr_items["id"]);
                g.Groupname = dr_items["groupname"].ToString().Trim();

                

                string[] split = dr_items["users"].ToString().Trim().Split(',');

                foreach (string s in split)
                {
                    g.Users.Add(s);

                    
                    
                }

                groups.Add(g);
            }

            connection.Close();

            return groups;
        }

        public static ArrayList getUserGroupMembership(string sql_server, string user)
        {
            ArrayList membership = new ArrayList();
            ArrayList groups = getGroups(sql_server);

            foreach (Group g in groups)
            {
                foreach(string u in g.Users)
                {
                    if(user.ToLower() == u.ToLower())
                    {
                        membership.Add(g.Groupname);
                    }
                }
            }

            return membership;
        }

        public static void logit(string msg)
        {
            try
            {
                string logPath = Application.StartupPath + "\\logs";

                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                DateTime dt = DateTime.Now;
                string filename = logPath;
                filename += "\\";
                filename += dt.Year.ToString();
                filename += dt.Month.ToString("00");
                filename += dt.Day.ToString("00");
                filename += ".log";

                FileStream file = new FileStream(filename, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file, Encoding.Default);

                string s;

                s = dt.ToString("T");
                s += " ";
                s += msg;
                s += "\r\n";
                sw.Write(s.ToString());

                sw.Close();
                file.Close();
            }
            catch (Exception e)
            {
                string errmsg = "An error occurred in the function logit. Additional info - " + e.Message;

                MessageBox.Show(errmsg,
                        "PCF",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
        }

        public static void log_event(string sql_server, string msg)
        {
            try
            {
                string connStr = "Data Source=" + sql_server + ";Initial Catalog=PCFsecure;Integrated Security=True;";

                SqlConnection conn = new SqlConnection(connStr);

                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT event_log (date_time, event)" +
                                        "Values(@date_time, @event);");

                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@date_time", SqlDbType.DateTime).Value = DateTime.Now;

                cmd.Parameters.Add("@event", SqlDbType.NChar).Value = msg.Substring(0, msg.Length > 256 ? 256 : msg.Length);

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch 
            {
            }
        }
    }
    
    public  class PCFsecureSettings
    {
        public string gsbin= string.Empty;
        public string ldapserver = string.Empty;
        public bool bldap_over_ssl = false;
        public string chq_printers = string.Empty;
        public string rep_printers = string.Empty;
        public string smtp_server = string.Empty;
        public string smtp_user = string.Empty;
        public string smtp_password = string.Empty;
        public string smtp_from = string.Empty;
        public string smtp_admin = string.Empty;
        public string web_service_dir = string.Empty;
        public string web_pages_dir = string.Empty;
        public string username = string.Empty;
        public string password = string.Empty;
        public string domain = string.Empty;
        public int multi_users;
        public string reprint_printer = string.Empty;
        public string sqlncli = string.Empty;
    }

    public class CapEncrypt
    {
        string m_D, m_E;
        ushort m_c1, m_c2, m_r;

        public CapEncrypt()
        {
            m_c1 = 52845;
            m_c2 = 22719;
            init();
        }

        public void init()
        {
            m_r = 45664;
            m_D = "";
        }

        public string decrypt(string dataToDecrypt)
        {
            m_D = "";

            try
            {
                if (dataToDecrypt.Length > 0)
                {
                    string s = toasc(dataToDecrypt.ToLower());

                    foreach (char c in s)
                    {
                        ushort v = c;
                        ushort plain = (ushort)(v ^ (m_r >> 8));
                        m_r = (ushort)((v + m_r) * m_c1 + m_c2);
                        m_D += Convert.ToChar(plain);
                    }
                }
            }
            catch
            {
                m_D = "";
            }
            return m_D;
        }

        public string encrypt(string dataToEncrypt)
        {
            m_E = "";

            if (dataToEncrypt.Length > 0)
            {
                char cipher;

                foreach (char c in dataToEncrypt)
                {
                    cipher = Convert.ToChar((c ^ (m_r >> 8)));
                    m_r = (ushort)((cipher + m_r) * m_c1 + m_c2);
                    m_E += tohex(cipher.ToString());
                }
            }

            return m_E.ToUpper();
        }
        public string tohex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;

        }

        private string toasc(string Data)
        {
            string Data1 = "";
            string sData = "";

            while (Data.Length > 0)
            {
                Data1 = System.Convert.ToChar(System.Convert.ToUInt32(Data.Substring(0, 2), 16)).ToString();
                sData = sData + Data1;
                Data = Data.Substring(2, Data.Length - 2);
            }

            return sData;
        }

        
    }

    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
     
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
     
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }
    
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }
    }

    public class Group
    {
        public Group()
        {
            Users = new ArrayList();
        }

        Int32 id;
        private string groupname;
        private ArrayList users;

        public Int32 Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public string Groupname
        {
            get
            {
                return this.groupname;
            }

            set
            {
                this.groupname = value;
            }
        }

        public ArrayList Users
        {
            get
            {
                return this.users;
            }

            set
            {
                this.users = value;
            }
        }
    }

    public class User
    {
        int id;
        private string username;
        private string email_address;
        private string rights;

        public Int32 Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }

        }
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
            }
        }

        public string Email_address
        {
            get
            {
                return this.email_address;
            }

            set
            {
                this.email_address = value;
            }
        }

        public string Rights
        {
            get
            {
                return this.rights;
            }

            set
            {
                this.rights = value;
            }
        }
    }
}
