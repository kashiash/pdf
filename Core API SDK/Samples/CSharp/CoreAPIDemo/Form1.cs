using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using PDFXCoreAPI;

namespace CoreAPIDemo
{

	public partial class Form1 : Form
	{
		public IPXC_Inst				m_pxcInst = null;
		public IPXC_Document			m_CurDoc = null;
#if DEBUG
		public static string m_sDirPath = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName + "\\CSharp\\CoreAPIDemo\\";
#else
		public static string		m_sDirPath = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).FullName + "\\CSharp\\CoreAPIDemo\\";
#endif
		public string					m_sFilePath = "";
		public uint CurrentPage
		{
			get { return uint.Parse(currentPage.Text) - 1; }
		}

		public TreeView GetBookmarkTree
		{
			get { return bookmarksTree; }
		}

		public ListViewItem SelectedNameDest_Item
		{
			get { return namedDestsList.SelectedItems.Count == 0 ? null : namedDestsList.SelectedItems[0]; }
		}

		public ListView GetNamedDestinationList
		{
			get { return namedDestsList; }
		}

		public BookmarkNode SelectedBookmarkNode
		{
			get { return bookmarksTree.SelectedNode as BookmarkNode; }
			set { bookmarksTree.SelectedNode = value; }
		}

		public ListView AttachmentView
		{
			get { return attachmentView; }
		}

		public enum eFormUpdateFlags
		{
			efuf_None					= 0,
			efuf_Bookmarks				= 0x1,
			efuf_NamedDests				= 0x2,
			efuf_Annotations			= 0x4,
			efuf_Attachments			= 0x8,
			efuf_All					= 0xff,
		}
		public enum eFormNameDestinationFlags
		{
			efcsf_None					= 0,
			efcsf_Destination			= 1,
			efcsf_Ascending				= 2,
			efcsf_Descending			= 3
		}

		public enum eFormAnnotationType
		{
			efat_Text					= 0,
			efat_Link					= 1,
			efat_FreeText				= 2,
			efat_Line					= 3,
			efat_SquareAndCircle		= 4,
			efat_PolygoneAndPolyline	= 5,
			efat_TextMarkup				= 6,
			efat_Popup					= 7,
			efat_FileAttachment			= 8,
			efat_Redact					= 9
		}

		public Form1()
		{
			m_pxcInst = new PXC_Inst();
			m_pxcInst.Init("");
			InitializeComponent();
		}

