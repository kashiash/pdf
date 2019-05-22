// This MFC Samples source code demonstrates using MFC Microsoft Office Fluent User Interface 
// (the "Fluent UI") and is provided only as referential material to supplement the 
// Microsoft Foundation Classes Reference and related electronic documentation 
// included with the MFC C++ library software.  
// License terms to copy, use or distribute the Fluent UI are available separately.  
// To learn more about our Fluent UI licensing program, please visit 
// http://go.microsoft.com/fwlink/?LinkId=238214.
//
// Copyright (C) Microsoft Corporation
// All rights reserved.

#include "stdafx.h"

#include "OutputWnd.h"
#include "Resource.h"
#include "MainFrm.h"
#include "RTFLoger.h"

#include <vector>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
FontName g_fn(L"Consolas");
FontSize g_fs(10);

enum STATE
{
	st_normal,
	st_open,			// just read '<'
	st_close,			// just read '>'
	st_tag,				// just read '/'
	st_macroname,		// reading macro name
	st_maxstate
};

enum CHARTYPE
{
	ch_other,			// any char
	ch_open,			// '<'
	ch_close,			// '>'
	ch_tag,				// '/'
	ch_max_char_type
};

#define _ML(state, class)	(((state) << 4) | (class))

static CHARTYPE _getchartype(WCHAR ch)
{
	switch (ch)
	{
	case L'/':
		return ch_tag;
	case L'<':
		return ch_open;
	case L'>':
		return ch_close;
	default:
//		if (((ch >= L'A') && (ch <= 'Z'))	||
//			((ch >= L'z') && (ch <= 'z'))	||
//			((ch >= L'0') && (ch <= '9')))
//			return ch_
		return ch_other;
	}
}

static const BYTE lookuptable[ch_max_char_type][st_maxstate] =
{
	// state:			st_normal	st_open			st_close		st_tag,			st_macroname
	/* ch_other		*/ {st_normal,	st_macroname,	st_normal,		st_macroname,	st_macroname	},
	/* ch_open		*/ {st_open,	st_macroname,	st_open,		st_macroname,	st_macroname	},
	/* ch_close		*/ {st_normal,	st_close,		st_normal,		st_close,		st_close		},
	/* ch_tag		*/ {st_normal,	st_tag,			st_normal,		st_macroname,	st_macroname	},
};

static STATE _get_next_state(CHARTYPE ch, STATE st)
{
	return (STATE)(lookuptable[ch][st]);
}

enum TagType
{
	tag_all,
	tag_color,
	tag_bold,
	tag_italic,
	tag_unknown,
};

struct StackItem
{
	StackItem() : prev_color(BlackColor), prev_font(g_fn), prev_fontsize(g_fs)
	{
		prev_bold = FALSE;
		prev_italic = FALSE;
		type = tag_all;
	}
	TagType		type;
	Color		prev_color;
	FontName	prev_font;
	FontSize	prev_fontsize;
	BOOL		prev_bold;
	BOOL		prev_italic;
};

BOOL _ASSERTDISPLAYFUNCTION(LPCWSTR sz)
{
	if (!sz || (sz[0] == '\0'))
		return FALSE;
	g_Log << g_fs << g_fn;
	g_Log << BlackColor;

	StackItem cstate;
	cstate.prev_color = BlackColor;
	cstate.prev_bold = FALSE;
	cstate.prev_italic = FALSE;
	cstate.prev_font = g_fn;
	cstate.prev_fontsize = g_fs;

	CStringW t;
	CStringW tag;
	BOOL bCloseTag;
	WCHAR ch;
	LPCWSTR p = sz;

	std::vector<StackItem>	tag_stack;

	enum STATE		state = st_normal;		// current state
	enum CHARTYPE	chclass;				// class of current character

	while ((ch = *p++) != '\0')
	{
		chclass = _getchartype(ch);
		state = _get_next_state(chclass, state);
		switch (state)
		{
		case st_normal:
			t.AppendChar(ch);
			break;
		case st_open:
			tag.Empty();
			bCloseTag = FALSE;
			break;
		case st_tag:
			bCloseTag = TRUE;
			break;
		case st_close:
			if (t.GetLength() > 0)
			{
				g_Log << t;
				t.Empty();
			}
			{
				if (tag.GetLength() == 0)
				{
					if (!bCloseTag)
						break;
				}
				TagType tp = tag_unknown;
				StackItem si = cstate;
				if (!bCloseTag)
				{
					if (tag.Compare(L"b") == 0)
					{
						si.type = tag_bold;
						cstate.prev_bold = TRUE;
						DWORD dwEff = CFM_BOLD;
						if (si.prev_italic)
							dwEff |= CFM_ITALIC;
						g_Log << FontEffects(CFM_BOLD);
					}
					else if (tag.Compare(L"i") == 0)
					{
						si.type = tag_italic;
						cstate.prev_italic = TRUE;
						DWORD dwEff = CFM_ITALIC;
						if (si.prev_bold)
							dwEff |= CFM_BOLD;
						g_Log << FontEffects(dwEff);
					}
					else if (tag.Compare(L"r") == 0)
					{
						si.type = tag_color;
						cstate.prev_color = RedColor;
						g_Log << RedColor;
					}
					else
					{
						si.type = tag_unknown;
					}
					tag_stack.push_back(si);
				}
				else
				{
					if (tag_stack.size() == 0)
						break;
					cstate = si = tag_stack[tag_stack.size() - 1];
					tag_stack.pop_back();
					switch (si.type)
					{
					case tag_bold:
					case tag_italic:
						{
							DWORD eff = 0;
							if (si.prev_bold)
								eff |= CFM_BOLD;
							if (si.prev_italic)
								eff |= CFM_ITALIC;
							g_Log << FontEffects(eff);
						}
						break;
					case tag_color:
						g_Log << si.prev_color;
						break;
					}
				}
			}
			break;
		case st_macroname:
			if (t.GetLength() > 0)
			{
				g_Log << t;
				t.Empty();
			}
			tag.AppendChar(ch);
		}
	}
	if (t.GetLength() > 0)
	{
		g_Log << t;
		t.Empty();
	}
	return FALSE;
}

