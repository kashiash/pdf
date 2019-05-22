using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("12. Actions")]
	class Actions
	{
		[Description("12.1. Add GoTo action as a bookmark")]
		static public int AddActionsGoTo(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);
			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "GoTo Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = Parent.CurrentPage;
			dest.nNullFlags = 4 | 8;
			dest.nType = PXC_DestType.Dest_XYZ;
			double[] point = { 20, 30, 0, 0 };
			dest.dValues = point;
			aList.AddGoto(dest);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("12.2. Add GoToR action as a bookmark")]
		static public int AddActionsGoToR(Form1 Parent)
		{

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "GoToR Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = 2;
			dest.nNullFlags = 4 | 8;
			dest.nType = PXC_DestType.Dest_XYZ;
			double[] point = { 20, 30, 0, 0 };
			dest.dValues = point;
			string sFilePath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			aList.AddGotoR(sFilePath, dest);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("12.3. Add GoToE action as a bookmark")]
		static public int AddActionsGoToE(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			uint atomGoToE = pxsInst.StrToAtom("GoToE");
			IPXC_NameTree attachments = Parent.m_CurDoc.GetNameTree("EmbeddedFiles");
			IPXS_PDFVariant var = null;
			try
			{
				var = attachments.Lookup("FeatureChartEU.pdf");
			}
			catch(Exception)
			{
				string sFilePath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
				IPXC_FileSpec fileSpec = Parent.m_CurDoc.CreateEmbeddFile(sFilePath);
				IPXC_EmbeddedFileStream EFS = fileSpec.EmbeddedFile;
				EFS.UpdateFromFile2(sFilePath);
				var = fileSpec.PDFObject;
			}

			attachments.Add("FeatureChartEU.pdf", var);

			IPXC_Action_Goto actionGoToE = Parent.m_pxcInst.GetActionHandler(atomGoToE).CreateEmptyAction(atomGoToE, Parent.m_CurDoc) as IPXC_Action_Goto;
			
			IPXC_GoToETargetPath targetPath = actionGoToE.TargetPath;
			IPXC_GoToETargetItem targetItem = targetPath.InsertNew();
			targetItem.FileName = "FeatureChartEU.pdf";
			targetItem = targetPath.InsertNew();
			targetItem.FileName = "MyStamps.pdf";
			targetItem = targetPath.InsertNew();
			targetItem.AnnotIndex = 0;
			targetItem.PageNumber = 0;

			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "GoToE Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			aList.Insert(0, actionGoToE);
			bookmark.Actions = aList;

			Marshal.ReleaseComObject(attachments);

			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks | (int)Form1.eFormUpdateFlags.efuf_Attachments;
		}

		[Description("12.4. Add Launch action as a bookmark")]
		static public int AddActionLaunch(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "Launch Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			string sFilePath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			aList.AddLaunch(sFilePath);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("12.5. Add URI action as a bookmark")]
		static public int AddActionURI(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "URI Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			string sFilePath = "https://www.tracker-software.com";
			aList.AddURI(sFilePath);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("12.6. Add JavaScript action as a bookmark")]
		static public int AddActionJS(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "Java Script Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			string sJS = "app.alert(\"Hello world!\", 3);";
			aList.AddJavaScript(sJS);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("12.7. Add Execute Command action as a bookmark")]
		static public int AddActionNamed(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);

			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			uint atomNamed = pxsInst.StrToAtom("Named");
			IPXC_Action_Named actionNamed = Parent.m_pxcInst.GetActionHandler(atomNamed).CreateEmptyAction(atomNamed, Parent.m_CurDoc) as IPXC_Action_Named;
			actionNamed.CmdName = "NextPage";
			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = "Execute Command Action";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			aList.Insert(0, actionNamed);
			bookmark.Actions = aList;

			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}
	}
}