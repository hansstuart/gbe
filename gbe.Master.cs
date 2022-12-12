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
    public partial class gbe : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool val = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if(val)
                build_menu();
        }

        void build_menu()
        {
           // Table tblMenu = (Table)Master.FindControl("tblMenu");

            const string CREATE_MODULE = "Create Module";
            const string CREATE_SPOOL = "Create Spool";
            const string CREATE_PO = "Create PO";
            const string USERS = "Users";
            const string PURCHASE_RETURN_ORDERS = "Purchase/Return Orders";
            const string ORDER_PARTS = "Order Parts";
            const string SPOOLS = "Spools";
            const string MODULES = "Modules";
            const string PARTS = "Parts";
            const string CUSTOMERS = "Customers";
            const string WELDER_FITTER_ACTIVITY = "Welder/Fitter Activity";
            const string WELD_MAPPING = "Weld Mapping";
            const string WELD_MAPPING_EXT = "Weld Mapping Ext.";
            const string QA = "QA";
            const string WELD_TESTS = "Weld Tests";
            const string DELIVERIES = "Deliveries";
            const string INVOICING = "Invoicing";
            const string VEHICLES = "Vehicles";
            const string DELIVERY_ADDRESSES = "Delivery Addresses";
            const string CREATE_DELIVERY = "Create Delivery";
            const string CLIENT_FREE_ISSUE_DELIVERIES = "Client Free Issue Deliveries";

            const string SETTINGS = "Settings";

            const string CONSUMABLES = "Consumables";
            const string CLIENT_FREE_ISSUE_ITEMS = "Client Free Issue Items";
            const string SUPPLIERS = "Suppliers";
            const string STOCK_LOCATIONS = "Stock Locations";
            const string STOCK_AUDIT_TRAIL = "Stock Audit Trail";
            const string STOCK = "Stock";

            const string ADD_TO_FAB_HOLDING_AREA = "Add To Fab. Holding Area";
            const string ADD_TO_FAB_SCHEDULE = "Add To Fab. Schedule";
            const string ADD_TO_DELIVERY_SCHEDULE = "Add To Delivery Schedule";
            const string VIEW_FAB_HOLDING_AREA = "View Fab. Holding Area";
            const string VIEW_FAB_QUARANTINE = "View Fab. Quarantine";
            const string VIEW_FAB_SCHEDULE = "View Fab. Schedule";
            const string VIEW_DELIV_SCHEDULE = "View Delivery Schedule";
            const string VIEW_DELIV_QUARANTINE = "View Delivery Quarantine";
            const string IMSL_CUSTOMERS = "IMSL Customers";

            const string TOTALS = "Totals";

            SortedList m_sl_menu_items = new SortedList();

            m_sl_menu_items.Add(CREATE_MODULE, "create_module.aspx");
            m_sl_menu_items.Add(CREATE_SPOOL, "create_spool.aspx");
            m_sl_menu_items.Add(CREATE_PO, "create_porder.aspx");
            m_sl_menu_items.Add(USERS, "maint1.aspx?t=users");
            m_sl_menu_items.Add(PURCHASE_RETURN_ORDERS, "porders.aspx");
            m_sl_menu_items.Add(ORDER_PARTS, "order_parts.aspx");
            m_sl_menu_items.Add(SPOOLS, "spools.aspx");
            m_sl_menu_items.Add(MODULES, "modules.aspx");
            m_sl_menu_items.Add(PARTS, "maint1.aspx?t=parts");
            m_sl_menu_items.Add(CUSTOMERS, "maint1.aspx?t=customers");
            m_sl_menu_items.Add(WELDER_FITTER_ACTIVITY, "welder_activity.aspx");
            m_sl_menu_items.Add(WELD_MAPPING, "weld_mapping.aspx");
            m_sl_menu_items.Add(WELD_MAPPING_EXT, "weld_mapping_ext.aspx");
            m_sl_menu_items.Add(QA, "qa.aspx");
            m_sl_menu_items.Add(WELD_TESTS, "weld_tests.aspx");
            m_sl_menu_items.Add(DELIVERIES, "deliveries.aspx");
            m_sl_menu_items.Add(INVOICING, "invoicing.aspx");
            m_sl_menu_items.Add(VEHICLES, "maint1.aspx?t=vehicles");
            m_sl_menu_items.Add(DELIVERY_ADDRESSES, "maint1.aspx?t=delivery_addresses");
            m_sl_menu_items.Add(CREATE_DELIVERY, "cust_delivery.aspx");
            m_sl_menu_items.Add(CLIENT_FREE_ISSUE_DELIVERIES, "cfii_deliveries.aspx");

            m_sl_menu_items.Add(SETTINGS, "maint1.aspx?t=settings");

            m_sl_menu_items.Add(CONSUMABLES, "maint1.aspx?t=consumable_parts");
            m_sl_menu_items.Add(CLIENT_FREE_ISSUE_ITEMS, "cfii_view.aspx");
            m_sl_menu_items.Add(SUPPLIERS, "maint1.aspx?t=suppliers");
            m_sl_menu_items.Add(STOCK_LOCATIONS, "maint1.aspx?t=locations");
            m_sl_menu_items.Add(STOCK_AUDIT_TRAIL, "stock_movement.aspx");
            m_sl_menu_items.Add(STOCK, "stock.aspx");
            m_sl_menu_items.Add(ADD_TO_FAB_HOLDING_AREA, "add_to_schedule.aspx?t=schedule_fab&h=1");
            m_sl_menu_items.Add(ADD_TO_FAB_SCHEDULE, "add_to_schedule.aspx?t=schedule_fab");
            m_sl_menu_items.Add(ADD_TO_DELIVERY_SCHEDULE, "add_to_schedule.aspx?t=schedule_delivery");
            m_sl_menu_items.Add(VIEW_FAB_HOLDING_AREA, "view_schedule.aspx?t=schedule_fab&h=1");
            m_sl_menu_items.Add(VIEW_FAB_QUARANTINE, "add_to_schedule.aspx?t=schedule_fab&q=1");
            m_sl_menu_items.Add(VIEW_FAB_SCHEDULE, "view_schedule.aspx?t=schedule_fab");
            m_sl_menu_items.Add(VIEW_DELIV_SCHEDULE, "view_schedule.aspx?t=schedule_delivery");
            m_sl_menu_items.Add(VIEW_DELIV_QUARANTINE, "add_to_schedule.aspx?t=schedule_delivery&q=1");
            m_sl_menu_items.Add(IMSL_CUSTOMERS, "maint1.aspx?t=customer_fab_mat");

            m_sl_menu_items.Add(TOTALS, "totals.aspx");

            // admin menus
            string[] admin_menu_main = { 
                CREATE_MODULE,
                CREATE_SPOOL,
                CREATE_PO,
                USERS,
                PURCHASE_RETURN_ORDERS,
                ORDER_PARTS,
                SPOOLS,
                MODULES,
                PARTS,
                CUSTOMERS,
                WELDER_FITTER_ACTIVITY,
                WELD_MAPPING,
                WELD_MAPPING_EXT,
                QA,
                WELD_TESTS,
                DELIVERIES,
                INVOICING,
                VEHICLES,
                DELIVERY_ADDRESSES,
                CREATE_DELIVERY,
                CLIENT_FREE_ISSUE_DELIVERIES,
                SETTINGS,
                ADD_TO_FAB_HOLDING_AREA,
                ADD_TO_DELIVERY_SCHEDULE,
                ADD_TO_FAB_SCHEDULE,
                VIEW_FAB_HOLDING_AREA,
                VIEW_FAB_QUARANTINE,
                VIEW_FAB_SCHEDULE,
                VIEW_DELIV_SCHEDULE,
                VIEW_DELIV_QUARANTINE,
                IMSL_CUSTOMERS,
                TOTALS
            };

            string[] admin_menu_stores = { 
                CONSUMABLES,
                CLIENT_FREE_ISSUE_ITEMS,
                SUPPLIERS,
                STOCK_LOCATIONS,
                STOCK_AUDIT_TRAIL,
                STOCK
            };

            // user menus
            string[] user_menu_main = { 
                CLIENT_FREE_ISSUE_DELIVERIES,
                CUSTOMERS,
                DELIVERIES,
                DELIVERY_ADDRESSES,
                INVOICING,
                MODULES,
                PARTS,
                QA,
                SPOOLS,
                VEHICLES,
                WELD_MAPPING,
                WELD_MAPPING_EXT,
                WELD_TESTS,
                WELDER_FITTER_ACTIVITY

            };

            string[] user_menu_stores = { 

            };

            // supervisor menus
            string[] supervisor_menu_main = { 
                CLIENT_FREE_ISSUE_DELIVERIES,
                CUSTOMERS,
                DELIVERIES,
                DELIVERY_ADDRESSES,
                MODULES,
                PARTS,
                QA,
                SPOOLS,
                VEHICLES,
                WELD_MAPPING,
                WELD_MAPPING_EXT,
                WELD_TESTS,
                ADD_TO_FAB_HOLDING_AREA,
                ADD_TO_DELIVERY_SCHEDULE,
                ADD_TO_FAB_SCHEDULE,
                VIEW_FAB_HOLDING_AREA,
                VIEW_FAB_QUARANTINE,
                VIEW_FAB_SCHEDULE,
                VIEW_DELIV_SCHEDULE,
                VIEW_DELIV_QUARANTINE
            };

            string[] supervisor_menu_stores = { 

            };

            // customer menus
            string[] customer_menu_main = { 
                CREATE_DELIVERY,
                CLIENT_FREE_ISSUE_DELIVERIES
            };

            string[] customer_menu_stores = { 

            };

            // storeman menus
            string[] storeman_menu_main = { 
                
            };

            string[] storeman_menu_stores = { 
                CONSUMABLES,
                CLIENT_FREE_ISSUE_ITEMS,
                SUPPLIERS,
                STOCK_LOCATIONS,
                STOCK_AUDIT_TRAIL,
                STOCK
            };

            string[] project_manager_menu_main = { 
                DELIVERIES,
                ORDER_PARTS,
                QA,
                SPOOLS,
            };

            string[] project_manager_menu_stores = { 
                CLIENT_FREE_ISSUE_ITEMS,
                STOCK_AUDIT_TRAIL,
                STOCK
            };

            string[] menu_main = { 
                
            };
            string[] menu_stores = { 
                
            };

            string login_id = System.Web.HttpContext.Current.User.Identity.Name;

            SortedList sl0 = new SortedList();

            sl0.Add("login_id", login_id);

            ArrayList a = new ArrayList();

            using (users u = new users())
            {
                a = u.get_user_data(sl0);
            }

            if (a.Count > 0)
            {
                user_data ud = (user_data)a[0];

                if (ud.role.ToUpper() == "ADMIN")
                {
                    menu_main = admin_menu_main;
                    menu_stores = admin_menu_stores;
                }

                if (ud.role.ToUpper() == "CUSTOMER")
                {
                    menu_main = customer_menu_main;
                    menu_stores = customer_menu_stores;
                }

                if (ud.role.ToUpper() == "STOREMAN")
                {
                    menu_main = storeman_menu_main;
                    menu_stores = storeman_menu_stores;
                }

                if (ud.role.ToUpper() == "USER")
                {
                    menu_main = user_menu_main;
                    menu_stores = user_menu_stores;
                }

                if (ud.role.ToUpper() == "SUPERVISOR")
                {
                    menu_main = supervisor_menu_main;
                    menu_stores = supervisor_menu_stores;
                }

                if (ud.role.ToUpper() == "PROJECT MANAGER")
                {
                    menu_main = project_manager_menu_main;
                    menu_stores = project_manager_menu_stores;
                }
            }

            SortedList sl = new SortedList();

            if (tblMenu != null )
            {
                if (menu_main.Length > 0)
                {
                    foreach (string s in menu_main)
                    {
                        if (m_sl_menu_items.ContainsKey(s))
                            sl.Add(s, m_sl_menu_items[s].ToString());
                    }

                    tblMenu.BorderStyle = BorderStyle.Groove;

                    TableRow r;
                    TableCell c;
                    HyperLink h;

                    foreach (DictionaryEntry e in sl)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");
                        c = new TableCell();
                        h = new HyperLink();
                        h.Font.Name = "Verdana";
                        h.Text = e.Key.ToString();
                        h.NavigateUrl = e.Value.ToString();
                        h.Width = 240;
                        h.CssClass = "GBE_Link";
                        c.Controls.Add(h);
                        r.Cells.Add(c);

                        tblMenu.Rows.Add(r);
                    }
                }
            }

            if (tblMenuStores != null)
            {
                sl.Clear();
                if (menu_stores.Length > 0)
                {
                    foreach (string s in menu_stores)
                    {
                        if (m_sl_menu_items.ContainsKey(s))
                            sl.Add(s, m_sl_menu_items[s].ToString());
                    }

                    tblMenuStores.BorderStyle = BorderStyle.Groove;

                    TableRow r;
                    TableCell c;
                    HyperLink h;

                    foreach (DictionaryEntry e in sl)
                    {
                        r = new TableRow();
                        r.BackColor = System.Drawing.Color.FromName("White");
                        c = new TableCell();
                        h = new HyperLink();
                        h.Font.Name = "Verdana";
                        h.Text = e.Key.ToString();
                        h.NavigateUrl = e.Value.ToString();
                        h.Width = 240;
                        h.CssClass = "GBE_Link";
                        c.Controls.Add(h);
                        r.Cells.Add(c);

                        tblMenuStores.Rows.Add(r);

                    }
                }
            }
        }
    }
}
