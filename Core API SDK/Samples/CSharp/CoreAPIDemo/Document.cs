using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("2. Document")]
	public class Document
	{
		[Description("2.1. Create new document")]
		static public int CreateNewDoc(Form1 Parent)
		{
			IPXC_Document coreDoc = Parent.m_pxcInst.NewDocument();
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;
			IPXC_UndoRedoData urd;
			IPXC_Pages pages = coreDoc.Pages;
			pages.AddEmptyPages(0, 1, ref rc, null, out urd);
			Parent.CloseDocument();
			Parent.m_CurDoc = coreDoc;
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("2.2. Open document with open file dialog")]
		static public int OpenDocWithOpenDialog(Form1 Parent)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "PDF Documents (*.pdf)|*.pdf|All Files (*.*)|*.*";
			ofd.DefaultExt = "pdf";
			ofd.FilterIndex = 1;
			ofd.CheckPathExists = true;
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Parent.CloseDocument();
				Parent.m_CurDoc = Parent.m_pxcInst.OpenDocumentFromFile(ofd.FileName, null);
			}

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("2.3. Open document from string path")]
		static public int OpenDocFromStringPath(Form1 Parent)
		{
			string sPath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			Parent.CloseDocument();
			Parent.m_CurDoc = Parent.m_pxcInst.OpenDocumentFromFile(sPath, null);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("2.4. Open document from IStream")]
		static public int OpenDocumentFromStream(Form1 Parent)
		{
			string sPath = System.Environment.CurrentDirectory + "\\Documents\\FormTypes.pdf";
			Parent.CloseDocument();
			FileStream srcStream = new FileStream(sPath, FileMode.Open);
			if (srcStream != null)
			{
				IStreamWrapper srcIStream = new IStreamWrapper(srcStream);
				Parent.m_CurDoc = Parent.m_pxcInst.OpenDocumentFrom(srcIStream, null);
			}

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		private class AuthCallback : IPXC_DocAuthCallback
		{
			public void AuthDoc(IPXC_Document pDoc, uint nFlags)
			{
				//If this method is called then the document is protected
				pDoc.AuthorizeWithPassword("111");
			}
		}

		[Description("2.5. Open password protected document from IAFS_Name")]
		static public int OpenPasswordProtectedDocument(Form1 Parent)
		{
			string sPath = System.Environment.CurrentDirectory + "\\Documents\\PasswordProtected.pdf";
			IAFS_Inst fsInst = (IAFS_Inst)Parent.m_pxcInst.GetExtension("AFS");
			IAFS_Name destPath = fsInst.DefaultFileSys.StringToName(sPath); //Converting string to name
			Parent.CloseDocument();
			AuthCallback clbk = new AuthCallback();
			Parent.m_CurDoc = Parent.m_pxcInst.OpenDocumentFrom(destPath, clbk);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("2.6. Save document to file")]
		static public void SaveDocToFile(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return;
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "PDF Documents (*.pdf)|*.pdf|All Files (*.*)|*.*";
			sfd.DefaultExt = "pdf";
			sfd.FilterIndex = 1;
			sfd.CheckPathExists = true;
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				Parent.m_CurDoc.WriteToFile(sfd.FileName);
				System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + sfd.FileName + "\"");
			}
		}

		[Description("2.7. Add PDF Security to the document")]
		static public void ProtectDoc(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return;
			Parent.m_CurDoc.SetStdEncryption(PXC_StdEncryptionMehtod.SEM_AV10, false, (uint)PXC_SecurityPermissions.Permit_Printing 
				| (uint)PXC_SecurityPermissions.Permit_Copying_And_TextGraphicsExtractions 
				| (uint) PXC_SecurityPermissions.Permit_Modification, "1243", "9524");
		}
	}
}
