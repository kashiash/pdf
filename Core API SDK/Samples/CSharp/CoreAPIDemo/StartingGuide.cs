using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("1. Starting Guide")]
	public class StartingGuide
	{
		[Description("1.1. Create new document")]
		static public int CreateNewDocument(Form1 Parent)
		{
			IPXC_Document coreDoc = Parent.m_pxcInst.NewDocument();
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;
			IPXC_UndoRedoData urd;
			IPXC_Pages pages = coreDoc.Pages;
			pages.AddEmptyPages(0, 4, ref rc, null, out urd);
			Parent.CloseDocument();
			Parent.m_CurDoc = coreDoc;
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("1.2. Open document from string path")]
		static public int OpenDocumentFromStringPath(Form1 Parent)
		{
			string sPath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			Parent.CloseDocument();
			Parent.m_CurDoc = Parent.m_pxcInst.OpenDocumentFromFile(sPath, null);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("1.3. Insert empty page into the beginning of the current document")]
		static public int InsertEmptyPagesIntoDoc(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcMedia = firstPage.get_Box(PXC_BoxType.PBox_MediaBox);
			IPXC_UndoRedoData urd = null;
			//Adding page with the size of the first page of the current document
			pages.AddEmptyPages(0, 1, ref rcMedia, null, out urd);
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("1.4. Add text to the center of the first page")]
		static public void AddTextToThePageCenter(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[0];
			PXC_Rect rcPage = page.get_Box(PXC_BoxType.PBox_PageBox);
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Tahoma", 0, 400);
			CC.SaveState();
			CC.SetFont(font);
			CC.SetStrokeColorRGB(0x000000);
			CC.SetFontSize(36);
			string str = "Centered Text";
			PXC_Rect rcOut = new PXC_Rect();
			//Calculating the text rectangle
			CC.ShowTextBlock(str, ref rcPage, ref rcPage, (uint)PXC_DrawTextFlags.DTF_CalcSizeOnly, -1, null, null, null, out rcOut);
			//Placing it in the center of page
			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			double nW = rcOut.right - rcOut.left;
			double nH = rcOut.top - rcOut.bottom;
			rcOut.left = nCX - nW / 2.0;
			rcOut.bottom = nCY - nH / 2.0;
			rcOut.right = rcOut.left + nW;
			rcOut.top = rcOut.bottom + nH;
			//Outputting the text
			PXC_Rect rc = new PXC_Rect();
			CC.ShowTextBlock(str, ref rcOut, ref rcOut, 0, -1, null, null, null, out rc);
			CC.RestoreState();
			page.PlaceContent(CC.Detach());
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("1.5. Draw Square annotation in the center of the first page")]
		static public int DrawSquareAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[0];
			PXC_Rect rcPage = page.get_Box(PXC_BoxType.PBox_PageBox);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Square annotation atom for the InsertNewAnnot method
			uint nSquare = pxsInst.StrToAtom("Square");
			//Placing it in the center of page
			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY - 100;
			rcOut.right = nCX + 200;
			rcOut.top = nCY + 100;
			IPXC_Annotation annot = page.InsertNewAnnot(nSquare, ref rcOut);
			IPXC_AnnotData_SquareCircle aData = annot.Data as IPXC_AnnotData_SquareCircle;
			aData.Opacity = 0.7;
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 1.0f, 1.0f);
			aData.Color = color;
			//Setting dashed border pattern
			var border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
			border.nWidth = 4.0f;
			border.DashArray = new float[10];
			border.DashArray[0] = border.DashArray[1] = 16.0f; //Width of dashes
			border.nDashCount = 2; //Number of dashes
			aData.set_Border(border);
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("1.6. Save document to file")]
		static public void SaveDocumentToFile(Form1 Parent)
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

	}
}
