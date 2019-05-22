using System.ComponentModel;
using System.Runtime.InteropServices;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("6. Stamps")]
	class Stamps
	{
		[Description("6.1. Add rotated Draft stamp from a standard collection by Stamp ID")]
		static public void AddStandardStampByID(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPB = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			IPXC_StampsManager stampManager = Parent.m_pxcInst.StampsManager;
			IPXC_StampInfo si = stampManager.FindStamp("Draft");
			//Creating stamp annotation
			IPXS_Inst pSInt = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			uint nStamp = pSInt.StrToAtom("Stamp");
			double nHeight = 0;
			double nWidth = 0;
			si.GetSize(out nWidth, out nHeight);
			PXC_Rect rc;
			rc.left = rcPB.left - nWidth / 2.0 + nHeight / 2.0;
			rc.right = rcPB.left + nWidth / 2.0 + nHeight / 2.0;
			rc.top = rcPB.top;
			rc.bottom = rc.top - nWidth;
			IPXC_Annotation annot = firstPage.InsertNewAnnot(nStamp, ref rc, 0);
			IPXC_AnnotData_Stamp stampData = (IPXC_AnnotData_Stamp)annot.Data;
			stampData.Rotation = 90;
			stampData.SetStampName(si.ID);
			annot.Data = stampData;
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("6.2. Add Expired stamp from a standard collection by index in collection")]
		static public void AddStandardStampByIndexInCollection(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPB = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			IPXC_StampsManager stampManager = Parent.m_pxcInst.StampsManager;
			uint nColIndex = (uint)stampManager.FindCollection("Standard");
			IPXC_StampsCollection sc = stampManager[nColIndex];
			IPXC_StampInfo si = null;
			for (uint i = 0; i < sc.Count; i++)
			{
				if (sc[i].ID == "Expired")
				{
					si = sc[i];
					break;
				}
			}
			//Creating stamp annotation
			IPXS_Inst pSInt = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			uint nStamp = pSInt.StrToAtom("Stamp");
			double nHeight = 0;
			double nWidth = 0;
			si.GetSize(out nWidth, out nHeight);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = nWidth;
			rc.top = rcPB.top;
			rc.bottom = rc.top - nHeight;
			IPXC_Annotation annot = firstPage.InsertNewAnnot(nStamp, ref rc, 0);
			IPXC_AnnotData_Stamp stampData = (IPXC_AnnotData_Stamp)annot.Data;
			stampData.SetStampName(si.ID);
			annot.Data = stampData;
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("6.3. Load stamps collection from stamp file and place stamp from it")]
		static public void LoadStampsCollectionFromFile(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			//Loading stamp collection from stamp file
			IAFS_Inst afsInst = (IAFS_Inst)Parent.m_pxcInst.GetExtension("AFS");
			string sPath = System.Environment.CurrentDirectory + "\\Documents\\MyStamps.pdf";
			IAFS_Name name = afsInst.DefaultFileSys.StringToName(sPath);
			int openFileFlags = (int)(AFS_OpenFileFlags.AFS_OpenFile_Read | AFS_OpenFileFlags.AFS_OpenFile_ShareRead);
			IAFS_File destFile = afsInst.DefaultFileSys.OpenFile(name, openFileFlags);
			IPXC_StampsCollection sc = Parent.m_pxcInst.StampsManager.LoadCollection(destFile);

			//Placing stamp from the loaded collection
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPB = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			IPXC_StampInfo si = sc[0]; //getting stamp by index - they are sorted by name
			//Creating stamp annotation
			IPXS_Inst pSInt = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			uint nStamp = pSInt.StrToAtom("Stamp");
			double nHeight = 0;
			double nWidth = 0;
			si.GetSize(out nWidth, out nHeight);
			PXC_Rect rc;//Annotation rectangle
			rc.left = 0;
			rc.right = nWidth;
			rc.top = rcPB.top;
			rc.bottom = rc.top - nHeight;
			IPXC_Annotation annot = firstPage.InsertNewAnnot(nStamp, ref rc, 0);
			IPXC_AnnotData_Stamp stampData = (IPXC_AnnotData_Stamp)annot.Data;
			stampData.set_BBox(rc); //Stamp rectangle boundaries
			stampData.SetStampName(si.ID);
			annot.Data = stampData;
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
		}
		[Description("6.4. Load stamp from image file")]
		static public void LoadStampFromTheImageFile(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			//Loading image file
			IAFS_Inst afsInst = (IAFS_Inst)Parent.m_pxcInst.GetExtension("AFS");
			string sPath = System.Environment.CurrentDirectory + "\\Images\\run_24.png";
			IAFS_Name name = afsInst.DefaultFileSys.StringToName(sPath);
			int openFileFlags = (int)(AFS_OpenFileFlags.AFS_OpenFile_Read | AFS_OpenFileFlags.AFS_OpenFile_ShareRead);
			IAFS_File destFile = afsInst.DefaultFileSys.OpenFile(name, openFileFlags);
			//Creating new collection
			IPXC_StampsCollection sc = Parent.m_pxcInst.StampsManager.CreateEmptyCollection("My Stamps");

			IPXC_StampInfo si = sc.AddStamp(destFile, "My Stamp");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPB = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			//Creating stamp annotation
			IPXS_Inst pSInt = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			uint nStamp = pSInt.StrToAtom("Stamp");
			double nHeight = 0;
			double nWidth = 0;
			si.GetSize(out nWidth, out nHeight);
			//Increasing width and height by 20
			PXC_Rect rc; //Annotation rectangle
			rc.left = 0;
			rc.right = nWidth * 20;
			rc.top = rcPB.top;
			rc.bottom = rc.top - nHeight * 20;
			IPXC_Annotation annot = firstPage.InsertNewAnnot(nStamp, ref rc, 0);
			IPXC_AnnotData_Stamp stampData = (IPXC_AnnotData_Stamp)annot.Data;
			stampData.set_BBox(rc); //Stamp rectangle boundaries
			stampData.SetStampName(si.ID);
			annot.Data = stampData;
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
		}
	}
}
