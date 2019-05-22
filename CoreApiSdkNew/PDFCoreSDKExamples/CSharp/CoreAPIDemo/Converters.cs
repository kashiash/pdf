using System;
using System.IO;
using System.ComponentModel;
using PDFXCoreAPI;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoreAPIDemo
{
	[Description("9. Converters")]
	class Converters
	{
		public class DrawTextCallbacks : IPXC_DrawTextCallbacks
		{
			public IPXC_Page m_currPage = null;
			public string m_Text = "";
			public void OnGetNewRect(IPXC_ContentCreator pCC, out PXC_Rect pRect, ref uint pFlags, out PXC_Rect pClip)
			{
				m_currPage.PlaceContent(pCC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				PXC_Rect rc = m_currPage.get_Box(PXC_BoxType.PBox_PageBox);
				IPXC_UndoRedoData urData;
				m_currPage.Document.CreateContentCreator();
				IPXC_Pages pages = m_currPage.Document.Pages;
				IPXC_Page page = pages.InsertPage(m_currPage.Number + 1, ref rc, out urData);
				Marshal.ReleaseComObject(m_currPage);
				m_currPage = page;

				PXC_Rect rect = new PXC_Rect();
				rect.top = rc.top - 40;
				rect.right = rc.right - 40;
				rect.bottom = rc.bottom + 40;
				rect.left = rc.left + 40;

				pRect = rect;
				pClip = rect;
				Marshal.ReleaseComObject(pages);
			}

			
			public void OnGetNewText(IPXC_ContentCreator pCC, out string ppText, out int pTextLen, out uint pFlags)
			{
				ppText = m_Text;
				pTextLen = -1;
				pFlags = 0;
				m_Text = "";
			}

			public void OnEndPara(IPXC_ContentCreator pCC, int nIndex, string pID, string pText, int nTextLen, IPXC_Rects pLines)
			{

			}
			public void OnFinal(IPXC_ContentCreator CC)
			{
				m_currPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				Marshal.ReleaseComObject(m_currPage);
			}
		}

		[Description("9.1. Convert from PDF to image")]
		static public void ConvertToImage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);

			IIXC_Inst ixcInst = Parent.m_pxcInst.GetExtension("IXC");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages[Parent.CurrentPage];
			double nHeight = 0.0;
			double nWidth = 0.0;
			Page.GetDimension(out nWidth, out nHeight);
			uint cx = (uint)(nWidth * 150 / 72.0);
			uint cy = (uint)(nHeight * 150 / 72.0);
			IIXC_Page ixcPage = ixcInst.Page_CreateEmpty(cx, cy, IXC_PageFormat.PageFormat_8ARGB, 0);
			IPXC_PageRenderParams param = Parent.m_pxcInst.CreateRenderParams();
			if (param != null)
			{
				param.RenderFlags |= ((uint)PXC_RenderFlags.RF_SmoothImages | (uint)PXC_RenderFlags.RF_SmoothLineArts);
				param.SetColor(PXC_RenderColor.RC_PageColor1, 255, 255, 255, 0);
				param.TextSmoothMode |= PXC_TextSmoothMode.TSM_Antialias;
			}
			tagRECT rc = new tagRECT();
			rc.right = (int)cx;
			rc.bottom = (int)cy;
			PXC_Matrix matrix = Page.GetMatrix(PXC_BoxType.PBox_PageBox);
			matrix = auxInst.MathHelper.Matrix_Scale(ref matrix, cx / nWidth, -cy / nHeight);
			matrix = auxInst.MathHelper.Matrix_Translate(ref matrix, 0, cy);
			Page.DrawToIXCPage(ixcPage, ref rc, ref matrix, param);
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_XDPI] = 150;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_YDPI] = 150;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_INTERLACE] = 1;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_FILTER] = 5;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_COMP_LEVEL] = 5;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_FORMAT] = (uint)IXC_ImageFileFormatIDs.FMT_PNG_ID;
			IIXC_Image ixcImg = ixcInst.CreateEmptyImage();
			ixcImg.InsertPage(ixcPage, 0);
			string sPath = Path.GetTempFileName();
			sPath = sPath.Replace(".tmp", ".png");
			ixcImg.Save(sPath, IXC_CreationDisposition.CreationDisposition_Overwrite);
			Process.Start(sPath);

			ixcImg.RemovePageByIndex(0);
			ixcPage = ixcInst.Page_CreateEmpty(cx, cy, IXC_PageFormat.PageFormat_8Gray, 0);
			if (param != null)
				param.SetColor(PXC_RenderColor.RC_PageColor1, 255, 255, 255, 255);
			Page.DrawToIXCPage(ixcPage, ref rc, ref matrix, param);
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_XDPI] = 150;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_YDPI] = 150;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_YSUBSAMPLING] = 20;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_FORMAT] = (uint)IXC_ImageFileFormatIDs.FMT_JPEG_ID;
			ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_JPEG_QUALITY] = 100;
			ixcImg.InsertPage(ixcPage, 0);
			sPath = sPath.Replace(".png", ".jpg");
			ixcImg.Save(sPath, IXC_CreationDisposition.CreationDisposition_Overwrite);
			Process.Start(sPath);
			Marshal.ReleaseComObject(Page);

			ixcImg.RemovePageByIndex(0);
			ixcPage = ixcInst.Page_CreateEmpty(cx, cy, IXC_PageFormat.PageFormat_8RGB, 0);
			for (int i = 0; i < pages.Count; i++)
			{
				Page = pages[(uint)i];
				Page.DrawToIXCPage(ixcPage, ref rc, ref matrix, param);
				ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_XDPI] = 150;
				ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_YDPI] = 150;
				ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_ITYPE] = 1;
				ixcPage.FmtInt[(uint)IXC_FormatParametersIDS.FP_ID_FORMAT] = (uint)IXC_ImageFileFormatIDs.FMT_TIFF_ID;
				ixcImg.InsertPage(ixcPage);
				ixcPage = ixcInst.Page_CreateEmpty(cx, cy, IXC_PageFormat.PageFormat_8Gray, 0);
				Marshal.ReleaseComObject(Page);
			}
			sPath = sPath.Replace(".jpg", ".tiff");
			ixcImg.Save(sPath, IXC_CreationDisposition.CreationDisposition_Overwrite);
			Process.Start(sPath);

			
			Marshal.ReleaseComObject(pages);
		}

		[Description("9.2. Convert from image to PDF")]
		static public int ConvertToPDF(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IIXC_Inst ixcInst = Parent.m_pxcInst.GetExtension("IXC");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages[0];
			double nHeight = 0.0;
			double nWidth = 0.0;
			Page.GetDimension(out nWidth, out nHeight);
			IIXC_Image img = ixcInst.CreateEmptyImage();
			img.Load(System.Environment.CurrentDirectory + "\\Images\\Editor_welcome.png");
			IIXC_Page ixcPage = img.GetPage(0);
			IPXC_Image pxcImg = Parent.m_CurDoc.AddImageFromIXCPage(ixcPage);
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			PXC_Rect rcImg = new PXC_Rect();

			CC.SaveState();
			{
				//Proportional resize rectangle calculation
				{
					double k1 = nWidth / nHeight;
					double k2 = (double)pxcImg.Width / pxcImg.Height;
					if (k1 >= k2)
					{
						rcImg.top = nHeight;
						rcImg.right = nWidth / 2.0 + rcImg.top * k2 / 2.0;
						rcImg.left = nWidth / 2.0 - rcImg.top * k2 / 2.0;
					}
					else
					{
						rcImg.right = nWidth;
						rcImg.top = nHeight / 2.0 + rcImg.right / k2 / 2.0;
						rcImg.bottom = nHeight / 2.0 - rcImg.right / k2 / 2.0;
					}
				}
				//Moving the image rectangle to the center

				PXC_Rect rcImage = new PXC_Rect();
				rcImage.right = 1;
				rcImage.top = 1;
				PXC_Matrix matrix = Page.GetMatrix(PXC_BoxType.PBox_PageBox);
				matrix = auxInst.MathHelper.Matrix_RectToRect(ref rcImage, ref rcImg);
				CC.ConcatCS(ref matrix);
				CC.PlaceImage(pxcImg);
			}
			CC.RestoreState();
			Page.PlaceContent(CC.Detach());

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("9.3. Convert from PDF to txt file")]
		static public void ConvertToTXT(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);

			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages[Parent.CurrentPage];
			IPXC_PageText Text = Page.GetText(null, false);

			string writePath = Path.GetTempFileName();
			writePath = writePath.Replace(".tmp", ".txt");
			StreamWriter stream = new StreamWriter(writePath);

			List<PXC_TextLineInfo> textsLineInfo = new List<PXC_TextLineInfo>();

			for (int i = 0; i < Text.LinesCount; i++)
			{
				PXC_TextLineInfo pxcTLI = Text.LineInfo[(uint)i];
				textsLineInfo.Add(pxcTLI);
			}

			textsLineInfo.Sort(delegate (PXC_TextLineInfo firstTLI, PXC_TextLineInfo secondTLI)
			{
				PXC_RectF rcFirst = firstTLI.rcBBox;
				PXC_RectF rcSecond = secondTLI.rcBBox;
				auxInst.MathHelper.Rect_TransformDF(ref firstTLI.Matrix, ref rcFirst);
				auxInst.MathHelper.Rect_TransformDF(ref secondTLI.Matrix, ref rcSecond);
				if (rcFirst.top < rcSecond.top)
					return 1;
				if (rcFirst.top > rcSecond.top)
					return -1;

				if (rcFirst.left < rcSecond.left)
					return -1;
				if (rcFirst.left > rcSecond.left)
					return 1;

				return 0;
			});

			for (int i = 0; i < Text.LinesCount; i++)
			{
				stream.Write(Text.GetChars(textsLineInfo[i].nFirstCharIndex, textsLineInfo[i].nCharsCount));
				if (i < Text.LinesCount - 1)
					stream.Write((textsLineInfo[i].rcBBox.top == textsLineInfo[i + 1].rcBBox.top) ? " " : "\r\n");
			}
			stream.Close();
			Process.Start(writePath);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("9.4. Convert from txt to PDF")]
		static public int ConvertFromTXT(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rc = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Page page = pages.InsertPage(0, ref rc, out urData);
			DrawTextCallbacks drawTextCallbacks = new DrawTextCallbacks();
			drawTextCallbacks.m_currPage = page;

			drawTextCallbacks.m_Text = File.ReadAllText(Environment.CurrentDirectory + "\\Documents\\Hobbit.txt");

			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Times New Roman", 0, 400);
			CC.SetColorRGB(0x00000000);
			CC.SetFont(font);
			CC.SetFontSize(15);

			PXC_Rect rect = new PXC_Rect();
			rect.top = rc.top - 40;
			rect.right = rc.right - 40;
			rect.bottom = rc.bottom + 40;
			rect.left = rc.left + 40;

			CC.ShowTextBlock(drawTextCallbacks.m_Text, rect, rect, (uint)PXC_DrawTextFlags.DTF_Center, -1, null, null, drawTextCallbacks, out rect);
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}
	}
}
