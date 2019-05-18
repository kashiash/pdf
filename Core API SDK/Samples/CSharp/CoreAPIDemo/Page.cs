using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("3. Page")]
	class Page
	{
		[Description("3.1. Insert empty pages into the beginning of the current document")]
		static public int InsertEmptyPages(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcMedia = firstPage.get_Box(PXC_BoxType.PBox_MediaBox);
			IPXC_UndoRedoData urd = null;
			//Adding pages with the size of the first page of the current document
			pages.AddEmptyPages(0, 3, ref rcMedia, null, out urd);
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("3.2. Insert page from another document into the beginning of the current document")]
		static public int InsertPagesFromOtherDocument(Form1 Parent)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "PDF Documents (*.pdf)|*.pdf|All Files (*.*)|*.*";
			ofd.DefaultExt = "pdf";
			ofd.FilterIndex = 1;
			ofd.CheckPathExists = true;
			IPXC_Document srcDoc = null;
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				srcDoc = Parent.m_pxcInst.OpenDocumentFromFile(ofd.FileName, null);
			}
			if (srcDoc == null)
				return 0;
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			pages.InsertPagesFromDoc(srcDoc, 0, 0, 1, (int)PXC_InsertPagesFlags.IPF_Annots_Copy | (int)PXC_InsertPagesFlags.IPF_Widgets_Copy);
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("3.3. Remove first page from the current document")]
		static public int RemoveFirstPageFromTheDocument(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IAUX_Inst auxInst = (IAUX_Inst)Parent.m_pxcInst.GetExtension("AUX");
			IBitSet bs = auxInst.CreateBitSet(1);
			bs.Set(0);
			IPXC_UndoRedoData urd = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			if (pages.Count > 1)
				pages.DeletePages(bs, null, out urd);
			else
				MessageBox.Show("The last page can't be removed from the document!");
			Marshal.ReleaseComObject(urd);
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("3.4. Move first page of the current document into the last page position")]
		static public int MoveFirstPageToBack(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IAUX_Inst auxInst = (IAUX_Inst)Parent.m_pxcInst.GetExtension("AUX");
			IBitSet bs = auxInst.CreateBitSet(1);
			bs.Set(0);
			IPXC_UndoRedoData urd = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			if (pages.Count > 1)
				pages.MovePages(bs, pages.Count, null, out urd);
			else
				MessageBox.Show("Current document has one page - nothing to move!");
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("3.5. Resize document pages to the size of the content")]
		static public void ResizeDocumentPagesToTheContent(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			for (uint i = 0; i < pages.Count; i++)
			{
				IPXC_Page page = pages[i];
				PXC_Rect contentRect = page.get_Box(PXC_BoxType.PBox_BBox);
				page.set_Box(PXC_BoxType.PBox_MediaBox, ref contentRect);
				Marshal.ReleaseComObject(page);
			}
			Marshal.ReleaseComObject(pages);
		}
	}
}
