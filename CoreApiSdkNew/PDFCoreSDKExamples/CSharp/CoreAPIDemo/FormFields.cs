using PDFXCoreAPI;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoreAPIDemo
{
	[Description("5. Form Fields")]
	class FormFields
	{
		delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);

		[Description("5.1. Add Text form fields")]
		static public void AddTextFieldsOnPage(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);
			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page firstPage = pages.InsertPage(0, rc, out urD);
			PXC_Rect textRC = new PXC_Rect();
			textRC.top = rc.top - 1.0 * 72.0;
			textRC.left = 1.0 * 72.0;
			textRC.bottom = rc.top - 2.0 * 72.0;
			textRC.right = rc.right - 1.0 * 72.0;

			//Ordinary text field
			IPXC_FormField firstTextBOX = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Text", ref index), PXC_FormFieldType.FFT_Text, 0, textRC);
			firstTextBOX.SetValueText("Ordinary text field");
			Marshal.ReleaseComObject(firstTextBOX);

			//Read-only and locked text field with custom style
			textRC.top = rc.top - 3.0 * 72.0;
			textRC.bottom = rc.top - 4.0 * 72.0;
			IPXC_FormField secondTextBOX = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Text", ref index), PXC_FormFieldType.FFT_Text, 0, textRC);
			secondTextBOX.SetValueText("Read-only and locked text field with custom style");
			IAUX_Inst auxInst = Parent.m_pxcInst.GetExtension("AUX");
			IPXC_Annotation annot = secondTextBOX.Widget[0];
			IPXC_AnnotData_Widget WData = (IPXC_AnnotData_Widget)annot.Data;
			IColor color = auxInst.CreateColor(ColorType.ColorType_RGB);
			color.SetRGB(0.9f, 0.9f, 0.6f);
			WData.FColor = color;
			color.SetRGB(0.6f, 0.9f, 0.9f);
			WData.SColor = color;
			PXC_AnnotBorder border = new PXC_AnnotBorder();
			border.nStyle = PXC_AnnotBorderStyle.ABS_Dashed;
			border.DashArray = new float[10];
			border.DashArray[0] = border.DashArray[1] = 16.0f; //Width of dashes
			border.nDashCount = 2; //Number of dashes
			border.nWidth = 5.0f;
			WData.set_Border(ref border);
			annot.Data = WData;
			secondTextBOX.SetFlags((uint)PXC_FormFieldFlag.FFF_ReadOnly, (uint)PXC_FormFieldFlag.FFF_ReadOnly);
			Marshal.ReleaseComObject(secondTextBOX);



			//90 degree orientation text field with multiline option enabled
			textRC.top = rc.top - 5.0 * 72.0;
			textRC.bottom = rc.top - 7.0 * 72.0;
			IPXC_FormField thirdTextBOX = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Text", ref index), PXC_FormFieldType.FFT_Text, 0, textRC);
			thirdTextBOX.SetFlags((uint)PXC_FormFieldFlag.TFF_MultiLine, (uint)PXC_FormFieldFlag.TFF_MultiLine);
			thirdTextBOX.SetValueText("90 degree orientation text field with multiline option enabled");
			annot = thirdTextBOX.Widget[0];
			WData = (IPXC_AnnotData_Widget)annot.Data;
			WData.ContentRotation = 90;
			annot.Data = WData;
			Marshal.ReleaseComObject(thirdTextBOX);

			//Time formatted text field with custom JS that gives current time
			textRC.top = rc.top - 8.0 * 72.0;
			textRC.bottom = rc.top - 9.0 * 72.0;
			IPXC_FormField fourthTextBOX = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Text", ref index), PXC_FormFieldType.FFT_Text, 0, textRC);
			annot = fourthTextBOX.Widget[0];
			annot.Flags |= (uint)PXC_SignDocumentFlags.Sign_TX_Date;
			IPXC_ActionsList actionsList = Parent.m_CurDoc.CreateActionsList();
			//Set script to ActionList
			actionsList.AddJavaScript("var now = new Date()\n" +
						"this.getField(\"Text4\").value = now.getHours() + \":\" + now.getMinutes() ");
			fourthTextBOX.Actions[PXC_TriggerType.Trigger_Format] = actionsList;
			Marshal.ReleaseComObject(fourthTextBOX);
			Marshal.ReleaseComObject(firstPage);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.2. Add Button form fields")]
		static public void AddButtonWithIconAndURI(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding button with icon with the URI action
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 1.5 * 72.0;
			rcPB.right = rcPB.left + 0.5 * 72.0;
			rcPB.top = rc.top - 0.14 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)
			IPXC_FormField googleButton = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Button", ref index), PXC_FormFieldType.FFT_PushButton, 0, ref rcPB);
			//Now we'll need to add the icon
			IPXC_Annotation annot = googleButton.Widget[0];
			IPXC_AnnotData_Widget WData = (IPXC_AnnotData_Widget)annot.Data;
			string sPath = System.Environment.CurrentDirectory + "\\Images\\gotoSource_24.png";
			IPXC_Image img = Parent.m_CurDoc.AddImageFromFile(sPath);
			float imgw = img.Width;
			float imgh = img.Height;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			CC.SaveState();
			CC.ScaleCS(imgw, imgh); //the image will be scaled to the button's size
			CC.PlaceImage(img);
			CC.RestoreState();
			IPXC_Content content = CC.Detach();
			PXC_Rect rcBBox;
			rcBBox.left = 0;
			rcBBox.top = imgh;
			rcBBox.right = imgw;
			rcBBox.bottom = 0;
			content.set_BBox(rcBBox);
			IPXC_XForm xForm = Parent.m_CurDoc.CreateNewXForm(ref rcPB);
			xForm.SetContent(content);
			WData.ButtonTextPosition = PXC_WidgetButtonTextPosition.WidgetText_IconOnly;
			WData.SetIcon(PXC_AnnotAppType.AAT_Normal, xForm, true);
			WData.Contents = "http://www.google.com"; //tooltip
			annot.Data = WData;
			//Setting the annotation's URI action
			IPXC_ActionsList AL = Parent.m_CurDoc.CreateActionsList();
			AL.AddURI("https://www.google.com");
			annot.set_Actions(PXC_TriggerType.Trigger_Up, AL);
			Marshal.ReleaseComObject(googleButton);


			//Adding button with icon and label and JS that goes to next page
			rcPB.left +=  1.2 * 72.0;
			rcPB.right = rcPB.left + 0.6 * 72.0;
			IPXC_FormField nextButton = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Button", ref index), PXC_FormFieldType.FFT_PushButton, 0, ref rcPB);
			//Now we'll need to add the icon
			annot = nextButton.Widget[0];
			WData = (IPXC_AnnotData_Widget)annot.Data;
			sPath = System.Environment.CurrentDirectory + "\\Images\\next_24.png";
			img = Parent.m_CurDoc.AddImageFromFile(sPath);
			imgw = img.Width;
			imgh = img.Height;
			CC.SaveState();
			CC.ScaleCS(imgw, imgh); //the image will be scaled to the button's size
			CC.PlaceImage(img);
			CC.RestoreState();
			content = CC.Detach();
			rcBBox.left = 0;
			rcBBox.top = imgh;
			rcBBox.right = imgw;
			rcBBox.bottom = 0;
			content.set_BBox(rcBBox);
			xForm = Parent.m_CurDoc.CreateNewXForm(ref rcPB);
			xForm.SetContent(content);
			WData.SetCaption(PXC_AnnotAppType.AAT_Normal, "Next Page");
			WData.ButtonTextPosition = PXC_WidgetButtonTextPosition.WidgetText_IconTextV;
			PXC_Point p = new PXC_Point();
			p.x = 0.5;
			p.y = 0.5;
			WData.set_IconOffset(p);
			WData.SetIcon(PXC_AnnotAppType.AAT_Normal, xForm, true);
			WData.Contents = "Next Page"; //tooltip
			annot.Data = WData;
			//Setting the annotation's Goto action
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = Page.Number + 1;
			dest.nType = PXC_DestType.Dest_XYZ;
			dest.nNullFlags = 15;
			AL = Parent.m_CurDoc.CreateActionsList();
			AL.AddGoto(dest);
			annot.set_Actions(PXC_TriggerType.Trigger_Up, AL);
			Marshal.ReleaseComObject(nextButton);

			//Adding text button that opens the file
			rcPB.left += 1.5 * 72.0;
			rcPB.right = rcPB.left + 2.0 * 72.0;
			IPXC_FormField openButton = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Button", ref index), PXC_FormFieldType.FFT_PushButton, 0, ref rcPB);
			//Now we'll need to add the icon
			annot = openButton.Widget[0];
			WData = (IPXC_AnnotData_Widget)annot.Data;
			WData.ButtonTextPosition = PXC_WidgetButtonTextPosition.WidgetText_TextOnly;
			WData.SetCaption(PXC_AnnotAppType.AAT_Normal, "Open File");
			WData.Contents = "Open File"; //tooltip
			annot.Data = WData;
			//Setting the annotation's Launch action
			AL = Parent.m_CurDoc.CreateActionsList();
			sPath = System.Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			AL.AddLaunch(sPath);
			annot.set_Actions(PXC_TriggerType.Trigger_Up, AL);
			Marshal.ReleaseComObject(openButton);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.3. Add Check Box form fields")]
		static public void AddCheckBoxFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding uncheked checkbox
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 3.2 * 72.0;
			rcPB.right = rcPB.left + 0.5 * 72.0;
			rcPB.top = rc.top - 0.2 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)
			IPXC_FormField checkBoxUnch = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "CheckBox", ref index), PXC_FormFieldType.FFT_CheckBox, 0, ref rcPB);
			Marshal.ReleaseComObject(checkBoxUnch);

			//Adding cheked checkbox
			rcPB.right = rc.right - 3.2 * 72.0;
			rcPB.left = rcPB.right - 0.5 * 72.0;
			IPXC_FormField checkBoxCh = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "CheckBox", ref index), PXC_FormFieldType.FFT_CheckBox, 0, ref rcPB);
			IPXC_Annotation annot = checkBoxCh.Widget[0];
			IPXC_AnnotData_Widget widget = (IPXC_AnnotData_Widget)annot.Data;
			checkBoxCh.CheckWidget((uint)checkBoxCh.GetWidgetIndex(annot), true);
			Marshal.ReleaseComObject(checkBoxCh);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.4. Add Radio Buttons form fields")]
		static public void AddRadioButtonsFormFields(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding group with RadioButtons
			IPXS_Inst pxsInst = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.top = rc.top - 0.35 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)
			rcPB.left = 1.7 * 72.0;
			rcPB.right = rcPB.left + 0.5 * 72.0;
			IPXC_FormField firstGroup = Parent.m_CurDoc.AcroForm.CreateField("RadioButton", PXC_FormFieldType.FFT_RadioButton, 0, ref rcPB);
			rcPB.left = 2.3 * 72.0;
			rcPB.right = rcPB.left + 0.5 * 72.0;
			Parent.m_CurDoc.AcroForm.CreateField("RadioButton", PXC_FormFieldType.FFT_RadioButton, 0, ref rcPB);
			rcPB.left = 2.9 * 72.0;
			rcPB.right = rcPB.left + 0.5 * 72.0;
			Parent.m_CurDoc.AcroForm.CreateField("RadioButton", PXC_FormFieldType.FFT_RadioButton, 0, ref rcPB);

			rcPB.right = rc.right - 1.5 * 72.0;
			rcPB.left = rcPB.right - 0.5 * 72.0;
			IPXC_FormField secondGroup = Parent.m_CurDoc.AcroForm.CreateField("RadioButton1", PXC_FormFieldType.FFT_RadioButton, 0, ref rcPB);
			rcPB.top = rcPB.top - 0.6 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0;
			Parent.m_CurDoc.AcroForm.CreateField("RadioButton1", PXC_FormFieldType.FFT_RadioButton, 0, ref rcPB);
			firstGroup.CheckWidget(1, true);
			secondGroup.CheckWidget(1, true);

			Marshal.ReleaseComObject(firstGroup);
			Marshal.ReleaseComObject(secondGroup);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.5. Add ListBox form fields")]
		static public void AddListFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding ListBox with items
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 1.5 * 72.0;
			rcPB.right = rc.right - 1.5 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 5.0 * 72.0; //top is greater then bottom (PDF Coordinate System)

			IPXC_FormField listBox = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "ListBox", ref index), PXC_FormFieldType.FFT_ListBox, 0, ref rcPB);
			listBox.InsertOptRecord("label1", "First Item");
			listBox.InsertOptRecord("label2", "Second Item");
			listBox.InsertOptRecord("label3", "Third Item");
			listBox.InsertOptRecord("label4", "Forth Item");
			listBox.SelectItem(1, true);
			Marshal.ReleaseComObject(listBox);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.6. Add Combo Box form fields")]
		static public void AddComboBoxFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding ComboBox with items
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 1.5 * 72.0;
			rcPB.right = rc.right - 1.5 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)

			IPXC_FormField comboBox = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "ComboBox", ref index), PXC_FormFieldType.FFT_ComboBox, 0, ref rcPB);
			comboBox.InsertOptRecord("lable1", "First Item");
			comboBox.InsertOptRecord("lable2", "Second Item");
			comboBox.InsertOptRecord("lable3", "Third Item");
			comboBox.InsertOptRecord("lable4", "Forth Item");
			comboBox.SelectItem(1, true);
			Marshal.ReleaseComObject(comboBox);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.7. Add Signature form fields")]
		static public void AddSignatureFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding Signature
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 1.5 * 72.0;
			rcPB.right = rc.right - 1.5 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)

			IPXC_FormField signature = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Signature", ref index), PXC_FormFieldType.FFT_Signature, 0, ref rcPB);
			Marshal.ReleaseComObject(signature);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.8. Add Barcode form fields")]
		static public void AddBarcodeFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding Barcode
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 2.5 * 72.0;
			rcPB.right = rc.right - 2.5 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 1.5 * 72.0; //top is greater then bottom (PDF Coordinate System)

			IPXC_FormField Barcode = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Barcode", ref index), PXC_FormFieldType.FFT_Barcode, 0, ref rcPB);
			Marshal.ReleaseComObject(Barcode);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.9. Add Date form fields")]
		static public void AddDateFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding Date 
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 2.5 * 72.0;
			rcPB.right = rc.right - 2.5 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 0.5 * 72.0; //top is greater then bottom (PDF Coordinate System)

			IPXC_FormField Date = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Date", ref index), PXC_FormFieldType.FFT_Text, 0, ref rcPB);
			IPXC_Annotation annot = Date.Widget[0];
			IPXC_AnnotData_Widget widget = (IPXC_AnnotData_Widget)annot.Data;
			IPXC_ActionsList actionsList = Parent.m_CurDoc.CreateActionsList();
			//Set script to ActionList
			actionsList.AddJavaScript("var now = new Date()\n" +
						"this.getField(\"Date1\").value = (now.getMonth() + 1) + \"/\" + now.getDate() + \"/\" + now.getFullYear(); ");
			Date.Actions[PXC_TriggerType.Trigger_Format] = actionsList;
			Marshal.ReleaseComObject(Date);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);

		}

		[Description("5.10. Add Image form fields")]
		static public void AddImageFormFields(Form1 Parent)
		{
			int index = 1;
			//delegate string CheckNamesFields(IPXC_Document Doc, string fieldName, ref int x);
			CheckNamesFields checkNamesFields = (IPXC_Document Doc, string fName, ref int inx) =>
			{
				string sFieldName = "";
				uint i = 0;
				do
				{
					sFieldName = fName + inx;
					IPXC_FormField ff = Doc.AcroForm.GetFieldByName(sFieldName);
					if (ff == null)
					{
						break;
					}
					inx++;
					i++;
				}
				while (i <= Doc.AcroForm.FieldsCount);
				inx++;
				return sFieldName;
			};

			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			PXC_Rect rc = new PXC_Rect();
			rc.top = 800;
			rc.right = 600;

			//Adding button with icon as Image
			IPXC_UndoRedoData urD = null;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages.InsertPage(0, rc, out urD);
			PXC_Rect rcPB = new PXC_Rect();
			rcPB.left = 1.5 * 72.0;
			rcPB.right = rcPB.left + 1.0 * 72.0;
			rcPB.top = rc.top - 1.5 * 72.0;
			rcPB.bottom = rcPB.top - 2.0 * 72.0; //top is greater then bottom (PDF Coordinate System)
			IPXC_FormField Image = Parent.m_CurDoc.AcroForm.CreateField(checkNamesFields(Parent.m_CurDoc, "Image", ref index), PXC_FormFieldType.FFT_PushButton, 0, ref rcPB);
			//Now we'll need to add the icon
			IPXC_Annotation annot = Image.Widget[0];
			IPXC_AnnotData_Widget WData = (IPXC_AnnotData_Widget)annot.Data;
			string sPath = System.Environment.CurrentDirectory + "\\Images\\Editor_welcome.png";
			IPXC_Image img = Parent.m_CurDoc.AddImageFromFile(sPath);
			float imgw = img.Width;
			float imgh = img.Height;
			IPXC_ContentCreator CC = Parent.m_CurDoc.CreateContentCreator();
			CC.SaveState();
			CC.ScaleCS(imgw, imgh); //the image will be scaled to the button's size
			CC.PlaceImage(img);
			CC.RestoreState();
			IPXC_Content content = CC.Detach();
			PXC_Rect rcBBox;
			rcBBox.left = 0;
			rcBBox.top = imgh;
			rcBBox.right = imgw;
			rcBBox.bottom = 0;
			content.set_BBox(rcBBox);
			IPXC_XForm xForm = Parent.m_CurDoc.CreateNewXForm(ref rcPB);
			xForm.SetContent(content);
			WData.ButtonTextPosition = PXC_WidgetButtonTextPosition.WidgetText_IconOnly;
			WData.SetIcon(PXC_AnnotAppType.AAT_Normal, xForm, true);
			annot.Data = WData;
			Marshal.ReleaseComObject(Image);
			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.11. Remove fields from page")]
		static public void RemoveFieldsFromPage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
			{
				Document.OpenDocumentFromStream(Parent);
				Parent.UpdateControlsFromDocument(0);
				Parent.UpdatePreviewFromCurrentDocument();
			}
				

			//Get AcroForm from Document
			IPXC_AcroForm acroForm = Parent.m_CurDoc.AcroForm;
			for (int i = (int)acroForm.FieldsCount - 1; i >= 0; i--)
			{
				IPXC_FormField field = acroForm.Field[(uint)i];
				if (field == null)
					continue;
				acroForm.DeleteField(field.FullName);
				Marshal.ReleaseComObject(field);
			}
		}

		[Description("5.12. Flatten fields by name")]
		static public void FlattenFieldsByName(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
			{
				Document.OpenDocumentFromStream(Parent);
				Parent.UpdateControlsFromDocument(0);
				Parent.UpdatePreviewFromCurrentDocument();
			}
			//Get AcroForm from Document
			IPXC_AcroForm acroForm = Parent.m_CurDoc.AcroForm;
			for (int i = (int)acroForm.FieldsCount - 1; i >= 0; i--)
			{
				IPXC_FormField field = acroForm.Field[(uint)i];
				if (field == null)
					continue;
				acroForm.FlattenField(field.FullName);
				Marshal.ReleaseComObject(field);
			}
		}

		[Description("5.13. Flatten all fields on page")]
		static public void FlattenAllFieldsOnPage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
			{
				Document.OpenDocumentFromStream(Parent);
				Parent.UpdateControlsFromDocument(0);
				Parent.UpdatePreviewFromCurrentDocument();
			}
			//Getting current page
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages[Parent.CurrentPage];
			//Get AcroForm from Document
			IPXC_AcroForm acroForm = Parent.m_CurDoc.AcroForm;
			acroForm.FlattenAllFields();

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
		}

		[Description("5.14. Export all fields from the current page")]
		static public void ExportAllFieldsOnPage(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
			{
				Document.OpenDocumentFromStream(Parent);
				Parent.UpdateControlsFromDocument(0);
				Parent.UpdatePreviewFromCurrentDocument();
			}
			//Getting current page
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page Page = pages[Parent.CurrentPage];
			//Locking CosDocument for reading
			Parent.m_CurDoc.CosDocument.LockDocument();
			//Creating new array that will store the variants of the needed form fields
			IPXS_Inst pxsInst = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			IPXS_PDFVariant arrVar = pxsInst.NewVar_Array(0, Parent.m_CurDoc.CosDocument);
			//Get AcroForm from Document
			IPXC_AcroForm acroForm = Parent.m_CurDoc.AcroForm;
			for (uint i = 0; i < acroForm.FieldsCount; i++)
			{
				IPXC_FormField field = acroForm.Field[i];
				if (field == null)
					continue;
				arrVar.Arr_Insert(field.PDFObject);
				Marshal.ReleaseComObject(field);
			}
			Parent.m_CurDoc.CosDocument.UnlockDocument();

			Marshal.ReleaseComObject(Page);
			Marshal.ReleaseComObject(pages);
			if (arrVar.Count == 0)
				return;
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "FDF Documents (*.fdf)|*.fdf|XFDF Documents (*.xfdf)|*.xfdf|HTML Documents (*.html)|*.hrml|XML Documents (*.xml)|*.xml";
			sfd.FilterIndex = 4;
			sfd.CheckPathExists = true;
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				IAFS_Inst afsInst = (IAFS_Inst)Parent.m_pxcInst.GetExtension("AFS");
				IAFS_Name name = afsInst.DefaultFileSys.StringToName(sfd.FileName);
				string sExt = Path.GetExtension(sfd.FileName).ToLower();
				sExt = sExt.TrimStart('.');
				acroForm.Export(name, sExt, arrVar, true, (uint)PXC_ExportFormFlags.FormExport_Default);
				Process.Start("explorer.exe", "/select, \"" + sfd.FileName + "\"");
			}
		}
	}
}
