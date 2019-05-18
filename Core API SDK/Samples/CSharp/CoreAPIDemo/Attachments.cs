using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{
	[Description("13. Attachments")]
	class Attachments
	{
		
		[Description("13.1. Add attachment as an annotation")]
		static public int AddAttachmentAsAnnotation(Form1 Parent)
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

		[Description("13.2. Add attachment to the Embedded Files tree")]
		static public int AddAttachment(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.CreateNewDoc(Parent);

			IAFS_Inst afsInst = Parent.m_pxcInst.GetExtension("AFS");

			IPXC_NameTree attachments = Parent.m_CurDoc.GetNameTree("EmbeddedFiles");
			IPXS_PDFVariant var = null;
			string path = Environment.CurrentDirectory + "\\Documents\\FeatureChartEU.pdf";
			IPXC_FileSpec fileSpec = Parent.m_CurDoc.CreateEmbeddFile(path);
			IPXC_EmbeddedFileStream embeddedFileStream = fileSpec.EmbeddedFile;;
			embeddedFileStream.UpdateFromFile2(path);
			var = fileSpec.PDFObject;
			attachments.Add("FeatureChartEU.pdf", var);
			Marshal.ReleaseComObject(attachments);
			return (int)Form1.eFormUpdateFlags.efuf_Attachments | (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("13.3. Remove selected attachment from the Attachments list")]
		static public int RemoveAttachment(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			if (Parent.AttachmentView.SelectedItems.Count == 0)
			{
				MessageBox.Show("Please select attachment from the Attachments list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}

			Form1.ListItemAttachment currentAnnot = Parent.AttachmentView.SelectedItems[0] as Form1.ListItemAttachment;
			if (currentAnnot.SubItems[currentAnnot.SubItems.Count - 1].Text == "Embedded File Item")
			{
				IPXC_NameTree attachments = Parent.m_CurDoc.GetNameTree("EmbeddedFiles");
				attachments.Remove(currentAnnot.SubItems[0].Text);
				return (int)Form1.eFormUpdateFlags.efuf_Attachments | (int)Form1.eFormUpdateFlags.efuf_Annotations;
			}
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[(uint)(currentAnnot.m_nPageNumber)];
			page.RemoveAnnots((uint)currentAnnot.m_nIndexOnPage, 1);

			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);


			return (int)Form1.eFormUpdateFlags.efuf_Attachments | (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}

		[Description("13.4. Change selected attachment's description")]
		static public int ChangeAttachmentsDescription(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			if (Parent.AttachmentView.SelectedItems.Count == 0)
			{
				MessageBox.Show("Please select attachment from the Attachments list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}

			Form1.ListItemAttachment currentAnnot = Parent.AttachmentView.SelectedItems[0] as Form1.ListItemAttachment;
			if (currentAnnot.SubItems[currentAnnot.SubItems.Count - 1].Text == "Embedded File Item")
			{
				IPXC_NameTree attachments = Parent.m_CurDoc.GetNameTree("EmbeddedFiles");
				IPXS_PDFVariant pdfVariant = attachments.Lookup(currentAnnot.SubItems[0].Text);
				IPXC_FileSpec fileSpec = Parent.m_CurDoc.GetFileSpecFromVariant(pdfVariant);
				fileSpec.Description = "Description";
				Marshal.ReleaseComObject(attachments);

				return (int)Form1.eFormUpdateFlags.efuf_Attachments | (int)Form1.eFormUpdateFlags.efuf_Annotations;
			}
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Annotation annotFileAttach = pages[(uint)currentAnnot.m_nPageNumber].GetAnnot((uint)currentAnnot.m_nIndexOnPage);
			IPXC_AnnotData_FileAttachment fileAttachment = annotFileAttach.Data as IPXC_AnnotData_FileAttachment;
			IPXC_FileSpec annotAttachFileSpec = fileAttachment.FileAttachment;
			annotAttachFileSpec.Description = "Description";

			return (int)Form1.eFormUpdateFlags.efuf_Attachments | (int)Form1.eFormUpdateFlags.efuf_Annotations;
		}
	}
}
