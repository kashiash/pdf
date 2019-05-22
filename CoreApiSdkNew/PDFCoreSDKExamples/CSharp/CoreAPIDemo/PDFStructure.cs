using PDFXCoreAPI;
using System.ComponentModel;

namespace CoreAPIDemo
{
	[Description("14. PDF Structure Document")]
	class PDFStructure
	{
		[Description("14.1. Add new dictionary to the Root dictionary")]
		static public int AddNewDictionaryToDocumentsRoot(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXS_Inst pxsInst = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			//Getting structure document
			IPXS_Document structDoc = Parent.m_CurDoc.CosDocument;
			//Locking document for write
			structDoc.LockDocumentExclusive();
			//Creating new indirect variant of the Dictionary type that will hold some elements
			IPXS_PDFVariant varHolder = pxsInst.NewVar_Dict(structDoc, true);
			//Adding our newly created variant to the root dictionary
			structDoc.Root.Dict_Set("TestDataHolder", varHolder);
			//Creating an item that will hold some custom data and will link to the bookmarks root
			IPXS_PDFVariant varBookItem = pxsInst.NewVar_Dict(structDoc, true);
			//Adding that item to the holder dictionary
			varHolder.Dict_Set("BookmarkItem", varBookItem);
			//Setting some custom fields for this item
			varBookItem.Dict_SetBool("IsRoot", true);
			varBookItem.Dict_SetName("CustName", "Root Link");
			varBookItem.Dict_SetInt("CustInt", 123);
			//Setting a link to the bookmark's Root object
			IPXC_Bookmark root = Parent.m_CurDoc.BookmarkRoot;
			//If we have at least one bookmark in the document, then the root won't be null
			if (root != null)
				varBookItem.Dict_Set("BookmarkLink", root.PDFObject);
			//Unlocking the document, since we've finished all of the work with it
			structDoc.UnlockDocumentExclusive();

			return (int)Form1.eFormUpdateFlags.efuf_All;
		}

		[Description("14.2. Read signature information from structure")]
		static public int ReadSignatureInfoFromStructure(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				Document.OpenDocFromStringPath(Parent);
			IPXS_Inst pxsInst = (IPXS_Inst)Parent.m_pxcInst.GetExtension("PXS");
			//Getting structure document
			IPXS_Document structDoc = Parent.m_CurDoc.CosDocument;
			//Locking document for read
			structDoc.LockDocument();
			//Getting Pages dictionary
			IPXS_PDFVariant pagesDict = structDoc.Root.Dict_Get("Pages");
			//Getting pages array dictionary
			IPXS_PDFVariant kidsDict = pagesDict.Dict_Get("Kids");
			//Getting running through pagess
			for (uint p = 0; p < kidsDict.Count; p++)
			{
				IPXS_PDFVariant pageDict = kidsDict[p];
				//Getting annotations dictionary
				IPXS_PDFVariant annotsDict = pageDict.Dict_Get("Annots");
				for (uint i = 0; i < annotsDict.Count; i++)
				{
					//Getting annotation by index
					IPXS_PDFVariant annotItem = annotsDict[i];
					//Getting subtype
					IPXS_PDFVariant subtypeDict = annotItem.Dict_Get("Subtype");
					string sName = subtypeDict.GetName();
					if (sName != "Widget")
						continue;
					//Getting FT node
					IPXS_PDFVariant ftDict = annotItem.Dict_Get("FT");
					string sFT = ftDict.GetName();
					if (sFT != "Sig")
						continue;
					//If it is a signature then getting the V node with it's info
					IPXS_PDFVariant vDict = annotItem.Dict_Get("V");
					//Getting Reason dict
					IPXS_PDFVariant reasonDict = vDict.Dict_Get("Reason");
					string sReason = reasonDict.GetString();
					//Getting ContactInfo
					IPXS_PDFVariant contactInfoDict = vDict.Dict_Get("ContactInfo");
					string sContactInfo = contactInfoDict.GetString();
					System.Windows.Forms.MessageBox.Show(sReason + "\n" + sContactInfo);
				}
			}

			//Unlocking the document, since we've finished all of the work with it
			structDoc.UnlockDocument();


			return (int)Form1.eFormUpdateFlags.efuf_None;
		}
	}
}
