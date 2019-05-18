using System.ComponentModel;
using System;
using PDFXCoreAPI;
using System.Runtime.InteropServices;

namespace CoreAPIDemo
{
	[Description("8. Pagemarks")]
	class Pagemarks
	{
		[Description("8.1. Add Headers and Footers on pages")]
		static public void AddHeadersAndFootersOnPages(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_HeaderAndFooterParams firstHeaderFooter = Parent.m_pxcInst.CreateHeaderAndFooterParams();
			IPXC_Font font = Parent.m_CurDoc.CreateNewFont("Arial", (uint)PXC_CreateFontFlags.CreateFont_Serif, 400);
			IColor color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0, 0, 0);
			firstHeaderFooter.Font = font;
			firstHeaderFooter.FillColor = color;
			firstHeaderFooter.LeftHeaderText = "%[Page]";
			firstHeaderFooter.LeftFooterText = "%[Page] of %[Pages]";
			firstHeaderFooter.BottomMargin = 36.0f;
			firstHeaderFooter.TopMargin = 36.0f;
			firstHeaderFooter.RightMargin = 36.0f;
			firstHeaderFooter.LeftMargin = 36.0f;
			firstHeaderFooter.FontSize = 30.0f;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IBitSet bitSet = auxInst.CreateBitSet(pages.Count);
			bitSet.Set(0, pages.Count, true);
			Parent.m_CurDoc.PlaceHeadersAndFooters(bitSet, firstHeaderFooter);

			IPXC_HeaderAndFooterParams secondHeaderFooter = Parent.m_pxcInst.CreateHeaderAndFooterParams();
			font = Parent.m_CurDoc.CreateNewFont("Comic Sans MS", (uint)PXC_CreateFontFlags.CreateFont_Italic, 1000);
			color.SetRGB(0.35f, 0.48f, 0.35f);
			secondHeaderFooter.Font = font;
			secondHeaderFooter.FillColor = color;
			secondHeaderFooter.CenterHeaderText = "%[Date:MM.dd.yyyy] %[Time]";
			secondHeaderFooter.CenterFooterText = "Version %[AppVersion]";
			secondHeaderFooter.BottomMargin = 20.0f;
			secondHeaderFooter.TopMargin = 20.0f;
			secondHeaderFooter.RightMargin = 40.0f;
			secondHeaderFooter.LeftMargin = 40.0f;
			secondHeaderFooter.FontSize = 15.0f;
			Parent.m_CurDoc.PlaceHeadersAndFooters(bitSet, secondHeaderFooter);

			IPXC_HeaderAndFooterParams thirdHeaderFooter = Parent.m_pxcInst.CreateHeaderAndFooterParams(); 
			font = Parent.m_CurDoc.CreateNewFont("Times New Roman", (uint)PXC_CreateFontFlags.CreateFont_Monospaced, 700);
			color.SetRGB(0.67f, 0.23f, 0.8f);
			thirdHeaderFooter.Font = font;
			thirdHeaderFooter.FillColor = color;
			thirdHeaderFooter.RightHeaderText = "%[Computer]";
			thirdHeaderFooter.RightFooterText = "%[User]";
			thirdHeaderFooter.BottomMargin = 40.0f;
			thirdHeaderFooter.TopMargin = 40.0f;
			thirdHeaderFooter.RightMargin = 20.0f;
			thirdHeaderFooter.LeftMargin = 20.0f;
			thirdHeaderFooter.FontSize = 20.0f;
			Parent.m_CurDoc.PlaceHeadersAndFooters(bitSet, thirdHeaderFooter);
			Marshal.ReleaseComObject(pages);
		}

		[Description("8.2. Add Watermarks on page")]
		static public void AddWatermarksOnPage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_WatermarkParams watermark = Parent.m_pxcInst.CreateWatermarkParams();
			IColor fillColor = auxInst.CreateColor(ColorType.ColorType_RGB);
			fillColor.SetRGB(0.4f, 0.2f, 0.6f);
			watermark.Text = "WATERMARK";
			watermark.HAlign = 50;
			watermark.VAlign = 0;
			watermark.FillColor = fillColor;
			watermark.Rotation = -30;
			watermark.FontSize = 200;
			watermark.StrokeWidth = 5.0f;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IBitSet bitSet = auxInst.CreateBitSet(pages.Count);
			bitSet.Set(0, pages.Count, true);
			Parent.m_CurDoc.PlaceWatermark(bitSet, watermark);
			watermark.Text = "";
			watermark.ImageFile = System.Environment.CurrentDirectory + "\\Images\\Editor_welcome.png";
			PXC_Size rect = new PXC_Size();
			rect.cx = 200;
			rect.cy = 400;
			watermark.Rotation = -45;
			watermark.VAlign = 2;
			watermark.set_ImageSize(rect);
			watermark.Flags |= (uint)PXC_WatermarkFlags.WatermarkFlag_PlaceOnBackground;
			watermark.WatermarkType = PXC_WatermarkType.Watermark_Image;
			watermark.Opacity = 50;
			Parent.m_CurDoc.PlaceWatermark(bitSet, watermark);
			Marshal.ReleaseComObject(pages);
		}

		[Description("8.3. Add Background on page")]
		static public void AddBackgroundOnPage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_BackgroundParams backgroundParams = Parent.m_pxcInst.CreateBackgroundParams();
			IColor fillColor = auxInst.CreateColor(ColorType.ColorType_RGB);
			fillColor.SetRGB(0.5f, 1.0f, 0.5f);
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IBitSet bitSet = auxInst.CreateBitSet(pages.Count);
			bitSet.Set(0, pages.Count, true);
			backgroundParams.FillColor = fillColor;
			backgroundParams.VAlign = 0;
			backgroundParams.HAlign = 2;
			backgroundParams.Scale = 60;
			backgroundParams.Rotation = 45;
			Parent.m_CurDoc.PlaceBackgrounds(bitSet, backgroundParams);

			backgroundParams.BackgroundFile = System.Environment.CurrentDirectory + "\\Images\\Editor_welcome.png";
			backgroundParams.BackgroundPage = 0;
			backgroundParams.VAlign = 2;
			backgroundParams.HAlign = 0;
			backgroundParams.Scale = 60;
			backgroundParams.Rotation = 45;
			backgroundParams.Opacity = 50;
			backgroundParams.BackgroundType = PXC_BackgroundType.Background_Image;
			backgroundParams.Flags |= (uint)PXC_BackgroundFlags.BackgroundFlag_ScaleToPage;
			Parent.m_CurDoc.PlaceBackgrounds(bitSet, backgroundParams);
			Marshal.ReleaseComObject(pages);
		}
	}
}
