using System;
using System.Collections.Generic;
using System.ComponentModel;
using PDFXCoreAPI;
using System.Windows.Forms;
using System.Security;
using System.Runtime.InteropServices;

namespace CoreAPIDemo
{
	[Description("10. Bookmarks")]
	class Bookmarks
	{
		delegate void SortByAnything(SortByAnything sort, IPXC_Bookmark bookmark, uint actionType);
		delegate PXC_Point GetXYFromDestination(IPXC_Document document, PXC_Destination dest);

		[Description("10.1. Add Bookmark after the currently selected bookmark in the Bookmarks Tree")]
		static public int AddSiblingBookmark(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;
			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewSibling(false);
			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = (Parent.CurrentPage + 1) + " page";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = Parent.CurrentPage;
			dest.nNullFlags = 12;
			dest.nType = PXC_DestType.Dest_XYZ;
			double[] point = { 20, 30, 0, 0 };
			dest.dValues = point;
			aList.AddGoto(dest);
			bookmark.Actions = aList;
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.2. Add Bookmark as a last child of the currently selected bookmark in the Bookmarks Tree")]
		static public int AddChildBookmark(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			IPXC_Bookmark bookmark = null;
			if (Parent.SelectedBookmarkNode == null)
				bookmark = Parent.m_CurDoc.BookmarkRoot.AddNewChild(true);
			else
				bookmark = Parent.SelectedBookmarkNode.m_Bookmark.AddNewChild((Parent.SelectedBookmarkNode.m_Bookmark.ChildrenCount > 0));
			IPXC_ActionsList aList = Parent.m_CurDoc.CreateActionsList();
			bookmark.Title = (Parent.CurrentPage + 1) + " page";
			bookmark.Style = PXC_BookmarkStyle.BookmarkFont_Normal;
			PXC_Destination dest = new PXC_Destination();
			dest.nPageNum = Parent.CurrentPage;
			dest.nNullFlags = 0;
			dest.nType = PXC_DestType.Dest_FitR;
			IPXC_Pages pages = Parent.m_CurDoc.Pages;
			IPXC_Page page = pages[Parent.CurrentPage];
			PXC_Rect rc = page.get_Box(PXC_BoxType.PBox_BBox);
			double[] rect = { rc.left, rc.bottom, rc.right, rc.top };
			dest.dValues = rect;
			aList.AddGoto(dest);
			bookmark.Actions = aList;
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);

			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.3. Remove currently selected bookmark in the Bookmarks Tree with it's children")]
		static public int RemoveSelectedBookmark(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			if (Parent.SelectedBookmarkNode == null)
			{
				MessageBox.Show("There are no selected bookmarks - please select a bookmark from the Bookmarks Tree", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}
			Parent.SelectedBookmarkNode.m_Bookmark.Unlink();
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.4. Remove currently selected bookmark in the Bookmarks Tree without it's children")]
		static public int RemoveSelectedBookmarkWithoutChildren(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			if (Parent.SelectedBookmarkNode == null)
			{
				MessageBox.Show("There are no selected bookmarks - please select a bookmark from the Bookmarks Tree", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}
			IPXC_Bookmark selBookmark = Parent.SelectedBookmarkNode.m_Bookmark;

			while (selBookmark.ChildrenCount > 0)
			{
				IPXC_Bookmark childBookmark = selBookmark.FirstChild;
				selBookmark.FirstChild.Unlink();
				selBookmark.AddSibling(childBookmark, false);
			}
			selBookmark.Unlink();
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.5. Move Up currently selected bookmark in the Bookmarks Tree")]
		static public int MoveUpSelectedBookmark(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;

			if (Parent.SelectedBookmarkNode == null)
			{
				MessageBox.Show("There are no selected bookmarks - please select a bookmark from the Bookmarks Tree", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}
			IPXC_Bookmark pxcBookmark = Parent.SelectedBookmarkNode.m_Bookmark;
			if (pxcBookmark == Parent.m_CurDoc.BookmarkRoot.FirstChild)
				return 0;

			if (pxcBookmark == pxcBookmark.Parent.FirstChild)
			{
				IPXC_Bookmark parent = pxcBookmark.Parent;
				pxcBookmark.Unlink();
				parent.AddSibling(pxcBookmark, true);
			}
			else
			{
				IPXC_Bookmark prevBookmark = pxcBookmark.Previous;
				pxcBookmark.Unlink();
				prevBookmark.AddSibling(pxcBookmark, true);
			}
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.6. Move Down currently selected bookmark in the Bookmarks Tree")]
		static public int MoveDownSelectedBookmark(Form1 Parent)
		{
			if (Parent.m_CurDoc == null)
				return 0;
			if (Parent.SelectedBookmarkNode == null)
			{
				MessageBox.Show("There are no selected bookmarks - please select a bookmark from the Bookmarks Tree", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return 0;
			}
			IPXC_Bookmark pxcBookmark = Parent.SelectedBookmarkNode.m_Bookmark;
			if (pxcBookmark == Parent.m_CurDoc.BookmarkRoot.LastChild)
				return 0;

			if (pxcBookmark == pxcBookmark.Parent.LastChild)
			{
				IPXC_Bookmark parent = pxcBookmark.Parent;
				pxcBookmark.Unlink();
				parent.AddSibling(pxcBookmark, false);
			}
			else
			{
				IPXC_Bookmark nextBookmark = pxcBookmark.Next;
				pxcBookmark.Unlink();
				nextBookmark.AddSibling(pxcBookmark, false);
			}
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.7. Sort bookmarks by name in the current document")]
		static public int SortBookmarksByName(Form1 Parent)
		{
			//delegate void SortByAnything(SortByAnything sort, IPXC_Bookmark bookmark, uint actionType);
			SortByAnything sortByAnything = (sort, root, actionType) => {
				List<IPXC_Bookmark> bookmarks = new List<IPXC_Bookmark>();
				while (root.ChildrenCount > 0)
				{
					bookmarks.Add(root.FirstChild);
					root.FirstChild.Unlink();
				}

				bookmarks.Sort(delegate (IPXC_Bookmark firstNode, IPXC_Bookmark secondNode)
				{
					return Form1.NativeMethods.StrCmpLogicalW(firstNode.Title, secondNode.Title);
				});

				foreach (IPXC_Bookmark bookmark in bookmarks)
				{
					if (bookmark.ChildrenCount > 0)
					{
						sort(sort, bookmark, actionType);
					}
					root.AddChild(bookmark, true);

				}
			};
			if (Parent.m_CurDoc == null)
				return 0;
			sortByAnything(sortByAnything, Parent.m_CurDoc.BookmarkRoot, 0);
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}

		[Description("10.8. Sort bookmarks by page in the current document")]
		static public int SortBookmarksByPage(Form1 Parent)
		{
			//delegate double[] GetXYFromDestination(IPXC_Bookmark bookmark, PXC_Destination dest);
			GetXYFromDestination getXYFromDestination = (IPXC_Document doc, PXC_Destination destination) => {
				PXC_DestType Type = destination.nType;
				PXC_Point retValue = new PXC_Point();
				IPXC_Pages pages = doc.Pages;
				IPXC_Page page = pages[destination.nPageNum];
				PXC_Rect contentBBox = page.get_Box(PXC_BoxType.PBox_BBox);
				PXC_Rect pageBBox = page.get_Box(PXC_BoxType.PBox_PageBox);
				bool IsContentType = (Type == PXC_DestType.Dest_FitB) || (Type == PXC_DestType.Dest_FitBH) || (Type == PXC_DestType.Dest_FitBV);
				retValue.x = IsContentType ? contentBBox.left : pageBBox.left;
				retValue.y = IsContentType ? contentBBox.top : pageBBox.top;
				switch (Type)
				{
				case PXC_DestType.Dest_XYZ:
					{
						if ((destination.nNullFlags & 1) == 0)
							retValue.x = destination.dValues[0];
						if ((destination.nNullFlags & 2) == 0)
							retValue.y = destination.dValues[1];
						break;
					}
				case PXC_DestType.Dest_FitH:
					{
						if ((destination.nNullFlags & 2) == 0)
							retValue.y = destination.dValues[1];
						break;
					}
				case PXC_DestType.Dest_FitV:
					{
						if ((destination.nNullFlags & 1) == 0)
							retValue.x = destination.dValues[0];
						break;
					}
				case PXC_DestType.Dest_FitR:
					{
						if ((destination.nNullFlags & 1) == 0)
							retValue.x = destination.dValues[0];
						if ((destination.nNullFlags & 8) == 0)
							retValue.y = destination.dValues[3];
						break;
					}
				case PXC_DestType.Dest_FitBH:
					{
						if ((destination.nNullFlags & 2) == 0)
							retValue.y = destination.dValues[1];
						break;
					}
				case PXC_DestType.Dest_FitBV:
					{
						if ((destination.nNullFlags & 1) == 0)
							retValue.x = destination.dValues[0];
						break;
					}
				default:
					break;
				}
				Marshal.ReleaseComObject(page);
				Marshal.ReleaseComObject(pages);
				return retValue;
			};
			//delegate void SortByAnything(SortByAnything sort, IPXC_Bookmark root);
			SortByAnything sortByAnything = (sort, root, actionType) => {
				List<Tuple<IPXC_Bookmark, PXC_Destination>> bookmarks = new List<Tuple<IPXC_Bookmark, PXC_Destination>>();
				int MAX_VALUE = int.MaxValue;
				PXC_Destination invalidDest = new PXC_Destination();
				invalidDest.nPageNum = (uint)MAX_VALUE;

				while (root.ChildrenCount > 0)
				{
					Tuple<IPXC_Bookmark, PXC_Destination> currentBookmark = Tuple.Create(root.FirstChild, invalidDest);
					if (root.FirstChild.Actions != null)
					{
						for (int i = (int)root.FirstChild.Actions.Count - 1; i >= 0; i--)
						{
							if (root.FirstChild.Actions[(uint)i].Type == actionType)
							{
								IPXC_Action_Goto actionGoTo = root.FirstChild.Actions[(uint)i] as IPXC_Action_Goto;
								PXC_Destination currDest = actionGoTo.IsNamedDest
									? Parent.m_CurDoc.GetNamedDestination(actionGoTo.DestName)
									: actionGoTo.get_Dest();
								currentBookmark = Tuple.Create(root.FirstChild, currDest);
								break;
							}
						}
					}
					root.FirstChild.Unlink();

					if ((bookmarks.Count == 0) || (currentBookmark.Item2.nPageNum > bookmarks[bookmarks.Count - 1].Item2.nPageNum))
					{
						bookmarks.Add(currentBookmark);
						continue;
					}
					else if (currentBookmark.Item2.nPageNum < bookmarks[0].Item2.nPageNum)
					{
						bookmarks.Insert(0, currentBookmark);
						continue;
					}

					int first = 0;
					int last = bookmarks.Count;

					while (first < last)
					{
						int mid = first + (last - first) / 2;
						if (currentBookmark.Item2.nPageNum == bookmarks[mid].Item2.nPageNum)
						{
							if ((MAX_VALUE == currentBookmark.Item2.nPageNum) && (MAX_VALUE == bookmarks[mid].Item2.nPageNum))
							{
								if (Form1.NativeMethods.StrCmpLogicalW(currentBookmark.Item1.Title, bookmarks[mid].Item1.Title) == 1)
									first = mid + 1;
								else
									last = mid;
							}
							else
							{
								PXC_Point currentBookmarkXY = getXYFromDestination(Parent.m_CurDoc, currentBookmark.Item2);
								PXC_Point bookmarkXY_FromList = getXYFromDestination(Parent.m_CurDoc, bookmarks[mid].Item2);
								if (currentBookmarkXY.y < bookmarkXY_FromList.y)
								{
									first = mid + 1;
								}
								else if (currentBookmarkXY.y > bookmarkXY_FromList.y)
								{
									last = mid;
								}
								else
								{
									if (currentBookmarkXY.x < bookmarkXY_FromList.x)
										last = mid;
									else
										first = mid + 1;
								}
							}
						}
						else if (currentBookmark.Item2.nPageNum < bookmarks[mid].Item2.nPageNum)
						{
							last = mid;
						}
						else
						{
							first = mid + 1;
						}
					}
					bookmarks.Insert(last, currentBookmark);
				}

				foreach(Tuple<IPXC_Bookmark, PXC_Destination> bookmark in bookmarks)
				{
					root.AddChild(bookmark.Item1, true);
					if (bookmark.Item1.ChildrenCount > 0)
					{
						sort(sort, bookmark.Item1, actionType);
					}				
				}
			};
			if (Parent.m_CurDoc == null)
				return 0;

			IPXS_Inst pxsInst = Parent.m_pxcInst.GetExtension("PXS") as IPXS_Inst;
			uint nGoTo = pxsInst.StrToAtom("GoTo");
			sortByAnything(sortByAnything, Parent.m_CurDoc.BookmarkRoot, nGoTo);
			return (int)Form1.eFormUpdateFlags.efuf_Bookmarks;
		}
	}
}
