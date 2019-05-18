using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("11. Named Destinations")]
	class NamedDestinations
	{
		[Description("11.1. Add Named Destination after the currently selected item in the Named Destinations List")]
		static public int AddNewDestination(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = Parent.CurrentPage;
			dest.nNullFlags = 12;
			dest.nType = PXC_DestType.Dest_XYZ;
			double[] point = { 20, 30, 0, 0 };
			dest.dValues = point;
			Parent.m_CurDoc.SetNamedDestination("P." + (Parent.CurrentPage + 1), dest);
			return (int)Form1.eFormUpdateFlags.efuf_NamedDests;
		}

		[Description("11.2. Remove currently selected Named Destination from the Named Destinations List")]
		static public int RemoveNamedDest(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;
			if (Parent.SelectedNameDest_Item == null)
				return 0;

			IPXC_NameTree nameTree = Parent.m_CurDoc.GetNameTree("Dests");
			nameTree.Remove(Parent.SelectedNameDest_Item.Text);
			Marshal.ReleaseComObject(nameTree);
			return (int)Form1.eFormUpdateFlags.efuf_NamedDests;
		}
	}
}