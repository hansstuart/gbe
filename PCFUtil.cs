using System;
using System.Runtime.InteropServices;

public class PCFUtil
{
    [DllImport("PCFUtil.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Int32 psconvert(string gsBin,string psFile, string oFile, 
		string device, bool bAutoRotate, string pPassword,  Int32 permissions);

    public PCFUtil()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int ps_convert(string gsBin, string psFile, string oFile,
        string device, bool bAutoRotate, string pPassword, Int32 permissions)
    {
        return psconvert(gsBin, psFile, oFile, 
		        device, bAutoRotate, pPassword, permissions);
    }


}