		[DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
		private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

		[SuppressUnmanagedCodeSecurity]
		internal static class NativeMethods
		{
			[DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
			public static extern int StrCmpLogicalW(string psz1, string psz2);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			SetWindowTheme(sampleTree.Handle, "explorer", null);
			SetWindowTheme(bookmarksTree.Handle, "explorer", null);
			SetWindowTheme(namedDestsList.Handle, "explorer", null);
			SetWindowTheme(annotsView.Handle, "explorer", null);
			SetWindowTheme(attachmentView.Handle, "explorer", null);
			ImageList il = new ImageList();
			try
			{
				string sImgFolder = System.Environment.CurrentDirectory + "\\Images\\";
				Bitmap img = new Bitmap(sImgFolder + "folder_24.png");
				il.Images.Add(img);
				img = new Bitmap(sImgFolder + "run_24.png");
				il.Images.Add(img);
				img = new Bitmap(sImgFolder + "runGreyed_24.png");
				il.Images.Add(img);
				sampleTree.ImageList = il;
				sampleTree.TreeViewNodeSorter = new NodeSorter();

				ImageList ilfb = new ImageList();
				img = new Bitmap(sImgFolder + "bookmark_24.png");
				ilfb.Images.Add(img);
				bookmarksTree.ImageList = ilfb;

				ImageList ilnd = new ImageList();
				img = new Bitmap(1, 1);
				ilnd.Images.Add(img);
				img = new Bitmap(sImgFolder + "dest_24.png");
				ilnd.Images.Add(img);
				img = new Bitmap(sImgFolder + "sortAsc_24.png");
				ilnd.Images.Add(img);
				img = new Bitmap(sImgFolder + "sortDesc_24.png");
				ilnd.Images.Add(img);
				namedDestsList.SmallImageList = ilnd;


				namedDestsList.Columns[0].ImageIndex = (int)eFormNameDestinationFlags.efcsf_None;
				namedDestsList.Columns[1].ImageIndex = (int)eFormNameDestinationFlags.efcsf_None;

				ImageList ila = new ImageList();
				img = new Bitmap(1, 1);
				ila.Images.Add(img);
				img = new Bitmap(sImgFolder + "annot_24.png");
				ila.Images.Add(img);

				annotsView.SmallImageList = ila;
				annotsView.Columns[0].ImageIndex = 0;
				annotsView.Columns[1].ImageIndex = 0;

				ImageList ilattch = new ImageList();
				img = new Bitmap(sImgFolder + "attachment_24.png");
				ilattch.Images.Add(img);

				attachmentView.SmallImageList = ilattch;

				addAnnotType.SelectedIndex = (int)eFormAnnotationType.efat_SquareAndCircle;
			}
			catch (Exception)
			{
			}
			RefillTree();
		}

		public class NodeSorter : IComparer
		{
			// Compare the length of the strings, or the strings
			// themselves, if they are the same length.
			public int Compare(object x, object y)
			{
				TreeNode tx = x as TreeNode;
				TreeNode ty = y as TreeNode;

				int index = tx.Text.IndexOf(' ');
				string sXNum = tx.Text.Substring(0, index);
				index = ty.Text.IndexOf(' ');
				string sYNum = ty.Text.Substring(0, index);

				string[] aXNums = sXNum.Split('.');
				string[] aYNums = sYNum.Split('.');
				for (int i = 0; i < aXNums.Length; i++)
				{
					if (i >= aYNums.Length)
						return 1; //x is greater
					int nX = int.Parse(aXNums[i]);
					int nY = int.Parse(aYNums[i]);
					if (nX == nY)
						continue;
					return (nX > nY) ? 1 : -1;
				}
				return 0;
			}
		}

		private void RefillTree()
		{
			sampleTree.BeginUpdate();
			sampleTree.Nodes.Clear();
			Type[] typeList = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type t in typeList)
			{
				AddClassToTree(t);
			}

			sampleTree.Sort();
			sampleTree.EndUpdate();
		}
		private void AddClassToTree(Type classType)
		{
			DescriptionAttribute attr = classType.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
			if (attr == null)
				return;

			string[] aFilters = filterEdit.Text.Split(' ');
			TreeNode root = sampleTree.Nodes.Insert(-1, attr.Description);
			root.ImageIndex = 0;
			root.SelectedImageIndex = 0;
			foreach (MethodInfo mi in classType.GetMethods())
			{
				attr = mi.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (attr == null)
					continue;
				bool bSuccess = true;
				for (int i = 0; i < aFilters.Length; i++)
				{
					if ((aFilters[i].Length == 0) && (aFilters.Length > 1))
						continue;
					if (attr.Description.IndexOf(aFilters[i], StringComparison.CurrentCultureIgnoreCase) < 0)
					{
						bSuccess = false;
						break;
					}
				}
				if (bSuccess)
				{
					TreeNode child = root.Nodes.Insert(-1, classType.Name + "_" + mi.Name, attr.Description);
					if (child != null)
					{
						child.ImageIndex = 2;
						child.SelectedImageIndex = 1;
					}
				}
			}
			if (root.Nodes.Count == 0)
				sampleTree.Nodes.Remove(root);
		}

		public class BookmarkNode : TreeNode
		{
			public BookmarkNode(IPXC_Bookmark book)
			{
				m_Bookmark = book;
			}
			~BookmarkNode()
			{
				m_Bookmark = null;
			}
			public IPXC_Bookmark m_Bookmark = null;
		}

		public class ListItemDestination : ListViewItem
		{
			public int m_nPageNumber = 0;
		}

		public class ListItemAnnotation : ListItemDestination
		{
			public int m_nIndexOnPage = 0;
		}

		public class ListItemAttachment : ListItemAnnotation
		{
			public IPXC_EmbeddedFileStream m_pxcEmbeddedFileStream = null;
		}

		public int CountAllBookmarks(IPXC_Bookmark root)
		{
			int retValue = 0;
			IPXC_Bookmark child = root.FirstChild;
			for (int i = 0; i < root.ChildrenCount; i++)
			{
				if (child.ChildrenCount > 0)
					retValue += CountAllBookmarks(child);
				child = child.Next;
				retValue++;
			}
			return retValue;
		}
		public void AddBookmarkToTree(IPXC_Bookmark root, TreeNode node = null)
		{
			Action addBookmarkToTree = () =>
			{
				IPXC_Bookmark child = root.FirstChild;
				for (int i = 0; i < root.ChildrenCount; i++)
				{
					IPXC_ActionsList aList = child.Actions;
					BookmarkNode childNode = new BookmarkNode(child);
					childNode.Name = ((node != null) ? (node.Name + ".") : "Bookmark") + (i + 1);
					childNode.ImageIndex = 0;
					childNode.SelectedImageIndex = 0;
					childNode.Text = child.Title;
					if (child.ChildrenCount > 0)
						AddBookmarkToTree(child, childNode);
					if (node != null)
						node.Nodes.Add(childNode);
					else
						bookmarksTree.Nodes.Add(childNode);
					child = child.Next;
					bookmarkProgress.Value++;
				}
			};
			Invoke(addBookmarkToTree);
		}

		private MethodInfo GetCurrentMethod(TreeNode curNode)
		{
			do
			{
				TreeNode node = curNode;
				if (node == null)
					node = sampleTree.SelectedNode;
				if (node == null)
					break;
				int nIndex = node.Name.IndexOf('_');
				if (nIndex < 0)
					break;
				string sClass = node.Name.Substring(0, nIndex);
				string sMethod = node.Name.Substring(nIndex + 1);

				Type thisType = Type.GetType("CoreAPIDemo." + sClass + ", CoreAPIDemo");
				if (thisType == null)
					break;

				MethodInfo theMethod = thisType.GetMethod(sMethod);
				if (theMethod == null)
					break;
				return theMethod;
			} while (false);
			return null;
		}

		private int GetMethodLine(string className, string methodName)
		{
			string sPath = m_sDirPath + className + ".cs";
			string sData = System.IO.File.ReadAllText(sPath);
			int nIndex = sData.IndexOf(methodName);

			string[] aDataLen = sData.Substring(0, nIndex).Split('\n');
			return aDataLen.Length;
		}

		private string GetMethodCode(string className, string methodName)
		{
			try
			{
				string sPath = m_sDirPath + className + ".cs";
				string sData = System.IO.File.ReadAllText(sPath);
				int nIndex = sData.IndexOf(methodName);
				while (nIndex >= 0)
				{
					if (sData[nIndex] == '\n')
					{
						nIndex++;
						break;
					}
					nIndex--;
				}

				if (nIndex < 0)
					return "";

				string sWS = "";
				for (int i = nIndex; i < sData.Length; i++)
				{
					if (!Char.IsWhiteSpace(sData[i]))
					{
						sWS = sData.Substring(nIndex, i - nIndex);
						break;
					}
				}
				int nEnd = sData.IndexOf("\n" + sWS + "}", nIndex);
				if (nEnd < 0)
					return "";
				nEnd += sWS.Length + 2;
				string sRes = sData.Substring(nIndex, nEnd - nIndex);
				string[] aData = sRes.Split('\n');
				sRes = "";
				for (int i = 0; i < aData.Length; i++)
				{
					string sTmp = aData[i];
					int nPos = sTmp.IndexOf(sWS);
					if (nPos < 0)
						sRes += sTmp;
					else
						sRes += sTmp.Substring(sWS.Length);
				}
				return sRes;
			}
			catch (Exception)
			{

			}
			return "";
		}
		private void UpdateCodeSample(TreeNode curNode)
		{
			codeSource.Text = "";
			MethodInfo theMethod = GetCurrentMethod(curNode);
			if (theMethod == null)
				return;
			byte[] bytes = theMethod.GetMethodBody().GetILAsByteArray();
			codeSource.Text = GetMethodCode(theMethod.DeclaringType.Name, theMethod.Name);
		}

		private void InvokeMethod(TreeNode curNode)
		{
			MethodInfo theMethod = GetCurrentMethod(curNode);
			if (theMethod == null)
			{
				if ((curNode == null) || (curNode.Name.IndexOf('_') >= 0))
					MessageBox.Show("Please select a sample from the needed category in sample tree and click Run Sample to execute it.");
				return;
			}

			object invRes = theMethod.Invoke(this, new Object[] { this });

			UpdateControlsFromDocument((invRes == null) ? (int)eFormUpdateFlags.efuf_None : (int)invRes);
			UpdatePreviewFromCurrentDocument();
		}

		public void UpdatePreviewFromCurrentDocument()
		{
			if (m_CurDoc == null)
			{
				previewImage.Image = null;
				return;
			}
			IPXC_Pages pages = null;
			IPXC_Page srcPage = null;
			try
			{
				int nPage = int.Parse(currentPage.Text) - 1;
				pages = m_CurDoc.Pages;
				if (nPage >= pages.Count)
				{
					nPage = (int)pages.Count - 1;
					currentPage.Text = (nPage + 1).ToString();
				}

				srcPage = pages[(uint)nPage];
				IAUX_Inst auxInst = (IAUX_Inst)m_pxcInst.GetExtension("AUX");
				IIXC_Inst ixcInst = (IIXC_Inst)m_pxcInst.GetExtension("IXC");
				//Getting source page matrix
				PXC_Matrix srcPageMatrix = srcPage.GetMatrix(PXC_BoxType.PBox_PageBox);
				//Getting source page Page Box without rotation
				PXC_Rect srcRect = srcPage.get_Box(PXC_BoxType.PBox_PageBox);
				//Getting visual source Page Box by transforming it through matrix
				auxInst.MathHelper.Rect_Transform(srcPageMatrix, ref srcRect);
				//We'll insert the visual src page into visual dest page indented rectangle including page rotations and clipping

				PXC_Rect destRect;
				destRect.left = 0;
				destRect.right = previewImage.Width;
				destRect.top = 0;
				destRect.bottom = previewImage.Height;

				//Fit rect to rect
				{
					PXC_Rect rcRes, rcR, rcS;
					rcS = destRect;
					rcS.top = rcS.bottom;
					rcS.bottom = 0;
					rcR = srcRect;
					{
						var k1 = (rcR.right - rcR.left) / Math.Abs(rcR.top - rcR.bottom);
						var k2 = (rcS.right - rcS.left) / Math.Abs(rcS.top - rcS.bottom);

						if (k1 >= k2)
						{
							var h = (rcS.right - rcS.left) / k1;
							rcRes = rcS;
							rcRes.bottom += (Math.Abs(rcRes.top - rcRes.bottom) - h) / 2;
							rcRes.top = rcRes.bottom + h;
						}
						else
						{
							var w = Math.Abs(rcS.top - rcS.bottom) * k1;
							rcRes = rcS;
							rcRes.left += ((rcRes.right - rcRes.left) - w) / 2;
							rcRes.right = rcRes.left + w;
						}
					}

					destRect.left = 0;
					destRect.right = rcRes.right - rcRes.left;
					destRect.top = 0;
					destRect.bottom = rcRes.top - rcRes.bottom;
				}

				if (((int)(destRect.right - destRect.left) < 1) || ((int)(destRect.bottom - destRect.top) < 1))
					return;
				Bitmap image = new Bitmap((int)(destRect.right - destRect.left), (int)(destRect.bottom - destRect.top));


				PXC_Matrix pageToRectMatrix = auxInst.MathHelper.Matrix_RectToRect(srcRect, destRect);
				tagRECT rcTmp;
				rcTmp.left = (int)destRect.left;
				rcTmp.right = (int)destRect.right;
				rcTmp.top = (int)destRect.top;
				rcTmp.bottom = (int)destRect.bottom;
				pageToRectMatrix = auxInst.MathHelper.Matrix_Multiply(srcPageMatrix, pageToRectMatrix);

				Graphics g = Graphics.FromImage(image);
				g.Clear(Color.White);
				IntPtr hdc = g.GetHdc();
				srcPage.DrawToDevice((uint)hdc, ref rcTmp, ref pageToRectMatrix, 0/*(uint)PDFXEdit.PXC_DrawToDeviceFlags.DDF_AsVector*/);
				g.ReleaseHdc(hdc);
				previewImage.Image = image;
				g.Dispose();
				g = null;
			}
			finally
			{
				Marshal.ReleaseComObject(srcPage);
				Marshal.ReleaseComObject(pages);
			}
			
		}

		public void FillNamedDestinationsList()
		{
			IPXC_NameTree nameTree = null;
			ListItemDestination[] listItems = null;
			int count = 0;
			namedDestsList.Invoke((MethodInvoker)delegate
				{
					nameTree = m_CurDoc.GetNameTree("Dests");
					namedDestsList.BeginUpdate();
					namedDestsList.Items.Clear();
					namedDestsProgress.Visible = true;
					count = (int)nameTree.Count;
					namedDestsProgress.Maximum = count;
				});
			listItems = new ListItemDestination[count];
			for (int i = 0; i < count; i++)
			{
				string nameDest = "";
				string destPage = "";
				IPXS_PDFVariant pdfVariant = null;
				PXC_Destination dest = new PXC_Destination();
				Invoke((MethodInvoker)delegate
				{
					namedDestsProgress.Value = i;
					try
					{
						nameTree.Item((uint)i, out nameDest, out pdfVariant);
						dest = m_CurDoc.FillDestination(pdfVariant);
						destPage = (dest.nPageNum + 1).ToString();
					}
					catch (Exception)
					{
					}
				});
				ListItemDestination item = new ListItemDestination();
				item.Name = "item_" + i;
				item.Text = nameDest;
				item.ImageIndex = (int)eFormNameDestinationFlags.efcsf_Destination;
				if (destPage == "")
					item.m_nPageNumber = int.MaxValue;
				else
					item.m_nPageNumber = Convert.ToInt32(destPage);
				listItems[i] = item;
				listItems[i].SubItems.Add(destPage);
			}
			namedDestsList.Invoke((MethodInvoker)delegate
			{
				namedDestsProgress.Visible = false;
				namedDestsProgress.Value = 0;
				namedDestsList.Items.AddRange(listItems);
				namedDestsList.EndUpdate();
			});
			Marshal.ReleaseComObject(nameTree);
		}

		public uint GetAnnotsCount()
		{
			IPXC_Pages pages = m_CurDoc.Pages;
			uint retValue = 0;
			int pageCount = (int)pages.Count;
			for (int i = 0; i < pageCount; i++)
			{
				IPXC_Page page = pages[(uint)i];
				retValue += page.GetAnnotsCount();
				Marshal.ReleaseComObject(page);
			}
			Marshal.ReleaseComObject(pages);
			return retValue;
		}

		public void FillAttachmentItems(IPXC_Annotation annot, IPXC_NameTree nameTree, ref string[] attachmentsItems, int annotPageOrIdAttach, ref ListItemAttachment attachment)
		{
			IPXC_FileSpec fileSpec = null;
			IPXC_EmbeddedFileStream embeddedFileStream = null;
			if (annot != null)
			{
				IPXC_AnnotData_FileAttachment fileAttachment = annot.Data as IPXC_AnnotData_FileAttachment;
				fileSpec = fileAttachment.FileAttachment;
				embeddedFileStream = fileSpec.EmbeddedFile;
				string[] array = fileSpec.FileName.Split('/');
				attachmentsItems[0] = array[array.Length - 1];
			}
			else
			{
				IPXS_PDFVariant pdfVariant = null;
				nameTree.Item((uint)annotPageOrIdAttach, out attachmentsItems[0], out pdfVariant);
				fileSpec = m_CurDoc.GetFileSpecFromVariant(pdfVariant);
				embeddedFileStream = fileSpec.EmbeddedFile;
			}
			
			attachmentsItems[1] = fileSpec.Description;
			attachmentsItems[2] = embeddedFileStream.ModificationDate.ToString();
			attachmentsItems[3] = Math.Round(embeddedFileStream.UnCompressedSize / 1024.0, 2) + " KB (" + embeddedFileStream.UnCompressedSize + ") bytes.";
			attachmentsItems[4] = annot == null ? "Embedded File Item" : "Page " + (annotPageOrIdAttach + 1);
			attachment.m_pxcEmbeddedFileStream = embeddedFileStream;
		}

		public void FillAnnotationsList()
		{
			IPXC_Pages pages = null;
			int pageCount = 0;
			IPXS_Inst pxsInst = null;
			ListItemAnnotation[] listItems = null;
			ListItemAttachment[] attachments = new ListItemAttachment[0];
			Invoke((MethodInvoker)delegate
			{
				pages = m_CurDoc.Pages;
				pxsInst = m_pxcInst.GetExtension("PXS");
				annotsView.BeginUpdate();
				attachmentView.BeginUpdate();
				annotsView.Items.Clear();
				annotProgress.Visible = true;

				pageCount = (int)pages.Count;
				annotProgress.Maximum = pageCount;
				listItems = new ListItemAnnotation[GetAnnotsCount()];
			});
			int currentItem = 0;
			for (int i = 0; i < pageCount; i++)
			{
				IPXC_Page currentPage = null;
				int annotPage = i;
				int annotCount = 0;
				Invoke((MethodInvoker)delegate
				{
					currentPage = pages[(uint)i];
					annotCount = (int)currentPage.GetAnnotsCount();
				});

				for (int j = 0; j < annotCount; j++)
				{
					string annotType = "";
					uint atom = 0;
					Invoke((MethodInvoker)delegate
					{
						atom = currentPage.GetAnnot((uint)j).Type;
						annotType = pxsInst.AtomToStr(atom);
					});
					ListItemAnnotation item = new ListItemAnnotation();
					item.Name = "annot_" + i + "." + j;
					item.Text = annotType;
					item.ImageIndex = 1;
					item.m_nPageNumber = annotPage;
					item.m_nIndexOnPage = j;
					item.SubItems.Add((annotPage + 1).ToString());
					listItems[currentItem] = item;
					currentItem++;

					if (annotType == "FileAttachment")
					{
						string[] attachmentsItems = new string[5];  // Name, Description, Modified, Size, Location in Document
						ListItemAttachment attachment = new ListItemAttachment();
						Invoke((MethodInvoker)delegate
						{
							try
							{
								IPXC_Annotation annotFileAttach = currentPage.GetAnnot((uint)j);
								FillAttachmentItems(annotFileAttach, null, ref attachmentsItems, annotPage, ref attachment);
							}
							catch (Exception)
							{
							}
						});
						attachment.Name = "item_" + attachmentView.Items.Count;
						attachment.Text = attachmentsItems[0];
						attachment.m_nIndexOnPage = j;
						attachment.m_nPageNumber = annotPage;
						attachment.ImageIndex = 0;
						for (int k = 1; k < 5; k++)
						{
							attachment.SubItems.Add(attachmentsItems[k]);
						}
						Array.Resize(ref attachments, attachments.Length + 1);

						attachments[attachments.Length -1] = attachment;
					}
				}
				Invoke((MethodInvoker)delegate
				{
					annotProgress.Value++;
				});
				Marshal.ReleaseComObject(currentPage);
			}
			Invoke((MethodInvoker)delegate
			{
				annotProgress.Visible = false;
				annotProgress.Value = 0;
				annotsView.Items.AddRange(listItems);
				attachmentView.Items.AddRange(attachments);
				annotsView.EndUpdate();
				attachmentView.EndUpdate();
			});
			Marshal.ReleaseComObject(pages);
		}

		public void FillAttachmentsList()
		{
			IPXC_NameTree attachmentNameTree = null;
			ListItemAttachment[] listItems = null;
			int count = 0;
			Invoke((MethodInvoker)delegate
			{
				attachmentNameTree = m_CurDoc.GetNameTree("EmbeddedFiles");
				attachmentView.BeginUpdate();
				attachmentView.Items.Clear();
				count = (int)attachmentNameTree.Count;
			});
			listItems = new ListItemAttachment[count];
			for (int i = 0; i < count; i++)
			{
				string[] attachmentsItems = new string[5];	// Name, Description, Modified, Size, Location in Document
				ListItemAttachment item = new ListItemAttachment();
				Invoke((MethodInvoker)delegate
				{
					try
					{
						FillAttachmentItems(null, attachmentNameTree, ref attachmentsItems, i, ref item);
					}
					catch (Exception)
					{
					}
				});
				item.Name = "item_" + i;
				item.Text = attachmentsItems[0];
				item.ImageIndex = 0;
				listItems[i] = item;
				for (int j = 1; j < 5; j++)
				{
					listItems[i].SubItems.Add(attachmentsItems[j]);
				}
			}
			Invoke((MethodInvoker)delegate
			{
				attachmentView.Items.AddRange(listItems);
				attachmentView.EndUpdate();
			});
			Marshal.ReleaseComObject(attachmentNameTree);
		}

		public void FillBookmarksTree()
		{
			BookmarkNode remBookmark = bookmarksTree.SelectedNode as BookmarkNode;
			IPXC_Bookmark bookmark = null;
			if (remBookmark != null)
				bookmark = remBookmark.m_Bookmark;
			bookmarksTree.Nodes.Clear();
			//Refilling bookmarks tree
			bookmarkProgress.Visible = false;
			bookmarkProgress.Value = 0;
			IPXC_Bookmark root = m_CurDoc.BookmarkRoot;
			if ((root != null) && (root.ChildrenCount != 0))
			{
				bookmarkProgress.Visible = true;
				Thread th = new Thread(delegate()
				{
					bookmarkProgress.Invoke((MethodInvoker)delegate
					{
						bookmarkProgress.Maximum = CountAllBookmarks(m_CurDoc.BookmarkRoot);
						bookmarksTree.BeginUpdate();
					});
					AddBookmarkToTree(root);
					bookmarkProgress.Invoke((MethodInvoker)delegate
					{
						bookmarksTree.EndUpdate();
						bookmarkProgress.Visible = false;
						SelectBookmarkNodeByBookmark(bookmark, bookmarksTree.Nodes);
					});
					
				});
				th.Start();
			}
		}

		public bool SelectBookmarkNodeByBookmark(IPXC_Bookmark bookmark, TreeNodeCollection nodeCollection)
		{
			if (bookmark == null)
				return false;

			foreach (BookmarkNode bookNode in nodeCollection)
			{
				if (bookNode.m_Bookmark == bookmark)
				{
					bookmarksTree.SelectedNode = bookNode;
					bookNode.EnsureVisible();
					bookmarksTree.Focus();
					return true;
				}
				if (SelectBookmarkNodeByBookmark(bookmark, bookNode.Nodes))
					return true;
			}
			return false;
		}

		public void UpdateControlsFromDocument(int flags)
		{
			pagesCount.Text = "/0";
			if (m_CurDoc == null)
			{
				bookmarksTree.Nodes.Clear();
				GC.Collect();
				GC.WaitForPendingFinalizers();
				return;
			}
			IPXC_Pages pages = m_CurDoc.Pages;
			pagesCount.Text = "/" + pages.Count.ToString();
			int nPage = int.Parse(currentPage.Text) - 1;
			if (nPage >= pages.Count)
			{
				nPage = (int)pages.Count - 1;
				currentPage.Text = (nPage + 1).ToString();
			}
			Marshal.ReleaseComObject(pages);

			if ((flags & (int)eFormUpdateFlags.efuf_Bookmarks) > 0)
				FillBookmarksTree();
			Thread thread = new Thread(delegate() 
			{
				if ((flags & (int)eFormUpdateFlags.efuf_Attachments) > 0)
					FillAttachmentsList();
				if ((flags & (int)eFormUpdateFlags.efuf_Annotations) > 0)
					FillAnnotationsList();
				if ((flags & (int)eFormUpdateFlags.efuf_NamedDests) > 0)
					FillNamedDestinationsList();
			});
			thread.Start();
		}

		public void CloseDocument()
		{
			if (m_CurDoc != null)
			{
				m_CurDoc.Close();
				m_CurDoc = null;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}

		private void runSample_Click(object sender, EventArgs e)
		{
			InvokeMethod(null);
		}
		private void Form1_ResizeEnd(object sender, EventArgs e)
		{
			UpdatePreviewFromCurrentDocument();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (previewImage.Image != null)
				previewImage.Image.Dispose();
			CloseDocument();
			//GC.Collect();
			//GC.WaitForPendingFinalizers();
			m_pxcInst.Finalize();
			m_pxcInst = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private void currentPage_TextChanged(object sender, EventArgs e)
		{
			UpdatePreviewFromCurrentDocument();
		}

		private void prevPage_Click(object sender, EventArgs e)
		{
			if (m_CurDoc == null)
				return;
			int nPage = int.Parse(currentPage.Text) - 1;
			if (nPage > 0)
				currentPage.Text = (nPage).ToString();
		}

		private void nextPage_Click(object sender, EventArgs e)
		{
			if (m_CurDoc == null)
				return;
			int nPage = int.Parse(currentPage.Text) - 1;
			IPXC_Pages pages = m_CurDoc.Pages;
			if (nPage < (pages.Count - 1))
			{
				currentPage.Text = (nPage + 2).ToString();
			}
			Marshal.ReleaseComObject(pages);
		}
		private void sampleTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			InvokeMethod(e.Node);
		}

		private void bookmarksTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			IPXS_Inst pxsInst = m_pxcInst.GetExtension("PXS");
			uint typeGoTo = pxsInst.StrToAtom("GoTo");

			BookmarkNode curNode = e.Node as BookmarkNode;
			if (curNode.m_Bookmark == null)
				return;

			if (curNode.m_Bookmark.Actions == null)
				return;
			IPXC_Pages pages = m_CurDoc.Pages;
			IPXC_ActionsList aList = curNode.m_Bookmark.Actions;
			for (int i = (int)aList.Count - 1; i >= 0; i--)
			{
				if (aList[(uint)i].Type == typeGoTo)
				{
					IPXC_Action_Goto actGoTo = (IPXC_Action_Goto)aList[(uint)i];
					if (actGoTo.IsNamedDest)
					{
						try
						{
							uint nPageNum = m_CurDoc.GetNamedDestination(actGoTo.DestName).nPageNum;
							if (nPageNum > pages.Count - 1)
								continue;
							currentPage.Text = (nPageNum + 1).ToString();
							break;
						}
						catch(Exception)
						{
							continue;
						}
					}

					currentPage.Text = (actGoTo.get_Dest().nPageNum + 1).ToString();
					break;
				}
			}
			Marshal.ReleaseComObject(pages);
		}

		private void sampleTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			UpdateCodeSample(e.Node);
			if ((e.X < e.Node.Bounds.Left) && (e.Node.Name.IndexOf('_') >= 0))
				InvokeMethod(e.Node);
		}

