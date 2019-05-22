using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("4. Content")]
	public class Content
	{

		delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint color = 0x00000000);
		delegate IPXC_Pattern CreateImagePattern(string str, IPXC_Document Doc, IIXC_Inst g_ImgCore, IMathHelper mathHelper);
		delegate void DrawArrLine(IPXC_ContentCreator CC, double xfrom, double yfrom, double xto, double yto, bool bDashed, bool bArr = true, uint nColor = 0x00000000);
		delegate void DrawCS(IPXC_ContentCreator CC, PXC_Point point, bool bCircle = true, double nWidth = 0.74 * 72.0, bool bArr = true, uint nColor = 0x00000000);
		delegate void DrawN(IPXC_ContentCreator CC, PXC_Point point, IPXC_Font font);
		delegate void FillByGradient(IPXC_Document Doc, IPXC_ContentCreator CC, PXC_Rect rect);
		delegate void CrossArrLine(IPXC_Document Doc, IPXC_ContentCreator CC, PXC_Point p);
		delegate void ChageImages(object contentObj);

		[Description("4.3. Text Rendering Mode")]
		static public void DrawTextRenderingModesOnPage(Form1 Parent)
		{
			const uint argbDarkLime = 0x00008888;
			const uint argbBlack = 0x00000000;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double Width = 0;
				double Height = 0;
				ContCrt.CalcTextSize(fontSize, sText, out Width, out Height, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - Width / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};
			//delegate IPXC_Pattern CreateImagePattern(string str, IPXC_Document Doc, IIXC_Inst g_ImgCore, IMathHelper mathHelper);
			CreateImagePattern crtImgPat = (str, doc, ImgCore, mathHelper) =>
			{
				IPXC_Pattern Patt = null;
				IPXC_Image Img = doc.AddImageFromFile(str);
				PXC_Rect bbox;
				bbox.left = 0;
				bbox.bottom = 0;
				bbox.right = Img.Width * 72.0 / 96.0;
				bbox.top = Img.Height * 72.0 / 96.0;
				IPXC_ContentCreator ContCrt = doc.CreateContentCreator();

				PXC_Matrix im = new PXC_Matrix();
				mathHelper.Matrix_Reset(ref im);
				im = mathHelper.Matrix_Scale(ref im, bbox.right, bbox.top);
				ContCrt.SaveState();
				ContCrt.ConcatCS(im);
				ContCrt.PlaceImage(Img);
				ContCrt.RestoreState();
				Patt = doc.CreateTilePattern(ref bbox);
				IPXC_Content content = ContCrt.Detach();
				content.set_BBox(ref bbox);
				Patt.SetContent(content, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				return Patt;
			};
			//delegate void FillByGradient(IPXC_Document Doc, IPXC_ContentCreator CC, PXC_Rect rect);
			FillByGradient fillByGradient = (Doc, ConCret, rect) =>
			{
				IPXC_GradientStops Stops;
				Stops = Doc.CreateShadeStops();

				Stops.AddStopRGB(0.0, 0x00ff0000);
				Stops.AddStopRGB(1.0, 0x0000ffff);

				IPXC_Shading Shade;
				PXC_Point p0, p1;
				p0.x = rect.left; p0.y = rect.top;
				p1.x = rect.left; p1.y = rect.bottom;

				Shade = Doc.CreateLinearShade(ref p0, ref p1, Stops, 3);

				ConCret.SaveState();
				ConCret.SetShadeAsPattern(Shade, true);
				ConCret.SetStrokeColorRGB(argbBlack);
				ConCret.Rect(rect.left, rect.bottom, rect.right, rect.top);
				ConCret.FillPath(true, false, PXC_FillRule.FillRule_Winding);
				ConCret.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);

			double x = 1 * 72.0;
			double y = rc.top - 1 * 72.0;
			double xs = 3.5 * 72.0;
			double ys = 1 * 72.0;
			double ts = 0.45 * 72.0;

			IPXC_Font Font;
			IPXC_Font Font2;
			Font = Parent.m_CurDoc.CreateNewFont("Impact", 0, 0);
			double fntsize = 25.0;
			string text = "Text Rendering Mode";
			double nWidth = 0.0;
			double nHight = 0.0;

			CC.SaveState();
			CC.SetFont(Font);
			CC.SetFontSize(fntsize);
			CC.CalcTextSize(fntsize, text, out nWidth, out nHight, -1);

			CC.SetFillColorRGB(argbDarkLime);
			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_Fill);
			CC.ShowTextLine(x, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + nWidth / 2, y - ts, "TRM_Fill", 15);

			x += xs;
			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_Stroke);
			CC.ShowTextLine(x, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + nWidth / 2, y - ts, "TRM_Stroke", 15);

			x -= xs; y -= ys;
			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);
			CC.ShowTextLine(x, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + nWidth / 2, y - ts, "TRM_FillStroke", 15);

			x += xs;
			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_None);
			CC.ShowTextLine(x, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + nWidth / 2, y - ts, "TRM_None", 15);

			x -= xs;
			y -= ys;
			double w = 3.2 * 72.0;
			double h = 1 * 72.0;
			//
			text = "ABC";
			Font2 = Parent.m_CurDoc.CreateNewFont("Arial Black", 0, 0);
			fntsize = 50;
			CC.SetFontSize(fntsize);
			CC.SetFont(Font2);

			PXC_Rect pr;
			pr.left = x;
			pr.right = pr.left + w;
			pr.top = y;
			pr.bottom = pr.top - h;

			fillByGradient(Parent.m_CurDoc, CC, pr);

			CC.SaveState();
			CC.SetCharSpace(2.0);
			CC.SetTextScale(150.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_Stroke);
			CC.ShowTextLine(x + 0.35 * 72.0, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + w / 2, y - 1.1 * 72.0, "TRM_Stroke", 15);

			x += xs;
			pr.left += xs;
			pr.right += xs;

			CC.SaveState();
			CC.SetCharSpace(2.0);
			CC.SetTextScale(150.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_Clip_Stroke);
			CC.ShowTextLine(x + 0.35 * 72.0, y, text, -1, 0);
			fillByGradient(Parent.m_CurDoc, CC, pr);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + w / 2, y - 1.1 * 72.0, "TRM_Clip_Stroke", 15);

			text = "FILL";
			CC.SetFont(Font);
			CC.SetFontSize(120);


			x = rc.right / 4; y -= 1.4 * 72.0;
			pr.left += xs; pr.right += xs;
			CC.SaveState();
			CC.SetFillColorRGB(argbDarkLime);
			CC.SetCharSpace(2.0);
			CC.SetTextScale(150.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);
			CC.ShowTextLine(x + 0.35 * 72.0, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, rc.right / 2 + 15, y - 1.8 * 72.0, "SOLID COLOR", 15);

			x = rc.right / 4; y -= 1.8 * 72.0;
			pr.left += xs; pr.right += xs;
			CC.SaveState();
			IPXC_Pattern Pat = Parent.m_CurDoc.GetStdTilePattern((PXC_StdPatternType)4);
			CC.SetPatternRGB(Pat, true, argbDarkLime);

			CC.SetCharSpace(2.0);
			CC.SetTextScale(150.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);
			CC.ShowTextLine(x + 0.35 * 72.0, y, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, rc.right / 2 + 15, y - 1.8 * 72.0, "HatchType_Horizontal PATTERN", 15);


			x = rc.right / 4;
			y -= 1.8 * 72.0;
			pr.left += xs;
			pr.right += xs;

			CC.SaveState();
			IIXC_Inst Ixc_Inst = (IIXC_Inst)Parent.m_pxcInst.GetExtension("IXC");
			IAUX_Inst Aux_Inst = Parent.m_pxcInst.GetExtension("AUX");
			IMathHelper math = Aux_Inst.MathHelper;
			Pat = crtImgPat(System.Environment.CurrentDirectory + "\\Images\\CoreAPI_32.ico", Parent.m_CurDoc, Ixc_Inst, math);
			CC.SetPatternRGB(Pat, true, argbDarkLime);
			CC.SetCharSpace(2.0);
			CC.SetTextScale(150.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);
			CC.ShowTextLine(x + 0.35 * 72.0, y, text, -1, 0);
			Pat = null;
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, rc.right / 2 + 15, y - 1.8 * 72.0, "IMAGE PATTERN FOR STROKE", 15);

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.4. Text Character and Word Spacing")]
		static public void DrawTextWithDifferentSpacingOnPage(Form1 Parent)
		{
			const uint argbDarkLime = 0x00008888;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 1000);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double Width = 0;
				double Height = 0;
				ContCrt.CalcTextSize(fontSize, sText, out Width, out Height, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - Width / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);

			double x = 1.8 * 72.0;
			double y = rc.top - 0.5 * 72.0;
			double xs = 2.5 * 72.0;
			double ys = 0.85 * 72.0;

			string[] txts = new string[]
				{
					"-2 PT",
					"DEFAULT",
					"2 PT",
					"-10 PT",
					"DEFAULT",
					"10 PT"
				};
			for (int i = 0; i < 6; i++)
			{
				drawTitle(Parent.m_CurDoc, CC, x, y - i * ys - 5, txts[i], 15);
			}

			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);

			CC.SetFillColorRGB(argbDarkLime);
			string text = "Character Spacing";
			CC.SetCharSpace(-2.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetCharSpace(0);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetCharSpace(2.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);
			CC.SetCharSpace(0);

			y -= ys;
			text = "Word Spacing";
			CC.SetWordSpace(-10);
			drawTitle(Parent.m_CurDoc, CC, x + xs - 25, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetWordSpace(0);
			drawTitle(Parent.m_CurDoc, CC, x + xs - 25, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetWordSpace(10);
			drawTitle(Parent.m_CurDoc, CC, x + xs - 25, y + 2, text, 25, 400, argbDarkLime);
			CC.SetWordSpace(0);

			CC.RestoreState();

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.5. Text Scaling, Subscript and Superscript")]
		static public void DrawTextWithScaleSubSuperscriptOnPage(Form1 Parent)
		{
			const uint argbDarkLime = 0x00008888;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 1000);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double Width = 0;
				double Height = 0;
				ContCrt.CalcTextSize(fontSize, sText, out Width, out Height, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - Width / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);

			double x = 1.8 * 72.0;
			double y = rc.top - 0.5 * 72.0;
			double xs = 2.5 * 72.0;
			double ys = 0.85 * 72.0;


			string[] txts = new string[]
				{
					"80%",
					"DEFAULT (100%)",
					"120%",
					"+10 PT",
					"-10 PT",
					"±10 PT"
				};

			for (int i = 0; i < 6; i++)
			{
				drawTitle(Parent.m_CurDoc, CC, x, y - i * ys - 5, txts[i], 15);
			}

			CC.SaveState();
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_FillStroke);
			CC.SetFillColorRGB(argbDarkLime);
			string text = "Horizontal Scaling";
			CC.SetTextScale(80);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetTextScale(100);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);

			y -= ys;
			CC.SetTextScale(120);
			drawTitle(Parent.m_CurDoc, CC, x + xs, y + 2, text, 25, 400, argbDarkLime);
			CC.SetTextScale(100);

			y -= ys;
			text = "This text is ";
			drawTitle(Parent.m_CurDoc, CC, x + xs - 40, y + 2, text, 25, 400, argbDarkLime);
			CC.SetTextRise(10.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs + 110, y + 2, "superscripted", 25, 400, argbDarkLime);

			CC.SetTextRise(0.0);
			y -= ys;
			drawTitle(Parent.m_CurDoc, CC, x + xs - 40, y + 2, text, 25, 400, argbDarkLime);
			CC.SetTextRise(-10);
			drawTitle(Parent.m_CurDoc, CC, x + xs + 100, y + 2, "subscripted", 25, 400, argbDarkLime);

			CC.SetTextRise(0.0);
			y -= ys;
			drawTitle(Parent.m_CurDoc, CC, x + xs - 82, y + 2, "This", 25, 400, argbDarkLime);
			CC.SetTextRise(-10.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs - 28, y + 2, "text", 25, 400, argbDarkLime);
			CC.SetTextRise(10.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs + 41, y + 2, "moves", 25, 400, argbDarkLime);
			CC.SetTextRise(0.0);
			drawTitle(Parent.m_CurDoc, CC, x + xs + 128, y + 2, "around", 25, 400, argbDarkLime);

			CC.RestoreState();

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.9. Draw Arcs")]
		static public void DrawArcsOnPage(Form1 Parent)
		{
			const uint argbBlack = 0x00000000;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IAUX_Inst auxInst = (IAUX_Inst)Parent.m_pxcInst.GetExtension("AUX");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages.InsertPage(0, ref rc, out urData);
			double rx = 1 * 72.0;
			double ry = 1 * 72.0;
			double X = 2.3 * 72.0;
			double Y = rc.top - 2.5 * 72.0;

			CC.SetLineWidth(0.5);
			CC.SetStrokeColorRGB(argbBlack);
			IColor clr = auxInst.CreateColor(ColorType.ColorType_Gray);
			clr.SetGray(0.94f);
			CC.SetColor(clr, null);

			PXC_Rect rect;
			rect.left = X - rx;
			rect.top = Y + ry;
			rect.right = X + rx;
			rect.bottom = Y - ry;

			CC.Ellipse(rect.left, rect.bottom, rect.right, rect.top);
			CC.FillPath(false, true, PXC_FillRule.FillRule_Winding);
			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "ELLIPSE", 15);

			rect.left += 4 * 72.0;
			rect.right += 4 * 72.0;
			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "ARC", 15);
			CC.Arc(rect, 0, 270, true);
			CC.StrokePath(false);

			rect.left -= 4 * 72.0;
			rect.right -= 4 * 72.0;
			rect.top -= 3 * 72.0;
			rect.bottom -= 3 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "CLOSED ARC", 15);
			CC.SetStrokeColorRGB(argbBlack);
			CC.Arc(rect, 0, 270.0, true);
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);

			rect.left += 4 * 72.0;
			rect.right += 4 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "PIE", 15);
			CC.Pie(rect, 0, 270.0);
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);

			rect.left -= 4 * 72.0;
			rect.right -= 4 * 72.0;
			rect.top -= 3 * 72.0;
			rect.bottom -= 3 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "CHORD", 15);
			CC.SetStrokeColorRGB((uint)(200 << 0) + (200 << 8) + (200 << 16));
			CC.SetDash(3, 3, 0);
			CC.Arc(rect, 0, 270.0, true);
			CC.StrokePath(false);

			CC.NoDash();
			CC.SetStrokeColorRGB(argbBlack);
			CC.Chord(rect, 0, 270.0);
			CC.StrokePath(false);

			rect.left += 4 * 72.0;
			rect.right += 4 * 72.0;

			PXC_Point pnt;
			pnt.x = (rect.left + rect.right) / 2;
			pnt.y = (rect.top + rect.bottom) / 2;
			drawTitle(Parent.m_CurDoc, CC, (rect.left + rect.right) / 2.0, rect.bottom - 0.2 * 72.0, "CIRCLE", 15);
			CC.Circle(pnt.x, pnt.y, ry);
			CC.FillPath(false, true, PXC_FillRule.FillRule_Winding);

			firstPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			Marshal.ReleaseComObject(firstPage);

			IPXC_Page secondPage = pages.InsertPage(0, ref rc, out urData);
			PXC_Point center;
			double r0 = 1.5 * 72.0;
			double r = r0;
			double dr = 6.0;
			int i = 0;

			center.x = 2.3 * 72.0;
			center.y = rc.top - 2.5 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, center.x, center.y - r - 18.0, "FILLED, STROKE", 15);
			CC.SetLineWidth(0.5);
			CC.SetStrokeColorRGB(argbBlack);

			for (i = 360; i > 0; i -= 30)
			{
				CC.MoveTo(center.x, center.y);
				CC.CircleArc(center.x, center.y, r, 0.0, i, false);
				uint c = (uint)Math.Round(i * 240.0 / 360.0);
				CC.SetFillColorRGB(((c << 0) + (c << 8) + (255 << 16)));
				CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);
				r -= dr;
			}

			r = r0;
			center.x += 4 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, center.x, center.y - r - 18.0, "FILLED, NO STROKE", 15);
			for (i = 360; i > 0; i -= 30)
			{
				CC.MoveTo(center.x, center.y);
				CC.CircleArc(center.x, center.y, r, 0.0, i, false);
				uint c = (uint)Math.Round(i * 240.0 / 360.0);
				CC.SetFillColorRGB(((c << 0) + (c << 8) + (255 << 16)));
				CC.FillPath(true, false, PXC_FillRule.FillRule_Winding);
				r -= dr;
			}

			r = r0;
			center.x -= 4 * 72.0;
			center.y -= 4.5 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, center.x, center.y - r - 18.0, "STROKE, NOT CLOSED PATH", 15);

			CC.SetStrokeColorRGB(argbBlack);
			for (i = 360; i > 0; i -= 30)
			{
				CC.CircleArc(center.x, center.y, r, 0, i, false);
				CC.StrokePath(false);
				r -= dr;
			}

			r = r0;
			center.x += 4 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, center.x, center.y - r - 18.0, "STROKE, CLOSED PATH", 15);

			for (i = 360; i > 0; i -= 30)
			{
				CC.MoveTo(center.x, center.y);
				CC.CircleArc(center.x, center.y, r, 0, i, false);
				CC.StrokePath(true);
				r -= dr;
			}

			secondPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(secondPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.10. Draw Polygons and Curves")]
		static public void DrawPolygonsAndCurvesOnPage(Form1 Parent)
		{
			const uint argbBlack = 0x00000000;
			const uint argbDarkLime = 0x00008888;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};


			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);

			double x = 2.2 * 72.0;
			double y = rc.top - 2.5 * 72.0;
			double r = 1.2 * 72.0;

			double b = 1.3333333;
			double br = b * r;
			double[] p = new double[12];
			p[0] = p[8] = p[10] = x - r;
			p[1] = p[3] = y + br;
			p[2] = p[4] = p[6] = x + r;
			p[5] = p[11] = y;
			p[7] = p[9] = y - br;

			drawTitle(Parent.m_CurDoc, CC, x, y - r - 0.1 * 72.0, "FAST-BEZIER \"CIRCLE\"", 15);

			CC.SetLineWidth(1.0);
			CC.SetStrokeColorRGB(argbBlack);
			CC.MoveTo(x - r, y);
			CC.PolyCurveSA(p, false);
			CC.StrokePath(false);

			x = 6.2 * 72.0;
			y = rc.top - 2.5 * 72.0;
			double rr = r / 2.0;
			double a = 0.05 * 72.0;

			PXC_Point center;
			center.x = x;
			center.y = y;
			PXC_Point p1 = center;
			PXC_Point p2 = center;

			p1.y -= rr;
			p2.y += rr;

			drawTitle(Parent.m_CurDoc, CC, x, y - r - 0.1 * 72.0, "CHINA MONAD", 15);

			CC.SetLineWidth(1.0);
			CC.SetColorRGB(argbBlack);

			CC.CircleArc(center.x, center.y, r, 90.0, -90.0, true);
			CC.CircleArc(p1.x, p1.y, rr, 270.0, 90.0, true);
			CC.CircleArc(p2.x, p2.y, rr, -90.0, 90.0, true);
			CC.Circle(p1.x, p1.y, 0.1 * 72.0);
			CC.Circle(p2.x, p2.y, 0.1 * 72.0);
			CC.FillPath(false, true, PXC_FillRule.FillRule_EvenOdd);
			CC.CircleArc(center.x, center.y, r, 90.0, 270.0, true);
			CC.StrokePath(false);

			const int ncnt = 8;
			x = 2.2 * 72.0;
			y = rc.top - 6 * 72.0;
			r = 1.3 * 72.0;
			double[] xy = new double[ncnt * 2];

			drawTitle(Parent.m_CurDoc, CC, x, y - r - 0.1 * 72.0, "POLYGON", 15);

			a = -90;
			for (int i = 0; i < ncnt; i++)
			{
				xy[i * 2] = x + r * Math.Cos(a * Math.PI / 180.0);
				xy[i * 2 + 1] = y - r * Math.Sin(a * Math.PI / 180.0);
				a += 360.0 / ncnt;
			}
			CC.PolygonSA(xy, true);
			CC.SetFillColorRGB(argbDarkLime);
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);

			double xorig = 6.2 * 72.0;
			double yorig = rc.top - 6 * 72.0;
			r = 1.2 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, rc.right - x, y - r - 0.2 * 72.0, "LISSAJOUS FIGURE", 15);

			CC.SetFillColorRGB(argbDarkLime);
			CC.SetStrokeColorRGB(argbBlack);
			for (int i = 0; i < 200; i++)
			{
				double ang = Math.PI * i / 100.0;
				x = xorig + r * Math.Cos(3 * ang);
				y = yorig - r * Math.Sin(5 * ang);
				if (i > 0)
					CC.LineTo(x, y);
				else
					CC.MoveTo(x, y);
			}
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);

			x = 2.2 * 72.0;
			y = rc.top - 9 * 72.0;
			double w = 2.5 * 72.0;
			double h = 1.2 * 72.0;

			drawTitle(Parent.m_CurDoc, CC, x, y - r - 0.1 * 72.0, "RECTANGLE", 15);

			CC.SetFillColorRGB(argbDarkLime);
			CC.SetStrokeColorRGB(argbBlack);
			CC.Rect(x - w / 2, y - h / 2, x + w / 2, y + h / 2);
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);

			x = 6.2 * 72.0;
			y = rc.top - 9 * 72.0;
			w = 2.5 * 72.0;
			h = 1.2 * 72.0;
			double ew = w / 5.0;
			double eh = h / 5.0;

			PXC_Rect rect;
			rect.left = x - w / 2;
			rect.top = y + h / 2;
			rect.right = x + w / 2;
			rect.bottom = y - h / 2;

			drawTitle(Parent.m_CurDoc, CC, x, y - r - 0.1 * 72.0, "ROUND RECTANGLE", 15);

			CC.SetFillColorRGB(argbDarkLime);
			CC.SetStrokeColorRGB(argbBlack);
			CC.RoundRect(rect.left, rect.bottom, rect.right, rect.top, ew, eh);
			CC.FillPath(true, true, PXC_FillRule.FillRule_Winding);
			CC.SetStrokeColorRGB(((255 << 0) + (255 << 8) + (255 << 16)));
			CC.SetLineWidth(0.0);
			CC.SetDash(1, 1, 0);
			rect.right = rect.left + ew;
			rect.bottom = rect.top - eh;
			CC.Ellipse(rect.left, rect.bottom, rect.right, rect.top);
			CC.StrokePath(false);

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.6. Fills and Gradients")]
		static public void DrawFillingsOnPage(Form1 Parent)
		{
			const uint argbBlack = 0x00000000;
			const uint argbDarkLime = 0x00008888;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages.InsertPage(0, ref rc, out urData);

			double x = 2.0 * 72.0;
			double y = rc.top - 2.0 * 72.0;
			double r = 1.0 * 72.0;
			double rr;

			string[] RuleTitles = { "NONZERO WINDING NUMBER RULE", "EVEN-ODD RULE" };
			PXC_FillRule[] rules = { PXC_FillRule.FillRule_Winding, PXC_FillRule.FillRule_EvenOdd };

			for (int i = 0; i < 2; i++)
			{
				x = 2.0 * 72.0;

				PXC_FillRule rule = rules[i];
				drawTitle(Parent.m_CurDoc, CC, (rc.right + rc.left) / 2, y - r - 15, RuleTitles[i], 15);

				const int num = 5;
				double[] points = new double[num * 2];

				double a = -90;
				for (int j = 0; j < num; j++)
				{
					points[j * 2] = x + r * Math.Cos(a * Math.PI / 180.0);
					points[j * 2 + 1] = y - r * Math.Sin(a * Math.PI / 180.0);
					a += 2.0 * (360 / num);
				}
				CC.PolygonSA(points, true);

				CC.SetStrokeColorRGB(argbBlack);
				CC.SetFillColorRGB(argbDarkLime);
				CC.FillPath(true, true, rule);

				x = (rc.right + rc.left) / 2;
				rr = r;
				PXC_Rect ps = new PXC_Rect();

				ps.left = x - rr;
				ps.bottom = y - rr;
				ps.right = x + rr;
				ps.top = y + rr;

				CC.Arc(ps, 0.0, 360.0, true);
				rr = r / 2;

				ps.left = x - rr;
				ps.bottom = y - rr;
				ps.right = x + rr;
				ps.top = y + rr;

				CC.Arc(ps, 0.0, 360.0, true);
				CC.FillPath(true, true, rule);

				x = rc.right - 2.0 * 72.0;

				ps.left = x - rr;
				ps.bottom = y - rr;
				ps.right = x + rr;
				ps.top = y + rr;

				CC.Arc(ps, 0.0, 360.0, true);
				rr = r / 2;

				ps.left = x - rr;
				ps.bottom = y - rr;
				ps.right = x + rr;
				ps.top = y + rr;

				CC.Arc(ps, 0.0, 360.0, true);
				CC.FillPath(true, true, rule);

				y -= 3.0 * 72.0;
			}
			firstPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			Marshal.ReleaseComObject(firstPage);

			IPXC_Page secondPage = pages.InsertPage(0, ref rc, out urData);

			double w = (rc.right - rc.left - 3 * 72.0) / 2.0;
			double h = 1 * 72.0;
			y = rc.top - 1.0 * 72.0 - h;
			x = rc.left + 1.0 * 72.0;

			IPXC_GradientStops Stops = Parent.m_CurDoc.CreateShadeStops();

			Stops.AddStopRGB(0.0, 0x4bb5fd);
			Stops.AddStopRGB(0.5, 0x66b9ff);
			Stops.AddStopRGB(0.5, 0xffd900);
			Stops.AddStopRGB(1.0, 0xfff500);

			PXC_Point p0, p1;
			p0.x = x;
			p0.y = y + h;
			p1.x = x;
			p1.y = y;

			IPXC_Shading Shade = Parent.m_CurDoc.CreateLinearShade(p0, p1, Stops, 3);

			CC.SaveState();
			CC.Rect(x, y, x + w, y + h);
			CC.ClipPath(PXC_FillRule.FillRule_Winding, true);
			CC.Shade(Shade);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, x + w / 2, y - 0.1 * 72.0, "Linear Gradient", 15);

			x = rc.left + 5 * 72.0 + w / 2;
			r = h / 2;
			Shade = null;
			Stops.Reset();
			p0.x = x;
			p0.y = y + r;
			p1.x = p0.x - 0.5 * r;
			p1.y = p0.y + 0.5 * r;
			Stops.AddStopRGB(0.0, 0x5c5c5c);
			Stops.AddStopRGB(1.0, 0xf2f2f2);
			Shade = Parent.m_CurDoc.CreateRadialShade(p0, p1, h / 2, 0.0, Stops, 0);
			CC.Shade(Shade);
			drawTitle(Parent.m_CurDoc, CC, x, y - 0.1 * 72.0, "Radial Gradient", 15);

			secondPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(secondPage);

			IPXC_Page thirdPage = pages.InsertPage(0, ref rc, out urData);
			rc.left = 72.0;
			rc.top = 800 - 72.0;
			rc.right = 600 - 72.0;
			rc.bottom = 800 - 3.5 * 72.0;
			h = rc.bottom - rc.top;

			drawTitle(Parent.m_CurDoc, CC, 600 / 2, rc.bottom - 0.2 * 72.0, "GRADIENT FILL (GRADIENT_FILL_RECT_H)", 15);
			IPXC_GradientStops stops;
			stops = Parent.m_CurDoc.CreateShadeStops();

			stops.AddStopRGB(0.0, 0x00000000);
			stops.AddStopRGB(0.33, 0x000000ff);
			stops.AddStopRGB(0.66, 0x0000ff00);
			stops.AddStopRGB(1.0, 0x00ff0000);

			IPXC_Shading shade;
			PXC_Point point0, point1;
			point0.x = rc.left;
			point0.y = rc.bottom;
			point1.x = rc.right;
			point1.y = rc.bottom;
			shade = Parent.m_CurDoc.CreateLinearShade(ref point0, ref point1, stops, 6);

			double[] xy = new double[16];
			xy[0] = rc.left;
			xy[1] = rc.top;
			xy[2] = rc.left + (rc.right - rc.left) / 3;
			xy[3] = rc.top;
			xy[4] = rc.left + (rc.right - rc.left) / 3;
			xy[5] = rc.top - (rc.top - rc.bottom) / 8;
			xy[6] = rc.right;
			xy[7] = rc.top - (rc.top - rc.bottom) / 8;
			xy[8] = rc.right;
			xy[9] = rc.bottom;
			xy[10] = rc.left + ((rc.right - rc.left) / 3) * 2;
			xy[11] = rc.bottom;
			xy[12] = rc.left + ((rc.right - rc.left) / 3) * 2;
			xy[13] = rc.bottom + (rc.top - rc.bottom) / 8;
			xy[14] = rc.left;
			xy[15] = rc.bottom + (rc.top - rc.bottom) / 8;

			CC.SaveState();
			CC.SetShadeAsPattern(shade, true);
			CC.PolygonSA(xy, true);
			CC.SetStrokeColorRGB(argbBlack);
			CC.FillPath(true, false, PXC_FillRule.FillRule_Winding);
			CC.RestoreState();

			string text = "PDF-XCHANGE";
			IPXC_Font Font = Parent.m_CurDoc.CreateNewFont("Impact", 0, 0);
			CC.SetFont(Font);
			CC.SetFontSize(120);

			stops = Parent.m_CurDoc.CreateShadeStops();

			stops.AddStopRGB(0.0, 0x00000000);
			stops.AddStopRGB(0.33, 0x000000ff);
			stops.AddStopRGB(0.66, 0x0000ff00);
			stops.AddStopRGB(1.0, 0x00ff0000);

			rc.top = 6.94 * 72.0;
			rc.bottom = 5.63 * 72.0;

			point0.x = rc.left;
			point0.y = rc.top;
			point1.x = rc.left;
			point1.y = rc.bottom;

			shade = Parent.m_CurDoc.CreateLinearShade(ref point0, ref point1, stops, 3);

			CC.SaveState();
			CC.SetShadeAsPattern(shade, true);
			CC.SetTextScale(70.0);
			CC.SetTextRenderMode(PXC_TextRenderingMode.TRM_Fill);
			CC.ShowTextLine(72, y - 1.8 * 72.0, text, -1, 0);
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 600 / 2, y - 3.8 * 72.0, "GRADIENT FILL (GRADIENT_FILL_RECT_V)", 15);

			thirdPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(thirdPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.7. Patterns")]
		static public void DrawPatternsOnPage(Form1 Parent)
		{
			const uint argbBlack = 0x00000000;
			const uint argbDarkLime = 0x00008888;
			//delegate IPXC_Pattern CreateImagePattern(string str, IPXC_Document Doc, IIXC_Inst g_ImgCore, IMathHelper mathHelper);
			CreateImagePattern crtImgPat = (str, doc, ImgCore, mathHelper) =>
			{
				IPXC_Pattern Patt = null;
				IPXC_Image Img = doc.AddImageFromFile(str);
				PXC_Rect bbox;
				bbox.left = 0;
				bbox.bottom = 0;
				bbox.right = Img.Width * 72.0 / 96.0;
				bbox.top = Img.Height * 72.0 / 96.0;
				IPXC_ContentCreator ContCrt = doc.CreateContentCreator();

				PXC_Matrix im = new PXC_Matrix();
				mathHelper.Matrix_Reset(ref im);
				im = mathHelper.Matrix_Scale(ref im, bbox.right, bbox.top);
				ContCrt.SaveState();
				ContCrt.ConcatCS(im);
				ContCrt.PlaceImage(Img);
				ContCrt.RestoreState();
				Patt = doc.CreateTilePattern(ref bbox);
				IPXC_Content content = ContCrt.Detach();
				content.set_BBox(ref bbox);
				Patt.SetContent(content, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				return Patt;
			};
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IIXC_Inst Ixc_Inst = (IIXC_Inst)Parent.m_pxcInst.GetExtension("IXC");
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);

			double w = (rc.right - rc.left - 3 * 72.0) / 2.0;
			double h = 1 * 72.0;
			double Y = rc.top - 1 * 72.0 - h;
			double dy = 2 * 72.0;
			double[] X = { rc.left + 1.0 * 72.0, rc.left + 5 * 72.0 };

			CC.SetLineWidth(1.0);
			CC.SetStrokeColorRGB(argbBlack);
			CC.SetFillColorRGB(argbDarkLime);

			CC.Rect(X[0], Y, X[0] + w, Y + h);
			CC.StrokePath(false);
			drawTitle(Parent.m_CurDoc, CC, X[0] + w / 2, Y - 0.1 * 72.0, "STROKE, NO FILL", 15);

			CC.Rect(X[1], Y, X[1] + w, Y + h);
			CC.FillPath(false, false, PXC_FillRule.FillRule_Winding);
			drawTitle(Parent.m_CurDoc, CC, X[1] + w / 2, Y - 0.1 * 72.0, "FILL, NO STROKE", 15);

			Y -= dy;

			CC.Rect(X[0], Y, X[0] + w, Y + h);
			CC.FillPath(false, true, PXC_FillRule.FillRule_Winding);
			drawTitle(Parent.m_CurDoc, CC, X[0] + w / 2, Y - 0.1 * 72.0, "STROKE & FILL", 15);
			string[] PatternTitles =
			{
				"PATTERN FILL: CrossHatch",
				"PATTERN FILL: CrossDiagonal",
				"PATTERN FILL: DiagonalLeft",
				"PATTERN FILL: DiagonalRight",
				"PATTERN FILL: Horizontal",
				"PATTERN FILL: Vertical"
			};

			int k = 1;
			IPXC_Pattern Pat;
			for (int i = (int)PXC_StdPatternType.StdPattern_CrossHatch; i <= (int)PXC_StdPatternType.StdPattern_Vertical; i++)
			{
				Pat = Parent.m_CurDoc.GetStdTilePattern((PXC_StdPatternType)i);
				CC.SetPatternRGB(Pat, true, argbDarkLime);
				Pat = null;
				CC.Rect(X[k], Y, X[k] + w, Y + h);
				CC.FillPath(false, true, PXC_FillRule.FillRule_Winding);
				drawTitle(Parent.m_CurDoc, CC, X[k] + w / 2, Y - 0.1 * 72.0, PatternTitles[i], 15);
				k ^= 1;
				if (k == 0)
					Y -= dy;
			}
			IAUX_Inst Aux_Inst = Parent.m_pxcInst.GetExtension("AUX");
			IMathHelper math = Aux_Inst.MathHelper;
			Pat = crtImgPat(System.Environment.CurrentDirectory + "\\Images\\CoreAPI_32.ico", Parent.m_CurDoc, Ixc_Inst, math);
			CC.SetPatternRGB(Pat, true, argbDarkLime);
			CC.Rect(X[k], Y, X[k] + w, Y + h);
			CC.FillPath(false, true, PXC_FillRule.FillRule_Winding);
			drawTitle(Parent.m_CurDoc, CC, X[1] + w / 2, Y - 0.1 * 72.0, "PATTERN FILL: Image", 15);

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.1. Coordinate System Transformations (matrix usages)")]
		static public void AddCoordinateSystemTransformations(Form1 Parent)
		{
			uint argbGray = 0x00bbbbbb;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 700);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};
			//delegate void DrawArrLine(IPXC_ContentCreator CC, double xfrom, double yfrom, double xto, double yto, double linewidth, bool bDashed);
			DrawArrLine drawArrLine = (ContCret, xfrom, yfrom, xto, yto, bDashes, bArr, color) =>
			{
				ContCret.SaveState();
				ContCret.SetLineWidth(1.0);
				ContCret.SetStrokeColorRGB(color);

				ContCret.NoDash();
				if (bDashes)
				{
					ContCret.SetDash(6.0, 6.0, 5);
				}

				ContCret.MoveTo(xfrom, yfrom);
				ContCret.LineTo(xto, yto);
				ContCret.StrokePath(true);
				ContCret.RestoreState();

				if (xto > xfrom && bArr)
				{
					double[] xy = new double[6];
					xy[0] = xto - 2;
					xy[1] = yto + 1;
					xy[2] = xto + 2;
					xy[3] = yto;
					xy[4] = xto - 2;
					xy[5] = yto - 1;
					ContCret.PolygonSA(xy, true);
					ContCret.StrokePath(true);
				}
				else if (yto > yfrom && bArr)
				{
					double[] xy = new double[6];
					xy[0] = xto + 1;
					xy[1] = yto - 2;
					xy[2] = xto;
					xy[3] = yto + 2;
					xy[4] = xto - 1;
					xy[5] = yto - 2;
					ContCret.PolygonSA(xy, true);
					ContCret.StrokePath(true);
				}

			};
			//delegate void DrawCS(IPXC_ContentCreator CC, double x0, double y0, double w, double h);
			DrawCS drawCS = (ContCret, point, bCircle, nWidth, bArr, color) =>
			{
				drawArrLine(ContCret, point.x - nWidth, point.y, point.x + nWidth, point.y, false, bArr, color);
				drawArrLine(ContCret, point.x, point.y - nWidth, point.x, point.y + nWidth, false, bArr, color);
				if (bCircle)
				{
					ContCret.Circle(point.x, point.y, 1);
					ContCret.StrokePath(true);
				}
			};
			//delegate void DrawN(IPXC_ContentCreator CC, double cx, double baseLineY);
			DrawN drawN = (ContCret, point, font) =>
			{
				string text = "n";
				ContCret.SetLineWidth(0.5);
				ContCret.SetFont(font);
				ContCret.SetFontSize(144);
				ContCret.SetTextRenderMode(PXC_TextRenderingMode.TRM_Stroke);
				ContCret.ShowTextLine(point.x + 0.1 * 72.0, point.y + 1.782 * 72.0, text);
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages.InsertPage(0, ref rc, out urData);
			IAUX_Inst Aux_Inst = Parent.m_pxcInst.GetExtension("AUX");
			IMathHelper math = Aux_Inst.MathHelper;
			IPXC_Font Font = Parent.m_CurDoc.CreateNewFont("Times New Roman", 0, 1000);
			PXC_Matrix globalMatrix = new PXC_Matrix();
			PXC_Matrix contentMatrix = new PXC_Matrix();
			PXC_Point p;
			p.x = 0;
			p.y = 0;

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2 * 72.0, rc.top - 3.5 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p);
				drawArrLine(CC, p.x - 0.5 * 72.0, p.y, p.x - 0.5 * 72.0, p.y + 72.0, true);
				drawArrLine(CC, p.x, p.y - 0.5 * 72.0, p.x + 72, p.y - 0.5 * 72.0, true);
				drawTitle(Parent.m_CurDoc, CC, p.x - 0.7 * 72.0, p.y + 0.5 * 72.0, "t", 15, 700);
				drawTitle(Parent.m_CurDoc, CC, p.x - 0.6 * 72.0, p.y + 0.35 * 72.0, "y", 10);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.35 * 72.0, p.y - 0.6 * 72.0, "t", 15, 700);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.45 * 72.0, p.y - 0.7 * 72.0, "x", 10);
				CC.SaveState();
				{
					//Translate contentMatrix from origin to given piont
					contentMatrix = math.Matrix_Translate(ref contentMatrix, 0.5 * 72.0, 1.0 * 72.0);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.5 * 72.0, rc.top - 5 * 72.0, "TRANSLATION", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.5 * 72.0, rc.top - 3.5 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p);
				drawArrLine(CC, p.x - 0.5 * 72.0, p.y, p.x - 0.5 * 72.0, p.y + 1.5 * 72.0, true);
				drawTitle(Parent.m_CurDoc, CC, p.x - 0.7 * 72.0, p.y + 0.5 * 72.0, "S", 15, 700);
				drawTitle(Parent.m_CurDoc, CC, p.x - 0.6 * 72.0, p.y + 0.35 * 72.0, "y", 10);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.35 * 72.0, p.y - 0.6 * 72.0, "S", 15, 700);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.45 * 72.0, p.y - 0.7 * 72.0, "x", 10);
				drawArrLine(CC, p.x, p.y - 0.5 * 72.0, p.x + 1.3 * 72, p.y - 0.5 * 72.0, true);
				CC.SaveState();
				{
					//Scale contentMatrix from origin to given size
					contentMatrix = math.Matrix_Scale(ref contentMatrix, 1.3, 1.7);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 5.7 * 72.0, rc.top - 5 * 72.0, "SCALING", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2.5 * 72.0, rc.top - 7.5 * 72.0);

				CC.ConcatCS(globalMatrix);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.5 * 72.0, p.y + 0.3 * 72.0, "Θ", 15);
				CC.CircleArc(p.x, p.y, 30, 0, 45);
				drawCS(CC, p);
				CC.SaveState();
				{
					//Rotate contentMatrix from origin to given angle
					contentMatrix = math.Matrix_Rotate(ref contentMatrix, 45);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.5 * 72.0, rc.top - 8.5 * 72.0, "ROTATION", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.7 * 72.0, rc.top - 7.5 * 72.0);
				CC.ConcatCS(globalMatrix);
				CC.CircleArc(p.x, p.y, 30, 0, 20);
				CC.CircleArc(p.x, p.y, 30, 90, 70);
				drawCS(CC, p);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.1 * 72.0, p.y + 0.65 * 72.0, "ß", 15);
				drawTitle(Parent.m_CurDoc, CC, p.x + 0.5 * 72.0, p.y + 0.23 * 72.0, "α", 15);
				CC.SaveState();
				{
					//Skew contentMatrix from origin to given angles α and ß
					contentMatrix = math.Matrix_Skew(ref contentMatrix, 20, 20);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false);
					drawN(CC, p, Font);
					CC.RestoreState();
				}
				CC.RestoreState();
			}
			drawTitle(Parent.m_CurDoc, CC, 5.7 * 72.0, rc.top - 8.5 * 72.0, "SKEWING", 15);
			firstPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(firstPage);

			IPXC_Page secondPage = pages.InsertPage(1, ref rc, out urData);

			CC.SaveState();
			{
				//Reset globalMatrix
				math.Matrix_Reset(ref globalMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2 * 72.0, rc.top - 1.5 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false);
				drawN(CC, p, Font);
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.3 * 72.0, rc.top - 2.3 * 72.0, "ORIGIN", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2 * 72.0, rc.top - 4.2 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Translate contentMatrix from origin to given piont
					contentMatrix = math.Matrix_Translate(ref contentMatrix, 0.4 * 72.0, 0.4 * 72.0);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.5 * 72.0, rc.top - 5 * 72.0, "STEP 1: TRANSLATION", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2 * 72.0, rc.top - 7 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Rotate contentMatrix from origin to given angle
					contentMatrix = math.Matrix_Rotate(ref contentMatrix, 30);
					//Translate contentMatrix from origin to given piont
					contentMatrix = math.Matrix_Translate(ref contentMatrix, 0.4 * 72.0, 0.4 * 72.0);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.5 * 72.0, rc.top - 7.8 * 72.0, "STEP 2: ROTATION", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 2 * 72.0, rc.top - 9.8 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Scale contentMatrix from origin to given size
					contentMatrix = math.Matrix_Scale(ref contentMatrix, 1.2, 1);
					//Rotate contentMatrix from origin to given angle
					contentMatrix = math.Matrix_Rotate(ref contentMatrix, 30);
					//Translate contentMatrix from origin to given piont
					contentMatrix = math.Matrix_Translate(ref contentMatrix, 0.4 * 72.0, 0.4 * 72.0);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 2.5 * 72.0, rc.top - 10.6 * 72.0, "STEP 3: SCALING", 15);


			CC.SaveState();
			{
				//Reset globalMatrix
				math.Matrix_Reset(ref globalMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.5 * 72.0, rc.top - 1.5 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false);
				drawN(CC, p, Font);
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 5.8 * 72.0, rc.top - 2.3 * 72.0, "ORIGIN", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.5 * 72.0, rc.top - 4.2 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Scale contentMatrix from origin to given size
					contentMatrix = math.Matrix_Scale(ref contentMatrix, 1.2, 1);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 6 * 72.0, rc.top - 5 * 72.0, "STEP 1: SCALING", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.5 * 72.0, rc.top - 7 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Rotate contentMatrix from origin to given angle
					contentMatrix = math.Matrix_Rotate(ref contentMatrix, 30);
					//Scale contentMatrix from origin to given size
					contentMatrix = math.Matrix_Scale(ref contentMatrix, 1.2, 1);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 6 * 72.0, rc.top - 7.8 * 72.0, "STEP 2: ROTATION", 15);

			CC.SaveState();
			{
				//Reset globalMatrix and contentMatrix
				math.Matrix_Reset(ref globalMatrix);
				math.Matrix_Reset(ref contentMatrix);
				//Translate globalMatrix from origin to given piont
				globalMatrix = math.Matrix_Translate(ref globalMatrix, 5.5 * 72.0, rc.top - 9.8 * 72.0);
				CC.ConcatCS(globalMatrix);
				drawCS(CC, p, false, 53.28, false, argbGray);
				CC.SaveState();
				{
					//Translate contentMatrix from origin to given piont
					contentMatrix = math.Matrix_Translate(ref contentMatrix, 0.4 * 72.0, 0.35 * 72.0);
					//Rotate contentMatrix from origin to given angle
					contentMatrix = math.Matrix_Rotate(ref contentMatrix, 30);
					//Scale contentMatrix from origin to given size
					contentMatrix = math.Matrix_Scale(ref contentMatrix, 1.2, 1);
					CC.ConcatCS(contentMatrix);
					drawCS(CC, p, false, 53.28, false);
					drawN(CC, p, Font);
				}
				CC.RestoreState();
			}
			CC.RestoreState();
			drawTitle(Parent.m_CurDoc, CC, 6 * 72.0, rc.top - 10.6 * 72.0, "STEP 3: TRANSLATION", 15);

			secondPage.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(secondPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.8. Stroke Types")]
		static public void AddStrokeTypes(Form1 Parent)
		{
			uint argbBlack = 0x00000000;
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};
			//delegate IPXC_Pattern CreateImagePattern(string str, IPXC_Document Doc, IIXC_Inst g_ImgCore, IMathHelper mathHelper);
			CreateImagePattern crtImgPat = (str, doc, ImgCore, mathHelper) =>
			{
				IPXC_Pattern Patt = null;
				IPXC_Image Img = doc.AddImageFromFile(str);
				PXC_Rect bbox;
				bbox.left = 0;
				bbox.bottom = 0;
				bbox.right = Img.Width * 72.0 / 96.0;
				bbox.top = Img.Height * 72.0 / 96.0;
				IPXC_ContentCreator ContCrt = doc.CreateContentCreator();

				PXC_Matrix im = new PXC_Matrix();
				mathHelper.Matrix_Reset(ref im);
				im = mathHelper.Matrix_Scale(ref im, bbox.right, bbox.top);
				ContCrt.SaveState();
				ContCrt.ConcatCS(im);
				ContCrt.PlaceImage(Img);
				ContCrt.RestoreState();
				Patt = doc.CreateTilePattern(ref bbox);
				IPXC_Content content = ContCrt.Detach();
				content.set_BBox(ref bbox);
				Patt.SetContent(content, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				return Patt;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IIXC_Inst ixcInst = Parent.m_pxcInst.GetExtension("IXC");

			double x = (rc.right + rc.left) / 2.0;
			double y = rc.top - 0.8 * 72.0;
			CC.SetLineWidth(5.0);
			CC.SetStrokeColorRGB(argbBlack);
			CC.NoDash();
			CC.MoveTo(x - 3.5 * 72.0, y);
			CC.LineTo(x + 3.5 * 72.0, y);
			CC.StrokePath(false);
			drawTitle(Parent.m_CurDoc, CC, x, y - 8, "SOLID LINE", 15);

			x = (rc.right + rc.left) / 2.0;
			y = rc.top - 1.5 * 72.0;
			CC.SetDash(20.0, 10.0, 5.0);
			CC.MoveTo(x - 3.5 * 72.0, y);
			CC.LineTo(x + 3.5 * 72.0, y);
			CC.StrokePath(false);
			CC.NoDash();
			drawTitle(Parent.m_CurDoc, CC, x, y - 8, "DASHED", 15);

			x = (rc.right + rc.left) / 2.0;
			y = rc.top - 2.2 * 72.0;
			double[] dashArr = { 70, 10, 10, 5, 3, 5, 10, 10 };
			CC.SetDashCA(dashArr, 1.0);
			CC.MoveTo(x - 3.5 * 72.0, y);
			CC.LineTo(x + 3.5 * 72.0, y);
			CC.StrokePath(false);
			CC.NoDash();
			drawTitle(Parent.m_CurDoc, CC, x, y - 8, "POLY DASHED", 15);

			x = (rc.right + rc.left) / 2.0;
			y = rc.top - 3.0 * 72.0;
			string[] titles = { "LineCap_Butt", "LineCap_Round", "LineCap_Square" };
			PXC_LineCap[] caps = { PXC_LineCap.LineCap_Butt, PXC_LineCap.LineCap_Round, PXC_LineCap.LineCap_Square };
			for (int i = 0; i < caps.Length; i++)
			{
				CC.SetLineCap(caps[i]);
				CC.MoveTo(x - 72.0, y);
				CC.LineTo(x + 72.0, y);
				CC.StrokePath(false);
				drawTitle(Parent.m_CurDoc, CC, x, y - 15, titles[i], 15);
				y -= 0.8 * 72.0;
			}


			x = (rc.right + rc.left) / 2.0 - 2.0 * 72.0;
			y = rc.top - 5.9 * 72.0;
			double r = 0.5 * 72.0;
			CC.SetLineWidth(15);
			CC.SaveState();
			PXC_LineJoin[] joins = { PXC_LineJoin.LineJoin_Bevel, PXC_LineJoin.LineJoin_Miter, PXC_LineJoin.LineJoin_Round };
			titles[0] = "LineJoin_Miter";
			titles[1] = "LineJoin_Round";
			titles[2] = "LineJoin_Bevel";
			double[] pts = new double[3 * 2];
			for (int i = 0; i < joins.Length; i++)
			{
				double a = 30;
				for (int j = 0; j < 3; j++)
				{
					double xx = r * Math.Cos(a * Math.PI / 180.0);
					double yy = r * Math.Sin(a * Math.PI / 180.0);
					pts[j * 2 + 0] = x + xx;
					pts[j * 2 + 1] = y - yy;
					a += 120;
				}
				CC.SetLineJoin(joins[i]);
				CC.PolygonSA(pts, true);
				CC.StrokePath(true);
				drawTitle(Parent.m_CurDoc, CC, x, y - r, titles[i], 15);
				x += 2.0 * 72.0;
			}
			CC.RestoreState();

			x = (rc.right + rc.left) / 2.0 - 2.0 * 72.0;
			y = rc.top - 7.7 * 72.0;
			CC.SaveState();
			CC.SetLineJoin(PXC_LineJoin.LineJoin_Miter);
			titles[0] = "NO MITER LIMIT";
			titles[1] = "MITER LIMIT";
			for (int i = 0; i < 2; i++)
			{
				CC.MoveTo(x - 72.0, y + 0.5 * 72.0);
				CC.LineTo(x + 72.0, y);
				CC.LineTo(x - 72.0, y);
				CC.StrokePath(false);
				drawTitle(Parent.m_CurDoc, CC, x, y - 14, titles[i], 15);
				x += 4.0 * 72.0;
				CC.SetMiterLimit(3.0);
			}
			CC.RestoreState();

			x = rc.left + 2.5 * 72.0;
			y = rc.top - 8.4 * 72.0;
			IColor Color;
			Color = auxInst.CreateColor(ColorType.ColorType_Gray);
			int numSteps = 256;
			float minV = 0.0f;
			float maxV = 1.0f;
			double width = 1.5 * 72.0;
			double height = 1.5 * 72.0;
			double dx = (width / (numSteps - 1)) / 2.0;
			double dy = (height / (numSteps - 1)) / 2.0;
			PXC_Rect rect = new PXC_Rect();
			rect.left = x - width / 2.0;
			rect.right = rect.left + width;
			rect.bottom = y - height - 2.0;
			rect.top = rect.bottom + height;
			CC.SetLineWidth(1.0);
			CC.SetLineJoin(PXC_LineJoin.LineJoin_Bevel);
			for (int i = 0; i < numSteps; i++)
			{
				Color.SetGray(minV + ((maxV - minV) * i / (numSteps - 1)));
				CC.SetColor(Color, Color);
				CC.Rect(rect.left, rect.top, rect.right, rect.bottom);
				CC.StrokePath(true);
				rect.left += dx; rect.right -= dx;
				rect.bottom += dy; rect.top -= dy;
			}
			drawTitle(Parent.m_CurDoc, CC, x, y - 1.9 * 72.0, "GRADIENT FILL EMULATION", 15);


			x = rc.right - 2.5 * 72.0;
			y = rc.top - 9.4 * 72.0;
			width = 3.0 * 72.0;
			height = 1.5 * 72.0;
			rect.left = x - (width / 2);
			rect.right = rect.left + width;
			rect.top = y + height / 2;
			rect.bottom = rect.top - height;
			CC.SetLineWidth(15);
			IAUX_Inst Aux_Inst = Parent.m_pxcInst.GetExtension("AUX");
			IMathHelper math = Aux_Inst.MathHelper;
			IPXC_Pattern Pat = crtImgPat(System.Environment.CurrentDirectory + "\\Images\\CoreAPI_32.ico", Parent.m_CurDoc, ixcInst, math);
			CC.SetPattern(Pat, false);
			CC.Ellipse(rect.left, rect.bottom, rect.right, rect.top);
			CC.StrokePath(true);
			drawTitle(Parent.m_CurDoc, CC, x, y - 65, "Stroke With Image Pattern", 15);

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.2. Place image with different transformations")]
		static public void PlaceImageWithDifferentTransformations(Form1 Parent)
		{
			//delegate void DrawTitle(IPXC_Document Doc, IPXC_ContentCreator ContCrt, double cx, double baseLineY, string sText, double fontSize, double fontWidth = 400.0, uint argbFillColor = 0x00000000);
			DrawTitle drawTitle = (Doc, ContCrt, cx, baseLineY, sText, fontSize, fontWidth, color) =>
			{
				IPXC_Font defFont = Doc.CreateNewFont("Arial", 0, 400);
				ContCrt.SaveState();
				ContCrt.SetFillColorRGB(color);
				ContCrt.SetFont(defFont);
				double nWidth = 0;
				double nHeight = 0;
				ContCrt.CalcTextSize(fontSize, sText, out nWidth, out nHeight, -1);
				ContCrt.SetFontSize(fontSize);
				ContCrt.ShowTextLine(cx - nWidth / 2.0, baseLineY, sText, -1, (uint)PXC_ShowTextLineFlags.STLF_Default | (uint)PXC_ShowTextLineFlags.STLF_AllowSubstitution);
				ContCrt.RestoreState();
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc;
			rc.left = 0;
			rc.right = 600;
			rc.top = 800;
			rc.bottom = 0;

			IPXC_UndoRedoData urData;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, ref rc, out urData);
			IAUX_Inst Aux_Inst = Parent.m_pxcInst.GetExtension("AUX");
			IMathHelper mathHelper = Aux_Inst.MathHelper;
			IPXC_Image Img = Parent.m_CurDoc.AddImageFromFile(System.Environment.CurrentDirectory + "\\Images\\Editor_welcome.png");
			PXC_Matrix contentMatrix = new PXC_Matrix();
			PXC_Matrix globalMatrix = new PXC_Matrix();

			//Placing image by it's original size / 5.0
			mathHelper.Matrix_Reset(ref contentMatrix);
			mathHelper.Matrix_Reset(ref globalMatrix);
			CC.SaveState();
			{
				globalMatrix = mathHelper.Matrix_Translate(ref globalMatrix, 1.0 * 72.0, rc.top - 4.0 * 72.0);
				CC.ConcatCS(globalMatrix);
				CC.SaveState();
				{
					contentMatrix = mathHelper.Matrix_Scale(ref contentMatrix, Img.Width / 5.0, Img.Height / 5.0);
					CC.ConcatCS(contentMatrix);
					CC.PlaceImage(Img);
				}
				CC.RestoreState();
				drawTitle(Parent.m_CurDoc, CC, contentMatrix.a / 2, -15, "ORIGINAL", 15);
			}
			CC.RestoreState();

			//Placing 45 degree rotated image
			mathHelper.Matrix_Reset(ref contentMatrix);
			mathHelper.Matrix_Reset(ref globalMatrix);
			CC.SaveState();
			{
				globalMatrix = mathHelper.Matrix_Translate(ref globalMatrix, 6.3 * 72.0, rc.top - 4.0 * 72.0);
				CC.ConcatCS(globalMatrix);
				CC.SaveState();
				{
					contentMatrix = mathHelper.Matrix_Scale(ref contentMatrix, Img.Width / 5.0, Img.Height / 5.0);
					contentMatrix = mathHelper.Matrix_Rotate(ref contentMatrix, 45);
					CC.ConcatCS(contentMatrix);
					CC.PlaceImage(Img);
				}
				CC.RestoreState();
				drawTitle(Parent.m_CurDoc, CC, contentMatrix.a / -2, -15, "ROTATION", 15);
			}
			CC.RestoreState();

			//Stretching image inside the given rectangle
			mathHelper.Matrix_Reset(ref contentMatrix);
			mathHelper.Matrix_Reset(ref globalMatrix);
			PXC_Rect rect = new PXC_Rect();
			rect.right = 2 * 72.0;
			rect.top = 2 * 72.0;
			CC.SaveState();
			{
				globalMatrix = mathHelper.Matrix_Translate(ref globalMatrix, 1.0 * 72.0, rc.top - 9.0 * 72.0);
				CC.ConcatCS(globalMatrix);
				CC.SaveState();
				{
					CC.SetLineWidth(4.0);
					CC.Rect(rect.left, rect.bottom, rect.right, rect.top);
					CC.StrokePath(true);
					contentMatrix = mathHelper.Matrix_Scale(ref contentMatrix, rect.right, rect.top);
					CC.ConcatCS(contentMatrix);
					CC.PlaceImage(Img);
				}
				CC.RestoreState();
				drawTitle(Parent.m_CurDoc, CC, contentMatrix.a / 2, -15, "STRETCHING", 15);
			}
			CC.RestoreState();
			//Proportionally placing image inside the given rectangle
			mathHelper.Matrix_Reset(ref contentMatrix);
			mathHelper.Matrix_Reset(ref globalMatrix);
			PXC_Rect rcImg = new PXC_Rect();
			rect = new PXC_Rect();
			rect.right = 3 * 72.0;
			rect.top = 2 * 72.0;

			CC.SaveState();
			{
				globalMatrix = mathHelper.Matrix_Translate(ref globalMatrix, 4.3 * 72.0, rc.top - 9.0 * 72.0);
				CC.ConcatCS(globalMatrix);
				CC.SaveState();
				double nW = rect.right - rect.left;
				double nH = rect.top - rect.bottom;
				{
					//Proportional resize rectangle calculation
					{
						double k1 = nW / nH;
						double k2 = (double)Img.Width / Img.Height;
						if (k1 >= k2)
						{
							rcImg = rect;
							rcImg.right = nW / 2.0 + rcImg.top * k2 / 2.0;
							rcImg.left = nW / 2.0 - rcImg.top * k2 / 2.0;
						}
						else
						{
							rcImg = rect;
							rcImg.top = nH / 2.0 + rcImg.right / k2 / 2.0;
							rcImg.bottom = nH / 2.0 - rcImg.right / k2 / 2.0;
						}
					}
					//Moving the image rectangle to the center


					CC.SetLineWidth(4.0);
					CC.Rect(rect.left, rect.bottom, rect.right, rect.top);
					CC.StrokePath(true);
					PXC_Rect rcImage = new PXC_Rect();
					rcImage.right = 1;
					rcImage.top = 1;
					contentMatrix = mathHelper.Matrix_RectToRect(ref rcImage, ref rcImg);
					CC.ConcatCS(contentMatrix);
					CC.PlaceImage(Img);
				}
				CC.RestoreState();
				drawTitle(Parent.m_CurDoc, CC, nW / 2.0, -15, "PROPORTIONALLY PLACING", 15);
			}
			CC.RestoreState();

			Page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.11. Change all of the Images to grayscale")]
		static public void ChangeAllOfTheImagesToGrayscale(Form1 Parent)
		{
			//delegate void ChageImageItemsToGrayscale(object contentObj)
			ChageImages changeImages = null;
			changeImages = delegate(object contentObj)
			{
				IPXC_Content content = contentObj as IPXC_Content;
				if (content == null)
					return;
				for (uint i = 0; i < content.Items.Count; i++)
				{
					IPXC_ContentItem item = content.Items[i];
					if (item == null)
						continue;
					if (item.Type == PXC_CIType.CIT_XForm)
					{
						IPXC_XForm xf = content.Document.GetXFormByHandle(item.XForm_Handle);
						if (xf == null)
							continue;
						IPXC_Content cont = xf.GetContent(PXC_ContentAccessMode.CAccessMode_FullClone);
						changeImages(cont);
						xf.SetContent(cont, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
						continue;
					}
					if ((item.Type != PXC_CIType.CIT_Image) && (item.Type != PXC_CIType.CIT_InlineImage))
						continue;
					IIXC_Page ixcPage = item.Image_CreateIXCPage(true, PXC_RenderingIntent.RI_RelativeColorimetric);
					IPXC_Image newImage = content.Document.AddImageFromIXCPage(ixcPage, 0);
					item.Image_Handle = newImage.Handle;
				}
			};
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			for (uint i = 0; i < pages.Count; i++)
			{
				IPXC_Page page = pages[i];
				if (page == null)
					continue;
				IPXC_Content content = page.GetContent(PXC_ContentAccessMode.CAccessMode_FullClone);
				if (content == null)
				{
					Marshal.ReleaseComObject(page);
					continue;
				}
				changeImages(content);
				page.PlaceContent(content, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				Marshal.ReleaseComObject(page);
			}
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.12. Change all of the Images to black and white")]
		static public void ChangeAllOfTheImagesToBlackAndWhite(Form1 Parent)
		{
			//delegate void ChageImageItemsToGrayscale(object contentObj)
			ChageImages changeImages = null;
			changeImages = delegate (object contentObj)
			{
				IPXC_Content content = contentObj as IPXC_Content;
				if (content == null)
					return;
				for (uint i = 0; i < content.Items.Count; i++)
				{
					IPXC_ContentItem item = content.Items[i];
					if (item == null)
						continue;
					if (item.Type == PXC_CIType.CIT_XForm)
					{
						IPXC_XForm xf = content.Document.GetXFormByHandle(item.XForm_Handle);
						if (xf == null)
							continue;
						IPXC_Content cont = xf.GetContent(PXC_ContentAccessMode.CAccessMode_FullClone);
						changeImages(cont);
						xf.SetContent(cont, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
						continue;
					}
					if ((item.Type != PXC_CIType.CIT_Image) && (item.Type != PXC_CIType.CIT_InlineImage))
						continue;
					IIXC_Page ixcPage = item.Image_CreateIXCPage(false, PXC_RenderingIntent.RI_RelativeColorimetric);
					uint[] colors = { 0, 0x00FFFFFF };
					ixcPage.ReduceColorsFixedPalette(colors, IXC_DitherMethod.Dither_None);
					IPXC_Image newImage = content.Document.AddImageFromIXCPage(ixcPage, 0);
					item.Image_Handle = newImage.Handle;
				}
			};
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			for (uint i = 0; i < pages.Count; i++)
			{
				IPXC_Page page = pages[i];
				if (page == null)
					continue;
				IPXC_Content content = page.GetContent(PXC_ContentAccessMode.CAccessMode_FullClone);
				if (content == null)
				{
					Marshal.ReleaseComObject(page);
					continue;
				}
				changeImages(content);
				page.PlaceContent(content, (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
				Marshal.ReleaseComObject(page);
			}
			Marshal.ReleaseComObject(pages);
		}

		[Description("4.13. Add XFA formatted text block")]
		static public void AddXFAFormattedTextBlock(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[0];
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			PXC_Rect rcText;
			rcText.left = 0;
			rcText.bottom = 0;
			rcText.top = 700;
			rcText.right = 500;
			PXC_Rect rcBlock;
			CC.ShowTextBlock("<body><p><span style=\"xfa-spacerun:yes\">Hello     goodbye</span></p></body>", rcText, rcText, (uint)PXC_DrawTextFlags.DTF_RichText, -1, null, null, null, out rcBlock);  //draw text block
			page.PlaceContent(CC.Detach(), (uint)PXC_PlaceContentFlags.PlaceContent_Replace);
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);
		}
	}
}