/////////////////////////////////////////////////////////////////////////////
// COutputBar

COutputWnd::COutputWnd()
{
}

COutputWnd::~COutputWnd()
{
}

BEGIN_MESSAGE_MAP(COutputWnd, CDockablePane)
	ON_WM_CREATE()
	ON_WM_SIZE()
END_MESSAGE_MAP()

int COutputWnd::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDockablePane::OnCreate(lpCreateStruct) == -1)
		return -1;

	CRect rectDummy;
	rectDummy.SetRectEmpty();

	// Create tabs window:
	if (!m_wndTabs.Create(CMFCTabCtrl::STYLE_FLAT, rectDummy, this, 1))
	{
		TRACE0("Failed to create output tab window\n");
		return -1;      // fail to create
	}

	// Create output panes:
	const DWORD dwStyle = WS_CHILD | WS_VISIBLE | WS_HSCROLL | WS_VSCROLL | WS_CLIPSIBLINGS | WS_CLIPCHILDREN |
		ES_AUTOHSCROLL | ES_AUTOVSCROLL | ES_MULTILINE | ES_NOHIDESEL | ES_SAVESEL | ES_READONLY;

	if (!m_wndOutputInfo.Create(dwStyle, rectDummy, &m_wndTabs, 2))
	{
		TRACE0("Failed to create output window\n");
		return -1;      // fail to create
	}
	m_wndOutputInfo.SetFont(&afxGlobalData.fontRegular);
	g_Log.SetHWND(m_wndOutputInfo.m_hWnd);

	UpdateFonts();

	CString strTabName;
	BOOL bNameValid;

	// Attach list windows to tab:
	bNameValid = strTabName.LoadString(IDS_INFO_TAB);
	ASSERT(bNameValid);
	m_wndTabs.AddTab(&m_wndOutputInfo, strTabName, (UINT)0);

	// Fill output tabs with some dummy text (nothing magic here)
	FillInfoWindow();

	return 0;
}

void COutputWnd::OnSize(UINT nType, int cx, int cy)
{
	CDockablePane::OnSize(nType, cx, cy);

	// Tab control should cover the whole client area:
	m_wndTabs.SetWindowPos (NULL, -1, -1, cx, cy, SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOZORDER);
}

void COutputWnd::FillInfoWindow()
{
	LOG(S_OK, L"<b><r>PDF-XChange</r> SDK Samples</b> v.0.1\n"
L"(c) 2015, Tracker Software Products (Canada) Ltd.\n");
}

void COutputWnd::UpdateFonts()
{
	// do nothing
}

/////////////////////////////////////////////////////////////////////////////
// COutputList

COutputList::COutputList()
{
}

COutputList::~COutputList()
{
}

BEGIN_MESSAGE_MAP(COutputList, CRichEditCtrl)
	ON_WM_CONTEXTMENU()
	ON_COMMAND(ID_EDIT_COPY, OnEditCopy)
	ON_COMMAND(ID_EDIT_CLEAR, OnEditClear)
	ON_COMMAND(ID_VIEW_OUTPUTWND, OnViewOutput)
	ON_MESSAGE(WM_SETFONT, OnSetFont)
	ON_WM_WINDOWPOSCHANGING()
END_MESSAGE_MAP()
/////////////////////////////////////////////////////////////////////////////
// COutputList message handlers

void COutputList::OnContextMenu(CWnd* /*pWnd*/, CPoint point)
{
	CMenu menu;
	menu.LoadMenu(IDR_OUTPUT_POPUP);

	CMenu* pSumMenu = menu.GetSubMenu(0);

	if (AfxGetMainWnd()->IsKindOf(RUNTIME_CLASS(CFrameWndEx)))
	{
		CMFCPopupMenu* pPopupMenu = new CMFCPopupMenu;

		if (!pPopupMenu->Create(this, point.x, point.y, (HMENU)pSumMenu->m_hMenu, FALSE, TRUE))
			return;

		((CFrameWndEx*)AfxGetMainWnd())->OnShowPopupMenu(pPopupMenu);
		UpdateDialogControls(this, FALSE);
	}

	SetFocus();
}

void COutputList::OnEditCopy()
{
	MessageBox(_T("Copy output"));
}

void COutputList::OnEditClear()
{
	MessageBox(_T("Clear output"));
}

void COutputList::OnViewOutput()
{
	g_Log << nl;
	_ASSERTDISPLAYFUNCTION(L"[b][r]PDF-XChange[/r] SDK Samples[/b] v.0.1");

	//CDockablePane* pParentBar = DYNAMIC_DOWNCAST(CDockablePane, GetOwner());
	//CMDIFrameWndEx* pMainFrame = DYNAMIC_DOWNCAST(CMDIFrameWndEx, GetTopLevelFrame());

	//if (pMainFrame != NULL && pParentBar != NULL)
	//{
	//	pMainFrame->SetFocus();
	//	pMainFrame->ShowPane(pParentBar, FALSE, FALSE, FALSE);
	//	pMainFrame->RecalcLayout();

	//}
}

LRESULT COutputList::OnSetFont(WPARAM wParam, LPARAM lParam)
{
	return 0;
}

