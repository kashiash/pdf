using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("7. Annotations")]
	class Annotations
	{

		[Description("7.1. Add Text annotations")]
		static public int AddTextAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Text annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("Text");
			
			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 250;
			rcOut.right = nCX - 150;
			rcOut.top = nCY + 300;
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_Text aData = annot.Data as IPXC_AnnotData_Text;
			aData.Contents = "Text Annotation 1.";
			aData.Title = "Text Annotation 1";
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.8f, 0.8f);
			aData.Color = color;
			aData.Opacity = 0.5f;
			annot.Data = aData;

			rcOut.bottom -= 100;
			rcOut.top -= 100;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Text;
			aData.Contents = "Text Annotation 2.";
			aData.Title = "Text Annotation 2";
			color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.5f, 0.4f, 0.48f);
			aData.Color = color;
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.2. Add Link annotations")]
		static public int AddLinkAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Link annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("Link");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 250;
			rcOut.right = nCX + 150;
			rcOut.top = nCY + 300;
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_Link aData = annot.Data as IPXC_AnnotData_Link;
			aData.Contents = "Link Annotation 1.";
			aData.HighlighMode = PXC_AnnotHighlightMode.AHM_Outline;
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.8f, 0.8f);
			aData.Color = color;
			//Setting dashed border pattern
			var border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
			border.nWidth = 4.0f;
			border.DashArray = new float[10];
			border.DashArray[0] = border.DashArray[1] = 16.0f; //Width of dashes
			border.nDashCount = 2; //Number of dashes
			aData.set_Border(border);
			//Setting the annotation's URI action
			IPXC_ActionsList AL = Parent.m_CurDoc.CreateActionsList();
			AL.AddURI("https://www.google.com");
			annot.set_Actions(PXC_TriggerType.Trigger_Up, AL);
			annot.Data = aData;

			rcOut.bottom -= 200;
			rcOut.top -= 200;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Link;
			aData.Contents = "Link Annotation 2.";
			color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.5f, 0.4f, 0.48f);
			aData.Color = color;
			//Setting the annotation's Goto action
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = page.Number + 1;
			dest.nType = PXC_DestType.Dest_XYZ;
			dest.nNullFlags = 15;
			AL = Parent.m_CurDoc.CreateActionsList();
			AL.AddGoto(dest);
			annot.set_Actions(PXC_TriggerType.Trigger_Up, AL);
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.3. Add Free text annotations")]
		static public int AddFreeTextAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Free text annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("FreeText");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 200;
			rcOut.right = nCX + 200;
			rcOut.top = nCY + 300;
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_FreeText aData = annot.Data as IPXC_AnnotData_FreeText;
			aData.Contents = "Free Text Annotation 1.";
			aData.Title = "Free Text Annotation 1.";
			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Arial", (uint)PXC_CreateFontFlags.CreateFont_Monospaced, 700);
			aData.DefaultFont = font;
			aData.DefaultFontSize = 40;
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.7f, 0.7f, 0.7f);
			aData.DefaultTextColor = color;
			aData.Opacity = 0.5;
			aData.TextRotation = 90;
			aData.Subject = "Typewriter";
			//Setting dashed border pattern
			var border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Beveled;
			border.nWidth = 4.0f;
			border.DashArray = new float[15];
			border.DashArray[0] = border.DashArray[1] = 5.0f; //Width of dashes
			border.nDashCount = 2; //Number of dashes
			aData.set_Border(border);
			annot.Data = aData;

			rcOut.bottom -= 200;
			rcOut.top -= 200;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_FreeText;
			aData.Contents = "Free Text Annotation 2.";
			aData.DefaultFontSize = 15.0;
			color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.SColor = color;
			aData.DefaultTextColor = color;
			//Setting dashed border pattern
			border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
			border.nWidth = 10.0f;
			border.DashArray = new float[20];
			border.DashArray[0] = border.DashArray[1] = 5.0f; //Width of dashes
			border.nDashCount = 4; //Number of dashes
			aData.set_Border(border);
			annot.Data = aData;

			rcOut.bottom -= 200;
			rcOut.top -= 200;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_FreeText;
			aData.Contents = "Free Text Annotation 3.";
			color = auxInst.CreateColor(ColorType.ColorType_RGB);
			aData.DefaultFontSize = 15.0;
			color.SetRGB(1.0f, 1.0f, 1.0f);
			aData.SColor = color;
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.DefaultTextColor = color;
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.4. Add Line annotations")]
		static public int AddLineAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Line annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("Line");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 200;
			rcOut.right = nCX + 200;
			rcOut.top = nCY + 300;
			PXC_Point startPoint = new PXC_Point();
			startPoint.x = rcPage.left + 50;
			startPoint.y = rcPage.top - 50;
			PXC_Point endPiont = new PXC_Point();
			endPiont.x = rcPage.right - 50;
			endPiont.y = startPoint.y;
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_Line aData = annot.Data as IPXC_AnnotData_Line;
			aData.Title = "Line annotation 1.";
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.Color = color;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			PXC_AnnotBorder border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Solid;
			border.nWidth = 3.0f;
			aData.set_Border(border);
			annot.Data = aData;

			startPoint.y = startPoint.y - 50;
			endPiont.y = startPoint.y;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Line;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			aData.Title = "Line annotation 2.";
			aData.Color = color;
			aData.SetLineEndings(PXC_AnnotLineEndingStyle.LE_None, PXC_AnnotLineEndingStyle.LE_OpenArrow);
			aData.set_Border(border);
			annot.Data = aData;

			startPoint.y = startPoint.y - 50;
			endPiont.y = startPoint.y;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Line;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			aData.Title = "Line annotation 3.";
			aData.Color = color;
			aData.FColor = color;
			aData.SetLineEndings(PXC_AnnotLineEndingStyle.LE_ClosedArrow, PXC_AnnotLineEndingStyle.LE_ClosedArrow);
			aData.set_Border(border);
			annot.Data = aData;

			startPoint.y = startPoint.y - 50;
			endPiont.y = startPoint.y;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Line;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			aData.Title = "Line annotation 4.";
			aData.Color = color;
			aData.FColor = color;
			aData.SetLineEndings(PXC_AnnotLineEndingStyle.LE_Circle, PXC_AnnotLineEndingStyle.LE_None);
			aData.set_Border(border);
			annot.Data = aData;

			startPoint.y = startPoint.y - 50;
			endPiont.y = startPoint.y;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Line;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			aData.Title = "Line annotation 5.";
			aData.ShowCaption = true;
			aData.CaptionInLine = true;
			PXC_Size size;
			size.cx = 0;
			size.cy = 10;
			aData.set_CaptionOffset(ref size);
			aData.Contents = "Line annotation 5.";
			aData.Color = color;
			annot.Data = aData;

			startPoint.y = startPoint.y - 50;
			endPiont.y = startPoint.y;
			annot = page.InsertNewAnnot(nText, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_Line;
			aData.SetLinePoints(ref startPoint, ref endPiont);
			aData.Title = "Line annotation 6.";
			aData.Color = color;
			aData.FColor = color;
			aData.LeaderLine = 15.0;
			aData.LeaderLineExtension = 15.0;
			aData.set_Border(border);
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.5. Add Square and Circle annotations")]
		static public int AddSquareAndCircleAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Square and Circle annotations atom for the InsertNewAnnot method
			uint nSquare = pxsInst.StrToAtom("Square");
			uint nCircle = pxsInst.StrToAtom("Circle");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 200;
			rcOut.right = nCX + 200;
			rcOut.top = nCY + 300;
			IPXC_Annotation annot = page.InsertNewAnnot(nSquare, ref rcOut);
			IPXC_AnnotData_SquareCircle aData = annot.Data as IPXC_AnnotData_SquareCircle;
			aData.Title = "Square annotation 1.";
			annot.Data = aData;

			rcOut.bottom -= 150;
			rcOut.top -= 150;
			annot = page.InsertNewAnnot(nSquare, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_SquareCircle;
			aData.Title = "Square annotation 2.";
			IColor color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.SColor = color;
			color.SetRGB(0.15f, 0.5f, 0.12f);
			aData.FColor = color;
			annot.Data = aData;

			rcOut.bottom -= 150;
			rcOut.top -= 150;
			annot = page.InsertNewAnnot(nCircle, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_SquareCircle;
			aData.Title = "Circle annotation 3.";
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.SColor = color;
			color.SetRGB(0.5f, 0.5f, 0.5f);
			aData.FColor = color;
			//Setting dashed border pattern
			PXC_AnnotBorder border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
			border.nWidth = 4.0f;
			border.DashArray = new float[] { 10f, 8f, 6f, 4f, 2f, 2f, 4f, 6f, 8f, 10f};//Width of dashes
			border.nDashCount = 4; //Number of dashes
			aData.set_Border(border);
			annot.Data = aData;

			rcOut.bottom -= 150;
			rcOut.top -= 150;
			annot = page.InsertNewAnnot(nCircle, ref rcOut);
			aData = annot.Data as IPXC_AnnotData_SquareCircle;
			aData.Title = "Circle annotation 4.";
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.SColor = color;
			color.SetRGB(0.5f, 0.5f, 0.5f);
			aData.FColor = color;
			//Setting dashed border pattern
			border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Solid;
			border.nWidth = 5.0f;
			aData.set_Border(border);
			annot.Data = aData;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.6. Add Polygon and Polyline annotations")]
		static public int AddPolygonAndPolylineAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Polygon and Polyline annotations atom for the InsertNewAnnot method
			string sPolygon = "Polygon";
			string sPolyline = "PolyLine";
			uint nText = pxsInst.StrToAtom(sPolygon);

			for (int p = 0; p < 2; p++)
			{
				double nCX = (rcPage.right - rcPage.left) / 2.0;
				double nCY = (rcPage.top - rcPage.bottom) / 2.0;
				PXC_Rect rcOut = new PXC_Rect();
				rcOut.left = nCX - 150;
				rcOut.bottom = nCY + 200;
				rcOut.right = nCX + 150;
				rcOut.top = nCY + 300;
				if (p > 0)
				{
					nText = pxsInst.StrToAtom(sPolyline);
					rcOut.bottom = nCY - 200;
					rcOut.top = nCY - 100;
				}
					
				
				IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
				IPXC_AnnotData_Poly aData = annot.Data as IPXC_AnnotData_Poly;
				aData.Title = (p == 0) ? sPolygon : sPolyline;
				aData.Title += " annotation 1.";
				IPXC_PolygonSrcF poly = aData.Vertices;
				double r = 1.5 * 72.0;
				double a = -90.0;
				poly.Clear();
				for (int i = 0; i < 4 + p; i++)
				{
					PXC_PointF pointF = new PXC_PointF();
					pointF.x = (float)(rcOut.left + r * Math.Cos(a * Math.PI / 180.0));
					pointF.y = (float)(rcOut.bottom - r * Math.Sin(a * Math.PI / 180.0));
					a += 360.0 / 4;
					poly.Insert(ref pointF);
				}
				aData.Vertices = poly;
				IColor color = auxInst.CreateColor(ColorType.ColorType_RGB);
				color.SetRGB(0.5f, 0.2f, 0.2f);
				aData.FColor = color;
				color.SetRGB(0.0f, 0.0f, 0.0f);
				aData.SColor = color;

				annot.Data = aData;

				annot = page.InsertNewAnnot(nText, ref rcOut);
				aData = annot.Data as IPXC_AnnotData_Poly;
				aData.Title = (p == 0) ? sPolygon : sPolyline;
				aData.Title += " annotation 2.";
				poly = aData.Vertices;
				poly.Clear();
				for (int i = 0; i < 6 + p; i++)
				{
					PXC_PointF pointF = new PXC_PointF();
					pointF.x = (float)(nCX + r * Math.Cos(a * Math.PI / 180.0));
					pointF.y = (float)(rcOut.bottom - r * Math.Sin(a * Math.PI / 180.0));
					a += 360.0 / 6;
					poly.Insert(ref pointF);
				}
				aData.Vertices = poly;
				color = auxInst.CreateColor(ColorType.ColorType_RGB);
				color.SetRGB(0.2f, 0.5f, 0.2f);
				aData.FColor = color;
				color.SetRGB(0.0f, 0.0f, 0.0f);
				aData.SColor = color;
				PXC_AnnotBorder border = new PXC_AnnotBorder();
				border.nStyle = PXC_AnnotBorderStyle.ABS_Solid;
				border.nWidth = 5.0f;
				aData.set_Border(border);
				aData.BlendMode = PXC_BlendMode.BlendMode_Multiply;
				annot.Data = aData;


				annot = page.InsertNewAnnot(nText, ref rcOut);
				aData = annot.Data as IPXC_AnnotData_Poly;
				aData.Title = (p == 0) ? sPolygon : sPolyline;
				aData.Title += " annotation 3.";
				poly = aData.Vertices;
				poly.Clear();
				for (int i = 0; i < 8 + p; i++)
				{
					PXC_PointF pointF = new PXC_PointF();
					pointF.x = (float)(rcOut.right + r * Math.Cos(a * Math.PI / 180.0));
					pointF.y = (float)(rcOut.bottom - r * Math.Sin(a * Math.PI / 180.0));
					a += 360.0 / 8;
					poly.Insert(ref pointF);
				}
				aData.Vertices = poly;
				color = auxInst.CreateColor(ColorType.ColorType_RGB);
				color.SetRGB(0.2f, 0.2f, 0.5f);
				aData.FColor = color;
				color.SetRGB(0.0f, 0.0f, 0.0f);
				aData.SColor = color;
				border = new PXC_AnnotBorder();
				border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
				border.nWidth = 3.0f;
				border.DashArray = new float[] { 10f, 8f, 6f, 4f, 2f, 2f, 4f, 6f, 8f, 10f };//Width of dashes
				border.nDashCount = 4; //Number of dashes
				aData.set_Border(border);
				aData.BlendMode = PXC_BlendMode.BlendMode_Multiply;
				annot.Data = aData;
			}
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.7. Add Highlight, Underline, Strikeout and Squiggly annotations")]
		static public int AddTextMarkupAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 4.0 * 3.0;

			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Arial", (uint)PXC_CreateFontFlags.CreateFont_Monospaced, 700);
			CC.SetFontSize(30);
			CC.SetFont(font);
			CC.SetColorRGB(0x00000000);
			for (int i = 0; i < 4; i++)
			{
				CC.ShowTextLine(nCX - 190, nCY- (40 * i), "This is a story of long ago.", -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
			}
			page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			IPXC_PageText Text = page.GetText(null, false);

			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 150;
			rcOut.bottom = nCY - 100;
			rcOut.right = nCX + 150;
			rcOut.top = nCY + 100;
			string[] textMarkups = new string[] { "Highlight", "Underline", "Squiggly", "StrikeOut" };
			//Getting Highlight, Underline, Strikeout and Squiggly annotations atom for the InsertNewAnnot method
			for (int i = 0; i < 4; i++)
			{
				uint nText = pxsInst.StrToAtom(textMarkups[i]);
				IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
				IPXC_AnnotData_TextMarkup aData = annot.Data as IPXC_AnnotData_TextMarkup;
				aData.Title = textMarkups[i] + " annotation 1.";
				IPXC_QuadsF quadsF = Parent.m_pxcInst.CreateQuads();
				uint afafaf = quadsF.Count;
				PXC_RectF rectF = new PXC_RectF();
				Text.GetTextQuads3(0 + (uint)(i * 28), 7, quadsF, out rectF);
				aData.Quads = quadsF;
				annot.Data = aData;

				annot = page.InsertNewAnnot(nText, ref rcOut);
				aData = annot.Data as IPXC_AnnotData_TextMarkup;
				aData.Title = textMarkups[i] + " annotation 2.";
				Text.GetTextQuads3(19 + (uint)(i * 28), 9, quadsF, out rectF);
				IColor color = auxInst.CreateColor(ColorType.ColorType_RGB);
				color.SetRGB(0.0f, 1.0f, 1.0f);
				aData.Color = color;
				aData.Quads = quadsF;
				annot.Data = aData;
			}

			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);
			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.8. Add Popup annotation")]
		static public int AddPopupAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting Popup annotation atom for the InsertNewAnnot method
			uint nSquare = pxsInst.StrToAtom("Square");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 200;
			rcOut.right = nCX + 200;
			rcOut.top = nCY + 300;
			IPXC_Annotation sqAnnot = page.InsertNewAnnot(nSquare, ref rcOut);
			IPXC_AnnotData_SquareCircle aSData = sqAnnot.Data as IPXC_AnnotData_SquareCircle;
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.8f, 0.8f);
			aSData.Color = color;
			aSData.Title = "Square annotation 1.";
			aSData.Contents = "Popup Annotation 1.";
			sqAnnot.Data = aSData;

			//Getting Text annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("Popup");
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_Popup aData = annot.Data as IPXC_AnnotData_Popup;
			aData.Opened = true;
			annot.Data = aData;
			sqAnnot.SetPopup(annot);

			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.9. Add File attachment annotations")]
		static public int AddFile_AttachmentAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			//Getting File attachment annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("FileAttachment");

			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 2.0;
			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 200;
			rcOut.bottom = nCY + 250;
			rcOut.right = nCX - 150;
			rcOut.top = nCY + 300;
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_FileAttachment aData = annot.Data as IPXC_AnnotData_FileAttachment;
			aData.Contents = "FileAttachment Annotation 1.";
			string sFilePath = System.Environment.CurrentDirectory + "\\Documents\\Hobbit.txt";
			IPXC_FileSpec fileSpec = Parent.m_CurDoc.CreateEmbeddFile(sFilePath);
			IPXC_EmbeddedFileStream EFS = fileSpec.EmbeddedFile;
			EFS.UpdateFromFile2(sFilePath);
			aData.FileAttachment = fileSpec;
			annot.Data = aData;

			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations | (int)Form1.eFormUpdateFlags.efuf_Attachments;
		}

		[Description("7.10. Add Redact annotations")]
		static public int AddRedactAnnotation(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IPXC_UndoRedoData urData = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages[0];
			PXC_Rect rcPage = firstPage.get_Box(PXC_BoxType.PBox_PageBox);
			Marshal.ReleaseComObject(firstPage);
			IPXC_Page page = pages.InsertPage(0, ref rcPage, out urData);
			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			double nCX = (rcPage.right - rcPage.left) / 2.0;
			double nCY = (rcPage.top - rcPage.bottom) / 4.0 * 3.0;

			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Arial", (uint)PXC_CreateFontFlags.CreateFont_Monospaced, 700);
			CC.SetFontSize(30);
			CC.SetFont(font);
			CC.SetColorRGB(0x00000000);
			for (int i = 0; i < 4; i++)
			{
				CC.ShowTextLine(nCX - 190, nCY - (40 * i), "This is a story of long ago.", -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
			}
			page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			IPXC_PageText Text = page.GetText(null, false);

			PXC_Rect rcOut = new PXC_Rect();
			rcOut.left = nCX - 150;
			rcOut.bottom = nCY - 100;
			rcOut.right = nCX + 150;
			rcOut.top = nCY + 100;
			//Getting Redact annotation atom for the InsertNewAnnot method
			uint nText = pxsInst.StrToAtom("Redact");
			IPXC_Annotation annot = page.InsertNewAnnot(nText, ref rcOut);
			IPXC_AnnotData_Redaction aData = annot.Data as IPXC_AnnotData_Redaction;
			aData.Title = "Redact annotation 1.";

			IPXC_QuadsF quadsF = Parent.m_pxcInst.CreateQuads();
			uint afafaf = quadsF.Count;
			PXC_RectF rectF = new PXC_RectF();
			Text.GetTextQuads3(8, 75, quadsF, out rectF);
			aData.Quads = quadsF;
			var color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.0f, 0.0f, 0.0f);
			aData.FColor = color;
			aData.SColor = color;
			annot.Data = aData;

			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.11. Remove all of the annotations from the current page")]
		static public int RemoveAnnotations(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[Parent.CurrentPage];
			page.RemoveAnnots(0, page.GetAnnotsCount());
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("7.12. Rotate annotations on the current page")]
		static public int RotateAnnotations(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[Parent.CurrentPage];
			for (uint i = 0; i < page.GetAnnotsCount(); i++)
			{
				IPXC_Annotation annot = page.GetAnnot(i);
				PXC_Point rcRotPnt;
				//We are taking the center point of the annotation as the rotation point
				rcRotPnt.x = annot.get_Rect().left + (annot.get_Rect().right - annot.get_Rect().left) / 2.0f;
				rcRotPnt.y = annot.get_Rect().top - (annot.get_Rect().top - annot.get_Rect().bottom) / 2.0f;
				//We will be adding 45 degrees to the annotation rotation each time
				annot.Handler.RotateAnnot(annot, 45.0f, false, rcRotPnt);
			}
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}
	}
}