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
	}
}
