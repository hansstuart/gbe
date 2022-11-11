using System;
using System.Data;
using System.Configuration;
using System.Collections;

namespace gbe
{
    public class cost_centre
    {
        public SortedList m_sl_cost_centre = new SortedList();

        const string CC_GBE = "GBE";
        const string CC_PIPECENTER = "PipeCenter";
        const string CC_OLMAT = "OLMAT";
        const string CC_BUXTON_MCNULTY = "Buxton & McNulty";
        const string CC_DGR_FAB_MAT = "DGR Fab & Mat";
        const string CC_DGR_FAB_ONLY = "DGR Fab Only";
        const string CC_AP_FAB_ONLY = "Associated Pipework";
        const string CC_GENERIC = "Generic";
        const string CC_WATKINS = "Watkins";

        const string CC_APOLLO = "Apollo";
        const string CC_C_WATKINS = "C.Watkins";
        const string CC_CPS = "CPS";
        const string CC_EXCEL = "Excel";
        const string CC_SHAWSTON = "Shawston";

        public cost_centre()
        {
            m_sl_cost_centre.Add(1, CC_GBE);
            m_sl_cost_centre.Add(2, CC_PIPECENTER);
            m_sl_cost_centre.Add(3, CC_OLMAT);
            m_sl_cost_centre.Add(4, CC_DGR_FAB_MAT);
            m_sl_cost_centre.Add(5, CC_DGR_FAB_ONLY);
            m_sl_cost_centre.Add(6, CC_BUXTON_MCNULTY);
            m_sl_cost_centre.Add(7, CC_AP_FAB_ONLY);
            m_sl_cost_centre.Add(8, CC_GENERIC);
            m_sl_cost_centre.Add(9, CC_WATKINS);
            m_sl_cost_centre.Add(10, CC_APOLLO);
            m_sl_cost_centre.Add(11, CC_C_WATKINS);
            m_sl_cost_centre.Add(12, CC_CPS);
            m_sl_cost_centre.Add(13, CC_EXCEL);
            m_sl_cost_centre.Add(14, CC_SHAWSTON);
        }
    }
}