		private void splitter2_SplitterMoved(object sender, SplitterEventArgs e)
		{
			UpdatePreviewFromCurrentDocument();
		}

		private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			UpdatePreviewFromCurrentDocument();
		}

		private FormWindowState m_LastWndState = FormWindowState.Normal;
		private void Form1_Resize(object sender, EventArgs e)
		{
			if (WindowState != m_LastWndState)
			{
				UpdatePreviewFromCurrentDocument();
				m_LastWndState = WindowState;
			}

		}
		private void filterEdit_TextChanged(object sender, EventArgs e)
		{
			RefillTree();
			sampleTree.ExpandAll();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			filterEdit.Text = "";
			RefillTree();
		}

		private void expandAll_Click(object sender, EventArgs e)
		{
			sampleTree.ExpandAll();
		}

		private void collapseAll_Click(object sender, EventArgs e)
		{
			sampleTree.CollapseAll();
		}
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			try
			{
				TreeNode curNode = sampleTree.SelectedNode;
				MethodInfo theMethod = GetCurrentMethod(curNode);
				if (theMethod == null)
					return;

				int fileline = GetMethodLine(theMethod.DeclaringType.Name, theMethod.Name);

				string filePath = m_sDirPath + theMethod.DeclaringType.Name + ".cs";

				EnvDTE.DTE dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
				dte.MainWindow.Activate();
				EnvDTE.Window w = dte.ItemOperations.OpenFile(filePath);
				((EnvDTE.TextSelection)dte.ActiveDocument.Selection).GotoLine(fileline, true);
			}
			catch (Exception err)
			{
				Console.Write(err.Message);
			}
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			CloseDocument();

			UpdateControlsFromDocument(0);
			UpdatePreviewFromCurrentDocument();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Document.SaveDocToFile(this);
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			if (m_CurDoc == null)
				return;

			String FileName = Path.GetTempFileName();
			FileName = FileName.Replace(".tmp", ".pdf");
			m_CurDoc.WriteToFile(FileName);
			Process.Start(FileName);
		}

		private void addBookmark_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Bookmarks.AddSiblingBookmark(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void removeBookmark_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Bookmarks.RemoveSelectedBookmark(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void moveBookmarkUp_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Bookmarks.MoveUpSelectedBookmark(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void moveBookmarkDown_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Bookmarks.MoveDownSelectedBookmark(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void expandBookmarks_Click(object sender, EventArgs e)
		{
			bookmarksTree.ExpandAll();
		}

		private void collapseBookmarks_Click(object sender, EventArgs e)
		{
			bookmarksTree.CollapseAll();
		}

		private void addDest_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(NamedDestinations.AddNewDestination(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void removeDest_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(NamedDestinations.RemoveNamedDest(this));
			UpdatePreviewFromCurrentDocument();
		}

		private void namedDestsList_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			namedDestsList.Columns[e.Column ^ 1].ImageIndex = (int)eFormNameDestinationFlags.efcsf_None;
			int Sort = namedDestsList.Columns[e.Column].ImageIndex;

			namedDestsList.Columns[e.Column].ImageIndex = (int)eFormNameDestinationFlags.efcsf_Ascending;
			if ((Sort == (int)eFormNameDestinationFlags.efcsf_None) || (Sort == (int)eFormNameDestinationFlags.efcsf_Ascending))
				namedDestsList.Columns[e.Column].ImageIndex = (int)eFormNameDestinationFlags.efcsf_Descending;

			namedDestsList.ListViewItemSorter = new ListViewColumnComparer(e.Column, Sort);
		}

		class ListViewColumnComparer : IComparer
		{
			public int m_nColumnIndex { get; set; }
			public int m_nSortIndex { get; set; }

			public ListViewColumnComparer(int columnIndex, int sortIndex)
			{
				m_nColumnIndex = columnIndex;
				m_nSortIndex = sortIndex;
			}

			public int Compare(object x, object y)
			{
				ListItemDestination X = x as ListItemDestination;
				ListItemDestination Y = y as ListItemDestination;
				string ComparerX = X.SubItems[0].Text;
				string ComparerY = Y.SubItems[0].Text;

				int nMod = 1;
				if ((m_nSortIndex == (int)eFormNameDestinationFlags.efcsf_None) || (m_nSortIndex == (int)eFormNameDestinationFlags.efcsf_Ascending))
					nMod = -1;
				if (m_nColumnIndex == 1)
				{
					if (X.m_nPageNumber == Y.m_nPageNumber)
						return nMod * NativeMethods.StrCmpLogicalW(ComparerX, ComparerY);
					return nMod * X.m_nPageNumber.CompareTo(Y.m_nPageNumber);
				}
				return nMod * NativeMethods.StrCmpLogicalW(ComparerX, ComparerY);
			}
		}
		private void namedDestsList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (e.Item.SubItems[1].Text != "")
				currentPage.Text = e.Item.SubItems[1].Text;
		}

		private void addAnnot_Click(object sender, EventArgs e)
		{
			switch (addAnnotType.SelectedIndex)
			{
				case (int)eFormAnnotationType.efat_Text:
					UpdateControlsFromDocument(Annotations.AddTextAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_Link:
					UpdateControlsFromDocument(Annotations.AddLinkAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_FreeText:
					UpdateControlsFromDocument(Annotations.AddFreeTextAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_Line:
					UpdateControlsFromDocument(Annotations.AddLineAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_SquareAndCircle:
					UpdateControlsFromDocument(Annotations.AddSquareAndCircleAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_PolygoneAndPolyline:
					UpdateControlsFromDocument(Annotations.AddPolygonAndPolylineAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_TextMarkup:
					UpdateControlsFromDocument(Annotations.AddTextMarkupAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_Popup:
					UpdateControlsFromDocument(Annotations.AddPopupAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_FileAttachment:
					UpdateControlsFromDocument(Annotations.AddFile_AttachmentAnnotation(this));
					break;
				case (int)eFormAnnotationType.efat_Redact:
					UpdateControlsFromDocument(Annotations.AddRedactAnnotation(this));
					break;
				default:
					MessageBox.Show("Please, select annotation type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
			}
			UpdatePreviewFromCurrentDocument();
		}

		private void removeAnnot_Click(object sender, EventArgs e)
		{
			if (m_CurDoc == null)
				return;

			if (annotsView.SelectedItems.Count == 0)
			{
				MessageBox.Show("Please, select annotation in list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			ListItemAnnotation currentAnnot = annotsView.SelectedItems[0] as ListItemAnnotation;
			IPXC_Pages pages = m_CurDoc.Pages;
			IPXC_Page page = pages[(uint)(currentAnnot.m_nPageNumber)];
			page.RemoveAnnots((uint)currentAnnot.m_nIndexOnPage, 1);
			UpdateControlsFromDocument((int)eFormUpdateFlags.efuf_Annotations);
			UpdatePreviewFromCurrentDocument();
			Marshal.ReleaseComObject(page);
			Marshal.ReleaseComObject(pages);
		}

		private void openAttach_Click(object sender, EventArgs e)
		{
			OpenAttachment();
		}
		private void attachmentView_DoubleClick(object sender, EventArgs e)
		{
			OpenAttachment();
		}
		public void OpenAttachment()
		{
			if (m_CurDoc == null)
				return;
			if (attachmentView.SelectedItems.Count == 0)
				return;

			IAFS_Inst afsInst = m_pxcInst.GetExtension("AFS");
			ListItemAttachment item = attachmentView.SelectedItems[0] as ListItemAttachment;

			string sType = item.SubItems[0].Text.Split('.')[1];
			m_sFilePath = Path.GetTempFileName();
			m_sFilePath = m_sFilePath.Replace(".tmp", "." + sType);
			bool bIsSuccessful = false;
			IPXC_Document coreDoc = null;
			do
			{
				//Check whether it's a PDF file
				try
				{
					IAFS_Name name = afsInst.DefaultFileSys.StringToName(m_sFilePath);
					IAFS_File file = afsInst.DefaultFileSys.OpenFile(name, (int)AFS_OpenFileFlags.AFS_OpenFile_Write | (int)AFS_OpenFileFlags.AFS_OpenFile_Read
						| (int)AFS_OpenFileFlags.AFS_OpenFile_CreateNew | (int)AFS_OpenFileFlags.AFS_OpenFile_ShareRead);

					item.m_pxcEmbeddedFileStream.SaveToFile(file);
					file.Close();

					coreDoc = m_pxcInst.OpenDocumentFromFile(m_sFilePath, null);
					bIsSuccessful = true;
					break;
				}
				catch (Exception)
				{
					
				}
				//Else we try image file
				try
				{
					IIXC_Inst ixcInst = m_pxcInst.GetExtension("IXC");
					IAUX_Inst auxInst = m_pxcInst.GetExtension("AUX");

					coreDoc = m_pxcInst.NewDocument();

					IIXC_Image img = ixcInst.CreateEmptyImage();
					img.Load(m_sFilePath);
					IPXC_Pages pages = coreDoc.Pages;
					for (uint i = 0; i < img.PagesCount; i++)
					{
						IIXC_Page ixcPage = img.GetPage(i);
						PXC_Rect rc;
						rc.left = 0;
						rc.right = ixcPage.Width;
						rc.top = ixcPage.Height;
						rc.bottom = 0;
						IPXC_UndoRedoData urd;
						IPXC_Page docPage = pages.InsertPage(i, ref rc, out urd);
						IPXC_Image pxcImg = coreDoc.AddImageFromIXCPage(ixcPage);
						IPXC_ContentCreator CC = coreDoc.CreateContentCreator();
						CC.SaveState();
						{
							PXC_Rect rcImage = new PXC_Rect();
							rcImage.right = 1;
							rcImage.top = 1;
							PXC_Matrix matrix = docPage.GetMatrix(PXC_BoxType.PBox_PageBox);
							matrix = auxInst.MathHelper.Matrix_RectToRect(ref rcImage, ref rc);
							CC.ConcatCS(ref matrix);
							CC.PlaceImage(pxcImg);
						}
						CC.RestoreState();
						docPage.PlaceContent(CC.Detach());
						Marshal.ReleaseComObject(docPage);
					}
					bIsSuccessful = true;
					Marshal.ReleaseComObject(pages);
					break;
				}
				catch (Exception)
				{

				}
				//Next we try text files
				try
				{
					do
					{
						if (sType != "txt")
							break;
						coreDoc = m_pxcInst.NewDocument();

						PXC_Rect rc;
						rc.left = 0;
						rc.right = 600;
						rc.top = 800;
						rc.bottom = 0;
						IPXC_UndoRedoData urData;
						IPXC_Pages pages = coreDoc.Pages;
						IPXC_Page page = pages.InsertPage(0, ref rc, out urData);
						IPXC_ContentCreator CC = coreDoc.CreateContentCreator();

						Converters.DrawTextCallbacks drawTextCallbacks = new Converters.DrawTextCallbacks();
						drawTextCallbacks.m_currPage = page;
						drawTextCallbacks.m_Text = File.ReadAllText(m_sFilePath);

						IPXC_Font font = coreDoc.CreateNewFont("Times New Roman", 0, 400);
						CC.SetColorRGB(0x00000000);
						CC.SetFont(font);
						CC.SetFontSize(15);

						PXC_Rect rect = new PXC_Rect();
						rect.top = rc.top - 40;
						rect.right = rc.right - 40;
						rect.bottom = rc.bottom + 40;
						rect.left = rc.left + 40;

						CC.ShowTextBlock(drawTextCallbacks.m_Text, rect, rect, (uint)PXC_DrawTextFlags.DTF_Center, -1, null, null, drawTextCallbacks, out rect);
						bIsSuccessful = true;
						Marshal.ReleaseComObject(page);
						Marshal.ReleaseComObject(pages);
					} while (false);
				}
				catch (Exception)
				{

				}
				if (bIsSuccessful)
					break;
				//If it's not a supported file type, we open with standard system viewer
				Process.Start(m_sFilePath);

			} while (false);
			if (bIsSuccessful)
			{
				CloseDocument();
				m_CurDoc = coreDoc;
				UpdateControlsFromDocument((int)eFormUpdateFlags.efuf_All);
				UpdatePreviewFromCurrentDocument();
			}
			
			m_sFilePath = "";
		}

		private void addAttach_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Attachments.AddAttachment(this));
		}

		private void removeAttach_Click(object sender, EventArgs e)
		{
			UpdateControlsFromDocument(Attachments.RemoveAttachment(this));
			UpdatePreviewFromCurrentDocument();
		}
	}
}
